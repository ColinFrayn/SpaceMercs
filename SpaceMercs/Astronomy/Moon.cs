using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using SpaceMercs.Graphics;
using SpaceMercs.Graphics.Shapes;
using System.IO;
using System.Xml;
using static SpaceMercs.Planet;

namespace SpaceMercs {
    public class Moon : HabitableAO {
        public override float DrawScale { get { return (float)Math.Sqrt(Radius / 1000.0) / 50f; } }

        public Moon(int _seed, Planet parent, int id) {
            ID = id;
            Seed = _seed;
            Parent = parent;
            _MissionList = null;
            Random rnd = new Random(Seed);
            Ox = rnd.Next(Const.SeedBuffer);
            Oy = rnd.Next(Const.SeedBuffer);
            Oz = rnd.Next(Const.SeedBuffer);
        }
        public Moon(XmlNode xml, Planet parent) : base(xml, parent) {
            // Bugfix - handle some dodgy data saved down because moon orbital period was wrapping as it was miscalculated too large
            if (OrbitalPeriod < 0 || AxialRotationPeriod < 0) {
                Random rnd = new Random();
                OrbitalPeriod = Utils.NextGaussian(rnd, Const.MoonOrbitalPeriod, Const.MoonOrbitalPeriodSigma);
                OrbitalPeriod /= (OrbitalDistance / Const.MoonOrbit) * Math.Pow(Radius / Const.MoonRadius, 0.5);
                AxialRotationPeriod = Utils.NextGaussian(rnd, OrbitalPeriod, OrbitalPeriod / 15f);
            }

            BaseColour = Const.PlanetTypeToCol2(Type);
        }

        public void SetupRandom(Random rnd) {
            Planet pl = Parent as Planet ?? throw new Exception("Moon'sparent is not a planet!");
            do {
                Radius = Utils.NextGaussian(rnd, Const.MoonRadius, Const.MoonRadiusSigma);
            } while (Radius < Const.MoonRadiusMin);

            OrbitalDistance = Utils.NextGaussian(rnd, Const.MoonOrbit * (double)(ID + 1), Const.MoonOrbitSigma);
            OrbitalDistance += Radius;
            bool bOK = true;
            do {
                Temperature = pl.BaseTemp - 5d * (OrbitalDistance / Const.MoonOrbit); // Base level
                double tempmod = 0.0;
                bOK = true;
                if (Temperature > 180 && Temperature < 320 && rnd.Next(4) == 0) { _type = PlanetType.Oceanic; tempmod = Utils.NextGaussian(rnd, 40, 5); }
                else {
                    int r = rnd.Next(25);
                    if (r < 2) { _type = PlanetType.Oceanic; tempmod = Utils.NextGaussian(rnd, 40, 5); }
                    else if (r < 5) { _type = PlanetType.Desert; }
                    else if (r < 9) { _type = PlanetType.Volcanic; tempmod = Utils.NextGaussian(rnd, 40, 5); }
                    else { _type = PlanetType.Rocky; tempmod = Utils.NextGaussian(rnd, 5, 5); }
                }
                Temperature += (int)tempmod;
                if (Temperature < 10) Temperature = 10;

                // Check that this is ok
                if (Temperature > 320 && Type == PlanetType.Oceanic) bOK = false;
                if (Temperature < 210 && Type == PlanetType.Oceanic) bOK = false;
                if (Temperature < 270 && Type == PlanetType.Oceanic) _type = PlanetType.Ice;
                if (Temperature < 160 && Type == PlanetType.Volcanic) bOK = false;
            } while (bOK != true);

            BaseColour = Const.PlanetTypeToCol2(Type);

            // Orbital period
            double prot = Utils.NextGaussian(rnd, Const.MoonOrbitalPeriod, Const.MoonOrbitalPeriodSigma);
            prot /= (OrbitalDistance / Const.MoonOrbit) * Math.Pow(Radius / Const.MoonRadius, 0.5);
            OrbitalPeriod = (int)prot;

            // Axial rotation
            double arot = Utils.NextGaussian(rnd, prot, prot / 15f);
            AxialRotationPeriod = (int)arot;
        }

        // Save this moon to an Xml file
        public override void SaveToFile(StreamWriter file, GlobalClock clock) {
            file.WriteLine("<Moon ID=\"" + ID.ToString() + "\">");
            if (!string.IsNullOrEmpty(Name) && !string.Equals(Name, "Unnamed")) file.WriteLine("<Name>" + Name + "</Name>");
            SaveMissions(file);
            Colony?.SaveToFile(file, clock);
            file.WriteLine("</Moon>");
        }

        public void ExpandFromXml(XmlNode xml) {
            XmlNode? xmlc = xml.SelectSingleNode("Colony");
            if (xmlc != null) SetColony(new Colony(xmlc, this));
            LoadMissions(xml);
            XmlNode? xmln = xml.SelectSingleNode("Name");
            if (xmln != null) Name = xml.SelectNodeText("Name", string.Empty);
        }

        public bool HasBeenEdited() {
            if (!string.IsNullOrEmpty(Name)) return true;
            if (Colony is not null) return true;
            if (CountMissions > 0) return true;
            if (Scanned) return true;
            return false;
        }

        // Overrides
        public override void DrawBaseIcon(ShaderProgram prog) {
            if (Colony is null) return;
            float scale = Const.PlanetScale * Const.MoonScale * 1.5f;
            Colony.DrawBaseIcon(prog, scale);
        }
        public override void DrawSelected(ShaderProgram prog, int Level, double elapsedSeconds) {
            float scale = DrawScale * Const.PlanetScale * Const.MoonScale;
            Matrix4 pScaleM = Matrix4.CreateScale(scale);
            Matrix4 pTurnM = Matrix4.CreateRotationY(RotationAngle(elapsedSeconds));
            Matrix4 pRotateM = Matrix4.CreateRotationX((float)Math.PI / 2f);
            Matrix4 modelM = pRotateM * pTurnM * pScaleM;
            prog.SetUniform("model", modelM);

            prog.SetUniform("lightEnabled", true);
            prog.SetUniform("textureEnabled", true);
            prog.SetUniform("texPos", 0f, 0f);
            prog.SetUniform("texScale", 1f, 1f);

            SetupTextureMap(32, 16);
            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, iTexture);
            GL.UseProgram(prog.ShaderProgramHandle);
            Sphere.CachedBuildAndDraw(Level, true);
            GL.BindTexture(TextureTarget.Texture2D, 0);
        }
        public override void SetupTextureMap(int width, int height) {
            if (iTexture == -1 || texture == null) iTexture = GL.GenTexture();
            else {
                if (texture.Length >= (width * height * 3)) return;
            }
            texture = Terrain.GenerateMap(this, width, height);
            GL.BindTexture(TextureTarget.Texture2D, iTexture);
            GL.TexEnv(TextureEnvTarget.TextureEnv, TextureEnvParameter.TextureEnvMode, (float)TextureEnvMode.Modulate);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgb, width, height, 0, PixelFormat.Rgb, PixelType.UnsignedByte, texture);
            Textures.SetTextureParameters();
        }
        public override void ClearData() {
            GL.DeleteTexture(iTexture);
            texture = null;
            iTexture = -1;
        }
        public override void SetName(string str) {
            // Meh
        }
        public override Star GetSystem() {
            if (Parent is Planet pl && pl.Parent is Star st) return st;
            throw new Exception($"Unexpected Parent setup for Moon in {nameof(GetSystem)}");
        }
        public override string PrintCoordinates() {
            return Parent.PrintCoordinates() + "." + ID;
        }
        public override int GetPopulation(Race? rc = null) {
            if (Colony is not null && (Colony.Owner == rc || rc is null)) {
                return Colony.BaseSize;
            }
            return 0;
        }
        public override double DistanceFromStar() {
            if (Parent is Planet pl) {
                return OrbitalDistance + pl.OrbitalDistance;
            }
            throw new Exception($"Unexpected Parent setup for Moon in {nameof(DistanceFromStar)}");
        }
        public override double HabitableScore(Race rc) {
            double raceTechCount = rc.ResearchCount * 2d;
            double score = raceTechCount + 100d - TDiff(rc);
            if (Parent is not Planet pl || pl.Colony is null) score -= 20d;
            if (Type == PlanetType.Oceanic) score += 25d;
            if (Type == PlanetType.Volcanic) score -= 10d;
            return score * 0.7d; // Moons are worse than planets
        }
    }
}
