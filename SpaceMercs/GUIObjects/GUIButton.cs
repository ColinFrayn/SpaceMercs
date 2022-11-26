using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using OpenTK.Windowing.Desktop;

namespace SpaceMercs {
  class GUIButton : GUIObject {
    public delegate void GUIButton_Trigger();

    private double ButtonX, ButtonY;
    private double ButtonWidth, ButtonHeight;
    private bool State, Blend, Stipple;
    private readonly GUIButton_Trigger Trigger = null;
    private readonly TextLabel tlText;

    public GUIButton(string strText, GameWindow parentWindow, GUIButton_Trigger _trigger) {
      Alpha = 0.4f;
      Window = parentWindow;
      Trigger = _trigger;
      ButtonWidth = 0.05;
      ButtonHeight = 0.02;
      Active = false;
      State = false;
      Blend = true;
      tlText = new TextLabel(strText);
    }

    // Change the text to be shown on this button & update the texture. If it's the same then don't redo texture.
    public void UpdateText(string strText) {
      tlText.UpdateText(strText);
    }

    // Display the button
    public override void Display(int x, int y) {
      if (!Active) return;

      int WindowWidth = Window.Size.X;
      int WindowHeight = Window.Size.Y;
      double xpos = (double)x / (double)WindowWidth, ypos = (double)y / (double)WindowHeight;

      // Set up transparency
      GL.Disable(EnableCap.Lighting);
      GL.Disable(EnableCap.Texture2D);
      if (Blend) {
        GL.Enable(EnableCap.Blend);
        GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
      }
      else {
        GL.Disable(EnableCap.Blend);
      }

      // Draw the button background
      if (xpos >= ButtonX && xpos <= (ButtonX + ButtonWidth) && ypos >= ButtonY && ypos <= (ButtonY + ButtonHeight)) GL.Color4(0.8f, 0.8f, 0.8f, Alpha);
      else if (State) GL.Color4(0.6f, 0.6f, 0.6f, Alpha);
      else GL.Color4(0.4f, 0.4f, 0.4f, Alpha);
      GL.Disable(EnableCap.DepthTest);
      GL.Begin(BeginMode.Quads);
      GL.Vertex3(ButtonX, ButtonY, 0.1f);
      GL.Vertex3(ButtonX + ButtonWidth, ButtonY, 0.1f);
      GL.Vertex3(ButtonX + ButtonWidth, ButtonY + ButtonHeight, 0.1f);
      GL.Vertex3(ButtonX, ButtonY + ButtonHeight, 0.1f);
      GL.End();

      // Draw the button text
      GL.PushMatrix();
      GL.Translate(ButtonX, ButtonY, 0.2);
      GL.Rotate(180.0, Vector3d.UnitX);    
      tlText.Draw(TextLabel.Alignment.TopLeft,ButtonWidth,ButtonHeight);
      GL.PopMatrix();

      // Set up stipple
      if (Stipple) {
        GL.Enable(EnableCap.LineStipple);
        GL.LineStipple(1, 255);
      }
      else GL.Disable(EnableCap.LineStipple);

      // Draw the framework
      GL.Disable(EnableCap.DepthTest);
      GL.Color3(1.0, 1.0, 1.0);
      GL.Begin(BeginMode.LineLoop);
      GL.Vertex3(ButtonX, ButtonY, 0.15f);
      GL.Vertex3(ButtonX + ButtonWidth, ButtonY, 0.15f);
      GL.Vertex3(ButtonX + ButtonWidth, ButtonY + ButtonHeight, 0.15f);
      GL.Vertex3(ButtonX, ButtonY + ButtonHeight, 0.15f);
      GL.End();
      GL.Enable(EnableCap.DepthTest);
      GL.Disable(EnableCap.LineStipple);
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
    public void SetPosition(double x, double y) {
      ButtonX = x;
      ButtonY = y;
    }
    public void SetSize(double w, double h) {
      ButtonWidth = w;
      ButtonHeight = h;
    }
  }
}
