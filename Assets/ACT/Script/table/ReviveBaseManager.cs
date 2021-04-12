using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ProtoBuf.ProtoContract(ImplicitFields = ProtoBuf.ImplicitFields.AllFields)]
public class ReviveBase : ITableItem
{
    public SmartInt ID;
    public SmartInt Cost;

    public int Key() { return ID; }
};

public class ReviveBaseManager : TableManager<ReviveBase, ReviveBaseManager>
{
    public override string TableName() { return "ReviveBase"; }
    //public override int MakeKey(ReviveBase obj) { return obj.Key(); }
}

