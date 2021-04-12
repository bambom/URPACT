using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class PackSheet
{
	private int mIndex;
	private int mItemCountMax;
	private GameObject mGoThis;
	private PackItemPosInfo mPos = new PackItemPosInfo();
	public  List<PackItem> mItemList = new List<PackItem>();
	private UIEventListener.VoidDelegate mOnSheetClick;
	
	public GameObject goSheetRoot {get { return mGoThis ;}}
	
	public PackSheet(GameObject goParent,int CountPerPage,int index)
	{
		GameObject GoRoot = new GameObject();
		mGoThis = GoRoot;
		mGoThis.transform.parent = goParent.transform;
		mGoThis.name = index.ToString();
		
		mGoThis.layer = 8;
		mGoThis.transform.localPosition = Vector3.zero;
		mGoThis.transform.localScale = Vector3.one;
		
		mIndex = index;
		mItemCountMax = CountPerPage;
	    CreateSheet();
	}
	
	void CreateSheet()
	{
		if( mItemList.Count >0 )
			mItemList.Clear();
		
		
		for(int i = 0; i< mItemCountMax; i++){
			PackItem Pitem = new PackItem(i);
			Pitem.ThisObject.transform.parent = mGoThis.transform;
			Pitem.ThisObject.transform.localPosition = Vector3.zero;
			Pitem.ThisObject.transform.localScale = 
				new Vector3(PackItemPosInfo.RowMargin,PackItemPosInfo.ColumnMargin,1.0f);
			
			mPos.Row = (PackItemPosInfo.RowMax - i/ PackItemPosInfo.RowMax);
			mPos.Column = (i% PackItemPosInfo.ColumnMax);
			
			Pitem.SetPos(mPos.Row,mPos.Column);
			mItemList.Add(Pitem);
		}	
		ResetItemPostionByRowColumn();
	}
	
	void ResetItemPostionByRowColumn()
	{
		Vector3 Vpos = Vector3.zero;
		
		//Anchor something
		float BasePosX = PackItemPosInfo.RowMargin*2*PackItemPosInfo.RowMax;
		float BasePosY = PackItemPosInfo.ColumnMargin*2*PackItemPosInfo.ColumnMax;
		BasePosX = BasePosX/2 - PackItemPosInfo.RowMargin ;
		BasePosY = BasePosY/2 + PackItemPosInfo.ColumnMargin;
		
		for(int i = 0;i< mItemList.Count ;i++){			
			Vpos.x = 0 - BasePosX + mItemList[i].Pos.Column*  PackItemPosInfo.ColumnMargin*2;
			Vpos.y = 0 - BasePosY + mItemList[i].Pos.Row *  PackItemPosInfo.RowMargin*2;
			//Vpos.z = mItemList[i].Pos.PosZ;
			mItemList[i].ThisObject.transform.localPosition = Vpos;
		}
	}
	
	void SheetReset()
	{
		for(int i=0;i<mItemCountMax;i++){
			mItemList[i].Update(null);
		}
	}
	
	bool ItemTypeContain(Item item,int MainTF,int MainTT,int SubTF,int SubTT)
	{
		//  except Wrist
		if( item.ItemBase.MainType == (int)EItemType.Equip 
			&& SubTF < (int)EquipSubType.EST_Wrist 
			&& SubTT > (int)EquipSubType.EST_Wrist
			&& item.ItemBase.SubType == (int)EquipSubType.EST_Wrist)
		{
			return false;
		}
		if( (int)item.ItemBase.MainType <= MainTT && (int)item.ItemBase.MainType >= MainTF 
				&& (int)item.ItemBase.SubType <= SubTT && (int)item.ItemBase.SubType >= SubTF){
			return true;	
		}
		return false;
	}
	
	void OnClickItem(GameObject go)
	{
		if( mOnSheetClick != null)
			mOnSheetClick(go);
	}
	

	#region Sort
	public void UpdateByType(List<PackageItem> Items,int index,
								int MainTF,int MainTT,int SubTF,int SubTT)
	{
		int nCount = 0;
		SheetReset();
		for(int i = 0; i<Items.Count; i++)
		{
			if( ItemTypeContain(Items[i].PackItm,MainTF,MainTT,SubTF,SubTT) ){
				if( nCount >= index*mItemCountMax ){
					mItemList[nCount-index*mItemCountMax].Update(Items[i].PackItm);
				}
				nCount++;				
				if(nCount >= (index+1)*mItemCountMax)
					return;
			}
		}
	}	

	public void Update(List<PackageItem> Items,int Page)
	{
		int nCount = 0;
		SheetReset();
		for(int i=0; i<mItemCountMax; i++){
			mItemList[i].PackItemEventInit(OnClickItem);
			if( Items.Count > Page*mItemCountMax+i)
				mItemList[i].Update(Items[Page*mItemCountMax+i].PackItm);
		}
	}

	public void PackSheetInfoInit(List<PackageItem> Items,int Page,UIEventListener.VoidDelegate OnClick)
	{
		int nCount = 0;
		mOnSheetClick = OnClick;
		
		for(int i=0; i<mItemCountMax; i++){
			mItemList[i].PackItemEventInit(OnClickItem);
			if( Items.Count > Page*mItemCountMax+i)
				mItemList[i].PackItemInfoInit(Items[Page*mItemCountMax+i].PackItm);
		}
	}	
	#endregion Sort
	
	
	#region NoSort
	public void UpdateByType(Dictionary<string, Item> Items,int index,
								int MainTF,int MainTT,int SubTF,int SubTT)
	{
		int nCount = 0;
		SheetReset();
		foreach(Item item in Items.Values){
			if( ItemTypeContain(item,MainTF,MainTT,SubTF,SubTT) ){
				if( nCount >= index*mItemCountMax ){
					mItemList[nCount-index*mItemCountMax].Update(item);
				}
				nCount++;				
				if(nCount >= (index+1)*mItemCountMax)
					return;
			}
		}
	}
		
	public void Update(Dictionary<string, Item> Items,int index)
	{
		int nCount = 0;
		SheetReset();
		foreach(Item item in Items.Values){
			if( nCount >= index*mItemCountMax ){
				mItemList[nCount-index*mItemCountMax].Update(item);
			}
			nCount++;
			
			if(nCount >= (index+1)*mItemCountMax)
				return;
		}
	}
		
	public void PackSheetInfoInit(Dictionary<string, Item> Items,int Page,UIEventListener.VoidDelegate OnClick)
	{
		int nCount = 0;
		mOnSheetClick = OnClick;
		
		for(int i=0;i<mItemCountMax;i++){
			mItemList[i].PackItemEventInit(OnClickItem);
		}
		
		foreach(Item item in Items.Values){
			if( nCount >= Page*mItemCountMax ){
				mItemList[nCount-Page*mItemCountMax].PackItemInfoInit(item);
			}
			nCount++;
			
			if(nCount >= (Page+1)*mItemCountMax)
				return;
		}
	}	
	#endregion NoSort
}

