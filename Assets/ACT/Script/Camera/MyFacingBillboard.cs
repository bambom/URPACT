using UnityEngine;
using System.Collections;

public class MyFacingBillboard : MonoBehaviour 
{
	Transform cameraTarget;

	void Start()
	{
        if (Camera.main)
		    cameraTarget = Camera.main.transform;
	}

    void LateUpdate() 
    {
		if (cameraTarget!=null)
        	transform.rotation = cameraTarget.rotation;
    }
}
