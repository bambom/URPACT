using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ChatWnd : Window<ChatWnd>
{
    public override string PrefabName { get { return "ChatWnd"; } }

    UIInput mMessageInput;
	UILabel mSendToLabel;
	Vector3 mPrePos;
	Transform mParentGrid;
	Transform mWorldGrid;
	Transform mTeamGrid;
	Transform mNameGrid;
	Transform mListPanel;
	GameObject mPopPanel;
	GameObject mInputObj;
	
	const int mMaxCount = 6;
	const int mMaxPrivateCount = 5;
	int mViewChanCount = 0;
	//int mPreChatCount = 0;
	int mChatChannel;	
	string mSelfName;
	string mGetPlayerName;

    protected override bool OnOpen()
    {
		mSelfName = PlayerDataManager.Instance.Attrib.Name;
		InGameMainWnd.Instance.WndObject.transform.Find("ChatMessage").gameObject.SetActive(false);
        mMessageInput = Control("MsgInput").GetComponent<UIInput>();
		mInputObj = Control("MsgInput");
		mMessageInput.onSubmit = OnSubmit;
		mListPanel = Control("ChatListPanel").transform;
		mParentGrid = Control("WorldListGrid").transform;
		mWorldGrid = Control("WorldListGrid").transform;
		mTeamGrid = mListPanel.Find("TeamListGrid").transform;
		mNameGrid = Control("NameGrid").transform;
		mPopPanel = Control("PopPanel");
		mSendToLabel = Control("SentTo").transform.Find("Label").GetComponent<UILabel>();
		mPrePos = mInputObj.transform.localPosition;
		
		UIEventListener.Get(Control("AddButton",mPopPanel)).onClick = OnClickAddFriend;
		UIEventListener.Get(Control("ChatButton",mPopPanel)).onClick = OnClickPrivateChat;	
		UIEventListener.Get(Control("InfoButton",mPopPanel)).onClick = OnClickGetPlayerInfo;
		UIEventListener.Get(Control("WorldButton")).onClick = OnClickWorldChannel;
		UIEventListener.Get(Control("ChatButton")).onClick = OnClickClose;
		UIEventListener.Get(Control("BgButton")).onClick = OnClickClose;
		mPopPanel.SetActive(false);
		
		InitChatMsg();
        return base.OnOpen();
    }

    protected override bool OnClose()
    {
        return base.OnClose();
    }
	
	void InitChatMsg()
	{
		int index = 0;
		while(index < Chat.Instance.WorldMessages.Count)
		{
			AddChatMessage(Chat.Instance.WorldMessages[index],null,0);
			index++;
		}
		InitPrtMsg(true);
		InitPrtMsg(false);
		OnClickWorldChannel(Control("WorldButton"));
	}
		
    void InitPrtMsg(bool IsInNameList)
	{
		foreach(string name in Chat.Instance.PrivateMsgDic.Keys)
		{
			if(Chat.Instance.SendToMeName.Contains(name) != IsInNameList)
				continue;
		    int flag = Chat.Instance.SendToMeName.Contains(name)? 1:2;
			int index = 0;
			while(index < Chat.Instance.PrivateMsgDic[name].Count)
			{
				AddChatMessage(Chat.Instance.PrivateMsgDic[name][index],name,flag);
				index++;
			}
		}			
	}
	void OnClickWorldChannel(GameObject go)
	{
		mChatChannel = (int)EChatChannel.World;
		MovePanel(Chat.Instance.WorldMessages.Count);
		ShowCurGrid(mWorldGrid,mParentGrid);
		mSendToLabel.gameObject.SetActive(false);
		mInputObj.transform.localPosition = mPrePos;
	}
	
	void OnSubmit(string inputString)
	{
        string message = mMessageInput.text;
		//message = ForbiddenWordManager.Instance.EscapeString(message);
		if(string.IsNullOrEmpty(message))
		{
			//MessageBoxWnd.Instance.Show("Your input is empty,Please input again!",MessageBoxWnd.StyleExt.Alpha);
			return;
		}
		switch(mChatChannel)
		{
			case (int)EChatChannel.World:
				Request(new SendChatCmd(message),delegate(string err ,Response response)
				{
					if(!string.IsNullOrEmpty(err))
					{
						MessageBoxWnd.Instance.Show(err,MessageBoxWnd.StyleExt.Alpha);
					}
				});
				break;
		
			case (int)EChatChannel.Private:
				Request(new SendChatCmd(message,mGetPlayerName), delegate(string err, Response response)
				{
					if(!string.IsNullOrEmpty(err))
					{
						MessageBoxWnd.Instance.Show(err,MessageBoxWnd.StyleExt.Alpha);
					    return;
					} 		
				    ChatResponse chatResponse = (response != null) ? response.Parse<ChatResponse>() : null;
				    ChatMessage sendMessage = new ChatMessage();
				    sendMessage.user = mSelfName;
				    sendMessage.channel = (int)EChatChannel.Private;
				    sendMessage.msg = message;
				    sendMessage.time = chatResponse.time;
					AddMsgToList(sendMessage);
				    AddChatMessage(sendMessage,null,0);				    
		        });
				break;
				
			default:
				break;
		}
		
		mMessageInput.text = "";
	    mMessageInput.selected = false;
	}	

	void CreatePrivateChan(string name,bool isView)
	{
		if(IsContainObj(mNameGrid.gameObject,name))
		{  
			if(isView)
				UpdateNameList(mNameGrid.Find(name).gameObject);
			return;
		}
		else 
		{
			GameObject nameChan = GameObject.Instantiate(Resources.Load("NameChannel")) as GameObject;
			SetObjectPos(nameChan,mNameGrid.gameObject);
			nameChan.name = name;
			Control("NameLabel",nameChan).GetComponent<UILabel>().text = name;	
			Control("NameLabel",nameChan).GetComponent<UILabel>().color = Color.green;
			UIEventListener.Get(Control("CloseBtn",nameChan)).onClick = OnClickDestroyChan;
			UIEventListener.Get(nameChan).onClick = OnClickPrivateChat;
			if(isView){
				nameChan.SetActive(false);
				UpdateNameList(nameChan);	
			}
			else{
				if(mViewChanCount == mMaxPrivateCount)
					nameChan.SetActive(false);
				else{
					nameChan.SetActive(true);
				    mViewChanCount++;
				}
			}
		
			mNameGrid.GetComponent<UIGrid>().repositionNow = true;
			GameObject privateListGrid = GameObject.Instantiate(Resources.Load("PrivateListGrid")) as GameObject;
			SetObjectPos(privateListGrid,mListPanel.gameObject);
			privateListGrid.transform.localPosition = mWorldGrid.localPosition;
			privateListGrid.name = name;
			privateListGrid.SetActive(false);
		}
	}
	
	bool IsContainObj(GameObject go,string name)
	{
		for(int i=0; i<go.transform.childCount; i++)
		{
			if(go.transform.GetChild(i).name == name)
				return true;
		}
		return false;
	}
	
	void UpdateNameList(GameObject obj)
	{
		if(obj.activeSelf)
			return;
		float lastViewPos = -mNameGrid.GetComponent<UIGrid>().cellHeight * (mMaxPrivateCount -1);
		if(mViewChanCount < mMaxPrivateCount){		
			obj.SetActive(true);
			mViewChanCount++;				
		}
		else{
			for(int i=0; i<mNameGrid.childCount; i++)
			{
				if(Mathf.Approximately(lastViewPos,mNameGrid.GetChild(i).localPosition.y) && 
				   mNameGrid.GetChild(i).gameObject.activeSelf)
				{
					mNameGrid.GetChild(i).gameObject.SetActive(false);
					break;
				}
			}
			obj.SetActive(true);
		}
		mNameGrid.GetComponent<UIGrid>().repositionNow = true;
	}
	
	void OnClickDestroyChan(GameObject go)
	{
		int i;
		for(i=0; i<mNameGrid.childCount; i++)
		{
			if(!mNameGrid.GetChild(i).gameObject.activeSelf)
			{
				mNameGrid.GetChild(i).gameObject.SetActive(true);
				break;
			}
		}
		if(i == mNameGrid.childCount)
			mViewChanCount--;
		go.transform.parent.gameObject.SetActive(false);
		GameObject.Destroy(go.transform.parent.gameObject);
		mNameGrid.GetComponent<UIGrid>().repositionNow = true;
		GameObject.Destroy(mListPanel.Find(go.transform.parent.name).gameObject);
		Chat.Instance.PrivateMsgDic.Remove(go.transform.parent.name);
		OnClickWorldChannel(Control("WorldButton"));
	}
	
	void OnClickClose(GameObject go)
	{
		Close();		
		InGameMainWnd.Instance.WndObject.transform.Find("ChatMessage").gameObject.SetActive(true);		
		if(Chat.Instance.SendToMeName.Count == 0)
		{
			GameObject chatRemindObj = InGameMainWnd.Instance.WndObject.transform.
				                       Find("ChatMessage/ChatIcon").gameObject;
			chatRemindObj.GetComponent<TweenScale>().enabled = false;
			chatRemindObj.SetActive(false);
		}
		if (!InGameMainWnd.Exist)
			InGameMainWnd.Instance.Open();
		else
			InGameMainWnd.Instance.Show();	
	}
	
	public void AddChatMessage(ChatMessage chatMessage,string sendToName,int flag)
	{
		Transform parentTran;
	    GameObject chatItem = GameObject.Instantiate(Resources.Load("ChatItem")) as GameObject;
		
		if(sendToName == null)
			GetParentGrid(chatMessage.channel,chatMessage.user,out parentTran,flag);
		else
			GetParentGrid(chatMessage.channel,sendToName,out parentTran,flag);
		
		SetObjectPos(chatItem,parentTran.gameObject);
	    UIEventListener.Get(chatItem).onClick = OnClickPop;
		if(parentTran.childCount > Chat.Instance.MaxLength){
			GameObject desObj = parentTran.GetChild(0).gameObject;
			desObj.SetActive(false);
			GameObject.Destroy(desObj);
		}
		chatItem.transform.Find("MessageLabel").GetComponent<UILabel>().text = chatMessage.msg;
		chatItem.transform.Find("PlayerName").GetComponent<UILabel>().text = chatMessage.user + ":";
		chatItem.transform.Find("PlayerName").GetComponent<UILabel>().color = Color.green;
		chatItem.transform.Find("StoreLabel").GetComponent<UILabel>().text = chatMessage.user;
		chatItem.transform.Find("TimeLabel").GetComponent<UILabel>().text = GetTime(Global.JSLongToDataTime(chatMessage.time));
		switch(chatMessage.channel)
		{			
			case (int)EChatChannel.Private:
				chatItem.transform.Find("MessageLabel").GetComponent<UILabel>().color = Color.magenta;				    
			    break;			
			default:
				break;
		}
		parentTran.GetComponent<UIGrid>().repositionNow = true;
		UpdatePanelPos(chatMessage,parentTran);
	}
	
    void UpdatePanelPos(ChatMessage chatMsg,Transform trans)
	{
		if(trans != mParentGrid)
			return;
		switch(chatMsg.channel)
		{
			case (int)EChatChannel.World:
				MovePanel(Chat.Instance.WorldMessages.Count);
				break;
			case (int)EChatChannel.Private:
			    if(chatMsg.user == mSelfName)
					MovePanel(Chat.Instance.PrivateMsgDic[mGetPlayerName].Count);
				else
				    MovePanel(Chat.Instance.PrivateMsgDic[chatMsg.user].Count);
				break;
			default:
				break;
		}
	}
			
	string GetTime(System.DateTime now)
	{	
		return string.Format("{0}:{1}:{2}",AddZero(now.Hour),AddZero(now.Minute),AddZero(now.Second));
	}
	
	string AddZero(int num)
	{ 
		if(num < 10)
		    return string.Format("0{0}",num);
		else
			return string.Format("{0}",num);
	}
	
	void GetParentGrid(int chan,string name,out Transform trans,int flag)
	{	
		switch(chan)
		{
			case (int)EChatChannel.Private:
				if(name == mSelfName)
				{
				    trans = mListPanel.Find(mGetPlayerName);
				    if(trans != mParentGrid)
				        mNameGrid.Find(mGetPlayerName).Find("MsgIcon").gameObject.SetActive(true);
				}
				else
				{
					trans = mListPanel.Find(name);		
					switch(flag)
					{
						case 0:
							CreatePrivateChan(name,false);								
							if(trans != mParentGrid){
						        mNameGrid.Find(name).Find("MsgIcon").gameObject.SetActive(true);
						        Chat.Instance.SendToMeName.Add(name);
						        if(Chat.Instance.SendToMeName.Count > Chat.Instance.MaxLength)
							    	Chat.Instance.SendToMeName.RemoveAt(0);
							}
							break;
						case 1:		
							CreatePrivateChan(name,mViewChanCount < mMaxPrivateCount);	
							mNameGrid.Find(name).Find("MsgIcon").gameObject.SetActive(true);
							break;
						case 2:
							CreatePrivateChan(name,false);
							mNameGrid.Find(name).Find("MsgIcon").gameObject.SetActive(false);
							break;					  
					}	
					trans = mListPanel.Find(name);
				}
				break;
			
			case (int)EChatChannel.World:
			    trans = mWorldGrid;
				break;
			
			default:
			    trans = mTeamGrid;
				break;
		}			
	}
		
	void ShowCurGrid(Transform curParentGrid,Transform preParentGrid)
	{
		if(curParentGrid == preParentGrid)
			return;
		if(preParentGrid != null)
			preParentGrid.gameObject.SetActive(false);
		curParentGrid.gameObject.SetActive(true);
		mParentGrid = curParentGrid;
	}
	
	void MovePanel(int num)
	{
		Debug.Log(num);
		Vector3 targetPos = Vector3.zero;
		
		if(num > mMaxCount)
		{
			//if(mPreChatCount == num)
				//return;
			float offset = 65f;
			targetPos.y = offset+(num -1-mMaxCount)*mParentGrid.GetComponent<UIGrid>().cellHeight;			
		} 
	
		Vector3 delta = targetPos - mParentGrid.parent.localPosition;
		mParentGrid.parent.localPosition = targetPos;
		Vector4 cr = mParentGrid.parent.GetComponent<UIPanel>().clipRange;
		cr.x -= delta.x;
		cr.y -= delta.y;
		mParentGrid.parent.GetComponent<UIPanel>().clipRange = cr;
		//mPreChatCount = num;		
	}
	
	void OnClickPop(GameObject go)
	{
		string name = go.transform.Find("StoreLabel").GetComponent<UILabel>().text;
		if(name == mSelfName)
			return;
		mGetPlayerName = name;
		mPopPanel.SetActive(true);
		Vector3 target = mPopPanel.transform.localPosition;
		target.y = GetYPos(go.transform);
		mPopPanel.transform.localPosition = target;	
		Control("PlayerName",mPopPanel).GetComponentInChildren<UILabel>().text = name;
	}
	
	float GetYPos(Transform tran)
	{
		float yPos = 0;
		for(int i = 1; i<5; i++)
		{
			yPos += tran.localPosition.y;
			tran = tran.parent;
		}
		float yOffset = 100.0f;
		yPos += yOffset;
		return yPos;
	}
	
	void UpdateSize(GameObject go)
	{
		UILabel label = go.GetComponent<UILabel>();
		float startPos = label.transform.localPosition.x;
		label.pivot = UIWidget.Pivot.Right;
		float curPos = label.transform.localPosition.x;
		label.pivot = UIWidget.Pivot.Left;
		float xSize = Mathf.Abs(curPos - startPos);
		Vector3 target = mPrePos;
		target.x += xSize + 10.0f;
		mInputObj.transform.localPosition = target;
	}
	
	void OnClickGetPlayerInfo(GameObject go)
	{
	    ShowPlayerInfoWnd.Instance.RequestGetUserInfo(mGetPlayerName,new Vector3(0,0,-30));		
	}
	
	void OnClickAddFriend(GameObject go)
	{
		mPopPanel.SetActive(false);
		Request(new AddFriendCmd(mGetPlayerName), delegate(string err, Response response)
        {
            if (!string.IsNullOrEmpty(err))
            {
				MessageBoxWnd.Instance.Show( err ,MessageBoxWnd.StyleExt.Alpha);
                return;
            }
			MessageBoxWnd.Instance.Show("AddFriendSuccess",MessageBoxWnd.StyleExt.Alpha);
        });
	}
	
	void OnClickPrivateChat(GameObject go)
	{
		if(go.transform.parent.gameObject != mPopPanel){
			mGetPlayerName = go.name;
			go.transform.Find("MsgIcon").gameObject.SetActive(false);
			if(Chat.Instance.SendToMeName.Contains(go.name))
			{
				int index = Chat.Instance.SendToMeName.IndexOf(go.name);
				Chat.Instance.SendToMeName.RemoveAt(index);
			}
		}
		OpenPrtChat(mGetPlayerName);
	}
	
	public void OpenPrtChat(string name)
	{
		mChatChannel = (int)EChatChannel.Private;
		mSendToLabel.text = string.Format("To {0}:",name);
		mSendToLabel.color = Color.magenta;
		mSendToLabel.gameObject.SetActive(true);
	    mPopPanel.SetActive(false);
		UpdateSize(mSendToLabel.gameObject);
		CreatePrivateChan(name,true);
		if(Chat.Instance.PrivateMsgDic.ContainsKey(name))
			MovePanel(Chat.Instance.PrivateMsgDic[name].Count);
		ShowCurGrid(mListPanel.Find(name),mParentGrid);
		mGetPlayerName = name;
	}
	
	void AddMsgToList(ChatMessage chatMessage)
	{
	    if((Chat.Instance.PrivateMsgDic == null)||
		   (Chat.Instance.PrivateMsgDic != null && 
			!Chat.Instance.PrivateMsgDic.ContainsKey(mGetPlayerName)))		   		 
			Chat.Instance.PrivateMsgDic.Add(mGetPlayerName,new List<ChatMessage>());
		
		Chat.Instance.PrivateMsgDic[mGetPlayerName].Add(chatMessage);			    
		while(Chat.Instance.PrivateMsgDic[mGetPlayerName].Count > Chat.Instance.MaxLength)			    
			Chat.Instance.PrivateMsgDic[mGetPlayerName].RemoveAt(0);
	}
	
	void SetObjectPos(GameObject child, GameObject parent)
	{
		child.transform.parent = parent.transform;
		child.transform.localPosition = Vector3.zero;
		child.transform.localScale = Vector3.one;
	}
}
