using UnityEngine;
using System.Collections;


[AddComponentMenu("Camera-Control/Mouse Orbit")]
public class MouseOrbit : MonoBehaviour
{
    public Transform target;
    public float distance = 10.0f;

    public float xSpeed = 250.0f;
    public float ySpeed = 120.0f;

    public float yMinLimit = -20;
    public float yMaxLimit = 80;

    public float x = 0.0f;
    public float y = 45.0f;

    private Quaternion rotation;
    private Vector3 position;

    void Start()
    {
        //Vector3 angles = transform.eulerAngles;
        //x = angles.y;
        //y = angles.x;

        if (GetComponent<Rigidbody>())
            GetComponent<Rigidbody>().freezeRotation = true;
    }

    void LateUpdate()
    {
//        if (Input.GetAxis("Mouse ScrollWheel") != 0)
//        {
//
//            distance -= Input.GetAxis("Mouse ScrollWheel");
//            if (distance > 10)
//                distance = 10;
//            else if (distance < 1f)
//                distance = 1f;
//        }
//		
//        if (target && Input.GetMouseButton(1))
//        {
//            x += Input.GetAxis("Mouse X") * xSpeed * 0.02f;
//            y -= Input.GetAxis("Mouse Y") * ySpeed * 0.02f;
//
//            y = ClampAngle(y, yMinLimit, yMaxLimit);
//        }
		
        Quaternion rotation = Quaternion.Euler(y, x, 0);
        Vector3 position = rotation * new Vector3(0.0f, 0.0f, -distance) + target.position;
        transform.rotation = rotation;
        transform.position = position;
    }

    static float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360)
            angle += 360;
		
        if (angle > 360)
            angle -= 360;
		
        return Mathf.Clamp(angle, min, max);
    }
}
