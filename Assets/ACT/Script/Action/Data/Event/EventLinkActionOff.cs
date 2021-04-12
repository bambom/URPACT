using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.ComponentModel;

namespace Data
{
    [Serializable]
    public class EventLinkActionOff : EventData
    {
        private int mActionRequestIdx = 0;
        [XmlAttribute("ActionRequestIdx"), DisplayName("连接关闭动作索引"), DefaultValue(0)]
        public int ActionRequestIdx { get { return mActionRequestIdx; } set { mActionRequestIdx = value; } }

        public override EventType Type() { return EventType.LinkActionOff; }
        
        public override string ToString()
        {
            return "关闭连接动作";
        }
        
        public override String EventContent()
        {
            return ActionRequestIdx.ToString();
        }
    }
}
