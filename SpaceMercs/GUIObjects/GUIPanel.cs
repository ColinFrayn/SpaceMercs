using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using OpenTK.Windowing.Desktop;
using SpaceMercs.Graphics;

namespace SpaceMercs {
  // A fold-out context menu object
  class GUIPanel : GUIObject {
    private double PanelW, PanelH;
    private readonly List<IPanelItem> Items = new List<IPanelItem>();
    private double _ZDepth = 0.5;
    private int PanelWidth;
    private double IconW = 0.08, IconH = 0.08;
    // Public properties
    public double PanelX, PanelY, BorderY;
    public int ClickX, ClickY;
    public int Count {  get { return Items.Count; } }

    // Keep track of previous settings when drawing
    private bool bBlendState, bLightingState, bTextureState, bDepthMaskState, bDepthTestState;

    // Constructors
    public GUIPanel(GameWindow parent) : base(parent,true,1.0f) {
      PanelX = 0.0;
      PanelY = 0.0;
      Reset();
    }
    public GUIPanel(GameWindow parent, double px, double py) : base(parent, true, 1.0f) {
      PanelX = px;
      PanelY = py;
      Reset();
    }

    // Set up the panel
    public void Reset() {
      PanelWidth = 5;
      Items.Clear();
      Active = false;
      SetIconScale(1.0);
    }
    public IPanelItem InsertIcon(uint ID, int texID, double texX, double texY, double texW, double texH, bool bEnabled, GUIPanel subPanel) {
      int col = Items.Count % PanelWidth;
      int row = (Items.Count - col) / PanelWidth;
      double IconX = PanelX + (col * IconW);
      double IconY = PanelY + (row * IconH);
      Items.Add(new IconPanelItem(texID, new Vector4d(texX, texY, texW, texH), new Vector4d(IconX,IconY,IconW,IconH), bEnabled, ID, _ZDepth+0.1));
      IPanelItem icon = Items[Items.Count - 1];
      if (subPanel != null) {
        icon.SetSubPanel(subPanel);
      }
      PanelW = Math.Min(Items.Count, PanelWidth) * IconW;
      PanelH = Math.Ceiling((double)Items.Count / (double)PanelWidth) *IconH;
      return icon;
    }
    public void InsertText(string strText) {
      int col = Items.Count % PanelWidth;
      int row = (Items.Count - col) / PanelWidth;
      double IconX = PanelX + (col * IconW);
      double IconY = PanelY + (row * IconH);
      // Not implemented - this will throw!
      Items.Add(new TextPanelItem(strText, new Vector4d(IconX, IconY, IconW, IconH), _ZDepth + 0.1));
      PanelW = Math.Min(Items.Count, PanelWidth) * IconW;
      PanelH = Math.Ceiling((double)Items.Count / (double)PanelWidth) * IconH;
      // TODO
    }
    public void SetWidth(int w) {
      PanelWidth = w;
      PanelW = Math.Min(Items.Count, PanelWidth) * IconW;
      double nRows = Math.Ceiling((double)(Items.Count) / (double)PanelWidth);
      PanelH = nRows * IconH;
      // Reposition all items
      int col = 0, row = 0;
      foreach (IPanelItem pi in Items) {
        double IconX = PanelX + (col * IconW);
        double IconY = PanelY + (row * IconH);
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
    public double Height {
      get {
        return Math.Max(1.0,Math.Ceiling((double)Items.Count / (double)PanelWidth)) * IconH;
      }
    }
    public void SetIconScale(double sc) {
      if (Window != null) {
        IconW = sc * 0.08 * (double)Window.Size.Y / (double)Window.Size.X;
      }
      else {
        IconW = sc * 0.08;
      }
      IconH = sc * 0.08;
      foreach (IPanelItem pi in Items) {
        pi.SetIconSize(IconW, IconH);
      }
    }
    public double ZDepth {
      set {
        _ZDepth = value;
        foreach (IPanelItem pi in Items) {
          pi.SetZDist(_ZDepth + 0.1);
        }
      }
    }
    public void SetPosition(double xx, double yy) {
      PanelX = xx;
      PanelY = yy;
      SetWidth(PanelWidth);
    }
    
    // Display the menu
    public override void Display(int mx, int my, ShaderProgram prog) {
      if (!Active) return;
      double xx = (double)mx / (double)Window.Size.X;
      double yy = (double)my / (double)Window.Size.Y;
      HoverItem = Display2(xx, yy);
    }

    // Display the panel, using window-relative fractional coords instead of mouse coords. Return the hover item.
    public IPanelItem Display2(double xx, double yy) {
      IPanelItem piHover = null;
      double BorderX = 1.0 / (double)Window.Size.X;
      BorderY = 1.0 / (double)Window.Size.Y;
      SetupDrawing();

      // Draw the background
      GL.LineWidth(1.0f);
      GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);
      GL.Color4(0.1, 0.1, 0.1, 1.0);
      GL.Begin(BeginMode.Quads);
      GL.Vertex3(PanelX - BorderX, PanelY - BorderY, _ZDepth);
      GL.Vertex3(PanelX + PanelW + BorderX, PanelY - BorderY, _ZDepth);
      GL.Vertex3(PanelX + PanelW + BorderX, PanelY + PanelH + BorderY, _ZDepth);
      GL.Vertex3(PanelX - BorderX, PanelY + PanelH + BorderY, _ZDepth);
      GL.End();

      // Draw the icons
      foreach (IPanelItem pi in Items) {
        IPanelItem piHover2 = pi.Draw(xx, yy, this);
        if (piHover2 != null) {
          piHover = piHover2;
        }
      }

      // Draw the frame
      GL.Color4(0.7, 0.7, 0.7, 1.0);
      GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line);
      GL.Begin(BeginMode.Quads);
      GL.Vertex3(PanelX - BorderX, PanelY - BorderY, _ZDepth);
      GL.Vertex3(PanelX + PanelW + BorderX, PanelY - BorderY, _ZDepth);
      GL.Vertex3(PanelX + PanelW + BorderX, PanelY + PanelH + BorderY, _ZDepth);
      GL.Vertex3(PanelX - BorderX, PanelY + PanelH + BorderY, _ZDepth);
      GL.End();
      GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);

      // Draw the selected icon frame
      if (piHover != null && piHover.Enabled) {
        piHover.DrawSelectionFrame();
      }

      ResetDrawing();
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
    public override void TrackMouse(int x, int y) {
      // Nothing here
    }

    // Set the default (calculated) values
    public override void Initialise() {
      // Nothing to do here
    }

    // Set up for drawing
    private void SetupDrawing() {
      bBlendState = GL.IsEnabled(EnableCap.Blend);
      GL.GetBoolean(GetPName.DepthTest, out bDepthTestState);
      GL.GetBoolean(GetPName.DepthWritemask, out bDepthMaskState);
      bTextureState = GL.IsEnabled(EnableCap.Texture2D);
      bLightingState = GL.IsEnabled(EnableCap.Lighting);

      GL.Disable(EnableCap.Blend);
      GL.Disable(EnableCap.DepthTest);
      GL.DepthMask(false);
      GL.Disable(EnableCap.Texture2D);
      GL.Disable(EnableCap.Lighting);
      GL.Color4(1.0, 1.0, 1.0, 1.0);
    }

    // Put back rendering state as it was before
    private void ResetDrawing() {
      if (bBlendState) GL.Enable(EnableCap.Blend);
      else GL.Disable(EnableCap.Blend);

      if (bLightingState) GL.Enable(EnableCap.Lighting);
      else GL.Disable(EnableCap.Lighting);

      if (bTextureState) GL.Enable(EnableCap.Texture2D);
      else GL.Disable(EnableCap.Texture2D);

      if (bDepthTestState) GL.Enable(EnableCap.DepthTest);
      else GL.Disable(EnableCap.DepthTest);

      GL.DepthMask(bDepthMaskState);
      GL.Color4(1.0, 1.0, 1.0, 1.0);
    }

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
      if (xx >= (PanelX - BorderX) && yy >= (PanelY - BorderY) && xx <= (PanelX + PanelW + BorderX) && yy <= (PanelY + PanelH + BorderY))  {
        return true;
      }
      return false;
    }
  }
}
