using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class LocalPlayer : Player
{
    Controller mController;
    DownTile mDownTile;
    int mHitCount;
	GuideFightWnd mGuideWnd;

    public int HitCount { get { return mHitCount; } }

    public LocalPlayer(UnitInfo unitInfo, int level)
        : base(unitInfo, level)
    {
        if (MainScript.Instance != null)
            mPlayerData = PlayerDataManager.Instance.Data;

        mDownTile = unitInfo.GetComponentInChildren<DownTile>();
    }

    public override void Init()
    {
        base.Init();

        mHitCount = 0;
        mController = UGameObject.GetComponent<Controller>();
    }

    public override void Start()
    {
        base.Start();
    }

    public override void UpdateAttributes()
    {
        base.UpdateAttributes();

        UpdateUIHp();
		UpdateUIAbility();
		
        mDownTile.OnSoulChanged(GetAttrib(EPA.CurSoul), GetAttrib(EPA.AbilityMax));
    }

    public override void Revive()
    {
        base.Revive();

        if (mController && mController.RespawnEffect)
            GameObject.Instantiate(mController.RespawnEffect, Position, Quaternion.identity);

        mBuffManager.AddBuff(Global.ReviveBuffId);

        UpdateUIHp();
    }

    public override ECombatResult Combat(Unit target, int damageCoff, int damageBase, bool skillAttack, int actionCoff)
    {
        ECombatResult ret = base.Combat(target, damageCoff, damageBase, skillAttack, actionCoff);
        if (UUnitInfo.UnitTopUI)
        {
            if (ret == ECombatResult.ECR_Critical)
                UUnitInfo.UnitTopUI.OnInfoPopup("@", ret, UUnitInfo.Unit);
            else if (ret == ECombatResult.ECR_Block)
                UUnitInfo.UnitTopUI.OnInfoPopup("Block", ret, target);
        }

        return ret;
    }

    public override bool Hurt(Unit attacker, int damage, ECombatResult result)
    {
        if (Global.PauseAll)
            return false;

        if (State == UnitState.Die)
            return false;

        if (mController != null && mController.InputLocked)
            return false;

        // check buff settings.
        if (ActionStatus.SkillItem != null)
            ActionStatus.SkillItem.SkillInput.OnHurt(attacker);

        // build the damage here.
		if(!Global.GuideMode || GetCurrentHp() - damage > 0)
			AddHp(-damage);

        mHitCount++;
		
        UpdateUIHp();

        if (UUnitInfo.UnitTopUI)
            UUnitInfo.UnitTopUI.OnInfoPopup(damage.ToString(), result, attacker);

        // just send hurt command.
        if (ActionStatus.Listener != null)
            ActionStatus.Listener.OnHurt(damage);

        GameEventManager.Instance.EnQueue(new OnHitEffectEvent(UUnitInfo.Model.gameObject));

        return true;
    }

    protected override void OnDead()
    {
        ///战斗窗体消失
        if (FightMainWnd.Exist)
            FightMainWnd.Instance.Hide();

        ///打开新窗体
        if (GameLevel.Instance != null)
            GameLevel.Instance.OnPlayerDead();
    }

    public override void AddSoul(int value)
    {
        int maxSoul = GetAttrib(EPA.SoulMax);
        mMainAttrib.CurSoul = Mathf.Clamp(
            mMainAttrib.CurSoul + value,
            0,
            maxSoul);

        UpdateUISoul(mMainAttrib.CurSoul, maxSoul);

        // just send hurt command.
        if (ActionStatus.Listener != null)
            ActionStatus.Listener.OnAttribChanged(EPA.CurSoul);
    }

    public override void AddAbility(int value)
    {
        int abilityMax = GetAttrib(EPA.AbilityMax);
        mMainAttrib.CurAbility = Mathf.Clamp(
            mMainAttrib.CurAbility + value, 
            0,
            abilityMax);

        if (FightMainWnd.Exist)
            FightMainWnd.Instance.UpdateAbility(mMainAttrib.CurAbility, abilityMax);

        // just send hurt command.
        if (ActionStatus.Listener != null)
            ActionStatus.Listener.OnAttribChanged(EPA.CurAbility);
    }

    public override bool UpLevel()
    {
        mMainAttrib.Level += 1;
        Level = mMainAttrib.Level;

        UpdateAttributes();

        mMainAttrib.CurHP = GetAttrib(EPA.HPMax);

        UpdateUIHp();
        return true;
    }

    void UpdateUISoul(int curSoul, int maxSoul)
    {
        if (FightMainWnd.Exist)
            FightMainWnd.Instance.UpdateSoul(mDownTile, curSoul, maxSoul);
	}

    public override void UpdateUIHp()
    {
		if (Global.GuideMode)
		{
			mGuideWnd = mGuideWnd == null ? GameObject.Find("GuideFightWnd").GetComponent<GuideFightWnd>() : mGuideWnd;
			mGuideWnd.UpdateHp(GetCurrentHp(), GetMaxHp());
		}
		else
		{
			if (FightMainWnd.Exist)
				FightMainWnd.Instance.UpdateHp(GetCurrentHp(), GetMaxHp());
		}
    }

    public void LinkSkill(SkillInput skillInput, int interruptIndex)
    {
        ActionStatus.LinkAction(
            ActionStatus.ActiveAction.mActionInterrupts[interruptIndex],
            skillInput);
    }

    public void PlaySkill(SkillItem skillItem, string action)
    {
        ActionStatus.SkillItem = skillItem;
        PlayAction(action);
    }
}

