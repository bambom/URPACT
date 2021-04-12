using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[ProtoBuf.ProtoContract(ImplicitFields = ProtoBuf.ImplicitFields.AllFields)]
public class SignInBase : ITableItem
{
	public SmartInt ID;
    public string IconLabel;
    public string Items;
    public SmartInt Exp;
    public SmartInt Gold;
	public SmartInt Strength;
    public SmartInt SP;
    public SmartInt Else;
	public SmartInt Gem;

	public int Key () { return ID; }
};

public class SignInBaseManager : TableManager<SignInBase, SignInBaseManager>
{
	public override string TableName () { return "SignInBase"; }
}
