using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.ComponentModel;

namespace Data
{
    [Serializable]
    public class EventExeScript : EventData
    {
        private String mScriptCmd = String.Empty;
        [XmlAttribute("ScriptCmd"), DisplayName("脚本命令"), DefaultValue("")]
        public String ScriptCmd { get { return mScriptCmd; } set { mScriptCmd = value; } }

        public override EventType Type() { return EventType.ExeScript; }
        
        public override string ToString()
        {
            return "执行脚本命令";
        }

        public override String EventContent()
        {
            return ScriptCmd;
        }
    }
}
