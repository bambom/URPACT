using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ProtoBuf.ProtoContract(ImplicitFields = ProtoBuf.ImplicitFields.AllFields)]
public class SkillBase : ITableItem
{
    public SmartInt ID;
    public SmartInt Type;
    public SmartInt Slot1;
    public SmartInt Slot2;
    public SmartInt Slot3;
    public SmartInt Slot4;
	
    public int Key() { return ID; }
};

public class SkillBaseManager : TableManager<SkillBase, SkillBaseManager>
{
    public override string TableName() { return "SkillBase"; }
    //public override int MakeKey(SkillBase obj) { return obj.Key(); }
}
