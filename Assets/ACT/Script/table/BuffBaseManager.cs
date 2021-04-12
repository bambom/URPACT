using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ProtoBuf.ProtoContract(ImplicitFields = ProtoBuf.ImplicitFields.AllFields)]
public class BuffBase : ITableItem
{
    public SmartInt ID;
    public SmartInt Type;
    public SmartInt Time;
    public SmartInt SpecialEffect;
    public string Parameter;
    public string AttribEffect;
    public SmartInt Multipy;
    public SmartInt Add;
    public SmartInt SummonUnit;
    public string Effect;
    public string KeepEffect;
    public string EndEffect;
	
	public int Key () { return ID; }
};

public class BuffBaseManager : TableManager<BuffBase, BuffBaseManager>
{
	public override string TableName() { return "BuffBase"; }
}

