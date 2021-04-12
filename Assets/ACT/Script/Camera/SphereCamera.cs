using UnityEngine;
using System.Collections;

public class SphereCamera : MonoBehaviour
{
    public GameObject mLookAt;
    public float mRadius = 8;
    public float mAngles = 45;

    float normalDistance = 0.8f;
    private Vector3 sunriseV3;
    private Vector3 sunsetV3;

    void Start()
    {

    }

    void Update()
    {
        if (!mLookAt)
            return;

        if (Input.GetAxis("Mouse ScrollWheel") != 0)
        {

            normalDistance += Input.GetAxis("Mouse ScrollWheel") / 2;
            if (normalDistance > 1)
                normalDistance = 1;
            else if (normalDistance < 0.6f)
                normalDistance = 0.6f;
        }

        Vector3 p = mLookAt.transform.position;
        sunriseV3 = new Vector3(p.x, p.y - 8, p.z);
        sunsetV3 = new Vector3(p.x, p.y + 8, p.z);

        Vector3 center = (sunriseV3 + sunsetV3) * 0.5F;
        center -= new Vector3(0, 1, 0);
        Vector3 riseRelCenter = sunriseV3 - center;
        Vector3 setRelCenter = sunsetV3 - center;

        transform.position = Vector3.Slerp(riseRelCenter, setRelCenter, normalDistance);
        transform.position += center;

        transform.LookAt(mLookAt.transform);
        //else
        //{
        //    transform.position = mLookAt.transform.position + new Vector3(0, mRadius, -mRadius);
        //    transform.forward = (mLookAt.transform.position - transform.position).normalized;
        //}
    }
}