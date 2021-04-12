using System;
using UnityEngine;


public class GameCellInfo
{
    [Flags]
    public enum EDoorFlag
    {
        None = 0,
        Up = 1,
        Down = 2,
        Left = 4,
        Right = 8,

        // 2 combines
        UD = Up | Down,
        LR = Left | Right,
        UL = Up | Left,
        UR = Up | Right,
        DL = Down | Left,
        DR = Down | Right,

        // 3 combines
        UDL = Up | Down | Left,
        UDR = Up | Down | Right,
        LRU = Left | Right | Up,
        LRD = Left | Right | Down,

        // 4 combines
        UDLR = Up | Down | Left | Right,
    };

    GameLevel mLevel;
    GameCellInfo mParent;
    GameObject mEnterDoor;
    int mX;
    int mY;

    public GameCellInfo Parent { get { return mParent; } }
    public GameLevel Level { get { return mLevel; } }
    public ECellType CellType { get; set; }
    public EDoorFlag DoorMask { get; set; }
    public GameObject CellObject { get; set; }
    public GameObject EnterDoor { get { return mEnterDoor; } }
    public GameObject BridgeObject { get; set; }
    public GameObject Trigger { get; set; }
    public GameObject[] Doors { get; set; }
    public GameCell Cell { get; set; }
    public int X { get { return mX; } }
    public int Y { get { return mY; } }
    public float Height { get; set; }

    public GameCellInfo(GameLevel level, GameCellInfo parent, int x, int y)
    {
        mLevel = level;
        mParent = parent;
        CellType = ECellType.None;
        DoorMask = EDoorFlag.None;
        mX = x;
        mY = y;
    }

    public void SetDoors(Transform[] doors)
    {
        Doors = new GameObject[] {
            doors[0] ? doors[0].gameObject : null,
            doors[1] ? doors[1].gameObject : null,
            doors[2] ? doors[2].gameObject : null,
            doors[3] ? doors[3].gameObject : null };

        float minSqrMagnitude = float.MaxValue;
        foreach (GameObject door in Doors)
        {
            if (door == null || BridgeObject == null)
                continue;

            float sqrMagnitude = (door.transform.position - BridgeObject.transform.position).sqrMagnitude;
            if (sqrMagnitude < minSqrMagnitude || mEnterDoor == null)
            {
                minSqrMagnitude = sqrMagnitude;
                mEnterDoor = door;
            }
        }
    }
}
