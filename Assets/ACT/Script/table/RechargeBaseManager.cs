using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[ProtoBuf.ProtoContract(ImplicitFields = ProtoBuf.ImplicitFields.AllFields)]
public class RechargeBase : ITableItem
{
	public SmartInt ID;
	public string Name;
	public string Icon;
	public string ProductID;
	public string SandboxID;
    public SmartInt Gem;
    public SmartInt USD;
    public SmartInt Gold;
    public SmartInt Item;

	public int Key () { return ID; }
};

public class RechargeBaseManager : TableManager<RechargeBase, RechargeBaseManager>
{
	public override string TableName() { return "RechargeBase"; }
    //public override int MakeKey(RechargeBase obj) { return obj.Key (); }
}
