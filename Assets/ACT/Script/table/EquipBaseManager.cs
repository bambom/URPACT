using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ProtoBuf.ProtoContract(ImplicitFields = ProtoBuf.ImplicitFields.AllFields)]
public class EquipBase : ITableItem
{
    public int ID;
    public string Desc;
    public byte StrengthenCount;
    public int Damage;
    public int HPMax;
    public int Defense;
    public int SpecialDamage;
    public int SpecialDefense;
    public int Critical;
    public int Tough;
    public int Hit;
    public int Block;
    public int MoveSpeed;
    public int FastRate;
    public int StiffAdd;
    public int StiffSub;
	public string ExtraAttrib;
	
	public int Key () { return ID; }
};

public class EquipBaseManager : TableManager<EquipBase, EquipBaseManager>
{
	public override string TableName() { return "EquipBase"; }
    //public override int MakeKey(EquipBase obj) { return obj.Key (); }
}

