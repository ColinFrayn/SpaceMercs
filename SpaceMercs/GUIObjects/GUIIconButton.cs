using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using OpenTK.Windowing.Desktop;
using SpaceMercs.Graphics;
using SpaceMercs.Graphics.Shapes;

namespace SpaceMercs {
    class GUIIconButton : GUIObject {
        public float ButtonX, ButtonY;
        public float ButtonWidth, ButtonHeight;
        public bool bState;
        public bool bBlend;
        private int iTexID;
        private readonly float TX, TY, TH, TW;
        public object? InternalData;
        public delegate void GUIIconButton_Trigger(GUIIconButton self);
        public GUIIconButton_Trigger? Trigger = null;

        public GUIIconButton(GameWindow parentWindow, TexSpecs ts, float x, float y, float w, float h, GUIIconButton_Trigger? trigger = null, object? dat = null) : base(parentWindow, true, 0.4f) {
            TX = ts.X;
            TY = ts.Y;
            TW = ts.W;
            TH = ts.H;
            InternalData = dat;
            ButtonWidth = w;
            ButtonHeight = h;
            ButtonX = x;
            ButtonY = y;
            Trigger = trigger;
            bState = false;
            bBlend = true;
            iTexID = ts.ID;
        }
        public GUIIconButton(GameWindow parentWindow, int TextureID, float tx, float ty, float tw, float th, float x, float y, float w, float h, GUIIconButton_Trigger? trigger = null, object? dat = null) : base(parentWindow, true, 0.4f) {
            TX = tx;
            TY = ty;
            TW = tw;
            TH = th;
            InternalData = dat;
            ButtonWidth = w;
            ButtonHeight = h;
            ButtonX = x;
            ButtonY = y;
            Trigger = trigger;
            bState = false;
            bBlend = true;
            iTexID = TextureID;
        }

        public void Updateicon(int id) {
            iTexID = id;
        }

        // Display the button
        public override void Display(int x, int y, ShaderProgram prog) {
            if (!Active) return;

            int WindowWidth = Window.Size.X;
            int WindowHeight = Window.Size.Y;
            double xpos = (double)x / (double)WindowWidth, ypos = (double)y / (double)WindowHeight;

            // Set up transparency
            if (bBlend) {
                GL.Enable(EnableCap.Blend);
                GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
            }
            else {
                GL.Disable(EnableCap.Blend);
            }
            GL.Disable(EnableCap.DepthTest);
            GL.Disable(EnableCap.Blend);

            // Draw the button background
            Vector4 col = new Vector4(1f, 1f, 1f, Alpha);
            if (xpos >= ButtonX && xpos <= (ButtonX + ButtonWidth) && ypos >= ButtonY && ypos <= (ButtonY + ButtonHeight)) col = new Vector4(0.2f, 1f, 0.4f, Alpha);
            else if (bState) col = new Vector4(1f, 0.2f, 0.2f, Alpha);

            prog.SetUniform("lightEnabled", false);
            prog.SetUniform("textureEnabled", true);
            Matrix4 translateM = Matrix4.CreateTranslation(ButtonX, ButtonY, 0.005f);
            Matrix4 scaleM = Matrix4.CreateScale(ButtonWidth, ButtonHeight, 1f);
            Matrix4 modelM = scaleM * translateM;
            prog.SetUniform("model", modelM);
            prog.SetUniform("flatColour", col);
            prog.SetUniform("texPos", TX, TY);
            prog.SetUniform("texScale", TW, TH);
            GL.Enable(EnableCap.Texture2D);
            GL.BindTexture(TextureTarget.Texture2D, iTexID);
            GL.UseProgram(prog.ShaderProgramHandle);
            Square.Textured.BindAndDraw();

            prog.SetUniform("textureEnabled", false);
            translateM = Matrix4.CreateTranslation(ButtonX, ButtonY, 0.01f);
            modelM = scaleM * translateM;
            prog.SetUniform("model", modelM);
            prog.SetUniform("flatColour", new Vector4(0.8f, 0.8f, 0.8f, Alpha));
            GL.UseProgram(prog.ShaderProgramHandle);
            Square.Lines.BindAndDraw();
        }

        // See if there's anything that needs to be done for the slider bar after a L-click
        public override bool CaptureClick(int x, int y) {
            if (!Active) return false;

            int WindowWidth = Window.Size.X;
            int WindowHeight = Window.Size.Y;
            double xpos = (double)x / (double)WindowWidth, ypos = (double)y / (double)WindowHeight;

            if (xpos >= ButtonX && xpos <= (ButtonX + ButtonWidth) && ypos >= ButtonY && ypos <= (ButtonY + ButtonHeight)) {
                if (Trigger != null) {
                    Trigger(this);
                }
                else {
                    if (bState == true) bState = false;
                    else bState = true;
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
    }
}
