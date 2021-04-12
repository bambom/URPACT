using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BackPack
{
	GameObject mGoPackBack;
	GameObject mGoPoint;
	static GameObject mGoFocus;
	int mPageCountMax = 3;
	int mCountPerPage = 16;
	static int msCountPerPage = 16;
	
	static int mItemIDMax = 0;
	static int mItemNewCount = 0;
	public static int ItemIDMax { get { return mItemIDMax; }}
	
	List<PackSheet> mSheetList = new List<PackSheet>();
	SheetPoint mSpoint;
	UIEventListener.VoidDelegate mOnPackClick;
	
	public static int CountPerPage {get{return msCountPerPage;}}
	public SheetPoint SHPoint{get {return mSpoint;}}

	public BackPack(int Count,GameObject goRoot,GameObject goPointRoot)
	{
		if(mSheetList.Count > 0){
			mSheetList.Clear();
		}
		mGoPackBack = goRoot;
		mGoPoint = goPointRoot;
		mPageCountMax = Count;
		//msCountPerPage = Count;
		CreateBackPack();
		
		
//		mSpoint = new SheetPoint(mPageCountMax);
//		mSpoint.SetToTargetGameObjectRoot(mGoPoint);
//		mSpoint.MoveStep();
	}
	
	void CreateBackPack()
	{
		for(int i = 0;i<mPageCountMax ; i++){
			//Root is Wrong
			PackSheet Psheet = new PackSheet(mGoPackBack,mCountPerPage,i);
			mSheetList.Add(Psheet);
		}
		ResetPackSheetPos();
	}
	
	void ResetPackSheetPos()
	{
		Vector3 Vpos = Vector3.zero;
		float SheetWidMargin = PackItemPosInfo.RowMargin*2*PackItemPosInfo.RowMax + PackItemPosInfo.BaseMargin;
		float SheetHeightMargin = PackItemPosInfo.ColumnMargin*2*PackItemPosInfo.ColumnMax;
		float BasePosX = 0.0f;
		float BasePosY = 0.0f;
		
		for(int i = 0;i < mSheetList.Count;i++)
		{	
			//Diff Anchor Lead TO Diff Base;
//			BasePosX = SheetWidMargin/2-PackItemPosInfo.RowMargin ;
//			BasePosY = SheetHeightMargin/2 + PackItemPosInfo.ColumnMargin;
			Vpos.x = BasePosX + SheetWidMargin*i;
			Vpos.y = BasePosY ;//+ SheetHeightMargin*i;
			Vpos.z = -1.0f;
			mSheetList[i].goSheetRoot.transform.localPosition = Vpos;
		}
	}
	
	public void AddSheet()
	{
		PackSheet PAddSheet = new PackSheet(mGoPackBack,mCountPerPage,mSheetList.Count);
		mSheetList.Add(PAddSheet);
		mPageCountMax = mSheetList.Count; //Haven Add 1 UpLine
		//msCountPerPage = mSheetList.Count;
		ResetPackSheetPos();
		
//		mSpoint.AddPoint();		
//		mSpoint.MoveStep();
	}

	
	public void PackBackInfoInit(List<PackageItem> Items,UIEventListener.VoidDelegate OnClick)
	{
		mOnPackClick = OnClick;
		for(int i = 0;i < mSheetList.Count;i++){
			mSheetList[i].PackSheetInfoInit(Items,i,OnClickItem);
		}
	}
	
	#region Sort
	public void Update(List<PackageItem> Items)
	{
		for(int i = 0;i < mSheetList.Count;i++){
			mSheetList[i].Update(Items,i);
		}
	}
	
	public void UpdateByType(List<PackageItem> Items,int MainTF,int MainTT,int SubTF,int SubTT)
	{
		for(int i = 0;i < mSheetList.Count;i++){
			mSheetList[i].UpdateByType(Items,i,MainTF,MainTT,SubTF,SubTT);
		}	
	}	
	#endregion Sort
	
	
	//For Equip
	public void UpdateByItemID()
	{
		for(int i = 0;i<mSheetList.Count;i++)
		{
			for(int j=0;j< BackPack.CountPerPage;j++){
				mSheetList[i].mItemList[j].UpdateByItemID();
			}
		}
	}

	public void OnClickItem(GameObject go)
	{
		GameObject itemFrameObj = go.transform.parent.Find("ChooseFrame").gameObject;
		if( mGoFocus == itemFrameObj)
			return;
		
		if(mOnPackClick != null)
			mOnPackClick(go);
		
		if( mGoFocus !=null)
		{
			mGoFocus.SetActive(false);
			mGoFocus.GetComponent<TweenAlpha>().enabled =false;
		}
		
		itemFrameObj.SetActive(true);
		itemFrameObj.GetComponent<TweenAlpha>().enabled = true;
		mGoFocus = itemFrameObj;
//		mGoFocus.transform.parent = go.transform.parent.transform;
//		mGoFocus.transform.localPosition = Vector3.zero + new Vector3(0.0f,0.0f,-3.0f);
//		mGoFocus.transform.localScale = new Vector3(3.0f,3.0f,0.0f);
	}
	
	
	public void PackBackInfoInit(Dictionary<string, Item> Items,UIEventListener.VoidDelegate OnClick)
	{
		mOnPackClick = OnClick;
		for(int i = 0;i < mSheetList.Count;i++){
			mSheetList[i].PackSheetInfoInit(Items,i,OnClickItem);
		}
	}

	public void Update(Dictionary<string, Item> Items)
	{
		for(int i = 0;i < mSheetList.Count;i++){
			mSheetList[i].Update(Items,i);
		}
	}
	
	public void Update()
	{
		
	}
	
	/// <summary>
	/// Updates the type of the by.
	/// </summary>
	/// <param name='Items'>
	/// Items.
	/// </param>
	/// <param name='MainTF'>MainTypeFrom
	/// Main T.
	/// </param>
	/// <param name='MainTT'>MainTypeTo
	/// Main T.
	/// </param>
	/// <param name='SubTF'>
	/// Sub T.
	/// </param>
	/// <param name='SubTT'>
	/// Sub T.
	/// </param>
	public void UpdateByType(Dictionary<string, Item> Items,int MainTF,int MainTT,int SubTF,int SubTT)
	{
		for(int i = 0;i < mSheetList.Count;i++){
			mSheetList[i].UpdateByType(Items,i,MainTF,MainTT,SubTF,SubTT);
		}	
	}	
	
	
	public static void GoFocusSet(GameObject go)
	{
		mGoFocus = go;
		mGoFocus.SetActive(false);
	}
	
	public static void PackageMaxIDSet()
	{
		foreach(Item item in PlayerDataManager.Instance.BackPack.Items.Values)
		{
			if(item.ID > mItemIDMax)
			{
				mItemIDMax = item.ID;
			}
		}
	}
	
	public static int PackageCheckNewCout()
	{
		mItemNewCount = 0;
		foreach(Item item in PlayerDataManager.Instance.BackPack.Items.Values)
		{
			if(item.ID > mItemIDMax)
			{
				mItemNewCount++;
			}
		}
		return mItemNewCount;
	}
	
	
	public static bool PackageCheckNew()
	{
		bool ret = false;
		foreach(Item item in PlayerDataManager.Instance.BackPack.Items.Values)
		{
			if(item.ID > mItemIDMax)
			{
				ret = true;
			}
		}
		return ret;
	}
	
	//There is one hidden page 
	public static bool PackageFullCheck()
	{
		if( (msCountPerPage*(PlayerDataManager.Instance.BackPack.Pages)) 
			<= PlayerDataManager.Instance.BackPack.Items.Count)
		{
			return true;
		}
		return false;
	}	
}

