using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ProtoBuf.ProtoContract(ImplicitFields = ProtoBuf.ImplicitFields.AllFields)]
public class WishingWell : ITableItem
{
    public SmartInt ShopType;
    public string WellName;
	public string Icon;
	public string Desc;

	
    public int Key() { return ShopType; }
};

public class WishingWellManager : TableManager<WishingWell, WishingWellManager>
{
	public override string TableName() { return "WishingWell"; }
}