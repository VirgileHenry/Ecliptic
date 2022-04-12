using System;
using System.Collections.Generic;
using System.Text;
using OpenTK;
using System.Threading;
using Ecliptic.Core.Rendering;
using Ecliptic.Core.World;
using Ecliptic.World.MarchingCubes;
using Ecliptic.World.Generation;
using Ecliptic.Maths;

namespace Ecliptic.World
{    
    public class Planet : CelestialBody
    {
        //represent a planet
        //can vary in size, color, shape, etc

        private int seed;
        private int radius;

        //planet mesh
        private PlanetChunk root;
        private Thread planetGenerationThread;
        private bool updatingMesh;
        private int meshRefreshDelayMs;

        private GameShaders planetShader = GameShaders.defaultFlatShader;

        /// <summary>
        /// Build a procedural planet from the given seed. 
        /// As soon as it is modified by the player, we should start to store the planet's data.
        /// </summary>
        /// <param name="planetSeed">seed of the planet</param>
        /// <param name="systemPlace">place of the planet in the system, usefull for size, orbit raidus, etc</param>
        public Planet(CelestialBody parent, int planetSeed, int systemPlace) : base(parent)
        {
            seed = planetSeed;
            radius = 4;
            updatingMesh = true;
            meshRefreshDelayMs = 500;

            isStatic = false;
            orbit = new Orbit(parent, Vector3.UnitY * 30, 30, 1);

            root = new PlanetChunk(-Vector3.One * radius * 2, Vector3.One * radius * 2, 0, radius);
            root.LoadChildren(-Vector3.One * radius * 2, Vector3.One * radius * 2, radius * 4, radius);

            
            //thread that manage planet generation and mesh updating
            planetGenerationThread = new Thread(new ThreadStart(PlanetGeneration));
            planetGenerationThread.IsBackground = true;
            planetGenerationThread.Start();

            toRender = true;

            isStatic = true; // don't move
            toRender = true;
        }

        /// <summary>
        /// Generate a planet
        /// TODO : keep track of cam positon and dynamically manage lod of the mesh, loading/unloading it
        /// </summary>
        private void PlanetGeneration()
        {
            root.GenerateMeshRecursive();


            //refresh the mesh
            while (updatingMesh)
            {
                //update chunks depending of the position of the target
                root.ReloadLodRecursive(Camera.mainCamera.position, this);

                Thread.Sleep(meshRefreshDelayMs);
            }
        }


        public override void AskForRenderThisFrame()
        {
            Renderer.meshRenderQueue[planetShader].Add(this);
        }

        /// <summary>
        /// We will transfer this task to the draw method
        /// </summary>
        /// <param name="shader"></param>
        public override void SetMeshMaterialToShader(ShaderProgram shader) { }

        public override void Draw()
        {
            root.RecursiveMeshDraw();
            Console.WriteLine("drawing");
        }

        public override void Update(float deltaTime)
        {            
            rotation *= Quaternion.FromEulerAngles(0.02f * deltaTime, 0, 0);
            ComputeTransformMatrix();

            base.Update(deltaTime);
        }

    }



    /// <summary>
    /// planets are trees of planet chunks, so we can zoom in 
    /// each node have 8 child, and represent a cube of the planet, holding values
    /// the values represent the potential at each corner of the cube
    /// for the marching cubes algorithm
    /// each child is a eighth of the node cube
    /// each child can apply marching cubes on a grid of 16x16x16 inside itslef
    /// /// </summary>
    public class PlanetChunk
    {
        //store the 3D pos and the potential on the w component
        public bool isLeaf; //wether children is null or not
        public int depth;
        public PlanetChunk[] children;
        Vector3 minPos;
        public float cellSize;
        private int planetRadius;
        //to now where my own mesh data is in the big mesh (only usefull for leaves)
        private Mesh mesh;

        public static int leafGridSize = 8;
        public static int maxDepthLod = 10;
        public static float lodDistMultiplier = 2f;

        //some hack to offset values
        private static Dictionary<int, int> vertexOrder = new Dictionary<int, int>() { { 0, 0 }, { 1, 2 }, { 2, 1 } };
        
        public PlanetChunk(Vector3 minPos, Vector3 maxPos, int depth, int radius)
        {
            cellSize = (maxPos.X - minPos.X);
            this.minPos = minPos;
            this.planetRadius = radius;

            children = new PlanetChunk[0];
            isLeaf = true;
            this.depth = depth;
        }

        /// <summary>
        /// This should be called on leaf child : it generates all the 8 child of this cell
        /// </summary>
        /// <param name="minPos">smallest 3d position of the corner of the actual cell</param>
        /// <param name="maxPos">biggest 3d position of the corner of the actual cell</param>
        /// <param name="cellSize">Size of the actual cell</param>
        /// <param name="radius">Radius of the planet we are generating</param>
        public void LoadChildren(Vector3 minPos, Vector3 maxPos, float cellSize, int radius)
        {
            //check that we are end of line (so children arn't loaded yet
            if(isLeaf)
            {
                isLeaf = false;
                float childSize = cellSize / 2;
                Vector3 midPos = (minPos + maxPos) / 2;
                Vector3 offsetX = Vector3.UnitX * childSize;
                Vector3 offsetY = Vector3.UnitY * childSize;
                Vector3 offsetZ = Vector3.UnitZ * childSize;
                children = new PlanetChunk[8];
                children[0] = new PlanetChunk(minPos, midPos, depth + 1, radius);
                children[1] = new PlanetChunk(minPos + offsetX, midPos + offsetX, depth + 1, radius);
                children[2] = new PlanetChunk(midPos - offsetY, maxPos - offsetY, depth + 1, radius);
                children[3] = new PlanetChunk(minPos + offsetZ, midPos + offsetZ, depth + 1, radius);
                children[4] = new PlanetChunk(minPos + offsetY, midPos + offsetY, depth + 1, radius);
                children[5] = new PlanetChunk(midPos - offsetZ, maxPos - offsetZ, depth + 1, radius);
                children[6] = new PlanetChunk(midPos, maxPos, depth + 1, radius);
                children[7] = new PlanetChunk(midPos - offsetX, maxPos - offsetX, depth + 1, radius);
            }
        }

        /// <summary>
        /// Unloads all children recursively, and remove their mesh
        /// </summary>
        /// <param name="mesh">refference to the mesh so children can remove their mesh</param>
        /// <param name="root">reference to the root so children can re-offset meshes</param>
        public void UnloadChildrenRecursive()
        {
            if(isLeaf)
            {
                if(mesh != null)
                {
                    mesh.Unload();
                    mesh = null;
                }
            }
            else
            {
                foreach(PlanetChunk child in children)
                {
                    child.UnloadChildrenRecursive();
                }

                isLeaf = true;
                children = new PlanetChunk[0];
                if(mesh != null)
                {
                    mesh.Unload();
                    mesh = null;
                }
            }
        }


        /// <summary>
        /// Recursively generates the mesh of the cell by calling this method in each child
        /// and applying marching cubes on leaves
        /// </summary>
        /// <param name="mesh">The mesh we want to add the new mesh on</param>
        public void GenerateMeshRecursive()
        {            
            if(isLeaf)
            {
                //check that we have no mesh yet
                if(mesh == null)
                {
                    mesh = GenerateLeafMesh(0.5f);
                    Universe.mainThreadActions.Add(mesh.InitializeGLMesh);
                    Universe.mainThreadActions.Add(mesh.SetUpMesh);
                }
            }
            else
            {
                for(int i = 0; i < 8; i++)
                {
                    children[i].GenerateMeshRecursive();
                }
            }
        }

        /// <summary>
        /// Apply a marching cube algorithm of the leaf on a grid of size leafGridSize
        /// </summary>
        /// <param name="isoLevel">the level of the surface of the planet</param>
        /// <returns></returns>
        public Mesh GenerateLeafMesh(float isoLevel)
        {
            List<Vertex> vertices = new List<Vertex>();
            List<int> triangles = new List<int>();

            Vector3 offsetX = Vector3.UnitX * cellSize / leafGridSize;
            Vector3 offsetY = Vector3.UnitY * cellSize / leafGridSize;
            Vector3 offsetZ = Vector3.UnitZ * cellSize / leafGridSize;

            //store the potentials at the corner of each cube
            Vector4[] potentials = new Vector4[8];

            for(int x = 0; x < leafGridSize; x++)
            {
                for(int y = 0; y < leafGridSize; y++)
                {
                    for(int z = 0; z < leafGridSize; z++)
                    {
                        potentials[0] = new Vector4(minPos + x * offsetX + y * offsetY + z * offsetZ, PlanetGenerator.GetPlanetPotential(minPos + x * offsetX + y * offsetY + z * offsetZ, planetRadius));
                        potentials[1] = new Vector4(minPos + (x + 1) * offsetX + y * offsetY + z * offsetZ, PlanetGenerator.GetPlanetPotential(minPos + (x + 1) * offsetX + y * offsetY + z * offsetZ, planetRadius));
                        potentials[2] = new Vector4(minPos + (x + 1) * offsetX + y * offsetY + (z + 1) * offsetZ, PlanetGenerator.GetPlanetPotential(minPos + (x + 1) * offsetX + y * offsetY + (z + 1) * offsetZ, planetRadius));
                        potentials[3] = new Vector4(minPos + x * offsetX + y * offsetY + (z + 1) * offsetZ, PlanetGenerator.GetPlanetPotential(minPos + x * offsetX + y * offsetY + (z + 1) * offsetZ, planetRadius));
                        potentials[4] = new Vector4(minPos + x * offsetX + (y + 1) * offsetY + z * offsetZ, PlanetGenerator.GetPlanetPotential(minPos + x * offsetX + (y + 1) * offsetY + z * offsetZ, planetRadius));
                        potentials[5] = new Vector4(minPos + (x + 1) * offsetX + (y + 1) * offsetY + z * offsetZ, PlanetGenerator.GetPlanetPotential(minPos + (x + 1) * offsetX + (y + 1) * offsetY + z * offsetZ, planetRadius));
                        potentials[6] = new Vector4(minPos + (x + 1) * offsetX + (y + 1) * offsetY + (z + 1) * offsetZ, PlanetGenerator.GetPlanetPotential(minPos + (x + 1) * offsetX + (y + 1) * offsetY + (z + 1) * offsetZ, planetRadius));
                        potentials[7] = new Vector4(minPos + x * offsetX + (y + 1) * offsetY + (z + 1) * offsetZ, PlanetGenerator.GetPlanetPotential(minPos + x * offsetX + (y + 1) * offsetY + (z + 1) * offsetZ, planetRadius));

                        //apply one marching cubes algorithm to potentials
                        // Calculate unique index for each cube configuration.
                        // There are 256 possible values
                        // A value of 0 means cube is entirely inside surface; 255 entirely outside.
                        // The value is used to look up the edge table, which indicates which edges of the cube are cut by the isosurface.
                        int cubeIndex = 0;
                        if (potentials[0].W < isoLevel) cubeIndex |= 1;
                        if (potentials[1].W < isoLevel) cubeIndex |= 2;
                        if (potentials[2].W < isoLevel) cubeIndex |= 4;
                        if (potentials[3].W < isoLevel) cubeIndex |= 8;
                        if (potentials[4].W < isoLevel) cubeIndex |= 16;
                        if (potentials[5].W < isoLevel) cubeIndex |= 32;
                        if (potentials[6].W < isoLevel) cubeIndex |= 64;
                        if (potentials[7].W < isoLevel) cubeIndex |= 128;

                        int triangleOffset = vertices.Count;

                        // Create triangles for current cube configuration
                        for (int i = 0; MCData.triangulation[cubeIndex][i] != -1; i += 3)
                        {
                            // Get indices of corner points A and B for each of the three edges
                            // of the cube that need to be joined to form the triangle.
                            int a0 = MCData.cornerIndexAFromEdge[MCData.triangulation[cubeIndex][i]];
                            int b0 = MCData.cornerIndexBFromEdge[MCData.triangulation[cubeIndex][i]];

                            int a1 = MCData.cornerIndexAFromEdge[MCData.triangulation[cubeIndex][i + 1]];
                            int b1 = MCData.cornerIndexBFromEdge[MCData.triangulation[cubeIndex][i + 1]];

                            int a2 = MCData.cornerIndexAFromEdge[MCData.triangulation[cubeIndex][i + 2]];
                            int b2 = MCData.cornerIndexBFromEdge[MCData.triangulation[cubeIndex][i + 2]];

                            Vector3 pos1 = InterpolateVerts(potentials[a0], potentials[b0], isoLevel);
                            Vector3 pos2 = InterpolateVerts(potentials[a1], potentials[b1], isoLevel);
                            Vector3 pos3 = InterpolateVerts(potentials[a2], potentials[b2], isoLevel);

                            Vertex[] tri = new Vertex[3]
                            {
                                new Vertex(pos1, CustomMathsMethods.GetFaceNormalVector(pos1, pos3, pos2), new Vector2(0, 0)),
                                new Vertex(pos3, CustomMathsMethods.GetFaceNormalVector(pos3, pos2, pos1), new Vector2(0, 0)),
                                new Vertex(pos2, CustomMathsMethods.GetFaceNormalVector(pos2, pos1, pos3), new Vector2(0, 0))
                            };
                            
                            for(int j = 0; j < 3; j++)
                            {
                                //look for matching vertices in the mesh
                                int index = -1;
                                for(int k = 0; k < vertices.Count; k++)
                                {
                                    if(Vector3.DistanceSquared(vertices[k].position, tri[j].position) < 0.000001f * cellSize * cellSize)
                                    {
                                        index = k;
                                        break;
                                    }
                                }

                                if(index != -1)
                                {
                                    //there is alredy the vertex in the mesh
                                    vertices[index].Merge(tri[j]);
                                    triangles.Add(index);
                                }
                                else
                                {
                                    vertices.Add(tri[j]);
                                    triangles.Add(vertices.Count - 1);
                                }
                            }
                        }
                    }
                }
            }

            return new Mesh(vertices.ToArray(), triangles.ToArray(), false);
        }

        /// <summary>
        /// interpolate between the two given positions with parmater 'isoLevel'
        /// </summary>
        /// <param name="v1">position 1, with 4th component being the potential</param>
        /// <param name="v2">position 2, with 4th component being the potenital</param>
        /// <param name="isoLevel"></param>
        /// <returns></returns>
        private Vector3 InterpolateVerts(Vector4 v1, Vector4 v2, float isoLevel)
        {
            float t = (isoLevel - v1.W) / (v2.W - v1.W);
            return new Vector3(v1) + t * (new Vector3(v2) - new Vector3(v1));
        }


        /// <summary>
        /// Check for changes in the lod of the meshes depending of the position of the target,
        /// Then eventually reload the mesh to follow the changes 
        /// </summary>
        /// <param name="mesh">reference to the planet mesh to change it</param>
        /// <param name="targetPos">the position we are folowing for the lod</param>
        /// <param name="root">reference to the root of the planet (for mesh removing)</param>
        public void ReloadLodRecursive(Vector3 targetPos, Planet planet)
        {

            //compute distance from center to target
            float distanceSquarred = Vector3.DistanceSquared(planet.GetWorldPosition(minPos + Vector3.One * cellSize / 2), targetPos);

            if (isLeaf && depth < maxDepthLod)
            {
                //check for increasing lod if target is close
                if(distanceSquarred < cellSize * lodDistMultiplier * cellSize * lodDistMultiplier)
                {
                    //load childrens, load children meshes
                    LoadChildren(minPos, minPos + Vector3.One * cellSize, cellSize, planetRadius);
                    GenerateMeshRecursive();

                    //when child mesh is loaded, delete our own
                    if(mesh != null)
                    {
                        mesh.Unload();
                        mesh = null;
                    }
                }
            }

            if(!isLeaf)
            {
                if(distanceSquarred > cellSize * lodDistMultiplier * cellSize * lodDistMultiplier)
                {
                    //player too far : unload childrens
                    
                    //generate mesh
                    if (mesh == null)
                    {
                        mesh = GenerateLeafMesh(0.5f);
                        Universe.mainThreadActions.Add(mesh.InitializeGLMesh);
                        Universe.mainThreadActions.Add(mesh.SetUpMesh);
                        Console.WriteLine("here");
                    }

                    //remove childrens, become leaf, regenerate mesh
                    UnloadChildrenRecursive();
                }
                else
                {
                    //call the the reload on childrens
                    foreach(PlanetChunk child in children)
                    {
                        child.ReloadLodRecursive(targetPos, planet);
                    }
                }
            }

        }


        /// <summary>
        /// Custom draw for our planet : draw every mesh in the octree
        /// </summary>
        public void RecursiveMeshDraw()
        {
            if(isLeaf && mesh != null)
            {
                mesh.Draw();
            }
            else
            {
                foreach(PlanetChunk child in children)
                {
                    if(child != null) child.RecursiveMeshDraw();
                }
            }
        }

    }

}
