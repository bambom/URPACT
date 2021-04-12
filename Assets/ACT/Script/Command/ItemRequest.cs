using System.Collections.Generic;
using UnityEngine;
using LitJson;
using System.ComponentModel;

[ProtoBuf.ProtoContract]
public class EquipCmd : RequestCmd
{
    [ProtoBuf.ProtoMember(1)]
    public int id { get; set; }

    public EquipCmd() { }
    public EquipCmd(int _id) { id = _id; }
}

[ProtoBuf.ProtoContract]
public class UnEquipCmd : RequestCmd
{
    [ProtoBuf.ProtoMember(1)]
    public int id { get; set; }

    public UnEquipCmd() { }
    public UnEquipCmd(int _id) { id = _id; }
}

[ProtoBuf.ProtoContract]
public class CombineCmd : RequestCmd
{
    [ProtoBuf.ProtoMember(1)]
    public int id { get; set; }

    [ProtoBuf.ProtoMember(2), DefaultValue(1)]
    public int time { get; set; }

    public CombineCmd() { }
    public CombineCmd(int _id, int _time) { id = _id; time = _time; }
}

[ProtoBuf.ProtoContract]
public class StrengthenCmd : RequestCmd
{
    [ProtoBuf.ProtoMember(1)]
    public int equip { get; set; }
    
    [ProtoBuf.ProtoMember(2)]
    public int stone { get; set; }
    
    public StrengthenCmd() { }
    public StrengthenCmd(int equipid, int stoneid) { equip = equipid; stone = stoneid; }
}

[ProtoBuf.ProtoContract]
public class UseItemCmd : RequestCmd
{
    [ProtoBuf.ProtoContract]
    public class UseItemData : ICommand
    {
        [ProtoBuf.ProtoMember(1)]
        public int Gold { get; set; }

        [ProtoBuf.ProtoMember(2)]
        public int Gem { get; set; }

        [ProtoBuf.ProtoMember(3)]
        public int Exp { get; set; }

        [ProtoBuf.ProtoMember(4)]
        public int SP { get; set; }

        [ProtoBuf.ProtoMember(5)]
        public int Strength { get; set; }

        [ProtoBuf.ProtoMember(6)]
        public Dictionary<string, int> Items;
    }

    [ProtoBuf.ProtoMember(1)]
    public int id { get; set; }

    public UseItemCmd() { }
    public UseItemCmd(int _id) { id = _id; }
}

[ProtoBuf.ProtoContract]
public class EquipPropsCmd : RequestCmd
{
    [ProtoBuf.ProtoMember(1)]
    public int id { get; set; }
    
    [ProtoBuf.ProtoMember(2)]
    public int slot { get; set; }
    
    public EquipPropsCmd() { }
    public EquipPropsCmd(int _id, int _slot) { id = _id; slot = _slot; }
}