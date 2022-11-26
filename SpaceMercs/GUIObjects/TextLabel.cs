using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using System.Drawing.Imaging;

namespace SpaceMercs {
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

    // Keep track of previous settings when drawing
    private bool bBlendState, bLightingState, bTextureState, bDepthMaskState;
    private int iBlendSrc, iBlendDest;

    // Set up the text label
    public TextLabel() {
      strText = new List<string>();
      strText.Add(" ");
      SetupTextLabel();
    }
    public TextLabel(string strIn) {
      strText = new List<string>();
      strText.Add(strIn);
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
    public void UpdateTextMaxLine(string strNew, int maxlen) {
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

    // Setup the texture
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
      BitmapData data = TextBitmap.LockBits(new Rectangle(0, 0, Width, Height), ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

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
