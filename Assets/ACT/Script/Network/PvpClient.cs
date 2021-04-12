using UnityEngine;
using System.Collections;
using WebSocket4Net;
using System;
using System.Collections.Generic;

public class PvpClient : MonoBehaviour
{
    public GameObject[] SpawnPoints;
    
    const int PVP_RESULT_DRAW = 0; // 平局
    const int PVP_RESULT_WIN = 1; // pvp赢了
    const int PVP_RESULT_LOSE = 2; // 输了
    const float PVP_TIMEOUT = 30.0f; // 30 seconds.
	float mPvPTimeConter = 0.0f; // chixu shichang
    Unit mOwner;
    GameClient mGameClient;
	float mSceneReadyTime = 0;
	bool mGameStarted = false;
	bool mGameTimeCounter = true;
	bool mGameTimeCounterStart = true;

    public GameClient GameClient { get { return mGameClient; } }

    static PvpClient msInstance;
    public static PvpClient Instance { get { return msInstance; } }

    int GetPlayerIndex(PvpData data)
    {
        string userName = PlayerDataManager.Instance.Attrib.Name;
        for (int i = 0; i < data.users.Length; i++)
        {
            if (data.users[i].Attrib.Name == userName)
                return i;
        }
        return -1;
    }
	
	void SpawnInit()
	{
		if(SpawnPoints == null){
			GameObject[] goes  = {
				GameObject.Find("SpawnPoint01"),
				GameObject.Find("SpawnPoint02")
			};
			SpawnPoints = goes;
		}
	}
	
	void Awake()
	{
        msInstance = this;
		SpawnInit();
	}

    // Use this for initialization
    void Start()
    {
        // there is no data coming from.
        if (Global.GPvpData == null || Global.GPvpData.users == null)
        {
			Debug.LogError("PvpData is null");
            QuitPvpError();
            return;
        }
		
        // init the position for.
        mOwner = gameObject.GetComponent<UnitInfo>().Unit;

        // setup local player attribute, this is done in pvp balance.
        int playerIndex = GetPlayerIndex(Global.GPvpData);
        PlayerData playerData = Global.GPvpData.users[playerIndex];
        Player player = UnitManager.Instance.LocalPlayer as Player;
        player.PlayerData = playerData;
        player.UpdateAttributes();

        // setup player init position.
        if (playerIndex >= 0 && playerIndex < SpawnPoints.Length)
        {
			//Time.timeScale = 0;
            mOwner.SetPosition(SpawnPoints[playerIndex].transform.position);
            mOwner.SetOrientation(
                SpawnPoints[playerIndex].transform.rotation.eulerAngles.y * Mathf.Deg2Rad);
        }

        mGameClient = new GameClient(
            mOwner,
            playerIndex);

        mGameClient.Start(Global.GPvpData.server,
            Global.GPvpData.game,
            Global.GPvpData.pass);

        mGameClient.OnGameStarted = OnGameStarted;
        mGameClient.OnGameFinished = OnGameFinished;
        mGameClient.OnUnitJoinGame = OnUnitJoinGame;
		
		mSceneReadyTime = Time.realtimeSinceStartup;
    }

    void OnDestroy()
    {
        Global.GPvpData = null;
        msInstance = null;
        mGameClient.Destroy();
    }

    void QuitPvpError()
    {
		Debug.Log("QuitPvpError!");
		Global.ShowLoadingEnd();
        PvPWnd.RequestPvPFinishInfo(PVP_RESULT_LOSE);
    }
	

	// TimeOutCheck
	void TimeOutCheck()
	{
		mSceneReadyTime = 0;
		Debug.LogError("TimeOutCheck error!!!");
        QuitPvpError();
	}
	
    // Master should fire start message to games.
    void StartGame()
    {
        if (GameClient.IsMaster)
            GameClient.StartGame();
    }

    //新加暂停处理
    void OnApplicationPause(bool pause)
    {
        if (pause)
            OnGameTimeFinished();
    }

    void OnGameStarted()
    {
		Debug.Log("OnGameStarted");

        //TODO: game started.
        //Time.timeScale = 1;
		mGameStarted = true;
		Global.ShowLoadingEnd();
    }

    void OnGameTimeFinished()
    {
        // force the game finished.
        GameClient.MasterCheckGameFinished(true);
    }

    void OnGameFinished(bool win)
    {
		Debug.Log("OnGameFinished!");
        PvPWnd.RequestPvPFinishInfo(win ? PVP_RESULT_WIN : PVP_RESULT_LOSE);
    }

    // attach effect.
    void OnUnitJoinGame(Unit unit)
    {
        // setup the camp flag.
        if (unit.ServerId % 2 != mOwner.ServerId % 2)
            unit.UUnitInfo.Camp = EUnitCamp.EUC_ENEMY;

        // fire the start game message to all client.
        if (GameClient.IsMaster && GameClient.PlayerMap.Count >= Global.GPvpData.users.Length)
            StartGame();
    }
	
	void TimeCounterSoundFightPlay()
	{
		SoundManager.Instance.PlaySound("PVP_Fight");
	}
	
	void TimeConterReset()
	{
		mGameTimeCounter = true;
		mPvPTimeConter = 0.0f;
	}
	
	
    // Update is called once per frame
    void Update()
    {
		if (!mGameStarted)
		{
            if (mSceneReadyTime > 0 && Time.realtimeSinceStartup > mSceneReadyTime + PVP_TIMEOUT)
				TimeOutCheck();
		}
		
        // process the message queue.
		if(mGameClient != null)
        	mGameClient.UpdateMessageQueue();
		
//		if( mGameStarted && !mGameTimeCounterStart)
//		{	
//			mGameTimeCounterStart = true;
//			GameObject Go = GameObject.Find("Camera-CloseUp");
//			GameObject GoPvpStart = GameObject.Instantiate(Resources.Load("PVPStartLoading")) as GameObject;
//			GoPvpStart.transform.parent = Go.transform;
//			SoundManager.Instance.PlaySound("PVP_DaoJiShi");
//			Invoke("TimeCounterSoundFightPlay",3.0f);
//			Invoke("TimeConterReset",4.0f);
//		}
		
		if( FightMainWnd.Exist && mGameTimeCounter && mGameStarted )
		{
			mPvPTimeConter += Time.deltaTime;
			if( FightMainWnd.Instance.UpdatePvPTime( (int)Mathf.Floor(mPvPTimeConter)) )
			{
				OnGameTimeFinished();
			}
		}
    }
}