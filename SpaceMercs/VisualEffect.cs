using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using SpaceMercs.Graphics.Shapes;
using System.Diagnostics;

namespace SpaceMercs {
    public class VisualEffect {
        public enum EffectType { Damage, Healing, Shot, Frag, Explosion }
        private readonly EffectType type;
        private readonly long tStart;
        private readonly Dictionary<string, object> data;
        private readonly float X, Y, lScale;

        // Delegate
        public delegate void EffectFactory(EffectType tp, float xpos, float ypos, Dictionary<string, object> dict);

        public VisualEffect(EffectType tp, float x, float y, Stopwatch sw, float scale, Dictionary<string, object> dict) {
            type = tp;
            tStart = sw.ElapsedMilliseconds;
            data = dict;
            lScale = scale;
            X = x;
            Y = y;
        }

        // Returns true if this is expired and can be removed
        public bool Display(Stopwatch sw, float aspect, Matrix4 viewM) {
            switch (type) {
                case EffectType.Damage: return DisplayDamage(sw, aspect, viewM);
                case EffectType.Healing: return DisplayHealing(sw, aspect, viewM);
                case EffectType.Shot: return DisplayShotLine(sw, aspect, viewM);
                default: throw new NotImplementedException("Undisplayable Effect Type : " + type);
            }
        }

        private bool DisplayDamage(Stopwatch sw, float aspect, Matrix4 viewM) {
            return DisplayText(sw, aspect, viewM, Color.Red);
        }
        private bool DisplayHealing(Stopwatch sw, float aspect, Matrix4 viewM) {
            return DisplayText(sw, aspect, viewM, Color.Green);
        }
        private bool DisplayText(Stopwatch sw, float aspect, Matrix4 viewM, Color col) {
            long mili = sw.ElapsedMilliseconds - tStart;
            if (mili > 800) return true; // Remove this effect after a while
            object oVal = data["Value"];
            float val = (oVal is double) ? (float)((double)oVal) : (float)oVal;
            float scale = lScale * 0.8f;
            if (mili < 400) scale *= 0.8f + (mili / 400f);
            else scale *= 1.8f - ((mili - 400f) / 300f);
            TextRenderOptions tro = new TextRenderOptions() {
                Alignment = Alignment.CentreMiddle,
                Aspect = 1f,
                TextColour = col,
                XPos = X,
                YPos = Y,
                ZPos = 0.02f,
                Scale = scale,
                FlipY = true,
                Projection = Matrix4.CreatePerspectiveFieldOfView(Const.MapViewportAngle, aspect, 0.05f, 5000.0f),
                View = viewM
            };
            TextRenderer.DrawWithOptions(val.ToString("N1"), tro);

            return false;
        }
        private bool DisplayShotLine(Stopwatch sw, float aspect, Matrix4 viewM) {
            long mili = sw.ElapsedMilliseconds - tStart;
            float fx = (float)data["FX"];
            float fy = (float)data["FY"];
            float tx = (float)data["TX"];
            float ty = (float)data["TY"];
            float pow = (float)data["Power"];
            Color col = (Color)data["Colour"];
            float fract = 1f - (mili / (pow * 50f));
            if (fract < 0.0) return true;
            // TODO Replace this code
            //GL.LineWidth(2.0f);
            //GL.Begin(BeginMode.Lines);
            //GL.Color4(col.R * fract, col.G * fract, col.B * fract, 255);
            //GL.Vertex3(fx, fy, Const.GUILayer);
            //GL.Vertex3(tx, ty, Const.GUILayer);
            //GL.End();
            //GL.LineWidth(1.0f);
            return false;
        }
        private bool DisplayShotBullets(Stopwatch sw) {
            long mili = sw.ElapsedMilliseconds - tStart;
            double fx = (double)data["FX"];
            double tx = (double)data["TX"];
            double fy = (double)data["FY"];
            double ty = (double)data["TY"];
            double speed = (double)data["Speed"];
            double len = (double)data["Length"];
            Color col = (Color)data["Colour"];
            double dt = len / speed;
            long mililast = mili - (long)dt;
            if (mililast < 0) mililast = 0;
            double dist = Math.Sqrt((fx - tx) * (fx - tx) + (fy - ty) * (fy - ty));
            if (mililast * speed > dist) return true; // Shot has hit and trail dissipated
            long milimod = (long)Math.Min(mili, dist / speed);
            double xx = fx + ((tx - fx) * milimod * speed / dist);
            double yy = fy + ((ty - fy) * milimod * speed / dist);
            double st = Math.Max(0.0, (mililast * speed) / dist);
            double sx = fx + ((tx - fx) * st);
            double sy = fy + ((ty - fy) * st);
            // TODO Replace this code
            //GL.LineWidth(2.0f);
            //GL.Begin(BeginMode.Lines);
            //double fract = (mililast - (mili - dt)) / dt;
            //GL.Color4(col.R * fract, col.G * fract, col.B * fract, 255);
            //GL.Vertex3(sx, sy, Const.GUILayer);
            //double fract2 = 1.0 - ((mili - milimod) / dt);
            //GL.Color4(col.R * fract2, col.G * fract2, col.B * fract2, 255);
            //GL.Vertex3(xx, yy, Const.GUILayer);
            //GL.End();
            //GL.LineWidth(1.0f);
            return false;
        }
    }

}
