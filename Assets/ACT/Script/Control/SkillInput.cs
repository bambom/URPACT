using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SkillInput : MonoBehaviour
{
    UIFilledSprite mIcon;
    UISprite mBGSprite;
    GameObject mSkillPromptObj;
    bool mFirstCDDone = false;
    FinishLevelControl mFinishLevelControl;

    bool mEnabled = false;
    bool mSkillLinked = false;
    bool mSkillReady = false;
    bool mSelfBuff = false;
    bool mTargetBuff = false;
    float mCD = 0;
    float mSkillCD = 0;
    int mAcionCache = -1;
    int mInterruptIndex = -1;
    int mSkillStage = 1;
    SkillItem mSkillItem;
    LocalPlayer mLocalPlayer;
    List<Unit> mBuffTargets = null;
    bool[] mBuffCheckFlag = new bool[(int)EBuffMode.Max];
    ItemBase mItemBase = null;

    static int LastSkillId = 0;
    static bool EnableSkillKey = false;
    public static GameObject LastSkillObj;
    public static Transform mSegmentGrid;

    void Awake()
    {
        mBGSprite = transform.Find("bg").GetComponent<UISprite>();
        mIcon = transform.Find("icon").GetComponent<UIFilledSprite>();
        if (FightMainWnd.Exist)
        {
            mSegmentGrid = FightMainWnd.Instance.WndObject.transform.Find("SkillSegmentGrid");
            mFinishLevelControl = mSegmentGrid.GetComponent<FinishLevelControl>();
            if (mFinishLevelControl == null)
                mFinishLevelControl = mSegmentGrid.gameObject.AddComponent<FinishLevelControl>();
        }
    }

    public void Init(SkillItem skillItem)
    {
        mSkillItem = skillItem;
        mLocalPlayer = UnitManager.Instance.LocalPlayer as LocalPlayer;

        ResetSkillInfo();
        if (mSkillItem != null)
            Show(true);
        else
            Show(false);
    }

    void ResetSkillInfo()
    {
        if (mSkillItem == null)
            return;

        // setup the skill icon name.
        mItemBase = ItemBaseManager.Instance.GetItem(mSkillItem.SkillBase.ID);
        mIcon.spriteName = mItemBase.Icon;
        mBGSprite.spriteName = mItemBase.Icon; ;

        mSkillItem.SkillInput = this;
        if (!Global.GuideMode)
            mSkillItem.ApplyRune();

        mIcon.enabled = false;
        mIcon.fillAmount = 1.0f;

        mSkillCD = mSkillItem.CD;
        mCD = mSkillCD;

        mBuffCheckFlag[mSkillItem.SkillAttrib.Mode] = true;
        if (mSkillItem.RuneBase1 != null) mBuffCheckFlag[mSkillItem.RuneBase1.Mode] = true;
        if (mSkillItem.RuneBase2 != null) mBuffCheckFlag[mSkillItem.RuneBase2.Mode] = true;
        if (mSkillItem.RuneBase3 != null) mBuffCheckFlag[mSkillItem.RuneBase3.Mode] = true;
        if (mSkillItem.RuneBase4 != null) mBuffCheckFlag[mSkillItem.RuneBase4.Mode] = true;
    }

    void Show(bool enabled)
    {
        mEnabled = enabled;
        GetComponent<Collider>().enabled = mEnabled;
        mIcon.enabled = mEnabled;
        mBGSprite.enabled = mEnabled;
    }

    void OnPress(bool isPressed)
    {
        if (Global.PauseAll)
            return;

        // ֧�ּ��ܰ����ļ�⡣
        if (EnableSkillKey && mSkillItem != null && LastSkillId == mSkillItem.SkillAttrib.ID && Global.GInputBox != null)
        {
            if (isPressed)
                Global.GInputBox.OnKeyDown(EKeyList.KL_SkillAttack);
            else
                Global.GInputBox.OnKeyUp(EKeyList.KL_SkillAttack);
        }

        if (!isPressed || !mSkillReady)
            return;

        // the skill stage need check the CD.
        if (mSkillStage <= 1 && mCD < mSkillCD)
            return;

        // link skill now, it may enqueue to some time to play.
        mLocalPlayer.LinkSkill(this, mInterruptIndex);

        // ����ϵͳ��Ҫ�ļ�⼼�ܵİ���
        if (EnableSkillKey && Global.GInputBox != null)
        {
            if (isPressed)
                Global.GInputBox.OnKeyDown(EKeyList.KL_SkillAttack);
            else
                Global.GInputBox.OnKeyUp(EKeyList.KL_SkillAttack);
        }
    }

    public void PlaySkill()
    {
        // super skill cost soul, and the other cost ability.
        string skillAction = "";
        if (mSkillItem.SkillType == ESkillType.Super)
        {
            int curSoul = mLocalPlayer.GetAttrib(EPA.CurSoul);
            // the super skill cost the soul.
            if (curSoul >= mSkillItem.Energy3 && !string.IsNullOrEmpty(mSkillItem.SkillAttrib.Action3))
            {
                skillAction = mSkillItem.SkillAttrib.Action3;
                mLocalPlayer.AddSoul(-mSkillItem.Energy3);
                mSkillStage = 3;
            }
            else if (curSoul >= mSkillItem.Energy2 && !string.IsNullOrEmpty(mSkillItem.SkillAttrib.Action2))
            {
                skillAction = mSkillItem.SkillAttrib.Action2;
                mLocalPlayer.AddSoul(-mSkillItem.Energy2);
                mSkillStage = 2;
            }
            else
            {
                skillAction = mSkillItem.SkillAttrib.Action1;
                mLocalPlayer.AddSoul(-mSkillItem.Energy1);
                mSkillStage = 1;
            }
        }
        else
        {
            if (mSkillStage <= 1)
            {
                skillAction = mSkillItem.SkillAttrib.Action1;
                mLocalPlayer.AddAbility(-mSkillItem.Energy1);
            }
            else if (mSkillStage == 2)
            {
                skillAction = mSkillItem.SkillAttrib.Action2;
                mLocalPlayer.AddAbility(-mSkillItem.Energy2);
            }
            else if (mSkillStage == 3)
            {
                skillAction = mSkillItem.SkillAttrib.Action3;
                mLocalPlayer.AddAbility(-mSkillItem.Energy3);
            }
        }

        if (mBuffTargets != null)
            mBuffTargets.Clear();

        // ����Ӹ������buff
        ProcessBuff(mLocalPlayer, EBuffMode.PlaySkill);

        // ���ü��ܵĶ�����
        mSkillItem.SkillStage = mSkillStage;

        // ִ�ж�����
        mLocalPlayer.PlaySkill(mSkillItem, skillAction);

        // ����ϵͳ��Ҫ�ļ�⼼�ܵİ���
        LastSkillId = mItemBase.ID;

        mFirstCDDone = true;

        if (FightMainWnd.Exist)
            ReleaseSkill();

        // reset the cd
        mCD = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (!mEnabled)
            return;

        // update the skill cd.
        bool cdDone = UpdateCD();
        // update the skill icon.
        mSkillReady = UpdateIcon();
        // update the skillprompt.
        UpdateSkillPrompt(cdDone);

        if (!cdDone)
            mBGSprite.color = Color.white;
        else
            mBGSprite.color = mSkillReady ? Color.white : new Color(0.3f, 0.3f, 0.3f);
    }

    void ReleaseSkill()
    {
        if (mSkillItem != null && mSkillItem.SkillType != ESkillType.Super)
        {
            int curAbility = mLocalPlayer.GetAttrib(EPA.CurAbility);
            int skillSegCount = GetSegmentCount();
            if (skillSegCount > 1 && CanReleaseCurSeg())
            {
                if (gameObject != LastSkillObj)
                    ControlBtnBeat(LastSkillObj, false);
                ControlBtnBeat(gameObject, mSkillStage < skillSegCount);
                mSegmentGrid.gameObject.SetActive(true);
                int num = mSkillStage;
                num = num <= 1 ? 1 : num;
                ShowCurSkillSeg(num, skillSegCount);
                mFinishLevelControl.SkillDelayTime(1.0f);
            }
            else
            {
                ControlBtnBeat(LastSkillObj, false);
                mSegmentGrid.gameObject.SetActive(false);
            }
            LastSkillObj = gameObject;
        }
    }

    public void ControlBtnBeat(GameObject targetObj, bool isBeat)
    {
        if (targetObj != null)
        {
            Animation animation = targetObj.GetComponent<Animation>();
            if (isBeat)
            {
                if (!animation.isPlaying)
                    animation.Play();
            }
            else
            {
                if (animation != null && animation.isPlaying)
                {
                    animation.Stop();
                    targetObj.transform.localScale = new Vector3(0.85f, 0.85f, 1.0f);
                }
            }
            string objName = targetObj.name + "Panel";
            targetObj.transform.parent.Find(objName).gameObject.SetActive(isBeat);
			transform.Find("icon").gameObject.SetActive(!isBeat);
        }
    }

    bool CanReleaseCurSeg()
    {
        Debug.Log(mSkillStage);
        int curAbility = mLocalPlayer.GetAttrib(EPA.CurAbility);
        if ((mSkillStage <= 1 && curAbility >= mSkillItem.Energy2) ||
            mSkillStage == 2 ||
            mSkillStage == 3)
        {
            return true;
        }
        return false;
    }

    void ShowCurSkillSeg(int count, int segmentCount)
    {
		Transform pointParentTran = mSegmentGrid.Find("PointRoot");
		GameObject OneSegFrameObj = mSegmentGrid.Find("FrameOneSeg").gameObject;
		GameObject TwoSegFrameObj = mSegmentGrid.Find("FrameTwoSeg").gameObject;
				
		OneSegFrameObj.SetActive(!(segmentCount < 3));
		TwoSegFrameObj.SetActive(segmentCount < 3);		
        for (int i = 0; i < pointParentTran.childCount; i++)
        {
            if (i < segmentCount)
            {
                pointParentTran.GetChild(i).gameObject.SetActive(true);
                if (i < count)                                                       
                    pointParentTran.GetChild(i).GetComponent<UISprite>().spriteName = "Button30_Skill_Step_06";
                else
                    pointParentTran.GetChild(i).GetComponent<UISprite>().spriteName = "Button30_Skill_Step_05";
            }
            else
                pointParentTran.GetChild(i).gameObject.SetActive(false);
        }
    }

    void UpdateSkillPrompt(bool isCDDone)
    {
        if (mSkillItem != null && mSkillItem.SkillType == ESkillType.Super)
        {
            if (mSkillReady && isCDDone)
            {
                if (mSkillPromptObj == null)
                    CreatePromptObj();
                if (!mSkillPromptObj.activeSelf)
                {
                    mSkillPromptObj.SetActive(true);
                }
            }
            else
            {
                if (mSkillPromptObj != null)
                {
                    GameObject.Destroy(mSkillPromptObj);
                }
            }
        }

        if (mSkillItem != null && mSkillItem.SkillType != ESkillType.Super)
        {
            if (isCDDone && mFirstCDDone && mSkillReady)
            {
                if (mSkillPromptObj == null)
                    CreatePromptObj();
                mFirstCDDone = false;
                float duration = mSkillPromptObj.GetComponentInChildren<ParticleSystem>().duration;
                GameObject.Destroy(mSkillPromptObj, duration);
            }
        }
    }

    void CreatePromptObj()
    {
        mSkillPromptObj = GameObject.Instantiate(Resources.Load("SuccessEffect")) as GameObject;
        mSkillPromptObj.transform.parent = transform;
        mSkillPromptObj.transform.localPosition = new Vector3(0.0f, 0.0f, -2.0f);
        mSkillPromptObj.transform.localScale = Vector3.one;
        mSkillPromptObj.transform.localEulerAngles = Vector3.zero;
    }

    int GetSegmentCount()
    {
        int countOne = string.IsNullOrEmpty(mSkillItem.SkillAttrib.Action1) ? 0 : 1;
        int countTwo = string.IsNullOrEmpty(mSkillItem.SkillAttrib.Action2) ? 0 : 1;
        int countThree = string.IsNullOrEmpty(mSkillItem.SkillAttrib.Action3) ? 0 : 1;
        return countOne + countTwo + countThree;

    }
    bool UpdateCD()
    {
        if (mCD >= mSkillCD)
        {
            mIcon.enabled = false;
            return true;
        }

        mCD += Time.deltaTime;
        mIcon.fillAmount = Mathf.Max(1.0f - mCD / mSkillCD, 0.0f);
        mIcon.enabled = true;
        return false;
    }

    bool UpdateIcon()
    {
        // check the skill has action link
        Data.Action activeAction = mLocalPlayer.ActionStatus.ActiveAction;
        if (activeAction != null && activeAction.mActionCache != mAcionCache)
        {
            mInterruptIndex = -1;
            mAcionCache = activeAction.mActionCache;
            for (int i = 0; i < activeAction.mActionInterrupts.Count; i++)
            {
                Data.ActionInterrupt interrupt = activeAction.mActionInterrupts[i];
                if (interrupt.SkillID != mItemBase.ID)
                    continue;

                // the second stage of the skill.
                if (interrupt.CheckSkillID == 2 && string.IsNullOrEmpty(mSkillItem.SkillAttrib.Action2))
                    continue;

                // the third stage of the skill.
                if (interrupt.CheckSkillID == 3 && string.IsNullOrEmpty(mSkillItem.SkillAttrib.Action3))
                    continue;

                // update the skill icon.
                string skillIcon = mItemBase.Icon;
                if (interrupt.CheckSkillID == 2 && !string.IsNullOrEmpty(mSkillItem.SkillAttrib.Icon2))
                    skillIcon = mSkillItem.SkillAttrib.Icon2;
                else if (interrupt.CheckSkillID == 3 && !string.IsNullOrEmpty(mSkillItem.SkillAttrib.Icon3))
                    skillIcon = mSkillItem.SkillAttrib.Icon3;
                mIcon.spriteName = skillIcon;
                mBGSprite.spriteName = skillIcon;
                mSkillStage = interrupt.CheckSkillID;
                mInterruptIndex = i;
                break;
            }
            mSkillLinked = (mInterruptIndex >= 0);
        }

        //if this action does not contain any skill link.
        if (!mSkillLinked) return false;

        // check the interrupt is enabled.
        if (!mLocalPlayer.ActionStatus.GetInterruptEnabled(mInterruptIndex))
            return false;

        // check the skill has consume value.
        if (!Global.GuideMode)
        {
            if (mSkillItem.SkillType == ESkillType.Super)
            {
                int curSoul = mLocalPlayer.GetAttrib(EPA.CurSoul);
                // the super skill cost the soul.
                if (curSoul < mSkillItem.Energy1)
                    return false;
            }
            else
            {
                int curAbility = mLocalPlayer.GetAttrib(EPA.CurAbility);
                if (mSkillStage <= 1 && curAbility < mSkillItem.Energy1)
                    return false;

                if (mSkillStage == 2 && curAbility < mSkillItem.Energy2)
                    return false;

                if (mSkillStage == 3 && curAbility < mSkillItem.Energy3)
                    return false;
            }
        }

        return true;
    }

    public void OnHitTarget(Unit target)
    {
        ProcessBuff(target, EBuffMode.HitTarget);
    }

    public void OnHit(Unit target)
    {
        ProcessBuff(target, EBuffMode.OnHited);
    }

    public void OnHurt(Unit target)
    {
        ProcessBuff(target, EBuffMode.OnHurt);
    }

    void ProcessBuff(Unit target, EBuffMode mode)
    {
        if (target.Dead) return;
        // �Ż�һ�£�����ļ�ⲻ�������͵�������
        if (!mBuffCheckFlag[(int)mode]) return;

        // ����ǲ����Ѿ��й�buff�ˡ�
        if (mBuffTargets == null) mBuffTargets = new List<Unit>();
        if (mBuffTargets.Contains(target)) return;
        mBuffTargets.Add(target);

        if (mSkillItem.SkillAttrib.Mode == (int)mode)
            ProcessBuff(target, mSkillItem.SkillAttrib.Chance, mSkillItem.SkillAttrib.Target, mSkillItem.SkillAttrib.BuffID);

        ProcessRune(target, mSkillItem.RuneBase1, mode);
        ProcessRune(target, mSkillItem.RuneBase2, mode);
        ProcessRune(target, mSkillItem.RuneBase3, mode);
        ProcessRune(target, mSkillItem.RuneBase4, mode);
    }

    void ProcessRune(Unit target, RuneBase runeBase, EBuffMode mode)
    {
        if (runeBase == null || runeBase.Mode != (int)mode)
            return;

        ProcessBuff(target, runeBase.Chance, runeBase.Target, runeBase.BuffID);
    }

    void ProcessBuff(Unit target, int chance, int targetType, int id)
    {
        if (id == 0 || chance < Random.Range(0, 10001))
            return;

        if (targetType == (int)EBuffTarget.Self)
            mLocalPlayer.AddBuff(id);
        else
            target.AddBuff(id);
    }
}
