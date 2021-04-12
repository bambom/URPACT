using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ProtoBuf.ProtoContract(ImplicitFields = ProtoBuf.ImplicitFields.AllFields)]
public class ForgeReturn : ITableItem
{
    public SmartInt SubType;
    public SmartInt Index;
    public SmartInt Max;
    public SmartInt Return;

    public int Key() { return (SubType << 16) + Index; }
};

public class ForgeReturnManager : TableManager<ForgeReturn, ForgeReturnManager>
{
    public override string TableName() { return "ForgeReturn"; }
    //public override int MakeKey(ForgeReturn obj) { return obj.Key(); }

    public ForgeReturn GetItem(int subType, int index)
    {
        return GetItem((subType << 16) + index);
    }
}

