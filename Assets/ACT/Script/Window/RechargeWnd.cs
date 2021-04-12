using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RechargeWnd : Window<RechargeWnd> {

 	public override string PrefabName { get { return "RechargeWnd"; } }
	GameObject mGoResbarRoot;
	ResourceBar mResourceBar;
	string mProductID;
	Dictionary<GameObject, RechargeBase> mRechargeTable = new Dictionary<GameObject, RechargeBase>();
	
	protected override bool OnOpen()
    {
		Init();
		WinStyle = WindowStyle.WS_CullingMask;
        return base.OnOpen();;
    }

    protected override bool OnClose()
    {
		return base.OnClose();
    }
	
	void Init()
	{
		mGoResbarRoot = Control("ResBarRoot");
		UIEventListener.Get(Control("Return")).onClick = OnClickBtnReturn;
		
		ResourceBarInit();
		InitGrid();
	}
	
	void InitGrid()
	{
		string GridContent = "Recharge";
		int nCount = 0;
		GameObject ItemGrid = Control("ItemGrid");
		mRechargeTable.Clear();
		foreach( RechargeBase rechargeBase in RechargeBaseManager.Instance.GetAllItem())
		{
			GameObject goItem = GameObject.Instantiate(Resources.Load("RechargeItem"))as GameObject;
			goItem.name = GridContent + rechargeBase.ID;
			goItem.transform.parent = ItemGrid.transform;
			goItem.transform.localPosition = Vector3.zero;
			goItem.transform.localScale = Vector3.one;
			if(rechargeBase.USD == 0)
			{
				Control("GemNum",goItem).GetComponent<UILabel>().text = rechargeBase.Gold.ToString();
				Control("GoldNum",goItem).GetComponent<UILabel>().text = Mathf.Abs(rechargeBase.Gem).ToString(); //+ "Gem";
				Control("PriceIcon",goItem).GetComponent<UISprite>().spriteName = "Icon01_System_Gold_01";
				Control("BuyIcon",goItem).SetActive(true);
			}
			else
			{
				Control("GemNum",goItem).GetComponent<UILabel>().text = rechargeBase.Gem.ToString();
				float usdollor = rechargeBase.USD;
				Control("GoldNum",goItem).GetComponent<UILabel>().text = "$ " + (usdollor/100.0f).ToString();
				//Control("PriceIcon",goItem).SetActive(true);
				Control("BuyIcon",goItem).SetActive(false);
			}
			Control("Gem",goItem).GetComponent<UISprite>().spriteName = rechargeBase.Icon;
			UIEventListener.Get(goItem).onClick = OnClickBtnBuyOne;
			mRechargeTable[goItem] = rechargeBase;
		}
	}
	
	void OnClickBtnBuyOne(GameObject go)
	{
		if (mRechargeTable.ContainsKey(go))
		{
			RechargeBase rechargeBase = mRechargeTable[go];
			if ( rechargeBase.USD == 0){
				RequestExCharge(rechargeBase);
			}else{
				//Global.ShowLoadingStart();
				IAPUnity.BuyGem(rechargeBase.ProductID);
			}
		}
	}
	
	void OnClickBuyGlod(GameObject go)
	{
        Request(new ExchangeCmd(mProductID), delegate(string err, Response response)
        {
			Global.ShowLoadingEnd();
            if (!string.IsNullOrEmpty(err)){
				MessageBoxWnd.Instance.Show( err ,MessageBoxWnd.StyleExt.Alpha);
                return;
            }
			 MainAttrib attrib = (response != null) ? response.Parse<MainAttrib>() : null;
            if (attrib != null)
            {
                PlayerDataManager.Instance.Attrib.Gold = attrib.Gold;
                PlayerDataManager.Instance.Attrib.Gem = attrib.Gem;
            }
			mResourceBar.Update(attrib.Gold,attrib.Gem,attrib.SP,-1);
        });
		//Global.ShowLoadingStart();
	}
	
	void OnClickCancleBuyGlod(GameObject go)
	{
		//Global.ShowLoadingEnd();
	}
	
	void RequestExCharge(RechargeBase ReBase)
	{
		mProductID = ReBase.ProductID;
		MessageBoxWnd.Instance.Show("BuyGoldWithGem",MessageBoxWnd.Style.OK_CANCLE,Mathf.Abs(ReBase.Gem),Mathf.Abs(ReBase.Gold));
		MessageBoxWnd.Instance.OnClickedOk = OnClickBuyGlod;
		MessageBoxWnd.Instance.OnClickedCancle = OnClickCancleBuyGlod;
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
	
	void OnClickBtnReturn(GameObject go)
	{
		Close();
		if(RoleDeadWnd.Exist)
		{
			RoleDeadWnd.Instance.Show();
			return;
		}
		
		if(InGameMainWnd.Exist)
			InGameMainWnd.Instance.Show();
		
		if(FightMainWnd.Exist){
			LevelHelper.LevelContinue(false);
			FightMainWnd.Instance.Show();
		}
	}
	
	public void Update()
	{
		MainAttrib attrib = PlayerDataManager.Instance.Attrib;
		mResourceBar.Update(attrib.Gold,attrib.Gem,attrib.SP,-1);
	}
}
