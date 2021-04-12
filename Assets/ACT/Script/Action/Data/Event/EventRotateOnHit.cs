using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.ComponentModel;

namespace Data
{
    [Serializable]
    public class EventRotateOnHit : EventData
    {
        private bool mRotate = false;
        [XmlAttribute("Rotate"), DisplayName("受击后转向"), DefaultValue(false)]
        public bool Rotate { get { return mRotate; } set { mRotate = value; } }

        public override EventType Type() { return EventType.RotateOnHit; }
        
        public override string ToString()
        {
            return "受击后转向";
        }
        
        public override String EventContent()
        {
            return Rotate.ToString();
        }
    }
}
