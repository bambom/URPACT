using UnityEngine;
using System.Collections.Generic;

[ProtoBuf.ProtoContract]
public class MainAttrib : ICommand
{
    [ProtoBuf.ProtoMember(1)]
    public string Name { get; set; }

    [ProtoBuf.ProtoMember(2)]
    public SmartInt Role { get; set; }

    [ProtoBuf.ProtoMember(3)]
    public SmartInt Level { get; set; }

    [ProtoBuf.ProtoMember(4)]
    public SmartInt Gold { get; set; }

    [ProtoBuf.ProtoMember(5)]
    public SmartInt CostGold { get; set; }

    [ProtoBuf.ProtoMember(6)]
    public SmartInt TotalGold { get; set; }

    [ProtoBuf.ProtoMember(7)]
    public SmartInt Gem { get; set; }

    [ProtoBuf.ProtoMember(8)]
    public SmartInt CostGem { get; set; }

    [ProtoBuf.ProtoMember(9)]
    public SmartInt TotalGem { get; set; }

    [ProtoBuf.ProtoMember(10)]
    public SmartInt Battle { get; set; }

    [ProtoBuf.ProtoMember(11)]
    public SmartInt CurHP { get; set; }

    [ProtoBuf.ProtoMember(12)]
    public SmartInt CurExp { get; set; }

    [ProtoBuf.ProtoMember(13)]
    public SmartInt CurAbility { get; set; }

    [ProtoBuf.ProtoMember(14)]
    public SmartInt CurSoul { get; set; }

    [ProtoBuf.ProtoMember(15)]
    public SmartInt NextExp { get; set; }

    [ProtoBuf.ProtoMember(16)]
    public SmartInt Exp { get; set; }

    [ProtoBuf.ProtoMember(17)]
    public SmartInt HPMax { get; set; }

    [ProtoBuf.ProtoMember(18)]
    public SmartInt HPRestore { get; set; }

    [ProtoBuf.ProtoMember(19)]
    public SmartInt SoulMax { get; set; }

    [ProtoBuf.ProtoMember(20)]
    public SmartInt SoulRestore { get; set; }

    [ProtoBuf.ProtoMember(21)]
    public SmartInt Damage { get; set; }

    [ProtoBuf.ProtoMember(22)]
    public SmartInt Defense { get; set; }

    [ProtoBuf.ProtoMember(23)]
    public SmartInt SpecialDamage { get; set; }

    [ProtoBuf.ProtoMember(24)]
    public SmartInt SpecialDefense { get; set; }

    [ProtoBuf.ProtoMember(25)]
    public SmartInt Critical { get; set; }

    [ProtoBuf.ProtoMember(26)]
    public SmartInt Tough { get; set; }

    [ProtoBuf.ProtoMember(27)]
    public SmartInt Hit { get; set; }

    [ProtoBuf.ProtoMember(28)]
    public SmartInt Block { get; set; }

    [ProtoBuf.ProtoMember(29)]
    public SmartInt MoveSpeed { get; set; }

    [ProtoBuf.ProtoMember(30)]
    public SmartInt FastRate { get; set; }

    [ProtoBuf.ProtoMember(31)]
    public SmartInt StiffAdd { get; set; }

    [ProtoBuf.ProtoMember(32)]
    public SmartInt StiffSub { get; set; }

    [ProtoBuf.ProtoMember(33)]
    public SmartInt AbilityMax { get; set; }

    [ProtoBuf.ProtoMember(34)]
    public SmartInt AbHitAdd { get; set; }

    [ProtoBuf.ProtoMember(35)]
    public SmartInt AbRestore { get; set; }

    [ProtoBuf.ProtoMember(36)]
    public SmartInt AbUseAdd { get; set; }

    [ProtoBuf.ProtoMember(37)]
    public SmartInt Progress { get; set; }

    [ProtoBuf.ProtoMember(38)]
    public SmartInt SP { get; set; }

    [ProtoBuf.ProtoMember(39)]
    public SmartInt PvpExp { get; set; }

    [ProtoBuf.ProtoMember(40)]
    public SmartInt PvpLevel { get; set; }

    [ProtoBuf.ProtoMember(41)]
    public SmartInt NewGuide { get; set; }
}