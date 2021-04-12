using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ProtoBuf.ProtoContract(ImplicitFields = ProtoBuf.ImplicitFields.AllFields)]
public class SceneList : ITableItem
{
    public SmartInt ID;
	public SmartInt Type;
	public string   Name;
	public string   Intro;
	public string   Icon;
	public string   UnitBaseID;
	public string   ItemBaseID;
	public string 	Loading;
	
	public static int MakeKey(int ID, int Type) 
	{ 
		return (ID << 16) + Type; 
	}
	
    public int Key() { return MakeKey(ID, Type); }
};

public class SceneListManager : TableManager<SceneList, SceneListManager>
{
	public override string TableName() { return "SceneList"; }
	//public override int MakeKey(SceneList obj) { return obj.Key (); }
	
	public SceneList GetItem(int ID, int Type)
    {
        return GetItem(SceneList.MakeKey(ID, Type));
    }
	
}