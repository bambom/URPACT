using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Chat : MonoBehaviour
{
    // Use this for initialization	
    // set the maxumum queue length for 10
    static Chat mInstance = null;
    public static Chat Instance { get { return mInstance; } }
	
    const int mMaxLength = 50;
	public int MaxLength { get {return mMaxLength;} }
    long mLastMessageTime = 0;
	
	//Store the message
    List<ChatMessage> mWorldMessages = new List<ChatMessage>();
    public List<ChatMessage> WorldMessages { get { return mWorldMessages; } }
	Dictionary<string,List<ChatMessage>> mPrivateMsgDic = new Dictionary<string, List<ChatMessage>>();
	public Dictionary<string, List<ChatMessage>> PrivateMsgDic { get{return mPrivateMsgDic;} }	
	List<string> mSendToMeName = new List<string>();
	public List<string> SendToMeName { get {return mSendToMeName;}}
	
	void Start()
    {
        mInstance = this;

        GameObject.DontDestroyOnLoad(this);

        MainScript.Instance.Client.RegisterHandler((int)Client.ESystemRequest.UserSendMsg, OnUserSendMsg);

        RefreshChatMessages();
    }

    void OnDestroy()
    {
        MainScript.Instance.Client.UnRegisterHandler((int)Client.ESystemRequest.UserSendMsg, OnUserSendMsg);
    }

    void OnUserSendMsg(string err, Response response)
    {
        ChatMessage chatMessage = (response != null) ? response.Parse<ChatMessage>() : null;
        if (chatMessage == null)
            return;

        if (string.IsNullOrEmpty(chatMessage.msg))
            return;
		
							
		UpdateMsgList(chatMessage);
		if (ChatWnd.Exist)
        {				
            ChatWnd.Instance.AddChatMessage(chatMessage,null,0);
            System.DateTime time = Global.JSLongToDataTime(chatMessage.time);
            Debug.Log("Time:" + time);
        }
		else 
		{
			if(chatMessage.channel == (int)EChatChannel.Private){
				mSendToMeName.Add(chatMessage.user);
				if(Chat.Instance.SendToMeName.Count > Chat.Instance.MaxLength)
					Chat.Instance.SendToMeName.RemoveAt(0);
			}
		}
        if (InGameMainWnd.Exist)
        {
			string path;
			if(mSendToMeName.Count != 0)
			{
				path = "ChatMessage/ChatIcon";
				GameObject chatRemindObj = InGameMainWnd.Instance.WndObject.transform.Find(path).gameObject;
				chatRemindObj.GetComponent<TweenScale>().enabled = true;
				chatRemindObj.SetActive(true);	
				SoundManager.Instance.PlaySound("UI_XiTongTiShi");
			}
			if(chatMessage.channel == (int)EChatChannel.World)
			{
				path = "ChatMessage/ChatLabels/LabelOne";
	            UILabel labelOne = InGameMainWnd.Instance.WndObject.transform.Find(path).GetComponent<UILabel>();
				path = "ChatMessage/ChatLabels/LabelTwo";
	            UILabel labelTwo = InGameMainWnd.Instance.WndObject.transform.Find(path).GetComponent<UILabel>();
	
	            labelOne.text = labelTwo.text;
	            labelTwo.text = "[00FF00]" + chatMessage.user + ":[-]" + chatMessage.msg; ;
			}
        }  
    }

    public void DestroyChat()
    {
		mWorldMessages.Clear();
        mPrivateMsgDic.Clear();
        Destroy(this);
    }
	
	public void UpdateMsgList(ChatMessage chatMessage)
	{
		switch(chatMessage.channel)
		{
			case (int)EChatChannel.World:
				mWorldMessages.Add(chatMessage);
			    while(mWorldMessages.Count > mMaxLength)
				    mWorldMessages.RemoveAt(0);
				break;
			case (int)EChatChannel.Private:		
			    if((mPrivateMsgDic == null)||
				   (mPrivateMsgDic != null && !mPrivateMsgDic.ContainsKey(chatMessage.user)))
			   		 mPrivateMsgDic.Add(chatMessage.user,new List<ChatMessage>());
			
				mPrivateMsgDic[chatMessage.user].Add(chatMessage);
			    while(mPrivateMsgDic[chatMessage.user].Count > Chat.Instance.MaxLength)
				    mPrivateMsgDic[chatMessage.user].RemoveAt(0);
				break;
			default:
				break;			
		}
	}

    public void RefreshChatMessages()
    {
        StartCoroutine(MainScript.Execute(new GetChatCmd(mLastMessageTime), delegate(string err, Response response)
        {
            ChatMsgs chatMsgs = (response != null) ? response.Parse<ChatMsgs>() : null;
            if (chatMsgs != null && chatMsgs.msgs != null)
            {
                mWorldMessages.AddRange(chatMsgs.msgs);
                mLastMessageTime = chatMsgs.time;
            }
        }));
    }
}
