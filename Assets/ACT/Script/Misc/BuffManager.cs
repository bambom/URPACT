using System;
using System.Collections.Generic;

public class BuffManager
{
    Unit mOwner;
    SmartInt[] mBuffMultipy;
    SmartInt[] mBuffAdded;
    bool[] mBuffDirty;
    bool[] mBuffSpecialEffect;
    List<Buff> mBuffList = new List<Buff>();

    public Unit Owner { get { return mOwner; } }
    public bool CanHurt { get { return !mBuffSpecialEffect[(int)EBuffSpecialEffect.CanNotHurt]; } }
    public bool CanMove { get { return mBuffSpecialEffect[(int)EBuffSpecialEffect.CanMove]; } }
    public bool CanRotate { get { return mBuffSpecialEffect[(int)EBuffSpecialEffect.CanRotate]; } }

    public BuffManager(Unit owner)
    {
        mOwner = owner;
        mBuffMultipy = new SmartInt[(int)EPA.MAX];
        mBuffAdded = new SmartInt[(int)EPA.MAX];
        mBuffDirty = new bool[(int)EPA.MAX];
        mBuffSpecialEffect = new bool[(int)EBuffSpecialEffect.Max];
    }

    public void OnDestory()
    {
        foreach (Buff buff in mBuffList)
            buff.OnDestory(true);
        mBuffList.Clear();
    }

    public void AddBuff(int id)
    {
        BuffBase buffBase = BuffBaseManager.Instance.GetItem(id);
        if (buffBase == null)
        {
            UnityEngine.Debug.LogError("Buff not found in [BuffBase] table: " + id);
            return;
        }

        // remove the previous buff.
        for (int i = 0; i < mBuffList.Count; i++)
        {
            Buff buff = mBuffList[i];
            if (buff.ID == buffBase.ID)
            {
                buff.OnDestory(true);
                mBuffList.RemoveAt(i);
                break;
            }
        }

        // create a new buff and attach to it.
        Buff newBuff = new Buff(this, buffBase);
        if (newBuff.OnStart())
            mBuffList.Add(newBuff);
    }

    public void Update(float deltaTime)
    {
        for (int i = 0; i < mBuffList.Count; i++)
        {
            Buff buff = mBuffList[i];
            if (!buff.Update(deltaTime))
            {
                buff.OnDestory(false);
                mBuffList.RemoveAt(i--);
            }
        }
    }

    public void Apply(EPA attrib, int multipy, int add)
    {
        switch (attrib)
        {
            case EPA.CurHP:
                mOwner.AddHp(mOwner.GetAttrib(EPA.HPMax) * multipy / 10000 + add);
                break;
            case EPA.CurSoul:
                mOwner.AddSoul(mOwner.GetAttrib(EPA.SoulMax) * multipy / 10000 + add);
                break;
            case EPA.CurAbility:
                mOwner.AddAbility(mOwner.GetAttrib(EPA.AbilityMax) * multipy / 10000 + add);
                break;
            default:
                mBuffMultipy[(int)attrib] += multipy;
                mBuffAdded[(int)attrib] += add;
                mBuffDirty[(int)attrib] = true;
                break;
        }
    }

    public int Apply(EPA attrib, int value)
    {
        int idx = (int)attrib;
        if (!mBuffDirty[idx])
            return value;
        return value + value * mBuffMultipy[(int)attrib] / 10000 + mBuffAdded[(int)attrib];
    }

    public void Apply(EBuffSpecialEffect effect, string param, bool end)
    {
        if (end)
            mBuffSpecialEffect[(int)effect] = false;
        else
            mBuffSpecialEffect[(int)effect] = true;
    }
}