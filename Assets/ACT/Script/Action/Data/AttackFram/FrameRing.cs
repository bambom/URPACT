using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.ComponentModel;

namespace Data
{
    [Serializable]
    public class FrameRing : AttackFrame
    {
        private float mInnerRadius = 0;
        [XmlAttribute("InnerRadius"), DisplayName("内半径"), DefaultValue(0)]
        public float InnerRadius { get { return mInnerRadius; } set { mInnerRadius = value; } }
        
        private float mInnerRadiusFactor = 1.0f;
        [XmlAttribute("InnerRadiusFactor"), DisplayName("内半径缩放比例"), DefaultValue(1.0f)]
        public float InnerRadiusFactor { get { return mInnerRadiusFactor; } set { mInnerRadiusFactor = value; } }

        private float mOuterRadius = 0;
        [XmlAttribute("OuterRadius"), DisplayName("外半径"), DefaultValue(0)]
        public float OuterRadius { get { return mOuterRadius; } set { mOuterRadius = value; } }

        private float mOuterRadiusFactor = 1.0f;
        [XmlAttribute("OuterRadiusFactor"), DisplayName("外半径缩放比例"), DefaultValue(1.0f)]
        public float OuterRadiusFactor { get { return mOuterRadiusFactor; } set { mOuterRadiusFactor = value; } }

        private float mHeight = 0;
        [XmlAttribute("Height"), DisplayName("高度"), DefaultValue(0)]
        public float Height { get { return mHeight; } set { mHeight = value; } }

        private float mHeightFactor = 1.0f;
        [XmlAttribute("HeightFactor"), DisplayName("高度缩放比例"), DefaultValue(1.0f)]
        public float HeightFactor { get { return mHeightFactor; } set { mHeightFactor = value; } }

        public override HitDefnitionFramType Type()
        {
            return HitDefnitionFramType.RingType;
        }

        public override string ToString()
        {
            return "圆环体攻击框";
        }
    }
}