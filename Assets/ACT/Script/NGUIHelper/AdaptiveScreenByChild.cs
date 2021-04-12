using UnityEngine;
using System.Collections;

public class AdaptiveScreenByChild : MonoBehaviour {

	void Awake()
	{
		for (int i = 0; i < transform.childCount; i++) {
			Transform child = transform.GetChild(i);
			child.localPosition = new Vector3(child.localPosition.x * UIHelper.UIModify,child.localPosition.y,child.localPosition.z);
		}
	} 
}
