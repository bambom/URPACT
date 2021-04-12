using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.ComponentModel;

namespace Data
{
    [Serializable]
    public class AnimSlot
    {
        const String ANIM_CATEGORY = "动画设置";

        String mAnimation = "";
        [XmlAttribute("Animation"), DisplayName("动画"), Description("动画的名字"), Category(ANIM_CATEGORY)]
        public String Animation { get { return mAnimation; } set { mAnimation = value; } }

        int mStart = 0;
        [XmlAttribute("Start"), DisplayName("起始"), DefaultValue(0), Description("动画起始的在原动画的百分比"), Category(ANIM_CATEGORY)]
        public int Start { get { return mStart; } set { mStart = value; } }

        int mEnd = 100;
        [XmlAttribute("End"), DisplayName("结束"), DefaultValue(100), Description("动画结束的在原动画的百分比"), Category(ANIM_CATEGORY)]
        public int End { get { return mEnd; } set { mEnd = value; } }

        int mWeight = 1;
        [XmlAttribute("Weight"), DisplayName("权重"), DefaultValue(1), Description("动画的权重"), Category(ANIM_CATEGORY)]
        public int Weight { get { return mWeight; } set { mWeight = value; } }

        List<AnimControl> mAnimControlList = new List<AnimControl>();
        public List<AnimControl> AnimControlList { get { return mAnimControlList; } set { mAnimControlList = value; } }

        public void AddStartEnd()
        {
            AnimControl start_control = new AnimControl();
            start_control.ActionTimeProportion = 0;
            start_control.AnimTimeProportion = Start;
            AnimControlList.Add(start_control);

            AnimControl end_control = new AnimControl();
            end_control.ActionTimeProportion = 100;
            end_control.AnimTimeProportion = End;
            AnimControlList.Add(end_control);
        }

        public bool IsFullAnim() { return mStart == 0 && mEnd == 100; }

        public override string ToString()
        {
            return mAnimation + "[" + Weight + "]";
        }
    }

    [Serializable]
    public class AnimControl
    {
        const String ANIM_CONTROL_CATEGORY = "动画控制";

        int mActionTimeProportion = 0;
        [DisplayName("动作时间比例")]
        [Description("动作时间比例")]
        [Category(ANIM_CONTROL_CATEGORY)]
        public int ActionTimeProportion { get { return mActionTimeProportion; } set { mActionTimeProportion = value; } }

        int mAnimTimeProportion = 0;
        [DisplayName("动画时间比例")]
        [Description("动画时间比例")]
        [Category(ANIM_CONTROL_CATEGORY)]
        public int AnimTimeProportion { get { return mAnimTimeProportion; } set { mAnimTimeProportion = value; } }

        public override string ToString()
        {
            return "动画控制";
        }
    }
}
