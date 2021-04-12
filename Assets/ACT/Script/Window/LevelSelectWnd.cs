using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class LevelSelectWnd :  Window<LevelSelectWnd>
{
    public override string PrefabName { get { return "LevelSelectWnd"; } }
	 
	LevelSlider []mSlider = LevelSliderManager.Instance.GetAllItem();
	public PveInfo mPveInfo;
	
	GameObject mContentPanel;
	GameObject mWorldMapCamera;
	GameObject mWorldChapterRoot;
	GameObject mNextLevel;
	GameObject mFirstLevel;
	GameObject mChooseFrame;
	GameObject mLevelPath;
//	GameObject mCurOpenChapter;
	GameObject mCurWorldLine;
	GameObject mBorderFrame;
	Transform  mStrengthCountGrid;
	Transform  mLevelRewardGrid;
	Transform  mCameraStartPos;
	Transform  mTemporaryPos;
	Transform  []mCameraPos = new Transform[8];
	Transform  []mChapterRoot= new Transform[8];
	Transform  [,]mWorldmapLine =new Transform[8,11];
	UILabel mTitleLabel;
	UILabel mRmdPowerLabel;
	Camera mUICamera;
	UpdateProgress mUpdateProgress;
	CameraDragMove mCameraDragMove;
	
	const int mMaxDropCount = 5;
	int mDropCount = 0;	
	int mLevelID;
	int mQuestlevelID;
	string mChooseChapter;
	List<string> mDropItemBase = new List<string>();
	
	public delegate void QuestEntry(int QuestID);
	public QuestEntry QuestEntryLevel;
	private bool mIsClickQuest = false;
	private bool mMoveNear = true;
	
	UIGuideShow mUIGuideShow = null;
	IconTips mIconTips = null;
    protected override bool OnOpen()
    {
		InitObj();
		InitTranPos();
	    InitMapSetting();
		GetAllLevel();
        return base.OnOpen();
    }
	 
    protected override bool OnClose()
    {
		if( mUIGuideShow != null ){
			mUIGuideShow.Destroy();
			mUIGuideShow = null;
		}
		
        return base.OnClose();
    }	
	
	private void InitObj()
	{
		mContentPanel = Control("ContentPanel");
		mWorldMapCamera = GameObject.Find("WorldmapRoot").transform.Find("CameraPos0").gameObject;
		mWorldChapterRoot = GameObject.Find("ChapterRoot");
		mStrengthCountGrid = Control("CountGrid").transform;
		mLevelRewardGrid = Control("LevelReward").transform;
		mLevelPath = Control("LevelPath");
		mBorderFrame = Control("BorderFrame");
		mTitleLabel = Control("WndTitle").GetComponentInChildren<UILabel>();
		mRmdPowerLabel=Control("PowerLabel").GetComponent<UILabel>();
		mUICamera = GameObject.Find("Anchor").transform.parent.GetComponent<Camera>();
		
		UIEventListener.Get(Control("StartButton")).onClick = OnClickEntryGame;
		UIEventListener.Get(Control("BackButton")).onClick = OnClickBack;
		mContentPanel.SetActive(false);
		mBorderFrame.SetActive(false);
	}
		
	private void InitTranPos()
	{
		mCameraStartPos = GameObject.Find("StartPos").transform;
		mTemporaryPos = GameObject.Find("TemporaryPos").transform;
		SetTransEqual(mWorldMapCamera.transform,mCameraStartPos);
		for(int i=0 ;i<mCameraPos.Length; i++)
			mCameraPos[i] = GameObject.Find("CameraPos0" + (i+1).ToString()).transform;	
		
		Transform levelPoint = GameObject.Find("LevelPoint").transform;
		for(int i=0 ;i<mChapterRoot.Length; i++)
			mChapterRoot[i] = levelPoint.Find("Level0"+ (i+1).ToString());
		
		Transform worldmapLine = GameObject.Find("WorldmapLine").transform;
		string path;
		for(int i=0 ;i<8; i++)
		{
			for(int j=0; j<11; j++)
			{
				path = "WorldmapLine0"+(i+1).ToString();
	            path = path+"/"+path+"_";
				if(j+1<10)
					path += "0"+(j+1).ToString();
				else
					path += (j+1).ToString();
				
				mWorldmapLine[i,j] = worldmapLine.Find(path);
			}
		}
	}
	
	private void InitMapSetting()
	{
 	    mWorldMapCamera.SetActive(true);
		mCameraDragMove = WndObject.GetComponent<CameraDragMove>();
		if(mCameraDragMove == null)
			 mCameraDragMove = WndObject.AddComponent<CameraDragMove>();	
	}
		
	private void OnClickBack(GameObject go)
	{
		if(mContentPanel.activeSelf)
		{
			BackToWorldMap();
			if ( mIconTips != null )
			{
				mIconTips.Destroy();
				mIconTips = null;
			}
		}
		else 
		{
			mWorldMapCamera.GetComponent<TweenTransform>().enabled = false;
			mWorldMapCamera.SetActive(false);
			DestroyChapterObj();
			Close();
	        if (!InGameMainWnd.Exist)
	            InGameMainWnd.Instance.Open();
			else
				InGameMainWnd.Instance.Show();			
		}
	}
	
	private void DestroyCameraTween()
	{
		if(mWorldMapCamera.GetComponent<TweenTransform>() != null)
		{
			TweenTransform.Destroy(mWorldMapCamera.GetComponent<TweenTransform>());
		}
	}
	
	private void BackToWorldMap()
	{
		if( mUIGuideShow != null ){
			mUIGuideShow.Destroy();
			mUIGuideShow = null;
		}
		
		mIsClickQuest = false;
		mMoveNear = false;
		mBorderFrame.SetActive(false);
		mContentPanel.SetActive(false);	
//		mCurOpenChapter.SetActive(true);
		SetChapterTitle(true);
		if(mUpdateProgress != null)
			mUpdateProgress.DestroySelf();
		if(mCurWorldLine != null)
			mCurWorldLine.SetActive(false);
		if(mCameraDragMove != null)
			mCameraDragMove.enabled = true;
        MoveToPos(mTemporaryPos);
	}
		
	private void GetAllLevel()
	{	
		Global.ShowLoadingStart();
		Request(new GetPveInfoCmd(), delegate(string err, Response response)
		{
			Global.ShowLoadingEnd();
			if(!string.IsNullOrEmpty(err))
			{
				//promote the error message		
				MessageBoxWnd.Instance.Show( err ,MessageBoxWnd.StyleExt.Alpha);
				return;
			}			
			mPveInfo = response != null ? response.Parse<PveInfo>() : null;
			InitWorldMap();	
		});
	}
	
	void InitWorldMap()
	{
		foreach(LevelSlider levelSlider in mSlider)
		{
			string[] levelIds = levelSlider.Level.Split('|');
			string levelId = levelSlider.ID.ToString();	
			int i;
			
			for(i=0; i<levelIds.Length; i++)
			{
				LevelSetup levelSetup = LevelSetupManager.Instance.GetItem(int.Parse(levelIds[i]));
				string prePose = levelSetup.PrePose.ToString();
				
				if(mPveInfo.Levels == null)
				{
					if(prePose == "0")
						break;
				}
				else{
					if(mPveInfo.Levels.ContainsKey(levelIds[i]) ||
					   mPveInfo.Levels.ContainsKey(prePose) || 
					   levelSetup.PrePose.ToString() == "0")					
						break;
				}						
			}	
			
			if(i < levelIds.Length)
				CreateMap(levelSlider,true);
			else
				CreateMap(levelSlider,false);	
		}		
		if(QuestEntryLevel != null)
		{   
			QuestEntryLevel(mQuestlevelID);
		}
	}
	
	void CreateMap(LevelSlider level,bool isOpen)
	{
		GameObject mapItem = GameObject.Instantiate(Resources.Load("MapItem")) as GameObject;
		GameObject parent = mWorldChapterRoot.transform.Find(level.ID.ToString()).gameObject;
//	    for(int i=0 ;i<parent.transform.childCount ; i++)
//			GameObject.Destroy(parent.transform.GetChild(i).gameObject);
		SetObjPos(mapItem,parent);		
		string labelText = level.ClassName+"  "+"[FFFFFF]"+level.ClassIntro+"[-]";
		mapItem.transform.Find("ChapterNameLabel").GetComponent<UILabel>().text = labelText;
		mapItem.name = level.ID.ToString();
		mapItem.AddComponent<ChapterTitleFaceBorad>();
		
		if(isOpen)
		{
			mapItem.transform.Find("CloseSprite").gameObject.SetActive(false);
			mapItem.GetComponent<BoxCollider>().enabled = true;		
			UIEventListener.Get(mapItem).onClick  = OnClickChapterSelect;	
		}
		else
		{
			mapItem.transform.Find("ChapterBgSprite").GetComponent<UISprite>().spriteName="Button12_Checkpoint_Normal_06";
		}			
	}
	
	private void OnClickChapterSelect(GameObject go)
	{
		//Debug.Log("You Click Chapter:" +go.name);
		SetTransEqual(mTemporaryPos,mWorldMapCamera.transform);
        MoveToPos(mCameraPos[int.Parse(go.name)-1]);		
		mChooseChapter = go.name;	
		mMoveNear = true;
		if(mCameraDragMove != null) mCameraDragMove.enabled = false;
		mCameraDragMove.WaitForTime(0.7f);				
	}
	
	private void CameraMoveFinish(UITweener tween)
	{
		if(mMoveNear)
			ShowChapterContent();				
	}
	
	private void SetTransEqual(Transform transOne,Transform transTwo)
	{
		transOne.localPosition = transTwo.localPosition;
		transOne.localEulerAngles = transTwo.localEulerAngles;
		transOne.localScale = transTwo.localScale;
	}
	
	private void MoveToPos(Transform trans)
	{
		TweenTransform tweenTransform = mWorldMapCamera.GetComponent<TweenTransform>();
        tweenTransform.to = trans;
        tweenTransform.Reset();
        tweenTransform.Play(true);
		mWorldMapCamera.GetComponent<TweenTransform>().onFinished = CameraMoveFinish;
	}
	
	public void ShowBorderFrame()
	{
		mBorderFrame.SetActive(true);
	}
	
	public void ShowChapterContent()
	{
		if(!string.IsNullOrEmpty(mChooseChapter))
			ShowContent(mChooseChapter);
		
		if(mIsClickQuest)
			QuestChooseLevel();
		
		mUIGuideShow = new UIGuideShow(PrefabName);
		if(mUIGuideShow.GuideCheck()){
			mUIGuideShow.GuideStart();
		}		
	}
	
	private void ShowContent(string levelSliderId)
	{	
		SetChapterTitle(false);
		mContentPanel.SetActive(true);
		UpdateChapterContent(levelSliderId);		
	}
	
	private void UpdateChapterContent(string levelSliderId)
	{
//		mCurOpenChapter = Control(levelSliderId,mWorldChapterRoot);
//		mCurOpenChapter.SetActive(false);
		int ID = int.Parse(levelSliderId);
		LevelSlider levelSlider = LevelSliderManager.Instance.GetItem(ID);
		mTitleLabel.text = levelSlider.ClassName;	
		DestroyLevels();
		ProduceLevelList(levelSlider.Level);
	}

	private void DestroyLevels()
	{
		for(int i=0 ;i<mLevelPath.transform.childCount ;i++)
			GameObject.Destroy(mLevelPath.transform.GetChild(i).gameObject);
	}
	 
	private void SetChapterTitle(bool isVisible)
	{
		for(int i=0 ;i<mWorldChapterRoot.transform.childCount; i++)
			mWorldChapterRoot.transform.GetChild(i).gameObject.SetActive(isVisible);
	}
	
	private void DestroyChapterObj()
	{
		Transform []children = mWorldChapterRoot.GetComponentsInChildren<Transform>();
		foreach(Transform child in children)
		{
			if(child.parent.parent == mWorldChapterRoot.transform)
				GameObject.Destroy(child.gameObject);
		}
	}
	
	private void ProduceLevelList(string str)
	{
		int count = 0;	
		int openLevelCount = 0;
	    string[] levelIds = str.Split('|');
		
		foreach(string levelId in levelIds)
		{			
			if(int.Parse(levelId) < 1)
				continue;
			
			GameObject levelItem;
			int index;
			LevelSetup levelSetup = LevelSetupManager.Instance.GetItem(int.Parse(levelId));
			string prePose = levelSetup.PrePose.ToString();
			if(mPveInfo.Levels == null)
			{ 
				if(prePose == "0"){
					levelItem = GameObject.Instantiate(Resources.Load("OpenLevel")) as GameObject;
					index = 1;	
				}
				else{
					levelItem = GameObject.Instantiate(Resources.Load("CloseLevel")) as GameObject;	
				    index = 2;
				}
			}
			
			else 
			{
				if(mPveInfo.Levels.ContainsKey(levelId))
				{
					levelItem = GameObject.Instantiate(Resources.Load("OpenLevel")) as GameObject;
					index = 0;
				}
						
				else if(mPveInfo.Levels.ContainsKey(prePose) || prePose == "0")
				{
					levelItem = GameObject.Instantiate(Resources.Load("OpenLevel")) as GameObject;
					index = 1;
				}
				else{
					levelItem = GameObject.Instantiate(Resources.Load("CloseLevel")) as GameObject;	
					index = 2;
				}
			}
			if(index == 0)			
				openLevelCount++;	
			SetObjScreenPos(levelItem,count+1);
			CreateLevel(levelItem,levelId,index);
			count++;
			if(count == 1)
				mFirstLevel = levelItem;						
		}
		InitChoose();
	    UpdateLine(openLevelCount);
	}
	
	private void SetObjScreenPos(GameObject go,int num)
	{
		int chapterID = int.Parse(mChooseChapter) - 1;
		string objName;
		if(num < 10)
			objName = "Level0"+num;
		else
			objName = "Level"+num;
		Vector3 pos = mChapterRoot[chapterID].Find(objName).position;
		Vector3 screenPos = mWorldMapCamera.GetComponent<Camera>().WorldToScreenPoint(pos);
		Vector3 uiPos = mUICamera.ScreenToWorldPoint(screenPos);
		uiPos.z = 0.0f;
		go.transform.position = uiPos;
		go.transform.parent = mLevelPath.transform;
		go.transform.localScale = Vector3.one;
		go.transform.localEulerAngles = Vector3.zero;		
	}
	
	private void CreateLevel(GameObject levelItem,string levelId,int index)
	{
		SceneList sceneList = SceneListManager.Instance.GetItem(int.Parse(levelId),(int)ESceneType.Level);
		if(sceneList == null)
			sceneList = SceneListManager.Instance.GetItem(int.Parse(levelId),(int)ESceneType.SuperLevel);
		switch(index)
		{
			case 0:
			    string evaluateIconName;		
			    LevelHelper.ParseGradeInfo(int.Parse(levelId),mPveInfo.Levels[levelId].MaxScore,
										   out evaluateIconName);
				Control("Evaluate",levelItem).GetComponentInChildren<UISprite>().spriteName = evaluateIconName;	
			    UIEventListener.Get(levelItem).onClick  = OnClickItemIcon;
				break;
			case 1:
                Control("Evaluate", levelItem).GetComponentInChildren<UISprite>().gameObject.SetActive(false);
				Control("LevelIcon",levelItem).GetComponent<UISprite>().spriteName = sceneList.Icon;
			    UIEventListener.Get(levelItem).onClick  = OnClickItemIcon;
			    if(mNextLevel == null)
					mNextLevel = levelItem;	
				break;
			case 2:
				Control("LevelIcon",levelItem).GetComponent<UISprite>().spriteName = sceneList.Icon;
				break;
			default:
				break;
		}
		levelItem.name = levelId;		
	}
	
	private void InitChoose()
	{
		//Debug.Log(mNextLevel.name);
		if(mNextLevel != null){
			OnClickItemIcon(mNextLevel);
			mNextLevel = null;
		}
		else
			OnClickItemIcon(mFirstLevel);
	}
	
	private void UpdateLine(int count)
	{
		int id = int.Parse(mChooseChapter) - 1;
		for(int i=0; i<11 ;i++)
		{
			if(mCurWorldLine != mWorldmapLine[id,i].parent.gameObject)
				mCurWorldLine = mWorldmapLine[id,i].parent.gameObject; 
			if(!mWorldmapLine[id,i].parent.gameObject.activeSelf)
				mWorldmapLine[id,i].parent.gameObject.SetActive(true);
			if(i<count-1)
				mWorldmapLine[id,i].gameObject.SetActive(true);
			else
				mWorldmapLine[id,i].gameObject.SetActive(false);
		}
		if(count > 0 && count < 12)
		{   
			GameObject nextOpenLevelObj = mWorldmapLine[id,count -1].gameObject;
			nextOpenLevelObj.SetActive(true);
			MeshRenderer meshRenderer = nextOpenLevelObj.GetComponentInChildren<MeshRenderer>();
			if(meshRenderer != null)
			{
				meshRenderer.material.color = Color.red;
				mUpdateProgress = meshRenderer.gameObject.AddComponent<UpdateProgress>();
			}
		}

	}

    private void OnClickItemIcon(GameObject go)
    {
        Transform chooseFrame = go ? go.transform.Find("ChooseFrame") : null;
        if (!chooseFrame || chooseFrame.gameObject == mChooseFrame)
            return;

        mLevelID = int.Parse(go.name);
        //SceneList sceneList = SceneListManager.Instance.GetItem(mLevelID,(int)ESceneType.Level);
        //mLevelIntro.text = sceneList.Intro; 	    
        if (mChooseFrame != null && mChooseFrame.GetComponentInChildren<ParticleSystem>() != null)
            GameObject.Destroy(mChooseFrame.GetComponentInChildren<ParticleSystem>().gameObject);

        GameObject ChooseEffectObj = GameObject.Instantiate(Resources.Load("ChooseEffect")) as GameObject;
        if (!ChooseEffectObj.GetComponent<ParticleSystem>().isPlaying)
            ChooseEffectObj.GetComponent<ParticleSystem>().Play();

        mChooseFrame = chooseFrame.gameObject;
        SetObjPos(ChooseEffectObj, mChooseFrame);
        UpdateLevelInfo();
	}
	
	private void UpdateLevelInfo()
	{
	 	LevelSetup levelSetup = LevelSetupManager.Instance.GetItem(mLevelID);
		int selfPower = (int)PlayerDataManager.Instance.Attrib.Battle;
		int rmdPower = (int)levelSetup.Power;
		mRmdPowerLabel.text = rmdPower.ToString();
		mRmdPowerLabel.color = selfPower >= rmdPower ? Color.white : Color.red;
	    ShowLevelStrength(int.Parse(levelSetup.LimitStrength.ToString()));
		ShowLevelReward(levelSetup);
	}
	
	private void ShowLevelStrength(int count)
	{
		//Strength must be less than 4
	    mStrengthCountGrid.parent.gameObject.SetActive(true);
		for(int i=0; i<mStrengthCountGrid.childCount ;i++)
		{
			if(i<count)
				mStrengthCountGrid.GetChild(i).gameObject.SetActive(true);
			else
				mStrengthCountGrid.GetChild(i).gameObject.SetActive(false);
		}
		mStrengthCountGrid.GetComponent<UIGrid>().repositionNow = true;
	}
	
	private void ShowLevelReward(LevelSetup levelSetup)
	{
		for(int i=0; i<mLevelRewardGrid.childCount; i++)
			mLevelRewardGrid.GetChild(i).gameObject.SetActive(false);
		mDropCount = 0;
		mDropItemBase.Clear();
		
		SceneList sceneList = SceneListManager.Instance.GetItem((int)levelSetup.ID,(int)ESceneType.Level);
		if(sceneList == null)
			sceneList = SceneListManager.Instance.GetItem((int)levelSetup.ID,(int)ESceneType.SuperLevel);
		if(sceneList == null)
		{
			Debug.LogError("Get Scenelist Error!");
			return;
		}
	    string[] levelDropItems = sceneList.ItemBaseID.Split('|');
	    GetLevelReward(levelDropItems);
	}
	
	void OnClickCloseTips( )
	{
		mIconTips = null;
	}
	
	void OnClickReward( GameObject go )
	{
		string goText = Control("ItemBaseID",go).GetComponent<UILabel>().text;
		int BaseID = int.Parse(goText);
		ItemBase itemBase = ItemBaseManager.Instance.GetItem(BaseID);
		
		Vector3 Vpos = go.transform.localPosition;
		if( mIconTips == null )
		{
			mIconTips = new IconTips(WndObject,itemBase,Vpos);
			mIconTips.OnClickedClose = OnClickCloseTips;
		}
		else
		{
			mIconTips.UpdateDesc(itemBase,Vpos);	
		}		
	}
	
	private void GetLevelReward(string []dropItems)
	{
		if(mDropCount == mMaxDropCount)
			return;
		foreach(string item in dropItems)
		{
			ItemBase itemBase = ItemBaseManager.Instance.GetItem(int.Parse(item));
			if(itemBase == null)
			{
				Debug.LogError("ItemBase is not Exist in ID:"+item);
				continue;
			}
			if(itemBase.Role != PlayerDataManager.Instance.Attrib.Role &&
			   itemBase.Role.ToString() != "0")
				continue;
			if(mDropItemBase != null && mDropItemBase.Contains(itemBase.Icon))
				continue;			
			GameObject reward = mLevelRewardGrid.GetChild(mDropCount).gameObject;
			reward.SetActive(true);
			UIEventListener.Get (reward).onClick = OnClickReward;
			mLevelRewardGrid.GetComponent<UIGrid>().repositionNow = true;
			//Debug.Log(itemBase.ID);
			Control("ItemIcon",reward).GetComponent<UISprite>().spriteName = itemBase.Icon;
			Control("ItemBaseID",reward).GetComponent<UILabel>().text = itemBase.ID.ToString();
			UpdateItemQualityFrameIcon(reward.transform.Find("QualityIcon").gameObject,itemBase);
			mDropItemBase.Add(itemBase.Icon);
			mDropCount++; 
			if(mDropCount == mMaxDropCount)
				return;
		}
	}

	void SetObjPos(GameObject child,GameObject parent)
	{
		child.transform.parent = parent.transform;
		child.transform.localScale = Vector3.one;
		child.transform.localPosition = Vector3.zero;
		child.transform.localEulerAngles = Vector3.zero;
	}
	
	public void OnClickEntryGame(GameObject go)
	{   
	    if(BackPack.PackageFullCheck())
		{
			MessageBoxWnd.Instance.Show("PackIsFull",MessageBoxWnd.Style.OK_CANCLE);		
			MessageBoxWnd.Instance.OnClickedOk = SortPackage;
			return;
		}
        Request(new EnterLevelCmd(mLevelID), delegate(string err, Response response)
        {
            if (!string.IsNullOrEmpty(err))
            {
				MessageBoxWnd.Instance.Show( err ,MessageBoxWnd.StyleExt.Alpha);
                return;
            }
			
			Global.GLevelData = response != null ? response.Parse<LevelData>() : null;
			if (Global.GLevelData != null)
			{
				EnterLevel(Global.GLevelData.Info.ID);
			}
        });
	}
	
	private void SortPackage(GameObject go)
	{
		Hide();
		PackageWnd.Instance.Open();
		PackageWnd.Instance.ComeFromLevelSelect();
	}
	
	private bool EnterLevel(int levelId)
    {
        LevelSetup levelSetup = LevelSetupManager.Instance.GetItem(levelId);
        if (levelSetup == null)
            return false;
		
		DestroyChapterObj();
		Close();		
		if (InGameMainWnd.Exist)
			InGameMainWnd.Instance.Close();		

        Global.GLevelSetup = levelSetup;
		///进入副本时间点
		LevelHelper.LevelStart(DateTime.Now);
		Global.CurSceneInfo.ID = ESceneID.Invalid + (int)levelSetup.ID;
		
		if((int)levelSetup.ID > 7999)
			Global.CurSceneInfo.Type = ESceneType.SuperLevel;
		else
			Global.CurSceneInfo.Type = ESceneType.Level;
		
		Global.CurSceneInfo.Name = levelSetup.Type;
		return MainScript.Instance.LoadLevel(Global.CurSceneInfo);
	}	
	
	public void EnterQuestLevel(int QuestID)
	{	
		mQuestlevelID = QuestID;
		QuestEntryLevel = ClickQuestLevel;
		mIsClickQuest = true;
	}
	
	private void ClickQuestLevel(int ID)
	{
		string id = mQuestlevelID.ToString();
		string name = GetChapterName(id);
		Transform parent = mWorldChapterRoot.transform.Find(name).transform;
		GameObject chapterObj = null;
		for(int i=0;i<parent.childCount;i++)
		{
			if(parent.GetChild(i).name == name)
			{
				chapterObj = parent.GetChild(i).gameObject;
				break;
			}
		}
		if(chapterObj != null)
		{		
			Debug.Log(chapterObj.name);
			OnClickChapterSelect(chapterObj);
		}
		Debug.Log(Time.time);
	}
	
	private void QuestChooseLevel()
	{
		Debug.Log(Time.time);
		OnClickItemIcon(Control(mQuestlevelID.ToString(),mLevelPath));	
	}
	private string GetChapterName(string questLevelId)
	{
		string chapter = "";
		int flag = 0;
		foreach(LevelSlider levelSlider in mSlider)
		{
			string[] levelIds = levelSlider.Level.Split('|');
			chapter = levelSlider.ID.ToString();
			
			for(int i=0; i<levelIds.Length; i++)
			{
				if(levelIds[i] == questLevelId)
				{
					flag = 1;
					break;
				}						
			}	
			if(flag == 1)
				break;
		}
		return chapter;
	}
}