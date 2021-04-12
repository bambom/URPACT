using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ProtoBuf.ProtoContract(ImplicitFields = ProtoBuf.ImplicitFields.AllFields)]
public class LevelSlider : ITableItem
{
    public SmartInt ID;
	public string   ClassName;
	public string   ClassIntro;
	public string   Level;
	public string   Icon;
	
	public int Key () { return ID; }
};

public class LevelSliderManager : TableManager<LevelSlider, LevelSliderManager>
{
	public override string TableName() { return "LevelSlider"; }
    //public override int MakeKey(LevelSlider obj) { return obj.Key (); }
}