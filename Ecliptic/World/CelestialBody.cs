using System;
using System.Collections.Generic;
using System.Text;
using OpenTK;
using Ecliptic.Maths;
using Ecliptic.Core.World;
using Ecliptic.Core.Rendering;

namespace Ecliptic.World
{
    public enum CelestialBodyRenderMode
    {
        SelfOnly,
        ParentOnly,
        ChildrenOnly,
        ParentAndChildren,
    }
    
    /// <summary>
    /// Base class for celestial bodies : contains basic rendering and physics update
    /// </summary>
    public class CelestialBody : GameObject
    {
        public bool isStatic = true; //if the body can move
        public Orbit orbit;
        public CelestialBody parent;
        public CelestialBody[] children; //the bodies orbitting round us

        public CelestialBody(CelestialBody parent)
        {
            //initialize data
            this.parent = parent;
            children = new CelestialBody[0];
        }

        public void Load()
        {
            //actually load the celestial body data
        }

        public void Unload()
        {
            //save and unload the celestial body,
            //but still a little reference to it until parent is unloaded
        }

        /// <summary>
        /// Update itself and call the update method on every children
        /// </summary>
        /// <param name="deltaTime">time since last update frame</param>
        public override void Update(float deltaTime)
        {            
            if(!isStatic)
            {
                //move along the orbit
                position = orbit.GetPositionAt(GameConstants.universalTime);
            }
            
            foreach(CelestialBody child in children)
            {
                child.Update(deltaTime);
            }

            base.Update(deltaTime);
        }


    }
}
