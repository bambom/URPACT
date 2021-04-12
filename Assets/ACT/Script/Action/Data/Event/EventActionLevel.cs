using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.ComponentModel;

namespace Data
{
    [Serializable]
    public class EventActionLevel: EventData
    {
        private int mLevel = 0;
        [XmlAttribute("Level"), DisplayName("动作等级"), DefaultValue(0)]
        public int Level { get { return mLevel; } set { mLevel = value; } }

        public override EventType Type() { return EventType.ActionLevel; }
        
        public override string ToString()
        {
            return "动作等级提升";
        }

        public override String EventContent()
        {
            return Level.ToString();
        }
    }
}
