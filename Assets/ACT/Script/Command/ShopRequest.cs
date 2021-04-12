using System.Collections.Generic;
using UnityEngine;
using LitJson;

[ProtoBuf.ProtoContract]
public class BuyCmd : RequestCmd
{
    [ProtoBuf.ProtoMember(1)]
    public int id { get; set; }

    [ProtoBuf.ProtoMember(2)]
    public bool use { get; set; } 

    public BuyCmd() { }
    public BuyCmd(int _id) : this(_id, false) { }
    public BuyCmd(int _id, bool _use) { id = _id; use = _use; }
}

[ProtoBuf.ProtoContract]
public class SellCmd : RequestCmd
{
    [ProtoBuf.ProtoMember(1)]
    public int id { get; set; }

    [ProtoBuf.ProtoMember(2)]
    public int count { get; set; } 

    public SellCmd() { }
    public SellCmd(int _id, int _count) { id = _id; count = _count; }
}

[ProtoBuf.ProtoContract]
public class BatchSellCmd : RequestCmd
{
    [ProtoBuf.ProtoMember(1)]
    public int[] ids { get; set; } 

    public BatchSellCmd() { }
    public BatchSellCmd(int[] _ids) { ids = _ids; }
}


[ProtoBuf.ProtoContract]
public class RechargeCmd : RequestCmd
{
    [ProtoBuf.ProtoMember(1)]
    public string receipt { get; set; } 

    public RechargeCmd() { }
    public RechargeCmd(string _receipt) { receipt = _receipt; }
}

[ProtoBuf.ProtoContract]
public class ExchangeCmd : RequestCmd
{
    [ProtoBuf.ProtoMember(1)]
    public string product { get; set; }

    public ExchangeCmd() { }
    public ExchangeCmd(string _product) { product = _product; }
}

