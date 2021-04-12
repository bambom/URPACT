using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.ComponentModel;

namespace Data
{
    [Serializable]
    public class EventSetDirection: EventData
    {
        private int mAngle = 0;
        [XmlAttribute("Angle"), DisplayName("角度"), DefaultValue(0)]
        public int Angle { get { return mAngle; } set { mAngle = value; } }

        private bool mLocal = false;
        [XmlAttribute("Local"), DisplayName("相对于局部坐标系"), DefaultValue(false)]
        public bool Local { get { return mLocal; } set { mLocal = value; } }

        public override EventType Type() { return EventType.SetDirection; }
        
        public override string ToString()
        {
            return "设置朝向";
        }

        public override String EventContent()
        {
            return Angle.ToString() + " " + Local.ToString();
        }
    }
}
