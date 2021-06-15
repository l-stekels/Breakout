using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Collections.Generic;
using OpenTK.Graphics.OpenGL4;
using PixelFormat = OpenTK.Graphics.OpenGL4.PixelFormat;

namespace Breakout
{
    public class ResourceManager
    {
        public static Dictionary<string, Shader> Shaders = new();
        public static Dictionary<string, Texture2D> Textures = new();

        public static Shader LoadShader(string name, string vShaderFile, string fShaderFile, string gShaderFile = null)
        {
            Shaders[name] = LoadShaderFromFile(vShaderFile, fShaderFile, gShaderFile);
            return Shaders[name];
        }

        public static Shader GetShader(string name)
        {
            return Shaders[name];
        }

        public static Texture2D LoadTexture(string file, bool alpha, string  name)
        {
            Textures[name] = LoadTextureFromFile(file, alpha);
            return Textures[name];
        }

        public static Texture2D GetTexture(string name)
        {
            return Textures[name];
        }

        public static void Clear()
        {
            foreach(KeyValuePair<string, Shader> entry in Shaders)
            {
                GL.DeleteProgram(entry.Value.ID);
            }
            foreach(KeyValuePair<string, Texture2D> entry in Textures)
            {
                GL.DeleteTexture(entry.Value.ID);
            }
        }

        private ResourceManager() { }

        private static Shader LoadShaderFromFile(string vShaderFile, string fShaderFile, string gShaderFile = null)
        {
            ThrowIfFileDoesNotExist(vShaderFile);
            ThrowIfFileDoesNotExist(fShaderFile);
            return new Shader(
                File.ReadAllText(vShaderFile),
                File.ReadAllText(fShaderFile),
                string.IsNullOrEmpty(gShaderFile) ? null : File.ReadAllText(gShaderFile)
            );
        }

        private static Texture2D LoadTextureFromFile(string file, bool alpha)
        {
            ThrowIfFileDoesNotExist(file);
            Texture2D texture = new();
            var readFormat = System.Drawing.Imaging.PixelFormat.Format32bppRgb;
            if (alpha)
            {
                texture.InternalFormat = PixelInternalFormat.Rgba;
                texture.ImageFormat = PixelFormat.Bgra;
                readFormat = System.Drawing.Imaging.PixelFormat.Format32bppArgb;
            }
            using (Bitmap image = new(file))
            {
                BitmapData data = image.LockBits(
                    new Rectangle(0, 0, image.Width, image.Height),
                    ImageLockMode.ReadOnly,
                    readFormat
                );
                texture.Generate(image.Width, image.Height, data);
                return texture;
            }
        }

        private static void ThrowIfFileDoesNotExist(string filePath)
        {
            if (!System.IO.File.Exists(filePath))
            {
                throw new System.Exception($"File {filePath} does not exist!");
            }
        }
    }
}
