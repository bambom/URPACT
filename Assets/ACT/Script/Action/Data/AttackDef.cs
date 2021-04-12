using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Reflection;
using System.Xml.Serialization;


namespace Data
{
    [Serializable]
    public class AttackDef
    {
        public const String CATEGORY_HITEDJUDGE = "击中判定";
        public const String CATEGORY_HITRESULT = "击中结果";
        public const String CATEGORY_APPEARENCE = "外观";
        public const String CATEGORY_HITFRAME = "技能击中框";
        public const String CATEGORY_ATTACKER = "攻击者";
        public const String CATEGORY_ATTACKEE = "受击者";
        public const String CATEGORY_ATTACKEDAMAGE = "击中伤害";
        public const String CATEGORY_SKILL_LEVEL = "技能等级";

        [Serializable]
        [TypeConverterAttribute(typeof(ExpandableObjectConverter))]
        public class HitResultData
        {
            bool mEnabled = false;
            [Category(CATEGORY_HITRESULT)]
            [DisplayName("是否启用")]
            [Description("如果启用，改受击者对应的状态将采用对应的冲击速度/冲击时间/硬直时间，否则将采用默认的设置。")]
            [DefaultValue(false)]
            [XmlAttribute("Enabled")]
            public bool Enabled { get { return mEnabled; } set { mEnabled = value; } }

            Vector3 mAttackeeLash = Vector3.Zero;
            [Category(CATEGORY_HITRESULT)]
            [DisplayName("受击者冲击速度")]
            public Vector3 AttackeeLash { get { return mAttackeeLash; } set { mAttackeeLash = value; } }

            int mAttackeeTime = 100;
            [Category(CATEGORY_HITRESULT)]
            [DisplayName("受击者冲击时间")]
            [DefaultValue(100)]
            [XmlAttribute("AttackeeTime")]
            public int AttackeeTime { get { return mAttackeeTime; } set { mAttackeeTime = value; } }

            int mAttackeeStraightTime = 0;
            [Category(CATEGORY_HITRESULT)]
            [DisplayName("受击者的硬直时间")]
            [DefaultValue(0)]
            [XmlAttribute("AttackeeStraightTime")]
            public int AttackeeStraightTime { get { return mAttackeeStraightTime; } set { mAttackeeStraightTime = value; } }

            public override string ToString()
            {
                return Enabled ? "启用" : "";
            }
        };

        [Serializable]
        [TypeConverterAttribute(typeof(ExpandableObjectConverter))]
        public class PathNode
        {
            int mRatio = 0;
            [XmlAttribute("Ratio"), DefaultValue(0), DisplayName("百分比")]
            public int Ratio { get { return mRatio; } set { mRatio = value; } }

            int mX = 0;
            [XmlAttribute("X"), DefaultValue(0), DisplayName("位置X")]
            public int X { get { return mX; } set { mX = value; } }

            int mY = 0;
            [XmlAttribute("Y"), DefaultValue(0), DisplayName("位置Y")]
            public int Y { get { return mY; } set { mY = value; } }

            int mZ = 0;
            [XmlAttribute("Z"), DefaultValue(0), DisplayName("位置Z")]
            public int Z { get { return mZ; } set { mZ = value; } }

            public override string ToString()
            {
                return String.Format("{0}:[{1},{2},{3}]", Ratio, X, Y, Z);
            }
        };


        String mID = "";
        [Description("击中定义ID")]
        [DisplayName("击中定义ID")]
        public String ID { get { return mID; } set { mID = value; } }

        String mName = "";
        [Description("当前攻击定义名字")]
        [DisplayName("当前攻击定义名字")]
        public String Name { get { return mName; } set { mName = value; } }

        int mEnableSkillBuffer = 0;
        [Description("启用技能的buffer")]
        [DisplayName("启用技能的buffer")]
        public int EnableSkillBuffer { get { return mEnableSkillBuffer; } set { mEnableSkillBuffer = value; } }

        int mAlignmentTarget = 0;
        [Description("对齐攻击目标")]
        [DisplayName("对齐攻击目标")]
        public int AlignmentTarget { get { return mAlignmentTarget; } set { mAlignmentTarget = value; } }

        #region CATEGORY_HITEDJUDGE

        int mEventOnly = 0;
        [Category(CATEGORY_HITEDJUDGE)]
        [Description("该攻击定义是否仅能通过事件来触发如[AttackTargets]。")]
        [DisplayName("仅事件触发")]
        [DefaultValue(0)]
        public int EventOnly { get { return mEventOnly; } set { mEventOnly = (value != 0) ? 1 : 0; } }

        int mIsCapture = 0;
        [DisplayName("是否抓取")]
        [Description("该攻击定义是否为抓取技能，0为不是，1为抓取技能")]
        [DefaultValue(0)]
        [Category(CATEGORY_HITEDJUDGE)]
        public int IsCapture { get { return mIsCapture; } set { mIsCapture = (value != 0) ? 1 : 0; } }

        int mKeepLocal = 0;
        [Category(CATEGORY_HITEDJUDGE)]
        [DisplayName("相对位移"), DefaultValue(0), Description("攻击定义相对施放者位移")]
        public int KeepLocal { get { return mKeepLocal; } set { mKeepLocal = (value != 0) ? 1 : 0; } }

        int mPathInterpolation = 0;
        [Category(CATEGORY_HITEDJUDGE)]
        [Description("0为线性插值，1为贝塞尔曲线插值")]
        [DisplayName("路径插值")]
        [DefaultValue(0)]
        public int PathInterpolation { get { return mPathInterpolation; } set { mPathInterpolation = value; } }

        int mRotateSpeed = 0;
        [Category(CATEGORY_HITEDJUDGE)]
        [Description("旋转速度")]
        [DisplayName("旋转速度")]
        [DefaultValue(0)]
        public int RotateSpeed { get { return mRotateSpeed; } set { mRotateSpeed = value; } }

        int mAttachUnitRotate = 0;
        [Category(CATEGORY_HITEDJUDGE)]
        [Description("抓取单位跟随旋转")]
        [DisplayName("抓取单位跟随旋转")]
        [DefaultValue(0)]
        public int AttachUnitRotate { get { return mAttachUnitRotate; } set { mAttachUnitRotate = value; } }

        int mAttachUnitKeepLocal = 0;
        [Category(CATEGORY_HITEDJUDGE)]
        [Description("抓取单位KeepLocal")]
        [DisplayName("抓取单位KeepLocal")]
        [DefaultValue(0)]
        public int AttachUnitKeepLocal { get { return mAttachUnitKeepLocal; } set { mAttachUnitKeepLocal = value; } }

        int mAttachUnitSpeedY = 0;
        [Category(CATEGORY_HITEDJUDGE)]
        [Description("抓取后竖直方向上的速度")]
        [DisplayName("抓取后竖直方向上的速度")]
        [DefaultValue(0)]
        public int AttachUnitSpeedY { get { return mAttachUnitSpeedY; } set { mAttachUnitSpeedY = value; } }

        int mAttachUnitSpeedCenter = 0;
        [Category(CATEGORY_HITEDJUDGE)]
        [Description("抓取后移向中心的速度")]
        [DisplayName("抓取后移向中心的速度")]
        [DefaultValue(0)]
        public int AttachUnitSpeedCenter { get { return mAttachUnitSpeedCenter; } set { mAttachUnitSpeedCenter = value; } }

        int mMaxCountOuteDate = 1;
        [Category(CATEGORY_HITEDJUDGE)]
        [Description("达到最大次数后消失，0不消失,1消失")]
        [DisplayName("达到最大次数后消失")]
        [DefaultValue(1)]
        public int MaxCountOuteDate { get { return mMaxCountOuteDate; } set { mMaxCountOuteDate = value; } }

        int mOwnerActionChange = 0;
        [Category(CATEGORY_HITEDJUDGE)]
        [Description("0:与释放者没有任何关系;1:释放者动作改变;2:释放者动作状态等于受伤")]
        [DisplayName("释放者动作改变 ")]
        [DefaultValue(0)]
        public int OwnerActionChange { get { return mOwnerActionChange; } set { mOwnerActionChange = value; } }

        RaceType mRace = RaceType.Enemy;
        [Category(CATEGORY_HITEDJUDGE)]
        [Description("对目标敌方,友方还是自己产生伤害")]
        [DisplayName("阵营")]
        [DefaultValue(RaceType.Enemy)]
        public RaceType Race { get { return mRace; } set { mRace = value; } }


        [Browsable(false)]
        [DefaultValue((int)RaceType.Enemy)]
        public int RaceTypeInt { get { return (int)Race; } set { } }
        
        int mDelay = 0;
        [Category(CATEGORY_HITEDJUDGE)]
        [Description("该攻击定义产生之后，延迟该时间之后开始计算伤害。")]
        [DisplayName("延迟时间")]
        [DefaultValue(0)]
        public int Delay { get { return mDelay; } set { mDelay = value; } }

        int mTriggerTime = 0;
        [Description("事件触发的时间点(百分比)，数值为[0-100-200]，[0-100]期间是处于动画时间段里面的百分比，[100-200]期间是出于POSE时间段里面的百分比")]
        [DisplayName("触发时间")]
        [DefaultValue(0)]
        [Category(CATEGORY_HITEDJUDGE)]
        public int TriggerTime { get { return mTriggerTime; } set { mTriggerTime = value; } }

        int mDuration = 0;
        [Category(CATEGORY_HITEDJUDGE)]
        [Description("持续时间")]
        [DisplayName("持续时间")]
        [DefaultValue(0)]
        public int Duration { get { return mDuration; } set { mDuration = value; } }

        Vector3 mMovingSpeed = Vector3.Zero;
        [TypeConverterAttribute(typeof(Vector3))]
        [DisplayName("移动速度"), Category(CATEGORY_HITEDJUDGE), Browsable(false)]
        public Vector3 MovingSpeed { get { return mMovingSpeed; } set { mMovingSpeed = value; } }

        List<PathNode> mPath = new List<PathNode>();
        [DisplayName("移动路径"), Category(CATEGORY_HITEDJUDGE)]
        public List<PathNode> Path { get { return mPath; } }

        int mHitCount = 1;
        [Category(CATEGORY_HITEDJUDGE)]
        [Description("在攻击定义的生存周期里面，改攻击判定的总共的攻击次数")]
        [DisplayName("攻击频率")]
        [DefaultValue(1)]
        public int HitCount { get { return mHitCount; } set { mHitCount = value; } }

        int mPassNum = -1;
        [Category(CATEGORY_HITEDJUDGE)]
        [Description("一个攻击判定对[一个单位]的伤害判断次数，超过该次数攻击定义将对改单位无效。默认值为-1代表无穿透次数限制")]
        [DisplayName("最大穿透次数")]
        [DefaultValue(-1)]
        public int PassNum { get { return mPassNum; } set { mPassNum = value; } }

        int mMaxHitCount = -1;
        [Category(CATEGORY_HITEDJUDGE)]
        [Description("一个攻击判定最大的攻击次数，累加到这个值的时候，该攻击判定将会被终止。默认值为-1代表无最大击中次数限制")]
        [DisplayName("最大伤害次数")]
        [DefaultValue(-1)]
        public int MaxHitCount { get { return mMaxHitCount; } set { mMaxHitCount = value; } }

        Vector3 mFrameSize = Vector3.Zero;
        [Category(CATEGORY_HITEDJUDGE)]
        [Browsable(true)]
        [Description("技能击中框(长方体分别对应:宽,高,长; 圆柱体前2个参数分别为半径和高度，第三个无效;圆环为内半径,外半径,高度)")]
        [DisplayName("技能击中框")]
        public Vector3 FrameSize { get { return mFrameSize; } set { mFrameSize = value; } }

        Vector3 mFrameFinalFactor = new Vector3(1, 1, 1);
        [Category(CATEGORY_HITEDJUDGE)]
        [Browsable(true)]
        [Description("最终的技能框相对于初始技能框架的缩放比例:圆柱为半径，高度;圆环为内半径,外半径,高度")]
        [DisplayName("技能框缩放比例")]
        public Vector3 FrameFinalFactor { get { return mFrameFinalFactor; } set { mFrameFinalFactor = value; } }

        [Browsable(false), DefaultValue(0)]
        public UInt16 FrameType_Int { get { return (UInt16)mFramType; } set { mFramType = (HitDefnitionFramType)value; } }

        private HitDefnitionFramType mFramType = HitDefnitionFramType.CuboidType;
        [Category(CATEGORY_HITEDJUDGE)]
        [Description("技能击中框的类型:CuboidType为长方体;CylinderType为立方体; RingType:为圆环形; SomatoType:为受击体")]
        [DisplayName("技能击中框的类型")]
        [DefaultValue(HitDefnitionFramType.CuboidType)]
        public HitDefnitionFramType FramType 
        { 
            get { return mFramType; }  
            set 
            { 
                mFramType = value;
                switch (value)
                {
                    case HitDefnitionFramType.CuboidType:
                        mAttackType = new FrameCuboid();
                        break;
                    case HitDefnitionFramType.CylinderType:
                        mAttackType = new FrameCylinder();
                        break;
                    case HitDefnitionFramType.RingType:
                        mAttackType = new FrameRing();
                        break;
                    case HitDefnitionFramType.SomatoType:
                        mAttackType = new FrameSomato();
                        break;
                    case HitDefnitionFramType.FanType:
                        mAttackType = new FrameFan();
                        break;
                }
            }
        }

        AttackFrame mAttackType = null;
        [Category(CATEGORY_HITEDJUDGE)]
        [Description("技能击中框")]
        [DisplayName("技能击中框")]
        public AttackFrame AttackType { get { return mAttackType; } set { mAttackType = value; } }

        Vector3 mOffset = Vector3.Zero;
        [DisplayName("偏移"), Category(CATEGORY_HITEDJUDGE), Browsable(false)]
        public  Vector3 Offset { get { return mOffset; } set { mOffset = value; } }

        HeightStatusFlag mHeightStatusHitMask = (HeightStatusFlag)0x0F;
        [Category(CATEGORY_HITEDJUDGE)]
        [Description("有效击中高度组合")]
        [DisplayName("有效击中高度")]
        [DefaultValue((HeightStatusFlag)0x0F)]
        public HeightStatusFlag HeightStatusHitMask { get { return mHeightStatusHitMask; } set { mHeightStatusHitMask = value; } }

        [Browsable(false)]
        [DefaultValue(0x0F)]
        public int HeightStatusHitMaskInt { get { return (int)mHeightStatusHitMask; } set { } }
       
        #endregion


        #region CATEGORY_HITRESULT

        int mIsRemoteAttacks = 0;
        [Category(CATEGORY_HITRESULT)]
        [Description("0=普通攻击，1=远程攻击")]
        [DisplayName("是否远程攻击")]
        [DefaultValue(0)]
        public int IsRemoteAttacks { get { return mIsRemoteAttacks; } set { mIsRemoteAttacks = (value != 0) ? 1 : 0; } }

        private int mAttackLevel = 10;
        [Category(CATEGORY_HITRESULT)]
        [Description("攻击者的攻击等级，攻击等级小于受击者的动作等级的时候，受击者为霸体状态。默认值为10")]
        [DisplayName("攻击等级")]
        [DefaultValue(10)]
        public int AttackLevel { get { return mAttackLevel; } set { mAttackLevel = value; } }

        private int mFllowReleaser = 0;
        [Category(CATEGORY_HITRESULT)]
        [Description("跟随释放者移动")]
        [DisplayName("跟随释放者移动")]
        [DefaultValue(0)]
        public int FllowReleaser { get { return mFllowReleaser; } set { mFllowReleaser = value; } }

        private int mSkillAffectAttackLevel = 0;
        [Category(CATEGORY_HITRESULT)]
        [Description("技能影响攻击定义等级")]
        [DisplayName("技能影响攻击定义等级")]
        [DefaultValue(0)]
        public int SkillAffectAttackLevel { get { return mSkillAffectAttackLevel; } set { mSkillAffectAttackLevel = value; } }

        int mAttackerStraightTime = 0 ;
        [Category(CATEGORY_HITRESULT)]
        [Description("攻击者的硬直时间")]
        [DisplayName("攻击者的硬直时间")]
        [DefaultValue(0)]
        public int AttackerStraightTime { get { return mAttackerStraightTime; } set { mAttackerStraightTime = value; } }

        int mTargetStayAir = 0;
        [Category(CATEGORY_HITRESULT)]
        [Description("目标滞空")]
        [DisplayName("目标滞空")]
        [DefaultValue(0)]
        public int TargetStayAir { get { return mTargetStayAir; } set { mTargetStayAir = value; } }

        Vector3 mAttackerLash = Vector3.Zero;
        [Category(CATEGORY_HITRESULT)]
        [Description("攻击者的冲击速度")]
        [DisplayName("攻击者的冲击速度")]
        public Vector3 AttackerLash { get { return mAttackerLash; } set { mAttackerLash = value; } }


        int mAttackerTime = 100;
        [Category(CATEGORY_HITRESULT)]
        [Description("攻击者冲击时间")]
        [DisplayName("攻击者冲击时间")]
        public int AttackerTime { get { return mAttackerTime; } set { mAttackerTime = value; } }
       
        int mAttackeeStraightTime = 0;
        [Category(CATEGORY_HITRESULT)]
        [Description("受击者的硬直时间")]
        [DisplayName("受击者的硬直时间")]
        [DefaultValue(0)]
        public int AttackeeStraightTime { get { return mAttackeeStraightTime; } set { mAttackeeStraightTime = value; } }

        HitResultType mHitResult = HitResultType.StandHit;
        [Category(CATEGORY_HITRESULT)]
        [Description("击中结果")]
        [DisplayName("击中结果")]
        [DefaultValue(HitResultType.StandHit)]
        public HitResultType HitResult { get { return mHitResult; } set { mHitResult = value; } }

        HitResultData mGroundHit = new HitResultData();
        [Category(CATEGORY_HITRESULT)]
        [DisplayName("倒地受击数据")]
        public HitResultData GroundHit { get { return mGroundHit; } set { mGroundHit = value; } }

        HitResultData mLowAirHit = new HitResultData();
        [Category(CATEGORY_HITRESULT)]
        [DisplayName("低空受击数据")]
        public HitResultData LowAirHit { get { return mLowAirHit; } set { mLowAirHit = value; } }

        HitResultData mHighAirHit = new HitResultData();
        [Category(CATEGORY_HITRESULT)]
        [DisplayName("高空受击数据")]
        public HitResultData HighAirHit { get { return mHighAirHit; } set { mHighAirHit = value; } } 

        Vector3 mAttackeeLash = Vector3.Zero;
        [Category(CATEGORY_HITRESULT)]
        [Description("受击者冲击速度")]
        [DisplayName("受击者冲击速度")]
        public Vector3 AttackeeLash { get { return mAttackeeLash; } set { mAttackeeLash = value; } }

        int mAttackeeTime = 100;
        [Category(CATEGORY_HITRESULT)]
        [Description("受击者冲击时间")]
        [DisplayName("受击者冲击时间")]
        public int AttackeeTime { get { return mAttackeeTime; } set { mAttackeeTime = value; } }

        String mBuffID = String.Empty;
        [Category(CATEGORY_HITRESULT)]
        [Description("追加Buff的ID号")]
        [DisplayName("BuffID")]
        [DefaultValue("")]
        public String BuffID { get { return mBuffID; } set { mBuffID = value; } }

        DamageType mDamageType = DamageType.Normal;
        [Category(CATEGORY_HITRESULT)]
        [Description("击中伤害类型")]
        [DisplayName("击中伤害类型")]
        [DefaultValue(DamageType.Normal)]
        public DamageType DamageType { get { return mDamageType; } set { mDamageType = value; } }

        int mDamage = 50;
        [DefaultValue(50)]
        public int Damage { get { return mDamage; } set { mDamage = value; } }

        int mInitAbility = 0;
        [DefaultValue(0)]
        public int InitAbility { get { return mInitAbility; } set { mInitAbility = value; } }

        int mHitAbility = 0;
        [DefaultValue(0)]
        public int HitAbility { get { return mHitAbility; } set { mHitAbility = value; } }

        int mHoldOffsetOpen = 0;
        [DefaultValue(0)]
        [DisplayName("抓取偏移开关"), Description("目标位移控制固定偏移的开关"), Category(CATEGORY_HITRESULT)]
        public int HoldOffsetOpen { get { return mHoldOffsetOpen; } set { mHoldOffsetOpen = value; } }

        Vector3 mHoldOffset = Vector3.Zero;
        [TypeConverterAttribute(typeof(Vector3))]
        [DisplayName("抓取偏移"), Description("目标位移控制固定偏移"), Category(CATEGORY_HITRESULT)]
        public Vector3 HoldOffset { get { return mHoldOffset; } set { mHoldOffset = value; } }
        #endregion 


        #region CATEGORY_APPEARENCE
        String mHitedEffect = string.Empty;
        [Category(CATEGORY_APPEARENCE)]
        [Description("击中特效")]
        [DisplayName("击中特效")]
        [DefaultValue("")]
        public String HitedEffect { get { return mHitedEffect; } set { mHitedEffect = value; } }


        UInt16 mHitedEffectScale = 100;
        [Category(CATEGORY_APPEARENCE)]
        [Description("击中特效缩放,基数为100")]
        [DisplayName("击中特效缩放")]
        [DefaultValue(100)]
        public UInt16 HitedEffectScale { get { return mHitedEffectScale; } set { mHitedEffectScale = value; } }

        String mScript = string.Empty;
        [Category(CATEGORY_APPEARENCE)]
        [Description("击中脚本")]
        [DisplayName("击中脚本")]
        [DefaultValue("")]
        public String Script { get { return mScript; } set { mScript = value; } }

        Vector3 mHitedEffectOffset = new Vector3(0, 100, 0);
        [Category(CATEGORY_APPEARENCE)]
        [Description("站立击中特效偏移")]
        [DisplayName("站立击中特效偏移")]
        public Vector3 HitedEffectOffset { get { return mHitedEffectOffset; } set { mHitedEffectOffset = value; } }


        Vector3 mGroundHitedEffectOffset = new Vector3(0, 30, 0);
        [Category(CATEGORY_APPEARENCE)]
        [Description("倒地击中特效偏移")]
        [DisplayName("倒地击中特效偏移")]
        public Vector3 GroundHitedEffectOffset { get { return mGroundHitedEffectOffset; } set { mGroundHitedEffectOffset = value; } }

        Vector3 mLowAirHitedEffectOffset = new Vector3(0, 100, 0);
        [Category(CATEGORY_APPEARENCE)]
        [Description("低空击中特效偏移")]
        [DisplayName("低空击中特效偏移")]
        public Vector3 LowAirHitedEffectOffset { get { return mLowAirHitedEffectOffset; } set { mLowAirHitedEffectOffset = value; } }

        Vector3 mHighAirHitedEffectOffset = Vector3.Zero;
        [Category(CATEGORY_APPEARENCE)]
        [Description("高空击中特效偏移")]
        [DisplayName("高空击中特效偏移")]
        public Vector3 HighAirHitedEffectOffset { get { return mHighAirHitedEffectOffset; } set { mHighAirHitedEffectOffset = value; } }

        String mHitedSound = string.Empty;
        [Category(CATEGORY_APPEARENCE)]
        [Description("击中音效")]
        [DisplayName("击中音效")]
        [DefaultValue("")]
        public String HitedSound { get { return mHitedSound; } set { mHitedSound = value; } }

        int mHitedSoundIndex = -2;
        [Browsable(false)]
        public int HitedSoundIndex { get { return mHitedSoundIndex; } set { mHitedSoundIndex = value; } }


        String mSelfEffect=string.Empty;
        [Category(CATEGORY_APPEARENCE)]
        [Description("本体特效")]
        [DisplayName("本体特效")]
        [DefaultValue("")]
        public String SelfEffect { get { return mSelfEffect; } set { mSelfEffect = value; } }

        UInt16 mSelfEffectScale = 100;
        [Category(CATEGORY_APPEARENCE)]
        [Description("本体特效缩放,基数为100")]
        [DisplayName("本体特效缩放")]
        [DefaultValue(100)]
        public UInt16 SelfEffectScale { get { return mSelfEffectScale; } set { mSelfEffectScale = value; } }

        int mSelfEffectNT = 0;
        [Category(CATEGORY_APPEARENCE)]
        [Description("1=特效随动作中断不中断，播完为止,0=随动作中断而中断特效")]
        [DisplayName("本体特效NT")]
        [DefaultValue(0)]
        public int SeflEffectNT { get { return mSelfEffectNT; } set { mSelfEffectNT = value; } }

        int mEffectTriggerTime = 0;
        [Category(CATEGORY_APPEARENCE)]
        [Description("本体特效起效时间(只对本体特效有效)")]
        [DisplayName("本体特效起效时间")]
        [DefaultValue(0)]
        public int EffectTriggerTime { get { return mEffectTriggerTime; } set { mEffectTriggerTime = value; } }

        Vector3 mSelfEffectOffset = Vector3.Zero;
        [Category(CATEGORY_APPEARENCE)]
        [Description("本体特效偏移")]
        [DisplayName("本体特效偏移")]
        public Vector3 SelfEffectOffset { get { return mSelfEffectOffset; } set { mSelfEffectOffset = value; } }

        int mBaseGround = 0;
        [Category(CATEGORY_APPEARENCE)]
        [Description("0=相对击中框中心偏移播放，1=特效在地面播放")]
        [DisplayName("是否相对地面")]
        [DefaultValue(0)]
        public int BaseGround { get { return mBaseGround; } set { mBaseGround = (value != 0) ? 1 : 0; } }

        String mSelfSound =string.Empty;
        [Category(CATEGORY_APPEARENCE)]
        [Description("本体音效")]
        [DisplayName("本体音效")]
        [DefaultValue("")]
        public String SelfSound { get { return mSelfSound; } set { mSelfSound = value; } }

        int mSoundTriggerTime = 0;
        [Category(CATEGORY_APPEARENCE)]
        [Description("本体音效起效时间")]
        [DisplayName("本体音效起效时间")]
        [DefaultValue(0)]
        public int SoundTriggerTime { get { return mSoundTriggerTime; } set { mSoundTriggerTime = value; } }

        private WeaponType mWeaponType = WeaponType.None;
        [DisplayName("攻击定义的材质")]
        [Category(CATEGORY_APPEARENCE)]
        [DefaultValue(WeaponType.None)]
        public WeaponType WeaponType { get { return mWeaponType; } set { mWeaponType = value; } }

        [Browsable(false), DefaultValue(0)]
        public int WeaponType_Int { get { return (int)WeaponType; } set { } }

        #endregion

        public void LoadDefaultHitEffectValue()
        {
            HitedEffectOffset.X = 0;
            HitedEffectOffset.Y = 100;
            HitedEffectOffset.Z = 0;

            GroundHitedEffectOffset.X = 0;
            GroundHitedEffectOffset.Y = 30;
            GroundHitedEffectOffset.Z = 0;

            LowAirHitedEffectOffset.X = 0;
            LowAirHitedEffectOffset.Y = 100;
            LowAirHitedEffectOffset.Z = 0;
        }
    }
}
