using System;
using System.Collections.Generic;
using UnityEngine;

public class Buff
{
    enum EBuffType
    {
        Once = 0,
        Keep = 1,
        Delta = 2,
    }

    const float DeltaTime = 0.25f; // the delta tick 250 ms.

    BuffManager mManager;
    BuffBase mBuffBase;
    float mLeftTime;
    float mCheckTime = DeltaTime;
    EBuffType mBuffType;
    EPA mAttrib = EPA.MAX;
    EBuffSpecialEffect mSpecialEffect = EBuffSpecialEffect.Max;
    GameObject mStartEffect;
    GameObject mKeepEffect;
    GameObject mEndEffect;

    public int ID { get { return mBuffBase.ID; } }

    public Buff(BuffManager manager, BuffBase buffBase)
    {
        mManager = manager;
        mBuffBase = buffBase;
        mLeftTime = buffBase.Time * 0.001f;
        mBuffType = (EBuffType)(int)mBuffBase.Type;
        if (!string.IsNullOrEmpty(buffBase.AttribEffect))
            mAttrib = (EPA)Enum.Parse(typeof(EPA), buffBase.AttribEffect);
        mSpecialEffect = (EBuffSpecialEffect)(int)mBuffBase.SpecialEffect;
    }

    public bool Update(float deltaTime)
    {
        if (mBuffType == EBuffType.Delta)
        {
            if (mCheckTime <= 0)
            {
                mCheckTime += DeltaTime;
                Apply(false);
            }
            mCheckTime -= deltaTime;
        }
        mLeftTime -= deltaTime;
        return mLeftTime > 0;
    }

    public bool OnStart()
    {
        // play the start effect.
        mStartEffect = PlayEffect(mBuffBase.Effect);

        // play the keep effect.
        mKeepEffect = PlayEffect(mBuffBase.KeepEffect);
        
        Apply(false);

        return (mBuffType != EBuffType.Once);
    }

    public void OnDestory(bool dead)
    {
        if (!dead)
        {
            // play the end effect.
            mEndEffect = PlayEffect(mBuffBase.EndEffect);
            if (mEndEffect)
                GameObject.Destroy(mEndEffect, 1.0f);
        }
        
        if (mStartEffect)
            GameObject.Destroy(mStartEffect);

        // stop & destroy the keep effect.
        if (mKeepEffect)
            GameObject.Destroy(mKeepEffect);

        if (mBuffType == EBuffType.Keep)
            Apply(true);
    }

    // apply buff effect.
    void Apply(bool end)
    {
        if (mAttrib != EPA.MAX)
        {
            int multipy = end ? -(int)mBuffBase.Multipy : (int)mBuffBase.Multipy;
            int added = end ? -(int)mBuffBase.Add : (int)mBuffBase.Add;
            mManager.Apply(mAttrib, multipy, added);

            if (MainScript.Instance != null && MainScript.Instance.LogBuff)
                Debug.Log(string.Format("Buff effect: id={0} attrib={1} value={2} multipy={3} add={4} time={5}",
                    ID,
                    mAttrib,
                    mManager.Owner.GetAttrib(mAttrib),
                    multipy,
                    added,
                    mBuffBase.Time));
        }

        if (mSpecialEffect != EBuffSpecialEffect.Max)
        {
            mManager.Apply(mSpecialEffect, mBuffBase.Parameter, end);
        }
    }

    GameObject PlayEffect(string effectName)
    {
        UnitInfo unitInfo = mManager.Owner.UUnitInfo;
        if (string.IsNullOrEmpty(effectName) || !unitInfo)
            return null;

        UnityEngine.Object prefab = Resources.Load(effectName);
        if (prefab == null)
        {
            Debug.LogError("Buff effect not found: " + effectName);
            return null;
        }

        Vector3 pos = mManager.Owner.Position;
        if (unitInfo.BuffAttachPoint)
            pos = unitInfo.BuffAttachPoint.position;
        else if (unitInfo.UnitTopUI &&
            unitInfo.UnitTopUI.TopUI)
            pos = unitInfo.UnitTopUI.TopPosition;

        GameObject effect = GameObject.Instantiate(prefab, pos, Quaternion.identity) as GameObject;
        if (unitInfo.BuffAttachPoint)
            effect.transform.parent = unitInfo.BuffAttachPoint;
        else
            effect.transform.parent = unitInfo.transform;

        return effect;
    }
}
