using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;


public class UdpSession
{
    const byte UDP_ACK_FLAG = 0x80;
    const byte UDP_ORDER_FLAG = 0x40;
    const byte UDP_HEADER_SIZE = 0x03;
    const byte UDP_HEADER_MASK = 0x3f;
    const byte UDP_INIT_CODE = (UDP_ACK_FLAG | UDP_ORDER_FLAG | UDP_HEADER_SIZE);
    const byte UDP_ACK_CODE = (UDP_HEADER_SIZE);
    const float UDP_RESEND_TIME = 0.2f;
    const float UDP_KEEP_ALIVE_TIME = 10.0f;

    public delegate void OnEventHandler(int code);

    public class UdpPackage
    {
        public byte[] Data;
        public int Count;
        public OnEventHandler Callback;

        public byte Code { get { return Data[0]; } }
        public bool Valid { get { return (Data[0] & UDP_HEADER_MASK) == (Data.Length & UDP_HEADER_MASK); } }
        public UInt16 Sequence { get { return ParseSequence(Data); } }
        public bool Ack { get { return (Data[0] & UDP_ACK_FLAG) != 0; } }
        public bool Order { get { return (Data[0] & UDP_ORDER_FLAG) != 0; } }

        public UdpPackage(byte[] data, int count, OnEventHandler callback)
        {
            Data = data;
            Count = count;
            Callback = callback;
        }
    }

    private UdpClient mUdpClient;
    private IPEndPoint mIPEndPoint;
    private UInt16 mSequence = 10;
    private volatile UInt16 mWantSequence = 0;
    private Queue<UdpPackage> mRequestQueue = new Queue<UdpPackage>();
    private UdpPackage mPendingPackage = null;
    private float mLastSendTime = 0.0f;
    private bool mConnected = false;

    /// <summary>
    /// init and start the udp client.
    /// </summary>
    /// <param name="host"></param>
    /// <param name="port"></param>
    public UdpSession(string host, int port)
    {
        mUdpClient = new UdpClient(host, port);
    }

    public void Open()
    {
        // send the init code to the server.
        Send(UDP_INIT_CODE, null, 20, delegate(int code)
        {
            if (code == 0)
            {
                mConnected = true;
                if (mOpened != null)
                    mOpened(this, null);
            }
            else
            {
                mConnected = false;
                if (mError != null)
                    mError(this, null);
            }
        });

        mUdpClient.BeginReceive(ReceiveCallback, this);
    }

    /// <summary>
    /// the main data callback.
    /// </summary>
    /// <param name="ar"></param>
    void ReceiveCallback(IAsyncResult ar)
    {
        Byte[] data = (mIPEndPoint == null) ?
            mUdpClient.Receive(ref mIPEndPoint) :
            mUdpClient.EndReceive(ar, ref mIPEndPoint);

        OnData(data);

        // try to receive again.
        mUdpClient.BeginReceive(ReceiveCallback, this);
    }

    #region "Messages"
    public class DataReceivedEventArgs : EventArgs
    {
        public DataReceivedEventArgs(byte[] data) { Data = data; }
        public byte[] Data { get; private set; }
    }

    // message received.
    private EventHandler<DataReceivedEventArgs> mDataReceived;
    public event EventHandler<DataReceivedEventArgs> DataReceived
    {
        add { mDataReceived += value; }
        remove { mDataReceived -= value; }
    }

    private EventHandler mOpened;
    public event EventHandler Opened
    {
        add { mOpened += value; }
        remove { mOpened -= value; }
    }

    private EventHandler mError;
    public event EventHandler Error
    {
        add { mError += value; }
        remove { mError -= value; }
    }

    private EventHandler mClosed;
    public event EventHandler Closed
    {
        add { mClosed += value; }
        remove { mClosed -= value; }
    }
    #endregion

    public static UInt16 ParseSequence(byte[] data)
    {
        return (ushort)(data[1] | (data[2] << 8));
    }

    /// <summary>
    /// client get a udp pakcage data from server.
    /// </summary>
    /// <param name="data"></param>
    /// <param name="ack"></param>
    void OnData(byte[] data)
    {
        int dataLen = data.Length;
        if (dataLen < UDP_HEADER_SIZE)
            return;

        UdpPackage package = new UdpPackage(data, 0, null);
        if (!package.Valid)
            return;

        // directly reply ack request, donot use the Send method as we in another thread.
        if (package.Ack)
        {
            byte[] ackData = new byte[] { UDP_ACK_CODE, data[1], data[2] };
            mUdpClient.Send(ackData, ackData.Length);
        }

        if (package.Code == UDP_ACK_CODE && data.Length == UDP_HEADER_SIZE)
            OnDataAck(package.Sequence);
        else if (data.Length > UDP_HEADER_SIZE && mDataReceived != null)
        {
            byte[] segmentData = new byte[data.Length - UDP_HEADER_SIZE];
            Array.Copy(data, UDP_HEADER_SIZE, segmentData, 0, segmentData.Length);
            mDataReceived(this, new DataReceivedEventArgs(segmentData));
        }
    }

    void OnDataAck(UInt16 sequence)
    {
        if (sequence == mWantSequence)
            mWantSequence = 0;
    }

    public void Update()
    {
        TrySend();
    }

    void TrySend()
    {
        //mUdpClient.Send(request.Data, request.Data.Length);
        if (mWantSequence > 0)
        {
            // check the pending data. [mPendingPackage]
            // TODO: we need check do we reach a timeout.
            if (Time.realtimeSinceStartup > mLastSendTime + UDP_RESEND_TIME)
            {
                Send(mPendingPackage);

                mPendingPackage.Count--;
                if (mPendingPackage.Count < 0)
                {
                    OnEventHandler Callback = mPendingPackage.Callback;
                    mPendingPackage = null;
                    if (Callback != null)
                        Callback(-1);

                    // resend time out, try Send again now.
                    mWantSequence = 0;
                    TrySend();
                }
            }
        }
        else
        {
            // check the sucess!!!
            if (mPendingPackage != null)
            {
                OnEventHandler Callback = mPendingPackage.Callback;
                mPendingPackage = null;
                if (Callback != null)
                    Callback(0);
            }

            // sending the requests in the queue.
            while (mRequestQueue.Count > 0)
            {
                // send the package in the queue.
                UdpPackage package = mRequestQueue.Dequeue();
                Send(package);

                if (package.Ack)
                {
                    // setup the sequence flag.
                    mWantSequence = package.Sequence;
                    mPendingPackage = package;
                    break;
                }
            }

            // send the keep alive package.
            if (Time.realtimeSinceStartup > mLastSendTime + UDP_KEEP_ALIVE_TIME)
                Send(new UdpPackage(new byte[] { UDP_ACK_CODE, 0, 0 }, 0, null));
        }
    }

    void Send(UdpPackage package)
    {
        mUdpClient.Send(package.Data, package.Data.Length);
        mLastSendTime = Time.realtimeSinceStartup;
    }

    /// <summary>
    /// send internal.
    /// </summary>
    /// <param name="code"></param>
    /// <param name="sequence"></param>
    /// <param name="buff"></param>
    /// <param name="length"></param>
    void Send(byte code, UInt16 sequence, byte[] buff, int ack, OnEventHandler callback)
    {
        int length = (buff != null) ? buff.Length : 0;
        byte[] package = new byte[length + UDP_HEADER_SIZE];

        package[0] = code;
        package[1] = (byte)(mSequence & 0xff);
        package[2] = (byte)(mSequence >> 8);

        if (length > 0)
            Array.Copy(buff, 0, package, UDP_HEADER_SIZE, length);

        mRequestQueue.Enqueue(new UdpPackage(package, ack, callback));
        TrySend();
    }

    /// <summary>
    /// send buffer with code.
    /// and sequence will add up.
    /// </summary>
    /// <param name="code"></param>
    /// <param name="buff"></param>
    /// <param name="length"></param>
    void Send(byte code, byte[] buff, int ack, OnEventHandler callback)
    {
        Send(code, mSequence++, buff, ack, callback);
    }

    /// <summary>
    /// send a buffer to the server
    /// option is need to have a ack flag.
    /// </summary>
    /// <param name="buff"></param>
    /// <param name="ack">this package need an ack</param>
    /// <param name="order">this package need to be order.</param>
    public void Send(byte[] buff, int ack, bool order)
    {
        byte code = (byte)((byte)(buff.Length + UDP_HEADER_SIZE) & UDP_HEADER_MASK);
        if (ack > 0) code |= UDP_ACK_FLAG;
        if (order) code |= UDP_ORDER_FLAG;
        Send(code, buff, ack, null);
    }

    /// <summary>
    /// send a string to the server.
    /// string will be encoding to utf8 bytes.
    /// </summary>
    /// <param name="msg"></param>
    public void Send(string msg)
    {
        Send(Encoding.UTF8.GetBytes(msg), 10, true);
    }

    public void Close()
    {
        byte[] closeData = new byte[] { UDP_ACK_CODE, 1, 0 };
        mUdpClient.Send(closeData, closeData.Length);
        mUdpClient.Close();
    }



    //////////////
    private int mState = 0;
    public UdpSession()
    {
        mState = 1;
    }
}