using System;
using System.Collections.Generic;
using System.Text;
using OpenTK.Graphics.OpenGL4;

namespace Ecliptic.Core.Rendering
{
    public enum TextureType
    {
        diffuse,
        specular,
    }

    struct Texture
    {
        int id;
        TextureType type;
        TextureWrapMode verticalTextureWrapMode;
        TextureWrapMode horizontalTextureWrapMode;

        /// <summary>
        /// Load a texture from the given path.
        /// </summary>
        /// <param name="path">path to the texture we want to load</param>
        public Texture(string path)
        {
            id = GL.GenTexture();
            type = TextureType.diffuse;
            verticalTextureWrapMode = TextureWrapMode.Repeat;
            horizontalTextureWrapMode = TextureWrapMode.Repeat;
        }
    }
}
