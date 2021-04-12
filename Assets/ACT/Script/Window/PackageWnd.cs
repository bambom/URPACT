using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PackageWnd : Window<PackageWnd>
{
	public override string PrefabName { get { return "PackageWnd"; } }

	int mCountPackPages = 2;
	int mFocusStoneId = -1;
	
	public BackPack mBackPack;
	DescDetail mDescDetail;
	ResourceBar mResourceBar;
	ItemToolBar mItemToolBar;
	UIDraggablePanel mScriptPackDrag;
	
	GameObject mGoPackGridNode;
	GameObject mGoPointRoot;
	GameObject mGoDescRoot;
	GameObject mGoResbarRoot;
	GameObject mGoToolBarRoot;
	GameObject mGoFocusItem;
	GameObject mGoPackCountLabel;
	
	GameObject mGoBtnUpdate; //mGoBtnStrengthen
	GameObject mGoBtnEquip;
	GameObject mGoBtnSell;
	GameObject mGoBtnCombine;
	GameObject mGoBtnUse;
	GameObject mGoBtnMed1;
	GameObject mGoBtnMed2;
	
	GameObject mPageGrid;
	
	bool mIsComeFromMain = true;
	public static Strengthen  StrengthenWnd;
	public static Combine	  CombineWnd;
	
	Dictionary<byte, ItemBase> mTryItems = new Dictionary<byte, ItemBase>();
	UIGuideShow mUIGuideShow = null;
	UseItem mUseItemData = null;
	#region Protected
	protected override bool OnOpen()
    {
		Init();
		WinStyle = WindowStyle.WS_CullingMask;
        return base.OnOpen();
    }
	
    protected override bool OnClose()
    {
		if( mUIGuideShow != null ){
			mUIGuideShow.Destroy();
			mUIGuideShow = null;
		}
		EquipChangeWnd.Instance.Close();
        return base.OnClose();
    }
	
	public override void Hide()
	{
		if( mUIGuideShow != null ){
			mUIGuideShow.Destroy();
			mUIGuideShow = null;
		}
		if(EquipChangeWnd.Exist)
			EquipChangeWnd.Instance.Hide();
		base.Hide();
	}
	
	public override void Show()
	{
		if(mUIGuideShow == null)
			mUIGuideShow = new UIGuideShow(PrefabName);
		if(mUIGuideShow.GuideCheck()){
			mUIGuideShow.GuideStart();
		}
		
		if(EquipChangeWnd.Exist)
			EquipChangeWnd.Instance.Show();
		base.Show();
	}
	
	#endregion
	
	void ObjectInit()
	{
		mGoPackGridNode = Control("PackPageRoot");
		mGoPointRoot = Control("PointRoot");
		mGoDescRoot = Control("AttribRoot");
		mGoToolBarRoot = Control("ToolBarRoot");
		mGoResbarRoot = Control("ResBarRoot");
			
		mGoBtnUpdate = Control("BtnUpdate");
		mGoBtnCombine = Control("BtnCombine");
		mGoBtnEquip = Control("BtnEquip");
		mGoBtnSell = Control("BtnSell");
		mGoBtnUse = Control("BtnUse");
		
		mPageGrid = Control("PackPageGrid");
		//mGoBtnMed1 = Control("MedEquip1");
		//mGoBtnMed2 = Control("MedEquip2");
		mGoPackCountLabel = Control("CountLabel");
		UIEventListener.Get(Control("BtnCharctorAttrib")).onClick = OnClickBtnCharctorAttrib;
		UIEventListener.Get(Control("BtnReturn")).onClick 	= OnClickBtnReturn;
		UIEventListener.Get(Control("BtnAddSlot")).onClick 	= OnClickBtnAddSlot;
		UIEventListener.Get(mGoBtnUpdate).onClick 	= OnClickBtnStrengthen;
		UIEventListener.Get(mGoBtnCombine).onClick = OnClickBtnCombine;
		UIEventListener.Get(mGoBtnEquip).onClick	= OnClickBtnEquip;
		UIEventListener.Get(mGoBtnSell).onClick	= OnClickBtnSell;
		UIEventListener.Get(mGoBtnUse).onClick = OnClickBtnUse;
		//UIEventListener.Get(mGoBtnMed1).onClick	= OnClickBtnMedEquip;
		//UIEventListener.Get(mGoBtnMed2).onClick	= OnClickBtnMedEquip;
		//UIEventListener.Get(Control ("BtnShop")).onClick = OnClickBtnShop;
	}
	
	void OnDragFinish()
	{
		if(mScriptPackDrag.CurIndex < 0)
			mScriptPackDrag.CurIndex = 0;
		if(mScriptPackDrag.CurIndex > mCountPackPages)
			mScriptPackDrag.CurIndex = mCountPackPages;
		
		Debug.Log("mScriptPackDrag.CurIndex = " + mScriptPackDrag.CurIndex );
		//mBackPack.SHPoint.SetFocusPoint(mScriptPackDrag.CurIndex);
	}
		
	void UpdatePackageCountLabel()
	{
		int ItemCount = PlayerDataManager.Instance.BackPack.Items.Count;
		int Capcity = PlayerDataManager.Instance.BackPack.Pages*BackPack.CountPerPage;
		string itemcount = ItemCount.ToString();
		string capcity = Capcity.ToString();

		if(BackPack.PackageFullCheck())
			mCountPackPages = Mathf.CeilToInt( (float)ItemCount/16 );
		else 
			mCountPackPages = PlayerDataManager.Instance.BackPack.Pages;
		Debug.Log("mCountPackPages" + mCountPackPages);
		
		if(ItemCount > Capcity)
		{
			mGoPackCountLabel.GetComponent<UILabel>().text = "[FF0000]"+ itemcount+"[-]/" + capcity;
//			int buyCount = Mathf.CeilToInt( (ItemCount - Capcity)/BackPack.CountPerPage );
//			PackageBuyPage PBP = PackageBuyPageManager.Instance.GetItem(PlayerDataManager.Instance.BackPack.Pages-1); //-2+1
//			MessageBoxWnd.Instance.Show("BuySlotOrNot",MessageBoxWnd.Style.OK_CANCLE,PBP.Gem);
//			MessageBoxWnd.Instance.OnClickedOk = OnClickBuySlot;
//			MessageBoxWnd.Instance.OnClickedCancle = OnClickBuySlotCancle;
		}else
			mGoPackCountLabel.GetComponent<UILabel>().text = itemcount + "/" + capcity;
	}
	
	void Init()
	{	
		CharactorModelInit();
		ObjectInit();		
		UpdatePackageCountLabel();
		
		mBackPack = new BackPack(mCountPackPages,mGoPackGridNode,mGoPointRoot);
		PackInfoInit();
		
		mDescDetail = new DescDetail(DescType.AttribAll,PlayerDataManager.Instance.Attrib,mGoDescRoot,false);
		mDescDetail.Init(null);
		SwapDescIcon(false);
	
		ResourceBarInit();
		ItemToolBarInit();
		
		//InitMedEquipIcon();
		BtnUpdate(null);
		
		BackPack.PackageMaxIDSet();
		
		mUIGuideShow = new UIGuideShow(PrefabName);
		if(mUIGuideShow.GuideCheck()){
			mUIGuideShow.GuideStart();
		}
	}
	
	void OnClickBuySlot(GameObject go)
	{
		RequsetPackPageBuyInfo();
	}

	#region ResourceBar
	void OnClickedAddMoney(GameObject go)
	{
		Close ();
	}
	
	void OnClickedAddPhysic(GameObject go)
	{
		
		
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
	
	
	
	#region ItemToolBarBtn
	void PackPageGridReset()
	{
		mPageGrid.transform.localPosition = Vector3.zero;
		Vector4 V4 = mPageGrid.GetComponent<UIPanel>().clipRange;
		V4.x = 0.0f;
		mPageGrid.GetComponent<UIPanel>().clipRange = V4;
	}
	
	void ItemToolBarInit()
	{
		mItemToolBar = new ItemToolBar();
		mItemToolBar.GoItemToolBar.transform.parent = mGoToolBarRoot.transform;
		mItemToolBar.GoItemToolBar.transform.localPosition = Vector3.zero;
		mItemToolBar.GoItemToolBar.transform.localScale = Vector3.one;
		
		mItemToolBar.ClickedAcesories = OnClickedAcesories;
		mItemToolBar.ClickedArmor = OnClickedArmor;
		mItemToolBar.ClickedHelmet = OnClickedHelmet;
		mItemToolBar.ClickedBracers = OnClickedBracers;
		mItemToolBar.ClickedShoulder = OnClickedShoulder;
		mItemToolBar.ClickedBoots = OnClickedBoots;
		mItemToolBar.ClickedTools = OnClickedTools;
		mItemToolBar.ClickedWeapon = OnClickedWeapon;
	}
	
	void OnClickedAcesories(GameObject go)
	{
		int MainTF = (int)EItemType.Equip;
		int MainTT = (int)EItemType.Equip;
		int SubTF =  (int)EquipSubType.EST_NeckLace;
		int SubTT =  (int)EquipSubType.EST_Ring;
		
		if(mItemToolBar.Focus == ItemTool.Invalid){
			MainTF = (int)EItemType.Equip;
			MainTT = (int)EItemType.Props;
			SubTF =  (int)EquipSubType.EST_Weapon;
			SubTT =  (int)EquipSubType.EST_Ring;
		}else{

		}
		PackPageGridReset();
		mBackPack.UpdateByType(PlayerDataManager.Instance.BackPack.PackSortList,
				MainTF,MainTT,SubTF,SubTT);
	}
	
	void OnClickedArmor(GameObject go)
	{
		int MainTF = (int)EItemType.Equip;
		int MainTT = (int)EItemType.Equip;
		int SubTF =  (int)EquipSubType.EST_Armor;
		int SubTT =  (int)EquipSubType.EST_Armor;

		if(mItemToolBar.Focus == ItemTool.Invalid){
			MainTF = (int)EItemType.Equip;
			MainTT = (int)EItemType.Props;
			SubTF =  (int)EquipSubType.EST_Weapon;
			SubTT =  (int)EquipSubType.EST_Ring;
		}else{
			
		}
		PackPageGridReset();
		mBackPack.UpdateByType(PlayerDataManager.Instance.BackPack.PackSortList,
				MainTF,MainTT,SubTF,SubTT);
	}
	
	void OnClickedHelmet(GameObject go)
	{
		int MainTF = (int)EItemType.Equip;
		int MainTT = (int)EItemType.Equip;
		int SubTF =  (int)EquipSubType.EST_Helmet;
		int SubTT =  (int)EquipSubType.EST_Helmet;
				
		if(mItemToolBar.Focus == ItemTool.Invalid){
			MainTF = (int)EItemType.Equip;
			MainTT = (int)EItemType.Props;
			SubTF =  (int)EquipSubType.EST_Weapon;
			SubTT =  (int)EquipSubType.EST_Ring;
		}else{
			
		}
		PackPageGridReset();
		mBackPack.UpdateByType(PlayerDataManager.Instance.BackPack.PackSortList,
				MainTF,MainTT,SubTF,SubTT);
	}
	
	void OnClickedBracers(GameObject go)
	{
		int MainTF = (int)EItemType.Equip;
		int MainTT = (int)EItemType.Equip;
		int SubTF = (int)EquipSubType.EST_Wrist;
		int SubTT = (int)EquipSubType.EST_Wrist;
				
		if(mItemToolBar.Focus == ItemTool.Invalid){
			MainTF = (int)EItemType.Equip;
			MainTT = (int)EItemType.Props;
			SubTF =  (int)EquipSubType.EST_Weapon;
			SubTT =  (int)EquipSubType.EST_Ring;
		}else{
			
		}
		PackPageGridReset();
		mBackPack.UpdateByType(PlayerDataManager.Instance.BackPack.PackSortList,
				MainTF,MainTT,SubTF,SubTT);
	}
	
	void OnClickedShoulder(GameObject go)
	{
		int MainTF = (int)EItemType.Equip;
		int MainTT = (int)EItemType.Equip;
		int SubTF =  (int)EquipSubType.EST_Shoulder;
		int SubTT =  (int)EquipSubType.EST_Shoulder;
				
		if(mItemToolBar.Focus == ItemTool.Invalid){
			MainTF = (int)EItemType.Equip;
			MainTT = (int)EItemType.Props;
			SubTF =  (int)EquipSubType.EST_Weapon;
			SubTT =  (int)EquipSubType.EST_Ring;
		}else{
			
		}
		PackPageGridReset();
		mBackPack.UpdateByType(PlayerDataManager.Instance.BackPack.PackSortList,
				MainTF,MainTT,SubTF,SubTT);
	}
	
	void OnClickedBoots( GameObject go)
	{
		int MainTF = (int)EItemType.Equip;
		int MainTT = (int)EItemType.Equip;
		int SubTF =  (int)EquipSubType.EST_Shoe;
		int SubTT =  (int)EquipSubType.EST_Shoe;
				
		if(mItemToolBar.Focus == ItemTool.Invalid){
			MainTF = (int)EItemType.Equip;
			MainTT = (int)EItemType.Props;
			SubTF =  (int)EquipSubType.EST_Weapon;
			SubTT =  (int)EquipSubType.EST_Ring;
		}else{
			
		}
		PackPageGridReset();
		mBackPack.UpdateByType(PlayerDataManager.Instance.BackPack.PackSortList,
				MainTF,MainTT,SubTF,SubTT);
	}
	
	
	void OnClickedTools(GameObject go)
	{
		int MainTF = (int)EItemType.Rune;
		int MainTT = (int)EItemType.Props;
		int SubTF =  1;
		int SubTT =  9;
				
		if(mItemToolBar.Focus == ItemTool.Invalid){
			MainTF = (int)EItemType.Equip;
			MainTT = (int)EItemType.Props;
			SubTF =  (int)EquipSubType.EST_Weapon;
			SubTT =  (int)EquipSubType.EST_Ring;
		}else{
			
		}
		PackPageGridReset();
		mBackPack.UpdateByType(PlayerDataManager.Instance.BackPack.PackSortList,
				MainTF,MainTT,SubTF,SubTT);
	}
	
	void OnClickedWeapon(GameObject go)
	{
		int MainTF = (int)EItemType.Equip;
		int MainTT = (int)EItemType.Equip;
		int SubTF =  (int)EquipSubType.EST_Weapon;
		int SubTT =  (int)EquipSubType.EST_Weapon;
				
		if(mItemToolBar.Focus == ItemTool.Invalid){
			MainTF = (int)EItemType.Equip;
			MainTT = (int)EItemType.Props;
			SubTF =  (int)EquipSubType.EST_Weapon;
			SubTT =  (int)EquipSubType.EST_Ring;
		}else{
			
		}
		PackPageGridReset();
		mBackPack.UpdateByType(PlayerDataManager.Instance.BackPack.PackSortList,
				MainTF,MainTT,SubTF,SubTT);
	}
	#endregion ItemToolBarBtn
	
	
	#region PackItem
	public void OnClickPackItem(GameObject go)
	{
		mGoFocusItem = go.transform.parent.gameObject;
		Item item = ItemGetByItemID (mGoFocusItem);
		BtnUpdate(item);
		
		if(item == null){
			return;
		}
		
		if(mDescDetail.DscType == DescType.AttribAll){
			mDescDetail.Destroy();
			mDescDetail = new DescDetail(DescType.AttribOne,PlayerDataManager.Instance.Attrib,mGoDescRoot,true);
		}
		mDescDetail.Init( item );
		SwapDescIcon(true);
		
		if (item.ItemBase.MainType == (int)EItemType.Equip)
        {
            mTryItems[item.ItemBase.SubType] = item.ItemBase;
            EquipChangeWnd.Instance.Charactor.EquipWithReplace(mTryItems);
        }
	}
	
	void PackInfoInit()
	{
		//BackPack.GoFocusSet(Control("ItemFocus"));
		mBackPack.PackBackInfoInit(PlayerDataManager.Instance.BackPack.PackSortList,OnClickPackItem);
		mScriptPackDrag = mGoPackGridNode.transform.parent.GetComponent<UIDraggablePanel>();
		mScriptPackDrag.GodMove = true;
		mScriptPackDrag.OnGodDragFinished = OnDragFinish;
	}
	#endregion PackItem
	
	#region PackageWnd
	public void UpdateByItemID()
	{
		mBackPack.UpdateByItemID();
	}
	
	public void Update()
	{
		mBackPack.Update(PlayerDataManager.Instance.BackPack.PackSortList);
		Item item = ItemGetByItemID (mGoFocusItem);
		BtnUpdate(item);
		
		if(item == null){
			if(mDescDetail.DscType == DescType.AttribOne){
				mDescDetail.Destroy();
				mDescDetail = new DescDetail(DescType.AttribAll,PlayerDataManager.Instance.Attrib,mGoDescRoot,true);
			}
		}
		mDescDetail.Init( item );
	}

    int GetForgeReturn(EquipItem equipItem)
    {
        int strengthenCount = equipItem.GetAttrib(EquipAttrib.StrengthenCount);
        if (strengthenCount == 0)
            return 0;

        TargetAttrib targetAttrib = TargetAttribManager.Instance.GetItem(equipItem.ItemBase.MainType, equipItem.ItemBase.SubType);
        if (targetAttrib == null)
            return 0;

        EquipAttrib attrib = (EquipAttrib)System.Enum.Parse(typeof(EquipAttrib), targetAttrib.Attrib);
        int finalValue = equipItem.GetAttrib(attrib);
        int strengthenValue = equipItem.Item.GetAttrib(attrib.ToString());
        if (strengthenValue == 0)
            return 0;

        // 装备的基础值。
        int baseValue = finalValue - strengthenValue;

        // 百分比的基础为10000.
        float averageValue = 10000.0f * strengthenValue / baseValue / strengthenCount;
        for (int i = 1; ; i++)
        {
            ForgeReturn forgeReturn = ForgeReturnManager.Instance.GetItem(equipItem.ItemBase.SubType, i);
            if (forgeReturn == null)
                return 0;

            if (averageValue < forgeReturn.Max)
                return strengthenCount * forgeReturn.Return;
        }
    }
	
	bool CheckEuipHasBaseAttrib(ItemBase itembase)
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
	
	void BtnUpdate(Item item)
	{
        BtnReset();
        if (item == null) return;

        int price = item.ItemBase.SellPrice * item.Num;
        if (item.ItemBase.MainType == (int)EItemType.Equip)
        {
            // 这里要根据强化次数来计算一下强化的返还银币值。
            price += GetForgeReturn(item.ToEquipItem());
			
			bool bBaseAttrib = CheckEuipHasBaseAttrib(item.ItemBase);
			mGoBtnUpdate.SetActive(true);
			BtnSetAble(mGoBtnUpdate,bBaseAttrib);		
           
			mGoBtnEquip.SetActive(true);
			BtnSetAble(mGoBtnEquip,!item.ToEquipItem().Equiped);
			BtnSetAble(mGoBtnSell,!item.ToEquipItem().Equiped);	
        }
        else if ( //item.ItemBase.MainType == (int)EItemType.Rune||
                 item.ItemBase.MainType == (int)EItemType.Stone)
        {
			if( //item.ItemBase.Name.Contains("5") || 
				item.ItemBase.ID == (int)StoneBaseID.StoneLevel5  
				|| item.ItemBase.ID == (int)StoneBaseID.StoneLevel4To5  )
				mGoBtnCombine.SetActive(false);
			else
	            mGoBtnCombine.SetActive(true);
        }
        else if (item.ItemBase.MainType == (int)EItemType.Props)
        {
            if (item.ItemBase.SubType == 1)
            {
                mGoBtnUse.SetActive(true);
            }
            else if (item.ItemBase.SubType == 2)
            {
				BtnSetAble(mGoBtnSell,false);	
                //mGoBtnMed1.SetActive(true);
                //mGoBtnMed2.SetActive(true);
                //ResetMedEquipIcon();
                //UpdateMedEquipIcon();
            }
        }

        mGoBtnSell.transform.Find("Price").GetComponent<UILabel>().text = price.ToString();
	}
	
	void BtnSetAble(GameObject go,bool bStatus)
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
	
	void BtnReset()
	{
		//mGoBtnMed1.SetActive(false);
		//mGoBtnMed2.SetActive(false);	
		
		mGoBtnUpdate.SetActive(false);
		BtnSetAble(mGoBtnUpdate,true);
		
		mGoBtnCombine.SetActive(false);
		
		mGoBtnEquip.SetActive(false);
		BtnSetAble(mGoBtnEquip,true);
		
		mGoBtnUse.SetActive(false);
		
		mGoBtnSell.SetActive(true);
		BtnSetAble(mGoBtnSell,true);
		mGoBtnSell.transform.Find("Price").GetComponent<UILabel>().text = "";
	}
	
	void SwapDescIcon(bool flag)
	{
		Control("BtnCharctorAttrib").transform.Find("Forground").gameObject.SetActive(flag);
	}
	
	void OnClickBtnCharctorAttrib(GameObject go)
	{
		SwapDescPanel(go);
	}
	
	void OnBuyPageSuccess()
	{
		int ItemCount = PlayerDataManager.Instance.BackPack.Items.Count;
		int Capcity = PlayerDataManager.Instance.BackPack.Pages*BackPack.CountPerPage;
		string itemcount = ItemCount.ToString();
		string capcity = Capcity.ToString();
		
		if( mCountPackPages >= PlayerDataManager.Instance.BackPack.Pages )
		{
			if(BackPack.PackageFullCheck())
				mCountPackPages = Mathf.CeilToInt( (float)ItemCount/16 );
			else 
				mCountPackPages = PlayerDataManager.Instance.BackPack.Pages;
			mGoPackCountLabel.GetComponent<UILabel>().text = itemcount + "/" + capcity;
		}else{
			mBackPack.AddSheet();
		}
		
		if(ItemCount > Capcity){
			mGoPackCountLabel.GetComponent<UILabel>().text = "[FF0000]"+ itemcount+"[-]/" + capcity;
//			int buyCount = Mathf.CeilToInt( (ItemCount - Capcity)/BackPack.CountPerPage );
//			PackageBuyPage PBP = PackageBuyPageManager.Instance.GetItem(PlayerDataManager.Instance.BackPack.Pages-1); //-2+1
//			MessageBoxWnd.Instance.Show("BuySlotOrNot",MessageBoxWnd.Style.OK_CANCLE,PBP.Gem);
//			MessageBoxWnd.Instance.OnClickedOk = OnClickBuySlot;
//			MessageBoxWnd.Instance.OnClickedCancle = OnClickBuySlotCancle;
		}else
			mGoPackCountLabel.GetComponent<UILabel>().text = itemcount + "/" + capcity;
		
		Update();
		RequestAttribInfo();
	}
	
	void OnClickBuySlotCancle(GameObject go)
	{
		
		
	}
	
	void OnClickBtnAddSlot(GameObject go)
	{		
		PackageBuyPage PBP = PackageBuyPageManager.Instance.GetItem(PlayerDataManager.Instance.BackPack.Pages + 1);
		MessageBoxWnd.Instance.Show("BuySlotOrNot",MessageBoxWnd.Style.OK_CANCLE,PBP.Gem);
		MessageBoxWnd.Instance.OnClickedOk = OnClickBuySlot;
		MessageBoxWnd.Instance.OnClickedCancle = OnClickBuySlotCancle;
	}
	
	void OnClickBtnShop(GameObject go)
	{
		Close ();
		if(!NpcShopWnd.Exist)
			NpcShopWnd.Instance.Open();
		else
			NpcShopWnd.Instance.Show();
	}
	
	public void OnClickBtnReturn(GameObject go)
	{
		if(mIsComeFromMain)
		{
			Close ();
			if( !InGameMainWnd.Exist )
				InGameMainWnd.Instance.Open();
			else
				InGameMainWnd.Instance.Show();
		}
		else
		{
			Close();
			if( !LevelSelectWnd.Exist )
				LevelSelectWnd.Instance.Open();
			else
				LevelSelectWnd.Instance.Show();
		}
	}
	
	void OnEquSuccess()
	{
		RequestPackageInfo(true);
		RequestAttribInfo();
	}
	
	public void OnClickBtnEquip(GameObject go)
	{
		Item item = ItemGetByItemID(mGoFocusItem);
		RequestEquipInfo(item.ID);
	}
	
	void OnUseSuccess()
	{
		RequestPackageInfo(false);
	}
	
	void OnClickBtnUse(GameObject go)
	{
		Item item = ItemGetByItemID(mGoFocusItem);
		if(item.ItemBase.ID == (int)PropsBaseID.Strength)
		{
			if ( PlayerDataManager.Instance.StrengthInfo.value >= 12 )
			{
				MessageBoxWnd.Instance.Show("VitalityIsFull",MessageBoxWnd.StyleExt.Alpha);
				return;
			}
		}
		
		
		RequestItemUseInfo(item.ID);
	}
	
	#region ItemSell
	void OnSellSuccess()
	{
		RequestAttribInfo();
		RequestPackageInfo(false);
	}
	
	void OnClickSellOk(GameObject go)
	{
		Item item = ItemGetByItemID(mGoFocusItem);
		RequestSellInfo(item.ID,item.Num);
	}
	
	void OnClickSellCancle(GameObject go)
	{

	}
	
	void OnClickBtnSell(GameObject go)
	{
		Item item = ItemGetByItemID(mGoFocusItem);
		if(item == null)
			return;
		
		MessageBoxWnd.Instance.Show("SureToSell",MessageBoxWnd.Style.OK_CANCLE);
		MessageBoxWnd.Instance.OnClickedOk = OnClickSellOk;
		MessageBoxWnd.Instance.OnClickedCancle = OnClickSellCancle; 
	}
	#endregion ItemSell
	
	public void OnClickBtnStrengthen(GameObject go)
	{
		Hide();
		Item item = ItemGetByItemID(mGoFocusItem);
		EquipStrengthenShow(item);
	}
	
	public void OnClickBtnCombine(GameObject go)
	{
		Hide();
		Item item = ItemGetByItemID(mGoFocusItem);
		CombinePanelShow(item);
	}
	
	void OnMedEquipSuccess()
	{
		RequestPackageInfo(false);
	}
	
	void OnClickBtnMedEquip(GameObject go)
	{
		Item item = ItemGetByItemID(mGoFocusItem);
		if(string.Compare(go.name,"MedEquip1")== 0){
			RequstMedEquipInfo(item.ID,1);
		}else if(string.Compare(go.name,"MedEquip2") == 0){
			RequstMedEquipInfo(item.ID,2);
		}
	}	
	#endregion PackageWnd
	
	
	#region MedEquipIcon
	void ResetMedEquipIcon()
	{
		mGoBtnMed1.transform.Find("MedIcon1").GetComponent<UISprite>().spriteName 
			= "Button09_MedicineItem_Bottom_03";
		mGoBtnMed1.transform.Find("Label").GetComponent<UILabel>().text = null;
		mGoBtnMed2.transform.Find("MedIcon2").GetComponent<UISprite>().spriteName 
			= "Button09_MedicineItem_Bottom_03";
		mGoBtnMed2.transform.Find("Label").GetComponent<UILabel>().text = null; 
	}
	
	void UpdateMedEquipIcon()
	{
		//InitMedEquipIcon();
	}
	
	void InitMedEquipIcon()
	{
		foreach(Item item in PlayerDataManager.Instance.BackPack.Items.Values){
			if(null != item.ToPropsItem()){
				if( item.ToPropsItem().Slot == 1){
					mGoBtnMed1.transform.Find("MedIcon1").GetComponent<UISprite>().spriteName = item.ItemBase.Icon;
					mGoBtnMed1.transform.Find("Label").GetComponent<UILabel>().text = item.Num.ToString();
				}else if( item.ToPropsItem().Slot == 2){
					mGoBtnMed2.transform.Find("MedIcon2").GetComponent<UISprite>().spriteName = item.ItemBase.Icon;
					mGoBtnMed2.transform.Find("Label").GetComponent<UILabel>().text = item.Num.ToString(); 
				}
			}	
		}
	}
	#endregion MedEquipIcon
	
	
	void SwapDescPanel(GameObject go)
	{	
		if(mDescDetail.DscType == DescType.AttribOne){
			mDescDetail.Destroy();
			mDescDetail = new DescDetail(DescType.AttribAll,PlayerDataManager.Instance.Attrib,mGoDescRoot,true);	
			mDescDetail.Init(null);
			SwapDescIcon(false);
		}else if(mDescDetail.DscType == DescType.AttribAll){		
			Item item = ItemGetByItemID (mGoFocusItem);
			if(item == null)
				return;
			
			mDescDetail.Destroy();
			mDescDetail = new DescDetail(DescType.AttribOne,PlayerDataManager.Instance.Attrib,mGoDescRoot,true);
			mDescDetail.Init( item );
			SwapDescIcon(true);
		}
	}
	
	
	Item ItemGetByItemID(GameObject go)
	{
		if(go == null)
			return null;
		
		GameObject goItemId = go.transform.Find("ItemID").gameObject;
		int ItemID = int.Parse(goItemId.GetComponent<UILabel>().text);
		Item item = PlayerDataManager.Instance.BackPack.Find(ItemID);
		return item;
	}
	
	void CharactorModelInit()
	{
		if( !EquipChangeWnd.Exist )
			EquipChangeWnd.Instance.Open();
		//Control ("CharactorCollider").AddComponent<PressDrag>().target 
		// 				   = Control ("CharactorCollider").transform;
		PressDrag PDScript = Control ("CharactorCollider").GetComponent<PressDrag>();
		
		if(null != PDScript){
			PDScript.target = EquipChangeWnd.Instance.CharactorRoot.transform;
		}else{
			PDScript = Control ("CharactorCollider").AddComponent<PressDrag>();
			PDScript.target = EquipChangeWnd.Instance.CharactorRoot.transform;
		}
		
		//EquipChangeWnd.Instance.ResetPos(new Vector3(-280.0f,-180.0f,500.0f));
		//UIEventListener.Get( Control ("CharactorCollider") ).onClick = OnClickCharactor;
	}
	
	//*********************************************************************************************************//	
	void CombinePanelShow(Item item)
	{
		if(item.ItemBase.ID  == (int) StoneBaseID.StoneLevel4To5)
		{
			MessageBoxWnd.Instance.Show("CantCombine",MessageBoxWnd.StyleExt.Alpha);
			return;
		}
		CombineWnd = new Combine(item.ID);
		CombineWnd.CombineSuccess = CombineSuccess;
	}
	
	void CombineSuccess()
	{
		//RequestAttribInfo();
        //RequestPackageInfo(false);
		//mResourceBar.Update(attrib.Gold,attrib.Gem,-1);
	}

	//*********************************************************************************************************//	
	/// <summary>
	/// 显示装备强化面板.
	/// </summary>
	
	void SthenSuccess()
	{
		SoundManager.Instance.PlaySound("UI_Task_Complete");
		//RequestAttribInfo();
        //RequestPackageInfo(false);
		//mResourceBar.Update(attrib.Gold,attrib.Gem,-1);
	}
	
	void EquipStrengthenShow(Item item)
	{
		StrengthenWnd = new Strengthen(item.ID);
		StrengthenWnd.StrengthenSuccess = SthenSuccess;
	}
	
	void OnUseStrenthInfo()
	{
		RequestPhysicInfo();
		RequestPackageInfo(false);
	}
	
	
	void UseItemOnClickCancle( GameObject go )
	{
		if( mUseItemData != null )
		{
			mUseItemData = null;
		}
	}
	
	void UseItemOnClick( GameObject go )
	{
		RequestPackageInfo(false);
	}
	
	//*********************************************************************************************************//
	void RequestItemUseInfo(int nItemID)
	{
		Item item = PlayerDataManager.Instance.BackPack.Find(nItemID);
		
		Request(new UseItemCmd(nItemID), delegate(string err, Response response)
        {
            if (!string.IsNullOrEmpty(err))
            {
				MessageBoxWnd.Instance.Show( err ,MessageBoxWnd.StyleExt.Alpha );
                return;
            }
			
			if(item.ItemBase.ID == (int)PropsBaseID.Strength)
			{
				OnUseStrenthInfo();
				SoundManager.Instance.PlaySound("UI_YaoJi");
				MessageBoxWnd.Instance.Show("VitalityRestored",MessageBoxWnd.StyleExt.Alpha);
			}
			else
			{
 	           OnUseSuccess();	
			   UseItemCmd.UseItemData UiData = (response != null) ? response.Parse<UseItemCmd.UseItemData>() : null;
			   if( mUseItemData == null )
			   {
					mUseItemData = new UseItem( UiData, item.Num-1,nItemID );	
					mUseItemData.OnClickUSE = UseItemOnClick;
					mUseItemData.OnClickUSECancle = UseItemOnClickCancle; 
			   }
			}
        });	
	}
	
	
	void RequestEquipInfo(int nItemID)
	{
		Request(new EquipCmd(nItemID), delegate(string err, Response response)
        {
            if (!string.IsNullOrEmpty(err))
            {
				MessageBoxWnd.Instance.Show( err ,MessageBoxWnd.StyleExt.Alpha );
                return;
            }
            OnEquSuccess();
			SoundManager.Instance.PlaySound("UI_Prop_PutOn");
        });	
	}
	
	void RequestSellInfo(int nItemID,int Count)
	{
		Global.ShowLoadingStart();
        Request(new SellCmd(nItemID,Count), delegate(string err, Response response)
        {
            if (!string.IsNullOrEmpty(err))
            {
				Global.ShowLoadingEnd();
				MessageBoxWnd.Instance.Show(err,MessageBoxWnd.StyleExt.Alpha);
                return;
            }
            OnSellSuccess();
			SoundManager.Instance.PlaySound("UI_Coin_Get");
        });
	}
	
	void RequstMedEquipInfo(int nItemID,int nSlotIndex)
	{
 		Request(new EquipPropsCmd(nItemID,nSlotIndex), delegate(string err, Response response)
        {
			if (!string.IsNullOrEmpty(err))
            {
				MessageBoxWnd.Instance.Show( err ,MessageBoxWnd.StyleExt.Alpha );
				return;
			}
			OnMedEquipSuccess();
		});
	}
		
	void RequsetPackPageBuyInfo()
	{
		Request(new BuyPackagePageCmd(), delegate(string err, Response response)
        {
			if (!string.IsNullOrEmpty(err))
            {
				MessageBoxWnd.Instance.Show( err ,MessageBoxWnd.StyleExt.Alpha );
				return;
			}
			PlayerDataManager.Instance.BackPack.Pages++;
			OnBuyPageSuccess();
		});
	}
	
	
	void RequestPackageInfo(bool updateEquip)
	{
        Request(new GetBackPackCmd(), delegate(string err, Response response)
        {
			Global.ShowLoadingEnd();
            if (!string.IsNullOrEmpty(err))
            {
                Debug.LogError("RequestPackageInfo Error!!" + err);
				MessageBoxWnd.Instance.Show( err ,MessageBoxWnd.StyleExt.Alpha);
                return;
            }
            // update the user 
            Package backPackData = (response != null) ? response.Parse<Package>() : null;

            if (backPackData != null)
            {
                PlayerDataManager.Instance.OnBackPack(backPackData, updateEquip);
				
				BackPack.PackageMaxIDSet();
				UpdatePackageCountLabel();
				
				{
					Item item = ItemGetByItemID (mGoFocusItem);
					BtnUpdate(item);
				}
				if(updateEquip){
					UpdateByItemID();
					EquipChangeWnd.Instance.Equip();
				}else{
					Update();
				}
            }
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

            // update the user attribute.
            MainAttrib attrib = (response != null) ? response.Parse<MainAttrib>() : null;
            if (attrib != null)
			{
				PlayerDataManager.Instance.OnMainAttrib(attrib);
			}
			mResourceBar.Update(attrib.Gold,attrib.Gem,attrib.SP,-1);
        });
		//Global.ShowLoadingStart();
	}
	
//	void OnGetPhysicSuccess()
//	{
//		MainAttrib attrib = PlayerDataManager.Instance.Attrib;
//		StrengthInfo Phy = PlayerDataManager.Instance.StrengthInfo;
//		mResourceBar.Update(attrib.Gold,attrib.Gem,Phy.value);
//	}
	
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
			//OnGetPhysicSuccess();
		});
	}
	
	public void ComeFromLevelSelect()
	{
		mIsComeFromMain = false;
	}
}

