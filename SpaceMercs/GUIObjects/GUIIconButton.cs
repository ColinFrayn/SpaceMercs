using OpenTK.Graphics.OpenGL;
using OpenTK.Windowing.Desktop;
using SpaceMercs.Graphics;

namespace SpaceMercs {
    class GUIIconButton : GUIObject {
        public double ButtonX, ButtonY;
        public double ButtonWidth, ButtonHeight;
        public bool bState;
        public bool bBlend;
        private int iTexID;
        private readonly double TX, TY, TH, TW;
        public object? InternalData;
        public delegate void GUIIconButton_Trigger(GUIIconButton self);
        public GUIIconButton_Trigger? Trigger = null;

        public GUIIconButton(GameWindow parentWindow, int TextureID, double tx, double ty, double tw, double th, double x, double y, double w, double h, GUIIconButton_Trigger? trigger = null, object? dat = null) : base(parentWindow, true, 0.4f) {
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
            GL.Enable(EnableCap.Texture2D);
            if (bBlend) {
                GL.Enable(EnableCap.Blend);
                GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
            }
            else {
                GL.Disable(EnableCap.Blend);
            }

            GL.Disable(EnableCap.DepthTest);
            GL.Enable(EnableCap.Texture2D);
            GL.BindTexture(TextureTarget.Texture2D, iTexID);
            if (xpos >= ButtonX && xpos <= (ButtonX + ButtonWidth) && ypos >= ButtonY && ypos <= (ButtonY + ButtonHeight)) GL.Color4(0.2f, 1.0f, 0.4f, Alpha);
            else if (bState) GL.Color4(1.0f, 0.2f, 0.2f, Alpha);
            else GL.Color4(1.0f, 1.0f, 1.0f, Alpha);
            GL.Begin(BeginMode.Quads);
            GL.TexCoord2(TX, TY);
            GL.Vertex2(ButtonX, ButtonY);
            GL.TexCoord2(TX + TW, TY);
            GL.Vertex2(ButtonX + ButtonWidth, ButtonY);
            GL.TexCoord2(TX + TW, TY + TH);
            GL.Vertex2(ButtonX + ButtonWidth, ButtonY + ButtonHeight);
            GL.TexCoord2(TX, TY + TH);
            GL.Vertex2(ButtonX, ButtonY + ButtonHeight);
            GL.End();
            GL.Disable(EnableCap.Texture2D);

            GL.Color4(0.8f, 0.8f, 0.8f, Alpha);
            GL.Begin(BeginMode.LineLoop);
            GL.Vertex2(ButtonX, ButtonY);
            GL.Vertex2(ButtonX + ButtonWidth, ButtonY);
            GL.Vertex2(ButtonX + ButtonWidth, ButtonY + ButtonHeight);
            GL.Vertex2(ButtonX, ButtonY + ButtonHeight);
            GL.End();
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
