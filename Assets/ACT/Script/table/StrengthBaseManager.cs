using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ProtoBuf.ProtoContract(ImplicitFields = ProtoBuf.ImplicitFields.AllFields)]
public class StrengthBase : ITableItem
{
    public SmartInt ID;
    public SmartInt Gem;
    public SmartInt Recover;
	
    public int Key() { return ID; }
};

public class StrengthBaseManager : TableManager<StrengthBase, StrengthBaseManager>
{
    public override string TableName() { return "StrengthBase"; }
    //public override int MakeKey(StrengthBase obj) { return obj.Key(); }
}
