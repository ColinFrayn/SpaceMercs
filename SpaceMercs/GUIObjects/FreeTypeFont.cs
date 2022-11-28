using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using SharpFont;
using SpaceMercs.Graphics;
using System.IO;
using System.Reflection;

namespace SpaceMercs {

  public struct Character {
    public int TextureID { get; set; }
    public Vector2 Size { get; set; }
    public Vector2 Bearing { get; set; }
    public int Advance { get; set; }
  }

  internal class FreeTypeFont {
    private readonly Dictionary<uint, Character> _characters = new Dictionary<uint, Character>();
    private readonly VertexBuffer vertexBuffer;
    private readonly IndexBuffer indexBuffer;
    private readonly VertexArray vertexArray;
    public uint PixelHeight;

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

      // set 1 byte pixel alignment 
      GL.PixelStore(PixelStoreParameter.UnpackAlignment, 1);

      // set texture unit
      GL.Enable(EnableCap.Texture2D);
      GL.ActiveTexture(TextureUnit.Texture0);

      // Load the useful characters of ASCII set, skipping the functional chars at the beginning
      for (uint c = 32; c < 127; c++) {
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
      GL.Disable(EnableCap.Texture2D);

      // set default (4 byte) pixel alignment 
      GL.PixelStore(PixelStoreParameter.UnpackAlignment, 4);

      VertexPos2DTex[] vertices = new VertexPos2DTex[4];
      int[] indices = new int[6];
      vertices[0] = new VertexPos2DTex(new Vector2(0f, -1f), new Vector2(0f,0f));
      vertices[1] = new VertexPos2DTex(new Vector2(0f, 0f), new Vector2(0f,1f));
      vertices[2] = new VertexPos2DTex(new Vector2(1f, 0f), new Vector2(1f,1f));
      vertices[3] = new VertexPos2DTex(new Vector2(1f, -1f), new Vector2(1f,0f));
      indices[0] = 0;
      indices[1] = 1;
      indices[2] = 2;
      indices[3] = 0;
      indices[4] = 2;
      indices[5] = 3;

      vertexBuffer = new VertexBuffer(VertexPos2DTex.VertexInfo, vertices.Length);
      vertexBuffer.SetData(vertices);
      indexBuffer = new IndexBuffer(indices.Length);
      indexBuffer.SetData(indices);
      vertexArray = new VertexArray(vertexBuffer);
    }

    public void RenderText(ShaderProgram prog, string text, float x, float y, float xScale, float yScale) {
      //GL.Enable(EnableCap.Texture2D);
      //GL.ActiveTexture(TextureUnit.Texture0);
      GL.UseProgram(prog.ShaderProgramHandle);
      GL.BindVertexArray(vertexArray.VertexArrayHandle);
      GL.BindBuffer(BufferTarget.ElementArrayBuffer, indexBuffer.IndexBufferHandle);

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

        Matrix4 modelM = scaleM * transRelM * transOriginM;
        prog.SetUniform("model", modelM);

        // Render glyph texture over quad
        //GL.BindTexture(TextureTarget.Texture2D, ch.TextureID);

        // Render quad
        GL.DrawElements(PrimitiveType.Triangles, indexBuffer.IndexCount, DrawElementsType.UnsignedInt, 0);
        break; // DEBUG
      }

      GL.BindVertexArray(0);
      GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
      GL.BindTexture(TextureTarget.Texture2D, 0);
      //GL.Disable(EnableCap.Texture2D);
    }
  }
}
