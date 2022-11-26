using System;
using System.Drawing;
using OpenTK.Mathematics;
using OpenTK.Graphics.OpenGL;

namespace SpaceMercs {
  class IconPanelItem : IPanelItem {
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

    public IconPanelItem(int _texID, Vector4d texRect, Vector4d iconRect, bool _Enabled, uint _ID, double zd) {
      texID = _texID;
      texX = texRect.X;
      texY = texRect.Y;
      texW = texRect.Z;
      texH = texRect.W;
      iconX = iconRect.X;
      iconY = iconRect.Y;
      iconW = iconRect.Z;
      iconH = iconRect.W;
      ID = _ID;
      Enabled = _Enabled;
      ZDist = zd;
      SubPanel = null;
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
      // Draw the icon
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
      if (ovTexID !=-1) {
        GL.BindTexture(TextureTarget.Texture2D, ovTexID);
        GL.Begin(BeginMode.Quads);
        GL.TexCoord2(ovTX, ovTY);
        GL.Vertex3(iconX + (iconW * ovX), iconY + (iconH*ovY), ZDist + 0.0001);
        GL.TexCoord2(ovTX + ovTW, ovTY);
        GL.Vertex3(iconX + (iconW * ovX) + (iconW*ovW), iconY + (iconH * ovY), ZDist + 0.0001);
        GL.TexCoord2(ovTX + ovTW, ovTY + ovTH);
        GL.Vertex3(iconX + (iconW * ovX) + (iconW * ovW), iconY + (iconH * ovY) + (iconH * ovH), ZDist + 0.0001);
        GL.TexCoord2(ovTX, ovTY + ovTH);
        GL.Vertex3(iconX + (iconW * ovX), iconY + (iconH * ovY) + (iconH * ovH), ZDist + 0.0001);
        GL.End();
      }
      GL.Disable(EnableCap.Texture2D);

      if (SubPanel != null) {
        // If this item hovered then open subpanel
        if (piHover != null) {
          SubPanel.Activate();
        }

        // Draw subpanel if it's active
        if (SubPanel.Active) {
          if (iconY > 0.5) {
            SubPanel.SetPosition(iconX, iconY - (SubPanel.Height + BorderY * 2.0));
          }
          else {
            SubPanel.SetPosition(iconX, iconY + iconH + BorderY * 2.0);
          }
          IPanelItem piHover2 = SubPanel.Display2(xx, yy);
          if (piHover2 != null) piHover = piHover2;
          // If subpanel is active but not hovered then close it, unless we're hovering on empty squares
          if (piHover == null) {
            if (!SubPanel.IsHover(xx, yy)) {
              SubPanel.Deactivate();
            }
          }
        }
        // Otherwise draw an arrow indicating that subpanel exists
        else {
          GL.Color3(0.8, 0.8, 0.8);
          GL.Begin(BeginMode.Triangles);
          if (iconY > 0.5) {
            GL.Vertex3(iconX + (iconW * 0.35), iconY + (iconH * 0.15), ZDist + 0.01);
            GL.Vertex3(iconX + (iconW * 0.65), iconY + (iconH * 0.15), ZDist + 0.01);
            GL.Vertex3(iconX + (iconW * 0.5), iconY - (iconH * 0.15), ZDist + 0.01);
          }
          else {
            GL.Vertex3(iconX + (iconW * 0.35), iconY + (iconH * 0.85), ZDist + 0.01);
            GL.Vertex3(iconX + (iconW * 0.65), iconY + (iconH * 0.85), ZDist + 0.01);
            GL.Vertex3(iconX + (iconW * 0.5), iconY + (iconH * 1.15), ZDist + 0.01);
          }
          GL.End();
        }
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
      SubPanel = gpl;
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
