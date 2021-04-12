using UnityEngine;
using System.Collections;

public class CameraFieldOfView : MonoBehaviour {
	public Camera target;
	public float max = 60;
	public float min = 20;
	public float speed = 0.2f;
	Vector3 mouseDownPosition;
	float lastDistance = 0;
	// Use this for initialization
	void Start () {
		if(target == null) target = Camera.main;
	}
	
	void OnClick()
	{
			lastDistance = 0;
	}
	
	// Update is called once per frame
	void OnDrag (Vector2 delta)
	{
		if(Application.platform == RuntimePlatform.IPhonePlayer || Application.platform == RuntimePlatform.Android)
		{
			if(Input.touchCount  > 1 )
			{
				Touch touch1 = Input.GetTouch(0);
				Touch touch2 = Input.GetTouch(1);
				
				float distance = Vector3.Distance(touch1.position,touch2.position);
				if(lastDistance == 0)
				{
					lastDistance = distance;
				}
				float delay = (distance - lastDistance) * speed;
				target.fieldOfView = Mathf.Clamp( target.fieldOfView - delay ,min,max); 
				lastDistance = distance;
			}
		}
		else
		{
			if(Input.GetMouseButtonDown(1))
			{
				mouseDownPosition = Input.mousePosition;
			}
			if(Input.GetMouseButton(1))
			{
				float distance = Vector3.Distance(mouseDownPosition,Input.mousePosition);
				if(lastDistance == 0)
				{
					lastDistance = distance;
				}
				float delay = (distance - lastDistance) * speed;
				target.fieldOfView = Mathf.Clamp( target.fieldOfView - delay ,min,max); 
				lastDistance = distance;
			}
		}
	}
}
