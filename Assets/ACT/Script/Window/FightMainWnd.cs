using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class FightMainWnd : Window<FightMainWnd>
{
    UISlider mSliderHP;
    UILabel mHPLabel;
	UISlider mSliderAbility;
	UILabel mAbilityLabel;
	
	GameObject mPlayerAgainstHp;
	UISlider mPlayerAgainstSliderHP;
	UILabel mPlayerAgainstHPLabel;
	UISlider mPlayerAgainstSliderAbility;
	UILabel mPlayerAgainstAbilityLabel;
	
	int mTimeCounter = 120;
	
	UISlider mMonsterHP;
	UILabel mMonsterLabel;
	
	GameObject mBossHp;
	UISprite mBossHPPercent;
	UILabel mBossName;
	UILabel mBossLevel;
	UILabel mBossHPCapacity;
	
	GameObject mMonsterHpPanel;

	GameObject mEnergy1;
	GameObject mEnergy2;
	GameObject mEnergy3;

	GameObject mMapPanel;
    public override string PrefabName { get { return "FightMainWnd"; } }
	LevelMap mLevelSmallMap = new LevelMap();
	
	SkillItem mSuperSkillItem = null;
	ShopBase[] mShopB = new ShopBase[2];
	Combo mCombo;
	
    protected override bool OnOpen()
    {	
		UIPosSitToScreen();
		ObjectInit();
		mCombo = new Combo(WndObject.transform);
        UIHelper.Register(Control("buttonnormal")).onPress = OnClickNormal;
		UIEventListener.Get(Control("PauseBtn")).onClick = OnClickPause;

		SeatToScene();
		UpdateSkill();

		SetJobNormalAttackIcon(PlayerDataManager.Instance.Attrib.Role);
		///AI处于激活状态
        if (Global.PauseAll)
            Global.PauseAll = false;
		
        return base.OnOpen();
    }
	
	public override void Show ()
	{
		base.Show();
	}
	
	void MonsterBloodInit()
	{
		mMonsterHP = Control("MonsterSliderhp").GetComponent<UISlider>();;
		mMonsterLabel = Control("Label", Control("MonsterBlood")).GetComponent<UILabel>();
		mMonsterHpPanel = Control ("MonsterBlood");
		if( MainScript.Instance.GmMode == EGameMode.GMRelease )
			mMonsterHpPanel.SetActive(false);
	}
	
	void BossBloodInit()
	{
		mBossHp = Control("BossHP");
		mBossHp.SetActive(false);
		mBossHPPercent = mBossHp.transform.Find("BossBlood").GetComponent<UISprite>();
		mBossName = mBossHp.transform.Find("BossName").GetComponent<UILabel>();
		mBossLevel = mBossHp.transform.Find("BossLevel").GetComponent<UILabel>();
		mBossHPCapacity = mBossHp.transform.Find("BossHPCapacity").GetComponent<UILabel>();
	}
	
	void AgainstPlayerBloodInit()
	{
		mPlayerAgainstHp = Control ("PlayerAgainstBlood");
		GameObject GoAgainstSlider = Control("sliderhp",mPlayerAgainstHp);
		mPlayerAgainstSliderHP = GoAgainstSlider.GetComponent<UISlider>();
		mPlayerAgainstHPLabel = Control("Label",GoAgainstSlider).GetComponent<UILabel>();
		GameObject GoAgainstAblitySlider = Control("slidersoul",mPlayerAgainstHp);
		mPlayerAgainstSliderAbility = GoAgainstAblitySlider.GetComponent<UISlider>();;
		mPlayerAgainstAbilityLabel = Control("Label", GoAgainstAblitySlider ).GetComponent<UILabel>();
		mPlayerAgainstAbilityLabel.text = "";
		Control("Energy1",mPlayerAgainstHp).SetActive(false);
		Control("Energy2",mPlayerAgainstHp).SetActive(false);
		Control("Energy3",mPlayerAgainstHp).SetActive(false);
		mPlayerAgainstHp.SetActive(false);
	}
	
	void LocalPlayerBloodInit()
	{
		GameObject Goblood = Control ("blood");
		GameObject GoHpSlider = Control("sliderhp",Goblood);
        mSliderHP = GoHpSlider.GetComponent<UISlider>();
        mHPLabel = Control("Label", GoHpSlider).GetComponent<UILabel>();
		GameObject GoSoulSlider = Control("slidersoul",Goblood);
		mSliderAbility = GoSoulSlider.GetComponent<UISlider>();;
		mAbilityLabel = Control("Label",GoSoulSlider).GetComponent<UILabel>();
		mAbilityLabel.text = "";
		mEnergy1 = Control("Energy1",Goblood);
		mEnergy2 = Control("Energy2",Goblood);
		mEnergy3 = Control("Energy3",Goblood);
		//mSliderAbility.sliderValue = 0.0f;
		mEnergy1.SetActive(false);
		mEnergy2.SetActive(false);
		mEnergy3.SetActive(false);
		
        LocalPlayer player = UnitManager.Instance.LocalPlayer;
        UpdateAbility(player.GetAttrib(EPA.CurAbility), player.GetAttrib(EPA.AbilityMax));
        UpdateHp(player.GetAttrib(EPA.CurHP), player.GetAttrib(EPA.HPMax));
        Control("labellevel",Goblood).GetComponent<UILabel>().text 
			= PlayerDataManager.Instance.Attrib.Level.ToString();
		//BASEROLE ID change will leadingto bug
		Control("JobIcon",Goblood).GetComponent<UISprite>().spriteName 
			= UnitBaseManager.Instance.GetItem(PlayerDataManager.Instance.Attrib.Role).Label;
	}
	
	void MapInit()
	{
		mMapPanel = Control("LevelMap");
		mLevelSmallMap.MapObject = Control("panel");
	}
	
	void ObjectInit()
	{
		CreateGameObjectByPrefabName("FightLeftUp");
		//CreateGameObjectByPrefabName("FightLeftDown");
		CreateGameObjectByPrefabName("FightRightUp");
		CreateGameObjectByPrefabName("FightRightDown");
		
		MapInit();
		LocalPlayerBloodInit();
		BossBloodInit();
		MonsterBloodInit();
		AgainstPlayerBloodInit();
	}
	
	void CreateGameObjectByPrefabName(string Name)
	{
		GameObject go = GameObject.Instantiate(Resources.Load(Name)) as GameObject;
		go.transform.parent = Control(Name+"Root").transform;
		go.transform.localPosition = Vector3.zero;
		go.transform.localScale = Vector3.one;
	}
	
	void UIPosSitToScreen()
	{
		Debug.Log("Screen.Wid: " + Screen.width +"Screen.height: " +Screen.height);
		Control("FightLeftUpRoot").transform.localPosition = new Vector3( (0-Screen.width/2),(Screen.height/2),0.0f)*UIHelper.WordToScreenModify;
		//Control("LeftDownRoot").transform.localPosition = new Vector3( (0-Screen.width/2),(0-Screen.height/2),0.0f);
		Control("FightRightUpRoot").transform.localPosition = new Vector3( (Screen.width/2),(Screen.height/2),0.0f)*UIHelper.WordToScreenModify;
		Control("FightRightDownRoot").transform.localPosition = new Vector3( (Screen.width/2),(0-Screen.height/2),0.0f)*UIHelper.WordToScreenModify;
	}	
		
	void SetJobNormalAttackIcon(int role)
	{
		int baseRole = 1003;
		string[] Icons = {
			"Button25_Attack_Warrior_01",
			"Button25_Attack_Assassin_01",
			"Button25_Attack_Wizard_01",
		};
		
		Control("icon",Control("buttonnormal")).GetComponent<UISprite>().spriteName = Icons[role - baseRole];
		Control("buttonnormal").GetComponent<UIImageButton>().normalSprite = Icons[role - baseRole];
		Control("buttonnormal").GetComponent<UIImageButton>().hoverSprite = Icons[role - baseRole];
		Control("buttonnormal").GetComponent<UIImageButton>().pressedSprite = Icons[role - baseRole];
	}
	
	
    protected override bool OnClose()
	{
        return base.OnClose();
    }
	
	void SeatToScene()
	{
		switch(Global.CurSceneInfo.ID)
		{
			case ESceneID.Invalid:
			{

			}break;
			case ESceneID.PVP:
			{
				SeatToPvP();
			}break;
			default: //PvE
			{
				mPlayerAgainstHp.SetActive(false);
				BuildLevelMap();
				InitEquipMed();
			}
			break;
		}
	}
	
	void SeatToPvP()
	{
		GameObject[] equipMedUIs = 
		{
			Control("EquipMed1"),
			Control("EquipMed2"),
			Control("buttonmed1"),
			Control("buttonmed2")
		};
		
		for(int i= 0; i<equipMedUIs.Length; i++){
			equipMedUIs[i].SetActive(false);
		}
		mMapPanel.SetActive(false);
		mPlayerAgainstHp.SetActive(true);
	}
	
	void ResetEquipMed()
	{
        GameObject[] equipMedUIs = 
		{
			Control ("EquipMed1"),
			Control ("EquipMed2")
		};
		
		for(int i = 0; i<equipMedUIs.Length; i++)
		{
			//equipMedUIs[i].transform.GetComponent<BoxCollider>().enabled = false;
			equipMedUIs[i].transform.Find("Count").GetComponent<UILabel>().text = " ";
			equipMedUIs[i].transform.Find("ItemID").GetComponent<UILabel>().text = ((int)PropsBaseID.Hp + i*3).ToString();
			equipMedUIs[i].transform.Find("GemRoot/Count").GetComponent<UILabel>().text = ((int)mShopB[i].Gem).ToString();
			equipMedUIs[i].transform.Find("bg").GetComponent<UISprite>().alpha = 0.3f;
			//equipMedUIs[i].transform.Find("GemRoot").gameObject.SetActive(false);
			UIEventListener.Get(equipMedUIs[i]).onClick = OnClickEquipMed;
		}	
	}
	
	void InitShopBase()
	{
		ShopBase[] AllSb = ShopBaseManager.Instance.GetAllItem();
		for(int i =0; i<AllSb.Length; i++)
		{
			if(AllSb[i].Item == (int)PropsBaseID.Hp && AllSb[i].Count == 1)
			{
				mShopB[0] = AllSb[i];
			}
			else if(AllSb[i].Item == (int)PropsBaseID.Mana && AllSb[i].Count == 1)
			{
				mShopB[1] = AllSb[i];
			}
		}
	}
	
	void InitEquipMed()
	{
        GameObject[] equipMedUIs = 
		{
			Control ("EquipMed1"),
			Control ("EquipMed2")
		};

		InitShopBase();
		ResetEquipMed();
        
		foreach (Item item in PlayerDataManager.Instance.BackPack.Items.Values)
        {
			if( (int)PropsBaseID.Hp == item.ItemBase.ID )
			{
				//equipMedUIs[0].transform.GetComponent<BoxCollider>().enabled = true;
				equipMedUIs[0].transform.Find("ItemID").GetComponent<UILabel>().text = item.ID.ToString();
				equipMedUIs[0].transform.Find("bg").GetComponent<UISprite>().spriteName = item.ItemBase.Icon;
				equipMedUIs[0].transform.Find("Count").GetComponent<UILabel>().text = item.Num.ToString();
				equipMedUIs[0].transform.Find("bg").GetComponent<UISprite>().alpha = 1.0f;
				equipMedUIs[0].transform.Find("GemRoot").gameObject.SetActive(false);
            	//UIEventListener.Get(equipMedUIs[0]).onClick = OnClickEquipMed;
			}else if( (int)PropsBaseID.Mana == item.ItemBase.ID ) {
				//equipMedUIs[1].transform.GetComponent<BoxCollider>().enabled = true;
				equipMedUIs[1].transform.Find("ItemID").GetComponent<UILabel>().text = item.ID.ToString();
				equipMedUIs[1].transform.Find("bg").GetComponent<UISprite>().spriteName = item.ItemBase.Icon;
				equipMedUIs[1].transform.Find("Count").GetComponent<UILabel>().text = item.Num.ToString();
				equipMedUIs[1].transform.Find("bg").GetComponent<UISprite>().alpha = 1.0f;
				equipMedUIs[1].transform.Find("GemRoot").gameObject.SetActive(false);
            	//UIEventListener.Get(equipMedUIs[1]).onClick = OnClickEquipMed;
			}
        }
	}

    void OnClickEquipMed(GameObject go)
    {
		int itemID = int.Parse( go.transform.Find("ItemID").GetComponent<UILabel>().text );
        Item item = PlayerDataManager.Instance.BackPack.Find(itemID);
		
		if(item == null)
		{
			UseItemRequestInfoWithGem(go);
		}
		else
		{
			PropsItem propItem = item.ToPropsItem();
			UseItemRequstInfo(go, propItem);
		}
    }

    void InitSkills()
    {
        // prebuild the skill items.
        GameObject[] skillUIs = 
        {
            Control("buttonskill0"),
            Control("buttonskill1"),
            Control("buttonskill2"),
			Control("buttonskill3"),
        };
        foreach (Item item in PlayerDataManager.Instance.SkillPack.Items.Values)
        {
            SkillItem skillItem = item.ToSkillItem();
            if (skillItem == null || skillItem.SkillBase == null)
                continue;

            int skillIdx = Mathf.Max(0, skillItem.SkillBase.Type - 1);
            if (skillIdx > skillUIs.Length - 1 || skillUIs[skillIdx] == null)
                continue;

            skillUIs[skillIdx].AddComponent<SkillInput>().Init(skillItem);
            skillUIs[skillIdx] = null;
        }
    }

    void OnClickNormal(GameObject go, bool press)
    {
        if (Global.GInputBox == null || Time.timeScale == 0 || Global.PauseAll) return;

        // 普通攻击目前还没有技能绑定的～
        UnitManager.Instance.LocalPlayer.ActionStatus.SkillItem = null;

        if (press)
            Global.GInputBox.OnKeyDown(EKeyList.KL_Attack);
        else
            Global.GInputBox.OnKeyUp(EKeyList.KL_Attack);
    }
	
	public void UpdateMonsterHp(int curHp, int maxHp)
	{
        float percent = (float)curHp / maxHp;
        mMonsterHP.sliderValue = percent;
        mMonsterLabel.text = string.Format("{0}/{1}", curHp, maxHp);
	}
	
	SkillItem IsHaveSuperSkill()
	{
		return mSuperSkillItem;
	}
	
	public void UpdateSoul(DownTile downTile, int curSoul,int maxSoul)
	{
		SkillItem skillItem = IsHaveSuperSkill();
        if (skillItem != null)
        {
            if (curSoul >= skillItem.Energy3)
            {
                mEnergy3.SetActive(true);
                mEnergy2.SetActive(true);
                mEnergy1.SetActive(true);
                downTile.OnSoulChanged(curSoul - skillItem.Energy3, maxSoul - skillItem.Energy3);
            }
            else if (curSoul >= skillItem.Energy2)
            {
                mEnergy3.SetActive(false);
                mEnergy2.SetActive(true);
                mEnergy1.SetActive(true);
                downTile.OnSoulChanged(
                    curSoul - skillItem.Energy2,
                    skillItem.Energy3 - skillItem.Energy2);
            }
            else if (curSoul >= skillItem.Energy1)
            {
                mEnergy3.SetActive(false);
                mEnergy2.SetActive(false);
                mEnergy1.SetActive(true);
                downTile.OnSoulChanged(
                    curSoul - skillItem.Energy1,
                    skillItem.Energy2 - skillItem.Energy1);
            }
            else
            {
                mEnergy3.SetActive(false);
                mEnergy2.SetActive(false);
                mEnergy1.SetActive(false);
                downTile.OnSoulChanged(curSoul, skillItem.Energy1);
            }
        }
	}

    public void UpdateAbility(int curAbility, int maxAbility)
    {
        mSliderAbility.sliderValue = (float)curAbility / maxAbility;
        mAbilityLabel.text = string.Format("{0}/{1}", curAbility, maxAbility);
    }
	
    public void UpdateHp(int curHp, int maxHp)
    {
		curHp = curHp < 0 ? 0 : curHp; 
        float percent = (float)curHp / maxHp;
        mSliderHP.sliderValue = percent;
        mHPLabel.text = string.Format("{0}/{1}", curHp, maxHp);
	}

	private void OnClickPause(GameObject go)
    {
		if (FightMainWnd.Exist)
		{
		    WndObject.SetActive(false);
		}

		if( !PauseWnd.Exist )
		{
			PauseWnd.Instance.Open();
		}
	}

	void BuildLevelMap()
	{
        GameLevel gameLevel = GameLevel.Instance;
		GameCell[] gameCells = gameLevel.Cells;
		if(gameCells != null)
			mLevelSmallMap.BuildMapCell(gameCells, gameLevel);
	}
	
	public void ShowLevelMap()
	{
		mMapPanel.SetActive(true);
	}
	
	public void HideLevelMap()
	{
		mMapPanel.SetActive(false);
	}
	
	public void ShowAroundMapCell(GameCell cell)
	{
		mLevelSmallMap.OnShowAroundCell(cell);
	}
	
	public void EnterCell(GameCell cell)
	{
		mLevelSmallMap.OnEnterCell(cell);
	}
	
	public void FinishCell(GameCell cell)
	{
		mLevelSmallMap.OnFinishCell(cell);
		ShowLevelMap();
	}
	public void PlayerMoveNotify(Vector3 position)
	{
		mLevelSmallMap.OnPlayerMove(position);
	}
	
	public void UpdateSkill()
	{
		GameObject[] skillUIs = 
        {
            Control("buttonskill0"),
            Control("buttonskill1"),
            Control("buttonskill2"),
			Control("buttonskill3"),
        };
		
		for (int skillIdx = 0; skillIdx < skillUIs.Length; skillIdx++)
		{
			SkillInput skillInput = skillUIs[skillIdx].GetComponent<SkillInput>();
			if( skillInput == null)
				skillInput = skillUIs[skillIdx].AddComponent<SkillInput>();
			SkillItem thisSkillItem = null;
		    foreach (SkillItem skillItem in SkillDataManager.Instance.GetEquipSkillItem())
	       	{
				if( skillItem.SkillType == ESkillType.Super )
					mSuperSkillItem = skillItem;
				
				if (skillItem != null && 
					skillItem.SkillBase != null && 
					skillItem.Equiped &&
					skillItem.SkillBase.Type == skillIdx + 1)
				{
					thisSkillItem = skillItem;
					break;
				}
	        }
			skillInput.Init(thisSkillItem);
		}
	}

    void OnUseItemSuccess(GameObject go, PropsItem propItem)
	{
        int newNum = propItem.Item.Num - 1;
        if (newNum <= 0)
        {
			go.transform.Find("bg").GetComponent<UISprite>().alpha = 0.3f;
            //go.transform.Find("bg").GetComponent<UISprite>().spriteName = "Button09_MedicineItem_Bottom_03";
            //go.transform.GetComponent<BoxCollider>().enabled = false;
			go.transform.Find("Count").GetComponent<UILabel>().text = " ";
			
			GameObject gemRoot = go.transform.Find("GemRoot").gameObject;
			if( !gemRoot.activeSelf )
			{
				gemRoot.SetActive(true);
			}
			
        }else{
			go.transform.Find("Count").GetComponent<UILabel>().text = newNum.ToString();
		}
        propItem.Item.Num = newNum;

        PropsBase propsBase = propItem.PropsBase;
        Unit localPlayer = UnitManager.Instance.LocalPlayer;

        int hp = localPlayer.GetAttrib(EPA.HPMax) * propsBase.HPPercent / 100 + propsBase.HP;
        if (hp > 0)
            localPlayer.AddHp(hp);

        int soul = localPlayer.GetAttrib(EPA.SoulMax) * propsBase.SoulPercent / 100 + propsBase.Soul;
        if (soul > 0)
            localPlayer.AddSoul(soul);

        int ability = localPlayer.GetAttrib(EPA.AbilityMax) * propsBase.AbilityPercent / 100 + propsBase.Ability;
        if (ability > 0)
            localPlayer.AddAbility(ability);

        if (propsBase.BuffID > 0)
            localPlayer.AddBuff(propsBase.BuffID);
	}
	
	
	void OnClickOkToBuyGem( GameObject go )
	{
		Hide();
		if( !RechargeWnd.Exist )
			RechargeWnd.Instance.Open();
	}
	
	void OnClickCancleToResume( GameObject go )
	{
		LevelHelper.LevelContinue(false);
	}
	
	void OnBuySuccess(int ItemBaseID)
	{
		PropsBase propsBase = PropsBaseManager.Instance.GetItem(ItemBaseID);
        Unit localPlayer = UnitManager.Instance.LocalPlayer;
        int hp = localPlayer.GetAttrib(EPA.HPMax) * propsBase.HPPercent / 100 + propsBase.HP;
        if (hp > 0)
            localPlayer.AddHp(hp);

        int soul = localPlayer.GetAttrib(EPA.SoulMax) * propsBase.SoulPercent / 100 + propsBase.Soul;
        if (soul > 0)
            localPlayer.AddSoul(soul);

        int ability = localPlayer.GetAttrib(EPA.AbilityMax) * propsBase.AbilityPercent / 100 + propsBase.Ability;
        if (ability > 0)
            localPlayer.AddAbility(ability);

        if (propsBase.BuffID > 0)
            localPlayer.AddBuff(propsBase.BuffID);
	}
	
	void UseItemRequestInfoWithGem( GameObject go )
	{
		GameObject gemRoot = go.transform.Find("GemRoot").gameObject;
		if( !gemRoot.activeSelf )
		{
			gemRoot.SetActive(true);
		}
		
		int itemBaseid = int.Parse( go.transform.Find("ItemID").GetComponent<UILabel>().text );
		int SBGem = 100;
		int ShopID = 0;
		if( mShopB[0].Item == itemBaseid )
		{
			ShopID = mShopB[0].ID;
			SBGem = mShopB[0].Gem;
		}
		else if(mShopB[1].Item == itemBaseid )
		{
			ShopID = mShopB[1].ID;
			SBGem = mShopB[1].Gem;
		}
		
		if(PlayerDataManager.Instance.Attrib.Gem >= SBGem)
		{
	        Request(new BuyCmd(ShopID,true), delegate(string err, Response response)
	        {
	            if (!string.IsNullOrEmpty(err))
	            {
					MessageBoxWnd.Instance.Show(("BuyMedFailed"),
						MessageBoxWnd.StyleExt.Alpha);
	                return;
	            }
	            OnBuySuccess(itemBaseid);
				SoundManager.Instance.PlaySound("UI_YaoJi");
				RequestAttribInfo();
	        });	
		}else{
			LevelHelper.LevelPause(true);
			MessageBoxWnd.Instance.Show("NotEnoughGem",MessageBoxWnd.Style.OK_CANCLE);
			MessageBoxWnd.Instance.OnClickedOk = OnClickOkToBuyGem;
			MessageBoxWnd.Instance.OnClickedCancle = OnClickCancleToResume;
		}
	}
	
	
	
	
    void UseItemRequstInfo(GameObject go, PropsItem propItem)
	{
        Request(new UseItemCmd(propItem.Item.ID), delegate(string err, Response response)
        {
            if (!string.IsNullOrEmpty(err))
            {
                Debug.LogError("RequestPackageInfo Error!!" + err);
				MessageBoxWnd.Instance.Show(err);
                return;
            }

            OnUseItemSuccess(go, propItem);
			SoundManager.Instance.PlaySound("UI_YaoJi");
			RequestPackageInfo();
        });
	}

	void RequestAttribInfo()
	{
        Request(new GetUserAttribCmd(), delegate(string err, Response response)
        {
			//Global.ShowLoadingEnd();
            if (!string.IsNullOrEmpty(err))
            {
				MessageBoxWnd.Instance.Show( err ,MessageBoxWnd.StyleExt.Alpha);
                return;
            }
			
            MainAttrib attrib = (response != null) ? response.Parse<MainAttrib>() : null;
            if (attrib != null)
            {
                PlayerDataManager.Instance.Attrib.Gem = attrib.Gem;
            }			

        });
		//Global.ShowLoadingStart();
	}
	
	
	void RequestPackageInfo()
	{
        Request(new GetBackPackCmd(), delegate(string err, Response response)
        {
            if (!string.IsNullOrEmpty(err))
            {
                Debug.LogError("RequestPackageInfo Error!!" + err);
				MessageBoxWnd.Instance.Show(err);
                return;
            }
            // update the user 
            Package backPackData = (response != null) ? response.Parse<Package>() : null;
            Debug.Log("backPackData:" + LitJson.JsonMapper.ToJson(backPackData));
            if (backPackData != null)
            {
                PlayerDataManager.Instance.OnBackPack(backPackData, false);
                //ShowProcessLoadingEnd();
            }
        });
		//ShowProcessLoadingStart();
	}
	
	public void UpdateBossHp(string name, int level, int curHp, int maxHp)
	{
		if( !mBossHp.activeSelf && curHp > 0)
			mBossHp.SetActive(true);
		curHp = curHp < 0 ? 0 : curHp; 
        float percent = (float)curHp / maxHp;
        mBossHPPercent.fillAmount = percent;
		
        mBossHPCapacity.text = string.Format("{0}/{1}", curHp, maxHp);
		mBossLevel.text = level.ToString();
		mBossName.text = name;
	}
	
	public void BossDied()
	{
		mBossHp.SetActive(false);
	}
	
	public void UpdatePlayerAgainstInfo(int Role,int level)
	{
		if (!mPlayerAgainstHp.activeSelf)
			return ;
		if(Role < 1003)
			Role = 1003;
		Debug.Log("mPlayerAgainstHp labellevel " + level);
		GameObject Go = mPlayerAgainstHp.transform.Find("exp/labellevel").gameObject;
		Go.GetComponent<UILabel>().text = level.ToString();
		Debug.Log("mPlayerAgainstHPLabel");
		Go = mPlayerAgainstHp.transform.Find("JobRoot/JobIcon").gameObject;
		Go.GetComponent<UISprite>().spriteName = UnitBaseManager.Instance.GetItem(Role).Label;
	}
	
	
	public void UpdatePlayerAgainstAbility(int curAbility, int maxAbility)
    {
		if (!mPlayerAgainstHp.activeSelf)
			return ;
		
        mPlayerAgainstSliderAbility.sliderValue = (float)curAbility / maxAbility;
        mPlayerAgainstAbilityLabel.text = string.Format("{0}/{1}", curAbility, maxAbility);
	}

	public void UpdatePlayerAgainstHp(float curHp, float maxHp)
    {
		if( Global.CurSceneInfo.ID != ESceneID.PVP )
			return;
		
		if( !mPlayerAgainstHp.activeSelf && curHp > 0)
			mPlayerAgainstHp.SetActive(true);
		
		if (!mPlayerAgainstHp.activeSelf)
			return ;
		
		mPlayerAgainstSliderHP.sliderValue = curHp / maxHp;;
		mPlayerAgainstHPLabel.text = string.Format("{0}/{1}", curHp, maxHp);
	}
	
	public bool UpdatePvPTime(int DtTime)
	{
		if (!mPlayerAgainstHp.activeSelf)
			return false;
		
		GameObject Go = mPlayerAgainstHp.transform.Find("TimeCounter").gameObject;	
		int LastTime = mTimeCounter - DtTime;
		if(LastTime <= 0)
			LastTime = 0;
		Go.GetComponent<UILabel>().text = LastTime.ToString();
		
		
		if( mTimeCounter <= DtTime )
			return true;
		
		return false;
	}

	
	public void ReviveUpdateUI()
	{
		LocalPlayer player = UnitManager.Instance.LocalPlayer;
		UpdateAbility(player.GetAttrib(EPA.CurAbility), player.GetAttrib(EPA.AbilityMax));
        UpdateHp(player.GetAttrib(EPA.CurHP), player.GetAttrib(EPA.HPMax));
	}
	
	public void OnComboHit(int comboHit)
	{
		mCombo.OnComboHit(comboHit);
	}
}
