using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CloseupEffect : MonoBehaviour {
	
	List<CloseupBase>mArray = new List<CloseupBase>();
	
	Camera mCamera;
	// Use this for initialization
	int index = 0;
	float totalTime = 0;
	bool mEffectEnd = false;
	int mEndCall = 0;
	float mOldGrayScale = 0f; 
	
	int mOrigCullling = 0;
	bool mOrigGrayscaleEnable = false;
	float mOrigGrayscal = 0f;
	bool mOrigMotionBlur = false;
	Color mOrigcol;
	
	Color mOrigFogColor;
	Color mOrigBackColor;
	
	List<GameObject> mScreenEffecet = new List<GameObject>();
	List<GameObject> mUnitEffecet = new List<GameObject>();
	
	void Start () {
		
		mCamera = (Camera)GameObject.FindWithTag("MainCamera").GetComponent<Camera>();
		OrigStatue();
	}
	
	// Update is called once per frame
	void Update () {
		
		if (index > 0 && index < mArray.Count && !mEffectEnd)
			SetGrayscale(mArray[index].GrayScale);
	
	}
	
	void PlayAll()
	{
		index++;
		if (index <  mArray.Count)
		{
			//PlayMainEffect(mArray[0].MainEffect);
			SetBackGroungColor(mArray[index].BGEffect);
			PlayScreenEffect(mArray[index].PlayEffect);
			CullingMask(mArray[index].CullingMask);
			EnbleMotionBlur(mArray[index].MotionBlur);
			SetGrayscale(mArray[index].GrayScale);
			PlayCameraAnimation(mArray[index].CameraAnimation);
			SetAmbientLight(mArray[index].AmbientLight);
		}
	}
	
	public void PlayCloseupEffect(List<CloseupBase> CloseupArray)
	{
		Reset();
		ClearEffect();
		EffectDataParse(CloseupArray);
	}
	
	public void PlayCloseupEffect(List<CloseupBase> CloseupArray, Unit unit)
	{
		PlayCloseupEffect(CloseupArray);
	}
	
	void EffectDataParse(List<CloseupBase> CloseupArray)
	{
		mArray = CloseupArray;
		totalTime =(float) mArray[0].TotalTime;

		float lastTime = 0f;
		mEffectEnd = false;
		if (mArray[0].Type == 0 && mArray[0].Param > 0)
			PauseGame();
		else if (mArray[0].Type == 1)
			SetGameSpeed(mArray[0].Param);
		
		mEndCall = mArray[0].EndCall;
			
		Invoke("Reset", totalTime/1000);
		for (int i=1; i<mArray.Count; i++ )
		{
			float delayTime = (float)mArray[i].Time;
			Invoke("PlayAll", (delayTime-lastTime)/1000);
			lastTime = delayTime;
		} 
	}
	
	void SetBackGroungColor(int enble)
	{
		if(enble == -1)
			return;
		else if(enble == 1)
		{
			RenderSettings.fogColor = Color.black;
			mCamera.backgroundColor = Color.black;
		}
		else
		{
			RenderSettings.fogColor = mOrigFogColor;
			mCamera.backgroundColor = mOrigBackColor;
		}
	}
	
 	void CullingMask(string cullingMask)
	{
		string[] cullingArray = cullingMask.Split('|');
		if (int.Parse(cullingArray[0]) == -1)
			return;
		int mask = 0;
		if (int.Parse(cullingArray[0]) != 0)
		{
			foreach(string cm in cullingArray)
				mask += 1 << int.Parse(cm);
		}
		mCamera.cullingMask = mask;
	}
	
	void EnbleMotionBlur(int enable)
	{
		if (enable < 0)
			return;
		bool able = false;
		able = enable > 0 ? true : false;
		mCamera.GetComponent<MotionBlur>().enabled = able;
	}
	
	void SetGrayscale(string grayScale)
	{
		string[] scaleArray = grayScale.Split('|');
		if (float.Parse(scaleArray[0]) == 1)
		{
			mCamera.GetComponent<GrayscaleEffect>().enabled = true;
			mCamera.GetComponent<GrayscaleEffect>().rampOffset += float.Parse(scaleArray[1]); 
			mOldGrayScale = float.Parse(scaleArray[1]);
		}
		else if (float.Parse(scaleArray[0]) == 0)
		{
			mCamera.GetComponent<GrayscaleEffect>().enabled = false;
		}
		else if (float.Parse(scaleArray[0]) == -1 && mCamera.GetComponent<GrayscaleEffect>().enabled)
		{
			mCamera.GetComponent<GrayscaleEffect>().rampOffset += mOldGrayScale; 
		}
	}
	
	void PlayCameraAnimation(string name)
	{
		if (name == "-1")
			return;
		mCamera.GetComponent<Animation>().Stop();
		mCamera.GetComponent<Animation>().Play(name);
	}
	
	void SetAmbientLight(string ambienLightColor)
	{
		string[] col = ambienLightColor.Split('|');
		if (col[0] == null || float.Parse(col[0]) < 0)
			return;
		Color color = new Color(float.Parse(col[0])/255, float.Parse(col[1])/255, float.Parse(col[2])/255, 1f);
		RenderSettings.ambientLight = color;
	}
	
	void OrigStatue()
	{
		mOrigCullling = mCamera.cullingMask;
		mOrigGrayscaleEnable = mCamera.GetComponent<GrayscaleEffect>().enabled;
		if (mOrigGrayscaleEnable)
			mOrigGrayscal = mCamera.GetComponent<GrayscaleEffect>().rampOffset;
		mOrigMotionBlur = mCamera.GetComponent<MotionBlur>().enabled;
		mOrigcol = RenderSettings.ambientLight;
		
		mOrigFogColor = RenderSettings.fogColor;
		mOrigBackColor = mCamera.backgroundColor;
		
	}
	
	void SetGameSpeed(float speed)
	{
		if (speed == -1)
			return;
		Time.timeScale *= speed;
		Time.timeScale = Time.timeScale < 0.1f ? 0.1f : Time.timeScale;
	}
	
	void PauseGame()
	{
		Time.timeScale = 0f;
	}
	
	void PlayScreenEffect(string effectName)
	{
		if (effectName != "-1")
		{
			ScreenEffect effect = GameObject.FindWithTag("MainCamera").GetComponent<ScreenEffect>();
      		if (effect)
			{
            	mScreenEffecet.Add(effect.PlayScreenEffect(effectName));
			}
		}
	}
	
	void PlayMainEffect(string prefabName)
	{
		//if (mOwner != null && prefabName != "-1")
		//{
		//	UnitEffect efc = mOwner.UUnitInfo.gameObject.GetComponent<UnitEffect>();
		//	mUnitEffecet.Add(efc.PlayUnitEffect(prefabName));
		//}
		
	}
	void Reset()
	{
		mCamera.cullingMask = mOrigCullling;
		mCamera.GetComponent<MotionBlur>().enabled = mOrigMotionBlur;
		mCamera.GetComponent<GrayscaleEffect>().enabled = mOrigGrayscaleEnable;
		RenderSettings.fogColor = mOrigFogColor;
		mCamera.backgroundColor = mOrigBackColor;
		if (mOrigGrayscaleEnable)
			mCamera.GetComponent<GrayscaleEffect>().rampOffset = mOrigGrayscal; 
		RenderSettings.ambientLight = mOrigcol;
		mEffectEnd = true;
		Time.timeScale = 1.0f;
		index = 0;

		
		//end the current and begining the next closeup.
		if (mEndCall > 0)
		{
			List<CloseupBase> callList = CloseupManager.Instance.GetCollection(mEndCall);
			if (callList.Count == 0)
			{
				Debug.LogWarning("closeup not find!");
				return;
			}
			mEndCall = 0;
			PlayCloseupEffect(callList);
		}
	}
	
	void ClearEffect()
	{
		if (mUnitEffecet.Count != 0)
		{
			for (int i=0; i<mUnitEffecet.Count; i++)
			{
				Destroy(mUnitEffecet[i]);
			}
			mUnitEffecet.Clear();
		}
		if (mScreenEffecet.Count != 0)
		{
			for (int i=0; i<mScreenEffecet.Count; i++)
			{
				Destroy(mScreenEffecet[i]);
			}
			mScreenEffecet.Clear();
		}
	}
}
