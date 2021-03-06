using OpenTK.Mathematics;
using OpenTK.Graphics.OpenGL4;

namespace Breakout
{
    public class SpriteRenderer
    {
        private readonly Shader _shader;

        private int _quadVao;

        public SpriteRenderer(Shader shader)
        {
            _shader = shader;
            InitRenderData();
        }

        ~SpriteRenderer()
        {
            GL.DeleteVertexArray(_quadVao);
        }

        public void DrawSprite(Texture2D texture, Vector2 position)
        {
            Vector2 size = new(800, 200);
            float rotate = 0;
            Vector3 color = new(1);
            DrawSprite(texture, position, size, rotate, color);
        }

        public void DrawSprite(Texture2D texture, Vector2 position, Vector2 size)
        {
            float rotate = 0;
            Vector3 color = new(1);
            DrawSprite(texture, position, size, rotate, color);
        }

        public void DrawSprite(Texture2D texture, Vector2 position, Vector2 size, float rotate)
        {
            Vector3 color = new(1);
            DrawSprite(texture, position, size, rotate, color);
        }

        public void DrawSprite(Texture2D texture, Vector2 position, Vector2 size, float rotate, Vector3 color)
        {
            _shader.Use();
            Matrix4 model = Matrix4.Identity;

            Matrix4.CreateScale(size.X, size.Y, 1.0f, out Matrix4 scale);
            Matrix4.CreateTranslation(position.X, position.Y, 0.0f, out Matrix4 translate);
            Matrix4.CreateTranslation(0.5f * size.X, 0.5f * size.Y, 0.0f, out Matrix4 scaleUp);
            Matrix4.CreateRotationZ(MathHelper.DegreesToRadians(rotate), out Matrix4 rotation);
            Matrix4.CreateTranslation(-0.5f * size.X, -0.5f * size.Y, 0.0f, out Matrix4 scaleDown);
            // Movement
            model = translate * model;
            // Scale to change rotation center
            model = scaleUp * model;
            // Rotation
            model = rotation * model;
            // Scale down to original size
            model = scaleDown * model;
            // Scale to given size
            model = scale * model;

            _shader.SetMatrix4("model", model);
            _shader.SetVector3("spriteColor", color);

            GL.ActiveTexture(TextureUnit.Texture0);
            texture.Bind();

            GL.BindVertexArray(_quadVao);
            GL.DrawArrays(PrimitiveType.Triangles, 0, 6);
            GL.BindVertexArray(0);
        }

        private void InitRenderData()
        {
            float[] vertices =
            {
                // Vertex
                // position
                // coordinates  // Texture coordinates
                0.0f, 1.0f,     0.0f, 1.0f,
                1.0f, 0.0f,     1.0f, 0.0f,
                0.0f, 0.0f,     0.0f, 0.0f,

                0.0f, 1.0f,     0.0f, 1.0f,
                1.0f, 1.0f,     1.0f, 1.0f,
                1.0f, 0.0f,     1.0f, 0.0f
            };

            _quadVao = GL.GenVertexArray();
            int vertexBufferObject = GL.GenBuffer();

            GL.BindBuffer(BufferTarget.ArrayBuffer, vertexBufferObject);
            GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StaticDraw);

            GL.BindVertexArray(_quadVao);
            GL.EnableVertexAttribArray(0);
            GL.VertexAttribPointer(0, 4, VertexAttribPointerType.Float, true, 4 * sizeof(float), 0);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindVertexArray(0);
        }
    }
}
