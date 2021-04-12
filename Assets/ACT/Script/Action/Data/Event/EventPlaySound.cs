using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.ComponentModel;

namespace Data
{
    [Serializable]
    public class EventPlaySound: EventData
    {
        private String mSoundName = String.Empty;
        [XmlAttribute("SoundName"), DisplayName("声音名字"), DefaultValue("")]
        public String SoundName { get { return mSoundName; } set { mSoundName = value; } }

        private bool mCheckMatril = false;
        [XmlAttribute("CheckMatril"), DisplayName("检查材质"), DefaultValue(false)]
        public bool CheckMatril { get { return mCheckMatril; } set { mCheckMatril = value; } }

        [Browsable(false)]
        public int SoundIndex = -2;

        public override EventType Type() { return EventType.PlaySound; }
        
        public override string ToString()
        {
            return "播放声音";
        }
       
        public override String EventContent()
        {
            return SoundName;
        }
    }
}
