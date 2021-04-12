using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Xml;
using System.Xml.Serialization;

namespace Data.Camera
{
    [Serializable]
    public class CameraSlot
    {
        String mName = "未命名";
        [XmlAttribute("Name"), DisplayName("名称"), Description("槽的名字，如‘向前移动1个单位’")]
        public String Name { get { return mName; } set { mName = value; } }

        int mDelayTime = 0;
        [XmlAttribute("DelayTime"), DefaultValue(0), DisplayName("延迟时间"), Description("起效延迟时间，单位为毫秒")]
        public int DelayTime { get { return mDelayTime; } set { mDelayTime = value; } }

        float mFOV = 45;
        [XmlAttribute("FOV"), DefaultValue(45), DisplayName("视角")]
        public float FOV { get { return mFOV; } set { mFOV = value; } }

        int mTime = 100;
        [XmlAttribute("Time"), DefaultValue(100), DisplayName("控制时间"), Description("摄像机控制的起效时间，经过这些事件后达到对应的数据项")]
        public int Time { get { return mTime; } set { mTime = value; } }
        
        Vector3 mOffset = new Vector3();
        [DisplayName("相对位移"), Description("摄像机的相对位移值，单位为厘米")]
        public Vector3 Offset { get { return mOffset; } set { mOffset = value; } }

        Vector3 mRotate = new Vector3();
        [DisplayName("相对旋转"), Description("摄像机的相对位移值，单位为角度")]
        public Vector3 Rotate { get { return mRotate; } set { mRotate = value; } }

        bool mAdjustMove = false;
        [XmlAttribute("AdjustMove"), DefaultValue(false), DisplayName("调整位移"), Description("摄像机位移，根据角色的朝向做调整。")]
        public bool AdjustMove { get { return mAdjustMove; } set { mAdjustMove = value; } }

        bool mAdjustRotate = false;
        [XmlAttribute("AdjustRotate"), DefaultValue(false), DisplayName("调整旋转"), Description("摄像机旋转，根据角色的朝向做调整。")]
        public bool AdjustRotate { get { return mAdjustRotate; } set { mAdjustRotate = value; } }

        public override string ToString()
        {
            return Name;
        }
    }
}
