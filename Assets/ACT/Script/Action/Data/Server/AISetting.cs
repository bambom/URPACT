using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace Data.Server
{
    [Serializable]
    [TypeConverterAttribute(typeof(ExpandableObjectConverter))]
    public class AISetting
    {
        public AISetting() { }

        AIGroup mEasyGroup = new AIGroup("简单");
        public AIGroup EasyGroup { get { return mEasyGroup; } set { mEasyGroup = value; } }

        AIGroup mNormalGroup = new AIGroup("普通");
        public AIGroup NormalGroup { get { return mNormalGroup; } set { mNormalGroup = value; } }

        AIGroup mHardGroup = new AIGroup("困难");
        public AIGroup HardGroup { get { return mHardGroup; } set { mHardGroup = value; } }

        AIGroup mNightmareGroup = new AIGroup("噩梦");
        public AIGroup NightmareGroup { get { return mNightmareGroup; } set { mNightmareGroup = value; } }

        public void BuildActionCache(ActionGroup group)
        {
            mEasyGroup.BuildActionCache(group);
            mNormalGroup.BuildActionCache(group);
            mHardGroup.BuildActionCache(group);
            mNightmareGroup.BuildActionCache(group);
        }

        public override string ToString()
        {
            return "AISetting";
        }
    }
}
