using OpenTK.Graphics.OpenGL;
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
    public ShaderAttribute( int location, ActiveAttribType type) {
      Location = location;
      Type = type;
    }
  }

  internal class ShaderProgram : IDisposable {
    public static string VertexShaderCode = @"
#version 330 core

uniform vec2 viewportSize;
uniform float colourFactor;

layout (location = 0) in vec2 aPosition;
layout (location = 1) in vec4 aColour;

out vec4 vColour;

void main()
{
  float nx = aPosition.x / viewportSize.x * 2f - 1f;
  float ny = aPosition.y / viewportSize.y * 2f - 1f;
  gl_Position = vec4(nx, ny, 0f, 1f);

  vColour = aColour * colourFactor;
}";
    public static string PixelShaderCode = @"
#version 330 core

in vec4 vColour;
out vec4 pixelColour;

void main() 
{
  pixelColour = vColour;
}";

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

    // Access

    public IReadOnlyDictionary<string, ShaderUniform> GetUniformList() {
      return new Dictionary<string, ShaderUniform>(Uniforms).ToImmutableDictionary();
    }

    public IReadOnlyDictionary<string, ShaderAttribute> GetAttributeList() {
      return new Dictionary<string, ShaderAttribute>(Attributes).ToImmutableDictionary();
    }

    public void SetUniform(string name, float v1) {
      if (!Uniforms.ContainsKey(name)) {
        throw new ArgumentException($"Uniform was not found : {nameof(name)}");
      }
      ShaderUniform uniform = Uniforms[name];
      if (uniform.Type != ActiveUniformType.Float) {
        throw new ArgumentException($"Uniform was not of type Float : {nameof(name)}");
      }
      GL.UseProgram(ShaderProgramHandle);
      GL.Uniform1(uniform.Location, v1);
      GL.UseProgram(0);
    }

    public void SetUniform(string name, float v1, float v2) {
      if (!Uniforms.ContainsKey(name)) {
        throw new ArgumentException($"Uniform was not found : {nameof(name)}");
      }
      ShaderUniform uniform = Uniforms[name];
      if (uniform.Type != ActiveUniformType.FloatVec2) {
        throw new ArgumentException($"Uniform was not of type FloatVec2 : {nameof(name)}");
      }
      GL.UseProgram(ShaderProgramHandle);
      GL.Uniform2(uniform.Location, v1, v2);
      GL.UseProgram(0);
    }


    // System
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
