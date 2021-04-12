using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.ComponentModel;

namespace Data
{
    [Serializable]
    public class EventSetHeightStatus : EventData
    {
        private int mHeightStatus = 0;
        [XmlAttribute("HeightStatus"), DisplayName("高度状态"), DefaultValue(0)]
        public int HeightStatus { get { return mHeightStatus; } set { mHeightStatus = value; } }

        public override EventType Type() { return EventType.SetHeightStatus; }
       
        public override string ToString()
        {
            return "设置高度状态";
        }

        public override String EventContent()
        {
            return HeightStatus.ToString();
        }
    }
}
