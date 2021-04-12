using UnityEngine;
using System.Collections;

public class LookAtCamara : MonoBehaviour 
{
	public bool Lock=false;
		
	Transform TheCamera;
	// Use this for initialization
	void Start () 
	{
		TheCamera=Camera.main.transform;
	}
	
	// Update is called once per frame
	void FixedUpdate () 
	{
		transform.rotation=TheCamera.transform.rotation;
		if(Lock)
		{
			transform.rotation=new Quaternion (0,transform.rotation.y,0,transform.rotation.w);
		}
	}
}
