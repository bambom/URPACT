using UnityEngine;
using System.Collections;

public class ControlCommon
{
//	GameObject mGoControlRoot;
//	public GameObject GoObject { get { return mGoControlRoot; } }
//	
//	public ControlCommon(GameObject goRoot)
//	{
//		mGoControlRoot = new GameObject( "CommonRoot" );
//		/*
//		mGoControlRoot = GameObject.Instantiate(Resources.Load(PrefabName)) as GameObject;
//		mGoControlRoot.name = "ControlCommon";
//		mGoControlRoot.transform.parent = goRoot.transform;
//		mGoControlRoot.transform.localScale = Vector3.one;
//		mGoControlRoot.transform.localPosition = Vector3.zero;
//		*/
//	}
	
	public string TextColorGet(bool bStaus)
	{
		if(bStaus)
			return "[FF0000]";
		else
			return "[FFFFFF]";
	}
	
	public string TextColorGet(int Quality)
	{
		string color = "";
		switch(Quality)
		{
			case (int)ItemQuality.IQ_White: color ="[FFFFFF]";break;
			case (int)ItemQuality.IQ_Green: color ="[3BBC34]";break;
			case (int)ItemQuality.IQ_Blue:  color ="[0F7BBF]";break;
			case (int)ItemQuality.IQ_Purple:color ="[9035A6]";break;
			default: color ="[FFFFFF]";break;
		}
		return color;
	}
	
	public	void DestroyGridListItem( Transform goTrans )
    {
		if( goTrans.childCount == 0 )
			return;
		
		for(int i=0 ;i<goTrans.childCount ;i++)
			GameObject.Destroy(goTrans.GetChild(i).gameObject);
        goTrans.DetachChildren();
    }
	
	
	public void BtnSetAble(GameObject go,bool bStatus)
	{
		GameObject goDis = go.transform.Find("Disable").gameObject;
		if(goDis == null)
			return;
		
		go.GetComponent<BoxCollider>().enabled = bStatus;
		goDis.SetActive(!bStatus);
		if(bStatus)//NGUI Bug
		{
			UIImageButton uiib = go.GetComponent<UIImageButton>();
			go.transform.Find("Background").GetComponent<UISprite>().spriteName = uiib.normalSprite;
		}
	}
	
	
	public  string ItemIconGet(ItemBase itemBase)
	{
		if( itemBase.MainType == (int)EItemType.Equip )
		{
			switch((int)itemBase.SubType)
			{
				case (int)EquipSubType.EST_Weapon://武器
					return "Button01_Weapon_01";
				case (int)EquipSubType.EST_Helmet://头盔
					return "Button01_Helmet_01";
				case (int)EquipSubType.EST_Armor://上衣
					return "Button01_Armor_01";
				case (int)EquipSubType.EST_Shoulder://肩甲
					return "Button01_Shoulder_01";
				case (int)EquipSubType.EST_Shoe://鞋子
					return "Button01_Boots_01";
				case (int)EquipSubType.EST_Wrist://护腕
					return "Button01_Bracers_01";
				case (int)EquipSubType.EST_NeckLace://项链
					return "Button01_Accessory_01";
				case (int)EquipSubType.EST_WaistBand://腰带
					return "Button01_Accessory_01";
				//case (int)EquipSubType.EST_Wrist://护腕
				case (int)EquipSubType.EST_Ring://戒指	
					return "Button01_Accessory_01";	
			}
		}else{
			return "Button01_Item_01";
		}	
		return "Button01_Item_01";
	}

	public 	bool CheckEuipHasBaseAttrib(ItemBase itembase)
	{
		if( itembase.MainType == (int)EItemType.Equip )
		{
			switch(itembase.SubType)
			{
				case (int)EquipSubType.EST_Weapon://武器
				case (int)EquipSubType.EST_Helmet://头盔
				case (int)EquipSubType.EST_Armor://上衣
				case (int)EquipSubType.EST_Shoulder://肩甲
				case (int)EquipSubType.EST_Shoe://鞋子
				case (int)EquipSubType.EST_Wrist://护腕
						return true;
				case (int)EquipSubType.EST_NeckLace://项链
				case (int)EquipSubType.EST_WaistBand://腰带
				//case (int)EquipSubType.EST_Wrist://护腕
				case (int)EquipSubType.EST_Ring://戒指	
						return false;
				default:return false;
			}
		}
		return false;
	}
	
	public int GetMainAttirbBaseBySubType(ItemBase itemBase)
	{
		EquipBase EBase = EquipBaseManager.Instance.GetItem(itemBase.ID);
		switch(itemBase.SubType){
			case (int)EquipSubType.EST_Weapon://武器
					return EBase.Damage;
			case (int)EquipSubType.EST_Helmet://头盔
					return EBase.HPMax;
			case (int)EquipSubType.EST_Armor://上衣
					return EBase.Defense;
			case (int)EquipSubType.EST_Shoulder://肩甲
					return EBase.HPMax;
			case (int)EquipSubType.EST_Shoe://鞋子
					return EBase.SpecialDefense;
			case (int)EquipSubType.EST_NeckLace://项链
					return EBase.Hit;
			case (int)EquipSubType.EST_WaistBand://腰带
					return EBase.Block;
			case (int)EquipSubType.EST_Wrist://护腕
					return EBase.SpecialDamage;
			case (int)EquipSubType.EST_Ring://戒指	
					return EBase.Critical;
		}
		return 0;
	}
	
	public int GetMainAttirbBaseBySubType(Item item)
	{
		switch(item.ItemBase.SubType){
			case (int)EquipSubType.EST_Weapon://武器
					return item.ToEquipItem().EquipBase.Damage;
			case (int)EquipSubType.EST_Helmet://头盔
					return item.ToEquipItem().EquipBase.HPMax;
			case (int)EquipSubType.EST_Armor://上衣
					return item.ToEquipItem().EquipBase.Defense;
			case (int)EquipSubType.EST_Shoulder://肩甲
					return item.ToEquipItem().EquipBase.HPMax;
			case (int)EquipSubType.EST_Shoe://鞋子
					return item.ToEquipItem().EquipBase.SpecialDefense;
			case (int)EquipSubType.EST_NeckLace://项链
					return item.ToEquipItem().EquipBase.Hit;
			case (int)EquipSubType.EST_WaistBand://腰带
					return item.ToEquipItem().EquipBase.Block;
			case (int)EquipSubType.EST_Wrist://护腕
					return item.ToEquipItem().EquipBase.SpecialDamage;
			case (int)EquipSubType.EST_Ring://戒指	
					return item.ToEquipItem().EquipBase.Critical;
		}
		return 0;
	}

	public int GetMainAttirbExtraBySubType(ItemBase itemBase)
	{
		return 0;
	}	
	
	public int GetMainAttirbExtraBySubType(Item item)
	{
		EquipAttrib EA = EquipAttrib.Damage;
		switch(item.ItemBase.SubType){
			case (int)EquipSubType.EST_Weapon://武器
					EA = EquipAttrib.Damage;break;
			case (int)EquipSubType.EST_Helmet://头盔
					EA = EquipAttrib.HPMax;break;
			case (int)EquipSubType.EST_Armor://上衣
					EA = EquipAttrib.Defense;break;
			case (int)EquipSubType.EST_Shoulder://肩甲
					EA = EquipAttrib.HPMax;break;
			case (int)EquipSubType.EST_Shoe://鞋子
					EA = EquipAttrib.SpecialDefense;break;
			case (int)EquipSubType.EST_NeckLace://项链
					EA = EquipAttrib.Hit;break;
			case (int)EquipSubType.EST_WaistBand://腰带
					EA = EquipAttrib.Block;break;
			case (int)EquipSubType.EST_Wrist://护腕
					EA = EquipAttrib.SpecialDamage;break;
			case (int)EquipSubType.EST_Ring://戒指	
					EA = EquipAttrib.Critical;break;
			default:return 0;
		}
		return item.GetAttrib(EA.ToString());
		//return 0;
	}	
		
	public int GetMainAttribAllBySubType(Item item)
	{
		switch(item.ItemBase.SubType){
			case (int)EquipSubType.EST_Weapon://武器
					return item.ToEquipItem().GetAttrib(EquipAttrib.Damage);
			case (int)EquipSubType.EST_Helmet://头盔
					return item.ToEquipItem().GetAttrib(EquipAttrib.HPMax);
			case (int)EquipSubType.EST_Armor://上衣
					return item.ToEquipItem().GetAttrib(EquipAttrib.Defense);
			case (int)EquipSubType.EST_Shoulder://肩甲
					return item.ToEquipItem().GetAttrib(EquipAttrib.HPMax);
			case (int)EquipSubType.EST_Shoe://鞋子
					return item.ToEquipItem().GetAttrib(EquipAttrib.SpecialDefense);
			case (int)EquipSubType.EST_NeckLace://项链
					return item.ToEquipItem().GetAttrib(EquipAttrib.Hit);
			case (int)EquipSubType.EST_WaistBand://腰带
					return item.ToEquipItem().GetAttrib(EquipAttrib.Block);
			case (int)EquipSubType.EST_Wrist://护腕
					return item.ToEquipItem().GetAttrib(EquipAttrib.SpecialDamage);
			case (int)EquipSubType.EST_Ring://戒指	
					return item.ToEquipItem().GetAttrib(EquipAttrib.Critical);
		}
		return 0;
	}
	
	public int GetMainAttribAllBySubType(ItemBase itemBase)
	{
		return GetMainAttirbBaseBySubType(itemBase);	
	}
	
	//Had Strengthen  Already Count
	public int GetEquipStrengthenCount(Item item)
	{
		if(item.ItemBase.MainType == (int)EItemType.Equip)
		{
			//return item.ToEquipItem().GetAttrib(EquipAttrib.StrengthenCount);
			EquipBase EBase = EquipBaseManager.Instance.GetItem(item.ItemBase.ID);
			return EBase.StrengthenCount;
		}
		return 0;
	}
	
	//Get Strengthen Count
	public int GetEquipStrengthenCount(ItemBase itemBase)
	{
		if(itemBase.MainType == (int)EItemType.Equip){
			EquipBase EBase = EquipBaseManager.Instance.GetItem(itemBase.ID);
			return EBase.StrengthenCount;
		}
		return 0;
	}
	
	public int GetEquipScoreBySubType(ItemBase itemBase)
	{
		return itemBase.Score;
	}
	
	public int GetEquipScoreBySubType(Item item)
	{
		return item.ItemBase.Score;
	}
//	public GameObject Control(string name)
//    {
//        return Control(name, mGoControlRoot);
//    }
//	
//	public GameObject Control(string name, GameObject parent)
//    {
//        Transform[] children = parent.GetComponentsInChildren<Transform>();
//        foreach (Transform child in children){
//            if (child.name == name)
//                return child.gameObject;
//        }
//        return null;
//    }
//	
//	public void Destroy()
//	{
//		if ( mGoControlRoot != null )
//			GameObject.Destroy(mGoControlRoot);
//		mGoControlRoot = null;
//	}
	
	
	
	public void UpdateItemQualityFrameIcon(int Quality, GameObject goQuality)
	{
		QualitySprite QSp = QualitySprite.Button10_BaseItem_Quality_00;
		switch( Quality )
		{
			case (int) QualitySprite.Button10_BaseItem_Quality_00:
				QSp = QualitySprite.Button10_BaseItem_Quality_00;
				goQuality.SetActive(false);
				break;
			case (int) QualitySprite.Button10_BaseItem_Quality_01:
				QSp = QualitySprite.Button10_BaseItem_Quality_01;
				goQuality.SetActive(true);
				goQuality.GetComponent<UISprite>().spriteName= QSp.ToString();
				break;
			case (int) QualitySprite.Button10_BaseItem_Quality_02:
				QSp = QualitySprite.Button10_BaseItem_Quality_02;
				goQuality.SetActive(true);
				goQuality.GetComponent<UISprite>().spriteName= QSp.ToString();
				break;
			case (int) QualitySprite.Button10_BaseItem_Quality_03:
				QSp = QualitySprite.Button10_BaseItem_Quality_03;
				goQuality.SetActive(true);
				goQuality.GetComponent<UISprite>().spriteName= QSp.ToString();
				break;
			case (int) QualitySprite.Button10_BaseItem_Quality_04:
				QSp = QualitySprite.Button10_BaseItem_Quality_04;
				goQuality.SetActive(true);
				goQuality.GetComponent<UISprite>().spriteName= QSp.ToString();
				break;
			default:
				goQuality.SetActive(false);
				break;
		}
	}
	
	
}

