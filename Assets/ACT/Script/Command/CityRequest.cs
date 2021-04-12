using System.Collections.Generic;
using UnityEngine;
using LitJson;

[ProtoBuf.ProtoContract]
class PositionInfo
{
    [ProtoBuf.ProtoMember(1)]
    public float x { get; set; }

    [ProtoBuf.ProtoMember(2)]
    public float y { get; set; }

    [ProtoBuf.ProtoMember(3)]
    public float z { get; set; }
};

[ProtoBuf.ProtoContract]
class MoveInfo : ICommand
{
    [ProtoBuf.ProtoMember(1)]
    public string user { get; set; }

    [ProtoBuf.ProtoMember(2)]
    public PositionInfo pos { get; set; }
};

[ProtoBuf.ProtoContract]
class CityPlayerInfo
{
    [ProtoBuf.ProtoMember(1)]
    public PlayerData data { get; set; }

    [ProtoBuf.ProtoMember(2)]
    public PositionInfo pos { get; set; }
};

[ProtoBuf.ProtoContract]
class CityUsers : ICommand
{
    [ProtoBuf.ProtoMember(1)]
    public CityPlayerInfo[] users { get; set; }
};

[ProtoBuf.ProtoContract]
public class GetCityUsersCmd : RequestCmd
{
}

[ProtoBuf.ProtoContract]
public class MoveCmd : RequestCmd
{
    [ProtoBuf.ProtoMember(1)]
    public float x { get; set; }

    [ProtoBuf.ProtoMember(2)]
    public float y { get; set; }

    [ProtoBuf.ProtoMember(3)]
    public float z { get; set; }

    public MoveCmd() { }
    public MoveCmd(float _x, float _y, float _z)
    {
        x = _x;
        y = _y;
        z = _z;
    }
}