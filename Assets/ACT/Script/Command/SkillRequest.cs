using System.Collections.Generic;
using UnityEngine;
using LitJson;

[ProtoBuf.ProtoContract]
public class SkillLearnCmd : RequestCmd
{
    [ProtoBuf.ProtoMember(1)]
    public int id { get; set; } 

    public SkillLearnCmd() { }
    public SkillLearnCmd(int _id) { id = _id; }
}

[ProtoBuf.ProtoContract]
public class SkillUpgradeCmd : RequestCmd
{
    [ProtoBuf.ProtoMember(1)]
    public int id { get; set; } 

    public SkillUpgradeCmd() { }
    public SkillUpgradeCmd(int _id) { id = _id; }
}

[ProtoBuf.ProtoContract]
public class SkillEquipCmd : RequestCmd
{
    [ProtoBuf.ProtoMember(1)]
    public int id { get; set; } 

    public SkillEquipCmd() { }
    public SkillEquipCmd(int _id) { id = _id; }
}

[ProtoBuf.ProtoContract]
public class SkillBuySlotCmd : RequestCmd
{
    [ProtoBuf.ProtoMember(1)]
    public int id { get; set; } 

    public SkillBuySlotCmd() { }
    public SkillBuySlotCmd(int _id) { id = _id; }
}

[ProtoBuf.ProtoContract]
public class SkillAddRuneCmd : RequestCmd
{
    [ProtoBuf.ProtoMember(1)]
    public int id { get; set; }

    [ProtoBuf.ProtoMember(2)]
    public int rune { get; set; } 

    [ProtoBuf.ProtoMember(3)]
    public int slot { get; set; } 

    public SkillAddRuneCmd() { }
    public SkillAddRuneCmd(int _id, int _rune, int _slot) { id = _id; rune = _rune; slot = _slot; }
}