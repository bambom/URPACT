using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LevelHelper : MonoBehaviour
{
	AsyncOperation mAsync;	
	GameObject mGoLocalPlayer;
	
	
	static Dictionary<int, DateTime> mEntryLevelTime = new Dictionary<int, DateTime>();
	public static Dictionary<int, DateTime> EntryLevelTime{ get{ return mEntryLevelTime; } }

    public void Load(LoadingSceneInfo LSceneInfo)
    {
        SoundManager.Instance.UnloadRuntimeClips();
        ActionManager.Instance.Clear();
        MainScript.Instance.Client.Clear();

		if( !LoadingWnd.Exist )
			LoadingWnd.Instance.Open();
			
		Debug.Log("LoadLevelAsync :" + LSceneInfo.Name);
        StartCoroutine("LoadLevelAsync", LSceneInfo);
    }

    IEnumerator LoadLevelAsync(LoadingSceneInfo LSceneInfo)
    {
		mAsync = Application.LoadLevelAsync(LSceneInfo.Name);
		yield return mAsync;

		if(LoadingWnd.Exist)
			LoadingWnd.Instance.Close();
		// NGUIDebug.Log("LoadLevelAsync  " + LSceneInfo.Name);
        // on load finished.
        OnLoadFinished(LSceneInfo);
        // remove myself.
        Destroy(this);
    }

    void OnLoadFinished(LoadingSceneInfo LSceneInfo)
    {
		CreateLocalPlayer();
		if(LSceneInfo.Type == ESceneType.Main){
			BuildCity();
		}else if(LSceneInfo.Type == ESceneType.Level
			|| LSceneInfo.Type == ESceneType.SuperLevel){
       		if (Global.GLevelData != null)
           		BuildLevel(Global.GLevelData);
		}else if(LSceneInfo.Type == ESceneType.PVP){
			BuildPvP();
		}else if(LSceneInfo.Type == ESceneType.CreateRole){
			BuildCreateRole(LSceneInfo.Data);
		}else if(LSceneInfo.Type == ESceneType.GameGuide){
			BuildGameGuide();
		}
    }
	
	void BuildGameGuide()
	{
		
	}
	
	void BuildCreateRole(int Data)
	{
		Debug.Log("BuildCreateRole" + Data);
		switch(Data){
			case 0: MainScript.Instance.ConfigPlatform();break;
			case 1:
				{
				    if (!RoleChooseWnd.Exist){
						RoleChooseWnd.Instance.Open();
				    	RoleChooseWnd.Instance.OnRoleList(LoginHelper.roleList);
					}
				}
				break;		
			default:break;
		}
	}
	
	void BuildPvP()
	{
		Global.ShowLoadingStart();
		if (PvPWnd.Exist)
			PvPWnd.Instance.Close();
		if (!FightMainWnd.Exist)
			FightMainWnd.Instance.Open();
		else
			FightMainWnd.Instance.Show();
		
		if (mGoLocalPlayer != null)
		{
			if (mGoLocalPlayer.GetComponent<PvpClient>() == null)
				mGoLocalPlayer.AddComponent<PvpClient>();
			
			if (mGoLocalPlayer.GetComponent<PlayerListener>() == null)
				mGoLocalPlayer.AddComponent<PlayerListener>();
		}
	}
	
	void BuildCity()
	{
	    GameObject cityObject = new GameObject("CityLevel");
	    cityObject.AddComponent<CityLevel>();
	}
	
	
	void Update()
	{
		if(mAsync != null && LoadingWnd.Exist)
		{
			LoadingWnd.Instance.UpdateProgress(mAsync.progress);
		}
	}
	
    LevelBuildInfo BuildInfo(LevelData levelData)
    {
        LevelBuildInfo buildInfo = new LevelBuildInfo();
        buildInfo.WayLength = levelData.Info.WayLength;
        buildInfo.ExtraWay = levelData.Info.ExtraWay;
        buildInfo.GeniusNum = levelData.Info.GeniusNum;
        buildInfo.ExtraNum = levelData.Info.ExtraNum;

        if (Global.GLevelSetup != null)
        {
            //buildInfo.CellLength = Global.GLevelSetup.CellLength;
            //buildInfo.BridgeWidth = Global.GLevelSetup.BridgeWidth;
            //buildInfo.BridgeLength = Global.GLevelSetup.BridgeLength;
        }

        buildInfo.MonsterNormal1 = levelData.Info.Normal1[0];
        buildInfo.M1level = levelData.Info.Normal1[1];
        buildInfo.Pool1Num = levelData.Info.Normal1[2];

        buildInfo.MonsterNormal2 = levelData.Info.Normal2[0];
        buildInfo.M2level = levelData.Info.Normal2[1];
        buildInfo.Pool2Num = levelData.Info.Normal2[2];

        buildInfo.MonsterGenius = levelData.Info.Genius[0];
        buildInfo.M3level = levelData.Info.Genius[1];
        buildInfo.Pool3Num = levelData.Info.Genius[2];

        buildInfo.MonsterBoss = levelData.Info.Boss[0];
        buildInfo.M4level = levelData.Info.Boss[1];
        buildInfo.Pool4Num = levelData.Info.Boss[2];

        buildInfo.MonsterExtra = levelData.Info.Extra[0];
        buildInfo.M5level = levelData.Info.Extra[1];
        buildInfo.Pool5Num = levelData.Info.Extra[2];

        buildInfo.NormalTrigger = levelData.Info.Triggers[0];
        buildInfo.GeniusTrigger = levelData.Info.Triggers[1];
        buildInfo.BossTrigger = levelData.Info.Triggers[2];
        buildInfo.ExtraTrigger = levelData.Info.Triggers[3];

        // chest data builder.
        if (levelData.Info.Chest != null && levelData.Info.Chest.Length == 3)
        {
            buildInfo.ChestID = levelData.Info.Chest[0];
            buildInfo.ChestLevel = levelData.Info.Chest[1];
            buildInfo.ChestNum = levelData.Info.Chest[2];
        }

        return buildInfo;
    }

    void BuildLevel(LevelData levelData)
    {
        LevelBuildInfo buildInfo = BuildInfo(levelData);
		
        GameObject level = new GameObject("GameLevel");
        GameLevel gameLevel = level.AddComponent<GameLevel>();
        LevelBuilder builder = new LevelBuilder();
        builder.Build(gameLevel, buildInfo);
		
		UnitManager.Instance.LocalPlayer.CurrentGameLevel = gameLevel;
    }

    void CreateLocalPlayer()
    {
        GameObject localPlayer = GameObject.Find("Warrior");
		if (localPlayer == null || PlayerDataManager.Instance.Data == null)
			return;

        UnitInfo unitInfo = localPlayer.GetComponent<UnitInfo>();
		int playerRole = PlayerDataManager.Instance.Attrib.Role;
        UnitBase palyerBase = UnitBaseManager.Instance.GetItem(playerRole);
        if (palyerBase == null)
            return;

        GameObject playerPrefab = Resources.Load(palyerBase.Prefab) as GameObject;
        if (playerPrefab == null)
            return;

        Vector3 pos = localPlayer.transform.position;
        Quaternion rot = localPlayer.transform.rotation;
        Controller sourceController = localPlayer.GetComponent<Controller>();
        Transform cameraTag = sourceController.CameraTag;
        Vector3 cameraPos = sourceController.CameraPos;
        Vector3 cameraLookAtOffset = sourceController.CameraLookAtOffset;
        if (unitInfo && unitInfo.Unit != null)
            unitInfo.Unit.Destory();
        else
            GameObject.Destroy(localPlayer);

        GameObject player = GameObject.Instantiate(playerPrefab, pos, rot) as GameObject;
        Controller destController = player.GetComponent<Controller>();
        destController.CameraTag = cameraTag;
        destController.CameraPos = cameraPos;
        destController.CameraLookAtOffset = cameraLookAtOffset;
		mGoLocalPlayer = player;
    }


	#region LevelSomething
	public static void LevelReset()
	{
		if(mEntryLevelTime.Count > 0)
			mEntryLevelTime.Clear();
	}
	
	public static void LevelStart(DateTime Dt)
	{
		if(mEntryLevelTime.Count > 0)
			mEntryLevelTime.Clear();
		
		int nCount = mEntryLevelTime.Count;
		mEntryLevelTime.Add(nCount,Dt);
	}
	
	public static void LevelPause(bool bStatus)
	{
		UnitManager.Instance.LocalPlayer.UGameObject.SendMessage("LockInput",bStatus.ToString());
		LevelTimeTrack(System.DateTime.Now);
		Global.Pause = bStatus;
	}
	
	public static void LevelContinue(bool bStatus)
	{
		UnitManager.Instance.LocalPlayer.UGameObject.SendMessage("LockInput",bStatus.ToString());
		UnitManager.Instance.LocalPlayer.PlayAction(Data.CommonAction.Idle);
		LevelTimeTrack(System.DateTime.Now);
		Global.Pause = bStatus;
	}
	
	public static void LevelTimeTrack(DateTime Dt)
	{
		int nCount = mEntryLevelTime.Count;
		mEntryLevelTime.Add(nCount,Dt);
	}
	#endregion LevelSomething
	
    public static void ParseGradeInfo(int levelId,int score,out string LevelAssess)
	{
		InstanceEvaluate insEvaluates = InstanceEvaluateManager.Instance.GetItem(levelId);
		string[] arrStrGradeInfo = {insEvaluates.S_Grade,insEvaluates.A_Grade,insEvaluates.B_Grade,
			insEvaluates.C_Grade,insEvaluates.D_Grade};
		int[] arrIntGradeInfo = new int [ arrStrGradeInfo.Length *2 ];
		string[] tempArrStr = new string [2];
		for(int i =0;i<arrStrGradeInfo.Length;i++)
		{
			tempArrStr = arrStrGradeInfo[i].Split('|');
			arrIntGradeInfo[i*2] = Convert.ToInt32(tempArrStr[0]);
			arrIntGradeInfo[i*2+1] = Convert.ToInt32(tempArrStr[1]);
		}
		
		int tempCase = -1;
		for(int j = 0;j<arrIntGradeInfo.Length;j=j+2)
		{
			if(score >= arrIntGradeInfo[j])
			{
				tempCase = j;
				break;
			}
			else
			{
				continue;
			}
		}
		
		switch(tempCase)
		{
			case 0:
				LevelAssess = CommonData.GradeLevelS;
				break;
			case 2:
				LevelAssess = CommonData.GradeLevelA;
				break;
			case 4:
				LevelAssess = CommonData.GradeLevelB;
				break;
			case 6:
				LevelAssess = CommonData.GradeLevelC;
				break;
			case 8:
				LevelAssess = CommonData.GradeLevelD;
				break;
			default:
				LevelAssess = "";
				Debug.LogError("Get Level Value Error");
				break;
		}
	}
}
