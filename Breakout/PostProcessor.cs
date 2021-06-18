using System;
using OpenTK.Graphics.OpenGL4;

namespace Breakout
{
    public class PostProcessor
    {
        public Shader PostProcessingShader;
        public Texture2D Texture;
        public int Width;
        public int Height;
        public bool Confuse = false;
        public bool Chaos = false;
        public bool Shake = false;
        private int MSFBO;
        private int FBO;
        private int RBO;
        private int VAO;

        public PostProcessor(Shader shader, int width, int height)
        {
            Texture = new Texture2D();
            (PostProcessingShader, Width, Height) = (shader, width, height);
            GL.GenFramebuffers(1, out MSFBO);
            GL.GenFramebuffers(1, out FBO);
            GL.GenRenderbuffers(1, out RBO);

            GL.BindFramebuffer(FramebufferTarget.Framebuffer, MSFBO);
            GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, RBO);
            GL.RenderbufferStorageMultisample(RenderbufferTarget.Renderbuffer, 8, RenderbufferStorage.Rgba8, Width, Height);
            GL.FramebufferRenderbuffer(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0, RenderbufferTarget.Renderbuffer, RBO);
            if (GL.CheckFramebufferStatus(FramebufferTarget.Framebuffer) != FramebufferErrorCode.FramebufferComplete)
            {
                Console.WriteLine($"ERROR::POSTPROCESSOR: Failed to initialize MSFBO");
            }

            GL.BindFramebuffer(FramebufferTarget.Framebuffer, FBO);
            Texture.Generate(Width, Height, null);
            GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0, TextureTarget.Texture2D, Texture.ID, 0);
            if (GL.CheckFramebufferStatus(FramebufferTarget.Framebuffer) != FramebufferErrorCode.FramebufferComplete)
            {
                Console.WriteLine($"ERROR::POSTPROCESSOR: Failed to initialize FBO");
            }
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
            InitRenderData();
            PostProcessingShader.SetInteger("scene", 0);
           

            int[] edgeKernel = new int[9]
            {
                -1, -1, -1,
                -1,  8, -1,
                -1, -1, -1
            };
            GL.Uniform2(
                GL.GetUniformLocation(PostProcessingShader.ID, "edge_kernel"),
                9,
                edgeKernel
            );

            float[] blurKernel = new float[9]
            {
                1.0f / 16.0f, 2.0f / 16.0f, 1.0f / 16.0f,
                2.0f / 16.0f, 4.0f / 16.0f, 2.0f / 16.0f,
                1.0f / 16.0f, 2.0f / 16.0f, 1.0f / 16.0f
            };
            GL.Uniform2(
                GL.GetUniformLocation(PostProcessingShader.ID, "blur_kernel"),
                9,
                blurKernel
            );
        }

        public void BeginRender()
        {
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, MSFBO);
            GL.ClearColor(0.0f, 0.0f, 0.0f, 1.0f);
            GL.Clear(ClearBufferMask.ColorBufferBit);
        }

        public void EndRender()
        {
            GL.BindFramebuffer(FramebufferTarget.ReadFramebuffer, MSFBO);
            GL.BindFramebuffer(FramebufferTarget.DrawFramebuffer, FBO);
            GL.BlitFramebuffer(0, 0, Width, Height, 0, 0, Width, Height, ClearBufferMask.ColorBufferBit, BlitFramebufferFilter.Nearest);
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
        }

        public void Render(float time)
        {
            PostProcessingShader.SetFloat("time", time)
                .SetInteger("confuse", Convert.ToInt32(Confuse))
                .SetInteger("chaos", Convert.ToInt32(Chaos))
                .SetInteger("shake", Convert.ToInt32(Shake));

            GL.ActiveTexture(TextureUnit.Texture0);
            Texture.Bind();
            GL.BindVertexArray(VAO);
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

            GL.GenVertexArrays(1, out VAO);
            GL.GenBuffers(1, out int VBO);

            GL.BindBuffer(BufferTarget.ArrayBuffer, VBO);
            GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StaticDraw);

            GL.BindVertexArray(VAO);
            GL.EnableVertexAttribArray(0);
            GL.VertexAttribPointer(0, 4, VertexAttribPointerType.Float, false, 4 * sizeof(float), 0);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindVertexArray(0);
        }
    }
}
