using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using OpenTK.Windowing.Desktop;
using SpaceMercs.Graphics;
using SpaceMercs.Graphics.Shapes;

namespace SpaceMercs {
    // A fold-out context menu object
    class GUIPanel : GUIObject {
        public enum PanelDirection { Horizontal, Vertical };
        private float PanelW = 0f, PanelH = 0f;
        private readonly List<PanelItem> Items = new List<PanelItem>();
        private readonly float _ZDepth = 0.5f;
        private const float MenuSize = 50; // Pixels
        private float IconScale = 1f;
        private float IconW { get { return IconScale * (float)MenuSize / (float)Window.Size.X; } }
        private float IconH { get { return IconScale * (float)MenuSize / (float)Window.Size.Y; } }

        // Public properties
        public readonly PanelDirection Direction = PanelDirection.Horizontal;
        public float PanelX { get; private set; }
        public float PanelY { get; private set; }
        public float BorderY { get; private set; }
        public float BorderX { get; private set; }
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
        public PanelItem InsertIconItem(object datum, TexSpecs spec, bool bEnabled, GUIPanel? subPanel, Func<bool>? getBoolFunc = null) {
            PanelItem pi = new IconPanelItem(spec, bEnabled, datum, getBoolFunc != null);
            pi.SetSubPanel(subPanel);
            pi.SetToggleDelegate(getBoolFunc);
            Items.Add(pi);
            float aspect = (float)Window.Size.X / (float)Window.Size.Y;
            UpdatePanelDimensions(aspect);
            return pi;
        }
        public void InsertTextItem(object datum, string strText, float aspect, Func<bool>? getBoolFunc = null) {
            PanelItem pi = new TextPanelItem(strText, datum, getBoolFunc != null);
            pi.SetToggleDelegate(getBoolFunc);
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
                if (HoverItem?.Datum is null) return -1;
                if (HoverItem?.Datum?.GetType() == typeof(int) || HoverItem?.Datum?.GetType() == typeof(uint)) {
                    return Convert.ToInt32(HoverItem.Datum);
                };
                return -1;
            }
        }
        public object? HoverObject {
            get {
                return HoverItem?.Datum;
            }
        }
        public void SetIconScale(float sc) {
            IconScale = sc;
        }
        public void SetPosition(float xx, float yy) {
            PanelX = xx;
            PanelY = yy;
        }
        public void SetClick(int x, int y) {
            ClickX = x;
            ClickY = y;
        }
        public void Activate(float px, float py) {
            PanelX = px;
            PanelY = py;
            this.Activate();
        }
        public PanelItem? GetItem(uint ID) {
            foreach (PanelItem it in Items) {
                if (it.Datum is uint id && id == ID) return it;
            }
            return null;
        }

        // Display the panel, using window-relative fractional coords instead of mouse coords. Return the hover item.
        public PanelItem? DisplayAndCalculateMouseHover(ShaderProgram prog, double fmousex, double fmousey) {
            PanelItem? piHover = null;
            BorderX = 1f / (float)Window.Size.X;
            BorderY = 1f / (float)Window.Size.Y;
            float aspect = (float)Window.Size.X / (float)Window.Size.Y;
            UpdatePanelDimensions(aspect);

            GL.Disable(EnableCap.Blend);
            GL.Disable(EnableCap.DepthTest);
            GL.DepthMask(false);
            prog.SetUniform("view", Matrix4.Identity);
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
            foreach (PanelItem pi in Items) {
                PanelItem? piHover2 = pi.Draw(prog, fmousex, fmousey, this, new Vector2(px, py), new Vector2(Direction == PanelDirection.Horizontal ? IconW : PanelW, IconH), _ZDepth + 0.01f, aspect);
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

            // Return details of the item we're hovering over, if any
            return piHover;
        }

        #region GUIObject
        // Display the panel
        public override void Display(int mx, int my, ShaderProgram prog) {
            if (!Active) return;
            double fmousex = (double)mx / (double)Window.Size.X;
            double fmousey = (double)my / (double)Window.Size.Y;
            HoverItem = DisplayAndCalculateMouseHover(prog, fmousex, fmousey);
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
