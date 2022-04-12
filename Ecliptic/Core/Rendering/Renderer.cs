using System;
using System.Collections.Generic;
using System.Text;
using OpenTK.Graphics.OpenGL4;
using System.IO;
using Ecliptic.Core.World;
using Ecliptic.World;

namespace Ecliptic.Core.Rendering
{
    public enum GameShaders
    {
        //store all the different shaders we use in the game
        //shader nums are the rendering priority
        defaultFlatShader,
        defaultSmoothShader,
        atmosphereShader,
        Length //should always be last elem
    }

    public static class Renderer
    {
        //this clas actually renders everything to the game window
        public static Dictionary<GameShaders, ShaderProgram> shaders;
        //all the meshes to render for each specific shader program
        public static Dictionary<GameShaders, List<GameObject>> meshRenderQueue;

        /// <summary>
        /// Initialize the renderer : create and compile the shaders.
        /// This must be only called at the beginning of the application
        /// </summary>
        public static void InitializeRenderer()
        {
            shaders = new Dictionary<GameShaders, ShaderProgram>();
            meshRenderQueue = new Dictionary<GameShaders, List<GameObject>>();

            for (int i = 0; i < (int)GameShaders.Length; i++)
            {
                meshRenderQueue.Add((GameShaders)i, new List<GameObject>());
            }

            //load the shaders
            //default shader
            shaders.Add(GameShaders.defaultFlatShader, new ShaderProgram(new Dictionary<ShaderType, string>()
            {
                { ShaderType.VertexShader, "D:/dev/.Synchronized/c#/Ecliptic/Ecliptic/Shaders/vertex_shader_flat.glsl" },
                { ShaderType.FragmentShader, "D:/dev/.Synchronized/c#/Ecliptic/Ecliptic/Shaders/fragment_shader_flat.glsl" }
            }));
            shaders.Add(GameShaders.defaultSmoothShader, new ShaderProgram(new Dictionary<ShaderType, string>()
            {
                { ShaderType.VertexShader, "D:/dev/.Synchronized/c#/Ecliptic/Ecliptic/Shaders/vertex_shader_smooth.glsl" },
                { ShaderType.FragmentShader, "D:/dev/.Synchronized/c#/Ecliptic/Ecliptic/Shaders/fragment_shader_smooth.glsl" }
            }));
            shaders.Add(GameShaders.atmosphereShader, new ShaderProgram(new Dictionary<ShaderType, string>()
            {
                { ShaderType.VertexShader, "D:/dev/.Synchronized/c#/Ecliptic/Ecliptic/Shaders/atmosphere_vertex.glsl" },
                { ShaderType.FragmentShader, "D:/dev/.Synchronized/c#/Ecliptic/Ecliptic/Shaders/atmosphere_fragment.glsl" }
            }));

            //set open gl main params
            GL.Enable(EnableCap.CullFace);
            GL.Enable(EnableCap.DepthTest);
        }

        /// <summary>
        /// Render every objects of the scene to be rendered to the window.
        /// This should be called each frame, after the update call
        /// </summary>
        public static void Render()
        {
            //from : https://stackoverflow.com/questions/39923583/use-different-shader-programs-in-opengl
            //we should have :
            //set shader program
            //set uniforms
            //bind vertex array
            //bind textures
            //change any other state (glEnable, etc)
            //draw with glDrawElement, glDrawArray
            //we will do this for every shader program.

            //clear buffers
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            //then render everything
            for (int i = 0; i < (int)GameShaders.Length; i++)
            {
                //use shader program
                GL.UseProgram(shaders[(GameShaders)i].id);

                //set uniforms
                //camera uniforms
                Camera.mainCamera.SetProjectionAndViewMatrixInShader(shaders[(GameShaders)i]);
                //main light uniform
                Scene.universe.SetMainLightToShader(shaders[(GameShaders)i]);

                //bind vertex arrays, bind textures and draw all in the mesh draw method
                foreach(GameObject go in meshRenderQueue[(GameShaders)i])
                {
                    go.SetModelMatrixToShader(shaders[(GameShaders)i]);
                    go.SetMeshMaterialToShader(shaders[(GameShaders)i]);
                    go.Draw();
                }

                //clear the mesh array for next frame
                meshRenderQueue[(GameShaders)i].Clear();
            }

            Game.instance.SwapBuffers();
        }




    }
}
