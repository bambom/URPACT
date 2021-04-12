using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.ComponentModel;

namespace Data
{
    [Serializable]
    public class FrameSomato : AttackFrame
    {
        public override HitDefnitionFramType Type()
        {
            return HitDefnitionFramType.SomatoType;
        }

        public override string ToString()
        {
            return "受击体攻击框";
        }
    }
}