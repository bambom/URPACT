using UnityEngine;
using System.Collections;

public class AutoClose : MonoBehaviour
{

	// Use this for initialization
	void Start ()
	{
		Invoke("CloseMessageBox",2.0f);
	}
	
	void CloseMessageBox()
	{
        if (MessageBoxWnd.Exist)
		    MessageBoxWnd.Instance.Close();
	}
}

