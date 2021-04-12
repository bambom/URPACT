using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.ComponentModel;

namespace Data
{
    [Serializable]
    public class EventSetVelocity : EventData
    {
        private int mVelocityX = 0;
        [XmlAttribute("VelocityX"), DisplayName("速度X分量"), DefaultValue(0)]
        public int VelocityX { get { return mVelocityX; } set { mVelocityX = value; } }

        private int mVelocityY = 0;
        [XmlAttribute("VelocityY"), DisplayName("速度Y分量"), DefaultValue(0)]
        public int VelocityY { get { return mVelocityY; } set { mVelocityY = value; } }

        private int mVelocityZ = 0;
        [XmlAttribute("VelocityZ"), DisplayName("速度Z分量"), DefaultValue(0)]
        public int VelocityZ { get { return mVelocityZ; } set { mVelocityZ = value; } }

        public override EventType Type() { return EventType.SetVelocity; }
        
        public override string ToString()
        {
            return "设置速度";
        }

        public override String EventContent()
        {
            return VelocityX.ToString() + " " + VelocityY.ToString() + " " + VelocityZ.ToString(); 
        }
    }
}
