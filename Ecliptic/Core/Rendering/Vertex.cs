using System;
using System.Collections.Generic;
using System.Text;
using OpenTK;

namespace Ecliptic.Core.Rendering
{
    public struct Vertex
    {
        public Vector3 position;
        public Vector3 normal;
        public Vector2 textureCoords;
        public int mergedNum;

        //size in byte of the struct : pos   norm    text  merged
        public static int byteSize = 3 * 4 + 3 * 4 + 2 * 4 + 4;

        /// <summary>
        /// create a new vertex from the given data
        /// </summary>
        /// <param name="pos">vertex position</param>
        /// <param name="norm">vertex normal</param>
        /// <param name="textCoords">vertex texture coordonates</param>
        public Vertex(Vector3 pos, Vector3 norm, Vector2 textCoords)
        {
            position = pos;
            normal = Vector3.Normalize(norm);
            textureCoords = textCoords;
            mergedNum = 1;
        }

        public void Merge(Vertex other)
        {
            float t = mergedNum / (mergedNum + other.mergedNum);
            position = position * t + other.position * (1 - t);
            normal = normal * t + other.normal * (1 - t);
            mergedNum += 1;
        }
    }
}
