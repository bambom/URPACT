using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.ComponentModel;

namespace Data
{
    [Serializable]
    public class EventSetVelocity_Y : EventData
    {
        private int mVelocityY = 0;
        [XmlAttribute("VelocityY"), DisplayName("速度Y分量"), DefaultValue(0)]
        public int VelocityY { get { return mVelocityY; } set { mVelocityY = value; } }

        public override EventType Type() { return EventType.SetVelocity_Y; }
        
        public override string ToString()
        {
            return "设置Y方向速度";
        }

        public override String EventContent()
        {
            return VelocityY.ToString();
        }
    }
}
