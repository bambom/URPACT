using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class GameLevel : MonoBehaviour
{
    // public field should be serialized.
    public GameCell[] Cells;
    public GameObjectPool[] MonsterPool;
    public GameObject StartBridge;
    public GameObject EndBridge;

    [HideInInspector]
    public ObjectPool SoulBallPool;

    [HideInInspector]
    public GameCell CurrentCell;

    public SmartInt TotalExpWeight;
    public SmartInt TotalGoldWeight;
    public SmartInt TotalSoulWeight;
    public SmartInt CurScore;
    public SmartInt CurExp;
    public SmartInt CurGold;
    public SmartInt CurSoul;
    public SmartInt CurSP;
	public float CellWidth;
	public float CellLength;
    public Dictionary<int, int> DropInfo = new Dictionary<int, int>();

    static GameLevel msInstance;
    public static GameLevel Instance { get { return msInstance; } }

    GameLevel()
    {
        msInstance = this;
    }

    void Awake()
    {
        TotalExpWeight = 0;
        TotalGoldWeight = 0;
        TotalSoulWeight = 0;
        CurScore = 100;
        CurExp = 0;
        CurGold = 0;
        CurSoul = 0;
        CurSP = 0;
    }

    void Start()
    {
        // create the soul ball pool.
        SoulBallPool = gameObject.GetComponent<ObjectPool>();
        if (!SoulBallPool)
            SoulBallPool = gameObject.AddComponent<ObjectPool>();
        SoulBallPool.Prefab = Resources.Load("SoulBall") as GameObject;

        GameObject standCell = GameObject.Find("StandardCell");
        if (standCell != null)
            standCell.SetActive(false);

        Cells[0].OpenEnterDoor();

        LocalPlayer localPlayer = UnitManager.Instance.LocalPlayer;
        if (localPlayer != null)
        {
            localPlayer.SetPosition(StartBridge.transform.position);
            localPlayer.SetOrientation(StartBridge.transform.rotation.eulerAngles.y * Mathf.Deg2Rad + 90);
        }

        if (MainScript.Instance != null)
            FightMainWnd.Instance.Open();

        msInstance = this;
    }

    void OnDestroy()
    {
        // clear the level data.
        Global.GLevelData = null;
        msInstance = null;
    }

    public void OnBegin(GameCell cell)
    {
        CurrentCell = cell;

        if (FightMainWnd.Exist)
            FightMainWnd.Instance.EnterCell(cell);
        Invoke("HideLevelMap", 0.1f);
    }

    public void OnFinish(GameCell finishCell)
    {
        foreach (GameCell cell in Cells)
        {
            if (cell.Parent == finishCell)
            {
                cell.OpenEnterDoor();
                if (FightMainWnd.Exist)
                    FightMainWnd.Instance.ShowAroundMapCell(cell);
            }
        }

        if (FightMainWnd.Exist)
            FightMainWnd.Instance.FinishCell(finishCell);
    }

    public void LevelFinish()
    {
        if (FightMainWnd.Exist)
            FightMainWnd.Instance.Close();

        if (!LevelFinishWnd.Exist){
            LevelFinishWnd.Instance.Open();
			SoundManager.Instance.PlaySound("UI_FuBenTongGuan");
		}
    }

    public void ExitLevel()
    {
        // request exist the level request.
        StartCoroutine(MainScript.Execute(new LeaveLevelCmd(), delegate(string err, Response response)
        {
            Invoke("BackTOMainTown", 1.0f);
        }));
    }

    public void PickUpItem(int itemID, int number)
    {
        int tryNum;
        if (DropInfo.TryGetValue(itemID, out tryNum))
            DropInfo[itemID] += number;
        else
            DropInfo.Add(itemID, number);
    }

    void BackTOMainTown()
    {
        if (FightMainWnd.Exist)
            FightMainWnd.Instance.Close();

        SceneList sceneListItem = SceneListManager.Instance.GetItem((int)ESceneID.Main, (int)ESceneType.Main);

        Global.CurSceneInfo.ID = ESceneID.Main;
        Global.CurSceneInfo.Type = ESceneType.Main;
        Global.CurSceneInfo.Name = sceneListItem.Name;
        MainScript.Instance.LoadLevel(Global.CurSceneInfo);
    }

    void HideLevelMap()
    {
        if (FightMainWnd.Exist)
            FightMainWnd.Instance.HideLevelMap();
    }

    public void SpawnSoulBall(Vector3 pos, int soul)
    {
        GameObject soulBall = SoulBallPool.Spawn(true);
        soulBall.GetComponent<SoulBall>().Begin(pos, soul, delegate(SoulBall ball)
        {
            SoulBallPool.Recycle(ball.gameObject);
        });
    }

    public void OnPlayerDead()
    {
        if (!RoleDeadWnd.Exist)
            RoleDeadWnd.Instance.Open();
    }

    public void OnReconnect()
    {
        foreach (GameCell cell in Cells)
        {
            if (cell.DoorOpenFailed)
                cell.OpenEnterDoor();
        }
    }
}
