using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WebSocket4Net;
using UnityEngine;

//public abstract class SimpleClient
//{
//    // private members.
//    UdpSession mUdpSession;
//    Queue<byte[]> mMessageQueue;
//    object mQueueLock;

//    public UdpSession Session { get { return mUdpSession; } }
//    public abstract bool ProcessPacket(byte[] data);
//    public virtual void OnOpend() { }
//    public virtual void OnClosed() { }

//    public void Start(string server)
//    {
//        mMessageQueue = new Queue<byte[]>();
//        mQueueLock = new object();

//        Uri serverUri = new Uri(server);

//        Debug.Log("ConnectServer:" + serverUri.Host + ":" + serverUri.Port);
//        mUdpSession = new UdpSession(serverUri.Host, serverUri.Port);
//        mUdpSession.Opened += new EventHandler(mWebSocket_Opened);
//        mUdpSession.Error += new EventHandler(mWebSocket_Error);
//        mUdpSession.Closed += new EventHandler(mWebSocket_Closed);
//        mUdpSession.DataReceived += new EventHandler<UdpSession.DataReceivedEventArgs>(mWebSocket_DataReceived);
//        mUdpSession.Open();
//    }

//    public void Destroy()
//    {
//        // close socket if opened.
//        //if (mUdpSession.State == WebSocketState.Open)
//            mUdpSession.Close();
//    }

//    void mWebSocket_Closed(object sender, EventArgs e)
//    {
//        OnClosed();
//        Debug.Log("mWebSocket_Closed");
//    }

//    void mWebSocket_Error(object sender, EventArgs e)
//    {
//        Debug.Log("mWebSocket_Error: ");
//    }

//    void mWebSocket_Opened(object sender, EventArgs e)
//    {
//        Debug.Log("mWebSocket_Opened");
//        OnOpend();
//    }

//    void mWebSocket_DataReceived(object sender, UdpSession.DataReceivedEventArgs e)
//    {
//        lock (mQueueLock)
//        {
//            mMessageQueue.Enqueue(e.Data);
//        }
//    }

//    void mWebSocket_MessageReceived(object sender, MessageReceivedEventArgs e)
//    {
//        Debug.Log("mWebSocket_MessageReceived");
//    }

//    public void SendCommand(BinaryCommand command)
//    {
//        SendCommand(command, false);
//    }

//    public void SendCommand(BinaryCommand command, bool local)
//    {
//        CommandSerializer ser = new CommandSerializer();
//        command.Serialize(ser);
//        byte[] bytes = ser.FetchBytes();
//        mUdpSession.Send(bytes, command.NeedAck, command.NeedOrder);

//        // process packets at local.
//        if (local)
//            ProcessPacket(bytes);
//    }

//    public void UpdateMessageQueue()
//    {
//        mUdpSession.Update();

//        lock (mQueueLock)
//        {
//            while (mMessageQueue.Count > 0)
//            {
//                byte[] message = mMessageQueue.Dequeue();
//                if (message.Length == 0)
//                    continue;

//                ProcessPacket(message);
//            }
//        }
//    }
//}   

public abstract class SimpleClient
{
    // private members.
    UdpSession mUdpSession;
    Queue<byte[]> mMessageQueue;
    object mQueueLock;

    public UdpSession Session { get { return mUdpSession; } }
    public abstract bool ProcessPacket(byte[] data);
    public virtual void OnOpend() { }
    public virtual void OnClosed() { }

    public void Start(string server)
    {
        mMessageQueue = new Queue<byte[]>();
        mQueueLock = new object();

        Uri serverUri = new Uri(server);

        Debug.Log("ConnectServer:" + serverUri.Host + ":" + serverUri.Port);
        mUdpSession = new UdpSession();
    }

    public void Destroy()
    {
    }

    public void SendCommand(BinaryCommand command)
    {
        SendCommand(command, false);
    }

    public void SendCommand(BinaryCommand command, bool local)
    {
        CommandSerializer ser = new CommandSerializer();
        command.Serialize(ser);
        byte[] bytes = ser.FetchBytes();
        mUdpSession.Send(bytes, command.NeedAck, command.NeedOrder);

        MainScript.Instance.PVPRequest(bytes);

        // process packets at local.
        if (local)
            ProcessPacket(bytes);
    }

    public void UpdateMessageQueue()
    {
        mUdpSession.Update();

        lock (mQueueLock)
        {
            while (mMessageQueue.Count > 0)
            {
                byte[] message = mMessageQueue.Dequeue();
                if (message.Length == 0)
                    continue;

                ProcessPacket(message);
            }
        }
    }
}   
