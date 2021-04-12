using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.ComponentModel;

namespace Data
{
    [Serializable]
    [XmlInclude(typeof(FrameCuboid))]
    [XmlInclude(typeof(FrameCylinder))]
    [XmlInclude(typeof(FrameRing))]
    [XmlInclude(typeof(FrameSomato))]
    [XmlInclude(typeof(FrameFan))]
    [TypeConverterAttribute(typeof(ExpandableObjectConverter))]
    public abstract class AttackFrame
    {
        public abstract HitDefnitionFramType Type();
    }
}