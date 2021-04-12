using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum UISpName
{
	Button05_Package_Page_01,
	Button05_Package_Page_02
};

public class BasePoint
{
	public string PrefabName { get { return "PackBasePoint"; } }
	public GameObject goPoint;
	public UISprite UISpriteScript;
	private int mIndex = 0;
	
	public BasePoint(GameObject goParent,int index)
	{
		if(null == goPoint){
			goPoint = GameObject.Instantiate(Resources.Load(PrefabName)) as GameObject;
			goPoint.transform.parent = goParent.transform;
			goPoint.transform.localPosition = Vector3.zero;
			goPoint.transform.localScale = Vector3.one;
			PackPointPosInfo.RowMargin = goPoint.transform.Find("Sprite").transform.localScale.x;
			UISpriteScript = goPoint.GetComponentInChildren<UISprite>();
		}
		mIndex = index;
	}
	
	public void SetFocus()
	{
		UISpName SpName = UISpName.Button05_Package_Page_01;
		UISpriteScript.spriteName = SpName.ToString();
	}
	
	public void SetUnFocus()
	{
		UISpName SpName = UISpName.Button05_Package_Page_02;
		UISpriteScript.spriteName = SpName.ToString();
	}
}

public class SheetPoint
{
	private int mCount;
	private bool mStatus;
	private GameObject mGoRoot;
	private List<BasePoint> mListPoint = new List<BasePoint>();
	private float mSheetBaseStep = 0.0f;
	private int mCurFocusIndex = 0;
	
	public GameObject GoSpRoot{ get { return mGoRoot; } }
	
	
	public SheetPoint(int Count)
	{
		if(mListPoint.Count > 0 ){
			mListPoint.Clear();
		}
		mCount = Count;
		mSheetBaseStep = PackPointPosInfo.RowMargin;
		
		mGoRoot = new GameObject();
		mGoRoot.name = "GoPackPointRoot";
		mGoRoot.layer = 8;
		mGoRoot.transform.localPosition = Vector3.zero;
		mGoRoot.transform.localScale = Vector3.one;
		
		CreatePoints();
		SetFocusPoint(mCurFocusIndex);
	}
	
	void CreatePoints()
	{
		for(int i = 0;i<mCount;i++){
			BasePoint Bp = new BasePoint(mGoRoot,i);
			mListPoint.Add(Bp);
		}
		ResetPointPos();
	}
	
	void ResetPointPos()
	{
		Vector3 Vpos = Vector3.zero;
		for(int i = 0; i<mCount; i++){
			Vpos.x = mListPoint[0].goPoint.transform.localPosition.x + PackPointPosInfo.RowMargin*2*i;
			Vpos.y = mListPoint[0].goPoint.transform.localPosition.y;
			Vpos.z = mListPoint[0].goPoint.transform.localPosition.z;
			mListPoint[i].goPoint.transform.localPosition = Vpos;
		}
	}
	
	public void AddPoint()
	{
		BasePoint AddBp = new BasePoint(mGoRoot,mListPoint.Count);
		mListPoint.Add(AddBp);
		mCount = mListPoint.Count;
		ResetPointPos();
		SetFocusPoint(mCurFocusIndex);
	}
	
	public void SetFocusPoint(int index)
	{
		for(int i = 0; i<mListPoint.Count; i++){
			mListPoint[i].SetUnFocus();
		}
		mListPoint[index].SetFocus();
	}
	
	public void MoveStep()
	{
		float VxDis = mSheetBaseStep * mListPoint.Count;
		mGoRoot.transform.localPosition = Vector3.zero;
		
		Vector3 vDestPos = Vector3.zero;
		vDestPos.x = 0- VxDis;
		mGoRoot.transform.localPosition = vDestPos;
	}
	
	public void SetToTargetGameObjectRoot(GameObject go)
	{
		mGoRoot.transform.parent = go.transform;
		mGoRoot.transform.localPosition = Vector3.zero - new Vector3(mCount*PackPointPosInfo.RowMargin,0.0f,0.0f) ;
		mGoRoot.transform.localScale = Vector3.one;
	}
}

