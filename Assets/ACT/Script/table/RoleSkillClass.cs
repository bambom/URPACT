using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ProtoBuf.ProtoContract(ImplicitFields = ProtoBuf.ImplicitFields.AllFields)]
public class RoleSkillClass : ITableItem
{
    public SmartInt Role;
    public SmartInt SubType;
    public SmartInt Type;
    public string Name;
    public string Icon;
    public string PrefabName;

	
	public int Key () { return Role; }
};

public class RoleSkillClassManager : CollectionTableManager<RoleSkillClass, RoleSkillClassManager>
{
	public override string TableName () { return "RoleSkillClass"; }
	//public override int MakeKey (RoleSkillClass obj) { return obj.Key (); }
}