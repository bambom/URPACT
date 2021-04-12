using UnityEngine;
using System.Collections;

	
public class PackItem : ControlCommon
{
	public string PrefabName { get { return "PackBaseItem"; } }
	private int mIndex;
	
	public PackItemPosInfo Pos = new PackItemPosInfo();
	public GameObject ThisObject;
	
	private GameObject mStatus; //NEW
	private GameObject mEquipCheck; //Equip Or Not
	private GameObject mIconBg; //ItemIcon
	private GameObject mQualityBg; //ItemBaseAttribe
	private GameObject mItemID; 
	private GameObject mFocusRoot;
	private GameObject mCountNo;
	private GameObject mEquipComp;
	
	private UIEventListener.VoidDelegate mOnItemClick;
	
	public PackItem(int index)
	{
		if( null == ThisObject ){	
			ThisObject = GameObject.Instantiate(Resources.Load(PrefabName)) as GameObject;
			ThisObject.name = PrefabName + index.ToString();
			mIndex = index;
			PackItemPosInfo.RowMargin = ThisObject.transform.localScale.x;
			PackItemPosInfo.ColumnMargin = ThisObject.transform.localScale.y;
		}
		GObjectInit();
		ResetItem();
	}

	public void SetPos(int Row, int Column)
	{
		Pos.Row = Row;
		Pos.Column = Column;
	}
	
	private void GObjectInit()
	{
		mStatus = ThisObject.transform.Find("Status").gameObject;
		mEquipCheck = ThisObject.transform.Find("EquipCheck").gameObject;	
		mIconBg	= ThisObject.transform.Find("Icon/Background").gameObject;	
		mQualityBg = ThisObject.transform.Find("QualityFrame").gameObject;	
		mItemID	= ThisObject.transform.Find("ItemID").gameObject;	
		mFocusRoot = ThisObject.transform.Find("FocusRoot").gameObject;	
		mCountNo = ThisObject.transform.Find("CountNo/LabelCount").gameObject;
		mEquipComp = ThisObject.transform.Find("EquipComp").gameObject;
	}
	
	public void ResetItem()
	{
		mStatus.SetActive(false);	
		mEquipCheck.SetActive(false);	 
		mIconBg.SetActive(false);	
		mQualityBg.SetActive(false);	
		mCountNo.SetActive(false);	
		mEquipComp.SetActive(false);
		mItemID.GetComponent<UILabel>().text = (-1).ToString();	
	}
	
	public void UpdateByItemID()
	{
		string strID = mItemID.GetComponent<UILabel>().text;
		int ID = int.Parse(strID);
		Item item = PlayerDataManager.Instance.BackPack.Find(ID);
		if(item == null)
			return;
		
		Update(item);
	}
	
	
	public void Update(Item item)
	{
		if(item == null){
			ResetItem();
			return;
		}
		
		mItemID.GetComponent<UILabel>().text = item.ID.ToString();	
		UpdateItemQualityFrameIcon(item.ItemBase.Quality,mQualityBg);
		mIconBg.SetActive(true);
		
		if( item.ItemBase.MainType == (int)EItemType.Equip ){
			mIconBg.GetComponent<UISprite>().spriteName = item.ItemBase.Icon;
			if( item.ToEquipItem().Equiped ){
				mEquipCheck.SetActive(true);
			}else{
				mEquipCheck.SetActive(false);	
			}
		}else{
			mCountNo.SetActive(true);
			mCountNo.GetComponent<UILabel>().text= item.Num.ToString();
			mIconBg.GetComponent<UISprite>().spriteName =  item.ItemBase.Icon;
		}
		
		if ( BackPack.ItemIDMax < item.ID )
		{
			mStatus.SetActive(true);
		}else{
			if( mStatus.activeSelf )
				mStatus.SetActive(false);
		}
		
		UpdateEquipGS(item);
	}
	
	#region MainAttribe
	int GetMainAttribBySubType(Item item)
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
			//Control("LabelMain").GetComponent<UILabel>().text = Ta.Attrib + " :" + item.ToEquipItem().GetAttrib(EquipAttrib.HPMax);
		}
		return 0;
	}
	
	int CompareWithTwoEquip(Item item)
	{
		if(item.ToEquipItem()== null || 
			(int)item.ItemBase.MainType != (int)EItemType.Equip){
			Debug.Log("Item is not Equip");
			return 0;
		}
		
		foreach(Item itemTmp in PlayerDataManager.Instance.BackPack.Items.Values){
			if(itemTmp.ID == item.ID || (int)itemTmp.ItemBase.MainType != (int)EItemType.Equip )
				continue;
			
			if( itemTmp.ItemBase.SubType == item.ItemBase.SubType
					&& (itemTmp.ToEquipItem().Equiped)){
				return GetMainAttribBySubType(item) 
						- GetMainAttribBySubType(itemTmp);
			}
		}
		return GetMainAttribBySubType(item);
	}
		
	int CompareWithTwoEquipGScore(Item item)
	{
		if(item.ToEquipItem()== null || 
			(int)item.ItemBase.MainType != (int)EItemType.Equip){
			Debug.Log("Item is not Equip");
			return 0;
		}
		
		foreach(Item itemTmp in PlayerDataManager.Instance.BackPack.Items.Values){
			if(itemTmp.ID == item.ID || (int)itemTmp.ItemBase.MainType != (int)EItemType.Equip )
				continue;
			
			if( itemTmp.ItemBase.SubType == item.ItemBase.SubType
					&& (itemTmp.ToEquipItem().Equiped)){
				return GetEquipScoreBySubType(item) 
						- GetEquipScoreBySubType(itemTmp);
			}
		}
		return GetEquipScoreBySubType(item);
	}
	#endregion MainAttribe 
	
	public void UpdateEquipGS(Item item)
	{
		if( item.ItemBase.MainType == (int)EItemType.Equip ){
			if(item.ToEquipItem().Equiped){
				if(mEquipComp.activeSelf)
					mEquipComp.SetActive(false);
				return;
			}
			int Delta = CompareWithTwoEquipGScore(item);  //CompareWithTwoEquip(item);
			if(!mEquipComp.activeSelf)
				mEquipComp.SetActive(true);	
			
			if(Delta > 0){
				mEquipComp.transform.Find("Background").GetComponent<UISprite>().spriteName
					= "Button05_Package_Compare_01";
				mEquipComp.transform.Find("LabelNo").GetComponent<UILabel>().text
					= //"[00FF00]" + 
						Mathf.Abs(Delta).ToString();
						//+ "[-]";		
			}else if(Delta < 0){
				mEquipComp.transform.Find("Background").GetComponent<UISprite>().spriteName
					= "Button05_Package_Compare_02";
				mEquipComp.transform.Find("LabelNo").GetComponent<UILabel>().text
					= //"[FF0000]" +
						Mathf.Abs(Delta).ToString();
						// + "[-]";		
			}else {
				if(mEquipComp.activeSelf)
					mEquipComp.SetActive(false);	
			}
		}else{
			if(mEquipComp.activeSelf)
				mEquipComp.SetActive(false);	
		}
	}
	
	public void OnItemClicked(GameObject go)
	{
		if(mOnItemClick!=null)
			mOnItemClick(go);
		if(mStatus.activeSelf)
			mStatus.SetActive(false);
	}
	
	public void PackItemEventInit(UIEventListener.VoidDelegate OnClick)
	{
		mOnItemClick = OnClick;
		UIEventListener.Get(mIconBg.transform.parent.gameObject).onClick = OnItemClicked;
	}
	
	public void PackItemInfoInit(Item item)
	{
		Update(item);
	}
	

	
}

