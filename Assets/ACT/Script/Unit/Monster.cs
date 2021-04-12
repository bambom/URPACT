using System.Collections.Generic;
using UnityEngine;


public class Monster : Unit
{
    int mHP = 0;
    int mHPMax = 0;
    MonsterAttrib mMonsterAttrib;
	bool mIsBoss = false;
	string mName; 
	GuideFightWnd mGuideWnd;
    public Monster(UnitInfo unitInfo, int level)
        : base(unitInfo, level)
    {
    }

    public override void Init()
    {
        SetIsDead(false);

        State = UnitState.Normal;

        UpdateAttributes();
		
		UnitBase unit = UnitBaseManager.Instance.GetItem(UUnitInfo.UnitID);
		if(UUnitInfo.PoolIndex == 4 ) // boss.
			mIsBoss = true;
		mName = unit.Name;

        if (MyTriggerCell != null)
            MyTriggerCell.SendMessage("UnitSpawned", this);

        base.Init();
    }

    public override void UpdateAttributes()
    {
        mMonsterAttrib = MonsterAttribManager.Instance.GetItem(UUnitInfo.UnitID, Level);
        if (mMonsterAttrib != null)
        {
            mHP = mMonsterAttrib.HPMax;
            mHPMax = mMonsterAttrib.HPMax;
        }
        else
        {
            mHP = 1000;
            mHPMax = 1000;
        }
    }

    public override int GetAttrib(EPA idx)
    {
        int ret = 0;
        switch (idx)
        {
            // 即时值直接返回，不用计算buff加成。
            case EPA.CurHP: return mHP;
            case EPA.Level: return Level;
            //------------------------------------------------
            case EPA.HPMax: ret = mHPMax; break;
            //------------------------------------------------
            case EPA.Damage: ret = mMonsterAttrib.Damage; break;
            case EPA.Defense: ret = mMonsterAttrib.Defense; break;
            //------------------------------------------------
            case EPA.SpecialDamage: ret = mMonsterAttrib.SpecialDamage; break;
            case EPA.SpecialDefense: ret = mMonsterAttrib.SpecialDefense; break;
            //------------------------------------------------
            case EPA.Critical: ret = mMonsterAttrib.Critical; break;
            case EPA.Block: ret = mMonsterAttrib.Block; break;
            case EPA.Hit: ret = mMonsterAttrib.Hit; break;
            case EPA.Tough: ret = mMonsterAttrib.Tough; break;
            default: return 0;
        }
        return mBuffManager.Apply(idx, ret);
    }

    public override void AddHp(int hp)
    {
        if (State == UnitState.Die) 
            return;

        mHP = Mathf.Clamp(mHP + hp, 0, GetAttrib(EPA.HPMax));
        if (mHP <= 0)
            SetIsDead(true);

        UpdateMonsterHp();
    }

    public override bool Hurt(Unit attacker, int damage, ECombatResult result)
    {
        if (State == UnitState.Die)
            return false;

        mHP -= damage;
        if (mHP <= 0)
            SetIsDead(true);

        // hit effects.
        if (UUnitInfo.Model)
            GameEventManager.Instance.EnQueue(new OnHitEffectEvent(UUnitInfo.Model.gameObject));

        // update the hp bar.
        if (UUnitInfo.UnitTopUI)
        {
            UUnitInfo.UnitTopUI.OnInfoPopup(damage.ToString(), result, attacker);
        }

        UpdateMonsterHp();
        return true;
    }

    public override void Update(float deltaTime)
    {
        base.Update(deltaTime);

        // 怪物掉下去死亡～～～
        if (!Dead && Position.y < -50)
            Hurt(null, 99999, ECombatResult.ECR_Normal);
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
    }

    protected override void OnDead()
    {
        base.OnDead();

        if (MyTriggerCell != null)
            MyTriggerCell.SendMessage("UnitDie", this);

        ProcessDeath();
    }

    void ProcessDeath()
    {
        // setup the death action.
        if (ActionStatus != null && ActionStatus.HeightState == Data.HeightStatusFlag.Stand)
            ActionStatus.ChangeAction(ActionStatus.ActionGroup.StandDeath, 0);

        // update hp silder.
        if (UUnitInfo.UnitTopUI)
            UUnitInfo.UnitTopUI.OnHpUpdate();

        // the level profit for player.
        ProcessProfit();

        // drop items.
        ProcessDropItem();

        if( mIsBoss ) // boss.
		{
			if(Global.GuideMode && mGuideWnd != null)
				mGuideWnd.BossDied();
			else
			{
				if(FightMainWnd.Exist)
	            	FightMainWnd.Instance.BossDied();
			}
		}
    }

    void ProcessProfit()
    {
        if (Global.GLevelData == null ||
            Global.GLevelData.Profit == null ||
            mMonsterAttrib == null ||
            GameLevel.Instance == null)
            return;

        // gold
        if (GameLevel.Instance.TotalGoldWeight > 0)
        {
            int gold = Global.GLevelData.Profit.Gold * mMonsterAttrib.GoldWeight / GameLevel.Instance.TotalGoldWeight;
            if (gold > 0)
            {
                GameLevel.Instance.CurGold += gold;
                //UUnitInfo.UnitTopUI.OnInfoPopup("$+" + gold, UnitTopUI.GoldColor);
            }
        }

        // exp
        if (GameLevel.Instance.TotalExpWeight > 0)
        {
            int exp = Global.GLevelData.Profit.Exp * mMonsterAttrib.ExpWeight / GameLevel.Instance.TotalExpWeight;
            if (exp > 0)
            {
                GameLevel.Instance.CurExp += exp;
                //UUnitInfo.UnitTopUI.OnInfoPopup("Exp+" + exp, UnitTopUI.ExpColor, 0.3f);
            }
        }

        // soul
        if (GameLevel.Instance.TotalSoulWeight > 0)
        {
            int soul = Global.GLevelData.Profit.Soul * mMonsterAttrib.SoulWeight / GameLevel.Instance.TotalSoulWeight;
            if (soul > 0)
                GameLevel.Instance.SpawnSoulBall(Position, soul);
        }
    }

    void ProcessDropItem()
    {
        if (UUnitInfo.DropInfo == null)
            return;
		System.Random rand = new System.Random();
		foreach(DropItemInfo dropItem in UUnitInfo.DropInfo)
		{
	        GameObject item = GameObject.Find("DropPool").gameObject.GetComponent<ItemPool>().Get();
	        if (item == null)
	            return;
			GameObject playerObj = UnitManager.Instance.LocalPlayer.UUnitInfo.gameObject;
			float playerHeight = playerObj.GetComponent<CharacterController>().height;
			float itemHeight = UnitManager.Instance.LocalPlayer.UUnitInfo.gameObject.transform.Find("shadow").position.y + (playerHeight * 0.05f);
	        Vector3 position = UUnitInfo.gameObject.transform.position;
	        item.transform.position = new Vector3((float)(position.x + rand.Next(-1,1) + rand.NextDouble()), 
				itemHeight, (float)(position.z + rand.Next(-1,1) + rand.NextDouble()));
	        item.GetComponent<DropItem>().Init(dropItem);
			ItemBase itemBase = ItemBaseManager.Instance.GetItem(dropItem.ItemID);
			if(itemBase.MainType != (int)EItemType.Props || itemBase.SubType != 3 )
			{
				item.GetComponent<BoxCollider>().size *= 3; 
			}
		}
    }

    void UpdateMonsterHp()
    {
        if (UUnitInfo.UnitTopUI)
        {
            UUnitInfo.UnitTopUI.OnHpUpdate();
            UUnitInfo.UnitTopUI.OnHpUpdate();
        }

        if (mIsBoss)
        {
            if (Global.GuideMode)
            {
                mGuideWnd = mGuideWnd == null ? GameObject.Find("GuideFightWnd").GetComponent<GuideFightWnd>() : mGuideWnd;
                mGuideWnd.UpdateBossHp(mName, UUnitInfo.Level, mHP, mHPMax);
            }
            else if (FightMainWnd.Exist)
                FightMainWnd.Instance.UpdateBossHp(mName, UUnitInfo.Level, mHP, mHPMax);
        }
        else
        {
            if (FightMainWnd.Exist && MainScript.Instance.GmMode == EGameMode.GMDebug)
                FightMainWnd.Instance.UpdateMonsterHp(mHP, mHPMax);
        }
    }
}