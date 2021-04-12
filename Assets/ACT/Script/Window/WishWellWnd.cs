using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WishingWellWnd : Window<WishingWellWnd>
{
 	public override string PrefabName { get { return "WishingWellWnd"; } }	
	public string PrefabItemName { get { return "WishingItem" ;} }
	
	GameObject mGoBtnOnce;
	GameObject mGoBtnMore;
	
	GameObject mGoResbarRoot;
	ResourceBar mResourceBar;
	
	Dictionary<GameObject, WishingWell> mWishingWellTable = new Dictionary<GameObject, WishingWell>();
	WishingWell mCurWish = null;
	ShopBase mCurShopBase = null;
	List<ShopBase> mWishBaseList = null;

	protected override bool OnOpen()
    {
		//Attetion Order
		WishDefaultInit();
		WishListInit();
		ObjectInit();
		WinStyle = WindowStyle.WS_CullingMask;
        return base.OnOpen();;
    }

    protected override bool OnClose()
    {
		return base.OnClose();
    }
	
	void WishDefaultInit()
	{	
		foreach(WishingWell Ww in WishingWellManager.Instance.GetAllItem())
		{
			mCurWish = Ww;
			break;
		}
	}
	
	void WishListInit()
	{
		if( mWishBaseList == null )
			mWishBaseList = new List<ShopBase>();
		if( mWishBaseList.Count > 0)
			mWishBaseList.Clear();
		
		foreach( ShopBase SB in ShopBaseManager.Instance.GetAllItem() )
		{
			if( mCurWish.ShopType == SB.Type )
			{
				mWishBaseList.Add(SB);
			}
		}
		mWishBaseList.Sort((x, y) => ((int)x.MinLevel).CompareTo ((int)y.MinLevel));	
	}
	
	void WishListUpdate()
	{
		WishListInit();
	}
	
	void ObjectInit()
	{
		mGoResbarRoot = Control("ResBarRoot");
		mGoBtnOnce = Control ("BtnOnce");
		mGoBtnMore = Control ("BtnMore");
		UIEventListener.Get (mGoBtnOnce).onClick = OnClickOnceBtn ;
		UIEventListener.Get (mGoBtnMore).onClick = OnClickMoreBtn ;
		UIEventListener.Get(Control("Return")).onClick = OnClickBtnReturn;
		ResourceBarInit();
		BtnInit();
		InitGrid();
	}
	
	int PropsCheck( int itemid )
	{
		foreach(PropsBase PB in PropsBaseManager.Instance.GetAllItem())
		{
			if( PB.ID == itemid )
				return PB.DropCount;
		}
		return 0;
	}
	
	int ShopBaseColumnGet(int Count)
	{
		MainAttrib Attrib = PlayerDataManager.Instance.Attrib;
		for(int i = 0; i<mWishBaseList.Count; i++ )
		{
			if( Attrib.Level >= mWishBaseList[i].MinLevel 
				&& Attrib.Level <= mWishBaseList[i].MaxLevel )
			{
				if( PropsCheck( mWishBaseList[i].Item ) == Count)
					return i ;
			}
		}
		return 0;
	}
	
	
	void BtnInit()
	{
		int SBIndex = ShopBaseColumnGet( 1 );
		int OnceGem = mWishBaseList[SBIndex].Gem;
		mGoBtnOnce.transform.Find("Price").gameObject.GetComponent<UILabel>().text 
			= OnceGem.ToString();
		
		SBIndex = ShopBaseColumnGet( 10 );
		int TenGem = mWishBaseList[SBIndex].Gem;
		mGoBtnMore.transform.Find("Price").gameObject.GetComponent<UILabel>().text 
			= TenGem.ToString();
		
		
		MainAttrib Attrib = PlayerDataManager.Instance.Attrib;
		if( OnceGem <= Attrib.Gem )
		{
			BtnSetAble(mGoBtnOnce,true);
		}
		else
		{
			BtnSetAble(mGoBtnOnce,false);
		}
		
		if ( TenGem <= Attrib.Gem )
		{
			BtnSetAble(mGoBtnMore,true);
		}
		else
		{
			BtnSetAble(mGoBtnMore,false);
		}
	}
	
	void BtnUpdate()
	{
		BtnInit();
	}
	
	int LevelLimitGet( )
	{
		MainAttrib Attrib = PlayerDataManager.Instance.Attrib;
		int AttribLevel = Attrib.Level;
		for(int i = 0; i< mWishBaseList.Count; i++)
		{
			if( AttribLevel > mWishBaseList[i].MinLevel )
			{
				return i;
			}
		}
		return 0;
	}
	
	
	void InitGrid()
	{
		string GridContent = "Wishing";
		int nCount = 0;
		GameObject ItemGrid = Control("ItemGrid");
		mWishingWellTable.Clear();
		foreach( WishingWell Wish in WishingWellManager.Instance.GetAllItem() )
		{
			GameObject goItem = GameObject.Instantiate(Resources.Load(PrefabItemName))as GameObject;
			goItem.name = GridContent + Wish.ShopType.ToString();
			goItem.transform.parent = ItemGrid.transform;
			goItem.transform.localPosition = Vector3.zero;
			goItem.transform.localScale = Vector3.one;

//			Control ("Icon",goItem).GetComponent<UILabel>().text = Wish.Icon;
			Control ("Title",goItem).GetComponent<UILabel>().text = Wish.WellName; 
			if( 0 == string.Compare( Wish.WellName,"Destiny Well") )
			{
				Control("Desc",goItem).GetComponent<UILabel>().text 
					= string.Format( Wish.Desc , LevelLimitGet()+1 );
			}
			else
				Control("Desc",goItem).GetComponent<UILabel>().text = Wish.Desc;
			
			UIEventListener.Get(goItem).onClick = OnClickBtnWish;
			mWishingWellTable[goItem] = Wish;
		}
	}
	
	void ResourceBarInit()
	{
	 	MainAttrib attrib = PlayerDataManager.Instance.Attrib;
		mResourceBar = new ResourceBar(mGoResbarRoot);
		//mResourceBar.ClickedAddMoney = OnClickedAddMoney;
		//mResourceBar.ClickedAddPhysic = OnClickedAddPhysic;
		mResourceBar.Update(attrib.Gold,attrib.Gem,attrib.SP,-1);
		mResourceBar.UpdateByResType(ResourceBar.ResType.RT_NonPhysic);
	}
	
	void OnClickOnceBtn( GameObject go )
	{
		int ShopID = 0;
		int SBIndex = ShopBaseColumnGet(1);
		ShopID = mWishBaseList[SBIndex].ID;
		RequestBuyAndUseItem(ShopID);
	}
	
	void RequestBuyAndUseItem(int ShopID)
	{
		Request(new BuyCmd(ShopID,true), delegate(string err, Response response)
        {
            if (!string.IsNullOrEmpty(err))
            {
				MessageBoxWnd.Instance.Show(err,MessageBoxWnd.StyleExt.Alpha);
                return;
            }
			
			UseItemCmd.UseItemData UiData = (response != null) ? response.Parse<UseItemCmd.UseItemData>() : null;
			if(UiData != null)
			{
				UseItem UseItemData = new UseItem( UiData, 0, 0 );	
				//mUseItemData.OnClickUSE = UseItemOnClick;
				//mUseItemData.OnClickUSECancle = UseItemOnClickCancle; 
			}
			RequestAttribInfo();
        });			
	}	
	
	
	void RequestAttribInfo()
	{
        Request(new GetUserAttribCmd(), delegate(string err, Response response)
        {
            if (!string.IsNullOrEmpty(err))
            {
				MessageBoxWnd.Instance.Show( err ,MessageBoxWnd.StyleExt.Alpha);
                return;
            }
            MainAttrib attrib = (response != null) ? response.Parse<MainAttrib>() : null;
            if (attrib != null)
			{
				PlayerDataManager.Instance.OnMainAttrib(attrib);
			}
			mResourceBar.Update(attrib.Gold,attrib.Gem,attrib.SP,-1);	
        });
	}
	
	
	void OnClickMoreBtn( GameObject go )
	{
		int ShopID = 0;
		int SBIndex = ShopBaseColumnGet(10);
		ShopID = mWishBaseList[SBIndex].ID;
		RequestBuyAndUseItem(ShopID);
	}
	
	void OnClickBtnWish( GameObject go )
	{
		if (mWishingWellTable.ContainsKey(go))
		{
			mCurWish = mWishingWellTable[go];
			WishListUpdate();
			BtnUpdate();
		}
	}
	

	void OnClickBtnReturn( GameObject go )
	{
		Close();
		if(InGameMainWnd.Exist)
			InGameMainWnd.Instance.Show();
	}	
	
}
