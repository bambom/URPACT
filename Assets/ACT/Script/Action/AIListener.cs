using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class AIListener : IActionListener
{
    public enum EAITragetType
    {
        EATT_SELF = 0, //自身目标
        EATT_ENEMY = 1, //敌人目标
        EATT_FRIEND = 2, //队友目标
        EATT_PARENT = 3, //父级目标
        EATT_CHILD = 4, //子级目标
        EATT_Max,
    };

    ActionStatus mActionStatus;
    Data.Server.AIGroup mAIGroup;
    Data.Server.AIStatus mActiveStatus;
    Data.Server.AIList mActiveList;
    int mStatusIdx = 0;
    int mQueuedStatusIdx = -1;
    int mLoopCount = 0;
    bool mActionChanging = false;
    int mCurrentActionID;
    float mStatusTime = 0;
    Vector3 mTargetPos = Vector3.zero;
    Unit mOwner;
    Unit mEnemy;
    Unit mTarget;
    Unit mChild;
    Unit mParent;

    Dictionary<int, int> mActionCount = new Dictionary<int, int>();

    struct ActionInfo : IComparable<ActionInfo>
    {
        public int ActionID;
        public float Time;

        public int CompareTo(ActionInfo b) { return Time.CompareTo(b.Time); }
    }
    List<ActionInfo> mActionCDMap = new List<ActionInfo>();

    public AIListener(Unit owner)
    {
        mOwner = owner;
        mActionStatus = mOwner.ActionStatus;
        
        changeAIDiff(owner.UUnitInfo.AIDiff);
    }

    // Update is called once per frame
    public void Update(float deltaTime)
    {
        if (Global.PauseAll)
            return;

        mStatusTime += deltaTime;

        ProcessActionCD(deltaTime);

        if (mOwner.ActionStatus.FaceTarget)
            faceTarget(mTarget);
    }

    void ProcessActionCD(float deltaTime)
    {
        if (mActionCDMap.Count == 0)
            return;

        int count = 0;
        while (count < mActionCDMap.Count && mActionCDMap[count].Time < mStatusTime)
            count++;

        if (count > 0)
            mActionCDMap.RemoveRange(0, count);
    }

    public void OnActionChanging(Data.Action oldAction, Data.Action newAction)
    {
        if (Global.PauseAll || mOwner.Dead)
            return;

        bool doActionChoose = false;
        bool hasQueuedStatus = (mQueuedStatusIdx >= 0);

        if (newAction != null)
            doActionChoose = checkStatusChange(newAction.ActionCache);

        if (mActionChanging)
            return;

        if (!doActionChoose)
        {
            if (oldAction == null || newAction == null)
            {
                // 非正常状态，强制进行选择动作。
                doActionChoose = true;
                mLoopCount = 0;
            }
            else if (oldAction.ActionCache == mCurrentActionID)
            {
                // 判断当前是否为AI选择的动作。
                if (oldAction.DefaultLinkActionID == newAction.ID)
                {
                    // 循环次数减1
                    mLoopCount--;

                    if (newAction.ID == Data.CommonAction.Idle || mLoopCount > 0)
                        doActionChoose = true;
                }
                else
                {
                    // 当前Action是被中断的，不需要重新选择。
                    mLoopCount = 0;
                }
            }
            else
            {
                // 非AI选择动作结束，属于被中断的动作流程。
                // 继续执行到自循环动作
                if (newAction.ID == Data.CommonAction.Idle)
                {
                    // 进入AI选择流程。
                    doActionChoose = true;
                    mLoopCount = 0;
                }
            }
        }

        // for action choose.
        if (doActionChoose)
        {
            // 如果队列里面有安排好了的AI状态，现在进行切换。
            if (hasQueuedStatus)
            {
                changeActionStatus(mQueuedStatusIdx);

                // 清楚队列标志位。
                mQueuedStatusIdx = -1;
            }

            chooseAction();
        }
    }

    bool checkStatusChange(int id)
    {
        // 当AI活动，并且没有AI状态在队列中。
        if (mActiveStatus != null && mQueuedStatusIdx < 0)
        {
            foreach (Data.Server.AIStatusSwitch sw in mActiveStatus.AIStatusSwitchList)
            {
                if (sw.SwitchStatusID == mStatusIdx || sw.SwitchStatusID >= mAIGroup.AIStatusList.Count)
                    continue;

                // 到达指定的动作，切换状态号。
                bool switchSucess = true;
                bool isinchecked = false;
                if ((sw.Condition & 1) != 0)
                {
                    isinchecked = true;
                    switchSucess &= (mStatusTime * 1000 >= sw.SelfTime);
                }
                if ((sw.Condition & (1 << 1)) != 0) //执行指定动作
                {
                    isinchecked = true;
                    if (id == sw.ActionCache)
                    {
                        if (sw.SelfActionCount > 1)
                        {
                            int count;
                            if (mActionCount.TryGetValue(id, out count))
                            {
                                mActionCount[id] = ++count;

                                switchSucess &= (count >= sw.SelfActionCount);
                            }
                            else
                            {
                                mActionCount[id] = 1;
                                switchSucess &= false;
                            }
                        }
                        else
                            switchSucess &= true;
                    }
                    else
                        switchSucess &= false;
                }
                if ((sw.Condition & (1 << 2)) != 0)
                {
                    isinchecked = true;
                    switchSucess &= CompareAIVariable(mOwner, sw.SelfVaribleName, sw.SelfVaribleCompare, sw.SelfVaribleValue);
                }
                if ((sw.Condition & (1 << 3)) != 0)
                {
                    isinchecked = true;
                    if (mTarget != null)
                        switchSucess &= false;
                    else
                        switchSucess &= false;
                }

                if ((sw.Condition & (1 << 7)) != 0)
                {
                    isinchecked = true;
                    Unit target = getTargetObject((EAITragetType)sw.TargetType);
                    if (sw.TargetExist)
                        switchSucess &= (target != null);
                    else
                        switchSucess &= (target == null);
                }
                if ((sw.Condition & (1 << 8)) != 0) //执行指定动作
                {
                    isinchecked = true;
                    if (id == sw.TargetActionCache)
                    {
                        if (sw.TargetActionCount > 1)
                        {
                            int count;
                            if (mActionCount.TryGetValue(id, out count))
                            {
                                mActionCount[id] = ++count;

                                switchSucess &= (count >= sw.TargetActionCount);
                            }
                            else
                            {
                                mActionCount[id] = 1;
                                switchSucess &= false;
                            }
                        }
                        else
                            switchSucess &= true;
                    }
                    else
                        switchSucess &= false;
                }
                if ((sw.Condition & (1 << 9)) != 0)
                {
                    isinchecked = true;
                    switchSucess &= CompareAIVariable(mTarget, sw.TargetVaribleName, sw.TargetVaribleCompare, sw.TargetVaribleValue);
                    //int variable = 0;
                    //Unit* target = getTargetObject(it->TargetType);
                    //if (target && target->GetVariable ((EVariableType)it->ConditionVaribleName, variable) &&
                    //    NetMath::Compare(variable, (ECompareType)it->ConditionVaribleCompare, it->ConditionVaribleValue))
                    //{
                    //    switchSucess = true;
                    //}
                }
                if ((sw.Condition & (1 << 10)) != 0)
                {
                    isinchecked = true;
                    if (mTarget != null)
                    {
                        float distance = MathUtility.DistanceMax(mOwner.Position, mTarget.Position) * 100.0f;
                        switchSucess &= (distance >= sw.TargetDistanceMin && distance <= sw.TargetDistanceMax);
                    }
                    else
                        switchSucess &= false;
                }
                if ((sw.Condition & (1 << 11)) != 0)
                {
                    isinchecked = true;
                    if (mTarget != null)
                        switchSucess &= false;
                    else
                        switchSucess &= false;
                }
                switchSucess &= isinchecked;

                if (switchSucess)
                {
                    if (sw.ActionSwitchNow != 0)
                    {
                        changeActionStatus(sw.SwitchStatusID);
                        return true;
                    }
                    else
                    {
                        // 将弄成队列里面去。
                        mQueuedStatusIdx = sw.SwitchStatusID;
                    }

                    break;
                }
            }
        }
        return false;
    }

    bool CompareAIVariable(Unit unit, int variableIdx, int comparetype, int comvalue)
    {
        switch (variableIdx)
        {
            case (int)EVariableIdx.EVI_HP:
                return CustomVariable.Compare((ECompareType)comparetype, unit.GetAttrib(EPA.CurHP), comvalue);
            case (int)EVariableIdx.EVI_HPPercent:
                return CustomVariable.Compare((ECompareType)comparetype,
                    unit.GetAttrib(EPA.CurHP) * 100 / unit.GetAttrib(EPA.HPMax),
                    comvalue);
            case (int)EVariableIdx.EVI_Level:
                return CustomVariable.Compare((ECompareType)comparetype, unit.Level, comvalue);
            default:
                {
                    int varIndex = comvalue - (int)EVariableIdx.EVI_Custom;
                    CustomVariable variable = unit.GetVariable(varIndex);
                    return variable.Compare((ECompareType)comparetype, comvalue);
                }
        }
    }

    public void changeAIDiff(int diff)
    {
        if (diff == 0)
            mAIGroup = mActionStatus.ActionGroup.AISetting.EasyGroup;
        else if (diff == 1)
            mAIGroup = mActionStatus.ActionGroup.AISetting.NormalGroup;
        else if (diff == 2)
            mAIGroup = mActionStatus.ActionGroup.AISetting.HardGroup;
        else if (diff == 3)
            mAIGroup = mActionStatus.ActionGroup.AISetting.NightmareGroup;
		changeActionStatus(0);
    }

    //-----------------------------------------------------------------------
    public void changeActionStatus(int idx)
    {
        if (idx >= mAIGroup.AIStatusList.Count)
            return;

        // 刷新目标列表。
        refreshTargetList();

        mStatusIdx = idx;
        mActiveStatus = mAIGroup.AIStatusList[idx];

        mActionCDMap.Clear();
        mActionCount.Clear();
        mLoopCount = 0;
        mStatusTime = 0;
    }

    //-----------------------------------------------------------------------
    void faceTarget(Unit target)
    {
        if (target == null) return;

        float x = target.Position.x - mOwner.Position.x;
        float z = target.Position.z - mOwner.Position.z;
        float dir = Mathf.Atan2(x, z);
        mOwner.SetOrientation(dir);
    }
    //-----------------------------------------------------------------------
    void play(Unit target, int id)
    {
        if (mActiveStatus != null && mActiveStatus.AIActionCDList.Count != 0)
        {
            foreach (Data.Server.AIActionCD actionCD in mActiveStatus.AIActionCDList)
            {
                if (actionCD.ActionCache == id)
                {
                    ActionInfo actionInfo = new ActionInfo() { ActionID = id, Time = mStatusTime + actionCD.CD * 0.001f };
                    mActionCDMap.Add(actionInfo);
                    if (mActionCDMap.Count > 1)
                        mActionCDMap.Sort();
                    break;
                }
            }
        }
        mActionStatus.ChangeAction(id, 0);
    }

    //-----------------------------------------------------------------------
    void chooseAction()
    {
        // 清楚当前选择。
        mActiveList = null;

        // 进行AI选择了。
        // 循环执行AI选择动作。
        if (mLoopCount > 0)
        {
            // 设置标签，避免重复计算[onActionFinished]
            mActionChanging = true;
            play(mTarget, mCurrentActionID);
            mActionChanging = false;
            return;
        }

        // choose a target when action finished.
        if (mActiveStatus != null && mActiveStatus.AILists.Count != 0)
        {
            mActiveList = mActiveStatus.AILists[mActiveStatus.AILists.Count - 1];
            mTarget = getTargetObject((EAITragetType)mActiveStatus.TargetType);
			if (mTarget == null) refreshTargetList();
            mActionStatus.ActionTarget = mTarget;

            if (fetchTargetPosition((EAITragetType)mActiveStatus.TargetType, ref mTargetPos))
            {
                float targetDistanceSqr = MathUtility.DistanceSqr(mOwner.Position, mTargetPos);

                // choose a action set base the distance.
                int checkDist = (int)(targetDistanceSqr * 10000);
                foreach (Data.Server.AIList aiList in mActiveStatus.AILists)
                {
                    if (aiList.DistanceSqr < checkDist)
                        break;
                    mActiveList = aiList;
                }
            }

            // 是否有AI距离可以选择。
            if (mActiveList == null || mActiveList.AISlots.Count == 0)
            {
                mActionChanging = false;
                return;
            }


            // 计算出总共的动作几率。
            int totalProability = 0, idx = 0;
            int action_enabled = 0;
            foreach (Data.Server.AISlot slot in mActiveList.AISlots)
            {
                if (!string.IsNullOrEmpty(slot.SwitchActionID))
                {
                    int checkid = slot.ActionCache;
                    if (mActionCDMap.Count == 0 || !mActionCDMap.Exists(delegate(ActionInfo info) { return info.ActionID == checkid; }))
                    {
                        action_enabled |= (1 << idx);
                        totalProability += slot.Ratio;
                    }
                }
                idx++;
            }

            idx = 0;
            if (totalProability <= 0)
                return;

            // 取出随机值范围值。
            int randValue = UnityEngine.Random.Range(0, totalProability);
            Data.Server.AISlot targetSlot = null;
            foreach (Data.Server.AISlot slot in mActiveList.AISlots)
            {
                if ((action_enabled & (1 << idx)) != 0)
                {
                    randValue -= slot.Ratio;
                    if (randValue < 0)
                    {
                        targetSlot = slot;
                        break;
                    }
                }
                idx++;
            }

            if (targetSlot != null)
            {
                // 刷新目标列表。
                if (targetSlot.RefreshTargetList != 0)
                    refreshTargetList();

                mActionChanging = true;
                play(mTarget, targetSlot.ActionCache);
                mActionChanging = false;

                mLoopCount = targetSlot.Count;
                mCurrentActionID = targetSlot.ActionCache;
            }
        }
    }

    Unit getTargetObject(EAITragetType target_type)
    {
        switch (target_type)
        {
            case EAITragetType.EATT_SELF: return mOwner;
            case EAITragetType.EATT_ENEMY: return mEnemy;
            case EAITragetType.EATT_FRIEND: return null;
            case EAITragetType.EATT_PARENT: return mParent;
            case EAITragetType.EATT_CHILD: return mChild;
            default: return null;
        }
    }

    //-----------------------------------------------------------------------
    Unit chooseTarget()
    {
        return UnitManager.Instance.LocalPlayer;
    }

    void refreshTargetList()
    {
        mEnemy = selectEnemy();
        mChild = selectChild();
        mParent = null;// gameObject;
    }

    Unit selectEnemy()
    {
        return UnitManager.Instance.LocalPlayer;
    }
    //-----------------------------------------------------------------------
    Unit selectChild()
    {
        return null;
    }
    //-----------------------------------------------------------------------
    bool fetchTargetPosition(EAITragetType target_type, ref Vector3 targetPos)
    {
        Unit target = getTargetObject(target_type);
        if (target == null)
            return false;

        targetPos = target.Position;

        return true;
    }

    public void OnInputMove() { }
    public void OnHitData(HitData hitData) { }
    public void OnHurt(int damage) { }
    public void OnBuff(UInt32 target, int id) { }
    public void OnFaceTarget() { }
    public void OnAttribChanged(EPA attrib) { }
}
