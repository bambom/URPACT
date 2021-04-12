using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.ComponentModel;

namespace Data.Server
{
    [Serializable]
    [TypeConverterAttribute(typeof(ExpandableObjectConverter))]
    public class AISlot
    {
        int mCount = 1;
        [XmlAttribute("Count"), DefaultValue(1), DisplayName("触发次数"), Category("触发条件")]
        public int Count { get { return mCount; } set { mCount = value; } }

        int mRatio = 1;
        [XmlAttribute("Ratio"), DefaultValue(1), DisplayName("触发几率"), Category("触发条件")]
        public int Ratio { get { return mRatio; } set { mRatio = value; } }

        int mSkillID = 0;
        [XmlAttribute("SkillID"), DefaultValue(0), DisplayName("呼叫技能"), Category("触发结果")]
        [Description("呼叫技能编号，用来计算伤害数值，0为不呼叫技能。默认值为0")]
        public int SkillID { get { return mSkillID; } set { mSkillID = value; } }

        int mRefreshTargetList = 1;
        [XmlAttribute("RefreshTargetList"), DefaultValue(1), DisplayName("刷新目标列表"), Category("触发结果")]
        [Description("1=刷新目标列表，0=不刷新。默认值为1")]
        public int RefreshTargetList { get { return mRefreshTargetList; } set { mRefreshTargetList = value; } }

        String mSwitchActionID = "";
        [XmlAttribute("SwitchActionID"), DefaultValue(""), DisplayName("动作编号"), Category("触发结果")]
        [Description("所切换到动作的编号")]
        public String SwitchActionID { get { return mSwitchActionID; } set { mSwitchActionID = value; } }

        int mActionCache = 0;
        public int ActionCache { get { return mActionCache; } set { mActionCache = value; } }

        public class AISlotCompare : Comparer<AISlot>
        {
            public override int Compare(AISlot x, AISlot y)
            {
                return x.SwitchActionID.CompareTo(y.SwitchActionID);
            }
        }

        public override string ToString()
        {
            return String.Format("[{0}][{1}][{2}]", new object[] { SwitchActionID, Ratio, Count });
        }
    }
}
