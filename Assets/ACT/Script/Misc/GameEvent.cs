using UnityEngine;
using System.Collections.Generic;

public abstract class GameEvent
{
	public bool CanIgnore = false;
    public abstract void Execute();
}

public class HitSoundEvent : GameEvent
{
    int mWeaponType;
    int mMaterialType;
    Vector3 mPosition;

    public HitSoundEvent(int wt, int mt, Vector3 pos)
    {
		CanIgnore = true;
        mWeaponType = wt;
        mMaterialType = mt;
        mPosition = pos;
    }

    public override void Execute()
    {
        //HitSoundManager.Instance.PlaySound(mWeaponType, mMaterialType, mPosition);
    }
}

public class ActionSoundEvent : GameEvent
{
	int mSoundIndex;
	Vector3 mPosition;
	float mVolume;
	
	public ActionSoundEvent(int sound, Vector3 pos, float volume)
	{
		CanIgnore = true;
		mSoundIndex = sound;
		mPosition = pos;
		mVolume = volume;
	}
	
	public override void Execute()
	{
		SoundManager.Instance.Play3DSound(mSoundIndex, mPosition, mVolume);
	}
}

public class InstantiateResourcesEvent : GameEvent
{
    string mResource;
    Vector3 mPosition;
    
    protected GameObject mObject;
    public InstantiateResourcesEvent(string res, Vector3 pos)
    {
        mResource = res;
        mPosition = pos;
    }

    public override void Execute()
    {
        mObject = Instantiate(mResource, mPosition);
    }

    public virtual GameObject Instantiate(string src, Vector3 pos)
    {
        Object obj = Resources.Load(mResource);
        if (obj == null)
            return null;

        mObject = GameObject.Instantiate(obj, pos, Quaternion.identity) as GameObject;
        return mObject;
    }
}

public class SendMessageEvent : GameEvent
{
    GameObject mTarget;
    string mScriptName;
    string mParams;

    public SendMessageEvent(GameObject target, string script, string param)
    {
        mTarget = target;
        mScriptName = script;
        mParams = param;
    }

    public override void Execute()
    {
        if (!mTarget)
            return;
        mTarget.SendMessage(mScriptName, mParams);
    }
}

public class OnHitEffectEvent : GameEvent
{
    GameObject mTarget;

    public OnHitEffectEvent(GameObject target)
    {
		CanIgnore = true;
        mTarget = target;
    }

    public override void Execute()
    {
        OnHitEffect.Attach(mTarget);
    }
}