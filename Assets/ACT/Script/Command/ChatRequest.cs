using System.Collections.Generic;
using UnityEngine;
using LitJson;


[ProtoBuf.ProtoContract]
public class ChatMessage : ICommand
{
    [ProtoBuf.ProtoMember(1)]
    public string user { get; set; }

    [ProtoBuf.ProtoMember(2)]
    public int channel { get; set; }

    [ProtoBuf.ProtoMember(3)]
    public string msg { get; set; }

    [ProtoBuf.ProtoMember(4)]
    public long time { get; set; }
}

[ProtoBuf.ProtoContract]
public class ChatMsgs : ICommand
{
    [ProtoBuf.ProtoMember(1)]
    public ChatMessage[] msgs { get; set; }

    [ProtoBuf.ProtoMember(2)]
    public long time { get; set; }
}

[ProtoBuf.ProtoContract]
public class ChatResponse : ICommand
{
    [ProtoBuf.ProtoMember(1)]
    public long time { get; set; }
}

[ProtoBuf.ProtoContract]
public class SendChatCmd : RequestCmd
{
    [ProtoBuf.ProtoMember(1)]
    public int channel { get; set; }

    [ProtoBuf.ProtoMember(2)]
    public string msg { get; set; }

    [ProtoBuf.ProtoMember(3)]
    public string target { get; set; }

    public SendChatCmd() { }

    // 世界频道
    public SendChatCmd(string _msg) { channel = (int)EChatChannel.World; msg = _msg; }

    // 私聊
    public SendChatCmd(string _msg, string _target) { channel = (int)EChatChannel.Private; msg = _msg; target = _target; }
}


[ProtoBuf.ProtoContract]
public class GetChatCmd : RequestCmd
{
    [ProtoBuf.ProtoMember(1)]
    public long since { get; set; }

    public GetChatCmd() { }
    public GetChatCmd(long _since) { since = _since; }
}