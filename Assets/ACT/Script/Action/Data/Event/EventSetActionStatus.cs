using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.ComponentModel;

namespace Data 
{
    [Serializable]
    public class EventSetActionStatus : EventData
    {
        private int mActionStatus = 0;
        [XmlAttribute("ActionStatus"), DisplayName("动作状态"), DefaultValue(0)]
        public int ActionStatus { get { return mActionStatus; } set { mActionStatus = value; } }

        public override EventType Type() { return EventType.SetActionStatus; }
        
        public override string ToString()
        {
            return "设置动作状态";
        }

        public override String EventContent()
        {
            return ActionStatus.ToString();
        }
    }
}
