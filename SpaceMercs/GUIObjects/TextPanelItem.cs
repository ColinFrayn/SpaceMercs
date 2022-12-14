using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using SpaceMercs.Graphics;

namespace SpaceMercs {
    class TextPanelItem : PanelItem {

        public TextPanelItem(string strText, Vector4 iconRect, float zd) : base(0, null, iconRect, true, 0, zd) {
            throw new NotImplementedException();
        }

        public override PanelItem Draw(ShaderProgram prog, double xx, double yy, GUIPanel gpParent) {
            PanelItem? piHover = null;
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
        public override void SetPos(float x, float y) {
            iconX = x;
            iconY = y;
        }
        public override void SetIconSize(float w, float h) {
            iconW = w;
            iconH = h;
        }
        public override void SetSubPanel(GUIPanel gpl) {
            //SubPanel = gpl;
            throw new Exception("Shouldn't be setting sub panels on test icon items");
        }
        public override void SetZDist(float zd) {
            ZDist = zd;
        }
        public override void SetOverlay(int iOvTexID, Vector4 texRect, Vector4 dimRect) {
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