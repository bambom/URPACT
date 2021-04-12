using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class Player : Unit
{
    protected PlayerData mPlayerData;
    protected MainAttrib mMainAttrib;
    const float RestoreTime = 0.25f;
    float mRestoreLeft = 0.0f;
    int mNavigating = 0;
    UnityEngine.AI.NavMeshAgent mNavMeshAgent;

    public Player(UnitInfo unitInfo, int level)
        : base(unitInfo, level)
    {
    }

    public PlayerData PlayerData { get { return mPlayerData; } set { mPlayerData = value; } }

    public override void UpdateAttributes()
    {
        if (mPlayerData == null)
        {
            mPlayerData = new PlayerData();
            mMainAttrib = mPlayerData.Attrib;

            PlayerAttrib playerAttrib = PlayerAttribManager.Instance.GetItem(UUnitInfo.UnitID, Level);
            if (playerAttrib != null)
            {
                mMainAttrib.CurHP = playerAttrib.HPMax;
                mMainAttrib.HPMax = playerAttrib.HPMax;
                //------------------------------------------------
                mMainAttrib.CurExp = 0;
                mMainAttrib.NextExp = playerAttrib.NextExp;
                //------------------------------------------------
                mMainAttrib.Level = Level;
                //------------------------------------------------
                mMainAttrib.Damage = playerAttrib.Damage;
                mMainAttrib.Defense = playerAttrib.Defense;
                //------------------------------------------------
                mMainAttrib.SpecialDamage = playerAttrib.SpecialDamage;
                mMainAttrib.SpecialDefense = playerAttrib.SpecialDefense;
                //------------------------------------------------
                mMainAttrib.Critical = playerAttrib.Critical;
                mMainAttrib.Block = playerAttrib.Block;
                mMainAttrib.Hit = playerAttrib.Hit;
                mMainAttrib.Tough = playerAttrib.Tough;
                //------------------------------------------------
                mMainAttrib.CurAbility = playerAttrib.AbilityMax;
                mMainAttrib.CurSoul = playerAttrib.SoulMax;
                mMainAttrib.HPRestore = playerAttrib.HPRestore;
                mMainAttrib.SoulMax = playerAttrib.SoulMax;
                mMainAttrib.SoulRestore = playerAttrib.SoulRestore;
                mMainAttrib.MoveSpeed = playerAttrib.MoveSpeed;
                mMainAttrib.FastRate = playerAttrib.FastRate;
                mMainAttrib.StiffAdd = playerAttrib.StiffAdd;
                mMainAttrib.StiffSub = playerAttrib.StiffSub;
                mMainAttrib.AbilityMax = playerAttrib.AbilityMax;
                mMainAttrib.AbHitAdd = playerAttrib.AbHitAdd;
                mMainAttrib.AbRestore = playerAttrib.AbRestore;
                mMainAttrib.AbUseAdd = playerAttrib.AbUseAdd;
            }
        }
        mMainAttrib = mPlayerData.Attrib;

        // reset up the current attributes.
        mMainAttrib.CurAbility = GetAttrib(EPA.AbilityMax);
        mMainAttrib.CurHP = GetAttrib(EPA.HPMax);
        mMainAttrib.CurSoul = 0;
		
		//attention order 
		UpdatePvPPlayerAgainstHp(mMainAttrib.CurHP,mMainAttrib.HPMax);
		UpdatePvPPlayerAgainstInfo(mMainAttrib.Role,mMainAttrib.Level);
		UpdatePvPPlayerAgainstAbility(mMainAttrib.CurAbility,mMainAttrib.AbilityMax);
    }

    public override int GetAttrib(EPA idx)
    {
        int ret = 0;
        switch (idx)
        {
            // 即时值直接返回，不用计算buff加成。
            case EPA.CurHP: return mMainAttrib.CurHP;
            case EPA.CurSoul: return mMainAttrib.CurSoul;
            case EPA.CurAbility: return mMainAttrib.CurAbility;
            case EPA.CurExp: return mMainAttrib.CurExp;
            case EPA.EXPMax: return mMainAttrib.NextExp;
            case EPA.Level: return mMainAttrib.Level;

            // 需要计算buff加成的在下面。
            case EPA.HPMax: ret = mMainAttrib.HPMax; break;
            case EPA.HPRestore: ret = mMainAttrib.HPRestore; break;

            case EPA.SoulMax: ret = mMainAttrib.SoulMax; break;
            case EPA.SoulRestore: ret = mMainAttrib.SoulRestore; break;

            case EPA.AbilityMax: ret = mMainAttrib.AbilityMax; break;
            case EPA.AbRestore: ret = mMainAttrib.AbRestore; break;
            case EPA.AbHitAdd: ret = mMainAttrib.AbHitAdd; break;

            case EPA.Damage: ret = mMainAttrib.Damage; break;
            case EPA.Defense: ret = mMainAttrib.Defense; break;
            case EPA.SpecialDamage: ret = mMainAttrib.SpecialDamage; break;
            case EPA.SpecialDefense: ret = mMainAttrib.SpecialDefense; break;

            case EPA.Critical: ret = mMainAttrib.Critical; break;
            case EPA.Block: ret = mMainAttrib.Block; break;
            case EPA.Hit: ret = mMainAttrib.Hit; break;
            case EPA.Tough: ret = mMainAttrib.Tough; break;

            case EPA.MoveSpeed: ret = mMainAttrib.MoveSpeed; break;
            case EPA.FastRate: ret = mMainAttrib.FastRate; break;
            case EPA.StiffAdd: ret = mMainAttrib.StiffAdd; break;
            case EPA.StiffSub: ret = mMainAttrib.StiffSub; break;
            default: return 0;
        }
        return mBuffManager.Apply(idx, ret);
    }

    public override void Update(float deltaTime)
    {
        base.Update(deltaTime);

        if (mRestoreLeft > 0)
            mRestoreLeft -= deltaTime;
        else
        {
            if (GetAttrib(EPA.HPRestore) > 0 && GetAttrib(EPA.CurHP) < GetAttrib(EPA.HPMax))
                AddHp(GetAttrib(EPA.HPRestore));

            if (GetAttrib(EPA.SoulRestore) > 0 && GetAttrib(EPA.CurSoul) < GetAttrib(EPA.SoulMax))
                AddSoul(GetAttrib(EPA.SoulRestore));

            mRestoreLeft += RestoreTime;
        }

        if (mNavigating > 0)
            CheckNavigationEnd();
    }

    public void StartNavigation(Vector3 pos)
    {
        if (mNavMeshAgent)
        {
            mNavigating = 1;
			mNavMeshAgent.enabled = true;
            mNavMeshAgent.destination = pos;
            mNavMeshAgent.speed = GetAttrib(EPA.MoveSpeed) * 0.01f;

            PlayAction(Data.CommonAction.RunInTown);
        }
        else
            SetPosition(pos);
    }

    void CheckNavigationEnd()
    {
        float pathEndThreshold = 0.1f;
        if (mNavigating > 10 && 
            (!mNavMeshAgent.hasPath || mNavMeshAgent.remainingDistance <= mNavMeshAgent.stoppingDistance + pathEndThreshold))
        {
            mNavigating = 0;
            mNavMeshAgent.enabled = false;
            PlayAction(Data.CommonAction.IdleInTown);
            return;
        }
        mNavigating++;
    }
	
    /// <summary>
    /// 默认的换装
    /// </summary>
    public override void Equip()
    {
        EquipWithReplace(null);
    }

    /// <summary>
    /// 支持替换装备的换装，比如试穿。
    /// </summary>
    /// <param name="replaceItem">试穿的装备</param>
    public void EquipWithReplace(IDictionary<byte, ItemBase> replaceItems)
    {
        // remove all equiped items.
        EquipInfo[] equipInfos = UGameObject.GetComponentsInChildren<EquipInfo>(true);
        foreach (EquipInfo equipInfo in equipInfos)
            GameObject.Destroy(equipInfo.gameObject);

        // attach the new equiped item.
        List<int> equipedSlots = new List<int>();
        EquipSlot[] equipSlots = UGameObject.GetComponentsInChildren<EquipSlot>(true);
        Package backPack = mPlayerData.Packages.BackPack;
        foreach (Item item in backPack.Items.Values)
        {
            SmartInt equiped = 0;
            ItemBase thisItem = item.ItemBase;
            if (thisItem == null ||
                thisItem.MainType != (int)EItemType.Equip ||
                string.IsNullOrEmpty(thisItem.Model) ||
                !item.Attrib.TryGetValue("Equip", out equiped) ||
                equiped == 0)
                continue;

            // replace this item if match.
            if (replaceItems != null && replaceItems.ContainsKey(thisItem.SubType))
                thisItem = replaceItems[thisItem.SubType];

            // equip this subitem.
            equipedSlots.Add(thisItem.SubType);

            if (!string.IsNullOrEmpty(thisItem.Model))
                EquipSingle(thisItem, equipSlots);
        }

        // check the default show item.
        for (int index = 1; index < 20; index++)
        {
            DefaultEquip defaultEquip = DefaultEquipManager.Instance.GetItem(UUnitInfo.UnitID, index);
            if (defaultEquip == null)
                break;

            ItemBase itemBase = defaultEquip.Show > 0 ? ItemBaseManager.Instance.GetItem(defaultEquip.Show) : null;
            if (itemBase == null || itemBase.MainType != (int)EItemType.Equip)
                continue;

            if (equipedSlots.Contains(itemBase.SubType))
                continue;

            // replace this item if match.
            if (replaceItems != null && replaceItems.ContainsKey(itemBase.SubType))
                itemBase = replaceItems[itemBase.SubType];

            if (!string.IsNullOrEmpty(itemBase.Model))
                EquipSingle(itemBase, equipSlots);
        }
	}

    /// <summary>
    /// 穿戴单独一件装备。
    /// </summary>
    /// <param name="itemBase"></param>
    /// <param name="equipSlots"></param>
    /// <returns></returns>
    bool EquipSingle(ItemBase itemBase, EquipSlot[] equipSlots)
    {
        UnityEngine.Object prefab = Resources.Load(itemBase.Model);
        if (prefab == null)
        {
            Debug.LogError(string.Format("ItemBase Error: ID={0} Name={1} Model={2} Message={3}",
                itemBase.ID,
                itemBase.Name,
                itemBase.Model,
                "ResourcesNotFound"));
            return false;
        }

        GameObject gameObject = GameObject.Instantiate(prefab) as GameObject;
        if (gameObject == null)
        {
            Debug.LogError(string.Format("ItemBase Error: ID={0} Name={1} Model={2} Message={3}",
                itemBase.ID,
                itemBase.Name,
                itemBase.Model,
                "ResourcesNotPrefab"));
            return false;
        }

        Transform equipParent = UUnitInfo.Model;
        foreach (EquipSlot equipSlot in equipSlots)
        {
            if (equipSlot.SubType == itemBase.SubType)
            {
                // remove the previous item.
                if (equipSlot.EquipObject)
                    GameObject.Destroy(equipSlot.EquipObject);
                equipSlot.EquipObject = gameObject;
                equipParent = equipSlot.transform;
                break;
            }
        }

        gameObject.layer = UUnitInfo.Model.gameObject.layer;
        foreach (Transform trans in gameObject.GetComponentsInChildren<Transform>(true))
            trans.gameObject.layer = gameObject.layer;
        gameObject.transform.parent = equipParent;
        gameObject.transform.localRotation = Quaternion.identity;
        gameObject.transform.localPosition = Vector3.zero;
		gameObject.transform.localScale = Vector3.one;
        return true;
    }

    public override void Init()
    {
        base.Init();

        UpdateAttributes();

        Equip();

        mNavMeshAgent = UUnitInfo.GetComponent<UnityEngine.AI.NavMeshAgent>();
		if (mNavMeshAgent != null)
			mNavMeshAgent.enabled = false;

        State = UnitState.Normal;
    }

    // for debugging...
    public override void SetLevel(int lv)
    {
        base.SetLevel(lv);

        mMainAttrib.Level = lv - 1;
        mMainAttrib.CurExp = 0;

        UpLevel();

        UpdateAttributes();
    }

    public override void Revive()
    {
        EnableCollision(true);

        State = UnitState.Normal;

        SetIsDead(false);

        PlayAction(Data.CommonAction.Revive);

        mMainAttrib.CurHP = GetAttrib(EPA.HPMax);
        mMainAttrib.CurAbility = GetAttrib(EPA.AbilityMax);

        if (!FightMainWnd.Exist)
            FightMainWnd.Instance.Open();
		else
			FightMainWnd.Instance.Show();

        if (RoleDeadWnd.Exist)
            RoleDeadWnd.Instance.Close();
    }

    public override bool Hurt(Unit attacker, int damage, ECombatResult result)
    {
        if (State == UnitState.Die)
            return false;

        if (damage <= 0)
            damage = 0;

        if (UUnitInfo.UnitTopUI)
            UUnitInfo.UnitTopUI.OnInfoPopup(damage.ToString(), result, attacker);

        if (ActionStatus.Listener != null)
            ActionStatus.Listener.OnHurt(damage);
		
        return true;
    }
	
	public override void AddAbility(int ability)
	{
		int abilitymax = mMainAttrib.AbilityMax;
		int finalability = ability + mMainAttrib.CurAbility;
		if(finalability >= abilitymax)
			finalability = abilitymax;
		else
			mMainAttrib.CurAbility = finalability ;
		
		UpdateUIAbility();
	}
	
	public virtual void UpdateUIAbility()
	{
        if (PvpClient.Instance != null)
        {
			UpdatePvPPlayerAgainstAbility(mMainAttrib.CurAbility,mMainAttrib.AbilityMax);
		}
	}
	
	
	public override void AddSoul(int soul)
	{
		
	}
	
    public override void AddHp(int hp)
    {
        int hpmax = GetMaxHp();
        int finalhp = hp + mMainAttrib.CurHP;
        if (finalhp >= hpmax)
            finalhp = hpmax;

        if (finalhp <= 0)
        {
            mMainAttrib.CurHP = 0;
            SetIsDead(true);
        }
        else
            mMainAttrib.CurHP = finalhp;

        UpdateUIHp();
    }

    public virtual void UpdateUIHp()
    {
        // other player ui hp top
        if (PvpClient.Instance != null)
        {
            UpdatePvPPlayerAgainstHp(mMainAttrib.CurHP,mMainAttrib.HPMax);
            
			//UpdatePvPPlayerAgainstInfo(mMainAttrib.Role,mMainAttrib.Level);
        }
    }

    public override int GetMaxHp()
    {
        return GetAttrib(EPA.HPMax);
    }

    public override int GetCurrentHp()
    {
        return GetAttrib(EPA.CurHP);
    }
	
	public int GetMaxSoul()
    {
        return GetAttrib(EPA.SoulMax);
    }

    public int GetCurrentSoul()
    {
        return GetAttrib(EPA.CurSoul);
    }

    public override void AddBuff(int id)
    {
        IActionListener playerListener = UnitManager.Instance.LocalPlayer.ActionStatus.Listener;
        if (playerListener != null)
            playerListener.OnBuff(ServerId, id);
        base.AddBuff(id);
    }
	
	public static Player CreateShowPlayer(PlayerData data, GameObject parent)
	{
		MainScript.Instance.MyDebugLog("CreateShowPlayer: " + System.DateTime.Now);
        UnitBase unitBase = UnitBaseManager.Instance.GetItem(data.Attrib.Role);
        if (unitBase == null)
        {
            Debug.LogError(string.Format("Fail to find unit [{0}] from UnitBase table.", data.Attrib.Role));
            return null;
        }

        // get the prefab with: data.Attrib.Role;
        UnityEngine.Object unitRes = Resources.Load(unitBase.ShowPrefab);
        if (unitRes == null)
        {
            Debug.LogError(string.Format("Fail to load [{0}] for Role [{1}].", unitBase.ShowPrefab, data.Attrib.Role));
            return null;
        }

        GameObject showPlayer = GameObject.Instantiate(unitRes) as GameObject;
        UnitInfo unitInfo = showPlayer.GetComponent<UnitInfo>();
        if (unitInfo == null || unitInfo.Unit == null)
        {
            Debug.LogError(string.Format("Fail to find UnitInfo [{0}] for Role [{1}].", unitBase.Prefab, data.Attrib.Role));
            return null;
        }
		
		if (parent != null)
		{
	        showPlayer.transform.parent = parent.transform;
	        showPlayer.transform.localPosition = Vector3.zero;
	        showPlayer.transform.localRotation = Quaternion.identity;
	        showPlayer.transform.localScale = Vector3.one;
	        showPlayer.layer = parent.layer;
        	unitInfo.Model.gameObject.layer = parent.layer;
		}
        unitInfo.UnitType = EUnitType.EUT_OtherPlayer;
        unitInfo.UnitID = data.Attrib.Role;
        unitInfo.Level = data.Attrib.Level;
		
		MainScript.Instance.MyDebugLog("CreateShowPlayer: Equip Before" + System.DateTime.Now);
       
		Player player = unitInfo.Unit as Player;
        player.PlayerData = data;
        player.Equip();
		
		return player;
	}
	
	public void UpdatePvPPlayerAgainstInfo(int Role, int Level)
	{
		 if (FightMainWnd.Exist)
			FightMainWnd.Instance.UpdatePlayerAgainstInfo(Role,Level);
	}
	
	public void UpdatePvPPlayerAgainstAbility(int curAbility, int maxAbility)
	{
		 if (FightMainWnd.Exist)
			FightMainWnd.Instance.UpdatePlayerAgainstAbility(curAbility,maxAbility);
	}
	
	public void UpdatePvPPlayerAgainstHp(int curHp, int maxHp)
	{
		 if (FightMainWnd.Exist)
			FightMainWnd.Instance.UpdatePlayerAgainstHp(curHp, maxHp);
	}
}
