using System;
using UnityEngine;
using System.Xml;
using System.Collections;
using System.Collections.Generic;

public class Global
{
	public static bool NewMailCome = false;
    public static InputBox GInputBox = null;
    public static LevelData GLevelData = null;
    public static LevelSetup GLevelSetup = null;
    public static PvpData GPvpData = null;
    public static bool PauseAll = false;
    public static SmartInt TotalMonster;
	public static LoadingSceneInfo CurSceneInfo = new LoadingSceneInfo();
	public static bool GuideMode = false;
	public static int GuideRole = 0;
	public static string SaveRoleID = "RoleID";
	public static string SaveClientID = "ClientID";
    public const int ReviveBuffId = 500;
	
	public static int CurGuideID = 0;
	public static int CurStep = 1;
	static bool mPause = false;
    public static bool Pause
    {
        get { return mPause; }
        set
        {
            mPause = value;
            if (mPause)
                Time.timeScale = 0;
            else
                Time.timeScale = 1;
        }
    }
	
    public static void Initialize()
    {
        //ActionManager.Instance.Init();
        SoundManager.Instance.Init();
    }

    public static DateTime JSLongToDataTime(long longTime)
    {
        long tempLong = new DateTime(1970, 1, 1).Ticks +
            longTime * 10000 + TimeZone.CurrentTimeZone.GetUtcOffset(DateTime.Now).Hours * 3600 * (long)10000000;
        return new DateTime(tempLong);
    }

    public static void ShowLoadingStart()
    {
        if (EmptyForLoadingWnd.Exist)
            EmptyForLoadingWnd.Instance.Show();
        else
            EmptyForLoadingWnd.Instance.Open();
    }

    public static void ShowLoadingEnd()
    {
        if (EmptyForLoadingWnd.Exist)
            EmptyForLoadingWnd.Instance.Close();
    }
}
