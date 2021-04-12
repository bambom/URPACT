using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ProtoBuf.ProtoContract(ImplicitFields = ProtoBuf.ImplicitFields.AllFields)]
public class CloseupBase : ITableItem
{
    public SmartInt ID;
	public SmartInt Time;
	public SmartInt TotalTime;
	public SmartInt Type;
	public float Param;
	public SmartInt EndCall;
	public SmartInt BGEffect;
	public string CullingMask;
	public int MotionBlur;
	public string CameraAnimation;
	public string AmbientLight;
	public string GrayScale;
	public string PlayEffect;
	
    public int Key() { return ID;}

};

public class CloseupManager : CollectionTableManager<CloseupBase, CloseupManager>
{
    public override string TableName() { return "CloseUpClip"; }

};