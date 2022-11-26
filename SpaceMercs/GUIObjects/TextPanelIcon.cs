using System;
using OpenTK.Mathematics;
using OpenTK.Graphics.OpenGL;

namespace SpaceMercs {
  class TextPanelItem : IPanelItem {
    private readonly int texID;
    private readonly double texX, texY, texW, texH;
    private double iconX, iconY, iconW, iconH;
    private int ovTexID = -1;
    private double ovX, ovY, ovW, ovH;
    private double ovTX, ovTY, ovTW, ovTH;
    public uint ID { get; private set; }
    public bool Enabled { get; private set; }
    public double ZDist { get; private set; }
    public GUIPanel SubPanel { get; private set; }

    public TextPanelItem(string strText, Vector4d iconRect, double zd) {
      iconX = iconRect.X;
      iconY = iconRect.Y;
      iconW = iconRect.Z;
      iconH = iconRect.W;
      texID = 0; // TEMP
      texX = texY = texW = texH = 0.0; // TEMP
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
    public IPanelItem Draw(double xx, double yy, GUIPanel gpParent) {
      IPanelItem piHover = null;
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
    public void SetPos(double x, double y) {
      iconX = x;
      iconY = y;
    }
    public void SetIconSize(double w, double h) {
      iconW = w;
      iconH = h;
    }
    public void SetSubPanel(GUIPanel gpl) {
      //SubPanel = gpl;
      throw new Exception("Shouldn't be setting sub panels on test icon items");
    }
    public void SetZDist(double zd) {
      ZDist = zd;
    }
    public void SetOverlay(int iOvTexID, Vector4d texRect, Vector4d dimRect) {
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
