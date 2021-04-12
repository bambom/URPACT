using UnityEngine;
using System.Collections;

public class SelfStretching : MonoBehaviour {

	void Awake()
	{
		transform.localScale = new Vector3(transform.localScale.x * UIHelper.UIModify,transform.localScale.y,transform.localScale.z);
	}
}
