using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.ComponentModel;

namespace Data
{
    [Serializable]
    public class FrameCuboid: AttackFrame
    {
        private float mWidth = 0;
        [Category("General"), XmlAttribute("Width"), DisplayName("宽度X"), DefaultValue(0)]
        public float Width { get { return mWidth; } set { mWidth = value; } }

        private float mWidthFactor = 1.0f;
        [Category("General"), XmlAttribute("WidthFactor"), DisplayName("宽度X缩放比例"), DefaultValue(1.0f)]
        public float WidthFactor { get { return mWidthFactor; } set { mWidthFactor = value; } }

        private float mHeight = 0;
        [Category("General"), XmlAttribute("Height"), DisplayName("高度Y"), DefaultValue(0)]
        public float Height { get { return mHeight; } set { mHeight = value; } }

        private float mHeightFactor = 1.0f;
        [Category("General"), XmlAttribute("HeightFactor"), DisplayName("高度Y缩放比例"), DefaultValue(1.0f)]
        public float HeightFactor { get { return mHeightFactor; } set { mHeightFactor = value; } }


        private float mLength = 0;
        [Category("General"), XmlAttribute("Length"), DisplayName("长度Z"), DefaultValue(0)]
        public float Length { get { return mLength; } set { mLength = value; } }

        private float mLengthFactor = 1.0f;
        [Category("General"), XmlAttribute("LengthFactor"), DisplayName("长度Z缩放比例"), DefaultValue(1.0f)]
        public float LengthFactor { get { return mLengthFactor; } set { mLengthFactor = value; } }

     
        public override HitDefnitionFramType Type()
        {
            return HitDefnitionFramType.CuboidType;
        }

        public override string ToString()
        {
            return "长方体攻击框";
        }
    }
}