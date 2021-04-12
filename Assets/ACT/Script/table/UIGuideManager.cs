using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ProtoBuf.ProtoContract(ImplicitFields = ProtoBuf.ImplicitFields.AllFields)]
public class UIGuide : ITableItem
{
    public SmartInt GuideID;
    public string 	Desc;
	public SmartInt Priority;
	public string 	CurWnd;
	public SmartInt Counts;
	public SmartInt Steps;
	public SmartInt Useup;
	public SmartInt Progress;
	public SmartInt Level;
	public SmartInt SP;
	public string Item;
	public SmartInt Trigger;
	public SmartInt Dir;
	public SmartInt Wait;
	public SmartInt UI;
	public string 	ControlName;
	public SmartInt ControlType;
	public string 	Function;
	public string 	ReverseWnd;
	public string 	Notice;
	
	public static int MakeKey(int GuideID, int Steps) 
	{ 
		return (GuideID << 16) + Steps; 
	}
	
    public int Key() { return MakeKey(GuideID, Steps); }
};

public class UIGuideManager : TableManager<UIGuide, UIGuideManager>
{
    public override string TableName() { return "UIGuide"; }
	//public override int MakeKey(UIGuide obj) { return obj.Key (); }
	
	public UIGuide GetItem(int GuideID, int Steps)
    {
        return GetItem(UIGuide.MakeKey(GuideID, Steps));
    }
}
