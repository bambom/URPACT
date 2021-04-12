using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UpdateHelper : MonoBehaviour
{
    static UpdateHelper msInstance = null;
    public static UpdateHelper Instance { get { return msInstance; } }

    WWW mWWWTxt = null;
    public float ProgressTxt { get { if (mWWWTxt == null) return 0.0f; return mWWWTxt.progress; } }

    WWW mWWWUpdateTxt = null;
    public float UpdateProgressTxt { get { if (mWWWUpdateTxt == null) return 0.0f; return mWWWUpdateTxt.progress; } }

    Dictionary<string, AssetBundle> mAssetBundles = new Dictionary<string, AssetBundle>();

    public string Server;

    public void UpdateWWWReset()
    {
        if (mWWWTxt != null)
            mWWWTxt = null;

        if (mWWWUpdateTxt != null)
            mWWWUpdateTxt = null;
    }


    // Use this for initialization
    void Start()
    {
        msInstance = this;
        Global.ShowLoadingStart();

        switch (MainScript.Instance.Server)
        {
            case EServerIpList.Local:
				Server = "127.0.0.1";
				break;
            case EServerIpList.Lan_04:
                Server = "192.168.0.4";
                break;
            default:
                Server = "192.168.0.100";
                break;
        }

        StartCoroutine("BeginDownload");
    }

    void Update()
    {
        if (EmptyForLoadingWnd.Exist && ProgressTxt > 0.0f)
        {
            string str = string.Format("{0:C}", ProgressTxt * 100 - 1).Substring(1, 4);
            EmptyForLoadingWnd.Instance.Progress("Check Update ..." + str + "%");
        }

        if (ProgressTxt >= 1.0f && EmptyForLoadingWnd.Exist && UpdateProgressTxt > 0.0f)
        {
            string str = string.Format("{0:C}", UpdateProgressTxt * 100 - 1).Substring(1, 4);
            EmptyForLoadingWnd.Instance.Progress("Check Update ..." + str + "%");
        }
    }

    string GetPlatformCode()
    {
#if UNITY_IPHONE
            return "IOS";
#else
        return "Android";
#endif
    }

    public AssetBundle GetAsset(string name)
    {
        AssetBundle ret;
        mAssetBundles.TryGetValue(name, out ret);
        return ret;
    }

    public TextAsset GetAsset(string name, string key)
    {
        AssetBundle assetBundle;
        if (!mAssetBundles.TryGetValue(name, out assetBundle))
            return null;
        return null /*assetBundle.Load(key) as TextAsset*/;
    }

    IEnumerator BeginDownload()
    {
        /*
		string fileListUrl = string.Format("http://{0}/{1}/{2}/",
            Server,
            MainScript.Instance.Version,
            GetPlatformCode());
        mWWWTxt = new WWW(fileListUrl + "config.txt");
        Debug.Log(fileListUrl + "config.txt");
        while (!mWWWTxt.isDone) {
			yield return 0;
		}

		if (string.IsNullOrEmpty(mWWWTxt.error)) {
	        GameConfig config = LitJson.JsonMapper.ToObject<GameConfig>(mWWWTxt.text, false);
	        if (config != null)
	        {
	            MainScript.Instance.Config = config;
	            if (MainScript.Instance.CheckUpdate)
	            {
	                foreach (GameConfig.UpdateFile updateInfo in config.files)
	                {
	                    mWWWUpdateTxt = WWW.LoadFromCacheOrDownload(fileListUrl + updateInfo.file, updateInfo.ver);
						while (!mWWWUpdateTxt.isDone) {
							yield return 0;
						}
						if (string.IsNullOrEmpty(mWWWUpdateTxt.error)) {
	                    	mAssetBundles[updateInfo.file] = mWWWUpdateTxt.assetBundle;
						}
	                }
	            }
	        }
		}
		*/
        SendMessage("UpdateFinish");
        Global.ShowLoadingEnd();

        yield break;
    }
}
