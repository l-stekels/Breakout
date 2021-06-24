using System;
using OpenTK.Graphics.OpenGL4;

namespace Breakout
{
    public class PostProcessor
    {
        private Shader _postProcessingShader;
        private Texture2D _texture;
        private int _width;
        private int _height;
        private bool _confuse = false;
        private bool _chaos = false;
        public bool Shake = false;
        private int _msfbo;
        private int _fbo;
        private int _rbo;
        private int _vao;

        public PostProcessor(Shader shader, int width, int height)
        {
            _texture = new Texture2D();
            (_postProcessingShader, _width, _height) = (shader, width, height);
            GL.GenFramebuffers(1, out _msfbo);
            GL.GenFramebuffers(1, out _fbo);
            GL.GenRenderbuffers(1, out _rbo);

            GL.BindFramebuffer(FramebufferTarget.Framebuffer, _msfbo);
            GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, _rbo);
            GL.RenderbufferStorageMultisample(RenderbufferTarget.Renderbuffer, 8, RenderbufferStorage.Rgba8, _width, _height);
            GL.FramebufferRenderbuffer(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0, RenderbufferTarget.Renderbuffer, _rbo);
            if (GL.CheckFramebufferStatus(FramebufferTarget.Framebuffer) != FramebufferErrorCode.FramebufferComplete)
            {
                Console.WriteLine($"ERROR::POSTPROCESSOR: Failed to initialize MSFBO");
            }

            GL.BindFramebuffer(FramebufferTarget.Framebuffer, _fbo);
            _texture.Generate(_width, _height, null);
            GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0, TextureTarget.Texture2D, _texture.Id, 0);
            if (GL.CheckFramebufferStatus(FramebufferTarget.Framebuffer) != FramebufferErrorCode.FramebufferComplete)
            {
                Console.WriteLine($"ERROR::POSTPROCESSOR: Failed to initialize FBO");
            }
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
            InitRenderData();
            _postProcessingShader.SetInteger("scene", 0);
           

            var edgeKernel = new int[9]
            {
                -1, -1, -1,
                -1,  8, -1,
                -1, -1, -1
            };
            GL.Uniform2(
                GL.GetUniformLocation(_postProcessingShader.Id, "edge_kernel"),
                9,
                edgeKernel
            );

            var blurKernel = new float[9]
            {
                1.0f / 16.0f, 2.0f / 16.0f, 1.0f / 16.0f,
                2.0f / 16.0f, 4.0f / 16.0f, 2.0f / 16.0f,
                1.0f / 16.0f, 2.0f / 16.0f, 1.0f / 16.0f
            };
            GL.Uniform2(
                GL.GetUniformLocation(_postProcessingShader.Id, "blur_kernel"),
                9,
                blurKernel
            );
        }

        public void BeginRender()
        {
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, _msfbo);
            GL.ClearColor(0.0f, 0.0f, 0.0f, 1.0f);
            GL.Clear(ClearBufferMask.ColorBufferBit);
        }

        public void EndRender()
        {
            GL.BindFramebuffer(FramebufferTarget.ReadFramebuffer, _msfbo);
            GL.BindFramebuffer(FramebufferTarget.DrawFramebuffer, _fbo);
            GL.BlitFramebuffer(0, 0, _width, _height, 0, 0, _width, _height, ClearBufferMask.ColorBufferBit, BlitFramebufferFilter.Nearest);
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
        }

        public void Render(float time)
        {
            _postProcessingShader.SetFloat("time", time)
                .SetInteger("confuse", Convert.ToInt32(_confuse))
                .SetInteger("chaos", Convert.ToInt32(_chaos))
                .SetInteger("shake", Convert.ToInt32(Shake));

            GL.ActiveTexture(TextureUnit.Texture0);
            _texture.Bind();
            GL.BindVertexArray(_vao);
            GL.DrawArrays(PrimitiveType.Triangles, 0, 6);
            GL.BindVertexArray(0);
        }

        private void InitRenderData()
        {
            float[] vertices =
             {
                -1.0f, -1.0f, 0.0f, 0.0f,
                 1.0f,  1.0f, 1.0f, 1.0f,
                -1.0f,  1.0f, 0.0f, 1.0f,

                -1.0f, -1.0f, 0.0f, 0.0f,
                 1.0f, -1.0f, 1.0f, 0.0f,
                 1.0f,  1.0f, 1.0f, 1.0f
            };

            GL.GenVertexArrays(1, out _vao);
            GL.GenBuffers(1, out int vbo);

            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
            GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StaticDraw);

            GL.BindVertexArray(_vao);
            GL.EnableVertexAttribArray(0);
            GL.VertexAttribPointer(0, 4, VertexAttribPointerType.Float, false, 4 * sizeof(float), 0);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindVertexArray(0);
        }
    }
}
