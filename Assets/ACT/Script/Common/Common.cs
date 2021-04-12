using UnityEngine;
using System.Collections;

public enum EKeyList
{
    KL_Attack = 0,
    KL_SubAttack,
    KL_SkillAttack,
    KL_AuxKey,

    KL_Grab,
    KL_Jump,
    KL_Move,
    KL_CameraUp,
    KL_CameraDown,
    KL_LastKey,
    KL_Max,
};

/// EOperation
public enum EOperation
{
    EO_None = 0,
    EO_Attack,
    EO_SpAttack,
    EO_Skill,
    EO_Move,
    EO_Jump,
    EO_Grab,
    EO_Front,
    EO_Back,
    EO_Last,
    EO_Auxiliary,
};

/// EInputType
public enum EInputType
{
    EIT_Click = 0,
    EIT_DoubleClick,
    EIT_Press,
    EIT_Release,
    EIT_Pressing,
    EIT_Releasing, 
};

public enum EUnitCamp
{
    EUC_NONE = 0,	    // ÖÐÁ¢
    EUC_FRIEND = 1,     // ÅóÓÑ
    EUC_ENEMY = 2,      // µÐÈË
};

public enum EUnitType
{
    EUT_NONE = 0,
    EUT_LocalPlayer = 1,
    EUT_OtherPlayer = 2,
    EUT_PVPPlayer = 3,
    EUT_Npc = 4,
    EUT_Monster = 5,
    EUT_MonsterItem = 6,
    EUT_Pet = 7,
    EUT_MAX,
};

public enum UnitState
{
    Normal = 0,
    Die = 1,
};

public enum ECompareType
{
    ECT_EQUAL = 0, // µÈÓÚ£š==£©
    ECT_NOT_EUQAL = 1, // ²»µÈÓÚ£š£¡=£©
    ECT_GREATER = 2, // ŽóÓÚ£š>£©
    ECT_LESS = 3, // Ð¡ÓÚ£š<£©
    ECT_GREATER_EQUAL = 4, // ŽóÓÚ»òµÈÓÚ£š>=£©
    ECT_LESS_EQUAL = 5, // Ð¡ÓÚ»òµÈÓÚ£š<=£©
};

public enum EVariableIdx
{
    EVI_HP = 0,
    EVI_HPPercent = 1,
    EVI_Level = 2,
    EVI_Custom = 3,
    EVI_CustomMax = 20,
};

public enum EPhysicsLayer
{
    Player = 9,
    Monster = 10,
};

public enum ECellType
{
    None = 0,
    Normal = 1,
    Genius = 2,
    Boss = 3,
    Extra = 4,
};

public enum EPA // player attribute
{
    CurHP = 0,
    HPMax,
    HPRestore,
    //------------------------------------------------
    CurSoul,
    SoulMax,
    SoulRestore,
    //------------------------------------------------
    CurAbility,
    AbilityMax,
    AbRestore,
    AbHitAdd,
    //------------------------------------------------
    CurExp,
    EXPMax,
    //------------------------------------------------
    Level,
    //------------------------------------------------
    Damage,
    Defense,
    //------------------------------------------------
    SpecialDamage,
    SpecialDefense,
    //------------------------------------------------
    Critical,
    Block,
    Hit,
    Tough,
    //------------------------------------------------
    MoveSpeed,
    //------------------------------------------------
    FastRate,
    StiffAdd,
    StiffSub,
    //------------------------------------------------
    MAX,
};


public enum EBuffSpecialEffect
{
    None = 0,
    CanNotHurt = 1,
    CanMove = 2,
    CanRotate = 3,
    Max,
}

public enum ECombatResult
{
    ECR_Normal,
    ECR_Block,
    ECR_Critical,
};


public enum EquipSubType
{
    EST_Weapon = 1,//武器
    EST_Helmet,//头盔
    EST_Armor,//上衣
    EST_Shoulder,//肩甲
    EST_Shoe,//鞋子
    EST_NeckLace = 6,//项链
    EST_WaistBand,//腰带
    EST_Wrist,//护腕
    EST_Ring//戒指		
};


public enum ItemQuality
{
    IQ_White,
    IQ_Green,
    IQ_Blue,
    IQ_Purple,
};

public enum EGameMode
{
    GMDebug,
    GMRelease
};

public enum ESceneType
{
    Invalid = 0,
    Startup = 1,
    GameGuide = 2,
    CreateRole = 3,
    Main = 4,
    PVP = 5,
    Reserve = 6,
    Level = 7,
	SuperLevel = 8,
    Max
}

public enum ESceneID
{
    Invalid = 0,
    Startup = 1000,
    A_FirstGameGuide = 2000,
    CreateRole = 3000,
    Main = 4000,
    PVP = 5000,
    Level = 7000,
};

public enum EChatChannel
{
    World = 0,
    Private = 1,
}

public enum QualitySprite
{
    Button10_BaseItem_Quality_00,
    Button10_BaseItem_Quality_01,
    Button10_BaseItem_Quality_02,
    Button10_BaseItem_Quality_03,
    Button10_BaseItem_Quality_04,

};

public enum ItemExpand
{
	Expand_Gem = 1,
	Expand_Gold = 2,
	Expand_Sp = 3,
}

public enum PropsBaseID
{
	Hp	 = 200,
	Mana = 203,
	Strength = 309,
};

public enum StoneBaseID
{
    StoneLevel1 = 10000,
    StoneLevel2,
    StoneLevel3,
    StoneLevel4,
    StoneLevel5,
    StoneLevel4To5 = 10005 //StrengthenSpeicial
};

//Strengehen And Combine Process
public enum SliderProgress
{
    Invalid,
    Start,
    Process,
    End
};

// pvp的排行榜
public enum ETopListType
{
    level = 0,
    money = 1,
    battle = 2,
    progress = 3,
    pvp = 4,
};

public class LoadingSceneInfo
{
    public ESceneID ID;
    public ESceneType Type;
    public string Name;
    public int Data = 0;
}

public enum EServerIpList
{
    Local = 0,
    Lan_04 = 1,
	China = 4,
    Public = 5,
	F = 6
}

public enum EPlatform
{
    Device = 0,
    GameCenter = 1,
}


public enum GuideType
{
	Invalid = -1,
	Progress = 0,
	SP = 1,
	Item = 2,
}

public enum EQuestType
{
	Story = 1,
	Bounty = 2,
	Active = 3
}


public enum RoleID
{
	Warrior = 1003,
	Assassin = 1004,
	Mage = 1005,
}