using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using SpaceMercs.Graphics;
using SpaceMercs.Graphics.Shapes;
using System.Diagnostics;
using static SpaceMercs.Delegates;

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
        public bool Display(Stopwatch sw, float aspect, Matrix4 viewM, ShaderProgram prog2D) {
            switch (type) {
                case EffectType.Damage: return DisplayDamage(sw, aspect, viewM);
                case EffectType.Healing: return DisplayHealing(sw, aspect, viewM);
                case EffectType.Shot: return DisplayShot(sw, prog2D);
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
        private bool DisplayShot(Stopwatch sw, ShaderProgram prog2D) {
            long mili = sw.ElapsedMilliseconds - tStart;
            float delay = (float)data["Delay"];
            if (mili < delay) return false;
            float duration = (float)data["Duration"];
            float fract = (mili-delay) / duration;
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

            GL.UseProgram(prog2D.ShaderProgramHandle);

            ThickLine2D line = ThickLine2D.Make_VertexPos2DCol(fx, fy, tx, ty, size, col);

            line.BindAndDraw();

            return false;
        }

        public void ResolveEffect(EffectFactory effectFactory, ItemEffect.ApplyItemEffect applyEffect, ShowMessage showMessage) {
            if (type != EffectType.Shot) return;
            if (!data.TryGetValue("Result", out object? oResult) || oResult is not ShotResult result) return;
            if (result.Damage is null || result.Damage.Count == 0) return;
            if (result.Target is null || result.Target is not IEntity tgt) return;
            // Graphics for damage
            double TotalDam = tgt.CalculateDamage(result.Damage);
            Weapon? wp = null;
            IEntity? source = null;
            if (data.TryGetValue("Source", out object? oSrc) && oSrc is IEntity src) {
                wp = src.EquippedWeapon;
                source = src;
            }
            effectFactory(EffectType.Damage, tgt.X + (tgt.Size / 2f), tgt.Y + (tgt.Size / 2f), new Dictionary<string, object>() { { "Value", TotalDam } });
            tgt.InflictDamage(result.Damage, applyEffect);
            if (tgt is Creature cr && cr.Health > 0.0) cr.CheckChangeTarget(TotalDam, source);

            // Apply effect?
            if (wp != null) {
                if (wp.Type.ItemEffect != null) {
                    result.Target.ApplyEffectToEntity(source, wp.Type.ItemEffect, effectFactory, applyEffect);
                }
            }

            // Add weapon experience if shot was with a weapon and from a soldier
            if (source is Soldier s && wp != null && tgt is not null) {
                int exp = Math.Max(1, tgt.Level - s.Level) * Const.DEBUG_WEAPON_SKILL_MOD;
                s.AddWeaponExperience(wp, exp, showMessage);
            }
        }
    }
}
