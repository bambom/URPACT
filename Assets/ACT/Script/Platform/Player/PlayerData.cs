using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

[ProtoBuf.ProtoContract]
public class PlayerData : ICommand
{
    [ProtoBuf.ProtoMember(1)]
    public MainAttrib Attrib { get; set; }

    [ProtoBuf.ProtoMember(2)]
    public Packages Packages { get; set; }

    public PlayerData()
    {
        Attrib = new MainAttrib();
        Packages = new Packages();
    }

    public void Init()
    {
        Packages.Init();
    }
}