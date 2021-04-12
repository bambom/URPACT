using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FindItem
{
	public ItemBase item;
	public int number;
	public FindItem(ItemBase itemBase, int num)
	{
		item = itemBase;
		number = num;
	}
}
public class FindItemWnd : Window<FindItemWnd> {
	
	List<FindItem>mItemList = new List<FindItem>();
	GameObject mTip;
	GameObject mIcon;
	GameObject mItemInfo;
    public override string PrefabName { get { return "FindTipWnd"; } }

    protected override bool OnOpen()
    {
		mTip = Control("Tip");
		mTip.SetActive(false);
		mIcon = mTip.transform.Find("Icon").gameObject;
		mItemInfo = mTip.transform.Find("ItemInfo").gameObject;
		WndObject.transform.localPosition = new Vector3(0, 0, -150);
        return base.OnOpen();;
    }

    protected override bool OnClose()
    {
        return base.OnClose();
    }
	
	public void AddItem(FindItem findItem)
	{
		mItemList.Add(findItem);
		if(mItemList.Count == 1)
			ShowTip();
	}
	
	void ShowTip()
	{
		mTip.SetActive(true);
		mTip.GetComponent<TweenAlpha>().Reset();
		mTip.GetComponent<TweenAlpha>().enabled = true;
		mTip.GetComponent<TweenAlpha>().onFinished= Finish;
		if(mItemList.Count > 0)
		{
			mIcon.GetComponent<UISprite>().spriteName = mItemList[0].item.Icon;
			string name = mItemList[0].item.Name;
			mItemInfo.GetComponent<UILabel>().color = GetItemColor(mItemList[0].item.Quality);
			string info = name;
			if(mItemList[0].number > 1)
				info = string.Format(name + "X{0}", mItemList[0].number);
			mItemInfo.GetComponent<UILabel>().text = info;
		}
	}
	
	void Finish(UITweener tween)
	{
		mTip.SetActive(false);
		Debug.Log("finish...........");
		mItemList.Remove(mItemList[0]);
		if(mItemList.Count == 0)
			return;
		ShowTip();
	}
	
	Color GetItemColor(int attribute)
	{
		switch(attribute)
		{
		case (int) QualitySprite.Button10_BaseItem_Quality_00: return Color.white;
		case (int) QualitySprite.Button10_BaseItem_Quality_01: return Color.green;
		case (int) QualitySprite.Button10_BaseItem_Quality_02: return Color.blue;
		case (int) QualitySprite.Button10_BaseItem_Quality_03: return new Color(0.4f, 0, 0.8f);//purple
		default: break;
		}	
		return Color.white;
	}
}
