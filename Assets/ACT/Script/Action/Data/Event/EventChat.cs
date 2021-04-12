using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.ComponentModel;

namespace Data
{
    [Serializable]
    public class EventCharData
    {
        private int mCahrID = 0;
        [XmlAttribute("CahrID"), DisplayName("喊话ID"), DefaultValue(0)]
        public int CahrID { get { return mCahrID; } set { mCahrID = value; } }

        private int mProbability = 0;
        [XmlAttribute("Probability"), DisplayName("喊话概率"), DefaultValue(0)]
        public int Probability { get { return mProbability; } set { mProbability = value; } }
       
        public override string ToString()
        {
            return "喊话内容";
        }
    };

    [Serializable]
    public class EventChat: EventData
    {
        private List<EventCharData> mCharDataList = new List<EventCharData>();
        public List<EventCharData> CharDataList { get { return mCharDataList; } set { mCharDataList = value; } }
        
        public override EventType Type() { return EventType.Chat; }
        
        public override string ToString()
        {
            return "喊话";
        }
        
        public override String EventContent()
        {
            String ret = "";
            foreach(EventCharData data in CharDataList)
                ret = ret + data .CahrID.ToString() + "(" + data.Probability.ToString() + ")" + " ";
            return ret;
        }
    }
}
