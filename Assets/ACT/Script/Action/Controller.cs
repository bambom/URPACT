using System;
using System.Collections;
using UnityEngine;

public class Controller : MonoBehaviour
{
    Unit mOwner;
    ActionStatus mActionStatus;
    float mCameraModify = 0.0f;
    bool mInputLocked = false;
    Vector3 mCameraPosCache = Vector3.zero;
    Vector3 mCameraOffset = Vector3.zero;
    InputBox mInputBox = null;

    public bool InputLocked { get { return mInputLocked; } }

    public Transform CameraTag;
    public Transform CameraTarget;
    public Vector3 CameraPos = new Vector3(3, 2, 0);
    public Vector3 CameraLookAtOffset = new Vector3(0, 1, 0);
    public GameObject LevelUpEffect;
    public GameObject RespawnEffect;
    public GameObject GotSoulEffect;
    public GameObject GotMoneyEffect;
    public GameObject PassLevelEffect;
    public GameObject GotBloodEffect;

    public float CameraModify { get { return mCameraModify; } }
    public Vector3 CameraOffset { set { mCameraOffset = value; } }


    // Use this for initialization
    void Start()
    {
        mOwner = GetComponent<UnitInfo>().Unit;
        mActionStatus = mOwner.ActionStatus;
		OnEnable();
    }

    void OnEnable()
    {
		if (mActionStatus != null)
		{
        	mInputBox = new InputBox(mOwner, this);
        	Global.GInputBox = mInputBox;
		}
    }
	
    void OnDestroy()
    {
		if (mInputBox != null)
			mInputBox.ResetInput();
	}
	
    // LateUpdate is called after all Update functions have been called
    void LateUpdate()
    {
        if (Global.PauseAll)
            return;

        if (!mInputLocked)
        {
            CheckActionInput(Time.deltaTime);

            mInputBox.Update(Time.deltaTime);
        }

        UpdateCamera(Time.deltaTime);
    }

    void CheckActionInput(float deltaTime)
    {
        if (mInputBox == null || mActionStatus == null || mActionStatus.ActiveAction == null || mActionStatus.HasQueuedAction)
            return;

        int interruptIdx = 0;
        foreach (Data.ActionInterrupt interrupt in mActionStatus.ActiveAction.ActionInterrupts)
        {
            if (!mActionStatus.GetInterruptEnabled(interruptIdx++))
                continue;

            bool checker = false;
            if (interrupt.NoInput)
                checker = true;
            else
            {
                if (interrupt.CheckInput1 == false)
                    continue;

                // ²»Œì²â°ŽŒüµÄÇé¿öÏÂ¡£
                if (interrupt.InputKey1 == 0)
                {
                    // ·ÇŒŒÄÜÖÐ¶ÏÖ±œÓÌø¹ý¡£
                    if (interrupt.SkillID == 0)
                        continue;

                    // Œì²âŒŒÄÜŒüÊÇ·ñ±»°ŽÏÂ¡£
                    //checker = mInputBox.IsSkillKeyDown(interrupt.SkillID);
                }
                else if (interrupt.CheckInput2)
                {
                    // or
                    checker =
                        mInputBox.HasInput(interrupt.InputKey1, interrupt.InputType1, deltaTime) ||
                        mInputBox.HasInput(interrupt.InputKey2, interrupt.InputType2, deltaTime) ||
                        mInputBox.HasInput(interrupt.InputKey3, interrupt.InputType3, deltaTime) ||
                        mInputBox.HasInput(interrupt.InputKey4, interrupt.InputType4, deltaTime);
                }
                else
                {
                    // and
                    checker =
                        (interrupt.InputKey1 == 0 || mInputBox.HasInput(interrupt.InputKey1, interrupt.InputType1, deltaTime)) &&
                        (interrupt.InputKey2 == 0 || mInputBox.HasInput(interrupt.InputKey2, interrupt.InputType2, deltaTime)) &&
                        (interrupt.InputKey3 == 0 || mInputBox.HasInput(interrupt.InputKey3, interrupt.InputType3, deltaTime)) &&
                        (interrupt.InputKey4 == 0 || mInputBox.HasInput(interrupt.InputKey4, interrupt.InputType4, deltaTime));
                }
            }

            // pass the key input, then check the other conditions.
            if (checker && (!interrupt.CheckAllCondition || mActionStatus.CheckActionInterrupt(interrupt)))
            {
                bool success = mActionStatus.LinkAction(interrupt, null);
                if (success)
                    break;
            }
        }
    }

    Vector3 mBeginOffset = Vector3.zero;
    Vector3 mAddtiveOffset = Vector3.zero;
    float mAdjustTime = 3.0f;
    float mAdjustLeftTime = 0.0f;

    void UpdateCamera(float deltaTime)
    {
        if (CameraTag == null)
            return;

        if (mCameraPosCache != CameraPos)
        {
            mCameraModify = Mathf.Atan2(CameraPos.x, CameraPos.z);
            mCameraPosCache = CameraPos;
        }

        Vector3 targetPos = mOwner.Position;
        if (CameraTarget)
        {
            if (mActionStatus.ActiveAction.CameraTarget)
            {
                if (mBeginOffset == Vector3.zero)
                    mBeginOffset = CameraTarget.position - targetPos;

                Vector3 nowOffset = CameraTarget.position - targetPos;
                mAddtiveOffset = nowOffset - mBeginOffset;
            }
            else
            {
                if (mBeginOffset != Vector3.zero)
                {
                    mBeginOffset = Vector3.zero;
                    mAdjustLeftTime = mAdjustTime;
                }

                if (deltaTime < mAdjustLeftTime)
                {
                    float alpha = deltaTime / mAdjustLeftTime;
                    mAddtiveOffset.x = Mathf.Lerp(mAddtiveOffset.x, 0, alpha);
                    mAddtiveOffset.z = Mathf.Lerp(mAddtiveOffset.z, 0, alpha);
                    mAdjustLeftTime -= deltaTime;
                }
                else
                {
                    mAddtiveOffset = Vector3.zero;
                }
            }

            targetPos.x += mAddtiveOffset.x;
            targetPos.z += mAddtiveOffset.z;
        }
        CameraTag.position = targetPos + CameraPos;
        CameraTag.LookAt(targetPos + CameraLookAtOffset);
        CameraTag.Translate(mCameraOffset);
    }

    void LockInput(string param)
    {
        mInputLocked = bool.Parse(param);

        if (mInputLocked)
            Global.PauseAll = true;
        else
            Global.PauseAll = false;

        mInputBox.ResetInput();

        CheckActionInput(Time.deltaTime);
    }
}
