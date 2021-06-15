using OpenTK.Graphics.OpenGL4;
using System.Drawing.Imaging;
using PixelFormat = OpenTK.Graphics.OpenGL4.PixelFormat;

namespace Breakout
{
    public class Texture2D
    {
        public int ID;
        public int Width = 0;
        public int Height = 0;
        public PixelInternalFormat InternalFormat;
        public PixelFormat ImageFormat;
        public TextureWrapMode WrapS;
        public TextureWrapMode WrapT;

        public Texture2D()
        {
            InternalFormat = PixelInternalFormat.Rgb;
            ImageFormat = PixelFormat.Bgr;
            WrapS = TextureWrapMode.Repeat;
            WrapT = TextureWrapMode.Repeat;
            ID = GL.GenTexture();
        }

        public void Generate(int width, int height, BitmapData data)
        {
            Width = width;
            Height = height;

            GL.BindTexture(TextureTarget.Texture2D, ID);
            GL.TexImage2D(TextureTarget.Texture2D, 0, InternalFormat, Width, Height, 0, ImageFormat, PixelType.UnsignedByte, data.Scan0);
            GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)WrapS);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)WrapT);

            GL.BindTexture(TextureTarget.Texture2D, 0);
        }

        public void Bind()
        {
            GL.BindTexture(TextureTarget.Texture2D, ID);
        }
    }
}
