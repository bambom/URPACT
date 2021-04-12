using UnityEngine;
using System.Collections;

public class ItemToolBar
{
	public string PrefabName { get { return "ItemToolBar"; } }
	
	private GameObject mItemToolBar;
	public GameObject GoItemToolBar {get {return mItemToolBar;} }
	
	
	public delegate void ClickWeapon(GameObject go);
	public ClickWeapon ClickedWeapon;

	public delegate void ClickHelmet(GameObject go);
	public ClickHelmet ClickedHelmet;

	public delegate void ClickShoulder(GameObject go);
	public ClickShoulder ClickedShoulder;

	public delegate void ClickArmor(GameObject go);
	public ClickArmor ClickedArmor;	
	
	public delegate void ClickBracers(GameObject go);
	public ClickBracers ClickedBracers;	
	
	public delegate void ClickBoots(GameObject go);
	public ClickBoots ClickedBoots;	
	
	public delegate void ClickAcesories(GameObject go);
	public ClickAcesories ClickedAcesories;
	
	public delegate void ClickTools(GameObject go);
	public ClickTools ClickedTools;	
	
	private ItemTool mStatus = ItemTool.Invalid;
	public ItemTool Focus { get{ return mStatus; } set{ mStatus = value; } }
	
	public ItemToolBar()
	{
		if(mItemToolBar == null){
			mItemToolBar = GameObject.Instantiate(Resources.Load(PrefabName)) as GameObject;
			mStatus = ItemTool.Invalid;
			ResetFocus();
		}
		
		
		UIEventListener.Get(Control("BtnWeapon")).onClick 		= OnClickWeaponItem;
		UIEventListener.Get(Control("BtnHelmet")).onClick 		= OnClickHelmetItem;
		UIEventListener.Get(Control("BtnShoulder")).onClick 	= OnClickShoulderItem;
		UIEventListener.Get(Control("BtnBracers")).onClick 		= OnClickBracersItem;
		UIEventListener.Get(Control("BtnBoots")).onClick 		= OnClickBootsItem;
		UIEventListener.Get(Control("BtnArmor")).onClick 		= OnClickArmorItem;
		UIEventListener.Get(Control("BtnAccessory")).onClick 	= OnClickAcesoriesItem;
		UIEventListener.Get(Control("BtnTools")).onClick 		= OnClickToolsItem;
	}
	
	private void ResetFocus()
	{
		GameObject[] Goes = {
			Control("BtnWeapon"),
			Control("BtnHelmet"),
			Control("BtnShoulder"),
			Control("BtnArmor"),
			Control("BtnBracers"),
			Control("BtnBoots"),
			Control("BtnAccessory"),
			Control("BtnTools")
		};
		
		foreach(GameObject go in Goes){
			go.transform.Find("Foreground").gameObject.SetActive(false);
		}
	}
	
	private void OnClickWeaponItem(GameObject go)
	{
		ResetFocus();
		if(Focus == ItemTool.Weapon){
			Focus = ItemTool.Invalid;
		}else{
			Focus = ItemTool.Weapon;
			go.transform.Find("Foreground").gameObject.SetActive(true);
		}
		if(null != ClickedWeapon )
		{
			ClickedWeapon(go);
		}
	}
	
	private void OnClickArmorItem(GameObject go)
	{
		ResetFocus();
		if(Focus == ItemTool.Armor){
			Focus = ItemTool.Invalid;
		}else{
			Focus = ItemTool.Armor;
			go.transform.Find("Foreground").gameObject.SetActive(true);
		}
		if(null != ClickedArmor )
		{
			ClickedArmor(go);
		}
	}
			
	private void OnClickAcesoriesItem(GameObject go)
	{
		ResetFocus();
		if(Focus == ItemTool.Accessory){
			Focus = ItemTool.Invalid;
		}else{
			Focus = ItemTool.Accessory;
			go.transform.Find("Foreground").gameObject.SetActive(true);
		}
		if(null != ClickedAcesories )
		{
			ClickedAcesories(go);
		}
	}
		
	private void OnClickHelmetItem(GameObject go)
	{
		ResetFocus();
		if(Focus == ItemTool.Helmet){
			Focus = ItemTool.Invalid;
		}else{
			Focus = ItemTool.Helmet;
			go.transform.Find("Foreground").gameObject.SetActive(true);
		}
		if(null != ClickedHelmet )
		{
			ClickedHelmet(go);
		}
	}
	
	private void OnClickShoulderItem(GameObject go)
	{
		ResetFocus();
		if(Focus == ItemTool.Shoulder){
			Focus = ItemTool.Invalid;
		}else{
			Focus = ItemTool.Shoulder;
			go.transform.Find("Foreground").gameObject.SetActive(true);
		}
		if(null != ClickedShoulder )
		{
			ClickedShoulder(go);
		}
	}
	
	private void OnClickBracersItem(GameObject go)
	{
		ResetFocus();
		if(Focus == ItemTool.Bracers){
			Focus = ItemTool.Invalid;
		}else{
			Focus = ItemTool.Bracers;
			go.transform.Find("Foreground").gameObject.SetActive(true);
		}
		if(null != ClickedBracers )
		{
			ClickedBracers(go);
		}
	}
	
	void OnClickBootsItem(GameObject go)
	{
		ResetFocus();
		if(Focus == ItemTool.Boots){
			Focus = ItemTool.Invalid;
		}else{
			Focus = ItemTool.Boots;
			go.transform.Find("Foreground").gameObject.SetActive(true);
		}
		if(null != ClickedBoots )
		{
			ClickedBoots(go);
		}
	}
	
	
	private void OnClickToolsItem(GameObject go)
	{
		ResetFocus();
		if(Focus == ItemTool.Item){
			Focus = ItemTool.Invalid;
		}else{
			Focus = ItemTool.Item;
			go.transform.Find("Foreground").gameObject.SetActive(true);
		}
		if(null != ClickedTools )
		{
			ClickedTools(go);
		}
	}
	
	
	private GameObject Control(string name)
    {
        return Control(name, mItemToolBar);
    }
	
	private GameObject Control(string name, GameObject parent)
    {
        Transform[] children = parent.GetComponentsInChildren<Transform>();
        foreach (Transform child in children){
            if (child.name == name)
                return child.gameObject;
        }
        return null;
    }
	
}

