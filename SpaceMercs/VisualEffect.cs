using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using SpaceMercs.Graphics;
using SpaceMercs.Graphics.Shapes;
using System.Diagnostics;
using static SpaceMercs.Delegates;

namespace SpaceMercs {
    public class VisualEffect {
        public enum EffectType { Damage, Healing, Shot, Frag, Explosion, Melee }
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
        public bool Display(Stopwatch sw, float aspect, Matrix4 viewM, ShaderProgram prog2D) {
            return type switch {
                EffectType.Damage => DisplayDamage(sw, aspect, viewM),
                EffectType.Healing => DisplayHealing(sw, aspect, viewM),
                EffectType.Shot => DisplayShot(sw, prog2D),
                EffectType.Explosion => DisplayExplosion(sw, prog2D),
                EffectType.Melee => true, // Nothing to display; remove it immediately
                _ => throw new NotImplementedException("Undisplayable Effect Type : " + type),
            };
        }

        private bool DisplayDamage(Stopwatch sw, float aspect, Matrix4 viewM) {
            return DisplayText(sw, aspect, viewM, Color.Red);
        }
        private bool DisplayHealing(Stopwatch sw, float aspect, Matrix4 viewM) {
            return DisplayText(sw, aspect, viewM, Color.Green);
        }
        private bool DisplayText(Stopwatch sw, float aspect, Matrix4 viewM, Color col) {
            long mili = sw.ElapsedMilliseconds - tStart;
            if (mili > 600) return true; // Remove this effect after a while
            float fract = ((float)mili / 600f);
            object oVal = data["Value"];
            float val = (oVal is double) ? (float)((double)oVal) : (float)oVal;
            float scale = lScale * 0.8f;
            if (fract < 0.5) scale *= 0.8f + fract;
            else scale *= 1.3f - ((fract - 0.5f) * 2f);
            TextRenderOptions tro = new TextRenderOptions() {
                Alignment = Alignment.CentreMiddle,
                Aspect = 1f,
                TextColour = col,
                XPos = X,
                YPos = Y + (fract * 1.5f),
                ZPos = 0.02f,
                Scale = scale,
                FlipY = true,
                Projection = Matrix4.CreatePerspectiveFieldOfView(Const.MapViewportAngle, aspect, 0.05f, 5000.0f),
                View = viewM
            };
            TextRenderer.DrawWithOptions(val.ToString("N1"), tro);

            return false;
        }
        private bool DisplayShot(Stopwatch sw, ShaderProgram prog2D) {
            long mili = sw.ElapsedMilliseconds - tStart;
            float delay = (float)data["Delay"];
            if (mili < delay) return false;
            float duration = (float)data["Duration"];
            float fract = (mili - delay) / duration;
            if (fract < 0f) fract = 0f;
            if (fract > 1f) return true;

            float fx = (float)data["FX"];
            float fy = (float)data["FY"];
            float tx = (float)data["TX"];
            float ty = (float)data["TY"];
            float size = (float)data["Size"];
            Color col = (Color)data["Colour"];
            if (data.TryGetValue("Length", out object? oLength)) {
                float length = (float)oLength;
                Vector2 tVec = new Vector2(tx - fx, ty - fy);
                float tLen = tVec.Length;
                if (tLen > 0f) {
                    float fEnd = Math.Min(fract, 1.0f);
                    float fStart = Math.Max(0f, fract - length / tLen);
                    tx = fx + fEnd * tVec.X;
                    ty = fy + fEnd * tVec.Y;
                    fx += fStart * tVec.X;
                    fy += fStart * tVec.Y;
                }
            }

            Vector4 vCol = new Vector4((float)col.R / 255f, (float)col.G / 255f, (float)col.B / 255f, 1f);
            prog2D.SetUniform("flatColour", vCol);
            GL.UseProgram(prog2D.ShaderProgramHandle);

            ThickLine line = ThickLine.Make_Vertex3D(fx, fy, 0f, tx, ty, 0f, size);

            line.BindAndDraw();

            return false;
        }
        private bool DisplayExplosion(Stopwatch sw, ShaderProgram prog2D) {
            long mili = sw.ElapsedMilliseconds - tStart;
            float duration = (float)data["Duration"];
            float fract = mili / duration;
            if (fract < 0f) fract = 0f;
            if (fract > 1f) return true;

            float size = (float)data["Size"];
            if (fract < 0.8) size *= fract * 1.25f;
            Color col = (Color)data["Colour"];

            GL.Enable(EnableCap.Blend);
            GL.Disable(EnableCap.DepthTest);

            Matrix4 scaleM = Matrix4.CreateScale(size);
            Matrix4 transOriginM = Matrix4.CreateTranslation(new Vector3(X, Y, 0.02f));
            prog2D.SetUniform("model", scaleM * transOriginM);

            Vector4 vCol = new Vector4((float)col.R / 255f, (float)col.G / 255f, (float)col.B / 255f, 0.5f);
            prog2D.SetUniform("flatColour", vCol);

            GL.UseProgram(prog2D.ShaderProgramHandle);
            Disc.Disc32.BindAndDraw();
            //Annulus.Annulus32.BindAndDraw();

            prog2D.SetUniform("model", Matrix4.Identity);
            GL.Enable(EnableCap.DepthTest);
            GL.Disable(EnableCap.Blend);

            return false;
        }

        public void ResolveEffect(MissionLevel level, EffectFactory effectFactory, ItemEffect.ApplyItemEffect applyEffect, ShowMessageDelegate showMessage, PlaySoundDelegate playSound) {
            if (type != EffectType.Shot && type != EffectType.Melee) return;
            int tx = (int)X;
            int ty = (int)Y;

            if (data.TryGetValue("Effect", out object? effect) && effect is ItemEffect iEffect) {
                Color col = Color.FromArgb(150, 150, 50, 50);
                if (iEffect.Radius > 0d) {
                    effectFactory(EffectType.Explosion, tx + 0.5f, ty + 0.5f, new Dictionary<string, object>() { { "Duration", 250f }, { "Size", (float)iEffect.Radius }, { "Colour", col } });
                }
                applyEffect(null, iEffect, tx, ty);
                return;
            }
            if (!data.TryGetValue("Result", out object? oResult) || oResult is not ShotResult result) return;
            if (!result.Hit) return;
            Weapon? wp = null;
            IEntity? source = null;
            if (result.Source is IEntity src) {
                wp = src.EquippedWeapon;
                source = src;
            }

            HashSet<IEntity> hsAttacked = new HashSet<IEntity>();

            // Is this an AoE Weapon then resolve the AoE
            if (wp?.Type?.WeaponShotType == WeaponType.ShotType.Grenade) {
                int r = (int)Math.Ceiling(wp?.Type?.Area ?? 0d);
                Color col = Color.FromArgb(150, 150, 50, 50);
                effectFactory(EffectType.Explosion, tx + 0.5f, ty + 0.5f, new Dictionary<string, object>() { { "Duration", 250f }, { "Size", (float)r }, { "Colour", col } });
                for (int y = Math.Max(0, ty - r); y <= Math.Min(level.Height - 1, ty + r); y++) {
                    for (int x = Math.Max(0, tx - r); x <= Math.Min(level.Width - 1, tx + r); x++) {
                        int dr2 = (ty - y) * (ty - y) + (tx - x) * (tx - x);
                        if (dr2 > r * r) continue;
                        IEntity? en = level.GetEntityAt(x, y);
                        if (en != null && !hsAttacked.Contains(en)) {
                            hsAttacked.Add(en);
                        }
                    }
                }
            }
            // Single target shot
            else {
                IEntity? en = level.GetEntityAt(tx, ty);
                if (en is null) return; // Weird!
                hsAttacked.Add(en);
            }

            // Sort out graphics for damage (if tgt is still alive)
            playSound("Smash");
            Utils.ResolveHits(hsAttacked, wp, source, effectFactory, applyEffect, showMessage);
        }
    }
}
