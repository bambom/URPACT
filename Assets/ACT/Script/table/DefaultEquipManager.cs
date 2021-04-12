using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ProtoBuf.ProtoContract(ImplicitFields = ProtoBuf.ImplicitFields.AllFields)]
public class DefaultEquip : ITableItem
{
    public int Role;
    public byte Index;
    //public byte Level;
    public int Show;
    public int Item;
    public int SkillLevel;
    public int Equiped;
    //public int Number;

    public int Key() { return (Role << 16) + Index; }
};

public class DefaultEquipManager : TableManager<DefaultEquip, DefaultEquipManager>
{
	public override string TableName() { return "DefaultPlayerEquip"; }

    public DefaultEquip GetItem(int role, int subType)
    {
        return GetItem((role << 16) + subType);
    }
}

