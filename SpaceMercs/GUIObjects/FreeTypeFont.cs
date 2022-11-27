using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using SharpFont;
using System.IO;
using System.Reflection;

namespace SpaceMercs {

  public struct Character {
    public int TextureID { get; set; }
    public Vector2 Size { get; set; }
    public Vector2 Bearing { get; set; }
    public int Advance { get; set; }
  }

  public class FreeTypeFont {
    private readonly Dictionary<uint, Character> _characters = new Dictionary<uint, Character>();
    private readonly int _vao;
    private readonly int _vbo;

    public FreeTypeFont(uint pixelheight) {
      Library lib = new Library();
      //SharpFont.Face face = new SharpFont.Face(lib, "GUIObjects/FreeSans.ttf");

      Assembly assembly = this.GetType().GetTypeInfo().Assembly;
      //string[] names = assembly.GetManifestResourceNames();
      Stream? resource_stream = assembly.GetManifestResourceStream("SpaceMercs.GUIObjects.FreeSans.ttf");
      if (resource_stream is null) {
        throw new Exception("Unable to build resource stream");
      }
      MemoryStream ms = new MemoryStream();
      resource_stream.CopyTo(ms);

      // Setup the new font face
      SharpFont.Face face = new SharpFont.Face(lib, ms.ToArray(), 0); // Unknown file format exception

      // ?
      face.SetPixelSizes(0, pixelheight);

      // set 1 byte pixel alignment 
      GL.PixelStore(PixelStoreParameter.UnpackAlignment, 1);

      // set texture unit
      GL.ActiveTexture(TextureUnit.Texture0);

      // Load first 128 characters of ASCII set
      for (uint c = 0; c < 128; c++) {
        try {
          // load glyph
          //face.LoadGlyph(c, LoadFlags.Render, LoadTarget.Normal);
          face.LoadChar(c, LoadFlags.Render, LoadTarget.Normal);
          GlyphSlot glyph = face.Glyph;
          FTBitmap bitmap = glyph.Bitmap;

          // create glyph texture
          int texObj = GL.GenTexture();
          GL.BindTexture(TextureTarget.Texture2D, texObj);
          GL.TexImage2D(TextureTarget.Texture2D, 0,
                        PixelInternalFormat.R8, bitmap.Width, bitmap.Rows, 0,
                        PixelFormat.Red, PixelType.UnsignedByte, bitmap.Buffer);

          // set texture parameters
          GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
          GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
          GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
          GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);

          // add character
          Character ch = new Character();
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

      // bind default texture
      GL.BindTexture(TextureTarget.Texture2D, 0);

      // set default (4 byte) pixel alignment 
      GL.PixelStore(PixelStoreParameter.UnpackAlignment, 4);

      float[] vquad =
      {
          // x   y      u     v    
          0.0f, -1.0f,  0.0f, 0.0f,
          0.0f,  0.0f,  0.0f, 1.0f,
          1.0f,  0.0f,  1.0f, 1.0f,
          0.0f, -1.0f,  0.0f, 0.0f,
          1.0f,  0.0f,  1.0f, 1.0f,
          1.0f, -1.0f,  1.0f, 0.0f
      };

      // Create [Vertex Buffer Object](https://www.khronos.org/opengl/wiki/Vertex_Specification#Vertex_Buffer_Object)
      _vbo = GL.GenBuffer();
      GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);
      int arraySize = 4 * 6 * 4;
      GL.BufferData<float>(BufferTarget.ArrayBuffer, arraySize, vquad, BufferUsageHint.StaticDraw);

      // [Vertex Array Object](https://www.khronos.org/opengl/wiki/Vertex_Specification#Vertex_Array_Object)
      _vao = GL.GenVertexArray();
      GL.BindVertexArray(_vao);
      GL.EnableVertexAttribArray(0);
      GL.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, 4 * 4, 0);
      GL.EnableVertexAttribArray(1);
      GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, 4 * 4, 2 * 4);

      GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
      GL.BindVertexArray(0);
    }

    public void RenderText(string text, float x, float y, float xScale, float yScale, Vector2 dir) {
      GL.ActiveTexture(TextureUnit.Texture0);
      GL.BindVertexArray(_vao);

      float angle_rad = (float)Math.Atan2(dir.Y, dir.X);
      Matrix4 rotateM = Matrix4.CreateRotationZ(angle_rad);
      Matrix4 transOriginM = Matrix4.CreateTranslation(new Vector3(x, y, 0f));

      // Iterate through all characters
      float char_x = 0.0f;
      foreach (char c in text) {
        if (!_characters.ContainsKey(c)) continue;
        Character ch = _characters[c];

        float w = ch.Size.X * xScale;
        float h = ch.Size.Y * yScale;
        float xrel = char_x + ch.Bearing.X * xScale;
        float yrel = (ch.Size.Y - ch.Bearing.Y) * yScale;

        // Now advance cursors for next glyph (note that advance is number of 1/64 pixels)
        char_x += (ch.Advance >> 6) * xScale; // Bitshift by 6 to get value in pixels (2^6 = 64 (divide amount of 1/64th pixels by 64 to get amount of pixels))

        Matrix4 scaleM = Matrix4.CreateScale(new Vector3(w, h, 1.0f));
        Matrix4 transRelM = Matrix4.CreateTranslation(new Vector3(xrel, yrel, 0.0f));

        Matrix4 modelM = scaleM * transRelM * rotateM * transOriginM; // OpenTK `*`-operator is reversed
        GL.UniformMatrix4(0, false, ref modelM);

        // Render glyph texture over quad
        GL.BindTexture(TextureTarget.Texture2D, ch.TextureID);

        // Render quad
        GL.DrawArrays(PrimitiveType.Triangles, 0, 6);
      }

      GL.BindVertexArray(0);
      GL.BindTexture(TextureTarget.Texture2D, 0);
    }
  }
}
