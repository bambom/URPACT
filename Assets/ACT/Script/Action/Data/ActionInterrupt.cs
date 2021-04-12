using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.ComponentModel;

namespace Data
{
    [Serializable]
    public class ActionInterrupt
    {
        const String CategoryActionSwitch = "动作切换";
        const String CategorySwitchCondition = "切换条件";
        const String CategoryInputOperation = "输入操作";
        const String CategoryDetectType = "检测类型";
        const String CategoryOther = "其它";

        // 动作切换
        String mInterruptName = "";
        String mActionID = "N0000";
        int mActionCache = 0;

        int mConnectMode = 0;
        int mConnectTime = 100;
        bool mEnabled = true;
        public int mEnableBegin = 0;
        public int mEnableEnd = 200;
        int mSkillID = 0;
        int mCheckSkillID = 0;
        bool mPlaySkill = true;
        bool mCheckSkillLv = false;
        int mSkillLv = 0;
        int mSkillCompareType = 0;

        // 受到伤害
        bool mHurted = false;
        int mHurtType = 0;
        bool mRemoteOnly = false;

        public bool mCheckAllCondition = false;

        public bool mConditionInterrupte = true;

        int mDetectType;

        // 动作输入1
        public bool mCheckInput1 = false;
        int mInputKey1 = 0;
        int mInputType1 = 0;

        // 动作输入2
        bool mCheckInput2 = false;
        int mInputKey2 = 0;
        int mInputType2 = 0;

        // 动作输入3
        bool mCheckInput3 = false;
        int mInputKey3 = 0;
        int mInputType3 = 0;

        // 动作输入4
        bool mCheckInput4 = false;
        int mInputKey4 = 0;
        int mInputType4 = 0;
        //任何情况
        bool mNoInput = false;

        // 接触地面
        public bool mTouchGround = false;

        // 接触墙壁
        public bool mTouchWall = false;

        // 到达最高点
        public bool mReachHighest = false;

        // 单位死亡
        public bool mUnitDead = false;

        // 击中目标
        public bool mHitTarget = false;
        bool mCheckHitTarget = false;
        String mHitTargetID = "";

        // 抓取结束
        bool mEndCapture = false;

        // 监控变量
        public bool mDetectVariable = false;
        int mVariable = 0;
        int mCompareType = 0;
        int mCompareValue = 0;

        // 感觉目标
        public bool mSenseTarget = false;
        public int mTargetDistanceMin = 0;
        public int mTargetDistanceMax = 1000;
        int mTargetLocalAngleMin = 0;
        int mTargetLocalAngleMax = 180;
        int mTargetWorldAngleMin = 0;
        int mTargetWorldAngleMax = 180;
        int mCheckTargetHeight = 0;

        //Buff
        String mBuffListID = "";
        String mBuffListState = "";
        int mCheckBuffID = 0;
        int mCheckBuffState = 0;
        int mCheckAllBuff = 0;

        #region "Attributes"
        [XmlAttribute("InterruptName"), DefaultValue(""), DisplayName("中断名称"), Category(CategoryActionSwitch)]
        public String InterruptName { get { return mInterruptName; } set { mInterruptName = value; } }
        [XmlAttribute("ActionID"), DisplayName("动作编号"), Category(CategoryActionSwitch)]
        public String ActionID { get { return mActionID; } set { mActionID = value; } }
        [XmlAttribute("ConnectImmediately"), Browsable(false), DefaultValue(true), DisplayName("替换方式"), Category(CategoryActionSwitch)]
        public bool ConnectImmediately { get { return (mConnectMode == 0); } set { mConnectMode = value ? 0 : 1; } }

        [XmlAttribute("ConnectMode"), DefaultValue(0), DisplayName("替换方式"), Category(CategoryActionSwitch)]
        public int ConnectMode { get { return mConnectMode; } set { mConnectMode = value; } }

        [XmlAttribute("ConnectTime"), DefaultValue(100), DisplayName("替换时间"), Category(CategoryActionSwitch)]
        public int ConnectTime { get { return mConnectTime; } set { mConnectTime = value; } }
        [XmlAttribute("Enabled"), DefaultValue(true), DisplayName("是否启用"), Category(CategoryActionSwitch)]
        public bool Enabled { get { return mEnabled; } set { mEnabled = value; } }
        [XmlAttribute("EnableBegin"), DefaultValue(0), DisplayName("启用起始时间"), Category(CategoryActionSwitch)]
        public int EnableBegin { get { return mEnableBegin; } set { mEnableBegin = value; } }
        [XmlAttribute("EnableEnd"), DefaultValue(200), DisplayName("启用结束时间"), Category(CategoryActionSwitch)]
        public int EnableEnd { get { return mEnableEnd; } set { mEnableEnd = value; } }
        [XmlAttribute("SkillID"), DefaultValue(0), DisplayName("呼叫技能"), Category(CategoryActionSwitch)]
        public int SkillID { get { return mSkillID; } set { mSkillID = value; } }
        [XmlAttribute("CheckSkillID"), DefaultValue(0), DisplayName("检测技能"), Category(CategoryActionSwitch)]
        public int CheckSkillID { get { return mCheckSkillID; } set { mCheckSkillID = value; } }
        [XmlAttribute("PlaySkill"), DefaultValue(true), DisplayName("使用技能"), Category(CategoryActionSwitch)]
        public bool PlaySkill { get { return mPlaySkill; } set { mPlaySkill = value; } }
        [XmlAttribute("CheckSkillLv"), DefaultValue(false), DisplayName("检测技能等级"), Category(CategoryActionSwitch)]
        public bool CheckSkillLv { get { return mCheckSkillLv; } set { mCheckSkillLv = value; } }
        [XmlAttribute("SkillLv"), DefaultValue(0), DisplayName("技能等级"), Category(CategoryActionSwitch)]
        public int SkillLv { get { return mSkillLv; } set { mSkillLv = value; } }
        [XmlAttribute("SkillCompareType"), DefaultValue(0), DisplayName("技能比较条件"), Category(CategoryActionSwitch)]
        public int SkillCompareType { get { return mSkillCompareType; } set { mSkillCompareType = value; } }

        [XmlAttribute("ConditionInterrupte"), DefaultValue(true), DisplayName("条件中断"), Category(CategorySwitchCondition)]
        public bool ConditionInterrupte { get { return mConditionInterrupte; } set { mConditionInterrupte = value; } }

        [XmlAttribute("CheckAllCondition"), DefaultValue(false), DisplayName("切换条件"), Category(CategorySwitchCondition)]
        public bool CheckAllCondition { get { return mCheckAllCondition; } set { mCheckAllCondition = value; } }

        [XmlAttribute("DetectType"), DefaultValue(0), DisplayName("监控目标类型1"), Category(CategoryDetectType)]
        public int DetectType { get { return mDetectType; } set { mDetectType = value;  } }

        [XmlAttribute("CheckInput1"), DefaultValue(false), DisplayName("输入操作1"), Category(CategoryInputOperation)]
        public bool CheckInput1 { get { return mCheckInput1; } set { mCheckInput1 = value; } }
        [XmlAttribute("InputKey1"), DefaultValue(0), DisplayName("按钮1"), Category(CategoryInputOperation)]
        public int InputKey1 { get { return mInputKey1; } set { mInputKey1 = value; } }
        [XmlAttribute("InputType1"), DefaultValue(0), DisplayName("按钮状态1"), Category(CategoryInputOperation)]
        public int InputType1 { get { return mInputType1; } set { mInputType1 = value; } }
        [XmlAttribute("NoInput"), DefaultValue(false), DisplayName("任何情况"), Category(CategoryInputOperation)]
        public bool NoInput { get { return mNoInput; } set { mNoInput = value; } }

        [XmlAttribute("CheckInput2"), DefaultValue(false), DisplayName("输入操作2"), Category(CategoryInputOperation)]
        public bool CheckInput2 { get { return mCheckInput2; } set { mCheckInput2 = value; } }
        [XmlAttribute("InputKey2"), DefaultValue(0), DisplayName("按钮2"), Category(CategoryInputOperation)]
        public int InputKey2 { get { return mInputKey2; } set { mInputKey2 = value; } }
        [XmlAttribute("InputType2"), DefaultValue(0), DisplayName("按钮状态2"), Category(CategoryInputOperation)]
        public int InputType2 { get { return mInputType2; } set { mInputType2 = value; } }

        [XmlAttribute("CheckInput3"), DefaultValue(false), DisplayName("输入操作3"), Category(CategoryInputOperation)]
        public bool CheckInput3 { get { return mCheckInput3; } set { mCheckInput3 = value; } }
        [XmlAttribute("InputKey3"), DefaultValue(0), DisplayName("按钮3"), Category(CategoryInputOperation)]
        public int InputKey3 { get { return mInputKey3; } set { mInputKey3 = value; } }
        [XmlAttribute("InputType3"), DefaultValue(0), DisplayName("按钮状态3"), Category(CategoryInputOperation)]
        public int InputType3 { get { return mInputType3; } set { mInputType3 = value; } }

        [XmlAttribute("CheckInput4"), DefaultValue(false), DisplayName("输入操作4"), Category(CategoryInputOperation)]
        public bool CheckInput4 { get { return mCheckInput4; } set { mCheckInput4 = value; } }
        [XmlAttribute("InputKey4"), DefaultValue(0), DisplayName("按钮4"), Category(CategoryInputOperation)]
        public int InputKey4 { get { return mInputKey4; } set { mInputKey4 = value; } }
        [XmlAttribute("InputType4"), DefaultValue(0), DisplayName("按钮状态4"), Category(CategoryInputOperation)]
        public int InputType4 { get { return mInputType4; } set { mInputType4 = value; } }

        [XmlAttribute("TouchGround"), DefaultValue(false), DisplayName("接触地面"), Category(CategoryOther)]
        public bool TouchGround { get { return mTouchGround; } set { mTouchGround = value; } }
        [XmlAttribute("TouchWall"), DefaultValue(false), DisplayName("接触墙壁"), Category(CategoryOther)]
        public bool TouchWall { get { return mTouchWall; } set { mTouchWall = value; } }
        [XmlAttribute("ReachHighest"), DefaultValue(false), DisplayName("到达最高点"), Category(CategoryOther)]
        public bool ReachHighest { get { return mReachHighest; } set { mReachHighest = value; } }
        [XmlAttribute("UnitDead"), DefaultValue(false), DisplayName("单位死亡"), Category(CategoryOther)]
        public bool UnitDead { get { return mUnitDead; } set { mUnitDead = value; } }
        [XmlAttribute("HitTarget"), DefaultValue(false), DisplayName("击中目标"), Category(CategoryOther)]
        public bool HitTarget { get { return mHitTarget; } set { mHitTarget = value; } }
        [XmlAttribute("EndCapture"), DefaultValue(false), DisplayName("抓取结束"), Category(CategoryOther)]
        public bool EndCapture { get { return mEndCapture; } set { mEndCapture = value; } }

        [XmlAttribute("CheckHitTarget"), DefaultValue(false), DisplayName("检测击中目标目标"), Category(CategoryOther)]
        public bool CheckHitTarget { get { return mCheckHitTarget; } set { mCheckHitTarget = value; } }
        [XmlAttribute("HitTargetID"), DefaultValue(""), DisplayName("击中目标ID"), Category(CategoryOther)]
        public String HitTargetID { get { return mHitTargetID; } set { mHitTargetID = value; } }

        [XmlAttribute("SenseTarget"), DefaultValue(false), DisplayName("感觉目标"), Category(CategoryOther)]
        public bool SenseTarget { get { return mSenseTarget; } set { mSenseTarget = value; } }
        [XmlAttribute("TargetDistanceMin"), DefaultValue(0), DisplayName("目标距离最小值"), Category(CategoryOther)]
        public int TargetDistanceMin { get { return mTargetDistanceMin; } set { mTargetDistanceMin = value; } }
        [XmlAttribute("TargetDistanceMax"), DefaultValue(10000), DisplayName("目标距离最大值"), Category(CategoryOther)]
        public int TargetDistanceMax { get { return mTargetDistanceMax; } set { mTargetDistanceMax = value; } }
        [XmlAttribute("TargetLocalAngleMin"), DefaultValue(0), DisplayName("目标朝向夹角最小值"), Category(CategoryOther)]
        public int TargetLocalAngleMin { get { return mTargetLocalAngleMin; } set { mTargetLocalAngleMin = value; } }
        [XmlAttribute("TargetLocalAngleMax"), DefaultValue(180), DisplayName("目标朝向夹角最大值"), Category(CategoryOther)]
        public int TargetLocalAngleMax { get { return mTargetLocalAngleMax; } set { mTargetLocalAngleMax = value; } }
        [XmlAttribute("TargetWorldAngleMin"), DefaultValue(0), DisplayName("目标位置夹角最小值"), Category(CategoryOther)]
        public int TargetWorldAngleMin { get { return mTargetWorldAngleMin; } set { mTargetWorldAngleMin = value; } }
        [XmlAttribute("TargetWorldAngleMax"), DefaultValue(180), DisplayName("目标位置夹角最大值"), Category(CategoryOther)]
        public int TargetWorldAngleMax { get { return mTargetWorldAngleMax; } set { mTargetWorldAngleMax = value; } }
        [XmlAttribute("CheckTargetHeight"), DefaultValue(0), DisplayName("检查目标高度"), Category(CategoryOther)]
        public int CheckTargetHeight { get { return mCheckTargetHeight; } set { mCheckTargetHeight = value; } }

        [XmlAttribute("Hurted"), DefaultValue(false), DisplayName("受到伤害"), Category(CategoryOther)]
        public bool Hurted { get { return mHurted; } set { mHurted = value; } }
        [XmlAttribute("HurtType"), DefaultValue(0), DisplayName("受到伤害种类"), Category(CategoryOther)]
        public int HurtType { get { return mHurtType; } set { mHurtType = value; } }
        [XmlAttribute("RemoteOnly"), DefaultValue(false), DisplayName("仅远程攻击"), Category(CategoryOther)]
        public bool RemoteOnly { get { return mRemoteOnly; } set { mRemoteOnly = value; } }

        [XmlAttribute("DetectVariable"), DefaultValue(false), DisplayName("监控变量"), Category(CategoryOther)]
        public bool DetectVariable { get { return mDetectVariable; } set { mDetectVariable = value; } }
        [XmlAttribute("Variable"), DefaultValue(0), DisplayName("监控变量名称"), Category(CategoryOther)]
        public int Variable { get { return mVariable; } set { mVariable = value; } }
        [XmlAttribute("CompareType"), DefaultValue(0), DisplayName("监控变量比较条件"), Category(CategoryOther)]
        public int CompareType { get { return mCompareType; } set { mCompareType = value; } }
        [XmlAttribute("CompareValue"), DefaultValue(0), DisplayName("监控变量比较数值"), Category(CategoryOther)]
        public int CompareValue { get { return mCompareValue; } set { mCompareValue = value; } }

        [XmlAttribute("CheckBuffID"), DefaultValue(0), DisplayName("检测BufferID"), Category(CategoryOther)]
        public int CheckBuffID { get { return mCheckBuffID; } set { mCheckBuffID = value; } }
        [XmlAttribute("CheckBuffState"), DefaultValue(0), DisplayName("检测Buffer状态"), Category(CategoryOther)]
        public int CheckBuffState { get { return mCheckBuffState; } set { mCheckBuffState = value; } }
        [XmlAttribute("CheckAllBuff"), DefaultValue(0), DisplayName("检查所有Buffer"), Category(CategoryOther)]
        public int CheckAllBuff { get { return mCheckAllBuff; } set { mCheckAllBuff = value; } }
        [XmlAttribute("BuffListID"), DefaultValue(""), DisplayName("中断buff ID"), Category(CategoryOther)]
        public String BuffListID { get { return mBuffListID; } set { mBuffListID = value; } }
        [XmlAttribute("BuffListState"), DefaultValue(""), DisplayName("中断buff状态"), Category(CategoryOther)]
        public String BuffListState { get { return mBuffListState; } set { mBuffListState = value; } }

        #endregion

        [XmlAttribute("ActionCache"), DefaultValue(0)]
        public int ActionCache { get { return mActionCache; } set { mActionCache = value; } }

        public override string ToString()
        {
            return mActionID + "(" + mInterruptName + ")";
        }
    }
}
