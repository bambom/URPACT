using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ProtoBuf.ProtoContract(ImplicitFields = ProtoBuf.ImplicitFields.AllFields)]
public class UnitBase : ITableItem
{
    public SmartInt ID;
    public SmartInt Type;
    public SmartInt AutoOpen;

    public string Name;
    public string Prefab;
    public string ShowPrefab;
    public string Icon;
    public string Label;
    public string CustomAction1;
    public string CustomAction2;
    public string CustomAction3;
    public string CustomWord1;
    public string CustomWord2;
    public string CustomWord3;
    public string Portrait1;
    public string Portrait2;
    public string Portrait3;
    public string Function1;
    public string BtnName1;
	public string Function2;
    public string BtnName2;
	public string Function3;
    public string BtnName3;
	public string Show;

	public int Key () { return ID; }
};

public class UnitBaseManager : TableManager<UnitBase, UnitBaseManager>
{
	public override string TableName () { return "UnitBase"; }
    ////public override int MakeKey(UnitBase obj) { return obj.Key (); }
}

