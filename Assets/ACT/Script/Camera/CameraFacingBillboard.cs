using UnityEngine;
 using System.Collections;
  
 public class CameraFacingBillboard : MonoBehaviour
 {
    private Camera cameraToLookAt;
	void Start()
	 {
		 cameraToLookAt = Camera.main;
	 }
  
    void LateUpdate() 
    {
		if(Time.frameCount % 10 == 0)
		{
	        Vector3 v = cameraToLookAt.transform.position - transform.position;
	        v.x = v.z = 0.0f;
	        transform.LookAt(cameraToLookAt.transform.position - v);
		}
    }
 }
