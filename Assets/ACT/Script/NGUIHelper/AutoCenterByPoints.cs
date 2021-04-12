using UnityEngine;
using System.Collections;

public class AutoCenterByPoints : MonoBehaviour {
	public Vector3[] points;
	public UIDraggablePanel mDrag;
	void Awake()
	{
		mDrag.onDragFinished = OnDragFinished;
	}
	void OnEnable () { 
		Recenter(); 
	}
	void OnDragFinished () { if (enabled) Recenter(); }
	
	public void Recenter()
	{
		if (mDrag == null || mDrag.panel == null)
		{
			return;
		}
		Vector3 point = Vector3.zero;
		float distance = 100000;
		foreach (Vector3 item in points) {
			float temp = Vector3.Distance(item,transform.localPosition);
			if(temp <= distance)
			{
				distance = temp;
				point = item;
			}
		}
		
		SpringPanel.Begin(gameObject, point, 8f);
	}
}
