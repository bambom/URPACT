using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AboutGameWnd : Window<AboutGameWnd>
{
    public override string PrefabName { get { return "AboutGameWnd"; } }
	
    protected override bool OnOpen()
    {
		Init();
        return base.OnOpen();
    }

    protected override bool OnClose()
    {
        return base.OnClose();
    }
	
	void Init()
	{
		UIEventListener.Get(Control("BtnClose")).onClick = OnClickClose;
		UIEventListener.Get(Control("GodVersion")).onClick = OnClickVersion;
		UIEventListener.Get(Control("GodWebSite")).onClick = OnClickWebSite;
		UIEventListener.Get(Control("ServiceWebSite")).onClick = OnClickServiceWebSite;		
	}
	
	void OnClickClose(GameObject go)
	{
		Close();
	}
	
	void OnClickVersion(GameObject go)
	{
		//Close();
	}
	
	void OnClickWebSite(GameObject go)
	{
		//Close();
		Application.OpenURL("http://gNetop.com/GOD");
	}
	
	void OnClickServiceWebSite(GameObject go)
	{
		//Close();
		Application.OpenURL("http://gNetop.com/GOD");
	}
}

