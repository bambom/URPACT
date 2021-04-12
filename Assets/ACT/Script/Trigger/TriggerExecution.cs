using UnityEngine;
using System.Collections.Generic;
public enum ExecutionType
{
    PlayCloseUp,
    PlayerPause,
    CameraView,
    WaveControl,
    PlayAction,
    ActiveObject,
    ActiveTrigger,
	ResetPosition,
	SpawnFinish,
	SwapPlayerControl,
	PlayAnimation
}

[System.Serializable]
public class TriggerExecution
{
    public float delayTime;
    public bool executionOver;
    public ExecutionType type;

    public string closeupId;
    public int playerPause;

    public float cameraViewDuration;
    public string cameraViewTo;
    public bool cameraViewReturn;

    public int waveCount;
    public int waveCheck;

    public bool localPlayer;
    public int unitId;
    public string playAction;
    public UnitInfo unitInfo;

    public Transform activeObject;
    public bool active;

    public TriggerCellEx trigger;
    public bool triggerActive;
	
	// Reset Position
	public Transform resetObject;
	public bool resetPlayer;
	public string resetObjectName;
	public Transform targetObject;
	
	public int dieNowType;
	
	public int controlTargetId;
	public Transform controlTargetObject;
	
	public AnimationClip animationClip;
	public string animationObjName;
	public Transform animationObj;
	
	
	TriggerCellEx mCurrentTriggerCellEx;

    public void execCameraView(float duration, string to, bool reset)
    {
        GameObject camera = GameObject.Find("Camera-CloseUp");
        Transform cameraTo = camera.transform.parent;
        if (!reset)
        {
            Transform triggerTransform = mCurrentTriggerCellEx.gameObject.transform;
            for (int i = 0; i < triggerTransform.childCount; i++)
            {
                cameraTo = triggerTransform.GetChild(i);
                if (cameraTo.name == to)
                    break;
            }
        }

        TweenTransform tweenTransform = camera.GetComponent<TweenTransform>();
        tweenTransform.to = cameraTo;
        tweenTransform.duration = duration;
        tweenTransform.Reset();
        tweenTransform.Play(true);
    }

    void execPlayAction(int unitId, string action)
    {
        if (localPlayer)
            UnitManager.Instance.LocalPlayer.PlayAction(action);

        if (unitInfo)
            unitInfo.Unit.PlayAction(action);

        foreach (Unit unit in UnitManager.Instance.UnitInfos)
        {
            if (unit.UnitID == unitId)
                unit.PlayAction(action);
        }
    }
	
	void execResetPosition()
	{
        if (resetPlayer)
        {
            foreach (Unit unit in UnitManager.Instance.UnitInfos)
            {
                if (unit.UnitType == EUnitType.EUT_LocalPlayer)
                {
                    unit.UGameObject.transform.position = targetObject.transform.position;
                    unit.UGameObject.transform.rotation = targetObject.transform.rotation;
                }
            }
        }

        if (!string.IsNullOrEmpty(resetObjectName))
        {
            GameObject obj = GameObject.Find(resetObjectName);
            if (obj != null)
            {
                obj.transform.position = targetObject.transform.position;
                obj.transform.rotation = targetObject.transform.rotation;
            }
        }

        if (resetObject != null)
        {
            resetObject.position = targetObject.transform.position;
            resetObject.rotation = targetObject.transform.rotation;
        }
	}
	
	void execSpawnFinish()
	{
        mCurrentTriggerCellEx.SpawnFinish(dieNowType);
	}
	
	void execSwapPlayerControl()
	{
		List<Unit>currentControlUnit = new List<Unit>();
		List<Unit>targetControlUnit = new List<Unit>();
		UnitManager.Instance.CleanEmptyObject();
		foreach (Unit unit in UnitManager.Instance.UnitInfos)
        {
			if(unit.UGameObject.GetComponent<Controller>() != null && unit.UGameObject.GetComponent<Controller>().enabled)
				currentControlUnit.Add(unit);
            if (controlTargetId > 0 && unit.UnitID == controlTargetId)
				targetControlUnit.Add(unit);
        }
		foreach(Unit curControl in currentControlUnit)
				curControl.UGameObject.GetComponent<Controller>().enabled = false;
		if(targetControlUnit.Count != 0)
		{
			foreach(Unit targetUnit in targetControlUnit)
			{
				targetUnit.UGameObject.GetComponent<Controller>().enabled = true;
				if(targetUnit.UnitType == EUnitType.EUT_LocalPlayer)
					UnitManager.Instance.LocalPlayer = targetUnit as LocalPlayer;
			}
		}
		if(controlTargetObject != null && controlTargetObject.GetComponent<Controller>() != null)
		{
			controlTargetObject.GetComponent<Controller>().enabled = true;
			if(controlTargetObject.GetComponent<UnitInfo>().UnitType == EUnitType.EUT_LocalPlayer)
				UnitManager.Instance.LocalPlayer = controlTargetObject.GetComponent<UnitInfo>().Unit as LocalPlayer;
		}
		GameObject camera = GameObject.Find("Camera-CloseUp");
		TweenTransform tweenTransf = camera.GetComponent<TweenTransform>();
		if(!tweenTransf.enabled)
			ResetCameraPos(null);
		else
			tweenTransf.onFinished = ResetCameraPos;
	}
	
	void ResetCameraPos(UITweener tween)
	{
		GameObject camera = GameObject.Find("Camera-CloseUp");
		Transform cameraTo = camera.transform.parent;
		Controller controller = UnitManager.Instance.LocalPlayer.UGameObject.GetComponent<Controller>();
		controller.CameraTag = cameraTo;
	}
	
	void execPlayAnimation()
	{
		if(animationObj != null)
			animationObj.GetComponent<Animation>().Play(animationClip.name);
		if(!string.IsNullOrEmpty(animationObjName))
			GameObject.Find(animationObjName).GetComponent<Animation>().Play(animationClip.name);
	}
	
    public void execution(TriggerCellEx parent)
    {
		mCurrentTriggerCellEx = parent;
        switch (type)
        {
            case ExecutionType.PlayCloseUp:
                UnitManager.Instance.LocalPlayer.UUnitInfo.PlayCloseup(closeupId);
                break;
            case ExecutionType.PlayerPause:
                UnitManager.Instance.LocalPlayer.PlayAction(Data.CommonAction.Idle);
                Global.PauseAll = (playerPause != 0);
                Global.GInputBox.ResetInput();
                break;
            case ExecutionType.CameraView:
                execCameraView(cameraViewDuration, cameraViewTo, cameraViewReturn);
                break;
            case ExecutionType.WaveControl:
                parent.WaveControl(waveCount, waveCheck);
                break;
            case ExecutionType.PlayAction:
                execPlayAction(unitId, playAction);
                break;
            case ExecutionType.ActiveObject:
                if (activeObject)
                    activeObject.gameObject.SetActive(active);
                break;
            case ExecutionType.ActiveTrigger:
                if (trigger)
                    trigger.OnExecution(triggerActive);
				break;
			case ExecutionType.ResetPosition:
				if(targetObject)
					execResetPosition();
                break;
			case ExecutionType.SpawnFinish:
				if(dieNowType > 0)
					execSpawnFinish();
				break;
			case ExecutionType.SwapPlayerControl:
				if(controlTargetId > 0 || controlTargetObject != null)
					execSwapPlayerControl();
				break;
			case ExecutionType.PlayAnimation:
				if( animationClip != null )
					execPlayAnimation();
				break;
        }
    }

    public TriggerExecution Clone()
    {
        return this.MemberwiseClone() as TriggerExecution;
    }
}
