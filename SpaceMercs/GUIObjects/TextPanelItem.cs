using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using SpaceMercs.Graphics;

namespace SpaceMercs {
    class TextPanelItem : PanelItem {
        private readonly TextMeasure measure;

        public TextPanelItem(string strText, uint _id) : base(null, true, _id) {
            measure = TextRenderer.MeasureText(strText);
        }

        public override PanelItem Draw(ShaderProgram prog, double xx, double yy, GUIPanel gpParent, Vector2 itemPos, Vector2 itemSize, float zdist) {
            PanelItem? piHover = null;
            double BorderY = gpParent.BorderY;
            float iconX = itemPos.X;
            float iconY = itemPos.Y;
            float iconW = itemSize.X;
            float iconH = itemSize.Y;
            if (xx >= iconX && xx <= iconX + iconW && yy >= iconY - BorderY && yy <= iconY + iconH + BorderY) {
                piHover = this;
            }
            
            // TODO

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
            GL.Vertex3(iconX, iconY, zdist);
            GL.TexCoord2(texX + texW, texY);
            GL.Vertex3(iconX + iconW, iconY, zdist);
            GL.TexCoord2(texX + texW, texY + texH);
            GL.Vertex3(iconX + iconW, iconY + iconH, zdist);
            GL.TexCoord2(texX, texY + texH);
            GL.Vertex3(iconX, iconY + iconH, zdist);
            GL.End();
            if (ovTexID != -1) {
                GL.BindTexture(TextureTarget.Texture2D, ovTexID);
                GL.Begin(BeginMode.Quads);
                GL.TexCoord2(ovTX, ovTY);
                GL.Vertex3(iconX + (iconW * ovX), iconY + (iconH * ovY), zdist + 0.0001);
                GL.TexCoord2(ovTX + ovTW, ovTY);
                GL.Vertex3(iconX + (iconW * ovX) + (iconW * ovW), iconY + (iconH * ovY), zdist + 0.0001);
                GL.TexCoord2(ovTX + ovTW, ovTY + ovTH);
                GL.Vertex3(iconX + (iconW * ovX) + (iconW * ovW), iconY + (iconH * ovY) + (iconH * ovH), zdist + 0.0001);
                GL.TexCoord2(ovTX, ovTY + ovTH);
                GL.Vertex3(iconX + (iconW * ovX), iconY + (iconH * ovY) + (iconH * ovH), zdist + 0.0001);
                GL.End();
            }
            GL.Disable(EnableCap.Texture2D);

            if (SubPanel != null) {
                throw new Exception("Text Panel shouldn't have sub panel");
            }
            if (piHover != null) DrawSelectionFrame(prog, iconX, iconY, iconW, iconH, zdist);
            return piHover;
        }
        public override void SetSubPanel(GUIPanel? gpl) {
            SubPanel = gpl;
        }
        public override void SetOverlay(TexSpecs ts, Vector4 dimRect) {
            ovTexID = ts.ID;
            ovTX = ts.X;
            ovTY = ts.Y;
            ovTW = ts.W;
            ovTH = ts.H;
            ovX = dimRect.X;
            ovY = dimRect.Y;
            ovW = dimRect.Z;
            ovH = dimRect.W;
        }
        public override float Width(float tw, float th) {
            return measure.Width * th / ((th/tw) * TextRenderer.FontSize);
        }
        public override float Height(float tw, float th) {
            return th;
        }
    }
}