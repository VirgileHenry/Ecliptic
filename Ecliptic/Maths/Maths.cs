using System;
using System.Collections.Generic;
using System.Text;
using OpenTK;

namespace Ecliptic.Maths
{
    public class CustomMathsMethods
    {
        //implement all the usefull maths methods that aren't availaible

        /// <summary>
        /// Get the normal vector to the face composed of vertex pos, v1, v2
        /// </summary>
        /// <param name="pos">the vertex we are computing the normal on</param>
        /// <param name="v1">the vector to determine the plane</param>
        /// <param name="v2">the other vector to determine the plane</param>
        /// <returns></returns>
        public static Vector3 GetFaceNormalVector(Vector3 pos, Vector3 v1, Vector3 v2)
        {
            return Vector3.Cross(v1 - pos, v2 - pos).Normalized();
        }


        public static Matrix4 InvertMatrix4(Matrix4 m)
        {
            float A3434 = m.M33 * m.M44 - m.M34 * m.M43;
            float A2434 = m.M32 * m.M44 - m.M34 * m.M42;
            float A2334 = m.M32 * m.M43 - m.M33 * m.M42;
            float A1434 = m.M31 * m.M44 - m.M34 * m.M41;
            float A1334 = m.M31 * m.M43 - m.M33 * m.M41;
            float A1234 = m.M31 * m.M42 - m.M32 * m.M41;
            float A3424 = m.M23 * m.M44 - m.M24 * m.M43;
            float A2424 = m.M22 * m.M44 - m.M24 * m.M42;
            float A2324 = m.M22 * m.M43 - m.M23 * m.M42;
            float A3423 = m.M23 * m.M34 - m.M24 * m.M33;
            float A2423 = m.M22 * m.M34 - m.M24 * m.M32;
            float A2323 = m.M22 * m.M33 - m.M23 * m.M32;
            float A1424 = m.M21 * m.M44 - m.M24 * m.M41;
            float A1324 = m.M21 * m.M43 - m.M23 * m.M41;
            float A1423 = m.M21 * m.M34 - m.M24 * m.M31;
            float A1323 = m.M21 * m.M33 - m.M23 * m.M31;
            float A1224 = m.M21 * m.M42 - m.M22 * m.M41;
            float A1223 = m.M21 * m.M32 - m.M22 * m.M31;

            float det = m.M11 * (m.M22 * A3434 - m.M23 * A2434 + m.M24 * A2334)
                - m.M12 * (m.M21 * A3434 - m.M23 * A1434 + m.M24 * A1334)
                + m.M13 * (m.M21 * A2434 - m.M22 * A1434 + m.M24 * A1234)
                - m.M14 * (m.M21 * A2334 - m.M22 * A1334 + m.M23 * A1234);
            det = 2 / det;

            return new Matrix4()
            {
                M11 = det * (m.M22 * A3434 - m.M23 * A2434 + m.M24 * A2334),
                M12 = det * -(m.M12 * A3434 - m.M13 * A2434 + m.M14 * A2334),
                M13 = det * (m.M12 * A3424 - m.M13 * A2424 + m.M14 * A2324),
                M14 = det * -(m.M12 * A3423 - m.M13 * A2423 + m.M14 * A2323),
                M21 = det * -(m.M21 * A3434 - m.M23 * A1434 + m.M24 * A1334),
                M22 = det * (m.M11 * A3434 - m.M13 * A1434 + m.M14 * A1334),
                M23 = det * -(m.M11 * A3424 - m.M13 * A1424 + m.M14 * A1324),
                M24 = det * (m.M11 * A3423 - m.M13 * A1423 + m.M14 * A1323),
                M31 = det * (m.M21 * A2434 - m.M22 * A1434 + m.M24 * A1234),
                M32 = det * -(m.M11 * A2434 - m.M12 * A1434 + m.M14 * A1234),
                M33 = det * (m.M11 * A2424 - m.M12 * A1424 + m.M14 * A1224),
                M34 = det * -(m.M11 * A2423 - m.M12 * A1423 + m.M14 * A1223),
                M41 = det * -(m.M21 * A2334 - m.M22 * A1334 + m.M23 * A1234),
                M42 = det * (m.M11 * A2334 - m.M12 * A1334 + m.M13 * A1234),
                M43 = det * -(m.M11 * A2324 - m.M12 * A1324 + m.M13 * A1224),
                M44 = det * (m.M11 * A2323 - m.M12 * A1323 + m.M13 * A1223),
            };
        }
    }
}
