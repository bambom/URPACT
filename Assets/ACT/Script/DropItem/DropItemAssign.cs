using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DropItemInfo
{
	public int ItemID;
	public int ItemNum;

    public DropItemInfo() { }
    public DropItemInfo(int item, int num) { ItemID = item; ItemNum = num; }
}

public class DropItemAssign : Singleton<DropItemAssign> {

	public void AssignItemToMonster( Dictionary<string, int> dropItem, GameCell cell)
	{
		if(dropItem.Count == 0)
			return;

		SpawnPoint[] sp = cell.GetComponentsInChildren<SpawnPoint>();
		Sort(sp);
		
		AssignToSpawn(sp, dropItem);	
	}
	
	void AssignToSpawn(SpawnPoint[] spawnArray, Dictionary<string, int> dropItem)
	{
		int spawnNum = 0;
		int randIndex = 0;
		List<DropItemInfo> list = new List<DropItemInfo>();
		
		System.Random rand = new System.Random();
		foreach(SpawnPoint sp in spawnArray)
		{
			if(dropItem.Count <= 0)
				return;
			
			spawnNum = sp.TotalSpawn;
			list.Clear();
			// monster is boss or genius? drop all for it.
			if(sp.SpawnList[0].PoolIndex == 4 || sp.SpawnList[0].PoolIndex == 3)
			{
				foreach(KeyValuePair<string,int>kp in dropItem)
				{
					DropItemInfo dropInfo = new DropItemInfo();
					dropInfo.ItemID = int.Parse(kp.Key);
					dropInfo.ItemNum = kp.Value;
					
					list.Add(dropInfo);
				}
				sp.DropInfo = list.ToArray();
				return;
			}
			else //range assign
			{
				while(spawnNum > 0 && dropItem.Count>0)
				{
					List<string> keys = new List<string>(dropItem.Keys);
					randIndex = rand.Next(0, keys.Count);
					DropItemInfo dropInfo = new DropItemInfo();
					dropInfo.ItemID = int.Parse(keys[randIndex]);
					dropInfo.ItemNum = dropItem[keys[randIndex]];
					
					list.Add(dropInfo);
					dropItem.Remove(keys[randIndex]);
					spawnNum--;
				}
				sp.DropInfo = list.ToArray();
			}
		}
	}
	
	void Sort(SpawnPoint[] spawnArray)
	{
		SpawnPoint temp;
		int j = 0;
		for(int i=1; i<spawnArray.Length; i++)
		{
			j = i;
			temp = spawnArray[i];
			while( j>0 && temp.SpawnList[0].PoolIndex > spawnArray[j-1].SpawnList[0].PoolIndex)
			{
				spawnArray[j] = spawnArray[j-1];
				j--;
			}
			spawnArray[j] = temp;
		}
	}
}
