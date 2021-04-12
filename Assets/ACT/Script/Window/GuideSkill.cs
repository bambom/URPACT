using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class GuideSkill : MonoBehaviour {
	
	public int index = 0;
	// Use this for initialization
	void Start () {
	
		SkillInput skillInput = gameObject.AddComponent<SkillInput>();
		List<SkillItem>list =  gameObject.transform.parent.GetComponent<GuideFightWnd>().SkillList;
		Debug.Log(list.Count);
		foreach(SkillItem sk in list)
			Debug.Log(sk.SkillBase.ID);
		if(list.Count > index)
			skillInput.Init(list[index]);
		else
			gameObject.SetActive(false);
	}
	
}
