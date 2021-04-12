using System.Collections.Generic;
using UnityEngine;
using LitJson;

[ProtoBuf.ProtoContract]
public class GetBackPackCmd : RequestCmd
{
}

[ProtoBuf.ProtoContract]
public class GetSkillPackCmd : RequestCmd
{
}

[ProtoBuf.ProtoContract]
public class BuyPackagePageCmd : RequestCmd
{
}