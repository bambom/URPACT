using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FriendsListWnd : Window<FriendsListWnd>
{
	public override string PrefabName { get { return "FriendsListWnd"; } }
	
	private FriendList mFriendsList = new FriendList();
	private GameObject mParentGrid;
	private GameObject mChooseFriend;
	private GameObject mBackButton;
	private GameObject mChatButton;
	private int mMaxCount = 5;
		
	#region Protected
	protected override bool OnOpen()
    {
		Init();
		WinStyle = WindowStyle.WS_Ext;
        return base.OnOpen();
    }
	
    protected override bool OnClose()
    {
        return base.OnClose();
    }
	#endregion
	
	private void Init()
	{
		mParentGrid = Control("ListGrid");
		mBackButton = Control("BackButton");
		mChatButton = Control("ChatButton");
		UIEventListener.Get(mBackButton).onClick = OnClickBtnClose;
		UIEventListener.Get(mChatButton).onClick = OnClickChat;
	    UIEventListener.Get(Control("AddButton")).onClick = OnClickAddFriend;
		UIEventListener.Get(Control("DeleteButton")).onClick = OnClickDelete;		
		UIEventListener.Get(Control("InfoButton")).onClick = OnClickGetInfor;
		InitFriendsList();
	}
	
	
	private void OnClickBtnClose(GameObject go)
	{
		Close();
		if( !InGameMainWnd.Exist )
			InGameMainWnd.Instance.Open();
		else 
			InGameMainWnd.Instance.Show();
	}
	
	private void OnClickAddFriend(GameObject go)
	{
		string playerName = Control("Input").GetComponent<UIInput>().text;;
		Debug.Log(PlayerDataManager.Instance.Attrib.Name);
	    if(playerName == PlayerDataManager.Instance.Attrib.Name){
			MessageBoxWnd.Instance.Show("CantAddSelf",MessageBoxWnd.StyleExt.Alpha);	
			return;
		}	
		AddFriendRequest(playerName);
		Control("Input").GetComponent<UIInput>().text = "";
	}
	
	private void OnClickDelete(GameObject go)
	{
		if(mChooseFriend == null)
			return;
		MessageBoxWnd.Instance.Show(("SureDeleteFriend")
			,MessageBoxWnd.Style.OK_CANCLE);
		MessageBoxWnd.Instance.OnClickedOk = OnClickSureDel;
	}
	
	private void OnClickSureDel(GameObject go)
	{
		if(mChooseFriend != null){
			string playerName = Control("FriendName",mChooseFriend).GetComponentInChildren<UILabel>().text;
			DeleteFriendRequest(playerName);
		}
	}
	
	private void OnClickChat(GameObject go)
	{
		
		if(mChooseFriend != null){
			if(!mChooseFriend.transform.Find("OnlineLabel").gameObject.activeSelf)
			{
				MessageBoxWnd.Instance.Show("UserNotOnline",MessageBoxWnd.StyleExt.Alpha);
				return;
			}
			string playerName = Control("FriendName",mChooseFriend).GetComponentInChildren<UILabel>().text;
			Close();
			if(InGameMainWnd.Exist)
				InGameMainWnd.Instance.Close();		
			InGameMainWnd.Instance.Open();
			ChatWnd.Instance.Open();
			ChatWnd.Instance.OpenPrtChat(playerName);
		}
	}
	
	private void OnClickGetInfor(GameObject go)
	{
		if(mChooseFriend != null){
			string playerName = Control("FriendName",mChooseFriend).GetComponentInChildren<UILabel>().text;
			ShowPlayerInfoWnd.Instance.RequestGetUserInfo(playerName,new Vector3(0,0,-30));
		}
	    
	}
	
	private void InitFriendsList()
	{		
		GetFriendsRequest();
	}
	
	private void UpdateFriendsList()
	{
		DestroyFriendList();
		if(mFriendsList.friends == null)
			return;	
		
		SortFriendList(true);
		SortFriendList(false);
		mParentGrid.transform.parent.GetComponent<UIDraggablePanel>().enabled =
				       mFriendsList.friends.Length > mMaxCount ? true : false;
	}
	
	private void DestroyFriendList()
	{
		Debug.Log(mParentGrid);
		for(int i=0; i<mParentGrid.transform.childCount; i++)
			GameObject.Destroy(mParentGrid.transform.GetChild(i).gameObject);
		mParentGrid.transform.DetachChildren();
	}
	
	private void OnClickChooseFriend(GameObject go)
	{	
		if(mChooseFriend == go)
			return;
		if(mChooseFriend != null)
			mChooseFriend.transform.Find("ChooseIcon").gameObject.SetActive(false);
		
        go.transform.Find("ChooseIcon").gameObject.SetActive(true);	
		mChooseFriend = go;		
	}
	
	private void SetObjectPos(GameObject child,GameObject parent)
	{
		child.transform.parent = parent.transform;
		child.transform.localScale = Vector3.one;
		child.transform.localPosition = Vector3.zero;
	}
	
	private void SortFriendList(bool showOnline)
	{
		for(int i=0 ;i<mFriendsList.friends.Length ;i++)
		{
			GameObject friendItem;
			if(showOnline)
			{
				if(!mFriendsList.friends[i].online)
					continue;
				friendItem= GameObject.Instantiate(Resources.Load("FriendItem")) as GameObject;
				SetObjectPos(friendItem,mParentGrid);
				Control("FriendName",friendItem).GetComponentInChildren<UILabel>().text = mFriendsList.friends[i].name;
				Control("FriendName",friendItem).GetComponentInChildren<UILabel>().color = Color.blue;
				Control("OnlineLabel",friendItem).GetComponent<UILabel>().color = Color.blue;
				Control("NotOnlineLabel",friendItem).SetActive(false);
			}
			else
			{
				if(mFriendsList.friends[i].online)
					continue;
				friendItem = GameObject.Instantiate(Resources.Load("FriendItem")) as GameObject;
				SetObjectPos(friendItem,mParentGrid);
				Control("FriendName",friendItem).GetComponentInChildren<UILabel>().text = mFriendsList.friends[i].name;	
				Control("OnlineLabel",friendItem).SetActive(false);
			}
			ShowRoleIcon(friendItem,mFriendsList.friends[i].role);		
			UIEventListener.Get(friendItem).onClick = OnClickChooseFriend;	
		}
		mParentGrid.gameObject.GetComponent<UIGrid>().repositionNow = true;
		
	}
	
	private void ShowRoleIcon(GameObject obj,int roleId)
	{
		switch(roleId)
		{
			case (int)RoleID.Warrior:
				Control("RoleIcon",obj).GetComponentInChildren<UISprite>().spriteName = "Button21_Attack_Job_01";
				break;
			case (int)RoleID.Assassin:
				Control("RoleIcon",obj).GetComponentInChildren<UISprite>().spriteName = "Button21_Attack_Job_02";
				break;
			case (int)RoleID.Mage:
				Control("RoleIcon",obj).GetComponentInChildren<UISprite>().spriteName = "Button21_Attack_Job_03";
				break;
			default:
				break;
		}
	}
	
	private void GetFriendsRequest()
	{
        Request(new GetFriendsCmd(), delegate(string err, Response response)
        {
            if (!string.IsNullOrEmpty(err))
            {
				MessageBoxWnd.Instance.Show( err ,MessageBoxWnd.StyleExt.Alpha);
                return;
            }
			
 			mFriendsList = (response != null) ? response.Parse<FriendList>() : null;	
            if (mFriendsList != null){
				PlayerDataManager.Instance.OnFriendsList(mFriendsList);
			}
			UpdateFriendsList();
        });
	}
	
	private void DeleteFriendRequest(string user)
	{
        Request(new DeleteFriendCmd(user), delegate(string err, Response response)
        {
            if (!string.IsNullOrEmpty(err))
            {
				MessageBoxWnd.Instance.Show( err ,MessageBoxWnd.StyleExt.Alpha);
                return;
            }
			GetFriendsRequest();
        });
	}
	
	private void AddFriendRequest(string user)
	{
        Request(new AddFriendCmd(user), delegate(string err, Response response)
        {
            if (!string.IsNullOrEmpty(err))
            {
				MessageBoxWnd.Instance.Show( err ,MessageBoxWnd.StyleExt.Alpha);
                return;
            }
			MessageBoxWnd.Instance.Show("AddFriendSuccess" ,MessageBoxWnd.StyleExt.Alpha);
			GetFriendsRequest();
        });
	}
}

