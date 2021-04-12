using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.ComponentModel;

namespace Data
{
    [Serializable]
    public class FrameFan : AttackFrame
    {
        private float mRadius = 0;
        [XmlAttribute("Radius"), DisplayName("半径"), DefaultValue(0)]
        public float Radius { get { return mRadius; } set { mRadius = value; } }


        private float mRadiusFactor = 1.0f;
        [XmlAttribute("RadiusFactor"), DisplayName("半径缩放比例"), DefaultValue(1.0f)]
        public float RadiusFactor { get { return mRadiusFactor; } set { mRadiusFactor = value; } }

        private float mHeight = 0;
        [XmlAttribute("Height"), DisplayName("高度"), DefaultValue(0)]
        public float Height { get { return mHeight; } set { mHeight = value; } }

        private float mHeightFactor = 1.0f;
        [XmlAttribute("HeightFactor"), DisplayName("高度缩放比例"), DefaultValue(1.0f)]
        public float HeightFactor { get { return mHeightFactor; } set { mHeightFactor = value; } }

        private float mStartAngle = 0;
        [XmlAttribute("StartAngle"), DisplayName("起始角度"), DefaultValue(0)]
        public float StartAngle { get { return mStartAngle; } set { mStartAngle = value; } }

        private float mStartAngleFactor = 1.0f;
        [XmlAttribute("StartAngleFactor"), DisplayName("起始角度缩放比例"), DefaultValue(1.0f)]
        public float StartAngleFactor { get { return mStartAngleFactor; } set { mStartAngleFactor = value; } }

        private float mEndAngle = 0.0f;
        [XmlAttribute("EndAngle"), DisplayName("终止角度"), DefaultValue(0.0f)]
        public float EndAngleFactor { get { return mEndAngle; } set { mEndAngle = value; } }

        private float mEndAngleFactor = 1.0f;
        [XmlAttribute("EndAngleFactor"), DisplayName("终止角度缩放比例"), DefaultValue(1.0f)]
        public float EndAngle { get { return mEndAngleFactor; } set { mEndAngleFactor = value; } }

        public override HitDefnitionFramType Type()
        {
            return HitDefnitionFramType.FanType;
        }

        public override string ToString()
        {
            return "扇形体攻击框";
        }
    }
}