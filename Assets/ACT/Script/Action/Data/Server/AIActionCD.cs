using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.ComponentModel;

namespace Data.Server
{
    [Serializable]
    public class AIActionCD
    {
        String mAction = "N0000";
        [XmlAttribute("Action"), DisplayName("动作ID")]
        public String Action { get { return mAction; } set { mAction = value; } }

        int mCD = 1000;
        [XmlAttribute("CD"), DisplayName("CD值(毫秒)")]
        public int CD { get { return mCD; } set { mCD = value; } }

        int mActionCache = 0;
        public int ActionCache { get { return mActionCache; } set { mActionCache = value; } }

        public override string ToString()
        {
            return String.Format("[{0}][{1}]", Action, CD);
        }
    }
}
