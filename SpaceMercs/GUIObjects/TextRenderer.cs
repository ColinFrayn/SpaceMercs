using OpenTK.Compute.OpenCL;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using SharpFont;
using SpaceMercs.Graphics;
using SpaceMercs.Graphics.Shapes;
using System.IO;
using System.Reflection;

// Library from here : https://github.com/space-wizards/SharpFont
// Based on this, updated for .NET : https://github.com/Robmaister/SharpFont
// Example from here : https://github.com/Rabbid76/c_sharp_opengl/tree/master/OpenTK_example_5

namespace SpaceMercs {
  internal static class TextRenderer {
    public enum TextAlign { Left, Centre, Right }
    //public TextAlign TextPos { get; set; } = TextRenderer.TextAlign.Centre;
    //public int Width { get; private set; }
    //public int Height { get; private set; }
    //public int Lines { get { return strText.Count; } }
    //public int Border { get; set; }
    //public int Padding { get; set; }
    //public Color BorderColour { get; set; }
    //public Color TextColour { get; set; }
    //public Color ShadowColour { get; set; }
    //public Color BackgroundColour { get; set; }
    //public bool bEnabled { get; set; }
    //public int Shadow { get; set; }

    // The text to display
    //private readonly List<string> strText;

    // SharpFont Settings
    private static readonly FreeTypeFont textLabelFont32;
    private static readonly string TextLabelVertexShader = @"
#version 460

uniform mat4 model;
uniform mat4 view;
uniform mat4 projection;

layout (location = 0) in vec2 in_pos;
layout (location = 1) in vec2 in_uv;

out vec2 vUV;
            
void main()
{
  vUV         = in_uv.xy;
	gl_Position = projection * view * model * vec4(in_pos.xy, 0.0, 1.0);
}";
    private static readonly string TextLabelVertexShader_New = @"
#version 460

uniform mat4 model;
uniform mat4 projection;

layout (location = 0) in vec2 in_pos;
layout (location = 1) in vec2 in_uv;

void main()
{
	gl_Position = projection * model * vec4(in_pos.xy, 0.0, 1.0);
}";
    private static readonly string TextLabelFragmentShader = @"
#version 460

in vec2 vUV;

layout (binding = 0) uniform sampler2D u_texture;

uniform vec3 textColour;

out vec4 fragColor;

void main()
{
  vec2 uv = vUV.xy;
  float textureVal = texture(u_texture, uv).r;
  fragColor = vec4(textColour.rgb*textureVal, 1f);
  //fragColor = vec4(textColour.rgb, 1f);
}";
    private static readonly string TextLabelFragmentShader_New = @"
#version 460

uniform vec3 textColour;

out vec4 fragColor;

void main()
{
  fragColor = vec4(textColour.rgb, 1f);
}";
    private static readonly ShaderProgram textLabelShaderProgram;

    // Static constructor sets up shader program
    static TextRenderer() {
      textLabelShaderProgram = new ShaderProgram(TextLabelVertexShader, TextLabelFragmentShader);
      //textLabelShaderProgram = new ShaderProgram(TextLabelVertexShader_New, TextLabelFragmentShader_New);
      textLabelShaderProgram.SetUniform("model", Matrix4.Identity);
      textLabelShaderProgram.SetUniform("view", Matrix4.Identity);
      textLabelShaderProgram.SetUniform("projection", Matrix4.Identity);
      textLabelShaderProgram.SetUniform("textColour", 1f, 1f, 1f);
      textLabelFont32 = new FreeTypeFont(32);
    }

    // Draw this label
    public static void Draw(string strText, Alignment ali, Color col) {
      DrawAtInternal(strText, ali, col, 0.3f, 0.2f, 0.4f, 0.5f);
    }
    public static void Draw(string strText, Alignment ali) {
      DrawAtInternal(strText, ali, Color.White, 0.3f, 0.2f, 0.4f, 0.5f);
    }
    public static void DrawAt(Alignment ali, int xshift, int yshift) {
      //DrawAtInternal(ali, 1.0 / (double)TextBitmap.Height, 1.0 / (double)TextBitmap.Height, xshift, yshift);
    }
    public static void Draw(Alignment ali, double dwidth, double dheight) {
      //DrawAtInternal(ali, dwidth / (double)TextBitmap.Width, dheight / (double)TextBitmap.Height, 0, 0);
    }
    public static void Draw(Alignment ali, double dwidth, double dheight, int xshift, int yshift) {
      //DrawAtInternal(ali, dwidth / (double)TextBitmap.Width, dheight / (double)TextBitmap.Height, xshift, yshift);
    }
    private static void DrawAtInternal(string strText, Alignment ali, Color col, float dXScale, float dYScale, float xshift, float yshift) {
      Matrix4 projectionM = Matrix4.CreateOrthographicOffCenter(-10.0f, 10.0f, 10.0f, -10.0f, -1.0f, 1.0f);

      // "View matrix" components?
      // TODO
      //Matrix4 transOriginM = Matrix4.CreateTranslation(new Vector3(xshift, yshift, 0f));

      //GL.Enable(EnableCap.Blend);
      //GL.BlendFunc(0, BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);

      GL.UseProgram(textLabelShaderProgram.ShaderProgramHandle);
      textLabelShaderProgram.SetUniform("projection", projectionM);
      textLabelShaderProgram.SetUniform("textColour", (float)col.R / 255f, (float)col.G / 255f, (float)col.B / 255f);

      // Get scale of text from Font and scale/align accordingly
      Vector2 textSize = textLabelFont32.MeasureText(strText);
      Matrix4 scaleM = Matrix4.CreateScale(new Vector3(1/textSize.Y, 1 / textSize.Y, 1.0f));
      // TODO ALIGN
      textLabelShaderProgram.SetUniform("view", scaleM);

      // Render the actual text using teh selected font
      textLabelFont32.RenderText(textLabelShaderProgram, strText);

      GL.UseProgram(0);
      //GL.Disable(EnableCap.Blend);

      //if (ali == Alignment.BottomLeft || ali == Alignment.BottomMiddle || ali == Alignment.BottomRight) yshift = -TextBitmap.Height;
      //if (ali == Alignment.CentreLeft || ali == Alignment.CentreMiddle || ali == Alignment.CentreRight) yshift = -TextBitmap.Height / 2;
      //if (ali == Alignment.TopMiddle || ali == Alignment.CentreMiddle || ali == Alignment.BottomMiddle) xshift = -TextBitmap.Width / 2;
      //if (ali == Alignment.TopRight || ali == Alignment.CentreRight || ali == Alignment.BottomRight) xshift = -TextBitmap.Width;
    }
  }
}
