﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.ComponentModel;

namespace Data
{
    [Serializable]
    public class EventStatusOn : EventData
    {
        private String mStatusName = String.Empty;
        [XmlAttribute("StatusName"), DisplayName("状态名字"), DefaultValue("")]
        public String StatusName { get { return mStatusName; } set { mStatusName = value; } }

        public override EventType Type() { return EventType.StatusOn; }
        
        public override string ToString()
        {
            return "开启状态";
        }

        public override String EventContent()
        {
            return StatusName;
        }
    }
}
