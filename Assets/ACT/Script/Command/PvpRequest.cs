using System.Collections.Generic;
using UnityEngine;
using LitJson;

[ProtoBuf.ProtoContract]
public class PvpData : ICommand
{
    [ProtoBuf.ProtoMember(1)]
    public string server { get; set; }

    [ProtoBuf.ProtoMember(2)]
    public int game { get; set; }

    [ProtoBuf.ProtoMember(3)]
    public int pass { get; set; }

    [ProtoBuf.ProtoMember(4)]
    public int map { get; set; }

    [ProtoBuf.ProtoMember(5)]
    public int timeout { get; set; }

    [ProtoBuf.ProtoMember(6)]
    public PlayerData[] users { get; set; }
}

[ProtoBuf.ProtoContract]
public class PvpInfo : ICommand
{
    [ProtoBuf.ProtoMember(1)]
    public int PvpLevel { get; set; }

    [ProtoBuf.ProtoMember(2)]
    public int PvpExp { get; set; }

    [ProtoBuf.ProtoMember(3), System.ComponentModel.Description("总挑战数")]
    public int TotalCount { get; set; }

    [ProtoBuf.ProtoMember(4), System.ComponentModel.Description("总胜利数")]
    public int WinCount { get; set; }

    [ProtoBuf.ProtoMember(5), System.ComponentModel.Description("当前连胜数")]
    public int KeepWin { get; set; }
}

[ProtoBuf.ProtoContract]
public class PvpFinish : ICommand
{
    [ProtoBuf.ProtoContract]
    public class PvpFinishData : ICommand
    {
        [ProtoBuf.ProtoMember(1)]
        public string user { get; set; }

        [ProtoBuf.ProtoMember(2)]
        public int result { get; set; }

        [ProtoBuf.ProtoMember(3)]
        public int team { get; set; }

        [ProtoBuf.ProtoMember(4)]
        public int sp { get; set; }

        [ProtoBuf.ProtoMember(5)]
        public int exp { get; set; }
    }
        
    [ProtoBuf.ProtoMember(1)]
    public PvpFinishData[] datas { get; set; }
}

[ProtoBuf.ProtoContract]
public class JoinPvpCmd : RequestCmd
{
}

[ProtoBuf.ProtoContract]
public class LeavePvpCmd : RequestCmd
{
}

[ProtoBuf.ProtoContract]
public class FinishPvpCmd : RequestCmd
{
    [ProtoBuf.ProtoMember(1)]
    public int result { get; set; }

    public FinishPvpCmd() { }
    public FinishPvpCmd(int _result) { result = _result; }
}

[ProtoBuf.ProtoContract]
public class GetPvpInfoCmd : RequestCmd
{
}