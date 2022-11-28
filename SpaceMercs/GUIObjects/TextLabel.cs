using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using SharpFont;
using SpaceMercs.Graphics;
using System.IO;
using System.Reflection;

// Library from here : https://github.com/space-wizards/SharpFont
// Based on this, updated for .NET : https://github.com/Robmaister/SharpFont
// Example from here : https://github.com/Rabbid76/c_sharp_opengl/tree/master/OpenTK_example_5

namespace SpaceMercs {
  class TextLabel {
    public enum Alignment { TopLeft, TopMiddle, TopRight, CentreLeft, CentreMiddle, CentreRight, BottomLeft, BottomMiddle, BottomRight };
    public enum TextAlign { Left, Centre, Right }
    public TextAlign TextPos { get; set; } = TextLabel.TextAlign.Centre;
    public int Width { get; private set; }
    public int Height { get; private set; }
    public int Lines { get { return strText.Count; } }
    public int Border { get; set; }
    public int Padding { get; set; }
    public Color BorderColour { get; set; }
    public Color TextColour { get; set; }
    public Color ShadowColour { get; set; }
    public Color BackgroundColour { get; set; }
    public bool bEnabled { get; set; }
    public int Shadow { get; set; }

    // The text to display
    private readonly List<string> strText;

    // SharpFont Settings
    private static readonly FreeTypeFont textLabelFont32;
    private static readonly string TextLabelVertexShader = @"
#version 460
uniform mat4 model;
uniform mat4 projection;

layout (location = 0) in vec2 in_pos;
layout (location = 1) in vec2 in_uv;

out vec2 vUV;
            
void main()
{
  vUV         = in_uv.xy;
	gl_Position = projection * model * vec4(in_pos.xy, 0.0, 1.0);
}";
    private static readonly string TextLabelFragmentShader = @"
#version 460

in vec2 vUV;

layout (binding = 0)  uniform sampler2D u_texture;

//layout (location = 2) uniform vec3 textColour;

out vec4 fragColor;

void main()
{
  //vec2 uv = vUV.xy;
  //float text = texture(u_texture, uv).r;
  fragColor = vec4(1f,1f,1f,1f); //vec4(textColour.rgb*text, 1f);
}";
    private static readonly ShaderProgram textLabelShaderProgram;

    // Static constructor sets up shader program
    static TextLabel() {
      textLabelShaderProgram = new ShaderProgram(TextLabelVertexShader, TextLabelFragmentShader);
      textLabelShaderProgram.SetUniform("model", Matrix4.Identity);
      GL.UseProgram(textLabelShaderProgram.ShaderProgramHandle);
      textLabelFont32 = new FreeTypeFont(32);
      GL.UseProgram(0);
    }

    // Set up the text label
    public TextLabel() {
      strText = new List<string> { " " };
      SetupTextLabel();
    }
    public TextLabel(string strIn) {
      strText = new List<string> { strIn };
      SetupTextLabel();
    }
    private void SetupTextLabel() {
      Border = 0;
      Padding = 5;
      BorderColour = Color.Black;
      BackgroundColour = Color.FromArgb(0,0,0,0);
      TextColour = Color.White;
      ShadowColour = Color.DarkGray;
      bEnabled = true;
      Shadow = 0;
    }

    // Change the label text
    public void UpdateText(string strNew) {
      if (strText.Equals(strNew)) return;
      strText.Clear();
      strText.Add(strNew);
      bEnabled = true;
    }
    public void UpdateTextWithWordWrap(string strNew, int maxlen) {
      strText.Clear();
      while (strNew.Length > 0) {
        int pos = 0, lastpos = 0;
        do {
          lastpos = pos;
          pos = strNew.IndexOf(' ', pos+1);
          if (pos == -1) {
            pos = strNew.Length;
            if (pos <= maxlen) lastpos = pos;
          }
        } while (pos < maxlen && pos < strNew.Length);
        strText.Add(strNew.Substring(0,lastpos));
        if (lastpos < strNew.Length) {
          strNew = strNew.Substring(lastpos + 1);
        }
        else break;
      }
    }
    public void UpdateTextFromList(IEnumerable<string> strListIn) {
      strText.Clear();
      foreach (string str in strListIn) {
        strText.Add(str);
      }
    }

    // Draw this label
    public void Draw(Vector2i Size, Alignment ali) {
      //DrawAtInternal(Size, ali, 1.0 / (double)TextBitmap.Height, 1.0 / (double)TextBitmap.Height, 0, 0);
      DrawAtInternal(Size, ali, 1.0, 1.0, 0, 0);
    }
    public void DrawAt(Vector2i Size, Alignment ali, int xshift, int yshift) {
      //DrawAtInternal(Size, ali, 1.0 / (double)TextBitmap.Height, 1.0 / (double)TextBitmap.Height, xshift, yshift);
    }
    public void Draw(Vector2i Size, Alignment ali, double dwidth, double dheight) {
      //DrawAtInternal(Size, ali, dwidth / (double)TextBitmap.Width, dheight / (double)TextBitmap.Height, 0, 0);
    }
    public void Draw(Vector2i Size, Alignment ali, double dwidth, double dheight, int xshift, int yshift) {
      //DrawAtInternal(Size, ali, dwidth / (double)TextBitmap.Width, dheight / (double)TextBitmap.Height, xshift, yshift);
    }
    private void DrawAtInternal(Vector2i Size, Alignment ali, double dXScale, double dYScale, int xshift, int yshift) {
      if (!bEnabled) return;

      Matrix4 projectionM = Matrix4.CreateOrthographicOffCenter(0.0f, Size.X, Size.Y, 0.0f, -1.0f, 1.0f);

      GL.Enable(EnableCap.Blend);
      GL.BlendFunc(0, BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);

      GL.UseProgram(textLabelShaderProgram.ShaderProgramHandle);
      textLabelShaderProgram.SetUniform("projection", projectionM);
      //textLabelShaderProgram.SetUniform("textColour", (float)TextColour.R / 255f, (float)TextColour.G / 255f, (float)TextColour.B / 255f);
      //_font.RenderText("This is sample text", xshift, yshift, (float)dXScale, (float)dYScale, new Vector2(1f, 0f));
      textLabelFont32.RenderText(textLabelShaderProgram, "This is sample text", 0.0f, 0.0f, 1f, 1f);

      //if (ali == Alignment.BottomLeft || ali == Alignment.BottomMiddle || ali == Alignment.BottomRight) yshift = -TextBitmap.Height;
      //if (ali == Alignment.CentreLeft || ali == Alignment.CentreMiddle || ali == Alignment.CentreRight) yshift = -TextBitmap.Height / 2;
      //if (ali == Alignment.TopMiddle || ali == Alignment.CentreMiddle || ali == Alignment.BottomMiddle) xshift = -TextBitmap.Width / 2;
      //if (ali == Alignment.TopRight || ali == Alignment.CentreRight || ali == Alignment.BottomRight) xshift = -TextBitmap.Width;
    }
  }
}
