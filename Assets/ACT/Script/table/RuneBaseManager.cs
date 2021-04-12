using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ProtoBuf.ProtoContract(ImplicitFields = ProtoBuf.ImplicitFields.AllFields)]
public class RuneBase : ITableItem
{
    public SmartInt ID;
    public SmartInt Value;
    public SmartInt Mode;
    public SmartInt Chance;
    public SmartInt Target;
    public SmartInt BuffID;

    public int Key() { return ID; }
};

public class RuneBaseManager : TableManager<RuneBase, RuneBaseManager>
{
    //public override int MakeKey(RuneBase obj) { return obj.Key(); }
	public override string TableName () { return "RuneBase"; }
}

