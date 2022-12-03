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
  gl_Position = projection * model * vec4(aPosition, 1f);

  vColour = aColour;
}";

        public static string VertexShaderPos3TexNorm = @"
#version 460

uniform mat4 model;
uniform mat4 view;
uniform mat4 projection;

layout (location = 0) in vec3 aPosition;
layout (location = 1) in vec2 aTexturePos;
layout (location = 2) in vec3 aNormal;

out vec3 vNorm;
out vec2 vUV;

void main()
{
  gl_Position = projection * view * model * vec4(aPosition, 1f);
  vUV = aTexturePos;
  vNorm = aNormal;
}";

        public static string VertexShaderPos3 = @"
#version 460

uniform mat4 model;
uniform mat4 view;
uniform mat4 projection;

layout (location = 0) in vec3 aPosition;

void main()
{
  gl_Position = projection * view * model * vec4(aPosition, 1f);
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

        public static string PixelShaderTexNorm = @"
#version 460

in vec2 vUV;
in vec3 vNorm;

layout (binding = 0) uniform sampler2D u_texture;

out vec4 fragColor;

void main()
{
  vec2 uv = vUV.xy;
  vec4 textureVal = texture(u_texture, uv);
  fragColor = textureVal;
}";
    }
}
