using System;
using System.Collections.Generic;
using System.Text;
using Ecliptic.World;

namespace Ecliptic.Core.World
{
    /// <summary>
    /// Can load and unload entire game worlds, and have all the methods we call for 
    /// rendering, applying physics, etc
    /// </summary>
    public static class Scene
    {
        public static Universe universe;
        public static Camera camera;

        /// <summary>
        /// Default load universe methods generate a new world
        /// </summary>
        public static void LoadUniverse()
        {
            universe = new Universe();

            //init camera
            camera = new Camera(1.5f);
        }

        /// <summary>
        /// Call the update method on every GameObject in the scene
        /// </summary>
        /// <param name="deltaTime"></param>
        public static void Update(float deltaTime)
        {
            if(universe != null)
            {
                universe.Update(deltaTime);
            }
        }


    }
}
