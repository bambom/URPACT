using UnityEngine;
using System.Collections;

public class CreateRoleWnd : Window<CreateRoleWnd>
{
    int mRole = 1003;
    UIInput mNameInput;
	UILabel mDescLabel;
	GameObject mDescPanel;	
	GameObject mReturn ;
	GameObject mWarriorObj;
	GameObject mAssassinObj;
	GameObject mMasterObj;
	GameObject mChooseObj;
	GameObject mGoCamera;
    public override string PrefabName { get { return "CreateRoleWnd"; } }

    protected override bool OnOpen()
    {
		//ResetParent For reason It is A 3D camera
		mGoCamera = GameObject.Instantiate(Resources.Load("CreateRoleCamera")) as GameObject;
		GameObject go = GameObject.Find("WindowsRoot");
		mGoCamera.transform.parent = go.transform;	
		mGoCamera.transform.localPosition = new Vector3(0.0f,0.0f,-11.0f);
		Init();
        return base.OnOpen();
    }

    protected override bool OnClose()
    {
		GameObject.Destroy(mGoCamera);
        return base.OnClose();
    }
	

	
	void Init()
	{
		mReturn = Control ("Return");
		mNameInput = Control("InputName").GetComponent<UIInput>();
		mDescLabel = Control("DescWnd").GetComponentInChildren<UILabel>();
		mWarriorObj = Control("ShowOption").transform.Find("WarriorRoot/Warrior").gameObject;
		mAssassinObj= Control("ShowOption").transform.Find("AssassinRoot/Assassin").gameObject;
		mMasterObj = Control("ShowOption").transform.Find("MageRoot/Mage").gameObject;
	
        UIEventListener.Get(Control("EnterButton")).onClick = OnClickCreateAccount;
		//UIEventListener.Get(Control("RandomButton")).onClick = OnClickRandomAccount;
        UIEventListener.Get(Control("1003")).onClick = OnClickRoleChoic;
        UIEventListener.Get(Control("1004")).onClick = OnClickRoleChoic;
        UIEventListener.Get(Control("1005")).onClick = OnClickRoleChoic;
		UIEventListener.Get(mReturn).onClick = OnClickReturnToRoleList;
		
		
		//ProduceAccount();
		if(Global.GuideRole == 0)
			OnClickRoleChoic(Control("1003"));
		else
		{
			Control("1003").GetComponent<TweenScale>().enabled = false;
			Control("1003").transform.localScale = Vector3.one;
			OnClickRoleChoic(Control(Global.GuideRole.ToString()));
			Global.GuideRole = 0;
		}
		//Control("")
		//mDescTips.GetComponent<UILabel>().text = UnitBaseManager.Instance.GetItem(mRole).Show;
        //mInputLabel = Control("LabelInput").GetComponent<UILabel>();
		
		if (LoginHelper.roleList == null 
			|| LoginHelper.roleList.users == null 
			|| LoginHelper.roleList.users.Length == 0)
		mReturn.SetActive(false);
	}
	
	void CharactorModelInit(GameObject go)
	{
		PressDrag PDScript = Control ("CharactorCollider").GetComponent<PressDrag>();
		if(null != PDScript){
			PDScript.target = go.transform;
		}else{
			PDScript = Control ("CharactorCollider").AddComponent<PressDrag>();
			PDScript.target = go.transform;
		}
	}	
	
	void OnClickReturnToRoleList( GameObject go )
	{
		Close ();
		RoleChooseWnd.Instance.Open();
		RoleChooseWnd.Instance.OnRoleList(LoginHelper.roleList);
	}
	
    void OnClickRoleChoic(GameObject go)
    {
		if(go == mChooseObj)
			return;
		if(mChooseObj != null){
			mChooseObj.GetComponent<TweenScale>().enabled = false;
			mChooseObj.transform.localScale = Vector3.one;
		}
		go.GetComponent<TweenScale>().enabled = true;
		mChooseObj = go;
	    switch(go.name)
		{
			case "1003":
				ShowRoleObj(true,false,false);
				break;
			case "1004":
				ShowRoleObj(false,true,false);
				break;
			case "1005":
				ShowRoleObj(false,false,true);
				break;
			default:
				break;
		}			
        mRole = int.Parse(go.name);
		mDescLabel.GetComponent<UILabel>().text = UnitBaseManager.Instance.GetItem(mRole).Show;
    }
	
    void ShowRoleObj(bool isShowWarrior,bool isShowAssassin, bool isShowMaster)
	{
        mWarriorObj.SetActive(isShowWarrior);
		mAssassinObj.SetActive(isShowAssassin);
		mMasterObj.SetActive(isShowMaster);
		
		mWarriorObj.transform.parent.localEulerAngles = Vector3.zero;
		mAssassinObj.transform.parent.localEulerAngles = Vector3.zero;
		mMasterObj.transform.parent.localEulerAngles = Vector3.zero;
		
		if(isShowWarrior){
			CharactorModelInit(mWarriorObj.transform.parent.gameObject);
		}else if(isShowAssassin){
			CharactorModelInit(mAssassinObj.transform.parent.gameObject);
		}else if(isShowMaster){
			CharactorModelInit(mMasterObj.transform.parent.gameObject);
		}
		
		
	}
	
	void OnClickRandomAccount(GameObject go)
	{
		ProduceAccount();
	}
	
    void OnClickCreateAccount(GameObject go)
    {
        string user = mNameInput.text; // get from the editbox UI.
        if ( !IsName(user) || /*!ForbiddenWordManager.Instance.IsValid(user) ||*/ !IsRightLength(user))
		{
			//Debug.Log("你输入的内容含敏感词或长度不符，请重新输入");
			
			MessageBoxWnd.Instance.Show(("RoleNameForbid"),
				MessageBoxWnd.StyleExt.Alpha);
            return;
		}
		
		if(user == "system")
		{
			MessageBoxWnd.Instance.Show("System has used this name,please try others.",MessageBoxWnd.StyleExt.Alpha);
			return;
		}
		
		MainScript.Instance.MyDebugLog("OnClickCreateAccount" + System.DateTime.Now);
		Global.ShowLoadingStart();
        Request(new CreateUserCmd(user, mRole), delegate(string err, Response response)
        {
            if (!string.IsNullOrEmpty(err))
            {
				Global.ShowLoadingEnd();
				MessageBoxWnd.Instance.Show(err,MessageBoxWnd.StyleExt.Alpha);
                return;
            }
            // 获取玩家角色数据
			SoundManager.Instance.PlaySound("UI_Enter_01");
			PlayerPrefs.SetString(RoleChooseWnd.SaveUserName,user);
           	RoleList roleList = response != null ? response.Parse<RoleList>() : null;
			LoginHelper.roleList = roleList;
			MainScript.Instance.MyDebugLog("OnClickCreateAccount: CreateUserCmd" + System.DateTime.Now);
            if (roleList != null && roleList.users != null && roleList.users.Length > 0)
            {
                // 角色选择界面。
                RoleChooseWnd.Instance.Open();
                RoleChooseWnd.Instance.OnRoleList(roleList);
                Close();
            }
			Global.ShowLoadingEnd();
			
        });		
    }
	
	bool IsName(string text) // 
    {
        bool isForbidden = false;
        int stringLength = text.Length;
        for (int i = 0; i < stringLength; i++)
        {             
			char c = text[i];
            if (!char.IsControl(c) // 控制字符
                && char.IsLetterOrDigit(c)// 是字母或者数字
                && !char.IsPunctuation(c)// 不是标点符号
                && !char.IsSymbol(c)//非符号类 上下标 箭头 货币等
                && !char.IsWhiteSpace(c)//非空白类 空格等
                ) isForbidden = true;
            else return false; //如果检测到一个字符不合格就直接跳出
     	}
        return isForbidden;
    }
	bool IsRightLength(string text)
	{
		int charNumber = 0;
		foreach(char c in text)//get the string lenght
		{
			if(IsChinese(c))
				charNumber += 2;
			else
				charNumber++;			
		}
		
		Debug.Log(charNumber);
		if(charNumber >=1 && charNumber <14)
			return true;
		else
			return false;
	}
	bool IsChinese(char c) //char is Chinese 
	{
		return (int)c >= 0x4e00&&(int)c<=0x9fa5;
	}
	
	void ProduceAccount()
	{
	    RoleCreateName[] roleCreateNames = RoleCreateNameManager.Instance.GetAllItem();		
		int index = Random.Range(0,roleCreateNames.Length);
		Debug.Log(index);
	    string resultStr = roleCreateNames[index].Surname;
		//Debug.Log(resultStr);
		//index = Random.Range(0,roleCreateNames.Length);
		//resultStr += roleCreateNames[index].Forename;
		//Debug.Log(index);
		Debug.Log(resultStr);
		
		if(!string.IsNullOrEmpty(resultStr))
		{	
			mNameInput.text = resultStr;
		}
	}
}
