using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using OpenTK.Windowing.Desktop;
using System.Collections.Concurrent;

namespace SpaceMercs {
  class GUIMessageBox : GUIObject {
    private double BoxX, BoxY;
    private double BoxHeight;
    private double ButtonY;
    private const double ButtonWidth = 0.065, ButtonHeight = 0.04;
    private const double ButtonBorder = 0.01, ButtonSplit = 0.03;
    private const double MaxBoxHeight = 0.8, MaxBoxWidth = 0.8;
    private readonly double ButtonAlpha;
    private readonly TextLabel tlText, tlButton1, tlButton2;
    private double TextW { get { return Math.Min(tlText.Width / 1000.0, MaxBoxWidth); } }
    private double TextH { get { return Math.Min(TextW * tlText.Height / tlText.Width, MaxBoxHeight - (ButtonHeight + ButtonBorder)); } }

    private Action? OnClick = null;
    private Action<object>? OnClickObj = null;
    private object? Obj = null;
    private bool Decision = false;

    private class MsgConfig {
      public List<string> Lines = new List<string>();
      public Action? OnClick = null;
      public Action<object>? OnClickObj = null;
      public object? Obj = null;
      public bool Decision = false;
    }
    private readonly ConcurrentQueue<MsgConfig> queue = new ConcurrentQueue<MsgConfig>();

    public GUIMessageBox(GameWindow parentWindow) {
      Alpha = 0.8f;
      ButtonAlpha = 0.8f;
      Window = parentWindow;
      Active = false;
      tlText = new TextLabel();
      tlButton1 = new TextLabel("OK");
      tlButton2 = new TextLabel("Cancel");
    }

    // Change the text to be shown on this button & update the texture. If it's the same then don't redo texture.
    public void PopupMessage(string strText) {
      List<string> lines = new List<string>(strText.Split('\n'));
      PopupMessage(lines);
    }
    public void PopupMessage(IEnumerable<string> lines) {
      if (Active) {
        queue.Enqueue(new MsgConfig() { Decision = false, Lines=new List<string>(lines), OnClick=null });
      }
      else {
        SetupBoxes(lines);
        OnClick = null;
        OnClickObj = null;
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
        OnClickObj = null;
        Decision = true;
      }
      Active = true;
    }
    public void PopupConfirmation(string strText, Action<object> _onClick, object _obj) {
      List<string> lines = new List<string>(strText.Split('\n'));
      PopupConfirmation(lines, _onClick, _obj);
    }
    public void PopupConfirmation(IEnumerable<string> lines, Action<object> _onClick, object _obj) {
      if (Active) {
        queue.Enqueue(new MsgConfig() { Decision = true, Lines = new List<string>(lines), OnClickObj = _onClick, Obj = _obj });
      }
      else {
        SetupBoxes(lines);
        OnClickObj = _onClick;
        OnClick = null;
        Obj = _obj;
        Decision = true;
      }
      Active = true;
    }
    private void SetupBoxes(IEnumerable<string> lines) {
      tlText.UpdateTextFromList(lines);
      tlText.SetTextAlign(TextLabel.TextAlign.Centre);
      BoxHeight = ButtonHeight + (ButtonBorder * 2) + TextH; // lines.Count() * 0.04;
      if (BoxHeight > 0.6) BoxHeight = 0.6;
      BoxX = (1.0 - MaxBoxWidth) / 2.0;
      BoxY = (1.0 - BoxHeight) / 2.0;
      ButtonY = (BoxY + BoxHeight) - (ButtonHeight + ButtonBorder);
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
      GL.Enable(EnableCap.Blend);
      GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

      // Draw the message box background
      GL.Color4(0.4f, 0.4f, 0.4f, Alpha);
      GL.Disable(EnableCap.DepthTest);
      GL.Begin(BeginMode.Quads);
      GL.Vertex3(BoxX, BoxY, 0.1f);
      GL.Vertex3(BoxX + MaxBoxWidth, BoxY, 0.1f);
      GL.Vertex3(BoxX + MaxBoxWidth, BoxY + BoxHeight, 0.1f);
      GL.Vertex3(BoxX, BoxY + BoxHeight, 0.1f);
      GL.End();

      // Draw the main message text
      GL.PushMatrix();
      GL.Translate(0.5, BoxY, 0.15);
      GL.Rotate(180.0, Vector3d.UnitX);    
      tlText.Draw(TextLabel.Alignment.TopMiddle, TextW, TextH);
      GL.PopMatrix();

      // Draw the framework
      GL.Color4(1.0, 1.0, 1.0, Alpha);
      GL.Begin(BeginMode.LineLoop);
      GL.Vertex3(BoxX, BoxY, 0.15f);
      GL.Vertex3(BoxX + MaxBoxWidth, BoxY, 0.15f);
      GL.Vertex3(BoxX + MaxBoxWidth, BoxY + BoxHeight, 0.15f);
      GL.Vertex3(BoxX, BoxY + BoxHeight, 0.15f);
      GL.End();

      DrawButton(xpos, ypos, 1);
      if (Decision) DrawButton(xpos, ypos, 2);
    }
    private void DrawButton(double xpos, double ypos, int id) {
      double ButtonX = (1.0 - ButtonWidth) / 2.0;
      if (Decision) {
        if (id == 1) ButtonX = 0.5 - (ButtonSplit + ButtonWidth);
        else if (id == 2) ButtonX = 0.5 + ButtonSplit;
        else throw new NotImplementedException();
      }

      // Draw the button
      if (xpos >= ButtonX && xpos <= (ButtonX + ButtonWidth) && ypos >= ButtonY && ypos <= (ButtonY + ButtonHeight)) GL.Color4(0.8f, 0.8f, 0.8f, ButtonAlpha);
      else GL.Color4(0.4f, 0.4f, 0.4f, ButtonAlpha);
      GL.Begin(BeginMode.Quads);
      GL.Vertex3(ButtonX, ButtonY, 0.2f);
      GL.Vertex3(ButtonX + ButtonWidth, ButtonY, 0.2f);
      GL.Vertex3(ButtonX + ButtonWidth, ButtonY + ButtonHeight, 0.2f);
      GL.Vertex3(ButtonX, ButtonY + ButtonHeight, 0.2f);
      GL.End();

      // Draw the button text
      GL.PushMatrix();
      if (id == 1) GL.Translate(ButtonX + 0.015, ButtonY + 0.008, 0.25);
      else GL.Translate(ButtonX, ButtonY, 0.25);
      GL.Rotate(180.0, Vector3d.UnitX);
      if (id == 1) tlButton1.Draw(TextLabel.Alignment.TopLeft, ButtonWidth * 0.55, ButtonHeight * 0.6);
      else tlButton2.Draw(TextLabel.Alignment.TopLeft, ButtonWidth, ButtonHeight);
      GL.PopMatrix();

      // Draw the framework
      GL.Color4(1.0, 1.0, 1.0, ButtonAlpha);
      GL.Begin(BeginMode.LineLoop);
      GL.Vertex3(ButtonX, ButtonY, 0.2f);
      GL.Vertex3(ButtonX + ButtonWidth, ButtonY, 0.2f);
      GL.Vertex3(ButtonX + ButtonWidth, ButtonY + ButtonHeight, 0.2f);
      GL.Vertex3(ButtonX, ButtonY + ButtonHeight, 0.2f);
      GL.End();
      GL.Enable(EnableCap.DepthTest);
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
          OnClickObj?.Invoke(Obj);
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
          OnClickObj?.Invoke(Obj);
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
        OnClickObj?.Invoke(Obj);
        CheckNext();
      }
    }

    private void CheckNext() {
      if (!queue.TryDequeue(out MsgConfig msgc)) {
        Active = false;
        return;
      }
      SetupBoxes(msgc.Lines);
      OnClick = msgc.OnClick;
      OnClickObj = msgc.OnClickObj;
      Obj = msgc.Obj;
      Decision = msgc.Decision;
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

  }
}
