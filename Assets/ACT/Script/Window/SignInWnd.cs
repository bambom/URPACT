using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class SignInWnd : Window<SignInWnd>
{
    int mSignInCount = 0;
    int mAlreadySignin = 0;
    List<GameObject> mSignInButton = new List<GameObject>();
    GameObject mLastSelectButton;
    GameObject mSelectButton;
    GameObject mRewardData;
    GameObject mRewardItem;
    UIGrid mRewardGrid;
    UIGrid mItemGrid;
    UIImageButton mBack;
    UIImageButton mClaim;
    FindItem mFindItem;
	SignInBase[] mSbArray;
	
    public override string PrefabName { get { return "SignInWnd"; } }
    public const int SignInNum = 15;

    protected override bool OnOpen()
    {
        for (int i = 1; i <= SignInNum; i++)
        {
            mSignInButton.Add(Control(i.ToString()));
            mSignInButton[i - 1].transform.Find("Check").gameObject.SetActive(false);
            mSignInButton[i - 1].transform.Find("Select").gameObject.SetActive(false);
        }
		mSbArray = SignInBaseManager.Instance.GetAllItem();
        mRewardData = Resources.Load("Reward") as GameObject;
        mRewardItem = Resources.Load("BaseItem") as GameObject;
        mRewardGrid = Control("RewardGrid").GetComponent<UIGrid>();
        mItemGrid = Control("ItemGrid").GetComponent<UIGrid>();

        mClaim = Control("ClaimButton").GetComponent<UIImageButton>();
        UIEventListener.Get(mClaim.gameObject).onClick = OnClickClaim;
        mBack = Control("BackButton").GetComponent<UIImageButton>();
        UIEventListener.Get(mBack.gameObject).onClick = OnClickBack;
        mBack.gameObject.SetActive(false);

        Hide();
        GetSignInNumber();
        //Init();
        return base.OnOpen();
    }

    protected override bool OnClose()
    {
        return base.OnClose();
    }

    void GetSignInNumber()
    {
        //Global.ShowLoadingStart();
        Request(new GetLoginInfoCmd(), delegate(string err, Response response)
        {
            //Global.ShowLoadingEnd();			
            if (!string.IsNullOrEmpty(err))
            {
                Debug.LogError("Get SignIn Info error!!!" + err);
                return;
            }

            GetLoginInfoCmd.LoginInfo info = response != null ? response.Parse<GetLoginInfoCmd.LoginInfo>() : null;
            if (info != null)
            {
                Debug.Log("IP: " + info.LastLoginIp);
                Debug.Log("Count: " + info.SignInCount);

                mSignInCount = info.SignInCount;
                if (info.CanSignIn && mSignInCount < mSbArray.Length)
                {
                    if (InGameMainWnd.Exist)
                        InGameMainWnd.Instance.Hide();

                    Show();
                    Init();
                }
                else
                    Close();
            }
        });
    }

    void Init()
    {
        Control("Number").GetComponent<UILabel>().text = (mSignInCount + 1).ToString();
        InitSignInButton();
    }

    void InitSignInButton()
    {
        mAlreadySignin = mSignInCount % SignInNum;
        int baseSignNum = mSignInCount - mAlreadySignin;
        for (int i = 0; i < mSignInButton.Count; i++)
        {
            int signDay = baseSignNum + i + 1;
            SignInBase signInBase = SignInBaseManager.Instance.GetItem(signDay);
            if (signInBase == null) continue;

            mSignInButton[i].transform.Find("Icon").GetComponent<UISprite>().spriteName = signInBase.IconLabel;
            mSignInButton[i].transform.Find("Label").GetComponent<UILabel>().text = signDay.ToString();

            UIEventListener.Get(mSignInButton[i]).onClick = OnClickSignInButton;

            if (i < mAlreadySignin)
                mSignInButton[i].transform.Find("Check").gameObject.SetActive(true);

            if (i == mAlreadySignin)
                OnClickSignInButton(mSignInButton[i]);
        }
    }

    void OnClickSignInButton(GameObject go)
    {
        if (go.Equals(mSelectButton))
            return;

        mClaim.isEnabled = false;
		GameObject mendSprite = mClaim.gameObject.transform.Find("Mend").gameObject;
		mendSprite.SetActive(false);
        mLastSelectButton = mSelectButton != null ? mSelectButton : null;
        if (mLastSelectButton != null)
            mLastSelectButton.transform.Find("Select").gameObject.SetActive(false);

        go.transform.Find("Select").gameObject.SetActive(true);
        mSelectButton = go;
        int index = int.Parse(go.name);
        if (index == mAlreadySignin + 1)
		{
            mClaim.isEnabled = true;
			mendSprite.SetActive(true);
		}

        int baseSignNum = mSignInCount - mAlreadySignin;
        ShowReward(baseSignNum + index);
    }

    void ShowReward(int id)
    {
        ClearReward();

        SignInBase signInBase = SignInBaseManager.Instance.GetItem(id);
        if (signInBase == null)
            return;

        if (signInBase.Gold > 0)
            CreateRewardData("Icon01_System_Gold_01", signInBase.Gold);
        if (signInBase.Exp > 0)
            CreateRewardData("Icon01_System_Xp_01", signInBase.Exp);
        if (signInBase.SP > 0)
            CreateRewardData("Icon01_System_Sp_01", signInBase.SP);
        if (signInBase.Strength > 0)
            CreateRewardData("Button16_Strength_Normal_01", signInBase.Strength);
		if (signInBase.Gem > 0)
			CreateRewardData("Icon01_System_Gem_01", signInBase.Gem);
			
        string[] args = signInBase.Items.Split('|');
        for (int i = 0; i < args.Length - 1; i += 2)
        {
            int itemId = int.Parse(args[i]);
            int itemNum = int.Parse(args[i + 1]);
            ItemBase itemBase = ItemBaseManager.Instance.GetItem(itemId);
            if (itemBase == null)
                continue;

            if (itemBase.Role > 0 && itemBase.Role != PlayerDataManager.Instance.Attrib.Role)
                continue;

            CreateRewardItem(itemBase, itemNum);
            mFindItem = new FindItem(itemBase, itemNum);
            break;
        }

        mRewardGrid.Reposition();
        mItemGrid.Reposition();
    }

    void ClearReward()
    {
        for (int i = 0; i < mRewardGrid.transform.childCount; i++)
            GameObject.Destroy(mRewardGrid.transform.GetChild(i).gameObject);
        for (int i = 0; i < mItemGrid.transform.childCount; i++)
            GameObject.Destroy(mItemGrid.transform.GetChild(i).gameObject);
        mRewardGrid.transform.DetachChildren();
        mItemGrid.transform.DetachChildren();
    }

    void CreateRewardData(string sprite, int Count)
    {
        GameObject rewarData = GameObject.Instantiate(mRewardData) as GameObject;
        rewarData.transform.Find("Sprite").GetComponent<UISprite>().spriteName = sprite;
		UILabel label = rewarData.transform.Find("Label").GetComponent<UILabel>(); 
        label.text = Count.ToString();
		label.color = Color.white;
        rewarData.transform.parent = mRewardGrid.transform;
		rewarData.transform.localScale = Vector3.one;
    }

    void CreateRewardItem(ItemBase item, int Count)
    {
        GameObject rewardItem = GameObject.Instantiate(mRewardItem) as GameObject;
        rewardItem.transform.Find("Icon/Background").GetComponent<UISprite>().spriteName = item.Icon;
		UILabel itemCountLabel = rewardItem.transform.Find("CountNo/LabelCount").GetComponent<UILabel>();
		if(Count > 1)
		{
			itemCountLabel.gameObject.SetActive(true);
        	itemCountLabel.text = Count.ToString();
		}
		else
			itemCountLabel.gameObject.SetActive(false);
		UISprite background = rewardItem.transform.Find("Quality").GetComponent<UISprite>();
		SetQualityBackground(background, item.Quality);
        rewardItem.transform.parent = mItemGrid.transform;
        rewardItem.transform.localPosition = Vector3.zero;
        rewardItem.transform.localScale = new Vector3(55, 55, 1);
    }
	
	void SetQualityBackground(UISprite background, int quality)
	{
		background.enabled = true;
		switch(quality)
		{
			case (int)ItemQuality.IQ_White:
				background.enabled = false;
				break;
			case (int)ItemQuality.IQ_Blue:
				background.spriteName = "Button10_BaseItem_Quality_02";
				break;
			case (int)ItemQuality.IQ_Green:
				background.spriteName = "Button10_BaseItem_Quality_01";
				break;
			case (int)ItemQuality.IQ_Purple:
				background.spriteName = "Button10_BaseItem_Quality_03";
				break;
		}
	}

    void OnClickClaim(GameObject go)
    {
        mSignInButton[mAlreadySignin].transform.Find("Check").gameObject.SetActive(true);
        go.SetActive(false);
		Request(new SignInCmd(), delegate(string err, Response response)
        {
            if (!string.IsNullOrEmpty(err))
            {
                Debug.LogError("SignIn failed! " + err);
                return;
            }
            SoundManager.Instance.PlaySound("UI_Prop_Selected");
            if (mFindItem != null)
            {
                if (!FindItemWnd.Exist)
                    FindItemWnd.Instance.Open();
                FindItemWnd.Instance.AddItem(mFindItem);
            }
            UpdatePakageData();
        });
        mBack.gameObject.SetActive(true);
    }

    void OnClickBack(GameObject go)
    {
		CloseWnd();
    }

    void CloseWnd()
    {
        Close();
        if (InGameMainWnd.Exist)
            InGameMainWnd.Instance.Show();
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

    public void ShowTest(int day)
    {
        mSignInCount = day;
        Open();
    }
}
