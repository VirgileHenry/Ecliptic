using System;
using System.Collections.Generic;
using System.Text;
using OpenTK;
using OpenTK.Graphics.OpenGL4;
using Ecliptic.Core.Rendering;
using Ecliptic.Core.World;
using Ecliptic.World;

namespace Ecliptic.Core
{
    class Game : GameWindow
    {
        public static Game instance;
        private string version = "0.1";
        private double lastFrameTime;


        public Game()
        {
            instance = this;

            //no menu for now, so load the scene
            Scene.LoadUniverse();

            lastFrameTime = System.DateTime.Now.TimeOfDay.TotalMilliseconds;
        }
        
        /// <summary>
        /// called when the game window is loaded. Set up the engine
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLoad(EventArgs e)
        {
            //call the base load method
            base.OnLoad(e);

            //change the title
            Title = "Ecliptic-" + version;

            Renderer.InitializeRenderer();
        }

        /// <summary>
        /// Called when a frame is being rendered : call the renderer to render next frame
        /// </summary>
        /// <param name="e">The arguments for the frame event.
        /// only valid for the current frame, do not store this</param>
        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);

            //compute delta time
            double deltaTime = (System.DateTime.Now.TimeOfDay.TotalMilliseconds - lastFrameTime) / 1000;
            lastFrameTime = System.DateTime.Now.TimeOfDay.TotalMilliseconds;

            //update and render the world
            Scene.Update((float)deltaTime);
            Renderer.Render();

            Title = $"Ecliptic-{version} : {(int)(1 / deltaTime)}fps : {(float)((int)(GameConstants.universalTime * 10)) / 10}sec";
        }

        /// <summary>
        /// Called when the window is resized by the user. We need to recompute projection matrix !
        /// </summary>
        /// <param name="e">resize event data</param>
        protected override void OnResize(EventArgs e)
        {

            base.OnResize(e);

            GL.Viewport(ClientRectangle.X, ClientRectangle.Y, ClientRectangle.Width, ClientRectangle.Height);

            //recompute the projection matrix
            Camera.mainCamera.ComputeProjectionMatrix();
        }

        /// <summary>
        /// Return the aspect ratio of the current game window (usefull for cam's perspective matrix)
        /// aspect ratio is width / height
        /// NOT YET IMPLEMENTED
        /// </summary>
        /// <returns>the current game window aspect ratio</returns>
        public float GetAspectRatio()
        {
            return 1;
        }
    }
}
