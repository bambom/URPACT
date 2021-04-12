using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.ComponentModel;

namespace Data
{
    [Serializable]
    public class EventGoToTargets : EventData
    {
        private int mOffsetX = 0;
        [XmlAttribute("OffsetX"), DisplayName("偏移量X"), DefaultValue(0)]
        public int OffsetX { get { return mOffsetX; } set { mOffsetX = value; } }

        private int mOffsetY = 0;
        [XmlAttribute("OffsetY"), DisplayName("偏移量Y"), DefaultValue(0)]
        public int OffsetY { get { return mOffsetY; } set { mOffsetY = value; } }

        private int mOffsetZ = 0;
        [XmlAttribute("OffsetZ"), DisplayName("偏移量Z"), DefaultValue(0)]
        public int OffsetZ { get { return mOffsetZ; } set { mOffsetZ = value; } }

        private int mTargetType = 0;
        [XmlAttribute("TargetType"), DisplayName("目标类型"), DefaultValue(0)]
        public int TargetType { get { return mTargetType; } set { mTargetType = value; } }

        private bool mLocal = false;
        [XmlAttribute("Local"), DisplayName("局部坐标系"), DefaultValue(false)]
        public bool Local { get { return mLocal; } set { mLocal = value; } }

        private bool mRandom = false;
        [XmlAttribute("Random"), DisplayName("随机"), DefaultValue(false)]
        public bool Random { get { return mRandom; } set { mRandom = value; } }

        public override EventType Type() { return EventType.GoToTargets; }
       
        public override string ToString()
        {
            return "走向目标";
        }

        public override String EventContent()
        {
            return OffsetX.ToString() + " " + OffsetY.ToString() + " " +
                OffsetZ.ToString() + " " + Local.ToString() + Random.ToString() + " " +
                    TargetType.ToString();
        }
    }
}
