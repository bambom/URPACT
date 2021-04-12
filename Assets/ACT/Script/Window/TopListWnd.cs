using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TopListWnd : Window<TopListWnd>
{
    ETopListType mCurrentType = ETopListType.battle;
    int mCurrentPage = 0;
    int mTotalNum = 0;

    int TotalPage { get { return (mTotalNum + ItemsPerPage - 1) / ItemsPerPage; } }

    const int ItemsPerPage = 10;
	const int MaxPageCount = 5;
	const float MagnifyTimes = 1.2f;
    public override string PrefabName { get { return "TopListWnd"; } }
    Transform mToplistGrid;
	Transform mCharactorRotate;
    GameObject mSelfRankObj;
    GameObject mCurrentChoose;
	GameObject mChooseBtn;
    UILabel mSelfRankLabel;
    UILabel mSelfNameLabel;
    UILabel mSelfInfLabel;
    UILabel mChoosePlayerName;
    UILabel mChoosePlayerPvp;
	UILabel mPageInfoLabel;
	UISprite mSelfRoleSprite;

    bool mIsInpage = true;
	//LevelSlider []mSlider = LevelSliderManager.Instance.GetAllItem();

    protected override bool OnOpen()
    {
        InitObject();
        UIEventListener.Get(Control("BackButton")).onClick = OnClickBack;
        UIEventListener.Get(Control("LevelButton")).onClick = OnClickOptRank;
        UIEventListener.Get(Control("BattleButton")).onClick = OnClickOptRank;
        UIEventListener.Get(Control("MoneyButton")).onClick = OnClickOptRank;
        UIEventListener.Get(Control("ArenaButton")).onClick = OnClickOptRank;
		UIEventListener.Get(Control("NextPageBtn")).onClick = OnClickNextPage;
		UIEventListener.Get(Control("PrePageBtn")).onClick = OnClickPrePage;
        UIEventListener.Get(Control("SelfRank")).onClick = OnClickSelfRank;
		
		CharactorModelInit();
        OnClickOptRank(Control("BattleButton"));
		WinStyle = WindowStyle.WS_Ext;
        return base.OnOpen();
    }

    protected override bool OnClose()
    {
		if(EquipChangeWnd.Exist)
			EquipChangeWnd.Instance.Close();
        return base.OnClose();
    }
	
	void CharactorModelInit()
	{
		if( !EquipChangeWnd.Exist )
			EquipChangeWnd.Instance.Open();
//		else
//			EquipChangeWnd.Instance.Show();

		PressDrag PDScript = Control ("CharactorCollider").GetComponent<PressDrag>();
		
		if(null != PDScript){
			PDScript.target = EquipChangeWnd.Instance.CharactorRoot.transform;
		}else{
			PDScript = Control ("CharactorCollider").AddComponent<PressDrag>();
			PDScript.target = EquipChangeWnd.Instance.CharactorRoot.transform;
		}
		mCharactorRotate = Control("CharactorRotate",EquipChangeWnd.Instance.WndObject).transform;
	}
	
	void OnClickOptRank(GameObject go)
	{
		mCurrentPage = 0;
		switch(go.name)
		{
		case "BattleButton":
			mCurrentType = ETopListType.battle;
			break;
		case "LevelButton":
			mCurrentType = ETopListType.level;
			break;
		case "MoneyButton":
			mCurrentType = ETopListType.money;
			break;
		case "ArenaButton":
			mCurrentType = ETopListType.pvp;
			break;
		default:
			break;
		}
		UpdateBtnScale(go);
		RefreshCurrentPage();
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
	
	void OnClickPrePage(GameObject go)
	{
		if(mCurrentPage == 0)
			return;
		mCurrentPage --;
		RefreshCurrentPage();
	}
	
	void OnClickNextPage(GameObject go)
	{
		if(mCurrentPage == TotalPage -1)
			return;
		mCurrentPage ++;
		RefreshCurrentPage();
	}
	
    void RefreshCurrentPage()
    {
        string type = mCurrentType.ToString(); // 
        Request(new GetTopListCmd(type, mCurrentPage, ItemsPerPage), delegate(string err, Response response)
        {
            GetTopListResponse getTopListResponse = (response != null) ? response.Parse<GetTopListResponse>() : null;
            if (getTopListResponse != null)
            {
                DestroyListItem();
                // we get the total mail number.
                mTotalNum = getTopListResponse.total;

                // show my rank	
                ShowSelfRank(getTopListResponse.rank);
				mPageInfoLabel.text = string.Format("{0}/{1}",mCurrentPage+1,TotalPage);
                // show the others.
                GameObject selectItem = null;
                for (int i = 0; i < getTopListResponse.list.Length; i++)
                {
                    GameObject playerItem = GameObject.Instantiate(Resources.Load("PlayerItem")) as GameObject;
                    UIEventListener.Get(playerItem).onClick = OnClickPlayerItem;
                    SetObjectPosition(playerItem, mToplistGrid.gameObject);
                    UpdateItem(playerItem, getTopListResponse.list[i].Attrib, i);

                    if (selectItem == null)
                        selectItem = playerItem;
                    mToplistGrid.GetComponent<UIGrid>().Reposition();
                }
				
                // selected the default.
                if (selectItem)
                    OnClickPlayerItem(selectItem);

                if (!mIsInpage)
                    StressSelf();
            }
        });
        mToplistGrid.GetComponent<UIGrid>().Reposition();
    }

    void OnClickPlayerItem(GameObject go)
    {
        if (mCurrentChoose == null || go != mCurrentChoose.transform.parent.gameObject)
        {
            GameObject selected = go.transform.Find("Selected").gameObject;
            selected.SetActive(true);

            if (mCurrentChoose)
                mCurrentChoose.SetActive(false);
            mCurrentChoose = selected;

            mChoosePlayerName.text = Control("NameLabel", go).GetComponent<UILabel>().text;
            ShowModel(mChoosePlayerName.text);
        }
    }

    void ShowModel(string name)
    {
        Request(new GetUserDataCmd(name, false), delegate(string err, Response response)
        {
            PlayerData playerData = (response != null) ? response.Parse<PlayerData>() : null;
            if (playerData == null)
                return;

            /*GameObject showNode = Control("PlayerModel");
            for (int i = 0; i < showNode.transform.childCount; i++)
                GameObject.Destroy(showNode.transform.GetChild(i).gameObject);
			showNode.transform.DetachChildren();
			
            Player player = Player.CreateShowPlayer(playerData, showNode);
			//add the SpinWithMouse
		    if(showNode.transform.childCount > 0)
				showNode.transform.GetChild(0).gameObject.AddComponent<SpinWithMouse>();
			
            if (player != null)
                player.PlayAction(Data.CommonAction.Show);*/
			if(mCharactorRotate == null){
				Debug.LogError("mCharactorRotate is null");
			    return;
			}
			mCharactorRotate.localEulerAngles = Vector3.zero;
			mCharactorRotate.localScale = Vector3.one * 0.9f;
			EquipChangeWnd.Instance.UpdateCharactor(playerData);
			
			Debug.Log(playerData.Attrib.PvpLevel.ToString());
			GetPvpTile(playerData.Attrib.PvpLevel.ToString());
        });
    }

    void OnClickBack(GameObject go)
    {
        Close();
        if (!InGameMainWnd.Exist)
            InGameMainWnd.Instance.Open();
		else
			InGameMainWnd.Instance.Show();
    }

    void UpdateItem(GameObject go, GetTopListResponse.TopListAttrib attrib, int n)
    {
        //change the color
        int rank = n + 1 + mCurrentPage * ItemsPerPage;
        go.name = rank.ToString();
        Control("NameLabel", go).GetComponent<UILabel>().text = attrib.Name;
        UnitBase unitBase = UnitBaseManager.Instance.GetItem(attrib.Role);
        Control("RoleIcon", go).GetComponent<UISprite>().spriteName = unitBase.Label;
        Control("RankLabel", go).GetComponent<UILabel>().text = rank.ToString();
		go.transform.Find("PvpLevelLabel").GetComponent<UILabel>().text = attrib.PvpLevel.ToString();	
        switch (mCurrentType)
        {
            case ETopListType.level:
                Control("InformationLabel", go).GetComponent<UILabel>().text = attrib.Level.ToString();
                break;
            case ETopListType.money:
			    Debug.Log("TotalGem:"+attrib.TotalGold.ToString());
                Control("InformationLabel", go).GetComponent<UILabel>().text = attrib.TotalGold.ToString();
                break;
            case ETopListType.battle:
                Control("InformationLabel", go).GetComponent<UILabel>().text = ( Mathf.Ceil( attrib.Battle /10.0f) ).ToString();
                break;
            case ETopListType.pvp:
                Control("InformationLabel", go).GetComponent<UILabel>().text = attrib.PvpExp.ToString();//GetTranscriptName(attrib.Progress.ToString(),true);
                break;
            default:
                break;

        }
    }
	
	void GetPvpTile(string pvpLevel)
	{
		PvpBase pvpBase = PvpBaseManager.Instance.GetItem(int.Parse(pvpLevel));
		if(pvpBase == null)
			mChoosePlayerPvp.text = "No Title Of The Arena";
		else
			mChoosePlayerPvp.text = pvpBase.ArenaTitle;
	}
    void SetObjectPosition(GameObject child, GameObject parent)
    {
        child.transform.parent = parent.transform;
        child.transform.localScale = Vector3.one;
        child.transform.localPosition = Vector3.zero;
    }

    void InitObject()
    {
        mToplistGrid = Control("ListGrid").transform;
        mSelfRankObj = Control("SelfRankLabels");
        mSelfRankLabel = Control("RankLabel", mSelfRankObj).GetComponent<UILabel>();
        mSelfNameLabel = Control("NameLabel", mSelfRankObj).GetComponent<UILabel>();
        mSelfRoleSprite = Control("RoleIcon", mSelfRankObj).GetComponent<UISprite>();
        mSelfInfLabel = Control("InformationLabel", mSelfRankObj).GetComponent<UILabel>();
        mChoosePlayerName = Control("PlayerNameLabel").GetComponent<UILabel>();
		mChoosePlayerPvp = Control("PvpTitleLabel").GetComponent<UILabel>();
		mPageInfoLabel = Control("PageInfo").GetComponentInChildren<UILabel>();
    }

    void DestroyListItem()
    {
		for(int i=0 ;i<mToplistGrid.childCount ;i++)
			GameObject.Destroy(mToplistGrid.GetChild(i).gameObject);
        mToplistGrid.DetachChildren();
    }

    void ShowSelfRank(int rank)
    {
        MainAttrib attrib = PlayerDataManager.Instance.Attrib;
        mSelfNameLabel.text = attrib.Name; //attrib.Gold.ToString();
        UnitBase unitBase = UnitBaseManager.Instance.GetItem(int.Parse(attrib.Role.ToString()));
        mSelfRoleSprite.spriteName = unitBase.Label;

        switch (mCurrentType)
        {
            case ETopListType.level:
                mSelfInfLabel.text = attrib.Level.ToString();
                break;
            case ETopListType.money:
                mSelfInfLabel.text = attrib.TotalGold.ToString();
                break;
            case ETopListType.battle:
                mSelfInfLabel.text = ( Mathf.Ceil( attrib.Battle /10.0f) ).ToString();
                break;
            case ETopListType.pvp:
                mSelfInfLabel.text = attrib.PvpExp.ToString();//GetTranscriptName(attrib.Progress.ToString(),false);
                break;
            default:
                break;
        }

        if (rank == 0 || rank == -1)
            mSelfRankLabel.text = "50+";
        else
            mSelfRankLabel.text = rank.ToString();

    }
	
	/*string GetTranscriptName(string str,bool AddColor)
	{
	    string resultStr = "";
		for(int i=0; i<mSlider.Length; i++)
		{
			string[] levelIds = mSlider[i].Level.Split('|');
			foreach(string levelId in levelIds)
			{
				if(levelId == str)
				{
					if(AddColor)
						resultStr = mSlider[i].ClassName + "[FFFF00]S"+ str +"[-]";
					else
						resultStr = mSlider[i].ClassName + "S"+ str;
					return resultStr;
				}
			}	
		}
		if(AddColor)
			resultStr = mSlider[0].ClassName + "[FFFF00]S"+ str +"[-]";
		else
			resultStr = mSlider[0].ClassName + "S"+ str;
		return resultStr;
	}*/

    void OnClickSelfRank(GameObject go)
    {
        if (mSelfRankLabel.text == "50+")
            return;
        int myRank = int.Parse(mSelfRankLabel.text);
        int selfPage = (myRank + ItemsPerPage - 1) / ItemsPerPage - 1;
		
		if(selfPage != mCurrentPage){		
			mCurrentPage = selfPage;
			mIsInpage = false;
			RefreshCurrentPage();		
		}
		else
			StressSelf();		
    }
	
    /*void MovePage(int page)
    {
        SpringPanel sp = mToplistGrid.parent.gameObject.GetComponent<SpringPanel>();
        if (sp == null) sp = mToplistGrid.parent.gameObject.AddComponent<SpringPanel>();

        Vector3 target = Vector3.zero;
        target.y = page * mToplistGrid.GetComponent<UIGrid>().cellHeight;
        sp.target = target;
        sp.strength = 13f;
        sp.enabled = true;
        sp.onFinished = null;
    }*/

    void StressSelf()
    {
        GameObject selfObject = Control(mSelfRankLabel.text, mToplistGrid.gameObject);
        OnClickPlayerItem(selfObject);
        mIsInpage = true;
    }
}
