using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Xml.Serialization;
using Data.Server;
using System.IO;

namespace Data
{
    [Serializable]
    public class ActionGroup : ICloneable
    {
        private List<Action> mActionList = new List<Action>();
        public List<Action> ActionList { get { return mActionList; } }

        private int mRace = 0;
        [Description("角色的ID号")]
        [DisplayName("角色ID")]
        public int Race { get { return mRace; } set { mRace = value; } }


        private int mGroupNum = 0;
        [DefaultValue(0)]
        [DisplayName("动作组编号")]
        public int GroupNum { get { return mGroupNum; } set { mGroupNum = value; } }

        private MaterialType mMaterialType = MaterialType.None;
        [DisplayName("Unit的材质")]
        [DefaultValue(MaterialType.None)]
        public MaterialType MaterialType { get { return mMaterialType; } set { mMaterialType = value; } }

        [Browsable(false), DefaultValue(0)]
        public int MaterialType_Int { get { return (int)mMaterialType; } set { } }

        public bool mRotateOnHit = true;
        [DefaultValue(true), DisplayName("受击后转向"), XmlAttribute("RotateOnHit")]
        public bool RotateOnHit { get { return mRotateOnHit; } set { mRotateOnHit = value; } }

        private bool mCheckDeath = false;
        [DefaultValue(false), DisplayName("侦测死亡"), XmlAttribute("CheckDeath")]
        public bool CheckDeath { get { return mCheckDeath; } set { mCheckDeath = value; } }

        private int mReflectUnit = 0;
        [DefaultValue(0), DisplayName("映射单位ID"), XmlAttribute("ReflectUnit")]
        public int ReflectUnit { get { return mReflectUnit; } set { mReflectUnit = value; } }

        private int mReflectGroupID = 0;
        [DefaultValue(0), DisplayName("映射动作组ID"), XmlAttribute("ReflectGroupID")]
        public int ReflectGroupID { get { return mReflectGroupID; } set { mReflectGroupID = value; } }

        private bool mHasCollision = false;
        [DefaultValue(false), DisplayName("是否生成碰撞"), XmlAttribute("HasCollision")]
        public bool HasCollision { get { return mHasCollision; } set { mHasCollision = value; } }

        private int mDefaultActionLevel = 10;
        [Description("单位的默认动作等级，用来进行霸体判断，每个动作可以重载动作等级。默认值为10")]
        [DisplayName("默认动作等级")]
        [DefaultValue(10)]
        public int DefaultActionLevel { get { return mDefaultActionLevel; } set { mDefaultActionLevel = value; } }

        AISetting mAISetting = new AISetting();
        [DisplayName("AI设置")]
        public AISetting AISetting { get { return mAISetting; } set { mAISetting = value; } }


        private String mStartupAction = "N0000";
        [Description("该单位(在城镇里面)创建出来时候的默认动作，默认值为N0000")]
        [DisplayName("起始动作(城镇)")]
        [DefaultValue("N0000")]
        public String StartupAction { get { return mStartupAction; } set { mStartupAction = value; } }

        private String mStartupActionInstance = "N0000";
        [Description("该单位(在副本里面)创建出来时候的默认动作，默认值为N0000")]
        [DisplayName("起始动作(副本)")]
        [DefaultValue("N0000")]
        public String StartupActionInstance { get { return mStartupActionInstance; } set { mStartupActionInstance = value; } } 

        private bool mCanCapture = true;
        [DefaultValue(true), DisplayName("能否被抓取"), XmlAttribute("CanCapture")]
        [Category("受击结果")]
        public bool CanCapture { get { return mCanCapture; } set { mCanCapture = value; } }

        Int32 mAttackModify = 100;
        [DefaultValue(100)]
        [DisplayName("受击修正")]
        [Description("基数为100")]
        [XmlAttribute("AttackModify")]
        [Category("重力加速度")]
        public Int32 AttackModify { get { return mAttackModify; } set { mAttackModify = value; } }

        private Int32 mModifyStartTime = 0;
        [Description("修正开始百分比，数值为[0-100-200]，[0-100]期间是处于动画时间段里面的百分比，[100-200]期间是出于POSE时间段里面的百分比")]
        [DisplayName("修正开始百分比")]
        [XmlAttribute("ModifyStartTime")]
        [Category("重力加速度")]
        [DefaultValue(0)]
        public Int32 ModifyStartTime { get { return mModifyStartTime; } set { mModifyStartTime = value; } }

        private Int32 mModifyEndTime = 200;
        [Description("修正结束百分比，数值为[0-100-200]，[0-100]期间是处于动画时间段里面的百分比，[100-200]期间是出于POSE时间段里面的百分比")]
        [DisplayName("修正结束百分比")]
        [XmlAttribute("ModifyEndTime")]
        [Category("重力加速度")]
        [DefaultValue(200)]
        public Int32 ModifyEndTime { get { return mModifyEndTime; } set { mModifyEndTime = value; } }
        

        public Int32 mGravtity = 1000;
        [Description("重力加速度")]
        [DisplayName("重力加速度")]
        [Category("重力加速度")]
        [DefaultValue(1000)]
        public Int32 Gravtity { get { return mGravtity; } set { mGravtity = value; } }

        private Int32 mAttackedGravtity = 1000;
        [Description("被攻击时的重力加速度")]
        [DisplayName("被攻击时的重力加速度")]
        [Category("重力加速度")]
        [DefaultValue(1000)]
        public Int32 AttackedGravtity { get { return mAttackedGravtity; } set { mAttackedGravtity = value; } }

        private Vector3 mLashModifier = new Vector3(1, 1, 1);
        [Description("当怪物受到攻击时，冲击速度减免量，默认为不减免(1 1 1)")]
        [DisplayName("冲击减免")]
        public Vector3 LashModifier { get { return mLashModifier; } set { mLashModifier = value; } }

        Vector3 mHoldOffset = Vector3.Zero;
        [TypeConverterAttribute(typeof(Vector3))]
        [DisplayName("被抓偏移")]
        public Vector3 HoldOffset { get { return mHoldOffset; } set { mHoldOffset = value; } }

        public int mBoundingLength = 80;
        [Category("包围盒")]
        [Description("单位的包围盒长度，包围盒用来检测受击范围等。")]
        [DisplayName("包围盒长度")]
        [DefaultValue(80)]
        public int BoundingLength { get { return mBoundingLength; } set { mBoundingLength = value; } }

        public int mBoundingWidth = 80;
        [Category("包围盒")]
        [Description("单位的包围盒宽度，包围盒用来检测受击范围等。")]
        [DisplayName("包围盒宽度")]
        [DefaultValue(80)]
        public int BoundingWidth { get { return mBoundingWidth; } set { mBoundingWidth = value; } }

        public int mBoundingHeight = 180;
        [Category("包围盒")]
        [Description("单位的包围盒高度，包围盒用来检测受击范围等。")]
        [DisplayName("包围盒高度")]
        [DefaultValue(180)]
        public int BoundingHeight { get { return mBoundingHeight; } set { mBoundingHeight = value; } }

        private String mStandStandHit = "H0000";
        [Description("站立受击时的动作ID号，此为默认动作号码，每个Action自己可以覆盖此方法")]
        [DisplayName("站立受击")]
        [Category("做:受击结果:站立")]
        [DefaultValue("H0000")]
        public String StandStandHit { get { return mStandStandHit; } set { mStandStandHit = value; } }

        private String mStandKnockOut = "H0010";
        [Description("受到击飞攻击时的动作ID号，此为默认动作号码，每个Action自己可以覆盖此方法")]
        [DisplayName("击飞")]
        [Category("做:受击结果:站立")]
        [DefaultValue("H0010")]
        public String StandKnockOut { get { return mStandKnockOut; } set { mStandKnockOut = value; } }

        private String mStandKnockBack = "H0020";
        [Description("受到击退攻击时的动作ID号，此为默认动作号码，每个Action自己可以覆盖此方法")]
        [DisplayName("击退")]
        [Category("做:受击结果:站立")]
        [DefaultValue("H0020")]
        public String StandKnockBack { get { return mStandKnockBack; } set { mStandKnockBack = value; } }

        private String mStandDiagUp = "H0030";
        [Description("受到浮空攻击时的动作ID号，此为默认动作号码，每个Action自己可以覆盖此方法")]
        [DisplayName("浮空")]
        [Category("做:受击结果:站立")]
        [DefaultValue("H0030")]
        public String StandDiagUp { get { return mStandDiagUp; } set { mStandDiagUp = value; } }

        private String mStandKnockDown = "H0040";
        [Description("受到击倒攻击时的动作ID号，此为默认动作号码，每个Action自己可以覆盖此方法")]
        [DisplayName("击倒")]
        [Category("做:受击结果:站立")]
        [DefaultValue("H0040")]
        public String StandKnockDown { get { return mStandKnockDown; } set { mStandKnockDown = value; } }

        private String mStandHold = "H0050";
        [Description("受到抓住攻击时的动作ID号，此为默认动作号码，每个Action自己可以覆盖此方法")]
        [DisplayName("抓住")]
        [Category("做:受击结果:站立")]
        [DefaultValue("H0050")]
        public String StandHold { get { return mStandHold; } set { mStandHold = value; } }

        private String mStandAirHit = "H0060";
        [Description("受到浮空追击攻击时的动作ID号，此为默认动作号码，每个Action自己可以覆盖此方法")]
        [DisplayName("浮空追击")]
        [Category("做:受击结果:站立")]
        [DefaultValue("H0060")]
        public String StandAirHit { get { return mStandAirHit; } set { mStandAirHit = value; } }

        private String mStandDownHit = "H0070";
        [Description("受到倒地追击攻击时的动作ID号，此为默认动作号码，每个Action自己可以覆盖此方法")]
        [DisplayName("倒地追击")]
        [Category("做:受击结果:站立")]
        [DefaultValue("H0070")]
        public String StandDownHit { get { return mStandDownHit; } set { mStandDownHit = value; } }

        private String mStandFallDown = "H0031";
        [Description("受到倒地攻击时的动作ID号，此为默认动作号码H0031，每个Action自己可以覆盖此方法")]
        [DisplayName("跌倒")]
        [Category("做:受击结果:站立")]
        [DefaultValue("H0031")]
        public String StandFallDown { get { return mStandFallDown; } set { mStandFallDown = value; } }


        private String mAirStandHit = "H0060";
        [Description("站立受击时的动作ID号，此为默认动作号码，每个Action自己可以覆盖此方法")]
        [DisplayName("站立受击")]
        [Category("做:受击结果:空中")]
        [DefaultValue("H0060")]
        public String AirStandHit { get { return mAirStandHit; } set { mAirStandHit = value; } }

        private String mAirKnockOut = "H0010";
        [Description("受到击飞攻击时的动作ID号，此为默认动作号码，每个Action自己可以覆盖此方法")]
        [DisplayName("击飞")]
        [Category("做:受击结果:空中")]
        [DefaultValue("H0010")]
        public String AirKnockOut { get { return mAirKnockOut; } set { mAirKnockOut = value; } }

        private String mAirKnockBack = "H0010";
        [Description("受到击退攻击时的动作ID号，此为默认动作号码，每个Action自己可以覆盖此方法")]
        [DisplayName("击退")]
        [Category("做:受击结果:空中")]
        [DefaultValue("H0010")]
        public String AirKnockBack { get { return mAirKnockBack; } set { mAirKnockBack = value; } }

        private String mAirDiagUp = "H0030";
        [Description("受到浮空攻击时的动作ID号，此为默认动作号码，每个Action自己可以覆盖此方法")]
        [DisplayName("浮空")]
        [Category("做:受击结果:空中")]
        [DefaultValue("H0030")]
        public String AirDiagUp { get { return mAirDiagUp; } set { mAirDiagUp = value; } }

        private String mAirKnockDown = "H0040";
        [Description("受到击倒攻击时的动作ID号，此为默认动作号码，每个Action自己可以覆盖此方法")]
        [DisplayName("击倒")]
        [Category("做:受击结果:空中")]
        [DefaultValue("H0040")]
        public String AirKnockDown { get { return mAirKnockDown; } set { mAirKnockDown = value; } }

        private String mAirHold = "H0050";
        [Description("受到抓住攻击时的动作ID号，此为默认动作号码，每个Action自己可以覆盖此方法")]
        [DisplayName("抓住")]
        [Category("做:受击结果:空中")]
        [DefaultValue("H0050")]
        public String AirHold { get { return mAirHold; } set { mAirHold = value; } }

        private String mAirAirHit = "H0060";
        [Description("受到浮空追击攻击时的动作ID号，此为默认动作号码，每个Action自己可以覆盖此方法")]
        [DisplayName("浮空追击")]
        [Category("做:受击结果:空中")]
        [DefaultValue("H0060")]
        public String AirAirHit { get { return mAirAirHit; } set { mAirAirHit = value; } }

        private String mAirDownHit = "H0060";
        [Description("受到倒地追击攻击时的动作ID号，此为默认动作号码，每个Action自己可以覆盖此方法")]
        [DisplayName("倒地追击")]
        [Category("做:受击结果:空中")]
        [DefaultValue("H0060")]
        public String AirDownHit { get { return mAirDownHit; } set { mAirDownHit = value; } }

        private String mAirFallDown = "H0031";
        [Description("受到倒地攻击时的动作ID号，此为默认动作号码H0031，每个Action自己可以覆盖此方法")]
        [DisplayName("跌倒")]
        [Category("做:受击结果:空中")]
        [DefaultValue("H0031")]
        public String AirFallDown { get { return mAirFallDown; } set { mAirFallDown = value; } }

        private String mFloorStandHit = "H0070";
        [Description("站立受击时的动作ID号，此为默认动作号码，每个Action自己可以覆盖此方法")]
        [DisplayName("站立受击")]
        [Category("做:受击结果:地面")]
        [DefaultValue("H0070")]
        public String FloorStandHit { get { return mFloorStandHit; } set { mFloorStandHit = value; } }

        private String mFloorKnockOut = "H0010";
        [Description("受到击飞攻击时的动作ID号，此为默认动作号码，每个Action自己可以覆盖此方法")]
        [DisplayName("击飞")]
        [Category("做:受击结果:地面")]
        [DefaultValue("H0010")]
        public String FloorKnockOut { get { return mFloorKnockOut; } set { mFloorKnockOut = value; } }

        private String mFloorKnockBack = "H0070";
        [Description("受到击退攻击时的动作ID号，此为默认动作号码，每个Action自己可以覆盖此方法")]
        [DisplayName("击退")]
        [Category("做:受击结果:地面")]
        [DefaultValue("H0070")]
        public String FloorKnockBack { get { return mFloorKnockBack; } set { mFloorKnockBack = value; } }

        private String mFloorDiagUp = "H0030";
        [Description("受到浮空攻击时的动作ID号，此为默认动作号码，每个Action自己可以覆盖此方法")]
        [DisplayName("浮空")]
        [Category("做:受击结果:地面")]
        [DefaultValue("H0030")]
        public String FloorDiagUp { get { return mFloorDiagUp; } set { mFloorDiagUp = value; } }

        private String mFloorKnockDown = "H0070";
        [Description("受到击倒攻击时的动作ID号，此为默认动作号码，每个Action自己可以覆盖此方法")]
        [DisplayName("击倒")]
        [Category("做:受击结果:地面")]
        [DefaultValue("H0070")]
        public String FloorKnockDown { get { return mFloorKnockDown; } set { mFloorKnockDown = value; } }

        private String mFloorHold = "H0050";
        [Description("受到抓住攻击时的动作ID号，此为默认动作号码，每个Action自己可以覆盖此方法")]
        [DisplayName("抓住")]
        [Category("做:受击结果:地面")]
        [DefaultValue("H0050")]
        public String FloorHold { get { return mFloorHold; } set { mFloorHold = value; } }

        private String mFloorAirHit = "H0070";
        [Description("受到浮空追击攻击时的动作ID号，此为默认动作号码，每个Action自己可以覆盖此方法")]
        [DisplayName("浮空追击")]
        [Category("做:受击结果:地面")]
        [DefaultValue("H0070")]
        public String FloorAirHit { get { return mFloorAirHit; } set { mFloorAirHit = value; } }

        private String mFloorDownHit = "H0070";
        [Description("受到倒地追击攻击时的动作ID号，此为默认动作号码，每个Action自己可以覆盖此方法")]
        [DisplayName("倒地追击")]
        [Category("做:受击结果:地面")]
        [DefaultValue("H0070")]
        public String FloorDownHit { get { return mFloorDownHit; } set { mFloorDownHit = value; } }

        private String mFloorFallDown = "H0031";
        [Description("受到倒地攻击时的动作ID号，此为默认动作号码H0031，每个Action自己可以覆盖此方法")]
        [DisplayName("跌倒")]
        [Category("做:受击结果:地面")]
        [DefaultValue("H0031")]
        public String FloorFallDown { get { return mFloorFallDown; } set { mFloorFallDown = value; } }

        private String mIdlePabodyEffect = "";
        [Description("待机霸体特效")]
        [DisplayName("待机霸体特效")]
        [Category("做:霸体特效:待机")]
        [DefaultValue("")]
        public String IdlePabodyEffect { get { return mIdlePabodyEffect; } set { mIdlePabodyEffect = value; } }

        private Vector3 mIdlePabodyEffectOffset = new Vector3();
        [Description("待机霸体特效偏移量")]
        [DisplayName("待机霸体特效偏移量")]
        [Category("做:霸体特效:待机")]
        public Vector3 IdlePabodyEffectOffset { get { return mIdlePabodyEffectOffset; } set { mIdlePabodyEffectOffset = value; } }

        private String mMovePabodyEffect = "";
        [Description("怪物是霸体的时候，播放的特效")]
        [DisplayName("霸体受击特效")]
        [Category("做:霸体特效:移动")]
        [DefaultValue("")]
        public String MovePabodyEffect { get { return mMovePabodyEffect; } set { mMovePabodyEffect = value; } }

        private Vector3 mMovePabodyEffectOffset = new Vector3();
        [Description("怪物是霸体的时候，播放特效的偏移量")]
        [DisplayName("霸体受击特效偏移量")]
        [Category("做:霸体特效:移动")]
        public Vector3 MovePabodyEffectOffset { get { return mMovePabodyEffectOffset; } set { mMovePabodyEffectOffset = value; } }

        private String mAttackPabodyEffect = "";
        [Description("怪物是霸体的时候，播放的特效")]
        [DisplayName("攻击受击特效")]
        [Category("做:霸体特效:攻击")]
        [DefaultValue("")]
        public String AttackPabodyEffect { get { return mAttackPabodyEffect; } set { mAttackPabodyEffect = value; } }

        private Vector3 mAttackPabodyEffectOffset = new Vector3();
        [Description("怪物是霸体的时候，播放特效的偏移量")]
        [DisplayName("霸体受击特效偏移量")]
        [Category("做:霸体特效:攻击")]
        public Vector3 AttackPabodyEffectOffset { get { return mAttackPabodyEffectOffset; } set { mAttackPabodyEffectOffset = value; } }

        private String mHitPabodyEffect = "";
        [Description("受伤霸体特效")]
        [DisplayName("受伤霸体特效")]
        [Category("做:霸体特效:受伤")]
        [DefaultValue("")]
        public String HitPabodyEffect { get { return mHitPabodyEffect; } set { mHitPabodyEffect = value; } }

        private Vector3 mHitPabodyEffectOffset = new Vector3();
        [Description("受伤霸体特效偏移量")]
        [DisplayName("受伤霸体特效偏移量")]
        [Category("做:霸体特效:受伤")]
        public Vector3 HitPabodyEffectOffset { get { return mHitPabodyEffectOffset; } set { mHitPabodyEffectOffset = value; } }

        private String mDefensePabodyEffect = "";
        [Description("防御霸体特效")]
        [DisplayName("防御霸体特效")]
        [Category("做:霸体特效:防御")]
        [DefaultValue("")]
        public String DefensePabodyEffect { get { return mDefensePabodyEffect; } set { mDefensePabodyEffect = value; } }

        private Vector3 mDefensePabodyEffectOffset = new Vector3();
        [Description("防御霸体特效偏移量")]
        [DisplayName("防御霸体特效偏移量")]
        [Category("做:霸体特效:防御")]
        public Vector3 DefensePabodyEffectOffset { get { return mDefensePabodyEffectOffset; } set { mDefensePabodyEffectOffset = value; } }

        private String mStandDeath = "H1040";
        [Description("站立死亡")]
        [DisplayName("站立死亡")]
        [Category("死亡")]
        [DefaultValue("H1040")]
        public String StandDeath { get { return mStandDeath; } set { mStandDeath = value; } }

        private String mDownDeath = "H1041";
        [Description("倒地死亡")]
        [DisplayName("倒地死亡")]
        [Category("死亡")]
        [DefaultValue("H1041")]
        public String DownDeath { get { return mDownDeath; } set { mDownDeath = value; } }

        private int mRelativeEndureEffectAlpha = 255;
        [Description("Alpha")]
        [DefaultValue(255)]
        [Category("相对霸体特效")]
        [DisplayName("Alpha")]
        public int RelativeEndureEffectAlpha { get { return mRelativeEndureEffectAlpha; } set { mRelativeEndureEffectAlpha = value; } }

        private int mRelativeEndureEffectRed = 255;
        [Description("红色")]
        [DefaultValue(255)]
        [Category("相对霸体特效")]
        [DisplayName("红色")]
        public int RelativeEndureEffectRed { get { return mRelativeEndureEffectRed; } set { mRelativeEndureEffectRed = value; } }

        private int mRelativeEndureEffectGreen = 30;
        [Description("蓝色")]
        [DefaultValue(30)]
        [Category("相对霸体特效")]
        [DisplayName("蓝色")]
        public int RelativeEndureEffectGreen { get { return mRelativeEndureEffectGreen; } set { mRelativeEndureEffectGreen = value; } }

        private int mRelativeEndureEffectBule = 30;
        [Description("绿色")]
        [DefaultValue(30)]
        [Category("相对霸体特效")]
        [DisplayName("绿色")]
        public int RelativeEndureEffectBule { get { return mRelativeEndureEffectBule; } set { mRelativeEndureEffectBule = value; } }

        private int mRelativeEndureEffectOverTime = 0;
        [Description("过度时间")]
        [DefaultValue(0)]
        [Category("相对霸体特效")]
        [DisplayName("过度时间")]
        public int RelativeEndureEffectOverTime { get { return mRelativeEndureEffectOverTime; } set { mRelativeEndureEffectOverTime = value; } }

        private String mDesc = "";
        [DisplayName("说明")]
        [DefaultValue("")]
        public String Desc { get { return mDesc; } set { mDesc = value; } }

        public override string ToString()
        {
            String name = "动作组" + GroupNum;

            if (Desc.Length != 0)
                name += "[" + Desc + "]";

            return name;
        }

        public Object Clone()
        {
            // save current object to xml.
            MemoryStream memoryStream = new MemoryStream();
            XmlSerializer serializer = new XmlSerializer(typeof(ActionGroup));
            serializer.Serialize(memoryStream, this);

            // deserializer out.
            memoryStream.Seek(0, SeekOrigin.Begin);
            ActionGroup newActionGroup = (ActionGroup)serializer.Deserialize(memoryStream);
            memoryStream.Close();

            // reset copy datas.
            return newActionGroup;
        }

        public String GetUniqueID(String srcID)
        {
            String KeyChar = srcID.Substring(0, 1);
            int i = int.Parse(srcID.Substring(1));
            while (true)
            {
                String newID = KeyChar + i.ToString("D4");
                bool isUnique = true;
                foreach (Action tmpAction in ActionList)
                {
                    if (newID == tmpAction.ID)
                    {
                        isUnique = false;
                        break;
                    }
                }

                if (isUnique)
                    break;
                i++;
            }
            return KeyChar + i.ToString("D4");
        }

        public Action GetAction(String actionID)
        {
            foreach (Action action in ActionList)
            {
                if (action.ID == actionID)
                    return action;
            }
            return null;
        }

        public string NormalizeActionID(String actionID)
        {
            for (int i = 1; i < actionID.Length; i++)
            {
                if (!char.IsDigit(actionID[i]))
                    return actionID.Substring(0, i);
            }
            return actionID;
        }

        public int GetActionIdx(String actionID)
        {
            actionID = NormalizeActionID(actionID);

            int idx = -1;
            foreach (Action action in ActionList)
            {
                idx++;
                if (action.ID == actionID)
                    return idx;
            }
            return idx;
        }

        public Action GetAction(int idx)
        {
            return idx < mActionList.Count ? mActionList[idx] : null;
        }

        public void SortActionList()
        {
            ActionList.Sort(new Action.ActionCompare());
        }

        public void BuildActionCache()
        {
            AISetting.BuildActionCache(this);

            foreach (Action action in ActionList)
                action.BuildActionCache(this);
        }
    }
}
