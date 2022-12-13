using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using SpaceMercs.Graphics;
using SpaceMercs.Graphics.Shapes;

namespace SpaceMercs {
    class IconPanelItem : IPanelItem {
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

        public IconPanelItem(int _texID, Vector4 texRect, Vector4 iconRect, bool _Enabled, uint _ID, float zd) {
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
        public IPanelItem? Draw(ShaderProgram prog, double xx, double yy, GUIPanel gpParent) {
            IPanelItem? piHover = null;
            float BorderY = gpParent.BorderY;

            if (xx >= iconX && xx <= iconX + iconW && yy >= iconY - BorderY && yy <= iconY + iconH + BorderY) {
                piHover = this;
            }
            if (Enabled) { // TODO: Used for what?
                if (piHover != null) {
                    //GL.Color3(1.0, 1.0, 1.0);
                }
                else {
                    //GL.Color3(0.8, 0.8, 0.8);
                }
            }
            else {
                //GL.Color3(0.3, 0.3, 0.3);
            }

            // Draw the icon
            GL.Disable(EnableCap.Blend);
            GL.Disable(EnableCap.DepthTest);
            GL.BindTexture(TextureTarget.Texture2D, texID);
            GL.DepthMask(false);
            prog.SetUniform("textureEnabled", true);
            prog.SetUniform("lightEnabled", false);

            Matrix4 translateM = Matrix4.CreateTranslation(iconX, iconY, ZDist);
            Matrix4 scaleM = Matrix4.CreateScale(texW, texH, 1f);
            Matrix4 modelM = scaleM * translateM;
            prog.SetUniform("model", modelM);
            GL.UseProgram(prog.ShaderProgramHandle);
            Square.Flat.BindAndDraw();

            if (ovTexID != -1) {
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
                    IPanelItem? piHover2 = SubPanel.DisplayNormalisedCoords(prog, xx, yy);
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
                    prog.SetUniform("flatColour", 0.8f, 0.8f, 0.8f);
                    scaleM = Matrix4.CreateScale(iconW * 0.3f, iconH * 0.3f, 1f);
                    if (iconY > 0.5) {
                        translateM = Matrix4.CreateTranslation(iconX + (iconW * 0.5f), iconY + (iconH * 0.15f), ZDist + 0.01f);
                        modelM = scaleM * translateM;
                        prog.SetUniform("model", modelM);
                        Triangle.Flat.BindAndDraw();
                        // TODO: Need to get texture coords right
                        //GL.Vertex3(iconX + (iconW * 0.35), iconY + (iconH * 0.15), ZDist + 0.01);
                        //GL.Vertex3(iconX + (iconW * 0.65), iconY + (iconH * 0.15), ZDist + 0.01);
                        //GL.Vertex3(iconX + (iconW * 0.5), iconY - (iconH * 0.15), ZDist + 0.01);
                    }
                    else {
                        translateM = Matrix4.CreateTranslation(iconX + (iconW * 0.5f), iconY + (iconH * 1.0f), ZDist + 0.01f);
                        modelM = scaleM * translateM;
                        prog.SetUniform("model", modelM);
                        Triangle.Flat.BindAndDraw();
                        // TODO: Need to get texture coords right
                        //GL.Vertex3(iconX + (iconW * 0.35), iconY + (iconH * 0.85), ZDist + 0.01);
                        //GL.Vertex3(iconX + (iconW * 0.65), iconY + (iconH * 0.85), ZDist + 0.01);
                        //GL.Vertex3(iconX + (iconW * 0.5), iconY + (iconH * 1.15), ZDist + 0.01);
                    }
                    GL.End();
                }
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
            SubPanel = gpl;
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
