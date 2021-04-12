using UnityEngine;
using System.Collections;

public class GetClick : MonoBehaviour {

	// Use this for initialization
	private Camera mUICamera;
	private GameObject mPopPanel;
	private Transform mButtonRoot;
	private string mPlayerName;
	
	void Start () {
		if (mUICamera == null) 
			mUICamera = GameObject.Find("Anchor").transform.parent.GetComponent<Camera>();//NGUITools.FindCameraForLayer(gameObject.layer);
		Debug.Log(mUICamera.transform.name);
	}
	
	void LateUpdate () {			
		if(InGameMainWnd.Exist && InGameMainWnd.Instance.WndObject.activeSelf)
			this.enabled = true;		
		else 
		{
			this.enabled = false;
			return;
		}
				
		Vector3 iClickPos = Vector3.zero;
		bool haveClick = false;
		//Touch event
		if( ( Input.touchCount > 0 ) &&
			( ( Input.GetTouch(0).phase == TouchPhase.Began ) ||
			  ( Input.GetTouch(0).phase == TouchPhase.Moved )))
		{
			iClickPos = Input.GetTouch(0).position;
			haveClick = true;
		}
		
		//Click event
		if ( Input.GetMouseButtonDown(0) )
		{
			iClickPos = Input.mousePosition;
			haveClick = true;
		}
		
		if (UICamera.hoveredObject == null)
		{
			RaycastHit hitInfo;
			Ray ClickRay = Camera.main.ScreenPointToRay( iClickPos);
			if (haveClick && Physics.Raycast(ClickRay, out hitInfo))
			{
				GameObject clickObj = hitInfo.collider.gameObject;
				UnitInfo unitInfo = clickObj.GetComponent<UnitInfo>();
				if (unitInfo != null && unitInfo.Unit != null && unitInfo.UnitType == EUnitType.EUT_OtherPlayer)
				{
					if(mPopPanel != null && mPopPanel.activeSelf)
						return;
					if(mPopPanel == null){					
						mPopPanel = GameObject.Instantiate(Resources.Load("PopPanel")) as GameObject;
						mButtonRoot = mPopPanel.transform.Find("ButtonRoot");
						
						UIEventListener.Get(mPopPanel.transform.Find("Background").gameObject).onClick = OnClickClose;
						UIEventListener.Get(mButtonRoot.Find("AddButton").gameObject).onClick = OnClickAddFrd;
						UIEventListener.Get(mButtonRoot.Find("ChatButton").gameObject).onClick = OnClickChat;
						UIEventListener.Get(mButtonRoot.Find("InfoButton").gameObject).onClick = OnClcikGetInfo;
					}		
					
					mPopPanel.transform.parent = GameObject.Find("Anchor").transform;
					mPopPanel.transform.localPosition = new Vector3(0,0,-20);
					mPopPanel.transform.localScale = Vector3.one;					
					mButtonRoot.position = mUICamera.ScreenToWorldPoint(iClickPos);
				    mButtonRoot.localPosition = GetPopPos(mButtonRoot.localPosition);		
					mButtonRoot.Find("PlayerName").GetComponentInChildren<UILabel>().text =
					    GetPlayerName(unitInfo.Unit as Player);			
					mPlayerName = GetPlayerName(unitInfo.Unit as Player);
				}	
			}
		}
	}
	
	void OnClickClose(GameObject go)
	{
		GameObject.Destroy(mPopPanel);
		Debug.Log("Alraeady Destroyed");
	}
	
	void OnClickAddFrd(GameObject go)
	{
		GameObject.Destroy(mPopPanel);
		StartCoroutine(MainScript.Execute(new AddFriendCmd(mPlayerName), delegate(string err, Response response)
        {
            if (!string.IsNullOrEmpty(err))
            {
				MessageBoxWnd.Instance.Show( err ,MessageBoxWnd.StyleExt.Alpha);
                return;
            }
			MessageBoxWnd.Instance.Show("AddFriendSuccess" ,MessageBoxWnd.StyleExt.Alpha);
        }));
	}
	
	void OnClickChat(GameObject go)
	{
		GameObject.Destroy(mPopPanel);
		if(SystemWnd.Exist)
			SystemWnd.Instance.Close();
		if( !ChatWnd.Exist )
			ChatWnd.Instance.Open();
		ChatWnd.Instance.OpenPrtChat(mPlayerName);
	}
	
	void OnClcikGetInfo(GameObject go)
	{	
		ShowPlayerInfoWnd.Instance.RequestGetUserInfo(mPlayerName,new Vector3(0,0,-30));	
	}
	
    string GetPlayerName(Player player)
	{
		return player.PlayerData.Attrib.Name;
	}
	
	Vector3 GetPopPos(Vector3 pos)
	{
		float xOffest = mButtonRoot.Find("Background").localScale.x/2;
		float yOffset = mButtonRoot.Find("Background").localScale.y/2;
		if(pos.x > 0 && pos.y > 0)
		{
			pos.x -= xOffest;
			pos.y -= yOffset;
		}
		
		else if(pos.x > 0 && pos.y < 0)
		{
			pos.x -= xOffest;
			pos.y += yOffset;
		}
			
		else if(pos.x < 0 && pos.y > 0)
		{
			pos.x += xOffest;
			pos.y -= yOffset;
		}
		
		else if(pos.x < 0 && pos.y < 0)
		{
			pos.x += xOffest;
			pos.y += yOffset;
		}
		pos.z = 0;
		return pos;
	}
}
