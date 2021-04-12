using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.ComponentModel;

namespace Data
{
    [Serializable]
    public class EventHasCollision : EventData
    {
        private bool mHasCollision = false;
        [XmlAttribute("HasCollision"), DisplayName("产生碰撞 "), DefaultValue(false)]
        public bool HasCollision { get { return mHasCollision; } set { mHasCollision = value; } }

        public override EventType Type() { return EventType.HasCollision; }
       
        public override string ToString()
        {
            return "有碰撞";
        }
        
        public override String EventContent()
        {
            return HasCollision.ToString();
        }
    }
}
