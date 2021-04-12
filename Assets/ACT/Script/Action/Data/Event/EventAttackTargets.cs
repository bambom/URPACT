using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.ComponentModel;

namespace Data
{
    [Serializable]
    public class EventAttackTargets : EventData
    {
        private int mHitIndex = 0;
        [XmlAttribute("HitIndex"), DisplayName("攻击定义索引"), DefaultValue(0)]
        public int HitIndex { get { return mHitIndex; } set { mHitIndex = value; } }

        private int mCount = 0;
        [XmlAttribute("Count"), DisplayName("攻击次数"), DefaultValue(0)]
        public int Count { get { return mCount; } set { mCount = value; } }

        private int mRandom = 0;
        [XmlAttribute("Random"), DisplayName("随机"), DefaultValue(0)]
        public int Random { get { return mRandom; } set { mRandom = value; } }

        private int mAttackType = 0;
        [XmlAttribute("AttackType"), DisplayName("攻击目标的类型"), DefaultValue(0)]
        public int AttackType { get { return mAttackType; } set { mAttackType = value; } }


        public override EventType Type() { return EventType.AttackTargets; }
       
        public override string ToString()
        {
            return "攻击敌人";
        }
        
        public override String EventContent()
        {
            return HitIndex.ToString() + " " + Count.ToString() + " " +
                Random.ToString() + " " + AttackType.ToString();
        }
    }
}
