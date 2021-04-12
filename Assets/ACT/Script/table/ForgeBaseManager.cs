using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ProtoBuf.ProtoContract(ImplicitFields = ProtoBuf.ImplicitFields.AllFields)]
public class ForgeBase : ITableItem
{
    public SmartInt ID;
    public SmartInt Gold;
    public string Damage;
    public string Defense;
    public string SpecialDamage;
    public string SpecialDefense;
    public string Hit;
	public string Block;
	public string Critical;
	public string Tough;
	public string HPMax;
	
	public int Key () { return ID; }
};

public class ForgeBaseManager : TableManager<ForgeBase, ForgeBaseManager>
{
	public override string TableName() { return "ForgeBase"; }
    //public override int MakeKey(ForgeBase obj) { return obj.Key (); }
}

