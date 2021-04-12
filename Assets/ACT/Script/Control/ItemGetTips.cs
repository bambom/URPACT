using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ItemGetTips : MonoBehaviour
{
	private Queue<Item> mItemGetQueue = new Queue<Item>();
	
	// Use this for initialization
	void Start ()
	{
		GameObject.DontDestroyOnLoad(this);
		
	}
	
	// Update is called once per frame
	void Update ()
	{
		
		
	}
}

