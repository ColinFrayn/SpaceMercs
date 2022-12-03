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
out vec3 vFragPos;

void main()
{
  gl_Position = projection * view * model * vec4(aPosition, 1f);
  vUV = aTexturePos;
  //vNorm = aNormal;
  vNorm = mat3(transpose(inverse(model))) * aNormal;  
  vFragPos = vec3(model * vec4(aPosition, 1.0));
}";

        public static string VertexShaderPos3FlatNorm = @"
#version 460

uniform mat4 model;
uniform mat4 view;
uniform mat4 projection;

layout (location = 0) in vec3 aPosition;
layout (location = 1) in vec3 aNormal;

out vec3 vNorm;
out vec3 vFragPos;

void main()
{
  gl_Position = projection * view * model * vec4(aPosition, 1f);
  //vNorm = aNormal;
  vNorm = mat3(transpose(inverse(model))) * aNormal;  
  vFragPos = vec3(model * vec4(aPosition, 1.0));
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

        public static string PixelShaderTexLitNorm = @"
#version 460

uniform vec3 lightPos;
uniform float ambient;

in vec2 vUV;
in vec3 vNorm;
in vec3 vFragPos;  

layout (binding = 0) uniform sampler2D u_texture;

out vec4 fragColor;

void main()
{
  vec4 textureVal = texture(u_texture, vUV);
  vec3 norm = normalize(vNorm);
  vec3 lightDir = normalize(lightPos - vFragPos);  
  float diff = max(dot(norm, lightDir), 0.0);
  vec3 diffuseCol = diff * vec3(1.0,1.0,1.0); // * lightColor;
  vec3 ambientCol = ambient * vec3(1.0,1.0,1.0); // * lightColor
  vec3 result = (ambientCol + diffuseCol) * textureVal.xyz;
  fragColor = vec4(result, 1.0);
  //fragColor = textureVal;
}";

        public static string PixelShaderLitFlatColour = @"
#version 460

uniform vec3 lightPos;
uniform float ambient;
uniform vec4 flatColour;

in vec3 vNorm;
in vec3 vFragPos;  

out vec4 fragColor;

void main()
{
  vec3 norm = normalize(vNorm);
  vec3 lightDir = normalize(lightPos - vFragPos);  
  float diff = max(dot(norm, lightDir), 0.0);
  vec3 diffuseCol = diff * vec3(1.0,1.0,1.0); // * lightColor;
  vec3 ambientCol = ambient * vec3(1.0,1.0,1.0); // * lightColor
  vec3 result = (ambientCol + diffuseCol) * flatColour.xyz;
  fragColor = vec4(result, 1.0);
}";
    }
}
