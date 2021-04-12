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
    public class AIStatus
    {
        String mName = "默认状态";
        [XmlAttribute("Name"), DisplayName("状态名称")]
        public String Name { get { return mName; } set { mName = value; } }

        int mTargetType = 1;
        [XmlAttribute("TargetType"), DefaultValue(1), DisplayName("目标类型"), Category("状态切换")]
        public int TargetType { get { return mTargetType; } set { mTargetType = value; } }

        List<AIList> mAILists = new List<AIList>();
        public List<AIList> AILists { get { return mAILists; } }

        List<AIStatusSwitch> mAIStatusSwitchList = new List<AIStatusSwitch>();
        public List<AIStatusSwitch> AIStatusSwitchList { get { return mAIStatusSwitchList; } }

        List<AIActionCD> mAIActionCDList = new List<AIActionCD>();
        public List<AIActionCD> AIActionCDList { get { return mAIActionCDList; } }

        public override string ToString()
        {
            return Name;
        }
    }
}
