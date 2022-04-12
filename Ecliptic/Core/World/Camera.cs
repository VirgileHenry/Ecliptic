using System;
using System.Collections.Generic;
using System.Text;
using OpenTK;
using Ecliptic.Core.Rendering;
using Ecliptic.Maths;
using OpenTK.Graphics.OpenGL4;

namespace Ecliptic.Core.World
{
    public class Camera : GameObject
    {
        public Matrix4 projectionMatrix;
        public static Camera mainCamera;

        public float fovy;
        public float zNear;
        public float zFar;

        /// <summary>
        /// Create a new Camera projection matrix and initalize world position at center
        /// </summary>
        /// <param name="fovy">angle of the width of the camera</param>
        /// <param name="aspectRatio">width / height</param>
        /// <param name="zNear">distance to the near plane</param>
        /// <param name="zFar">distance to the far plane</param>
        public Camera(float fovy)
        {
            //check there can't be more than one camera
            if(mainCamera != null)
            {
                throw new Exception("There can not have more than one active camera");
            }
            
            this.fovy = fovy;
            zNear = 0.01f;
            zFar = int.MaxValue;
            
            //set ourslef as main camera
            mainCamera = this;
            //we don't render the camera
            toRender = false;
            mesh = null;
            //initialize pos and rot
            position = new Vector3(0, 2, 3);
            rotation = Quaternion.FromEulerAngles(-0.5f, 0, 0);

            ComputeTransformMatrix();
            ComputeProjectionMatrix();
        }

        /// <summary>
        /// Re-compute the new camera projection matrix based on the camera's parameters.
        /// The aspect ratio is asked to the game window class
        /// </summary>
        public void ComputeProjectionMatrix()
        {
            float aspectRatio = Game.instance.GetAspectRatio();
            projectionMatrix = Matrix4.CreatePerspectiveFieldOfView(fovy, aspectRatio, zNear, zFar);
        }

        /// <summary>
        /// Set the camera's projection matrix and view matrix in the openGL shader program
        /// </summary>
        /// <param name="shaderProg">the aimed shader program</param>
        public void SetProjectionAndViewMatrixInShader(ShaderProgram shaderProg)
        {
            //set the projection matrix
            int projMatLoc = GL.GetUniformLocation(shaderProg.id, "projectionMat");
            GL.UniformMatrix4(projMatLoc, false, ref projectionMatrix);

            //set the view cam matrix
            int viewMatLoc = GL.GetUniformLocation(shaderProg.id, "viewMat");
            Matrix4 inverseWorldMat = Matrix4.Invert(worldTransform);
            GL.UniformMatrix4(viewMatLoc, false, ref inverseWorldMat);

            //set the camera position
            int camPosLoc = GL.GetUniformLocation(shaderProg.id, "cameraPosition");
            GL.Uniform3(camPosLoc, ref position);
        }
    }
}
