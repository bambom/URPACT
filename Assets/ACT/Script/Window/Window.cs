using UnityEngine;
using System.Collections;


public enum WindowStyle{
	WS_Normal,
	WS_Ext,
	WS_CullingMask,
};

public abstract class Window<T> where T : class, new() 
{
    static T msInstance = null;
	static int mCullingMask = 0;
    public static T Instance { get { return msInstance ?? (msInstance = new T()); } }
	public static bool Exist { get { return msInstance != null; } }
	
	GameObject mCamera = null;
	GameObject mRootUI = null;
    GameObject mWndObject = null;
	GameObject mExtBackground = null;
	
    public abstract string PrefabName { get; }
    public GameObject WndObject { get { return mWndObject; } }
    protected virtual bool OnOpen() { return true; }
    protected virtual bool OnClose() { return true; } 
	protected virtual bool OnShow() { return true; }
    protected virtual bool OnHide() { return true; }

	public WindowStyle mWindowStyle = WindowStyle.WS_Normal;
	public WindowStyle WinStyle { get { return mWindowStyle; } set { mWindowStyle = value; }}
    
	public GameObject Control(string name)
    {
        if (msInstance == null)
            return null;

        return Control(name, mWndObject);
    }

    public GameObject Control(string name, GameObject parent)
    {
        if (msInstance == null)
            return null;

        Transform[] children = parent.GetComponentsInChildren<Transform>();
        foreach (Transform child in children)
        {
            if (child.name == name)
                return child.gameObject;
        }
        return null;
    }

    public void Open()
    {
        if (mWndObject)
        {
            Debug.LogError("Window:" + PrefabName + "The window already opened!!!!");
            GameObject.Destroy(mWndObject);
            if (WindowStyle.WS_Ext <= mWindowStyle)
                GameObject.Destroy(mExtBackground);
            //return;
        }
        mWndObject = GameObject.Instantiate(Resources.Load(PrefabName)) as GameObject;
        // attach to the root.//attention Order
        mRootUI = GameObject.Find("Anchor");
        WndObject.transform.parent = mRootUI.transform;
        WndObject.transform.localPosition = Vector3.zero;
        WndObject.transform.localScale = Vector3.one;

        OnOpen();

        //attention Order
        mCamera = GameObject.Find("Camera-CloseUp");
        if (mCamera != null)
            mCullingMask = mCamera.GetComponent<Camera>().cullingMask;

        if (WindowStyle.WS_Ext <= mWindowStyle)
        {
            mExtBackground = GameObject.Instantiate(Resources.Load("BackgroundExtWnd")) as GameObject;
            mExtBackground.transform.parent = mRootUI.transform;
            mExtBackground.transform.localPosition = Vector3.zero + new Vector3(0.0f, 0.0f, 800.0f);
            mExtBackground.transform.localScale = Vector3.one + new Vector3(1500.0f, 1500.0f, 1.0f);
        }

        if (WindowStyle.WS_CullingMask <= mWindowStyle)
        {
            Debug.Log("LayerMask NGUI : = " + LayerMask.NameToLayer("NGUI"));
            //attention Order
            if (mCamera != null)
                mCamera.GetComponent<Camera>().cullingMask = LayerMask.NameToLayer("NGUI");
        }
    }

    public void Close()
    {
        OnClose();
		if(mWndObject != null)
	        GameObject.Destroy(mWndObject);

        if (WindowStyle.WS_Ext <= mWindowStyle 
			&& mExtBackground!=null)
            GameObject.Destroy(mExtBackground);

        if (WindowStyle.WS_CullingMask <= mWindowStyle)
            mCamera.GetComponent<Camera>().cullingMask = mCullingMask;
		
        msInstance = null;
    }

    public virtual void Show()
    {
        if (Exist)
        {
            WndObject.SetActive(true);
            if (WindowStyle.WS_Ext <= mWindowStyle)
                mExtBackground.SetActive(true);
        }

        OnShow();
    }

    public virtual void Hide()
    {
        if (Exist)
        {
            WndObject.SetActive(false);
            if (WindowStyle.WS_Ext <= mWindowStyle)
                mExtBackground.SetActive(false);
        }

        OnHide();
	}

	
	void OnClickedTimeOutQuit( GameObject go )
	{
		Application.Quit();
	}
	
    public void Request(RequestCmd request, Client.OnResponse callback)
    {
        MainScript.Instance.Request(request, delegate(string err, Response response)
        {
            if (!mWndObject)
                return;

            if (string.Compare(err, "Time out") == 0)
                Global.ShowLoadingEnd();

            callback(err, response);
        });
    }
	
	
	public void BtnSetAble(GameObject go,bool bStatus)
	{
		GameObject goDis = go.transform.Find("Disable").gameObject;
		if(goDis == null)
			return;
		
		go.GetComponent<BoxCollider>().enabled = bStatus;
		goDis.SetActive(!bStatus);
		if(bStatus)//NGUI Bug
		{
			UIImageButton uiib = go.GetComponent<UIImageButton>();
			go.transform.Find("Background").GetComponent<UISprite>().spriteName = uiib.normalSprite;
		}
	}
	
	
	public void NewInfoTipsCountSet(GameObject go,int Count)
	{
		Control("QuestState",go).GetComponent<UILabel>().text = Count.ToString();
	}
	
	public void UpdateItemQualityFrameIcon(GameObject go,ItemBase itemBase)
	{
		//Debug.Log(quality);
		QualitySprite QSp = QualitySprite.Button10_BaseItem_Quality_00;
		
		switch((int)itemBase.Quality)
		{
			case (int) QualitySprite.Button10_BaseItem_Quality_00:
				QSp = QualitySprite.Button10_BaseItem_Quality_00;
				go.SetActive(false);
				break;
			case (int) QualitySprite.Button10_BaseItem_Quality_01:
				QSp = QualitySprite.Button10_BaseItem_Quality_01;
				go.SetActive(true);
				go.GetComponent<UISprite>().spriteName= QSp.ToString();
				break;
			case (int) QualitySprite.Button10_BaseItem_Quality_02:
				QSp = QualitySprite.Button10_BaseItem_Quality_02;
				go.SetActive(true);
				go.GetComponent<UISprite>().spriteName= QSp.ToString();
				break;
			case (int) QualitySprite.Button10_BaseItem_Quality_03:
				QSp = QualitySprite.Button10_BaseItem_Quality_03;
				go.SetActive(true);
				go.GetComponent<UISprite>().spriteName= QSp.ToString();
				break;
			case (int) QualitySprite.Button10_BaseItem_Quality_04:
				QSp = QualitySprite.Button10_BaseItem_Quality_04;
				go.SetActive(true);
				go.GetComponent<UISprite>().spriteName= QSp.ToString();
				break;
			default:
				go.SetActive(false);
				break;
		}
	}
}
