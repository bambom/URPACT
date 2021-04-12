using UnityEngine;
using System.Collections;

public class GuideWnd : Window<GuideWnd>
{
    public override string PrefabName { get { return "GuideWnd"; } }
	
	GameObject mGoGuideSay;
	GameObject mMoveButton;
	GameObject mBgRoot;
	UILabel mLabelBtn;
	UILabel mLabelContent;
	
	protected override bool OnOpen()
    {
		//WinStyle = WindowStyle.WS_Ext;
		Init();
        return base.OnOpen();
    }

    protected override bool OnClose()
    {
        return base.OnClose();
    }
	
	void Init()
	{
		this.WndObject.transform.localPosition = new Vector3(0.0f,0.0f,-200.0f);
		mBgRoot = Control ("BgRoot");
		mGoGuideSay = Control ("GuideSay");
		mMoveButton = Control ("MoveButton");
		mLabelBtn = Control ("LabelBtn",mGoGuideSay).GetComponent<UILabel>();
		mLabelContent = Control ("LabelContent",mGoGuideSay).GetComponent<UILabel>();
		
	}
	
	void ArrowBtnReset(GameObject[] Arrows)
	{
		for(int i= 0;i< Arrows.Length; i++){
			if(Arrows[i].activeSelf)
				Arrows[i].SetActive(false);
		}
	}
	
	void ArrowBtnUpdate(int Dir,Vector3 vPos)
	{
		GameObject[] Arrows = 
		{
			mMoveButton.transform.Find("ArrowUp").gameObject,
			mMoveButton.transform.Find("ArrowDown").gameObject,
			mMoveButton.transform.Find("ArrowLeft").gameObject,
			mMoveButton.transform.Find("ArrowRight").gameObject
		};
		ArrowBtnReset(Arrows);
		Arrows[Dir-1].SetActive(true);
		mMoveButton.transform.localPosition = vPos;
	}
	
	
	void UpdateGuideSay(int UI)
	{
		switch(UI)
		{
			case 0:
			{
				if( !mGoGuideSay.activeSelf )
					mGoGuideSay.SetActive(true);
				if( !mBgRoot.activeSelf )
					mBgRoot.SetActive(true);
			}break;
			case 1:
			{
				if( mGoGuideSay.activeSelf )
					mGoGuideSay.SetActive(false);
				if( !mBgRoot.activeSelf )
					mBgRoot.SetActive(true);
			}break;
			case 2:
			{
				if( mGoGuideSay.activeSelf )
					mGoGuideSay.SetActive(false);
				if( mBgRoot.activeSelf )
					mBgRoot.SetActive(false);
			}break;	
		}
	}
	
	public void Update(GameObject ControlName,UIGuide UiG)
	{
		if(UiG.Trigger == -1)
		{
			Global.CurStep += 1;
			if(Global.CurStep == UiG.Counts){
				MainScript.Instance.Request(new FinishNewGuideCmd(Global.CurGuideID-1), delegate(string err, Response response)
		        {
		            if (!string.IsNullOrEmpty(err))
		            {
						MessageBoxWnd.Instance.Show( err ,MessageBoxWnd.StyleExt.Alpha);
		                return;
		            }
					//RequestAttribInfo();
                });
			}
		}
		
		ControlName.transform.parent = WndObject.transform;
		Vector3 VPos = ControlName.transform.localPosition;
		VPos.z = -1.0f;
		ControlName.transform.localPosition = VPos;
		ArrowBtnUpdate(UiG.Dir,VPos);
		UpdateGuideSay(UiG.UI);
		
		Debug.Log("ControlName  " + ControlName);
		Debug.Log("UiG.Function  " + UiG.Function);
		UIButtonMessage UIMsg = ControlName.GetComponent<UIButtonMessage>();
		if( UIMsg == null)
		{
			UIMsg = ControlName.AddComponent<UIButtonMessage>();
		}
		UIMsg.target = ControlName;
		UIMsg.functionName = UiG.Function;
	}	
}

