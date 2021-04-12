using UnityEngine;
using System.Collections;

public class CamerSunChange : MonoBehaviour
{

    float normalDistance = 0.5f;
    private Vector3 sunriseV3;
    private Vector3 sunsetV3;
	Transform targetCamera;
    void Start() {
		targetCamera = Camera.main.transform;
        Vector3 p = targetCamera .position;
        sunriseV3 = new Vector3(p.x, p.y - 3, p.z);
        sunsetV3 = new Vector3(p.x, p.y + 3, p.z);
    }

    void Update()
    {
        if (Input.GetAxis("Mouse ScrollWheel") != 0)
        {
            normalDistance += Input.GetAxis("Mouse ScrollWheel");
            if (normalDistance > 1)
                normalDistance = 1;
            else if (normalDistance < 0)
                normalDistance = 0;
            Vector3 center = (sunriseV3 + sunsetV3) * 0.5F;
            center -= new Vector3(0, 1, 0);
            Vector3 riseRelCenter = sunriseV3 - center;
            Vector3 setRelCenter = sunsetV3 - center;
            targetCamera.position = Vector3.Slerp(riseRelCenter, setRelCenter, normalDistance);
            targetCamera.position += center;

        }
    }
}
