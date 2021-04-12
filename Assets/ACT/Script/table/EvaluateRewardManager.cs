using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ProtoBuf.ProtoContract(ImplicitFields = ProtoBuf.ImplicitFields.AllFields)]
public class EvaluateReward : ITableItem
{
    public SmartInt 	ID;
	public string   	Evaluate;
	public SmartInt  	Exp;
	public SmartInt   	Gold;
	public string		Item;
	public SmartInt   	Dower;
	
	public int Key () { return ID; }
};

public class EvaluateRewardManager : TableManager<EvaluateReward, EvaluateRewardManager>
{
	public override string TableName() { return "EvaluateReward"; }
    //public override int MakeKey(EvaluateReward obj) { return obj.Key (); }
}