using System;
using System.Collections.Generic;
using UnityEngine;
using LitJson;

public class LevelBuilder
{
    struct Direction
    {
        public int X;
        public int Y;
        public Direction(int x, int y)
        {
            X = x;
            Y = y;
        }
    };

    struct CellInfo
    {
        public GameObject Object;
        public int Rotate;
        public CellInfo(Transform transform, int rotate)
        {
            Object = transform.gameObject;
            Rotate = rotate;
        }
    };

    GameLevel mLevel;
    GameObject mCells;
    GameObject mBridges;
    Transform mStandardCell;
    LevelBuildInfo mBuildInfo;
    GameCellInfo[] mCellInfos;
    List<KeyValuePair<Transform, float>> mBridgeInfos = new List<KeyValuePair<Transform, float>>();
    System.Random mRandom = new System.Random();
    UnityEngine.Object[][] mTriggerResources;
    Direction[] mDirection = new Direction[] { 
        new Direction(0, 1),
        new Direction(0, -1),
        new Direction(1, 0),
        new Direction(-1, 0),
    }; // 4 dirs.

    public void Build(GameLevel level, LevelBuildInfo info)
    {
        Debug.Log(string.Format("BuildLevel: WayLength={0} ExtraWay={1} GeniusNum={2} ExtraNum={3}", 
            info.WayLength, 
            info.ExtraWay, 
            info.GeniusNum, 
            info.ExtraNum));

        mLevel = level;
        mBuildInfo = info;

        Init();

        // build the monster pool.
        BuildMonsterPool();
		
		BuildDropPool();

        // build the trunck path.
        BuildPath(mCellInfos[0], 1, mBuildInfo.WayLength + 2);

        // build the extra branch.
        BuildExtra();

        // assign the cell type. default is Normal.
        BuildCellType();

        // build the scene
        BuildScene();

        // finally build the cells.
        BuildCells();

        // build the chests
        BuildChests();
    }

    UnityEngine.Object[] LoadTrigger(string triggerArray)
    {
		if (string.IsNullOrEmpty(triggerArray))
			return null;
		
        string[] triggers = triggerArray.Split('|');
        UnityEngine.Object[] ret = new UnityEngine.Object[triggers.Length];
        for (int i = 0; i < triggers.Length; i++)
        {
            ret[i] = Resources.Load(triggers[i]);
            if (ret[i] == null)
                Debug.LogError(string.Format("Fail to load trigger {0} in {1}", triggers[i], triggerArray));
        }
        return ret;
    }

    void Init()
    {
        mStandardCell = GameObject.Find(mBuildInfo.StandardCell).transform;
        mTriggerResources = new UnityEngine.Object[][] {
            LoadTrigger(mBuildInfo.NormalTrigger),
            LoadTrigger(mBuildInfo.GeniusTrigger),
            LoadTrigger(mBuildInfo.BossTrigger),
            LoadTrigger(mBuildInfo.ExtraTrigger),
        };

        mCellInfos = new GameCellInfo[mBuildInfo.WayLength + 2 + mBuildInfo.ExtraWay];
        mCellInfos[0] = new GameCellInfo(mLevel, null, 0, 0);

        mCells = new GameObject("Cells");
        Attach(mLevel.gameObject, mCells);

        mBridges = new GameObject("Bridges");
        Attach(mLevel.gameObject, mBridges);

        // setup the cell width & height.
        Transform cellTransform = mStandardCell.Find("Cell4/BaseCollider/DownCube");
        if (cellTransform)
        {
            mBuildInfo.CellWidth = cellTransform.lossyScale.x;
            mBuildInfo.CellLength = cellTransform.lossyScale.z;
        }
		mLevel.CellWidth = mBuildInfo.CellWidth;
		mLevel.CellLength = mBuildInfo.CellLength;
        BuildBridgeInfos();
    }

    void BuildBridgeInfos()
    {
        // check the bridge length.
        Transform bridgeLength = mStandardCell.Find("Bridge/BaseCollider/Length");
        if (bridgeLength)
            mBuildInfo.BridgeWidth = bridgeLength.lossyScale.x;

        // the base bridge.
        BuildBridgeInfo(mBuildInfo.Bridge);

        int i = 1;
        while (BuildBridgeInfo(mBuildInfo.Bridge + i)) 
            i++;
    }

    bool BuildBridgeInfo(string name)
    {
        Transform bridge = mStandardCell.Find(name);
        if (!bridge)
            return false;

        // setup the bridge height.
        float bridgeHeight = 0;
        Transform child = bridge.Find("BaseCollider/DownCube");
        if (child != null)
        {
            float rand = child.transform.rotation.eulerAngles.z * Mathf.Deg2Rad;
            bridgeHeight = child.lossyScale.x * Mathf.Sin(rand);
        }

        mBridgeInfos.Add(new KeyValuePair<Transform,float>(bridge, bridgeHeight));
        return true;
    }

    void Attach(GameObject parent, GameObject child)
    {
        child.transform.parent = parent.transform;
        child.transform.localPosition = Vector3.zero;
        child.transform.localRotation = Quaternion.identity;
    }

    GameCellInfo GetCell(int x, int y, int length)
    {
        for (int i = 0; i < length; i++)
        {
            if (mCellInfos[i].X == x && mCellInfos[i].Y == y)
                return mCellInfos[i];
        }
        return null;
    }

    void BuildMonsterPool()
    {
        GameObject poolsObj = new GameObject("Pools");
        Attach(mLevel.gameObject, poolsObj);

        GameObjectPool[] pools = 
        {
            poolsObj.AddComponent<GameObjectPool>(),
            poolsObj.AddComponent<GameObjectPool>(),
            poolsObj.AddComponent<GameObjectPool>(),
            poolsObj.AddComponent<GameObjectPool>(),
            poolsObj.AddComponent<GameObjectPool>()
        };
        pools[0].SetUp(mBuildInfo.MonsterNormal1, mBuildInfo.M1level, mBuildInfo.Pool1Num);
        pools[1].SetUp(mBuildInfo.MonsterNormal2, mBuildInfo.M2level, mBuildInfo.Pool2Num);
        pools[2].SetUp(mBuildInfo.MonsterGenius, mBuildInfo.M3level, mBuildInfo.Pool3Num);
        pools[3].SetUp(mBuildInfo.MonsterBoss, mBuildInfo.M4level, mBuildInfo.Pool4Num);
        pools[4].SetUp(mBuildInfo.MonsterExtra, mBuildInfo.M5level, mBuildInfo.Pool5Num);
        mLevel.MonsterPool = pools;
    }

    bool BuildPath(GameCellInfo parent, int curLength, int targetLength)
    {
        if (curLength == targetLength)
            return true;

        // random path direction.
        int idx = mRandom.Next(1, 4);
        Direction tmp = mDirection[0];
        mDirection[0] = mDirection[idx];
        mDirection[idx] = tmp;

        // find a empty path to go.
        foreach (Direction dir in mDirection)
        {
            if (GetCell(parent.X + dir.X, parent.Y + dir.Y, curLength) == null)
            {
                mCellInfos[curLength] = new GameCellInfo(mLevel, parent, parent.X + dir.X, parent.Y + dir.Y);
                if (BuildPath(mCellInfos[curLength], curLength + 1, targetLength))
                    return true;
            }
        }

        // noway to go.
        return false;
    }

    void BuildExtra()
    {
        int leftLength = mBuildInfo.ExtraWay;
        int branchLeng = Math.Min(mRandom.Next(1, Math.Max(mBuildInfo.ExtraWay / 2, 2)), leftLength);
        int lastBranchLeng = 0;
        while (leftLength > 0)
        {
            bool pathFind = false;
            for (int i = 0; !pathFind && i < 10; i++) // try 10 times.
            {
                // random choose a cell from the trunck (boss ignore).
                int branchIdx = mRandom.Next(1, mBuildInfo.WayLength);
                int curLength = mBuildInfo.WayLength + 2 + mBuildInfo.ExtraWay - leftLength;
                GameCellInfo branchCell = mCellInfos[branchIdx];

                // here we found a path can be put in the trunck.
                pathFind = BuildPath(branchCell, curLength, curLength + branchLeng);
            }

            if (pathFind)
            {
                leftLength -= branchLeng;
                lastBranchLeng = branchLeng;
                branchLeng = Math.Min(mRandom.Next(1, Math.Max(mBuildInfo.ExtraWay / 2, 2)), leftLength);
            }
            else
            {
                Debug.Log("PathFind Failed:" + branchLeng);
                leftLength += lastBranchLeng;
                branchLeng += lastBranchLeng;
            }
        }
    }

    void BuildCellType()
    {
        // setup cells type.
        for (int i = 0; i < mCellInfos.Length; i++)
        {
            if (i == 0)
                mCellInfos[i].CellType = ECellType.None;
            else if (i < mBuildInfo.WayLength)
                mCellInfos[i].CellType = ECellType.Normal;
            else if (i == mBuildInfo.WayLength)
                mCellInfos[i].CellType = ECellType.Boss;
            else if (i == mBuildInfo.WayLength + 1)
                mCellInfos[i].CellType = ECellType.None;
            else
                mCellInfos[i].CellType = ECellType.Extra;
        }

        // the genius cells.
        if (mBuildInfo.WayLength - 1 > mBuildInfo.GeniusNum)
        {
            int geniusLeft = mBuildInfo.GeniusNum;
            while (geniusLeft > 0)
            {
                // random choose from the trunck.
                GameCellInfo cell = mCellInfos[mRandom.Next(1, mBuildInfo.WayLength)];
                if (cell.CellType != ECellType.Genius)
                {
                    // now turn it to be a genius.
                    cell.CellType = ECellType.Genius;
                    geniusLeft--;
                }
            }
        }
        else
        {
            // all the cell is genius cell.
            for (int i = 1; i < mBuildInfo.WayLength; i++)
                mCellInfos[i].CellType = ECellType.Genius;
        }
		
		// the extra cells.
		if (mBuildInfo.ExtraWay > mBuildInfo.ExtraNum)
		{
			int normalLeft = mBuildInfo.ExtraWay - mBuildInfo.ExtraNum;
			while (normalLeft > 0)
			{
                GameCellInfo cell = mCellInfos[mRandom.Next(mBuildInfo.WayLength + 2, mCellInfos.Length)];
                if (cell.CellType != ECellType.Normal)
                {
                    cell.CellType = ECellType.Normal;
                    normalLeft--;
                }
			}
		}
    }

    void BuildScene()
    {
        BuildBridgeObject();

        BuildDoorMask();
        
        BuildCellObject();
    }

    void BuildDoorMask()
    {
        // build the door mask.  [up | down | left | right]
        foreach (GameCellInfo cell in mCellInfos)
        {
            if (cell.Parent == null)
                continue;

            if (cell.Parent.Y + 1 == cell.Y)
            {
                cell.DoorMask |= GameCellInfo.EDoorFlag.Up;
                cell.Parent.DoorMask |= GameCellInfo.EDoorFlag.Down;
            }
            else if (cell.Parent.Y - 1 == cell.Y)
            {
                cell.DoorMask |= GameCellInfo.EDoorFlag.Down;
                cell.Parent.DoorMask |= GameCellInfo.EDoorFlag.Up;
            }
            else if (cell.Parent.X + 1 == cell.X)
            {
                cell.DoorMask |= GameCellInfo.EDoorFlag.Left;
                cell.Parent.DoorMask |= GameCellInfo.EDoorFlag.Right;
            }
            else if (cell.Parent.X - 1 == cell.X)
            {
                cell.DoorMask |= GameCellInfo.EDoorFlag.Right;
                cell.Parent.DoorMask |= GameCellInfo.EDoorFlag.Left;
            }
        }
    }

    Vector3 GetCellPosition(GameCellInfo cell)
    {
        return new Vector3(
            (mBuildInfo.CellWidth + mBuildInfo.BridgeWidth) * cell.X, cell.Height,
            (mBuildInfo.CellLength + mBuildInfo.BridgeWidth) * cell.Y);
    }

    void BuildCellObject()
    {
        // the door mask dictionary.
        Dictionary<GameCellInfo.EDoorFlag, CellInfo> doorDict = new Dictionary<GameCellInfo.EDoorFlag, CellInfo>();
        doorDict[GameCellInfo.EDoorFlag.Up] = new CellInfo(mStandardCell.Find(mBuildInfo.Cell1U_D), 0);
        doorDict[GameCellInfo.EDoorFlag.Down] = new CellInfo(mStandardCell.Find(mBuildInfo.Cell1U_D), 180);
        doorDict[GameCellInfo.EDoorFlag.Left] = new CellInfo(mStandardCell.Find(mBuildInfo.Cell1L_R), 0);
        doorDict[GameCellInfo.EDoorFlag.Right] = new CellInfo(mStandardCell.Find(mBuildInfo.Cell1L_R), 180);
        doorDict[GameCellInfo.EDoorFlag.UD] = new CellInfo(mStandardCell.Find(mBuildInfo.Cell2UD), 0);
        doorDict[GameCellInfo.EDoorFlag.LR] = new CellInfo(mStandardCell.Find(mBuildInfo.Cell2LR), 0);
        doorDict[GameCellInfo.EDoorFlag.UL] = new CellInfo(mStandardCell.Find(mBuildInfo.Cell2LD_RU), 0);
        doorDict[GameCellInfo.EDoorFlag.UR] = new CellInfo(mStandardCell.Find(mBuildInfo.Cell2LU_RD), 0);
        doorDict[GameCellInfo.EDoorFlag.DL] = new CellInfo(mStandardCell.Find(mBuildInfo.Cell2LU_RD), 180);
        doorDict[GameCellInfo.EDoorFlag.DR] = new CellInfo(mStandardCell.Find(mBuildInfo.Cell2LD_RU), 180);
        doorDict[GameCellInfo.EDoorFlag.UDL] = new CellInfo(mStandardCell.Find(mBuildInfo.Cell3UD), 180);
        doorDict[GameCellInfo.EDoorFlag.UDR] = new CellInfo(mStandardCell.Find(mBuildInfo.Cell3UD), 0);
        doorDict[GameCellInfo.EDoorFlag.LRU] = new CellInfo(mStandardCell.Find(mBuildInfo.Cell3LR), 0);
        doorDict[GameCellInfo.EDoorFlag.LRD] = new CellInfo(mStandardCell.Find(mBuildInfo.Cell3LR), 180);
        doorDict[GameCellInfo.EDoorFlag.UDLR] = new CellInfo(mStandardCell.Find(mBuildInfo.Cell4), 0);

        // build the cell base 
        foreach (GameCellInfo cell in mCellInfos)
        {
            // we should ignore the start & end cell
            if (cell.CellType == ECellType.None)
                continue;

            CellInfo cellInfo = doorDict[cell.DoorMask];
            Vector3 position = GetCellPosition(cell);
            Quaternion rotate = Quaternion.identity;
            rotate.eulerAngles = new Vector3(0, cellInfo.Rotate, 0);

            // build the cell gameobject.
            cell.CellObject = GameObject.Instantiate(cellInfo.Object, position, rotate) as GameObject;

            // build the door gameobjects.
            Transform[] doors = new Transform[] { 
                cell.CellObject.transform.Find(mBuildInfo.DoorU),
                cell.CellObject.transform.Find(mBuildInfo.DoorD),
                cell.CellObject.transform.Find(mBuildInfo.DoorL),
                cell.CellObject.transform.Find(mBuildInfo.DoorR) };
            cell.SetDoors(doors);

            // build the trigger for this cell.
            UnityEngine.Object[] triggerRes = null;
            switch (cell.CellType)
            {
                case ECellType.Normal:
                    triggerRes = mTriggerResources[0];
                    break;
                case ECellType.Genius:
                    triggerRes = mTriggerResources[1];
                    break;
                case ECellType.Boss:
                    triggerRes = mTriggerResources[2];
                    break;
                case ECellType.Extra:
                    triggerRes = mTriggerResources[3];
                    break;
            }

            if (triggerRes != null)
            {
                // load the trigger and bind under the cell object.
                UnityEngine.Object triggerObj = triggerRes[mRandom.Next(0, triggerRes.Length)];
                cell.Trigger = GameObject.Instantiate(triggerObj) as GameObject;
				if (cell.Trigger == null)
					Debug.LogError("Trigger is null: " + cell.CellType);

                // get the monster number.
                SpawnPoint[] spawnPoints = cell.Trigger.GetComponentsInChildren<SpawnPoint>();
                foreach (SpawnPoint spawnPoint in spawnPoints)
                {
                    if (spawnPoint.SpawnList.Length == 0)
                        continue;

                    SpawnInfo spawnInfo = spawnPoint.SpawnList[0];
                    int poolIndex = spawnInfo.PoolIndex - 1;
                    if (poolIndex >= mLevel.MonsterPool.Length ||
                        mLevel.MonsterPool[poolIndex].MonsterAttrib == null)
                        continue;

                    GameObjectPool objPool = mLevel.MonsterPool[poolIndex];
                    MonsterAttrib monsterAttrib = objPool.MonsterAttrib;
                    mLevel.TotalExpWeight += monsterAttrib.ExpWeight * spawnPoint.TotalSpawn;
                    mLevel.TotalGoldWeight += monsterAttrib.GoldWeight * spawnPoint.TotalSpawn;
                    mLevel.TotalSoulWeight += monsterAttrib.SoulWeight * spawnPoint.TotalSpawn;
                }

                Attach(cell.CellObject, cell.Trigger);
            }
            else
                Debug.LogError("Trigger is null: " + cell.CellType);
        }
    }

    void BuildBridgeObject()
    {
        // build the cell base 
        foreach (GameCellInfo cell in mCellInfos)
        {
            if (cell.Parent == null)
                continue;

            Vector3 bridgePos = GetCellPosition(cell);
            Quaternion bridegRot = Quaternion.identity;
            if (cell.Parent.Y + 1 == cell.Y)
            {
                bridgePos.z -= (mBuildInfo.CellLength + mBuildInfo.BridgeWidth) * 0.5f;
                bridegRot.eulerAngles = new Vector3(0, -90, 0);
            }
            else if (cell.Parent.Y - 1 == cell.Y)
            {
                bridgePos.z += (mBuildInfo.CellLength + mBuildInfo.BridgeWidth) * 0.5f;
                bridegRot.eulerAngles = new Vector3(0, 90, 0);
            }
            else if (cell.Parent.X + 1 == cell.X)
            {
                bridgePos.x -= (mBuildInfo.CellWidth + mBuildInfo.BridgeWidth) * 0.5f;
            }
            else if (cell.Parent.X - 1 == cell.X)
            {
                bridgePos.x += (mBuildInfo.CellWidth + mBuildInfo.BridgeWidth) * 0.5f;
                bridegRot.eulerAngles = new Vector3(0, 180, 0);
            }

            // this is a start bridge.
            Transform bridgeBase = null;
            if (cell.CellType == ECellType.None ||
                cell.Parent.CellType == ECellType.None)
            {
                if (cell.Parent.CellType == ECellType.None)
                    bridgeBase = mStandardCell.Find(mBuildInfo.BridgeStart);
                else
                {
                    bridgeBase = mStandardCell.Find(mBuildInfo.BridgeEnd);
                    bridgePos.y = cell.Parent.Height;
                    bridegRot.eulerAngles = new Vector3(0, bridegRot.eulerAngles.y + 180, 0);
                }
            }
            else
            {
                KeyValuePair<Transform, float> bridgeInfo = mBridgeInfos[UnityEngine.Random.Range(0, mBridgeInfos.Count)];
                bridgeBase = bridgeInfo.Key;
                float height = bridgeInfo.Value;
                float parentHeight = cell.Parent.Height;
                float maxDepth = -height * 3;
                if (parentHeight > maxDepth && UnityEngine.Random.Range(0, 2) > 0)
                {
                    cell.Height = parentHeight + height;
                    bridgePos.y = cell.Height - height * 0.5f;
                }
                else
                {
                    cell.Height = parentHeight - height;
                    bridgePos.y = cell.Height + height * 0.5f;
                    bridegRot.eulerAngles = new Vector3(0, bridegRot.eulerAngles.y + 180, 0);
                }
            }

            cell.BridgeObject = GameObject.Instantiate(bridgeBase.gameObject, bridgePos, bridegRot) as GameObject;

			if (bridgeBase.name == mBuildInfo.BridgeEnd)
            {
                LevelOver over = cell.BridgeObject.GetComponentInChildren<LevelOver>();
                if (over != null)
                    over.Level = mLevel;
				mLevel.EndBridge = cell.BridgeObject;
			}
			else if (bridgeBase.name == mBuildInfo.BridgeStart)
				mLevel.StartBridge = cell.BridgeObject;
        }
    }

    void BuildCells()
    {
        List<GameCell> cells = new List<GameCell>();
        foreach (GameCellInfo cellInfo in mCellInfos)
        {
            if (cellInfo.BridgeObject != null)
                cellInfo.BridgeObject.transform.parent = mBridges.transform;

            if (cellInfo.CellObject != null)
            {
                // create the game cell object and build the hierarchy.
                cellInfo.CellObject.transform.parent = mCells.transform;

                // build the game cell.
                GameCell gameCell = cellInfo.CellObject.AddComponent<GameCell>();
                cellInfo.Cell = gameCell;
                gameCell.Level = mLevel;
                gameCell.Parent = cellInfo.Parent.Cell;
                gameCell.Doors = cellInfo.Doors;
                gameCell.EnterDoor = cellInfo.EnterDoor;
                gameCell.CellType = cellInfo.CellType;
                gameCell.Trigger = cellInfo.Trigger;
				gameCell.CellInfo = cellInfo;

                cells.Add(gameCell);

                // link the triggers.
                TriggerCellEx trigger = cellInfo.Trigger.GetComponent<TriggerCellEx>();
                trigger.TargetGameCell = gameCell;

                SpawnPoint[] spawnPoints = cellInfo.Trigger.GetComponentsInChildren<SpawnPoint>();
                foreach (SpawnPoint spawnPoint in spawnPoints)
                    spawnPoint.Level = mLevel;
            }
        }

        mLevel.Cells = cells.ToArray();
    }
	
	void BuildDropPool()
	{
		GameObject obj = new GameObject("DropPool");
		Attach(mLevel.gameObject, obj);
		obj.AddComponent<ItemPool>().InitCapacity = 5;
	}

    void BuildChests()
    {
        UnitBase unitBase = UnitBaseManager.Instance.GetItem(mBuildInfo.ChestID);
        if (unitBase == null)
            return;

        GameObject chestPrefab = Resources.Load(unitBase.Prefab) as GameObject;
        if (chestPrefab == null)
        {
            Debug.LogError("Fail to load chest prefab: " + unitBase.Prefab);
            return;
        }

        for (int leftChestNum = mBuildInfo.ChestNum, round = 0; leftChestNum > 0; round++)
        {
            List<GameCellInfo> chooseCells = new List<GameCellInfo>();
            for (int i = 1; i < mBuildInfo.WayLength; i++)
                chooseCells.Add(mCellInfos[i]);

            int assignNum = Mathf.Min(chooseCells.Count, leftChestNum);
            while (assignNum-- > 0)
            {
                int cell = UnityEngine.Random.Range(0, chooseCells.Count);
                GameCellInfo cellInfo = chooseCells[cell];
                chooseCells.RemoveAt(cell);

                SpawnPoint[] spawnPoints = cellInfo.Trigger.GetComponentsInChildren<SpawnPoint>();
                SpawnPoint spawnPoint = spawnPoints[round];
                GameObject chestInstance = GameObject.Instantiate(
                    chestPrefab,
                    spawnPoint.transform.position,
                    spawnPoint.transform.rotation) as GameObject;

                leftChestNum--;

                UnitInfo unitInfo = chestInstance.GetComponent<UnitInfo>();
                if (unitInfo &&
                    Global.GLevelData != null &&
                    Global.GLevelData.Profit.ChestDrops != null &&
                    leftChestNum < Global.GLevelData.Profit.ChestDrops.Length)
                {
                    DropInfo dropInfo = Global.GLevelData.Profit.ChestDrops[leftChestNum];
                    foreach (KeyValuePair<string, int> drops in dropInfo.drops)
                        unitInfo.DropInfo.Add(new DropItemInfo(int.Parse(drops.Key), drops.Value));
                }
            }
        }
    }
}
