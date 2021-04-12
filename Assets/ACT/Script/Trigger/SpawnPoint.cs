using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class SpawnInfo
{
    public int PoolIndex;
    public string StartAction = "N0000";
    public int Diff = 0;
    public int AIStatus = 0;
    public int Ratio = 10;
};

public class SpawnPoint : MonoBehaviour
{
    public float Delay = 0.0f;
    public float LoopTime = 1;
    public int TotalSpawn = 1;
    public SpawnInfo[] SpawnList;
	public DropItemInfo[] DropInfo;
    public GameLevel Level;

    float mLifeTime = 0;
    float mLoopTimeLeft = 0;
    int mSpawned = 0;
    int mTotalRatio = 0;
    // Use this for initialization
    void Start()
    {
        enabled = false;
        foreach (SpawnInfo info in SpawnList)
            mTotalRatio += info.Ratio;
    }

    void Update()
    {
        mLifeTime += Time.deltaTime;
        if (mLifeTime < Delay)
            return;

        mLoopTimeLeft -= Time.deltaTime;
        if (mLoopTimeLeft <= 0)
            Spawn();
    }

    public void SpawnStated()
    {
        if (SpawnList.Length > 0)
            enabled = true;
        mLifeTime = 0.0f;
    }

    public void SpawnPaused()
    {
        enabled = false;
    }

    public void SpawnResumed()
    {
        enabled = true;
    }

    public void SpawnReset()
    {
        enabled = false;
        mLifeTime = 0;
        mLoopTimeLeft = 0;
        mSpawned = 0;
    }

    public int SpawnFinish()
    {
        int left = TotalSpawn - mSpawned;
        mSpawned = TotalSpawn;
        return left;
    }

    void Spawn()
    {
        if (mSpawned >= TotalSpawn)
        {
            enabled = false;
            return;
        }
        // create pawn.

        SpawnInfo chooseInfo = null;
        int ratio = Random.Range(0, mTotalRatio);
        foreach (SpawnInfo info in SpawnList)
        {
            chooseInfo = info;
            ratio -= info.Ratio;
            if (ratio <= 0)
                break;
        }
		List<DropItemInfo> dropInfo = new List<DropItemInfo>();;
		if(DropInfo != null)
		{
			if(chooseInfo.PoolIndex == 4 || chooseInfo.PoolIndex == 3)
				dropInfo.AddRange(DropInfo);
			else
			{
				DropItemInfo info = mSpawned < DropInfo.Length ? DropInfo[mSpawned] : null;
				dropInfo.Add(info);
			}
		}
        if (Spawn(chooseInfo, dropInfo))
        {
            mLoopTimeLeft = LoopTime;
            mSpawned++;
        }
    }

    bool Spawn(SpawnInfo info, List<DropItemInfo> dropInfo)
    {
        int poolIndex = info.PoolIndex - 1;
        if (poolIndex >= Level.MonsterPool.Length || Level.MonsterPool[poolIndex] == null)
            return false;

        GameObject go = Level.MonsterPool[poolIndex].Spawn(transform.position, transform.rotation);
        if (!go) 
            return false;

        UnitInfo unitinfo = go.GetComponent<UnitInfo>();
        if (unitinfo != null)
        {
            unitinfo.PoolIndex = info.PoolIndex;
            unitinfo.triggerCell = transform.parent.gameObject;

            unitinfo.Unit.Init();

            unitinfo.Unit.PlayAction(info.StartAction);
			unitinfo.DropInfo = dropInfo;
            AIListener listener = unitinfo.Unit.ActionStatus.Listener as AIListener;
            if (listener != null)
            {
                if (info.Diff != 0)
                    listener.changeAIDiff(info.Diff);

                if (info.AIStatus != 0)
                    listener.changeActionStatus(info.AIStatus);
            }
        }

        return true;
    }
}
