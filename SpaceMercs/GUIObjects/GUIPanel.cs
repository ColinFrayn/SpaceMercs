using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using OpenTK.Windowing.Desktop;
using SpaceMercs.Graphics;
using SpaceMercs.Graphics.Shapes;

namespace SpaceMercs {
    // A fold-out context menu object
    class GUIPanel : GUIObject {
        private float PanelW, PanelH;
        private readonly List<IPanelItem> Items = new List<IPanelItem>();
        private float _ZDepth = 0.5f;
        private int PanelWidth;
        private float IconW = 0.08f, IconH = 0.08f;
        // Public properties
        public float PanelX, PanelY, BorderY;
        public int ClickX, ClickY;
        public int Count { get { return Items.Count; } }

        // Keep track of previous settings when drawing
        private bool bBlendState, bLightingState, bTextureState, bDepthMaskState, bDepthTestState;

        // Constructors
        public GUIPanel(GameWindow parent) : base(parent, true, 1f) {
            PanelX = 0f;
            PanelY = 0f;
            Reset();
        }
        public GUIPanel(GameWindow parent, float px, float py) : base(parent, true, 1f) {
            PanelX = px;
            PanelY = py;
            Reset();
        }

        // Set up the panel
        public void Reset() {
            PanelWidth = 5;
            Items.Clear();
            Active = false;
            SetIconScale(1f);
        }
        public IPanelItem InsertIcon(uint ID, int texID, float texX, float texY, float texW, float texH, bool bEnabled, GUIPanel? subPanel) {
            int col = Items.Count % PanelWidth;
            int row = (Items.Count - col) / PanelWidth;
            float IconX = PanelX + (col * IconW);
            float IconY = PanelY + (row * IconH);
            Items.Add(new IconPanelItem(texID, new Vector4(texX, texY, texW, texH), new Vector4(IconX, IconY, IconW, IconH), bEnabled, ID, _ZDepth + 0.1f));
            IPanelItem icon = Items[Items.Count - 1];
            if (subPanel != null) {
                icon.SetSubPanel(subPanel);
            }
            PanelW = (float)Math.Min(Items.Count, PanelWidth) * IconW;
            PanelH = (float)Math.Ceiling((double)Items.Count / (double)PanelWidth) * IconH;
            return icon;
        }
        public void InsertText(string strText) {
            int col = Items.Count % PanelWidth;
            int row = (Items.Count - col) / PanelWidth;
            float IconX = PanelX + (col * IconW);
            float IconY = PanelY + (row * IconH);
            // Not implemented - this will throw!
            Items.Add(new TextPanelItem(strText, new Vector4(IconX, IconY, IconW, IconH), _ZDepth + 0.1f));
            PanelW = (float)Math.Min(Items.Count, PanelWidth) * IconW;
            PanelH = (float)Math.Ceiling((double)Items.Count / (double)PanelWidth) * IconH;
        }
        public void SetWidth(int w) {
            PanelWidth = w;
            PanelW = Math.Min(Items.Count, PanelWidth) * IconW;
            float nRows = (float)Math.Ceiling((double)(Items.Count) / (double)PanelWidth);
            PanelH = nRows * IconH;
            // Reposition all items
            int col = 0, row = 0;
            foreach (IPanelItem pi in Items) {
                float IconX = PanelX + (col * IconW);
                float IconY = PanelY + (row * IconH);
                pi.SetPos(IconX, IconY);
                col++;
                if (col == PanelWidth) {
                    col = 0;
                    row++;
                }
            }
        }
        public IPanelItem? HoverItem { get; private set; } = null;
        public int HoverID {
            get {
                if (HoverItem == null) return -1;
                return (int)HoverItem.ID;
            }
        }
        public float Height {
            get {
                return (float)Math.Max(1.0, Math.Ceiling((double)Items.Count / (double)PanelWidth)) * IconH;
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
            foreach (IPanelItem pi in Items) {
                pi.SetIconSize(IconW, IconH);
            }
        }
        public float ZDepth {
            set {
                _ZDepth = value;
                foreach (IPanelItem pi in Items) {
                    pi.SetZDist(_ZDepth + 0.1f);
                }
            }
        }
        public void SetPosition(float xx, float yy) {
            PanelX = xx;
            PanelY = yy;
            SetWidth(PanelWidth);
        }

        // Display the menu
        public override void Display(int mx, int my, ShaderProgram prog) {
            if (!Active) return;
            double xx = (double)mx / (double)Window.Size.X;
            double yy = (double)my / (double)Window.Size.Y;
            HoverItem = Display2(prog, xx, yy);
        }

        // Display the panel, using window-relative fractional coords instead of mouse coords. Return the hover item.
        public IPanelItem Display2(ShaderProgram prog, double xx, double yy) {
            IPanelItem piHover = null;
            float BorderX = 1f / (float)Window.Size.X;
            BorderY = 1f / (float)Window.Size.Y;

            GL.Disable(EnableCap.Blend);
            GL.Disable(EnableCap.DepthTest);
            GL.DepthMask(false);

            Matrix4 translateM = Matrix4.CreateTranslation(PanelX - BorderX, PanelY - BorderY, _ZDepth);
            Matrix4 scaleM = Matrix4.CreateScale(PanelW + BorderX * 2f, PanelH * BorderY * 2f, 1f);
            Matrix4 modelM = scaleM * translateM;
            prog.SetUniform("model", modelM);
            prog.SetUniform("flatColour", new Vector4(0.1f, 0.1f, 0.1f, 0.3f));
            GL.UseProgram(prog.ShaderProgramHandle);
            Square.Flat.Bind();
            Square.Flat.Draw();
            Square.Flat.Unbind();

            // Draw the icons
            foreach (IPanelItem pi in Items) {
                IPanelItem piHover2 = pi.Draw(prog, xx, yy, this);
                if (piHover2 != null) {
                    piHover = piHover2;
                }
            }

            // Draw the frame
            translateM = Matrix4.CreateTranslation(PanelX - BorderX, PanelY - BorderY, _ZDepth + 0.001f);
            modelM = scaleM * translateM;
            prog.SetUniform("model", modelM);
            prog.SetUniform("flatColour", new Vector4(0.7f, 0.7f, 0.7f, 1f));
            GL.UseProgram(prog.ShaderProgramHandle);
            Square.Lines.Bind();
            Square.Lines.Draw();
            Square.Lines.Unbind();

            // Draw the selected icon frame
            if (piHover != null && piHover.Enabled) {
                piHover.DrawSelectionFrame(); // TODO
            }

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
    }
}
