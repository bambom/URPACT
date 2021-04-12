using UnityEngine;
using System.Collections;

public class ItemPool : MonoBehaviour{
	
	public int InitCapacity;
	
	GameObject mPrefabe;
	Stack mPool;
	
	void Start()
	{
		mPool = mPool == null ? new Stack(InitCapacity) : mPool;
		mPrefabe = mPrefabe == null ? Resources.Load("DropItemPanel") as GameObject : mPrefabe;
		
		Extend(InitCapacity);
	}

	public GameObject Get()
	{
		if(mPool.Count == 0)
			Extend(InitCapacity);
		
		return mPool.Pop() as GameObject;
	}
	
	void Extend(int capacity)
	{
		if(mPrefabe == null)
			return;
		
		int i = mPool.Count;
		while(i < capacity)
		{
			GameObject obj = GameObject.Instantiate(mPrefabe) as GameObject;
			obj.SetActive(false);
			mPool.Push(obj);
			i++;
		}
	}
	
	public void Revert(GameObject obj)
	{
		mPool.Push(obj);
	}
	
	void Clean()
	{
		mPool.Clear();
	}
}
