using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using SpaceMercs.Graphics;
using SpaceMercs.Graphics.Shapes;
using System.IO;
using System.Xml;

namespace SpaceMercs {
    public class SpaceHulk : OrbitalAO {
        public SpaceHulk(Star parent) : base(0d, parent) {
            // Set it up with a placeholder orbit
            AxialRotationPeriod = Const.DayLength * 2.5d;
        }
        
        public void SetupSpaceHulkMissions(Random rnd, Team playerTeam) {
            Mission mh = Mission.CreateSpaceHulkMission(this, rnd, playerTeam);
            AddMission(mh);
            Mission? ma = Mission.TryCreateSpaceHulkArtifactMission(this, rnd, playerTeam);
            if (ma is not null) AddMission(ma);
        }

        // Overrides
        public override float DrawScale { get { return 1.8f; } }
        public override void DrawSelected(ShaderProgram prog, int Level, double elapsedSeconds) {
            float scale = Const.PlanetScale * DrawScale;

            Matrix4 pScaleM = Matrix4.CreateScale(scale);
            float rot = RotationAngle(elapsedSeconds);
            Matrix4 pTurnM = Matrix4.CreateRotationY(rot);
            Matrix4 pCounterRotateM = Matrix4.CreateRotationX(rot * 0.23f);
            Matrix4 modelM = pCounterRotateM * pTurnM * pScaleM;
            prog.SetUniform("model", modelM);

            prog.SetUniform("lightEnabled", true);
            prog.SetUniform("textureEnabled", false);
            GL.UseProgram(prog.ShaderProgramHandle);
            Cube.Norm.BindAndDraw();
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
        public override Planet.PlanetType Type => Planet.PlanetType.SpaceHulk;
        public override void SaveToFile(StreamWriter file, GlobalClock clock) {
            file.WriteLine(" <SpaceHulk>");
            file.WriteLine("  <Orbit>" + Math.Round(OrbitalDistance, 0).ToString() + "</Orbit>");
            SaveMissions(file);
            file.WriteLine(" </SpaceHulk>");
        }
        public void LoadFromFile(Star parent, XmlNode xml) {
            Parent = parent;
            OrbitalDistance = xml.SelectNodeDouble("Orbit", 0.0);
            LoadMissions(xml);
        }
    }
}
