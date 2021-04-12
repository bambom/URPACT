using UnityEngine;
using System.Collections;

public class Strengthen : ControlCommon
{
	public string PrefabName { get { return "StrengthenWnd"; } }
	
	GameObject mGoStrengthen;
	GameObject mGoResbarRoot;
	UISlider mStrengthenSlider;
	UISlider mProcessSlider;
	ResourceBar mResourceBar;
			
	GameObject mBefore;
	GameObject mAfter;
	
	public delegate void Success();
	public Success StrengthenSuccess;
	
	int mItemID;
	int mStoneID;
	int mStoneIndexMax = -1; //Stone Have the most higher level Index;
	int mPreviewRotateID = 1;
	bool mAnimatProgress = false;
	bool mRotateMutex = false; // true  Rotating
	bool mPackBackSync = false; // After Strengthen Sync
	
	int mPreAttrib = 0;
	
	SliderProgress mSProgress =  SliderProgress.Invalid;
	public static string SelectStone = "SelectStone";
	
	GameObject mExtBackground ;
	GameObject mAnchor;
	
	public Strengthen(int ItemID)
	{
		mAnchor = GameObject.Find("Anchor");
		mExtBackground = GameObject.Instantiate(Resources.Load("BackgroundExtWnd")) as GameObject;
		mExtBackground.transform.parent = mAnchor.transform;
		mExtBackground.transform.localPosition = Vector3.zero + new Vector3(0.0f, 0.0f, 800.0f);
		mExtBackground.transform.localScale = Vector3.one + new Vector3(1500.0f, 1500.0f, 1.0f);
		
		mItemID = ItemID;
		if(mGoStrengthen == null){
			mGoStrengthen = GameObject.Instantiate(Resources.Load(PrefabName)) as GameObject;
	        GameObject RootUI = GameObject.Find("Anchor");
	        mGoStrengthen.transform.parent = RootUI.transform;
	        mGoStrengthen.transform.localPosition = Vector3.zero; // + new Vector3(0.0f,0.0f,-60.0f);
	        mGoStrengthen.transform.localScale = Vector3.one;
			
			mBefore = Control("EquipPanelBefore");
			mAfter = Control("EquipPanelAfter");
			mSProgress = SliderProgress.End;
			mGoStrengthen.AddComponent<ControlHelper>().ControlHelpUpdate = StrengthenUpdate;
		}
		ResourceBarInit();
		InitStone();
		Init();
		
		
		UIGuideShow UIGShow = new UIGuideShow(PrefabName);
		if(UIGShow.GuideCheck()){
			UIGShow.GuideStart();
		}
	}
	
	//
	void CheckStone()
	{
		if( PlayerPrefs.HasKey(SelectStone) )
		{
			string strObject = PlayerPrefs.GetString(SelectStone);
			if(!string.IsNullOrEmpty(strObject))
			{
				string nCount = Control(strObject).transform.Find("CountNo/LabelCount").GetComponent<UILabel>().text;
				if(0 != int.Parse(nCount)){
					SelectStoneClick( Control(strObject) );
					return;
				}
			}
		}
		SelectStoneClick( Control("GridItemStrong" + mStoneIndexMax.ToString()) );
		//SelectStoneClick( Control("GridItemStrong0") );
	}
	
	
	#region ResourceBar
	void OnClickedAddMoney(GameObject go)
	{
		if(PackageWnd.Exist)
			PackageWnd.Instance.Close();
		if(mExtBackground)
		{
			GameObject.Destroy(mExtBackground);
			mExtBackground = null;
		}
		
		if(mGoStrengthen){
			GameObject.Destroy(mGoStrengthen);
			mGoStrengthen = null;
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
	#endregion ResourceBar
	
	
	#region InitUI
	void Init()
	{
		Item item = GetItem(mItemID);
		if(item == null){
			Debug.Log("ItemInit ItemID is Wrong!");
		}
	
		InitItemBeforeInfo(item);
		InitItemAfterInfo();
		InitSlider(item);
		
		UpdateItemQualityFrameIcon(item.ItemBase.Quality,Control("Quality",mAfter));
		UpdateItemQualityFrameIcon(item.ItemBase.Quality,Control("Quality",mBefore));
		
		TargetAttrib Ta = TargetAttribManager.Instance.GetItem(item.ItemBase.MainType,item.ItemBase.SubType);
		Control ("EquipPanelicon",mBefore).GetComponent<UISprite>().spriteName = item.ItemBase.Icon; 
		Control ("EquipPanelicon",mAfter).GetComponent<UISprite>().spriteName 
				= "Button08_EquipItem_Bottom_0"+item.ItemBase.SubType; 
		
		Control ("DescStrength").GetComponent<UILabel>().text = "+ " + Ta.ShowName;
		UIEventListener.Get(Control("BtnCancle")).onClick = EquipStrengthCancleOnClick;
		UIEventListener.Get(Control("BtnSure")).onClick = EquipStrengthSureOnClick;	
	}
	
	void InitSlider(Item item)
	{
		//Attention float not int
		float StrengthenCount = item.ToEquipItem().GetAttrib(EquipAttrib.StrengthenCount);
		float CountMax = EquipBaseManager.Instance.GetItem(item.ItemBase.ID).StrengthenCount;
		Control("StrengthenCount").GetComponent<UILabel>().text
			= "("+StrengthenCount.ToString()+ "/" + CountMax.ToString() +")";	
		
		mStrengthenSlider = Control("StrengthenSlider").GetComponent<UISlider>();
		mProcessSlider = Control("ProcessSlider").GetComponent<UISlider>();
		mProcessSlider.sliderValue = 0.0f;
		mStrengthenSlider.sliderValue = StrengthenCount/CountMax;
	}
	
	void UpdateStrengthenCountSlider()
	{
		Item item = GetItem(mItemID);
		float StrengthenCount = item.ToEquipItem().GetAttrib(EquipAttrib.StrengthenCount);
		float CountMax = EquipBaseManager.Instance.GetItem(item.ItemBase.ID).StrengthenCount;
		Control("StrengthenCount").GetComponent<UILabel>().text
			= "("+StrengthenCount.ToString()+ "/" + CountMax.ToString() +")";	
		
		mStrengthenSlider = Control("StrengthenSlider").GetComponent<UISlider>();
		mStrengthenSlider.sliderValue = StrengthenCount/CountMax;
	}
	
	void InitStone()
	{
		string str = "GridItemStrong";
		
		for(int i = 0;i< 5;i++){
			int StoneID = int.Parse ( Control ("StoneIDLabel",Control (str+i)).GetComponent<UILabel>().text);
			if( i == 0 )
				mStoneIndexMax = i;
			Control ("DropTex",Control (str+i)).GetComponent<UISprite>().spriteName 
				= ItemBaseManager.Instance.GetItem(StoneID).Icon; 
			Control ("LabelCount",Control (str+i)).GetComponent<UILabel>().text 
				= GetPackStoneNumByItemBase(StoneID).ToString();
			if(0 == GetPackStoneNumByItemBase(StoneID)){
				Control ("DropTex",Control (str+i)).GetComponent<UISprite>().alpha = 0.5f;
			}else
				mStoneIndexMax = i;
			UIEventListener.Get( Control (str+i) ).onClick = SelectStoneClick;
		}	
		CheckStone();
	}
	
	void UpdateStone()
	{
		InitStone();
	}
	
	void InitItemAfterInfo()
	{
		Control ("NameLabel",mAfter).GetComponent<UILabel>().text = " ";
		Control ("MainAttribLabel",mAfter).GetComponent<UILabel>().text = " ";
	}
	
	void UpdateItemAfterInfo()
	{
		Item item = GetItem(mItemID);
		TargetAttrib Ta = TargetAttribManager.Instance.GetItem(item.ItemBase.MainType,item.ItemBase.SubType);
		Control ("NameLabel",mAfter).GetComponent<UILabel>().text = item.ItemBase.Name;
		int Increase = GetMainAttribAllBySubType(item);
		Control ("MainAttribLabel",mAfter).GetComponent<UILabel>().text = "+ " + Ta.ShowName + " : " + Increase;
		Control ("EquipPanelicon",mAfter).GetComponent<UISprite>().spriteName = item.ItemBase.Icon; 
		int delta = Increase - mPreAttrib;
		CreateSuccessEffect();
		SoundManager.Instance.PlaySound("UI_QiangHua_Success");
		MessageBoxWnd.Instance.Show(("StrengthenContent"),MessageBoxWnd.StyleExt.Alpha,Ta.ShowName,"[FFFF00]"+ delta +"[-]");	
	}
	
	void CreateSuccessEffect()
	{
		if(mAfter == null)
			return;		
		GameObject effect = GameObject.Instantiate(Resources.Load("SuccessEffect")) as GameObject;
		effect.transform.parent = mAfter.transform;
		effect.transform.localPosition = new Vector3(0.0f,0.0f,-10.0f);
		effect.transform.localEulerAngles = Vector3.zero;
		effect.transform.localScale = Vector3.one;
	    float time = effect.GetComponentInChildren<ParticleSystem>().duration;
		GameObject.Destroy(effect,time);
	}
	
	void InitItemBeforeInfo(Item item)
	{
		TargetAttrib Ta = TargetAttribManager.Instance.GetItem(item.ItemBase.MainType,item.ItemBase.SubType);
		Control ("NameLabel",mBefore).GetComponent<UILabel>().text = item.ItemBase.Name;
		mPreAttrib = GetMainAttribAllBySubType(item);
		Control ("MainAttribLabel",mBefore).GetComponent<UILabel>().text = "+ " + Ta.ShowName + " : " + GetMainAttribAllBySubType(item);
	}
	
	void UpdateItemBeforeInto(Item item)
	{
		InitItemBeforeInfo(item);
	}
	#endregion InitUI
		
	public void EquipStrengthSureOnClick( GameObject go )
	{		
		//For Test
//		mAnimatProgress  = true;
//		return;
		
		if( mSProgress != SliderProgress.End ){ return; }	
		if(mProcessSlider.sliderValue != 0.0f){
			mProcessSlider.sliderValue = 0.0f;
		}
		
		if( mStoneID == -1){
			MessageBoxWnd.Instance.Show("NotEnoughStone",MessageBoxWnd.StyleExt.Alpha);
			return ;
		}
		Item tmpItemStone = PlayerDataManager.Instance.BackPack.Find( mStoneID );
		if(tmpItemStone == null){
			MessageBoxWnd.Instance.Show("NotEnoughStone",MessageBoxWnd.StyleExt.Alpha);
			return ;
		}
			
		Item tmpItem = GetItem(mItemID);			
		UpdateItemBeforeInto(tmpItem);
	
		int StoneBaseID = tmpItemStone.ItemBase.ID;
		int price = GetForesGold(tmpItem,StoneBaseID);	
		
		if( price >  PlayerDataManager.Instance.Attrib.Gold){
			MessageBoxWnd.Instance.Show(("NotEnoughGold"),
				MessageBoxWnd.StyleExt.Alpha);
			return ;
		}
		
		
		if( ( EquipBaseManager.Instance.GetItem(tmpItem.ItemBase.ID).StrengthenCount 
				- tmpItem.ToEquipItem().GetAttrib(EquipAttrib.StrengthenCount)) > 0
			&& PlayerDataManager.Instance.BackPack.Find( mStoneID ).Num > 0 )
		{
			mSProgress = SliderProgress.Start;
			SoundManager.Instance.PlaySound("UI_QiangHua");
		    MainScript.Instance.Request(new StrengthenCmd(mItemID,mStoneID), delegate(string err, Response response)
	        {
	            if (!string.IsNullOrEmpty(err)) {
					if(Control("StrengthPanel(Clone)")!=null){
						GameObject.Destroy(Control("StrengthPanel(Clone)"));
					}
					MessageBoxWnd.Instance.Show( err,MessageBoxWnd.StyleExt.Alpha );
					mSProgress = SliderProgress.End;
	                return;
	            }
	            OnStrengthSuccess();
            });	
		}else{

			MessageBoxWnd.Instance.Show( 
				("EquipAlreadyStrengthen"),
				MessageBoxWnd.StyleExt.Alpha );
			//Close();
		}
	}
	
	public void EquipStrengthCancleOnClick( GameObject go )
	{
		Close();
		if(PackageWnd.Exist){
			PackageWnd.Instance.Show();
			PackageWnd.Instance.Update();
		}
	}
	
	void SelectStoneClick( GameObject go )
	{
		if(mRotateMutex)
			return;
		
		int StoneBaseID = int.Parse ( Control ("StoneIDLabel",go).GetComponent<UILabel>().text);
		mStoneID = GetPackStoneIdByItemBase(StoneBaseID);
		//UpdateStrengthenItemFocus( go );
		UpdateStrengthenDescByClickStone(StoneBaseID);
		QuaternionUpdate(go);
	}
	
	void UpdateStrengthenItemFocus( GameObject go )
	{
		GameObject SubFocusItem = Control("SubItemFocus");
		SubFocusItem.transform.parent = go.transform.Find(go.name.Substring("GridItemStrong".Length));
		SubFocusItem.transform.localScale = new Vector3(80.0f,80.0f,0.0f);
		SubFocusItem.transform.localPosition = Vector3.zero;
	}

	int GetMainAttribAllBySubType(Item item)
	{
		switch(item.ItemBase.SubType){
			case (int)EquipSubType.EST_Weapon://武器
					return item.ToEquipItem().GetAttrib(EquipAttrib.Damage);
			case (int)EquipSubType.EST_Helmet://头盔
					return item.ToEquipItem().GetAttrib(EquipAttrib.HPMax);
			case (int)EquipSubType.EST_Armor://上衣
					return item.ToEquipItem().GetAttrib(EquipAttrib.Defense);
			case (int)EquipSubType.EST_Shoulder://肩甲
					return item.ToEquipItem().GetAttrib(EquipAttrib.HPMax);
			case (int)EquipSubType.EST_Shoe://鞋子
					return item.ToEquipItem().GetAttrib(EquipAttrib.SpecialDefense);
			case (int)EquipSubType.EST_NeckLace://项链
					return item.ToEquipItem().GetAttrib(EquipAttrib.Hit);
			case (int)EquipSubType.EST_WaistBand://腰带
					return item.ToEquipItem().GetAttrib(EquipAttrib.Block);
			case (int)EquipSubType.EST_Wrist://护腕
					return item.ToEquipItem().GetAttrib(EquipAttrib.SpecialDamage);
			case (int)EquipSubType.EST_Ring://戒指	
					return item.ToEquipItem().GetAttrib(EquipAttrib.Critical);
		}
		return 0;
	}
	
	int GetForesGold(Item item,int StoneBID)
	{
		ForgeGold Fs = ForgeGoldManager.Instance.GetItem(item.ItemBase.Level);
		switch(StoneBID)
		{
			case (int) StoneBaseID.StoneLevel1:return Fs.ForgeGold1;
			case (int) StoneBaseID.StoneLevel2:return Fs.ForgeGold2;
			case (int) StoneBaseID.StoneLevel3:return Fs.ForgeGold3;
			case (int) StoneBaseID.StoneLevel4:return Fs.ForgeGold4;
			case (int) StoneBaseID.StoneLevel5:return Fs.ForgeGold5;
		}
		return 0;
	}

	void UpdateStrengthenDescByClickStone(int StoneBaseID)
	{
		Item tmpItem = GetItem(mItemID);
		int price = GetForesGold(tmpItem,StoneBaseID);		
		TargetAttrib Ta = TargetAttribManager.Instance.GetItem(tmpItem.ItemBase.MainType,tmpItem.ItemBase.SubType);
		UILabel Desc = Control ("DescStrength").GetComponent<UILabel>();
		Control("Price").GetComponent<UILabel>().text = price.ToString();
		Desc.text = "+ " + Ta.ShowName + " : " + GetStoneAttribBonus(tmpItem,StoneBaseID);
	}
	
	string GetStoneAttribBonus(Item item,int StoneID)
	{
		ForgeBase FB = ForgeBaseManager.Instance.GetItem(StoneID);
		switch(item.ItemBase.SubType)
		{
			case (int)EquipSubType.EST_Weapon://武器
					return StringFormat(item.ToEquipItem().EquipBase.Damage,FB.Damage); 
			case (int)EquipSubType.EST_Helmet://头盔
					return StringFormat(item.ToEquipItem().EquipBase.HPMax,FB.HPMax); 
			case (int)EquipSubType.EST_Armor://上衣
					return StringFormat(item.ToEquipItem().EquipBase.Defense,FB.Defense); 
			case (int)EquipSubType.EST_Shoulder://肩甲
					return StringFormat(item.ToEquipItem().EquipBase.HPMax,FB.HPMax); 
			case (int)EquipSubType.EST_Shoe://鞋子
					return StringFormat(item.ToEquipItem().EquipBase.SpecialDefense,FB.SpecialDefense); 
			case (int)EquipSubType.EST_NeckLace://项链
					return StringFormat(item.ToEquipItem().EquipBase.Hit,FB.Hit); 
			case (int)EquipSubType.EST_WaistBand://腰带
					return StringFormat(item.ToEquipItem().EquipBase.Block,FB.Block); 
			case (int)EquipSubType.EST_Wrist://护腕
					return StringFormat(item.ToEquipItem().EquipBase.SpecialDamage,FB.SpecialDamage); 
			case (int)EquipSubType.EST_Ring://戒指	
					return StringFormat(item.ToEquipItem().EquipBase.Critical,FB.Critical); 
			default:
					return "";
		}
	}
	
	
	void OnStrengthSuccess()
	{	
		mAnimatProgress = true;
		mSProgress = SliderProgress.Process;
		RequestAttribInfo();
		RequestPackageInfo(true);
	}
		
	string StringFormat(int attrib,string str)
	{
		string[] strArray = str.Split('|');
		//strArray[0] = ( int.Parse(strArray[0])*attrib/100 ).ToString() + "%";
		//strArray[1] = ( int.Parse(strArray[1])*attrib/100 ).ToString() + "%";
		
		strArray[0] = ( int.Parse(strArray[0])*attrib/10000 ).ToString();
		strArray[1] = ( int.Parse(strArray[1])*attrib/10000 ).ToString();
		
		string strRet = strArray[0] + "~" + strArray[1];
		return strRet;
	}
	
	int GetPackStoneIdByItemBase(int ib)
	{
		foreach( Item item in PlayerDataManager.Instance.BackPack.Items.Values )
		{
			if( ib == item.Base )
				return (int)item.ID;
		}
		return -1;
	}
	
	
	int GetPackStoneNumByItemBase(int ib)
	{
		foreach( Item item in PlayerDataManager.Instance.BackPack.Items.Values )
		{
			if( ib == item.Base )
				return (int)item.Num;
		}
		return 0;
	}
	
	Item GetItem(int ItemID)
	{
		Item tmpItem = PlayerDataManager.Instance.BackPack.Find(ItemID);
		if(tmpItem == null){
			Debug.Log("ItemID is Wrong!");
			return null;
		}
		Debug.Log("tmpItem.nam :" + tmpItem.ItemBase.Name );
		return tmpItem;
	}
	
	void StrengthenSuccessed()
	{
		//Close();
		UpdateStone();
		UpdateStrengthenCountSlider();
		UpdateItemAfterInfo();
		mSProgress = SliderProgress.End;
		mAnimatProgress = false;
		mProcessSlider.sliderValue = 0.0f;
		mPackBackSync = false;
		if(StrengthenSuccess != null)
			StrengthenSuccess();
	}
	
	#region SliderProgress
	void StrengthenUpdate()
	{
		SilderUpdate();
	}
	
	void SilderUpdate ()
	{
		if(mAnimatProgress){
			if(mProcessSlider.sliderValue <= 1.0f){
				mProcessSlider.sliderValue += Time.deltaTime;
			}
		}
		
		if( mPackBackSync 
			&& (mProcessSlider.sliderValue >=1.0f) )
		{
			Debug.Log("StrengthenSuccessed");
			StrengthenSuccessed();
		}
	}
	#endregion SliderProgress
	
	
	#region Rotation
	void RotationBackAnimatin(float rotate)
	{
		string str = "GridItemStrong";
		Vector3 vSourcePos = Vector3.zero;
		Vector3 vDesPos = Vector3.zero;
		for(int i = 0;i< 5;i++){
			vSourcePos = Control(str+i).transform.localEulerAngles;
			vDesPos = vSourcePos;
			vDesPos.z = vSourcePos.z - rotate;
			Control(str+i).transform.localEulerAngles = vDesPos;
		}	
	}
	
	void TweenRotaFinish(UITweener tween)
	{
		mRotateMutex = false;
	}
	
	void RotationAnimation(GameObject go,float rotate)
	{
		TweenRotation Tr = go.GetComponent<TweenRotation>();
		if( Tr != null){
			GameObject.Destroy(Tr);
			Tr = null;
		}
		
		Tr = go.AddComponent<TweenRotation>();
		Tr.method = UITweener.Method.Linear;
		Tr.duration = 0.5f;
		
		float vPosz = go.transform.localEulerAngles.z;	
		Vector3 VRotation = new Vector3(0.0f,0.0f,vPosz);
		Tr.from = VRotation;
		Tr.to = new Vector3(0.0f,0.0f,vPosz+rotate);
		Tr.onFinished = TweenRotaFinish;
	}
	
	void QuaternionUpdate(GameObject go)
	{
		string str = "GridItemStrong";
		int nIndex = int.Parse(go.name.Substring(str.Length)) + 1;
		int nCount = nIndex - mPreviewRotateID;
		mPreviewRotateID = nIndex;
		mRotateMutex = true;
		float BaseRotate = 72.0f;
		float Rotate = nCount * BaseRotate;
		
		Debug.Log("QuaternionUpdate Rotate : " + Rotate.ToString());
		RotationAnimation(go.transform.parent.gameObject,Rotate);
		RotationBackAnimatin(Rotate);
		PlayerPrefs.SetString(SelectStone,go.name);
	}
	#endregion Rotation
	
	
	GameObject Control(string name)
    {
        return Control(name, mGoStrengthen);
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
		
		if(mGoStrengthen)
		{
			GameObject.Destroy(mGoStrengthen);
			mGoStrengthen = null;
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
				Debug.Log("mPackBackSync = true");
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
			if(mGoStrengthen != null)
				mResourceBar.Update(attrib.Gold,attrib.Gem,attrib.SP,-1);
        });
		//Global.ShowLoadingStart();
	}
}

