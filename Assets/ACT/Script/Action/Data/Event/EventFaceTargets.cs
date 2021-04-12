using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.ComponentModel;

namespace Data
{
    [Serializable]
    public class EventFaceTargets : EventData
    {
        private int mFaceType = 0;
        [XmlAttribute("FaceType"), DisplayName("面向类型"), DefaultValue(0)]
        public int FaceType { get { return mFaceType; } set { mFaceType = value; } }

        public override EventType Type() { return EventType.ActionLevel; }
       
        public override string ToString()
        {
            return "面向目标";
        }

        public override String EventContent()
        {
            return FaceType.ToString();
        }
    }
}
