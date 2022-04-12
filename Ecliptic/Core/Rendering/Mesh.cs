using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using OpenTK;
using OpenTK.Graphics.OpenGL4;
using Ecliptic.Maths;

namespace Ecliptic.Core.Rendering
{
    public class Mesh
    {
        public Vertex[] vertices;
        public int[] triangles;
        protected int VAO;
        protected int VBO;
        protected int EBO;
        public GameShaders shader; //shader used to render the mesh (ref to the renderer shaders)
        public Material material;
        public bool useFirstVertexConvention;

        /// <summary>
        /// Instanciate a new mesh from the given data
        /// </summary>
        /// <param name="vertices">vertices of the new mesh</param>
        /// <param name="triangles">triangles of the new mesh</param>
        public Mesh(Vertex[] vertices, int[] triangles, bool setUp = true,  GameShaders shader = GameShaders.defaultFlatShader, bool firstVertexConvention = false)
        {
            this.vertices = vertices;
            this.triangles = triangles;

            this.shader = shader;
            material = Material.defaultMat;
            useFirstVertexConvention = firstVertexConvention;

            if (setUp)
            {
                InitializeGLMesh();
                SetUpMesh();
            }
        }


        public Mesh()
        {
            vertices = new Vertex[0];
            triangles = new int[0];

            shader = GameShaders.defaultFlatShader;
            material = Material.defaultMat;
            useFirstVertexConvention = false;
        }

        /// <summary>
        /// Create a primitive cube
        /// </summary>
        public static Mesh CreateCube(float size = 1f)
        {
            //create a cube
            Vertex[] vertices = new Vertex[8]
            {
                new Vertex(new Vector3(-size, -size, -size), new Vector3(0, -1, 0), new Vector2(0, 0)),
                new Vertex(new Vector3(-size, -size, size), new Vector3(0, 0, 1), new Vector2(0, 0)),
                new Vertex(new Vector3(size, -size, -size), new Vector3(0, 0, 0), new Vector2(0, 0)),
                new Vertex(new Vector3(size, -size, size), new Vector3(1, 0, 0), new Vector2(0, 0)),
                new Vertex(new Vector3(size, size, -size), new Vector3(0, 1, 0), new Vector2(0, 0)),
                new Vertex(new Vector3(size, size, size), new Vector3(0, 0, 0), new Vector2(0, 0)),
                new Vertex(new Vector3(-size, size, -size), new Vector3(0, 0, -1), new Vector2(0, 0)),
                new Vertex(new Vector3(-size, size, size), new Vector3(-1, 0, 0), new Vector2(0, 0)),
            };

            int[] triangles = new int[36] {
                0, 2, 3,   0, 3, 1, // front
                4, 6, 7,   4, 7, 5, // back
                3, 2, 4,   3, 4, 5, // right
                7, 6, 0,   7, 0, 1, // left
                6, 4, 2,   6, 2, 0, // bottom 
                1, 3, 5,   1, 5, 7  // top
            };

            return new Mesh(vertices, triangles, true, GameShaders.defaultFlatShader, true);
        }

        /// <summary>
        /// create a quad sphere centered at 0, 0, 0 and of radius radius
        /// </summary>
        /// <param name="radius">the radius of the sphere</param>
        /// <returns></returns>
        public static Mesh CreateSphere(float radius = 1f, int precision = 10)
        {
            //TO IMPLEMENT
            return CreateCube(radius);
        }

        public override string ToString()
        {
            string result = "Mesh :\nVertices : ";
            foreach (Vertex vert in vertices)
            {
                result += $"{vert.position}, ";
            }
            result += $" (Length : {vertices.Length})\nTriangles : ";
            foreach(uint tri in triangles)
            {
                result += $"{tri}, ";
            }
            result += $" (Length : {triangles.Length})";
            return result;
        }

        /// <summary>
        /// IMPORTANT : this should be called before any atempt at SetUp or Draw and from the main thread.
        /// </summary>
        public void InitializeGLMesh()
        {
            //initialize the openGL buffers
            VAO = GL.GenVertexArray();
            VBO = GL.GenBuffer();
            EBO = GL.GenBuffer();

            GL.BindVertexArray(VAO);
        }

        /// <summary>
        /// generate all the GL data for the mesh to be rendered.
        /// Should be called at the creation of the mesh
        /// </summary>
        public virtual void SetUpMesh()
        {

            GL.BindBuffer(BufferTarget.ArrayBuffer, VBO);
            GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * Vertex.byteSize, vertices, BufferUsageHint.StaticDraw);

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, EBO);
            GL.BufferData(BufferTarget.ElementArrayBuffer, triangles.Length * sizeof(int), triangles, BufferUsageHint.StaticDraw);

            //vertex positions
            GL.EnableVertexAttribArray(0);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, Vertex.byteSize, 0);

            //vertex normals
            GL.EnableVertexAttribArray(1);
            GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, Vertex.byteSize, 3 * 4);

            //vertex texture coords
            GL.EnableVertexAttribArray(2);
            GL.VertexAttribPointer(2, 2, VertexAttribPointerType.Float, false, Vertex.byteSize, 6 * 4);

            //unbind vertex array (what for ?)
            GL.BindVertexArray(0);
        }

        /// <summary>
        /// Send the material data to the given shader throught the material struct
        /// </summary>
        /// <param name="shader">the shader we need to send the data throught</param>
        public void SetMeshMaterialToShader(ShaderProgram shader)
        {
            //set ambiant property
            int matAmbLoc = GL.GetUniformLocation(shader.id, "material.ambiant");
            GL.Uniform3(matAmbLoc, material.ambiant);

            //set diffuse property
            int matDiffLoc = GL.GetUniformLocation(shader.id, "material.diffuse");
            GL.Uniform3(matDiffLoc, material.diffuse);

            //set specular property
            int matSpecLoc = GL.GetUniformLocation(shader.id, "material.specular");
            GL.Uniform3(matSpecLoc, material.specular);

            //set shininess property
            int matShinLoc = GL.GetUniformLocation(shader.id, "material.shininess");
            GL.Uniform1(matShinLoc, material.shininess);

            //set shininess property
            int matEmisLoc = GL.GetUniformLocation(shader.id, "material.emissive");
            GL.Uniform1(matEmisLoc, material.emissive ? 1 : 0);
        }

        /// <summary>
        /// Draw the mesh to the screen.
        /// </summary>
        public virtual void Draw()
        {
            //this should bind vertex array, bind textures and draw the mesh
            //render the mesh with the given shader
            GL.BindVertexArray(VAO);
            if(useFirstVertexConvention) { GL.ProvokingVertex(ProvokingVertexMode.FirstVertexConvention); }
            GL.DrawElements(BeginMode.Triangles, triangles.Length, DrawElementsType.UnsignedInt, 0);
            GL.BindVertexArray(0);
        }

        /// <summary>
        /// Delete (and hopefully free memory) the vertex and triangles arrays
        /// </summary>
        public void Unload()
        {
            vertices = new Vertex[0];
            triangles = new int[0];
        }

    }
}
