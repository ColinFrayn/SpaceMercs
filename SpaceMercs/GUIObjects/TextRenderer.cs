using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using SharpFont;
using SpaceMercs.Graphics;
using SpaceMercs.Graphics.Shapes;

// Library from here : https://github.com/space-wizards/SharpFont
// Based on this, updated for .NET : https://github.com/Robmaister/SharpFont
// Example from here : https://github.com/Rabbid76/c_sharp_opengl/tree/master/OpenTK_example_5

namespace SpaceMercs {
    internal enum TextAlign { Left, Centre, Right }

    internal class TextRenderOptions {
        public TextAlign TextPos { get; set; } = TextAlign.Centre;
        public float FixedWidth { get; set; } = 1f;
        public float FixedHeight { get; set; } = 1f;
        //public float Border { get; set; }
        public int Padding { get; set; } = 0; // In points
        //public Color BorderColour { get; set; }
        public Color TextColour { get; set; } = Color.White;
        //public Color ShadowColour { get; set; } = Color.LightGray;
        //public Color BackgroundColour { get; set; } = Color.Black;
        //public int Shadow { get; set; } = 0;
        public float XPos { get; set; } = 0f;
        public float YPos { get; set; } = 0f;
        public float ZPos { get; set; } = 0f;
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
        private const int _fontSize = 32;

        // Static constructor sets up shader program
        static TextRenderer() {
            textLabelShaderProgram = new ShaderProgram(TextLabelVertexShader, TextLabelFragmentShader);
            textLabelShaderProgram.SetUniform("model", Matrix4.Identity);
            textLabelShaderProgram.SetUniform("view", Matrix4.Identity);
            textLabelShaderProgram.SetUniform("projection", Matrix4.Identity);
            textLabelShaderProgram.SetUniform("textColour", 1f, 1f, 1f);
            textLabelFont32 = new FreeTypeFont(_fontSize);
        }

        // External measuring
        public static Vector2 MeasureText(string strText) => textLabelFont32.MeasureText(strText);
        public static int FontSize => _fontSize;

        // Draw this label (public wrappers)
        public static void Draw(string strText, Alignment ali) {
            TextRenderOptions tro = new TextRenderOptions() {
                Alignment = ali,
            };
            DrawAtInternal(strText, tro);
        }
        public static void Draw(string strText, Alignment ali, Color col) {
            TextRenderOptions tro = new TextRenderOptions() {
                Alignment = ali,
                TextColour = col,
            };
            DrawAtInternal(strText, tro);

        }
        public static void DrawAt(string strText, Alignment ali, float scale, float aspect, float xshift, float yshift, Color? col = null) {
            TextRenderOptions tro = new TextRenderOptions() {
                Alignment = ali,
                TextColour = col ?? Color.White,
                Aspect = aspect,
                XPos = xshift,
                YPos = yshift,
                Scale = scale
            };
            DrawAtInternal(strText, tro);
        }
        public static void DrawWithOptions(string strText, TextRenderOptions tro) {
            DrawAtInternal(strText, tro);
        }

        // Draw this label with the given settings
        private static void DrawAtInternal(string strText, TextRenderOptions tro) {
            float yScale = tro.Scale;
            float xScale = yScale / tro.Aspect;
            float yShift = tro.YPos;
            float xShift = tro.XPos;

            // Blend this font so we don't' obscure the background in the gaps
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

            // Setup projection so scren is unitised
            Matrix4 projectionM = Matrix4.CreateOrthographicOffCenter(0.0f, 1.0f, 1.0f, 0.0f, -1.0f, 1.0f);
            textLabelShaderProgram.SetUniform("projection", projectionM);

            // Render in the desired colour
            textLabelShaderProgram.SetUniform("textColour", (float)tro.TextColour.R / 255f, (float)tro.TextColour.G / 255f, (float)tro.TextColour.B / 255f);

            // Get scale of text from Font and scale/align accordingly
            Vector2 textSize = textLabelFont32.MeasureText(strText);
            float textHeight = textLabelFont32.PixelHeight;

            // Translate based on required alignment
            if (tro.Alignment == Alignment.TopLeft || tro.Alignment == Alignment.TopMiddle || tro.Alignment == Alignment.TopRight) yShift += tro.IsFixedSize ? tro.FixedHeight : yScale;
            if (tro.Alignment == Alignment.CentreLeft || tro.Alignment == Alignment.CentreMiddle || tro.Alignment == Alignment.CentreRight) yShift += tro.IsFixedSize ? tro.FixedHeight / 2f : yScale / 2f;
            if (tro.Alignment == Alignment.TopMiddle || tro.Alignment == Alignment.CentreMiddle || tro.Alignment == Alignment.BottomMiddle) xShift -= tro.IsFixedSize ? tro.FixedWidth / 2f : textSize.X * xScale / (2f * textHeight);
            if (tro.Alignment == Alignment.TopRight || tro.Alignment == Alignment.CentreRight || tro.Alignment == Alignment.BottomRight) xShift -= tro.IsFixedSize ? tro.FixedWidth : textSize.X * xScale / textHeight;

            // Generate the view matrix based on the desired scale
            Matrix4 scaleM;
            if (tro.IsFixedSize) {
                float scale = tro.FixedHeight;
                float textLen = scale * textSize.X / (tro.Aspect * textHeight);
                if (textLen > tro.FixedWidth) scale /= (textLen / tro.FixedWidth);
                scaleM = Matrix4.CreateScale(new Vector3(scale / tro.Aspect, scale, 1.0f));
                // Text align (horizontal only, and only for fixed size)
                if (tro.TextPos != TextAlign.Left) {
                    float gap = tro.FixedWidth - (scale * textSize.X / (tro.Aspect * textHeight));
                    if (tro.TextPos == TextAlign.Centre) xShift += gap / 2f;
                    if (tro.TextPos == TextAlign.Right) xShift += gap;
                }
            }
            else {
                scaleM = Matrix4.CreateScale(new Vector3(xScale / textHeight, yScale / textHeight, 1.0f));
            }
            Matrix4 transOriginM = Matrix4.CreateTranslation(new Vector3(xShift, yShift, tro.ZPos));
            Matrix4 viewM = scaleM * transOriginM;
            textLabelShaderProgram.SetUniform("view", viewM);

            // --- Render the actual text using the selected font ---
            textLabelFont32.RenderText(textLabelShaderProgram, strText);

            // Clean up
            GL.UseProgram(0);
            GL.Disable(EnableCap.Blend);
        }
    }
}
