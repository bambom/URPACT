using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.ComponentModel;

namespace Data
{
    [Serializable]
    public class EventSetColor : EventData
    {
        private int mColorRed = 0;
        [XmlAttribute("ColorRed"), DisplayName("红色分量"), DefaultValue(0)]
        public int ColorRed { get { return mColorRed; } set { mColorRed = value; } }

        private int mColorGreen = 0;
        [XmlAttribute("ColorGreen"), DisplayName("绿色分量"), DefaultValue(0)]
        public int ColorGreen { get { return mColorGreen; } set { mColorGreen = value; } }

        private int mColorBlue = 0;
        [XmlAttribute("ColorBlue"), DisplayName("蓝色分量"), DefaultValue(0)]
        public int ColorBlue { get { return mColorBlue; } set { mColorBlue = value; } }

        private int mColorAlpha = 0;
        [XmlAttribute("ColorAlpha"), DisplayName("Alpha分量"), DefaultValue(0)]
        public int ColorAlpha { get { return mColorAlpha; } set { mColorAlpha = value; } }

        private int mColorTime = 0;
        [XmlAttribute("ColorTime"), DisplayName("颜色持续时间"), DefaultValue(0)]
        public int ColorTime { get { return mColorTime; } set { mColorTime = value; } }

        public override EventType Type() { return EventType.SetColor; }
       
        public override string ToString()
        {
            return "设置颜色";
        }

        public override String EventContent()
        {
            return ColorGreen.ToString() + " " + ColorGreen.ToString() + " " +
                ColorBlue.ToString() + " " + ColorAlpha.ToString() + " " +
                ColorTime.ToString();
        }
    }
}
