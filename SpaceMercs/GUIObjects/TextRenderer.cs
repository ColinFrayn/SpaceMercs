using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using SpaceMercs.Graphics;
using SpaceMercs.Graphics.Shapes;
using System.Windows.Media.TextFormatting;

// Library from here : https://github.com/space-wizards/SharpFont
// Based on this, updated for .NET : https://github.com/Robmaister/SharpFont
// Example from here : https://github.com/Rabbid76/c_sharp_opengl/tree/master/OpenTK_example_5

namespace SpaceMercs {
    internal enum TextAlign { Left, Centre, Right }

    internal class TextRenderOptions {
        public Color TextColour { get; set; } = Color.White;
        public float KerningShift { get; set; } = 0f;
        public float XPos { get; set; } = 0f;
        public float YPos { get; set; } = 0f;
        public float ZPos { get; set; } = 0f;
        public float Scale { get; set; } = 1f;
        public Alignment Alignment { get; set; } = Alignment.TopLeft;
        public float Aspect { get; set; } = 1f;
        public Matrix4? View { get; set; } = null;
        public Matrix4? Projection { get; set; } = null;
        public bool FlipY { get; set; } = false;

        // TBC
        //public float Border { get; set; }
        //public Color BorderColour { get; set; }
        //public Color ShadowColour { get; set; } = Color.LightGray;
        //public Color BackgroundColour { get; set; } = Color.Black;
        //public int Shadow { get; set; } = 0;
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
        public static TextMeasure MeasureText(string strText, float kerningShift = 0f) => textLabelFont32.MeasureText(strText, kerningShift);
        public static int FontSize => _fontSize;

        // Draw this label (public wrappers)
        public static void Draw(string strText, Alignment ali) {
            TextRenderOptions tro = new TextRenderOptions() {
                Alignment = ali,
            };
            DrawAtInternal(new List<string>() { strText }, tro);
        }
        public static void Draw(string strText, Alignment ali, Color col) {
            TextRenderOptions tro = new TextRenderOptions() {
                Alignment = ali,
                TextColour = col,
            };
            DrawAtInternal(new List<string>() { strText }, tro);

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
            DrawAtInternal(new List<string>() { strText }, tro);
        }
        public static void DrawWithOptions(string strText, TextRenderOptions tro) {
            DrawAtInternal(new List<string>() { strText }, tro);
        }
        public static void DrawWithOptions(IEnumerable<string> allText, TextRenderOptions tro) {
            DrawAtInternal(allText, tro);
        }

        // Draw this label with the given settings
        private static void DrawAtInternal(IEnumerable<string> allText, TextRenderOptions tro) {
            // Blend this font so we don't obscure the background in the gaps
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

            // Setup the projection so that the screen coordinates are unitised
            Matrix4 projectionM = tro.Projection ?? Matrix4.CreateOrthographicOffCenter(0.0f, 1.0f, 1.0f, 0.0f, -1.0f, 1.0f);
            textLabelShaderProgram.SetUniform("projection", projectionM);

            // Setup the desired font colour in the shader program
            textLabelShaderProgram.SetUniform("textColour", (float)tro.TextColour.R / 255f, (float)tro.TextColour.G / 255f, (float)tro.TextColour.B / 255f);

            // Generate the scale matrix based on the desired scale size, bound size, text size etc.
            float pixelSize = textLabelFont32.PixelSize;
            float yScale = tro.Scale;
            float xScale = tro.Scale / tro.Aspect;
            Matrix4 scaleM = Matrix4.CreateScale(new Vector3(xScale / pixelSize, (tro.FlipY ? -1f : 1f) * yScale / pixelSize, 1.0f));

            // Calculate font location
            float yShift = tro.YPos;
            float xShift = tro.XPos;
            float finalXScale = scaleM.M11;
            float finalYScale = scaleM.M22;

            float longest = 0.0f;
            foreach (string strText in allText) {
                // Get calculated dimensions of the text so we can scale/align accordingly
                TextMeasure textSize = textLabelFont32.MeasureText(strText, tro.KerningShift);
                if (textSize.Width > longest) longest = textSize.Width;
            }
            
            // Change the starting location if text alignment is not TL. We always align to the range between max height and the origin.
            if (tro.Alignment == Alignment.BottomLeft || tro.Alignment == Alignment.BottomMiddle || tro.Alignment == Alignment.BottomRight) yShift -= finalYScale * pixelSize * allText.Count();
            if (tro.Alignment == Alignment.CentreLeft || tro.Alignment == Alignment.CentreMiddle || tro.Alignment == Alignment.CentreRight) yShift -= finalYScale * pixelSize * allText.Count() / 2f;
            if (tro.Alignment == Alignment.TopMiddle || tro.Alignment == Alignment.CentreMiddle || tro.Alignment == Alignment.BottomMiddle) xShift -= finalXScale * longest / 2f;
            if (tro.Alignment == Alignment.TopRight || tro.Alignment == Alignment.CentreRight || tro.Alignment == Alignment.BottomRight) xShift -= finalXScale * longest;

            // Print all lines
            foreach (string strText in allText) {
                // Align to the top left corner of the text
                yShift += pixelSize * finalYScale;

                // Calculate translation to align us correctly and scale to achieve the desired size
                Matrix4 transOriginM = Matrix4.CreateTranslation(new Vector3(xShift, yShift, tro.ZPos));
                Matrix4 viewM = scaleM * transOriginM * (tro.View ?? Matrix4.Identity);
                textLabelShaderProgram.SetUniform("view", viewM);

                // --- Render the actual text aligned at the origin line, using the selected font ---
                textLabelFont32.RenderText(textLabelShaderProgram, strText, tro.KerningShift);
            }

            // Clean up
            GL.UseProgram(0);
            GL.Disable(EnableCap.Blend);
        }
    }
}
