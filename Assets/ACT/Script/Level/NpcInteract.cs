using UnityEngine;
using System.Collections;

/// <summary>
//NPC_Jisirenyuan
//Npc_Jisizhang
//Npc_Manzuzhang
//Npc_Manzuzhanshi
//NPC_Weibing1
//Npc_Nvlieshou
//NPC_Weibing3
//GameStartDoor
/// </summary>
public enum ENPCID
{
	INVALID			= -1,
	BackPack		= 2000,				
	Skill,		                           
	PvP,			
	TopList,	
	Store,
	Quest,
	Activity,
	GAMESTARTDOOR	= 2009,
	MAX
}

/// <summary>
/// NpcInteract
///	simplest script for attach to npc.
/// </summary>
public class NpcInteract : MonoBehaviour
{	
	GameObject mNpcName;
	//GameObject mNpcStyle;
	GameObject mNpcIcon;	
	bool mNpcTriggerEntry = false;
    Npc mNpc;
	int mNpcId = 0;

	void Awake()
	{		
		//get controls
		Transform[] children = gameObject.GetComponentsInChildren<Transform>();
        foreach (Transform child in children)
        {
            if (child.name == "NpcIcon")
                mNpcIcon = child.gameObject;
			if (child.name == "NpcName")
                mNpcName = child.gameObject;
//			if (child.name == "NpcStyle")
//                mNpcStyle = child.gameObject;
        }
	}
	
	void Start()
	{
		InitNpcHeadInfo();
	}
	
	void Update()
	{

	}
	
	void InitNpcHeadInfo()
	{
        if (null == mNpcIcon || null == mNpcName)
            return;

        mNpcName.SetActive(!mNpcTriggerEntry);
        mNpcIcon.SetActive(!mNpcTriggerEntry);
        UnitInfo npcUnit = gameObject.GetComponent<UnitInfo>();
        mNpc = npcUnit.Unit as Npc;
        NpcInfoInit(npcUnit.UnitID);
	}
	
	void UpdateNpcHeadInfo()
	{
		InitNpcHeadInfo();
	}
	
	void RefreshNpcHeadInfo()
	{
        //if (mNpcName)
        //    mNpcName.SetActive(!mNpcTriggerEntry);
        //if(mNpcStyle)
        //  mNpcStyle.SetActive(!mNpcTriggerEntry);
        //if (mNpcIcon)
        //    mNpcIcon.SetActive(mNpcTriggerEntry);
	}
	
	void NpcInfoInit(int NPCID)
	{		
		mNpcId = NPCID;	
		if(-1 == NPCID)
			return;
		if(NPCID == (int)ENPCID.GAMESTARTDOOR)
			return;
		
		UnitBase unitBase = UnitBaseManager.Instance.GetItem(NPCID);
		mNpcName.GetComponent<UILabel>().text = unitBase.Name;
		UISprite uisp = mNpcIcon.GetComponent<UISprite>();
		uisp.spriteName = unitBase.Label;
		uisp.MakePixelPerfect(0.95f);
    }


    void OnTriggerEnter(Collider other)
    {
        UnitInfo unitInfo = other.GetComponent<UnitInfo>();
        if (unitInfo.Unit == UnitManager.Instance.LocalPlayer)
        {
            mNpcTriggerEntry = true;

            RefreshNpcHeadInfo();

            UnitBase unitBase = UnitBaseManager.Instance.GetItem(mNpcId);
            if (unitBase != null)
                mNpc.PlayAction(unitBase.CustomAction1);

            UpdateInGameMainWndNpcBtn(mNpcTriggerEntry);
        }
    }
	
	void OnTriggerExit(Collider other)
	{
		UnitInfo unitInfo = other.GetComponent<UnitInfo>();
		if(unitInfo.Unit == UnitManager.Instance.LocalPlayer)
		{
			mNpcTriggerEntry = false;

			RefreshNpcHeadInfo();

            UnitBase unitBase = UnitBaseManager.Instance.GetItem(mNpcId);
            if (unitBase != null)
                mNpc.PlayAction(unitBase.CustomAction2);

			UpdateInGameMainWndNpcBtn(mNpcTriggerEntry);
		}
	}
	
	void UpdateInGameMainWndNpcBtn(bool Status)
	{
		if(InGameMainWnd.Exist 
			&& InGameMainWnd.Instance.WndObject.activeSelf){
			InGameMainWnd.Instance.NpcDialogueSet(Status,(ENPCID)mNpcId);
		}
	}
}
