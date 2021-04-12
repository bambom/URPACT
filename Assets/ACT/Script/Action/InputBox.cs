using System;
using System.Collections;
using UnityEngine;

public class InputBox
{
    [Serializable]
	public class KeyState
    {
        public string AxisName;
        public float PressedTime = 10.0f;
        public float ReleasedTime = 10.0f;
        public int Pressed = 0;			// 0=release 1=click 2=double click
        public int Operation = 0;		// for action input binding...
	};

    public KeyState[] KeyStates = new KeyState[(int)EKeyList.KL_Max];
    const float DoubleClickTime = 0.4f;
    const float LongPressedTime = 0.3f;

    Unit mOwner;
    ActionStatus mActionStatus;
    Controller mController;
    Vector2 mInputVector = Vector2.zero;
    Vector2 mLastInputVector = Vector2.one;
    bool mMoveKeyDown = false;

	// Use this for initialization
    public InputBox(Unit owner, Controller controller)
    {
        mOwner = owner;
        mController = controller;
        mActionStatus = mOwner.ActionStatus;

        for (int i = 0; i < KeyStates.Length; i++)
            KeyStates[i] = new KeyState();
        KeyStates[(int)EKeyList.KL_Jump].AxisName = "Jump";
        KeyStates[(int)EKeyList.KL_Attack].AxisName = "Attack";
        KeyStates[(int)EKeyList.KL_SubAttack].AxisName = "SubAttack";
        KeyStates[(int)EKeyList.KL_SkillAttack].AxisName = "SkillAttack";
        KeyStates[(int)EKeyList.KL_AuxKey].AxisName = "AuxKey";
	}

    // Update is called once per frame
    public void Update(float deltaTime)
    {
        if (Global.PauseAll)
            return;

        // update the key status first.
        UpdateKeyStatus(deltaTime);

        UpdateMoveInput(deltaTime);

        mInputVector = new Vector2(NGUIJoystick.Delta.x, -NGUIJoystick.Delta.y);
        if (mInputVector == Vector2.zero)
		{
			if (NGUIJoystick.Pressed)
            	mInputVector = mLastInputVector;
			else
				mInputVector = new Vector2(Input.GetAxis("Horizontal"), -Input.GetAxis("Vertical"));
		}

        bool moveKeyDown = (mInputVector != Vector2.zero);
        if (moveKeyDown)
            mLastInputVector = mInputVector;

        if (moveKeyDown != mMoveKeyDown)
        {
            if (moveKeyDown)
                OnKeyDown(KeyStates[(int)EKeyList.KL_Move]);
            else
                OnKeyUp(KeyStates[(int)EKeyList.KL_Move]);
            mMoveKeyDown = moveKeyDown;
        }

	}

    void UpdateKeyStatus(float deltaTime)
    {
        // check the key status.
        for (int idx = 0; idx < KeyStates.Length; idx++)
        {
            KeyState keyStatus = KeyStates[idx];
            keyStatus.PressedTime += deltaTime;
            keyStatus.ReleasedTime += deltaTime;

            if (string.IsNullOrEmpty(keyStatus.AxisName))
                continue;

            // check the key state.
			if (MainScript.Instance == null)
			{
	            bool pressed = Input.GetAxis(keyStatus.AxisName) > 0;
	            if (pressed && keyStatus.Pressed == 0)
	                OnKeyDown(keyStatus);
	
	            if (!pressed && keyStatus.Pressed != 0)
	                OnKeyUp(keyStatus);
			}
        }
    }
	
	public void OnKeyDown(EKeyList key)
	{
		OnKeyDown(KeyStates[(int)key]);
	}
	
	public void OnKeyUp(EKeyList key)
    {
        OnKeyUp(KeyStates[(int)key]);
    }
	
    public void OnKeyDown(KeyState keyStatus)
    {
        keyStatus.Pressed = keyStatus.PressedTime < DoubleClickTime ? 2 : 1;
        keyStatus.PressedTime = 0.0f;
    }

    public void OnKeyUp(KeyState keyStatus)
    {
        keyStatus.Pressed = 0;
        keyStatus.ReleasedTime = 0.0f;
    }

    void UpdateMoveInput(float deltaTime)
    {
        if (mController == null ||
            mActionStatus == null ||
            mActionStatus.ActiveAction == null)
        {
            return;
        }

        // check the rotate & move.
        Vector2 direction = mInputVector;
        if (direction == Vector2.zero)
            return;

        // process moving.
        if (mActionStatus.CanMove && mActionStatus.ActiveAction.MoveSpeed > 0)
        {
            direction.Normalize();

            Player player = (Player)mOwner;
            float moveSpeed = player.GetAttrib(EPA.MoveSpeed) * 0.01f;

            Vector2 moveTrans = direction * moveSpeed * deltaTime;

            float x = -moveTrans.x, y = moveTrans.y;
            MathUtility.Rotate(ref x, ref y, mController.CameraModify);

            // rotating
            if (mActionStatus.CanRotate)
                mOwner.SetOrientation(Mathf.Atan2(x, y));

            // moving.
            mOwner.Move(new Vector3(x, 0, y));

            if (mActionStatus.Listener != null)
                mActionStatus.Listener.OnInputMove();
        }
        else if (mActionStatus.CanRotate) // just rotate.
        {
            float x = -mInputVector.x, y = mInputVector.y;
            MathUtility.Rotate(ref x, ref y, mController.CameraModify);

            mOwner.SetOrientation(Mathf.Atan2(x, y));
        }
    }
    
    //--------------------------------------------------------------------------------------
    bool checkInputType(EKeyList inpuKey, EInputType inputType, float deltaTime)
    {
	    int pressed = KeyStates[(int)inpuKey].Pressed;
        float pressedTime = KeyStates[(int)inpuKey].PressedTime;
        float releasedTime = KeyStates[(int)inpuKey].ReleasedTime;

	    bool ret = false;
	    switch (inputType)
	    {
	    case EInputType.EIT_Click:
		    ret = (pressedTime == 0);
		    break;
	    case EInputType.EIT_DoubleClick:
            ret = (pressed == 2 && pressedTime == 0);
		    break;
	    case EInputType.EIT_Press:
		    ret = (pressed != 0 && (pressedTime < LongPressedTime && pressedTime + deltaTime >= LongPressedTime));
		    break;
	    case EInputType.EIT_Release:
		    ret = (pressed == 0 && releasedTime == 0);
		    break;
	    case EInputType.EIT_Pressing:
		    ret = (pressed != 0);
		    break;
	    case EInputType.EIT_Releasing:
		    ret = (pressed == 0);
		    break;
	    }
	    return ret;
    }
    //--------------------------------------------------------------------------------------
    bool HasInput(EOperation operation, EInputType inputType, float deltaTime)
    {
	    bool ret = false;
	    switch (operation)
	    {
	    case EOperation.EO_Attack:
            ret = checkInputType(EKeyList.KL_Attack, inputType, deltaTime);
		    break;
	    case EOperation.EO_SpAttack:
            ret = checkInputType(EKeyList.KL_SubAttack, inputType, deltaTime);
		    break;
	    case EOperation.EO_Skill:
            ret = checkInputType(EKeyList.KL_SkillAttack, inputType, deltaTime);
		    break;
        case EOperation.EO_Move:
            ret = checkInputType(EKeyList.KL_Move, inputType, deltaTime);
		    break;
	    case EOperation.EO_Jump:
		    ret = checkInputType(EKeyList.KL_Jump, inputType, deltaTime);
		    break;
	    case EOperation.EO_Grab:
	    case EOperation.EO_Front:
	    case EOperation.EO_Back:
            break;
	    case EOperation.EO_Last:
		    ret = checkInputType(EKeyList.KL_LastKey, inputType, deltaTime);
		    break;
	    case EOperation.EO_Auxiliary:
		    ret = checkInputType(EKeyList.KL_AuxKey, inputType, deltaTime);
		    break;
	    }
	    return ret;
    }

    public bool HasInput(int operation, int inputType, float deltaTime)
    {
        return HasInput((EOperation)operation, (EInputType)inputType, deltaTime);
    }

    public void ResetInput()
    {
        foreach (KeyState keyState in KeyStates)
        {
            if (keyState.Pressed != 0)
                OnKeyUp(keyState);
        }
        mMoveKeyDown = false;
    }
}
