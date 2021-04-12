using UnityEngine;
using System.Collections;

public class SelfStretchingClip : MonoBehaviour {
	
	void Awake()
	{
		UIPanel panel = GetComponent<UIPanel>();
		if(panel.clipping != UIDrawCall.Clipping.None)
		{
			Vector4 clipRange = panel.clipRange;
			float modify = Mathf.Abs(UIHelper.UIModify - 1) > 0.01f ? UIHelper.UIModify * 0.85f : UIHelper.UIModify;
			clipRange.z *= modify;
			panel.clipRange = clipRange;
		}
	}
}
