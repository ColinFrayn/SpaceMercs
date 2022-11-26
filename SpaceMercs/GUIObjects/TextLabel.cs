using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using SharpFont;
using System.IO;
using System.Reflection;

// Library from here : https://github.com/space-wizards/SharpFont
// Based on this, updated for .NET : https://github.com/Robmaister/SharpFont
// Example from here : https://github.com/Rabbid76/c_sharp_opengl/tree/master/OpenTK_example_5

namespace SpaceMercs {
  public struct Character {
    public int TextureID { get; set; }
    public Vector2 Size { get; set; }
    public Vector2 Bearing { get; set; }
    public int Advance { get; set; }
  }

  class TextLabel {
    public enum Alignment { TopLeft, TopMiddle, TopRight, CentreLeft, CentreMiddle, CentreRight, BottomLeft, BottomMiddle, BottomRight };
    public enum TextAlign { Left, Centre, Right }
    private readonly Font TextFont = new Font(FontFamily.GenericSansSerif, 24);
    private Bitmap TextBitmap;
    private TextAlign TextPos = TextLabel.TextAlign.Centre;
    private int textureId = -1;
    public int Width { get; private set; }
    public int Height { get; private set; }
    public int Lines { get { return strText.Count; } }
    private readonly List<string> strText;
    private int iBorder;
    private int iPadding;
    private Color cBorderColor;
    private Color cTextColor;
    private Color cShadowColor;
    private Color cBackgroundColor;
    public bool bEnabled { get; set; }
    private int iShadow;

    // NEW SharpFont stuff
    Dictionary<uint, Character> _characters = new Dictionary<uint, Character>();
    int _vao;
    int _vbo;

    // Keep track of previous settings when drawing
    private bool bBlendState, bLightingState, bTextureState, bDepthMaskState;
    private int iBlendSrc, iBlendDest;

    private void NEW_Setup() {
      // initialize library
      Library lib = new Library();

      Assembly assembly = Assembly.GetExecutingAssembly();
      //string[] names = assembly.GetManifestResourceNames();
      Stream resource_stream = assembly.GetManifestResourceStream("OpenTK_example_5.Resource.FreeSans.ttf");
      MemoryStream ms = new MemoryStream();
      resource_stream.CopyTo(ms);
      SharpFont.Face face = new SharpFont.Face(lib, ms.ToArray(), 0);

      //SharpFont.Face face = new SharpFont.Face(lib, "FreeSans.ttf");
      face.SetPixelSizes(0, 32);

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
          GL.TextureParameter(texObj, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
          GL.TextureParameter(texObj, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
          GL.TextureParameter(texObj, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
          GL.TextureParameter(texObj, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);

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
            // x      y      u     v    
                0.0f, -1.0f,   0.0f, 0.0f,
                0.0f,  0.0f,   0.0f, 1.0f,
                1.0f,  0.0f,   1.0f, 1.0f,
                0.0f, -1.0f,   0.0f, 0.0f,
                1.0f,  0.0f,   1.0f, 1.0f,
                1.0f, -1.0f,   1.0f, 0.0f
            };

      // Create [Vertex Buffer Object](https://www.khronos.org/opengl/wiki/Vertex_Specification#Vertex_Buffer_Object)
      _vbo = GL.GenBuffer();
      GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);
      GL.BufferData(BufferTarget.ArrayBuffer, 4 * 6 * 4, vquad, BufferUsageHint.StaticDraw);

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

    public void NEW_RenderText(string text, float x, float y, float scale, Vector2 dir) {
      GL.ActiveTexture(TextureUnit.Texture0);
      GL.BindVertexArray(_vao);

      float angle_rad = (float)Math.Atan2(dir.Y, dir.X);
      Matrix4 rotateM = Matrix4.CreateRotationZ(angle_rad);
      Matrix4 transOriginM = Matrix4.CreateTranslation(new Vector3(x, y, 0f));

      // Iterate through all characters
      float char_x = 0.0f;
      foreach (var c in text) {
        if (_characters.ContainsKey(c) == false)
          continue;
        Character ch = _characters[c];

        float w = ch.Size.X * scale;
        float h = ch.Size.Y * scale;
        float xrel = char_x + ch.Bearing.X * scale;
        float yrel = (ch.Size.Y - ch.Bearing.Y) * scale;

        // Now advance cursors for next glyph (note that advance is number of 1/64 pixels)
        char_x += (ch.Advance >> 6) * scale; // Bitshift by 6 to get value in pixels (2^6 = 64 (divide amount of 1/64th pixels by 64 to get amount of pixels))

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

    public void NEW_ExampleText() {
      Matrix4 projectionM = Matrix4.CreateScale(new Vector3(1f / this.Width, 1f / this.Height, 1.0f));
      projectionM = Matrix4.CreateOrthographicOffCenter(0.0f, this.Width, this.Height, 0.0f, -1.0f, 1.0f);

      GL.ClearColor(0.2f, 0.3f, 0.3f, 1.0f);
      GL.Clear(ClearBufferMask.ColorBufferBit);

      GL.Enable(EnableCap.Blend);
      //GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
      GL.BlendFunc(0, BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);

      //text_prog.Use();
      GL.UniformMatrix4(1, false, ref projectionM);

      GL.Uniform3(2, new Vector3(0.5f, 0.8f, 0.2f));
      NEW_RenderText("This is sample text", 25.0f, 50.0f, 1.2f, new Vector2(1f, 0f));

      GL.Uniform3(2, new Vector3(0.3f, 0.7f, 0.9f));
      NEW_RenderText("More sample text", 50.0f, 200.0f, 0.9f, new Vector2(1.0f, -0.25f));
    }

    public static string TextLabelVertexShader = @"
#version 460

layout (location = 0) in vec2 in_pos;
layout (location = 1) in vec2 in_uv;

out vec2 vUV;

layout (location = 0) uniform mat4 model;
layout (location = 1) uniform mat4 projection;

void main()
{
    vUV         = in_uv.xy;
    gl_Position = projection * model * vec4(in_pos.xy, 0.0, 1.0);
}";
    public static string TextLabelFragmentShader = @"
#version 460

in vec2 vUV;

layout (binding=0) uniform sampler2D u_texture;

  layout (location = 2) uniform vec3 textColor;

out vec4 fragColor;

void main()
{
    vec2 uv = vUV.xy;
    float text = texture(u_texture, uv).r;
    fragColor = vec4(textColor.rgb*text, text);
}";

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
      iBorder = 0;
      iPadding = 5;
      cBorderColor = Color.Black;
      cBackgroundColor = Color.FromArgb(0,0,0,0);
      cTextColor = Color.White;
      cShadowColor = Color.DarkGray;
      TextBitmap = null;
      bEnabled = true;
      iShadow = 0;
      SetupTexture();
    }

    // Change the label text and rebuild the texture
    public void UpdateText(string strNew) {
      if (strText.Equals(strNew)) return;
      strText.Clear();
      strText.Add(strNew);
      SetupTexture();
      bEnabled = true;
    }

    // Update the text on multiple lines
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
      SetupTexture();
    }

    // Update the text using a list of strings
    public void UpdateTextFromList(IEnumerable<string> strListIn) {
      strText.Clear();
      foreach (string str in strListIn) {
        strText.Add(str);
      }
      SetupTexture();
    }

    // Setup the texture with the current text
    private void SetupTexture() {
      bool bTexture = GL.IsEnabled(EnableCap.Texture2D);
      int maxwidth = 0, maxheight = 0;
      Dictionary<string, Size> StringLength = new Dictionary<string, Size>();
      foreach (string str in strText) {
        Size sz = TextRenderer.MeasureText(str, TextFont);
        StringLength.Add(str, sz);
        if (sz.Width > maxwidth) maxwidth = sz.Width;
        if (sz.Height > maxheight) maxheight = sz.Height;
      }
      Width = maxwidth + iBorder * 2 + iPadding * 2 + iShadow - 17; // Apparently the MeasureText function adds in a border of 17 in X
      if (Width < 1) Width = 1;
      int sumheight = maxheight * strText.Count;
      Height = sumheight + iBorder * 2 + iPadding * 2 + iShadow;
      if (Height < 1) Height = 1;
      GL.DeleteTexture(textureId);
      if (TextBitmap != null) TextBitmap.Dispose();
      TextBitmap = new Bitmap(Width, Height);
      GL.Enable(EnableCap.Texture2D);
      textureId = GL.GenTexture();
      GL.BindTexture(TextureTarget.Texture2D, textureId);
      GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
      GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
      GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
      GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);
      // Just allocate memory, so we can update efficiently using TexSubImage2D
      GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, Width, Height, 0, OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, IntPtr.Zero); // Mem allocated but not freed?

      // Draw the texture
      using (System.Drawing.Graphics gfx = System.Drawing.Graphics.FromImage(TextBitmap)) {
        StringFormat sFormat = new StringFormat(StringFormat.GenericTypographic);
        // Clear to the background color
        gfx.Clear(cBackgroundColor);
        // Draw the required text
        //gfx.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
        gfx.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAliasGridFit;
        Brush sb = new SolidBrush(cShadowColor);
        int index = 0;
        foreach (string str in strText) {
          Size sz = StringLength[str];
          int xpos = iBorder + iPadding;
          if (TextPos == TextAlign.Centre) {
            xpos = (Width - (sz.Width + iShadow - 17)) / 2;
          }
          if (TextPos == TextAlign.Right) {
            xpos = Width - (iBorder + iPadding + iShadow) - (sz.Width-17);
          }
          if (iShadow > 0) {
            for (int i = 0; i < iShadow; i++) {
              gfx.DrawString(str, TextFont, sb, (float)(xpos + i), (float)(iBorder + iPadding + i) + (float)(maxheight * index), sFormat);
            }
          }
          Brush b = new SolidBrush(cTextColor);
          gfx.DrawString(str, TextFont, b, (float)xpos, (float)(iBorder + iPadding) + (float)(maxheight * index), sFormat);
          index++;
        }
        // Add a border
        if (iBorder > 0) {
          Pen p = new Pen(cBorderColor, iBorder);
          p.Alignment = System.Drawing.Drawing2D.PenAlignment.Inset;
          gfx.DrawRectangle(p, 0, 0, Width-1, Height-1);
        }
        if (!bTexture) GL.Disable(EnableCap.Texture2D);
      }

      // Set this image as the texture
      System.Drawing.Imaging.BitmapData data = TextBitmap.LockBits(new Rectangle(0, 0, Width, Height), System.Drawing.Imaging.ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

      GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, Width, Height, 0, OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0); // Mem allocated but not freed?
      TextBitmap.UnlockBits(data);
    }

    // Delete texture memory
    public void Dispose() {
      if (textureId != -1) {
        GL.DeleteTexture(textureId);
      }
      textureId = -1;
    }

    // Set up for drawing
    private void SetupDrawing() {
      bBlendState = GL.IsEnabled(EnableCap.Blend);
      bLightingState = GL.IsEnabled(EnableCap.Lighting);
      bTextureState = GL.IsEnabled(EnableCap.Texture2D);
      GL.GetBoolean(GetPName.DepthWritemask, out bDepthMaskState);
      GL.GetInteger(GetPName.BlendSrc, out iBlendSrc);
      GL.GetInteger(GetPName.BlendDst, out iBlendDest);

      // Set up blending
      GL.Enable(EnableCap.Blend);
      GL.DepthMask(false);
      GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

      // Set up other params
      GL.Disable(EnableCap.Lighting);
      GL.Enable(EnableCap.Texture2D);
      GL.BindTexture(TextureTarget.Texture2D, textureId);
      GL.Color4(1.0, 1.0, 1.0, 1.0);
    }

    // Put back rendering state as it was before
    private void ResetDrawing() {
      if (bBlendState) GL.Enable(EnableCap.Blend);
      else GL.Disable(EnableCap.Blend);

      if (bLightingState) GL.Enable(EnableCap.Lighting);
      else GL.Disable(EnableCap.Lighting);

      if (bTextureState) GL.Enable(EnableCap.Texture2D);
      else GL.Disable(EnableCap.Texture2D);

      GL.DepthMask(bDepthMaskState);

      GL.BlendFunc((BlendingFactor)iBlendSrc, (BlendingFactor)iBlendDest);
    }

    // Draw this label
    public void Draw(Alignment ali) {
      DrawAtInternal(ali, 1.0 / (double)TextBitmap.Height, 1.0 / (double)TextBitmap.Height, 0, 0);
    }
    public void DrawAt(Alignment ali, int xshift, int yshift) {
      DrawAtInternal(ali, 1.0 / (double)TextBitmap.Height, 1.0 / (double)TextBitmap.Height, xshift, yshift);
    }
    public void Draw(Alignment ali, double dwidth, double dheight) {
      DrawAtInternal(ali, dwidth / (double)TextBitmap.Width, dheight / (double)TextBitmap.Height, 0, 0);
    }
    public void Draw(Alignment ali, double dwidth, double dheight, int xshift, int yshift) {
      DrawAtInternal(ali, dwidth / (double)TextBitmap.Width, dheight / (double)TextBitmap.Height, xshift, yshift);
    }
    private void DrawAtInternal(Alignment ali, double dXScale, double dYScale, int xshift, int yshift) {
      if (!bEnabled) return;

      SetupDrawing();

      if (ali == Alignment.BottomLeft || ali == Alignment.BottomMiddle || ali == Alignment.BottomRight) yshift = -TextBitmap.Height;
      if (ali == Alignment.CentreLeft || ali == Alignment.CentreMiddle || ali == Alignment.CentreRight) yshift = -TextBitmap.Height / 2;
      if (ali == Alignment.TopMiddle || ali == Alignment.CentreMiddle || ali == Alignment.BottomMiddle) xshift = -TextBitmap.Width / 2;
      if (ali == Alignment.TopRight || ali == Alignment.CentreRight || ali == Alignment.BottomRight) xshift = -TextBitmap.Width;

      if (Const.DEBUG_TEXTLABEL_RECTANGLES) GL.Disable(EnableCap.Texture2D);
      GL.PushMatrix();
      GL.Rotate(180.0, Vector3d.UnitX);
      GL.Scale(dXScale, dYScale, 1.0);
      GL.Begin(BeginMode.Quads);
      GL.Color3(1.0, 1.0, 1.0);
      GL.TexCoord2(0, 0); GL.Vertex2(xshift, yshift);
      GL.TexCoord2(1, 0); GL.Vertex2(TextBitmap.Width + xshift, yshift);
      GL.TexCoord2(1, 1); GL.Vertex2(TextBitmap.Width + xshift, TextBitmap.Height + yshift);
      GL.TexCoord2(0, 1); GL.Vertex2(xshift, TextBitmap.Height + yshift);
      GL.End();
      GL.PopMatrix();

      ResetDrawing();
    }

    // Configure the text
    public void SetShadow(int s) {
      iShadow = s;
      SetupTexture();
    }
    public void SetBorder(int b, Color bcol) {
      iBorder = b;
      cBorderColor = bcol;
      SetupTexture();
    }
    public void SetPadding(int p) {
      iPadding = p;
      SetupTexture();
    }
    public void SetTextAlign(TextAlign ta) {
      if (ta == TextPos) return;
      TextPos = ta;
      SetupTexture();
    }
    public void SetBorderColor(Color col) {
      cBorderColor = col;
      SetupTexture();
    }
    public void SetTextColor(Color col) {
      cTextColor = col;
      SetupTexture();
    }
    public void SetBackgroundColor(Color col) {
      cBackgroundColor = col;
      SetupTexture();
    }
    public void SetShadowColor(Color col) {
      cShadowColor = col;
      SetupTexture();
    }
  }
}
