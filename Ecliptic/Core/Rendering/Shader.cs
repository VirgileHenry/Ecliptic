using System;
using OpenTK.Graphics.OpenGL4;
using System.IO;
using System.Collections.Generic;

namespace Ecliptic.Core.Rendering
{
    public struct Shader
    {
        public int id;

        /// <summary>
        /// Load and compile a shader from given file
        /// </summary>
        /// <param name="shaderName">path to the shader's location</param>
        /// <param name="type">type of the shader</param>
        public Shader(string shaderName, ShaderType type)
        {
            //create a new shader
            id = GL.CreateShader(type);
            GL.ShaderSource(id, File.ReadAllText(shaderName));
            GL.CompileShader(id);

            //check the log
            string shaderInfoLog = GL.GetShaderInfoLog(id);
            //check for compile errors
            if (!string.IsNullOrEmpty(shaderInfoLog))
            {
                throw new Exception(shaderInfoLog);
            }
        }
    }

    public struct ShaderProgram
    {
        public int id;

        /// <summary>
        /// Create a shader program to render the scene
        /// </summary>
        /// <param name="shadersLocation">path to every shaders</param>
        public ShaderProgram(Dictionary<ShaderType, string> shadersLocation)
        {
            id = GL.CreateProgram();

            //shader parameters

            //load shaders
            Dictionary<ShaderType, Shader> shaders = new Dictionary<ShaderType, Shader>();

            foreach (KeyValuePair<ShaderType, string> location in shadersLocation)
            {
                shaders.Add(location.Key, new Shader(location.Value, location.Key));
                GL.AttachShader(id, shaders[location.Key].id);
            }

            //load the program
            GL.LinkProgram(id);

            //once the program is linked, no need to keep the shaders in memory
            foreach (KeyValuePair<ShaderType, Shader> shader in shaders)
            {
                GL.DetachShader(id, shader.Value.id);
                GL.DeleteShader(shader.Value.id);
            }

            //check the log
            string shaderInfoLog = GL.GetProgramInfoLog(id);
            //check for compile errors
            if (!string.IsNullOrEmpty(shaderInfoLog))
            {
                throw new Exception(shaderInfoLog);
            }
        }

        /// <summary>
        /// Use the shader program.
        /// </summary>
        public void Use()
        {
            GL.UseProgram(id);
            //set the gl params
        }

    }
}
