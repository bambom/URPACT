using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Xml.Serialization;

namespace Data.Server
{
    [Serializable]
    [TypeConverterAttribute(typeof(ExpandableObjectConverter))]
    public class AIList
    {
        private int mDistance = 0;
        [XmlAttribute("Distance"), DefaultValue(0)]
        public int Distance { get { return mDistance; } set { mDistance = value; } }

        public int DistanceSqr { get { return mDistance * mDistance; } }

        private List<AISlot> mAISlots = new List<AISlot>();
        public List<AISlot> AISlots { get { return mAISlots; } }

        public class AIListCompare : Comparer<AIList>
        {
            public override int Compare(AIList x, AIList y)
            {
                return y.Distance - x.Distance;
            }
        }

        public override string ToString()
        {
            return "距离[" + Distance.ToString() + "]";
        }
    }
}
