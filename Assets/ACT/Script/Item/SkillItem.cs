using UnityEngine;
using System;
using System.Collections.Generic;

public enum ESkillType
{
    Passive = 0, // the passive skill
    Normal = 1, // normal skill
    Role1 = 2, // the role skill
    Role2 = 3, // the role skill
    Super = 4, // the supper skill
}

public enum ERuneType
{
    DamageBase = 1, // 数值
    SubEnergy = 2, // 数值
    SubCD = 3, // 数值
    Buff = 4, // 万分比
}

public enum ESkillAttrib
{
    Lv = 0,
    Equip,
    SlotNum,
    Slot1,
    Slot2,
    Slot3,
    Slot4,
}

public enum EBuffMode
{
    None = 0,
    PlaySkill = 1,
    HitTarget = 2,
    OnHited = 3,
    OnHurt = 4,
    NormalAttack = 5,
    Always = 6,
    Max,
}

public enum EBuffTarget
{
    None = 0,
    Self = 1,
    HitTarget = 2,
    Attacker = 3,
}

// SkillItem
//  it is a shadow copy of item
//  just for use item as a skill easy.
public class SkillItem
{
    Item mItem;
    SkillBase mSkillBase;
    SkillAttrib mSkillAttrib;
    RuneBase mRuneBase1;
    RuneBase mRuneBase2;
    RuneBase mRuneBase3;
    RuneBase mRuneBase4;

    public Item Item { get { return mItem; } }
    public SkillBase SkillBase { get { return mSkillBase; } }
    public RuneBase RuneBase1 { get { return mRuneBase1; } }
    public RuneBase RuneBase2 { get { return mRuneBase2; } }
    public RuneBase RuneBase3 { get { return mRuneBase3; } }
    public RuneBase RuneBase4 { get { return mRuneBase4; } }
    public SkillAttrib SkillAttrib { get { return mSkillAttrib; } }
    public ESkillType SkillType { get { return (ESkillType)(int)mSkillBase.Type; } }
    public SkillInput SkillInput = null;
    public int SkillStage = 0;

    public int DamageCoff { get { return (SkillStage < 2) ? mSkillAttrib.DamageCoff1 : (SkillStage < 3 ? mSkillAttrib.DamageCoff2 : mSkillAttrib.DamageCoff3); } }
    public int DamageBase { get { return GetAttrib(ERuneType.DamageBase) + ((SkillStage < 2) ? mSkillAttrib.DamageBase1 : (SkillStage < 3 ? mSkillAttrib.DamageBase2 : mSkillAttrib.DamageBase3)); } }
    public int Energy1 { get { return mSkillAttrib.Energy1 * (10000 + GetAttrib(ERuneType.SubEnergy)) / 10000; } }
    public int Energy2 { get { return mSkillAttrib.Energy2 * (10000 + GetAttrib(ERuneType.SubEnergy)) / 10000; } }
    public int Energy3 { get { return mSkillAttrib.Energy3 * (10000 + GetAttrib(ERuneType.SubEnergy)) / 10000; } }
    public float CD { get { return mSkillAttrib.CD * 0.001f * (1.0f + GetAttrib(ERuneType.SubCD) * 0.0001f); } }

    // skill attributes.
    public int Lv { get { return GetAttrib(ESkillAttrib.Lv); } }
    public bool Equiped { get { return GetAttrib(ESkillAttrib.Equip) != 0; } }
    public int SlotNum { get { return GetAttrib(ESkillAttrib.SlotNum); } }
    public int Slot1 { get { return GetAttrib(ESkillAttrib.Slot1); } }
    public int Slot2 { get { return GetAttrib(ESkillAttrib.Slot2); } }
    public int Slot3 { get { return GetAttrib(ESkillAttrib.Slot3); } }
    public int Slot4 { get { return GetAttrib(ESkillAttrib.Slot4); } }

    public int GetAttrib(ESkillAttrib attrib) 
	{ 
		if(mItem != null)
			return mItem.GetAttrib(attrib.ToString()); 
		return 0;
	}
    public int GetAttrib(ERuneType attrib) 
	{
		if(mItem != null)
			return mItem.GetAttrib(attrib.ToString());
		return 0;
	}

    public SkillItem(Item item)
    {
        mItem = item;
        mSkillBase = SkillBaseManager.Instance.GetItem(item.Base);
        if (mSkillBase == null)
            Debug.LogError("Skill not found in [SkillBase] table: " + item.Base);

        mSkillAttrib = SkillAttribManager.Instance.GetItem(item.Base, Lv);
        if (mSkillAttrib == null)
            Debug.LogError("Skill not found in [SkillAttrib] table: " + item.Base);
    }
	
	public SkillItem(int skillId, int skillLevel)
    {
        mSkillBase = SkillBaseManager.Instance.GetItem(skillId);
        if (mSkillBase == null)
            Debug.LogError("Skill not found in [SkillBase] table: " + skillId);

        mSkillAttrib = SkillAttribManager.Instance.GetItem(skillId, skillLevel);
        if (mSkillAttrib == null)
            Debug.LogError("Skill not found in [SkillAttrib] table: " + skillId);
    }

    public void ApplyRune()
    {
        // clear the previous value.
        Array values = Enum.GetValues(typeof(ERuneType));
        foreach (System.Object obj in values)
        {
            string valueName = obj.ToString();
            if (mItem.Attrib.ContainsKey(valueName))
                mItem.Attrib.Remove(valueName);
        }

        mRuneBase1 = ApplyRune(Slot1);
        mRuneBase2 = ApplyRune(Slot2);
        mRuneBase3 = ApplyRune(Slot3);
        mRuneBase4 = ApplyRune(Slot4);
    }

    RuneBase ApplyRune(int rune)
    {
        if (rune == 0) return null;

        ItemBase itemBase = ItemBaseManager.Instance.GetItem(rune);
        if (itemBase == null)
        {
            Debug.LogError("Rune not found in [ItemBase] table: " + rune);
            return null;
        }

        RuneBase runeBase = RuneBaseManager.Instance.GetItem(rune);
        if (runeBase == null)
        {
            Debug.LogError("Rune not found in [RuneBase] table: " + rune);
            return null;
        }

        ERuneType runeType = (ERuneType)(int)itemBase.SubType;
		if (runeType != ERuneType.Buff)
		{
        	string runeStr = runeType.ToString();
        	mItem.SetAttrib(runeStr, mItem.GetAttrib(runeStr) + runeBase.Value);
		}
        return runeBase;
    }
}
