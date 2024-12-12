using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using OpenTK.Windowing.Desktop;
using SpaceMercs.Graphics;
using SpaceMercs.Graphics.Shapes;
using System.Collections.Concurrent;

namespace SpaceMercs {
    class GUIMessageBox : GUIObject {
        private float BoxX, BoxY;
        private float BoxHeight;
        private float ButtonY;
        private const float ButtonWidth = 0.065f, ButtonHeight = 0.04f;
        private const float ButtonBorder = 0.01f, ButtonSplit = 0.03f;
        private const float MaxBoxHeight = 0.8f, MaxBoxWidth = 0.8f;
        private const float ButtonGap = 0.04f;
        private const float TopBorder = 0.015f;
        private const float MessageFontScale = 0.045f;
        private const float ButtonAlpha = 0.0f; // Was previously overrideable?
        private List<string> Lines;

        private Action? OnClick = null;
        private bool Decision = false;

        private class MsgConfig {
            public List<string> Lines = new List<string>();
            public Action? OnClick = null;
            public Action<object>? OnClickObj = null;
            public object? Obj = null;
            public bool Decision = false;
        }
        private readonly ConcurrentQueue<MsgConfig> queue = new ConcurrentQueue<MsgConfig>();

        public GUIMessageBox(GameWindow parentWindow) : base(parentWindow, false, 0.8f) {
            Lines = new List<string>();
        }

        // Change the text to be shown on this button & update the texture. If it's the same then don't redo texture.
        public void PopupMessage(string strText, Action? _onClick = null) {
            List<string> lines = new List<string>(strText.Replace("\r", string.Empty).Split('\n'));
            PopupMessage(lines, _onClick);
        }
        public void PopupMessage(IEnumerable<string> lines, Action? _onClick = null) {
            if (Active) {
                queue.Enqueue(new MsgConfig() { Decision = false, Lines = new List<string>(lines), OnClick = _onClick });
            }
            else {
                SetupBoxes(lines);
                OnClick = _onClick;
                Decision = false;
            }
            Active = true;
        }
        public void PopupConfirmation(string strText, Action _onClick) {
            List<string> lines = new List<string>(strText.Split('\n'));
            PopupConfirmation(lines, _onClick);
        }
        public void PopupConfirmation(IEnumerable<string> lines, Action _onClick) {
            if (Active) {
                queue.Enqueue(new MsgConfig() { Decision = true, Lines = new List<string>(lines), OnClick = _onClick });
            }
            else {
                SetupBoxes(lines);
                OnClick = _onClick;
                Decision = true;
            }
            Active = true;
        }
        private void SetupBoxes(IEnumerable<string> lines) {
            Lines = new List<string>(lines);

            BoxHeight = ButtonHeight + (ButtonBorder * 2f) + MessageFontScale * 1.15f * Lines.Count + TopBorder + ButtonGap;
            if (BoxHeight > 0.6f) BoxHeight = 0.6f;
            BoxX = (1f - MaxBoxWidth) / 2f;
            BoxY = (1f - BoxHeight) / 2f;
            ButtonY = BoxY + BoxHeight + ButtonGap;// - (ButtonHeight + ButtonBorder);
        }

        // Draw the message box and buttons
        public override void Display(int x, int y, ShaderProgram prog) {
            if (!Active) return;

            int WindowWidth = Window.Size.X;
            int WindowHeight = Window.Size.Y;
            float xpos = (float)x / (float)WindowWidth, ypos = (float)y / (float)WindowHeight;

            // Set up transparency
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
            GL.Disable(EnableCap.DepthTest);

            // Draw the message box background
            Matrix4 translateM = Matrix4.CreateTranslation(BoxX, BoxY, 0.005f);
            Matrix4 scaleM = Matrix4.CreateScale(MaxBoxWidth, BoxHeight, 1f);
            Matrix4 modelM = scaleM * translateM;
            prog.SetUniform("model", modelM);
            prog.SetUniform("flatColour", new Vector4(0.4f, 0.4f, 0.4f, Alpha));
            GL.UseProgram(prog.ShaderProgramHandle);
            Square.Flat.BindAndDraw();

            // Draw the button text at the correct location, horizontally centred
            float aspect = (float)WindowWidth / (float)WindowHeight;
            float ModifiedFontScale = MessageFontScale;

            TextRenderOptions tro = new TextRenderOptions() {
            Alignment = Alignment.TopMiddle,
                Aspect = aspect,
                TextColour = Color.White,
                XPos = 0.5f,
                YPos = BoxY + TopBorder,
                ZPos = 0.015f
            };
            foreach (string str in Lines) {
                TextMeasure tm = TextRenderer.MeasureText(str);
                tro.Scale = ModifiedFontScale * (tm.Width > 1000f ? 1000f/tm.Width : 1f);
                TextRenderer.DrawWithOptions(str, tro);
                tro.YPos += MessageFontScale * 1.15f;
            }

            translateM = Matrix4.CreateTranslation(BoxX, BoxY, 0.01f);
            modelM = scaleM * translateM;
            prog.SetUniform("model", modelM);
            prog.SetUniform("flatColour", new Vector4(1f, 1f, 1f, 1f));
            GL.UseProgram(prog.ShaderProgramHandle);
            Square.Lines.BindAndDraw();

            DrawButton(prog, xpos, ypos, 1, aspect);
            if (Decision) DrawButton(prog, xpos, ypos, 2, aspect);

            GL.Enable(EnableCap.DepthTest);
        }
        private void DrawButton(ShaderProgram prog, float xpos, float ypos, int id, float aspect) {
            float ButtonX = (1f - ButtonWidth) / 2f;
            if (Decision) {
                ButtonX = id switch {
                    1 => 0.5f - (ButtonSplit + ButtonWidth),
                    2 => 0.5f + ButtonSplit,
                    _ => throw new NotImplementedException()
                };
            }

            // Draw the button background
            Vector4 col = new Vector4(0.3f, 0.3f, 0.3f, ButtonAlpha);
            if (xpos >= ButtonX && xpos <= (ButtonX + ButtonWidth) && ypos >= ButtonY && ypos <= (ButtonY + ButtonHeight)) col = new Vector4(0.75f, 0.75f, 0.75f, ButtonAlpha);

            Matrix4 translateM = Matrix4.CreateTranslation(ButtonX, ButtonY, 0.015f);
            Matrix4 scaleM = Matrix4.CreateScale(ButtonWidth, ButtonHeight, 1f);
            Matrix4 modelM = scaleM * translateM;
            prog.SetUniform("model", modelM);
            prog.SetUniform("flatColour", col);
            GL.UseProgram(prog.ShaderProgramHandle);
            Square.Flat.BindAndDraw();

            // Draw the button text at the correct location, horizontally centred
            string Text = (id == 1) ? "OK" : "Cancel";
            TextRenderOptions tro = new TextRenderOptions() {
                Alignment = Alignment.CentreMiddle,
                Aspect = aspect,
                TextColour = Color.White,
                XPos = ButtonX + ButtonWidth * 0.5f,
                YPos = ButtonY + ButtonHeight * 0.42f,
                ZPos = 0.025f,
                Scale = ButtonHeight * 0.7f
            };
            TextRenderer.DrawWithOptions(Text, tro);

            translateM = Matrix4.CreateTranslation(ButtonX, ButtonY, 0.02f);
            modelM = scaleM * translateM;
            prog.SetUniform("model", modelM);
            prog.SetUniform("flatColour", new Vector4(1f, 1f, 1f, 1f));
            GL.UseProgram(prog.ShaderProgramHandle);
            Square.Lines.BindAndDraw();
        }

        // See if there's anything that needs to be done for the slider bar after a L-click
        public override bool CaptureClick(int x, int y) {
            if (!Active) return false;

            int WindowWidth = Window.Size.X;
            int WindowHeight = Window.Size.Y;
            double xpos = (double)x / (double)WindowWidth, ypos = (double)y / (double)WindowHeight;

            if (Decision) {
                double ButtonX = 0.5 - (ButtonSplit + ButtonWidth);
                if (xpos >= ButtonX && xpos <= (ButtonX + ButtonWidth) && ypos >= ButtonY && ypos <= (ButtonY + ButtonHeight)) {
                    OnClick?.Invoke();
                    CheckNext();
                    return true;
                }
                ButtonX = 0.5 + ButtonSplit;
                if (xpos >= ButtonX && xpos <= (ButtonX + ButtonWidth) && ypos >= ButtonY && ypos <= (ButtonY + ButtonHeight)) {
                    CheckNext();
                    return true;
                }
            }
            else {
                double ButtonX = (1.0 - ButtonWidth) / 2.0;
                if (xpos >= ButtonX && xpos <= (ButtonX + ButtonWidth) && ypos >= ButtonY && ypos <= (ButtonY + ButtonHeight)) {
                    OnClick?.Invoke();
                    CheckNext();
                    return true;
                }
            }

            return false;
        }
        public void DefaultAction() {
            if (Decision) {
                CheckNext();
            }
            else {
                OnClick?.Invoke();
                CheckNext();
            }
        }
        public void Clear() {
            queue.Clear();
            Active = false;
        }

        private void CheckNext() {
            if (!queue.TryDequeue(out MsgConfig? msgc) || msgc is null) {
                Active = false;
                return;
            }
            SetupBoxes(msgc.Lines);
            OnClick = msgc.OnClick;
            Decision = msgc.Decision;
        }

        // Are we hovering over this control?
        public override bool IsHover(int x, int y) {
            if (!Active) return false;

            int WindowWidth = Window.Size.X;
            int WindowHeight = Window.Size.Y;
            double xpos = (double)x / (double)WindowWidth, ypos = (double)y / (double)WindowHeight;

            if (Decision) {
                double ButtonX = 0.5 - (ButtonSplit + ButtonWidth);
                if (xpos >= ButtonX && xpos <= (ButtonX + ButtonWidth) && ypos >= ButtonY && ypos <= (ButtonY + ButtonHeight)) {
                    return true;
                }
                ButtonX = 0.5 + ButtonSplit;
                if (xpos >= ButtonX && xpos <= (ButtonX + ButtonWidth) && ypos >= ButtonY && ypos <= (ButtonY + ButtonHeight)) {
                    return true;
                }
            }
            else {
                double ButtonX = (1.0 - ButtonWidth) / 2.0;
                if (xpos >= ButtonX && xpos <= (ButtonX + ButtonWidth) && ypos >= ButtonY && ypos <= (ButtonY + ButtonHeight)) {
                    return true;
                }
            }

            return false;
        }

        // Unused
        public override void TrackMouse(int x, int y) { }
        public override void Initialise() { }
    }
}
