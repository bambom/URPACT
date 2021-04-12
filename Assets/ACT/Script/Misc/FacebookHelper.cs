using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class FBResult 
{
}

public delegate void FacebookDelegate(FBResult ret);

public static class FacebookHelper
{
	public static bool IsInited = false;
    //public static bool IsLoggedIn { get { return FB.IsLoggedIn; } }
	public static bool IsLoggedIn { get { return false; } }
	
	public static void Init()
	{
		//FB.Init(OnInitComplete, OnHideUnity);
	}
	
	static void OnInitComplete()
	{
		IsInited = true;
	}
	
	static void OnHideUnity(bool isGameShown)
	{
	}


    public static void Login(FacebookDelegate callback)
	{
		if (!IsInited) 
		{
			Debug.LogError("Fail init facebook");
			return;
		}
		
		//FB.Login("publish_actions", callback);
	}


    public static void Logout()
    {
        //FB.Logout();
    }

    public static IEnumerator PublishScreenshot(string message)
    {
		yield return new WaitForEndOfFrame();
		/*
        string screenshotName = "Screenshot.png";
        Application.CaptureScreenshot(screenshotName);
        var pathToImage = Application.persistentDataPath + "/" + screenshotName;
        if (!File.Exists(pathToImage))
		{
			Debug.LogError("ImageNotFound:" + pathToImage);
            yield break;
		}

        var screenshot = File.ReadAllBytes(pathToImage);

        var wwwForm = new WWWForm();
        wwwForm.AddBinaryData("image", screenshot, screenshotName);
        wwwForm.AddField("message", message);
		*/
		/*
        FB.API("/me/photos", Facebook.HttpMethod.POST, delegate(FBResult result) {
			Debug.Log("data: " + result.Text + " err:" + result.Error);
		}, wwwForm);
		*/
    }
	
	public static void PublishMessage(string message)
	{
		/*
        GameConfig config = MainScript.Instance.Config;
		FB.Feed(
            link: config != null ? config.link : "http://god.sincegame.com",
			linkName: config != null ? config.linkName : "God of Destiny",
			linkCaption: config != null ? config.linkCaption : "Find your hero between Warrior Assassin and Mage",
			linkDescription: config != null ? config.linkDescription : "Download God of Destiny now",
			picture: config != null ? config.picture : "http://god.sincegame.com/God/Icon.png");
			*/
	}

    public static void PublishScore(int score)
    {
		/*
        if (!IsLoggedIn)
            return;

        var query = new Dictionary<string, string>();
        query["score"] = score.ToString();
        //FB.API("/me/scores", Facebook.HttpMethod.POST, delegate(FBResult r) { FbDebug.Log("Result: " + r.Text); }, query);
        */
    }
}
