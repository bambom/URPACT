using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ProtoBuf.ProtoContract(ImplicitFields = ProtoBuf.ImplicitFields.AllFields)]
public class PlayerAttrib : ITableItem
{
    public SmartInt ID;
    public SmartInt Level;
    public SmartInt NextExp;
    public SmartInt Exp;
    public SmartInt HPMax;
    public SmartInt HPRestore;
    public SmartInt SoulMax;
    public SmartInt SoulRestore;
    public SmartInt Damage;
    public SmartInt Defense;
    public SmartInt SpecialDamage;
    public SmartInt SpecialDefense;
    public SmartInt Critical;
    public SmartInt Tough;
    public SmartInt Hit;
    public SmartInt Block;
    public SmartInt MoveSpeed;
    public SmartInt FastRate;
    public SmartInt StiffAdd;
    public SmartInt StiffSub;
    public SmartInt AbilityMax;
    public SmartInt AbHitAdd;
    public SmartInt AbRestore;
    public SmartInt AbUseAdd;

    public static int MakeKey(int id, int level) 
	{ 
		return (id << 16) + level; 
	}

    public int Key() { return MakeKey(ID, Level); }
};

public class PlayerAttribManager : TableManager<PlayerAttrib, PlayerAttribManager>
{
    public override string TableName() { return "PlayerAttrib"; }
    //public override int MakeKey(PlayerAttrib obj) { return obj.Key(); }

    public PlayerAttrib GetItem(int id, int level)
    {
        return GetItem(PlayerAttrib.MakeKey(id, level));
    }
}
