using UnityEngine;
using System.Collections;

public class IconTips : ControlCommon
{
	public string PrefabName { get { return "IconTips"; } }
	
	GameObject mGoIconTipsRoot;
	GameObject mGoGsRoot;
	UILabel mTitleName;
	UILabel mContentDesc;
	UILabel mMainAttrib;
	Vector3 mVpos;
	ItemBase mItemBase = null;
	public delegate void OnClickClose();
	public OnClickClose OnClickedClose;
	
	public IconTips( GameObject goRoot,ItemBase itemBase,Vector3 vPos )
	{
		mGoIconTipsRoot = GameObject.Instantiate(Resources.Load(PrefabName)) as GameObject;
		mGoIconTipsRoot.name = "GoIconTips";
		mGoIconTipsRoot.transform.parent = goRoot.transform;
		mGoIconTipsRoot.transform.localScale = Vector3.one;
		mGoIconTipsRoot.transform.localPosition = Vector3.zero + new Vector3(0.0f,0.0f,-2.0f);
		
		mItemBase = itemBase;
		mGoGsRoot = Control ("GSRoot");
		mMainAttrib = Control("LabelMain").GetComponent<UILabel>();
		
		mVpos = vPos;
		UIEventListener.Get (Control("BtnClose")).onClick = OnClickTipsClose;
		IconDecInit( itemBase );
	}
	
	void OnClickTipsClose( GameObject go )
	{
		Destroy();
		if( OnClickedClose != null )
		{
			OnClickedClose();
		}
	}
	
	public void UpdateDesc(ItemBase itemBase,Vector3 vPos)
	{
		IconDecInit(itemBase);	
	}
	
	public void IconDecInit(ItemBase itemBase)
	{
		if(mGoIconTipsRoot == null)
			return; 
		if( Control("Title") == null )
			return;
		
		// Title And Desc
		mTitleName = Control("Title").GetComponent<UILabel>();
		mContentDesc = Control ("Desc").GetComponent<UILabel>();
		mContentDesc.text = itemBase.Desc;
		mTitleName.text = TextColorGet( (int)itemBase.Quality ) + itemBase.Name + "[-]";
		Control("EquipIcon").GetComponent<UISprite>().spriteName = ItemIconGet(itemBase);	
		bool bStatus = itemBase.Level > PlayerDataManager.Instance.Attrib.Level ? true : false;
		Control("Level").GetComponent<UILabel>().text 
			= TextColorGet(bStatus) + "LV" + itemBase.Level.ToString() + "[-]";

		// All	
		if( itemBase.MainType != (int)EItemType.Equip){
			mMainAttrib.text = "";
			if ( mGoGsRoot.activeSelf )
				mGoGsRoot.SetActive(false);
			return;
		}
		else
		{
			if ( !mGoGsRoot.activeSelf )
				mGoGsRoot.SetActive(true);
		}
		
		TargetAttrib Ta = TargetAttribManager.Instance.GetItem(itemBase.MainType,itemBase.SubType);
		if( CheckEuipHasBaseAttrib(itemBase) ){
			mMainAttrib.text = Ta.ShowName + "   " + GetMainAttribAllBySubType(itemBase); //[C78100]
		}else{
			mMainAttrib.GetComponent<UILabel>().text = "";
		}
		
		//CompareRootUpdate(Control("BaseAttribRoot").transform.Find("LabelMainStrengthDes").gameObject,
		//		itemBase,AttribType.MainAll);
		// GS
		//CompareRootUpdate(Control("GSRoot").transform.Find("GSStrengthDes").gameObject,
		//		itemBase,AttribType.GScore);
		Control("GSLabel").GetComponent<UILabel>().text = itemBase.Score.ToString();	
	}

	
	GameObject Control(string name)
    {
        return Control(name, mGoIconTipsRoot);
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

	public void Destroy()
	{
		if(mGoIconTipsRoot != null)
			GameObject.Destroy(mGoIconTipsRoot);
		mGoIconTipsRoot = null;
	}
	
}

