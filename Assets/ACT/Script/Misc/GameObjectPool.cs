using UnityEngine;
using System.Collections;

public class GameObjectPool : MonoBehaviour
{
    Stack mAvailables;
    ArrayList mAllItem;
    MonsterAttrib mMonsterAttrib;

    public GameObject Prefab;
    public string PrefabName;
    public int ID = 0;
    public int Level = 0;
    public int InitialCapacity = 0;
    public bool SetActiveRecursively = false;

    public MonsterAttrib MonsterAttrib { get { return mMonsterAttrib; } }

    void Start()
    {
        Init();
    }

    public bool SetUp(int id, int lv, int count)
    {
        if (id == 0 || lv == 0 || count == 0)
            return false;
		
        ID = id;
        Level = lv;

        UnitBase unitBase = UnitBaseManager.Instance.GetItem(id);
        if (unitBase == null)
        {
            Debug.LogError("Can't find unit base with id: " + id + lv);
            return false;
        }

        mMonsterAttrib = MonsterAttribManager.Instance.GetItem(id, lv);
        if (unitBase == null)
        {
            Debug.LogError("Can't find MonsterAttrib id: " + id + " lv: " + lv);
            return false;
        }

        PrefabName = unitBase.Prefab;
        Prefab = Resources.Load(PrefabName) as GameObject;
        InitialCapacity = count;

        return true;
    }

    public void Init()
    {
        if (Prefab != null && InitialCapacity > 0)
        {
            mAvailables = new Stack(InitialCapacity);
            mAllItem = new ArrayList(InitialCapacity);
            PrePopulate(InitialCapacity * 2);
            InitialCapacity /= 2;
        }
    }

    public GameObject Spawn(Vector3 position, Quaternion rotation)
    {
		if (Prefab == null)
			return null;
		
        GameObject result;
        if (mAvailables.Count == 0)
        {
            result = GameObject.Instantiate(Prefab, position, rotation) as GameObject;
            UnitInfo unitInfo = result.GetComponent<UnitInfo>();
            if (unitInfo != null)
            {
                unitInfo.Pool = this;
                unitInfo.initialization = true;
                unitInfo.UnitID = ID;
                unitInfo.Level = Level;
                unitInfo.Unit.UpdateAttributes();
            }
            else
            {
                Debug.LogError("GameObjectPool " + result.transform.name + " is not UnitInfo");
            }
            mAllItem.Add(result);
        }
        else
        {
            result = mAvailables.Pop() as GameObject;
            if (result)
            {
                Transform resultTrans = result.transform;
                resultTrans.position = position;
                resultTrans.rotation = rotation;
                this.SetActive(result, true);
            }
            else
            {
                Debug.LogError("GameObjectPool " + result.transform.name + " is not Spawn");
            }
        }
        return result;
    }

    public bool Unspawn(GameObject obj)
    {
        if (!mAvailables.Contains(obj))
        {
            mAvailables.Push(obj);
            this.SetActive(obj, false);
            return true;
        }
        return false;
    }

    public void PrePopulate(int count)
    {
        if (mAllItem == null)
            mAllItem = new ArrayList(count);

        if (mAvailables == null)
            mAvailables = new Stack(count);

        if (count < InitialCapacity)
        {
            Clear(InitialCapacity - count);
        }
        else
        {
            int max = count - InitialCapacity;
            for (var i = 0; i < max; i++)
            {
                if (Prefab == null)
                    Prefab = Resources.Load(PrefabName) as GameObject;

                GameObject result = GameObject.Instantiate(Prefab) as GameObject;
                UnitInfo unit = result.GetComponent<UnitInfo>();
                UnitInfo unitInfo = result.GetComponent<UnitInfo>();
                if (unitInfo != null)
                {
                    unitInfo.Pool = this;
                    unitInfo.initialization = true;
                    unitInfo.UnitID = ID;
                    unitInfo.Level = Level;
                    unitInfo.Unit.UpdateAttributes();
                }
                mAllItem.Add(result);
                Unspawn(result);
            }
        }

        InitialCapacity = count;
    }

    public void UnspawnAll()
    {
        foreach (GameObject obj in mAllItem)
        {
            if (obj.activeSelf)
                Unspawn(obj);
        }
    }

    public void Clear()
    {
        foreach (GameObject obj in mAllItem)
        {
            GameObject.Destroy(obj);
        }
        mAvailables.Clear();
        mAllItem.Clear();

        UnitManager.Instance.CleanEmptyObject();
    }

    public void Clear(int count)
    {
        for (int i = 0; i < count; i++)
        {
            GameObject.Destroy(mAllItem[i] as GameObject);
            mAvailables.Pop();
        }
        mAllItem.RemoveRange(0, count);
        UnitManager.Instance.CleanEmptyObject();
    }

    public int GetActiveCount()
    {
        return mAllItem.Count - mAvailables.Count;
    }

    public int GetAvailableCount()
    {
        return mAvailables.Count;
    }

    void SetActive(GameObject obj, bool val)
    {
        if (!val)
        {
            obj.transform.parent = null;
            obj.transform.position = transform.position;
        }

        // enable & disable animations.
        Animation[] animations = obj.GetComponentsInChildren<Animation>(true);
        foreach (Animation animation in animations)
            animation.enabled = val;

        if (SetActiveRecursively)
            obj.SetActive(val);
        else
            obj.SetActive(val);
    }
}
