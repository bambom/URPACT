using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PvPWnd : Window<PvPWnd>
{
	public enum Pray
	{
		Invalid,
		Start,
		Prayed,
		Fights		
	}
	
    public override string PrefabName { get { return "PvPWnd"; } }
	
	GameObject mControlRoot;
	UILabel mBattleLabel;
	UILabel mNameLabel;
	UILabel mPvPLabel;
	UILabel mPlayerLevel;
	UILabel mCBattleLabel;
	UILabel mCNameLabel;
	UILabel mCPvPLabel;
	UILabel mCPlayerLevel;	
	GameObject mGoCharactorPanel;
	GameObject mGoChallengPanel;
	
	//GameObject mGoRankList;
	GameObject mGoReturn;
	GameObject mGoReady;
	GameObject mGoMatchLabel;
	GameObject mGoSpecialEffect;
	Vector3 mStartPos;
	
	bool mPvPStatus = false;
	
	static PvPResult mPvPResult;
	Pray mPray = Pray.Invalid;
	
    protected override bool OnOpen()
    {
		Init();
		WinStyle = WindowStyle.WS_Ext;
        return base.OnOpen();
    }

    protected override bool OnClose()
    {
		EquipChangeWnd.Instance.Close();
		return base.OnClose();
    }
	
	protected override bool OnHide ()
	{
		EquipChangeWnd.Instance.Hide();
		return base.OnHide ();
	}
	
	protected override bool OnShow ()
	{
		//UpdateChallengPanel();
		if(EquipChangeWnd.Exist)
			EquipChangeWnd.Instance.Show();
		else 
			EquipChangeWnd.Instance.Open();
		
		EquipChangeWnd.Instance.ResetPos(new Vector3(-240.0f,-170.0f,525.0f));
		return base.OnShow ();
	}
	
	private void Init()
	{
		mControlRoot = Control("ControlRoot");
		mGoCharactorPanel = Control ("CharactorPanel");
		mGoChallengPanel = Control ("ChallengPanel");
		
		mBattleLabel = Control("GS",mGoCharactorPanel).GetComponent<UILabel>();
		mNameLabel = Control("Name",mGoCharactorPanel).GetComponent<UILabel>();
		mPvPLabel = Control("PvP",mGoCharactorPanel).GetComponent<UILabel>();
		mPlayerLevel = Control("Level",mGoCharactorPanel).GetComponent<UILabel>();
		mCBattleLabel = Control("GS",mGoChallengPanel).GetComponent<UILabel>();
		mCNameLabel = Control("Name",mGoChallengPanel).GetComponent<UILabel>();
		mCPvPLabel = Control("PvP",mGoChallengPanel).GetComponent<UILabel>();
		mCPlayerLevel = Control("Level",mGoChallengPanel).GetComponent<UILabel>();
		
		mGoSpecialEffect = Control ("SpecialEffect");
		mGoSpecialEffect.SetActive(false);
		
		//mGoRankList = Control("Rank");
		mGoReturn = Control("Return");
		mGoReady = Control("Ready");
		
		//UIEventListener.Get(mGoRankList).onClick = OnClickRankList;
		UIEventListener.Get(mGoReady).onClick = OnClickReadyToFight;
		UIEventListener.Get(mGoReturn).onClick = OnClickReturn;
		UIEventListener.Get(Control("Pray")).onClick = OnClickPray;
		//UIEventListener.Get(Control("ChallengeAdd")).onClick = OnClickChallenge;
		mGoMatchLabel = Control ("MatchLabel");
		mStartPos = mGoMatchLabel.transform.localPosition;
		mGoMatchLabel.SetActive(false);
		
		CharactorModelInit();
		CharactorInfoInit();
		
		InitChallengPanel();
		
		UpdatePray(Pray.Prayed);
		UpdateReady(Control("Ready"),mPvPStatus);
		
		RegisterPvPHandle();
		RequestPvPInfo();
	}
	
	void UpdateCharactorInfo()
	{
		CharactorInfoInit();
	}
	
	void CharactorInfoInit()
	{
		MainAttrib  Attrib = PlayerDataManager.Instance.Attrib;
		mBattleLabel.text = Attrib.Battle.ToString();
		mNameLabel.text = Attrib.Name;
		
		PvpBase PvPBM = PvpBaseManager.Instance.GetItem(Attrib.PvpLevel);
		mPvPLabel.text = PvPBM.ArenaTitle;
		mPlayerLevel.text = Attrib.Level.ToString();
	}
	
	
	void RegisterPvPHandle()
    {
        MainScript.Instance.Client.RegisterHandler((int)Client.ESystemRequest.UserPvpBegin, OnUserPvpBegin);
		MainScript.Instance.Client.RegisterHandler((int)Client.ESystemRequest.UserPvpGameStart, OnUserPvpGameStart);
		MainScript.Instance.Client.RegisterHandler((int)Client.ESystemRequest.UserPvpGameEnd, OnUserPvpGameEnd);
    }
	
    void OnDestroy()
    {
        MainScript.Instance.Client.UnRegisterHandler((int)Client.ESystemRequest.UserPvpBegin, OnUserPvpBegin);
		MainScript.Instance.Client.UnRegisterHandler((int)Client.ESystemRequest.UserPvpGameStart, OnUserPvpGameStart);
		MainScript.Instance.Client.UnRegisterHandler((int)Client.ESystemRequest.UserPvpGameEnd, OnUserPvpGameEnd);
    }
	
	void OnClickChallenge(GameObject go)
	{
		
	}
	
	void OnClickPray(GameObject go)
	{
		if( mPray == Pray.Prayed )
			return ;
		UpdatePray(Pray.Prayed);
	}
	
	void OnClickRankList(GameObject go)
	{
		Close();		
//		if(EquipChangeWnd.Exist)
//			EquipChangeWnd.Instance.Close();
		if(TopListWnd.Exist)
			TopListWnd.Instance.Show();
		else
			TopListWnd.Instance.Open();
	}
	
	
	void RemoveAnimation(GameObject go)
	{
		TweenPosition Tp = go.GetComponent<TweenPosition>();
		if( Tp != null){
			GameObject.Destroy(Tp);
			Tp = null;
		}
		go.transform.localPosition = mStartPos;
	}
	
	void LinearAnimation(GameObject go,TweenPosition.OnFinished OnFinish)
	{
		if( !mGoSpecialEffect.activeSelf )
			mGoSpecialEffect.SetActive(true);
		
		RemoveAnimation(go);
		TweenPosition Tp = go.AddComponent<TweenPosition>();
		Tp.method = UITweener.Method.Linear;
		Tp.style = UITweener.Style.Once;
		Tp.duration = 4.0f;
		Tp.from = new Vector3(445.0f,-40.0f,0.0f);//go.transform.localPosition;
		Tp.to = new Vector3(445.0f,-40.0f,0.0f);//go.transform.localPosition;//new Vector3(240.0f,-170.0f,0.0f);
		Tp.onFinished = OnFinish;
	}
	
	void PinpPongAnimation(GameObject go)
	{
		RemoveAnimation(go);
		TweenPosition Tp = go.AddComponent<TweenPosition>();
		Tp.from = go.transform.localPosition + new Vector3(-100.0f,0.0f,0.0f);;
		Tp.to = new Vector3(Tp.from.x + 200.0f,Tp.from.y,Tp.from.z);
		Tp.method = UITweener.Method.Linear;
		Tp.style = UITweener.Style.PingPong;
	}
	
	void UpdatePray(Pray praySta)
	{
		GameObject goPray  = Control ("Pray");
		GameObject[] goPrayChilds = {
			goPray.transform.Find("IconFight").gameObject,
			goPray.transform.Find("IconPray").gameObject,
			goPray.transform.Find("Gem").gameObject,
			goPray.transform.Find("GemLabel").gameObject,
			goPray.transform.Find("PrayLabel").gameObject
		};
		mPray = praySta;
		switch(praySta)
		{
			case Pray.Start:
				{
					goPrayChilds[0].SetActive(false);
					goPrayChilds[1].SetActive(false);
					goPrayChilds[2].SetActive(true);
					goPrayChilds[3].SetActive(true);
					goPrayChilds[4].SetActive(true);
				}
				break;
			case Pray.Prayed:
				{
					goPrayChilds[0].SetActive(false);
					goPrayChilds[1].SetActive(true);
					goPrayChilds[2].SetActive(false);
					goPrayChilds[3].SetActive(false);
					goPrayChilds[4].SetActive(false);
				}
				break;
			case Pray.Fights:
				{
					goPrayChilds[0].SetActive(true);
					goPrayChilds[1].SetActive(false);
					goPrayChilds[2].SetActive(false);
					goPrayChilds[3].SetActive(false);
					goPrayChilds[4].SetActive(false);
				}
				break;
		}
	}
	
	
	void UpdateReady(GameObject BtnGo,bool Status)
	{
		GameObject go = mGoMatchLabel;
		if(!Status){
			mGoMatchLabel.SetActive(false);
			RemoveAnimation(go);
			BtnGo.transform.Find("Cancle").gameObject.SetActive(false);
			BtnGo.transform.Find("Label").GetComponent<UILabel>().text = "自动匹配";
		}else{
			mGoMatchLabel.SetActive(true);
			PinpPongAnimation(go);
			BtnGo.transform.Find("Cancle").gameObject.SetActive(true);
			BtnGo.transform.Find("Label").GetComponent<UILabel>().text = "取消";
		}
	}
	
	void OnClickReadyToFight(GameObject go)
	{
		mPvPStatus = !mPvPStatus;
		UpdateReady(go,mPvPStatus);
		
		if(mPvPStatus){
			Debug.Log ("RequestJoinPvP");
			BtnUpdate(false);
	      	Request(new JoinPvpCmd(), delegate(string err, Response response)
	        {
				mGoReady.SetActive(true);
	            if (!string.IsNullOrEmpty(err))
	            {
					MessageBoxWnd.Instance.Show(err);
	                return;
	            }	
				Debug.Log ("RequestJoinPvP Suceess");
	        });	
		}else{
			BtnUpdate(false);
	      	Request(new LeavePvpCmd(), delegate(string err, Response response)
	        {
				BtnUpdate(true);
	            if (!string.IsNullOrEmpty(err))
	            {
					MessageBoxWnd.Instance.Show(err);
	                return;
	            }	
				Debug.Log ("LeavePvpCmd Suceess");
	        });	
		}
		
	}
	
	void OnClickReturn(GameObject go)
	{
		//match status
		if(mPvPStatus){
	      	Request(new LeavePvpCmd(), delegate(string err, Response response)
	        {
	            if (!string.IsNullOrEmpty(err))
	            {
					MessageBoxWnd.Instance.Show(err);
	                return;
	            }	
				Debug.Log ("LeavePvpCmd Suceess");
	        });	
		}
		OnDestroy();
		Close();
		if(InGameMainWnd.Exist)
			InGameMainWnd.Instance.Show();
		else
			InGameMainWnd.Instance.Open();
	}
	
	
	void CharactorModelInit()
	{
		if( !EquipChangeWnd.Exist )
			EquipChangeWnd.Instance.Open();
		//Control ("CharactorCollider").AddComponent<PressDrag>().target = Control ("CharactorCollider").transform;
		PressDrag PDScript = Control ("CharactorCollider",mGoCharactorPanel).GetComponent<PressDrag>();
		
		if(null != PDScript){
			PDScript.target = EquipChangeWnd.Instance.CharactorRoot.transform;
		}else{
			PDScript = Control ("CharactorCollider",mGoCharactorPanel).AddComponent<PressDrag>();
			PDScript.target = EquipChangeWnd.Instance.CharactorRoot.transform;
		}
		EquipChangeWnd.Instance.ResetPos(new Vector3(-240.0f,-200.0f,525.0f));
		//UIEventListener.Get( Control ("CharactorCollider") ).onClick = OnClickCharactor;
	}
	
	void EnterPvPScene(UITweener tween )
	{
		if(Exist)
			Close();
		
		if(InGameMainWnd.Exist)
			InGameMainWnd.Instance.Close();
		
		SceneList SLItem = SceneListManager.Instance.GetItem((int)ESceneID.PVP,(int)ESceneType.PVP);
		
		Global.CurSceneInfo.ID = ESceneID.PVP;
		Global.CurSceneInfo.Type = ESceneType.PVP;
		Global.CurSceneInfo.Name = SLItem.Name;
		
		MainScript.Instance.LoadLevel(Global.CurSceneInfo);
		
	}
	
	int SetAgainstPlayerAttrib()
	{
		MainAttrib Attrib;
		for(int i = 0;i<Global.GPvpData.users.Length;i++){
			Attrib = Global.GPvpData.users[i].Attrib;
			if( string.Compare(Attrib.Name,
					PlayerDataManager.Instance.Attrib.Name) != 0){
				mCBattleLabel.text = Attrib.Battle.ToString();
				mCNameLabel.text = Attrib.Name;
				PvpBase PvPBM = PvpBaseManager.Instance.GetItem(Attrib.PvpLevel);
				mCPvPLabel.text = PvPBM.ArenaTitle;
				return i;
			}
		}
		return 0;
	}
		
	void InitChallengPanel()
	{
		if( mGoChallengPanel.activeSelf )
			mGoChallengPanel.SetActive(false);
	}
	
	
	void UpdateChallengInfo(int index)
	{
		MainAttrib Attrib = Global.GPvpData.users[index].Attrib;
		PvpBase PvPBM = PvpBaseManager.Instance.GetItem(Attrib.PvpLevel);
		
		mCBattleLabel.text = Attrib.Battle.ToString();
		mCNameLabel.text = Attrib.Name;
		mCPvPLabel.text = PvPBM.ArenaTitle;
		mCPlayerLevel.text = Attrib.Level.ToString();	
	}
	
	
	void BtnUpdate(bool bStatus)
	{
		//mGoRankList.SetActive(bStatus);
		mGoReturn.SetActive(bStatus);
		mGoReady.SetActive(bStatus);
	}
	
	void UpdateChallengPanel()
	{
        Debug.Log("UpdateChallengPanel");

        if (!mGoChallengPanel)
            return;
		
		if(Global.GPvpData == null){
			if (mGoChallengPanel.activeSelf)
				mGoChallengPanel.SetActive(false);
			return;
		}
		
        if (!mGoChallengPanel.activeSelf)
            mGoChallengPanel.SetActive(true);

		BtnUpdate(false);
		
        int index = SetAgainstPlayerAttrib();
        if (EquipChangeWnd.Exist)
        {
            Debug.Log("UpdateChallengInfo");
            UpdateChallengInfo(index);
            EquipChangeWnd.Instance.CreateOtherPlayer(Global.GPvpData.users[index]);
            EquipChangeWnd.Instance.OtherResetPos(new Vector3(240.0f, -200.0f, 525.0f));
            PressDrag PDScript = Control("CharactorCollider", mGoChallengPanel).GetComponent<PressDrag>();
            if (null != PDScript)
            {
                PDScript.target = EquipChangeWnd.Instance.OtherCharactorRoot.transform;
            }
            else
            {
                PDScript = Control("CharactorCollider",mGoChallengPanel).AddComponent<PressDrag>();
                PDScript.target = EquipChangeWnd.Instance.OtherCharactorRoot.transform;
            }
			
            LinearAnimation(mGoChallengPanel, EnterPvPScene);
        }
	}
	
	void UpdateUserPvPInfo(PvpInfo pvpInfo)
	{
		GameObject goPvPResult = Control("PvPResult");
		Control("KeepWin").GetComponent<UILabel>().text = pvpInfo.KeepWin.ToString();
		Control("PvpExp").GetComponent<UILabel>().text = pvpInfo.PvpExp.ToString();
		Control("WinCount").GetComponent<UILabel>().text = pvpInfo.WinCount.ToString();
		//Control("ChallengeCount").GetComponent<UILabel>().text = "(1/5)";	
	}
	
	static void UpdateUserPvPFinishData(PvpFinish.PvpFinishData[] PvpData)
	{
		for(int i = 0;i<PvpData.Length;i++){
			if( 0 == string.Compare(PvpData[i].user,PlayerDataManager.Instance.Attrib.Name) ){
				mPvPResult = new PvPResult(PvpData[i]);
				
				if(FightMainWnd.Exist)
					FightMainWnd.Instance.Close();
				return;
			}
		}
	}
	
	void OnUserPvpBegin(string err, Response response)
	{
        Global.GPvpData = (response != null) ? response.Parse<PvpData>() : null;
        if (Global.GPvpData == null)
            return;
		
		UpdateChallengPanel();
		
	}
	
	void OnUserPvpGameStart(string err, Response response)
	{
        PvpData pvpData = (response != null) ? response.Parse<PvpData>() : null;
        if (pvpData == null)
            return;
		Debug.Log("OnUserPvpGameStart");
	}
	
	void OnUserPvpGameEnd(string err, Response response)
	{
        PvpData pvpData = (response != null) ? response.Parse<PvpData>() : null;
        if (pvpData == null)
            return;
		Debug.Log("OnUserPvpGameEnd");
	}
	
	void RequestJoinPvP()
	{
		Debug.Log ("RequestJoinPvP");
      	Request(new JoinPvpCmd(), delegate(string err, Response response)
        {
            if (!string.IsNullOrEmpty(err))
            {
				MessageBoxWnd.Instance.Show(err);
                return;
            }	
			Debug.Log ("RequestJoinPvP Suceess");
        });
	}
	
	void RequestPvPInfo()
	{
		Debug.Log ("RequestPvPInfo");
        Request(new GetPvpInfoCmd(), delegate(string err, Response response)
        {
            if (!string.IsNullOrEmpty(err))
            {
				MessageBoxWnd.Instance.Show(err);
                return;
            }

			PvpInfo pvpInfo = (response != null) ? response.Parse<PvpInfo>() : null;
			UpdateUserPvPInfo(pvpInfo);
			Debug.Log ("RequestPvPInfo Suceess");
        });
	}
	
	public static void RequestPvPFinishInfo(int result)
	{
		Debug.Log ("RequestPvPFinishInfo");
        MainScript.Instance.Request(new FinishPvpCmd(result), delegate(string err, Response response)
        {
            if (!string.IsNullOrEmpty(err))
            {
				MessageBoxWnd.Instance.Show(err);
                return;
            }

			PvpFinish pvpData = (response != null) ? response.Parse<PvpFinish>() : null;
			UpdateUserPvPFinishData(pvpData.datas);
			Debug.Log ("RequestPvPFinishInfo Suceess");
        });
	}
}

