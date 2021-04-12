using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ProtoBuf.ProtoContract(ImplicitFields = ProtoBuf.ImplicitFields.AllFields)]
public class SkillAttrib : ITableItem
{
    public SmartInt ID;
    public string Desc;
    public SmartInt Level;
    public string Action1;
    public string Action2;
    public string Action3;
    public string Icon2;
    public string Icon3;
    public SmartInt CD;
    public SmartInt LevelRequest;
    public SmartInt SP;
    public SmartInt Gold;
    public SmartInt Gem;
    public SmartInt Energy1;
    public SmartInt Energy2;
    public SmartInt Energy3;
    public SmartInt DamageCoff1;
    public SmartInt DamageBase1;
    public SmartInt DamageCoff2;
    public SmartInt DamageBase2;
    public SmartInt DamageCoff3;
    public SmartInt DamageBase3;
    public SmartInt Mode;
    public SmartInt Chance;
    public SmartInt Target;
    public SmartInt BuffID;
	
    public static int MakeKey(int id, int level) 
	{ 
		return (id << 16) + level; 
	}
    public int Key() { return MakeKey(ID, Level); }
};

public class SkillAttribManager : TableManager<SkillAttrib, SkillAttribManager>
{
    public override string TableName() { return "SkillAttrib"; }
    //public override int MakeKey(SkillAttrib obj) { return obj.Key(); }

    public SkillAttrib GetItem(int id, int level)
    {
        return GetItem(SkillAttrib.MakeKey(id, level));
    }
}
