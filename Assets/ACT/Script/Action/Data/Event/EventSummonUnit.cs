using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.ComponentModel;

namespace Data
{
    [Serializable]
    public class EventSummonUnit : EventData
    {
        private int mRace= 0;
        [XmlAttribute("Race"), DisplayName("怪物类型"), DefaultValue(0)]
        public int Race { get { return mRace; } set { mRace = value; } }

        private int mPosX = 0;
        [XmlAttribute("PosX"), DisplayName("位置X"), DefaultValue(0)]
        public int PosX { get { return mPosX; } set { mPosX = value; } }

        private int mPosY = 0;
        [XmlAttribute("PosY"), DisplayName("位置Y"), DefaultValue(0)]
        public int PosY { get { return mPosY; } set { mPosY = value; } }

        private int mPosZ = 0;
        [XmlAttribute("PosZ"), DisplayName("位置Z"), DefaultValue(0)]
        public int PosZ { get { return mPosZ; } set { mPosZ = value; } }

        private int mAngle = 0;
        [XmlAttribute("Angle"), DisplayName("角度"), DefaultValue(0)]
        public int Angle { get { return mAngle; } set { mAngle = value; } }

        private int mModifyAttr = 0;
        [XmlAttribute("ModifyAttr"), DisplayName("修改父亲属性"), DefaultValue(0)]
        public int ModifyAttr { get { return mModifyAttr; } set { mModifyAttr = value; } }

        private bool mLocal = false;
        [XmlAttribute("Local"), DisplayName("局部坐标"), DefaultValue(false)]
        public bool Local { get { return mLocal; } set { mLocal = value; } }

        private String mActionId = String.Empty;
        [XmlAttribute("ActionId"), DisplayName("起始动作"), DefaultValue("")]
        public String ActionId { get { return mActionId; } set { mActionId = value; } }

        public override EventType Type() { return EventType.SummonUnit; }
       
        public override string ToString()
        {
            return "SummonUnit";
        }

        public override String EventContent()
        {
            return Race.ToString() + " " + PosX.ToString() + " " + 
                PosY.ToString() + " " + PosZ.ToString() + " " +
                Angle.ToString() + " " + ActionId;
        }

    }
}
