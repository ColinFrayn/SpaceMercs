using OpenTK.Windowing.Common.Input;
using OpenTK.Windowing.Desktop;
using SpaceMercs.MainWindow;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;

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
                      MinimumSize = (600, 600*5/7),
                      AspectRatio = (700, 500),
                      API = OpenTK.Windowing.Common.ContextAPI.OpenGL,
                      APIVersion = new Version(4, 6),
                      Icon = CreateWindowIcon(),
                  };
                using (MapView window = new MapView(GameWindowSettings.Default, nws)) {
                    window.Run();
                }
            }
            catch (Exception ex) {
                MessageBox.Show("A fatal error has occurred : " + ex);
            }
        }

        public static WindowIcon? CreateWindowIcon() {
            string strBitmapFile = Path.Combine(StaticData.GraphicsLocation, @"Icons\Icon1.bmp");
            if (!File.Exists(strBitmapFile)) return null;
            Bitmap TextureBitmap = new Bitmap(strBitmapFile);
            BitmapData bmpData = TextureBitmap.LockBits(new Rectangle(0, 0, TextureBitmap.Width, TextureBitmap.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            int texture_size = bmpData.Width * bmpData.Height * 4;
            byte[] bytes = new byte[texture_size];
            Marshal.Copy(bmpData.Scan0, bytes, 0, texture_size);
            // Flip R and B for some reason.
            for (int i = 0; i < bmpData.Width * bmpData.Height; i++) {
                byte temp = bytes[i * 4 + 0];
                bytes[i * 4 + 0] = bytes[i * 4 + 2];
                bytes[i * 4 + 2] = temp;
            }
            var windowIcon = new WindowIcon(new OpenTK.Windowing.Common.Input.Image(bmpData.Width, bmpData.Height, bytes));
            return windowIcon;
        }
    }
}
