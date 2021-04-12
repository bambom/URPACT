using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ProtoBuf.ProtoContract(ImplicitFields = ProtoBuf.ImplicitFields.AllFields)]
public class ExtraAttrib : ITableItem
{
    public int ID;
    public int Sequence;
    public string Attrib;
    public int Value;

    public static int MakeKey(int id, int sequence)
    {
        return (id << 16) + sequence;
    }

    public int Key() { return MakeKey(ID, Sequence); }
};

public class ExtraAttribManager : TableManager<ExtraAttrib, ExtraAttribManager>
{
    public override string TableName() { return "ExtraAttrib"; }
    //public override int MakeKey(ExtraAttrib obj) { return obj.Key(); }

    public ExtraAttrib GetItem(int id, int sequence)
    {
        return GetItem(ExtraAttrib.MakeKey(id, sequence));
    }
}
