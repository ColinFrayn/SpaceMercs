using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using OpenTK.Windowing.Desktop;
using SpaceMercs.Graphics;
using SpaceMercs.Graphics.Shapes;

namespace SpaceMercs {
    // A fold-out context menu object
    class GUIPanel : GUIObject {
        public enum PanelDirection { Horizontal, Vertical };
        private readonly PanelDirection Direction = PanelDirection.Horizontal;
        private float PanelW = 0f, PanelH = 0f;
        private readonly List<PanelItem> Items = new List<PanelItem>();
        private float _ZDepth = 0.5f;
        private float IconW = 0.08f, IconH = 0.08f;

        // Public properties
        public float PanelX { get; private set; }
        public float PanelY { get; private set; }
        public float BorderY { get; private set; }
        public int ClickX { get; private set; }
        public int ClickY { get; private set; }
        public int Count { get { return Items.Count; } }

        // Constructors
        public GUIPanel(GameWindow parent, float px = 0f, float py = 0f, PanelDirection direction = PanelDirection.Horizontal) : base(parent, true, 1f) {
            PanelX = px;
            PanelY = py;
            Direction = direction;
            Reset();
        }

        // Set up the panel
        public void Reset() {
            Items.Clear();
            Active = false;
            SetIconScale(1f);
        }
        public PanelItem InsertIconItem(uint ID, TexSpecs spec, bool bEnabled, GUIPanel? subPanel) {
            PanelItem icon = new IconPanelItem(spec, bEnabled, ID);
            icon.SetSubPanel(subPanel);
            Items.Add(icon);
            UpdatePanelDimensions(1f); // Aspect is irrelevant for icon panels
            return icon;
        }
        public void InsertTextItem(uint ID, string strText, float aspect) {
            PanelItem pi = new TextPanelItem(strText, ID);
            Items.Add(pi);
            UpdatePanelDimensions(aspect);
        }

        private void UpdatePanelDimensions(float aspect) {
            PanelW = 0f;
            PanelH = 0f;
            foreach (PanelItem pi in Items) {
                if (Direction == PanelDirection.Horizontal) { PanelW += pi.Width(IconW, IconH, aspect); PanelH = Math.Max(PanelH, pi.Height(IconW, IconH)); }
                else { PanelW = Math.Max(PanelW, pi.Width(IconW, IconH, aspect)); PanelH += pi.Height(IconW, IconH); }
            }
        }
        public PanelItem? HoverItem { get; private set; } = null;
        public int HoverID {
            get {
                if (HoverItem == null) return -1;
                return (int)HoverItem.ID;
            }
        }
        public void SetIconScale(float sc) {
            if (Window != null) {
                IconW = sc * 0.08f * (float)Window.Size.Y / (float)Window.Size.X;
            }
            else {
                IconW = sc * 0.08f;
            }
            IconH = sc * 0.08f;
        }
        public void SetPosition(float xx, float yy) {
            PanelX = xx;
            PanelY = yy;
        }
        public void SetClick(int x, int y) {
            ClickX = x;
            ClickY = y;
        }

        #region GUIObject
        // Display the panel
        public override void Display(int mx, int my, ShaderProgram prog) {
            if (!Active) return;
            double fmousex = (double)mx / (double)Window.Size.X;
            double fmousey = (double)my / (double)Window.Size.Y;
            HoverItem = DisplayAndCalculateMouseHover(prog, fmousex, fmousey);
        }

        // Display the panel, using window-relative fractional coords instead of mouse coords. Return the hover item.
        public PanelItem? DisplayAndCalculateMouseHover(ShaderProgram prog, double fmousex, double fmousey) {
            PanelItem? piHover = null;
            float BorderX = 1f / (float)Window.Size.X;
            BorderY = 1f / (float)Window.Size.Y;

            GL.Disable(EnableCap.Blend);
            GL.Disable(EnableCap.DepthTest);
            GL.DepthMask(false);
            prog.SetUniform("textureEnabled", false);
            prog.SetUniform("lightEnabled", false);

            Matrix4 translateM = Matrix4.CreateTranslation(PanelX - BorderX, PanelY - BorderY, _ZDepth);
            Matrix4 scaleM = Matrix4.CreateScale(PanelW + BorderX * 2f, PanelH + BorderY * 2f, 1f);
            Matrix4 modelM = scaleM * translateM;
            prog.SetUniform("model", modelM);
            prog.SetUniform("flatColour", new Vector4(0.1f, 0.1f, 0.1f, 0.5f));
            GL.UseProgram(prog.ShaderProgramHandle);
            Square.Flat.BindAndDraw();

            // Draw the icons
            float px = PanelX, py = PanelY;
            float aspect = (float)Window.Size.Y / (float)Window.Size.X;
            foreach (PanelItem pi in Items) {
                PanelItem? piHover2 = pi.Draw(prog, fmousex, fmousey, this, new Vector2(px, py), new Vector2(PanelW, IconH), _ZDepth + 0.01f, aspect);
                if (piHover2 is not null) {
                    piHover = piHover2;
                }
                if (Direction == PanelDirection.Horizontal) px += pi.Width(IconW, IconH, aspect);
                else py += pi.Height(IconW, IconH);
            }

            // Draw the frame
            translateM = Matrix4.CreateTranslation(PanelX - BorderX, PanelY - BorderY, _ZDepth + 0.01f);
            modelM = scaleM * translateM;
            prog.SetUniform("model", modelM);
            prog.SetUniform("flatColour", new Vector4(0.7f, 0.7f, 0.7f, 1f));
            prog.SetUniform("textureEnabled", false);
            GL.UseProgram(prog.ShaderProgramHandle);
            Square.Lines.BindAndDraw();

            // Return if we're hovering over anything
            return piHover;
        }

        // See if there's anything that needs to be done for the panel after a L-click
        public override bool CaptureClick(int x, int y) {
            // Check if we're hovering somewhere
            if (HoverItem != null) {
                // Handle the click
                // TODO: Click on GUIPanel
                return true;
            }

            return false;
        }

        // Track a mouse
        public override void TrackMouse(int x, int y) { }

        // Set the default (calculated) values
        public override void Initialise() { }

        // Are we hovering over this control?
        public override bool IsHover(int x, int y) {
            if (!Active) return false;

            // Hovering on list?
            if (HoverItem != null) {
                return true;
            }

            return false;
        }
        public bool IsHover(double xx, double yy) {
            double BorderX = 1.0 / (double)Window.Size.X;
            double BorderY = 1.0 / (double)Window.Size.Y;
            if (xx >= (PanelX - BorderX) && yy >= (PanelY - BorderY) && xx <= (PanelX + PanelW + BorderX) && yy <= (PanelY + PanelH + BorderY)) {
                return true;
            }
            return false;
        }

        #endregion // GUIObject
    }
}
