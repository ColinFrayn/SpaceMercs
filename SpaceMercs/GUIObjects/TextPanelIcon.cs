using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using SpaceMercs.Graphics;

namespace SpaceMercs {
    class TextPanelItem : IPanelItem {
    private readonly int texID;
    private readonly float texX, texY, texW, texH;
    private float iconX, iconY, iconW, iconH;
    private int ovTexID = -1;
    private float ovX, ovY, ovW, ovH;
    private float ovTX, ovTY, ovTW, ovTH;
    public uint ID { get; private set; }
    public bool Enabled { get; private set; }
    public float ZDist { get; private set; }
    public GUIPanel? SubPanel { get; private set; }

    public TextPanelItem(string strText, Vector4 iconRect, float zd) {
      iconX = iconRect.X;
      iconY = iconRect.Y;
      iconW = iconRect.Z;
      iconH = iconRect.W;
      texID = 0; // TEMP
      texX = texY = texW = texH = 0f; // TEMP
      ID = 0;
      Enabled = true;
      ZDist = zd;
      SubPanel = null;
      throw new NotImplementedException();
    }
    public void DrawSelectionFrame() {
      GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line);
      GL.Color3(1.0, 1.0, 1.0);
      GL.Begin(BeginMode.Quads);
      GL.Vertex3(iconX, iconY, ZDist + 0.1);
      GL.Vertex3(iconX + iconW, iconY, ZDist + 0.1);
      GL.Vertex3(iconX + iconW, iconY + iconH, ZDist + 0.1);
      GL.Vertex3(iconX, iconY + iconH, ZDist + 0.1);
      GL.End();
      GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);
    }
    public IPanelItem Draw(ShaderProgram prog, double xx, double yy, GUIPanel gpParent) {
      IPanelItem? piHover = null;
      double BorderY = gpParent.BorderY;
      if (xx >= iconX && xx <= iconX + iconW && yy >= iconY - BorderY && yy <= iconY + iconH + BorderY) {
        piHover = this;
      }
      if (Enabled) {
        if (piHover != null) {
          GL.Color3(1.0, 1.0, 1.0);
        }
        else {
          GL.Color3(0.8, 0.8, 0.8);
        }
      }
      else {
        GL.Color3(0.3, 0.3, 0.3);
      }
      // Draw the text string
      GL.Enable(EnableCap.Texture2D);
      GL.BindTexture(TextureTarget.Texture2D, texID);
      GL.Begin(BeginMode.Quads);
      GL.TexCoord2(texX, texY);
      GL.Vertex3(iconX, iconY, ZDist);
      GL.TexCoord2(texX + texW, texY);
      GL.Vertex3(iconX + iconW, iconY, ZDist);
      GL.TexCoord2(texX + texW, texY + texH);
      GL.Vertex3(iconX + iconW, iconY + iconH, ZDist);
      GL.TexCoord2(texX, texY + texH);
      GL.Vertex3(iconX, iconY + iconH, ZDist);
      GL.End();
      if (ovTexID != -1) {
        GL.BindTexture(TextureTarget.Texture2D, ovTexID);
        GL.Begin(BeginMode.Quads);
        GL.TexCoord2(ovTX, ovTY);
        GL.Vertex3(iconX + (iconW * ovX), iconY + (iconH * ovY), ZDist + 0.0001);
        GL.TexCoord2(ovTX + ovTW, ovTY);
        GL.Vertex3(iconX + (iconW * ovX) + (iconW * ovW), iconY + (iconH * ovY), ZDist + 0.0001);
        GL.TexCoord2(ovTX + ovTW, ovTY + ovTH);
        GL.Vertex3(iconX + (iconW * ovX) + (iconW * ovW), iconY + (iconH * ovY) + (iconH * ovH), ZDist + 0.0001);
        GL.TexCoord2(ovTX, ovTY + ovTH);
        GL.Vertex3(iconX + (iconW * ovX), iconY + (iconH * ovY) + (iconH * ovH), ZDist + 0.0001);
        GL.End();
      }
      GL.Disable(EnableCap.Texture2D);

      if (SubPanel != null) {
        throw new Exception("Text Panel shouldn't have sub panel");
      }
      return piHover;
    }
    public void SetPos(float x, float y) {
      iconX = x;
      iconY = y;
    }
    public void SetIconSize(float w, float h) {
      iconW = w;
      iconH = h;
    }
    public void SetSubPanel(GUIPanel gpl) {
      //SubPanel = gpl;
      throw new Exception("Shouldn't be setting sub panels on test icon items");
    }
    public void SetZDist(float zd) {
      ZDist = zd;
    }
    public void SetOverlay(int iOvTexID, Vector4 texRect, Vector4 dimRect) {
      ovTexID = iOvTexID;
      ovTX = texRect.X;
      ovTY = texRect.Y;
      ovTW = texRect.Z;
      ovTH = texRect.W;
      ovX = dimRect.X;
      ovY = dimRect.Y;
      ovW = dimRect.Z;
      ovH = dimRect.W;
    }

  }
}
