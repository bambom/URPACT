using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Xml.Serialization;

namespace Data.Server
{
    [Serializable]
    [TypeConverterAttribute(typeof(ExpandableObjectConverter))]
    public class AIStatusSwitch
    {
        int mSwitchStatusID = 0;
        [XmlAttribute("SwitchStatusID"), DefaultValue(0), DisplayName("模式编号"), Category("状态切换")]
        public int SwitchStatusID { get { return mSwitchStatusID; } set { mSwitchStatusID = value; } }

        int mActionSwitchNow = 1;
        [XmlAttribute("ActionSwitchNow"), DefaultValue(1), DisplayName("立即替换"), Category("状态切换")]
        public int ActionSwitchNow { get { return mActionSwitchNow; } set { mActionSwitchNow = value; } }

        int mTargetType = 0;
        [XmlAttribute("TargetType"), DefaultValue(0), DisplayName("目标类型"), Category("状态切换")]
        public int TargetType { get { return mTargetType; } set { mTargetType = value; } }

        int mCondition = 0;
        [XmlAttribute("Condition"), DefaultValue(0), DisplayName("切换条件"), Category("状态切换")]
        public int Condition { get { return mCondition; } set { mCondition = value; } }

        bool mTargetExist = false;
        [XmlAttribute("TargetExist"), DefaultValue(false), DisplayName("目标是否存在"), Category("是否存在")]
        public bool TargetExist { get { return mTargetExist; } set { mTargetExist = value; } }

        String mTargetActionID = "";
        [XmlAttribute("TargetActionID"), DefaultValue(""), DisplayName("动作编号"), Category("监测动作")]
        public String TargetActionID { get { return mTargetActionID; } set { mTargetActionID = value; } }

        int mTargetActionCount = 1;
        [XmlAttribute("TargetActionCount"), DefaultValue(1), DisplayName("执行次数"), Category("监测动作")]
        public int TargetActionCount { get { return mTargetActionCount; } set { mTargetActionCount = value; } }

        int mTargetVaribleName = 0;
        [XmlAttribute("TargetVaribleName"), DefaultValue(0), DisplayName("变量名称"), Category("监测变量")]
        public int TargetVaribleName { get { return mTargetVaribleName; } set { mTargetVaribleName = value; } }

        int mTargetVaribleCompare = 0;
        [XmlAttribute("TargetVaribleCompare"), DefaultValue(0), DisplayName("变量比较条件"), Category("监测变量")]
        public int TargetVaribleCompare { get { return mTargetVaribleCompare; } set { mTargetVaribleCompare = value; } }

        int mTargetVaribleValue = 0;
        [XmlAttribute("TargetVaribleValue"), DefaultValue(0), DisplayName("变量比较数值"), Category("监测变量")]
        public int TargetVaribleValue { get { return mTargetVaribleValue; } set { mTargetVaribleValue = value; } }

        int mTargetDistanceMin = 0;
        [XmlAttribute("TargetDistanceMin"), DefaultValue(0), DisplayName("监控距离最小值"), Category("监控距离")]
        public int TargetDistanceMin { get { return mTargetDistanceMin; } set { mTargetDistanceMin = value; } }

        int mTargetDistanceMax = 1000;
        [XmlAttribute("TargetDistanceMax"), DefaultValue(1000), DisplayName("监控距离最大值"), Category("监控距离")]
        public int TargetDistanceMax { get { return mTargetDistanceMax; } set { mTargetDistanceMax = value; } }

        [XmlAttribute("TargetDistance"), DefaultValue(1000), Browsable(false)]
        public int TargetDistance { get { return 1000; } set { mTargetDistanceMax = value; } }

        int mTargetSkill = 0;
        [XmlAttribute("TargetSkill"), DefaultValue(0), DisplayName("呼叫技能"), Category("呼叫技能")]
        public int TargetSkill { get { return mTargetSkill; } set { mTargetSkill = value; } }


        String mSelfActionID = "";
        [XmlAttribute("SelfActionID"), DefaultValue(""), DisplayName("动作编号"), Category("监测动作")]
        public String SelfActionID { get { return mSelfActionID; } set { mSelfActionID = value; } }

        int mSelfActionCount = 1;
        [XmlAttribute("SelfActionCount"), DefaultValue(1), DisplayName("执行次数"), Category("监测动作")]
        public int SelfActionCount { get { return mSelfActionCount; } set { mSelfActionCount = value; } }

        int mSelfVaribleName = 0;
        [XmlAttribute("SelfVaribleName"), DefaultValue(0), DisplayName("变量名称"), Category("监测变量")]
        public int SelfVaribleName { get { return mSelfVaribleName; } set { mSelfVaribleName = value; } }

        int mSelfVaribleCompare = 0;
        [XmlAttribute("SelfVaribleCompare"), DefaultValue(0), DisplayName("变量比较条件"), Category("监测变量")]
        public int SelfVaribleCompare { get { return mSelfVaribleCompare; } set { mSelfVaribleCompare = value; } }

        int mSelfVaribleValue = 0;
        [XmlAttribute("SelfVaribleValue"), DefaultValue(0), DisplayName("变量比较数值"), Category("监测变量")]
        public int SelfVaribleValue { get { return mSelfVaribleValue; } set { mSelfVaribleValue = value; } }

        int mSelfTime = 30000;
        [XmlAttribute("SelfTime"), DefaultValue(30000), DisplayName("到达时间"), Category("到达时间")]
        public int SelfTime { get { return mSelfTime; } set { mSelfTime = value; } }

        int mSelfSkill = 0;
        [XmlAttribute("SelfSkill"), DefaultValue(0), DisplayName("呼叫技能"), Category("呼叫技能")]
        public int SelfSkill { get { return mSelfSkill; } set { mSelfSkill = value; } }
		
		
        int mActionCache = 0;
        public int ActionCache { get { return mActionCache; } set { mActionCache = value; } }
		
		int mTargetActionCache = 0;
        public int TargetActionCache { get { return mTargetActionCache; } set { mTargetActionCache = value; } }

        public override string ToString()
        {
            return "转换到[" + SwitchStatusID + "]";
        }
    }
}
