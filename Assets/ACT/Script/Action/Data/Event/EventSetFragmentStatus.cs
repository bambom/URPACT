using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.ComponentModel;

namespace Data
{
    [Serializable]
    public class EventSetFragmentStatus : EventData
    {
        private int mFragmentStatus = 0;
        [XmlAttribute("FragmentStatus"), DisplayName("片段状态"), DefaultValue(0)]
        public int FragmentStatus { get { return mFragmentStatus; } set { mFragmentStatus = value; } }

        public override EventType Type() { return EventType.SetFragmentStatus; }
        
        public override string ToString()
        {
            return "设置Fragment状态";
        }

        public override String EventContent()
        {
            return FragmentStatus.ToString();
        }
    }
}
