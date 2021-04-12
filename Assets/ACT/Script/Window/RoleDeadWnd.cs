using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class RoleDeadWnd : Window<RoleDeadWnd>
{	
    public override string PrefabName { get { return "RoleDeadWnd"; } }
	
	int mReviveTimes = 0;
	GameObject mRoleDeadRoot;
	GameObject mCounter;
	bool mbMutex = false;
	float mTime = 3.0f;
	float mStartTime = 0.0f;
	
    protected override bool OnOpen()
    {
		Init();
        return base.OnOpen();
    }
	
	void Init()
	{		
		WndObject.AddComponent<ControlHelper>().ControlHelpUpdate = UpdateTimeCounter;
		UIEventListener.Get(Control("BtnReturn")).onClick = OnClickBtnBacktoMain;
		UIEventListener.Get(Control("BtnRevive")).onClick = OnClickBtnRelive;		
		
		mRoleDeadRoot = Control ("RoleDeadRoot");
		mCounter = Control("TimeCounter");
		mCounter.SetActive(false);	
		
		RequestReviveInfo();
		CreateDropItemGrid();

        if (Global.GInputBox != null)
            Global.GInputBox.ResetInput();
	}
	
	void RequestReviveInfo()
	{
        Request(new ReviveInfoCmd(), delegate(string err, Response response)
        {
			if (!string.IsNullOrEmpty(err)){
              	MessageBoxWnd.Instance.Show(err,MessageBoxWnd.StyleExt.Alpha);
                return;
            }
			ReviveInfo reviveInfo = (response != null) ? response.Parse<ReviveInfo>() : null;
			UpdateReviveBtn(reviveInfo.Count);
			UnitManager.Instance.LocalPlayer.PlayAction(Data.CommonAction.Idle);
		});
	}
	
	void UpdateReviveBtn(int Count)
	{
        if (Count < 0)
        {
            string str = ("Free");
            string strRet = string.Format(str + "({0})", Mathf.Abs(-Count));
            Control("GemLabel", Control("BtnRevive")).GetComponent<UILabel>().text = strRet;
			Control("BtnRevive").transform.Find("GemIcon").gameObject.SetActive(false);
        }
        else
        {
            int costGem = ReviveBaseManager.Instance.GetItem(Count + 1).Cost;
            //string str = ("ReviveForGem");
            string strRet = costGem.ToString();//string.Format(str + "({0})", costGem);
            Control("GemLabel", Control("BtnRevive")).GetComponent<UILabel>().text = strRet;
			Control("BtnRevive").transform.Find("GemIcon").gameObject.SetActive(true);
        }
	}
		
	void ShowNoGemTip()
	{
		string str = ("NotEnoughGem");
		MessageBoxWnd.Instance.Show(str,MessageBoxWnd.Style.OK_CANCLE);
		MessageBoxWnd.Instance.OnClickedOk = OnClickBuyGem;
		MessageBoxWnd.Instance.OnClickedCancle = OnClickBtnBacktoMain;
	}
	
	void OnClickBuyGem( GameObject go )
	{
		Hide();
		if ( !RechargeWnd.Exist )
			RechargeWnd.Instance.Open();
	}
	
    void OnClickBtnBacktoMain( GameObject go )
    {
		// request exist the level request.
		Global.ShowLoadingStart();
        Request(new LeaveLevelCmd(), delegate(string err, Response response)
        {
			Global.ShowLoadingEnd();
			if (!string.IsNullOrEmpty(err))
            {
                MessageBoxWnd.Instance.Show( err ,MessageBoxWnd.StyleExt.Alpha );
                return;
            }
           	BackToMainTown();
        });
    }
		
    void BackToMainTown()
    {
		Close();
        if (FightMainWnd.Exist)
            FightMainWnd.Instance.Close();
		
		SceneList sceneList = SceneListManager.Instance.GetItem((int)ESceneID.Main,(int)ESceneType.Main);
		
		Global.CurSceneInfo.ID = ESceneID.Main;
		Global.CurSceneInfo.Type = ESceneType.Main;
		Global.CurSceneInfo.Name = sceneList.Name;
        MainScript.Instance.LoadLevel(Global.CurSceneInfo);
    }
	
	void ReliveResume()
	{
        UnitManager.Instance.LocalPlayer.Revive();
		FightMainWnd.Instance.ReviveUpdateUI();
        //RequestMainAttrib();Sync CurHP CurAbility in Revive already
	}
	
	void OnClickBtnRelive(GameObject go)
    {
		Global.ShowLoadingStart();
		Request(new ReviveCmd(), delegate(string err, Response response)
        {
			Global.ShowLoadingEnd();
            if (!string.IsNullOrEmpty(err)){
				ShowNoGemTip();
                return;
            }
			
			//ReliveResume();
			mbMutex = true;	
			mRoleDeadRoot.SetActive(false);
			mCounter.SetActive(true);
			mStartTime = Time.realtimeSinceStartup;
			
			Global.Pause = true;
		});
		
	}
	
    void RequestMainAttrib()
    {
        Request(new GetUserAttribCmd(), delegate(string err, Response response)
        {
            if (!string.IsNullOrEmpty(err)) {
                return;
            }
            MainAttrib attrib = (response != null) ? response.Parse<MainAttrib>() : null;
            if (attrib != null)
                PlayerDataManager.Instance.OnMainAttrib(attrib);
        });
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
			Global.Pause = false;
			ReliveResume();
		}
	}
}
