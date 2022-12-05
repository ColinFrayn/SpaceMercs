using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using OpenTK.Windowing.Desktop;
using SpaceMercs.Graphics;
using SpaceMercs.Graphics.Shapes;

namespace SpaceMercs {
    class GUIButton : GUIObject {
        public delegate void GUIButton_Trigger();

        private float ButtonX, ButtonY;
        private float ButtonWidth, ButtonHeight;
        private bool State, Blend, Stipple;
        private readonly GUIButton_Trigger Trigger;
        private string Text;

        public GUIButton(string strText, GameWindow parentWindow, GUIButton_Trigger _trigger) : base(parentWindow, false, 0.3f) {
            Trigger = _trigger;
            ButtonWidth = 0.05f;
            ButtonHeight = 0.02f;
            State = false;
            Blend = true;
            Text = strText;
        }

        // Change the text to be shown on this button & update the texture. If it's the same then don't redo texture.
        public void UpdateText(string strText) {
            Text = strText;
        }

        // Display the button
        public override void Display(int x, int y, ShaderProgram prog) {
            if (!Active) return;

            int WindowWidth = Window.Size.X;
            int WindowHeight = Window.Size.Y;
            float xpos = (float)x / (float)WindowWidth, ypos = (float)y / (float)WindowHeight;

            // Set up transparency
            if (Blend) {
                GL.Enable(EnableCap.Blend);
                GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
            }
            else {
                GL.Disable(EnableCap.Blend);
            }
            GL.Disable(EnableCap.DepthTest);

            // Draw the button background
            Vector4 col = new Vector4(0.3f, 0.3f, 0.3f, Alpha);
            if (xpos >= ButtonX && xpos <= (ButtonX + ButtonWidth) && ypos >= ButtonY && ypos <= (ButtonY + ButtonHeight)) col = new Vector4(0.6f, 0.6f, 0.6f, Alpha);
            else if (State) col = new Vector4(0.45f, 0.45f, 0.45f, Alpha);

            Matrix4 translateM = Matrix4.CreateTranslation(ButtonX, ButtonY, 0.005f);
            Matrix4 scaleM = Matrix4.CreateScale(ButtonWidth, ButtonHeight, 1f);
            Matrix4 modelM = scaleM * translateM;
            prog.SetUniform("model", modelM);
            prog.SetUniform("flatColour", col);
            GL.UseProgram(prog.ShaderProgramHandle);
            Square.Flat.Bind();
            Square.Flat.Draw();
            Square.Flat.Unbind();

            // Draw the button text
            TextRenderOptions tro = new TextRenderOptions() {
                Alignment = Alignment.TopLeft,
                Aspect = (float)WindowWidth / (float)WindowHeight,
                FixedHeight = ButtonHeight,
                FixedWidth = ButtonWidth,
                IsFixedSize = true,
                TextColour = Color.White,
                TextPos = TextAlign.Centre,
                XPos = ButtonX,
                YPos = ButtonY,
                ZPos = 0.15f
            };
            TextRenderer.DrawWithOptions(Text, tro);

            translateM = Matrix4.CreateTranslation(ButtonX, ButtonY, 0.01f);
            modelM = scaleM * translateM;
            prog.SetUniform("model", modelM);
            prog.SetUniform("flatColour", new Vector4(1f, 1f, 1f, 1f));
            GL.UseProgram(prog.ShaderProgramHandle);
            Square.Lines.Bind();
            Square.Lines.Draw();
            Square.Lines.Unbind();

            //// Set up stipple
            //if (Stipple) {
            //    GL.Enable(EnableCap.LineStipple);
            //    GL.LineStipple(1, 255);
            //}
            //else GL.Disable(EnableCap.LineStipple);

            //GL.Disable(EnableCap.LineStipple);
            GL.Enable(EnableCap.DepthTest);
        }

        // See if there's anything that needs to be done for the slider bar after a L-click
        public override bool CaptureClick(int x, int y) {
            if (!Active) return false;

            int WindowWidth = Window.Size.X;
            int WindowHeight = Window.Size.Y;
            double xpos = (double)x / (double)WindowWidth, ypos = (double)y / (double)WindowHeight;

            if (xpos >= ButtonX && xpos <= (ButtonX + ButtonWidth) && ypos >= ButtonY && ypos <= (ButtonY + ButtonHeight)) {
                if (Trigger != null) {
                    Trigger();
                }
                else {
                    if (State == true) State = false;
                    else State = true;
                }
                return true;
            }

            return false;
        }

        // Track the slider bar
        public override void TrackMouse(int x, int y) {
            // Nothing to do here
        }

        // Set the default (calculated) values
        public override void Initialise() {
            // Nothing to do here
        }

        // Are we hovering over this control?
        public override bool IsHover(int x, int y) {
            if (!Active) return false;

            int WindowWidth = Window.Size.X;
            int WindowHeight = Window.Size.Y;
            double xpos = (double)x / (double)WindowWidth, ypos = (double)y / (double)WindowHeight;

            if (xpos >= ButtonX && xpos <= (ButtonX + ButtonWidth) && ypos >= ButtonY && ypos <= (ButtonY + ButtonHeight)) {
                return true;
            }

            return false;
        }

        // Configuration
        public void SetBlend(bool b) {
            Blend = b;
        }
        public void SetStipple(bool b) {
            Stipple = b;
        }
        public void SetPosition(float x, float y) {
            ButtonX = x;
            ButtonY = y;
        }
        public void SetSize(float w, float h) {
            ButtonWidth = w;
            ButtonHeight = h;
        }
    }
}
