using UnityEngine;
using System.IO;
using System.Collections;
using System.Text;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;

public class ActionManager : Singleton<ActionManager>
{
    Dictionary<int, Data.UnitActionInfo> mUnitActionInfos = new Dictionary<int, Data.UnitActionInfo>();
	
    public ActionManager()
    {
    }

    public void Clear()
    {
        mUnitActionInfos.Clear();
    }

    // parse a data array from the table data.
    static TextAsset LoadActionType(string name)
    {
        return UpdateHelper.Instance.GetAsset("ActionType.unity3d", name); ;
    }

    Data.UnitActionInfo Load(int id)
    {
        string actionTypeBin = "Action_" + id;
        TextAsset textAsset = null;
    #if !UNITY_EDITOR
        // we load from the server tables.
        if (MainScript.Instance != null && 
            MainScript.Instance.CheckUpdate && 
            UpdateHelper.Instance != null)
            textAsset = LoadActionType(actionTypeBin);
        else
    #endif
        textAsset = (TextAsset)Resources.Load(actionTypeBin);

        using (MemoryStream stream = new MemoryStream(textAsset.bytes))
        {
            BinaryFormatter bf = new BinaryFormatter();
            Data.UnitActionInfo actionData = (Data.UnitActionInfo)bf.Deserialize(stream);
            stream.Close();
            return actionData;
        }
    }

    public Data.UnitActionInfo GetUnitActionInfo(int id)
    {
        Data.UnitActionInfo ret;
        if (!mUnitActionInfos.TryGetValue(id, out ret))
        {
            ret = Load(id);
            mUnitActionInfos[id] = ret;
        }
        return ret;
    }
}
