using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ShowPlayerInfoWnd : Window<ShowPlayerInfoWnd>
{	
    public override string PrefabName { get { return "ShowPlayerInfoWnd"; } }
	
	private static PlayerData mShowPlayerData;
	public PlayerData ShowPlayer {get{return mShowPlayerData;} set{ mShowPlayerData = value;}}
	
	
	private GameObject mEquipRoot;
	private GameObject mExpSlider;
    private GameObject mItemFocus;
	private GameObject mAttibIcon;
	private GameObject mRoleInfoBtn;
	private GameObject mEquipInfoBtn;
	private GameObject mChooseItem;
	GameObject mGoAttribRoot;
	private int mItemFocusId = -1;
	DescDetail mDescDetail;
	UILabel mUserInfoLabel;
	TweenAlpha mTweenAlpha;
	
	
	protected override bool OnOpen()
    {
		Init();
        return base.OnOpen();
    }

    protected override bool OnClose()
    {
        if (!InGameMainWnd.Exist)
            InGameMainWnd.Instance.Open();
        return base.OnClose();
    }
	
	private void Init()
	{	
		mRoleInfoBtn = Control("RoleInfoBtn");
		mEquipInfoBtn= mRoleInfoBtn.transform.parent.Find("EquipInfoBtn").gameObject;
		
		mEquipRoot = Control("EquipRoot" );
		mItemFocus = Control("ItemFocus");
		mTweenAlpha = mItemFocus.GetComponent<TweenAlpha>();
		mGoAttribRoot = Control("AttribRoot");
		UIEventListener.Get(Control("BgButton")).onClick = OnClickCloseWnd;
		
		UIEventListener.Get(mRoleInfoBtn).onClick = OnClickGetRoleInfo;
		UIEventListener.Get(mEquipInfoBtn).onClick = OnClickGetEquipInfo;

		InitCharactor();
		InitItemFocus();
		InitEquipment();
		UpdateEquipment();
		mDescDetail = new DescDetail(DescType.AttribAll,mShowPlayerData.Attrib,mGoAttribRoot,false);
		mDescDetail.Init(null);
		//UpdateAttribAllPanel();
	}
	
	private void InitItemFocus()
	{
		mItemFocus.SetActive(false);
	    mTweenAlpha.enabled = false;
	}
	
	private void UpdateItemFocus(GameObject go)
	{
		if(!mItemFocus.activeSelf)
			mItemFocus.SetActive(true);
		if(!mTweenAlpha.enabled)
			mTweenAlpha.enabled = true;
		mItemFocus.transform.parent = go.transform;
		mItemFocus.transform.localPosition = new Vector3(-2,2,0);
		mItemFocus.transform.localScale = new Vector3(90.0f,90.0f,0.0f);
	}
	

	private void InitCharactor()
	{			
		if( !EquipChangeWnd.Exist )
			EquipChangeWnd.Instance.Open();
		
		EquipChangeWnd.Instance.UpdateCharactor(mShowPlayerData);
		
		PressDrag PDScript = Control ("CharactorCollider").GetComponent<PressDrag>();
		
		if(null != PDScript){
			PDScript.target = EquipChangeWnd.Instance.CharactorRoot.transform;
		}else{
			PDScript = Control ("CharactorCollider").AddComponent<PressDrag>();
			PDScript.target = EquipChangeWnd.Instance.CharactorRoot.transform;
		}
		
		EquipChangeWnd.Instance.ResetPos(new Vector3(-90.0f,-180.0f,500.0f));
		mUserInfoLabel = Control("UserInfoLabel").GetComponent<UILabel>();
		mUserInfoLabel.text =string.Format("Lv{0}  {1}" ,mShowPlayerData.Attrib.Level,mShowPlayerData.Attrib.Name);
	}
	
	private void OnClickGetRoleInfo(GameObject go)
	{
		if(mItemFocusId != -1)
		{
			UpdateAttribAllPanel();
		}	
	}
	
	private void OnClickGetEquipInfo(GameObject go)
	{
		if(mItemFocusId != -1 && mChooseItem != null){
			OnClickEquipItem(mChooseItem);
		}
	}
	
	private void UpdateCharactor()
	{
		EquipChangeWnd.Instance.Equip();
	}
	
	void OnClickCloseWnd(GameObject go)
	{
		if(EquipChangeWnd.Exist)
			EquipChangeWnd.Instance.Close();
		Close();
	}
	
	//Here for LeftSide
	void InitEquipment()
	{
		GameObject go ;
		for(int i= 1; i < 10; i++)
		{
			go = mEquipRoot.transform.Find(i.ToString()).gameObject;
			UIEventListener.Get( go ).onClick = OnClickEquipItem;
			go.transform.Find("Icon").GetComponent<UISprite>().spriteName = "Button08_EquipItem_Bottom_0"+i;
			go.transform.Find("QualityFrame").gameObject.SetActive(false);
			//go.GetComponent<UIButtonScale>().enabled = false;
			//go.transform.Find("ItemID").GetComponent<UILabel>().text = "0" ;
		}
	}
	
	void ResetEquipment()
	{
		InitEquipment();
	}
	
	void UpdateEquipment()
	{
		ResetEquipment();
		GameObject tmpUISp;
		foreach (Item item in mShowPlayerData.Packages.BackPack.Items.Values)
		{
			if( item.ItemBase.MainType == (int)EItemType.Equip 
				&& item.ToEquipItem().Equiped )
			{
				mEquipRoot.transform.Find(((int)item.ItemBase.SubType).ToString()).Find("Icon").gameObject.SetActive(true);
				mEquipRoot.transform.Find(((int)item.ItemBase.SubType).ToString()).Find("Icon").gameObject.GetComponent<UISprite>().spriteName 
					= item.ItemBase.Icon;
				mEquipRoot.transform.Find(((int)item.ItemBase.SubType).ToString()).Find("ItemID").GetComponent<UILabel>().text
					= item.ID.ToString();
				mEquipRoot.transform.Find(((int)item.ItemBase.SubType).ToString()).GetComponent<BoxCollider>().enabled = true;
				tmpUISp = mEquipRoot.transform.Find(((int)item.ItemBase.SubType).ToString()).Find("QualityFrame").gameObject;
				
				UpdateItemQualityFrameIcon(tmpUISp,item.ItemBase);
				
			}
		}
	}
	
	void UpdateAttribAllPanel()
	{
		if(mDescDetail.DscType == DescType.AttribOne){
			mDescDetail.Destroy();
			mDescDetail = new DescDetail(DescType.AttribAll,mShowPlayerData.Attrib,mGoAttribRoot,false);
		}
		mDescDetail.Init(null);
		if(mRoleInfoBtn.activeSelf)
		{
			mRoleInfoBtn.SetActive(false);
			mEquipInfoBtn.SetActive(true);
		}
	}
	
	void UpdateAttribOnePanel(Item item)
	{
		if(mDescDetail.DscType == DescType.AttribAll){
			mDescDetail.Destroy();
			mDescDetail = new DescDetail(DescType.AttribOne,mShowPlayerData.Attrib,mGoAttribRoot,false);
		}
		mDescDetail.Init(item);
		if(!mRoleInfoBtn.activeSelf)
		{
			mRoleInfoBtn.SetActive(true);
			mEquipInfoBtn.SetActive(false);
		}
	}
	
	void OnClickCharactor ( GameObject go )
	{
		mItemFocus.SetActive(false);
		UpdateAttribAllPanel();
	}
	
	void OnClickEquipItem( GameObject go)
	{
		mItemFocusId = int.Parse( go.transform.Find("ItemID").GetComponent<UILabel>().text );
		Item item = mShowPlayerData.Packages.BackPack.Find(mItemFocusId);
		mChooseItem = go;
		UpdateItemFocus(go);
		if(item == null)
			return;
		UpdateAttribOnePanel(item);
	}
	
	
	public void RequestGetUserInfo(string User,Vector3 pos)
	{   	
 		MainScript.Instance.Request(new GetUserDataCmd(User,true), delegate(string err, Response response)
        {
            mShowPlayerData = (response != null) ? response.Parse<PlayerData>() : null;
			ShowPlayerInfoWnd.Instance.Open();
			ShowPlayerInfoWnd.Instance.WndObject.transform.localPosition = pos;
        });
	}
}
