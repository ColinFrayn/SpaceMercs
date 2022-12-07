using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using SharpFont;
using SpaceMercs.Graphics;
using SpaceMercs.Graphics.Shapes;
using System.IO;
using System.Reflection;

namespace SpaceMercs {

    public struct TexChar {
        public int TextureID { get; set; }
        public Vector2 Size { get; set; }
        public Vector2 Bearing { get; set; }
        public int Advance { get; set; }
    }

    public struct TextMeasure {
        public float Width;
        public float Height;
        public float MaxYBearing;
        public float MaxYDrop;
    }

    internal class FreeTypeFont {
        private readonly Dictionary<uint, TexChar> _characters = new Dictionary<uint, TexChar>();
        public uint PixelSize { get; private set; }
        public uint PixelHeight { get; private set; }

        public FreeTypeFont(uint pixelheight) {
            PixelSize = pixelheight;

            Library lib = new Library();

            Assembly assembly = this.GetType().GetTypeInfo().Assembly;
            //string[] names = assembly.GetManifestResourceNames();
            Stream? resource_stream = assembly.GetManifestResourceStream("SpaceMercs.GUIObjects.FreeSans.ttf");
            if (resource_stream is null) {
                throw new Exception("Unable to build resource stream");
            }
            MemoryStream ms = new MemoryStream();
            resource_stream.CopyTo(ms);

            // Setup the new font face
            SharpFont.Face face = new SharpFont.Face(lib, ms.ToArray(), 0);

            // Set the font scale
            face.SetPixelSizes(0, PixelSize);

            // Set 1 byte pixel alignment 
            GL.PixelStore(PixelStoreParameter.UnpackAlignment, 1);

            // Set texture unit
            GL.ActiveTexture(TextureUnit.Texture0);

            // Load the useful characters of ASCII set, skipping the functional chars at the beginning
            for (uint c = 32; c < 127; c++) {
                try {
                    // Load glyph
                    face.LoadChar(c, LoadFlags.Render, LoadTarget.Normal);
                    GlyphSlot glyph = face.Glyph;
                    FTBitmap bitmap = glyph.Bitmap;

                    // Create glyph texture
                    int texObj = GL.GenTexture();
                    GL.BindTexture(TextureTarget.Texture2D, texObj);
                    GL.TexImage2D(TextureTarget.Texture2D, 0,
                                  PixelInternalFormat.R8, bitmap.Width, bitmap.Rows, 0,
                                  PixelFormat.Red, PixelType.UnsignedByte, bitmap.Buffer);

                    // Set texture parameters
                    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
                    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
                    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
                    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);

                    // Add character
                    TexChar ch = new TexChar();
                    ch.TextureID = texObj;
                    ch.Size = new Vector2(bitmap.Width, bitmap.Rows);
                    ch.Bearing = new Vector2(glyph.BitmapLeft, glyph.BitmapTop);
                    ch.Advance = (int)glyph.Advance.X.Value;
                    _characters.Add(c, ch);
                    if (glyph.BitmapTop > PixelHeight) PixelHeight = (uint)glyph.BitmapTop;
                }
                catch (Exception ex) {
                    Console.WriteLine(ex);
                }
            }

            // Bind default texture
            GL.BindTexture(TextureTarget.Texture2D, 0);

            // Set default (4 byte) pixel alignment 
            GL.PixelStore(PixelStoreParameter.UnpackAlignment, 4);
        }

        // Return a translation offset to be used to get the alignment / scaling right
        public TextMeasure MeasureText(string text, float kerningShift = 0f) {
            float char_x = 0f, maxBearing = 0f, maxDrop = 0f;
            foreach (char c in text) {
                TexChar ch = _characters['?']; // Unprintable character => ?
                if (_characters.ContainsKey(c)) { ch = _characters[c]; }  // Try to get the correct character

                // Now advance cursors for next glyph (note that advance is number of 1/64 pixels)
                if (char_x > 0) char_x += kerningShift;
                char_x += (ch.Advance >> 6); // Bitshift by 6 to get value in pixels (2^6 = 64 (divide amount of 1/64th pixels by 64 to get amount of pixels))

                if (ch.Bearing.Y > maxBearing) maxBearing = ch.Bearing.Y;
                float drop = ch.Size.Y - ch.Bearing.Y;
                if (drop > maxDrop) maxDrop = drop;
            }
            return new TextMeasure() { Width = char_x, Height = maxBearing + maxDrop, MaxYBearing = maxBearing, MaxYDrop = maxDrop };
        }

        public void RenderText(ShaderProgram prog, string text, float kerningShift = 0f) {
            GL.ActiveTexture(TextureUnit.Texture0);

            // Iterate through all characters
            float char_x = 0.0f;
            foreach (char c in text) {
                TexChar ch = _characters['?']; // Unprintable character => ?
                if (_characters.ContainsKey(c)) { ch = _characters[c]; }  // Try to get the correct character

                float w = ch.Size.X;
                float h = ch.Size.Y;
                float xrel = char_x + ch.Bearing.X;
                float yrel = -ch.Bearing.Y;

                // Transform to local coords & scale to draw this glyph at the orgin line with the correct spacing
                Matrix4 scaleM = Matrix4.CreateScale(new Vector3(w, h, 1.0f));
                Matrix4 transRelM = Matrix4.CreateTranslation(new Vector3(xrel, yrel, 0.0f));
                Matrix4 modelM = scaleM * transRelM;
                prog.SetUniform("model", modelM);

                // Render glyph texture over quad
                GL.BindTexture(TextureTarget.Texture2D, ch.TextureID);

                // Render quad
                GL.UseProgram(prog.ShaderProgramHandle);
                Square.Textured.Bind();
                Square.Textured.Draw();

                // Now advance cursors for next glyph (note that advance is number of 1/64 pixels)
                if (char_x > 0) char_x += kerningShift;
                char_x += (ch.Advance >> 6); // Bitshift by 6 to get value in pixels (2^6 = 64 (divide amount of 1/64th pixels by 64 to get amount of pixels))
            }

            Square.Textured.Unbind();
            GL.BindTexture(TextureTarget.Texture2D, 0);
            GL.UseProgram(0);
        }
    }
}
