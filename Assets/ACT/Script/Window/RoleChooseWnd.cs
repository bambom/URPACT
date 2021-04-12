using UnityEngine;
using System.Collections.Generic;
using System.Collections;

/// <summary>
/// 删除角色窗体。
/// </summary>
public class RoleChooseWnd : Window<RoleChooseWnd>
{
    public static string SaveUserName = "SaveUserName";
    RoleList mRoleList;
    int mSelectedRole = 0;
	int mRoleMaxCount = 3;
	List<GameObject> mCtrlRoleList = new List<GameObject>();
    GameObject mGoCamera;
    public override string PrefabName { get { return "RoleChooseWnd"; } }
	/// <summary>
	/// 创建窗体.
	/// </summary>
    protected override bool OnOpen()
    {
		mGoCamera = GameObject.Instantiate(Resources.Load("RoleChooseCamera")) as GameObject;
		GameObject go = GameObject.Find("WindowsRoot");
		mGoCamera.transform.parent = go.transform;	
		mGoCamera.transform.localPosition = new Vector3(21141.0f,275.0f,-1620.0f);

		Init();	

		CharactorModelInit();

        return base.OnOpen();
    }
	
    protected override bool OnClose()
    {
		GameObject.Destroy(mGoCamera);
        return base.OnClose();
    }
	
	private void Init()
    {
		///按钮点击事件
        UIEventListener.Get(Control("ContinueGame")).onClick = OnClickContinueGame;
		UIEventListener.Get(Control("DeleteRole")).onClick = OnClickDeleteRole;
		UIEventListener.Get(Control("Facebook")).onClick = OnClickFacebook;
        // 清除进入游戏的信息。
        MainScript.Instance.ClientUserName = null;	
		mCtrlRoleList.Add(Control("1".ToString()));
		mCtrlRoleList.Add(Control("2".ToString()));
		mCtrlRoleList.Add(Control("3".ToString()));

        UpdateFacebookStatus();
	}
	
	void CharactorModelInit()
	{	
		GameObject goSubRole = Control("Role");
		PressDrag PDScript = Control ("CharactorCollider").GetComponent<PressDrag>();
		if(null != PDScript){
			PDScript.target = goSubRole.transform;
		}else{
			PDScript = Control ("CharactorCollider").AddComponent<PressDrag>();
			PDScript.target = goSubRole.transform;
		}
	}	
	
	private void ResetRoleInfoBg()
	{
		GameObject go ;
		for(int i =0 ;i<mCtrlRoleList.Count;i++)
		{
			go = Control("Bg",mCtrlRoleList[i]);
			go.GetComponent<UISprite>().spriteName = "Window13_Choose_Player_04";
		}
	}
	
	void ClickRoleListColumn( GameObject go )
	{
		int nIndex = int.Parse(go.name);
		if( nIndex > mRoleList.users.Length ){
			this.Close();
			if(!CreateRoleWnd.Exist)
				CreateRoleWnd.Instance.Open();
			return;
		}
		ResetRoleInfoBg();
		GameObject GRole = Control( "Bg" , mCtrlRoleList[nIndex-1] );
		GRole.GetComponent<UISprite>().spriteName = "Window13_Choose_Player_03";
		mSelectedRole = nIndex-1;
		LoginHelper.RoleIndex = mSelectedRole;
		OnSelectRole(mRoleList.users[nIndex-1].Name);
	}
	
	void SaveClientID()
	{
		if( MainScript.Instance.CheckAccount())
		{
			if( null == PlayerPrefs.HasKey(Global.SaveClientID) ||
				string.IsNullOrEmpty(PlayerPrefs.GetString(Global.SaveClientID)))
			{
				switch(MainScript.Instance.Platform)
				{
					case EPlatform.Device:
						NGUIDebug.Log("Device MainScript.Instance.ClientId" + MainScript.Instance.ClientId);
						PlayerPrefs.SetString(Global.SaveClientID,MainScript.Instance.ClientId);
						break;
					case EPlatform.GameCenter:
						NGUIDebug.Log("GameCenter Social.localUser.id" + Social.localUser.id);
						PlayerPrefs.SetString(Global.SaveClientID,Social.localUser.id);	
						break;
				}
			}else{
				string ClientIDS = PlayerPrefs.GetString(Global.SaveClientID);
				string IDS;
				switch(MainScript.Instance.Platform)
				{
					case EPlatform.Device:
					{
						IDS = ClientIDS + '|' + MainScript.Instance.ClientId;
						NGUIDebug.Log("Device IDS" + IDS);
						PlayerPrefs.SetString(Global.SaveClientID,IDS);
					}
					break;
					case EPlatform.GameCenter:	
					{
						IDS = ClientIDS + '|' + Social.localUser.id;
						NGUIDebug.Log("GameCenter IDS" + IDS);
						PlayerPrefs.SetString(Global.SaveClientID,IDS);	
					}
					break;
				}
			}
		}
	}
	
	private void InitRoleList(int nRoleNum)
	{
		GameObject go ;
		for(int i=0; i<mRoleMaxCount; i++){
			if(i<nRoleNum){
				mCtrlRoleList[i].transform.Find("LabelCreate").gameObject.SetActive(false);
				go = Control("RoleInfo",mCtrlRoleList[i]);
				Control("Level",go).GetComponent<UILabel>().text = "Lv " + mRoleList.users[i].Level.ToString() ;
				Control("Name",go).GetComponent<UILabel>().text = mRoleList.users[i].Name ;	
				Control("Icon",go).GetComponent<UISprite>().spriteName 
					= UnitBaseManager.Instance.GetItem(mRoleList.users[i].Role).Label;
			}else{
				mCtrlRoleList[i].transform.Find("RoleInfo").gameObject.SetActive(false);
				mCtrlRoleList[i].transform.Find("LabelCreate").gameObject.SetActive(true);
			}
			UIEventListener.Get(mCtrlRoleList[i]).onClick = ClickRoleListColumn;
		}
		//SetFocus
		{
			ResetRoleInfoBg();
			int index = GetPreLoginUser();
			if(index >= nRoleNum)
				index = nRoleNum - 1;
			GameObject GRole = Control( "Bg" , mCtrlRoleList[index] );
			GRole.GetComponent<UISprite>().spriteName = "Window13_Choose_Player_03";
			mSelectedRole = index;
			LoginHelper.RoleIndex = mSelectedRole;
			OnSelectRole(mRoleList.users[index].Name);
		}
		
		//NGUIDebug.Log("SaveClientID");
		//SaveClientID();
	}
	
	private int GetPreLoginUser()
	{
		int index;
		for(index = 0 ;index<mRoleList.users.Length ;index++)
		{
			if(PlayerPrefs.HasKey(SaveUserName) && 
			   mRoleList.users[index].Name == PlayerPrefs.GetString(SaveUserName))
			   break;
		}
		return index;
	}
	
    public void OnRoleList(RoleList roleList)
    {
        mRoleList = roleList;
		InitRoleList(roleList.users.Length);
    }
	
	void OnClickedCancle( GameObject go )
	{
		Application.Quit();
	}
	
    void OnSelectRole(string user)
    {
		MainScript.Instance.MyDebugLog("OnSelectRole: " + System.DateTime.Now);
		Global.ShowLoadingStart();
        Request(new GetUserDataCmd(user, false), delegate(string err, Response response)
        {
            PlayerData playerData = (response != null) ? response.Parse<PlayerData>() : null;
            if (playerData == null) {
                MessageBoxWnd.Instance.Show(("CreateRoleFailed")
					,MessageBoxWnd.Style.CANCLE);
				MessageBoxWnd.Instance.OnClickedCancle = OnClickedCancle;
                return;
            }
            // 创建角色的3D模型。
			GameObject goRole = Control("Role");
			goRole.transform.localEulerAngles = new Vector3(0.0f,180.0f,0.0f);
            CreateRole(goRole, playerData);
			Global.ShowLoadingEnd();
        });
    }

    void CreateRole(GameObject parent, PlayerData data)
    {
		MainScript.Instance.MyDebugLog("CreateRole: " + System.DateTime.Now);
		if( parent.transform.childCount>0){
			Transform[] children = parent.GetComponentsInChildren<Transform>();
	        foreach (Transform child in children) {
				if(parent.name == child.name)
					continue;
	            GameObject.Destroy(child.gameObject);
	        }
		}
		Player player = Player.CreateShowPlayer(data, parent);
		MainScript.Instance.MyDebugLog("CreateShowPlayer: Done" + System.DateTime.Now);
		if (player != null)
        	player.PlayAction(Data.CommonAction.Show);
    }
	
	void OnDeleteRoleClickedOk( GameObject go )
	{
		// 删除角色请求
        string user = mRoleList.users[mSelectedRole].Name;
        Request(new DeleteUserCmd(user), delegate(string err, Response response)
        {
            if (!string.IsNullOrEmpty(err))
            {
               	MainScript.Instance.MyDebugLog("DeleteUserRequest Error!!" + err);
                return;
            }

            RoleList roleList = response != null ? response.Parse<RoleList>() : null;
			LoginHelper.roleList = roleList;
            if (roleList != null && roleList.users != null && roleList.users.Length > 0)
                OnRoleList(roleList);
            else
            {
                ///进入创建角色窗体
                CreateRoleWnd.Instance.Open();
                Close();
            }
        });
	}
	
	void OnDeleteRoleClickedCancle( GameObject go )
	{
		
	}
	
	/// <summary>
	/// 删除角色，角色信息置空，进入创建角色.
	/// </summary>
    private void OnClickDeleteRole(GameObject go)
    {
		MessageBoxWnd.Instance.Show(("DeleteRole"));
		MessageBoxWnd.Instance.OnClickedOk = OnDeleteRoleClickedOk;
		MessageBoxWnd.Instance.OnClickedCancle = OnDeleteRoleClickedCancle;
    }
	
	/// <summary>
	/// 继续游戏，进入主城
	/// </summary>
    private void OnClickContinueGame(GameObject go)
    {
		///窗体切换
        string user = mRoleList.users[mSelectedRole].Name;
        MainScript.Instance.ClientUserName = user;
		Global.ShowLoadingStart();
        Request(new EnterGameCmd(user), delegate(string err, Response response)
        {
			Global.ShowLoadingEnd();
            if (!string.IsNullOrEmpty(err))
            {
                MainScript.Instance.MyDebugLog("EnterGameCmd Error!!" + err);
                return;
            }

            MainScript.Instance.UserLogined = true;

            PlayerData playerData = response != null ? response.Parse<PlayerData>() : null;
            if (playerData != null)
            {
                PlayerDataManager.Instance.OnPlayerData(playerData);
				//NGUIDebug.Log("SystemBroadcast");
			    PlayerPrefs.SetString(SaveUserName,user);
				SoundManager.Instance.PlaySound("UI_Enter_02");
                //在这里添加聊天和公告的脚本，如果已经添加就不再添加
                if (MainScript.Instance.gameObject.GetComponent<SystemBroadcast>() == null)
                    MainScript.Instance.gameObject.AddComponent<SystemBroadcast>();
                if (MainScript.Instance.gameObject.GetComponent<Chat>() == null)
                    MainScript.Instance.gameObject.AddComponent<Chat>();
				Close();
				
				SceneList SLItem = SceneListManager.Instance.GetItem((int)ESceneID.Main,(int)ESceneType.Main);

				Global.CurSceneInfo.ID = ESceneID.Main;
				Global.CurSceneInfo.Type = ESceneType.Main;
				Global.CurSceneInfo.Name = SLItem.Name;		
        		MainScript.Instance.LoadLevel(Global.CurSceneInfo);
            }
        });
		
	}

    void UpdateFacebookStatus()
    {
        GameObject facebookObj = Control("Label", Control("Facebook"));
        UILabel label = facebookObj ? facebookObj.GetComponent<UILabel>() : null;
        if (label)
            label.text = FacebookHelper.IsInited ? (FacebookHelper.IsLoggedIn ? "Logout" : "LogIn") : "UnInit";
    }
	
	void OnClickFacebook(GameObject go)
	{
		if (FacebookHelper.IsInited)
		{
	        if (FacebookHelper.IsLoggedIn)
			{
	            FacebookHelper.Logout();
				UpdateFacebookStatus();
			}
	        else
	        {
	            FacebookHelper.Login(delegate(FBResult result)
	            {
	                UpdateFacebookStatus();
	            });
	        }
		}
	}
	
		
	void RequestPhysicInfo()
	{
		Request(new GetStrengthCmd(), delegate(string err, Response response)
        {
			if (!string.IsNullOrEmpty(err))
            {
				MessageBoxWnd.Instance.Show( err ,MessageBoxWnd.StyleExt.Alpha );
				return;
			}

            StrengthInfo strengthInfo = (response != null) ? response.Parse<StrengthInfo>() : null;
            if (strengthInfo != null)
                PlayerDataManager.Instance.OnStrengthInfo(strengthInfo);
		});
	}
}
