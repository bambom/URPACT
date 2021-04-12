using UnityEngine;
using System.Collections;

public class UnitTopUI : MonoBehaviour
{
    Unit mOwner;
    GameObject mTopUI;
    UISlider mSliderHP;
    UILabel mUnitName;
	UILabel mCritical;
	UILabel mDamage;
	UILabel mHurt;
    Transform mCameraTarget;
    ObjectPool mCriticalPool;
	ObjectPool mDamagePool;
	ObjectPool mHurtPool;
    public GameObject TopUI { get { return mTopUI; } }
    public Vector3 TopPosition { get { return mTopUI.transform.position; } }

    public static readonly Color CriticalColor = new Color(1, 192f / 255, 0);
    public static readonly Color BlockColor = new Color(108f / 255, 108f / 255, 108f / 255);
    public static readonly Color EnemyHurtColor = Color.white;
    public static readonly Color PlayerHurtColor = Color.red;
    public static readonly Color ExpColor = new Color(242f / 255, 54f / 255, 233f / 255);
    public static readonly Color ManaColor = new Color(0f / 255, 176f / 255, 240f / 255);
    public static readonly Color SoulColor = new Color(1, 192f / 255, 0);
    public static readonly Color HPColor = new Color(0, 176f / 255, 80f / 255);
    public static readonly Color GoldColor = Color.white;

    public void Bind(Unit owner)
    {
        mOwner = owner;
    }

    void Start()
    {
        if (Camera.main)
            mCameraTarget = Camera.main.transform;

        mTopUI = GameObject.Instantiate(Resources.Load("UnitTopUI")) as GameObject;
        mTopUI.transform.parent = transform;
        mSliderHP = mTopUI.GetComponentInChildren<UISlider>();
        mUnitName = mTopUI.transform.Find("UnitName").GetComponent<UILabel>();
		mCritical = mTopUI.transform.Find("Critical").GetComponent<UILabel>();
		mDamage = mTopUI.transform.Find("Damage").GetComponent<UILabel>();
		mHurt = mTopUI.transform.Find("Hurt").GetComponent<UILabel>();
		mCriticalPool = gameObject.AddComponent<ObjectPool>();
		mCriticalPool.Prefab = mCritical.gameObject;
		mDamagePool = gameObject.AddComponent<ObjectPool>();
		mDamagePool.Prefab = mDamage.gameObject;
		mHurtPool = gameObject.AddComponent<ObjectPool>();
		mHurtPool.Prefab = mHurt.gameObject;
		
        // disable it all.
        mSliderHP.gameObject.SetActive(false);
        mUnitName.gameObject.SetActive(false);
		mCritical.gameObject.SetActive(false);
		mDamage.gameObject.SetActive(false);
		mHurt.gameObject.SetActive(false);

        if (mOwner != null)
        {
            float height = 1.0f;
            CapsuleCollider collider = gameObject.GetComponent<CapsuleCollider>();
            if (collider) height = collider.height;
            else
            {
                CharacterController controller = gameObject.GetComponent<CharacterController>();
                if (controller) height = controller.height;
            }

            mTopUI.transform.localPosition = new Vector3(
                mOwner.UUnitInfo.HpPointDifference.x,
                mOwner.UUnitInfo.HpPointDifference.y + height,
                mOwner.UUnitInfo.HpPointDifference.z);
			
            if (mOwner is Player)
                CreatePlayerInfo(mOwner as Player);
        }
    }

    void CreatePlayerInfo(Player player)
    {
		mUnitName.gameObject.SetActive(true);
		switch(Global.CurSceneInfo.ID)
		{
			case ESceneID.Invalid:
			{
		    	mUnitName.text = "";//string.Format("Lv. {0} {1}", 
								 //		        player.PlayerData.Attrib.Level,
								 //		        player.PlayerData.Attrib.Name);
			}break;
			case ESceneID.Startup:
			{
		
			}break;
			
			case ESceneID.A_FirstGameGuide:
			{
				mUnitName.text = "" ;
			}break;
			
			case ESceneID.Main:
			{
				mTopUI.transform.localScale = mTopUI.transform.localScale*1.5f;
				if(mOwner.UnitType == EUnitType.EUT_Npc){
					mUnitName.text = "" ;
				}else if(mOwner is LocalPlayer){
					mUnitName.text =  "[FFFF00]" + string.Format("Lv. {0} {1}", 
										        player.PlayerData.Attrib.Level,
										        player.PlayerData.Attrib.Name) +"[-]";
				}else{
					mUnitName.text =  string.Format("Lv. {0} {1}", 
										        player.PlayerData.Attrib.Level,
										        player.PlayerData.Attrib.Name) ;
				}
				//+"[-]"
			}break;
			case ESceneID.PVP:
			{
				if(mOwner is LocalPlayer){
		    		mUnitName.text = string.Format("Lv. {0} {1}", 
										        player.PlayerData.Attrib.Level,
										        player.PlayerData.Attrib.Name);
				}else{
		    		mUnitName.text = "[FF0000]" + string.Format("Lv. {0} {1}", 
										        player.PlayerData.Attrib.Level,
										        player.PlayerData.Attrib.Name) +"[-]";
				}
			}break;
			default: //Level
		    	mUnitName.text = "";//string.Format("Lv. {0} {1}", 
					 //		        player.PlayerData.Attrib.Level,
					 //		        player.PlayerData.Attrib.Name);
				break;
		}
    }

    void LateUpdate()
    {
        if (mCameraTarget)
            mTopUI.transform.rotation = mCameraTarget.rotation;
    }

    public void OnHpUpdate()
    {
        if (mOwner.Dead)
        {
            mSliderHP.gameObject.SetActive(false);
            return;
        }

        mSliderHP.gameObject.SetActive(true);
        mSliderHP.sliderValue = (float)mOwner.GetAttrib(EPA.CurHP) / mOwner.GetAttrib(EPA.HPMax);
    }

    public void OnInfoPopup(string text, ECombatResult res, Unit attackOrigin)
    {
        OnInfoPopup(text, res, 0.0f, attackOrigin);
    }

    public void OnInfoPopup(string text, ECombatResult res, float stay, Unit attackOrigin)
    {
        UnitManager.Instance.LocalPlayer.UUnitInfo.UnitTopUI.OnInfoPopup(
            mUnitName.transform.position, text, res, stay, attackOrigin);
    }

    const float TotalTime = 0.8f;
    const float ScaleTime = 0.2f;

    const float InitScaleMin = 8.0f;
    const float InitScaleMax = 8.0f;
    const float TargetScale = 1.5f;
    const iTween.EaseType ScaleType = iTween.EaseType.linear;

    const float InitRange = 0.5f;
    const float DropRadius = 1.0f;
    const float ThrowHeight = -0.2f;
    const float DropHeight = -0.6f;
    const iTween.EaseType DropType = iTween.EaseType.linear;
	
	GameObject GetLabel(ECombatResult res, Unit attackOrigin)
	{
		Debug.Log(attackOrigin.UnitType);
		switch(res)
		{
			case ECombatResult.ECR_Critical:
			{
				if(attackOrigin.UnitType != EUnitType.EUT_LocalPlayer)
					return mHurtPool.Spawn(true);
				return mCriticalPool.Spawn(true);
			}
			case ECombatResult.ECR_Normal:
			{
				if(attackOrigin.UnitType != EUnitType.EUT_LocalPlayer)
					return mHurtPool.Spawn(true);
				return mDamagePool.Spawn(true);
			}
		}
		return mDamagePool.Spawn(true);
	}

    public void OnInfoPopup(Vector3 position, string text, ECombatResult res, float stay, Unit attackOrigin)
    {
        GameObject damageText = GetLabel(res, attackOrigin);
        damageText.transform.parent = mUnitName.transform.parent;
        damageText.transform.position = position;
        damageText.transform.rotation = mUnitName.transform.rotation;
        damageText.transform.localScale = mUnitName.transform.localScale * Random.Range(InitScaleMin, InitScaleMax);
        damageText.SetActive(true);

        UILabel damageLabel = damageText.GetComponent<UILabel>();
     
        damageLabel.text = text;

	    iTween.ScaleTo(damageText, iTween.Hash(
	            "scale", mUnitName.transform.localScale * TargetScale,
	            "delay", stay,
	            "easetype", ScaleType,
	            "time", ScaleTime));
		
        Vector3 offset = new Vector3(Random.Range(-InitRange, InitRange), 0, Random.Range(-InitRange, InitRange));
        Vector3[] paths = new Vector3[] {
            damageText.transform.position + offset,
            damageText.transform.position + offset + offset * DropRadius * 0.5f + new Vector3(0, ThrowHeight, 0),
            damageText.transform.position + offset + offset * DropRadius + new Vector3(0, DropHeight, 0),
        };
		if(text != "@")
		{
	        iTween.MoveTo(damageText, iTween.Hash(
	            "path", paths,
	            "time", TotalTime - ScaleTime,
	            "delay", stay + ScaleTime,
	            "easetype", DropType,
	            "oncomplete", GetCompeletFuncName(res, attackOrigin),
	            "oncompletetarget", gameObject,
	            "oncompleteparams", damageText));
		}
		else
		{
			iTween.MoveTo(damageText, iTween.Hash(
	            "time", TotalTime - ScaleTime,
	            "delay", stay + ScaleTime,
	            "easetype", DropType,
	            "oncomplete", GetCompeletFuncName(res, attackOrigin),
	            "oncompletetarget", gameObject,
	            "oncompleteparams", damageText));
		}
    }
	
	string GetCompeletFuncName(ECombatResult res, Unit attackOrigin)
	{
		switch(res)
		{
			case ECombatResult.ECR_Critical:
			{
				if(attackOrigin.UnitType != EUnitType.EUT_LocalPlayer)
					return "HurtComplete";
				return "CritiaclComplete";
			}
			case ECombatResult.ECR_Normal:
			{
				if(attackOrigin.UnitType != EUnitType.EUT_LocalPlayer)
					return "HurtComplete";
				return "DamageComplete";
			}
		}
		return "";
	}

    void MoveComplete(GameObject targetGameObject)
    {
        iTween[] tween = targetGameObject.GetComponents<iTween>();
       	
		for(int i = 0;i<tween.Length;i++){
			 if (tween[i]) GameObject.Destroy(tween[i]);
		}
    }
	
	void CritiaclComplete(object target)
	{
		 GameObject targetGameObject = target as GameObject;
		 MoveComplete(targetGameObject);
		 mCriticalPool.Recycle(targetGameObject);
	}
	
	void DamageComplete(object target)
	{
		GameObject targetGameObject = target as GameObject;
		MoveComplete(targetGameObject);
		mDamagePool.Recycle(targetGameObject);
	}
	
	void HurtComplete(object target)
	{
		GameObject targetGameObject = target as GameObject;
		MoveComplete(targetGameObject);
		mHurtPool.Recycle(targetGameObject);
	}
}
