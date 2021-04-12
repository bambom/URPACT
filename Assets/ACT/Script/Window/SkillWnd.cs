using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SkillWnd : Window<SkillWnd>
{
    public override string PrefabName { get { return "SkillStudyWnd"; } }
	
	int mRoleID;
	List<List<ItemBase>>mSkillInType = new List<List<ItemBase>>();
	List<List<GameObject>>mSkillButtonArray = new List<List<GameObject>>();
	List<GameObject>mRuneArray = new List<GameObject>();
	List<int>mActivationSkillId = new List<int>();
	
	Object mSkillButton;
	Object mRuneItem;
	GameObject mSegment;
	GameObject mSkillDesc;
	GameObject mSkillName;
	GameObject mSelectFrame;
	GameObject mLastTypeButton;
	GameObject mUseButton;
	GameObject mLastChoose;
	GameObject mRuneButton;
	GameObject mNeedSp;
	GameObject mSp;
	GameObject mSkillItem;
	GameObject mSelectSlot;
	GameObject mLastSelectSlot;
	GameObject mNeedLv;
	GameObject mGoResbarRoot;
	GameObject mSelectSkillButton;
	GameObject mSkillType;
	
	int mCurrentPage = 0;
	int mSelectSkillID = 0;
	int mLastUseSkillID = 0;
	int mLastPage = -1;
	int mSelectIndex = 0;
	int mChooseRuneId = 0;
    int mChooseSlot = 1;
	int mRuneItemBaseId = 0;
	int mOpenSlot = 0;
	
	
	ResourceBar mResourceBar;
	
	UIImageButton mLearnSkillButton;
	TweenPosition mRuneTween;
	UIGuideShow mUIGuideShow = null;
	UILabel mSpRequire;
	
    protected override bool OnOpen()
    {
		WinStyle = WindowStyle.WS_CullingMask;

		LocalPlayer player = UnitManager.Instance.LocalPlayer as LocalPlayer;
		mRoleID = player.PlayerData.Attrib.Role;
		mSkillButton = Resources.Load("LearnSkillButton");
		//mSegment = Control("Segment");
		//mSegment.SetActive(false);
		mSelectFrame = Control("SelectFrame");
		mSkillDesc = Control("SkillDescription");
		mSkillName = Control("SkillName");
		mSkillItem = Control("SkillItem");
		mSkillType = Control("SkillType");
		mNeedSp = Control("NeedSP");
		//mSp = Control("SP");
		mNeedLv = Control("NeedLevel");
		mNeedLv.SetActive(false);
		mLearnSkillButton = Control("LearnSkill").GetComponent<UIImageButton>();
		UIEventListener.Get(mLearnSkillButton.gameObject).onClick = OnClickLearnButton;
		mRuneButton = Control("SkillStone");
		UIEventListener.Get(mRuneButton).onClick = OnOpenRuneWindow;
		UIEventListener.Get(Control("Studded")).onClick = OnSkillStrengthen;
		//mUseButton = Control("UseSkill");
		//UIEventListener.Get(mUseButton).onClick = OnUseSkill;
		mSpRequire= Control("SPNotEnough").GetComponent<UILabel>();
		mRuneTween = Control("RuneCompose").GetComponent<TweenPosition>();
		mRuneTween.onFinished = MoveRuneComposeFinish;
		
		InitSkillType();
		UpdateSkillData();
		InitAllSkill();
		SetTypeButtonIcon();
		UpdateActivationSkill();
		
		UIEventListener.Get(Control("Return")).onClick = OnClickbtnReturn;
		
		mGoResbarRoot = Control("ResBarRoot");
		ResourceBarInit();
//		mGemLabel = Control("LabelGem").GetComponent<UILabel>();
//      mGoldLabel = Control("LabelGold").GetComponent<UILabel>();	
//		mEnergy	= Control ("LabelEnergy").GetComponent<UILabel>();
		
		OnClickTypeButton(WndObject.transform.Find("SkillType/1").gameObject);
		
		mUIGuideShow = new UIGuideShow(PrefabName);
		if(mUIGuideShow.GuideCheck()){
			mUIGuideShow.GuideStart();
		}
		
        //InitAttrib();
		//ShowSkillButton(1);
		WinStyle = WindowStyle.WS_Ext;
        return base.OnOpen();
    }
	
	void MoveRuneComposeFinish( UITweener tween )
	{
		if( mUIGuideShow == null )
			mUIGuideShow = new UIGuideShow(PrefabName);
		if(mUIGuideShow.GuideCheck()){
			mUIGuideShow.GuideStart();
		}
	}
	
	#region ResourceBar
	void OnClickedAddMoney(GameObject go)
	{
		Close ();
	}
	
	void OnClickedAddPhysic(GameObject go)
	{
		
		
	}
	
	void UpdateResourceBar()
	{
		MainAttrib attrib = PlayerDataManager.Instance.Attrib;
		mResourceBar.Update(attrib.Gold,attrib.Gem,attrib.SP,-1);
	}
	
	void ResourceBarInit()
	{
	 	MainAttrib attrib = PlayerDataManager.Instance.Attrib;
		mResourceBar = new ResourceBar(mGoResbarRoot);
		mResourceBar.ClickedAddMoney = OnClickedAddMoney;
		mResourceBar.ClickedAddPhysic = OnClickedAddPhysic;
		mResourceBar.Update(attrib.Gold,attrib.Gem,attrib.SP,-1);
		mResourceBar.UpdateByResType(ResourceBar.ResType.RT_NonPhysic);
	}
	#endregion ResourceBar
	
	public	void OnClickbtnReturn ( GameObject go)
	{
		if( mUIGuideShow != null ){
			mUIGuideShow.Destroy();
			mUIGuideShow = null;
		}
		Close();
		if(!InGameMainWnd.Exist)
			InGameMainWnd.Instance.Open();
		else
			InGameMainWnd.Instance.Show();
	}
	
	private void InitAttrib()
	{
		Debug.Log("SP:" + PlayerDataManager.Instance.Attrib.SP);
		mSp.transform.Find("Label").GetComponent<UILabel>().text = PlayerDataManager.Instance.Attrib.SP.ToString();
	}
	
    protected override bool OnClose()
    {
        return base.OnClose();
    }
	
	void InitSkillType()
	{
		RoleSkillClass[] skillType = RoleSkillClassManager.Instance.GetCollection(mRoleID).ToArray();
		for(int i=0; i<skillType.Length; i++)
		{
			if(skillType[i].SubType == 2)
				continue;
			string name = string.Format("{0}", i+1);
			GameObject typeButton = Control(name);
			UIEventListener.Get(typeButton).onClick = OnClickTypeButton;
			//typeButton.transform.Find("Icon").GetComponent<UISprite>().spriteName = skillType[i].Icon;
			typeButton.transform.Find("Select").gameObject.SetActive(false);
			SetTypeButtonBackground(skillType[i].Type, typeButton.transform.Find("Background").GetComponent<UISprite>());
			mSkillInType.Add( new List<ItemBase>() );
			mSkillButtonArray.Add( new List<GameObject>() );
		}
	}
	
	void SetTypeButtonBackground(int type,  UISprite sprite)
	{
		switch(type)
		{
		case 1: sprite.spriteName = "Button21_Attack_Jobskill_03";
				break;
		case 2: sprite.spriteName = "Button21_Attack_Jobskill_01";
				break;
		case 3: sprite.spriteName = "Button21_Attack_Jobskill_02";
				break;
		case 4: sprite.spriteName = "Button21_Attack_Blastkill_01";
				break;
		default: break;
		}
	}
	
	void InitAllSkill()
	{
		ItemBase[] itemArray = ItemBaseManager.Instance.GetAllItem();
		foreach(ItemBase item in itemArray)
		{
			if(item.Role == mRoleID && item.MainType == (int)EItemType.Skill)
			{
				// ignore passive skill.
				if(item.SubType != 2)
				{
					SkillBase skill = SkillBaseManager.Instance.GetItem(item.ID);
					mSkillInType[skill.Type - 1].Add(item);		
				}
			}
		}	
	}
	
	void SetTypeButtonIcon()
	{
		for(int i=0; i<mSkillType.transform.childCount; i++)
		{
			mSkillType.transform.GetChild(i).Find("Icon").GetComponent<UISprite>().spriteName = mSkillInType[i][0].Icon;
			mSkillType.transform.GetChild(i).Find("SkillStatus").gameObject.SetActive(false);
			UpdateSkillStatus(i+1);
		}
	}
	
	void UpdateSkillStatus(int type)
	{
		List<ItemBase> skillList = SkillDataManager.Instance.GetCanLearnSkill();
		GameObject status = mSkillType.transform.GetChild(type - 1).Find("SkillStatus").gameObject;
		foreach (ItemBase item in skillList)
		{		
			SkillBase skill = SkillBaseManager.Instance.GetItem(item.ID);
			if(skill.Type == type)
			{
				if(!status.activeSelf)
					status.SetActive(true);
				return;
			}
		}
		if(status.activeSelf)
			status.SetActive(false);
	}
	
	public void OnClickTypeButton(GameObject go)
	{
		int type = int.Parse(go.name);
		if(mCurrentPage != type)
		{
			if(mLastTypeButton != null)
				mLastTypeButton.transform.Find("Select").gameObject.SetActive(false);
			go.transform.Find("Select").gameObject.SetActive(true);
			mLastTypeButton = go;
			ShowSkillButton(type);
		}
	}
	
	void ShowSkillButton(int page)
	{
		HideLastPage();
		mCurrentPage = page;
		ShowCurrentPage();
		UpdateSkillStatus(page);
		
	}
	
	void HideLastPage()
	{
		if(mCurrentPage > 0)
		{
			foreach(GameObject button in mSkillButtonArray[mCurrentPage-1])
				button.SetActive(false);
		}
	}
	
	void ShowCurrentPage()
	{
		UpdateActivationSkill();
		if(mSkillButtonArray[mCurrentPage - 1].Count == 0)
			CreateSkillButton();
		else
		{
			for(int i=0; i<mSkillButtonArray[mCurrentPage - 1].Count; i++)
			{
				GameObject skillButton = mSkillButtonArray[mCurrentPage - 1][i];
				skillButton.SetActive(true);
				//skillButton.transform.Find("Transform/Activation").GetComponent<UISprite>().gameObject.SetActive(false);
				ShowSkillInfo(skillButton, i);
				//EnbleButton(mSkillButtonArray[mCurrentPage - 1][i], i);
			}
		}
		if(mSkillButtonArray[mCurrentPage-1].Count > 0 )
		{
			mSelectIndex = mSkillButtonArray[mCurrentPage-1].Count <= mSelectIndex ? 0 : mSelectIndex;	
			OnClickSkillButton(mSkillButtonArray[mCurrentPage-1][mSelectIndex]);
		}
	}
	
	void UpdateActivationSkill()
	{
		mActivationSkillId.Clear();
		foreach (SkillItem skillItem in SkillDataManager.Instance.GetEquipSkillItem())
       	{
			if (skillItem != null && 
				skillItem.SkillBase != null && 
				skillItem.Equiped )
			{
				mActivationSkillId.Add(skillItem.Item.ID);
			}
        }
	}
	
	void CreateSkillButton()
	{
		for(int i=0; i<mSkillInType[mCurrentPage - 1].Count; i++)
		{
			GameObject skillButton = GameObject.Instantiate(mSkillButton) as GameObject;
			skillButton.transform.parent = mSkillItem.transform;
			skillButton.transform.localScale = Vector3.one;
			ItemBase item = ItemBaseManager.Instance.GetItem( mSkillInType[mCurrentPage-1][i].ID);
			// only open first skill.
			if(i == 0)
			{
				skillButton.transform.Find("Lock").gameObject.SetActive(false);
				skillButton.transform.Find("Icon").GetComponent<UISprite>().spriteName = item.Icon;
			}
			else
			{
				skillButton.GetComponent<BoxCollider>().enabled = false;
				skillButton.transform.Find("Icon").gameObject.SetActive(false);
			}
			UIEventListener.Get(skillButton).onClick = OnClickSkillButton;
			skillButton.name = i.ToString();
			//skillButton.transform.Find("Transform/Activation").GetComponent<UISprite>().gameObject.SetActive(false);
			skillButton.transform.Find("ID/0").gameObject.name = item.ID.ToString();
			ShowSkillInfo(skillButton, i);
			
			mSkillButtonArray[mCurrentPage - 1].Add(skillButton);
		}
		mSkillItem.GetComponent<UIGrid>().Reposition();
	}
	
	void ShowSkillInfo(GameObject skillButton, int buttonIndex)
	{
		int skillID = mSkillInType[mCurrentPage-1][buttonIndex].ID;
		ShowActivationFrame(skillButton);
		UpdateRuneSlot(skillButton, skillID);
	}
	
	void ShowActivationFrame(GameObject go)
	{
		if(go.activeSelf)
		{
			foreach(int id in mActivationSkillId)
			{
				GameObject skillItem = go.transform.Find("ID").gameObject;
				Transform[] transArray = skillItem.GetComponentsInChildren<Transform>();
				string name = "";
				foreach(Transform trans in transArray )
					name = trans.name;
				int skillId = int.Parse(name);
				//if(id == skillId)
					//go.transform.Find("Transform/Activation").GetComponent<UISprite>().gameObject.SetActive(true);
			}
		}
	}
	
	void OnClickSkillButton(GameObject go)
	{	
		mSelectSkillButton = go;
		mSelectIndex = int.Parse(go.name);
		
		mSelectFrame.transform.parent = go.transform.Find("Transform");
		mSelectFrame.transform.localPosition = new Vector3(0, 0, 4);
		
		int skillID = mSkillInType[mCurrentPage-1][mSelectIndex].ID;
		mSelectSkillID = skillID;

		Debug.Log("Skill ID:  " + skillID);
		int level = GetSkillLevel(mSelectSkillID);
		SkillAttrib nextSkill = SkillAttribManager.Instance.GetItem(skillID, level + 1);
		Control("Level").GetComponent<UILabel>().text = level.ToString();
		level = level==0 ? 1 : level;
		SkillAttrib attrib = SkillAttribManager.Instance.GetItem(skillID, level);
		
		if(attrib == null)
			return;
		mSkillDesc.GetComponent<UILabel>().text = attrib.Desc;	
		mSkillName.GetComponent<UILabel>().text = mSkillInType[mCurrentPage-1][mSelectIndex].Name;	
		
		if(nextSkill != null)
		{
			ShowPrice(nextSkill);
			if(!mLearnSkillButton.gameObject.activeSelf)
				mLearnSkillButton.gameObject.SetActive(true);
			mLearnSkillButton.isEnabled = true;
			mLearnSkillButton.transform.Find("MendSprite").gameObject.SetActive(true);
			mNeedLv.SetActive(false);
			if( nextSkill != null && PlayerDataManager.Instance.Attrib.Level < nextSkill.LevelRequest)
			{
				if(mSpRequire.text != "")
					mSpRequire.text = "";
				mLearnSkillButton.isEnabled = false;
				mLearnSkillButton.transform.Find("MendSprite").gameObject.SetActive(false);
				mNeedLv.SetActive(true);
				mNeedLv.transform.Find("Num").GetComponent<UILabel>().text = nextSkill.LevelRequest.ToString();
			}
		}
		else
		{
			ShowPrice(attrib);
			mLearnSkillButton.gameObject.SetActive(false);
		}
		//UpdateSkillSegment(skillID);
		EnableButton();
	}
	
	void ShowPrice(SkillAttrib attrib)
	{
		UISprite sprite = mNeedSp.transform.Find("Sprite").GetComponent<UISprite>();
		UILabel label = mNeedSp.transform.Find("Label").GetComponent<UILabel>();
		if(PlayerDataManager.Instance.Attrib.SP >= attrib.SP)
		{
			mSpRequire.text = "";
			sprite.spriteName = "Icon01_System_Sp_01";
			label.text = attrib.SP.ToString();
		}
		else
		{
			sprite.spriteName = "Icon01_System_Gem_01";
			label.text = (attrib.SP * 0.1f).ToString();
			mSpRequire.text = string.Format("��Ҫ���� SP [FF0000]{0}[-] ����ʹ����ʯѧϰ����", attrib.SP);
		}
	}
	
	void EnableButton()
	{
		if(!SkillDataManager.Instance.HasSkill(mSelectSkillID))
		{
			mRuneButton.GetComponent<UIImageButton>().isEnabled = false;
			mRuneButton.transform.Find("MendSprite").gameObject.SetActive(false);
		}
		else
		{
			mRuneButton.GetComponent<UIImageButton>().isEnabled = true;
			mRuneButton.transform.Find("MendSprite").gameObject.SetActive(true);
		}
	}
	
	public void OnClickLearnButton(GameObject go)
	{
		int level = GetSkillLevel(mSelectSkillID);
		level++;
		
		if( !LearnCheck(level) )
			return;
		if(level == 1 )
			OnLearnSkill();
		else
			OnUpgradeSkill();	
	}
	
	void OnLearnSkill()
	{
		Request(new SkillLearnCmd(mSelectSkillID), delegate(string err, Response response)
        {
            if (string.IsNullOrEmpty(err))
			{			
				UpdatePlayerAttrib();
				SoundManager.Instance.PlaySound("UI_LevelUp");
				CreateSuccessEffect();
			}
			else
			{
				MessageBoxWnd.Instance.Show(("LearnSkillFailed"));
				//Debug.LogError("Fail to Learn Skill, error: " + err);    
			}
        });
	}
	
	void CreateSuccessEffect()
	{
		Transform parentTran = mSkillButtonArray[mCurrentPage - 1][0].transform;
		if(parentTran == null)
			return;		
		Transform posTran = parentTran.Find("Transform");
		if(posTran == null)
			return;		
		GameObject effect = GameObject.Instantiate(Resources.Load("SuccessEffect")) as GameObject;
		effect.transform.parent = parentTran;
		effect.transform.localPosition = posTran.localPosition;
		effect.transform.localEulerAngles = Vector3.zero;
		effect.transform.localScale = Vector3.one;
	    float time = effect.GetComponentInChildren<ParticleSystem>().duration;
		GameObject.Destroy(effect,time);
	}
	
	void OnUpgradeSkill()
	{
		Request(new SkillUpgradeCmd(mSelectSkillID), delegate(string err, Response response)
        {
            if (string.IsNullOrEmpty(err))
			{
				UpdatePlayerAttrib();
				SoundManager.Instance.PlaySound("UI_LevelUp");
				CreateSuccessEffect();
			}
			else
			{
				Debug.LogError("Fail to Upgrade Skill, error: " + err);     
				MessageBoxWnd.Instance.Show(("UpdateSkillFailed"));
				//Debug.LogError("Fail to Upgrade Skill, error: " + err);     
			}
        });
	}
	
	bool LearnCheck(int level)
	{
		SkillAttrib attrib = SkillAttribManager.Instance.GetItem(mSelectSkillID, level);
		if (attrib == null)
		{
			MessageBoxWnd.Instance.Show("�Ѿ��ﵽ��ߵȼ�");
			//Debug.LogError("skill not found! maybe has reached the highest level!");
			return false;
		}

		if( PlayerDataManager.Instance.Attrib.SP < attrib.SP)
		{
			if(PlayerDataManager.Instance.Attrib.Gem >= (int)(attrib.SP * 0.1f))
			{
				MessageBoxWnd.Instance.Show(string.Format("SP ����,���Ƿ�ʹ�� {0} ��ʯѧϰ����?", attrib.SP * 0.1f));
				if(level == 1)
					MessageBoxWnd.Instance.OnClickedOk = OnClickOkLearnSkill;
				else
					MessageBoxWnd.Instance.OnClickedOk = OnClickOkUpgradeSkill;
			}
			else
			{
				MessageBoxWnd.Instance.Show("��ʯ����!���Ƿ�Ҫ��ֵ?");
				MessageBoxWnd.Instance.OnClickedOk = OpenRechargeWindow;
			}
			return false;
		}
		return true;
	}
	
	void OnClickOkLearnSkill(GameObject go)
	{
		OnLearnSkill();
	}
	
	void OnClickOkUpgradeSkill(GameObject go)
	{
		OnUpgradeSkill();
	}
	
	void OpenRechargeWindow(GameObject go)
	{
		Close();
		if(!RechargeWnd.Exist)
			RechargeWnd.Instance.Open();
	}
	
	void OnUseSkill(GameObject go)
	{
		if(SkillDataManager.Instance.HasActivationSkill(mSelectSkillID))
			return;
		Request(new SkillEquipCmd(mSelectSkillID), delegate(string err, Response response)
        {
            if (string.IsNullOrEmpty(err))
			{
				//UseSkillDisplace();
				UpdatePlayerAttrib();
				mLastUseSkillID = mSelectSkillID;
				
			}
			else
			{
				MessageBoxWnd.Instance.Show(("EquipSkillFailed"));
				//Debug.LogError("Fail to Equip Skill, error: " + err);
			}
        });
	}
	
	int GetSkillLevel(int skillID)
	{
		int level;
		SkillItem skill = SkillDataManager.Instance.GetSkillItem(skillID);
		level = skill != null ? skill.Lv: 0;
		
		return level;
	}
	
	void UpdateSkillData()
	{
		Request(new GetSkillPackCmd(), delegate(string err, Response response)
        {
            if (!string.IsNullOrEmpty(err))
            {
				MessageBoxWnd.Instance.Show(("RequsetSkillInfo"));
                //Debug.LogError("Request Skill Info Error!!" + err);
                return;
            }
			RequestAttribInfo();
            Package skillData = (response != null) ? response.Parse<Package>() : null;
			// update the package
            if (skillData != null)
				PlayerDataManager.Instance.OnSkillPack(skillData);	
			SkillDataManager.Instance.UpdateSkillItem();
			ShowSkillButton(mCurrentPage);
			InitRuneSlot();
        });
	}
	
	void UpdatePlayerAttrib()
	{
		MainScript.Instance.Request(new GetUserAttribCmd(), delegate(string err, Response response)
        {
			//Global.ShowLoadingEnd();
            if (!string.IsNullOrEmpty(err))
            {
				MessageBoxWnd.Instance.Show( err ,MessageBoxWnd.StyleExt.Alpha);
                return;
            }

            // update the user attribute.
            MainAttrib attrib = (response != null) ? response.Parse<MainAttrib>() : null;
			Debug.Log("Level: " + attrib.Level);
            if (attrib != null)
				PlayerDataManager.Instance.OnMainAttrib(attrib);
			UpdateSkillData();
        });
	}	
	
	void RequestAttribInfo()
	{
        Request(new GetUserAttribCmd(), delegate(string err, Response response)
        {
            if (!string.IsNullOrEmpty(err))
            {
                return;
            }

            // update the user attribute.
            MainAttrib attrib = (response != null) ? response.Parse<MainAttrib>() : null;
            if (attrib != null)
			{
				PlayerDataManager.Instance.OnMainAttrib(attrib);
				UpdateResourceBar();
				//InitAttrib();
				//ShowProcessLoadingEnd();
			}
        });
	}
	
	void UpdateSkillSegment(int skillID)
	{
		int level = GetSkillLevel(skillID); 
		mSegment.SetActive(true);
		SkillAttrib attrib = SkillAttribManager.Instance.GetItem(skillID, level);
		if (attrib == null)
			return;
		UISprite segment1 = mSegment.transform.Find("Segment1/Sprite").GetComponent<UISprite>();
		UISprite segment2 = mSegment.transform.Find("Segment2/Sprite").GetComponent<UISprite>();
		if( !string.IsNullOrEmpty( attrib.Action1 ) )
			segment1.spriteName = "Button13_SkillStep_Open_01";
		else
			segment1.spriteName = "Button13_SkillStep_Close_01";
			
		if( !string.IsNullOrEmpty( attrib.Action2 ) )
			segment2.spriteName = "Button13_SkillStep_Open_01";
		else
			segment2.spriteName = "Button13_SkillStep_Close_01";
		Debug.Log(attrib.Action2);
		//if( !string.IsNullOrEmpty( attrib.Action3 ) )
		//	skillButton.transform.Find("Transform/Segment/Segment3/Front").gameObject.SetActive(true);
	}
	
	void OnBuyRuneSlot(GameObject go)
	{
		int price = GetSlotPrice();
		if(mOpenSlot < 4 && PlayerDataManager.Instance.Attrib.Gold < price )
		{
			MessageBoxWnd.Instance.Show("�Բ��𣬽�Ҳ��㣬���Ƿ�Ҫ��ֵ?");
			MessageBoxWnd.Instance.OnClickedOk = OpenRechargeWindow;
			return;
		}
		else if(mOpenSlot == 4 && PlayerDataManager.Instance.Attrib.Gem < price )
		{
			MessageBoxWnd.Instance.Show("�Բ�����ʯ���㣬���Ƿ�Ҫ��ֵ?");
			MessageBoxWnd.Instance.OnClickedOk = OpenRechargeWindow;
			return;
		}
		
		Request(new SkillBuySlotCmd(mSelectSkillID), delegate(string err, Response response)
        {
            if (string.IsNullOrEmpty(err))
			{
				UpdatePlayerAttrib();
				Debug.Log("Buy Slot success!");
			}
			else
			{
				MessageBoxWnd.Instance.Show(("BuySlotFailed"));
				Debug.LogError("Fail to Buy Slot, error: " + err);
			}
			
			RequestAttribInfo();
			
        });
	}
	
	void OnClickSlot(GameObject go)
	{
		if(go.Equals(mSelectSlot))
			return;
		mChooseSlot = int.Parse(go.name);

		if(mLastSelectSlot != null)
			mLastSelectSlot.transform.Find("SelectSlot").gameObject.SetActive(false);
		go.transform.Find("SelectSlot").gameObject.SetActive(true);
		mSelectSlot = go;
		mLastSelectSlot = mSelectSlot;
		SkillItem skill = SkillDataManager.Instance.GetSkillItem(mSelectSkillID);
		if(skill == null)
			return;
		
		int[] slot = {skill.Slot1, skill.Slot2, skill.Slot3, skill.Slot4};
		ItemBase item = ItemBaseManager.Instance.GetItem(slot[mChooseSlot-1]);
		GameObject slotDesc = Control("SelectSlotDesc");
		string name = "";
		string desc = "";
		UILabel nameLabel = slotDesc.transform.Find("SelectRuneName").GetComponent<UILabel>();
		if(item != null)
		{
			name = "[FF0000]����Ƕ\n[-]" + item.Name;
			desc = item.Desc;
			SetQualityColor(nameLabel, item.Quality);
		}
		nameLabel.text = name;
		slotDesc.transform.Find("Desc").GetComponent<UILabel>().text = desc;
	}
	
	void UpdateRuneSlot(GameObject skillButton, int skillID)
	{	
		SkillItem skill = SkillDataManager.Instance.GetSkillItem(skillID);
		if(skill == null)
			return;
		int[] slot = {skill.Slot1, skill.Slot2, skill.Slot3, skill.Slot4};

		ItemBase item;
		GameObject slotButton;
		UISprite icon;
		UISprite runeLock;
		Debug.Log("numeber:" + skill.SlotNum);
		for(int i=0; i<slot.Length; i++)
		{
			slotButton = skillButton.transform.Find( string.Format("Transform/StoneSlot/Slot{0}", i+1) ).gameObject;
			slotButton.GetComponent<BoxCollider>().enabled = false;
			icon = slotButton.transform.Find("Icon").GetComponent<UISprite>();
			runeLock = slotButton.transform.Find("RuneLock").GetComponent<UISprite>();
			int num = skill.SlotNum;
			num = (num == 0) ? 1 : num;
			if(num > i)
			{
				runeLock.gameObject.SetActive(false);
				item = ItemBaseManager.Instance.GetItem(slot[i]);
				if(item != null)
					icon.spriteName = item.Icon;
				//UIEventListener.Get( slotButton ).onClick = OnClickSlot;
			}
			//else
			//	UIEventListener.Get( slotButton ).onClick = OnBuyRuneSlot;
		}
	}
	
	void EnableAllSkillTypeButton(bool enable)
	{
		for(int i=0; i<mSkillType.transform.childCount; i++)
		{
			mSkillType.transform.GetChild(i).GetComponent<BoxCollider>().enabled = enable;
		}
	}
	/*******************************************************************************************************************************/
	public void OnOpenRuneWindow(GameObject go)
	{
		EnableAllSkillTypeButton(false);
		mRuneItem = Resources.Load("RuneItem");
		ItemBase item = ItemBaseManager.Instance.GetItem( mSelectSkillID );
		Control("RuneSkill").transform.Find("Icon").GetComponent<UISprite>().spriteName = item.Icon;
		SetTypeButtonBackground(mCurrentPage, Control("RuneSkill").transform.Find("Background").GetComponent<UISprite>());
		Control("Name").GetComponent<UILabel>().text = item.Name;
		UIGuideEvent.Get( Control("Close") ).onClick = OnCloseRuneWnd;
		Control("Lv").GetComponent<UILabel>().text = GetSkillLevel(mSelectSkillID).ToString();
		Control("RuneDesc").GetComponent<UILabel>().text = "";
		Control("RuneName").GetComponent<UILabel>().text = "";
		mSelectSlot = null;
		mLastSelectSlot = null;
	
		UpdatePakageData();
	}
	
	int GetSlotPrice()
	{
		SkillItem skill = SkillDataManager.Instance.GetSkillItem(mSelectSkillID);
		if(skill.SlotNum >= 4)
			return 0;
		int slotIndex = skill.SlotNum;
		slotIndex = slotIndex == 0 ? 1 : slotIndex;
		mOpenSlot = slotIndex + 1;
		SkillBase skillBase = SkillBaseManager.Instance.GetItem(mSelectSkillID);
		int[] price = {skillBase.Slot1, skillBase.Slot2, skillBase.Slot3, skillBase.Slot4};
		return price[slotIndex];
	}
	
	void InitRuneSlot()
	{
		SkillItem skill = SkillDataManager.Instance.GetSkillItem(mSelectSkillID);
		if(skill == null)
			return;
		Debug.Log("slot num: " + skill.SlotNum);
		Debug.Log("slot id: " + skill.Item.ID);
		int[] slot = {skill.Slot1, skill.Slot2, skill.Slot3, skill.Slot4};

		ItemBase item;
		GameObject slotButton;
		UISprite icon;
		UISprite runeLock;
		GameObject openSlot = Control("OpenSlot");
		if(skill.SlotNum < 4)
		{
			openSlot.GetComponent<UIImageButton>().isEnabled = true;
			openSlot.transform.Find("MendSprite").gameObject.SetActive(true);
			UIEventListener.Get( openSlot ).onClick = OnBuyRuneSlot;
		}
		else
		{
			openSlot.GetComponent<UIImageButton>().isEnabled = false;
			openSlot.transform.Find("MendSprite").gameObject.SetActive(false);
		}
		
		openSlot.transform.Find("Price/Label").GetComponent<UILabel>().text = GetSlotPrice().ToString();
		Debug.Log("slot: " + mOpenSlot);
		if(mOpenSlot < 4)
			openSlot.transform.Find("Price/Sprite").GetComponent<UISprite>().spriteName = "Icon01_System_Gold_01";
		else
			openSlot.transform.Find("Price/Sprite").GetComponent<UISprite>().spriteName = "Icon01_System_Gem_01";
		GameObject runeSlot = Control("RuneSlot");
		for(int i=0; i<slot.Length; i++)
		{
			slotButton = runeSlot.transform.Find(string.Format("{0}", i+1)).gameObject;
			icon = slotButton.transform.Find("Icon").GetComponent<UISprite>();
			runeLock = slotButton.transform.Find("RuneLock").GetComponent<UISprite>();
			runeLock.gameObject.SetActive(true);
			slotButton.transform.Find("SelectSlot").gameObject.SetActive(false);
			int num = skill.SlotNum;
			num = (num == 0) ? 1 : num;
			if(num > i)
			{
				runeLock.gameObject.SetActive(false);
				item = ItemBaseManager.Instance.GetItem(slot[i]);
				Debug.Log(slot[i]);
				if(item != null)
					icon.spriteName = item.Icon;
				else
					icon.spriteName = "Button08_EquipItem_Bottom_04";
			}
			UIEventListener.Get( slotButton ).onClick = OnClickSlot;
			//else
			//	UIEventListener.Get( slotButton ).onClick = OnBuyRuneSlot;
		}
		Control("RuneGrid").GetComponent<UIGrid>().Reposition();
		AutoChooseSlot();
	}
	
	void InitRuneItem()
	{
		mRuneArray.Clear();
		ClearItem();
		
		foreach (Item item in PlayerDataManager.Instance.BackPack.Items.Values)
		{	
			if(item.ItemBase.MainType == (int)EItemType.Rune)
			{
				int num = item.Num;
				while(num > 99)
				{
					CreateRuneItem(item, 99);
					num -= 99;
				}
				if(num > 0)
					CreateRuneItem(item, num);
			}
		}
		if(mRuneArray.Count != 0)
			OnClickRuneItem(mRuneArray[0].transform.Find("Icon").gameObject);
		else
		{
			Control("RuneDesc").GetComponent<UILabel>().text = "";
			Control("RuneName").GetComponent<UILabel>().text = "";
		}

		Control("RuneGrid").GetComponent<UIGrid>().Reposition();
		GameObject studdedButton = Control("Studded");
		if(mRuneArray.Count == 0)
		{
			studdedButton.GetComponent<UIImageButton>().isEnabled = false;
			studdedButton.transform.Find("MendSprite").gameObject.SetActive(false);
		}
		else
		{
			studdedButton.GetComponent<UIImageButton>().isEnabled = true;
			studdedButton.transform.Find("MendSprite").gameObject.SetActive(true);
		}
		InitRuneSlot();
	}
	
	void ClearItem()
	{
		for(int i=0; i< Control("RuneGrid").transform.childCount; i++)
			GameObject.Destroy(Control("RuneGrid").transform.GetChild(i).gameObject);
		Control("RuneGrid").transform.DetachChildren();
	}
	
	void CreateRuneItem(Item item, int num)
	{
		GameObject rune = GameObject.Instantiate(mRuneItem) as GameObject;
		GameObject.Destroy(rune.GetComponent<UIPanel>());
		rune.transform.parent = Control("RuneGrid").transform;
		rune.transform.localScale = new Vector3(55,55,1);
		rune.transform.localPosition = Vector3.zero;
		rune.transform.Find("Check").gameObject.SetActive(false);
		rune.transform.Find("CountNo/LabelCount").GetComponent<UILabel>().text = num.ToString();
		rune.transform.Find("Icon/Background").GetComponent<UISprite>().spriteName = item.ItemBase.Icon;
		rune.transform.Find("ItemID").GetComponent<UILabel>().text = item.ID.ToString();
		rune.transform.Find("ItemBaseID").GetComponent<UILabel>().text = item.ItemBase.ID.ToString();
		//rune.transform.Find("Level/LabelLvl").GetComponent<UILabel>().text = item.ItemBase.Level.ToString();
		rune.name = mRuneArray.Count.ToString();
		RuneItemQuality(rune, item.ItemBase.Quality);
		UIEventListener.Get(rune.transform.Find("Icon").gameObject).onClick = OnClickRuneItem;
		//UIEventListener.Get(Control("Compose")).onClick = OnCompose;

		mRuneArray.Add(rune);
	}
	
	void SetQualityColor(UILabel label, int quality)
	{
		switch(quality)
		{
			case (int)ItemQuality.IQ_White:
				label.color = Color.white;
				break;
			case (int)ItemQuality.IQ_Blue:
				label.color = Color.blue;
				break;
			case (int)ItemQuality.IQ_Green:
				label.color = Color.green;
				break;
			case (int)ItemQuality.IQ_Purple:
				label.color = new Color(0.4f, 0, 0.8f);//purple;
				break;
		}
	}
	
	void RuneItemQuality(GameObject runeItem, int quality)
	{
		UISprite qualitySprite = runeItem.transform.Find("Quality").GetComponent<UISprite>();
		qualitySprite.enabled = true;
		switch(quality)
		{
			case (int)ItemQuality.IQ_White:
				qualitySprite.enabled = false;
				break;
			case (int)ItemQuality.IQ_Blue:
				qualitySprite.spriteName = "Button10_BaseItem_Quality_02";
				break;
			case (int)ItemQuality.IQ_Green:
				qualitySprite.spriteName = "Button10_BaseItem_Quality_01";
				break;
			case (int)ItemQuality.IQ_Purple:
				qualitySprite.spriteName = "Button10_BaseItem_Quality_03";
				break;
		}
	}
	
	void OnClickRuneItem(GameObject go)
	{
		if(go.Equals(mLastChoose))
			return;
		if(mLastChoose != null){
			mLastChoose.transform.parent.Find("Check").GetComponentInChildren<TweenAlpha>().enabled = false;
			mLastChoose.transform.parent.Find("Check").gameObject.SetActive(false);
		}
		Transform parentObj = go.transform.parent;
		GameObject checkBox = parentObj.Find("Check").gameObject;
		int index = int.Parse(parentObj.name);
		checkBox.SetActive(true);
		checkBox.GetComponentInChildren<TweenAlpha>().enabled = true;
		
		mChooseRuneId = int.Parse(mRuneArray[index].transform.Find("ItemID").GetComponent<UILabel>().text);
		mRuneItemBaseId = int.Parse(mRuneArray[index].transform.Find("ItemBaseID").GetComponent<UILabel>().text);
		Item tmpItem = PlayerDataManager.Instance.BackPack.Find(mChooseRuneId);
		UILabel nameLabel = Control("RuneName").GetComponent<UILabel>();
		Control("RuneDesc").GetComponent<UILabel>().text = tmpItem.ItemBase.Desc;
		nameLabel.text = tmpItem.ItemBase.Name;
		SetQualityColor(nameLabel, tmpItem.ItemBase.Quality);

		mLastChoose = go;
	}
	
	void OnCloseRuneWnd(GameObject go)
	{
		EnableAllSkillTypeButton(true);
	}

	public void OnSkillStrengthen(GameObject go)
	{
		Debug.Log(mChooseRuneId);
		if(StrengthenCheck())
			SkillStrengthen();
	}
	
	void SkillStrengthen()
	{
		if(mChooseRuneId <= 0)
			return;
		Debug.Log("Slot indexas7das987d98as7d9as9d7as97d9as7das97d9as7: " + mChooseSlot);
        Request(new SkillAddRuneCmd(mSelectSkillID, mChooseRuneId, mChooseSlot), delegate(string err, Response response)
        {
            if (string.IsNullOrEmpty(err))
			{
				UpdateRuneData();
				SoundManager.Instance.PlaySound("UI_Task_Complete");
				//UpdateSkillSlot();
				Debug.Log("Strengthen success!");
			}
			else
			{
				MessageBoxWnd.Instance.Show("ǿ������ʧ��, ����: " + err);
				//Debug.LogError("Fail to Strengthen Skill, error: " + err);
			}
        });
	}
	
	public void UpdateRuneData()
	{
		mSelectSlot = null;
		mLastSelectSlot = null;
		UpdatePlayerAttrib();
		UpdatePakageData();
	}
	
	bool StrengthenCheck()
	{
		SkillItem skill = SkillDataManager.Instance.GetSkillItem(mSelectSkillID);
		if(mChooseSlot!=1 && mChooseSlot > skill.SlotNum)
		{
			MessageBoxWnd.Instance.Show("û�п���!");
			return false;
		}
		int[] slot = {skill.Slot1, skill.Slot2, skill.Slot3, skill.Slot4};
		Item tmpItem = PlayerDataManager.Instance.BackPack.Find(mChooseRuneId);
		Debug.Log("Choose rune" + tmpItem.ItemBase.ID);
		for(int i=0; i<slot.Length; i++)
		{
			ItemBase item = ItemBaseManager.Instance.GetItem(slot[i]);
			if(item != null && item.SubType == tmpItem.ItemBase.SubType)
				return	ReplacePrompt(item.Level, tmpItem.ItemBase.Level-1, i+1);	
		}
		ItemBase itemBase = ItemBaseManager.Instance.GetItem(slot[mChooseSlot - 1]);
		if( itemBase == null )
			return true;
		else
		{
			if(itemBase.SubType == tmpItem.ItemBase.SubType)
				return ReplacePrompt(itemBase.Level, tmpItem.ItemBase.Level-1, mChooseSlot);
			return ReplacePrompt(itemBase.Level, tmpItem.ItemBase.Level, mChooseSlot);
		}
		return true;
	}
	
	bool ReplacePrompt(int targetLevel, int srcLevel, int slotIndex)
	{
		Debug.Log("target:" + targetLevel);
		Debug.Log("Src:" + srcLevel);
		if(targetLevel > srcLevel)
		{
			MessageBoxWnd.Instance.Show("������Ƕ�˷���!");
			return false;
		}
		else
		{
			MessageBoxWnd.Instance.Show(string.Format("ȷ��Ҫ�滻���� {0}?", slotIndex),  MessageBoxWnd.Style.OK_CANCLE);
			MessageBoxWnd.Instance.OnClickedOk = ReplaceRune;
			return false;
		}
		return false;
	}
	
	void AutoChooseSlot()
	{
		mSelectSlot = null;
		SkillItem skill = SkillDataManager.Instance.GetSkillItem(mSelectSkillID);
		if(skill == null)
			return;
		int[] slot = {skill.Slot1, skill.Slot2, skill.Slot3, skill.Slot4};
		if(skill.SlotNum>0)
		{
			for(int i=0; i< slot.Length; i++)
			{
				if(slot[i] == 0 && i <= skill.SlotNum)
				{
					GameObject slotButton = Control("RuneSlot").transform.Find(string.Format("{0}", i+1)).gameObject;
					OnClickSlot(slotButton);
					return;
				}	
			}
		}
		GameObject button = Control("RuneSlot").transform.Find("1").gameObject;
		
		OnClickSlot(button);
	}
	
	void ReplaceRune(GameObject go)
	{
		SkillStrengthen();
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
			InitRuneItem();
			Control("RuneGrid").GetComponent<UIGrid>().Reposition();
			AutoChooseSlot();
        });
	}
	
	void UpdateSkillSlot()
	{
		Request(new GetSkillPackCmd(), delegate(string err, Response response)
        {
            if (!string.IsNullOrEmpty(err))
            {
				MessageBoxWnd.Instance.Show(("RequsetSkillInfo"));
                //Debug.LogError("Request Skill Info Error!!" + err);
                return;
            }
			
            Package skillData = (response != null) ? response.Parse<Package>() : null;
			// update the package
            if (skillData != null)
				PlayerDataManager.Instance.OnSkillPack(skillData);
			InitRuneSlot();
        });
	}
}

