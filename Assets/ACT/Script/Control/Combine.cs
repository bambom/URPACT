using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Combine
{
	public string PrefabName { get { return "CombineWnd"; } }
	
	GameObject mGoCombine;
	GameObject mGoResbarRoot;
	ResourceBar mResourceBar;
	GameObject mDestPanel;
	GameObject mSourcePanel;
	
	public delegate void Success();
	public Success CombineSuccess;
	
	//ID
	int mStoneID;	
	//BaseID 
	int mTargetStoneBaseID;
	
	bool mAnimatProgress = false; // true Combine
	bool mPackBackSync = false;   // true Sync
	List<UISlider> mProcessSlider = new List<UISlider>();
	SliderProgress mSProgress =  SliderProgress.Invalid;
	
	GameObject mExtBackground;
	GameObject mAnchor;
	
	public Combine(int StoneID)
	{
		mAnchor = GameObject.Find("Anchor");
		mExtBackground = GameObject.Instantiate(Resources.Load("BackgroundExtWnd")) as GameObject;
		mExtBackground.transform.parent = mAnchor.transform;
		mExtBackground.transform.localPosition = Vector3.zero + new Vector3(0.0f, 0.0f, 800.0f);
		mExtBackground.transform.localScale = Vector3.one + new Vector3(1500.0f, 1500.0f, 1.0f);
		
		mStoneID = StoneID;
		if(mGoCombine == null){
			mGoCombine = GameObject.Instantiate(Resources.Load(PrefabName)) as GameObject;
	        GameObject RootUI = GameObject.Find("Anchor");
	        mGoCombine.transform.parent = RootUI.transform;
	        mGoCombine.transform.localPosition = Vector3.zero; // + new Vector3(0.0f,0.0f,-60.0f);
	        mGoCombine.transform.localScale = Vector3.one;
			mSProgress = SliderProgress.End;
			mGoCombine.AddComponent<ControlHelper>().ControlHelpUpdate = CombineUpdate;
		}
		
		Init();
		SliderProgressInit();
		ResourceBarInit();
		
		UIGuideShow GShow = new UIGuideShow(PrefabName);
		if(GShow.GuideCheck()){
			GShow.GuideStart();
		}
	}
	
	void OnClickedAddMoney(GameObject go)
	{
		if(PackageWnd.Exist)
		{
			//PackageWnd.Instance.Show();
			PackageWnd.Instance.Close();
		}
		
		if(mExtBackground)
		{
			GameObject.Destroy(mExtBackground);
			mExtBackground = null;
		}
		
		if(mGoCombine){
			GameObject.Destroy(mGoCombine);
			mGoCombine = null;
		}
		
	}
	
	void OnClickedAddPhysic(GameObject go)
	{
		
		
	}
	
	void ResourceBarInit()
	{
		mGoResbarRoot = Control("ResBarRoot");
	 	MainAttrib attrib = PlayerDataManager.Instance.Attrib;
		mResourceBar = new ResourceBar(mGoResbarRoot);
		mResourceBar.ClickedAddMoney = OnClickedAddMoney;
		mResourceBar.ClickedAddPhysic = OnClickedAddPhysic;
		mResourceBar.Update(attrib.Gold,attrib.Gem,attrib.SP,-1);
		mResourceBar.UpdateByResType(ResourceBar.ResType.RT_NonPhysic);
	}
	
	void InitItemBefore(Item item)
	{
		Control("EquipPanelicon",mSourcePanel).GetComponent<UISprite>().spriteName = item.ItemBase.Icon;
		GameObject goCount = Control("SourceCountPanel");
		Control("CountNum",goCount).GetComponent<UILabel>().text = item.Num.ToString();
		Control("NameLabel",mSourcePanel).GetComponent<UILabel>().text = item.ItemBase.Name;
		Control("MainAttribLabel",mSourcePanel).GetComponent<UILabel>().text = item.ItemBase.Desc;
	}
	
	void UpdateItemBefore(Item item)
	{
		GameObject goCount = Control("SourceCountPanel");
		if(item != null)
			Control("CountNum",goCount).GetComponent<UILabel>().text = item.Num.ToString();
		else
			Control("CountNum",goCount).GetComponent<UILabel>().text = " ";
	}
	
	void InitItemAfter(Item item)
	{
		CombineBase Cb = CombineBaseManager.Instance.GetItem(item.Base);
		mTargetStoneBaseID = Cb.Target;
		ItemBase CombineItem = ItemBaseManager.Instance.GetItem(mTargetStoneBaseID);
		Control ("EquipPanelicon",mDestPanel).GetComponent<UISprite>().spriteName  = CombineItem.Icon;		
		
		Item tmpItem = ItemGetByBaseID(mTargetStoneBaseID);
		GameObject goCount = Control("CountPanel");
		if(tmpItem != null){
			Control("CountNum",goCount).GetComponent<UILabel>().text = tmpItem.Num.ToString();
		}else{
			Control("CountNum",goCount).GetComponent<UILabel>().text = " ";
		}
		
		Control("NameLabel",mDestPanel).GetComponent<UILabel>().text = CombineItem.Name;
		Control("MainAttribLabel",mDestPanel).GetComponent<UILabel>().text = CombineItem.Desc;
	}
	
	void UpdateItemAfter()
	{
		Control("CostGold",mGoCombine).GetComponent<UILabel>().text = "";
		Item tmpItem = ItemGetByBaseID(mTargetStoneBaseID);
		GameObject goCount = Control("CountPanel");
		if(tmpItem != null)
			Control("CountNum",goCount).GetComponent<UILabel>().text = tmpItem.Num.ToString();
		else {
			Control("CountNum",goCount).GetComponent<UILabel>().text = " ";
		}
		UpdateStone4To5();	
	}
	
	void InitStone4To5()
	{
		Item StoneSpecial = ItemGetByBaseID((int) StoneBaseID.StoneLevel4To5);
		if(StoneSpecial != null){
			Item tmpItem = ItemGet(mStoneID);
			CombineBase Cb = CombineBaseManager.Instance.GetItem(tmpItem.Base);
			int NeedNum = Cb.ItemNum;
			Control ("DropTex",Control ("GridItemStrong4")).GetComponent<UISprite>().spriteName = StoneSpecial.ItemBase.Icon;
			Control ("CountNum",Control ("GridItemStrong4")).GetComponent<UILabel>().text = StoneSpecial.Num.ToString() + "/" + NeedNum.ToString();
		}else{
			Control ("CountNum",Control ("GridItemStrong4")).GetComponent<UILabel>().text = "";
		}	
	}
	
	void UpdateStone4To5()
	{
		if(mTargetStoneBaseID != (int)StoneBaseID.StoneLevel5 )
			return ;
		Item StoneSpecial = ItemGetByBaseID((int) StoneBaseID.StoneLevel4To5);
		if( StoneSpecial != null ){
			Control ("DropTex",Control ("GridItemStrong4")).GetComponent<UISprite>().spriteName = StoneSpecial.ItemBase.Icon;
			Control ("CountNum",Control ("GridItemStrong4")).GetComponent<UILabel>().text = StoneSpecial.Num.ToString();
		}else{
			if(Control ("StrongPanel").transform.Find("GridItemStrong4").gameObject.activeSelf)
				Control ("CountNum",Control("GridItemStrong4")).GetComponent<UILabel>().text = "0";
		}	
	}
	
	void Init()
	{	
		Item tmpItem = ItemGet(mStoneID);
		for(int i = 0;i< 4;i++){
			Control ("DropTex",Control ("GridItemStrong"+i)).GetComponent<UISprite>().spriteName = tmpItem.ItemBase.Icon;
		}
		
		if( tmpItem.ItemBase.ID == (int) StoneBaseID.StoneLevel4 ){
			Control ("StrongPanel").transform.Find("GridItemStrong4").gameObject.SetActive(true);
 			InitStone4To5();
		}else{
			Control ("StrongPanel").transform.Find("GridItemStrong4").gameObject.SetActive(false);
			//Control ("CountNum",Control ("GridItemStrong4")).GetComponent<UILabel>().text = "";
		}
		
		mSourcePanel =  Control("SourcePanel");	
		mDestPanel = Control("EquipPanel");
		
		InitItemBefore(tmpItem);
		InitItemAfter(tmpItem);

		CombineBase Cb = CombineBaseManager.Instance.GetItem(tmpItem.Base);
		Control("CostGold",mGoCombine).GetComponent<UILabel>().text = (Cb.Gold *CheckCombineTimes()).ToString();
		UIEventListener.Get(Control("BtnCancle")).onClick = CombineCancleOnClick;
		UIEventListener.Get(Control("BtnSure")).onClick = CombineSureOnClick;		
	}
	
	void SliderProgressInit()
	{
		string str0 = "ProcessSlider0";
		for(int i=1; i<=4; i++){
			mProcessSlider.Add( Control(str0 + i).GetComponent<UISlider>() );
			mProcessSlider[i-1].sliderValue = 0.0f;
		}
		
		string str1 = "ProcessSlider1";
		for(int i=1; i<=4; i++){
			mProcessSlider.Add( Control(str1 + i).GetComponent<UISlider>() );
			mProcessSlider[i+4-1].sliderValue = 0.0f;
		}
		int index = mProcessSlider.Count;
		mProcessSlider.Add(Control("ProcessSlider25").GetComponent<UISlider>());
		mProcessSlider[index].sliderValue = 0.0f;
	}
	
	void OnCombineSuccess()
	{
		mAnimatProgress = true;
		mSProgress = SliderProgress.Process;
		RequestAttribInfo();
		RequestPackageInfo(false);      
	}
	
	int CheckCombineTimes()
	{
		int time = 0;
		Item item = ItemGet(mStoneID);
		if(item == null)
			return 0;
		
		time = ( item.Num /4 );
		if( mTargetStoneBaseID == (int)StoneBaseID.StoneLevel5 )
		{
			Item StoneSpecial = ItemGetByBaseID((int) StoneBaseID.StoneLevel4To5);	
			if(StoneSpecial == null)
			{
				MessageBoxWnd.Instance.Show("NotEnoughStoneLevel4To5",MessageBoxWnd.StyleExt.Alpha);
				return 0;
			}
			else
			{
				CombineBase Cb = CombineBaseManager.Instance.GetItem(item.Base);
				int NeedNum = Cb.ItemNum;
				if( StoneSpecial.Num / NeedNum < time)
					time = StoneSpecial.Num / NeedNum;
			}
		}
		return time;
	}
	
	public void CombineSureOnClick(GameObject go) 
	{
		//For Test
//		mAnimatProgress  = true;
//		return;		
		
		if(mSProgress != SliderProgress.End) { return; }
		Item tmpItem = ItemGet(mStoneID);
		
		if(tmpItem == null)
			return;
		
		if(tmpItem.ItemBase.MainType  != (int)EItemType.Stone){
			return;
		}
		
        int time = 1; // default is 1 ,real value need to check Gold And StoneNum
		if( tmpItem.Num >=4 ){
			time = CheckCombineTimes();
			mSProgress = SliderProgress.Start;
			SoundManager.Instance.PlaySound("UI_HeCheng");
            MainScript.Instance.Request(new CombineCmd(tmpItem.ID, time), delegate(string err, Response response)
	        {
	            if (!string.IsNullOrEmpty(err))
	            {
					MessageBoxWnd.Instance.Show( err ,MessageBoxWnd.StyleExt.Alpha );
					mSProgress = SliderProgress.End;
	                return;
	            }
	            OnCombineSuccess();
            });	
		}else{
			MessageBoxWnd.Instance.Show("NotEnoughStone",MessageBoxWnd.StyleExt.Alpha);
			//Close();
		}
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
	
	Item ItemGet(int ItemID)
	{
		Item item = PlayerDataManager.Instance.BackPack.Find(ItemID);
		return item;
	}
	
	public void CombineCancleOnClick(GameObject go)
	{	
		Close();
		if(PackageWnd.Exist)
		{
			PackageWnd.Instance.Show();
			PackageWnd.Instance.Update();
		}
		if(SkillWnd.Exist)
		{
			SkillWnd.Instance.Show();
			SkillWnd.Instance.UpdateRuneData();
		}
	}
	
	void CombineSuccessed()
	{
		mAnimatProgress = false;
		mPackBackSync = false;
		
		Item item = ItemGet(mStoneID);
		UpdateItemBefore(item);
		UpdateItemAfter();
		CreateSuccessEffect();
		SoundManager.Instance.PlaySound("UI_HeCheng_Success");
		MessageBoxWnd.Instance.Show("CombineSucess",MessageBoxWnd.StyleExt.Alpha);	    
		if(CombineSuccess != null)
			CombineSuccess();
	}
	
	void CreateSuccessEffect()
	{
		if(mDestPanel == null)
			return;		
		GameObject effect = GameObject.Instantiate(Resources.Load("SuccessEffect")) as GameObject;
		effect.transform.parent = mDestPanel.transform;
		effect.transform.localPosition = new Vector3(0.0f,0.0f,-10.0f);
		effect.transform.localEulerAngles = Vector3.zero;
		effect.transform.localScale = Vector3.one;
	    float time = effect.GetComponentInChildren<ParticleSystem>().duration;
		GameObject.Destroy(effect,time);
	}
	
	void CombineUpdate()
	{
		if( mAnimatProgress ){
			
			//float dt = 
			mProcessSlider[0].sliderValue += Time.deltaTime;
			mProcessSlider[2].sliderValue += Time.deltaTime/2;
			
			if( mProcessSlider[0].sliderValue == 1.0f){
				mProcessSlider[1].sliderValue += Time.deltaTime;
			}
			if( mProcessSlider[2].sliderValue == 1.0f){
				mProcessSlider[3].sliderValue += Time.deltaTime;
			}
									
			mProcessSlider[4].sliderValue += Time.deltaTime;
			mProcessSlider[6].sliderValue += Time.deltaTime/2;
			
			if( mProcessSlider[4].sliderValue == 1.0f){
				mProcessSlider[5].sliderValue += Time.deltaTime;
			}
			
			if( mProcessSlider[6].sliderValue == 1.0f){
				mProcessSlider[7].sliderValue += Time.deltaTime;
			}
			
			if(mProcessSlider[7].sliderValue == 1.0f){
				mProcessSlider[8].sliderValue += Time.deltaTime;
			}		
			
			if( mPackBackSync 
				&& mProcessSlider[8].sliderValue >= 1.0f)
			{
				CombineSuccessed();
			}
		}
	}
	
	
	GameObject Control(string name)
    {
        return Control(name, mGoCombine);
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
	
	void Close()
	{
		if(mExtBackground)
		{
			GameObject.Destroy(mExtBackground);
			mExtBackground = null;
		}
		
		if(PackageWnd.Exist)
			PackageWnd.Instance.Show();
		
		if(mGoCombine)
		{
			GameObject.Destroy(mGoCombine);
			mGoCombine = null;
		}
	}

	void RequestPackageInfo(bool updateEquip)
	{
        MainScript.Instance.Request(new GetBackPackCmd(), delegate(string err, Response response)
        {
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
				mPackBackSync = true;
                //Global.ShowLoadingEnd();
            }
        });
		//Global.ShowLoadingStart();
	}
	
	void RequestAttribInfo()
	{
        MainScript.Instance.Request(new GetUserAttribCmd(), delegate(string err, Response response)
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
			if(mGoCombine != null)
				mResourceBar.Update(attrib.Gold,attrib.Gem,attrib.SP,-1);
        });
		//Global.ShowLoadingStart();
	}
	
}

