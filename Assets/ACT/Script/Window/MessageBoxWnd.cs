using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MessageBoxWnd : Window<MessageBoxWnd>
{
	public enum Style
	{
		OK_CANCLE,
		OK,
		CANCLE,
		NONE
	};
	
	public enum StyleExt
	{
		Toast,
		Alpha,
		Solid,
	};
	
    public override string PrefabName { get { return "MessageBoxWnd"; } }

	private UILabel mContent;	
	private UILabel mTitle;
	private GameObject mBtnOK;
	private GameObject mBtnCancle;
	private GameObject mGrayFilter;
	
	private static Style mWndStyle = Style.OK_CANCLE;
	private static StyleExt mWndStyleExt = StyleExt.Solid;
	
	public static Style MsgBoxStyle { get { return mWndStyle; } set { mWndStyle = value; }} 
	public static StyleExt MsgBoxStyleExt { get { return mWndStyleExt; } set { mWndStyleExt = value; }} 
	
	public delegate void OnClickOK(GameObject go);
	public delegate void OnClickCancle(GameObject go);
	public OnClickOK OnClickedOk;
	public OnClickCancle OnClickedCancle;
	
	public string Content { get {return mContent.text;} set { Global.ShowLoadingEnd(); mContent.text = value;}}
	public string Title { get {return mTitle.text;} set {mTitle.text = value;}}

    protected override bool OnOpen()
    {
		Init();
		BringToTop();
        return base.OnOpen();
    }

    protected override bool OnClose()
    {
		MsgBoxStyle = Style.OK_CANCLE;
		MsgBoxStyleExt = StyleExt.Solid;
		return base.OnClose();
    }
	
	public void Show(string content)
	{
		this.Open();
		if( !string.IsNullOrEmpty(content) ) Content = LanguagesManager.Instance.GetItem(content);
	}
	
	public void Show(string content,params object[] Args)
	{
		string strContent = string.Format(LanguagesManager.Instance.GetItem(content),Args);
		this.Open();
		if( !string.IsNullOrEmpty(content) ) Content = strContent;// LanguagesManager.Instance.GetItem(content);
	}
	
	public void Show(string content,Style styl)
	{
		MsgBoxStyle = styl;
		this.Open();
		if( !string.IsNullOrEmpty(content) ) Content = LanguagesManager.Instance.GetItem(content);
	}
	
	public void Show(string content,Style styl,params object[] Args)
	{
		MsgBoxStyle = styl;
		string strContent = string.Format(LanguagesManager.Instance.GetItem(content),Args);
		this.Open();
		if( !string.IsNullOrEmpty(content) ) Content = strContent;
	}
	
	
	
	public void Show(string content,StyleExt styl)
	{
		MsgBoxStyleExt = styl;
		this.Open();
		if( !string.IsNullOrEmpty(content) ) Content = LanguagesManager.Instance.GetItem(content);
	}
	
	public void Show(string content,StyleExt styl,params object[] Args)
	{
		MsgBoxStyleExt = styl;
		string strContent = string.Format(LanguagesManager.Instance.GetItem(content),Args);
		this.Open();
		if( !string.IsNullOrEmpty(content) ) Content = strContent;
	}
	
	public void Show(string title,string content)
	{
		this.Open();
		if( !string.IsNullOrEmpty(title) ) Title = title;
		if( !string.IsNullOrEmpty(content) ) Content = LanguagesManager.Instance.GetItem(content);
	}
	
	private void BringToTop()
	{
		WndObject.transform.localPosition = new Vector3(0.0f,0.0f,-250.0f);
	}
	
	private void Init()
	{
		mTitle = Control("Title").GetComponent<UILabel>();
		mContent = Control("Content").GetComponent<UILabel>();
		mBtnOK = Control("IbtnSure");
		mBtnCancle = Control("IbtnCancle");
		mGrayFilter = Control("Alpha");
		
		UIEventListener.Get(mBtnOK).onClick = OnClickIbtnSure;
		UIEventListener.Get(mBtnCancle).onClick =  OnClickIbtnCancle;
		
		SetWndStyle();
		SetWndStyleExt();
	}
	
	private void SetWndStyle()
	{
		switch(mWndStyle)
		{
			case Style.NONE:
				{
					mBtnCancle.SetActive(false);
					mBtnOK.SetActive(false);
					mGrayFilter.SetActive(false);
				}
				break;
			case Style.OK_CANCLE:
				break;
			case Style.OK:
				{
					mBtnCancle.SetActive(false);
					Vector3 vSource = mBtnOK.transform.localPosition;
					Vector3 vPos = new Vector3(0.0f,vSource.y,vSource.z);
					mBtnOK.transform.localPosition = vPos;
				}
				break;
			case Style.CANCLE:
				{
					mBtnOK.SetActive(false);
					Vector3 vSource = mBtnCancle.transform.localPosition;
					Vector3 vPos = new Vector3(0.0f,vSource.y,vSource.z);
					mBtnCancle.transform.localPosition = vPos;
				}
				break;
			default:
				break;
		}
	}
	
	private void ChangeWndStyleExt()
	{
		mBtnCancle.SetActive(false);
		mBtnOK.SetActive(false);
		mGrayFilter.SetActive(false);
		
		Control("Title").SetActive(false);

		GameObject goParent = Control ("BgRoot");
		Vector3 VScale = goParent.transform.Find("Bg").transform.localScale;
		VScale = new Vector3(VScale.x,VScale.y/3,VScale.z);
		goParent.transform.Find("Bg").transform.localScale = VScale;

		VScale = goParent.transform.Find("Frame").transform.localScale;
		VScale = new Vector3(VScale.x,VScale.y/3,VScale.z);
		goParent.transform.Find("Frame").transform.localScale = VScale;
	
		VScale = Control("Content").transform.localPosition;
		VScale = new Vector3(VScale.x,0.0f,VScale.z);
		Control("Content").transform.localPosition = VScale;
	}
	
	private void SetWndStyleExt()
	{
		switch(mWndStyleExt)
		{
			case StyleExt.Alpha:
				{
					if(null == WndObject.GetComponent<TweenAlpha>()){
						TweenAlpha Ta = WndObject.AddComponent<TweenAlpha>();
						Ta.to = 0.0f;
						Ta.delay = 2.0f;
					}
					if(null == WndObject.GetComponent<AutoClose>()){	
						WndObject.AddComponent<AutoClose>();
					}
				}
				ChangeWndStyleExt();
				break;
			
			case StyleExt.Toast:
				{
					if(null == WndObject.GetComponent<AutoClose>()){	
						WndObject.AddComponent<AutoClose>();
					}
				}
				ChangeWndStyleExt();
				break;
			default:
				break;
		}
	}
	
	
	private void OnClickIbtnCancle( GameObject go )
	{
		if( OnClickedCancle!= null )
		{
			OnClickedCancle( go );
			OnClickedCancle = null;
		}
		Close ();
	}
	
	private void OnClickIbtnSure( GameObject go )
	{
		if( OnClickedOk != null )
		{
			OnClickedOk( go );
			OnClickedOk = null;
		}
		Close ();
	}
}