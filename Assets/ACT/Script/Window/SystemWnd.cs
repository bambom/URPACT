using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SystemWnd : Window<SystemWnd>
{
    public override string PrefabName { get { return "SystemWnd"; } }
	
	static bool bMusic = false;
	static bool bSound = false;
	
    protected override bool OnOpen()
    {
		Init();
        return base.OnOpen();
    }

    protected override bool OnClose()
    {
        return base.OnClose();
    }
	
	void Init()
	{
		UIEventListener.Get(Control("BtnReturnToRoleList")).onClick = OnClickBackToRoleList;
		//UIEventListener.Get(Control("Music")).onClick = OnClickMusic;
		//UIEventListener.Get(Control("Sound")).onClick = OnClickSound;
		UIEventListener.Get(Control("BtnAbout")).onClick = OnClickAbout;
		UIEventListener.Get(Control("BtnClose")).onClick = OnClickClose;
	}
	

    void OnClickBackToRoleList(GameObject go)
    {
		Reset(false);
		RequestLeaveGame();
    }
	
	void OnClickMusic(GameObject go)
    {
		bMusic = !bMusic;
		string[] musics = {
			"Audio_Test",
			"BackGroundMusic_Audio_0",
			"BackGroundMusic_Audio_1",
			"BackGroundMusic_Audio_2",
			"BackGroundMusic_Audio_3",
			"BackGroundMusic_Audio_4",
			"BackGroundMusic_Audio_5",
			"BackGroundMusic_Audio_6",
			"BackGroundMusic_Audio_7",
			"BackGroundMusic_Audio_8",
			"BackGroundMusic_Audio_9",
			"BackGroundMusic_Audio_Guide",
		};
		GameObject goAudio = null;
		
		for(int i = 0;i<musics.Length;i++)
		{
			goAudio = GameObject.Find(musics[i]);
			if( goAudio != null )
			{
				if( bMusic )
					goAudio.GetComponent<AudioSource>().volume = 0.0f;
				else
					goAudio.GetComponent<AudioSource>().volume = 1.0f;
			}
		}
	}
	
	void OnClickSound(GameObject go)
    {
		bSound = !bSound;
		
	}
	
	void OnClickAbout(GameObject go)
	{
		Reset(false);
		if(InGameMainWnd.Exist)
			InGameMainWnd.Instance.SetSystemStatus(false);
		Close();
		
		if(!AboutGameWnd.Exist)
			AboutGameWnd.Instance.Open();
	}
	
	void OnClickClose(GameObject go)
    {
		Reset(false);
		if(InGameMainWnd.Exist)
			InGameMainWnd.Instance.SetSystemStatus(false);
		Close();
	}
	
	public void Reset(bool Status)
	{
		UnitManager.Instance.LocalPlayer.UGameObject.SendMessage("LockInput",Status.ToString());	
	}
	
	
	void LeaveGame()
	{
		if (InGameMainWnd.Exist)
            InGameMainWnd.Instance.Close();

        if (Exist)
           Close();

        MainScript.Instance.UserLogined = false;
				
		if (MainScript.Instance.gameObject.GetComponent<Chat>() != null)
			MainScript.Instance.gameObject.GetComponent<Chat>().DestroyChat();
	
		if (MainScript.Instance.gameObject.GetComponent<SystemBroadcast>() != null)
			MainScript.Instance.gameObject.GetComponent<SystemBroadcast>().DestroyNotice();
		{
			SceneList SLItem = SceneListManager.Instance.GetItem((int)ESceneID.CreateRole,(int)ESceneType.CreateRole);
			Global.CurSceneInfo.ID = ESceneID.CreateRole;
			Global.CurSceneInfo.Type = ESceneType.CreateRole;
			Global.CurSceneInfo.Data = 1;
			Global.CurSceneInfo.Name = SLItem.Name;
	        MainScript.Instance.LoadLevel(Global.CurSceneInfo);			
		}
	}
	
	void RequestLeaveGame()
	{
		LoginHelper.roleList.users[LoginHelper.RoleIndex] = PlayerDataManager.Instance.Attrib;
		string user = LoginHelper.roleList.users[LoginHelper.RoleIndex].Name;
        MainScript.Instance.ClientUserName = user;
        Request(new LeaveGameCmd(user), delegate(string err, Response response)
        {
            if (!string.IsNullOrEmpty(err)){
                Debug.Log("LeaveGameCmd Error!!" + err);
                return;
            }	
			LeaveGame();
        });	
	}
}
