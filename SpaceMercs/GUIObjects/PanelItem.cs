using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using SpaceMercs.Graphics;
using SpaceMercs.Graphics.Shapes;
using System.Security.Cryptography;

namespace SpaceMercs {
    abstract class PanelItem {
        public bool Enabled { get; protected set; }
        public uint ID { get; protected set; }

        protected float ZDist;
        protected GUIPanel? SubPanel;
        protected readonly int texID;
        protected int ovTexID = -1;
        protected float ovX, ovY, ovW, ovH;
        protected float ovTX, ovTY, ovTW, ovTH;
        protected readonly float texX, texY, texW, texH;
        protected float iconX, iconY, iconW, iconH;

        public PanelItem(TexSpecs? ts, Vector4? iconRect, bool _enabled, uint _ID, float zd) {
            texID = ts?.ID ?? 0;
            texX = ts?.X ?? 0f;
            texY = ts?.Y ?? 0f;
            texW = ts?.W ?? 0f;
            texH = ts?.H ?? 0f;
            iconX = iconRect?.X ?? 0f;
            iconY = iconRect?.Y ?? 0f;
            iconW = iconRect?.Z ?? 0f;
            iconH = iconRect?.W ?? 0f;
            SubPanel = null;
            ZDist = zd;
            ID = _ID;
            Enabled = _enabled;
        }

        public void DrawSelectionFrame(ShaderProgram prog) {
            prog.SetUniform("flatColour", new Vector4(1f, 1f, 1f, 1f));
            prog.SetUniform("textureEnabled", false);
            prog.SetUniform("lightEnabled", false);
            Matrix4 translateM = Matrix4.CreateTranslation(iconX, iconY, ZDist + 0.1f);
            Matrix4 scaleM = Matrix4.CreateScale(iconW, iconH, 1f);
            Matrix4 modelM = scaleM * translateM;
            prog.SetUniform("model", modelM);
            GL.UseProgram(prog.ShaderProgramHandle);
            Square.Lines.BindAndDraw();
        }

        abstract public PanelItem? Draw(ShaderProgram prog, double xx, double yy, GUIPanel gpParent);
        abstract public void SetPos(float x, float y);
        abstract public void SetIconSize(float w, float h);
        abstract public void SetSubPanel(GUIPanel? gpl);
        abstract public void SetZDist(float zd);
        abstract public void SetOverlay(TexSpecs ts, Vector4 dimRect);
    }
}
