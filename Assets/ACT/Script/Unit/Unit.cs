using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public abstract class Unit
{
    public const int MAX_VARIABLE_NUM = 20;

    UnitInfo mUnitInfo;

    bool mOnGround = false;
    bool mOnTouchWall = false;
    bool mOnHighest = false;
    bool mDead = false;

    Unit mHitTarget = null;
    Animation mCachedAnimation;
    AnimationState mCachedAnimationState;
    ActionStatus mActionStatus;
    CharacterController mController;
    Collider mCollider;
    Transform mTransform;
    CustomVariable[] mVariables = new CustomVariable[MAX_VARIABLE_NUM];
    Vector3 mPosition = Vector2.zero;
    float mOrientation = 0.0f;
    float mAnimSpeed = 1.0f;
    float mRadius = 0.5f;
    bool mCollisionEnabled = true;
    int mCacheLayerMask = 0;

    Vector3 mSyncOffset = Vector3.zero;
    float mSyncRotate = 0.0f;
    float mSyncTime = 0.0f;

    protected BuffManager mBuffManager;

    public GameObject MyTriggerCell { get { return mUnitInfo.triggerCell; } }
    public uint ServerId { get; set; }
    public int UnitID { get { return mUnitInfo.UnitID; } }
    public int Level { get { return mUnitInfo.Level; } set { mUnitInfo.Level = value; } }
    public float Radius { get { return mRadius; } }
    public GameObject UGameObject { get { return mUnitInfo.gameObject; } }
    public UnitInfo UUnitInfo { get { return mUnitInfo; } }
    public bool UnitEnable { get { return mUnitInfo.enabled; } }
    public EUnitType UnitType { get { return mUnitInfo.UnitType; } }
    public EUnitCamp Camp { get { return mUnitInfo.Camp; } }
    public ActionStatus ActionStatus { get { return mActionStatus; } }
    public bool OnGround { get { return mOnGround; } }
    public bool OnTouchWall { get { return mOnTouchWall; } }
    public bool OnHighest { get { return mOnHighest; } }
    public bool Dead { get { return mDead; } }
    public Unit HitTarget { get { return mHitTarget; } }
    public Vector3 Position { get { return mPosition; } }
    public float Orientation { get { return mOrientation; } }
    public UnitState State { get { return mUnitInfo.State; } set { mUnitInfo.State = value; } }
    public CustomVariable GetVariable(int idx) { return mVariables[idx]; }
    public GameLevel CurrentGameLevel { get; set; }
    public Unit Owner { get; set; }
    public BuffManager BuffManager { get { return mBuffManager; } }

    public abstract bool Hurt(Unit attacker, int damage, ECombatResult result);
    public virtual int GetMaxHp() { return 0; }
    public virtual int GetCurrentHp() { return 0; }
    public virtual void AddHp(int v) { }
    public virtual void AddSoul(int v) { }
    public virtual void AddAbility(int v) { }
    public virtual bool UpLevel() { return false; }
    public virtual int GetAttrib(EPA idx) { return 0; }
    public virtual void Equip() { }
    public virtual void Revive() { }

    public Unit(UnitInfo unitInfo, int level)
    {
        mUnitInfo = unitInfo;
        mTransform = unitInfo.transform;

        // controller.
        mController = unitInfo.GetComponent<CharacterController>();
        if (mController)
        {
            mController.enabled = false;
            mRadius = Mathf.Max(mRadius, mController.radius);
        }

        // setup radius.
        mCollider = unitInfo.GetComponent<Collider>();
        if (mCollider && mCollider is CapsuleCollider)
            mRadius = (mCollider as CapsuleCollider).radius;

        for (int i = 0; i < mVariables.Length; i++)
            mVariables[i] = new CustomVariable();

        // build the cache layer mask
        // for unit raycast params.
        for (int i = 0; i < 32; i++)
        {
            if (!Physics.GetIgnoreLayerCollision(unitInfo.gameObject.layer, i))
                mCacheLayerMask |= (1 << i);
        }
    }

    public virtual void OnDestroy()
    {
        mBuffManager.OnDestory();
        if (mUnitInfo.Pool != null)
            mUnitInfo.Pool.Unspawn(UGameObject);
        else
            GameObject.Destroy(UGameObject);
    }

    public void Destory()
    {
        UnitManager.Instance.Destroy(this);
    }

    protected void SetIsDead(bool dead)
    {
        mDead = dead;
        if (State != UnitState.Die && mDead)
            OnDead();
    }

    protected virtual void OnDead()
    {
        // disable the buffs.
        mBuffManager.OnDestory();

        // disable the controller.
        State = UnitState.Die;
    }

    public virtual void Init()
    {
        mActionStatus = new ActionStatus(this);
        mBuffManager = new BuffManager(this);
        if (mUnitInfo.Model)
            mCachedAnimation = mUnitInfo.Model.GetComponent<Animation>();

        mDead = false;
        mOnGround = false;
        mOnTouchWall = false;
        mOnHighest = false;
        mDead = false;
        mHitTarget = null;

        State = UnitState.Normal;
        mPosition = mTransform.position;
        mOrientation = mTransform.eulerAngles.y * Mathf.Deg2Rad;
        mActionStatus.ChangeActionGroup(mUnitInfo.ActionGroup);

        if (mUnitInfo.AIEnable)
            mActionStatus.Bind(new AIListener(this));

        if (mUnitInfo.initialization)
            UnitManager.Instance.Add(this);
    }

    public virtual void Start()
    {

    }

    public virtual void Update(float deltaTime)
    {
        mActionStatus.Update(deltaTime);
        mBuffManager.Update(deltaTime);

        if (mSyncTime > 0)
            UpdateSyncPosition(deltaTime);
    }

    void UpdateSyncPosition(float deltaTime)
    {
        float timePass = Mathf.Min(mSyncTime, deltaTime);
        float passLerp = timePass / mSyncTime;
        float leftLerp = 1.0f - passLerp;
        mSyncTime -= timePass;
        Vector3 offset = mSyncOffset * passLerp;

        // do linear interpret location.
        Move(offset);
		
        // lerp orientation.
        if (mSyncRotate != 0)
        {
            SetOrientation(mOrientation + mSyncRotate * passLerp);
            mSyncRotate *= leftLerp;
        }

        mSyncOffset *= leftLerp;
    }

    public void ClearSyncMove()
    {
        mSyncTime = 0; // clear the sync flag.
    }

    public void SetSyncMove(float x, float z, float time, bool faceDir)
    {
        // the unit should not be in hurt mode. [ActionStatus == 3]
        if (mActionStatus.ActionState ==  Data.EActionState.Hit)
            return;

        // force the unit to be run action.
        if (mActionStatus.ActiveAction.ID != Data.CommonAction.Run)
            PlayAction(Data.CommonAction.Run);

        mSyncOffset = new Vector3(x - mPosition.x, 0, z - mPosition.z);
        mSyncTime = time;

        // apply orientation immediately.
        if (faceDir)
        {
            float targetRotate = Mathf.Atan2(mSyncOffset.x, mSyncOffset.z);
            mSyncRotate = (targetRotate - mOrientation) % (Mathf.PI * 2);
            if (mSyncRotate > Mathf.PI)
                mSyncRotate -= Mathf.PI * 2;
            else if (mSyncRotate < -Mathf.PI)
                mSyncRotate += Mathf.PI * 2;
        }
        else
            mSyncRotate = 0;
    }

    public void SetPosition(Vector3 pos)
    {
        mPosition = pos;
        mTransform.position = mPosition;
    }

    public void SetOrientation(float orient)
    {
        mOrientation = orient;
        Vector3 eulerAngles = mTransform.eulerAngles;
        eulerAngles.y = mOrientation * Mathf.Rad2Deg;
        mTransform.eulerAngles = eulerAngles;
    }

    public void PlayAction(string action)
    {
        if (!string.IsNullOrEmpty(action))
            mActionStatus.ChangeAction(action, 0);
    }

    public void EnableCollision(bool enable)
    {
        mCollisionEnabled = enable;

        if (mController && mUnitInfo.UnitType == EUnitType.EUT_LocalPlayer)
            mController.enabled = enable;

        if (mCollider)
            mCollider.enabled = enable;
    }

    public void Move(Vector3 trans)
    {
        if (mUnitInfo.UnitType == EUnitType.EUT_LocalPlayer)
        {
            if (trans.y != 0) mOnGround = false;
            if (mController != null && mController.enabled)
            {
                CollisionFlags collisionFlags = mController.Move(trans);
                if ((collisionFlags & CollisionFlags.Below) == CollisionFlags.Below)
                    mOnGround = true;

                if ((collisionFlags & CollisionFlags.Sides) == CollisionFlags.Sides)
                    mOnTouchWall = true;
                else if (trans.x != 0 || trans.z != 0)
                    mOnTouchWall = false;
            }
            else
                mTransform.position += trans;

            if (FightMainWnd.Exist)
                FightMainWnd.Instance.PlayerMoveNotify(mTransform.position);

        }
        else
        {
            if (mCollisionEnabled)
            {
				float addtiveCheckLength = mRadius * 2;
                if (trans.x != 0 || trans.z != 0)
                {
                    // normalize direction.
                    Vector3 direction = new Vector3(trans.x, 0, trans.z);
                    float length = direction.magnitude;
                    direction /= length;

                    float backCheckOffset = 0.05f;
                    Vector3 checkPos = new Vector3(mPosition.x, mPosition.y + addtiveCheckLength, mPosition.z) - direction * backCheckOffset;
                    float checkLength = length + addtiveCheckLength + backCheckOffset;

                    RaycastHit hitInfo;
                    mOnTouchWall = false;
                    if (Physics.Raycast(checkPos, direction, out hitInfo, checkLength, mCacheLayerMask))
                    {
                        mOnTouchWall = true;
                        float hitDistance = hitInfo.distance - addtiveCheckLength;
                        if (hitDistance > 0)
                        {
                            trans.x = direction.x * hitDistance;
                            trans.z = direction.z * hitDistance;
                        }
                        else
                        {
                            trans.x = 0;
                            trans.z = 0;
                        }
                    }
                }

                mOnGround = false;
                if (trans.y < 0)
                {
                    RaycastHit hitInfo;
                    Vector3 checkPos = new Vector3(mPosition.x, mPosition.y + addtiveCheckLength, mPosition.z);
                    float checkLength = addtiveCheckLength - trans.y;
                    if (Physics.Raycast(checkPos, Vector3.down, out hitInfo, checkLength, mCacheLayerMask))
                    {
                        float hitDistance = hitInfo.distance - addtiveCheckLength;
                        if (Mathf.Abs(hitDistance) > 0.001f)
                            trans.y = -hitDistance;
                        else
                            trans.y = 0;
                        mOnGround = true;
                    }
                }
            }
            else
            {
                mOnGround = false;
            }

            if (trans == Vector3.zero)
                return;

            mTransform.position += trans;
        }

        mPosition = mTransform.position;
    }

    AnimationState FetchAnimation(Data.Action action, float speed)
    {
        if (action == null || action.AnimSlotList.Count == 0 || !mCachedAnimation)
            return null;

        Data.AnimSlot animSlot = action.AnimSlotList[UnityEngine.Random.Range(0, action.AnimSlotList.Count)];
        AnimationState animState = mCachedAnimation[animSlot.Animation];
        if (animState == null)
        {
            Debug.LogError(string.Format("Fail to change animation: {0}/{1}/{2}", UnitID, action.ID, animSlot.Animation));
            return null;
        }

        animState.normalizedTime = animSlot.Start * 0.01f;
        animState.speed = speed * (animSlot.End - animSlot.Start) * animState.length * 10.0f / (action.AnimTime);
        return animState;
    }

    public void PlayAnimation(Data.Action action, float speed)
    {
        AnimationState animState = FetchAnimation(action, speed);
        if (animState == null)
            return;

        mCachedAnimationState = animState;
        mAnimSpeed = animState.speed;

        float fadeLength = action.BlendTime * 0.001f;
        if (fadeLength == 0)
            mCachedAnimation.Play(animState.name);
        else
            mCachedAnimation.CrossFade(animState.name, fadeLength);
    }

    public void OnEnterPoseTime()
    {
        //if (mCachedAnimationState != null)
        //    mCachedAnimationState.speed = 0.001f;
    }

    public void BeginStaight()
    {
        if (mCachedAnimationState != null)
            mCachedAnimationState.speed = 0.001f;
    }

    public void EndStaight()
    {
        if (mCachedAnimationState != null)
            mCachedAnimationState.speed = mAnimSpeed;
    }

    public void OnReachHighest(bool value)
    {
        mOnHighest = value;
    }

    public void OnHitGround(bool value)
    {
        mOnGround = value;
    }

    public void OnHitTarget(Unit target)
    {
        mHitTarget = target;
    }

    public void ClearFlags()
    {
        mHitTarget = null;
    }

    // for debugging...
    public virtual void SetLevel(int lv)
    {
        Level = lv;
    }

    public virtual void UpdateAttributes()
    {
    }

    public virtual ECombatResult Combat(Unit target, int damageCoff, int damageBase, bool skillAttack, int actionCoff)
    {
        int damage;
        ECombatResult result = MathUtility.Combat(this, target, skillAttack, out damage);
        if (result != ECombatResult.ECR_Block)
        {
            // 技能的影响。
            damage = damage * damageCoff / 100 + damageBase;

            // 动作的系数调整在最后面。
            damage = damage * actionCoff / 50;

            // damage should not be 0.
            damage = Mathf.Max(damage, 1);

            target.Hurt(this, damage, result);

            if (MainScript.Instance != null && MainScript.Instance.LogCombat)
            {
                Debug.Log(string.Format("[{0}=>{1}], dmg={2} atk={3} hp={4} hpmax={5} defense={6}",
                    UnitID,
                    target.UnitID,
                    damage,
                    GetAttrib(EPA.Damage),
                    target.GetAttrib(EPA.CurHP),
                    target.GetAttrib(EPA.HPMax),
                    target.GetAttrib(EPA.Defense)));
            }
        }

        return result;
    }

    public void OnHit(HitData hitData, bool pvp)
    {
        // setup position.
        Vector3 position = Vector3.zero;
        float rotate = 0;
        NetCommon.Decode(hitData.HitX, hitData.HitY, hitData.HitZ, hitData.HitDir, ref position, ref rotate);

        // if the pos is too large than [xxx = 1.0f] ignore this hit data
        // to avoid flash moving...
        Vector3 offset = position - mPosition;
        if (pvp && offset.sqrMagnitude > 1.0f)
            return;

        Move(offset);
        SetOrientation(rotate);

        // avoid replay action.
        if (hitData.HitAction != byte.MaxValue && hitData.HitAction != mActionStatus.ActiveAction.ActionCache)
            mActionStatus.ChangeAction(hitData.HitAction, 0);

        // setup straight time.
        mActionStatus.SetStraightTime(hitData.StraightTime, true);

        // setup lash time.
        if (hitData.LashTime > 0)
        {
            mActionStatus.SetLashVelocity(
                hitData.LashX * 0.01f,
                hitData.LashY * 0.01f,
                hitData.LashZ * 0.01f,
                hitData.LashTime);
        }
    }

    public virtual void AddBuff(int id)
    {
        mBuffManager.AddBuff(id);
    }
}
