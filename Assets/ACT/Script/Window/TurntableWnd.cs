using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class TurntableWnd : Window<TurntableWnd>
{
	int mCurItemIndex = 0;
	int mChooseSequence;
    int mDropGroup = 0;
    int mTurnCount = 0;
	
	int mAddExp;
	int mCurLevel;
	int mLastLevel;
	int mRoleID;
	int mCurExp;
	int mNextExp;
	
    LevelSetup mLevelSetup = null;
	ChangePlate mChangePlate;
    FinishLevelControl mFinishLevelControl;
	GameObject mCostGemObj;
	UISprite mButtonSprite;
	GameObject mBackTownBtn;
	GameObject mPromptParentTran;
	GameObject mLevelUpPrompt;
	GameObject []mFrameObjArr = new GameObject[12];
	List<int> mGetIndexLst = new List<int>();
    public const int TurntableNum = 12;
    public override string PrefabName { get { return "TurntableWnd"; } }
	
    protected override bool OnOpen()
    {
        RequestTruntableData();				
        // setup the turn table cost.
        return base.OnOpen();
    } 
	
	void InitObj()
	{
	    mChangePlate = WndObject.GetComponent<ChangePlate>();
		if(mChangePlate == null)
			mChangePlate = WndObject.AddComponent<ChangePlate>();
		
		mCostGemObj = Control("GoButton").transform.Find("CostGem").gameObject;
		mButtonSprite= Control("GoButton").transform.Find("ButtonSprite").GetComponent<UISprite>();
		mBackTownBtn = Control("GoButton").transform.parent.Find("BackToTownBtn").gameObject;
		mPromptParentTran = WndObject.transform.parent.gameObject;
		UIEventListener.Get(Control("GoButton")).onClick = OnPressGo;
		UIEventListener.Get(mBackTownBtn).onClick = OnClickBackTown;
		for(int i=0; i<TurntableNum; i++)
			mGetIndexLst.Add(i);
	}
	
	void InitAttribValue()
	{
	    mRoleID = (int)PlayerDataManager.Instance.Attrib.Role;
		mCurLevel = (int)PlayerDataManager.Instance.Attrib.Level;
		mCurExp = (int)PlayerDataManager.Instance.Attrib.CurExp;
		mNextExp = (int)PlayerDataManager.Instance.Attrib.NextExp;
		mAddExp = 0;
		mLastLevel = mCurLevel;
		Debug.Log("mRoleID:"+mRoleID+"  mCurLevel"+mCurLevel+"curExp:"+mCurExp+"   nextExp:"+mNextExp+"   addExp:"+mAddExp);
	}
	
    void RequestTruntableData()
    {
		Global.ShowLoadingStart();
        Request(new GetTurntableDataCmd(), delegate(string err, Response response)
        {
			Global.ShowLoadingEnd();
            if (!string.IsNullOrEmpty(err))
            {
                // TODO: promote the error message.
				MessageBoxWnd.Instance.Show(err,MessageBoxWnd.StyleExt.Alpha);
                return;
            }
			
			if (int.TryParse(response.data, out mDropGroup))
			{
				BuildTurntable(mDropGroup);
				InitObj();
				InitAttribValue();
				int levelId = Global.GLevelData.Info.ID;
		        mLevelSetup = LevelSetupManager.Instance.GetItem(levelId);
		        int gemCost = mLevelSetup.Turntable1;
		     	UpdateExpandGem(gemCost);	
			}
        });
    }

    void BuildTurntable(int dropGroupId)
    {
        for (int sequence = 1; sequence <= TurntableNum; sequence++)
        {
            DropGroup dropInfo = DropGroupManager.Instance.GetItem(dropGroupId, sequence);
            if (dropInfo == null)
                break;

            string itemIcon = "";
            int itemCount = 0;
			bool isResourceItem = true;
            if (dropInfo.Gold > 0)
            {
                itemIcon = "Icon01_System_Gold_01";
                itemCount = dropInfo.Gold;
            }
            else if (dropInfo.SP > 0)
            {
                itemIcon = "Icon01_System_Sp_01";
                itemCount = dropInfo.SP;
            }
            else if (dropInfo.Exp > 0)
            {
                itemIcon = "Icon01_System_Xp_01";
                itemCount = dropInfo.Exp;
            }
            else if (dropInfo.Gem > 0)
            {
                itemIcon = "Icon01_System_Gem_01";
                itemCount = dropInfo.Gem;
            }
			
            else if (!string.IsNullOrEmpty(dropInfo.Items))
            {
				isResourceItem = false;
                int itemId = 0;
                string[] dropItems = dropInfo.Items.Split('|');
                foreach (string dropItem in dropItems)
                {
                    if (int.TryParse(dropItem, out itemId))
                    {
                        ItemBase itemBase = ItemBaseManager.Instance.GetItem(itemId);
                        if (itemBase == null) continue;
                        if (itemBase.Role > 0 &&
                            itemBase.Role != PlayerDataManager.Instance.Attrib.Role)
                            continue;

                        itemIcon = itemBase.Icon;
                        itemCount = dropInfo.Min;
						CreateSpinItem(sequence,itemIcon,itemCount,itemBase);
                        break;
                    }
                }
            }
			
			if(isResourceItem)
			{
				CreateSpinItem(sequence,itemIcon,itemCount,null);
			}
            // TODO: setup the turntable item's icon and count here.
        }
    }
	void CreateSpinItem(int index,string iconName,int count,ItemBase item)
	{
		GameObject parentObj = Control(index.ToString(),Control("DiskContent"));
		GameObject go;
		if(item == null)
		{
			go = GameObject.Instantiate(Resources.Load("ResourceItem"))as GameObject;
			SetObjPos(go,parentObj);
			Control("ItemIcon",go).GetComponent<UISprite>().spriteName = iconName;
			Control("CountLabel",go).GetComponent<UILabel>().text = count.ToString();	
			mFrameObjArr[index - 1] = go.transform.Find("ChooseFrame").gameObject;			
		}
		else
		{
			go = GameObject.Instantiate(Resources.Load("SpinGoodsItem")) as GameObject;
			SetObjPos(go,parentObj);
			Control("ItemIcon",go).GetComponent<UISprite>().spriteName = iconName;
			UpdateItemQualityFrameIcon(go.transform.Find("Quality").gameObject,item);
			mFrameObjArr[index - 1] = go.transform.Find("ChooseFrame").gameObject;
		}
	}
    /// <summary>
    /// when player click the go button.
    /// it send the fetch turntable reward to the server.
    /// then the server return the choosed item.
    /// </summary>
    /// <param name="go"></param>
    void OnPressGo(GameObject go)
    {
		Debug.Log("OnPressGo");
        FetchTurntableReward();
    }
	
	void OnClickBackTown(GameObject go)
	{	
		RequestAttribInfo();
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
			BackToMainTown();
        });
	}
	
	void BackToMainTown()
	{	
		Close();
    	SceneList SLItem = SceneListManager.Instance.GetItem((int)ESceneID.Main,(int)ESceneType.Main);		
		Global.CurSceneInfo.ID = ESceneID.Main;
		Global.CurSceneInfo.Type = ESceneType.Main;
		Global.CurSceneInfo.Name = SLItem.Name;
		MainScript.Instance.LoadLevel(Global.CurSceneInfo); 
	}
    void FetchTurntableReward()
    {
        Request(new FetchTurntableRewardCmd(), delegate(string err, Response response)
        {
            if (!string.IsNullOrEmpty(err))
            {
                // TODO: promote the error message.
				MessageBoxWnd.Instance.Show(err,MessageBoxWnd.Style.OK_CANCLE);
                return;
            }
			
			Debug.Log("OnPressGoTwo");
			int chooseSequence = 0;
			if (!int.TryParse(response.data, out chooseSequence))
				return;
			
            // TODO: mark the [chooseSequence] item in turntable as choosed.
			mChooseSequence = chooseSequence;
			mCurItemIndex = 0;
			Debug.Log("mChooseSequece:" + mChooseSequence);
			mChangePlate.onChooseItem = ChooseItem;
			mChangePlate.StartRun(mGetIndexLst.Count,GetGapCount()+1);		
            // update the turn count.
            mTurnCount++;
		   	SetBtnEnable(false);
        });
    }
	
	int GetGapCount()
	{
		int resCount = 0;
		for(int i=0 ;i<mGetIndexLst.Count ;i++)
		{
			if(mGetIndexLst[i]+1 < mChooseSequence)
				resCount++;
		}
		return resCount;
	}
	
	void UpdateExpandGem(int costCount)
	{
		if(costCount > 0)
		{
			mCostGemObj.SetActive(true);
			mCostGemObj.GetComponentInChildren<UILabel>().text = costCount.ToString();
		}
		else
		{
			mCostGemObj.SetActive(false);
		}
	}
	
	void ChooseItem()
	{
		if(mCurItemIndex >= mGetIndexLst.Count)
			mCurItemIndex = 0;	

		if(mCurItemIndex == 0)
		{
			mFrameObjArr[mGetIndexLst[mGetIndexLst.Count -1]].SetActive(false);
			mFrameObjArr[mGetIndexLst[mCurItemIndex++]].SetActive(true);
		}
		else
		{
			mFrameObjArr[mGetIndexLst[mCurItemIndex -1]].SetActive(false);
			mFrameObjArr[mGetIndexLst[mCurItemIndex++]].SetActive(true);
		}		
		if(mChangePlate.EndStop){
			if(mGetIndexLst[mCurItemIndex-1]+1 == mChooseSequence)	
			{
				mGetIndexLst.RemoveAt(mCurItemIndex-1);
			    SpinStop();
				mChangePlate.Stop();				
			}
		}         
	}
	
	void SpinStop()
	{
		Debug.Log("Dosomething:"+mChooseSequence);
		TweenAlpha tweenAlpha= mFrameObjArr[mChooseSequence-1].GetComponent<TweenAlpha>();
		tweenAlpha.Reset();
		tweenAlpha.Play(true);
		tweenAlpha.onFinished = onTweenFinish;
	}
	
	void onTweenFinish(UITweener tween)
	{
		SetBtnEnable(true);
		if(!mBackTownBtn.activeSelf)
			mBackTownBtn.SetActive(true);
	  	int gemCost = 0;
        if (mTurnCount == 1)
            gemCost = mLevelSetup.Turntable2;
        else
            gemCost = mLevelSetup.Turntable3;
		UpdateExpandGem(gemCost);
		mFrameObjArr[mChooseSequence-1].transform.parent.Find("ShadowPanel").gameObject.SetActive(true);	
		CreateSuccessEffect();
		JudgeLevelUp();
	}
	
	void CreateSuccessEffect()
	{
		Transform parentTran = mFrameObjArr[mChooseSequence-1].transform.parent;
		if(parentTran == null)
			return;		
		GameObject effect = GameObject.Instantiate(Resources.Load("SuccessEffect")) as GameObject;
		effect.transform.parent = parentTran.transform;
		effect.transform.localPosition = new Vector3(0.0f,0.0f,-10.0f);
		effect.transform.localEulerAngles = Vector3.zero;
		effect.transform.localScale = Vector3.one;
	    float time = effect.GetComponentInChildren<ParticleSystem>().duration;
		GameObject.Destroy(effect,time);
	}
	
	void JudgeLevelUp()
	{
		DropGroup dropInfo = DropGroupManager.Instance.GetItem(mDropGroup, mChooseSequence);
		if(dropInfo.Exp <= 0)
			return;
		
		Debug.Log("mCurLevel:"+mCurLevel);
		if(mCurLevel == CommonData.UserMaxLevel)
			return;
		
		mAddExp = (int)dropInfo.Exp;	
		if(mLastLevel != mCurLevel)
		{
			PlayerAttrib playerAttrib = PlayerAttribManager.Instance.GetItem(mRoleID,mCurLevel);
			mNextExp = (int)playerAttrib.NextExp;
		}
		mCurExp += mAddExp;
		if(mCurExp >= mNextExp)
		{
			mLastLevel = mCurLevel;
			GetCurLevel();
//			mLevelUpPrompt.SetActive(true);
			if(mLevelUpPrompt != null)
				GameObject.Destroy(mLevelUpPrompt);
			mLevelUpPrompt = GameObject.Instantiate(Resources.Load("SpecialEffectPrompt")) as GameObject;
			SetObjPos(mLevelUpPrompt,mPromptParentTran);
			mLevelUpPrompt.transform.localPosition = new Vector3(70.0f,80.0f,-5.0f);
			TweenScale tweenScale = mLevelUpPrompt.GetComponentInChildren<TweenScale>();			
			if(tweenScale != null)
			{
				tweenScale.transform.Find("QuestComplete").gameObject.SetActive(false);
				tweenScale.Reset();
	   			tweenScale.Play(true);
				ShowLevelUp();
			}
		}
		Debug.Log("mRoleID:"+mRoleID+"  mCurLevel"+mCurLevel+"curExp:"+mCurExp+"   nextExp:"+mNextExp+"   addExp:"+mAddExp);
	}
	
	void GetCurLevel()
	{
		while(mCurExp >= mNextExp && mCurLevel <= CommonData.UserMaxLevel)
		{
			Debug.Log("mCurLevel:"+mCurLevel+"   mCurExp:"+mCurExp+"   mNextExp:"+mNextExp);
			mCurExp -= mNextExp;
			mCurLevel++;
		    PlayerAttrib playerAttrib = PlayerAttribManager.Instance.GetItem(mRoleID,mCurLevel);
			mNextExp = (int)playerAttrib.NextExp;
		}
	}
	
	void UpdateAttribInfo()
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
			int curExp = (int)PlayerDataManager.Instance.Attrib.CurExp;
			int nextExp= (int)PlayerDataManager.Instance.Attrib.NextExp;	
			Debug.Log("After Update-"+"curExp:"+curExp+"   nextExp:"+nextExp);
        });
	}
	
	void ShowLevelUp()
	{
		mFinishLevelControl = WndObject.GetComponent<FinishLevelControl>();
		if(mFinishLevelControl == null)
			mFinishLevelControl = WndObject.AddComponent<FinishLevelControl>();	
		TweenScale tweenScale = mLevelUpPrompt.GetComponentInChildren<TweenScale>();
		float durationTime = tweenScale.duration + 0.5f;
		mFinishLevelControl.WaitForTime(durationTime,PromptMoveOut);		
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
	
	void SetBtnEnable(bool isEnable)
	{
		mCostGemObj.transform.parent.GetComponent<BoxCollider>().enabled = isEnable;
	    string spriteName = isEnable ? "Window20_Turntable_Bottom_09" : "Window20_Turntable_Bottom_14";
		mButtonSprite.spriteName = spriteName;
	}
	void SetObjPos(GameObject child,GameObject parent)
	{
		child.transform.parent = parent.transform;
		child.transform.localScale = Vector3.one;
		child.transform.localPosition = Vector3.zero;
		child.transform.localEulerAngles = Vector3.zero;
	}
}
