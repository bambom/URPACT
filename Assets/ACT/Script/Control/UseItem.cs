using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UseBaseItem
{
	public string Icon;
	public int 	  Count;
	public int    Quality;
}


public class UseItem : ControlCommon
{
	public string PrefabName { get { return "UseItemWnd"; } }
	public string PrefabBaseName { get { return "UseBaseItem"; } }
	
	UseItemCmd.UseItemData mUseItemData;
	
	GameObject mExtBackground;
	GameObject mAnchor;
	
	GameObject mGoUseWnd;
	int mUseItemCount;
	int mItemID;
	UILabel mUseLabel;
	
	public delegate void OnClickedUse( GameObject go );
	public OnClickedUse OnClickUSE;
	public delegate void OnClickedUseCancle( GameObject go );
	public OnClickedUseCancle OnClickUSECancle;
	
	List<UseBaseItem> mUseItemList = new List<UseBaseItem>();
	List<GameObject> mGoItem = new List<GameObject>();
	List<Vector3> mPosList = new List<Vector3>();
	
	float mDeltaTime = 0.0f;
	
	void SetWndStyle()
	{
		mAnchor = GameObject.Find("Anchor");
		mExtBackground = GameObject.Instantiate(Resources.Load("BackgroundExtWnd")) as GameObject;
		mExtBackground.transform.parent = mAnchor.transform;
		mExtBackground.transform.localPosition = Vector3.zero + new Vector3(0.0f, 0.0f, 800.0f);
		mExtBackground.transform.localScale = Vector3.one + new Vector3(1500.0f, 1500.0f, 1.0f);
	}
	
	public UseItem(UseItemCmd.UseItemData Data,int nCount,int ItemID)
	{
		//SetWndStyle();
		mUseItemData = Data;
		mUseItemCount = nCount;
		mItemID = ItemID;
		
		if(mGoUseWnd == null)
		{
			mGoUseWnd = GameObject.Instantiate(Resources.Load(PrefabName)) as GameObject;
	        GameObject RootUI = GameObject.Find("Anchor");
	        mGoUseWnd.transform.parent = RootUI.transform;
	        mGoUseWnd.transform.localPosition = Vector3.zero + new Vector3(0.0f,0.0f,-350.0f);
	        mGoUseWnd.transform.localScale = Vector3.one;		
		}
		
		UIEventListener.Get(Control ("BtnUse")).onClick = OnClickUse;
		UIEventListener.Get(Control ("BtnCancle")).onClick = OnClickUseCancle;
		BtnInit();
		InitGrid();
		AnimatePlay();
		
		if( ItemID == 0 )
		{
			mGoUseWnd.AddComponent<ControlHelper>().ControlHelpUpdate = UseItemUpdate;
			Control ("BtnUse").SetActive(false);
			Control ("BtnCancle").SetActive(false);
		}
	}
	
	void UseItemUpdate()
	{
		mDeltaTime += Time.deltaTime;
		
		if( mDeltaTime > 2.0f)
			Close();
	}
	
	void OnClickUse( GameObject go )
	{
		RequestItemUseInfo(mItemID);
	}
	
	void OnClickUseCancle( GameObject go )
	{
		if( OnClickUSECancle != null)
			OnClickUSECancle(go);
		
		Close();
	}
	
	void InitBaseItem( GameObject go)
	{
		
	}
	
	public void Update( UseItemCmd.UseItemData Data )
	{
		mUseItemCount -- ;
		BtnUpdate();
		UpdateBaseItemList();
		UpdateGrid ();
	}
	

	void RequestItemUseInfo(int nItemID)
	{
		Item item = PlayerDataManager.Instance.BackPack.Find(nItemID);
		MainScript.Instance.Request(new UseItemCmd(nItemID), delegate(string err, Response response)
        {
            if (!string.IsNullOrEmpty(err))
            {
				MessageBoxWnd.Instance.Show( err ,MessageBoxWnd.StyleExt.Alpha );
                return;
            }
			mUseItemData = (response != null) ? response.Parse<UseItemCmd.UseItemData>() : null;
			Update( mUseItemData );
			if( OnClickUSE != null && Control ("BtnUse") )
				OnClickUSE( Control ("BtnUse") );
        });	
	}
	
	void Close()
	{
		if(mExtBackground)
		{
			GameObject.Destroy(mExtBackground);
			mExtBackground = null;
		}
		
		if(mGoUseWnd)
		{
			GameObject.Destroy(mGoUseWnd);
			mGoUseWnd = null;
		}
	}
	
	void UpdateBaseItemList()
	{
		ResetBaseItemList();
		InitBaseItemList();
	}
	
	void GetCompeletFuncName()
	{
		
	}
	
	void UpdateGrid()
	{
		InitGrid();
		AnimatePlay();
	}
	
	void ResetBaseItemList()
	{
		if( mUseItemList.Count >0 )
			mUseItemList.Clear();
	}
	
	void InitBaseItemList()
	{
		if(mUseItemData.Gold > 0)
		{
			UseBaseItem UItem = new UseBaseItem();
			UItem.Icon = "Icon01_System_Gold_01";
			UItem.Count = 1;
			UItem.Quality = 0;
			mUseItemList.Add(UItem);
		}
		
		if(mUseItemData.Gem > 0)
		{
			UseBaseItem UItem = new UseBaseItem();
			UItem.Icon = "Icon01_System_Gem_01";
			UItem.Count = 1;
			UItem.Quality = 0;
			mUseItemList.Add(UItem);
		}
		
		if(mUseItemData.Exp > 0)
		{
			UseBaseItem UItem = new UseBaseItem();
			UItem.Icon = "Icon01_System_Xp_01";
			UItem.Count = 1;
			UItem.Quality = 0;
			mUseItemList.Add(UItem);
		}
		
		if(mUseItemData.SP > 0)
		{
			UseBaseItem UItem = new UseBaseItem();
			UItem.Icon = "Icon01_System_Sp_01";
			UItem.Count = 1;
			UItem.Quality = 0;
			mUseItemList.Add(UItem);
		}
		
		if(mUseItemData.Strength > 0)
		{
			UseBaseItem UItem = new UseBaseItem();
			UItem.Icon = "Button16_Strength_Normal_01";
			UItem.Count = 1;
			UItem.Quality = 0;
			mUseItemList.Add(UItem);
		}
		
		if(mUseItemData.Items.Count > 0)
		{
			int itemID =0;
			int itemCount = 0;
			ItemBase itemBase;
			foreach( KeyValuePair<string,int> item in mUseItemData.Items )
			{
				itemID = int.Parse(item.Key);
				itemCount = item.Value;
				itemBase = ItemBaseManager.Instance.GetItem(itemID);
				UseBaseItem UItem = new UseBaseItem();
				UItem.Icon = itemBase.Icon;
				UItem.Count = itemCount;
				UItem.Quality = itemBase.Quality;
				mUseItemList.Add(UItem);
			}
		}	
	}
	
	void AnimatePlay()
	{	
		if(mGoItem.Count == 0)
			return;
		
		Vector3 vMargin = new Vector3(130.0f,0.0f,0.0f);
		if( mPosList.Count > 0 )
			mPosList.Clear();
		
		for(int i = 0; i< mUseItemList.Count; i++)
		{
			Vector3 Vpos = Mathf.Pow ((-1),i) * vMargin* ( (i+1)/2 ) ;
			mPosList.Add(Vpos);	
		}
		
		for(int i = 0; i< mUseItemList.Count; i++)
		{
			iTween.MoveTo(mGoItem[i], iTween.Hash("position",mPosList[i],"time", 0.2f*i,"delay", 0.0f,
				"easetype", iTween.EaseType.linear,"islocal",true));
		}
	}
	
	
	void InitGrid()
	{
		GameObject ItemGrid = Control("ItemGrid");
		if(ItemGrid == null)
			return;	
		
		DestroyGridListItem(ItemGrid.transform);
		ResetBaseItemList();
		InitBaseItemList();
		mGoItem.Clear();
		for(int i = 0; i< mUseItemList.Count; i++)
		{
			GameObject goItem = GameObject.Instantiate(Resources.Load(PrefabBaseName))as GameObject;
			goItem.name = PrefabBaseName + i.ToString();
			goItem.transform.parent = ItemGrid.transform;
			goItem.transform.localPosition = Vector3.zero; //vStartPos + vMargin*i;
			goItem.transform.localScale = Vector3.one;
			goItem.transform.Find("Icon").GetComponent<UISprite>().spriteName = mUseItemList[i].Icon;
			UpdateItemQualityFrameIcon(mUseItemList[i].Quality,goItem.transform.Find("QualityIcon").gameObject);
			goItem.transform.Find("Count").GetComponent<UILabel>().text = mUseItemList[i].Count.ToString();
			mGoItem.Add(goItem);
			//UIEventListener.Get(goItem).onClick = OnClickBtnBuyOne;
		}
	}
	
	void BtnInit()
	{
		if(Control ("USE") == null)
			return;
		
		mUseLabel = Control("USE").GetComponent<UILabel>();
		if( mUseItemCount == 0 )
		{
			BtnSetAble(Control ("BtnUse"),false);
			mUseLabel.text = "使用";
		}
		else
		{
			BtnSetAble(Control ("BtnUse"),true);
			mUseLabel.text = "使用(" + mUseItemCount.ToString() + ")";
		}
	}
	
	void BtnUpdate()
	{
		BtnInit();
	}
		
	GameObject Control(string name)
    {
        return Control(name, mGoUseWnd);
    }
	
	GameObject Control(string name, GameObject parent)
    {
        Transform[] children = parent.GetComponentsInChildren<Transform>();
        foreach (Transform child in children){
            if (child.name == name)
                return child.gameObject;
        }
        return null;
    }
	
}

