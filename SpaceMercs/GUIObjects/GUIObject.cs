using OpenTK;
using OpenTK.Windowing.Desktop;

namespace SpaceMercs {
  public abstract class GUIObject {
    public bool Active { get; protected set; }
    public float Alpha { get; protected set; }
    public GameWindow Window { get; protected set; }

    public abstract void Display(int x, int y);
    public abstract bool CaptureClick(int x, int y);
    public abstract void TrackMouse(int x, int y);
    public abstract bool IsHover(int x, int y);
    public abstract void Initialise();
    public void Activate() { Active = true; }
    public void Deactivate() { Active = false; }
  }

}
