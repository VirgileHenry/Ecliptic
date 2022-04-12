using System;
using System.Collections.Generic;
using System.Text;
using Ecliptic.Core.Rendering;
using OpenTK;

namespace Ecliptic.World
{
    class CosmicCube : CelestialBody
    {
        public CosmicCube(CelestialBody parent) : base(parent)
        {
            toRender = true;
            mesh = Mesh.CreateCube(1f);
            position = new Vector3(0f, 0f, 0f);
            ComputeTransformMatrix();
        }

        public override void Update(float deltaTime)
        {
            rotation *= Quaternion.FromAxisAngle(new Vector3(0, 1, 0), deltaTime);
            ComputeTransformMatrix();
            
            base.Update(deltaTime);
        }
    }
}
