using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class UIGuideNode
{
	public bool bFinish;
	public List<UIGuide> GuideList;
}


public class UIGuideShow
{
	GameObject mGoUIGuideRoot;
	static Transform mCloneControl;
	
	int mCurGuideID;
	public int CurGuideID { get { return mCurGuideID ;} set { mCurGuideID = value; } }
	
	static int mCurStep;
	public static int CurStep { get { return mCurStep ;} set { mCurStep = value; } }
	static string mCurWnd;
	public static string CurWnd { get { return mCurWnd ;} set { mCurWnd = value; }  }
	
	
	static int mMaxStep ;
	public static int MaxStep { get { return mMaxStep ;} set { mMaxStep = value; } }

	static GameObject mSourceControl;
	public static GameObject SourceControl { get{ return mSourceControl;}}
	public static GameObject CloneControl {get { return mCloneControl.gameObject;}}
	List<UIGuide> mGuideList = new List<UIGuide>();
	List<UIGuide> mCurGuideList = new List<UIGuide>();
	
	Dictionary<int,UIGuideNode> mDicUIGuideNodes = new Dictionary<int, UIGuideNode>();
	
	int mCurListIndex;
	static UIGuide mCurUIGuide;
	public static UIGuide CurUIGuide { get { return mCurUIGuide ;}}
	
	static bool mbMutex = false;
	public static bool Mutex { get { return mbMutex; } set{ mbMutex = value;}}
	
	float mDeltaTime = 0.0f;
	
	public UIGuideShow(string CurWnd)
	{
		GameObject goRoot = GameObject.Find("UIGuideShow");
		if(null != goRoot){
			GameObject.Destroy(goRoot);
		}
		mCurWnd = CurWnd;
		mGoUIGuideRoot = new GameObject("UIGuideShow");
		mGoUIGuideRoot.AddComponent<ControlHelper>().ControlHelpUpdate = GuideUpdateCallBack;
	}
	
	void UpdateGuideList(int GuideID)
	{
		if(mGuideList.Count > 0)
			mGuideList.Clear();
		
		UIGuide[] Ug = UIGuideManager.Instance.GetAllItem();
		for(int i = 0; i<Ug.Length; i++){
			if(Ug[i].GuideID == GuideID)
				mGuideList.Add(Ug[i]);
		}
	}
	
	bool CheckGuideBit(int index)
	{
		MainAttrib Attrib = PlayerDataManager.Instance.Attrib;
		return (Attrib.NewGuide & (1 << index)) == 0;
	}
	
	void Init()
	{
		if(mGuideList.Count > 0)
			mGuideList.Clear();
		
		UIGuide[] Ug = UIGuideManager.Instance.GetAllItem();
		for(int i = 0; i<Ug.Length; i++){
			if(Ug[i].GuideID == mCurGuideID)
				mGuideList.Add(Ug[i]);
		}
		//mGuideList.Sort((x, y) => x.Priority.CompareTo (y.Priority));
		mMaxStep = mGuideList[0].Counts;
	}
	
	public void Hide()
	{
		if(null != mGoUIGuideRoot)
		{
			mGoUIGuideRoot.SetActive(false);
		}
	}
	
	
	public void Destroy()
	{
		if(null != mGoUIGuideRoot){
			GameObject.Destroy(mGoUIGuideRoot);	
			mGoUIGuideRoot = null;
		}
		if( GuideWnd.Exist )
			GuideWnd.Instance.Close();
	}
	
	Item ItemGetByBaseID(int ID)
	{
		foreach(Item item in PlayerDataManager.Instance.BackPack.Items.Values)
		{
			if(item.ItemBase.ID == ID)
				return item;
		}
		return null;
	}
	
	bool ExtBtnCheck( UIGuide UiG )
	{
		if( 0 == string.Compare(UiG.CurWnd,"InGameMainWnd") && mCurStep == 1)
		{
			//NGUIDebug.Log("InGameMainWnd.UIExtStatus = " + InGameMainWnd.UIExtStatus );
			return InGameMainWnd.UIExtStatus;
		}
		return false;
	}
		
	bool TriggerCheck(UIGuide UiG)
	{
		MainAttrib Attrib = PlayerDataManager.Instance.Attrib;
		if(Attrib.Progress < UiG.Progress)
			return false;
		
		if(Attrib.Level < UiG.Level)
			return false;
		
		if(Attrib.SP < UiG.SP)
			return false;
		
		if( !string.IsNullOrEmpty(UiG.Item) )
		{
			int ItemID = 0;
			Item item = null;
			if(UiG.Item.Contains("|"))
			{
				string[] baseID = UiG.Item.Split('|');	
				if(UiG.GuideID == 2)
				{
					//Special
					ItemID = int.Parse( baseID[ Attrib.Role - (int)RoleID.Warrior] );
					item = ItemGetByBaseID( ItemID );
					if( item == null )
						return false;
					if( item.Num < UiG.Trigger )
						return false;
				}
				else
				{
					//Common
					for(int i = 0; i<baseID.Length; i++)
					{
						ItemID = int.Parse( baseID[i] );
						item = ItemGetByBaseID( ItemID );
						if( item != null)
						{
							if( item.Num >= UiG.Trigger )
							{
								return true;
							}
						}
					}
					return false;
				}
			}
			else
			{
				ItemID = int.Parse( UiG.Item );
				item = ItemGetByBaseID( ItemID );
				if( item == null )
					return false;
				if( item.Num < UiG.Trigger )
					return false;
			}
		}
		return true;
	}
	
	int GuideCountGet()
	{
		int nCount = 0;
		UIGuide[] Ug = UIGuideManager.Instance.GetAllItem();
		for(int i = 0; i<Ug.Length; i++)
		{
			if( Ug[i].GuideID > nCount )
				nCount = Ug[i].GuideID;
		}
		return nCount;
	}
	
	void GuideNodesInit()
	{
		int nCount = GuideCountGet();
		if(mDicUIGuideNodes.Count > 0)
			mDicUIGuideNodes.Clear();
		for( int j=0; j<nCount; j++ )
		{
			UIGuideNode uGiNode = new UIGuideNode();
			uGiNode.bFinish = false;
			uGiNode.GuideList = new List<UIGuide>();
			
			UIGuide[] Ug = UIGuideManager.Instance.GetAllItem();
			for(int i = 0; i<Ug.Length; i++)
			{
				if( Ug[i].GuideID == j+1 )
					uGiNode.GuideList.Add(Ug[i]);
			}
			uGiNode.GuideList.Sort((x, y) => ((int)x.Steps).CompareTo (((int)y.Steps)));	
			mDicUIGuideNodes.Add(j,uGiNode);
		}

		for( int i = 0; i< nCount ; i++ )
		{
			if( CheckGuideBit(i) )
			{
				mDicUIGuideNodes[i].bFinish = false;
			}else{
				mDicUIGuideNodes[i].bFinish = true;
			}
		}
	}
	
	
	bool GuideNodesCheck()
	{
		GuideNodesInit();
		for(int i=0; i<mDicUIGuideNodes.Count; i++)
		{
			if ( mDicUIGuideNodes[i].bFinish == false )
			{
				if( TriggerCheck( mDicUIGuideNodes[i].GuideList[0] ) == true )
				{
					mCurGuideID = i+1 ;
					Global.CurGuideID = mCurGuideID; 
					mGuideList = mDicUIGuideNodes[i].GuideList;
					return true;
				}
			}
		}
		return false;
	}
	
	public bool GuideCheck()
	{
		if(MainScript.Instance.GmMode == EGameMode.GMDebug){
			return false;
		}
		
		//
		MainAttrib Attrib = PlayerDataManager.Instance.Attrib;
		if(Attrib.Level >= 10)
			return false;
			
		mCurStep = Global.CurStep ;
		if( GuideNodesCheck() == false )
			return false;
		
		if( 0 != string.Compare(mGuideList[mCurStep -1].CurWnd,mCurWnd))
		{
			Global.CurStep = 1;
			mCurStep = 1;
			return false;
		}
		
		for(int i = 0; i < mGuideList.Count; i++){
			if(mGuideList[i].Steps == mCurStep){
				mCurListIndex = i ;
				break;
			}
		}
		
		mCurUIGuide = UIGuideManager.Instance.GetItem(mCurGuideID,mCurStep);
		
		if( TriggerCheck( mGuideList[mCurListIndex] ) == false )
			return false;
		
		// True Extend
		if( ExtBtnCheck( mGuideList[mCurListIndex] ) == true )
		{
			Global.CurStep = 2;
			mCurStep = 2;
			mCurListIndex = 1;
			mCurUIGuide = UIGuideManager.Instance.GetItem(mCurGuideID,mCurStep);
		}
		
		mMaxStep = mGuideList[0].Counts;
		return true;
	}
	
	public void GuideStart()
	{
		if(!GuideWnd.Exist)
			GuideWnd.Instance.Open();
		
		bool ret = CloneUIControl(mGuideList[mCurListIndex]);
		GuideWnd.Instance.Update(CloneControl.gameObject,mGuideList[mCurListIndex]);
	}
	
	void GuideUpdate(int GuideID,int Step)
	{
		Debug.Log("GuideUpdate" + Step);
		if(!GuideWnd.Exist)
			GuideWnd.Instance.Open();

		for(int i = 0; i < mGuideList.Count; i++){
			if(mGuideList[i].Steps == Step){
				mCurListIndex = i ;
				break;
			}
		}
		bool ret = CloneUIControl(mGuideList[mCurListIndex]);
		GuideWnd.Instance.Update(CloneControl.gameObject,mGuideList[mCurListIndex]);
	}
	
	void GuideUpdateCallBack()
	{
		mCurStep = Global.CurStep;
		//Debug.Log("GuideUpdateCallBack  mCurStep :" + mCurStep);
		
		if(mCurStep == -1)
			return ;
		if(!mbMutex)
			return;
		
		if(mCurStep <= mMaxStep){
			mCurUIGuide = UIGuideManager.Instance.GetItem(mCurGuideID,mCurStep);			
			mDeltaTime += Time.deltaTime;
			if( mDeltaTime < (mCurUIGuide.Wait/2.0f) ){
				return;
			}else{
				mbMutex = false;
				mDeltaTime = 0.0f;
				GuideUpdate(mCurGuideID,mCurStep);
			}
		}
	}
	
	void CloneControlIsConPathNull ( UIGuide UiG )
	{
		if( UiG.GuideID == 2 )
		{
			List<PackageItem> List = PlayerDataManager.Instance.BackPack.PackSortList;
			int Count = List.Count;
			for( int i=0; i<Count; i++ )
			{
				if( List[i].PackItm.ItemBase.MainType == (int) EItemType.Equip 
					&& !List[i].PackItm.ToEquipItem().Equiped )
				{
					mSourceControl = GameObject.Find( UiG.ControlName + "PackBaseItem" + i );
					break;
				}
			}
		}
		else if( UiG.GuideID == 4 ) 
		{
			if(UiG.Steps == 3)
			{
				mSourceControl = GameObject.Find( UiG.ControlName + "PackBaseItem0");
			}
		}
		else if( UiG.GuideID == 5 )
		{
			if (UiG.Steps == 3) 
			{
				List<PackageItem> List = PlayerDataManager.Instance.BackPack.PackSortList;
				int Count = List.Count;
				for( int i=0; i<Count; i++ )
				{
					if( List[i].PackItm.ItemBase.MainType == (int) EItemType.Stone )
					{
						mSourceControl = GameObject.Find( UiG.ControlName + "PackBaseItem" + i );
						break;
					}
				}
			}
		}
	}
	
	public bool CloneUIControl(UIGuide UiG)
	{
		bool ret = false;
		Debug.Log("CloneUIControl : " + UiG.ControlName);
		if ( UiG.ControlType == 1 )
		{
			CloneControlIsConPathNull(UiG);	
		}else
			mSourceControl = GameObject.Find(UiG.ControlName);   
		
		if(mSourceControl == null){
			Debug.Log("mSourceControl " + UiG.ControlName);
			return ret;
		}
		
		Transform DesGo = mSourceControl.transform;
		if ( mCloneControl != null ){
			mCloneControl.gameObject.SetActive(false);
			Debug.Log("Destroy CloneControl :" + mCloneControl.gameObject.name);
			GameObject.Destroy(mCloneControl.gameObject);
			ret = true;
		}
		
        mCloneControl = GameObject.Instantiate(DesGo) as Transform;
        mCloneControl.parent = DesGo.parent;
        mCloneControl.localPosition = DesGo.localPosition;
        mCloneControl.localRotation = DesGo.localRotation;
        mCloneControl.localScale = DesGo.localScale;		
		GuideClickFeedBack GCFB = mCloneControl.gameObject.GetComponent<GuideClickFeedBack>();
		if(GCFB == null)
			GCFB = mCloneControl.gameObject.AddComponent<GuideClickFeedBack>();
		BoxCollider BoxCol = mCloneControl.gameObject.GetComponent<BoxCollider>();
		if(BoxCol == null){
			BoxCol = mCloneControl.gameObject.AddComponent<BoxCollider>();
			BoxCol.isTrigger = true;
			BoxCol.size = DesGo.localScale + new Vector3(0.0f,0.0f,60.0f);
		}else{
			BoxCol.size += new Vector3(0.0f,0.0f,60.0f);
		}
		return ret;
	}
}

