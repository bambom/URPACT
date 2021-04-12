using UnityEngine;
using System.Collections;

public class NGUIWorldToPoint : MonoBehaviour {
	
 	Transform m_target;
	Camera m_camera;
	Vector3 m_extents;
	
	public Transform target{ set { m_target = value; } }
	public Vector3 extents { set { m_extents = value; } }
	
	void Start()
	{
		m_camera = Camera.main;
		//m_camera = WindowManager.camera;
	}
	// Update is called once per frame
	void FixedUpdate () {
		Vector3 p = m_camera.WorldToScreenPoint(m_target.position);
		transform.localPosition = new Vector3(p.x - Screen.width / 2, p.y - Screen.height / 2 + m_extents.y * 150, 0);
	}
}
