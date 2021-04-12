using System;

[ProtoBuf.ProtoContract(ImplicitFields = ProtoBuf.ImplicitFields.AllFields)]
public class MonsterAttrib : ITableItem
{
    public SmartInt ID;
    public SmartInt Level;
    public SmartInt ExpWeight;
    public SmartInt GoldWeight;
    public SmartInt SoulWeight;
    public SmartInt HPMax;
    public SmartInt Damage;
    public SmartInt Defense;
    public SmartInt SpecialDamage;
    public SmartInt SpecialDefense;
    public SmartInt Critical;
    public SmartInt Tough;
    public SmartInt Hit;
    public SmartInt Block;

    public int Key() { return (ID << 16) + Level; }
}

public class MonsterAttribManager : TableManager<MonsterAttrib, MonsterAttribManager>
{
    public override string TableName() { return "MonsterAttrib"; }
    //public override int MakeKey(MonsterAttrib obj) { return obj.Key(); }

    public MonsterAttrib GetItem(int id, int lv)
    {
        return GetItem((id << 16) + lv);
    }
}
