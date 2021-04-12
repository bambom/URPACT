using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LoginWnd : Window<LoginWnd>
{
    GameObject mGameCenterButton;
    GameObject mGameCenterChecked;
    GameObject mFacebookButton;
    GameObject mFacebookChecked;
    GameObject mLoginButton;
    string mLoginID;
    Dictionary<GameObject, string> mServerItems = new Dictionary<GameObject, string>();

    const int MaxServerNum = 100;
    const string ServerKey = "Server";
    public override string PrefabName { get { return "LoginWnd"; } }
	
    protected override bool OnOpen()
    {
        mGameCenterButton = Control("GameCenter");
        mGameCenterChecked = Control("Checked", mGameCenterButton);

        mFacebookButton = Control("Facebook");
        mFacebookChecked = Control("Checked", mFacebookButton);

        mLoginButton = Control("Login");

        UIEventListener.Get(mGameCenterButton).onClick = OnClickGameCenterLogin;
        UIEventListener.Get(mFacebookButton).onClick = OnClickFacebookLogin;
        UIEventListener.Get(mLoginButton).onClick = OnClickLogin;

        ConfigServerList();

        mLoginID = MainScript.Instance.ClientId;

        RefreshLoginStatus();

        return base.OnOpen();
    }

    void ConfigServerList()
    {
        // create the server items based on the config server list.
        int serverNum = 0;
        GameConfig gameConfig = MainScript.Instance.Config;
        GameObject recommendItem = null;
        if (gameConfig != null && gameConfig.serverlist != null)
        {
            // get the saved server.
            string recommendServer = "";
            if (PlayerPrefs.HasKey(ServerKey))
                recommendServer = PlayerPrefs.GetString(ServerKey);

            foreach (string id in gameConfig.serverlist.Keys)
            {
                serverNum++;

                // find this one to process.
                GameObject serverItem = Control("ServerItem" + serverNum);
                if (serverItem == null)
                    break;

                // register the click event.
                UIEventListener.Get(serverItem).onClick = OnClickServerItem;

                // setup the id.
                mServerItems[serverItem] = id;

                // setup the recommend server.
                GameConfig.ServerInfo serverInfo = gameConfig.serverlist[id];
                if (string.IsNullOrEmpty(recommendServer) || recommendServer == id)
                {
                    recommendItem = serverItem;
                    recommendServer = id;
                    UICheckbox checkbox = serverItem.GetComponent<UICheckbox>();
                    checkbox.isChecked = true;
                }

                // setup the server name.
                UILabel label = serverItem.GetComponentInChildren<UILabel>();
                if (label)
                    label.text = serverInfo.name;
            }
        }

        // disable the other server items.
        while (++serverNum < MaxServerNum)
        {
            GameObject serverItem = Control("ServerItem" + serverNum);
            if (serverItem != null)
                serverItem.SetActive(false);
        }

        // reposition the server list.
        UIDraggablePanel draggablePanel = WndObject.GetComponentInChildren<UIDraggablePanel>();
        if (draggablePanel && recommendItem)
        {
            Vector3 targetPosition = draggablePanel.transform.localPosition;
            targetPosition.x -= recommendItem.transform.localPosition.x;
            SpringPanel.Begin(draggablePanel.gameObject, targetPosition, 8);
        }
    }

    protected override bool OnClose()
    {
        return base.OnClose();
    }

    int mCheckDisabled = Time.frameCount;
	const int CheckDisableFrame = 5;
    void OnClickGameCenterLogin(GameObject go)
    {
        mCheckDisabled = Time.frameCount;
        Social.localUser.Authenticate(delegate(bool success)
        {
            if (!WndObject) 
                return;

            if (!success && (mCheckDisabled + CheckDisableFrame >= Time.frameCount))
                MessageBoxWnd.Instance.Show(("GameCenterError"), MessageBoxWnd.Style.OK);

            RefreshLoginStatus();
        });
    }

    void OnClickFacebookLogin(GameObject go)
    {
        FacebookHelper.Login(delegate(FBResult result)
        {
            RefreshLoginStatus();
        });
    }

    void RefreshLoginStatus()
    {
        if (Social.localUser.authenticated)
        {
            Debug.Log("GameCenterId:" + Social.localUser.id);
            mLoginID = Social.localUser.id;
            mLoginButton.SetActive(true);
        }
        else if (FacebookHelper.IsLoggedIn)
        {
            //Debug.Log("FacebookId:" + FB.UserId);
            //mLoginID = FB.UserId;
            mLoginButton.SetActive(true);
        }
        else
            mLoginButton.SetActive(false);

        mGameCenterChecked.GetComponent<UISprite>().alpha = Social.localUser.authenticated ? 1.0f : 0.0f;
        mFacebookChecked.GetComponent<UISprite>().alpha = FacebookHelper.IsLoggedIn ? 1.0f : 0.0f;
    }

    void OnClickLogin(GameObject go)
    {
        MainScript.Instance.ClientId = mLoginID;
        MainScript.Instance.SendMessage("Login");
        Close();
    }

    void OnClickServerItem(GameObject go)
    {
        if (!mServerItems.ContainsKey(go))
            return;

        string serverId = mServerItems[go];
        if (PlayerPrefs.HasKey(ServerKey) || PlayerPrefs.GetString(ServerKey) != serverId)
        {
            PlayerPrefs.SetString(ServerKey, serverId);
            PlayerPrefs.Save();
        }

        GameConfig.ServerInfo serverInfo = MainScript.Instance.Config.serverlist[serverId];
        MainScript.Instance.ServerIp = serverInfo.server;
        MainScript.Instance.ServerPort = serverInfo.port;
    }
}

