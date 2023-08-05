using OpenTK.Windowing.Desktop;
using SpaceMercs.MainWindow;

namespace SpaceMercs {
    static class Program {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main() {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            try {
                NativeWindowSettings nws =
                  new NativeWindowSettings {
                      Size = (700, 500),
                      Title = $"SpaceMercs v{Const.strVersion}",
                      StartVisible = false,
                      StartFocused = true,
                      MinimumSize = (500, 400),
                      API = OpenTK.Windowing.Common.ContextAPI.OpenGL,
                      //Profile = OpenTK.Windowing.Common.ContextProfile.Core,
                      APIVersion = new Version(4, 6)
                  };
                using (MapView window = new MapView(GameWindowSettings.Default, nws)) {
                    window.Run();
                }
            }
            catch (Exception ex) {
                MessageBox.Show("A fatal error has occurred : " + ex);
            }
        }
    }
}
