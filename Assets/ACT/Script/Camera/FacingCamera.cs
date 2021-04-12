using UnityEngine;
using System.Collections;

public class FacingCamera : MonoBehaviour 
{
	void Start()
	{
        if (Camera.main)
			transform.rotation = Camera.main.transform.rotation;
	}
}
