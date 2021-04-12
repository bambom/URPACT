using System.Collections.Generic;
using UnityEngine;
using LitJson;

[ProtoBuf.ProtoContract]
public class PveInfo : ICommand
{
    [ProtoBuf.ProtoContract]
    public class PveData : ICommand
    {
        [ProtoBuf.ProtoMember(1)]
        public int PassCount { get; set; }
        [ProtoBuf.ProtoMember(2)]
        public int MaxScore { get; set; }
        [ProtoBuf.ProtoMember(3)]
        public int LastScore { get; set; }
        [ProtoBuf.ProtoMember(4)]
        public long Time { get; set; }
    }

    [ProtoBuf.ProtoMember(1)]
    public Dictionary<string, PveData> Levels { get; set; }
}

[ProtoBuf.ProtoContract]
public class ReviveInfo : ICommand
{
    [ProtoBuf.ProtoMember(1)]
    public int Count { get; set; }
}

[ProtoBuf.ProtoContract]
public class EnterLevelCmd : RequestCmd
{
    [ProtoBuf.ProtoMember(1)]
    public int level { get; set; }

    public EnterLevelCmd() { }
    public EnterLevelCmd(int id) { level = id; }
}

[ProtoBuf.ProtoContract]
public class OpenCellCmd : RequestCmd
{
    [ProtoBuf.ProtoMember(1)]
    public int type { get; set; }

    public OpenCellCmd() { }
    public OpenCellCmd(int inType) { type = inType; }
}

[ProtoBuf.ProtoContract]
public class LeaveLevelCmd : RequestCmd
{
}

[ProtoBuf.ProtoContract]
public class FinishLevelCmd : RequestCmd
{
    [ProtoBuf.ProtoContract]
    public class PickData : ICommand
    {
        [ProtoBuf.ProtoMember(1)]
        public int id { get; set; }

        [ProtoBuf.ProtoMember(2)]
        public int num { get; set; }
    }

    [ProtoBuf.ProtoMember(1)]
    public int score { get; set; }

    [ProtoBuf.ProtoMember(2)]
    public int gold { get; set; }

    [ProtoBuf.ProtoMember(3)]
    public int exp { get; set; }

    [ProtoBuf.ProtoMember(4)]
    public PickData[] drops { get; set; }
}

[ProtoBuf.ProtoContract]
public class ReviveInfoCmd : RequestCmd
{
}

[ProtoBuf.ProtoContract]
public class ReviveCmd : RequestCmd
{
}

[ProtoBuf.ProtoContract]
public class GetPveInfoCmd : RequestCmd
{
}

[ProtoBuf.ProtoContract]
public class GetTurntableDataCmd : RequestCmd
{
    // it will return the drop group index.
    // int groupIndex = response.Parse<int>();
}

[ProtoBuf.ProtoContract]
public class FetchTurntableRewardCmd : RequestCmd
{
    // it will return the selected sequence.
    // int sequence = response.Parse<int>();
}
