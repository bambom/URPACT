using System.Collections.Generic;
using UnityEngine;
using LitJson;

public class RequestCmd : ICommand
{
    public System.UInt16 Id { get; set; }

    public Client.OnResponse CallBack { get; set; }
}

[ProtoBuf.ProtoContract]
public class RequestMessage : ICommand
{
    [ProtoBuf.ProtoMember(1)]
    public ERequestTypes Type { get; set; }
}

[ProtoBuf.ProtoContract]
public class ServerTestRequest : RequestCmd
{
    [ProtoBuf.ProtoMember(1)]
    public byte[] Data { get; set; }

    public ServerTestRequest()
    {
        Data = System.Text.Encoding.UTF8.GetBytes("http://google-breakpad.googlecode.com/svn/trunk/");
    }
}