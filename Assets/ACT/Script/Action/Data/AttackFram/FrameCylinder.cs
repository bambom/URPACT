using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.ComponentModel;

namespace Data
{
    [Serializable]
    public class FrameCylinder : AttackFrame
    {
        private float mRadius = 0;
        [XmlAttribute("Radius"), DisplayName("半径"), DefaultValue(0)]
        public float Radius { get { return mRadius; } set { mRadius = value; } }

        private float mRadiusFactor = 1.0f;
        [XmlAttribute("RadiusFactor"), DisplayName("半径缩放比例"), DefaultValue(1.0f)]
        public float RadiusFactor 
        { 
            get { return mRadiusFactor; } 
            set { mRadiusFactor = value; } 
        }

        private float mHeight = 0;
        [XmlAttribute("Height"), DisplayName("高度"), DefaultValue(0)]
        public float Height { get { return mHeight; } set { mHeight = value; } }

        private float mHeightFactor = 1.0f;
        [XmlAttribute("HeightFactor"), DisplayName("高度缩放比例"), DefaultValue(1.0f)]
        public float HeightFactor { get { return mHeightFactor; } set { mHeightFactor = value; } }

        public override HitDefnitionFramType Type()
        {
            return HitDefnitionFramType.CylinderType;
        }

        public override string ToString()
        {
            return "圆柱体攻击框";
        }
    }
}