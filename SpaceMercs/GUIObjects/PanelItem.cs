using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using SpaceMercs.Graphics;
using SpaceMercs.Graphics.Shapes;

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
        protected readonly bool IsTogglable = false;
        protected Func<bool>? GetBool = null;

        public PanelItem(TexSpecs? ts, bool _enabled, uint _ID, bool togglable) {
            texID = ts?.ID ?? 0;
            texX = ts?.X ?? 0f;
            texY = ts?.Y ?? 0f;
            texW = ts?.W ?? 0f;
            texH = ts?.H ?? 0f;
            SubPanel = null;
            ID = _ID;
            Enabled = _enabled;
            IsTogglable = togglable;
        }

        public static void DrawSelectionFrame(ShaderProgram prog, float xpos, float ypos, float iconW, float iconH, float zdist) {
            prog.SetUniform("flatColour", new Vector4(1f, 1f, 1f, 1f));
            prog.SetUniform("textureEnabled", false);
            prog.SetUniform("lightEnabled", false);
            Matrix4 translateM = Matrix4.CreateTranslation(xpos, ypos, zdist + 0.1f);
            Matrix4 scaleM = Matrix4.CreateScale(iconW, iconH, 1f);
            Matrix4 modelM = scaleM * translateM;
            prog.SetUniform("model", modelM);
            GL.UseProgram(prog.ShaderProgramHandle);
            Square.Lines.BindAndDraw();
        }

        public void DrawToggleIcon(ShaderProgram prog, float xpos, float ypos, float iconW, float iconH, float zdist) {
            if (GetBool is null) return;
            prog.SetUniform("flatColour", new Vector4(0.7f, 0.7f, 0.7f, 1f));
            prog.SetUniform("textureEnabled", false);
            prog.SetUniform("lightEnabled", false);
            Matrix4 translateM = Matrix4.CreateTranslation(xpos + iconW - (iconH*0.75f), ypos + (iconH*0.25f), zdist + 0.1f);
            Matrix4 scaleM = Matrix4.CreateScale(iconH / 2f, iconH / 2f, 1f);
            Matrix4 modelM = scaleM * translateM;
            prog.SetUniform("model", modelM);
            GL.UseProgram(prog.ShaderProgramHandle);
            Square.Lines.BindAndDraw();
            if (GetBool()) {
                prog.SetUniform("flatColour", new Vector4(0f, 1f, 0f, 1f));
                GL.UseProgram(prog.ShaderProgramHandle);
                Square.Flat.BindAndDraw();
            }
        }

        abstract public PanelItem? Draw(ShaderProgram prog, double xx, double yy, GUIPanel gpParent, Vector2 itemPos, Vector2 itemSize, float zdist, float aspect);
        abstract public void SetSubPanel(GUIPanel? gpl);
        abstract public void SetOverlay(TexSpecs ts, Vector4 dimRect);
        abstract public float Width(float tw, float th, float aspect);
        abstract public float Height(float tw, float th);

        public void SetToggleDelegate(Func<bool>? getBool) {
            GetBool = getBool;
        }
        public void Enable() { Enabled = true; }
        public void Disable() { Enabled = false; }
    }
}
