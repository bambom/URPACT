using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ActionStatus
{
    private Data.ActionGroup mActionGroup = null;
    private Data.Action mActiveAction = null;
    private Data.ActionInterrupt mQueuedInterrupt = null;

    public string StartupAction = Data.CommonAction.Idle;
    public float PushPower = 1.0f;

    bool mInitialized = false;
    bool mIgnoreMove = false;
    bool mIgnoreGravity = false;
    bool mCanMove = false;
    bool mCanRotate = false;
    bool mCanHurt = false;
    bool mFaceTarget = false;
    bool mOnStarightHit = false;
    int mActionTime = 0;
    int mActionKey = -1;
	int mEventIndex = 0;
    int mHitDefIndex = 0;
    int mActionInterruptEnabled = 0;
    Data.HeightStatusFlag mHeightState = Data.HeightStatusFlag.None;
    int mActionLevel = 0;
    int mLashTime = 0;
    int mStraightTime = 0;
    float mTotalTime = 0.0f;
    float mGravity = 0.0f;
	float mStraighExtent = 0.1f;
    float mActionScale = 1.0f;
    bool mRotateOnHit = false;
    Vector3 mMoveRelDistance = Vector3.zero;
    Vector3 mVelocity = Vector3.zero;
    Vector3 mBounding = Vector3.zero;
    Unit mOwner = null;
    IActionListener mListener = null;
    List<GameObject> mActionEffects = null;
    SkillInput mQueuedSkillInput = null;
	
	GameObject mListTargetFrame;
	public static bool ShowListTarFrame = false;
	GameObject mBeatenFramObj;
	public static bool ShowBeatenFrame = false;

    public Unit ActionTarget;
    public SkillItem SkillItem;
    public Data.ActionGroup ActionGroup { get { return mActionGroup; } }
    public Data.Action ActiveAction { get { return mActiveAction; } }
    public Data.HeightStatusFlag HeightState { get { return mHeightState; } }
    public Data.EActionState ActionState { get { return (Data.EActionState)mActiveAction.ActionStatus; } }
    public Vector3 Bounding { get { return mBounding; } }
    public Vector3 Velocity { get { return mVelocity; } }
    public bool HasQueuedAction { get { return mQueuedInterrupt != null; } }
    public bool RotateOnHit { get { return mRotateOnHit; } }
    public bool CanMove { get { return mCanMove; } }
    public bool CanRotate { get { return mCanRotate; } }
    public bool CanHurt { get { return mOwner.BuffManager.CanHurt && mCanHurt; } }
    public bool FaceTarget { get { return mFaceTarget; } }
    public int ActionLevel { get { return mActionLevel; } }
    public IActionListener Listener { get { return mListener; } }
    public void Bind(IActionListener listener) { mListener = listener; }

    public ActionStatus(Unit owner)
    {
        mOwner = owner;
    }

    /// <summary>
    /// the main update entry.
    /// </summary>
    public void Update(float deltaTime)
    {
        if (!mInitialized)
            mInitialized = ChangeActionGroup(mOwner.UUnitInfo.ActionGroup);

        // modify the action scale.
        if (mActionScale != 1.0f)
            deltaTime *= mActionScale;

        int preTime = (int)(mTotalTime * 1000.0f);
        mTotalTime += deltaTime;
        int curTime = (int)(mTotalTime * 1000.0f);

        if (mActiveAction != null || mStraightTime > 0)
            TickAction(curTime - preTime);
		
		if (mListener != null)
			mListener.Update(deltaTime);
		
		UpdateBeatenFram();
    }
	
	void UpdateBeatenFram()
	{
		if (!ShowBeatenFrame)
		{
			if(mBeatenFramObj != null)
				GameObject.Destroy(mBeatenFramObj);
			return;
		}
	    float BoundOffsetX = mActiveAction.BoundingOffsetX;
	    float BoundOffsetY = mActiveAction.BoundingOffsetY;
	    float BoundOffsetZ = mActiveAction.BoundingOffsetZ;
		//Debug.Log("BoundOffsetX:" +BoundOffsetX+"BoundOffsetY:"+BoundOffsetY+"BoundOffsetZ"+BoundOffsetZ);
		MathUtility.Rotate(ref BoundOffsetX, ref BoundOffsetZ, mOwner.Orientation);

        Vector3 AttackeePos = mOwner.Position + new Vector3(
            BoundOffsetX, BoundOffsetY, BoundOffsetZ) * 0.01f;
		AttackeePos.y += Bounding.y / 2.0f;
		
		if(mBeatenFramObj != null)
			GameObject.Destroy(mBeatenFramObj);
		mBeatenFramObj = GameObject.Instantiate(Resources.Load("BeatenFrameCube")) as GameObject;
		mBeatenFramObj.transform.position = AttackeePos;
		mBeatenFramObj.transform.localScale = Bounding;
		mBeatenFramObj.transform.localEulerAngles = mOwner.UGameObject.transform.localEulerAngles;	
		GameObject.Destroy(mBeatenFramObj,1.0f);
	}
    void Reset()
    {
        mEventIndex = 0;
        mHitDefIndex = 0;
        mActionTime = 0;
        mActionKey = -1;
        mActionScale = 1.0f;
        mActionInterruptEnabled = 0;
        mQueuedInterrupt = null;
        mIgnoreMove = false;
        mIgnoreGravity = mActiveAction.mIgnoreGravity;
        mCanHurt = mActiveAction.mCanHurt;
        mGravity = mActiveAction != null ? mActionGroup.mGravtity * 0.01f : 9.8f;
        mMoveRelDistance = Vector3.zero;
        mRotateOnHit = mActionGroup.mRotateOnHit;
        mFaceTarget = mActiveAction != null ? mActiveAction.FaceTarget : false;
		
		// tell the unit clear flags while action changed.
		mOwner.ClearFlags();

	    if (mActiveAction != null)
	    {
            float sizeModifiy = 0.01f * 0.01f;
            mBounding.x = mActionGroup.mBoundingWidth * mActiveAction.mBoundingWidthRadio * sizeModifiy;
            mBounding.y = mActionGroup.mBoundingHeight * mActiveAction.mBoundingHeightRadio * sizeModifiy;
            mBounding.z = mActionGroup.mBoundingLength * mActiveAction.mBoundingLengthRadio * sizeModifiy;
			
            mOwner.EnableCollision(mActiveAction.mHasCollision != 2);

            if (mActiveAction.mRotateOnHit != 0)
                mRotateOnHit = (mActiveAction.mRotateOnHit == 1);

            if (mActiveAction.mActionLevel != 0)
                mActionLevel = mActiveAction.mActionLevel;
            else
                mActionLevel = mActionGroup.DefaultActionLevel;

            mHeightState = (Data.HeightStatusFlag)(1 << mActiveAction.mHeightStatus);
            mCanMove = mActiveAction.mCanMove;
            mCanRotate = mActiveAction.mCanRotate;

		    // copy the action request enabled/disabled flags.
		    for (int i = 0; i<mActiveAction.mActionInterrupts.Count; i++)
		    {
                Data.ActionInterrupt actionInterrupt = mActiveAction.mActionInterrupts[i];
			    if (actionInterrupt.Enabled)
				    mActionInterruptEnabled |= (1 << i);
            }

            // clear the skill info when we get a normal action.
            if (mActiveAction.ID[0] == 'N' && SkillItem != null)
                SkillItem = null;
        }

        // clear the effect walk with action.
        if (mActionEffects != null && mActionEffects.Count > 0)
        {
            foreach (GameObject effect in mActionEffects)
                GameObject.Destroy(effect);
            mActionEffects.Clear();
        }
    }
	
    public bool ChangeActionGroup(int groupIndex)
    {
        if (ActionManager.Instance == null)
        {
            Debug.LogError("ChangeActionGroup failed");
            return false;
        }

        Data.UnitActionInfo unitInfo = ActionManager.Instance.GetUnitActionInfo(mOwner.UnitID);
        foreach (Data.UnitActionInfo.UnitVarible v in unitInfo.UnitVaribleList)
            mOwner.GetVariable(v.Index).Set(v.Value, v.Max);

        mActionGroup = unitInfo.ActionGroups[groupIndex];
        ChangeAction(StartupAction, 0);

		mInitialized = true;
        return true;
    }

    /// <summary>
    /// tick this action.
    /// </summary>
    /// <param name="deltaTime"></param>
    void TickAction(int deltaTime)
    {
        if (mActiveAction == null)
            return;
		
        //mActionChanged = false;
        // Œì²âŽŠÓÚÓ²Ö±×ŽÌ¬¡£
        if (ProcessStraighting(ref deltaTime))
            return;

        if (ProcessQueuedAction(deltaTime))
            return;

        // check we are going to finished, tick current action to the end.
        int nextActionTime = 0;
        bool thisActionIsFinished = false;
        if ((mActionTime + deltaTime) > mActiveAction.TotalTime)
        {
            // get the new action tick time.
            nextActionTime = deltaTime;

            deltaTime = mActiveAction.TotalTime - mActionTime;
            nextActionTime -= deltaTime;

            thisActionIsFinished = true;
        }

        // next action key.
        int nextActionKey = GetNextKey(deltaTime);
		
        // tick the action need check the keys.
        if (nextActionKey > mActionKey)
        {
            // check the Events
            ProcessEventList(nextActionKey, deltaTime);

            // check the HitDefines
            ProcessHitDefineList(nextActionKey);

            // check the interrupt list.
            if (ProcessActionInterruptList(mActionKey, nextActionKey))
                return;
			
			if (mActiveAction.PoseTime > 0 && mActionKey < 100 && nextActionKey >= 100)
				mOwner.OnEnterPoseTime();
			
			// hack the event interrupts.
			mOwner.OnReachHighest(false);
        }

        // do relative & absolute moving.
        ProcessMoving(deltaTime);

        // tick time to this action.
        mActionTime += deltaTime;
        mActionKey = nextActionKey;

        // ³å»÷ŽŠÀí¡£
       ProcessLash(deltaTime);

        // this action is done!!
        if (thisActionIsFinished)
            ProcessTickFinish(nextActionTime);
		
    }

    bool ProcessQueuedAction(int deltaTime)
    {
        if (mQueuedInterrupt == null)
            return false;

        int actualQueuedTime = 0;
        if (!CheckTime(deltaTime, mQueuedInterrupt.ConnectTime, ref actualQueuedTime))
            return false;
		
        // get the new action tick time.
        int nextActionTime = mActionTime + deltaTime - actualQueuedTime;

        // change to the queued actions.
        if (mQueuedSkillInput != null)
        {
            mQueuedSkillInput.PlaySkill();
            mQueuedSkillInput = null;
        }
        else
            ChangeAction(mQueuedInterrupt.ActionCache, nextActionTime);

        // trun off queued actions.
        mQueuedInterrupt = null;

        return true;
    }

    bool CheckTime(int deltaTime, int checkRatio, ref int checkTime)
    {
        if (mActiveAction == null)
            return false;

        // check the queued time.
        checkTime = (checkRatio <= 100) ?
            mActiveAction.AnimTime * checkRatio / 100 :	// [0-100] AnimTime
            mActiveAction.AnimTime + mActiveAction.PoseTime * (checkRatio - 100) / 100; // [100-200] PoseTime

        // match the trigger time options.
        return (mActionTime == 0 && checkTime == 0) || (mActionTime < checkTime && mActionTime + deltaTime >= checkTime);
    }
    
    //---------------------------------------------------------------------
    public int GetCheckTime(int checkRatio)
    {
	    if (mActiveAction == null) return -1;

	    // check the queued time.
	    return (checkRatio <= 100) ? 
		    mActiveAction.AnimTime * checkRatio / 100 :	// [0-100] AnimTime
		    mActiveAction.AnimTime + mActiveAction.PoseTime * (checkRatio - 100) / 100; // [100-200] PoseTime
    }

    bool ProcessMoving(int deltaTime)
    {
        // do relative moving.
        if (mIgnoreMove)
        {
            mIgnoreMove = false;
            return true;
        }

        float dt = deltaTime * 0.001f;
        ProcessActionMove(mVelocity, dt);

        float x = mMoveRelDistance.x, z = mMoveRelDistance.z;
        if (x != 0 || z != 0)
            MathUtility.Rotate(ref x, ref z, mOwner.Orientation);

        mOwner.Move(new Vector3(x, mMoveRelDistance.y, z));

        if (!mIgnoreGravity)
        {
            float velocityModify = -mGravity * dt;
			if (mVelocity.y > 0 && mVelocity.y <= -velocityModify)
				mOwner.OnReachHighest(true);
            mVelocity.y += velocityModify;
        }

        mMoveRelDistance = Vector3.zero;
        return true;
    }

    void ProcessActionMove(Vector3 velocity, float dt)
    {
        mMoveRelDistance.x += velocity.x * dt;
        mMoveRelDistance.z += velocity.z * dt;

        // we need handle gravity effects here.
        if (mIgnoreGravity)
            mMoveRelDistance.y += velocity.y * dt;
        else
            mMoveRelDistance.y += velocity.y * dt - mGravity * dt * dt * 0.5f;
    }

    void ProcessTickFinish(int nextActionTime)
    {
        int nextAction = mActiveAction.mNextActionCache;
        if (mOwner.Dead && mActionGroup.CheckDeath)
        {
            if (mHeightState == Data.HeightStatusFlag.Stand)
                nextAction = mActionGroup.GetActionIdx(mActionGroup.StandDeath);
            else if (mHeightState == Data.HeightStatusFlag.Ground)
                nextAction = mActionGroup.GetActionIdx(mActionGroup.DownDeath);

            if (nextAction == mActiveAction.mActionCache)
                nextAction = mActiveAction.mNextActionCache;
        }

        ChangeAction(nextAction, nextActionTime);
    }

    public void ChangeAction(string id, int deltaTime)
    {
        if (mActionGroup == null)
            return;

        int idx = mActionGroup.GetActionIdx(id);
        if (idx < 0)
        {
            Debug.LogError("Fail change action to: " + id);
            return;
        }

        ChangeAction(idx, deltaTime);
    }

    public void ChangeAction(int actionIdx, int deltaTime)
    {
        Data.Action oldAction = mActiveAction;
        Data.Action action = mActionGroup.GetAction(actionIdx);

        // velocity.
        if (action.ResetVelocity)
            mVelocity = Vector3.zero;

        mActiveAction = action;
		
        Reset();

        // tick action now.
        if (deltaTime > 0)
            TickAction(deltaTime);

        if (mListener != null)
            mListener.OnActionChanging(oldAction, action);

        // change the moving speed.
        if (mOwner.UnitType == EUnitType.EUT_LocalPlayer || mOwner.UnitType == EUnitType.EUT_OtherPlayer)
        {
            if (action.ID == Data.CommonAction.Run)
            {
                Player player = (Player)mOwner;
                int baseSpeed = (action.MoveSpeed > 0) ? action.MoveSpeed : 300;
                mActionScale = (float)player.GetAttrib(EPA.MoveSpeed) / baseSpeed;
            }
        }

        // check is should we skip animation for optimazation...
        mOwner.PlayAnimation(mActiveAction, mActionScale);
    }
    
    //---------------------------------------------------------------------
    int GetNextKey(int deltaTime)
    {
	    if (mActiveAction == null) return -1;

	    int currentTime = mActionTime + deltaTime;

	    // [0-100]
	    if (currentTime <= mActiveAction.AnimTime)
		    return currentTime * 100 / mActiveAction.AnimTime;

	    // [200-...]
	    if (currentTime >= mActiveAction.TotalTime)
		    return 200;

	    // [101-199]
	    int leftTime = currentTime - mActiveAction.AnimTime;
	    return 100 + leftTime * 100 / mActiveAction.PoseTime;
    }
    
    //---------------------------------------------------------------------
    bool ProcessEventList(int nextKey, int deltaTime)
    {
        if (mActiveAction.mEvents.Count == 0 || mEventIndex >= mActiveAction.mEvents.Count)
		    return false;
    	
	    bool ret = false;
        while (mEventIndex < mActiveAction.mEvents.Count)
	    {
            Data.Event actionEvent = mActiveAction.mEvents[mEventIndex];
		    if (actionEvent.mTriggerTime > nextKey)
			    break;

		    // trigger this event.
            if (OnTriggerEvent(actionEvent, deltaTime))
		    {
			    TriggerEvent(actionEvent, deltaTime);
			    ret = true;
		    }
		    mEventIndex++;
	    }
	    return ret;
    }

    bool OnTriggerEvent(Data.Event actionEvent, int deltaTime)
    {
        return true;
    }

    void SwitchStatus(string name, bool on)
    {
        switch (name)
        {
            case "IgnoreGravity":
                mIgnoreGravity = on;
                break;
            case "CanMove":
                mCanMove = on;
                break;
            case "CanRotate":
                mCanRotate = on;
                break;
            case "CanHurt":
                mCanHurt = on;
                break;
            case "FaceTarget":
                mFaceTarget = on;
                break;
        }
    }

    void SetVelocity(Data.Event actionEvent, float x, float y, float z, int deltaTime)
    {
        int triggerTime = GetCheckTime(actionEvent.TriggerTime);

        ProcessMoving(triggerTime - mActionTime);

        mVelocity.x = x;
        mVelocity.y = y;
        mVelocity.z = z;

        ProcessMoving(deltaTime + mActionTime - triggerTime);

        // ignore move because we already moved.
        mIgnoreMove = true;
    }

    void SetDirection(Data.Event actionEvent, int angle, bool local)
    {
        float rad = Mathf.Deg2Rad * angle;
        mOwner.SetOrientation(local ? mOwner.Orientation + rad : rad);
    }

    void PlaySound(Data.Event actionEvent, int soundIndex, bool checkMaterial)
    {
        float volume = checkMaterial ? 0.5f : 1.0f;
        SoundManager.Instance.Play3DSound(soundIndex, mOwner.Position, volume);
    }

    void PlayEffect(Data.EventPlayEffect data)
    {
        //[Description("0=所有玩家阵营可见（默认） 1=自己与友方阵营可见 2=敌方阵营可见 3=仅自己可见")]//
        switch (data.VisibleType)
        {
            case 1:
                if (mOwner.Camp != EUnitCamp.EUC_FRIEND)
                    return;
                break;
            case 2:
                if (mOwner.Camp != EUnitCamp.EUC_ENEMY)
                    return;
                break;
            case 3:
                if (mOwner.UnitType != EUnitType.EUT_LocalPlayer)
                    return;
                break;
        }

        Object effectObj = Resources.Load(data.EffectName);
        if (effectObj == null)
        {
            Debug.LogError(string.Format("Effect not found at PlayEffect: [{0}][{1}][{2}]", 
                mOwner.UnitID, 
                mActiveAction.ID, 
                data.EffectName));
            return;
        }
		
        Vector3 position = mOwner.UGameObject.transform.position;
        Quaternion rotation = mOwner.UGameObject.transform.rotation;
        Vector3 offset = new Vector3(data.OffsetX * 0.01f, data.OffsetY * 0.01f, data.OffsetZ * 0.01f);
        position += rotation * offset;

        GameObject effect = GameObject.Instantiate(effectObj, position, rotation) as GameObject;
        if (effect == null)
        {
            Debug.LogError(string.Format("Effect error at PlayEffect: [{0}][{1}][{2}]",
                mOwner.UnitID,
                mActiveAction.ID,
                data.EffectName));
            return;
        }

        if (data.BindMode == 0)
            effect.transform.parent = mOwner.UGameObject.transform;
		effect.transform.localScale = new Vector3(1,1,1);

        // stop while action changed.
        if (data.StopMode == 1)
        {
            if (mActionEffects == null)
                mActionEffects = new List<GameObject>();
            mActionEffects.Add(effect);
        }
    }

    const float FTOffset = 0.2f;
    void ListTargets(Data.EventListTargets data)
    {
        ActionTarget = null;

        Vector3 forward = mOwner.UGameObject.transform.forward;
        Vector3 right = mOwner.UGameObject.transform.right;
        Vector3 basePos = mOwner.Position + forward * FTOffset;
        float fanDistance = data.FanRadius;
        float fanSqrSqrDistance = fanDistance * fanDistance;
        float fanAngle = data.FanAngle;
        float fanRadin = Mathf.Cos(fanAngle * 0.5f);
        float minCheck = 0;
		
		if(ShowListTarFrame)
		{
			switch(data.ListType)
			{			
				case Data.ListTargetFrameType.Cuboid_ListType:
				    if(mListTargetFrame != null)
						GameObject.Destroy(mListTargetFrame);
				    GameObject cubeListTarObj = GameObject.Instantiate(Resources.Load("ListTargetCube")) as GameObject;
				    UpdateMaterialCol(cubeListTarObj,data.ListMode);
				    cubeListTarObj.transform.parent = mOwner.UGameObject.transform;		
					//cube型的listtarget位置
				    Vector3 cubeListTarPos = Vector3.zero;
				    cubeListTarPos.x = (data.Right + data.Left) * 0.01f / 2.0f; //x的坐标
				    cubeListTarPos.y = (data.Top + data.Bottom) * 0.01f / 2.0f; //y的坐标
				    cubeListTarPos.z = (data.Front + data.Back) * 0.01f / 2.0f; //z的坐标
				    cubeListTarObj.transform.localPosition = cubeListTarPos;			    
				    //listTarget的角度
				    cubeListTarObj.transform.localEulerAngles = Vector3.zero;
				    //重置父节点
				    cubeListTarObj.transform.parent = mOwner.UGameObject.transform.parent;			   
					//listTarget的大小
				    Vector3 cubeListTarScale = Vector3.one;
				    cubeListTarScale.x = (data.Right - data.Left) * 0.01f; //x方向上的大小
				    cubeListTarScale.y = (data.Top - data.Bottom) * 0.01f; //y方向上的大小
				    cubeListTarScale.z = (data.Front - data.Back) * 0.01f; //z方向上的大小
				    cubeListTarObj.transform.localScale = cubeListTarScale;
				    mListTargetFrame = cubeListTarObj;
					break;
				
				case Data.ListTargetFrameType.Fan_ListType:
				    if(mListTargetFrame != null)
						GameObject.Destroy(mListTargetFrame);
				    GameObject fanListTarObj = GameObject.Instantiate(Resources.Load("ListTargetCylinder")) as GameObject;
				    UpdateMaterialCol(fanListTarObj,data.ListMode);
				    fanListTarObj.transform.parent = mOwner.UGameObject.transform;
				    //fan型的listTarget的位置
				    Vector3 fanListTarpos = Vector3.zero;
				    fanListTarpos.y = (data.Top + data.Bottom) * 0.01f / 4.0f; //y轴坐标
				    fanListTarObj.transform.localPosition = fanListTarpos;
				    //fan型的listTarget的角度
				    fanListTarObj.transform.localEulerAngles = Vector3.zero;
				    //fan型的listTarget的大小
				    Vector3 fanListTarScale = Vector3.one;
				    fanListTarScale.x = fanListTarScale.z = data.FanRadius * 0.01f *2.0f;
				    fanListTarScale.y = (data.Top - data.Bottom) * 0.01f / 2.0f; //y方向上的大小
				    fanListTarObj.transform.localScale = fanListTarScale;
				    //画出表示角度的两条线
				    DrawAngleLine(fanListTarObj,data.FanAngle,data.FanRadius);
					//重置父节点
					fanListTarObj.transform.parent = mOwner.UGameObject.transform.parent;
				    mListTargetFrame = fanListTarObj;
					break;
				default:
					break;
			}
			if(mListTargetFrame != null)
				GameObject.Destroy(mListTargetFrame,2.0f);
		}
		
        foreach (Unit target in UnitManager.Instance.UnitInfos)
        {
            if (target == mOwner || 
                target.Dead || 
                target.Camp == EUnitCamp.EUC_FRIEND ||
                target.Camp == mOwner.Camp)
                continue;

            Vector3 trans = target.Position - basePos;
            trans.y = 0;

            float sqrMagnitude = trans.sqrMagnitude;
            if (data.ListType == Data.ListTargetFrameType.Fan_ListType)
            {
                if (sqrMagnitude > fanSqrSqrDistance ||
                    Vector3.Dot(trans, forward) < fanRadin)
                    continue;
            }
            else
            {
                float length = Vector3.Dot(trans, forward);
                if (length < data.Back * 0.01f || length > data.Front * 0.01f)
                    continue;

                Vector3 side = trans - length * forward;
                float width = Vector3.Dot(side, right);
                if (width < data.Left * 0.01f || width > data.Right * 0.01f)
                    continue;
            }

            float thisCheck = sqrMagnitude;
            if (data.ListMode == Data.ListTargetMode.MinAngle)
                thisCheck = Mathf.Abs(Mathf.Atan2(trans.x, trans.z) - mOwner.Orientation);
            else if (data.ListMode == Data.ListTargetMode.Random)
                thisCheck = Random.Range(1, 100);

            if (ActionTarget == null || thisCheck < minCheck)
            {
                ActionTarget = target;
                minCheck = thisCheck;
            }
        }
    }
	
	void DrawAngleLine(GameObject go,int angle,int radius)
	{
	    Vector3 topCenterPos = go.transform.localPosition;
		topCenterPos.y += go.transform.localScale.y;
		LineRenderer line = go.AddComponent<LineRenderer>();
		line.SetWidth(0.02f,0.02f);
		line.SetVertexCount(3);
		Vector3 leftPos = topCenterPos;
		float angleLeftOffset = go.transform.parent.localEulerAngles.y + angle /2.0f;	
		float angelRightOffset= go.transform.parent.localEulerAngles.y - angle /2.0f;
		
		leftPos.x += Mathf.Sin(Mathf.Deg2Rad * angleLeftOffset) * radius * 0.01f;		
	    leftPos.z += Mathf.Cos(Mathf.Deg2Rad * angleLeftOffset) * radius * 0.01f;
		Vector3 rightPos = topCenterPos;
		rightPos.x +=Mathf.Sin(Mathf.Deg2Rad * angelRightOffset)* radius * 0.01f;
		rightPos.z +=Mathf.Cos(Mathf.Deg2Rad * angelRightOffset)* radius * 0.01f;
		
		Vector3 parentPos = go.transform.parent.localPosition;
		line.SetPosition(0,leftPos + parentPos);
		line.SetPosition(1,topCenterPos + parentPos);
		line.SetPosition(2,rightPos + parentPos);
	}
	
	void UpdateMaterialCol(GameObject go,Data.ListTargetMode mode)
	{
		switch(mode)
		{
		case Data.ListTargetMode.MinAngle:
			go.transform.GetComponent<Renderer>().material.SetColor("_TintColor",Color.yellow);
			Debug.Log("The Color of MinAngle is red");
		    break;
		case Data.ListTargetMode.MinDistance:
			go.transform.GetComponent<Renderer>().material.SetColor("_TintColor",Color.blue);
			Debug.Log("The Color of MinDistance is blue");
			break;
		case Data.ListTargetMode.Random:
			go.transform.GetComponent<Renderer>().material.SetColor("_TintColor",Color.green);
			Debug.Log("The Color of Random is green");
			break;
		default:
			break;
		}		
	}
    void FaceTargets(Data.EventFaceTargets data)
    {
        if (ActionTarget == null || !ActionTarget.UUnitInfo || ActionTarget.Dead)
            return;

        float x = ActionTarget.Position.x - mOwner.Position.x;
        float z = ActionTarget.Position.z - mOwner.Position.z;
        float dir = Mathf.Atan2(x, z);
        mOwner.SetOrientation(dir);

        if (mListener != null)
            mListener.OnFaceTarget();
    }

    void GoToTargets(Data.EventGoToTargets data)
    {
        if (ActionTarget == null || !ActionTarget.UUnitInfo || ActionTarget.Dead)
            return;

        int offsetX = data.Random ? Random.Range(0, data.OffsetX) : data.OffsetX;
        int offsetY = data.Random ? Random.Range(0, data.OffsetY) : data.OffsetY;
        int offsetZ = data.Random ? Random.Range(0, data.OffsetZ) : data.OffsetZ;
        Vector3 offset = new Vector3(offsetX * 0.01f, offsetY * 0.01f, offsetZ * 0.01f);
        if (offset != Vector3.zero)
        {
            if (data.Local)
                offset = ActionTarget.UUnitInfo.transform.rotation * offset;
            offset += ActionTarget.Radius * offset.normalized;
        }
        else
        {
            offset = ActionTarget.Radius * (mOwner.Position - ActionTarget.Position).normalized;
        }

        Vector3 targetPos = ActionTarget.Position + offset;
        mOwner.Move(targetPos - mOwner.Position);
    }

    bool SenseTarget(Data.ActionInterrupt interrupt)
    {
        if (ActionTarget == null || !ActionTarget.UUnitInfo || ActionTarget.Dead)
            return false;

        float distance = MathUtility.DistanceSqr(mOwner.Position, ActionTarget.Position) * 10000.0f;
        return distance <= interrupt.mTargetDistanceMax * interrupt.mTargetDistanceMax &&
            distance >= interrupt.mTargetDistanceMin * interrupt.mTargetDistanceMin;
    }

    void AddUnit(Data.EventAddUnit data)
    {
        UnitBase unitBase = UnitBaseManager.Instance.GetItem(data.Id);
        UnityEngine.Object prefab = unitBase != null ? Resources.Load(unitBase.Prefab) : null;
        if (prefab == null)
        {
            Debug.Log(string.Format("Fail to find prefab, Unit={0} Action={1} ID={2} Prefab={3}",
                mOwner.UnitID,
                mActiveAction.ID,
                data.Id,
                unitBase != null ? unitBase.Prefab : "NotFound"));
            return;
        }

        Vector3 offset = new Vector3(data.PosX, data.PosY, data.PosZ);
        Vector3 pos = mOwner.Position + mOwner.UUnitInfo.transform.rotation * (offset * 0.01f);

        GameObject obj = GameObject.Instantiate(prefab) as GameObject;
        UnitInfo unitInfo = obj.GetComponent<UnitInfo>();
        unitInfo.Unit.SetPosition(pos);
        if (data.Local)
            unitInfo.Unit.SetOrientation(mOwner.Orientation + data.Angle * Mathf.Deg2Rad);
        else
            unitInfo.Unit.SetOrientation(data.Angle * Mathf.Deg2Rad);
        unitInfo.Unit.PlayAction(data.ActionId);
        unitInfo.Camp = mOwner.Camp;
        unitInfo.Unit.Owner = mOwner;
        unitInfo.Unit.ActionStatus.SkillItem = SkillItem;
    }

    bool TriggerEvent(Data.Event actionEvent, int deltaTime)
    {
        switch (actionEvent.EventType)
        {
        case Data.EventType.StatusOn:
            {
                Data.EventStatusOn data = (Data.EventStatusOn)actionEvent.EventDetailData;
                SwitchStatus(data.StatusName, true);
            }
            break;
        case Data.EventType.StatusOff:
            {
                Data.EventStatusOff data = (Data.EventStatusOff)actionEvent.EventDetailData;
                SwitchStatus(data.StatusName, false);
            }
            break;
        case Data.EventType.SetVelocity:
            {
                Data.EventSetVelocity data = (Data.EventSetVelocity)actionEvent.EventDetailData;
                SetVelocity(actionEvent, data.VelocityX * -0.01f, data.VelocityY * 0.01f, data.VelocityZ * 0.01f, deltaTime);
            }
            break;
        case Data.EventType.SetVelocity_X:
            {
                Data.EventSetVelocity_X data = (Data.EventSetVelocity_X)actionEvent.EventDetailData;
                SetVelocity(actionEvent, data.VelocityX * -0.01f, mVelocity.y, mVelocity.z, deltaTime);
            }
            break;
        case Data.EventType.SetVelocity_Y:
            {
                Data.EventSetVelocity_Y data = (Data.EventSetVelocity_Y)actionEvent.EventDetailData;
                SetVelocity(actionEvent, mVelocity.x, data.VelocityY * 0.01f, mVelocity.z, deltaTime);
            }
            break;
        case Data.EventType.SetVelocity_Z:
            {
                Data.EventSetVelocity_Z data = (Data.EventSetVelocity_Z)actionEvent.EventDetailData;
                SetVelocity(actionEvent, mVelocity.x, mVelocity.y, data.VelocityZ * 0.01f, deltaTime);
            }
            break;
        case Data.EventType.SetDirection:
            {
                Data.EventSetDirection data = (Data.EventSetDirection)actionEvent.EventDetailData;
                SetDirection(actionEvent, data.Angle, data.Local);
            }
            break;
        case Data.EventType.PlaySound:
            {
                Data.EventPlaySound data = (Data.EventPlaySound)actionEvent.EventDetailData;
                if (data.SoundIndex == -2)
				{
                    data.SoundIndex = SoundManager.Instance.GetSoundIndex(data.SoundName);
					if (data.SoundIndex < 0)
						Debug.LogError(string.Format("Fail to playsound: [{0}][{1}][{2}]", mOwner.UnitID, mActiveAction.ID, data.SoundName));
				}
			
                if (data.SoundIndex >= 0)
                    PlaySound(actionEvent, data.SoundIndex, data.CheckMatril);
            }
            break;
        case Data.EventType.SetGravity:
            {
                Data.EventSetGravity data = (Data.EventSetGravity)actionEvent.EventDetailData;
                mGravity = data.Gravity * 0.01f;
            }
            break;
        case Data.EventType.RemoveMyself:
			//Debug.Log("Unit " + mOwner.UnitID + " RemoveMyself");
            mOwner.Destory();
            break;
        case Data.EventType.AdjustVarible:
            {
                Data.EventAdjustVarible data = (Data.EventAdjustVarible)actionEvent.EventDetailData;
                CustomVariable variable = mOwner.GetVariable(data.Slot);
                variable.Adjust(data.Value);
            }
            break;
        case Data.EventType.SetVariable:
            {
                Data.EventSetVariable data = (Data.EventSetVariable)actionEvent.EventDetailData;
                CustomVariable variable = mOwner.GetVariable(data.Slot);
                variable.Set(data.Value, data.MaxValue);
            }
            break;
        case Data.EventType.ListTargets:
            ListTargets((Data.EventListTargets)actionEvent.EventDetailData);
            break;
        case Data.EventType.ClearTargets:
            ActionTarget = null;
            break;
        case Data.EventType.FaceTargets:
            FaceTargets((Data.EventFaceTargets)actionEvent.EventDetailData);
            break;
        case Data.EventType.GoToTargets:
            GoToTargets((Data.EventGoToTargets)actionEvent.EventDetailData);
            break;
        case Data.EventType.PlayEffect:
            PlayEffect((Data.EventPlayEffect)actionEvent.EventDetailData);
            break;
        case Data.EventType.HasCollision:
            {
                Data.EventHasCollision data = (Data.EventHasCollision)actionEvent.EventDetailData;
                mOwner.EnableCollision(data.HasCollision);
            }
            break;
        case Data.EventType.ExeScript:
            {
                Data.EventExeScript data = (Data.EventExeScript)actionEvent.EventDetailData;
                string[] strs = data.ScriptCmd.Split('(');
                string scriptname = strs[0];
                string parameter = strs[1];
                mOwner.UGameObject.SendMessage(scriptname, parameter.Remove(parameter.Length - 1));
				//Debug.Log(scriptname);
            }
			break;
		case Data.EventType.ActionLevel:
			{
				Data.EventActionLevel data = (Data.EventActionLevel)actionEvent.EventDetailData;
				mActionLevel = data.Level;
			}
            break;
        case Data.EventType.AddUnit:
            AddUnit((Data.EventAddUnit)actionEvent.EventDetailData);
            break;
        }

        return true;
    }

    bool ProcessHitDefineList(int nextKey)
    {
	    if (mActiveAction.mAttackDefs.Count == 0 || mHitDefIndex >= mActiveAction.mAttackDefs.Count)
		    return false;
    	
	    bool ret = false;
	    while (mHitDefIndex < mActiveAction.mAttackDefs.Count)
	    {
            Data.AttackDef hit_data = mActiveAction.mAttackDefs[mHitDefIndex];
		    if (hit_data.TriggerTime > nextKey)
			    break;

		    if (hit_data.EventOnly == 0)
		    {
			    CreateHitDefine(hit_data, Vector3.zero, mActiveAction.ID);
			    ret = true;
		    }
		    mHitDefIndex++;
	    }
	    return ret;
    }

    bool CreateHitDefine(Data.AttackDef hit_data, Vector3 position, string action)
    {
        GameObject hitDefObject = new GameObject("HitDefinition");
        HitDefinition hitDefinition = hitDefObject.AddComponent<HitDefinition>();
        hitDefinition.Init(hit_data, mOwner, action, SkillItem);
        return true;
    }
        
    //---------------------------------------------------------------------
    bool ProcessActionInterruptList(int preKey, int nextKey)
    {
	    if (mQueuedInterrupt != null)
		    return false;

	    // check the action interrupts
	    if (mActiveAction.mActionInterrupts.Count == 0)
		    return false;

	    int interruptIdx = 0;
        foreach (Data.ActionInterrupt interrupt in mActiveAction.mActionInterrupts)
	    {
		    if (interrupt.mEnableBegin != 0 && interrupt.mEnableBegin > preKey && interrupt.mEnableBegin <= nextKey)
			    EnableActionRequest(interruptIdx, true);
    		
		    if (interrupt.mEnableEnd != 200 && interrupt.mEnableEnd > preKey && interrupt.mEnableEnd <= nextKey)
			    EnableActionRequest(interruptIdx, false);

            if (GetInterruptEnabled(interruptIdx++) && interrupt.mConditionInterrupte)
		    {
                if (ProcessActionInterrupt(interrupt))
				    return true;
		    }
	    }
	    return false;
    }
        
    //---------------------------------------------------------------------
    void EnableActionRequest(int statusIdx, bool enabled)
    {
	    if (enabled)
		    mActionInterruptEnabled |= (1 << statusIdx);
	    else
		    mActionInterruptEnabled &= ~(1 << statusIdx);
    }

    public bool GetInterruptEnabled(int idx) 
    { 
        return (mActionInterruptEnabled & (1<<idx)) != 0; 
    }

    bool ProcessActionInterrupt(Data.ActionInterrupt interrupt)
    {
	    if (!interrupt.mConditionInterrupte)
		    return false;

	    // the [interrupt.ConditionInterrupte] need user input.
	    // do not process it here.
	    if (interrupt.mCheckAllCondition && interrupt.mCheckInput1)
		    return false;
    	
	    if (!CheckActionInterrupt(interrupt))
		    return false;
		
	    return LinkAction(interrupt, null);
    }

    bool DetectVariable(Data.ActionInterrupt interrupt)
    {
        switch (interrupt.Variable)
		{
		case (int)EVariableIdx.EVI_HP:
            return CustomVariable.Compare((ECompareType)interrupt.CompareType, mOwner.GetAttrib(EPA.CurHP), interrupt.CompareValue);
		case (int)EVariableIdx.EVI_HPPercent:
			return CustomVariable.Compare((ECompareType)interrupt.CompareType, 
				mOwner.GetAttrib(EPA.CurHP) * 100 / mOwner.GetAttrib(EPA.HPMax), 
				interrupt.CompareValue);
		case (int)EVariableIdx.EVI_Level:
            return CustomVariable.Compare((ECompareType)interrupt.CompareType, mOwner.Level, interrupt.CompareValue);
		default:
			{
	        	int varIndex = interrupt.Variable - (int)EVariableIdx.EVI_Custom;
	        	CustomVariable variable = mOwner.GetVariable(varIndex);
	        	return variable.Compare((ECompareType)interrupt.CompareType, interrupt.CompareValue);
			}
		}
    }
    
    //---------------------------------------------------------------------
    public bool CheckActionInterrupt(Data.ActionInterrupt interrupt)
    {
        bool ret = false;
        if (interrupt.mCheckAllCondition)
        {
            ret = true;
            ret = ret && (!interrupt.mTouchGround || mOwner.OnGround);
            ret = ret && (!interrupt.mTouchWall || mOwner.OnTouchWall);
            ret = ret && (!interrupt.mReachHighest || mOwner.OnHighest);
            ret = ret && (!interrupt.mUnitDead || mOwner.Dead);
            ret = ret && (!interrupt.mHitTarget || (mOwner.HitTarget != null));
            ret = ret && (!interrupt.mDetectVariable || DetectVariable(interrupt));
            ret = ret && (!interrupt.mSenseTarget || SenseTarget(interrupt));
        }
        else
        {
            ret = false;
            ret = ret || (interrupt.mTouchGround && mOwner.OnGround);
            ret = ret || (interrupt.mTouchWall && mOwner.OnTouchWall);
            ret = ret || (interrupt.mReachHighest && mOwner.OnHighest);
            ret = ret || (interrupt.mUnitDead && mOwner.Dead);
            ret = ret || (interrupt.mHitTarget && (mOwner.HitTarget != null));
            ret = ret || (interrupt.mDetectVariable && DetectVariable(interrupt));
            ret = ret || (interrupt.mSenseTarget && SenseTarget(interrupt));
        }

        return ret;
    }
    //---------------------------------------------------------------------
    public bool LinkAction(Data.ActionInterrupt interrupt, SkillInput skillInput)
    {
	    if (interrupt.ConnectMode >= 2)
            return false;

        bool connectImmediately = (interrupt.ConnectMode == 0);
	    if (!connectImmediately && mActiveAction != null)
	    {
		    // check the queued time.
		    int actualQueuedTime = (interrupt.ConnectTime <= 100) ? 
			    mActiveAction.AnimTime * interrupt.ConnectTime / 100 :	// [0-100] AnimTime
			    mActiveAction.AnimTime + mActiveAction.PoseTime * (interrupt.ConnectTime - 100) / 100; // [100-200] PoseTime

		    // if the time already passed, do it immediately.
		    if (actualQueuedTime <= mActionTime)
			    connectImmediately = true;
	    }

	    // do it immediately if the request is this.
        if (!connectImmediately)
        {
            mQueuedInterrupt = interrupt;
            mQueuedSkillInput = skillInput;
        }
        else
        {
            mQueuedInterrupt = null;
            mQueuedSkillInput = null;
            if (skillInput != null)
                skillInput.PlaySkill();
            else
                ChangeAction(interrupt.ActionCache, 0);
        }
    	
	    return true;
    }

    public bool OnHit(Data.HitResultType HitResult, bool remoteAttacks)
    {
	    // copy the action request enabled/disabled flags.
	    for (int interruptIdx = 0; interruptIdx < mActiveAction.mActionInterrupts.Count; interruptIdx++)
        {
            Data.ActionInterrupt interrupt = mActiveAction.mActionInterrupts[interruptIdx];
		    if (GetInterruptEnabled(interruptIdx) && 
                interrupt.Hurted &&
                (interrupt.HurtType & (1 << (int)HitResult)) != 0 && 
                (!interrupt.RemoteOnly || remoteAttacks))
            {
			    LinkAction(interrupt, null);
				Debug.Log("OnHit Link Action: " + interrupt.ActionID);
			    return true;
		    }
	    }

        string changeAction = "";

		bool handled = true;
	    switch (HitResult)
	    {
	    case Data.HitResultType.StandHit:
		    {
			    if (mHeightState == Data.HeightStatusFlag.Stand)
				    changeAction = mActionGroup.StandStandHit;
			    else if(mHeightState == Data.HeightStatusFlag.LowAir || mHeightState == Data.HeightStatusFlag.HighAir)
				    changeAction = mActionGroup.AirStandHit;
			    else if(mHeightState == Data.HeightStatusFlag.Ground)
				    changeAction = mActionGroup.FloorStandHit;
		    }
		    break;
	    case Data.HitResultType.KnockOut:
		    {
			    if (mHeightState == Data.HeightStatusFlag.Stand)
				    changeAction = mActionGroup.StandKnockOut;
			    else if(mHeightState == Data.HeightStatusFlag.LowAir || mHeightState == Data.HeightStatusFlag.HighAir)
				    changeAction = mActionGroup.AirKnockOut;
			    else if(mHeightState == Data.HeightStatusFlag.Ground)
				    changeAction = mActionGroup.FloorKnockOut;
		    }
		    break;
	    case Data.HitResultType.KnockBack:
		    {
			    if(mHeightState == Data.HeightStatusFlag.Stand)
				    changeAction = mActionGroup.StandKnockBack;
			    else if(mHeightState == Data.HeightStatusFlag.LowAir || mHeightState == Data.HeightStatusFlag.HighAir)
				    changeAction = mActionGroup.AirKnockBack;
			    else if(mHeightState == Data.HeightStatusFlag.Ground)
				    changeAction = mActionGroup.FloorKnockBack;
		    }
		    break;
	    case Data.HitResultType.KnockDown:
		    {
			    if(mHeightState == Data.HeightStatusFlag.Stand)
				    changeAction = mActionGroup.StandKnockDown;
			    else if(mHeightState == Data.HeightStatusFlag.LowAir || mHeightState == Data.HeightStatusFlag.HighAir)
				    changeAction = mActionGroup.AirKnockDown;
			    else if(mHeightState == Data.HeightStatusFlag.Ground)
				    changeAction = mActionGroup.FloorKnockDown;
		    }
		    break;
	    case Data.HitResultType.DiagUp:
		    {
			    if(mHeightState == Data.HeightStatusFlag.Stand)
				    changeAction = mActionGroup.StandDiagUp;
			    else if(mHeightState == Data.HeightStatusFlag.LowAir || mHeightState == Data.HeightStatusFlag.HighAir)
				    changeAction = mActionGroup.AirDiagUp;
			    else if(mHeightState == Data.HeightStatusFlag.Ground)
				    changeAction = mActionGroup.FloorDiagUp;
		    }
		    break;
	    case Data.HitResultType.Hold:
		    {
			    if(mHeightState == Data.HeightStatusFlag.Stand)
				    changeAction = mActionGroup.StandHold;
			    else if(mHeightState == Data.HeightStatusFlag.LowAir || mHeightState == Data.HeightStatusFlag.HighAir)
				    changeAction = mActionGroup.AirHold;
			    else if(mHeightState == Data.HeightStatusFlag.Ground)
				    changeAction = mActionGroup.FloorHold;
		    }
		    break;
	    case Data.HitResultType.AirHit:
		    {
			    if(mHeightState == Data.HeightStatusFlag.Stand)
				    changeAction = mActionGroup.StandAirHit;
			    else if(mHeightState == Data.HeightStatusFlag.LowAir || mHeightState == Data.HeightStatusFlag.HighAir)
				    changeAction = mActionGroup.AirAirHit;
			    else if(mHeightState == Data.HeightStatusFlag.Ground)
				    changeAction = mActionGroup.FloorAirHit;
		    }
		    break;
	    case Data.HitResultType.DownHit:
		    {
			    if(mHeightState == Data.HeightStatusFlag.Stand)
				    changeAction = mActionGroup.StandDownHit;
			    else if(mHeightState == Data.HeightStatusFlag.LowAir || mHeightState == Data.HeightStatusFlag.HighAir)
				    changeAction = mActionGroup.AirDownHit;
			    else if(mHeightState == Data.HeightStatusFlag.Ground)
				    changeAction = mActionGroup.FloorDownHit;
		    }
		    break;
	    case Data.HitResultType.FallDown:
		    {
			    if(mHeightState == Data.HeightStatusFlag.Stand)
				    changeAction = mActionGroup.StandFallDown;
			    else if(mHeightState == Data.HeightStatusFlag.LowAir || mHeightState == Data.HeightStatusFlag.HighAir)
				    changeAction = mActionGroup.AirFallDown;
			    else if(mHeightState == Data.HeightStatusFlag.Ground)
				    changeAction = mActionGroup.FloorFallDown;
		    }
		    break;
	    default:
		    handled = false;
		    break;
        }

        if (!string.IsNullOrEmpty(changeAction))
            ChangeAction(changeAction, 0);

	    return handled;
    }


    public void SetLashVelocity(float x, float y, float z, int lashTime)
    {
        mVelocity.x = x * mActionGroup.LashModifier.X;
        mVelocity.y = y * mActionGroup.LashModifier.Y;
        mVelocity.z = z * mActionGroup.LashModifier.Z;
        mLashTime = lashTime;
    }

    bool ProcessLash(int deltaTime)
    {
        if (mLashTime <= 0)
            return false;

        mLashTime -= deltaTime;
        if (mLashTime > 0)
            return false;

        // ³å»÷Ê±ŒäÍê³É¡£
        if (mHeightState == Data.HeightStatusFlag.Stand || mHeightState == Data.HeightStatusFlag.Ground)
        {
            mVelocity.x = 0.0f;
            mVelocity.z = 0.0f;
        }

        return true;
    }

    public void SetStraightTime(int time, bool onHit)
    {
        mStraightTime = time;
        mOnStarightHit = onHit;
		if (mStraightTime > 0)
        	mOwner.BeginStaight();
    }

    bool ProcessStraighting(ref int deltaTime)
    {
        if (mStraightTime > 0)
		{
            mStraightTime -= deltaTime;
            if (mStraightTime <= 0)
            {
                if (mOwner.UUnitInfo.Model)
                    mOwner.UUnitInfo.Model.localPosition = Vector3.zero;

                mOwner.EndStaight();
                deltaTime = -mStraightTime;
            }
            else if (mOnStarightHit)
			{
				Vector3 straighMove = new Vector3(mStraighExtent, 0f, 0f);
                if (mOwner.UUnitInfo.Model)
                    mOwner.UUnitInfo.Model.localPosition = straighMove;
				mStraighExtent = -mStraighExtent;
			}

            return true;
		}

	    return false;
    }
}
