using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.IO;
using System.Xml.Serialization;
using Data.Server;

namespace Data
{
    [Serializable]
    public class UnitActionInfo : ICloneable
    {
        [Serializable]
        public class UnitVarible
        {
            int mIndex = 0;
            [DefaultValue(0), DisplayName("索引"), XmlAttribute("Index"), Category("索引")]
            public int Index { get { return mIndex; } set { mIndex = value; } }

            int mValue = 0;
            [DefaultValue(0), DisplayName("当前值"), XmlAttribute("Value"), Category("值")]
            public int Value { get { return mValue; } set { mValue = value; } }

            int mMax = 0;
            [DefaultValue(0), DisplayName("最大值"), XmlAttribute("Max"), Category("值")]
            public int Max { get { return mMax; } set { mMax = value; } }

            int mAutoIncTime = 0;
            [XmlAttribute("AutoIncTime"), DefaultValue(0), DisplayName("自增时间"), Category("变化"), Description("【测试之用】自定义变量的自增时间，单位为毫秒")]
            public int AutoIncTime { get { return mAutoIncTime; } set { mAutoIncTime = value; } }

            int mAutoIncValue = 0;
            [XmlAttribute("AutoIncValue"), DefaultValue(0), DisplayName("自增值"), Category("变化"), Description("【测试之用】自定义变量的自增值，单位为点数")]
            public int AutoIncValue { get { return mAutoIncValue; } set { mAutoIncValue = value; } }

            public override string ToString()
            {
                return String.Format("{0}={1}:{2}", Index, Value, Max);
            }
        }

        int mID = 0;
        public int ID { get { return mID; } set { mID = value; } }

        List<UnitVarible> mUnitVaribleList = new List<UnitVarible>();
        [DisplayName("变量列表")]
        public List<UnitVarible> UnitVaribleList { get { return mUnitVaribleList; } }

        List<ActionGroup> mActionGroups = new List<ActionGroup>();
        [DisplayName("动作组列表")]
        public List<ActionGroup> ActionGroups { get { return mActionGroups; } }

        private String mDesc = "";
        [DisplayName("说明")]
        [DefaultValue("")]
        public String Desc { get { return mDesc; } set { mDesc = value; } }

        private Vector3 mScale = new Vector3(1, 1, 1);
        [DisplayName("缩放比例")]
        public Vector3 Scale { get { return mScale; } set { mScale = value; } }

        public void RenameID(int newID)
        {
            ID = newID;

            foreach (ActionGroup group in ActionGroups)
                group.Race = ID;
        }
        
        public bool HasActionGroup(int group_num)
        {
            foreach (ActionGroup action_group in ActionGroups)
            {
                if (action_group.GroupNum == group_num)
                    return true;
            }
            return false;
        }

        public ActionGroup GetActionGroup(int group_num)
        {
            foreach (ActionGroup action_group in ActionGroups)
            {
                if (action_group.GroupNum == group_num)
                    return action_group;
            }
            return null;
        }

        public Object Clone()
        {
            // save current object to xml.
            MemoryStream memoryStream = new MemoryStream();
            XmlSerializer serializer = new XmlSerializer(typeof(UnitActionInfo));
            serializer.Serialize(memoryStream, this);

            // deserializer out.
            memoryStream.Seek(0, SeekOrigin.Begin);
            UnitActionInfo cloneObject = (UnitActionInfo)serializer.Deserialize(memoryStream);
            memoryStream.Close();

            // reset copy datas.
            return cloneObject;
        }

        public void BuildActionCache()
        {
            foreach (ActionGroup actionGroup in ActionGroups)
                actionGroup.BuildActionCache();
        }
    }
}
