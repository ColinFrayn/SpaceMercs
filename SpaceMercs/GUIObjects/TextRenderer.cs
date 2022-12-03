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
    internal enum TextAlign { Left, Centre, Right }

    internal class TextRenderOptions {
        public TextAlign TextPos { get; set; } = TextAlign.Centre;
        public float FixedWidth { get; set; } = 1f;
        public float FixedHeight { get; set; } = 1f;
        //public float Border { get; set; };
        //public float Padding { get; set; };
        //public Color BorderColour { get; set; };
        public Color TextColour { get; set; } = Color.White;
        //public Color ShadowColour { get; set; } = Color.LightGray;
        //public Color BackgroundColour { get; set; } = Color.Black;
        //public int Shadow { get; set; } = 0;
        public float XPos { get; set; } = 0f;
        public float YPos { get; set; } = 0f;
        public float Scale { get; set; } = 1f;
        public bool IsFixedSize { get; set; } = false;
        public Alignment Alignment { get; set; } = Alignment.TopLeft;
        public float Aspect { get; set; } = 1f;
    }

    internal static class TextRenderer {
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
  fragColor = vec4(textColour.rgb*textureVal, textureVal);
}";
        private static readonly ShaderProgram textLabelShaderProgram;

        // Static constructor sets up shader program
        static TextRenderer() {
            textLabelShaderProgram = new ShaderProgram(TextLabelVertexShader, TextLabelFragmentShader);
            textLabelShaderProgram.SetUniform("model", Matrix4.Identity);
            textLabelShaderProgram.SetUniform("view", Matrix4.Identity);
            textLabelShaderProgram.SetUniform("projection", Matrix4.Identity);
            textLabelShaderProgram.SetUniform("textColour", 1f, 1f, 1f);
            textLabelFont32 = new FreeTypeFont(32);
        }

        // Draw this label
        public static void Draw(string strText, Alignment ali) {
            DrawAtInternal(strText, ali, Color.White, 1f, 1f, 0f, 0f);
        }
        public static void Draw(string strText, Alignment ali, Color col) {
            DrawAtInternal(strText, ali, col, 1f, 1f, 0f, 0f);
        }
        public static void Draw(string strText, TextRenderOptions tro) {
            if (tro.IsFixedSize) {
                DrawAtInternal(strText, tro.Alignment, tro.TextColour, tro.FixedWidth, tro.FixedHeight, tro.XPos, tro.YPos);
            }
            else {
                DrawAtInternal(strText, tro.Alignment, tro.TextColour, tro.Scale / tro.Aspect, tro.Scale, tro.XPos, tro.YPos);
            }
        }
        public static void DrawAt(string strText, Alignment ali, float fwidth, float fheight, float xshift, float yshift) {
            DrawAtInternal(strText, ali, Color.White, fwidth, fheight, xshift, yshift);
        }

        private static void DrawAtInternal(string strText, Alignment ali, Color col, float xScale, float yScale, float xShift, float yShift) {
            Matrix4 projectionM = Matrix4.CreateOrthographicOffCenter(0.0f, 1.0f, 1.0f, 0.0f, -1.0f, 1.0f);

            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

            GL.UseProgram(textLabelShaderProgram.ShaderProgramHandle);
            textLabelShaderProgram.SetUniform("projection", projectionM);
            textLabelShaderProgram.SetUniform("textColour", (float)col.R / 255f, (float)col.G / 255f, (float)col.B / 255f);

            // Get scale of text from Font and scale/align accordingly
            Vector2 textSize = textLabelFont32.MeasureText(strText);
            float textHeight = textSize.Y;

            // Translate based on required alignment
            if (ali == Alignment.BottomLeft || ali == Alignment.BottomMiddle || ali == Alignment.BottomRight) yShift += yScale;
            if (ali == Alignment.CentreLeft || ali == Alignment.CentreMiddle || ali == Alignment.CentreRight) yShift += yScale / 2f;
            if (ali == Alignment.TopMiddle || ali == Alignment.CentreMiddle || ali == Alignment.BottomMiddle) xShift -= textSize.X * xScale / (2f * textHeight);
            if (ali == Alignment.TopRight || ali == Alignment.CentreRight || ali == Alignment.BottomRight) xShift -= textSize.X * xScale / textHeight;

            // Generate the view matrix
            Matrix4 scaleM = Matrix4.CreateScale(new Vector3(xScale / textHeight, yScale / textHeight, 1.0f));
            Matrix4 transOriginM = Matrix4.CreateTranslation(new Vector3(xShift, yShift, 0f));
            Matrix4 viewM = scaleM * transOriginM;
            textLabelShaderProgram.SetUniform("view", viewM);

            // Render the actual text using the selected font
            textLabelFont32.RenderText(textLabelShaderProgram, strText);

            GL.UseProgram(0);
            GL.Disable(EnableCap.Blend);
        }
    }
}
