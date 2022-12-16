using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using SpaceMercs.Graphics;
using SpaceMercs.Graphics.Shapes;
using System.Security.Cryptography;

namespace SpaceMercs {
    abstract class PanelItem {
        public bool Enabled { get; protected set; }
        public uint ID { get; protected set; }

        protected GUIPanel? SubPanel;
        protected readonly int texID;
        protected int ovTexID = -1;
        protected float ovX, ovY, ovW, ovH;
        protected float ovTX, ovTY, ovTW, ovTH;
        protected readonly float texX, texY, texW, texH;

        public PanelItem(TexSpecs? ts, bool _enabled, uint _ID) {
            texID = ts?.ID ?? 0;
            texX = ts?.X ?? 0f;
            texY = ts?.Y ?? 0f;
            texW = ts?.W ?? 0f;
            texH = ts?.H ?? 0f;
            SubPanel = null;
            ID = _ID;
            Enabled = _enabled;
        }

        public void DrawSelectionFrame(ShaderProgram prog, float xpos, float ypos, float iconW, float iconH, float zdist) {
            prog.SetUniform("flatColour", new Vector4(1f, 1f, 1f, 1f));
            prog.SetUniform("textureEnabled", false);
            prog.SetUniform("lightEnabled", false);
            Matrix4 translateM = Matrix4.CreateTranslation(xpos, ypos, zdist + 0.1f);
            Matrix4 scaleM = Matrix4.CreateScale(Width(iconW, iconH), Height(iconW, iconH), 1f);
            Matrix4 modelM = scaleM * translateM;
            prog.SetUniform("model", modelM);
            GL.UseProgram(prog.ShaderProgramHandle);
            Square.Lines.BindAndDraw();
        }

        abstract public PanelItem? Draw(ShaderProgram prog, double xx, double yy, GUIPanel gpParent, Vector2 itemPos, Vector2 itemSize, float zdist);
        abstract public void SetSubPanel(GUIPanel? gpl);
        abstract public void SetOverlay(TexSpecs ts, Vector4 dimRect);
        abstract public float Width(float tw, float th);
        abstract public float Height(float tw, float th);
    }
}
