using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.ComponentModel;

namespace Data
{
    [Serializable]
    public class EventSetVelocity_X : EventData
    {
        private int mVelocityX = 0;
        [XmlAttribute("VelocityX"), DisplayName("速度X分量"), DefaultValue(0)]
        public int VelocityX { get { return mVelocityX; } set { mVelocityX = value; } }

        public override EventType Type() { return EventType.SetVelocity_X; }
        
        public override string ToString()
        {
            return "设置X方向速度";
        }
       
        public override String EventContent()
        {
            return VelocityX.ToString();
        }
    }
}
