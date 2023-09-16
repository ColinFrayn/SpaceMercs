using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using SpaceMercs.Graphics;
using SpaceMercs.Graphics.Shapes;

namespace SpaceMercs {
    public class HyperGate : AstronomicalObject {
        public Star Parent { get; set; }

        public HyperGate(Star parent, double orbit) {
            Parent = parent;
            OrbitalDistance = orbit;
        }

        public static void DrawHyperGate(ShaderProgram prog) {
            float scale = Const.PlanetScale;
            Matrix4 pScaleM = Matrix4.CreateScale(scale);
            Matrix4 modelM = pScaleM;
            prog.SetUniform("model", modelM);

            prog.SetUniform("lightEnabled", true);
            prog.SetUniform("textureEnabled", false);
            GL.UseProgram(prog.ShaderProgramHandle);
            Torus.CachedBuildAndDraw(6, 15, true);
        }

        // Overrides
        public override float DrawScale { get { return 1.0f; } }
        public override AstronomicalObjectType AOType { get { return AstronomicalObjectType.HyperGate; } }
        public override void DrawSelected(ShaderProgram prog, int Level = 8) {
            DrawHyperGate(prog);
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
            return Parent;
        }
        public override string PrintCoordinates() {
            return Parent.PrintCoordinates() + ".HG";
        }
        public override int GetPopulation() => 0;
    }
}
