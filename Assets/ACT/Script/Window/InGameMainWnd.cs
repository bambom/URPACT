using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class InGameMainWnd : Window<InGameMainWnd>
{
	public override string PrefabName { get { return "InGameMainWnd"; } }
	UILabel mLabelOne;
	UILabel mLabelTwo;
	UILabel mTaskNum;
	UISprite mTaskBgSprite;
	GameObject mChatMessage;
	GameObject mQuestButton;
	GameObject mGoResbarRoot;
	GameObject mMailRemindObj;
	GameObject mChatRemindObj;
	GameObject mTaskRemindObj;
	GameObject mFacebookObj;
	GameObject mExtBtnStatus;
	GameObject mPackageStatus;
	GameObject mSkillStatus;
	ResourceBar mResourceBar;
	public ResourceBar ResourceBar{get{return mResourceBar;}}
	ParticleSystem mNpcParticleSystem;
	
	static bool mUIExtStatus = false;
	public static bool UIExtStatus { get { return mUIExtStatus ;}}
	
	ENPCID mClickNpcId = ENPCID.INVALID;
	
	
	UIGuideShow mUIGuideShow = null;
    protected override bool OnOpen()
    {	
		Init ();
		UpdateQuestButton();
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
	
	public override void Show() 
	{
		if (Exist)
		{
			WndObject.SetActive(true);
			InitInfoLabel();
			UpdateQuestButton();
			if(null == mUIGuideShow )
				mUIGuideShow = new UIGuideShow(PrefabName);
			if(mUIGuideShow.GuideCheck()){
				mUIGuideShow.GuideStart();
			}
		}
	}

    protected override bool OnHide()
    {
        if (NGUIJoystick.instance)
            NGUIJoystick.instance.ResetJoystick();
		
		if( mUIGuideShow != null ){
			mUIGuideShow.Destroy();
			mUIGuideShow = null;
		}
		
        return base.OnHide();
    }
	
	void UIPosSitToScreen()
	{
		Debug.Log("Screen.Wid: " + Screen.width +"Screen.height: " +Screen.height);
		Control("LeftUpRoot").transform.localPosition = new Vector3( (0-Screen.width/2),(Screen.height/2),0.0f)*UIHelper.WordToScreenModify;
		//Control("LeftDownRoot").transform.localPosition = new Vector3( (0-Screen.width/2),(0-Screen.height/2),0.0f);
		Control("RightUpRoot").transform.localPosition = new Vector3( (Screen.width/2),(Screen.height/2),0.0f)*UIHelper.WordToScreenModify;
		Control("RightDownRoot").transform.localPosition = new Vector3( (Screen.width/2),(0-Screen.height/2),0.0f)*UIHelper.WordToScreenModify;
	}
	
	void CreateGameObjectByPrefabName(string Name)
	{
		GameObject go = GameObject.Instantiate(Resources.Load(Name)) as GameObject;
		go.transform.parent = Control(Name+"Root").transform;
		go.transform.localPosition = Vector3.zero;
		go.transform.localScale = Vector3.one;
	}
	
	
	void ObjectInit()
	{
		CreateGameObjectByPrefabName("LeftUp");
		//CreateGameObjectByPrefabName("LeftDown");
		CreateGameObjectByPrefabName("RightUp");
		CreateGameObjectByPrefabName("RightDown");
		
		//SetSystemStatus(false);
		ResourceBarInit();
		UIEventListener.Get(Control("BtnExt")).onClick = OnClickBtnExt;
		UIExtInit();
		NpcDialogueInit();
	}
	
	void OpenSigninWnd()
	{
		//Hide();
		if(!SignInWnd.Exist)
			SignInWnd.Instance.Open();
	}

	void OnClickedAddMoney(GameObject go)
	{
		Hide();
	}
	
	
	
	void OnClickAddPhyiscOk(GameObject go)
	{
		RequestBuyPhysicInfo();
	}
	
	void OnClickAddPhyiscCancle(GameObject go)
	{
		
	}
	
	void OnClickedAddPhysic(GameObject go)
	{
//		StrengthInfo Phy = PlayerDataManager.Instance.StrengthInfo;
//		StrengthBase SBase =  StrengthBaseManager.Instance.GetItem(Phy.time+1);
//		MessageBoxWnd.Instance.Show("Cost Gem :" + SBase.Gem.ToString());
//		MessageBoxWnd.Instance.OnClickedOk = OnClickAddPhyiscOk;
//		MessageBoxWnd.Instance.OnClickedCancle = OnClickAddPhyiscCancle;
//		
//	    if(!StrengthWnd.Exist)
//			StrengthWnd.Instance.Open();
//	    StrengthWnd.Instance.WndObject.transform.localPosition = new Vector3(0,0,-20f);
	}
	
	void ResourceBarInit()
	{
		mGoResbarRoot = Control("ResBarRoot");
		mResourceBar = new ResourceBar(mGoResbarRoot);
		mResourceBar.ClickedAddMoney = OnClickedAddMoney;
		mResourceBar.ClickedAddPhysic = OnClickedAddPhysic;
		
		RequestPhysicInfo();
	}
	
	
	private void Init()
	{
		UIPosSitToScreen();
		ObjectInit();
		mUIExtStatus = false;
		mChatMessage = Control("ChatMessage");
		mLabelOne = Control("LabelOne").GetComponent<UILabel>();
		mLabelTwo = Control("LabelTwo").GetComponent<UILabel>();
		mTaskNum =	Control ("QuestState",Control("TipsPanel")).GetComponent<UILabel>();
		mTaskRemindObj = mTaskNum.transform.parent.gameObject;
		mTaskBgSprite = mTaskRemindObj.GetComponentInChildren<UISprite>();
		mQuestButton = Control("TaskEntry");     
		mMailRemindObj = Control("MailBox").transform.Find("MailRemind").gameObject;
		mChatRemindObj = Control("ChatMessage").transform.Find("ChatIcon").gameObject;
        mFacebookObj = Control("FacebookButton");
		mPackageStatus = Control("PackStatus");
		mSkillStatus = Control("SkillStatus");
		mExtBtnStatus = Control("ExtStatus");
        if (mFacebookObj)
        {
            if (FacebookHelper.IsLoggedIn)
                UIEventListener.Get(mFacebookObj).onClick = OnClickFacebook;
            else
                mFacebookObj.SetActive(false);
        }
		
		UIEventListener.Get(Control("LevelEntry")).onClick = OnClickLevelEntry;
		UIEventListener.Get(Control("ChatMessage")).onClick = OnClickChatWnd;
		UIEventListener.Get(Control("TaskEntry")).onClick = OnClickQuestWnd;
		//UIEventListener.Get(Control("PvPEntry")).onClick = OnClickPvPWnd;
		UIEventListener.Get(Control ("PrayEntry")).onClick = OnClickPrayWnd;
		UIEventListener.Get(Control("Friends")).onClick = OnClickFriendsWnd;
		UIEventListener.Get(Control("Package")).onClick = OnClickPackageWnd;
		UIEventListener.Get(Control("Skill")).onClick = OnClickSkillWnd;
		UIEventListener.Get(Control("TopList")).onClick = OnClickTopListWnd;
		UIEventListener.Get(Control("MailBox")).onClick = OnClickMailBoxWnd;
		UIEventListener.Get(Control("System")).onClick = OnClickSystemWnd;
		UIEventListener.Get(Control("Skill")).onClick = OnClickSkillWnd;
		UIEventListener.Get(Control("NpcShop")).onClick = OnClickNpcShopWnd;
		InitInfoLabel ();	
		Global.Pause = false;
		
		
		if(null == mUIGuideShow )
			mUIGuideShow = new UIGuideShow(PrefabName);
		if(mUIGuideShow.GuideCheck()){
			mUIGuideShow.GuideStart();
		}else{
			OpenSigninWnd();
		}
		
	}
	
	

	
	
	bool CheckNewInfo()
	{
		bool bNew = false;
		SkillDataManager.Instance.UpdateSkillItem();
		List<ItemBase>skill = SkillDataManager.Instance.GetCanLearnSkill();
		if(skill.Count > 0)
		{	
			if(!mSkillStatus.activeSelf)
				 mSkillStatus.SetActive(true);
			bNew = true;
		}
		else
		{
			if(mSkillStatus.activeSelf)
				mSkillStatus.SetActive(false);
		}
		
		//NewPackageStatus
		int nNewCount = BackPack.PackageCheckNewCout();
		if( nNewCount > 0)
		{
			if(!mPackageStatus.activeSelf)
				mPackageStatus.SetActive(true);
			NewInfoTipsCountSet(mPackageStatus , nNewCount);
			bNew = true;
		}else{
			if(mPackageStatus.activeSelf)
				mPackageStatus.SetActive(false);
		}
		return bNew;
	}
	
	
	private void InitInfoLabel()
	{	
		mMailRemindObj.GetComponent<TweenScale>().enabled = Global.NewMailCome;
		mMailRemindObj.SetActive(Global.NewMailCome);	
				
		if(InGameMainWnd.Instance.WndObject.GetComponent<GetClick>() == null)
			InGameMainWnd.Instance.WndObject.AddComponent<GetClick>();
		
		if(Chat.Instance.SendToMeName.Count != 0){
			mChatRemindObj.GetComponent<TweenScale>().enabled = true;
			mChatRemindObj.SetActive(true);
		}
		switch (Chat.Instance.WorldMessages.Count)
		{
			case 0:
				break;
			case 1:
	            ChatMessage chatMessage = Chat.Instance.WorldMessages[Chat.Instance.WorldMessages.Count - 1];
				mLabelTwo.text = "[00FF00]"+chatMessage.user+":[-] "+chatMessage.msg;
				break;
			default:
	            ChatMessage otherMessage = Chat.Instance.WorldMessages[Chat.Instance.WorldMessages.Count - 1];
				mLabelTwo.text = "[00FF00]"+otherMessage.user+":[-]" +otherMessage.msg;
	            otherMessage = Chat.Instance.WorldMessages[Chat.Instance.WorldMessages.Count - 2];
				mLabelOne.text = "[00FF00]"+otherMessage.user+":[-]" +otherMessage.msg;
				break;		
		}
		
		MainAttrib attrib = PlayerDataManager.Instance.Attrib;
		StrengthInfo Phy = PlayerDataManager.Instance.StrengthInfo;
		mResourceBar.Update(attrib.Gold,attrib.Gem,attrib.SP,Phy.value);
	    
		if(Chat.Instance.SendToMeName.Count != 0 || Global.NewMailCome)
			SoundManager.Instance.PlaySound("UI_XiTongTiShi");
	
		if( CheckNewInfo() == true )
		{
			if(!mExtBtnStatus.activeSelf)
				mExtBtnStatus.SetActive(true);
		}
		else
		{
			if( mExtBtnStatus.activeSelf )
				mExtBtnStatus.SetActive(false);
		}
	}
	
	void OnClickChatWnd( GameObject go )
	{
        if (SystemWnd.Exist)
            SystemWnd.Instance.Close();
        if (!ChatWnd.Exist)
            ChatWnd.Instance.Open();
	}
	
	#region PanelBtn
	void OnClickNpcDialogueBackPack()
	{
		Hide();
		if(PackageWnd.Exist)
			PackageWnd.Instance.Show();
		else
			PackageWnd.Instance.Open();
	}
	
	void OnClickNpcDialogueStore()
	{
		Hide ();
		if(NpcShopWnd.Exist)
			NpcShopWnd.Instance.Show();
		else
			NpcShopWnd.Instance.Open();
	}
	
	void OnClickNpcDialogueSkill()
	{
		Hide ();
		if(SkillWnd.Exist)
			SkillWnd.Instance.Show();
		else
			SkillWnd.Instance.Open();
	}
	
	void OnClickNpcDialogueTopList()
	{
		Hide ();
		if(TopListWnd.Exist)
			TopListWnd.Instance.Show();
		else 
			TopListWnd.Instance.Open();
	}
	
	void OnClickNpcDialoguePvP()
	{
		Hide();
		if(PvPWnd.Exist)
			PvPWnd.Instance.Show();
		else
			PvPWnd.Instance.Open();

	}
	
	void OnClickNpcDialogueEmail()
	{
		Hide();
		if(!MailWnd.Exist)
			MailWnd.Instance.Open();
		else
			MailWnd.Instance.Show();
		Global.NewMailCome = false;
		UnitManager.Instance.LocalPlayer.UGameObject.SendMessage("LockInput","true");
	}
	
	
	void OnClickNpcDialogueQuest()
	{
		Hide();
		if(QuestWnd.Exist)
			QuestWnd.Instance.Show();
		else 
			QuestWnd.Instance.Open();
	}
	
	void OnClickNpcDialogueGameEntry()
	{
		Hide();
		if(LevelSelectWnd.Exist)
			LevelSelectWnd.Instance.Show();
		else
			LevelSelectWnd.Instance.Open();
	}
	
	
	void OnClickNpcDialogue(GameObject go)
	{
		switch(mClickNpcId)
		{			
			case ENPCID.BackPack:OnClickNpcDialogueBackPack(); break;
			case ENPCID.Skill: OnClickNpcDialogueSkill();break;
			case ENPCID.PvP:OnClickNpcDialoguePvP();break;
			case ENPCID.TopList:OnClickNpcDialogueTopList();break;		
			case ENPCID.Store: OnClickNpcDialogueStore(); break;	
			case ENPCID.Quest: OnClickNpcDialogueQuest();break;	
			case ENPCID.Activity:OnClickNpcDialogueEmail(); break;	
			case ENPCID.GAMESTARTDOOR: OnClickNpcDialogueGameEntry();break;
			default:break;
		}
	}
	
	
	void NpcDialogueInit()
	{
		GameObject go = Control("NpcDiaRoot").transform.Find("NpcDialogue").gameObject;
		mNpcParticleSystem = go.GetComponentInChildren<ParticleSystem>();
		UIEventListener.Get(go).onClick = OnClickNpcDialogue;
		go.SetActive(false);
	}
	
	public void NpcDialogueSet(bool Status,ENPCID npcId)
	{
		Control("NpcDiaRoot").transform.Find("NpcDialogue").gameObject.SetActive(Status);
		//control Particle Emit or not
		if(Status)
			mNpcParticleSystem.Play();
		else 
			mNpcParticleSystem.Stop();
		if(!Status){
			mClickNpcId = ENPCID.INVALID;
			return;
		}
		GameObject goIcon = Control("NpcDiaRoot").transform.Find("NpcDialogue/Icon").gameObject;
		mClickNpcId = npcId ;
		UISprite NpcSp = goIcon.GetComponent<UISprite>();
		switch(npcId)
		{			
			case ENPCID.BackPack: 
				 NpcSp.spriteName = "Button15_MenuPackage_Normal_01"; 
					break;
			case ENPCID.Skill:
				 NpcSp.spriteName = "Button15_MenuSkill_Normal_01";
					break;
			case ENPCID.PvP:
				 NpcSp.spriteName = "Button15_MenuPvp_Normal_01";	
					break;
			case ENPCID.TopList:
			 	 NpcSp.spriteName = "Button15_MenuRank_Normal_01";	
					break;		
			case ENPCID.Store:
				 NpcSp.spriteName = "Button15_MenuStore_Normal_01";
					break;	
			case ENPCID.Quest:
				 NpcSp.spriteName = "Button15_MenuQuest_Normal_01";
					break;	
			case ENPCID.Activity:
				 NpcSp.spriteName = "Button15_MenuMail_Normal_01";
					break;	
			case ENPCID.GAMESTARTDOOR:
				 NpcSp.spriteName = "Button15_MenuWorld_Normal_01";
					break;	
		}
		NpcSp.MakePixelPerfect(1.30f);
	}
	
	void UIExtInit()
	{
		Control("BtnExt").transform.Find("IconArrow").gameObject.SetActive(mUIExtStatus);	
	}
	
	void UIExtReset()
	{
		UIExtInit();
	}
	
	
	public void OnClickBtnExt(GameObject go)
	{
		TweenPosition Tp = go.transform.parent.gameObject.GetComponent<TweenPosition>();
		if( Tp != null){
			GameObject.Destroy(Tp);
			Tp = null;
		}
		
		Tp = go.transform.parent.gameObject.AddComponent<TweenPosition>();
		Tp.method = UITweener.Method.Linear;
		Tp.delay = 0.0f;
		Tp.duration = 0.3f;
		if(go.transform.parent.transform.localPosition.x < 100){
			Tp.from = Vector3.zero;
			Tp.to = new Vector3(270.0f,0.0f,0.0f);
		}else{
			Tp.to = Vector3.zero;
			Tp.from = new Vector3(270.0f,0.0f,0.0f);
		}
		mUIExtStatus = !mUIExtStatus;
		//NGUIDebug.Log("UIExtStatus = " + UIExtStatus );
		UIExtReset();
	}
	
	void OnClickNpcShopWnd( GameObject go )
	{
		OnClickNpcDialogueStore();
	}
	
	void OnClickPvPWnd( GameObject go )
	{
		if(SystemWnd.Exist)
			SystemWnd.Instance.Close();
		Hide();
		if(PvPWnd.Exist)
			PvPWnd.Instance.Show();
		else
			PvPWnd.Instance.Open();
	}
	
	
	public void OnClickQuestWnd( GameObject go )
	{
		if(SystemWnd.Exist)
			SystemWnd.Instance.Close();
		Hide();
		if(!QuestWnd.Exist)
			QuestWnd.Instance.Open();
	}
	
	void OnClickLevelEntry( GameObject go)
	{
		if(SystemWnd.Exist)
			SystemWnd.Instance.Close();
		Hide();
		
		if(!LevelSelectWnd.Exist)
			LevelSelectWnd.Instance.Open();
	}
	
	void OnClickTopListWnd( GameObject go)
	{
		if(SystemWnd.Exist)
			SystemWnd.Instance.Close();
		if( !TopListWnd.Exist )
			TopListWnd.Instance.Open();
		Hide();
	}
	/// <summary>
	/// 点击进入邮箱
	/// </summary>
	void OnClickMailBoxWnd( GameObject go)
	{
		if(SystemWnd.Exist)
			SystemWnd.Instance.Close();
		Hide();
		if(!MailWnd.Exist)
			MailWnd.Instance.Open();
		else
			MailWnd.Instance.Show();
		Global.NewMailCome = false;
		///人物暂停
		UnitManager.Instance.LocalPlayer.UGameObject.SendMessage("LockInput","true");

	}
	
	public void SetSystemStatus(bool Status)
	{
		Control("System").transform.Find("Foreground").gameObject.SetActive(Status);
	}
	
	
	void OnClickSystemWnd( GameObject go)
	{
		SetSystemStatus(true);
		if(!SystemWnd.Exist){
			SystemWnd.Instance.Open();
			SystemWnd.Instance.Reset(true);
		}
	}
	
	void OnClickSkillWnd( GameObject go )
	{
		if(SystemWnd.Exist)
			SystemWnd.Instance.Close();
		Hide();
		if( !SkillWnd.Exist )
			SkillWnd.Instance.Open();
	}
	void OnClickPrayWnd( GameObject go )
	{
		if(SystemWnd.Exist)
			SystemWnd.Instance.Close();
		Hide ();
		if( !WishingWellWnd.Exist )
			WishingWellWnd.Instance.Open();
	}
	
	void OnClickFriendsWnd( GameObject go)
	{
		if(SystemWnd.Exist)
			SystemWnd.Instance.Close();
		Hide();
		if(!FriendsListWnd.Exist)
			FriendsListWnd.Instance.Open();
	}
	
	void OnClickPackageWnd( GameObject go)
	{
		if(SystemWnd.Exist)
			SystemWnd.Instance.Close();
		Hide();
		//if( !PackageWnd.Exist )
			PackageWnd.Instance.Open();
	}
	
	void OnClickGenius(GameObject go)
	{
		if(SystemWnd.Exist)
			SystemWnd.Instance.Close();
		Hide();
		if(!GeniusWnd.Exist)
			GeniusWnd.Instance.Open();
	}
	#endregion
	
	//======================================================================================
	void OnGetPhysicSuccess()
	{
		MainAttrib attrib = PlayerDataManager.Instance.Attrib;
		StrengthInfo Phy = PlayerDataManager.Instance.StrengthInfo;
		mResourceBar.Update(attrib.Gold,attrib.Gem,attrib.SP,Phy.value);
	}

    void OnClickFacebook(GameObject go)
    {
        FacebookHelper.PublishMessage("");
    }
	
	void RequestPhysicInfo()
	{
		Request(new GetStrengthCmd(), delegate(string err, Response response)
        {
			if (!string.IsNullOrEmpty(err))
            {
				MessageBoxWnd.Instance.Show( err ,MessageBoxWnd.StyleExt.Alpha );
				return;
			}
            StrengthInfo Physic = (response != null) ? response.Parse<StrengthInfo>() : null;
            if (Physic != null)
            {
                PlayerDataManager.Instance.OnStrengthInfo(Physic);
            }
			OnGetPhysicSuccess();
		});
	}
	
	void OnBuySuccess()
	{
		RequestAttribInfo();
		RequestPhysicInfo();
	}
	
	void RequestBuyPhysicInfo()
	{
		Request(new BuyStrengthCmd(), delegate(string err, Response response)
        {
			if (!string.IsNullOrEmpty(err))
            {
				MessageBoxWnd.Instance.Show( err ,MessageBoxWnd.StyleExt.Alpha );
				return;
			}
			OnBuySuccess();
		});
	}
	
	void OnClickedOk( GameObject go )
	{
		if(!Exist)
			Instance.Open();
	}
	
	void OnClickedCancle( GameObject go )
	{
		if(!Exist)
			Instance.Open();
	}
	
	void RequestPackageInfo()
	{
        Request(new GetBackPackCmd(), delegate(string err, Response response)
        {
            if (!string.IsNullOrEmpty(err))
            {
				MessageBoxWnd.Instance.OnClickedCancle = OnClickedCancle;
				MessageBoxWnd.Instance.OnClickedOk = OnClickedOk;
				MessageBoxWnd.Instance.Show(err);
				if(!Exist)
					Instance.Open();
                return;
            }
            // update the user 
            Package BackPackData = (response != null) ? response.Parse<Package>() : null;
            if (BackPackData != null)
				PlayerDataManager.Instance.OnBackPack(BackPackData, false);
        });
		//ShowProcessLoadingStart();
	}
		
	void RequestAttribInfo()
	{
        Request(new GetUserAttribCmd(), delegate(string err, Response response)
        {
            if (!string.IsNullOrEmpty(err))
            {
				MessageBoxWnd.Instance.OnClickedCancle = OnClickedCancle;
				MessageBoxWnd.Instance.OnClickedOk = OnClickedOk;
				MessageBoxWnd.Instance.Show(err);
				if(!Exist)
					Instance.Open();
                return;
            }

            // update the user attribute.
            MainAttrib attrib = (response != null) ? response.Parse<MainAttrib>() : null;
            if (attrib != null)
                PlayerDataManager.Instance.OnMainAttrib(attrib);
        });
		//ShowProcessLoadingStart();
	}
	
	void UpdateQuestButton()
	{
		mTaskRemindObj.SetActive(false);
		Request(new GetQuestListCmd(), delegate(string err, Response response)
        {
            if (!string.IsNullOrEmpty(err))
            {
                Debug.LogError(err);
                return;
            }
			QuestList list = response != null ? response.Parse<QuestList>() : null;
			QuestBase[] questArray = QuestBaseManager.Instance.GetAllItem();
			List<QuestBase>existingQuest = new List<QuestBase>();
			foreach(QuestBase quest in questArray)
			{
				if( list.quests != null && HasQuest(list.quests, quest.ID))
					continue;
				existingQuest.Add(quest);
			}
			if(existingQuest.Count == 0)
			{
				mTaskRemindObj.GetComponent<TweenScale>().enabled = false;
				mTaskRemindObj.SetActive(false);
				return;
			}
			Request(new GetPveInfoCmd(), delegate(string error, Response res)
       		{
	            if (!string.IsNullOrEmpty(error))
	            {
	                Debug.LogError("GetPveInfo error!!!"+ error);
	                return;
	            }
				PveInfo pveInfo = res != null ? res.Parse<PveInfo>() : null; 
				List<int>completeLevel = new List<int>();
				if(pveInfo != null && pveInfo.Levels != null )
				{
					foreach(KeyValuePair<string, PveInfo.PveData>level in pveInfo.Levels)
					{
						completeLevel.Add(int.Parse(level.Key));
					}
				}
				int compeletQuestCount = 0;
				foreach(QuestBase quest in existingQuest)
				{
					if(completeLevel.Count>0 && HasQuest(completeLevel.ToArray(), quest.Scene))
						compeletQuestCount++;
				}
				if(compeletQuestCount > 0)
				{
					//mTaskNum.color = Color.yellow;
					mTaskRemindObj.SetActive(true);
					mTaskBgSprite.spriteName = "Button04_Remind__02";
					mTaskNum.text = compeletQuestCount.ToString();
					mTaskRemindObj.GetComponent<TweenScale>().enabled = true;
				}
				else
				{
					// keep 2 Existing Quest. always 2.
					//mTaskNum.color = Color.red;
					mTaskRemindObj.SetActive(true);
					mTaskRemindObj.transform.localScale = Vector3.one;
					mTaskBgSprite.spriteName = "Button04_Remind__03";
					if(existingQuest.Count == 1)
						mTaskNum.text = "1";
					else 
						mTaskNum.text = "2";//existingQuest.Count.ToString();
					mTaskRemindObj.GetComponent<TweenScale>().enabled = false;
					
				}
       		 });

        });

	}
	
	bool HasQuest(int[] target, int id)
	{
		for(int i=0; i<target.Length; i++)
		{
			if(target[i] == id)
				return true;
		}
		return false;
	}
}
