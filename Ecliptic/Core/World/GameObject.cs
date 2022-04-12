using System;
using System.Collections.Generic;
using System.Text;
using OpenTK;
using Ecliptic.Core.Rendering;
using Ecliptic.World;
using OpenTK.Graphics.OpenGL4;

namespace Ecliptic.Core.World
{
    public class GameObject
    {
        //position, rotation and scale
        public Matrix4 worldTransform; //relative to parent
        public Vector3 position; //relative to parent
        public Quaternion rotation;

        public bool toRender; //if we need to render the go
        public Mesh mesh; //null if toRender is false;

        public GameObject()
        {
            //create a debug GameObject
            position = Vector3.Zero;
            rotation = Quaternion.Identity;
            ComputeTransformMatrix();

            toRender = false;
        }


        /// <summary>
        /// Recalculate the worldTransform matrix from position and rotation
        /// </summary>
        public void ComputeTransformMatrix()
        {
            worldTransform = Matrix4.CreateFromQuaternion(rotation) * Matrix4.CreateTranslation(position);
        }

        /// <summary>
        /// Called each frame.
        /// Empty on the base structure, but can be updated on child class that might need it
        /// </summary>
        /// <param name="deltaTime">time since the last Update Call</param>
        public virtual void Update(float deltaTime)
        {
            if (toRender)
            {
                AskForRenderThisFrame();
            }
        }

        /// <summary>
        /// Get the position in world space of the objectPos in current object space
        /// </summary>
        /// <param name="objectPos">the point relative to the object</param>
        /// <returns>the position of objectPos in world space</returns>
        public Vector3 GetWorldPosition(Vector3 objectPos)
        {
            Vector4 worldPos = new Vector4(objectPos, 1f) * worldTransform;
            return new Vector3(worldPos);
        }

        /// <summary>
        /// this puts our gameObject in the list of objects to render this frame.
        /// TODO : checks if object is visible to optimize rendering time
        /// </summary>
        public virtual void AskForRenderThisFrame()
        {
            //set itslef to the mesh render queue
            //we assume the render queue is properly initialized for each shader
            Renderer.meshRenderQueue[mesh.shader].Add(this);
        }

        /// <summary>
        /// Send the model worldpos matrix the the surrent shader program under the "modelMat" name
        /// </summary>
        /// <param name="shaderProg">the shader program we want to send the info to</param>
        public void SetModelMatrixToShader(ShaderProgram shaderProg)
        {
            //set the view cam matrix
            int modelMatLoc = GL.GetUniformLocation(shaderProg.id, "modelMat");
            GL.UniformMatrix4(modelMatLoc, false, ref worldTransform);
        }

        /// <summary>
        /// Call the setMaterialToShader on the associated mesh.
        /// Virtual so it can be overloaded for more complex objects (planets)
        /// </summary>
        /// <param name="shader">the shader program we want to set the material to</param>
        public virtual void SetMeshMaterialToShader(ShaderProgram shader)
        {
            mesh.SetMeshMaterialToShader(shader);
        }

        /// <summary>
        /// Call the Draw on the associated mesh.
        /// Virtual so it can be overloaded for more complex objects (planets)
        /// </summary>
        public virtual void Draw()
        {
            mesh.Draw();
        }

    }
}
