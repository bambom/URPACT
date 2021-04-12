using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SystemBroadcast : MonoBehaviour
{
    // Use this for initialization
	Queue<string> mMessages = new Queue<string>();
	UISprite mSprite;
	UILabel mLabel;
	Transform mParent;
	GameObject mNoticeWnd;
	bool mMoveLabel = false;
	
    void Start()
    {
		GameObject.DontDestroyOnLoad(this);
		mParent = GameObject.Find("Anchor").transform;
        MainScript.Instance.Client.RegisterHandler((int)Client.ESystemRequest.SystemBroadcast, OnSystemBroadcast);
		MainScript.Instance.Client.RegisterHandler((int)Client.ESystemRequest.UserGetNewMail,OnUserGetNewMail);
    }
	
    void OnDestroy()
    {
        MainScript.Instance.Client.UnRegisterHandler((int)Client.ESystemRequest.SystemBroadcast, OnSystemBroadcast);
		MainScript.Instance.Client.UnRegisterHandler((int)Client.ESystemRequest.UserGetNewMail,OnUserGetNewMail);
    }
	
	void FixedUpdate ()
	{
		if(mMoveLabel)
		{
			//Debug.Log("start move");
			Vector3 cr = mLabel.transform.localPosition;
			cr.x -=2.0f;
			mLabel.transform.localPosition = cr;
		}
	}

    void OnSystemBroadcast(string err, Response response)
    {
        ChatMessage chatMessage = (response != null) ? response.Parse<ChatMessage>() : null;
        if (chatMessage == null)
            return;
		
		//create the NoticeBar
		if(mNoticeWnd ==null){		
			mNoticeWnd = GameObject.Instantiate(Resources.Load("NoticeBar")) as GameObject;
		    mNoticeWnd.SetActive(false);
		}
				
		//put message into the queue
		if (!mNoticeWnd.activeSelf)
			OnShowMessage(chatMessage.msg);
		else		 
			mMessages.Enqueue(chatMessage.msg);					
        // show the chat message in a label.
        //chatMessage.msg;
    }
	
	void OnShowMessage(string msg)
	{	
		mNoticeWnd.SetActive(true);
		mLabel = mNoticeWnd.GetComponentInChildren<UILabel>();
		mSprite = mNoticeWnd.GetComponentInChildren<UISprite>();
		mLabel.text = msg;
		
		//set the position and the scale
		mNoticeWnd.transform.parent = mParent;		
		mNoticeWnd.transform.localPosition = new Vector3(0,260f,-100f);
		mNoticeWnd.transform.localScale = Vector3.one;
		
		//set the label position 
		mLabel.pivot = UIWidget.Pivot.Left;
		mLabel.transform.localPosition = new Vector3(mSprite.transform.localScale.x/2,mLabel.transform.localPosition.y,0);
		mLabel.pivot = UIWidget.Pivot.Right;
		
		//xDelta means the moving distance in x axis
		float xDelta = Mathf.Abs(-mSprite.transform.localScale.x/2 - mLabel.transform.localPosition.x);				
		float speed = 100.0f;
		float time = xDelta/speed;
		Debug.Log(xDelta);
		Debug.Log(time);
		StopCoroutine("BeginMove");
		StartCoroutine("BeginMove",time);
		Invoke("OnFinished", time);
    }
	
	IEnumerator BeginMove(float time)
	{
		mMoveLabel = true;
		yield return new WaitForSeconds(time);
		mMoveLabel = false;
	}
	
	void OnFinished()
	{
		if (mMessages.Count > 0)
			OnShowMessage(mMessages.Dequeue());
		else
			mNoticeWnd.SetActive(false);
	}
	
	public void DestroyNotice()
	{
		Destroy(mNoticeWnd);
		Destroy(this);
	}	
/*
	public UISprite m_sprite_bg;
	public UILable m_label;
	Queue<string> mMessages = new Queue<string>();
	
	float m_count = 0f;
	bool m_moveLabel = false;
	
    void OnDestroy()
    {
        mInstance = null;
		if(MainMonoBehavior.instance!=null && MainMonoBehavior.instance.WSClient!=null)
			MainMonoBehavior.instance.WSClient.UnRegisterHandler((int)Client.ESystemRequest.GetBrodcastMesg, OnGetBrodcastMsg);
    }
	void Awake()
    {
        mInstance = this;
    }
	
	void Start () 
	{
		Show(false);
		if(MainMonoBehavior.instance!=null && MainMonoBehavior.instance.WSClient!=null)
        	MainMonoBehavior.instance.WSClient.RegisterHandler((int)Client.ESystemRequest.GetBrodcastMesg, OnGetBrodcastMsg);
	}
	
	void Update()
	{
		MoveMessageLabel();
	}
	
	void OnGetBrodcastMsg(string err, Response response)
	{
		if (response == null || string.IsNullOrEmpty(response.data))
			return;
		
		if (!gameObject.active)
			OnShowMessage(response.data);
		else
			mMessages.Enqueue(response.data);
	}
	
	public void GetLocalBrodcastMsg( string msg )
	{
		if( string.IsNullOrEmpty( msg ) )
			return;
		
		if (!gameObject.active)
			OnShowMessage(msg);
		else
			mMessages.Enqueue(msg);
	}
	
	void OnShowMessage(string msg)
	{
		m_label.pivot = UIWidget.Pivot.Left;
		m_label.transform.localPosition = new Vector3(150,0,0);		
		m_label.text = msg;
		
		UIPanel mUIPanel = transform.GetComponent<UIPanel>();
		m_sprite_bg.transform.localScale = new Vector3(10000,m_sprite_bg.transform.localScale.y,m_sprite_bg.transform.localScale.z);
		mUIPanel.clipRange = new Vector4(0,0,10000,50);
		
		int messageLength = m_label.text.Length;
		float time = (messageLength/1.0f)+3.8f;
		
		Show(true);
        StopCoroutine("BeginMove");
		StartCoroutine("BeginMove",time);
		//Debug.Log ("   Time :"+time+"   messageLength:"+messageLength);
		Invoke("OnFinished", time);
	}
	
	IEnumerator BeginMove(float time)
	{
		m_moveLabel = true;
		yield return new WaitForSeconds(time);
		m_moveLabel = false;
	}
	
	void MoveMessageLabel()
	{
		
	}
		
	void OnFinished()
	{
		if (mMessages.Count > 0)
			OnShowMessage(mMessages.Dequeue());
		else
			Show(false);
	}
	
	void Show(bool visible)
	{
		gameObject.SetActiveRecursively(visible);
	}
}*/
	 
	void OnUserGetNewMail(string err, Response response)
    {
        if (response == null || !string.IsNullOrEmpty(err))
            return;
		
		if(!MailWnd.Exist)
			Global.NewMailCome = true;
		
		if(InGameMainWnd.Exist && InGameMainWnd.Instance.WndObject.activeSelf){
			GameObject mailRemindObj = GameObject.Find("MailBox").transform.Find("MailRemind").gameObject;
			mailRemindObj.SetActive(true);
			mailRemindObj.GetComponent<TweenScale>().enabled = true;
		    SoundManager.Instance.PlaySound("UI_XiTongTiShi");
		}
    }
}










