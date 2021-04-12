using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.ComponentModel;

namespace Data
{
    [Serializable]
    public class EventControlUnit : EventData
    {
        private int mRace = 0;
        [XmlAttribute("Race"), DisplayName("怪物种类"), DefaultValue(0)]
        public int Race { get { return mRace; } set { mRace = value; } }

        private String mActionId = String.Empty;
        [XmlAttribute("ActionId"), DisplayName("动作ID"), DefaultValue("")]
        public String ActionId { get { return mActionId; } set { mActionId = value; } }

        private int mMode = 0;
        [XmlAttribute("Mode"), DisplayName("控制模式"), DefaultValue(0)]
        public int Mode { get { return mMode; } set { mMode = value; } }

        private int mSkillId = 0;
        [XmlAttribute("SkillId"), DisplayName("技能ID"), DefaultValue(0)]
        public int SkillId { get { return mSkillId; } set { mSkillId = value; } }

        public override EventType Type() { return EventType.ControlUnit; }
        public override string ToString()
        {
            return "控制Unit";
        }

       
        public override String EventContent()
        {
            return Race.ToString() + " " + ActionId + " " + Mode.ToString() + " " + SkillId.ToString();
        }
    }
}
