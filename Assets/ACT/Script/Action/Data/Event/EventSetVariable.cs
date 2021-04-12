using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.ComponentModel;

namespace Data
{
    [Serializable]
    public class EventSetVariable : EventData
    {
        private int mSlot = 0;
        [XmlAttribute("Slot"), DisplayName("变量槽"), DefaultValue(0)]
        public int Slot { get { return mSlot; } set { mSlot = value; } }

        private int mValue = 0;
        [XmlAttribute("Value"), DisplayName("变量值"), DefaultValue(0)]
        public int Value { get { return mValue; } set { mValue = value; } }

        private int mMaxValue = 0;
        [XmlAttribute("MaxValue"), DisplayName("最大值"), DefaultValue(0)]
        public int MaxValue { get { return mMaxValue; } set { mMaxValue = value; } }

        public override EventType Type() { return EventType.SetVariable; }
       
        public override string ToString()
        {
            return "设置变量";
        }

        public override String EventContent()
        {
            return Slot.ToString() + " " + Value.ToString() + " " + MaxValue.ToString();
        }
    }
}
