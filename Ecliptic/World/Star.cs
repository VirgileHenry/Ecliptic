using System;
using System.Collections.Generic;
using System.Text;
using Ecliptic.Core.Rendering;
using Ecliptic.Maths;

namespace Ecliptic.World
{    
    /// <summary>
    /// Represent a star in the world : different way of rendering then planets
    /// </summary>
    public class Star : CelestialBody
    {
        private int seed;
        private int radius;

        public Star(CelestialBody parent, int starSeed, UniverseParams uParams, bool loadSystem = false) : base(parent)
        {
            seed = starSeed;

            isStatic = true; //stars don't move
            //set star vars
            Random rnd = new Random(seed);
            radius = rnd.Next(uParams.starMinSize, uParams.starMaxSize);
            radius = 10; //simple for now

            //create mesh
            toRender = true;
            mesh = Mesh.CreateCube(radius);
            mesh.material = new Material(new Color(1, 1, 0.7f), true);
            

            //load the planet
            if (loadSystem) { LoadSystem(uParams); }
        }

        public void LoadSystem(UniverseParams uParams)
        {
            //load or generate the planets of this star
            int planetNumber = 1; // CustomRandom.Geometric(uParams.geomParamPlanetNumber);
            children = new CelestialBody[planetNumber];
            for(int i = 0; i < planetNumber; i++)
            {
                children[i] = new Planet(this, (seed + $"{i}").GetHashCode(), i);
            }
        }
    }
}
