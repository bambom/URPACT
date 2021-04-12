using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PauseWnd : Window<PauseWnd>
{
    public override string PrefabName { get { return "PauseWnd"; } }
	
	GameObject mPauseRoot;
	GameObject mCounter;
	bool mbMutex = false;
	float mTime = 3.0f;
	float mStartTime = 0.0f;
    protected override bool OnOpen()
    {
		Init();
		return base.OnOpen();
    }

    protected override bool OnClose()
    {
        return base.OnClose();
    }
	
	void Init()
	{
		WndObject.AddComponent<ControlHelper>().ControlHelpUpdate = UpdateTimeCounter;
        UIEventListener.Get(Control("QuitLevel")).onClick = OnClickBacktoMainTown;
		UIEventListener.Get(Control("Continue")).onClick = OnClickBtnContinueGame;
		mPauseRoot = Control ("PauseRoot");
		mCounter = Control("TimeCounter");
		mCounter.SetActive(false);
		SeateToScene();
	}
	
	void SeateToScene()
	{
		switch(Global.CurSceneInfo.ID)
		{
			case ESceneID.Invalid:
			{
				LevelHelper.LevelPause(true);
				CreateDropItemGrid();
			}break;
			case ESceneID.PVP:
			{
				Control("UIGridPanel").SetActive(false);
			}break;
			default: //PvE
			{
				LevelHelper.LevelPause(true);
				CreateDropItemGrid();
			}break;
		}
	}

    void OnClickBacktoMainTown(GameObject go)
    {
		Global.ShowLoadingStart();
        Request(new LeaveLevelCmd(), delegate(string err, Response response)
        {
			Global.ShowLoadingEnd();
			if (!string.IsNullOrEmpty(err)){
				MessageBoxWnd.Instance.Show(err,MessageBoxWnd.StyleExt.Alpha);
                //return;
            }
            BackToMainTown();
        });
    }
	
    void BackToMainTown()
    {
        if (FightMainWnd.Exist)
		    FightMainWnd.Instance.Close();
		
		if (PauseWnd.Exist)
		    PauseWnd.Instance.Close();
		
		LevelHelper.LevelContinue(false);
		LevelHelper.LevelReset();
		
		SceneList SLItem = SceneListManager.Instance.GetItem((int)ESceneID.Main,(int)ESceneType.Main);

		Global.CurSceneInfo.ID = ESceneID.Main;
		Global.CurSceneInfo.Type = ESceneType.Main;
		Global.CurSceneInfo.Name = SLItem.Name;
        MainScript.Instance.LoadLevel(Global.CurSceneInfo);
	}
	
	void ResumeGame()
	{
		switch(Global.CurSceneInfo.ID)
		{
			case ESceneID.Invalid:
			{
				LevelHelper.LevelContinue(false);
			}break;
			case ESceneID.PVP:
			{
			}break;
			default: //PvE
			{
				LevelHelper.LevelContinue(false);
			}break;
		}
		Close();
		
		if (FightMainWnd.Exist)
		    FightMainWnd.Instance.Show();
		else
			FightMainWnd.Instance.Open();
	}
	
	void OnClickBtnContinueGame(GameObject go)
    {
		//ResumeGame();
		mbMutex = true;	
		mPauseRoot.SetActive(false);
		mCounter.SetActive(true);
		mStartTime = Time.realtimeSinceStartup;
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
	
	void UpdateTimeCounterUI(float dt)
	{
		string str = string.Format("{0:C}",dt).Substring(1,4);
		mCounter.GetComponent<UILabel>().text = str ;
	}
	
	void UpdateTimeCounter()
	{
		if( !mbMutex )
			return;
		
		Debug.Log("UpdateTimeCounter " + (Time.realtimeSinceStartup - mStartTime));
		float dtime = Time.realtimeSinceStartup - mStartTime;
		UpdateTimeCounterUI( mTime - dtime );
		if( mTime - dtime <= 0.0f )
		{
			mbMutex = false;
			ResumeGame();
		}
	}
}
