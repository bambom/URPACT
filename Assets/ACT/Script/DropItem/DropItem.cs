using UnityEngine;
using System.Collections;

public class DropItem : MonoBehaviour {
	
	public float margin_Y = 0.5f;
	public float TargetHeight = 0.5f;
	
	float mFlyTime = 0.5f;
	DropItemInfo mDropInfo = new DropItemInfo();
	FindItem mFindItem;
	bool mFly = false;
	Unit mPlayer;

	  // Update is called once per frame
    void Update()
    {
        if (mFly)
        {
            if (mFlyTime > Time.deltaTime)
            {
                Vector3 targetPos = new Vector3(mPlayer.Position.x, mPlayer.Position.y + TargetHeight, mPlayer.Position.z);
                transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime / mFlyTime);
                mFlyTime -= Time.deltaTime;
            }
            else
            {
				Hide();
				ShowTip();
            }
        }
    }
	
	public void Init(DropItemInfo info)
	{
		mPlayer = UnitManager.Instance.LocalPlayer;
		ItemBase item = ItemBaseManager.Instance.GetItem(info.ItemID);
		if(item != null)
		{
			transform.Find("DropItem").gameObject.GetComponent<UISprite>().spriteName = item.Icon;
			string Name = " " ;
			switch(item.Quality)
			{
				case (int)ItemQuality.IQ_White:Name = "[FFFFFF]" + item.Name + "[-]";break;
				case (int)ItemQuality.IQ_Green:Name = "[3BBC34]" + item.Name + "[-]";break;
				case (int)ItemQuality.IQ_Blue:Name = "[0F7BBF]" + item.Name + "[-]";break;
				case (int)ItemQuality.IQ_Purple:Name = "[400080]" + item.Name + "[-]";break;
				default:Name = "[FFFFFF]" + item.Name;break;
			}
			
			transform.Find("DropItemName").gameObject.GetComponent<UILabel>().text = Name;
			gameObject.SetActive(true);
			
			iTween.MoveFrom(gameObject, iTween.Hash("y", transform.localPosition.y + margin_Y, "time", 1f, "easetype", iTween.EaseType.linear, "looptype", iTween.LoopType.pingPong,"islocal",true));
			mDropInfo = info;
		}
	}
	
	void OnTriggerEnter(Collider collider)
	{
		iTween.Stop(gameObject);
		ItemBase item = ItemBaseManager.Instance.GetItem(mDropInfo.ItemID);
		
		if(item.MainType == (int)EItemType.Props && item.SubType == 3 )// fast use item
		{
			SoundManager.Instance.Play3DSound("UI_YaoJi",transform.position);
			FastUseItem(mDropInfo.ItemID, mDropInfo.ItemNum);
			Invoke("Hide", 0.1f);
		}
		else
		{
			SoundManager.Instance.Play3DSound("UI_Prop_Selected",transform.position);
			GameObject.Find("GameLevel").GetComponent<GameLevel>().PickUpItem(mDropInfo.ItemID, mDropInfo.ItemNum);
			mFindItem = new FindItem(item, mDropInfo.ItemNum);
			mFly = true;
		}
	}
	
	void Hide()
	{
		gameObject.SetActive(false);
		GameObject.Find("DropPool").GetComponent<ItemPool>().Revert(gameObject);
	}
	
	void FastUseItem(int id, int num)
	{
		PropsBase prop = PropsBaseManager.Instance.GetItem(id);
		if(prop == null)
			return;
		LocalPlayer player = UnitManager.Instance.LocalPlayer as LocalPlayer;
		for(int i=0; i<num; i++)
		{
			if(prop.BuffID > 0)
				player.AddBuff(prop.BuffID);
		}
	}
	
	void ShowTip()
	{
		if(mFindItem != null)
		{
			if(!FindItemWnd.Exist)
				FindItemWnd.Instance.Open();
			
			FindItemWnd.Instance.AddItem(mFindItem);
		}
	}
}
