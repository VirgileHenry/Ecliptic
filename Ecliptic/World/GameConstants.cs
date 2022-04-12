using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Ecliptic.World
{
    /// <summary>
    /// Keeps all game constants, like gravity, physic frame rate, etc
    /// </summary>
    class GameConstants
    {

        public static float G = 6.67430e-11f;
        public static int PHYSIC_FRAME_RATE = 20;
        public static float PHYSIC_DELTA_TIME = 1 / PHYSIC_FRAME_RATE;
        public static float universalTime;
    }
}
