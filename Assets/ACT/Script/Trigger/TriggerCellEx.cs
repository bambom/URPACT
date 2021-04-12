using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class Detection
{
    public string Name = "";

    bool mDetectionOver = false;
    public bool detectionOver { get { return mDetectionOver; } set { mDetectionOver = value; } }

    bool mExecutioned = false;
    public bool executioned { get { return mExecutioned; } set { mExecutioned = value; } }

    float mExcutionTime = 0;
    public float excutionTime { get { return mExcutionTime; } set { mExcutionTime = value; } }

    public bool AllDie = false;
    public int unitDieCount = 0;
    public bool twoAnd = true;
    public float passTime = 0;
    public int monsterType = 0;
    public List<TriggerExecution> executionExList = new List<TriggerExecution>();

    public Detection Clone()
    {
        Detection d = (Detection)this.MemberwiseClone();
        d.executionExList = new List<TriggerExecution>();
        foreach (TriggerExecution item in executionExList)
            d.executionExList.Add(item.Clone());
        return d;
    }
}

public class TriggerCellEx : MonoBehaviour
{
    public int waveCount = 0;
    public int waveCheck = 0;
    public bool mTrigger = false;
    public bool ifLevelLoaedEnable = false;
    public string screenFadeIndelay = "";
    public GameCell TargetGameCell;

    public List<Detection> detectionList = new List<Detection>();

    enum ESpawnOp
    {
        SpawnNone = 0,
        SpawnStated,
        SpawnPaused,
        SpawnResumed,
        SpawnReset,
    }

    float mLifeTime = 0;
    int mSpawnCount = 0;
    bool mAllDetectionOver = true;
    bool mScriptEnable = false;
    bool mColliderEnable = false;
    SpawnPoint[] mSpawnPoints;
    ESpawnOp mTriggerOp = ESpawnOp.SpawnNone;
    int[] mDieCountMap = new int[6];
    int[] mTotalCountMap = new int[6];

    public int TotalDie { get { return mDieCountMap[0]; } set { mDieCountMap[0] = value; } }
    public int TotalCount { get { return mTotalCountMap[0]; } set { mTotalCountMap[0] = value; } } 

    void Awake()
    {
        mScriptEnable = enabled;
        if (GetComponent<Collider>() != null)
            mColliderEnable = GetComponent<Collider>().enabled;

        mSpawnPoints = GetComponentsInChildren<SpawnPoint>();
        foreach (SpawnPoint spawnPoint in mSpawnPoints)
        {
            TotalCount += spawnPoint.TotalSpawn;
            mTotalCountMap[spawnPoint.SpawnList[0].PoolIndex] += spawnPoint.TotalSpawn;
        }

        UnitInfo[] infos = GetComponentsInChildren<UnitInfo>(true);
        TotalCount += infos.Length;

        mTrigger = ifLevelLoaedEnable;
        Global.TotalMonster += TotalCount;
    }

    void ExecuteOp(ESpawnOp op)
    {
        mTriggerOp = op;
        foreach (SpawnPoint spawnPoint in mSpawnPoints)
        {
            switch (op)
            {
            case ESpawnOp.SpawnStated: spawnPoint.SpawnStated(); break;
            case ESpawnOp.SpawnPaused: spawnPoint.SpawnPaused(); break;
            case ESpawnOp.SpawnResumed: spawnPoint.SpawnResumed(); break;
            case ESpawnOp.SpawnReset: spawnPoint.SpawnReset(); break;
            }
        }
    }

    bool CheckCondition(Detection detection, int type)
    {
        if (type == 0)
        {
            if (detection.AllDie || detection.unitDieCount >= 0)
			{
				int dieCount = mDieCountMap[detection.monsterType];
           		int maxCheck = detection.AllDie ? mTotalCountMap[detection.monsterType] : detection.unitDieCount;
				
				 return dieCount >= maxCheck;
			}
        }
        else
        {
            return mLifeTime >= detection.passTime;
        }
        return false;
    }

    void DetectionUpdate()
    {
        if (!mTrigger)
        {
            mAllDetectionOver = false;
            return;
        }

        mLifeTime += Time.deltaTime;

        if (TotalDie != TotalCount) 
            mAllDetectionOver = false;

        foreach (Detection detection in detectionList)
        {
            if (detection.detectionOver) 
                continue;

            mAllDetectionOver = false;
            if (!detection.executioned)
            {
                detection.executioned = CheckCondition(detection, 0);
                if (detection.twoAnd)
                    detection.executioned = detection.executioned && CheckCondition(detection, 1);
                else
                    detection.executioned = detection.executioned || CheckCondition(detection, 1);
            }
            else
            {
                detection.excutionTime += Time.deltaTime;
                detection.detectionOver = true;
                foreach (TriggerExecution item in detection.executionExList)
                {
                    //zhixing shi fou wanbi 
                    if (item.executionOver) continue;
                    detection.detectionOver = false;
                    if (item.delayTime <= detection.excutionTime)
                    {
                        //execution
                        item.executionOver = true;
                        item.execution(this);
                    }
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        mAllDetectionOver = true;
        DetectionUpdate();

        if (mAllDetectionOver)
        {
            Destroy(gameObject);

            if (TargetGameCell != null)
                TargetGameCell.OnFinish();
        }
        else if (waveCount > 0)
        {
            int liveCount = mSpawnCount - TotalDie;
            if (liveCount >= waveCount)
                ExecuteOp(ESpawnOp.SpawnPaused);
            else if (mTriggerOp == ESpawnOp.SpawnPaused && liveCount <= waveCheck)
                ExecuteOp(ESpawnOp.SpawnResumed);
        }
    }

    public void WaveControl(int _waveCount, int _waveCheck)
    {
        waveCount = _waveCount;
        waveCheck = _waveCheck;
        if (waveCount == 0)
        {
            // (waveCount == 0) means it should be paused.
            if (mTriggerOp != ESpawnOp.SpawnPaused)
                ExecuteOp(ESpawnOp.SpawnPaused);
        }
        else if (mTriggerOp == ESpawnOp.SpawnPaused)
            ExecuteOp(ESpawnOp.SpawnResumed);
    }

    void OnTriggerStay(Collider collider)
    {
        if (collider != null && !mTrigger && enabled && UnitManager.Instance.LocalPlayer != null 
			&& collider.gameObject.GetComponent<UnitInfo>() != null 
			&& collider.gameObject.GetComponent<UnitInfo>().UnitType == EUnitType.EUT_LocalPlayer)
        {
            OnExecution(true);
        }
    }

    void OnTriggerEnter(Collider collider)
    {
    }

    public void OnExecution(bool trigger)
    {
        enabled = trigger;
        mTrigger = trigger;
        if (trigger && TotalCount > 0 && gameObject.transform.childCount > 0)
            ExecuteOp(ESpawnOp.SpawnStated);

        if (TargetGameCell != null)
            TargetGameCell.OnBegin();
    }

    void UnitSpawned(object arg)
    {
        mSpawnCount++;
    }

    void UnitDie(object arg)
    {
        Unit unit = arg as Unit;
        if (unit != null && 
            unit.UUnitInfo && 
            unit.UUnitInfo.PoolIndex > 0)
            mDieCountMap[unit.UUnitInfo.PoolIndex]++;
        TotalDie++;
    }

    void DetectionReset()
    {
        mLifeTime = 0;
        mTrigger = false;
        enabled = false;

        for (int i = 0; i < mDieCountMap.Length; i++)
            mDieCountMap[i] = 0;

        foreach (Detection item in detectionList)
        {
            item.excutionTime = 0;
            item.detectionOver = false;
            item.executioned = false;
            foreach (TriggerExecution ee in item.executionExList)
                ee.executionOver = false;
        }

        ExecuteOp(ESpawnOp.SpawnReset);
    }

    public void TriggerReset()
    {
        DetectionReset();
        enabled = mScriptEnable;
        if (GetComponent<Collider>() != null)
            GetComponent<Collider>().enabled = mColliderEnable;
    }

    public void SpawnFinish(int dieNowType)
    {
        foreach (Unit unit in UnitManager.Instance.UnitInfos)
        {
            if (unit.UnitType != EUnitType.EUT_Monster ||
                unit.UUnitInfo.PoolIndex >= dieNowType)
                continue;
            unit.AddHp(-unit.GetAttrib(EPA.CurHP));
        }

        foreach (SpawnPoint spawnPoint in mSpawnPoints)
        {
            int poolType = spawnPoint.SpawnList[0].PoolIndex;
            if (poolType >= dieNowType)
                continue;

            int left = spawnPoint.SpawnFinish();
            if (left > 0)
            {
                mDieCountMap[poolType] += left;
                mSpawnCount += left;
                TotalDie += left;
            }
        }
    }
}
