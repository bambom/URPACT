using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CameraShakeEffect : MonoBehaviour
{
    class ShakeInfo
    {
        public float Time;
        public float Frequence;
        public float Amplitude;
        public float LeftTime;

        public ShakeInfo(float t, float f, float a)
        {
            Time = t;
            Frequence = f;
            Amplitude = a;
            LeftTime = 0;
        }
    }

    List<ShakeInfo> mQueuedList = new List<ShakeInfo>();
    Vector3 mOffset = Vector3.zero;
    Controller mController;

    // Use this for initialization
    void Start()
    {
        mController = gameObject.GetComponent<Controller>();
    }

    // Update is called once per frame
    void Update()
    {
        bool anyShake = false;
        mOffset = Vector2.zero;

        foreach (ShakeInfo shakeInfo in mQueuedList)
        {
            if (shakeInfo.Time <= Time.deltaTime)
                continue;

            shakeInfo.LeftTime += Time.deltaTime;
            if (shakeInfo.LeftTime * shakeInfo.Frequence > 1.0f)
            {
                Vector3 offset = new Vector3(
                    Random.Range(-shakeInfo.Amplitude, shakeInfo.Amplitude),
                    Random.Range(-shakeInfo.Amplitude, shakeInfo.Amplitude),
                    Random.Range(-shakeInfo.Amplitude, shakeInfo.Amplitude));
                mOffset += offset;
                shakeInfo.LeftTime = 0;
            }

            float newTime = shakeInfo.Time - Time.deltaTime;
            shakeInfo.Amplitude *= newTime / shakeInfo.Time;
            shakeInfo.Time = newTime;

            anyShake = true;
        }

        mController.CameraOffset = mOffset;

        if (!anyShake)
            Destroy(this);
    }

    void Reset()
    {
        mQueuedList.Clear();
        mOffset = Vector3.zero;
    }

    void Queue(float time, float frequence, float amplitude)
    {
        mQueuedList.Add(new ShakeInfo(time, frequence, amplitude));
    }

    public static void Attach(GameObject obj, float time, float frequence, float amplitude)
    {
        CameraShakeEffect cameraShake = obj.GetComponent<CameraShakeEffect>();
        if (!cameraShake)
            cameraShake = obj.AddComponent<CameraShakeEffect>();

        cameraShake.Queue(time, frequence, amplitude * 0.01f);
    }
}
