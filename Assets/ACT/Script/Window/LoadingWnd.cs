using UnityEngine;
using System.Collections;

public class LoadingWnd : Window<LoadingWnd>
{
	UISprite mProgressBar;
    public override string PrefabName { get { return "LoadingWnd"; } }

    protected override bool OnOpen()
    {
		if(GameObject.Find("CameraControl")!= null){
			GameObject.Find("CameraControl").gameObject.SetActive(false);
		}
		mProgressBar = Control("back").GetComponent<UISprite>();
		
		int nLen  = LoadingTipsManager.Instance.GetAllItem().Length;
		int nStart = 10001;
		int nID = Random.Range( nStart, nStart+nLen-1 );
		Debug.Log( "mProgressBar.ID = " + nID + "nLen = " + nLen);
		Control("Tips").GetComponent<UILabel>().text = LoadingTipsManager.Instance.GetItem(nID).Tips;
		SceneList SLItem = SceneListManager.Instance.GetItem((int)Global.CurSceneInfo.ID,(int)Global.CurSceneInfo.Type);
		Debug.Log( "Global.CurSceneInfo.ID = " + Global.CurSceneInfo.ID 
					+ "Global.CurSceneInfo.Type = " + Global.CurSceneInfo.Type 
					+ "ShowLoading  = " + SLItem.Loading );
		
		Control("ShowLoading").GetComponent<MeshRenderer>().materials[0].mainTexture 
			= Resources.Load(SLItem.Loading) as Texture;
		
		WinStyle = WindowStyle.WS_Ext;
        return base.OnOpen();
    }

    protected override bool OnClose()
    {
		if(GameObject.Find("CameraControl")!= null){
			GameObject.Find("CameraControl").gameObject.SetActive(true);
		}
        return base.OnClose();
    }
	
	public void UpdateProgress(float progress)
	{
		mProgressBar.fillAmount = progress;
	}
	
}
