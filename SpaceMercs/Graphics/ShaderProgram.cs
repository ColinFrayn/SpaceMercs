using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using System.Collections.Immutable;

namespace SpaceMercs.Graphics {
    public readonly struct ShaderUniform {
        public readonly int Location;
        public readonly ActiveUniformType Type;
        public ShaderUniform(int location, ActiveUniformType type) {
            Location = location;
            Type = type;
        }
    }
    public readonly struct ShaderAttribute {
        public readonly int Location;
        public readonly ActiveAttribType Type;
        public ShaderAttribute(int location, ActiveAttribType type) {
            Location = location;
            Type = type;
        }
    }

    internal class ShaderProgram : IDisposable {
        public readonly int ShaderProgramHandle;

        private readonly IReadOnlyDictionary<string, ShaderUniform> Uniforms;
        private readonly IReadOnlyDictionary<string, ShaderAttribute> Attributes;
        private bool isDisposed;

        public ShaderProgram(string vertexShaderCode, string pixelShaderCode) {
            // Setup shaders, compile & verify
            if (!CompileVertexShader(vertexShaderCode, out int vertexShaderHandle, out string vertexError)) {
                throw new ArgumentException($"Could not compile vertex shader : {vertexError}");
            }
            if (!CompilePixelShader(pixelShaderCode, out int pixelShaderHandle, out string pixelError)) {
                throw new ArgumentException($"Could not compile pixel shader : {pixelError}");
            }

            // Build shader program
            ShaderProgramHandle = CreateAndLinkProgram(vertexShaderHandle, pixelShaderHandle);

            // Pull out the relevant attributes
            Uniforms = CreateUniformList();
            Attributes = CreateAttributeList();
        }


        // Internal builders / setup

        private static bool CompilePixelShader(string code, out int pixelShaderHandle, out string error) {
            pixelShaderHandle = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(pixelShaderHandle, code);
            GL.CompileShader(pixelShaderHandle);
            error = GL.GetShaderInfoLog(pixelShaderHandle);
            return string.IsNullOrEmpty(error);
        }

        private static bool CompileVertexShader(string code, out int vertexShaderHandle, out string error) {
            vertexShaderHandle = GL.CreateShader(ShaderType.VertexShader);
            GL.ShaderSource(vertexShaderHandle, code);
            GL.CompileShader(vertexShaderHandle);
            error = GL.GetShaderInfoLog(vertexShaderHandle);
            return string.IsNullOrEmpty(error);
        }

        private static int CreateAndLinkProgram(int vertexShaderHandle, int pixelShaderHandle) {
            int shaderProgramHandle = GL.CreateProgram();
            GL.AttachShader(shaderProgramHandle, vertexShaderHandle);
            GL.AttachShader(shaderProgramHandle, pixelShaderHandle);
            GL.LinkProgram(shaderProgramHandle);
            GL.DetachShader(shaderProgramHandle, vertexShaderHandle);
            GL.DetachShader(shaderProgramHandle, pixelShaderHandle);
            GL.DeleteShader(vertexShaderHandle);
            GL.DeleteShader(pixelShaderHandle);
            return shaderProgramHandle;
        }

        private IReadOnlyDictionary<string, ShaderUniform> CreateUniformList() {
            GL.GetProgram(ShaderProgramHandle, GetProgramParameterName.ActiveUniforms, out int uniformCount);
            IDictionary<string, ShaderUniform> uniforms = new Dictionary<string, ShaderUniform>();
            for (int i = 0; i < uniformCount; i++) {
                GL.GetActiveUniform(ShaderProgramHandle, i, 256, out _, out _, out ActiveUniformType type, out string name);
                int loc = GL.GetUniformLocation(ShaderProgramHandle, name);
                if (uniforms.ContainsKey(name)) {
                    throw new Exception($"Found duplicate Uniform {name}");
                }
                uniforms.Add(name, new ShaderUniform(loc, type));
            }
            return uniforms.ToImmutableDictionary();
        }

        private IReadOnlyDictionary<string, ShaderAttribute> CreateAttributeList() {
            GL.GetProgram(ShaderProgramHandle, GetProgramParameterName.ActiveAttributes, out int attributeCount);
            IDictionary<string, ShaderAttribute> attributes = new Dictionary<string, ShaderAttribute>();
            for (int i = 0; i < attributeCount; i++) {
                GL.GetActiveAttrib(ShaderProgramHandle, i, 256, out _, out _, out ActiveAttribType type, out string name);
                int loc = GL.GetAttribLocation(ShaderProgramHandle, name);
                if (attributes.ContainsKey(name)) {
                    throw new Exception($"Found duplicate Attribute {name}");
                }
                attributes.Add(name, new ShaderAttribute(loc, type));
            }
            return attributes.ToImmutableDictionary();
        }


        // Access / Set Uniforms and attributes

        public IReadOnlyDictionary<string, ShaderUniform> GetUniformList() {
            return new Dictionary<string, ShaderUniform>(Uniforms).ToImmutableDictionary();
        }

        public IReadOnlyDictionary<string, ShaderAttribute> GetAttributeList() {
            return new Dictionary<string, ShaderAttribute>(Attributes).ToImmutableDictionary();
        }

        public void SetUniform(string name, bool b) {
            VerifyUniform(name);
            ShaderUniform uniform = Uniforms[name];
            if (uniform.Type != ActiveUniformType.Bool) {
                throw new ArgumentException($"Uniform was not of type Bool : {name}");
            }
            GL.UseProgram(ShaderProgramHandle);
            GL.Uniform1(uniform.Location, b ? 1 : 0); // Annoyingly you can't set bools directly
            GL.UseProgram(0);
        }

        public void SetUniform(string name, float v1) {
            VerifyUniform(name);
            ShaderUniform uniform = Uniforms[name];
            if (uniform.Type != ActiveUniformType.Float) {
                throw new ArgumentException($"Uniform was not of type Float : {name}");
            }
            GL.UseProgram(ShaderProgramHandle);
            GL.Uniform1(uniform.Location, v1);
            GL.UseProgram(0);
        }

        public void SetUniform(string name, float v1, float v2) {
            VerifyUniform(name);
            ShaderUniform uniform = Uniforms[name];
            if (uniform.Type != ActiveUniformType.FloatVec2) {
                throw new ArgumentException($"Uniform was not of type FloatVec2 : {name}");
            }
            GL.UseProgram(ShaderProgramHandle);
            GL.Uniform2(uniform.Location, v1, v2);
            GL.UseProgram(0);
        }

        public void SetUniform(string name, float v1, float v2, float v3) {
            VerifyUniform(name);
            ShaderUniform uniform = Uniforms[name];
            if (uniform.Type != ActiveUniformType.FloatVec3) {
                throw new ArgumentException($"Uniform was not of type Float-Vector3 : {name}");
            }
            GL.UseProgram(ShaderProgramHandle);
            GL.Uniform3(uniform.Location, v1, v2, v3);
            GL.UseProgram(0);
        }

        public void SetUniform(string name, Matrix4 m4) {
            VerifyUniform(name);
            ShaderUniform uniform = Uniforms[name];
            if (uniform.Type != ActiveUniformType.FloatMat4) {
                throw new ArgumentException($"Uniform was not of type Float-Matrix4 : {name}");
            }
            GL.UseProgram(ShaderProgramHandle);
            GL.UniformMatrix4(uniform.Location, false, ref m4);
            GL.UseProgram(0);
        }

        public void SetUniform(string name, Vector3 v3) {
            VerifyUniform(name);
            ShaderUniform uniform = Uniforms[name];
            if (uniform.Type != ActiveUniformType.FloatVec3) {
                throw new ArgumentException($"Uniform was not of type Float-Vector3 : {name}");
            }
            GL.UseProgram(ShaderProgramHandle);
            GL.Uniform3(uniform.Location, v3.X, v3.Y, v3.Z);
            GL.UseProgram(0);
        }

        public void SetUniform(string name, Vector4 v4) {
            VerifyUniform(name);
            ShaderUniform uniform = Uniforms[name];
            if (uniform.Type != ActiveUniformType.FloatVec4) {
                throw new ArgumentException($"Uniform was not of type Float-Vector4 : {name}");
            }
            GL.UseProgram(ShaderProgramHandle);
            GL.Uniform4(uniform.Location, v4.X, v4.Y, v4.Z, v4.W);
            GL.UseProgram(0);
        }

        private void VerifyUniform(string name) {
            if (!Uniforms.ContainsKey(name)) {
                throw new ArgumentException($"Uniform was not found : {name}");
            }
        }

        // IDisposable
        ~ShaderProgram() {
            Dispose();
        }

        public void Dispose() {
            if (isDisposed) return;
            isDisposed = true;
            GL.UseProgram(0);
            GL.DeleteProgram(ShaderProgramHandle);
            GC.SuppressFinalize(this);
        }

    }
}
