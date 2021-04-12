using UnityEngine;
using System;
using System.Collections;


public class LoginHelper : MonoBehaviour
{
    public static RoleList roleList;
    public static int RoleIndex = 0;
	private bool mReconnecting = false;

    void Start()
    {
        //Login();
    }

    public void Login()
    {
        StartCoroutine("ILogin");
    }


    void OnClickCancleToQuit(GameObject go)
    {
        Application.Quit();
    }

    void OnClickOkToQuit(GameObject go)
    {
        Application.Quit();
    }

    void OnClickOkToRelogin(GameObject go)
    {
        ReLogin();
    }

    void OnClickReloginOk(GameObject go)
    {
        ILogin();
    }

    void OnClickReloginCancle(GameObject go)
    {
        Application.Quit();
    }


    void Update()
    {
        if (!mReconnecting)
        {
            if (!MessageBoxWnd.Exist &&
                MainScript.Instance.UserLogined &&
                MainScript.Instance.Client.state == WebSocket4Net.WebSocketState.Closed)
            {
                MessageBoxWnd.Instance.Show(("NetWorkError"), MessageBoxWnd.Style.OK);
                MessageBoxWnd.Instance.OnClickedOk = OnClickOkToRelogin;
            }
        }
    }


    public void ReLogin()
    {
        // 若不在游戏内，则调用正常的登录。
        if (string.IsNullOrEmpty(MainScript.Instance.ClientUserName))
        {
            StartCoroutine("ILogin");
            return;
        }

        // turn on the reconnecting flag.
        mReconnecting = true;
		
        StartCoroutine("IReLogin");
    }

    IEnumerator ILogin()
    {
        if (MainScript.Instance.Client != null)
            MainScript.Instance.Client.GodClose();

        MainScript.Instance.Client.StartServer(
            MainScript.Instance.ServerIp,
            MainScript.Instance.ServerPort);

        float checkStep = 0.1f;
        float checkLeft = MainScript.Instance.ConnectTimeout;
        while (checkLeft > 0.0f)
        {
            checkLeft -= checkStep;
            if (MainScript.Instance.Client.state != WebSocket4Net.WebSocketState.Connecting)
                break;

            yield return new WaitForSeconds(checkStep);
        }

        MainScript.Instance.UserLogined = false;

        // try to login.
        if (MainScript.Instance.Client.state == WebSocket4Net.WebSocketState.Open)
        {
            Debug.Log("Connect to server success");
            Debug.Log("Try to login with: " + MainScript.Instance.ClientId);
            StartCoroutine(MainScript.Instance.Client.Execute(new LoginCmd(MainScript.Instance.ClientId),
                delegate(string err, Response response)
                {
                    Debug.Log("loin");
                    Global.ShowLoadingEnd();
                    if (!string.IsNullOrEmpty(err))
                    {
                        MessageBoxWnd.Instance.Show(err, MessageBoxWnd.Style.OK);
                        MessageBoxWnd.Instance.OnClickedOk = OnClickReloginOk;
                        return;
                    }
                    MainScript.Instance.UserLogined = true;

                    // 获取玩家数据
                    roleList = response != null ? response.Parse<RoleList>() : null;
                    if (roleList == null || roleList.users == null || roleList.users.Length == 0)
                    {
                        // 角色不存在，需要创建数据。
                        if (!CreateRoleWnd.Exist)
                            CreateRoleWnd.Instance.Open();
                    }
                    else if (!RoleChooseWnd.Exist)
                    {
                        // 创建角色界面掉线 
                        if (CreateRoleWnd.Exist)
                            CreateRoleWnd.Instance.Close();

                        // 角色选择界面。
                        RoleChooseWnd.Instance.Open();
                        RoleChooseWnd.Instance.OnRoleList(roleList);
                    }
                    Debug.Log("Login sucess");
                }));
        }
        yield break;
    }

    IEnumerator IReLogin()
    {
        // showing the loading icon.
        Global.ShowLoadingStart();

        // save the old time scale.
        float timeScale = Time.timeScale;
        Time.timeScale = 1.0f;

        if (MainScript.Instance.Client != null)
            MainScript.Instance.Client.GodClose();

        MainScript.Instance.Client.StartServer(
            MainScript.Instance.ServerIp,
            MainScript.Instance.ServerPort);

        float checkStep = 0.1f;
        float checkLeft = MainScript.Instance.ConnectTimeout;
        while (checkLeft > 0.0f)
        {
            checkLeft -= checkStep;
            if (MainScript.Instance.Client.state != WebSocket4Net.WebSocketState.Connecting)
                break;

            yield return new WaitForSeconds(checkStep);
        }

        // close the loading icon.
        Global.ShowLoadingEnd();

        // step back old time scale.
        Time.timeScale = timeScale;

        // try to login.
        if (MainScript.Instance.Client.state == WebSocket4Net.WebSocketState.Open)
        {
            Debug.Log("Try to relogin with: " + MainScript.Instance.ClientId);
            string id = MainScript.Instance.ClientId;
            string user = MainScript.Instance.ClientUserName;
            StartCoroutine(MainScript.Instance.Client.Execute(new ReLoginCmd(id, user),
                delegate(string err, Response response)
                {
                    if (!string.IsNullOrEmpty(response.error))
                    {
                        // login failed, try to load the create role window.
                        Debug.Log("ReLogin failed: " + err);
                        return;
                    }

                    // in level reconnect.
                    if (GameLevel.Instance != null)
                        GameLevel.Instance.OnReconnect();

                    // in city level reconnect.
                    if (CityLevel.Instance != null)
                        CityLevel.Instance.OnReconnect();

                    Debug.Log("ReLogin sucess");
                }));
        }
        mReconnecting = false;
        yield break;
    }
}
