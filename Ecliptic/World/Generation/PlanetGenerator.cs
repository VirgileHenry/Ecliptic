using System;
using System.Collections.Generic;
using OpenTK;

namespace Ecliptic.World.Generation
{
    public static class PlanetGenerator
    {
        /// <summary>
        /// Test method, that return simple values of a planet potential for the octree
        /// </summary>
        /// <param name="position">position we want to evaluate the potential</param>
        /// <returns>the potential at the given position</returns>
        public static float GetPlanetPotential(Vector3 position, int planetRadius)
        {

            return planetRadius - position.Length;

        }

        /// <summary>
        /// BETA : 3d noise generation, to play with
        /// </summary>
        /// <param name="position">were we want to evaluate the noise </param>
        /// <returns></returns>
        private static float Noise3(Vector3 position)
        {
            Vector3 p = new Vector3(MathF.Floor(position.X), MathF.Floor(position.Y), MathF.Floor(position.Z));
            Vector3 f = new Vector3(position.X % 1, position.Y % 1, position.Z % 1);

            float n = p.X + p.Y * 55.0f + p.Z * 101.0f;

            return Mix(
                Mix(
                    Mix(hash(n), hash(n + 1.0f), f.X),
                    Mix(hash(n + 55.0f), hash(n + 56.0f), f.X),
                    f.Y),
                Mix(
                    Mix(hash(n + 101.0f), hash(n + 102.0f), f.X),
                    Mix(hash(n + 156.0f), hash(n + 157.0f), f.X),
                    f.Y),
                f.Z);
        }

        /// <summary>
        /// Smoothly interpolate between x and y with parameter t
        /// </summary>
        /// <param name="x">value 1</param>
        /// <param name="y">value 2</param>
        /// <param name="t">parameter</param>
        /// <returns></returns>
        private static float Mix(float x, float y, float t)
        {
            return x * (1 - t) + y * t;
        }

        /// <summary>
        /// BETA hash method for floats
        /// </summary>
        /// <param name="n"></param>
        /// <returns></returns>
        private static float hash(float n)
        {
            return (MathF.Sin(n) * 43728.1453f) % 1;
        }

    }
}
