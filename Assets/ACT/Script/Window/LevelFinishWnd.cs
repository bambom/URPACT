using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class LevelFinishWnd : Window<LevelFinishWnd>
{
    public override string PrefabName { get { return "LevelFinishWnd"; } }
	
	GameObject mLevelUpPrompt;
	GameObject mEvaluationBase;
	FinishLevelControl mFinishLevelControl;
	UILabel mLabelGold;
	UILabel mLabelTime;
	InstanceEvaluate mInsEvaluates;
	string mStrGradeTex;
	bool mIsLevelup = false;
	
	UIGuideShow mUIGuideShow = null;
    protected override bool OnOpen()
    {
		Init();
		
		mUIGuideShow = new UIGuideShow(PrefabName);
		if(mUIGuideShow.GuideCheck()){
			mUIGuideShow.GuideStart();
		}
        return base.OnOpen();
    }
	
	void Init()
	{
		mLevelUpPrompt = Control("BtnReturn").transform.parent.Find("LevelUpPrompt").gameObject;
		mEvaluationBase= Control("BgRoot").transform.Find("EvaluationBaseObj").gameObject;
        UIEventListener.Get(Control("BtnReturn")).onClick = OnClickBacktoMainTown;
		mLabelGold = Control("gold",Control("Labels")).GetComponentInChildren<UILabel>();
		mLabelTime = Control("time",Control("Labels")).GetComponentInChildren<UILabel>();
		CreateDropItemGrid();
		
		LevelHelper.LevelPause(false);
		mLabelTime.text = GetPassLevelTimeSpan();
		LevelHelper.LevelReset();

		SetGrade();
		JudgeLevelUp();
		ControlTimeDelay(true);
		UpdateLevelProfit();
		SetProfit();			
	    //SendFinishLevelMsg();
	}
	
    public void OnClickBacktoMainTown(GameObject go)
    {
		if( mUIGuideShow != null ){
			mUIGuideShow.Destroy();
			mUIGuideShow = null;
		}
		SendFinishLevelMsg();
    }
	
	void OnClickReLogin( GameObject go )
	{
		TurnToTurntable();
	}
	
	void SendFinishLevelMsg()
	{		
		UnitManager.Instance.LocalPlayer.UGameObject.SendMessage("LockInput","false");
        // 生成副本结算请求数据。
        FinishLevelCmd cmd = new FinishLevelCmd();
        cmd.score = GameLevel.Instance.CurScore;
        cmd.gold = GameLevel.Instance.CurGold;
        cmd.exp = GameLevel.Instance.CurExp;
		Debug.Log(cmd.score);
		
        if (GameLevel.Instance.DropInfo != null)
        {
            int dropIndex = 0;
            cmd.drops = new FinishLevelCmd.PickData[GameLevel.Instance.DropInfo.Keys.Count];
            foreach (KeyValuePair<int, int> pair in GameLevel.Instance.DropInfo)
            {
                cmd.drops[dropIndex] = new FinishLevelCmd.PickData();
                cmd.drops[dropIndex].id = pair.Key;
                cmd.drops[dropIndex].num = pair.Value;
                dropIndex++;
            }
        }
		Global.ShowLoadingStart();
        Request(cmd, delegate(string err, Response response)
        {
            if (!string.IsNullOrEmpty(err) || response == null)
            {
				Global.ShowLoadingEnd();
                MessageBoxWnd.Instance.Show("Fail to Finished Level: " + err,MessageBoxWnd.Style.OK);
				MessageBoxWnd.Instance.OnClickedOk = OnClickReLogin;
                return;
            }
			Global.ShowLoadingEnd();
			RequestAttribInfo();
        });
	}
	
	void RequestAttribInfo()
	{
		MainScript.Instance.Request(new GetUserAttribCmd(), delegate(string err, Response response)
        {
            // update the user attribute.
            MainAttrib attrib = (response != null) ? response.Parse<MainAttrib>() : null;
            if (attrib == null)
			{
				MessageBoxWnd.Instance.Show("Fail to Update Attrib",MessageBoxWnd.StyleExt.Alpha);
				return;
			}
            // build new progress.
            if (attrib.Progress > PlayerDataManager.Instance.Attrib.Progress)
                FacebookHelper.PublishScore(attrib.Progress);

            PlayerDataManager.Instance.OnMainAttrib(attrib);
			RequestPackInfo();
        });
	}
	
	void RequestPackInfo()
	{
		MainScript.Instance.Request(new GetBackPackCmd(), delegate(string err, Response response)
        {
            if (!string.IsNullOrEmpty(err))
            {
				MessageBoxWnd.Instance.Show("Fail to Update Pack: " + err,MessageBoxWnd.StyleExt.Alpha);
                return;
            }
            // update the user 
            Package BackPackData = (response != null) ? response.Parse<Package>() : null;
            if (BackPackData != null){
				PlayerDataManager.Instance.OnBackPack(BackPackData, false);
			}
			TurnToTurntable();
        });
	}
	
	void JudgeLevelUp()
	{
		int curLevel = (int)PlayerDataManager.Instance.Attrib.Level;
		Debug.Log("curlevel:"+curLevel);
		if(curLevel == CommonData.UserMaxLevel)
			return;
		int curExp = (int)PlayerDataManager.Instance.Attrib.CurExp;
		int nextExp= (int)PlayerDataManager.Instance.Attrib.NextExp;		
        if(curExp + GameLevel.Instance.CurExp >= nextExp)
			mIsLevelup = true;
	}
	
	void TweenFinish(UITweener tween)
	{
		SoundManager.Instance.PlaySound("UI_LevelUp");
	}
	
	string GetPassLevelTimeSpan()
	{			
		TimeSpan timeSpan =  TimeSpan.Zero;
		TimeSpan sumTs =  TimeSpan.Zero;
		
		for(int i=0; i < LevelHelper.EntryLevelTime.Count; i=i+2)
		{
			DateTime tempStartTime = LevelHelper.EntryLevelTime[i];
			DateTime tempEndTime = LevelHelper.EntryLevelTime[i+1];
			timeSpan = tempEndTime.Subtract( tempStartTime );
			sumTs += timeSpan;
		}
		return sumTs.Minutes.ToString() +": " + sumTs.Seconds.ToString();	
	}
		
	void CreateDropItemGrid()
	{
        Dictionary<int, int> DicDropItem = GameLevel.Instance.DropInfo;
		if( DicDropItem == null){
			return;
		}
	
		List<int> listKey = new List<int>(DicDropItem.Keys); 
		int DropItemID = 0;
		string DropItemName = "";
		string IconName = "";
		int DropItemCount = 0;
		GameObject goDropItem ;
		ItemBase itemBase;
		for( int i=0;i<DicDropItem.Count;i++ ){
			DropItemID = listKey[i];
			itemBase =  ItemBaseManager.Instance.GetItem(DropItemID);
			DropItemName = itemBase.Name;
			IconName =  itemBase.Icon;
			DropItemCount = DicDropItem[ DropItemID ];

			goDropItem = Control("GridContentDropItem"+i.ToString(),Control("UIGridPanel"));
			Control("DropLabel",goDropItem).GetComponent<UILabel>().text = DropItemName;
			Control("DropTex",goDropItem).GetComponent<UISprite>().spriteName = IconName;
			Control("Count",Control("Label",goDropItem)).GetComponent<UILabel>().text = DropItemCount.ToString();
			UpdateItemQualityFrameIcon(Control("DropQuality",goDropItem),itemBase);
		}
		
		///隐藏其他GridItem
		for(int j = CommonData.DropItemMax; j>=DicDropItem.Count; j--){
			goDropItem = Control("GridContentDropItem"+j.ToString(),Control("UIGridPanel"));
			goDropItem.SetActive(false);
		}
	}
	
	void SetGrade()
	{
		string strGradeTex;
		int levelID = Global.GLevelData.Info.ID;
		mInsEvaluates = InstanceEvaluateManager.Instance.GetItem(levelID);
		
		int minusScore = (int)mInsEvaluates.M2_Point * UnitManager.Instance.LocalPlayer.HitCount;
		minusScore = minusScore > mInsEvaluates.M2_Point_Limit ? (int)mInsEvaluates.M2_Point_Limit :minusScore;
		int Score = mInsEvaluates.Base_Point - minusScore;
		
		LevelHelper.ParseGradeInfo(levelID,Score,out strGradeTex);
		mStrGradeTex = strGradeTex;
		GameLevel.Instance.CurScore = Score;
		Control("Evaluation").GetComponent<UISprite>().spriteName = mStrGradeTex;
	}
	
	void ControlTimeDelay(bool isSetGrade)
	{		
		mFinishLevelControl = WndObject.GetComponent<FinishLevelControl>();
		if(mFinishLevelControl == null)
			mFinishLevelControl = WndObject.AddComponent<FinishLevelControl>();
		if(isSetGrade)
		{
			//float duration = Control("EvaluationObj").GetComponent<Animation>().clip.length;
			mFinishLevelControl.WaitForTime(0.1f,ShowEvaBase);
		}
		else
		{
			TweenScale tweenScale = mLevelUpPrompt.GetComponentInChildren<TweenScale>();
			float durationTime = tweenScale.duration + 0.5f;
			mFinishLevelControl.WaitForTime(durationTime,PromptMoveOut);
		}
	}
	
	void UpdateLevelProfit()
	{
		int percent = 0;
		switch(mStrGradeTex)
		{
			case CommonData.GradeLevelS:
				percent = 100;
				break;
			case CommonData.GradeLevelA:
				percent = 90;
				break;
			case CommonData.GradeLevelB:
				percent = 80;
				break;
			case CommonData.GradeLevelC:
				percent = 70;
				break;
			case CommonData.GradeLevelD:
				percent = 60;
				break;
			default:
				Debug.LogError("Get Level Value Error");
				break;
		}
		
	    GameLevel.Instance.CurExp = GameLevel.Instance.CurExp * Global.GLevelSetup.KillWeight / 100+
			GameLevel.Instance.CurExp * percent * Global.GLevelSetup.EvaluateWeight/10000;
	    GameLevel.Instance.CurGold = GameLevel.Instance.CurGold * Global.GLevelSetup.KillWeight / 100+
			GameLevel.Instance.CurGold * percent * Global.GLevelSetup.EvaluateWeight/10000;
		
		GameLevel.Instance.CurSP = Global.GLevelData.Profit.SP * Global.GLevelSetup.KillWeight / 100+
			Global.GLevelData.Profit.SP * percent * Global.GLevelSetup.EvaluateWeight/10000;
		
		if (GameLevel.Instance.CurGold > Global.GLevelData.Profit.Gold)
            GameLevel.Instance.CurGold = Global.GLevelData.Profit.Gold;
        if (GameLevel.Instance.CurExp > Global.GLevelData.Profit.Exp)
            GameLevel.Instance.CurExp = Global.GLevelData.Profit.Exp;
		if( GameLevel.Instance.CurSP > Global.GLevelData.Profit.SP)
        	GameLevel.Instance.CurSP = Global.GLevelData.Profit.SP;
	}
	
	void SetProfit()
	{
        int tempExp = GameLevel.Instance.CurExp;
		int tempGold = GameLevel.Instance.CurGold;
		int tmpSp = GameLevel.Instance.CurSP;
        Control("Exp").GetComponent<UILabel>().text = tempExp.ToString();
        mLabelGold.text = tempGold.ToString();
		Control("SpLabel").GetComponent<UILabel>().text = tmpSp.ToString();
	}
	
	void TurnToTurntable()
	{	
		Close();
		if(TurntableWnd.Exist)
			TurntableWnd.Instance.Show();
		else
			TurntableWnd.Instance.Open();
	}
	
	public void ShowEvaBase()
	{
		mEvaluationBase.SetActive(true);
		//change the sound when the value come
		SoundManager.Instance.PlaySound("UI_FuBenTongGuan");
		TweenAlpha tweenAlpha = mEvaluationBase.GetComponent<TweenAlpha>();
		if(tweenAlpha != null)
		{
			tweenAlpha.Reset();
			tweenAlpha.Play(true);
			tweenAlpha.onFinished = LevelUpSign;
		}
	}
	
	private void LevelUpSign(UITweener tween)	
	{
		if(mIsLevelup)
		{
			mLevelUpPrompt.SetActive(true);
			TweenScale tweenScale = mLevelUpPrompt.GetComponentInChildren<TweenScale>();
			if(tweenScale != null)
			{
				tweenScale.Reset();
	   			tweenScale.Play(true);
//				float durationTime = tweenScale.duration + 1.0f;
				tweenScale.onFinished = TweenFinish;
				ControlTimeDelay(false);
			}		
		}
	}
	
	public void PromptMoveOut()
	{
		TweenPosition tweenPosition = mLevelUpPrompt.GetComponentInChildren<TweenPosition>();
		TweenAlpha []tweenAlphaArr = mLevelUpPrompt.GetComponentsInChildren<TweenAlpha>();
		if(tweenPosition != null)
		{
			tweenPosition.Reset();
       		tweenPosition.Play(true);
		}
		foreach(TweenAlpha tweenAlpha in tweenAlphaArr)
		{
			if(tweenAlpha != null)
			{
				tweenAlpha.Reset();
				tweenAlpha.Play(true);
			}
		}		
	}
}
