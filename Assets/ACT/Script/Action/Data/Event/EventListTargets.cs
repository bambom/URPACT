using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.ComponentModel;

namespace Data
{
    [Serializable]
    public class EventListTargets : EventData
    {
        private int mLeft = -100;
        [XmlAttribute("Left"), DisplayName("左边范围"), DefaultValue(-100)]
        public int Left { get { return mLeft; } set { mLeft = value; } }

        private int mRight = 100;
        [XmlAttribute("Right"), DisplayName("右边范围"), DefaultValue(100)]
        public int Right { get { return mRight; } set { mRight = value; } }

        private int mBack = 0;
        [XmlAttribute("Back"), DisplayName("后面范围"), DefaultValue(0)]
        public int Back { get { return mBack; } set { mBack = value; } }

        private int mFront = 200;
        [XmlAttribute("Front"), DisplayName("前面范围"), DefaultValue(200)]
        public int Front { get { return mFront; } set { mFront = value; } }

        private int mBottom = 0;
        [XmlAttribute("Bottom"), DisplayName("底部范围"), DefaultValue(0)]
        public int Bottom { get { return mBottom; } set { mBottom = value; } }

        private int mTop = 200;
        [XmlAttribute("Top"), DisplayName("顶部范围"), DefaultValue(200)]
        public int Top { get { return mTop; } set { mTop = value; } }

        private ListTargetFrameType mListType = ListTargetFrameType.Cuboid_ListType;
        [XmlAttribute("ListType"), DisplayName("列举类型"), DefaultValue(ListTargetFrameType.Cuboid_ListType)]
        public ListTargetFrameType ListType { get { return mListType; } set { mListType = value; } }

        [XmlAttribute("ListMode"), DisplayName("列举方式"), DefaultValue(ListTargetMode.MinDistance)]
        public ListTargetMode ListMode { get; set; }

        private int mFanRadius = 0;
        [XmlAttribute("FanRadius"), DisplayName("扇形半径"), DefaultValue(0)]
        public int FanRadius { get { return mFanRadius; } set { mFanRadius = value; } }

        private int mFanAngle = 0;
        [XmlAttribute("FanAngle"), DisplayName("扇形角度"), DefaultValue(0)]
        public int FanAngle { get { return mFanAngle; } set { mFanAngle = value; } }

        public override EventType Type() { return EventType.ListTargets; }
        
        public override string ToString()
        {
            return "列举目标";
        }

        public override String EventContent()
        {
            return Left.ToString() + " " + Right.ToString() + " " +
                Back.ToString() + Front.ToString() + " " +
                Bottom.ToString() + Top.ToString();
        }
    }
}
