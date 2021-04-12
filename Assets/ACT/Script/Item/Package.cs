using UnityEngine;
using System.Collections.Generic;

public enum PackageType
{
    BackPack = 0,
    SkillPack = 1,
}

public class PackageItem
{
	int 	mSortIndex;
	Item 	mItem;
	public int SortIndex {get { return mSortIndex; } set{ mSortIndex = value ;}}
	public Item PackItm {get {return mItem; } }
	public PackageItem(Item item)
	{
		mItem = item;
		mSortIndex = SortIndexGet(item);
	}
	
	int SortIndexGet(Item item)
	{
		//Level 120  120*100
		int SortIndex = item.ItemBase.MainType*1000000 + item.ItemBase.SubType*100000 
			+ item.ItemBase.Level*100 + item.ItemBase.Quality*10;
		
		switch(item.ItemBase.MainType)
		{
			case (int)EItemType.Equip:	
				{
					SortIndex += item.ToEquipItem().GetAttrib(EquipAttrib.StrengthenCount)%10;
					if(item.ToEquipItem().Equiped)
					{
						Debug.Log ("Item Equiped : " + item.ItemBase.Name);
						return SortIndex;
					}
				}break;
			case (int)EItemType.Stone:	SortIndex += (item.ItemBase.ID%1000); break;
			case (int)EItemType.Rune:	SortIndex += (item.ItemBase.ID%1000); break;
			case (int)EItemType.Props:	break;
			default: SortIndex = 0; break;
		}
		// have no equipattrib
		return  SortIndex += 1*10000000;
	}
}

// Package
//  the package class.
[ProtoBuf.ProtoContract]
public class Package : ICommand
{
    // the main item entry, do not edit it directly.
    // the JSON just support string as Key.
    [ProtoBuf.ProtoMember(1)]
    public Dictionary<string, Item> Items { get; set; }

    [ProtoBuf.ProtoMember(2)]
    public int Pages { get; set; }

    public Package()
    {
        Items = new Dictionary<string, Item>();
    }
		
    // init this package.
    public bool Init()
    {
        foreach (Item item in Items.Values)
            item.Init();

        return true;
    }

    // find a item based on there id.
    public Item Find(int ID)
    {
        Item item = null;
        Items.TryGetValue(ID.ToString(), out item);
        return item;
    }

    // remove item.
    public bool Remove(int ID)
    {
        Item item = Find(ID);
        if (item == null)
        {
            Debug.LogError("Item with ID not found: " + ID);
            return false;
        }

        Items.Remove(ID.ToString());
        return true;
    }

    // add item
    public bool Add(Item item)
    {
        if (Find(item.ID) != null)
        {
            Debug.LogError("Item with ID already exist: " + item.ID);
            return false;
        }

        item.Init();
        Items[item.ID.ToString()] = item;
        return true;
    }
	
	#region PackNew
	public void MaxIDRecord()
	{
		BackPack.PackageMaxIDSet();
	}
	
	#endregion
	
	#region Sort
	List<PackageItem> mPackItemList = new List<PackageItem>();
	public List<PackageItem> PackSortList { get{ return mPackItemList; } }
	public void Sort()
	{
		if(mPackItemList.Count > 0)
			mPackItemList.Clear();
		
		Debug.Log("Items.Count : " + Items.Count + "Items.Page : " + Items.Count);
		foreach (Item item in Items.Values){
			PackageItem PItem = new PackageItem(item);
			mPackItemList.Add(PItem);
		}
		mPackItemList.Sort((x, y) => x.SortIndex.CompareTo (y.SortIndex));
	}
	#endregion Sort
}

// Packages
//  the packages
[ProtoBuf.ProtoContract]
public class Packages : ICommand
{
    [ProtoBuf.ProtoMember(1)]
    public Package BackPack { get; set; }

    [ProtoBuf.ProtoMember(2)]
    public Package SkillPack { get; set; }

    public Packages()
    {
        BackPack = new Package();
        SkillPack = new Package();
    }

    public void Init()
    {
        BackPack.Init();
		BackPack.Sort();
		BackPack.MaxIDRecord();
		
        SkillPack.Init();
    }
}
