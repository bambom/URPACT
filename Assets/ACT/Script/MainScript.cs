using UnityEngine;
using System.Collections;
using System.IO;
using System;

public class MainScript : MonoBehaviour
{
    static MainScript msInstance = null;
    public static MainScript Instance { get { return msInstance; } }

    public EServerIpList Server = EServerIpList.Local;
    public EPlatform Platform = EPlatform.Device;
    public EGameMode GmMode = EGameMode.GMDebug;
    public string Version = "God20140910";
    public bool LogCombat = false;
    public bool LogBuff = false;
    public bool RecordForBot = false;
    public bool CheckUpdate = false;

    // member functions.
    Client mClient = new Client();
    string mClientId = null;
    string mServerIp = "127.0.0.1";
    int mServerPort = 3069;
    float mConnectTimeout = 100.0f;

    public Client Client { get { return mClient; } }
    public string ClientId { get { return mClientId; } set { mClientId = value; } }
    public string ClientUserName { get; set; }
    public string ServerIp { get { return mServerIp; } set { mServerIp = value; } }
    public int ServerPort { get { return mServerPort; } set { mServerPort = value; } }
    public float ConnectTimeout { get { return mConnectTimeout; } }
    public GameConfig Config { get; set; }
    public bool UserLogined { get; set; }
	
	float mProgressTime = 0.0f;
	float mStartTime = 0.0f;
    #region WriteDebugLog
    public string FileName
    {
        get
        {
            DateTime Dt = DateTime.Now;
            string fileName = string.Format("LogFile-{0}-{1}-{2}.txt", Dt.Year, Dt.Month, Dt.Day);
            return fileName;
        }
    }

    public string LogPath
    {
        get
        {
            string path = null;
            if (Application.platform == RuntimePlatform.IPhonePlayer)
            {
                path = Application.dataPath.Substring(0, Application.dataPath.Length - 5);
                path = path.Substring(0, path.LastIndexOf('/')) + "/Documents/";
            }
			else if(Application.platform==RuntimePlatform.Android){
				path = Application.persistentDataPath + "//";
			}
            //			else if(Application.platform==RuntimePlatform.Android){
            //		   		path = Application.persistentDataPath + "//";
            //		    }else if(Application.platform==RuntimePlatform.WindowsPlayer){
            //		    	path = Application.dataPath + "//";
            //		    }else if(Application.platform==RuntimePlatform.WindowsEditor){
            //				path = Application.dataPath;
            //			    path = path.Substring(0,path.LastIndexOf('/'))+"/GameLog/";
            //	 	    }else{
            //				path = Application.dataPath + "//";
            //			}
            return path + FileName;
        }
    }

    public void MyDebugLog(string str)
    {
        if (GmMode == EGameMode.GMDebug)
        {
            DateTime Dt = DateTime.Now;
            string Content = string.Format("{0}-{1}-{2} : {3}\n", Dt.Day, Dt.Hour, Dt.Minute, str);
            FileStream fs = new FileStream(LogPath, FileMode.Append, FileAccess.Write);
            StreamWriter sw = new StreamWriter(fs);
            sw.Flush();
            sw.BaseStream.Seek(0, SeekOrigin.End);
            sw.WriteLine(Content);
            sw.Flush();
            sw.Close();
        }
    }

    void StartWriteLog()
    {
        if (GmMode == EGameMode.GMDebug)
        {
            FileStream fs = new FileStream(LogPath, FileMode.Create, FileAccess.Write);
            StreamWriter sw = new StreamWriter(fs);
            sw.Flush();
            sw.BaseStream.Seek(0, SeekOrigin.Begin);
            string startStr = ("-------------------GMDebug-LogStart-GMDebug------------------------\n");
            sw.WriteLine(startStr);
            sw.Flush();
            sw.Close();
        }
        else
        {

        }
    }
    #endregion WriteDebugLog

    void Awake()
    {
        msInstance = this;
        mClientId = SystemInfo.deviceUniqueIdentifier;

        ConfigServer();
        StartWriteLog();
    }

    // Use this for initialization
    void Start()
    {
        GameObject.DontDestroyOnLoad(this);

        gameObject.AddComponent<UpdateHelper>();
    }

    void UpdateFinish()
    {
        if (gameObject.GetComponent<UpdateHelper>() != null)
            UpdateHelper.Instance.UpdateWWWReset();

		/*
		if (GmMode != EGameMode.GMDebug)
			FacebookHelper.Init();
		*/

        // do manager's init.
        ItemBaseManager.Instance.GetAllItem();
        EquipBaseManager.Instance.GetAllItem();
        DefaultEquipManager.Instance.GetAllItem();
        ConfigUser();
    }

    void ConfigServer()
    {
        switch (Server)
        {
            case EServerIpList.Local:
                mServerIp = "127.0.0.1";
                mServerPort = 3069;
                mConnectTimeout = 10.0f;
                break;
            case EServerIpList.Lan_04:
                mServerIp = "192.168.0.4";
                mServerPort = 3069;
                mConnectTimeout = 10.0f;
                break;
            case EServerIpList.China:
                mServerIp = "god.azku.cn";
                mServerPort = 3069;
                mConnectTimeout = 10.0f;
                break;
            case EServerIpList.Public:
                mServerIp = "god.azku.cn";
                mServerPort = 3069;
                mConnectTimeout = 10.0f;
                break;
			case EServerIpList.F:
			{
//				mServerIp = "27.54.255.220";
				mServerIp = "192.168.0.134";
                mServerPort = 9000;
				mConnectTimeout = 10.0f;
				break;
			}
				break;
        }
    }
	
	void OnApplicationFocus()
	{
		mStartTime = Time.realtimeSinceStartup;
	}
	
    void OnApplicationPause(bool Pause)
    {
//		if(Pause)
//		{
//			if( Global.CurSceneInfo.ID == ESceneID.PVP)
//				return;
//			Global.ShowLoadingStart();
//			mProgressTime = 3.0f;
//			EmptyForLoadingWnd.Instance.Progress( mProgressTime.ToString() );
//		}
	}

    void EntryCreateRoleScene()
    {
        SceneList SLItem = SceneListManager.Instance.GetItem((int)ESceneID.CreateRole, (int)ESceneType.CreateRole);
        Global.CurSceneInfo.ID = ESceneID.CreateRole;
        Global.CurSceneInfo.Type = ESceneType.CreateRole;
        Global.CurSceneInfo.Data = 0;
        Global.CurSceneInfo.Name = SLItem.Name;
        LoadLevel(Global.CurSceneInfo);
    }

    void EntryGuideScene()
    {
        SceneList SLItem = SceneListManager.Instance.GetItem((int)ESceneID.A_FirstGameGuide, (int)ESceneType.GameGuide);
        Global.CurSceneInfo.ID = ESceneID.A_FirstGameGuide;
        Global.CurSceneInfo.Type = ESceneType.GameGuide;
        Global.CurSceneInfo.Name = SLItem.Name;
        LoadLevel(Global.CurSceneInfo);
    }
	
	public bool CheckAccount()
	{
		if ( PlayerPrefs.HasKey(Global.SaveRoleID))
		{
			Global.GuideRole = int.Parse( PlayerPrefs.GetString(Global.SaveRoleID) );
			return false;
		}else 
			return true;
		
		#region GameCenter
		if( PlayerPrefs.HasKey(Global.SaveRoleID) )
		{
			Global.GuideRole = int.Parse( PlayerPrefs.GetString(Global.SaveRoleID) );
		}
		if( null == PlayerPrefs.HasKey(Global.SaveClientID) )
			return true;
		
		NGUIDebug.Log("CheckAccount");
		string ClientIDs = PlayerPrefs.GetString(Global.SaveClientID);
		string[] Clientids = ClientIDs.Split('|');
		string ClinetID = null ;
		if( Platform == EPlatform.Device ){
			ClinetID = mClientId;
			NGUIDebug.Log("ClinetID mClientId " + mClientId);
		}else if( Platform == EPlatform.GameCenter ){
			ClinetID = Social.localUser.id;
			NGUIDebug.Log("ClinetID Social.localUser.id " + Social.localUser.id);
		}
		
		for(int i = 0; i<Clientids.Length; i++)
		{
			if( 0 == string.Compare(Clientids[i],ClinetID) )
			{
				return false;
			}
		}
		return true;
		#endregion  GameCenter
	}
		
    void ConfigUser()
    {
        if (GmMode == EGameMode.GMDebug)
        {
            EntryCreateRoleScene();
            return;
        }

        if (!CheckAccount())
        {
            //false
            EntryCreateRoleScene();
        }
        else
        {
            //true
            EntryGuideScene();
        }
    }


    public void ConfigPlatform()
    {
        switch (Platform)
        {
            case EPlatform.Device:
                Global.ShowLoadingStart();
                gameObject.SendMessage("Login");
                break;
            case EPlatform.GameCenter:
                LoginWnd.Instance.Open();
                break;
        }
    }

    void OnDestroy()
    {
        if (RecordForBot)
        {
            using (FileStream stream = new FileStream(Application.dataPath + "\\commands.bytes", FileMode.Create))
            {
                ProtoBuf.Meta.RuntimeTypeModel.Default.Serialize(stream, mClient.RecordCommands);
                stream.Close();
            }
        }

        if (mClient != null)
            mClient.Close();
    }

    void Update()
    {
//		if(EmptyForLoadingWnd.Exist)
//		{
//			//Only in Levels ShowTimeCounter;
//			if( Global.CurSceneInfo.ID >= ESceneID.Level)
//			{
//				if( mProgressTime >0.0f )
//				{
//					mProgressTime -= (Time.realtimeSinceStartup - mStartTime);
//					string str = string.Format("{0:C}",mProgressTime).Substring(1,4);
//					EmptyForLoadingWnd.Instance.Progress(str);
//					Global.Pause = true;
//					//mCounter.GetComponent<UILabel>().text = str ;
//				}
//				else if (mProgressTime <= 0.0f)
//				{
//					Global.Pause = false;
//					//EmptyForLoadingWnd.Instance.Progress(null);
//					Global.ShowLoadingEnd();
//				}
//			}
//		}
		
        mClient.UpdateMessageQueue();
        GameEventManager.Instance.Update();
    }

    public void Request(object obj, Client.OnResponse callback)
    {
        StartCoroutine(mClient.Execute(obj, callback));
    }

    // helper function for there own execute.
    public static IEnumerator Execute(object obj, Client.OnResponse callback)
    {
        return msInstance.Client.Execute(obj, callback);
    }

    public bool LoadLevel(LoadingSceneInfo LSInfo)
    {
        LevelHelper helper = gameObject.AddComponent<LevelHelper>();
        helper.Load(LSInfo);
        return true;
    }

    //For Test
    void CompleteTransaction(string receipt)
    {
        MyDebugLog("RequestIAPBuyInfo" + receipt);
        StartCoroutine(MainScript.Execute(new RechargeCmd(receipt), delegate(string err, Response response)
        {
            Global.ShowLoadingEnd();
            if (!string.IsNullOrEmpty(err))
            {
                MyDebugLog("RequestIAPBuyInfo failed");
                return;
            }

            MainAttrib attrib = (response != null) ? response.Parse<MainAttrib>() : null;
            if (attrib != null)
            {
                Debug.Log("attrib.Gold=" + attrib.Gold);
                Debug.Log("attrib.Gem=" + attrib.Gem);

                PlayerDataManager.Instance.Attrib.Gold = attrib.Gold;
                PlayerDataManager.Instance.Attrib.Gem = attrib.Gem;
            }
			MessageBoxWnd.Instance.Show("BuyGem Succeed",MessageBoxWnd.StyleExt.Alpha);
            MyDebugLog("RequestIAPBuyInfo Succeessful");
            if (RechargeWnd.Exist)
                RechargeWnd.Instance.Update();
        }));
    }

    void FailedTransaction(string code)
    {
        MyDebugLog("FailedTransaction: " + code);
        Global.ShowLoadingEnd();
		MessageBoxWnd.Instance.Show("BuyGem Failed",MessageBoxWnd.StyleExt.Alpha);
    }



    /////////////
    public void PVPRequest(byte[] bytes)
    {
        StartCoroutine(mClient.SendPVP(bytes));
    }
}