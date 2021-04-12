using UnityEngine;
using System.Collections.Generic;

public enum EquipType
{
    Weapon = 1,
    Helmet = 2,
    Cloth = 3,
    Spaulders = 4,
    Shoes = 5,
}

// the custom attributes [Name] store in item.
public enum EquipAttrib
{
    Equip = 0,
	StrengthenCount,
    Damage,
    Defense,
    SpecialDamage,
    SpecialDefense,
    Critical,
    Tough,
    Hit,
    Block,
    MoveSpeed,
    FastRate,
    StiffAdd,
    StiffSub,
    AbilityMax,
    AbHitAdd,
    AbRestore,
    AbUseAdd,
	HPMax,

    Extra1,
    Extra2,
    Extra3,
}

// EquipItem
//  it is a shadow copy of item
//  just for use item as a equip easy.
public class EquipItem
{
    Item mItem;
    EquipBase mEquipBase;
    ExtraAttrib mExtra1;
    ExtraAttrib mExtra2;
    ExtraAttrib mExtra3;

    public Item Item { get { return mItem; } }
    public ItemBase ItemBase { get { return mItem.ItemBase; } }
    public EquipBase EquipBase { get { return mEquipBase; } }
    public ExtraAttrib Extra1 { get { return mExtra1; } }
    public ExtraAttrib Extra2 { get { return mExtra2; } }
    public ExtraAttrib Extra3 { get { return mExtra3; } }

    // is this equiped or not.
    public bool Equiped { get { return GetAttrib(EquipAttrib.Equip) != 0; } set { SetAttrib(EquipAttrib.Equip, value ? 1 : 0); } }

    // helper functions.
    public int GetAttrib(EquipAttrib attrib)
    {
        int value = 0;
        switch (attrib)
        {
            case EquipAttrib.Damage: value = mEquipBase.Damage; break;
            case EquipAttrib.Defense: value = mEquipBase.Defense; break;
            case EquipAttrib.SpecialDamage: value = mEquipBase.SpecialDamage; break;
            case EquipAttrib.SpecialDefense: value = mEquipBase.SpecialDefense; break;
            case EquipAttrib.Critical: value = mEquipBase.Critical; break;
            case EquipAttrib.Tough: value = mEquipBase.Tough; break;
            case EquipAttrib.Hit: value = mEquipBase.Hit; break;
            case EquipAttrib.Block: value = mEquipBase.Block; break;
            case EquipAttrib.MoveSpeed: value = mEquipBase.MoveSpeed; break;
            case EquipAttrib.FastRate: value = mEquipBase.FastRate; break;
            case EquipAttrib.StiffAdd: value = mEquipBase.StiffAdd; break;
            case EquipAttrib.StiffSub: value = mEquipBase.StiffSub; break;
			case EquipAttrib.HPMax: value = mEquipBase.HPMax;break;
			case EquipAttrib.StrengthenCount:value = 0;break;
        }
        return value + mItem.GetAttrib(attrib.ToString());
    }

    public void SetAttrib(EquipAttrib attrib, int value) { mItem.SetAttrib(attrib.ToString(), value); }

    public EquipItem(Item item)
    {
        mItem = item;
        mEquipBase = EquipBaseManager.Instance.GetItem(item.Base);
        if (mEquipBase == null)
            Debug.LogError("The equip isn't present in table: " + item.Base);

        int extra1 = mItem.GetAttrib(EquipAttrib.Extra1.ToString());
        int extra2 = mItem.GetAttrib(EquipAttrib.Extra2.ToString());
        int extra3 = mItem.GetAttrib(EquipAttrib.Extra3.ToString());

        if (extra1 > 0) mExtra1 = ExtraAttribManager.Instance.GetItem(extra1);
        if (extra2 > 0) mExtra2 = ExtraAttribManager.Instance.GetItem(extra2);
        if (extra3 > 0) mExtra3 = ExtraAttribManager.Instance.GetItem(extra3);
    }
}
