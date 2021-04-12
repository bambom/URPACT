using UnityEngine;
using System.Collections;

public class PvPResult 
{
	public string PrefabName { get { return "PvPResultWnd"; } }
	GameObject mGoPvPResult;
	PvpFinish.PvpFinishData mPvPData;
	
	UISlider mMasterSlider;
	UILabel mMasterLabel;
	UILabel mPvPExpLabel;
	//UILabel mPvPExpShowLabel;
	int mPvPLevel; //LevelPreview
 	int mPvPFinishExp;
	float mSliderValue;
	float mPvpNextExp;
	MainAttrib mAttrib;
	SliderProgress mSProgress =  SliderProgress.Invalid;
	
	public PvPResult(PvpFinish.PvpFinishData Data)
	{
		mPvPData = Data;
		if(mGoPvPResult == null){
			mGoPvPResult = GameObject.Instantiate(Resources.Load(PrefabName)) as GameObject;
	        GameObject RootUI = GameObject.Find("Anchor");
	        mGoPvPResult.transform.parent = RootUI.transform;
	        mGoPvPResult.transform.localPosition = Vector3.zero; // + new Vector3(0.0f,0.0f,-60.0f);
	        mGoPvPResult.transform.localScale = Vector3.one;
			
			mGoPvPResult.AddComponent<ControlHelper>().ControlHelpUpdate = SliderUpdate;
		}
		Init();
		RequestAttribInfo();
	}
	
	void Init()
	{
		UIEventListener.Get(Control("Return")).onClick = OnClickBtnReturn;
		Control("spLabel").GetComponent<UILabel>().text = mPvPData.sp.ToString();
		UISprite UISpritScpt = Control("ResultSprite").GetComponent<UISprite>();
		if(mPvPData.result == 1){
			UISpritScpt.spriteName = "Button20_Battle_Victory_01";
			//Play Sound of PVP Success
			SoundManager.Instance.PlaySound("UI_PVP_Success_02");
		}else{
			UISpritScpt.spriteName = "Button20_Battle_Defeate_01";
		}
		mPvPExpLabel = Control("PvPExp").GetComponent<UILabel>();
		//mPvPExpShowLabel = Control ("PvPExpShowLabel").GetComponent<UILabel>();
		
		mPvPFinishExp = mPvPData.exp;
		if(mPvPData.result == 1)
			mPvPExpLabel.text = "+" + mPvPFinishExp.ToString();
		else
			mPvPExpLabel.text = "-" + mPvPFinishExp.ToString();
		MainAttrib  Attrib = PlayerDataManager.Instance.Attrib;
		mPvPLevel = Attrib.PvpLevel;
		PvpBase PvPBM = PvpBaseManager.Instance.GetItem(Attrib.PvpLevel);
		mMasterLabel = Control("Master").GetComponent<UILabel>();
		mMasterLabel.text = PvPBM.ArenaTitle;
		mMasterSlider = Control("MasterLevelSlider").GetComponent<UISlider>();
		
		Debug.Log("Attrib.PvpExp = " + Attrib.PvpExp 
			+ "  PvPBM.Exp - PvPBM.NextExp = " + (PvPBM.Exp - PvPBM.NextExp)
			+ "  PvPBM.NextExp = " + PvPBM.NextExp );
		
		float PvpExp  = (float)(Attrib.PvpExp - (PvPBM.Exp - PvPBM.NextExp));
		float PvpExpMax = (float)PvPBM.NextExp ;
		
		//mPvPExpShowLabel.text = (Attrib.PvpExp - (PvPBM.Exp - PvPBM.NextExp)).ToString()
		//			+"/"+PvPBM.NextExp.ToString();
		
		mMasterSlider.sliderValue = PvpExp / PvpExpMax;	
		Debug.Log("mMasterSlider.sliderValue = " + mMasterSlider.sliderValue );
	}
	
	void OnClickBtnReturn(GameObject go)
	{
		RequestLeavePvpInfo();
	}

	void RequestLeavePvpInfo()
	{
		Debug.Log ("RequestLeavePvP");
      	MainScript.Instance.Request(new LeavePvpCmd(), delegate(string err, Response response)
        {
            if (!string.IsNullOrEmpty(err))
            {
				MessageBoxWnd.Instance.Show(err);
                return;
            }	
			Debug.Log ("RequestLeavePvP Suceess");
			
			SceneList SLItem = SceneListManager.Instance.GetItem((int)ESceneID.Main,(int)ESceneType.Main);
			Global.CurSceneInfo.ID = ESceneID.Main;
			Global.CurSceneInfo.Type = ESceneType.Main;
			Global.CurSceneInfo.Name = SLItem.Name;
			Close();		 
			MainScript.Instance.LoadLevel(Global.CurSceneInfo);
        });
	}
	
	void UpGradeProgress()
	{
		PvpBase PvPBM = PvpBaseManager.Instance.GetItem(mAttrib.PvpLevel);
		float PvpExp  = (float)(mAttrib.PvpExp - (PvPBM.Exp - PvPBM.NextExp));
		float PvpExpMax = (float)PvPBM.NextExp ;
		
		mSliderValue = PvpExp/PvpExpMax;	
		mPvpNextExp = PvPBM.NextExp;
		mMasterLabel.text = PvPBM.ArenaTitle;
		
		mSProgress = SliderProgress.Process;
	}
	
	void ReduceGradeProgress()
	{
		PvpBase PvPBM = PvpBaseManager.Instance.GetItem(mAttrib.PvpLevel);		
		float PvpExp  = (float)(mAttrib.PvpExp - (PvPBM.Exp - PvPBM.NextExp));
		float PvpExpMax = (float)PvPBM.NextExp ;
		
		mPvpNextExp = PvPBM.NextExp;
		mSliderValue = PvpExp/PvpExpMax;	
		mMasterLabel.text = PvPBM.ArenaTitle;
		
		mSProgress = SliderProgress.Process;
	}

	void UpdatePvPInfo()
	{
		mAttrib = PlayerDataManager.Instance.Attrib;
		PvpBase PvPBM = PvpBaseManager.Instance.GetItem(mAttrib.PvpLevel);		
		float PvpExp  = (float)(mAttrib.PvpExp - (PvPBM.Exp - PvPBM.NextExp));
		float PvpExpMax = (float)PvPBM.NextExp ;
		mPvpNextExp = PvpExpMax;
		// Final
		mSliderValue = PvpExp/PvpExpMax;	
		
		mSProgress = SliderProgress.Start;
	}	
	
	void SliderUpdate()
	{
		if(mSProgress == SliderProgress.End
			 || mSProgress == SliderProgress.Invalid){
			return;
		}
		
		//Before After
		if(mPvPLevel == mAttrib.PvpLevel)
		{
			Debug.Log("mPvPLevel == mAttrib.PvpLevel");
			Debug.Log("mMasterSlider.sliderValue = " + mSliderValue);
			if(mPvPData.result == 1){
				mPvPExpLabel.text = " + " + mPvPFinishExp.ToString();
				mMasterSlider.sliderValue += 1.0f/mPvpNextExp;
				if( mMasterSlider.sliderValue > mSliderValue)
				{	
					mMasterSlider.sliderValue = mSliderValue;
					mSProgress = SliderProgress.End;
					mPvPExpLabel.text = " ";
				}
			}else{
				mPvPExpLabel.text = " - " + mPvPFinishExp.ToString();
				mMasterSlider.sliderValue -= 1.0f/mPvpNextExp;
				if( mMasterSlider.sliderValue < mSliderValue)
				{
					mMasterSlider.sliderValue = mSliderValue;
					mSProgress = SliderProgress.End;
					mPvPExpLabel.text = " ";
				}
			}
		}else if(mPvPLevel < mAttrib.PvpLevel){
			Debug.Log("mPvPLevel < mAttrib.PvpLevel");
			mPvPExpLabel.text = " + " + mPvPFinishExp.ToString();
			if( mSProgress == SliderProgress.Start ){
				mMasterSlider.sliderValue += 1.0f/mPvpNextExp;
				if(mMasterSlider.sliderValue == 0.0f){
					ReduceGradeProgress();
				}
			}else if(mSProgress == SliderProgress.Process){
				mMasterSlider.sliderValue -= 1.0f/mPvpNextExp;
				if(mMasterSlider.sliderValue <= mSliderValue){
					mMasterSlider.sliderValue = mSliderValue;
					mSProgress = SliderProgress.End;
					mPvPExpLabel.text = " ";
				}
			}
		}else if(mPvPLevel > mAttrib.PvpLevel){
			Debug.Log("mPvPLevel > mAttrib.PvpLevel");
			mPvPExpLabel.text = " - " + mPvPFinishExp.ToString();
			if( mSProgress == SliderProgress.Start ){
				mMasterSlider.sliderValue += 1.0f/mPvpNextExp;
				if(mMasterSlider.sliderValue == 1.0f){
					UpGradeProgress();
				}
			}else if(mSProgress == SliderProgress.Process){
				mMasterSlider.sliderValue += 1.0f/mPvPFinishExp;
				if(mMasterSlider.sliderValue >= mSliderValue){
					mMasterSlider.sliderValue = mSliderValue;
					mSProgress = SliderProgress.End;
					mPvPExpLabel.text = " ";
				}
			}
		}
		
		mPvPFinishExp --;
		if( mPvPFinishExp <= 0 ){
			mPvPExpLabel.text = " ";
		}else{
			Debug.Log(" mPvPFinishExp  = " + mPvPFinishExp);
		}	
	}
	

	
	void RequestAttribInfo()
	{
        MainScript.Instance.Request(new GetUserAttribCmd(), delegate(string err, Response response)
        {
            if (!string.IsNullOrEmpty(err))
            {
				MessageBoxWnd.Instance.Show( err ,MessageBoxWnd.StyleExt.Alpha);
                return;
            }
            MainAttrib attrib = (response != null) ? response.Parse<MainAttrib>() : null;
            if (attrib != null)
			{
				PlayerDataManager.Instance.OnMainAttrib(attrib);
			}
			UpdatePvPInfo();
        });
	}	
	
	
	
	void Close()
	{
		if(mGoPvPResult != null){	
			GameObject.Destroy(mGoPvPResult);
			mGoPvPResult = null;
		}
	}
	
	GameObject Control(string name)
    {
        return Control(name, mGoPvPResult);
    }

    GameObject Control(string name, GameObject parent)
    {
        Transform[] children = parent.GetComponentsInChildren<Transform>();
        foreach (Transform child in children)
        {
            if (child.name == name)
                return child.gameObject;
        }
        return null;
    }
}

