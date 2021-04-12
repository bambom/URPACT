using UnityEngine;
using System.Collections;

public class RoleChooseModel : MonoBehaviour
{

	// Use this for initialization
	void Start ()
	{
		float designRatio = (float)WindowsRoot.DesignWidth / WindowsRoot.DesignHeight;
        float actualRatio = (float)Screen.width / Screen.height;
		if (actualRatio < designRatio){
			Vector3 vPos = Vector3.zero;
			vPos = transform.localPosition;
			vPos.x = vPos.x * actualRatio / designRatio;
			transform.localPosition = vPos;
		}
	}
	
	// Update is called once per frame
	void Update ()
	{
	
	}
}

