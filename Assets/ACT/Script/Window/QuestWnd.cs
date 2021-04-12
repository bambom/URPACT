using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Quest
{
    public enum EQuestState
    {
        New,
        Complete,
        Finish
    }
    public Quest(EQuestState state, QuestBase questBase)
    {
        State = state;
        QuestInfo = questBase;
    }

    public EQuestState State;
    public QuestBase QuestInfo;
}


public class QuestItem
{
    public delegate void VoidDelegate(GameObject go, QuestBase quest);
    public VoidDelegate OnSelectQuest;
    public Quest Quest;
    GameObject mItemObject;

    public QuestItem(Quest quest, GameObject obj)
    {
        Quest = quest;
        mItemObject = obj;
        UIEventListener.Get(obj).onClick = OnClickItem;
        obj.transform.Find("QuestNpc").GetComponent<UISprite>().spriteName = Quest.QuestInfo.Icon;
        obj.transform.Find("QuestSelectFrame").gameObject.SetActive(false);
    }

    public void OnClickItem(GameObject go)
    {
        mItemObject.transform.Find("QuestSelectFrame").gameObject.SetActive(true);
        OnSelectQuest(mItemObject, Quest.QuestInfo);
    }

    public void QuestCompelet()
    {
        mItemObject.transform.Find("State").GetComponent<UISprite>().spriteName = "Button04_Quest_Complete_01";
    }

    public void Show()
    {
        mItemObject.SetActive(true);
    }

    public void Hide()
    {
        mItemObject.SetActive(false);
    }

    public void Remove()
    {
        mItemObject.SetActive(false);
        GameObject.Destroy(mItemObject);
    }
}

public class QuestWnd : Window<QuestWnd>
{

    public override string PrefabName { get { return "QuestWnd"; } }

    int mLastSelectType = 0;
    int mSelectQuestType = 0;
    int mSelectQuestItemIndex = 0;
    int mNotOpenQuest = 0;
    QuestBase mSelectQuest;
    QuestBase mLastSelectQuest;
    Object mItemObj;
    Object mRewardObj;
    Object mItemBox;
    GameObject mSelectTypeButton;
    GameObject mQuestName;
    GameObject mQuestDesc;
    GameObject mQuestGrid;
    GameObject mRewardGrid;
    GameObject mTarget;
    GameObject mItemGrid;
    GameObject mSelectQuestItem;
    GameObject mCheckBox;
    GameObject mCheck;
    GameObject mQuestPanel;
    GameObject mSpecialEffectPrompt;
	Transform mPromptObjParent;
    bool mIsLevelUp = false;

    UIImageButton mFightButton;
    UISprite mFightSprite;
    UISprite mClaimSprite;

    List<List<Quest>> mQuestList = new List<List<Quest>>();
    List<List<QuestItem>> mQuestItemList = new List<List<QuestItem>>();
    List<int> mCompleteLevel = new List<int>();
    int[] mQuestFinish;
	UIGuideShow mUIGuideShow = null;
    protected override bool OnOpen()
    {
        GameObject type = Control("QuestType");
        for (int i = 0; i < type.transform.childCount; i++)
            UIEventListener.Get(type.transform.GetChild(i).gameObject).onClick = OnClickQuestType;

        UIEventListener.Get(Control("Back")).onClick = OnClickBack;
        mFightButton = Control("Fight").GetComponent<UIImageButton>();
        UIEventListener.Get(mFightButton.gameObject).onClick = OnClickFight;
        mQuestName = Control("QuestName");
        mQuestDesc = Control("QuestDescription");
        mItemObj = Resources.Load("QuestItem");
        mRewardObj = Resources.Load("Reward");
        mQuestGrid = Control("QuestGrid");
        mTarget = Control("Target");
        mRewardGrid = Control("RewardGrid");
        mItemBox = Resources.Load("BaseItem");
        mItemGrid = Control("ItemGrid");
        mCheck = Control("Check");
        mCheck.SetActive(false);
        mCheckBox = Control("CheckBox");
        mCheckBox.SetActive(false);
        mQuestPanel = Control("QuestList");
        mFightSprite = Control("FightSprite").GetComponent<UISprite>();
        mClaimSprite = Control("ClaimSprite").GetComponent<UISprite>();
		mPromptObjParent = WndObject.transform.parent;
        ClearDesc();
        GetCompleteQuest();

        return base.OnOpen();
    }

    void UpdateQuest()
    {
        Global.ShowLoadingEnd();
        ClearDesc();
        InitQuest();
        mSelectQuestType = 0;
        OnClickQuestType(Control("QuestType").transform.GetChild(0).gameObject);
        if (mQuestItemList.Count >= mSelectQuestType && mQuestItemList[mSelectQuestType - 1].Count > 0 && mQuestGrid.transform.childCount > 0)
        {
            mQuestItemList[mSelectQuestType - 1][0].OnClickItem(null);
            mQuestGrid.transform.GetChild(0).gameObject.transform.Find("QuestSelectFrame").gameObject.SetActive(true);
        }
		if ( mUIGuideShow == null )
 	    	mUIGuideShow = new UIGuideShow(PrefabName);
        if ( mUIGuideShow.GuideCheck() )
            mUIGuideShow.GuideStart();
    }

    protected override bool OnClose()
    {
        return base.OnClose();
    }

    public void OnClickBack(GameObject go)
    {
		if( mUIGuideShow != null ){
			mUIGuideShow.Destroy();
			mUIGuideShow = null;
		}
		
        Close();

        if (!InGameMainWnd.Exist)
            InGameMainWnd.Instance.Open();
        else
            InGameMainWnd.Instance.Show();
    }

    void GetReward()
    {
        Request(new FetchQuestRewardCmd(mSelectQuest.ID), delegate(string err, Response response)
        {
            if (!string.IsNullOrEmpty(err))
            {
                Debug.LogError("FetchQuestReward error:   " + err);
				SoundManager.Instance.PlaySound("UI_PVP_Lose");
                MessageBoxWnd.Instance.Show(err);
                return;
            }

            CreateSpecialEffect(true);
            JudgeLevelUp((int)mSelectQuest.Exp);

            mSelectQuest = null;
            UpdatePakageData();
            UpdatePlayerAttrib();
            GetFinishQuestList();
        });
    }

    void CreateSpecialEffect(bool isQuestCompelete)
    {
        if (mSpecialEffectPrompt != null)
            GameObject.Destroy(mSpecialEffectPrompt);
        mSpecialEffectPrompt = GameObject.Instantiate(Resources.Load("SpecialEffectPrompt")) as GameObject;
        mSpecialEffectPrompt.transform.parent = mPromptObjParent;
        mSpecialEffectPrompt.transform.localPosition = new Vector3(100.0f, 180.0f, -50f);
		mSpecialEffectPrompt.transform.localEulerAngles = Vector3.zero;
		mSpecialEffectPrompt.transform.localScale = Vector3.one;
        //GameObject.Destroy(mSpecialEffectPrompt, 1.5f);
		FinishLevelControl finishLevelCtrl = mSpecialEffectPrompt.GetComponent<FinishLevelControl>();
		if(finishLevelCtrl == null)
			finishLevelCtrl = mSpecialEffectPrompt.AddComponent<FinishLevelControl>();
		TweenScale tweenScale = mSpecialEffectPrompt.GetComponentInChildren<TweenScale>();
		if(tweenScale)
		{
			if(isQuestCompelete)
			{		
				tweenScale.transform.Find("LevelUp").gameObject.SetActive(false);
			    SoundManager.Instance.PlaySound("UI_ReWuWanCheng");
			}
			else
			{
				tweenScale.transform.Find("QuestComplete").gameObject.SetActive(false);
				SoundManager.Instance.PlaySound("UI_LevelUp");	
			}			
			tweenScale.Reset();
   			tweenScale.Play(true);
		}
		finishLevelCtrl.WaitForTime(tweenScale.duration + 0.5f,PromptMoveOut);
    }
	
	private void PromptMoveOut()
	{
		TweenPosition tweenPosition = mSpecialEffectPrompt.GetComponentInChildren<TweenPosition>();
		TweenAlpha []tweenAlphaArr = mSpecialEffectPrompt.GetComponentsInChildren<TweenAlpha>();
		if(tweenPosition != null)
		{
			tweenPosition.Reset();
       		tweenPosition.Play(true);
			tweenPosition.onFinished = TweenFinished;
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
	
    void TweenFinished(UITweener tween)
    {
        if (mIsLevelUp)
        {
            CreateSpecialEffect(false);
            mIsLevelUp = false;
        }
		else
		{
			if(mSpecialEffectPrompt != null)
				GameObject.Destroy(mSpecialEffectPrompt);
		}
    }

    void JudgeLevelUp(int addExp)
    {
		int curLevel = (int)PlayerDataManager.Instance.Attrib.Level;
		Debug.Log("curlevel:"+curLevel);
		if(curLevel == CommonData.UserMaxLevel)
			return;
        int curExp = (int)PlayerDataManager.Instance.Attrib.CurExp;
        int nextExp = (int)PlayerDataManager.Instance.Attrib.NextExp;
        if (curExp + addExp >= nextExp)
        {
            mIsLevelUp = true;
        }
    }

    void UpdatePakageData()
    {
        Request(new GetBackPackCmd(), delegate(string err, Response response)
        {
            if (!string.IsNullOrEmpty(err))
            {
                //MessageBoxWnd.Instance.Show(LanguagesManager.Instance.GetItem((int)LanguageID.).Text);
                Debug.LogError("Request pakage Info Error!!" + err);
                return;
            }

            Package data = (response != null) ? response.Parse<Package>() : null;
            // update the package
            if (data != null)
                PlayerDataManager.Instance.OnBackPack(data, false);

        });
    }

    void UpdatePlayerAttrib()
    {
        MainScript.Instance.Request(new GetUserAttribCmd(), delegate(string err, Response response)
        {
            //Global.ShowLoadingEnd();
            if (!string.IsNullOrEmpty(err))
            {
                MessageBoxWnd.Instance.Show(err, MessageBoxWnd.StyleExt.Alpha);
                return;
            }

            // update the user attribute.
            MainAttrib attrib = (response != null) ? response.Parse<MainAttrib>() : null;
            Debug.Log("Level: " + attrib.Level);
            if (attrib != null)
                PlayerDataManager.Instance.OnMainAttrib(attrib);

            UpdateLocalPlayerTopUI();
        });
    }

    void UpdateLocalPlayerTopUI()
    {
        Transform unitName = UnitManager.Instance.LocalPlayer.UGameObject.transform.Find("UnitTopUI(Clone)/UnitName");
        unitName.GetComponent<UILabel>().text = "[FFFF00]" + string.Format("Lv. {0} {1}",
                            PlayerDataManager.Instance.Attrib.Level, PlayerDataManager.Instance.Attrib.Name) + "[-]";
    }

    public void OnClickFight(GameObject go)
    {
        if (mSelectQuest == null || (mLastSelectQuest != null && mLastSelectQuest == mSelectQuest))
            return;

        mLastSelectQuest = mSelectQuest;
        if (IsQuestComplete(mSelectQuest))
        {
            GetReward();
            return;
        }
        
        Close();

        if (!LevelSelectWnd.Exist)
        {
            LevelSelectWnd.Instance.EnterQuestLevel(mSelectQuest.Scene);
            LevelSelectWnd.Instance.Open();
        }
    }

    bool EnterLevel(int levelId)
    {
        LevelSetup levelSetup = LevelSetupManager.Instance.GetItem(levelId);
        if (levelSetup == null)
            return false;

        Close();

        Global.GLevelSetup = levelSetup;

        LevelHelper.LevelReset();
        LevelHelper.LevelStart(System.DateTime.Now);

        Global.CurSceneInfo.ID = ESceneID.Invalid;
        Global.CurSceneInfo.Type = ESceneType.Level;
        Global.CurSceneInfo.Name = levelSetup.Type;
        return MainScript.Instance.LoadLevel(Global.CurSceneInfo);
    }

    void InitQuest()
    {
        QuestBase[] questArray = QuestBaseManager.Instance.GetAllItem();
        int newQuestCount = 0;
        foreach (QuestBase questBase in questArray)
        {
            if (IsQuestFinish(questBase.ID) || questBase.Type > 1)
                continue;
            if (mQuestList.Count < questBase.Type)
                mQuestList.Add(new List<Quest>());
            Quest.EQuestState state = Quest.EQuestState.New;
            if (IsQuestComplete(questBase))
                state = Quest.EQuestState.Complete;
            else
                newQuestCount++;
            if (state == Quest.EQuestState.New && newQuestCount > 2)
                continue;
            Quest quest = new Quest(state, questBase);
            if (state == Quest.EQuestState.New && newQuestCount == 2)
                mNotOpenQuest = quest.QuestInfo.ID;
            mQuestList[quest.QuestInfo.Type - 1].Add(quest);
        }
    }

    void GetCompleteQuest()
    {
        Global.ShowLoadingStart();
        Request(new GetPveInfoCmd(), delegate(string err, Response response)
        {
            if (!string.IsNullOrEmpty(err))
            {
                Global.ShowLoadingEnd();
                Debug.LogError("GetPveInfo error!!!" + err);
                return;
            }
            PveInfo pveInfo = response != null ? response.Parse<PveInfo>() : null;
            if (pveInfo != null && pveInfo.Levels != null)
            {
                foreach (KeyValuePair<string, PveInfo.PveData> level in pveInfo.Levels)
                {
                    mCompleteLevel.Add(int.Parse(level.Key));
                    Debug.Log(int.Parse(level.Key));
                }
            }
            GetFinishQuestList();
        });
    }

    void GetFinishQuestList()
    {
        Request(new GetQuestListCmd(), delegate(string err, Response response)
        {
            if (!string.IsNullOrEmpty(err))
            {
                Global.ShowLoadingEnd();
                Debug.LogError("no  rewarder!!!");
                return;
            }
            QuestList list = response != null ? response.Parse<QuestList>() : null;
            if (list == null || list.quests == null)
            {
                Debug.Log("NULLLLLLLLLLLLLLLLLLLLLLLLLLL!!!");
                UpdateQuest();
                return;
            }
            mQuestFinish = list.quests;
            foreach (int id in list.quests)
                Debug.Log(id);
            UpdateQuest();
        });
    }

    void CreateQuestItem(int selectType)
    {
        if (mQuestList.Count < selectType)
            return;
        Debug.Log(selectType);
        foreach (Quest quest in mQuestList[selectType - 1])
        {
            //if(IsQuestFinish(quest.ID))
            //	continue;
            GameObject obj = GameObject.Instantiate(mItemObj) as GameObject;
            obj.transform.parent = mQuestGrid.transform;
            obj.transform.localScale = Vector3.one;
            GameObject.Destroy(obj.GetComponent<UIPanel>());
            if (mQuestItemList.Count < selectType)
                mQuestItemList.Add(new List<QuestItem>());
            QuestItem item = new QuestItem(quest, obj);

            if (quest.State == Quest.EQuestState.Complete)
                item.QuestCompelet();

            item.OnSelectQuest = OnSelectQuest;
            mQuestItemList[selectType - 1].Add(item);
        }
    }

    bool IsQuestFinish(int questID)
    {
        if (mQuestFinish == null)
            return false;

        foreach (int id in mQuestFinish)
        {
            if (id == questID)
                return true;
        }
        return false;
    }

    bool IsQuestComplete(QuestBase quest)
    {
        if (mCompleteLevel == null)
            return false;
        foreach (int id in mCompleteLevel)
        {
            if (id == quest.Scene)
                return true;
        }
        return false;
    }

    void OnClickQuestType(GameObject go)
    {
        int selectType = int.Parse(go.name);
        if (mSelectQuestType == selectType)
            return;
        //if(mSelectTypeButton != null)
        //mSelectTypeButton.transform.localScale = Vector3.one;	
        Debug.Log("dfsdfsd" + mQuestItemList.Count);
        Debug.Log("selectType" + selectType);
        if (mQuestItemList.Count > 0 && mQuestItemList.Count < selectType)
        {
            MessageBoxWnd.Instance.Show("Coming soon...", MessageBoxWnd.Style.OK);
            return;
        }

        UpdateBtnScale(go);
        mLastSelectType = mSelectQuestType;
        mSelectQuestType = selectType;
        if (mSelectQuestItem != null)
        {
            mSelectQuestItem.transform.Find("QuestSelectFrame").gameObject.SetActive(false);
            mSelectQuestItem = null;
        }
        //go.transform.localScale = new Vector3(1, 1.5f, 1);;
        //mSelectTypeButton = go;

        ShowQuestList();
        if (mQuestItemList.Count >= mSelectQuestType && mQuestItemList[mSelectQuestType - 1].Count > 0 && mQuestGrid.transform.childCount > 0)
        {
            mQuestItemList[mSelectQuestType - 1][0].OnClickItem(null);
            mQuestGrid.transform.GetChild(0).gameObject.transform.Find("QuestSelectFrame").gameObject.SetActive(true);
        }
    }

    void UpdateBtnScale(GameObject go)
    {
        if (go == mSelectTypeButton)
            return;
        float magnifyTime = 1.2f;
        Vector3 pos;
        Vector3 scale;
        if (mSelectTypeButton != null)
        {
            mSelectTypeButton.transform.localScale = Vector3.one;
            scale = mSelectTypeButton.GetComponentInChildren<UILabel>().transform.localScale;
            scale.x /= magnifyTime;
            mSelectTypeButton.GetComponentInChildren<UILabel>().transform.localScale = scale;
            pos = mSelectTypeButton.transform.localPosition;
            pos.y -= mSelectTypeButton.transform.Find("Background").localScale.y * (magnifyTime - 1) / 2;
            mSelectTypeButton.transform.localPosition = pos;
        }
        go.transform.localScale = new Vector3(1, magnifyTime, 1);
        scale = go.GetComponentInChildren<UILabel>().transform.localScale;
        scale.x *= magnifyTime;
        go.GetComponentInChildren<UILabel>().transform.localScale = scale;
        pos = go.transform.localPosition;
        pos.y += go.transform.Find("Background").localScale.y * (magnifyTime - 1) / 2;
        go.transform.localPosition = pos;
        mSelectTypeButton = go;
    }

    void ShowQuestList()
    {
        if (mLastSelectType > 0 && mQuestItemList.Count >= mLastSelectType)
        {
            foreach (QuestItem item in mQuestItemList[mLastSelectType - 1])
                item.Hide();
            ClearDesc();
        }
        if (mQuestItemList.Count < mSelectQuestType)
            CreateQuestItem(mSelectQuestType);
        else
            ShowQuestItem();

        mQuestGrid.GetComponent<UIGrid>().Reposition();
    }

    void ShowQuestItem()
    {
        for (int i = 0; i < mQuestItemList[mSelectQuestType - 1].Count; i++)
        {
            QuestItem item = mQuestItemList[mSelectQuestType - 1][i];
            if (IsQuestFinish(item.Quest.QuestInfo.ID))
            {
                item.Remove();
                mQuestItemList[mSelectQuestType - 1].Remove(item);
                continue;
            }
            if (item.Quest.State == Quest.EQuestState.Complete)
                item.QuestCompelet();
            item.Show();
        }
    }

    void ShowQuestDesc(QuestBase quest)
    {
        mCheckBox.SetActive(true);
        mQuestName.GetComponent<UILabel>().text = quest.Name;
        mQuestDesc.GetComponent<UILabel>().text = quest.Desc;
        mTarget.GetComponent<UILabel>().text = quest.Target;
    }

    void ShowReward(QuestBase quest)
    {
        for (int i = 0; i < mRewardGrid.transform.childCount; i++)
        {
            mRewardGrid.transform.GetChild(i).gameObject.SetActive(false);
            GameObject.Destroy(mRewardGrid.transform.GetChild(i).gameObject);
        }
        if (quest.Gold > 0)
        {
            GameObject gold = GameObject.Instantiate(mRewardObj) as GameObject;
            gold.transform.Find("Sprite").GetComponent<UISprite>().spriteName = "Icon01_System_Gold_01";
            InitReward(gold, quest.Gold);
        }
        if (quest.Gem > 0)
        {
            GameObject gem = GameObject.Instantiate(mRewardObj) as GameObject;
            gem.transform.Find("Sprite").GetComponent<UISprite>().spriteName = "Icon01_System_Gem_01";
            InitReward(gem, quest.Gem);
        }
        if (quest.Exp > 0)
        {
            GameObject exp = GameObject.Instantiate(mRewardObj) as GameObject;
            exp.transform.Find("Sprite").GetComponent<UISprite>().spriteName = "Icon01_System_Xp_01";
            InitReward(exp, quest.Exp);
        }
        if (quest.SP > 0)
        {
            GameObject sp = GameObject.Instantiate(mRewardObj) as GameObject;
            sp.transform.Find("Sprite").GetComponent<UISprite>().spriteName = "Icon01_System_Sp_01";
            InitReward(sp, quest.SP);
        }
        mRewardGrid.GetComponent<UIGrid>().Reposition();

        for (int i = 0; i < mItemGrid.transform.childCount; i++)
        {
            mItemGrid.transform.GetChild(i).gameObject.SetActive(false);
            GameObject.Destroy(mItemGrid.transform.GetChild(i).gameObject);
        }
        if (!string.IsNullOrEmpty(quest.Items))
        {
            string[] itemArray = quest.Items.Split('|');
            for (int i = 0; i < itemArray.Length; i++)
            {
                ItemBase itemBase = ItemBaseManager.Instance.GetItem(int.Parse(itemArray[i]));
                if (itemBase != null)
                {
                    if (itemBase.Role != PlayerDataManager.Instance.Attrib.Role)
                    {
                        i++;
                        continue;
                    }
                    GameObject obj = GameObject.Instantiate(mItemBox) as GameObject;
                    obj.transform.parent = mItemGrid.transform;
                    obj.transform.localScale = Vector3.one;
                    i++;
                    obj.transform.Find("Icon/Background").GetComponent<UISprite>().spriteName = itemBase.Icon;
                    obj.transform.Find("CountNo/LabelCount").GetComponent<UILabel>().text = itemArray[i];
                    UpdateItemQualityFrameIcon(obj.transform.Find("Quality").gameObject,itemBase);
                }
            }
        }
        mItemGrid.GetComponent<UIGrid>().Reposition();
        mFightSprite.gameObject.SetActive(false);
        mClaimSprite.gameObject.SetActive(false);
    }

    void UpdateFightButtonState(QuestBase quest)
    {
        if (quest.ID == mNotOpenQuest)
        {
            mFightButton.gameObject.transform.Find("Label").GetComponent<UILabel>().text = "ս��";
            mFightButton.isEnabled = false;
        }
        else
        {
            if (IsQuestComplete(quest))
            {
                mFightButton.gameObject.transform.Find("Label").GetComponent<UILabel>().text = "�콱";
                mFightButton.normalSprite = "Button01_Normal_01";
                mCheck.SetActive(true);
                mClaimSprite.gameObject.SetActive(true);
                mFightButton.isEnabled = true;
            }
            else
            {
                mFightButton.gameObject.transform.Find("Label").GetComponent<UILabel>().text = "ս��";
                mFightButton.normalSprite = "Button01_Normal_02";
                Debug.Log("dasdasdadasdasdasd");
                mCheck.SetActive(false);
                mFightSprite.gameObject.SetActive(true);
                mFightButton.isEnabled = true;
            }
            mFightButton.hoverSprite = mFightButton.normalSprite;
            mFightButton.pressedSprite = mFightButton.normalSprite;
        }
    }

    void InitReward(GameObject obj, int reward)
    {
        GameObject.Destroy(obj.GetComponent<UIPanel>());
        obj.transform.Find("Sprite").GetComponent<UISprite>().MakePixelPerfect();
        obj.transform.parent = mRewardGrid.transform;
        obj.transform.localScale = Vector3.one;
        obj.transform.Find("Label").GetComponent<UILabel>().text = reward.ToString();
    }

    void OnSelectQuest(GameObject go, QuestBase quest)
    {
        if (go == null || (mSelectQuestItem != null && go.Equals(mSelectQuestItem)))
            return;

        if (mSelectQuestItem != null)
            mSelectQuestItem.transform.Find("QuestSelectFrame").gameObject.SetActive(false);

        mSelectQuestItem = go;
        mSelectQuest = quest;

        ShowReward(quest);
        ShowQuestDesc(quest);
        UpdateFightButtonState(quest);
    }

    void ClearDesc()
    {
        mCheckBox.SetActive(false);
        mCheck.SetActive(false);
        for (int i = 0; i < mRewardGrid.transform.childCount; i++)
        {
            mRewardGrid.transform.GetChild(i).gameObject.SetActive(false);
            GameObject.Destroy(mRewardGrid.transform.GetChild(i).gameObject);
        }
        for (int i = 0; i < mItemGrid.transform.childCount; i++)
        {
            mItemGrid.transform.GetChild(i).gameObject.SetActive(false);
            GameObject.Destroy(mItemGrid.transform.GetChild(i).gameObject);
        }
        mQuestName.GetComponent<UILabel>().text = "";
        mQuestDesc.GetComponent<UILabel>().text = "";
        mTarget.GetComponent<UILabel>().text = "";
    }
}