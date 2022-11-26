namespace SpaceMercs.Graphics {
  internal class ShaderCode {
    public static string VertexShader2DColourFactor = @"
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

    public static string VertexShader3DTransform = @"
#version 330 core

uniform vec2 viewportSize;
uniform mat4 transform;

layout (location = 0) in vec3 aPosition;
layout (location = 1) in vec4 aColour;

out vec4 vColour;

void main()
{
  float nx = aPosition.x / viewportSize.x * 2f - 1f;
  float ny = aPosition.y / viewportSize.y * 2f - 1f;
  gl_Position = vec4(aPosition, 1f) * transform;

  vColour = aColour;
}";

    public static string PixelShaderCode = @"
#version 330 core

in vec4 vColour;
out vec4 pixelColour;

void main() 
{
  pixelColour = vColour;
}";
  }
}
