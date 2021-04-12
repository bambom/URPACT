using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UnitManager
{
    static UnitManager sInstance = null;
    public static UnitManager Instance
    {
        get
        {
            if (sInstance == null)
                sInstance = new UnitManager();
            return sInstance;
        }
    }
	
    List<Unit> mUnitInfos = new List<Unit>();
    public List<Unit> UnitInfos { get { return mUnitInfos; } }

    public LocalPlayer LocalPlayer = null;
	
    public void Add(Unit unit)
    {
        if (unit.UnitType == EUnitType.EUT_LocalPlayer)
            LocalPlayer = unit as LocalPlayer;
			
        mUnitInfos.Add(unit);
    }

    public void Destroy(Unit unit)
    {
        if (unit == LocalPlayer)
            LocalPlayer = null;
		
        unit.OnDestroy();
		
		mUnitInfos.Remove(unit);
    }
	
	public void CleanEmptyObject()
	{
		for (int i = 0; i < mUnitInfos.Count; i++) {
			Unit unit = mUnitInfos[i];
			if(unit.UUnitInfo==null)
				mUnitInfos.RemoveAt(i--);
		}
	}
	
	public void DestroyAll()
    {
		mUnitInfos.RemoveAll(
			delegate(Unit obj) {
				if(obj.UnitType != EUnitType.EUT_LocalPlayer)
				{
					obj.OnDestroy();
					return true;
				}
				return false;
			}
		);
    }
	
	public void Reset()
	{
		LocalPlayer = null;
		mUnitInfos.Clear();
	}
	
	public void Kill(Unit unit)
	{
		unit.OnDestroy();
	}
}
