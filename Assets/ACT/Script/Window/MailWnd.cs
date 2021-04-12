using UnityEngine;
using System;
using System.Collections;

/// <summary>
/// 邮箱窗体类，用于显示系统邮件和好友邮件，收发邮件.
/// </summary>
public class MailWnd : Window<MailWnd>
{
	/// 邮件所在当前页数
    int mCurrentPage = 0;
	/// 收件箱邮件总数
    private int mTotalMailNum = 0;
	//邮件在收件箱中的index
	private int mIntMailIndex = 0;
	private int mDeleteMailId = -1;
	/// <summary>
	/// Constant magnify times.
	/// </summary>
	const float MagnifyTimes = 1.2f;
	/// 一页显示的邮件数
	private const int CONST_MAILS_PERPAGE 									= 4;
	/// 邮箱容量
	private const int CONST_MAILBOX_CAPACITY								= 200;
	/// 暂时先const string，若游戏中const string较多，可放入配置文件txt中读取
	private const string CONST_SENDMAIL_FAIL								= "发送邮件失败,请检查网络.";
	private const string CONST_SENDMAIL_SUCCESS								= "发送邮件成功.";	
	/// 触发邮件列表刷新的类型
	private E_REFRESHPAGEMAILS_STYLE m_eRefreshPageMails_Style				= E_REFRESHPAGEMAILS_STYLE.E_REFRESHPAGEMAILS_INVALID;	
	/// 寄件地址、标题、内容
   	private UIInput mInputSendTo;
    private UIInput mInputSendTitle;
    private UIInput mInputSendContent;
	/// 收件箱，邮件正文，寄件箱面板对象
	private GameObject mFixMailPanel;
	private GameObject mReceiveMailPanel;
	private GameObject mReadMailPanel;
	private GameObject mSendMailPanel;
	/// 邮件列表Grid对象
	private Transform mReceiveMailGrid;
	/// 邮件列表中所有邮件item对象
	private GameObject[] mAllMailItemsArr = new GameObject [ CONST_MAILS_PERPAGE ];
	private GameObject mAttachItemRoot;
	private GameObject mReceiveButton;
	private GameObject mReplyButton;
	private GameObject mMailBox;
	private GameObject mChooseBtn;
	
	private UILabel mPageNumLabel;
	private UILabel mMailCapacityLabel;
	/// 触发邮件列表刷新的类型
	private enum E_REFRESHPAGEMAILS_STYLE
	{
		E_REFRESHPAGEMAILS_INVALID					= -1,
		E_REFRESHPAGEMAILS_INIT						= 0,               // 初始化窗体时，刷新列表
		E_REFRESHPAGEMAILS_DELETEMAIL				= 1,               // 删除邮件时，刷新列表               
		E_REFRESHPAGEMAILS_NEWMAIL					= 2,               // 有新邮件时，刷新列表
		E_REFRESHPAGEMAILS_NEXTPAGE					= 3,               // 下一页时，刷新列表
		E_REFRESHPAGEMAILS_PREPAGE					= 4,               // 上一页时，刷新列表
		E_REFRESHPAGEMAILS_MAX
	}
	/// 邮件总页数
    private int TotalPage { get { return (mTotalMailNum + CONST_MAILS_PERPAGE - 1) / CONST_MAILS_PERPAGE <=0 ? 0 :
				(mTotalMailNum + CONST_MAILS_PERPAGE - 1) / CONST_MAILS_PERPAGE; } }
		
    public override string PrefabName { get { return "MailBoxWnd"; } }

    protected override bool OnOpen()
    {
		///获取面板对象
		mFixMailPanel = Control("FixedObjPanel");
		mReceiveMailPanel = Control("ReceiveMailPanel");
		mReadMailPanel = Control("ReadMailPanel");
		mSendMailPanel = Control("SendMailPanel");
		mReplyButton = Control("ReplyButton",mReadMailPanel);
		mReceiveMailGrid = Control("UIgrid",mReceiveMailPanel).transform;
		mPageNumLabel = Control("PageNum").GetComponent<UILabel>();
		mMailCapacityLabel = Control("MailCapacity").GetComponent<UILabel>();
		
		///邮箱面板，控件信息
		mMailBox = Control("MyBox",mFixMailPanel);
		UIEventListener.Get(mMailBox).onClick = OnClickMyMailBox;
		UIEventListener.Get(Control("SendMail",mFixMailPanel)).onClick = OnClickSendMailBox;
		UIEventListener.Get(Control("BackButton",mFixMailPanel)).onClick = OnClickCloseMailBox;
		///收件面板，控件信息
        UIEventListener.Get(Control("PrePageBtn",mReceiveMailPanel)).onClick = OnClickPrePage;
        UIEventListener.Get(Control("NextPageBtn",mReceiveMailPanel)).onClick = OnClickNextPage;
		UIEventListener.Get(mReplyButton).onClick = OnClickReplyMail;

		for(int i=0;i<CONST_MAILS_PERPAGE;i++){
			GameObject mailItemObj = Control("Mail"+i.ToString(),mReceiveMailPanel);
			///获取邮件对象数组
			mAllMailItemsArr[i] = mailItemObj;
			///点击单个邮件
			UIEventListener.Get(mailItemObj).onClick = OnClickOpenSingleMail;
        	UIEventListener.Get(Control("Delete",mailItemObj)).onClick = OnClickDeleteMail;			
			///邮件默认不显示
			mAllMailItemsArr[i].SetActive(false);
		}		
		mReceiveButton = Control("ReceiveButton",mReadMailPanel);
        UIEventListener.Get(mReceiveButton).onClick = OnClickFetchAttachment;
		mAttachItemRoot = Control("IconRoot",mReadMailPanel);
	
		mAttachItemRoot.SetActive(false);
		mReceiveButton.SetActive(false);
		
        UIEventListener.Get(Control("SendMailButton",mSendMailPanel)).onClick = OnClickSendMail;
		///寄件面板，获取输入的寄件信息
        mInputSendTo = Control("NameInput",mSendMailPanel).GetComponent<UIInput>();
        mInputSendTitle = Control("TitleInput",mSendMailPanel).GetComponent<UIInput>();
        mInputSendContent = Control("MailTextInput",mSendMailPanel).GetComponent<UIInput>();
		
		///刷新类型
		m_eRefreshPageMails_Style = E_REFRESHPAGEMAILS_STYLE.E_REFRESHPAGEMAILS_INIT;	
		///刷新当前页邮件	
		UpdateViewPanel(true,false,false);		
       	RefreshCurrentPageMails();	
		UpdateBtnScale(mMailBox);
		
		WinStyle = WindowStyle.WS_Ext;
        return base.OnOpen();
    }

    protected override bool OnClose()
    {
        return base.OnClose();
    }
	
      /// <summary>
   	/// 更新当前页邮件列表.
	/// </summary>
    void RefreshCurrentPageMails()
    {
        Request(new GetMailsCmd(mCurrentPage, CONST_MAILS_PERPAGE), delegate(string err, Response response)
        {
            GetMailResponse getMailResponse = (response != null) ? response.Parse<GetMailResponse>() : null;
            if (getMailResponse != null)
            {
				///收到新邮件
				if(mTotalMailNum!=0 && getMailResponse.total>mTotalMailNum)
				{
					m_eRefreshPageMails_Style = E_REFRESHPAGEMAILS_STYLE.E_REFRESHPAGEMAILS_NEWMAIL;
				}
				
				///第一次创建邮箱窗体
				if(m_eRefreshPageMails_Style == E_REFRESHPAGEMAILS_STYLE.E_REFRESHPAGEMAILS_INIT)
				{
                	mTotalMailNum = getMailResponse.total;
				///删除邮件时
				}else if(m_eRefreshPageMails_Style == E_REFRESHPAGEMAILS_STYLE.E_REFRESHPAGEMAILS_DELETEMAIL)
				{
					Debug.Log("删除邮件时，更新邮件list");
                	mTotalMailNum = getMailResponse.total;
					RecoverDefaultMailList();
					///删除的是本页的唯一邮件，自动到上页
					if( mTotalMailNum % CONST_MAILS_PERPAGE == 0 )
						OnClickPrePage(null);
				///新邮件时
				}else if(m_eRefreshPageMails_Style == E_REFRESHPAGEMAILS_STYLE.E_REFRESHPAGEMAILS_NEWMAIL)
				{
                	mTotalMailNum = getMailResponse.total;
					RecoverDefaultMailList();
					///本页已满，新邮件来事，自动到下页
					if( mTotalMailNum % CONST_MAILS_PERPAGE == 1 )
						OnClickNextPage(null);
				///上下页时
				}else if(m_eRefreshPageMails_Style == E_REFRESHPAGEMAILS_STYLE.E_REFRESHPAGEMAILS_NEXTPAGE ||
					m_eRefreshPageMails_Style == E_REFRESHPAGEMAILS_STYLE.E_REFRESHPAGEMAILS_PREPAGE )
				{
					RecoverDefaultMailList();
				}
				if( getMailResponse.total != 0 )
					/// 更新当前页邮件列表.
                	UpdateCurrMailListInfo(getMailResponse.mails);
				else
				{
					mMailCapacityLabel.text = string.Format("{0}/{1}",mTotalMailNum,CONST_MAILBOX_CAPACITY);
					mPageNumLabel.text = string.Format("{0}/{1}",0,TotalPage);		
				}
            }
        });
    }
	
	/// <summary>
	/// 刷新邮件列表时，先恢复邮件列表为默认状态，在赋值.
	/// </summary>
	private void RecoverDefaultMailList()
	{
		foreach(GameObject go in mAllMailItemsArr)
		{
			go.SetActive(false);
		}
	}
	
    /// <summary>
    /// 更新当前页邮件列表.
    /// </summary>
    /// <param name='mail'>
    /// Mail.
    /// </param>
    private void UpdateCurrMailListInfo(Mail[] currentPageMails)
    {	
		///显示所有邮件		
		for(int m=0;m<currentPageMails.Length;m++){
			///一封邮件对象
			GameObject gridItemObj = mAllMailItemsArr[m];
			if( null!= gridItemObj)
			{
				///对象可见
				gridItemObj.SetActive(true);
				
				Mail mailItem = currentPageMails[m];
				///有无附件需要显示
				gridItemObj.transform.Find("AttachIcon").gameObject.SetActive(mailItem.attach);
				///邮件是否未读
				gridItemObj.transform.Find("ReadIcon").gameObject.SetActive(mailItem.flag == 0);
			    gridItemObj.transform.Find("Labels/UserNameLabel").GetComponent<UILabel>().text = mailItem.sender;
				gridItemObj.transform.Find("Labels/ThemeLabel").GetComponent<UILabel>().text = mailItem.title;
				
				DateTime dt = Global.JSLongToDataTime(mailItem.time);
				gridItemObj.transform.Find("Labels/TimeLabel").GetComponent<UILabel>().text = string.Format("{0:g}",dt);

			}
		}			
		GameObject tempGO;
	///一页20封邮件显示未满，剩下的GridItem不显示
		if(currentPageMails.Length<CONST_MAILS_PERPAGE)
		{
			for(int j= currentPageMails.Length;j<CONST_MAILS_PERPAGE;j++){
				tempGO = mAllMailItemsArr[j];
				if(null!=tempGO)
					tempGO.SetActive(false);
			}
		}			
		///更新邮件数量和页码
		mMailCapacityLabel.text = string.Format("{0}/{1}",mTotalMailNum,CONST_MAILBOX_CAPACITY);
		mPageNumLabel.text = string.Format("{0}/{1}",mCurrentPage+1,TotalPage);		
		mReceiveMailGrid.GetComponent<UIGrid>().repositionNow = true;	
    }

    // user click the previous mail page.
    void OnClickPrePage(GameObject go)
    {
        if (mCurrentPage > 0)
        {
			///刷新状态
			m_eRefreshPageMails_Style = E_REFRESHPAGEMAILS_STYLE.E_REFRESHPAGEMAILS_PREPAGE;
			///切换页数
            mCurrentPage--;
            RefreshCurrentPageMails();
        }
    }

    // user click the next mail page.
    void OnClickNextPage(GameObject go)
    {
        if (mCurrentPage < TotalPage - 1)
        {
			///刷新状态
			m_eRefreshPageMails_Style = E_REFRESHPAGEMAILS_STYLE.E_REFRESHPAGEMAILS_NEXTPAGE;
            ///切换页数
            mCurrentPage++;
            RefreshCurrentPageMails();
        }
    }
	
	/// <summary>
	/// 点击单个邮件阅读，显示正文内容.
	/// </summary>
	/// <param name='go'>
	/// 邮件MailItem对象.
	/// </param>
    private void OnReadMail(GameObject go)
    {
		///邮件在本页中的index
		int mailIndex = GetMailIndexInPage(go);		
        //该邮件在收件箱中的ID
        int mailId = mCurrentPage * CONST_MAILS_PERPAGE + mailIndex;
		mIntMailIndex = mailId;
        Request(new ReadMailCmd(mailId), delegate(string err, Response response)
        {
            if (!string.IsNullOrEmpty(err))
            {
                // promote the error message.
                Debug.LogError("Fail to ReadMailRequest,mailId : " + mailId );
                return;
            }

            Mail mail = (response != null) ? response.Parse<Mail>() : null;
            ///显示邮件内容到面板上
			if (mail != null)
				ShowMailContent(mail);
        });
    }

    // user click the fetch mail attachment button.
    void OnClickFetchAttachment(GameObject go)
    {
        //该邮件在收件箱中的ID
		//展开收到的附件
		//Debug.Log(BackPack.PackageFullCheck());
		if(BackPack.PackageFullCheck())
		{
			MessageBoxWnd.Instance.Show("PackIsFull",
					MessageBoxWnd.StyleExt.Alpha);	
			return;
		}
        int mailId = mIntMailIndex;
        Request(new FetchAttachCmd(mailId), delegate(string err, Response response)
        {
            if (!string.IsNullOrEmpty(err))
            {
                MessageBoxWnd.Instance.Show(err,MessageBoxWnd.StyleExt.Alpha);
                return;
			}			
			MessageBoxWnd.Instance.Show("接收附近成功!",MessageBoxWnd.StyleExt.Alpha);		
			SoundManager.Instance.PlaySound("UI_Prop_Selected");
			mAttachItemRoot.SetActive(false);
			mReceiveButton.SetActive(false);
            UpdatePakageData();
            UpdatePlayerAttrib();			
        });
    }
	
	void UpdatePakageData()
	{
		Request(new GetBackPackCmd(), delegate(string err, Response response)
        {
            if (!string.IsNullOrEmpty(err))
            {
				//MessageBoxWnd.Instance.Show(LanguagesManager.Instance.GetItem((int)LanguageID.).Text);
                Debug.LogError("Request pakage Info Error!!" + err);
                return;
            }
			
            Package data = (response != null) ? response.Parse<Package>() : null;
			// update the package
            if (data != null)
				PlayerDataManager.Instance.OnBackPack(data, false);
        });
	}
	
	void UpdatePlayerAttrib()
	{
		MainScript.Instance.Request(new GetUserAttribCmd(), delegate(string err, Response response)
        {
			//Global.ShowLoadingEnd();
            if (!string.IsNullOrEmpty(err))
            {
				MessageBoxWnd.Instance.Show( err ,MessageBoxWnd.StyleExt.Alpha);
                return;
            }

            // update the user attribute.
            MainAttrib attrib = (response != null) ? response.Parse<MainAttrib>() : null;
            if (attrib != null)
				PlayerDataManager.Instance.OnMainAttrib(attrib);
        });
	}
    // user click the delete mail.
    void OnClickDeleteMail(GameObject go)
    {
		///刷新状态
		m_eRefreshPageMails_Style = E_REFRESHPAGEMAILS_STYLE.E_REFRESHPAGEMAILS_DELETEMAIL;
		
		///获得单个邮件在一页中的index
		int mailIndex = GetMailIndexInPage(go);
		mDeleteMailId = mCurrentPage * CONST_MAILS_PERPAGE + mailIndex;
		
		MessageBoxWnd.Instance.Show("确认删除该邮件?",MessageBoxWnd.Style.OK_CANCLE); 
		MessageBoxWnd.Instance.OnClickedOk = OnClickDeleteMailSure;
    }

	void OnClickDeleteMailSure(GameObject go)
	{
		///删除请求
        Request(new DeleteMailCmd(mDeleteMailId), delegate(string err, Response response)
        {
            if (!string.IsNullOrEmpty(err))
            {
                Debug.LogError("Fail to DeleteMailRequest,mDeleteMailId : " + mDeleteMailId );
                return;
            }			
			///刷新邮件
        	RefreshCurrentPageMails();
        });
	}	
	/// <summary>
	/// 发送邮件
	/// </summary>
	/// <param name='go'>
	/// 发送按钮对象.
	/// </param>
    void OnClickSendMail(GameObject go)
    {
        string to = mInputSendTo.text;
        string title = mInputSendTitle.text;
        string content = mInputSendContent.text;
		
		/*
		if(!ForbiddenWordManager.Instance.IsValid(to)||
		   !ForbiddenWordManager.Instance.IsValid(title)||
		   !ForbiddenWordManager.Instance.IsValid(content))
		{
			MessageBoxWnd.Instance.Show("您输入的内容包含禁言,请尝试重新输入!",
				MessageBoxWnd.StyleExt.Alpha);
			return;
		}	*/	
        Request(new SendMailCmd(to, title, content), delegate(string err, Response response)
        {
            if (!string.IsNullOrEmpty(err))
            {
                // promote the error message.
                Debug.LogError("Fail to SendMailRequest,Mail To : " + to + " Title: "+title+" Content: "+content );
				///提示发送失败
				MessageBoxWnd.Instance.Show(CONST_SENDMAIL_FAIL,MessageBoxWnd.StyleExt.Alpha);
                return;
            }			
			///提示发送成功
			MessageBoxWnd.Instance.Show(CONST_SENDMAIL_SUCCESS,MessageBoxWnd.StyleExt.Alpha);
			///邮件内容置空,保留收件人名字
			//mInputSendTo.text ="";
			mInputSendTitle.text = "";
			mInputSendContent.text = "";

        });	
    }	
	/// <summary>
	/// 阅读单个邮件，进入邮件正文面板.
	/// </summary>
	/// <param name='go'>
	/// 邮件对象.
	/// </param>
	private void OnClickOpenSingleMail(GameObject go)
	{				
		UpdateViewPanel(false,true,false);
		///阅读邮件
		OnReadMail(go);
	}
	
	void UpdateViewPanel(bool isViewReceive,bool isViewRead,bool isViewSend)
	{
		mReceiveMailPanel.SetActive(isViewReceive);
		mReadMailPanel.SetActive(isViewRead);
		mSendMailPanel.SetActive(isViewSend);
	}
	/// <summary>
	/// 阅读邮件，显示邮件内容
	/// </summary>
	/// <param name='go'>
	/// 邮件Mail对象.
	/// </param>
	private void ShowMailContent(Mail mail)
	{
		if(mReadMailPanel == null)
			return;
		
		if(Control("UserName",mReadMailPanel) == null)
			return;
		///邮件发送者
		Control("UserName",mReadMailPanel).GetComponent<UILabel>().text = mail.sender;
		///邮件标题
		Control("TitleLabel",mReadMailPanel).GetComponent<UILabel>().text = mail.title;
		///邮件时间
		DateTime dt = Global.JSLongToDataTime(mail.time);
		Control("TimeLabel",mReadMailPanel).GetComponent<UILabel>().text = string.Format("{0:g}",dt);
		///邮件正文
		Control("MailMainText",mReadMailPanel).GetComponent<UILabel>().text = mail.content;
		///显示附件
		if( mail.item >0 || mail.gem >0 || mail.gold > 0 )
		{
			///可见
			mReplyButton.SetActive(false);
			mAttachItemRoot.SetActive(true);
			mReceiveButton.SetActive(true);
			
			GameObject attachItem = mAttachItemRoot.transform.Find("Item").gameObject;
			GameObject attachGold = mAttachItemRoot.transform.Find("Gold").gameObject;
			GameObject attachGem = mAttachItemRoot.transform.Find("Gem").gameObject;
		
			if(mail.item > 0)
			{
				attachItem.SetActive(true);
				ItemBase itemBase = ItemBaseManager.Instance.GetItem(mail.item);
				Control("GoodsIcon",attachItem).GetComponent<UISprite>().spriteName = itemBase.Icon;
				UpdateItemQualityFrameIcon(Control("QualityIcon",attachItem),itemBase);		    
			}
			else 
				attachItem.SetActive(false);
			if(mail.gem > 0)
			{
				attachGem.SetActive(true);
				attachGem.GetComponentInChildren<UILabel>().text = mail.gem.ToString();
			}
			else
				attachGem.SetActive(false);
			if(mail.gold > 0)
			{
				attachGold.SetActive(true);
				attachGold.GetComponentInChildren<UILabel>().text = mail.gold.ToString();
			}
			else
				attachGold.SetActive(false);
			mAttachItemRoot.GetComponent<UIGrid>().repositionNow = true;
	        
		}else
		{
			if(mail.sender != "system")
				mReplyButton.SetActive(true);
			mAttachItemRoot.SetActive(false);
			mReceiveButton.SetActive(false);
		}		
	}

	private void OnClickReplyMail(GameObject go)
	{
		string name = Control("UserName",mReadMailPanel).GetComponent<UILabel>().text;
		OnClickSendMailBox(Control("SendMail",mFixMailPanel));
		if(mSendMailPanel.activeSelf)
		{
			Control("NameInput",mSendMailPanel).GetComponent<UIInput>().text = name;
		}
	}
	/// <summary>
	/// 进入收件箱.
	/// </summary>
	private void OnClickMyMailBox(GameObject go)
	{
		if(mReceiveMailPanel.activeSelf)
			return;
		UpdateViewPanel(true,false,false);
		UpdateBtnScale(go);
		///正文面板对象恢复
		mAttachItemRoot.SetActive(false);
		mReceiveButton.SetActive(false);
		
		///刷新当前页邮件	
        RefreshCurrentPageMails();
	}
	
	/// <summary>
	/// 进入发件箱.
	/// </summary>
	private void OnClickSendMailBox(GameObject go)
	{
		if(mSendMailPanel.activeSelf)
			return;
		UpdateViewPanel(false,false,true);
		//重置发送内容
		mInputSendTo.text ="";
		mInputSendTitle.text = "";
		mInputSendContent.text = "";
		UpdateBtnScale(go);
		Control("TimeLabel",mSendMailPanel).GetComponent<UILabel>().text = string.Format("{0:g}",DateTime.Now);
	}
	
	void UpdateBtnScale(GameObject go)
	{
		if(go == mChooseBtn)
			return;
		Vector3 pos;
		Vector3 scale;
		if(mChooseBtn != null){
			mChooseBtn.transform.localScale = Vector3.one;
			scale = mChooseBtn.GetComponentInChildren<UILabel>().transform.localScale;
			scale.x /= MagnifyTimes;
			mChooseBtn.GetComponentInChildren<UILabel>().transform.localScale = scale;
			pos = mChooseBtn.transform.localPosition;
			pos.y -= mChooseBtn.GetComponentInChildren<UISprite>().transform.localScale.y * (MagnifyTimes-1)/2;
			mChooseBtn.transform.localPosition = pos;
		}
		go.transform.transform.localScale = new Vector3(1,MagnifyTimes,1);
		scale = go.GetComponentInChildren<UILabel>().transform.localScale;
		scale.x *= MagnifyTimes;
		go.GetComponentInChildren<UILabel>().transform.localScale = scale;
	    pos = go.transform.localPosition;
		pos.y += go.GetComponentInChildren<UISprite>().transform.localScale.y * (MagnifyTimes-1)/2;
		go.transform.localPosition = pos;
		mChooseBtn = go;
	}
	
	/// <summary>
	/// 退出邮箱，返回InGame界面.
	/// </summary>
	private void OnClickCloseMailBox(GameObject go)
	{
		///窗体切换
		if(mSendMailPanel.activeSelf || mReadMailPanel.activeSelf)
		{
			OnClickMyMailBox(mMailBox);
			return;
		}
		
		Close();
		if( !InGameMainWnd.Exist )
			InGameMainWnd.Instance.Open();
		else 
			InGameMainWnd.Instance.Show();
		///暂停恢复
		UnitManager.Instance.LocalPlayer.UGameObject.SendMessage("LockInput","false");
	}	
	/// <summary>
	/// 获得单个邮件在一页中的index
	/// </summary>
	/// <returns>
	/// 邮件Index.
	/// </returns>
	/// <param name='go'>
	/// 邮件上的点击对象.
	/// </param>
	private int GetMailIndexInPage(GameObject clickGO)
	{
		string strName = clickGO.transform.name;
		string strMailItemName = "";
		///点击删除按钮，获得该邮件的index
		if(strName.Contains("Delete"))
		{
			strMailItemName = clickGO.transform.parent.transform.name;
		///点击邮件本身，获得该邮件的index
		}else if(strName.Contains("Mail"))
		{
			strMailItemName = clickGO.transform.name;
		}
		///邮件indx
		int index = Convert.ToInt32(strMailItemName.Substring("Mail".Length));
		return index;
	}
	
}
