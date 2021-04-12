using UnityEngine;
using System.Collections.Generic;

public class GameCell : MonoBehaviour
{
    // public will be serialized.
    public GameLevel Level;
    public GameCell Parent;
    public GameObject[] Doors;
    public GameObject EnterDoor;
    public GameObject Trigger;
    public ECellType CellType;
    public bool DoorOpenFailed = false;

    Dictionary<string, int> mDropInfo;
	public GameCellInfo CellInfo;
	
    void OpenDoor(GameObject door, bool open)
    {
        // ignore the same flag.
        if (door.GetComponent<Collider>().enabled != open)
            return;

        // setup the collider.
        door.GetComponent<Collider>().enabled = !open;

        // setup the partical system.
        ParticleSystem ps = door.GetComponentInChildren<ParticleSystem>();
        if (ps != null)
            ps.enableEmission = !open;

        // setup the animation
        Animation animation = door.GetComponentInChildren<Animation>();
        if (animation != null)
        {
            foreach (AnimationState clip in animation)
            {
                if (clip.name.Contains(open ? "close" : "open"))
                {
					animation.Play(clip.name);
                    break;
                }
            }
        }
    }

    public void OpenAllDoor()
    {
        foreach (GameObject door in Doors)
        {
            if (door == null)
                continue;

            OpenDoor(door, true);
        }
    }

    public void OpenEnterDoor()
    {
        if (MainScript.Instance == null)
        {
            OpenDoor(EnterDoor, true);
            return;
        }

        // assume all open will failed.
        DoorOpenFailed = true;

        // request open door.
        StartCoroutine(MainScript.Execute(new OpenCellCmd((int)CellType), delegate(string err, Response response)
        {
            if (!string.IsNullOrEmpty(err))
                return;

            DropInfo dropInfo = (response != null) ? response.Parse<DropInfo>() : null;
            if (dropInfo != null && dropInfo.drops != null)
            {
                mDropInfo = dropInfo.drops;
                DropItemAssign.Instance.AssignItemToMonster(mDropInfo, this);
            }

            OpenDoor(EnterDoor, true);

            DoorOpenFailed = false;
        }));
    }

    public void CloseAllDoor()
    {
        foreach (GameObject door in Doors)
        {
            if (door == null)
                continue;

            OpenDoor(EnterDoor, false);
        }
    }

    public void OnBegin()
    {
        CloseAllDoor();
		Level.OnBegin(this);
    }

    /// <summary>
    /// the cell is finished.
    /// </summary>
    public void OnFinish()
    {
        // tell the level i am finished
        // it will open the enter door of the next cell.
        Level.OnFinish(this);

        OpenAllDoor();
    }
}
