using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.ComponentModel;

namespace Data
{
    [Serializable]
    public class EventSetVelocity_Z : EventData
    {
        private int mVelocityZ = 0;
        [XmlAttribute("VelocityZ"), DisplayName("速度Z分量"), DefaultValue(0)]
        public int VelocityZ { get { return mVelocityZ; } set { mVelocityZ = value; } }

        public override EventType Type() { return EventType.SetVelocity_Z; }
        
        public override string ToString()
        {
            return "设置Z方向速度";
        }

        public override String EventContent()
        {
            return VelocityZ.ToString();
        }
    }
}
