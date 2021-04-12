using UnityEngine;
using System.Collections;

public class Hurt : MonoBehaviour {
	public EUnitType[] hurtUnitTypes = new EUnitType[]{ EUnitType.EUT_LocalPlayer};
	public float hurtTime;
	public float frequencyTime = 0.1f;
	public float intervalTime;
	public float toutalTime;
	
	public float hurtPercent;
	public int hurtFixed;
	
	float invHurtTime;
	float invIntervalTime;
	float invFrequency;
	void Awake()
	{
		invHurtTime = hurtTime;
		invIntervalTime = intervalTime;
		invFrequency = frequencyTime;
	}
	
	void Update()
	{
		toutalTime-=Time.deltaTime;
	}
	
	void OnTriggerStay(Collider other)
	{
		UnitInfo uinfo = other.GetComponent<UnitInfo>();
		
		if(uinfo==null) return;
		
		bool pass = false;
		foreach (EUnitType item in hurtUnitTypes) {
			if(item == uinfo.UnitType)
			{
				pass = true;
				break;
			}
		}
		
		if(!pass) return;
		
		Unit unit = uinfo.Unit;
		
		if (unit == null) return;
		
		if (toutalTime <= 0 ) { Destroy(gameObject); return; }
		if (hurtTime > 0)
		{
			hurtTime -= Time.deltaTime;
			frequencyTime -= Time.deltaTime;
			if(frequencyTime <=0)
			{
				frequencyTime = invFrequency;
				//hurt
				int maxhp = unit.GetAttrib(EPA.HPMax);
				unit.Hurt(null,(int)(maxhp * hurtPercent + hurtFixed), ECombatResult.ECR_Normal);
			}
		}
		else
		{
			intervalTime -= Time.deltaTime;
			if(intervalTime <=0)
			{
				hurtTime = invHurtTime;
				intervalTime = invIntervalTime;
			}
		}
	}
}
