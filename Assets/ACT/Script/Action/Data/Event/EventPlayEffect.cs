using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.ComponentModel;

namespace Data
{
    [Serializable]
    public class EventPlayEffect : EventData
    {
        private String mEffectName = String.Empty;
        [XmlAttribute("EffectName"), DisplayName("特效名字"), DefaultValue("")]
        public String EffectName { get { return mEffectName; } set { mEffectName = value; } }

        private float mOffsetX = 0;
        [XmlAttribute("OffsetX"), DisplayName("偏移X"), DefaultValue(0)]
        public float OffsetX { get { return mOffsetX; } set { mOffsetX = value; } }

        private float mOffsetY = 0;
        [XmlAttribute("OffsetY"), DisplayName("偏移Y"), DefaultValue(0)]
        public float OffsetY { get { return mOffsetY; } set { mOffsetY = value; } }

        private float mOffsetZ = 0;
        [XmlAttribute("OffsetZ"), DisplayName("偏移Z"), DefaultValue(0)]
        public float OffsetZ { get { return mOffsetZ; } set { mOffsetZ = value; } }

        private int mStopMode = 0;
        [XmlAttribute("StopMode")]
        [DisplayName("停止方式：0:与动作无关; 1: 随着动作结束而结束; 2:放者的动作状态=受伤结束; 3:动作被中断特效终止")]
        [DefaultValue(0)]
        public int StopMode { get { return mStopMode; } set { mStopMode = value; } }

        private int mBindMode = 0;
        [XmlAttribute("BindMode"), DisplayName("绑定方式"), DefaultValue(0)]
        public int BindMode { get { return mBindMode; } set { mBindMode = value; } }

        private float mScale = 1.0f;
        private float mScaleTime = 1.0f;

        [XmlAttribute("VisibleType"), DisplayName("可见对象"), DefaultValue(0)]
        [Description("0=所有玩家阵营可见（默认） 1=自己与友方阵营可见 2=敌方阵营可见 3=仅自己可见")]
        public int VisibleType { get; set; }

        public override EventType Type() { return EventType.PlayEffect; }
        
        public override string ToString()
        {
            return "播放特效";
        }

        public override String EventContent()
        {
            return EffectName + " " + OffsetX.ToString() + " " +
                OffsetY.ToString() + " " + OffsetZ.ToString() + " "
                + StopMode.ToString() + " " + BindMode.ToString();
        }
    }
}
