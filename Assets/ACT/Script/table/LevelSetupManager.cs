using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ProtoBuf.ProtoContract(ImplicitFields = ProtoBuf.ImplicitFields.AllFields)]
public class LevelSetup : ITableItem
{
    public SmartInt ID;
    public string Type;
    public string Icon;
    public string Name;
    public string Intro;
    public int Progress;
    public SmartInt KillWeight;
    public SmartInt EvaluateWeight;
    public int PrePose;
    public SmartInt LimitStrength;
    public string NormalDrop;
    public SmartInt GeniusNum;
    public string GeniusDrop;
    public string BossDrop;
    public SmartInt ExtraNum;
    public string ExtraDrop;
    public string TurntableGroups;
    public int Turntable1;
    public int Turntable2;
    public int Turntable3;
	public SmartInt Power;

    public int Key() { return ID; }
};

public class LevelSetupManager : TableManager<LevelSetup, LevelSetupManager>
{
    public override string TableName() { return "LevelSetup"; }
    //public override int MakeKey(LevelSetup obj) { return obj.Key(); }
}
