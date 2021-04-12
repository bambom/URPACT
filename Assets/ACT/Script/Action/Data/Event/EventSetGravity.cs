using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.ComponentModel;

namespace Data
{
    [Serializable]
    public class EventSetGravity : EventData
    {
        private int mGravity = 0;
        [XmlAttribute("Gravity"), DisplayName("设置重力"), DefaultValue(0)]
        public int Gravity { get { return mGravity; } set { mGravity = value; } }

        public override EventType Type() { return EventType.SetGravity; }
        
        public override string ToString()
        {
            return "设置重力";
        }

        public override String EventContent()
        {
            return Gravity.ToString();
        }
    }
}
