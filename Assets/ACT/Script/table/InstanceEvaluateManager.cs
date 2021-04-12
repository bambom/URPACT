using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ProtoBuf.ProtoContract(ImplicitFields = ProtoBuf.ImplicitFields.AllFields)]
public class InstanceEvaluate : ITableItem
{
    public SmartInt ID;
    public SmartInt Base_Point;
    public SmartInt M2_Point;
    public SmartInt M2_Point_Limit;
    public string S_Grade;
    public string A_Grade;
    public string B_Grade;
    public string C_Grade;
    public string D_Grade;

    public int Key() { return ID; }
};

public class InstanceEvaluateManager : TableManager<InstanceEvaluate, InstanceEvaluateManager>
{
    public override string TableName() { return "InstanceEvaluate"; }
    //public override int MakeKey(InstanceEvaluate obj) { return obj.Key(); }
}