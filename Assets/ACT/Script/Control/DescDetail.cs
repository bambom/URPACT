using UnityEngine;
using System.Collections;


public enum DescType
{
	AttribAll,
	AttribOne
};

public enum AttribType{
	GScore,
	MainAll,
	MainBase,
	MainExtra,
	StrengthenCount
}

public class DescDetail : ControlCommon
{
	public string PrefabOneDescName { get { return "PackAttribOne"; } }
	public string PrefabAllDescName { get { return "PackAttribAll"; } }
	
	private GameObject mGoDescDetail;
	private GameObject mGoDescParent;
	private DescType mDescType;
	public DescType DscType {get{return mDescType;}}  
	 
	private UILabel mTitleName;
	private UILabel mContentDesc;
	private UILabel mMainAttrib;
	private GameObject mExpSlider;
	private bool mbMutexChangeStyle = false;
	
	MainAttrib mPlayerMainAttrib;
	
	public DescDetail(DescType DType,MainAttrib Attrib,GameObject goRoot,bool bChangeStyle)
	{
		mGoDescParent = goRoot;
		if(null == mGoDescDetail){
			mDescType = DType;
			if(DType == DescType.AttribOne)
				mGoDescDetail = GameObject.Instantiate(Resources.Load(PrefabOneDescName)) as GameObject;
			else if(DType == DescType.AttribAll)
				mGoDescDetail = GameObject.Instantiate(Resources.Load(PrefabAllDescName))as GameObject; 
		}
		mPlayerMainAttrib = Attrib;
		
		mbMutexChangeStyle = bChangeStyle;
		
		mGoDescDetail.name = "GoDescDetail";
		mGoDescDetail.transform.parent = mGoDescParent.transform;
		mGoDescDetail.transform.localScale = Vector3.one;
		mGoDescDetail.transform.localPosition = Vector3.zero;
	}

	void DropDownAnimation()
	{
		TweenPosition Tp = mGoDescDetail.GetComponent<TweenPosition>();
		if( Tp != null){
			GameObject.Destroy(Tp);
			Tp = null;
		}
		
		mGoDescDetail.transform.localPosition = new Vector3(0.0f,560.0f,0.0f);
		Tp = mGoDescDetail.AddComponent<TweenPosition>();
		Tp.method = UITweener.Method.EaseIn;
		Tp.duration = 0.5f;
		Tp.from = new Vector3(0.0f,560.0f,0.0f);
		Tp.to = Vector3.zero;
	}
	
	public void Init(Item item)
	{
		if(mbMutexChangeStyle){
			DropDownAnimation();
			mbMutexChangeStyle = false;
		}
		
		switch(mDescType){
			case DescType.AttribAll:InitAll();break;
			case DescType.AttribOne:InitOneItem(item);break;
			default:break;
		}
	}
	
	void CompareRootUpdate(GameObject CompareRoot,Item item,AttribType AType)
	{
		if( CheckEuipHasBaseAttrib(item.ItemBase) ){
			if(item.ToEquipItem().Equiped){
				if(CompareRoot.activeSelf)
					CompareRoot.SetActive(false);
				return;
			}
			int Delta = CompareWithTwoEquip(item,AType);  
			
			if(!CompareRoot.activeSelf)
				CompareRoot.SetActive(true);	
			
			if(Delta > 0){
				CompareRoot.transform.Find("Label").GetComponent<UILabel>().text
					= "[00FF00]([-]";
				CompareRoot.transform.Find("UpOrDown").GetComponent<UISprite>().spriteName
					= "Button05_Package_Compare_01";
				CompareRoot.transform.Find("RightLabel").GetComponent<UILabel>().text
					= "[00FF00]" + Mathf.Abs(Delta).ToString() + ")[-]";			
			}else if(Delta < 0){
				CompareRoot.transform.Find("Label").GetComponent<UILabel>().text
					= "[FF0000]([-]";
				CompareRoot.transform.Find("UpOrDown").GetComponent<UISprite>().spriteName
					= "Button05_Package_Compare_02";
				CompareRoot.transform.Find("RightLabel").GetComponent<UILabel>().text
					= "[FF0000]" +Mathf.Abs(Delta).ToString() + ")[-]";			
			}else {
				if(CompareRoot.activeSelf)
					CompareRoot.SetActive(false);	
			}
			
		}else{
			if(CompareRoot.activeSelf)
				CompareRoot.SetActive(false);	
		}
	}
	
	//For NpcShop NeverEquiped
	void CompareRootUpdate(GameObject CompareRoot,ItemBase itemBase,AttribType AType)
	{
		if( CheckEuipHasBaseAttrib(itemBase) ){
			
			int Delta = CompareWithTwoEquip(itemBase,AType);  
			if(!CompareRoot.activeSelf)
				CompareRoot.SetActive(true);	
			
			if(Delta > 0){
				CompareRoot.transform.Find("Label").GetComponent<UILabel>().text
					= "[00FF00]([-]";
				CompareRoot.transform.Find("UpOrDown").GetComponent<UISprite>().spriteName
					= "Button05_Package_Compare_01";
				CompareRoot.transform.Find("RightLabel").GetComponent<UILabel>().text
					= "[00FF00]" +Mathf.Abs(Delta).ToString() + ")[-]";			
			}else if(Delta < 0){
				CompareRoot.transform.Find("Label").GetComponent<UILabel>().text
					= "[FF0000]([-]";
				CompareRoot.transform.Find("UpOrDown").GetComponent<UISprite>().spriteName
					= "Button05_Package_Compare_02";
				CompareRoot.transform.Find("RightLabel").GetComponent<UILabel>().text
					= "[FF0000]" +Mathf.Abs(Delta).ToString() + ")[-]";		
			}else {
				if(CompareRoot.activeSelf)
					CompareRoot.SetActive(false);	
			}
			
		}else{
			if(CompareRoot.activeSelf)
				CompareRoot.SetActive(false);	
		}
	}
		
	string ExtraShowAttribFormat(ExtraAttrib Extra,int quality)
	{
		string color = TextColorGet(quality);	
		TargetAttrib[] Ta = TargetAttribManager.Instance.GetAllItem();
		for(int i = 0; i<Ta.Length; i++)
		{
			if( 0 == string.Compare(Extra.Attrib,Ta[i].Attrib) )
			{
				if(i < 5 || i == 7)
					return Ta[i].ShowName + "  " + color + Extra.Value.ToString() + "[-]";
				else
				{
					float EValue = Extra.Value;
					return Ta[i].ShowName + "  " + color + (EValue/100.0f).ToString() + "%" + "[-]";
				}
			}else if ( 0 == string.Compare(Extra.Attrib,"Tough")){
					float EValue = Extra.Value;
					return "Tough" + "  " + color + (EValue/100.0f).ToString() + "%" + "[-]";
			}
		}
		return " ";
	}
	
	public void UpdateOne(ItemBase itemBase)
	{
		//DropDownAnimation();
		if(mGoDescDetail == null)
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
		Control("LabelLevel").GetComponent<UILabel>().text = TextColorGet(bStatus) + "LV[-]";
		Control("Level").GetComponent<UILabel>().text 
			= TextColorGet(bStatus) + itemBase.Level.ToString() + "[-]";	
				
		if(itemBase.MainType == (int)EItemType.Equip){
			StrengthenCountSet();
			//SetStarCount(item);	
		}else{
			ResetStrengthenCount();
			//ResetStarCount();
			return;
		}
		
		// All
		mMainAttrib = Control("LabelMain").GetComponent<UILabel>();
		TargetAttrib Ta = TargetAttribManager.Instance.GetItem(itemBase.MainType,itemBase.SubType);
		if( CheckEuipHasBaseAttrib(itemBase) ){
			mMainAttrib.text = Ta.ShowName + "   " + GetMainAttribAllBySubType(itemBase); //[C78100]
		}else{
			mMainAttrib.GetComponent<UILabel>().text = "";
		}
		CompareRootUpdate(Control("BaseAttribRoot").transform.Find("LabelMainStrengthDes").gameObject,
				itemBase,AttribType.MainAll);
		
		/*
		// Base
		mMainAttrib = Control("LabelMain").GetComponent<UILabel>();
		TargetAttrib Ta = TargetAttribManager.Instance.GetItem(itemBase.MainType,itemBase.SubType);
		if( CheckEuipHasBaseAttrib(itemBase) ){
			mMainAttrib.text = Ta.ShowName + "   " + GetMainAttirbBaseBySubType(itemBase);//[C78100]
		}else{
			mMainAttrib.GetComponent<UILabel>().text = "";
		}
		CompareRootUpdate(Control("BaseAttribRoot").transform.Find("LabelMainStrengthDes").gameObject,
				itemBase,AttribType.MainBase);
		*/
		
		//StrengthCount 
		int StrengthenCount = EquipBaseManager.Instance.GetItem(itemBase.ID).StrengthenCount;
		GameObject  goSlider = Control("Slider");
		if(StrengthenCount == 0){
			goSlider.GetComponent<UISlider>().sliderValue = 1.0f;
		}else{
			//float Percent = item.ToEquipItem().GetAttrib(EquipAttrib.StrengthenCount)/
			//				EquipBaseManager.Instance.GetItem(itemBase.ID).StrengthenCount;
			goSlider.GetComponent<UISlider>().sliderValue = 0.0f;
		}
		GameObject goStrength = Control("StrengthCountRoot");
		Control("LabelCount",goStrength).GetComponent<UILabel>().text = "0/"
				+ EquipBaseManager.Instance.GetItem(itemBase.ID).StrengthenCount.ToString();
		CompareRootUpdate(goStrength.transform.Find("CountStrengthDes").gameObject,
			itemBase,AttribType.StrengthenCount);
		if( !CheckEuipHasBaseAttrib(itemBase) )
		{
			goStrength.SetActive(false);
		}
		
		
		/*
		// Extra
		GameObject StrengthIcon = Control("StrengthRoot").transform.Find("Icon").gameObject;
		GameObject StrengthLabel = Control("StrengthRoot").transform.Find("LabelStrength").gameObject;
		int Delta = GetMainAttirbExtraBySubType(itemBase);
//		if( Delta > 0){
//			if( !StrengthIcon.activeSelf )
//				StrengthIcon.SetActive(true);
//			if( !StrengthLabel.activeSelf )
//				StrengthLabel.SetActive(true);
			StrengthLabel.GetComponent<UILabel>().text = Delta.ToString();
//		}else{
//			StrengthIcon.SetActive(false);
//			StrengthLabel.SetActive(false);
//		}
		CompareRootUpdate(Control("StrengthRoot").transform.Find("ExtraStrengthDes").gameObject,
				itemBase,AttribType.MainExtra);
		*/
		
		Control("ExtraAttrib1").GetComponent<UILabel>().text = " ";
		Control("ExtraAttrib2").GetComponent<UILabel>().text = " ";
		Control("ExtraAttrib3").GetComponent<UILabel>().text = " ";
		
		EquipBase Eb = EquipBaseManager.Instance.GetItem(itemBase.ID);
		if(Eb.ExtraAttrib != null){
			string[] str = Eb.ExtraAttrib.Split('|');
			for(int i = 0;i<str.Length;i++){
				Control("ExtraAttrib"+ (i+1).ToString()).GetComponent<UILabel>().text = "[00ff00]??????  [-]"; //(Buy Activation)
			}
		}
		

		// GS
		CompareRootUpdate(Control("GSRoot").transform.Find("GSStrengthDes").gameObject,
				itemBase,AttribType.GScore);
		Control("GSLabel").GetComponent<UILabel>().text = itemBase.Score.ToString();	
	}
	
	void InitOneItem(Item item)
	{		
		if(item == null)
			return;
		
		if(mGoDescDetail == null)
			return;
		if( Control("Title") == null )
			return;
		//DropDownAnimation();
		// Title And Desc
		mTitleName = Control("Title").GetComponent<UILabel>();
		mContentDesc = Control ("Desc").GetComponent<UILabel>();
		mContentDesc.text = item.ItemBase.Desc;
		
		mTitleName.text = TextColorGet( (int)item.ItemBase.Quality ) + item.ItemBase.Name + "[-]";
		Control("EquipIcon").GetComponent<UISprite>().spriteName = ItemIconGet(item.ItemBase);
		bool bStatus = item.ItemBase.Level > PlayerDataManager.Instance.Attrib.Level ? true : false;
		Control("LabelLevel").GetComponent<UILabel>().text = TextColorGet(bStatus) + "LV[-]";
		Control("Level").GetComponent<UILabel>().text 
			= TextColorGet(bStatus) + item.ItemBase.Level.ToString() + "[-]";
	
		if(item.ItemBase.MainType == (int)EItemType.Equip){
			StrengthenCountSet();
			//SetStarCount(item);	
		}else{
			ResetStrengthenCount();
			//ResetStarCount();
			return;
		}
		
		// All
		mMainAttrib = Control("LabelMain").GetComponent<UILabel>();
		TargetAttrib Ta = TargetAttribManager.Instance.GetItem(item.ItemBase.MainType,item.ItemBase.SubType);
		if( CheckEuipHasBaseAttrib(item.ItemBase) ){
			mMainAttrib.text = Ta.ShowName + "   " + GetMainAttribAllBySubType(item); //[C78100]
		}else{
			mMainAttrib.GetComponent<UILabel>().text = "";
		}
		CompareRootUpdate(Control("BaseAttribRoot").transform.Find("LabelMainStrengthDes").gameObject,
				item,AttribType.MainAll);
		
		
		/*
		// Base
		mMainAttrib = Control("LabelMain").GetComponent<UILabel>();
		TargetAttrib Ta = TargetAttribManager.Instance.GetItem(item.ItemBase.MainType,item.ItemBase.SubType);
		if( CheckEuipHasBaseAttrib(item.ItemBase) ){
			mMainAttrib.text = Ta.ShowName + "   " + GetMainAttirbBaseBySubType(item); //[C78100]
		}else{
			mMainAttrib.GetComponent<UILabel>().text = "";
		}
		CompareRootUpdate(Control("BaseAttribRoot").transform.Find("LabelMainStrengthDes").gameObject,
				item,AttribType.MainBase);
		*/
		
		//StrengthCount 
		float StrengthenCountMax = EquipBaseManager.Instance.GetItem(item.ItemBase.ID).StrengthenCount;
		GameObject  goSlider = Control("Slider");
		if(StrengthenCountMax == 0){
			goSlider.GetComponent<UISlider>().sliderValue = 1.0f;
		}else{
			float StrengthenCount = item.ToEquipItem().GetAttrib(EquipAttrib.StrengthenCount);
			float Percent = StrengthenCount/StrengthenCountMax;
			goSlider.GetComponent<UISlider>().sliderValue = Percent;
		}
		
		GameObject goStrength = Control("StrengthCountRoot");
		Control("LabelCount",goStrength).GetComponent<UILabel>().text = 
				item.ToEquipItem().GetAttrib(EquipAttrib.StrengthenCount) + "/"
				+ EquipBaseManager.Instance.GetItem(item.ItemBase.ID).StrengthenCount.ToString();
	
		CompareRootUpdate(Control("StrengthCountRoot").transform.Find("CountStrengthDes").gameObject,
			item,AttribType.StrengthenCount);

		if( !CheckEuipHasBaseAttrib(item.ItemBase) )
		{
			goStrength.SetActive(false);
		}
		
		/*
		// Extra
		GameObject StrengthIcon = Control("StrengthRoot").transform.Find("Icon").gameObject;
		GameObject StrengthLabel = Control("StrengthRoot").transform.Find("LabelStrength").gameObject;
		int Delta = GetMainAttirbExtraBySubType(item);
//		if( Delta > 0){
//			if( !StrengthIcon.activeSelf )
//				StrengthIcon.SetActive(true);
//			if( !StrengthLabel.activeSelf )
//				StrengthLabel.SetActive(true);
			StrengthLabel.GetComponent<UILabel>().text = Delta.ToString();
//		}else{
//			StrengthIcon.SetActive(false);
//			StrengthLabel.SetActive(false);
//		}
		CompareRootUpdate(Control("StrengthRoot").transform.Find("ExtraStrengthDes").gameObject,
				item,AttribType.MainExtra);
		*/
		
		if( item.ToEquipItem().Extra1 != null){
			Control("ExtraAttrib1").GetComponent<UILabel>().text = ExtraShowAttribFormat(item.ToEquipItem().Extra1,item.ItemBase.Quality);
		}else{
			Control("ExtraAttrib1").GetComponent<UILabel>().text =" ";
		}
		if( item.ToEquipItem().Extra2 != null){
			Control("ExtraAttrib2").GetComponent<UILabel>().text = ExtraShowAttribFormat(item.ToEquipItem().Extra2,item.ItemBase.Quality);
		}else{
			Control("ExtraAttrib2").GetComponent<UILabel>().text =" ";
		}
		if( item.ToEquipItem().Extra3 != null){
			Control("ExtraAttrib3").GetComponent<UILabel>().text = ExtraShowAttribFormat(item.ToEquipItem().Extra3,item.ItemBase.Quality);
		}else{
			Control("ExtraAttrib3").GetComponent<UILabel>().text =" ";
		}
		
		// GS
		CompareRootUpdate(Control("GSRoot").transform.Find("GSStrengthDes").gameObject,
				item,AttribType.GScore);
		Control("GSLabel").GetComponent<UILabel>().text = item.ItemBase.Score.ToString();		
	}
	
	void UpdateByItem(Item item)
	{
		InitOneItem(item);
	}
	
	void ResetStrengthenCount()
	{
		mGoDescDetail.transform.Find("Bg/BgRoot").gameObject.SetActive(false);
		mGoDescDetail.transform.Find("GSRoot").gameObject.SetActive(false);
		mGoDescDetail.transform.Find("StrengthCountRoot").gameObject.SetActive(false);
		mGoDescDetail.transform.Find("StrengthRoot").gameObject.SetActive(false);
		mGoDescDetail.transform.Find("WordDesc/BaseAttribRoot").gameObject.SetActive(false);
	}
	
	void StrengthenCountSet()
	{			
		mGoDescDetail.transform.Find("Bg/BgRoot").gameObject.SetActive(true);
		mGoDescDetail.transform.Find("GSRoot").gameObject.SetActive(true);
		mGoDescDetail.transform.Find("StrengthCountRoot").gameObject.SetActive(true);
		//mGoDescDetail.transform.Find("StrengthRoot").gameObject.SetActive(true);
		mGoDescDetail.transform.Find("WordDesc/BaseAttribRoot").gameObject.SetActive(true);
	}
		
	void Update(Item item)
	{		
		switch(mDescType){
			case DescType.AttribAll:UpdateAll();break;
			case DescType.AttribOne:UpdateByItem(item);break;
			default:break;
		}
	}
	
	void InitAll()
	{
		UpdateAll();
	}
	
	void UpdateAll()
	{
		//DropDownAnimation();
		//MainAttrib Attrib = PlayerDataManager.Instance.Data.Attrib;
		Control("Role").GetComponent<UISprite>().spriteName = "Button21_Attack_Job_0"
			+ (mPlayerMainAttrib.Role - 1002);
		mTitleName = Control("Title").GetComponent<UILabel>();
		mTitleName.text = mPlayerMainAttrib.Name;
		//if not LocalPlayer Hide TitleRoot
		if( string.Compare( mPlayerMainAttrib.Name,PlayerDataManager.Instance.Data.Attrib.Name ) != 0 )
		{
			mGoDescDetail.transform.Find("TitleRoot").gameObject.SetActive(false);
		}
		
		UpdateExp();
		UpdateMainAttrib();
	}

	void UpdateExp()
	{
		//MainAttrib attrib = PlayerDataManager.Instance.Attrib;
		float Cur = (float)mPlayerMainAttrib.CurExp;
		float ExpMax = (float)mPlayerMainAttrib.NextExp;
		float percent = (Cur/ExpMax);
		mExpSlider = Control("ExpSlider");
		mExpSlider.GetComponent<UISlider>().sliderValue = percent;
		Control("ExpLabel",mExpSlider).GetComponent<UILabel>().text = mPlayerMainAttrib.CurExp.ToString()+"/"+mPlayerMainAttrib.NextExp.ToString();
	}
	
	void UpdateMainAttrib()
	{
//		MainAttrib Attrib = PlayerDataManager.Instance.Data.Attrib;
//		Control ("Name").GetComponent<UILabel>().text =        	 "Name:" + Attrib.Name;
//		Control ("Role").GetComponent<UILabel>().text =          "Role:" + Attrib.Role.ToString();
		if(Control("Level") != null)
			Control ("Level").GetComponent<UILabel>().text =     mPlayerMainAttrib.Level.ToString();
//		Control ("Gold").GetComponent<UILabel>().text =        "Gold:"+Attrib.Gold.ToString();
//		Control ("Gem").GetComponent<UILabel>().text =          "Gem:"+Attrib.Gem.ToString();
//		Control ("CurHP").GetComponent<UILabel>().text =         "CurHP:"+Attrib.CurHP.ToString();
//		Control ("CurExp").GetComponent<UILabel>().text =        "CurExp:"+Attrib.CurExp.ToString();
//		Control ("NextExp").GetComponent<UILabel>().text =       "NextExp:"+Attrib.NextExp.ToString();
//		Control ("Exp").GetComponent<UILabel>().text =           "Exp:"+Attrib.CurExp.ToString();
		Control ("HPMax").GetComponent<UILabel>().text =         mPlayerMainAttrib.HPMax.ToString();
//		Control ("HPRestore").GetComponent<UILabel>().text =     "HPRestore:"+Attrib.HPRestore.ToString();
		Control ("SoulMax").GetComponent<UILabel>().text =       mPlayerMainAttrib.SoulMax.ToString();
		
//		Control ("SoulRestore").GetComponent<UILabel>().text =   "SoulRestore:"+Attrib.SoulRestore.ToString();
		Control ("Damage").GetComponent<UILabel>().text =        mPlayerMainAttrib.Damage.ToString();
		Control ("Defense").GetComponent<UILabel>().text =       mPlayerMainAttrib.Defense.ToString();
		Control ("SpecialDamage").GetComponent<UILabel>().text = mPlayerMainAttrib.SpecialDamage.ToString();
		Control ("SpecialDefense").GetComponent<UILabel>().text =mPlayerMainAttrib.SpecialDefense.ToString();
		
		Control ("Critical").GetComponent<UILabel>().text =      ((float)mPlayerMainAttrib.Critical /100.0f +"%").ToString();
		Control ("Tough").GetComponent<UILabel>().text =         ((float)mPlayerMainAttrib.Tough /100.0f +"%").ToString();
		Control ("Hit").GetComponent<UILabel>().text =           ((float)mPlayerMainAttrib.Hit /100.0f +"%").ToString();
		Control ("Block").GetComponent<UILabel>().text =         ((float)mPlayerMainAttrib.Block /100.0f +"%").ToString();
		
//		Control ("MoveSpeed").GetComponent<UILabel>().text =     "MoveSpeed:"+Attrib.MoveSpeed.ToString();
//		Control ("FastRate").GetComponent<UILabel>().text =      "FastRate:"+Attrib.FastRate.ToString();
//		Control ("StiffAdd").GetComponent<UILabel>().text =      "StiffAdd:"+Attrib.StiffAdd.ToString();
//		Control ("StiffSub").GetComponent<UILabel>().text =      "StiffSub:"+Attrib.StiffSub.ToString();
		Control ("AbilityMax").GetComponent<UILabel>().text =    mPlayerMainAttrib.AbilityMax.ToString();
//		Control ("AbHitAdd").GetComponent<UILabel>().text =      "AbHitAdd:"+Attrib.AbHitAdd.ToString();
//		Control ("AbRestore").GetComponent<UILabel>().text =     "AbRestore:"+Attrib.AbRestore.ToString();
//		Control ("AbUseAdd").GetComponent<UILabel>().text =      "AbUseAdd:"+Attrib.AbHitAdd.ToString();
		Control ("GSSum").GetComponent<UILabel>().text = 		 ( Mathf.Ceil( mPlayerMainAttrib.Battle /10.0f) ).ToString();

	}
	
	int CompareWithTwoEquip(ItemBase itemBase,AttribType AType)
	{
		foreach(Item itemTmp in PlayerDataManager.Instance.BackPack.Items.Values){
			if((int)itemTmp.ItemBase.MainType != (int)EItemType.Equip )
				continue;
			
			if( itemTmp.ItemBase.SubType == itemBase.SubType
					&& (itemTmp.ToEquipItem().Equiped)){
				switch(AType){
					case AttribType.GScore:
						return GetEquipScoreBySubType(itemBase) - GetEquipScoreBySubType(itemTmp);
					case AttribType.MainAll:
						return GetMainAttribAllBySubType(itemBase) - GetMainAttribAllBySubType(itemTmp);
					case AttribType.MainBase:
						return GetMainAttirbBaseBySubType(itemBase) - GetMainAttirbBaseBySubType(itemTmp);
					case AttribType.MainExtra:	
						return GetMainAttirbExtraBySubType(itemBase) - GetMainAttirbExtraBySubType(itemTmp);
					case AttribType.StrengthenCount:
						return GetEquipStrengthenCount(itemBase) - GetEquipStrengthenCount(itemTmp);
				}
			}
		}
		
		switch(AType){
			case AttribType.GScore:
				return GetEquipScoreBySubType(itemBase);
			case AttribType.MainAll:
				return GetMainAttribAllBySubType(itemBase);
			case AttribType.MainBase:
				return GetMainAttirbBaseBySubType(itemBase);
			case AttribType.MainExtra:	
				return GetMainAttirbExtraBySubType(itemBase);
			case AttribType.StrengthenCount:
				return GetEquipStrengthenCount(itemBase);
		}
		return 0;
	}
	
	int CompareWithTwoEquip(Item item,AttribType AType)
	{
		if(item.ToEquipItem()== null || 
			(int)item.ItemBase.MainType != (int)EItemType.Equip){
			Debug.Log("Item is not Equip");
			return 0;
		}
		
		foreach(Item itemTmp in PlayerDataManager.Instance.BackPack.Items.Values){
			if(itemTmp.ID == item.ID || (int)itemTmp.ItemBase.MainType != (int)EItemType.Equip )
				continue;
			
			if( itemTmp.ItemBase.SubType == item.ItemBase.SubType
					&& (itemTmp.ToEquipItem().Equiped)){
				switch(AType){
					case AttribType.GScore:
						return GetEquipScoreBySubType(item) - GetEquipScoreBySubType(itemTmp);
					case AttribType.MainAll:
						return GetMainAttribAllBySubType(item) - GetMainAttribAllBySubType(itemTmp);
					case AttribType.MainBase:
						return GetMainAttirbBaseBySubType(item) - GetMainAttirbBaseBySubType(itemTmp);
					case AttribType.MainExtra:	
						return GetMainAttirbExtraBySubType(item) - GetMainAttirbExtraBySubType(itemTmp);
					case AttribType.StrengthenCount:
						return GetEquipStrengthenCount(item) - GetEquipStrengthenCount(itemTmp);
				}
			}
		}
		
		switch(AType){
			case AttribType.GScore:
				return GetEquipScoreBySubType(item);
			case AttribType.MainAll:
				return GetMainAttribAllBySubType(item);
			case AttribType.MainBase:
				return GetMainAttirbBaseBySubType(item);
			case AttribType.MainExtra:	
				return GetMainAttirbExtraBySubType(item);
			case AttribType.StrengthenCount:
				return GetEquipStrengthenCount(item);
		}
		return 0;
	}
	
	GameObject Control(string name)
    {
        return Control(name, mGoDescDetail);
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
		if(mGoDescDetail != null)
			GameObject.Destroy(mGoDescDetail);
		mGoDescDetail = null;
	}
}

