using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ProtoBuf.ProtoContract(ImplicitFields = ProtoBuf.ImplicitFields.AllFields)]
public class ShopBase : ITableItem
{
    public SmartInt ID;
    public SmartInt Item;
    public SmartInt Count;
    public SmartInt Type;
    public SmartInt Gold;
    public SmartInt Gem;
    public SmartInt SP;
    public SmartInt MinLevel;
    public SmartInt MaxLevel;
    public SmartInt Vip;
	
	public int Key () { return ID; }
};

public class ShopBaseManager : TableManager<ShopBase, ShopBaseManager>
{
	public override string TableName() { return "ShopBase"; }
    //public override int MakeKey(ShopBase obj) { return obj.Key (); }
}

