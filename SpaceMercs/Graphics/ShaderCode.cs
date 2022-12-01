namespace SpaceMercs.Graphics {
    internal class ShaderCode {
        public static string VertexShaderPos2Col4 = @"
#version 460

uniform mat4 model;
uniform mat4 view;
uniform mat4 projection;

layout (location = 0) in vec2 aPosition;
layout (location = 1) in vec4 aColour;

out vec4 vColour;

void main()
{
  gl_Position = projection * view * model * vec4(aPosition.x, aPosition.y, 0f, 1f);

  vColour = aColour;
}";

        public static string VertexShaderPos3Col4 = @"
#version 460

uniform mat4 model;
uniform mat4 projection;

layout (location = 0) in vec3 aPosition;
layout (location = 1) in vec4 aColour;

out vec4 vColour;

void main()
{
  gl_Position = projection * model * vec4(aPosition.x, aPosition.y, aPosition.z, 1f);

  vColour = aColour;
}";

        public static string VertexShaderPos3 = @"
#version 460

uniform mat4 model;
uniform mat4 view;
uniform mat4 projection;

layout (location = 0) in vec3 aPosition;

void main()
{
  gl_Position = projection * view * model * vec4(aPosition.x, aPosition.y, aPosition.z, 1f);
}";

        public static string PixelShaderFlatColour = @"
#version 460

uniform vec4 flatColour;

out vec4 pixelColour;

void main() 
{
  pixelColour = flatColour;
}";

        public static string PixelShaderColourFactor = @"
#version 460

uniform float colourFactor;

in vec4 vColour;
out vec4 pixelColour;

void main() 
{
  pixelColour = vColour * colourFactor;
}";
    }
}
