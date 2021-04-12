using UnityEngine;
using System.Collections;

public class GuideClickFeedBack : MonoBehaviour
{
	delegate void UIGuideShowCall();
	UIGuideShowCall UIGuideShowCallBack;
	
	delegate void UIGuideShowPackBackCall();
	UIGuideShowPackBackCall UIGuideShowPackCallBack;
	
	// Use this for initialization
	void Start ()
	{
	
	}
	
	// Update is called once per frame
	void Update ()
	{
	
	}
	
	void UIGuideShowMutexUpdate(string NextWnd)
	{
		UIGuideShow.Mutex = true;
		Global.CurStep += 1;
		UIGuideShow.CurWnd = NextWnd;
		Debug.Log("UIGuideShowMutexUpdate Global.CurStep :" + Global.CurStep);
	}
	
	void UIGuideShowStepsUpdate()
	{
		GuideWnd.Instance.Close();
		if(UIGuideShow.CurUIGuide.Trigger != -1)
			Global.CurStep +=1;
	}
	
	void RequsetBackPackInfo()
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
                PlayerDataManager.Instance.OnBackPack(backPackData,false);
				if (UIGuideShowPackCallBack != null)
	            {
	                Debug.Log("UIGuideShowCallBack");
	                UIGuideShowPackCallBack();
	            }
				
            }
        });
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
				PlayerDataManager.Instance.OnMainAttrib(attrib);
			
			Global.CurStep = 1;

            if (GuideWnd.Exist)
			    GuideWnd.Instance.Close();

            if (UIGuideShowCallBack != null)
            {
                Debug.Log("UIGuideShowCallBack");
                UIGuideShowCallBack();
            }
			RequsetBackPackInfo();
        });
	}
	
	void UIGuideShowEnd()
	{
		MainScript.Instance.Request(new FinishNewGuideCmd(Global.CurGuideID-1), delegate(string err, Response response)
        {
            if (!string.IsNullOrEmpty(err))
            {
				MessageBoxWnd.Instance.Show( err ,MessageBoxWnd.StyleExt.Alpha);
                return;
            }
			RequestAttribInfo();
			//RequsetBackPackInfo();
        });
	}
	
	void OnClickQuestWnd()
	{
		UIGuideShow.CloneControl.SetActive(false);
		UIGuideShowStepsUpdate();
		if(SystemWnd.Exist)
			SystemWnd.Instance.Close();
		
		InGameMainWnd.Instance.Close();

		if(!QuestWnd.Exist)
			QuestWnd.Instance.Open();
		//InGameMainWnd.Instance.OnClickQuestWnd(UIGuideShow.SourceControl);
	}
	
	void OnClickFight()
	{
		UIGuideShow.CloneControl.SetActive(false);
		UIGuideShowStepsUpdate();
		QuestWnd.Instance.OnClickFight(UIGuideShow.SourceControl);
	}
	
	#region Guide 1
	void OnClickEntryGame()
	{
		UIGuideShow.CloneControl.SetActive(false);
		UIGuideShowCallBack = Guide1ClickBack;	
		UIGuideShowEnd();
		
//		LevelSelectWnd.Instance.OnClickEntryGame(UIGuideShow.SourceControl);
//		UIGuideShow.CloneControl.SetActive(false);
//		UIGuideShowStepsUpdate();
	}
	
	void Guide1ClickBack()
	{
		LevelSelectWnd.Instance.OnClickEntryGame(UIGuideShow.SourceControl);
		//LevelFinishWnd.Instance.OnClickBacktoMainTown(UIGuideShow.SourceControl);
	}
	
	void OnClickBacktoMainTown()
	{
		UIGuideShow.CloneControl.SetActive(false);
		UIGuideShowCallBack = Guide1ClickBack;	
		UIGuideShowEnd();
	}
	#endregion  Guide 1
	
	#region Guide 2
	void Guide2ClickFightkBack()
	{
		QuestWnd.Instance.OnClickBack(null);
	}
	
	
	void Guide2ClickFight()
	{
		QuestWnd.Instance.OnClickFight(UIGuideShow.SourceControl);	
		UIGuideShowPackCallBack = Guide2ClickFightkBack;
	}
	
	
	void OnClickFightGuide2()
	{	
		UIGuideShow.CloneControl.SetActive(false);
		UIGuideShowCallBack = Guide2ClickFight;
		UIGuideShowEnd();
	}
	
	void Guide2ClickBack()
	{
		QuestWnd.Instance.OnClickBack(UIGuideShow.SourceControl);
	}
	
	void OnClickBack()
	{
		UIGuideShow.CloneControl.SetActive(false);
		UIGuideShowCallBack = Guide2ClickBack;
		UIGuideShowEnd();
	}
	#endregion Guide 2
	
	#region Guide 3
	void OnClickBtnExt()
	{
		UIGuideShow.CloneControl.SetActive(false);
		Debug.Log("OnClickBtnExt");
		InGameMainWnd.Instance.OnClickBtnExt(UIGuideShow.SourceControl);
		UIGuideShowMutexUpdate(InGameMainWnd.Instance.PrefabName);
	}

	void OnClickPackageWnd()
	{
		UIGuideShow.CloneControl.SetActive(false);
		Debug.Log("OnClickPackageWnd");
		UIGuideShowStepsUpdate();
		if(SystemWnd.Exist)
			SystemWnd.Instance.Close();
		InGameMainWnd.Instance.Close();
		PackageWnd.Instance.Open();
	}
	
	void OnItemClicked()
	{
		UIGuideShow.CloneControl.SetActive(false);
		Debug.Log("OnClickPackItem UIGuideShow.SourceControl.transform.Find(Icon).gameObject ");
		PackageWnd.Instance.mBackPack.OnClickItem(UIGuideShow.SourceControl.transform.Find("Icon").gameObject);
		UIGuideShowMutexUpdate(PackageWnd.Instance.PrefabName);
	}
	
	void Guide3Click()
	{
		PackageWnd.Instance.OnClickBtnEquip(UIGuideShow.SourceControl);
		//PackageWnd.Instance.OnClickBtnReturn(UIGuideShow.SourceControl);
	}
	
	void OnClickBtnEquip()
	{	
		UIGuideShow.CloneControl.SetActive(false);
		UIGuideShowCallBack = Guide3Click;
		UIGuideShowEnd();
		
	}
	
	void OnClickBtnReturn()
	{
		UIGuideShow.CloneControl.SetActive(false);
		UIGuideShowCallBack = Guide3Click;
		UIGuideShowEnd();
	}
	#endregion Guide 3
	
	#region Guide 4 // NoUSE
	void OnClickFreeQuestWnd()
	{
		UIGuideShow.CloneControl.SetActive(false);
		UIGuideShowStepsUpdate();
		if(SystemWnd.Exist)
			SystemWnd.Instance.Close();	
		InGameMainWnd.Instance.Close();
		if(!QuestWnd.Exist)
			QuestWnd.Instance.Open();	
	}
	
	void OnClickFreeFight()
	{
		UIGuideShow.CloneControl.SetActive(false);
		UIGuideShowStepsUpdate();
		QuestWnd.Instance.OnClickFight(UIGuideShow.SourceControl);
	}
	
	void OnClickFreeEntryGame()
	{
		UIGuideShow.CloneControl.SetActive(false);
		UIGuideShowStepsUpdate();
		LevelSelectWnd.Instance.OnClickEntryGame(UIGuideShow.SourceControl);
	}
	
	void OnClickFreeBacktoMainTown()
	{
		UIGuideShow.CloneControl.SetActive(false);
		UIGuideShowCallBack = Guide1ClickBack;	
		UIGuideShowEnd();
	}
	#endregion Guide 4
	
	#region 5
	void OnClickSkillWnd()
	{
		UIGuideShow.CloneControl.SetActive(false);
		Debug.Log("OnClickSkillWnd");
		UIGuideShowStepsUpdate();
		if(SystemWnd.Exist)
			SystemWnd.Instance.Close();
		InGameMainWnd.Instance.Close();
		if( !SkillWnd.Exist )
			SkillWnd.Instance.Open();
	}
	
	void OnClickTypeButton()
	{
		UIGuideShow.CloneControl.SetActive(false);
		SkillWnd.Instance.OnClickTypeButton(UIGuideShow.SourceControl);
		UIGuideShowMutexUpdate(PackageWnd.Instance.PrefabName);
	}
	
	void OnClickLearnButton()
	{
		UIGuideShow.CloneControl.SetActive(false);
		UIGuideShowCallBack = Guide6ClickBack;	
		UIGuideShowEnd();
	}	
	
	void Guide6ClickBack()
	{
		SkillWnd.Instance.OnClickLearnButton(UIGuideShow.SourceControl);
	}
	
	#endregion 5

	
	#region Guide 6
	void OnClickBtnStrengthen()
	{
		UIGuideShow.CloneControl.SetActive(false);
		UIGuideShowStepsUpdate();
		PackageWnd.Instance.OnClickBtnStrengthen(UIGuideShow.SourceControl);
	}
	
	void Guide5Click()
	{
		PackageWnd.StrengthenWnd.EquipStrengthSureOnClick(UIGuideShow.SourceControl);
	}

	void EquipStrengthSureOnClick()
	{
		UIGuideShow.CloneControl.SetActive(false);
		UIGuideShowCallBack = Guide5Click;
		UIGuideShowEnd();
	}
	#endregion 6
	
	
	#region 9
	void OnClickBtnCombine()
	{
		UIGuideShow.CloneControl.SetActive(false);
		UIGuideShowStepsUpdate();
		PackageWnd.Instance.OnClickBtnCombine(UIGuideShow.SourceControl);
	}
	
	void Guide6Click()
	{
		PackageWnd.CombineWnd.CombineSureOnClick(UIGuideShow.SourceControl);
	}

	void CombineSureOnClick()
	{
		UIGuideShow.CloneControl.SetActive(false);
		UIGuideShowCallBack = Guide6Click;
		UIGuideShowEnd();
	}
	#endregion 9
	
	
	#region 11
	void Guide11ClickBack()
	{
		SkillWnd.Instance.OnSkillStrengthen(UIGuideShow.SourceControl);
	}
	
	void OnSkillStrengthen()
	{
		UIGuideShow.CloneControl.SetActive(false);
		UIGuideShowCallBack = Guide11ClickBack;	
		UIGuideShowEnd();
	}
	
	void OnOpenRuneWindow()
	{
		UIGuideShow.CloneControl.SetActive(false);
		SkillWnd.Instance.OnOpenRuneWindow(UIGuideShow.SourceControl);
		UIGuideShowMutexUpdate(SkillWnd.Instance.PrefabName);
	}
	#endregion 11
}

