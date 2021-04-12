using UnityEngine;
using System.Collections;

public class AdaptiveScreen3DByChild : MonoBehaviour {

	// Use this for initialization
	void Awake () {
		for (int i = 0; i < transform.childCount; i++) {
			Transform child = transform.GetChild(i);
			float modify = Mathf.Abs(UIHelper.UIModify - 1) > 0.01f ? UIHelper.UIModify * 0.9f : UIHelper.UIModify;
			child.localPosition = new Vector3(child.localPosition.x * modify,child.localPosition.y,child.localPosition.z);
		}
	}
}
