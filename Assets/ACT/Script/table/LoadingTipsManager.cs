using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[ProtoBuf.ProtoContract(ImplicitFields = ProtoBuf.ImplicitFields.AllFields)]
public class LoadingTips : ITableItem
{
    public SmartInt ID;
    public string Tips;
	public int Key () { return ID; }
};

public class LoadingTipsManager : TableManager<LoadingTips, LoadingTipsManager>
{
	public override string TableName() { return "LoadingTips"; }
    //public override int MakeKey(LoadingTips obj) { return obj.Key (); }
}
