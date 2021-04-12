using System;
using System.Collections.Generic;

// the total profit will get if finished level.
[ProtoBuf.ProtoContract]
public class LevelProfit
{
    [ProtoBuf.ProtoMember(1)]
    public int Gold { get; set; }

    [ProtoBuf.ProtoMember(2)]
    public int Exp { get; set; }

    [ProtoBuf.ProtoMember(3)]
    public int Soul { get; set; }

    [ProtoBuf.ProtoMember(4)]
    public int SP { get; set; }

    [ProtoBuf.ProtoMember(5)]
    public DropInfo[] ChestDrops { get; set; }
}

// the info we need to build the whole level.
[ProtoBuf.ProtoContract]
public class LevelInfo
{
    [ProtoBuf.ProtoMember(1)]
    public int ID { get; set; }

    [ProtoBuf.ProtoMember(2)]
    public int WayLength { get; set; }

    [ProtoBuf.ProtoMember(3)]
    public int ExtraWay { get; set; }

    [ProtoBuf.ProtoMember(4)]
    public int GeniusNum { get; set; }

    [ProtoBuf.ProtoMember(5)]
    public int ExtraNum { get; set; }

    [ProtoBuf.ProtoMember(6)]
    public int[] Normal1 { get; set; }

    [ProtoBuf.ProtoMember(7)]
    public int[] Normal2 { get; set; }

    [ProtoBuf.ProtoMember(8)]
    public int[] Genius { get; set; }

    [ProtoBuf.ProtoMember(9)]
    public int[] Boss { get; set; }

    [ProtoBuf.ProtoMember(10)]
    public int[] Extra { get; set; }

    [ProtoBuf.ProtoMember(11)]
    public string[] Triggers { get; set; }

    [ProtoBuf.ProtoMember(12)]
    public int[] Chest { get; set; }
};

// the level data, coming from the server
// it was serialized vs JSON
[ProtoBuf.ProtoContract]
public class LevelData : ICommand
{
    [ProtoBuf.ProtoMember(1)]
    public LevelInfo Info { get; set; }

    [ProtoBuf.ProtoMember(2)]
    public LevelProfit Profit { get; set; }
};

[ProtoBuf.ProtoContract]
public class DropInfo : ICommand
{
    [ProtoBuf.ProtoMember(1)]
    public Dictionary<string, int> drops { get; set; }
}