using System;
using System.Collections.Generic;
using System.Text;
using OpenTK;

namespace Ecliptic.Core.Rendering
{
    public struct Color
    {
        public float red;
        public float green;
        public float blue;
        public float alpha;

        public Color(float r, float g, float b)
        {
            red = r;
            green = g;
            blue = b;
            alpha = 1.0f;
        }
        public Color(float r, float g, float b, float a)
        {
            red = r;
            green = g;
            blue = b;
            alpha = a;
        }

        public Vector4 ToVec4()
        {
            return new Vector4(red, green, blue, alpha);
        }


    }

}
