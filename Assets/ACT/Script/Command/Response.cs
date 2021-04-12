using UnityEngine;
using System.Collections;
using System.IO;

[ProtoBuf.ProtoContract]
public class Response : ICommand
{
    [ProtoBuf.ProtoMember(1)]
    public string error { get; set; }

    [ProtoBuf.ProtoMember(2)]
    public string data { get; set; }

    [ProtoBuf.ProtoMember(3)]
    public int id { get; set; }

    public T Parse<T>()
    {
        return LitJson.JsonMapper.ToObject<T>(data, false);
    }
}
