using UnityEngine;
using System.Collections;

public class CameraShake : MonoBehaviour {
	
	float mFrequence = 0f;
	float mAmplitude = 0f;
	Vector3 mOrigPos = Vector3.zero;
	float mTotal = 0f;
	float mCount = 0f;
	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	public void Shake(float time, float frequence, float amplitude)
	{
		mOrigPos = transform.position;
		mTotal = frequence;
		mFrequence = frequence / time;
		mAmplitude = amplitude/100f;
		mCount = 0f;
		Play();
	}
	
	void Play()
	{
		transform.position = mOrigPos + new Vector3(Random.Range(-1,1)*mAmplitude, Random.Range(-1,1)*mAmplitude, 0f);
		mCount++;
		if (mCount < mTotal)
			Invoke("Play", mFrequence/1000);
		else
			transform.position = mOrigPos;
			
	}
}
