using System;
using System.Collections.Generic;
using System.Text;
using OpenTK;
using OpenTK.Graphics.OpenGL4;
using Ecliptic.Core.Rendering;

namespace Ecliptic.Core.World
{
    /// <summary>
    /// Represent a light object. Inherit from GameObject so contains a world pos, etc
    /// TODO : implement source light rendering
    /// </summary>
    public class LightSource : GameObject
    {
        private Color color;
        private float maxRange;
        private float intensity;
    }

    public struct Light
    {
        //the light struct we can pass to the shader
        Vector3 position;
        Vector3 color;
        Vector3 ambiant;
    }


    /// <summary>
    /// Contains all the lights in the scene, and methods to send the lights data to the GPU.
    /// The main light is the closest star
    /// </summary>
    class SceneLight
    {
        //unused for now : TODO : implement lots of light sources
        private LightSource[] lights;

        //main light:
        private Vector3 mainPosition;
        private Color mainColor;
        private Color ambiantColor;

        /// <summary>
        /// basic constructor
        /// </summary>
        /// <param name="position">the position of the main light</param>
        /// <param name="color">the color of the main ligh</param>
        /// <param name="ambiant">the color of the ambiant ligh</param>
        public SceneLight(Vector3 position, Color color, Color ambiant)
        {
            mainPosition = position;
            mainColor = color;
            ambiantColor = ambiant;
        }

        /// <summary>
        /// Allows to change the main light without rebuilding it
        /// </summary>
        /// <param name="position">the position of the main light</param>
        /// <param name="color">the color of the main light</param>
        /// /// <param name="ambiant">the color of the ambiant ligh</param>
        public void SetMainLightSource(Vector3 position, Color color, Color ambiant)
        {
            mainPosition = Vector3.Normalize(position);
            mainColor = color;
            ambiantColor = ambiant;
        }


        public void SetLightDataToShader(ShaderProgram shader)
        {
            //set the direction
            int mainPosLoc = GL.GetUniformLocation(shader.id, "light.position");
            GL.Uniform3(mainPosLoc, ref mainPosition);

            //set the color
            int mainColLoc = GL.GetUniformLocation(shader.id, "light.color");
            Vector4 color = mainColor.ToVec4();
            GL.Uniform4(mainColLoc, ref color);

            //set the ambiant color
            int ambColLoc = GL.GetUniformLocation(shader.id, "light.ambiant");
            Vector4 ambiant = ambiantColor.ToVec4();
            GL.Uniform4(ambColLoc, ref ambiant);
        }

    }
}
