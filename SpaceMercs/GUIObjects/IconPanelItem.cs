using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using SpaceMercs.Graphics;
using SpaceMercs.Graphics.Shapes;

namespace SpaceMercs {
    class IconPanelItem : PanelItem {

        public IconPanelItem(int _texID, Vector4 texRect, Vector4 iconRect, bool _enabled, uint _ID, float zd) : base(_texID, texRect, iconRect, _enabled, _ID, zd) {
            // Nothing to see here
        }

        public override PanelItem? Draw(ShaderProgram prog, double xx, double yy, GUIPanel gpParent) {
            PanelItem? piHover = null;
            float BorderY = gpParent.BorderY;

            if (xx >= iconX && xx <= iconX + iconW && yy >= iconY - BorderY && yy <= iconY + iconH + BorderY) {
                piHover = this;
            }
            if (Enabled) {
                if (piHover != null) {
                    prog.SetUniform("flatColour", new Vector4(1f, 1f, 1f, 1f));
                }
                else {
                    prog.SetUniform("flatColour", new Vector4(0.8f, 0.8f, 0.8f, 1f));
                }
            }
            else {
                prog.SetUniform("flatColour", new Vector4(0.3f, 0.3f, 0.3f, 1f));
            }

            // Draw the icon
            GL.Disable(EnableCap.Blend);
            GL.Disable(EnableCap.DepthTest);
            GL.BindTexture(TextureTarget.Texture2D, texID);
            GL.DepthMask(false);
            prog.SetUniform("textureEnabled", true);
            prog.SetUniform("lightEnabled", false);
            prog.SetUniform("texPos", texX, texY);
            prog.SetUniform("texScale", texW, texH);

            Matrix4 translateM = Matrix4.CreateTranslation(iconX, iconY, ZDist);
            Matrix4 scaleM = Matrix4.CreateScale(texW, texH, 1f);
            Matrix4 modelM = scaleM * translateM;
            prog.SetUniform("model", modelM);
            GL.UseProgram(prog.ShaderProgramHandle);
            Square.Flat.BindAndDraw();

            if (ovTexID != -1) {
                prog.SetUniform("texPos", ovTX, ovTY);
                prog.SetUniform("texScale", ovTW, ovTH);
                GL.BindTexture(TextureTarget.Texture2D, ovTexID);
                translateM = Matrix4.CreateTranslation(iconX + (iconW * ovX), iconY + (iconH * ovY), ZDist + 0.001f);
                scaleM = Matrix4.CreateScale(iconW * ovW, iconH * ovH, 1f);
                modelM = scaleM * translateM;
                prog.SetUniform("model", modelM);
                GL.UseProgram(prog.ShaderProgramHandle);
                Square.Flat.BindAndDraw();
            }
            prog.SetUniform("textureEnabled", false);

            if (SubPanel is not null) {
                // If this item hovered then open subpanel
                if (piHover is not null) {
                    SubPanel.Activate();
                }

                // Draw subpanel if it's active
                if (SubPanel.Active) {
                    if (iconY > 0.5) {
                        SubPanel.SetPosition(iconX, iconY - (SubPanel.Height + BorderY * 2f));
                    }
                    else {
                        SubPanel.SetPosition(iconX, iconY + iconH + BorderY * 2f);
                    }
                    PanelItem? piHover2 = SubPanel.DisplayNormalisedCoords(prog, xx, yy);
                    if (piHover2 is not null) piHover = piHover2;
                    // If subpanel is active but not hovered then close it, unless we're hovering on empty squares
                    if (piHover is null) {
                        if (!SubPanel.IsHover(xx, yy)) {
                            SubPanel.Deactivate();
                        }
                    }
                }
                // Otherwise draw an arrow indicating that subpanel exists
                else {
                    prog.SetUniform("textureEnabled", false);
                    prog.SetUniform("flatColour", new Vector4(0.8f, 0.8f, 0.8f, 1f));
                    scaleM = Matrix4.CreateScale(iconW * 0.3f, iconH * 0.3f, 1f);
                    if (iconY > 0.5) {
                        translateM = Matrix4.CreateTranslation(iconX + (iconW * 0.5f), iconY + (iconH * 0.15f), ZDist + 0.01f);
                        modelM = scaleM * translateM;
                        prog.SetUniform("model", modelM);
                        Triangle.Flat.BindAndDraw();
                    }
                    else {
                        translateM = Matrix4.CreateTranslation(iconX + (iconW * 0.5f), iconY + (iconH * 1.0f), ZDist + 0.01f);
                        modelM = scaleM * translateM;
                        prog.SetUniform("model", modelM);
                        Triangle.Flat.BindAndDraw();
                    }
                }
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
            SubPanel = gpl;
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
