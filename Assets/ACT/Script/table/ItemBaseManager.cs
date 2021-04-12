using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ProtoBuf.ProtoContract(ImplicitFields = ProtoBuf.ImplicitFields.AllFields)]
public class ItemBase : ITableItem
{
    public int ID;
    public string Name;
    public string Desc;
    public byte MainType;
    public byte SubType;
    public int Role;
    public byte Level;
    public byte Quality;
    public int Score;
    public string Model;
    public string Icon;
    public int SellPrice;
	
	public int Key () { return ID; }
};

public class ItemBaseManager : TableManager<ItemBase, ItemBaseManager>
{
	public override string TableName() { return "ItemBase"; }
	//public override int MakeKey(ItemBase obj) { return obj.Key (); }
}

