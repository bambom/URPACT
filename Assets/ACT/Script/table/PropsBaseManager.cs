using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ProtoBuf.ProtoContract(ImplicitFields = ProtoBuf.ImplicitFields.AllFields)]
public class PropsBase : ITableItem
{
    public SmartInt ID;
    public SmartInt HPPercent;
    public SmartInt HP;
    public SmartInt SoulPercent;
    public SmartInt Soul;
    public SmartInt AbilityPercent;
    public SmartInt Ability;
    public SmartInt BuffID;
	public SmartInt DropCount;

	
	public int Key () { return ID; }
};

public class PropsBaseManager : TableManager<PropsBase, PropsBaseManager>
{
	public override string TableName() { return "PropsBase"; }
    //public override int MakeKey(PropsBase obj) { return obj.Key (); }
}

