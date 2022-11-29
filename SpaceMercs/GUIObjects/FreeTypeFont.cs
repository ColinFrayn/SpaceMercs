using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using SharpFont;
using SpaceMercs.Graphics;
using SpaceMercs.Graphics.Shapes;
using System.IO;
using System.Reflection;
using System.Windows;

namespace SpaceMercs {

  public struct TexChar {
    public int TextureID { get; set; }
    public Vector2 Size { get; set; }
    public Vector2 Bearing { get; set; }
    public int Advance { get; set; }
  }

  internal class FreeTypeFont {
    private readonly Dictionary<uint, TexChar> _characters = new Dictionary<uint, TexChar>();
    private readonly GLShape square;

    public uint PixelHeight { get; private set; }

    public FreeTypeFont(uint pixelheight) {
      PixelHeight = pixelheight;

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
      face.SetPixelSizes(0, PixelHeight);

      // Set 1 byte pixel alignment 
      GL.PixelStore(PixelStoreParameter.UnpackAlignment, 1);

      // Set texture unit
      //GL.Enable(EnableCap.Texture2D);
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
        }
        catch (Exception ex) {
          Console.WriteLine(ex);
        }
      }

      // Bind default texture
      GL.BindTexture(TextureTarget.Texture2D, 0);
      //GL.Disable(EnableCap.Texture2D);

      // Set default (4 byte) pixel alignment 
      GL.PixelStore(PixelStoreParameter.UnpackAlignment, 4);

      square = Square.BuildTextured(Alignment.TopLeft);
      //square = Square.BuildMultiColoured(Alignment.TopLeft);
    }

    // Return a translation offset to be used to get the alignment / scaling right
    public Vector2 MeasureText(string text) {
      float char_x = 0.0f;
      foreach (char c in text) {
        TexChar ch = _characters['?']; // Unprintable character => ?
        if (_characters.ContainsKey(c)) { ch = _characters[c]; }  // Try to get the correct character
        // Now advance cursors for next glyph (note that advance is number of 1/64 pixels)
        char_x += (ch.Advance >> 6); // Bitshift by 6 to get value in pixels (2^6 = 64 (divide amount of 1/64th pixels by 64 to get amount of pixels))
      }
      float height = _characters['A'].Size.Y;
      return new Vector2(char_x, height);
    }

    public void RenderText(ShaderProgram prog, string text) {
      GL.ActiveTexture(TextureUnit.Texture0);
  
      // Iterate through all characters
      float char_x = 0.0f;
      foreach (char c in text) {
        TexChar ch = _characters['?']; // Unprintable character => ?
        if (_characters.ContainsKey(c)) { ch = _characters[c]; }  // Try to get the correct character

        float w = ch.Size.X;
        float h = ch.Size.Y;
        float xrel = char_x + ch.Bearing.X;
        float yrel = (ch.Size.Y - ch.Bearing.Y);

        // Now advance cursors for next glyph (note that advance is number of 1/64 pixels)
        char_x += (ch.Advance >> 6); // Bitshift by 6 to get value in pixels (2^6 = 64 (divide amount of 1/64th pixels by 64 to get amount of pixels))

        Matrix4 scaleM = Matrix4.CreateScale(new Vector3(w, h, 1.0f));
        Matrix4 transRelM = Matrix4.CreateTranslation(new Vector3(xrel, yrel, 0.0f));

        Matrix4 modelM = scaleM * transRelM;
        prog.SetUniform("model", modelM);

        // Render glyph texture over quad
        GL.BindTexture(TextureTarget.Texture2D, ch.TextureID);

        // Render quad
        GL.UseProgram(prog.ShaderProgramHandle);
        square.Bind();
        square.Draw();
      }

      square.Unbind();
      GL.BindTexture(TextureTarget.Texture2D, 0);
      GL.UseProgram(0);
    }
  }
}
