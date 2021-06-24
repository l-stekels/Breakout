using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;

namespace Breakout
{
    public class Shader
    {
        public int Id;

        private readonly Dictionary<string, int> _uniformLocations = new();

        public Shader(string vertexSource, string fragmentSource, string geometrySource)
        {
            int geometryShader = 0;
            int vertexShader = GL.CreateShader(ShaderType.VertexShader);
            GL.ShaderSource(vertexShader, vertexSource);
            CompileShader(vertexShader, "VERTEX");

            int fragmentShader = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(fragmentShader, fragmentSource);
            CompileShader(fragmentShader, "FRAGMENT");

            if (!string.IsNullOrEmpty(geometrySource))
            {
                geometryShader = GL.CreateShader(ShaderType.GeometryShader);
                GL.ShaderSource(geometryShader, geometrySource);
                CompileShader(geometryShader, "GEOMETRY");
            }

            Id = GL.CreateProgram();

            GL.AttachShader(Id, vertexShader);
            GL.AttachShader(Id, fragmentShader);
            if (!string.IsNullOrEmpty(geometrySource))
            {
                GL.AttachShader(Id, geometryShader);
            }
            LinkProgram(Id);


            GL.DetachShader(Id, vertexShader);
            GL.DetachShader(Id, fragmentShader);

            GL.DeleteShader(vertexShader);
            GL.DeleteShader(fragmentShader);
            if (!string.IsNullOrEmpty(geometrySource))
            {
                GL.DetachShader(Id, geometryShader);
                GL.DeleteShader(geometryShader);
            }
            GL.GetProgram(Id, GetProgramParameterName.ActiveUniforms, out int numberOfUniforms);

            for (var i = 0; i < numberOfUniforms; i++)
            {
                string key = GL.GetActiveUniform(Id, i, out _, out _);
                int location = GL.GetUniformLocation(Id, key);
                _uniformLocations.Add(key, location);
            }

        }

        public Shader Use()
        {
            GL.UseProgram(Id);
            return this;
        }

        public Shader SetFloat(string name, float value)
        {
            Use();
            GL.Uniform1(
                _uniformLocations[name],
                value
            );
            return this;
        }

        public Shader SetInteger(string name, int value)
        {
            Use();
            GL.Uniform1(
                _uniformLocations[name],
                value
            );
            return this;
        }

        public Shader SetVector2F(string name, float x, float y)
        {
            Use();
            GL.Uniform2(
                _uniformLocations[name],
                x,
                y
            );
            return this;
        }

        public Shader SetVector2F(string name, Vector2 vector)
        {
            Use();
            GL.Uniform2(
                _uniformLocations[name],
                vector.X,
                vector.Y
            );
            return this;
        }

        public Shader SetVector3F(string name, float x, float y, float z)
        {
            Use();
            GL.Uniform3(
                _uniformLocations[name],
                x,
                y,
                z
            );
            return this;
        }

        public Shader SetVector3F(string name, Vector3 vector)
        {
            Use();
            GL.Uniform3(
                _uniformLocations[name],
                vector.X,
                vector.Y,
                vector.Z
            );
            return this;
        }

        public Shader SetVector4(string name, float x, float y, float z, float w)
        {
            Use();
            GL.Uniform4(
                _uniformLocations[name],
                x,
                y,
                z,
                w
            );
            return this;
        }

        public Shader SetVector4(string name, Vector4 vector)
        {
            Use();
            GL.Uniform4(
                _uniformLocations[name],
                vector.X,
                vector.Y,
                vector.Z,
                vector.W
            );
            return this;
        }

        public Shader SetMatrix4(string name, Matrix4 matrix)
        {
            Use();
            GL.UniformMatrix4(
                _uniformLocations[name],
                false,
                ref matrix
            );
            return this;
        }

        private static void CompileShader(int shader, string type)
        {
            GL.CompileShader(shader);
            GL.GetShader(shader, ShaderParameter.CompileStatus, out var success);
            if (Convert.ToBoolean(success))
            {
                return;
            }
            string infoLog = GL.GetShaderInfoLog(shader);
            Console.WriteLine(
                $"| ERROR::SHADER: Compile-time error: Type: {type} \n {infoLog} \n -- -------------------------------------- --"
            );

        }

        private static void LinkProgram(int program)
        {
            GL.LinkProgram(program);
            GL.GetProgram(program, GetProgramParameterName.LinkStatus, out int success);

            if (Convert.ToBoolean(success))
            {
                return;
            }
            string infoLog = GL.GetProgramInfoLog(program);
            Console.WriteLine(
                $"| ERROR::SHADER: Link-time error: Type: PROGRAM \n {infoLog} \n -- -------------------------------------- --"
            );
        }
    }
}
