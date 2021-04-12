using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace Data
{
    [Serializable]
    [TypeConverter(typeof(Vector3Converter))]
    public class Vector3
    {
        float x;
        float y;
        float z;

        [DefaultValue(0.0f)]
        public float X { get { return x; } set { x = value; } }

        [DefaultValue(0.0f)]
        public float Y { get { return y; } set { y = value; } }

        [DefaultValue(0.0f)]
        public float Z { get { return z; } set { z = value; } }

        public Vector3(float mX, float mY, float mZ)
        {
            x = mX;
            y = mY;
            z = mZ;
        }

        public Vector3()
        {
            x = y = z = 0;
        }

        public override string  ToString()
        {  
            return  X + " " + Y + " " + Z;  
        }

        public static Vector3 Zero
        {
            get { return new Vector3(0,0,0);}
        }
    }
}
