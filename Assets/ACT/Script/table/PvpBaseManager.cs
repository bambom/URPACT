using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ProtoBuf.ProtoContract(ImplicitFields = ProtoBuf.ImplicitFields.AllFields)]
public class PvpBase : ITableItem
{
	public SmartInt ID;
	public string   ArenaTitle;
    public SmartInt NextExp;
    public SmartInt Exp;
	
	public int Key () { return ID; }
};


public class PvpBaseManager : TableManager<PvpBase, PvpBaseManager>
{
	public override string TableName () { return "PvpBase"; }
    //public override int MakeKey(PvpBase obj) {  return obj.Key (); }
}

