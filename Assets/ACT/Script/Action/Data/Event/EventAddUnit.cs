using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.ComponentModel;

namespace Data
{
    [Serializable]
    public class EventAddUnit : EventData
    {
        private int mId = 0;
        [XmlAttribute("Id"), DisplayName("Unit ID"), DefaultValue(0)]
        public int Id { get { return mId; } set { mId = value; } }

        private String mActionId = String.Empty;
        [XmlAttribute("ActionId"), DisplayName("动作ID"), DefaultValue("")]
        public String ActionId { get { return mActionId; } set { mActionId = value; } }

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

        private bool mLocal = false;
        [XmlAttribute("Local"), DisplayName("局部坐标系"), DefaultValue(false)]
        public bool Local { get { return mLocal; } set { mLocal = value; } }

        private int mSkillId = 0;
        [XmlAttribute("SkillId"), DisplayName("技能id"), DefaultValue(0)]
        public int SkillId { get { return mSkillId; } set { mSkillId = value; } }

        private bool mComboAddParent = false;
        [XmlAttribute("ComboAddParent"), DisplayName("ComboAddParent"), DefaultValue(false)]
        public bool ComboAddParent { get { return mComboAddParent; } set { mComboAddParent = value; } }

        private bool mFollowParent = false;
        [XmlAttribute("FollowParent"), DisplayName("跟随父亲"), DefaultValue(false)]
        public bool FollowParent { get { return mFollowParent; } set { mFollowParent = value; } }


        private bool mRandomRange = false;
        [XmlAttribute("RandomRange"), DisplayName("随机范围"), DefaultValue(false)]
        public bool RandomRange { get { return mRandomRange; } set { mRandomRange = value; } }


        private int mRandomMinWidth = 0;
        [XmlAttribute("RandomMinWidth"), DisplayName("随机宽度最小值"), DefaultValue(0)]
        public int RandomMinWidth { get { return mRandomMinWidth; } set { mRandomMinWidth = value; } }

        private int mRandomMaxWidth = 0;
        [XmlAttribute("RandomMaxWidth"), DisplayName("随机宽度最大值"), DefaultValue(0)]
        public int RandomMaxWidth { get { return mRandomMaxWidth; } set { mRandomMaxWidth = value; } }

        private int mRandomMinDepth = 0;
        [XmlAttribute("RandomMinDepth"), DisplayName("随机深度最小值"), DefaultValue(0)]
        public int RandomMinDepth { get { return mRandomMinDepth; } set { mRandomMinDepth = value; } }

        private int mRandomMaxDepth = 0;
        [XmlAttribute("RandomMaxDepth"), DisplayName("随机深度最大值"), DefaultValue(0)]
        public int RandomMaxDepth { get { return mRandomMaxDepth; } set { mRandomMaxDepth = value; } }

        public override EventType Type() { return EventType.AddUnit; }
        
        public override string ToString()
        {
            return "添加unit";
        }

        public override String EventContent()
        {
            String ret;
            ret = Id.ToString() + " " + PosX.ToString() + " " + PosY.ToString() + " " +
                PosZ.ToString() + " " + Angle.ToString() + " " + ActionId;
            return ret;
        }
    }
}
