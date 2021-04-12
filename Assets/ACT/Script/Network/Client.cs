using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using LitJson;
using System.Security.Cryptography;
using System;
using WebSocket4Net;
using System.IO;

public class Client
{
    public enum ESystemRequest
    {
        EnterCity = 10,
        LeaveCity = 11,
        MoveInCity = 12,
        UserSendMsg = 20,
        SystemBroadcast = 21,
        UserGetNewMail = 22,

        UserPvpBegin = 30,
        UserPvpGameStart = 31,
        UserPvpGameEnd = 32,

        Max = 1024,
    }

    private int mServerPort = 2066;
    private string mServerUrl = "http://192.168.0.100:2066";
    private WebSocket mWebSocket = null;
    private UInt16 mCounter = (int)ESystemRequest.Max;
    private Dictionary<int, RequestCmd> mCommandMap = new Dictionary<int, RequestCmd>();
    private Dictionary<int, List<OnResponse>> mResponseHandlers = new Dictionary<int, List<OnResponse>>();
    private Queue<byte[]> mDataQueue = new Queue<byte[]>();
    private List<byte[]> mCommandList = new List<byte[]>();
    private object mQueueLock = new object();
    private CommandBuilder mCommandBuilder = new CommandBuilder();

    public List<KeyValuePair<DateTime, byte[]>> RecordCommands;
    public WebSocketState state { get { return mWebSocket != null ? mWebSocket.State : WebSocketState.None; } }
    public delegate void OnResponse(string err, Response response);
	
    public void RegisterHandler(int responseId, OnResponse handler)
    {
		if (!mResponseHandlers.ContainsKey(responseId))
            mResponseHandlers[responseId] = new List<OnResponse>();

        List<OnResponse> handlers = mResponseHandlers[responseId];
        if (!handlers.Contains(handler))
            handlers.Add(handler);
    }

    public void UnRegisterHandler(int responseId, OnResponse handler)
    {
        List<OnResponse> handlers;
		if (!mResponseHandlers.TryGetValue(responseId, out handlers))
			return;
		
        handlers.Remove(handler);
    }

    public void StartServer(string ip, int port)
    {
		mServerPort = port;
        mServerUrl = String.Format("ws://{0}:{1}", ip, mServerPort);

        mWebSocket = new WebSocket(mServerUrl);
        mWebSocket.Opened += new EventHandler(mWebSocket_Opened);
        mWebSocket.Error += new EventHandler<SuperSocket.ClientEngine.ErrorEventArgs>(mWebSocket_Error);
        mWebSocket.Closed += new EventHandler(mWebSocket_Closed);
        mWebSocket.MessageReceived += new EventHandler<MessageReceivedEventArgs>(mWebSocket_MessageReceived);
        mWebSocket.DataReceived += new EventHandler<DataReceivedEventArgs>(mWebSocket_DataReceived);
        mWebSocket.Open();
    }
    void mWebSocket_Opened(object sender, EventArgs e)
    {
        Debug.Log("mWebSocket_Opened!!!");
    }

    void mWebSocket_Error(object sender, SuperSocket.ClientEngine.ErrorEventArgs e)
    {
        Debug.Log("mWebSocket_Error");
    }

    void mWebSocket_Closed(object sender, EventArgs e)
    {
        Debug.Log("mWebSocket_Closed!");
    }

    void mWebSocket_DataReceived(object sender, DataReceivedEventArgs e)
    {
        lock (mQueueLock)
        {
            Debug.Log("mWebSocket_DataReceived: " + e.Data.Length);
            mDataQueue.Enqueue(e.Data);
        }
    }

    void mWebSocket_MessageReceived(object sender, MessageReceivedEventArgs e)
    {
        Debug.Log("MessageReceived: " + e.Message);
    }

    private byte[] SendData(byte[] data)  //转换格式
    {
        short len = (short)(data[0] | (data[1] << 8));
        short cmd = (short)(data[2] | (data[3] << 8));

        int reflen = len + 4;

        byte d1 = 18;
        byte d2 = (byte)(len & 0x7F);
        byte d3 = (byte)((len >> 7) & 0x7F);
        if (d3 > 0)
        {
            d2 |= 0x80;
            reflen += 1;
        }
        byte d4 = 24;
        byte d5 = (byte)(cmd & 0x7F);
        byte d6 = (byte)((cmd >> 7) & 0x7F);
        if (d6 > 0)
        {
            d5 |= 0x80;
            reflen += 1;
        }

        int readPos = 4;
        int lenTo = 2;

        byte[] abyte0 = new byte[reflen];
        abyte0[0] = d1;
        abyte0[1] = d2;
        if (d3 > 0)
        {
            lenTo = 3;
            abyte0[2] = d3;
        }
        for (int j = 0; j < len; j++)
            abyte0[j + lenTo] = data[readPos++];


        abyte0[lenTo + len] = d4;
        abyte0[lenTo + len + 1] = d5;
        if (d6 > 0) abyte0[lenTo + len + 2] = d6;

        return abyte0;
    }

    public void UpdateMessageQueue()
    {
        lock (mQueueLock)
        {
            while (mDataQueue.Count > 0)
                mCommandList.Add(mDataQueue.Dequeue());
        }

        if (mCommandList.Count > 0)
        {
            for (int i = 0; i < mCommandList.Count; i++)
            {
                byte[] data = mCommandList[i];
                
                Response response = new Response();
                response.id = (int)(data[4] | (data[5] << 8));
                int len = (int)(data[0] | (data[1] << 8));
                response.data = Encoding.UTF8.GetString(data, 8, (int)len);
                

                //byte[] data1 = SendData(data);

                //MemoryStream stream = new MemoryStream(data);
                //Response response = ProtoBuf.Serializer.Deserialize<Response>(stream);  //反序列化
                if (response == null)
                    continue;
                Debug.Log(response.data.ToString());

                //if (response.id < (int)ESystemRequest.Max)
                //{
                //    // trying the handler map.
                //    List<OnResponse> handlers;
                //    if (mResponseHandlers.TryGetValue(response.id, out handlers))
                //    {
                //        foreach (OnResponse handler in handlers)
                //            handler(response.error, response);
                //    }
                //}
                //else
                {
                    RequestCmd request;
                    if (mCommandMap.TryGetValue(response.id, out request))
                    {
                        mCommandMap.Remove(response.id);
                        if (request.CallBack != null)
                            request.CallBack(response.error, response);
                    }
                }

                // if the server send [UserNotLogin] error code
                // the client close then relogin now.
                if (!string.IsNullOrEmpty(response.error) &&
                    response.error == "UserNotLogin")
                {
                    GodClose();
                    break;
                }
            }
            mCommandList.Clear();
        }
    }

    public void Clear()
    {
        mCommandMap.Clear();
    }

    public void Close()
    {
        mCommandMap.Clear();
        if (mWebSocket != null)
            mWebSocket.Close();
    }
	
	public void GodClose()
    {
        mCommandMap.Clear();
        if (mWebSocket != null)
            mWebSocket.GodClose();
    }

    public IEnumerator Execute(object obj, OnResponse callback)
    {
        float timeOut = 10.0f;

        // check the web socket state.
        if (mWebSocket.State != WebSocketState.Open)
        {
            // if it still not opend, fire error!!!.
            if (callback != null)
                callback("Net error, socket state: " + mWebSocket.State, null);
            yield break;
        }

        UInt16 requestID = mCounter++;
        if (mCounter == UInt16.MaxValue)
            mCounter = (int)ESystemRequest.Max;

        UInt16 msgId = (UInt16)(ERequestTypes)Enum.Parse(typeof(ERequestTypes), "E" + obj.GetType().Name);
        requestID = msgId;

        RequestCmd command = new RequestCmd();
        command.Id = requestID;
        command.CallBack = callback;

        //byte[] data = mCommandBuilder.Build(command);
        //short len = (short)(data.Length - 4);
        //data[0] = (byte)(len & 0xff);
        //data[1] = (byte)(len >> 8);

        string postData = JsonMapper.ToJson(obj);
        byte[] json = Encoding.UTF8.GetBytes(postData);
        int len = json.Length;
        byte[] data = new byte[len + 8];
        data[0] = (byte)(len & 0xff);
        data[1] = (byte)(len >> 8);
        data[2] = (byte)(200 & 0xff);
        data[3] = (byte)(200 >> 8);
        data[4] = (byte)(msgId & 0xff);
        data[5] = (byte)(msgId >> 8);
        data[6] = (byte)(0);
        data[7] = (byte)(0);
        for (int i = 0; i < len; ++i)
            data[8 + i] = json[i];

        // send the request to server.
        mWebSocket.Send(data);

        Debug.Log(data.ToString() + "   " + postData);

        if (MainScript.Instance.RecordForBot)
        {
            if (RecordCommands == null)
                RecordCommands = new List<KeyValuePair<DateTime, byte[]>>();
            RecordCommands.Add(new KeyValuePair<DateTime, byte[]>(DateTime.Now, data));
        }

        // we do not need the callback.
        if (callback == null)
            yield break;

        // push to the map and send the request.
        mCommandMap[requestID] = command;

        // wait for timeout.
        yield return new WaitForSeconds(timeOut);

        // if it still exist in map, do remove it now.
        if (mCommandMap.ContainsKey(requestID))
        {
            mCommandMap.Remove(requestID);
            if (callback != null)
            {
                callback("Time out", null);

                // close this client manually.
                GodClose();
            }
        }

        yield break;
    }


    ////////
    public IEnumerator SendPVP(byte[] bytes)
    {
        float timeOut = 10.0f;

        // check the web socket state.
        if (mWebSocket.State != WebSocketState.Open)
        {
            yield break;
        }

        // send the request to server.
        mWebSocket.Send(bytes);

        // wait for timeout.
        yield return new WaitForSeconds(timeOut);

        yield break;
    }
}
