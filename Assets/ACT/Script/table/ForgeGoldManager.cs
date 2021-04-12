using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ProtoBuf.ProtoContract(ImplicitFields = ProtoBuf.ImplicitFields.AllFields)]
public class ForgeGold : ITableItem
{
    public int ID;
    public int ForgeGold1;
    public int ForgeGold2;
    public int ForgeGold3;
    public int ForgeGold4;
    public int ForgeGold5;
	
	public int Key () { return ID; }
};

public class ForgeGoldManager : TableManager<ForgeGold, ForgeGoldManager>
{
	public override string TableName() { return "ForgeGold"; }
    //public override int MakeKey(ForgeGold obj) { return obj.Key (); }
}

