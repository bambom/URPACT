using UnityEngine;
using System.Collections;


public class XinshouyindaoRoot : MonoBehaviour
{
    public static int DesignWidth = 1200;
    public static int DesignHeight = 800;
	
	static float mManualHeight = 800;
	public static float ManualHeight { get{ return mManualHeight;} set{ mManualHeight = value;}}
	
	// Use this for initialization
	void Start ()
    {
        float designRatio = (float)DesignWidth / DesignHeight;
        float actualRatio = (float)Screen.width / Screen.height;
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
	}
}