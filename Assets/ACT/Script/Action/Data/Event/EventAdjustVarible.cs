using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.ComponentModel;

namespace Data
{
    [Serializable]
    public class EventAdjustVarible : EventData
    {
        private int mSlot = 0;
        [XmlAttribute("Slot"), DisplayName("变量槽"), DefaultValue(0)]
        public int Slot { get { return mSlot; } set { mSlot = value; } }

        private int mValue = 0;
        [XmlAttribute("Value"), DisplayName("变量值"), DefaultValue(0)]
        public int Value { get { return mValue; } set { mValue = value; } }

        private int mAppend = 0;
        [XmlAttribute("Append"), DisplayName("附加数据"), DefaultValue(0)]
        public int Append { get { return mAppend; } set { mAppend = value; } }

        public override EventType Type() { return EventType.AdjustVarible; }
        public override string ToString()
        {
            return "修正变量";
        }

        public override String EventContent()
        {
            return Slot.ToString() + " " + Value.ToString() + " " + Append.ToString();
        }
    }
}
