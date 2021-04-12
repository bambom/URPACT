using UnityEngine;
using UnityEngine.SocialPlatforms;


public class GameCenter : MonoBehaviour
{
    static bool mbLogined = false;

    void Start()
    {
        if (Social.localUser.authenticated)
            LoginWithUserId(Social.localUser.id);
        else
			Authenticate(gameObject);
    }
	
    void HandleAuthenticated(bool success)
    {
        if (success)
            LoginWithUserId(Social.localUser.id);
        else
		{
            MessageBoxWnd.Instance.Show(("GameCenterAuthFailed"), MessageBoxWnd.Style.OK);
            MessageBoxWnd.Instance.OnClickedOk = Authenticate;
		}
    }
	
	void Authenticate(GameObject go)
	{
		Social.localUser.Authenticate(HandleAuthenticated);
	}

    void LoginWithUserId(string id)
    {
        if (mbLogined == false)
        {
            MainScript.Instance.ClientId = Social.localUser.id;
            gameObject.SendMessage("Login");
            mbLogined = true;
        }
    }
}
	
