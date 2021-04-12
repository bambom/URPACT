using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ProtoBuf.ProtoContract(ImplicitFields = ProtoBuf.ImplicitFields.AllFields)]
public class PackageBuyPage : ITableItem
{
    public SmartInt ID;
    public SmartInt Gem;

	public int Key () { return ID; }
};

public class PackageBuyPageManager : TableManager<PackageBuyPage, PackageBuyPageManager>
{
    public override string TableName() { return "PackagePage"; }
}

