using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GeniusWnd : Window<GeniusWnd> {

	public override string PrefabName { get { return "GeniusWnd"; } }
	
	const int SkillButtonInterval = 200;
	GameObject mSelectFrame;
	GameObject mSkillDesc;
	GameObject mMoney;
	int mSelectButton = 0;
	int mSelectSkillID = 0;
	Object mSkillButton;
	MainAttrib mPlayerAttrib;
	List<ItemBase>mPassiveSkill = new List<ItemBase>();
	List<GameObject>mSkillButtonArray = new List<GameObject>();
	
 	protected override bool OnOpen()
	{
		mPlayerAttrib = PlayerDataManager.Instance.Attrib;
		mSelectFrame = Control("SelectFrame");
		mSkillButton = Resources.Load("LearnSkillButton");
		mSkillDesc = Control("SkillDescription");
		mMoney = Control("Money");
		UIEventListener.Get(Control("Return")).onClick = OnClickbtnReturn;
		UIEventListener.Get(Control("LearnSkill")).onClick = OnClickLearnButton;
		
		GetAllPassiveSkill();
		ShowPassiveSkill();
		WinStyle = WindowStyle.WS_Ext;
		return base.OnOpen();		
	}
	
	private	void OnClickbtnReturn ( GameObject go)
	{
		Close();
		if(!InGameMainWnd.Exist)
			InGameMainWnd.Instance.Open();
		else
			InGameMainWnd.Instance.Show();
	}
	
	void GetAllPassiveSkill()
	{
		LocalPlayer player = UnitManager.Instance.LocalPlayer as LocalPlayer;
		int roleID = player.PlayerData.Attrib.Role;
		ItemBase[] itemArray = ItemBaseManager.Instance.GetAllItem();
		foreach(ItemBase item in itemArray)
		{
			if(item.Role == roleID && item.MainType == (int)EItemType.Skill && item.SubType == 2)
					mPassiveSkill.Add(item);
		}	
	}
	
	void ShowPassiveSkill()
	{
		if(mSkillButtonArray.Count == 0)
			CreateSkillButton();
		else
		{
			for(int i=0; i<mSkillButtonArray.Count; i++)
			{
				GameObject skillButton = mSkillButtonArray[i];
				skillButton.SetActive(true);
				ShowSkillInfo(skillButton, i);
			}
		}	
		OnClickSkillButton(mSkillButtonArray[mSelectButton]);
	}
	
	void CreateSkillButton()
	{
		for(int i=0; i<mPassiveSkill.Count; i++)
		{
			GameObject skillButton = GameObject.Instantiate(mSkillButton) as GameObject;
			skillButton.transform.parent = WndObject.transform;
			skillButton.transform.localPosition = new Vector3(i*SkillButtonInterval, 0, 0);
			skillButton.transform.localScale = Vector3.one;
			skillButton.transform.Find("Transform/StoneSlot").gameObject.SetActive(false);
			skillButton.transform.Find("Transform/Activation").GetComponent<UISprite>().gameObject.SetActive(false);
			ItemBase item = ItemBaseManager.Instance.GetItem( mPassiveSkill[i].ID);
			skillButton.transform.Find("Icon").GetComponent<UISprite>().spriteName = item.Icon;
			UIEventListener.Get(skillButton).onClick = OnClickSkillButton;
			skillButton.name = i.ToString();

			ShowSkillInfo(skillButton, i);
			
			mSkillButtonArray.Add(skillButton);
		}
	}
	
	void ShowSkillInfo(GameObject skillButton, int buttonIndex)
	{
		int skillID = mPassiveSkill[buttonIndex].ID;
		int level = GetSkillLevel( skillID );
		skillButton.transform.Find("Level").GetComponent<UILabel>().text = level.ToString();
	}
	
	void OnClickSkillButton(GameObject go)
	{	
		mSelectButton = int.Parse(go.name);
		
		mSelectFrame.transform.parent = go.transform.Find("Transform");
		mSelectFrame.transform.localPosition = Vector3.zero;
		
		int skillID = mPassiveSkill[mSelectButton].ID;
		mSelectSkillID = skillID;
		Debug.Log(mSelectSkillID);
		
		int level = GetSkillLevel(mSelectSkillID);
		level = level <= 0 ? 1 : level;
		SkillAttrib attrib = SkillAttribManager.Instance.GetItem(skillID, level);
		if(attrib != null)
		{
			mSkillDesc.GetComponent<UILabel>().text = attrib.Desc;
			ShowPrice(attrib);
		}
	}
	
	void ShowPrice(SkillAttrib attrib)
	{
		int gold = attrib.Gold;
		if(gold != 0)
		{
			mMoney.transform.Find("Sptite").GetComponent<UISprite>().spriteName = "Icon01_System_Gold_01";
			mMoney.transform.Find("Label").GetComponent<UILabel>().text = attrib.Gold.ToString();
		}
		else
		{
			mMoney.transform.Find("Sptite").GetComponent<UISprite>().spriteName = "Icon01_System_Gem_01";
			mMoney.transform.Find("Label").GetComponent<UILabel>().text = attrib.Gem.ToString();
		}
	}
	
	void OnClickLearnButton(GameObject go)
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
	
	bool LearnCheck(int level)
	{
		SkillAttrib attrib = SkillAttribManager.Instance.GetItem(mSelectSkillID, level);
		if (attrib == null)
		{
			MessageBoxWnd.Instance.Show(("ThereisNoThisSkill"));
			//Debug.LogError("skill not found! maybe has reached the highest level!");
			return false;
		}
		if( mPlayerAttrib.Gold < attrib.Gold )
		{
			MessageBoxWnd.Instance.Show(("NotEnoughGold"));
			//Debug.LogError("money not enough!");
			return false;
		}
		if( mPlayerAttrib.Level < attrib.LevelRequest)
		{
			MessageBoxWnd.Instance.Show(string.Format(("NotEnoughLevel"), mPlayerAttrib.Level, attrib.Level));
			//Debug.LogError(string.Format("level not enough! {0} < {1}", mPlayerAttrib.Level, attrib.Level));
			return false;
		}
		return true;
	}
	
	void OnLearnSkill()
	{
		Request(new SkillLearnCmd(mSelectSkillID), delegate(string err, Response response)
        {
            if (string.IsNullOrEmpty(err))
				UpdateSkillData();
			else
			{
				MessageBoxWnd.Instance.Show(("LearnSkillFailed"));
				Debug.LogError(err);
			}
        });
	}
	
	void OnUpgradeSkill()
	{
		Request(new SkillUpgradeCmd(mSelectSkillID), delegate(string err, Response response)
        {
            if (string.IsNullOrEmpty(err))
				UpdateSkillData();
			else
			{
				MessageBoxWnd.Instance.Show(("UpdateSkillFailed"));
				Debug.LogError(err);
			}
        });
	}
	
	void UpdateSkillData()
	{
		Request(new GetSkillPackCmd(), delegate(string err, Response response)
        {
            if (!string.IsNullOrEmpty(err))
            {
				MessageBoxWnd.Instance.Show(("RequsetSkillInfo"));
                return;
            }
            Package skillData = (response != null) ? response.Parse<Package>() : null;
			// update the package
            if (skillData != null)
				PlayerDataManager.Instance.OnSkillPack(skillData);
			ShowPassiveSkill();
        });
	}
	
	int GetSkillLevel(int skillID)
	{
		int level;
		SkillItem skill = SkillDataManager.Instance.GetSkillItem(skillID);
		level = skill != null ? skill.Lv: 0;
		return level;
	}
	
}
