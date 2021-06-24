using System.IO;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System.Collections.Generic;
using OpenTK.Graphics.OpenGL4;

namespace Breakout
{
    public class ResourceManager
    {
        public static Dictionary<string, Shader> Shaders = new();
        public static Dictionary<string, Texture2D> Textures = new();

        public static string[] GetTexturePaths()
        {
            return Directory.GetFiles("Textures");
        }

        public static string[] GetLevelPaths()
        {
            return Directory.GetFiles("Levels/");
        }

        public static Shader LoadShader(string name, string[] vShaderFilePath, string[] fShaderFilePath)
        {
            Shaders.Add(
                name,
                LoadShaderFromFile(
                    Path.Combine(vShaderFilePath),
                    Path.Combine(fShaderFilePath)
                )
            );
            return Shaders[name];
        }

        public static Shader LoadShader(string name, string[] vShaderFilePath, string[] fShaderFilePath, string[] gShaderFilePath)
        {
            Shaders.Add(
                name,
                LoadShaderFromFile(
                    Path.Combine(vShaderFilePath),
                    Path.Combine(fShaderFilePath),
                    Path.Combine(gShaderFilePath)
                )
            );
            return Shaders[name];
        }

        public static Shader GetShader(string name)
        {
            return Shaders[name];
        }

        public static Texture2D LoadTexture(string filePath, string  name)
        {
            Textures.Add(
                name,
                LoadTextureFromFile(filePath)
            );
            return Textures[name];
        }

        public static Texture2D LoadTexture(string filePath)
        {
            string name = Path.GetFileNameWithoutExtension(filePath);
            Textures.Add(
                name,
                LoadTextureFromFile(filePath)
            );
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
                GL.DeleteProgram(entry.Value.Id);
            }
            foreach(KeyValuePair<string, Texture2D> entry in Textures)
            {
                GL.DeleteTexture(entry.Value.Id);
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

        private static Texture2D LoadTextureFromFile(string file)
        {
            ThrowIfFileDoesNotExist(file);
            Texture2D texture = new();
            Image<Rgba32> image = Image.Load<Rgba32>(file);
            image.Mutate(x => x.Flip(FlipMode.Vertical));
            List<byte> pixels = new(4 * image.Width * image.Height);

            for (int y = 0; y < image.Height; y++)
            {
                var row = image.GetPixelRowSpan(y);
                for (int x = 0; x < image.Width; x++)
                {
                    pixels.Add(row[x].B);
                    pixels.Add(row[x].G);
                    pixels.Add(row[x].R);
                    pixels.Add(row[x].A);
                }
            }

            return texture.Generate(image.Width, image.Height, pixels.ToArray());
        }

        public static void ThrowIfFileDoesNotExist(string filePath)
        {
            if (!File.Exists(filePath))
            {
                throw new System.Exception($"File {filePath} does not exist!");
            }
        }
    }
}
