using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using SpaceMercs.Graphics;
using SpaceMercs.Graphics.Shapes;

namespace SpaceMercs {
    class IconPanelItem : PanelItem {
        public IconPanelItem(TexSpecs ts, bool _enabled, uint _ID, bool togglable) : base(ts, _enabled, _ID, togglable) {
        }

        public override PanelItem? Draw(ShaderProgram prog, double xx, double yy, GUIPanel gpParent, Vector2 itemPos, Vector2 itemSize, float zdist, float aspect) {
            PanelItem? piHover = null;
            float BorderY = gpParent.BorderY;
            float iconX = itemPos.X;
            float iconY = itemPos.Y;
            float iconW = itemSize.X;
            float iconH = itemSize.Y;

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
            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, texID);
            prog.SetUniform("textureEnabled", true);
            prog.SetUniform("lightEnabled", false);
            prog.SetUniform("texPos", texX, texY);
            prog.SetUniform("texScale", texW, texH);

            Matrix4 translateM = Matrix4.CreateTranslation(iconX, iconY, zdist);
            Matrix4 scaleM = Matrix4.CreateScale(iconW, iconH, 1f);
            Matrix4 modelM = scaleM * translateM;
            prog.SetUniform("model", modelM);
            GL.UseProgram(prog.ShaderProgramHandle);
            Square.Textured.BindAndDraw();

            if (ovTexID != -1) { // Overlay texture
                prog.SetUniform("texPos", ovTX, ovTY);
                prog.SetUniform("texScale", ovTW, ovTH);
                GL.BindTexture(TextureTarget.Texture2D, ovTexID);
                translateM = Matrix4.CreateTranslation(iconX + (iconW * ovX), iconY + (iconH * ovY), zdist + 0.001f);
                scaleM = Matrix4.CreateScale(iconW * ovW, iconH * ovH, 1f);
                modelM = scaleM * translateM;
                prog.SetUniform("model", modelM);
                GL.UseProgram(prog.ShaderProgramHandle);
                Square.Textured.BindAndDraw();
            }
            prog.SetUniform("textureEnabled", false);

            // Toggle icon
            if (IsTogglable) {
                DrawToggleIcon(prog, iconX, iconY, iconW, iconH, zdist);
            }

            if (Enabled && SubPanel != null) {
                bool bJustOpened = false;
                // If this item hovered then open subpanel
                if (piHover is not null) {
                    if (gpParent.Direction == GUIPanel.PanelDirection.Horizontal) SubPanel.Activate(iconX, iconY + iconH);
                    else SubPanel.Activate(iconX + iconW + (BorderY*aspect), iconY);
                    bJustOpened = true;                    
                }

                // Draw subpanel if it's active
                if (SubPanel.Active) {
                    PanelItem? piHover2 = SubPanel.DisplayAndCalculateMouseHover(prog, xx, yy);
                    if (piHover2 is not null) piHover = piHover2;
                    // If subpanel is active but not hovered then close it, unless we're hovering on empty squares
                    if (piHover is null && !bJustOpened) {
                        if (!SubPanel.IsHover(xx, yy)) {
                            SubPanel.Deactivate();
                        }
                    }
                }
                // Otherwise draw an arrow indicating that subpanel exists
                else {
                    prog.SetUniform("textureEnabled", false);
                    prog.SetUniform("flatColour", new Vector4(0.6f, 0.6f, 0.6f, 1f));
                    scaleM = Matrix4.CreateScale(iconW * 0.25f, iconH * 0.25f, 1f);
                    if (iconY > 0.5f) {
                        translateM = Matrix4.CreateTranslation(iconX + (iconW * 0.5f), iconY + (iconH * 0.15f), zdist + 0.01f);
                        modelM = scaleM * translateM;
                        prog.SetUniform("model", modelM);
                        GL.UseProgram(prog.ShaderProgramHandle);
                        Triangle.Flat.BindAndDraw();
                    }
                    else {
                        translateM = Matrix4.CreateTranslation(iconX + (iconW * 0.5f), iconY + (iconH * 1.0f), zdist + 0.01f);
                        modelM = scaleM * translateM;
                        prog.SetUniform("model", modelM);
                        GL.UseProgram(prog.ShaderProgramHandle);
                        Triangle.Flat.BindAndDraw();
                    }
                }
            }
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
        public override float Width(float tw, float th, float aspect) {
            float width = tw;
            if (IsTogglable) width += th;
            return width;
        }
        public override float Height(float tw, float th) {
            return th;
        }
    }
}
