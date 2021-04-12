//HitDefinition



using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HitDefinition : MonoBehaviour
{
    Data.AttackDef mData;
    Unit mOwner;
    string mAction;
    Vector3 mInitPos = Vector3.zero;
    Vector3 mPos = Vector3.zero;
    float mOrientation = 0;
    float mDelayTime = 0.0f;
    float mLifeTime = 0.0f;
    bool mOutofDate = false;
    int mHitSucessCount = 0;
    int mLastHitCount = 0;
	int mHitSoundCount = 0;
	Transform mCacheTransform;
	GameObject mAttackFrame;
    bool mHitBlocked = false;
    SkillItem mSkillItem = null;
	
	bool mHaveHitOrt = false;
	Vector3 mHitOrientation = Vector3.zero;
	public static bool ShowAttackFrame = false;	
    public const int MAX_HIT_SOUND = 1;

    Vector3 mCubeHitDefSize = Vector3.zero;
    Vector2 mCylinderSize = Vector2.zero;
    Vector3 mRingSize = Vector3.zero;
    Vector4 mFanSize = Vector4.zero;

    Dictionary<GameObject, int> mHitedPassedMap = new Dictionary<GameObject, int>();

    public void Init(Data.AttackDef data, Unit owner, string action, SkillItem skillItem) 
    { 
        mData = data;
        mOwner = owner;
        mAction = action;
        mInitPos = owner.Position;
        mOrientation = owner.Orientation;
        mSkillItem = skillItem;

        // 每次攻击都会有怒气加成。
        if (skillItem == null && data.InitAbility > 0 && mOwner.UnitType == EUnitType.EUT_LocalPlayer)
        {
            LocalPlayer player = (LocalPlayer)mOwner;
            player.AddAbility(data.InitAbility * player.GetAttrib(EPA.AbHitAdd) / 50);
        }
    }

	// Use this for initialization
	void Start ()
    {
		mCacheTransform = transform;

        UpdatePosition(0);

        mDelayTime = mData.Delay * 0.001f;
		
        if (!string.IsNullOrEmpty(mData.SelfEffect))
        {			
            Object effectObj = Resources.Load(mData.SelfEffect);
            if (effectObj != null)
            {
                GameObject effect = GameObject.Instantiate(effectObj) as GameObject;
                if (effect)
                {
                    effect.transform.parent = gameObject.transform;
                    effect.transform.localPosition = new Vector3(
                        mData.SelfEffectOffset.X * 0.01f,
                        mData.SelfEffectOffset.Y * 0.01f,
                        mData.SelfEffectOffset.Z * 0.01f);
                    effect.transform.rotation = mOwner.UGameObject.transform.rotation;
                }
                else
                    Debug.LogError("Effect is not effect: " + mData.SelfEffect);
            }
            else
                Debug.LogError("Fail to create hit self effect:" + mData.SelfEffect);
        }
	}
	
	// Update is called once per frame
	void Update ()
    {
        float deltaTime = Time.deltaTime;
        if (mDelayTime > deltaTime)
        {
            mDelayTime -= deltaTime;
            return;
        }

		if (!mOwner.UUnitInfo)
            mOutofDate = true;
        else if (mLifeTime >= mData.Duration * 0.001f)
            mOutofDate = true;
		else if (mData.MaxHitCount > 0 && mHitSucessCount >= mData.MaxHitCount)
            mOutofDate = true;
		else if (mData.OwnerActionChange == 1 && mAction != mOwner.ActionStatus.ActiveAction.ID)//更换技能，并且技能不是同一个
			mOutofDate = true;

        if (mOutofDate || mHitBlocked || !mOwner.UUnitInfo)
        {
			Destroy(mAttackFrame);
            GameObject.Destroy(gameObject);
            return;
        }
		
		UpdatePosition(mLifeTime * 100.0f * 1000.0f / mData.Duration);//mLifeTime * 100.0f * 1000.0f/mData.Duration表示攻击的持续比例
        
		UpdateAttackFram(deltaTime);
		
		mHitSoundCount = 0;
        while (mLastHitCount < mData.HitCount)
        {
            float checkTime = mLastHitCount * mData.Duration * 0.001f / mData.HitCount;
            if (checkTime >= mLifeTime + deltaTime)
                break;

            CheckHit(mOwner);

            mLastHitCount++;
        }

        mLifeTime += deltaTime;
	}

    void UpdatePosition(float ratio)
    {
        if (mData.FllowReleaser != 0 || mData.KeepLocal != 0 || mData.FramType == Data.HitDefnitionFramType.SomatoType)
            mInitPos = mOwner.Position;//FllowReleaser是否跟随释放者。KeepLocal是否相对施放者位移，
	    //FramType技能击中框的类型:CuboidType为长方体;CylinderType为立方体; RingType:为圆环形; SomatoType:为受击体"

        Vector3 pos = Vector3.zero;
        Interplate(ratio, ref pos);

        RoatatePos(ref pos);
		
        mPos = mInitPos + pos * 0.01f;
		mCacheTransform.position = mPos;
    }

    void UpdateAttackFram(float deltaTime)
    {
		//if(!mHaveHitOrt)
			//Debug.Log("mHaveHitOrt is false!");
        switch (mData.FramType)
        {
            case Data.HitDefnitionFramType.CuboidType:
                {
                    Data.FrameCuboid data = (Data.FrameCuboid)mData.AttackType;
                    mCubeHitDefSize.x = data.Width * 0.01f;//攻击x范围
                    mCubeHitDefSize.y = data.Height * 0.01f;//攻击y范围
                    mCubeHitDefSize.z = data.Length * 0.01f;//攻击z范围						
			
			        if(ShowAttackFrame)
			        {
				        if(mAttackFrame != null)
					        Destroy(mAttackFrame);
				
				        mAttackFrame =(GameObject)Instantiate(Resources.Load("HitDefinitionCube"));				
				        mAttackFrame.transform.localScale = mCubeHitDefSize;					
				
				        mAttackFrame.transform.localPosition = new Vector3 (
					    mCacheTransform.position.x,
					    mCacheTransform.position.y + mCubeHitDefSize.y / 2.0f, 
					    mCacheTransform.position.z);	
						if(!mHaveHitOrt)
						{
							mHitOrientation = mOwner.UGameObject.transform.localEulerAngles;
							mHaveHitOrt = true;
						}
						if(mAttackFrame != null)
							mAttackFrame.transform.localEulerAngles = mHitOrientation;
	
					}
		        }
                break;			
            case Data.HitDefnitionFramType.CylinderType:
                {			       
                    Data.FrameCylinder data = (Data.FrameCylinder)mData.AttackType;
                    mCylinderSize.x = data.Radius * 0.01f;
                    mCylinderSize.y = data.Height * 0.01f;

			        if(ShowAttackFrame)
					{
				        if(mAttackFrame != null)
					        Destroy(mAttackFrame);
				        
				        mAttackFrame = (GameObject)Instantiate(Resources.Load("HitDefinitionCylinder"));
				        mAttackFrame.transform.localPosition = new Vector3 (mCacheTransform.position.x,
					    mCacheTransform.position.y + mCylinderSize.y / 2.0f, mCacheTransform.position.z);					        
				        mAttackFrame.transform.localScale = new Vector3(2*mCylinderSize.x, mCylinderSize.y/2.0f, 2*mCylinderSize.x);
						if(!mHaveHitOrt)
						{
							mHitOrientation = mOwner.UGameObject.transform.localEulerAngles;
							mHaveHitOrt = true;
						}
						if(mAttackFrame != null)
							mAttackFrame.transform.localEulerAngles = mHitOrientation;
					}
                }
                break;
            case Data.HitDefnitionFramType.RingType:
                {
                    Data.FrameRing data = (Data.FrameRing)mData.AttackType;
                    mRingSize.x = data.InnerRadius * 0.01f;
                    mRingSize.y = data.Height * 0.01f;
                    mRingSize.z = data.OuterRadius * 0.01f;
                }
                break;
            case Data.HitDefnitionFramType.SomatoType:
                {
                    mCubeHitDefSize = mOwner.ActionStatus.Bounding;
                }
                break;
            case Data.HitDefnitionFramType.FanType:
                {
                    Data.FrameFan data = (Data.FrameFan)mData.AttackType;
                    mFanSize.x = data.Radius * 0.01f;
                    mFanSize.y = data.Height * 0.01f;
                    mFanSize.z = data.StartAngle;
                    mFanSize.w = data.EndAngle;
                }
                break;
        }
	
    }
	
    void Interplate(float ratio, ref Vector3 pos)
    {
        if (mData.Path.Count == 0) //移动路径，mData.Path.Count 表示这个路径上的数目
            pos = Vector3.zero;
        else if (mData.Path.Count == 1) //移动路径数目为1
        {
            pos.x = mData.Path[0].X;
            pos.y = mData.Path[0].Y;
            pos.z = mData.Path[0].Z;
        }
        else//移动数目大于1的情况
        {
            for (int i = 1; i < mData.Path.Count; i++)
            {
                Data.AttackDef.PathNode preNode = mData.Path[i - 1];//路径上的前一个节点
                Data.AttackDef.PathNode curNode = mData.Path[i];//路径上的当前节点
                if (ratio < curNode.Ratio)//百分比
                {
                    float alpha = (ratio - preNode.Ratio) / (curNode.Ratio - preNode.Ratio);
                    pos.x = Mathf.Lerp(preNode.X, curNode.X, alpha);//按照alpha的比例在preNode和curNode之间插值
                    pos.y = Mathf.Lerp(preNode.Y, curNode.Y, alpha);//同上
                    pos.z = Mathf.Lerp(preNode.Z, curNode.Z, alpha);//同上
                    break;
                }

                if (curNode.Ratio == 0)//当前节点不占比例，直接用前面的内容来赋值
                {
                    pos.x = preNode.X;
                    pos.y = preNode.Y;
                    pos.z = preNode.Z;
                    break;
                }
            }
        }
    }

    void RoatatePos(ref Vector3 pos)
    {
        float x = pos.x, z = pos.z;
        MathUtility.Rotate(ref x, ref z, mOrientation);
        pos.x = x;
        pos.z = z;
    }

    void CheckHit(Unit self)
    {
        // monster only attack local player.
        if (self.UnitType == EUnitType.EUT_Monster &&
            self.Camp == EUnitCamp.EUC_ENEMY &&
            mData.Race == Data.RaceType.Enemy)
        {
            Unit target = UnitManager.Instance.LocalPlayer;
            if (target != null && target.UUnitInfo && CanHit(self, target))
                CheckHit(self, target);
            return;
        }
		int comboHit = 0;
        foreach (Unit target in UnitManager.Instance.UnitInfos)
        {
            if (!target.UUnitInfo || !CanHit(self, target))
                continue;

			if(CheckHit(self, target))
			{
				comboHit++;
			}
        }
		if(comboHit>0 && FightMainWnd.Exist)
		{
			FightMainWnd.Instance.OnComboHit(comboHit);
		}
    }

    bool CheckHit(Unit self, Unit target)
    {
	    // 转换offset到世界坐标系
        ActionStatus targetActionStatus = target.ActionStatus;
        Data.Action targetAction = targetActionStatus.ActiveAction;
	    float BoundOffsetX = targetAction.BoundingOffsetX;
	    float BoundOffsetY = targetAction.BoundingOffsetY;
	    float BoundOffsetZ = targetAction.BoundingOffsetZ;
		MathUtility.Rotate(ref BoundOffsetX, ref BoundOffsetZ, target.Orientation);

        Vector3 AttackeePos = target.Position + new Vector3(
            BoundOffsetX, BoundOffsetY, BoundOffsetZ) * 0.01f;

	    bool hitSuccess = false;
		
	    switch (mData.FramType)
        {
            case Data.HitDefnitionFramType.CuboidType:
            case Data.HitDefnitionFramType.SomatoType:
                // 四面体求交。
                if (MathUtility.RectangleHitDefineCollision(
                    mPos, mOrientation,
                    mCubeHitDefSize,
                    AttackeePos, target.Orientation,
                    targetActionStatus.Bounding))
                {
                    hitSuccess = true;
                }
                break;
            case Data.HitDefnitionFramType.CylinderType:
                // 圆柱求交
                if (MathUtility.CylinderHitDefineCollision(
                    mPos, mOrientation,
                    mCylinderSize.x, mCylinderSize.y,
                    AttackeePos, target.Orientation,
                    targetActionStatus.Bounding))
                {
                    hitSuccess = true;
                }
                break;
            case Data.HitDefnitionFramType.RingType:
                if (MathUtility.RingHitDefineCollision(
                    mPos, mOrientation,
                    mRingSize.x, mRingSize.y, mRingSize.z,
                    AttackeePos, target.Orientation,
                    targetActionStatus.Bounding))
                {
                    hitSuccess = true;
                }
                break;
            case Data.HitDefnitionFramType.FanType:
                if (MathUtility.FanDefineCollision(
                    mPos, mOrientation,
                    mFanSize.x, mFanSize.y, mFanSize.z, mFanSize.w,
                    AttackeePos, target.Orientation,
                    targetActionStatus.Bounding))
                {
                    hitSuccess = true;
                }
                break;
        }

        if (hitSuccess)
            return ProcessHit(target);

        return false;
    }

    bool CanHit(Unit self, Unit target)
    {
        if (mData.Race != Data.RaceType.Self && self == target)
            return false;

        if (mData.Race == Data.RaceType.Enemy && self.Camp == target.Camp)
            return false;

        if (mData.Race == Data.RaceType.TeamMember && self.Camp != target.Camp)
            return false;

        // 如果攻击高度不符合要求，停止击中判定
        if ((mData.HeightStatusHitMaskInt & (1 << target.ActionStatus.ActiveAction.HeightStatus)) == 0)
            return false;

        // 如果当前动作不接受受伤攻击，停止击中判定。
        if (!target.ActionStatus.CanHurt)
            return false;

        int hitCount = 0;
        if (mData.PassNum > 0 && mHitedPassedMap.TryGetValue(target.UGameObject, out hitCount) && hitCount >= mData.PassNum)
            return false;

        return true;
    }
	
    bool ProcessHit(Unit target)
    {
        // 设置穿越次数
        int hitCount = 0;
		if (mData.PassNum > 0)
		{
        	mHitedPassedMap.TryGetValue(target.UGameObject, out hitCount);
        	mHitedPassedMap[target.UGameObject] = ++hitCount;
		}

        // 累加击中次数。
        mHitSucessCount++;

        // 战士职业每次攻击都会有怒气加成。
        if (mSkillItem == null &&
			mHitSucessCount == 1 &&
            mData.HitAbility > 0 && 
            mOwner.UnitType == EUnitType.EUT_LocalPlayer)
        {
            LocalPlayer player = (LocalPlayer)mOwner;
            player.AddAbility(mData.HitAbility * player.GetAttrib(EPA.AbHitAdd) / 50);
        }

        // 召唤出来的单位，属性集中需要计算来之父亲的属性。
        Unit owner = mOwner;
        if (owner.Owner != null) owner = owner.Owner;

        // 攻击伤害的计算。
        int damageCoff = (mSkillItem != null) ? mSkillItem.DamageCoff : 100;
        int damageBase = (mSkillItem != null) ? mSkillItem.DamageBase : 0;

        // 击中目标的技能Buff
        if (mSkillItem != null && mSkillItem.SkillInput != null)
            mSkillItem.SkillInput.OnHitTarget(target);

        // 被格挡住了，执行格挡回弹动作。
        if (owner.Combat(target, damageCoff, damageBase, mSkillItem != null, mData.Damage) == ECombatResult.ECR_Block)
        {
            // do not process hit result in pvp mode.
            if (PvpClient.Instance != null && mOwner.UnitType != EUnitType.EUT_LocalPlayer)
                return false;

            if (mData.IsRemoteAttacks == 0 && mSkillItem == null)
            {
                mHitBlocked = true;
                owner.PlayAction(Data.CommonAction.Bounce);
                return false;
            }
        }
        
        // 击中目标。
        Hit(owner, target);

        // 设置攻击者的硬直时间和速度。
        mOwner.ActionStatus.SetStraightTime(mData.AttackerStraightTime, false);
        return true;
    }

    void Hit(Unit self, Unit target)
    {
        ActionStatus targetActionStatus = target.ActionStatus;

        // hit target.
        self.OnHitTarget(target);
		
		// sound.
		if (!string.IsNullOrEmpty(mData.HitedSound) && mHitSoundCount < MAX_HIT_SOUND)
		{
			if (mData.HitedSoundIndex == -2)
			{
				mData.HitedSoundIndex = SoundManager.Instance.GetSoundIndex(mData.HitedSound);
				if (mData.HitedSoundIndex < 0)
					Debug.LogError(string.Format("Fail to load hit sound: [{0}/{1}]", mOwner.UnitID, mData.HitedSound));
			}
			if (mData.HitedSoundIndex > 0)
				SoundManager.Instance.Play3DSound(mData.HitedSoundIndex, mOwner.Position, 1.0f);
			mHitSoundCount++;
		}

        // effect
        if (!string.IsNullOrEmpty(mData.HitedEffect))
        {
            Vector3 effectPos = target.Position;
            effectPos.y += targetActionStatus.Bounding.y * 0.5f;
            GameEventManager.Instance.EnQueue(
                new InstantiateResourcesEvent(mData.HitedEffect, effectPos), true);
        }

        // execute script
        if (!string.IsNullOrEmpty(mData.Script))
        {
            // "CameraShake(0.5, 10, 20);"
            string[] scripts = mData.Script.Split(';');
            foreach (string script in scripts)
            {
                string[] arr = script.Split('(');
                string message = arr[0];
                string param = arr[1].Substring(0, arr[1].IndexOf(')'));
                self.UGameObject.SendMessage(message, param);
            }
        }

        // do not process my hit result in pvp mode.
        if (PvpClient.Instance != null && self.UnitType == EUnitType.EUT_LocalPlayer)
            return;

        HitData hitData = new HitData();
        hitData.Target = target.ServerId;
		
        // 击中转向
        float targetRotate = target.Orientation;
		bool rotateOnHit = targetActionStatus.RotateOnHit;
        if (rotateOnHit)
        {
            if (mData.FramType == Data.HitDefnitionFramType.CylinderType)
            {
                float x = target.Position.x - mPos.x;
                float z = target.Position.z - mPos.z;
                float modify = Mathf.Atan2(x, z);
                targetRotate = modify + Mathf.PI;
            }
            else
                targetRotate = self.Orientation + Mathf.PI;
        }

        NetCommon.Encode(
            target.Position,
            targetRotate,
            ref hitData.HitX,
            ref hitData.HitY,
            ref hitData.HitZ,
            ref hitData.HitDir);

        // 单位在墙边上的时候，近战攻击者需要反弹。
        bool bounceBack = mData.IsRemoteAttacks == 0 && target.OnTouchWall;

        // 单位处于非霸体状态，需要被击中移动～
        bool processLash = true;

        // 攻击等级调整。
        int attackLevel = mData.AttackLevel;
		Data.HeightStatusFlag targetHeightStatus = targetActionStatus.HeightState;
        if (attackLevel < targetActionStatus.ActionLevel)
        {
            // 设置受击者的霸体硬直时间?
            hitData.HitAction = byte.MaxValue;

            // 单位处于霸体状态，不需要移动～
            processLash = false;

            // 攻击结果为霸体的情况系，非远程攻击的冲击速度转换为攻击者。受击者不受冲击速度影响
            bounceBack = mData.IsRemoteAttacks == 0;
        }
        else if (targetActionStatus.OnHit(mData.HitResult, mData.IsRemoteAttacks != 0))
        {
            hitData.HitAction = (byte)targetActionStatus.ActiveAction.ActionCache;
        }

        // 处理buff的东东
        if (targetActionStatus.SkillItem != null)
            targetActionStatus.SkillItem.SkillInput.OnHit(self);

        // 设置攻击者的冲击速度及冲击时间。
        int attackerLashTime = mData.AttackerTime;
        Vector3 attackerLash = attackerLashTime == 0 ? self.ActionStatus.Velocity : new Vector3(
            mData.AttackerLash.X * 0.01f,
            mData.AttackerLash.Y * 0.01f,
            mData.AttackerLash.Z * 0.01f);
        if (bounceBack)
        {
            attackerLash.x = mData.AttackeeLash.X * 0.01f;
            attackerLash.z = mData.AttackeeLash.Z * 0.01f;
            attackerLashTime = mData.AttackeeTime;
        }

        if (attackerLashTime > 0)
        {
            self.ActionStatus.SetLashVelocity(
                attackerLash.x,
                attackerLash.y,
                attackerLash.z,
                attackerLashTime);
        }

        // 处理受击者的冲击速度～
        LashProcess(mData, ref hitData, target, targetHeightStatus, processLash, rotateOnHit);

        // I was hited, tell the others.
        if (self.UnitType == EUnitType.EUT_OtherPlayer && target.UnitType == EUnitType.EUT_LocalPlayer)
        {
            if (target.ActionStatus.Listener != null)
                target.ActionStatus.Listener.OnHitData(hitData);
        }

        target.OnHit(hitData, false);
    }

    void LashProcess(Data.AttackDef attackDef, 
		ref HitData hitData, 
		Unit target, 
		Data.HeightStatusFlag targetHeightStatus, 
		bool processLash, 
		bool rotateOnHit)
	{
		int AttackeeStraightTime = attackDef.AttackeeStraightTime;
		float AttackeeLashX = attackDef.AttackeeLash.X;
        float AttackeeLashY = attackDef.AttackeeLash.Y;
        float AttackeeLashZ = attackDef.AttackeeLash.Z;
        int AttackeeTime = attackDef.AttackeeTime;

        Data.AttackDef.HitResultData hitResultData = null;
        switch (targetHeightStatus)
        {
            case Data.HeightStatusFlag.Ground:
                hitResultData = attackDef.GroundHit;
                break;
            case Data.HeightStatusFlag.LowAir:
                hitResultData = attackDef.LowAirHit;
                break;
            case Data.HeightStatusFlag.HighAir:
                hitResultData = attackDef.HighAirHit;
                break;
        }

        if (hitResultData != null && hitResultData.Enabled)
        {
            AttackeeLashX = hitResultData.AttackeeLash.X;
            AttackeeLashY = hitResultData.AttackeeLash.Y;
            AttackeeLashZ = hitResultData.AttackeeLash.Z;
            AttackeeTime = hitResultData.AttackeeTime;
            AttackeeStraightTime = hitResultData.AttackeeStraightTime;
        }

        if (processLash)
        {
            // 非受击转向的时候，冲击速度需要转换为本地坐标。
            if (!rotateOnHit)
            {
                Quaternion rotate = Quaternion.AngleAxis(mOrientation * Mathf.Rad2Deg + 180, Vector3.up);
                if (mData.FramType != Data.HitDefnitionFramType.CuboidType)
                {
                    Vector3 targetToOwner = mPos - target.Position;
                    targetToOwner.y = 0;
                    rotate = Quaternion.LookRotation(targetToOwner);
                }
                Vector3 lashVector = rotate * new Vector3(AttackeeLashX, AttackeeLashY, AttackeeLashZ);
                lashVector = target.UGameObject.transform.InverseTransformDirection(lashVector);

                AttackeeLashX = (short)lashVector.x;
                AttackeeLashZ = (short)lashVector.z;
            }

            hitData.LashX = (short)AttackeeLashX;
            hitData.LashY = (short)AttackeeLashY;
            hitData.LashZ = (short)AttackeeLashZ;
            hitData.LashTime = (short)AttackeeTime;
        }

        hitData.StraightTime = (short)AttackeeStraightTime;
	}
}


