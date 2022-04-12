using System;
using System.Collections.Generic;
using System.Text;

namespace Ecliptic.Maths
{
    public static class CustomRandom
    {
        public static int Geometric(float p)
        {
            Random rnd = new Random();
            return (int)MathF.Ceiling(MathF.Log(1 - (float)rnd.NextDouble()) / MathF.Log(1 - p));
        }
    }
}
