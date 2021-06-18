using System;
using System.Collections.Generic;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace Breakout
{
    public class ParticleGenerator
    {
        private List<Particle> Particles = new();
        private uint Amount;
        private Shader Shader;
        private Texture2D Texture;
        private int VAO;
        private int LastUsedParticle = 0;

        public ParticleGenerator(Shader shader, Texture2D texture, uint amount)
        {
            (Shader, Texture, Amount) = (shader, texture, amount);
            Init();
        } 

        public void Update(float dt, GameObject gameObject, uint newParticles)
        {
            Update(dt, gameObject, newParticles, new Vector2(0.0f, 0.0f));
        }

        public void Update(float dt, GameObject gameObject, uint newParticles, Vector2 offset)
        {
            for (uint i = 0; i < newParticles; ++i)
            {
                int unusedParticle = FirstUnusedParticle();
                RespawnParticle(Particles[unusedParticle], gameObject, offset);
            }
            for (int i = 0; i < Amount; ++i)
            {
                Particles[i].Life -= dt;
                if (Particles[i].Life <= 0.0f)
                {
                    continue;
                }
                Particles[i].Position -= Particles[i].Velocity * dt;
                Particles[i].Color.W -= dt * 2.5f;
            }
        }

        public void Draw()
        {
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.One);
            Shader.Use();
            foreach (Particle particle in Particles)
            {
                if (particle.Life <= 0.0f)
                {
                    continue;
                }
                Shader.SetVector2f("offset", particle.Position);
                Shader.SetVector4("color", particle.Color);
                Texture.Bind();
                GL.BindVertexArray(VAO);
                GL.DrawArrays(PrimitiveType.Triangles, 0, 6);
                GL.BindVertexArray(0);
            }
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
        }

        private void Init()
        {
            float[] particleQuad =
            {
                0.0f, 1.0f, 0.0f, 1.0f,
                1.0f, 0.0f, 1.0f, 0.0f,
                0.0f, 0.0f, 0.0f, 0.0f,

                0.0f, 1.0f, 0.0f, 1.0f,
                1.0f, 1.0f, 1.0f, 1.0f,
                1.0f, 0.0f, 1.0f, 0.0f
            };
            GL.GenVertexArrays(1, out VAO);
            GL.GenBuffers(1, out uint VBO);
            GL.BindVertexArray(VAO);

            GL.BindBuffer(BufferTarget.ArrayBuffer, VBO);
            GL.BufferData(BufferTarget.ArrayBuffer, particleQuad.Length * sizeof(float), particleQuad, BufferUsageHint.StaticDraw);

            GL.EnableVertexAttribArray(0);
            GL.VertexAttribPointer(0, 4, VertexAttribPointerType.Float, false, 4 * sizeof(float), 0);
            GL.BindVertexArray(0);
            for (uint i = 0; i < Amount; ++i)
            {
                Particles.Add(new Particle());
            }
        }

        private int FirstUnusedParticle()
        {
            for (int i = LastUsedParticle; i < Amount; ++i)
            {
                if (Particles[i].Life <= 0.0f)
                {
                    LastUsedParticle = i;
                    return i;
                }
            }
            for (int i = 0; i < LastUsedParticle; ++i)
            {
                if (Particles[i].Life <= 0.0f)
                {
                    LastUsedParticle = i;
                    return i;
                }
            }
            LastUsedParticle = 0;
            return 0;

        }

        private void RespawnParticle(Particle particle, GameObject gameObject)
        {
            RespawnParticle(particle, gameObject, new Vector2(0.0f, 0.0f));
        }

        private void RespawnParticle(Particle particle, GameObject gameObject, Vector2 offset)
        {
            Random rnd = new();
            float random = (rnd.Next(100) - 50) / 10.0f;
            float randomColor = 0.5f + (rnd.Next(100) / 100.0f);
            particle.Position = new Vector2(
                gameObject.Position.X + random + offset.X,
                gameObject.Position.Y + random + offset.Y
            );
            particle.Color = new Vector4(randomColor, randomColor, randomColor, 1.0f);
            particle.Life = 1.0f;
            particle.Velocity = gameObject.Velocity * 0.1f;
        }
    }
}
