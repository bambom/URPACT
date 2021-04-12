using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Xml.Serialization;

namespace Data.Camera
{
    [Serializable]
    public class CameraGroup
    {
        int mID = 1001;
        [XmlAttribute("ID")]
        public int ID { get { return mID; } set { mID = value; } }

        String mName = "摄像机";
        [XmlAttribute("Name")]
        public String Name { get { return mName; } set { mName = value; } }

        int mAttenuation = 0;
        [XmlAttribute("Attenuation"), DefaultValue(0), DisplayName("衰减距离"), Description("控制摄像机的衰减距离，只对一级摄像机起效！！！默认值为0")]
        public int Attenuation { get { return mAttenuation; } set { mAttenuation = value; } }

        bool mFirstLevel = false;
        [XmlAttribute("FirstLevel"), DefaultValue(false), DisplayName("是否一级摄像机"), Description("设置此参数作用于摄像机层次的控制，默认为二级摄像机控制。")]
        public bool FirstLevel { get { return mFirstLevel; } set { mFirstLevel = value; } }

        bool mResetParam = true;
        [XmlAttribute("ResetParam"), DefaultValue(true), DisplayName("是否重置摄像机"), Description("设置此参数作用于摄像机控制开始状态，默认为每次重置。")]
        public bool ResetParam { get { return mResetParam; } set { mResetParam = value; } }

        private List<CameraSlot> mCameraSlots = new List<CameraSlot>();
        public List<CameraSlot> CameraSlots { get { return mCameraSlots; } }

        public override string ToString()
        {
            return String.Format("{0}({1})", ID, Name);
        }
    }
}
