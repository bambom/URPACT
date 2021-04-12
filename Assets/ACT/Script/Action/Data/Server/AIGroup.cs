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
    public class AIGroup
    {
        public AIGroup() { }
        public AIGroup(String name) { mName = name; }

        String mName = "默认难度";
        [XmlAttribute("Name")]
        public String Name { get { return mName; } set { mName = value; } }

        private List<AIStatus> mAIStatusList = new List<AIStatus>();
        public List<AIStatus> AIStatusList { get { return mAIStatusList; } }

        public AIStatus GetStatusByName(String name)
        {
            foreach (AIStatus status in AIStatusList)
            {
                if (status.Name == name)
                    return status;
            }
            return null;
        }

        public void BuildActionCache(ActionGroup group)
        {
            foreach (AIStatus status in mAIStatusList)
            {
                foreach (AIList list in status.AILists)
                {
                    foreach (AISlot slot in list.AISlots)
                        slot.ActionCache = group.GetActionIdx(slot.SwitchActionID);
                }

                foreach (AIStatusSwitch sw in status.AIStatusSwitchList)
                    sw.ActionCache = group.GetActionIdx(sw.SelfActionID);
				
				foreach (AIStatusSwitch sw in status.AIStatusSwitchList)
                    sw.TargetActionCache = group.GetActionIdx(sw.TargetActionID);

                foreach (AIActionCD actionCD in status.AIActionCDList)
                    actionCD.ActionCache = group.GetActionIdx(actionCD.Action);
            }
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
