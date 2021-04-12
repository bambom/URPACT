using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NpcShopWnd : Window<NpcShopWnd>
{	
	private enum ItemTypeX
	{
		None,
		Weapon,
		Defend,
		Decorate,
		Prop,
		//Drug,
		Fuwen,
		Strengthen,
	}	
	private class Option
	{
		public int MainMin;
		public int MainMax;
		public int SubMin;
		public int SubMax;
	};
	
	private Option mOption = new Option();
	
	ItemTypeX mChooseType = ItemTypeX.None;	
	
	GameObject mGoDescRoot;
	GameObject mGoResbarRoot;
	GameObject mGoToolBarRoot;
	DescDetail mDescDetail;
	ResourceBar mResourceBar;
	ItemToolBar mItemToolBar;
    UIDraggablePanel mShopDragPanel;
    Dictionary<byte, ItemBase> mTryItems = new Dictionary<byte, ItemBase>();
	
	int mCount = 0;
	const int mItemCount = 12;
	GameObject mParentPanel;
	GameObject mChooseItem;
	
	GameObject mShopUnit;
	GameObject mListGrid;
	Transform mSlidePoint;
	
	GameObject  mFocusPoint;
	float mStartPosition;
	int  mCurrentIndex = 0;
	float mPointOffset = 35.0f;
	int mShopId;
	
    public override string PrefabName { get { return "NpcShopWnd"; } }

		
	
	#region Protected
	protected override bool OnOpen()
    {
		Init();
		WinStyle = WindowStyle.WS_Ext;
        return base.OnOpen();
    }
	
    protected override bool OnClose()
    {
		EquipChangeWnd.Instance.Close();
        return base.OnClose();
    }
	#endregion

	void Init()
	{
		CharactorModelInit();
		mGoDescRoot = Control("AttribRoot");
		mGoToolBarRoot = Control("ToolBarRoot");
		mGoResbarRoot = Control("ResBarRoot");
		UIEventListener.Get ( Control("BtnReturn") ).onClick = OnClickBtnReturn;
		UIEventListener.Get ( Control("BtnBuy")).onClick = OnClickBtnBuy;
		//UIEventListener.Get(Control ("BtnInventory")).onClick = OnClickBtnInven;
		
		mParentPanel = Control("GridPanel");
		mListGrid = Control("ListGrid",mParentPanel);
		mSlidePoint = Control("SlidePoint").transform;
		mShopDragPanel = mParentPanel.GetComponent<UIDraggablePanel>();
		
		mDescDetail = new DescDetail(DescType.AttribAll,PlayerDataManager.Instance.Attrib,mGoDescRoot,false);
		mDescDetail.Init(null);
		//SwapDescIcon(false);
	
		ResourceBarInit();
		ItemToolBarInit();
		InitOption();		
		RefreshList();
	}
	
	void CharactorModelInit()
	{
		if( !EquipChangeWnd.Exist )
			EquipChangeWnd.Instance.Open();

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
	
	void InitOption()
	{					
		mOption.MainMin = 1;
		mOption.MainMax = 10;
		mOption.SubMin =  1;
		mOption.SubMax =  10;		
	}
	
	private void OnClickBtnReturn( GameObject go )
	{
		Close();
		if( !InGameMainWnd.Exist )
			InGameMainWnd.Instance.Open();
		else 
			InGameMainWnd.Instance.Show();
	}

	
	#region ResourceBar
	void OnClickedAddMoney(GameObject go)
	{
		Close();
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
		UpdateOption(MainTF,MainTT,SubTF,SubTT);
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
		UpdateOption(MainTF,MainTT,SubTF,SubTT);
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
		UpdateOption(MainTF,MainTT,SubTF,SubTT);
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
		UpdateOption(MainTF,MainTT,SubTF,SubTT);
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
		UpdateOption(MainTF,MainTT,SubTF,SubTT);
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
		UpdateOption(MainTF,MainTT,SubTF,SubTT);
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
		UpdateOption(MainTF,MainTT,SubTF,SubTT);
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
		UpdateOption(MainTF,MainTT,SubTF,SubTT);
	}
	#endregion ItemToolBarBtn
	
	void UpdateOption(int mainMin,int mainMax,int subMin,int subMax)
	{
		mOption.MainMax = mainMax;
		mOption.MainMin = mainMin;
		mOption.SubMax = subMax;
		mOption.SubMin = subMin;
		RefreshList();
	}
	
	void RefreshList()
	{
		DestroyListItem();		
		UIGrid  mGrid = mListGrid.GetComponent<UIGrid>();
		ArrageByExpand(ItemExpand.Expand_Gem);
		ArrageByExpand(ItemExpand.Expand_Gold);
		ArrageByExpand(ItemExpand.Expand_Sp);		
		AddExtraItem();
		ShowPagePoint();  
		JudgeDrag();			
		mGrid.repositionNow = true; 	
		MovePage(0);
	}
		//reposition the grid	
	void ArrageByExpand(ItemExpand expand)
	{
		MainAttrib attrib = PlayerDataManager.Instance.Attrib;
		// load the shop item base,every unit have ten Items.		
		GameObject shopUnitPrefab = Resources.Load("ShopUnit") as GameObject;
        GameObject shopItemPrefab = Resources.Load("ShopItem") as GameObject;
		
        // register all of the shop items.
        ShopBase[] shopItems = ShopBaseManager.Instance.GetAllItem();	
        foreach (ShopBase shopItem in shopItems)
        {
            ItemBase itemBase = ItemBaseManager.Instance.GetItem(shopItem.Item);
            if (itemBase == null)
            {
                Debug.LogError(string.Format("Shop item [{0}] not found in item base [{1}].", shopItem.ID, shopItem.Item));
                continue;
            }		
            if (itemBase.Role != attrib.Role && itemBase.Role.ToString() !="0" )
                continue;
		    if (!IsInOption(itemBase))
				continue;
			
            bool canBuy = true;
            if (attrib.Level < shopItem.MinLevel ||
                attrib.Level > shopItem.MaxLevel)
				continue;
			
			if((int)shopItem.Type != (int)expand)
				continue;
			
			if (mCount % mItemCount == 0 )
			{   
				GameObject shopUnitObject = GameObject.Instantiate(shopUnitPrefab) as GameObject;
				SetObjectPosition(shopUnitObject,mListGrid);
				shopUnitObject.name = "Item" + (mCount/mItemCount +1).ToString();
				mShopUnit = shopUnitObject;						
			}		
            GameObject shopItemObject = GameObject.Instantiate(shopItemPrefab) as GameObject;
			mCount++;
				
			SetObjectPosition(shopItemObject,mShopUnit);
			shopItemObject.name = shopItem.ID.ToString();
			ItemExpend(Control("Price", shopItemObject),shopItem);			
            // TODO: add to the shop panel.
			UISprite shopItemIcon = Control("GoodsIcon", shopItemObject).GetComponent<UISprite>();
			shopItemIcon.spriteName = itemBase.Icon;
			UpdateItemQualityFrameIcon(Control("AttribeIcon",shopItemObject),itemBase);
			if(canBuy)
				UIEventListener.Get(shopItemObject).onClick = OnClickChooseItem;		
        }
	}
	
	void AddExtraItem()
	{
		int extraNum = mCount % mItemCount;
		if(extraNum >0 && extraNum <4)
		{
			for(int i=0; i<(4-extraNum) ;i++)
			{
				GameObject addObj = GameObject.Instantiate(Resources.Load("ShopExtraItem")) as GameObject;
				SetObjectPosition(addObj,mShopUnit);
			}			    
		}
	}
	
	void ItemExpend(GameObject item,ShopBase Sbase)
	{
		switch((int)Sbase.Type)
		{
			case (int)ItemExpand.Expand_Gem:
				Control("GoldIcon",item).GetComponent<UISprite>().spriteName = "Icon01_System_Gem_01";
				item.GetComponentInChildren<UILabel>().text = Sbase.Gem.ToString();
				break;	
			case (int)ItemExpand.Expand_Gold:			
				Control("GoldIcon",item).GetComponent<UISprite>().spriteName = "Icon01_System_Gold_01";
				item.GetComponentInChildren<UILabel>().text = Sbase.Gold.ToString();
			    item.GetComponentInChildren<UILabel>().color = Color.blue;
			    break;
			case (int)ItemExpand.Expand_Sp:
				Control("GoldIcon",item).GetComponent<UISprite>().spriteName = "Icon01_System_Sp_01";
				item.GetComponentInChildren<UILabel>().text = Sbase.SP.ToString();
			    item.GetComponentInChildren<UILabel>().color = Color.green;
				break;			
			default:
				break;				
		}
	}
	
    void JudgeDrag()
	{
		if(mCount < mItemCount)
			mShopDragPanel.enabled = false;
	    else
		{
			mShopDragPanel.enabled = true;
			mShopDragPanel.GodMove = true;
			mShopDragPanel.OnGodDragFinished = OnDragFinish;
		}
	}
	
	void OnDragFinish()
	{
		int totalPage = (mCount + mItemCount -1)/mItemCount;
		if(mShopDragPanel.CurIndex < 0)
			mShopDragPanel.CurIndex = 0;
		if(mShopDragPanel.CurIndex > totalPage -1)
			mShopDragPanel.CurIndex = totalPage - 1;	
		
		mFocusPoint.transform.localPosition = new Vector3(
			mStartPosition + mPointOffset * mShopDragPanel.CurIndex ,0,0);
		
	}
	bool IsInOption(ItemBase itemBase)
	{
		int main = (int)itemBase.MainType;
		int sub = (int)itemBase.SubType;
		if( itemBase.MainType == (int)EItemType.Equip 
			&& mOption.MainMax == mOption.MainMin
			&& mOption.SubMin < (int)EquipSubType.EST_Wrist 
			&& mOption.SubMax > (int)EquipSubType.EST_Wrist
			&& itemBase.SubType == (int)EquipSubType.EST_Wrist)
		{
			return false;
		}
		if(main > mOption.MainMax || main < mOption.MainMin||
			sub > mOption.SubMax || sub < mOption.SubMin)
			return false;
		else 
			return true;
	}
		
	ItemTypeX GetItemType(string mainType, string subType)
	{
		ItemTypeX resultType = ItemTypeX.None;
		switch(mainType){
			case "1":
				switch(subType){
					case "1":
						resultType = ItemTypeX.Weapon;
						break;
					case "2":						
				    case "3":					   
				    case "4":
					case "5":
						resultType = ItemTypeX.Defend;
						break;
					case "6":
					case "7":
				    case "8":
				    case "9":
				 		resultType = ItemTypeX.Decorate;
					    break;
					default:
						break;
		
				}
			    break;
			case "2":
			    resultType = ItemTypeX.Fuwen;
			    break;
			case "3":
				resultType = ItemTypeX.Strengthen;
				break;
			case "4":
				resultType = ItemTypeX.Prop;
				break;
			default:
				break;
		}	
		return resultType;
	}
	
	void UpdateDescribePanel(ItemBase itemBase)
	{
		if(mDescDetail.DscType == DescType.AttribAll){
			mDescDetail.Destroy();
			mDescDetail = new DescDetail(DescType.AttribOne,PlayerDataManager.Instance.Attrib,mGoDescRoot,true);
		}
		mDescDetail.UpdateOne( itemBase );
		//SwapDescIcon(true);
	}
	
	void OnClickChooseItem (GameObject go)
	{  
		if(mChooseItem == go)
			return;	
		
		mShopId = int.Parse(go.name);
		if(mChooseItem != null)
		{
			mChooseItem.transform.Find("ChooseFrame").gameObject.SetActive(false);  
			mChooseItem.transform.Find("ChooseFrame").GetComponent<TweenAlpha>().enabled = false;
		}
		go.transform.Find("ChooseFrame").gameObject.SetActive(true);
		go.transform.Find("ChooseFrame").GetComponent<TweenAlpha>().enabled = true;
		//UpdateItemExaplain(mShopId);	

        ShopBase shopBase = ShopBaseManager.Instance.GetItem(mShopId);
        ItemBase itemBase = ItemBaseManager.Instance.GetItem(shopBase.Item);
		UpdateDescribePanel(itemBase);
		mChooseItem = go;

        if (itemBase.MainType == (int)EItemType.Equip)
        {
            mTryItems[itemBase.SubType] = itemBase;
            EquipChangeWnd.Instance.Charactor.EquipWithReplace(mTryItems);
        }
	}

	string GetEquipAttribValue(string showName,ItemBase item)
	{
		string resultStr = "";
		EquipBase equipBase = EquipBaseManager.Instance.GetItem(item.ID);
		switch(showName.Replace(" ","")){
			case "Attack": 
				return equipBase.Damage.ToString();
			case "Life":
				return equipBase.HPMax.ToString();
			case "Defense":
				return equipBase.Defense.ToString();
			case "S-Attack":
				return equipBase.SpecialDamage.ToString();
			case "S-Defense":
				return equipBase.SpecialDefense.ToString();
			case "Hit":
				return equipBase.Hit.ToString();
			case "Block":
				return equipBase.Block.ToString();
			case "Tough":
				return equipBase.Tough.ToString();
			case "Critical":
				return equipBase.Critical.ToString();
			default:
				break;
		}
		return resultStr;
	}
		
	
	void MovePage(int page)
	{		
		Vector3 target = Vector3.zero;
		Vector3 relative = target - mParentPanel.transform.localPosition;
		mParentPanel.transform.localPosition = target;
		Vector4 cr = mParentPanel.GetComponent<UIPanel>().clipRange;
		cr.x -= relative.x;
		cr.y -= relative.y;
		mParentPanel.GetComponent<UIPanel>().clipRange = cr;	
	}
	
    void ShowPagePoint()
	{   
		Debug.Log("mCount:"+mCount);
		int totalPage = (mCount + mItemCount -1)/mItemCount;
		GetStartPos(totalPage);
		for(int i=0;i< totalPage;i++)
		{			
			GameObject point =  GameObject.Instantiate(Resources.Load("NormalPoint")) as GameObject;	
			point.transform.parent = mSlidePoint.transform;
			point.transform.localScale = new Vector3(28,28,0);
			//set the normalpoint's scale
			point.transform.localPosition = new Vector3(mStartPosition + mPointOffset*i,0,0);
			point.GetComponent<UISprite>().depth = 2;
		}
		if(mFocusPoint == null)
			mFocusPoint = GameObject.Instantiate(Resources.Load("FocusPoint")) as GameObject;
		mFocusPoint.transform.parent = mSlidePoint.transform;
		mFocusPoint.transform.localPosition = new Vector3(mStartPosition,0,0);
		mFocusPoint.transform.localScale = new Vector3(28,28,0);
		//set the fucospoint's scale,a little bigger than that of others
		mFocusPoint.GetComponent<UISprite>().depth = 3;
		
	}
	
	void GetStartPos(int pageCount)
	{
		if(pageCount % 2 ==1 || pageCount == 0)
		{
			int gapCount = pageCount/2;
			mStartPosition = -gapCount * mPointOffset;
		}
		else
		{
			float gapCount = (float)(pageCount-1)/2.0f;
			Debug.Log(gapCount);
			mStartPosition = -gapCount * mPointOffset;
		}
	}
	
	void DestroyListItem()
	{		
		for(int i=0; i< mListGrid.transform.childCount; i++)
		    GameObject.Destroy(mListGrid.transform.GetChild(i).gameObject);		
		mListGrid.transform.DetachChildren();
		
		for(int j=0; j< mSlidePoint.transform.childCount; j++)
			GameObject.Destroy(mSlidePoint.transform.GetChild(j).gameObject);
		mCount = 0;
		mFocusPoint = null;
	}
	
	
	void InitButtonClick()
	{
		UIEventListener.Get(Control("HotSellBtn")).onClick = OnClickHotSell;
		UIEventListener.Get(Control("EquipButton")).onClick = OnClickHotSell;
		UIEventListener.Get(Control("LeftPropButton")).onClick = OnClickHotSell;
	    UIEventListener.Get(Control("MaterialButton")).onClick = OnClickHotSell;
		Transform []children = Control("UpButtonOpt").GetComponentsInChildren<Transform>();
		foreach(Transform child in children)
		{
			if(child.parent.parent.name == "UpButtonOpt"){
				Debug.Log(child.name);
				UIEventListener.Get(child.gameObject).onClick  = OnClickItemOpt;	
			}
		}    		
	}
	
	void OnClickHotSell(GameObject go)
	{
		Debug.Log(go.name);
		switch(go.name){
			case "HotSellBtn":	
				mChooseType = ItemTypeX.None;			    
				break;
			case "EquipButton":
				mChooseType = ItemTypeX.Weapon;		
				break;
			case "LeftPropButton":
				mChooseType = ItemTypeX.Prop;
				break;
			case "MaterialButton":
				mChooseType = ItemTypeX.Fuwen;
				break;
			default:
				break;
		}
	    RefreshList();	
	}
	
	void OnClickItemOpt(GameObject go)
	{		
		Debug.Log(go.name);
		switch(go.name)
		{	
		    case "WeaponButton":
			    mChooseType = ItemTypeX.Weapon;
				break;			
		    case "DefendButton":
			    mChooseType = ItemTypeX.Defend;
				break;
			case "DecorateButton":
				mChooseType = ItemTypeX.Decorate;
				break;		
			case "FuwenButton":
			    mChooseType = ItemTypeX.Fuwen;
				break;
			case "StrengthenBtn":
			    mChooseType = ItemTypeX.Strengthen;
				break;
			default:
			    mChooseType = ItemTypeX.None;
				break;		
		}
		RefreshList();
	}
	
	void OnClickBtnInven(GameObject go)
	{
		Close ();
		if(PackageWnd.Exist)
			PackageWnd.Instance.Show();
		else
			PackageWnd.Instance.Open();
	}
	
    void OnClickBtnBuy(GameObject go)
    {
		int shopId = mShopId;
		Debug.Log(shopId);
		MainAttrib attrib = PlayerDataManager.Instance.Attrib;
		ShopBase shopItem = ShopBaseManager.Instance.GetItem(shopId);	
		
		if(shopItem == null)
		{
			Debug.LogError("shopitem is null");
			return;
		}
		if (attrib.Gold < shopItem.Gold || attrib.Gem < shopItem.Gem)
		{	
			Debug.Log("you can't buy this shopitem");
		    MessageBoxWnd.Instance.Show(("LackGem"),
				MessageBoxWnd.StyleExt.Alpha);
			return;
		}
		
        Request(new BuyCmd(shopId), delegate(string err, Response response)
        {
            if (!string.IsNullOrEmpty(err))
            {
				MessageBoxWnd.Instance.Show(("BuyFailed"),
					MessageBoxWnd.StyleExt.Alpha);
                return;
            }

            OnBuySuccess();
        });
    }
    // update the player attribute.
    void OnBuySuccess()
    {       
		RequestPackageInfo();
        Request(new GetUserAttribCmd(), delegate(string err, Response response)
        {
            if (!string.IsNullOrEmpty(err))
            {
                // buy failed.
				MessageBoxWnd.Instance.Show("Buy failed : "+err,MessageBoxWnd.StyleExt.Alpha);
                return;
            }

            // update the user attribute.
			MessageBoxWnd.Instance.Show(("BuySuccess"),
					MessageBoxWnd.StyleExt.Alpha);
            MainAttrib attrib = (response != null) ? response.Parse<MainAttrib>() : null;
            if (attrib != null)
                PlayerDataManager.Instance.OnMainAttrib(attrib);
			mResourceBar.Update(attrib.Gold,attrib.Gem,attrib.SP,-1);
			if(!FindItemWnd.Exist)
				FindItemWnd.Instance.Open();
			ShopBase shopItem = ShopBaseManager.Instance.GetItem(mShopId);
			ItemBase item = ItemBaseManager.Instance.GetItem(shopItem.Item);
			FindItemWnd.Instance.AddItem(new FindItem(item, 1));
        });
    }
	
	void SetObjectPosition(GameObject child, GameObject parent)
	{
		child.transform.parent = parent.transform;
		child.transform.localPosition = Vector3.zero;
		child.transform.localScale = Vector3.one;
	}
	
	void RequestPackageInfo()
	{
        Request(new GetBackPackCmd(), delegate(string err, Response response)
        {
            if (!string.IsNullOrEmpty(err))
            {
				MessageBoxWnd.Instance.Show(err,MessageBoxWnd.StyleExt.Alpha);
                return;
            }
            // update the user 
            Package BackPackData = (response != null) ? response.Parse<Package>() : null;
            if (BackPackData != null)
			{
				PlayerDataManager.Instance.OnBackPack(BackPackData, false);
			}
        });
		//ShowProcessLoadingStart();
	}
	
}
