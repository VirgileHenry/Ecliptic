using System;
using System.Collections.Generic;
using System.Text;
using OpenTK;

namespace Ecliptic.Core.Rendering
{
    public struct Material
    {
        public Vector3 ambiant;
        public Vector3 diffuse;
        public Vector3 specular;
        public float shininess;
        public bool emissive;

        public Material(Color color, bool emissive)
        {
            ambiant = new Vector3(color.red, color.green, color.blue);
            diffuse = new Vector3(color.red, color.green, color.blue);
            specular = new Vector3(color.red, color.green, color.blue);
            shininess = 0;
            this.emissive = emissive;
        }

        public static Material defaultMat = new Material()
        {
            ambiant = new Vector3(0.8f, 0.2f, 0.7f),
            diffuse = new Vector3(0.8f, 0.2f, 0.7f),
            specular = new Vector3(0.4f, 0.1f, 0.35f),
            shininess = 256,
            emissive = false,
        };
    }
}
