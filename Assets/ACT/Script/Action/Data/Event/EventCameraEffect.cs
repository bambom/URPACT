using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.ComponentModel;

namespace Data
{
    [Serializable]
    public class EventCameraEffect : EventData
    {
        private int mCamerId = 0;
        [XmlAttribute("CamerId"), DisplayName("相机ID"), DefaultValue(0)]
        public int CamerId { get { return mCamerId; } set { mCamerId = value; } }

        public override EventType Type() { return EventType.CameraEffect; }
        
        public override string ToString()
        {
            return "相机效果";
        }
        
        public override String EventContent()
        {
            return CamerId.ToString();
        }
    }
}
