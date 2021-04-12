using UnityEngine;
using System.Collections;

public class WindowsRoot : MonoBehaviour
{
    public static float DesignWidth = 1200.0f;
	public static float DesignHeight = 800.0f;
	
	static float mManualHeight = 800.0f;
	public static float ManualHeight { get{ return mManualHeight;} set{ mManualHeight = value;}}
	
	// Use this for initialization
	void Start ()
    {
        float designRatio = DesignWidth / DesignHeight;
        float actualRatio = (float)Screen.width / (float)Screen.height;
		Debug.Log("designRatio = " + designRatio);
		Debug.Log("actualRatio = " + actualRatio);
		UIRoot UIRoot = GetComponent<UIRoot>();
		mManualHeight = UIRoot.manualHeight;
		Debug.Log( "mManualHeight = " + mManualHeight);
        if (actualRatio < designRatio){
            UIRoot.manualHeight = (int)Mathf.Round( UIRoot.manualHeight * designRatio / actualRatio );
			mManualHeight = UIRoot.manualHeight;
			Debug.Log( "mManualHeight = " + mManualHeight);
        }
		GameObject.DontDestroyOnLoad(this);
	}
}
