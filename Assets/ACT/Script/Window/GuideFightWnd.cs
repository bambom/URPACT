using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GuideFightWnd : MonoBehaviour {
	
	public List<SkillItem>SkillList = new List<SkillItem>();
	
	UISlider mSliderHP;
    UILabel mHPLabel;
	int mGuidUnitID;
	GameObject mTalk;
	GameObject mSelectCharacter;
	
	GameObject mBossHp;
	UISprite mBossHPPercent;
	UILabel mBossName;
	UILabel mBossLevel;
	UILabel mBossHPCapacity;
	void Start()
    {
		Global.GuideMode = true;
		mSliderHP = GameObject.Find("sliderhp").GetComponent<UISlider>();
        mHPLabel = GameObject.Find("HpLabel").GetComponent<UILabel>();
		UIEventListener.Get(GameObject.Find("blood")).onEnable = OnBloodEnable;
		GameObject.Find("blood").SetActive(false);
		
		mBossHp = GameObject.Find("BossHP");
		mBossHp.SetActive(false);
		mBossHPPercent = mBossHp.transform.Find("BossBlood").GetComponent<UISprite>();
		mBossName = mBossHp.transform.Find("BossName").GetComponent<UILabel>();
		mBossLevel = mBossHp.transform.Find("BossLevel").GetComponent<UILabel>();
		mBossHPCapacity = mBossHp.transform.Find("BossHPCapacity").GetComponent<UILabel>();
		
		GameObject normalBtn = GameObject.Find("Normal");
		UIEventListener.Get(normalBtn).onPress = OnClickNormal;
		UIEventListener.Get(normalBtn).onEnable = OnNormalBtnEnable;

		normalBtn.SetActive(false);
		
		GameObject.Find("LeftJoystick").SetActive(false);
		mTalk = GameObject.Find("NpcTalk");
		mTalk.SetActive(false);
		mSelectCharacter = GameObject.Find("SelectRole");
		mSelectCharacter.GetComponent<TweenAlpha>().onFinished = OnAutoSelect;
		
		UIEventListener.Get(GameObject.Find("1100")).onClick = OnSelectCharacter;
		UIEventListener.Get(GameObject.Find("1101")).onClick = OnSelectCharacter;
		UIEventListener.Get(GameObject.Find("1102")).onClick = OnSelectCharacter;
		
		UIEventListener.Get(GameObject.Find("SkipGuide")).onClick = OnClickSkipGuide;
		UIEventListener.Get(GameObject.Find("StartGame")).onClick = OnClickSkipGuide;
		UIEventListener.Get(GameObject.Find("Retry")).onClick = OnClickTryAgain;
		UIEventListener.Get(GameObject.Find("SelectGame")).onEnable = OnShowSelectGame;
		GameObject.Find("SkipGuide").SetActive(false);
		GameObject.Find("SelectGame").SetActive(false);
		mSelectCharacter.SetActive(false);
		
    }
	void OnShowSelectGame(GameObject go)
	{
		Time.timeScale = 0;
	}
	void OnClickSkipGuide(GameObject go)
	{
		Destroy(this.gameObject);
		Time.timeScale = 1;
		Global.GuideMode = false;
		Global.GuideRole = GetRole();
		
		PlayerPrefs.SetString(Global.SaveRoleID,Global.GuideRole.ToString());
		
		UnitManager.Instance.LocalPlayer = null;	

		SceneList SLItem = SceneListManager.Instance.GetItem((int)ESceneID.CreateRole,(int)ESceneType.CreateRole);
		Global.CurSceneInfo.ID = ESceneID.CreateRole;
		Global.CurSceneInfo.Type = ESceneType.CreateRole;
		Global.CurSceneInfo.Data = 0;
		Global.CurSceneInfo.Name = SLItem.Name;
		MainScript.Instance.LoadLevel(Global.CurSceneInfo);	
		
		
		//SceneList scene = SceneListManager.Instance.GetItem((int)ESceneID.Startup, (int)ESceneType.Startup);
		//Application.LoadLevel(scene.Name);
	}
	
	void OnClickTryAgain(GameObject go)
	{
		Destroy(this.gameObject);
		UnitManager.Instance.CleanEmptyObject();
		Time.timeScale = 1;
		
//		SceneList SLItem = SceneListManager.Instance.GetItem((int)ESceneID.A_FirstGameGuide,(int)ESceneType.GameGuide);
//		Global.CurSceneInfo.ID = ESceneID.Startup;
//		Global.CurSceneInfo.Type = ESceneType.Startup;
//		Global.CurSceneInfo.Data = 0;
//		Global.CurSceneInfo.Name = SLItem.Name;		
//		MainScript.Instance.LoadLevel(Global.CurSceneInfo);	
		
		SceneList scene = SceneListManager.Instance.GetItem((int)ESceneID.A_FirstGameGuide, (int)ESceneType.GameGuide);
		Application.LoadLevel(scene.Name);
	}
	
	int GetRole()
	{
		switch(mGuidUnitID)
		{
		case 1100:
			return 1003;
		case 1101:
			return 1004;
		case 1102:
			return 1005;
		}
		return 0;
	}
	
	void OnClickNormal(GameObject go, bool press)
    {
        if (Global.GInputBox == null || Time.timeScale == 0 || Global.PauseAll) return;

		ActionStatus actionStatus = UnitManager.Instance.LocalPlayer.ActionStatus;
        actionStatus.SkillItem = null;
		if (actionStatus.SkillItem != null)
			Debug.LogError("ClearSkillItemError.");
        if (press)
		{
            Global.GInputBox.OnKeyDown(EKeyList.KL_Attack);
		}
        else
            Global.GInputBox.OnKeyUp(EKeyList.KL_Attack);
    }
	
	void OnNormalBtnEnable(GameObject go)
	{
		UISprite nomalIcon = GameObject.Find("NormalIcon").GetComponent<UISprite>();
		if(mGuidUnitID == 1100)
			nomalIcon.spriteName = "Button25_Attack_Warrior_01";
		else if(mGuidUnitID == 1101)
			nomalIcon.spriteName = "Button25_Attack_Assassin_01";
		else
			nomalIcon.spriteName = "Button25_Attack_Wizard_01";
	}
	void AssignSkill()
	{
		int index = 0;
		for (int i = 0; i < 9; i++)
        {
            DefaultEquip defaultEquip = DefaultEquipManager.Instance.GetItem(mGuidUnitID, i+1);
            if (defaultEquip == null)
                break;
			ItemBase item = ItemBaseManager.Instance.GetItem(defaultEquip.Item); 
			if(item.MainType == (int)EItemType.Skill)
			{
				SkillItem skill = new SkillItem(defaultEquip.Item, defaultEquip.SkillLevel);
				SkillList.Add(skill);
				index++;
			}
        }
	}
	
	void OnSelectCharacter(GameObject go)
	{
		mGuidUnitID = int.Parse(go.name);
		mTalk.SetActive(false);
		mSelectCharacter.SetActive(false);
		OnSelectComplete();
	}
	
	void OnBloodEnable(GameObject go)
	{
		UpdateHp(UnitManager.Instance.LocalPlayer.GetCurrentHp(), UnitManager.Instance.LocalPlayer.GetMaxHp());
	}
	
	void OnAutoSelect(UITweener tween)
	{
		// WARRIOR.
		mGuidUnitID = 1100;
		mSelectCharacter.SetActive(false);
		OnSelectComplete();
	}
	
	void OnSelectComplete()
	{
		foreach(Unit unit in UnitManager.Instance.UnitInfos)
		{
			if( unit.UGameObject.GetComponent<Controller>() != null)
			{
				if(unit.UnitID == mGuidUnitID )
					UnitManager.Instance.LocalPlayer = unit as LocalPlayer;
				unit.UGameObject.GetComponent<Controller>().enabled = false;
			}
		}
		UnitManager.Instance.LocalPlayer.UGameObject.GetComponent<Controller>().enabled = true;
		AssignSkill();
		GameObject.Find(string.Format("Role{0}", mGuidUnitID - 1099)).GetComponent<TriggerCellEx>().OnExecution(true);
	}
	
	public void UpdateHp(int curHp, int maxHp)
    {
		if( mSliderHP == null || mHPLabel == null)
			return;
		curHp = curHp < 0 ? 0 : curHp; 
        float percent = (float)curHp / maxHp;
        mSliderHP.sliderValue = percent;
        mHPLabel.text = string.Format("{0}/{1}", curHp, maxHp);
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
}
