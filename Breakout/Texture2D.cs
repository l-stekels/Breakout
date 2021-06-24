using OpenTK.Graphics.OpenGL4;
using System.Collections.Generic;
using System.Drawing.Imaging;
using PixelFormat = OpenTK.Graphics.OpenGL4.PixelFormat;

namespace Breakout
{
    public class Texture2D
    {
        public int Id;
        public int Width = 0;
        public int Height = 0;

        public Texture2D()
        {
            Id = GL.GenTexture();
        }

        public Texture2D Generate(int width, int height, byte[] pixels = null)
        {
            Width = width;
            Height = height;

            GL.BindTexture(TextureTarget.Texture2D, Id);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.LinearMipmapLinear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

            GL.TexImage2D(
                TextureTarget.Texture2D,
                0,
                PixelInternalFormat.Rgba,
                Width,
                Height,
                0,
                PixelFormat.Bgra,
                PixelType.UnsignedByte,
                pixels
            );
            GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);

            GL.BindTexture(TextureTarget.Texture2D, 0);
            return this;
        }

        public void Bind()
        {
            GL.BindTexture(TextureTarget.Texture2D, Id);
        }
    }
}
