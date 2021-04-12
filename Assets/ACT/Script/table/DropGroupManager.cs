using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ProtoBuf.ProtoContract(ImplicitFields = ProtoBuf.ImplicitFields.AllFields)]
public class DropGroup : ITableItem
{
    public SmartInt ID;
	public SmartInt Sequence;
	public SmartInt Chance;
	public string Items;
	public SmartInt Min;
	public SmartInt Max;
    public SmartInt Gold;
    public SmartInt SP;
    public SmartInt Exp;
    public SmartInt Gem;

    public int Key() { return MakeKey(ID, Sequence); }
    public static int MakeKey(int id, int sequence) { return (id << 16) + sequence; }
};

public class DropGroupManager : TableManager<DropGroup, DropGroupManager>
{
	public override string TableName() { return "DropGroup"; }
    public DropGroup GetItem(int id, int sequence) { return GetItem(DropGroup.MakeKey(id, sequence)); }
}
