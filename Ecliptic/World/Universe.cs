using System;
using System.Collections.Generic;
using System.Text;
using OpenTK;
using Ecliptic.Core.Rendering;
using Ecliptic.Core.World;
using Ecliptic.Maths;

namespace Ecliptic.World
{
    /// <summary>
    /// Universe is infinite and procedurally generated. It's main components is a star grid
    /// Stars are static objects, randomly positioned in it's grid cell.
    /// </summary>
    public class Universe : CelestialBody
    {
        private SceneLight light;
        //all actions to execute on the main thread
        public static List<Action> mainThreadActions;

        //store all the stars that are on a grid
        public Dictionary<Vector2Int, Star> starGrid;

        //Universe parameters
        private UniverseParams universeParams = new UniverseParams();

        public Universe() : base(null)
        {
            mainThreadActions = new List<Action>();
            
            toRender = false;

            position = new Vector3(0, 0, 0);
            rotation = Quaternion.FromEulerAngles(0, 0, 0);

            ComputeTransformMatrix();

            light = new SceneLight(new Vector3(10, 20, 20), new Color(1.0f, 0.8f, 0.6f), new Color(0.15f, 0.1f, 0.05f));
            //starGrid = new Dictionary<Vector2Int, Star>() { { new Vector2Int(0, 0), new Star(this, "00".GetHashCode(), universeParams, true) } };

            //init to 0 for now, load it from the world data then
            GameConstants.universalTime = 0;

            children = new CelestialBody[1] { new CosmicCube(this) };
        }

        /// <summary>
        /// set the main light data to the given shader program
        /// </summary>
        /// <param name="shader">the shader program we want to set the data to</param>
        public void SetMainLightToShader(ShaderProgram shader)
        {
            light.SetLightDataToShader(shader);
        }

        /// <summary>
        /// Exemple to show use of update
        /// </summary>
        /// <param name="deltaTime"></param>
        public override void Update(float deltaTime)
        {
            while(mainThreadActions.Count > 0)
            {
                if( mainThreadActions[0] != null) mainThreadActions[0].Invoke();
                mainThreadActions.RemoveAt(0);
            }

            //update universal time:
            GameConstants.universalTime += deltaTime;

            //stars are not children, so manually update them
            /*
            foreach(KeyValuePair<Vector2Int, Star> star in starGrid)
            {
                star.Value.Update(deltaTime);
            }
            */

            base.Update(deltaTime);
        }
    }
}
