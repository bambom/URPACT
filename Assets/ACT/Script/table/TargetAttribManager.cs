using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[ProtoBuf.ProtoContract(ImplicitFields = ProtoBuf.ImplicitFields.AllFields)]
public class TargetAttrib : ITableItem
{
    public SmartInt MainType;
    public SmartInt SubType;
    public string Attrib;
	public string ShowName;
	
	public static int MakeKey(int MainType, int SubType) 
	{ 
		return (MainType << 16) + SubType; 
	}
	
    public int Key() { return MakeKey(MainType, SubType); }
	
};

public class TargetAttribManager : TableManager<TargetAttrib, TargetAttribManager>
{
	public override string TableName () { return "TargetAttrib"; }
    //public override int MakeKey(TargetAttrib obj) { return obj.Key (); }
	
	public TargetAttrib GetItem(int MainType, int SubType)
    {
        return GetItem(TargetAttrib.MakeKey(MainType, SubType));
    }
}