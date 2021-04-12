using UnityEngine;
using System.Collections;

public class ClipChangeEffect : MonoBehaviour {
	public Rect mRect;
	public Transform[] changeTrans;
	public float time = 1;
	public bool move = false;
	public Vector3 position;
	public iTween.EaseType easetype;
	
	Vector3[] invTransScale;
	
	Rect invRect;
	Vector3 invPosition;
	UIPanel panel;
	
	BoxCollider myCollder;
	BoxCollider[] boxColliders;
	bool trigger = false;
	void Awake()
	{
		panel = GetComponent<UIPanel>();
		
		myCollder = GetComponent<BoxCollider>();
		boxColliders = GetComponentsInChildren<BoxCollider>();
		
		invRect = new Rect(panel.clipRange.x,panel.clipRange.y,panel.clipRange.z,panel.clipRange.w);
		invTransScale = new Vector3[changeTrans.Length];
		for (int i = 0; i < changeTrans.Length; i++) {
			invTransScale[i] = changeTrans[i].localScale;
		}
	}
	
	void OnClick()
	{
		iTween.Stop(gameObject);
		
		if(trigger)
		{
			CollidersVisable(true);
			if(move)
				iTween.MoveTo(gameObject,iTween.Hash("position",invPosition,"time",time,"islocal",true,"easetype",easetype));
			iTween.ValueTo(gameObject,iTween.Hash("from",mRect,"to",invRect,"time",time,"easetype",easetype,"onupdate","OnUpdateValueTo"));
		}
		else
		{
			CollidersVisable(false);
			invPosition = transform.localPosition;
			if(move)
				iTween.MoveTo(gameObject,iTween.Hash("position",position,"time",time,"islocal",true,"easetype",easetype));
			iTween.ValueTo(gameObject,iTween.Hash("from",invRect,"to",mRect,"time",time,"easetype",easetype,"onupdate","OnUpdateValueTo"));
		}
		trigger = !trigger;
	}
	
	void To()
	{
		if(!trigger)
		{
			CollidersVisable(false);
			OnClick();
		}
	}
	
	void From()
	{
		if(trigger)
		{
			CollidersVisable(true);
			OnClick();
		}
	}
	
	void CollidersVisable(bool visable)
	{
		foreach (BoxCollider item in boxColliders) {
			if(item!=myCollder)
				item.gameObject.SetActive(visable);
//				item.enabled = visable;
		}
	}
	
	void OnUpdateValueTo(Rect rect)
	{
		panel.clipRange = new Vector4(rect.x,rect.y,rect.width,rect.height);
		
		if(myCollder!=null)
		{
			myCollder.center = new Vector3(rect.x,rect.y,myCollder.center.z);
			myCollder.size = new Vector3(rect.width,rect.height,myCollder.size.z);
		}
		
		for (int i = 0; i < changeTrans.Length; i++) {
			changeTrans[i].localPosition = new Vector3(rect.x,rect.y,changeTrans[i].localPosition.z);
			changeTrans[i].localScale = new Vector3(invTransScale[i].x + (rect.width - invRect.width),invTransScale[i].y + (rect.height - invRect.height),changeTrans[i].localScale.z);
		}
	}
}
