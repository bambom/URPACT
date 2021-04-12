using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ProtoBuf.ProtoContract(ImplicitFields = ProtoBuf.ImplicitFields.AllFields)]
public class CombineBase : ITableItem
{
    public int ID;
    public byte Num;
    public int Target;
    public int Gold;
    public int Item;
    public byte ItemNum;
	
	public int Key () { return ID; }
};

public class CombineBaseManager : TableManager<CombineBase, CombineBaseManager>
{
	public override string TableName() { return "CombineBase"; }
	//public override int MakeKey (CombineBase obj) { return obj.Key (); }
}


