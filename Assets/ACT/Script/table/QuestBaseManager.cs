using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[ProtoBuf.ProtoContract(ImplicitFields = ProtoBuf.ImplicitFields.AllFields)]
public class QuestBase : ITableItem
{
	public SmartInt ID;
	public SmartInt Type;
	public string 	Name;
    public string 	Desc;
    public string 	Target;
	public SmartInt PreposeID;
    public string Icon;
    public SmartInt Scene;
    public SmartInt SP;
    public SmartInt Gold;
    public SmartInt Exp;
	public SmartInt Gem;
	public string 	Items;

	public int Key () { return ID; }
};

public class QuestBaseManager : TableManager<QuestBase, QuestBaseManager>
{
	public override string TableName () { return "QuestBase"; }
    //public override int MakeKey(QuestBase obj) { return obj.Key (); }
}
