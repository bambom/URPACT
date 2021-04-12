using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ProtoBuf.ProtoContract(ImplicitFields = ProtoBuf.ImplicitFields.AllFields)]
public class RoleCreateName : ITableItem
{
    public SmartInt ID;
	public string Role;
	public string Surname;
	public string Forename;

    public int Key() { return ID;}

};

public class RoleCreateNameManager : TableManager<RoleCreateName, RoleCreateNameManager>
{
	public override string TableName() { return "RoleCreateName"; }
    //public override int MakeKey(RoleCreateName obj) { return obj.Key (); }
}