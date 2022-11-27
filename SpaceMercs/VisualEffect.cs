using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using OpenTK.Mathematics;
using OpenTK.Graphics.OpenGL;

namespace SpaceMercs {
  class VisualEffect {
    public enum EffectType { Damage, Healing, Shot, Frag, Explosion }
    private readonly EffectType type;
    private readonly long tStart;
    private readonly Dictionary<string,object> data;
    private readonly double X, Y, lScale;
    private TextLabel tl = null;

    // Delegate
    public delegate void EffectFactory(EffectType tp, double xpos, double ypos, Dictionary<string, object> dict);

    public VisualEffect(EffectType tp, double x, double y, Stopwatch sw, double scale, Dictionary<string, object> dict) {
      type = tp;
      tStart = sw.ElapsedMilliseconds;
      data = dict;
      lScale = scale;
      X = x;
      Y = y;
    }

    // Returns true if this is expired and can be removed
    public bool Display(Stopwatch sw) {
      switch (type) {
        case EffectType.Damage: return DisplayDamage(sw);
        case EffectType.Healing: return DisplayHealing(sw);
        case EffectType.Shot: return DisplayShotLine(sw);
        default: throw new NotImplementedException("Undisplayable Effect Type : " + type);
      }
    }

    private bool DisplayDamage(Stopwatch sw) {
      long mili = sw.ElapsedMilliseconds - tStart;
      if (mili > 800) return true; // Remove this effect after a second
      if (tl == null) {
        double val = (double)data["Value"];
        tl = new TextLabel(val.ToString("N1"));
        tl.TextColour = Color.Red;
      }
      double dScale = lScale;
      if (mili < 400) dScale *= 0.8 + (mili / 600.0);
      else dScale *= 0.8 + ((800 - mili) / 600.0);
      //tl.SetAlpha((double)(800-mili) / 800.0);
      GL.PushMatrix();
      GL.Translate(X, Y, Const.GUILayer);
      GL.Scale(dScale, dScale, dScale);
      //tl.DrawAt(TextLabel.Alignment.CentreMiddle, 0, 0);
      GL.PopMatrix();
      return false;
    }
    private bool DisplayHealing(Stopwatch sw) {
      long mili = sw.ElapsedMilliseconds - tStart;
      if (mili > 800) return true; // Remove this effect after a second
      if (tl == null) {
        double val = (double)data["Value"];
        tl = new TextLabel(val.ToString("N1"));
        tl.TextColour = Color.Green;
      }
      double dScale = lScale;
      if (mili < 400) dScale *= 0.8 + (mili / 600.0);
      else dScale *= 0.8 + ((800 - mili) / 600.0);
      GL.PushMatrix();
      GL.Translate(X, Y, Const.GUILayer);
      GL.Scale(dScale, dScale, dScale);
      //tl.DrawAt(TextLabel.Alignment.CentreMiddle, 0, 0);
      GL.PopMatrix();
      return false;
    }
    private bool DisplayShotLine(Stopwatch sw) {
      long mili = sw.ElapsedMilliseconds - tStart;
      double fx = (double)data["FX"];
      double fy = (double)data["FY"];
      double tx = (double)data["TX"];
      double ty = (double)data["TY"];
      double pow = (double)data["Power"];
      Color col = (Color)data["Colour"];
      double fract = 1.0 - (mili / (pow * 50.0));
      if (fract < 0.0) return true;
      GL.LineWidth(2.0f);
      GL.Begin(BeginMode.Lines);
      GL.Color4(col.R * fract, col.G * fract, col.B * fract, 255);
      GL.Vertex3(fx, fy, Const.GUILayer);
      GL.Vertex3(tx, ty, Const.GUILayer);
      GL.End();
      GL.LineWidth(1.0f);
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
      GL.LineWidth(2.0f);
      GL.Begin(BeginMode.Lines);
      double fract = (mililast - (mili - dt)) / dt;
      GL.Color4(col.R * fract, col.G * fract, col.B * fract, 255);
      GL.Vertex3(sx, sy, Const.GUILayer);
      double fract2 = 1.0 - ((mili - milimod) / dt);
      GL.Color4(col.R * fract2, col.G * fract2, col.B * fract2, 255);
      GL.Vertex3(xx, yy, Const.GUILayer);
      GL.End();
      GL.LineWidth(1.0f);
      return false;
    }
  }

}
