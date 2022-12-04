using OpenTK.Windowing.Desktop;
using SpaceMercs.Graphics;

namespace SpaceMercs {
    internal abstract class GUIObject {
        public bool Active { get; protected set; }
        public float Alpha { get; private set; }
        public GameWindow Window { get; private set; }

        public GUIObject(GameWindow window, bool bActive, float alpha) {
            Window = window;
            Active = bActive;
            Alpha = alpha;
        }

        public abstract void Display(int x, int y, ShaderProgram prog);
        public abstract bool CaptureClick(int x, int y);
        public abstract void TrackMouse(int x, int y);
        public abstract bool IsHover(int x, int y);
        public abstract void Initialise();
        public void Activate() { Active = true; }
        public void Deactivate() { Active = false; }
    }

}