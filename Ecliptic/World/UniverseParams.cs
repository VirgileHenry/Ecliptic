using System;
using System.Collections.Generic;
using System.Text;

namespace Ecliptic.World
{
    public class UniverseParams
    {
        //store values for star and planet sizes, mass, etc...
        //everything is in meters
        public Int64 universalGridScale = 1000000000;
        public int starMinSize = 1000000;
        public int starMaxSize = 10000000;
        public int planetMinSize = 5000;
        public int planetMaxSize = 50000;
        public int planetMinDistance = 3000000;
        public int planetMaxDistance = 20000000;

        //random values for system generations
        public float geomParamPlanetNumber = 0.2f;

        public UniverseParams() { }
    }
}
