using System;
using System.Collections.Generic;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace Breakout
{
    public class ParticleGenerator
    {
        private List<Particle> _particles = new();
        private uint _amount;
        private Shader _shader;
        private Texture2D _texture;
        private int _vao;
        private int _lastUsedParticle = 0;

        public ParticleGenerator(Shader shader, Texture2D texture, uint amount)
        {
            (_shader, _texture, _amount) = (shader, texture, amount);
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
                RespawnParticle(_particles[unusedParticle], gameObject, offset);
            }
            for (int i = 0; i < _amount; ++i)
            {
                _particles[i].Life -= dt;
                if (_particles[i].Life <= 0.0f)
                {
                    continue;
                }
                _particles[i].Position -= _particles[i].Velocity * dt;
                _particles[i].Color.W -= dt * 2.5f;
            }
        }

        public void Draw()
        {
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.One);
            _shader.Use();
            foreach (Particle particle in _particles)
            {
                if (particle.Life <= 0.0f)
                {
                    continue;
                }
                _shader.SetVector2F("offset", particle.Position);
                _shader.SetVector4("color", particle.Color);
                _texture.Bind();
                GL.BindVertexArray(_vao);
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
            GL.GenVertexArrays(1, out _vao);
            GL.GenBuffers(1, out uint vbo);
            GL.BindVertexArray(_vao);

            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
            GL.BufferData(BufferTarget.ArrayBuffer, particleQuad.Length * sizeof(float), particleQuad, BufferUsageHint.StaticDraw);

            GL.EnableVertexAttribArray(0);
            GL.VertexAttribPointer(0, 4, VertexAttribPointerType.Float, false, 4 * sizeof(float), 0);
            GL.BindVertexArray(0);
            for (uint i = 0; i < _amount; ++i)
            {
                _particles.Add(new Particle());
            }
        }

        private int FirstUnusedParticle()
        {
            for (int i = _lastUsedParticle; i < _amount; ++i)
            {
                if (_particles[i].Life <= 0.0f)
                {
                    _lastUsedParticle = i;
                    return i;
                }
            }
            for (int i = 0; i < _lastUsedParticle; ++i)
            {
                if (_particles[i].Life <= 0.0f)
                {
                    _lastUsedParticle = i;
                    return i;
                }
            }
            _lastUsedParticle = 0;
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
