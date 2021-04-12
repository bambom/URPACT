using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UnitInfo : MonoBehaviour
{
    public int UnitID = 1003;
    public int Level = 1;
    public int ActionGroup = 0;
    public EUnitCamp Camp = EUnitCamp.EUC_FRIEND;
    public EUnitType UnitType = EUnitType.EUT_Monster;
	public Transform SizeTransform;
    public Transform Model;
    public Transform BuffAttachPoint;
    public bool AIEnable = false;
    public int AIDiff = 0;
	public UnitState State = UnitState.Normal;
	public bool initialization = false;
	public GameObjectPool Pool;
    public int PoolIndex = 0;
	public Vector3 HpPointDifference;
	public bool AwakeIfExecution = true;
	public GameObject triggerCell;
	public List<DropItemInfo> DropInfo;

    Unit mUnit = null;
    UnitTopUI mUnitTopUI = null;

    public Unit Unit { get { return mUnit; } }
    public UnitTopUI UnitTopUI { get { return mUnitTopUI; } }
	
    void Awake()
    {
     	if (UnitType == EUnitType.EUT_LocalPlayer)
         	mUnit = new LocalPlayer(this, Level);
     	else if (UnitType == EUnitType.EUT_OtherPlayer)
			mUnit = new Player(this, Level);
        else if (UnitType == EUnitType.EUT_Monster)
         	mUnit = new Monster(this, Level);
		else if(UnitType == EUnitType.EUT_Npc)
            mUnit = new Npc(this, Level);
		if (mUnit != null)
	     	mUnit.Init();
    }
	
	void Start()
	{
        if (mUnit != null)
        {
            mUnitTopUI = gameObject.AddComponent<UnitTopUI>();
            mUnitTopUI.Bind(mUnit);

            mUnit.Start();
        }
	}
	
	// Update is called once per frame
	void Update() 
    {
        if (mUnit != null)
            mUnit.Update(Time.deltaTime);
	}
	
	public void Destroy()
    {
        if (mUnit != null)
            mUnit.Destory();
	}
	
	public void PlayAction(string action)
	{
		mUnit.PlayAction(action);
	}

    void CameraShake(string param)
    {
		if (UnitManager.Instance.LocalPlayer == null || UnitType == EUnitType.EUT_OtherPlayer)
			return;
			
        string[] args = param.Split(", ".ToCharArray(), System.StringSplitOptions.RemoveEmptyEntries);
		Camera camera= (Camera)GameObject.FindWithTag("MainCamera").GetComponent<Camera>();
		CameraShake cameraShake = camera.GetComponent<CameraShake>();
        cameraShake.Shake( float.Parse(args[0]), float.Parse(args[1]), float.Parse(args[2]) );
    }
	
	public void PlayCloseup(string CloseupId)
    {
        if (UnitManager.Instance.LocalPlayer == null || UnitType == EUnitType.EUT_OtherPlayer)
            return;

		List<CloseupBase> closeupList =  CloseupManager.Instance.GetCollection(int.Parse(CloseupId));
		if (closeupList == null || closeupList.Count == 0)
		{
			Debug.LogWarning("closeup not find with id " + CloseupId);
			return;
		}

        GameObject mainCamera = GameObject.FindWithTag("MainCamera");
        if (mainCamera)
        {
            CloseupEffect efc = mainCamera.GetComponent<CloseupEffect>();
            if (efc != null)
                efc.PlayCloseupEffect(closeupList, mUnit);
        }
	}
		
    public void ScreenEffect(string param)
    {
        if (UnitManager.Instance.LocalPlayer == null || UnitType == EUnitType.EUT_OtherPlayer)
            return;

        GameObject mainCamera = GameObject.FindWithTag("MainCamera");
        if (mainCamera)
        {
            ScreenEffect effect = mainCamera.GetComponent<ScreenEffect>();
       	    if (effect)
                effect.PlayScreenEffect(param);
        }
    }
	
	void PlayEnvironmentShake()
    {
        if (UnitManager.Instance.LocalPlayer == null || UnitType == EUnitType.EUT_OtherPlayer)
            return;

		GameObject obj = GameObject.Find("ENV_OBJ");
        if (obj)
        {
            EnvironmentShake[] envShake = obj.GetComponentsInChildren<EnvironmentShake>();
            foreach (EnvironmentShake shake in envShake)
                shake.PlayEnvironmentShake();
        }
	}

	void ChangActionGroup(string param)
	{
		int index = 0;
		if (int.TryParse(param,out index))
			Unit.ActionStatus.ChangeActionGroup(index);
	}
}
