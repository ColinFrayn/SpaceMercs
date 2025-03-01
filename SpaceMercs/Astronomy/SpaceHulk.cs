using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using SpaceMercs.Graphics;
using SpaceMercs.Graphics.Shapes;

namespace SpaceMercs {
    public class SpaceHulk : OrbitalAO {
        public SpaceHulk(Star parent, double orbit) : base(orbit, parent) {
            AxialRotationPeriod = (int)Const.DayLength * 2.5;
        }

        // Overrides
        public override float DrawScale { get { return 1.0f; } }
        public override void DrawSelected(ShaderProgram prog, int Level, float elapsedSeconds) {
            float scale = Const.PlanetScale;

            Matrix4 pScaleM = Matrix4.CreateScale(scale);
            Matrix4 pTurnM = Matrix4.CreateRotationY(elapsedSeconds * 2f * (float)Math.PI / (float)AxialRotationPeriod);
            Matrix4 pCounterRotateM = Matrix4.CreateRotationX(elapsedSeconds * 0.73f * (float)Math.PI / (float)AxialRotationPeriod);
            Matrix4 modelM = pCounterRotateM * pTurnM * pScaleM;
            prog.SetUniform("model", modelM);

            prog.SetUniform("lightEnabled", true);
            prog.SetUniform("textureEnabled", false);
            GL.UseProgram(prog.ShaderProgramHandle);
            Cube.Flat.BindAndDraw();
        }
        public override void SetupTextureMap(int width, int height) {
            // Nothing to do
        }
        public override void ClearData() {
            // Nothing to do
        }
        public override void SetName(string str) {
            // Nothing to do
        }
        public override Star GetSystem() {
            if (Parent is Star st) return st;
            throw new Exception($"SpaceHulk Gate has illegal parent type : {Parent?.GetType()}");
        }
        public override string PrintCoordinates() {
            return Parent.PrintCoordinates() + ".SH";
        }
        public override int GetPopulation() => 0;
    }
}
