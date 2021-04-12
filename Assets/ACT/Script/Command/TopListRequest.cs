using System.Collections.Generic;
using UnityEngine;
using LitJson;

[ProtoBuf.ProtoContract]
public class GetTopListResponse : ICommand
{
    [ProtoBuf.ProtoContract]
    public class TopListAttrib
    {
        [ProtoBuf.ProtoMember(1)]
        public string Name { get; set; }
        [ProtoBuf.ProtoMember(2)]
        public int Role { get; set; }
        [ProtoBuf.ProtoMember(3)]
        public int Level { get; set; }
        [ProtoBuf.ProtoMember(4)]
        public int CurExp { get; set; }
        [ProtoBuf.ProtoMember(5)]
        public int TotalGold { get; set; }
        [ProtoBuf.ProtoMember(6)]
        public int Battle { get; set; }
        [ProtoBuf.ProtoMember(7)]
        public int Progress { get; set; }
        [ProtoBuf.ProtoMember(8)]
        public int PvpLevel { get; set; }
        [ProtoBuf.ProtoMember(9)]
        public int PvpExp { get; set; }
    }

    [ProtoBuf.ProtoContract]
    public class TopListItem
    {
        [ProtoBuf.ProtoMember(1)]
        public TopListAttrib Attrib { get; set; }
    }

    [ProtoBuf.ProtoMember(1)]
    public int page { get; set; }
    [ProtoBuf.ProtoMember(2)]
    public int rank { get; set; }
    [ProtoBuf.ProtoMember(3)]
    public int total { get; set; }
    [ProtoBuf.ProtoMember(4)]
    public TopListItem[] list { get; set; }
};

[ProtoBuf.ProtoContract]
public class GetTopListCmd : RequestCmd
{
    [ProtoBuf.ProtoMember(1)]
    public string type { get; set; }

    [ProtoBuf.ProtoMember(2)]
    public int page { get; set; }

    [ProtoBuf.ProtoMember(3)]
    public int size { get; set; }

    public GetTopListCmd(string _type, int _page, int _size)
    {
        type = _type;
        page = _page;
        size = _size;
    }
}