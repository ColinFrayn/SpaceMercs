using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using SpaceMercs.Graphics;
using SpaceMercs.Graphics.Shapes;
using System.IO;
using System.Xml;

namespace SpaceMercs {
    public class Planet : HabitableAO {
        [Flags]
        public enum PlanetType { Rocky = 0x1, Desert = 0x2, Volcanic = 0x4, Gas = 0x8, Oceanic = 0x10, Ice = 0x20, Star = 0x40 };
        public readonly List<Moon> Moons;
        public double tempbase;
        public override float DrawScale { get { return (float)Math.Pow(Radius / 1000.0, 0.4) / 25f; } }

        public Planet() {
            Parent = Star.Empty;
            Moons = new List<Moon>();
        }
        public Planet(int _seed, Star parent) {
            Parent = parent;
            Moons = new List<Moon>();
            _MissionList = null;
            Name = "";
            Seed = _seed;
            Random rnd = new Random(Seed);
            Ox = rnd.Next(Const.SeedBuffer);
            Oy = rnd.Next(Const.SeedBuffer);
            Oz = rnd.Next(Const.SeedBuffer);
        }
        public Planet(XmlNode xml, Star parent) : base(xml, parent) {
            // Load planet-specific stuff
            tempbase = xml.SelectNodeDouble("TempBase");

            Moons = new List<Moon>();
            XmlNode? xmlMoons = xml.SelectSingleNode("Moons");
            if (xmlMoons != null) {
                foreach (XmlNode xmlm in xmlMoons.ChildNodes) {
                    Moon mn = new Moon(xmlm, this);
                    Moons.Add(mn);
                }
            }
            else {
                GenerateMoons(Parent.GetSystem().Sector.ParentMap.PlanetDensity);
            }
            colour = Const.PlanetTypeToCol2(Type);
        }

        public static Planet Empty { get { return new Planet(); } }

        // Save this planet to an Xml file
        public override void SaveToFile(StreamWriter file) {
            file.WriteLine("<Planet ID=\"" + ID.ToString() + "\">");
            base.SaveToFile(file);
            // Write planet details to file
            file.WriteLine("<TempBase>" + tempbase.ToString() + "</TempBase>");
            // Now write out all moons, if necessary
            if (HasBeenEdited()) {
                file.WriteLine("<Moons>");
                foreach (Moon mn in Moons) {
                    mn.SaveToFile(file);
                }
                file.WriteLine("</Moons>");
            }
            file.WriteLine("</Planet>");
        }

        // Retrieve a moon from this system by ID
        public Moon? GetMoonByID(int ID) {
            if (ID < 0 || ID >= Moons.Count) return null;
            return Moons[ID];
        }

        // Are we hovering over anything here?
        public AstronomicalObject? GetHover(double mousex, double mousey, double px, double py) {
            py += Const.MoonGap * Const.PlanetScale;
            foreach (Moon mn in Moons) {
                if (Math.Abs(px - mousex) < (mn.DrawScale * Const.PlanetScale * Const.MoonScale * Const.SystemViewSelectionTolerance) && Math.Abs(py - mousey) < (mn.DrawScale * Const.PlanetScale * Const.MoonScale * Const.SystemViewSelectionTolerance)) return mn;
                py += (4.5 * Const.PlanetScale * Const.MoonScale);
            }
            return null;
        }

        // Build texture maps for the planets' radial brightness maps
        public static void BuildPlanetHalo() {
            double RMin = 0.8;
            GL.ActiveTexture(TextureUnit.Texture0);
            Textures.iPlanetHalo = GL.GenTexture();
            Textures.bytePlanetHalo = new byte[Textures.PlanetHaloTextureSize * Textures.PlanetHaloTextureSize * 3];
            for (int y = 0; y < Textures.PlanetHaloTextureSize; y++) {
                double dy = ((double)Textures.PlanetHaloTextureSize / 2.0) - ((double)y + 0.5);
                for (int x = 0; x < Textures.PlanetHaloTextureSize; x++) {
                    double dx = ((double)Textures.PlanetHaloTextureSize / 2.0) - ((double)x + 0.5);
                    double r = Math.Pow((dx * dx) + (dy * dy), 0.5) * 2.0 / (double)Textures.PlanetHaloTextureSize;
                    byte val;
                    if (r > 1.0) val = 0;
                    else if (r < RMin) val = 255;
                    else val = (byte)((1.0 - ((r - RMin) / (1.0 - RMin))) * 255.0);
                    Textures.bytePlanetHalo[((y * Textures.PlanetHaloTextureSize) + x) * 3 + 0] = val;
                    Textures.bytePlanetHalo[((y * Textures.PlanetHaloTextureSize) + x) * 3 + 1] = val;
                    Textures.bytePlanetHalo[((y * Textures.PlanetHaloTextureSize) + x) * 3 + 2] = val;
                }
            }
            GL.BindTexture(TextureTarget.Texture2D, Textures.iPlanetHalo);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgb, Textures.PlanetHaloTextureSize, Textures.PlanetHaloTextureSize, 0, PixelFormat.Rgb, PixelType.UnsignedByte, Textures.bytePlanetHalo);
            Textures.SetParameters();
            GL.TexEnv(TextureEnvTarget.TextureEnv, TextureEnvParameter.TextureEnvMode, (float)TextureEnvMode.Modulate);
        }

        // Draw a halo-type atmosphere effect round a planet
        public void DrawHalo(ShaderProgram prog) {
            if (Type == PlanetType.Gas) return;

            float pscale = DrawScale * Const.PlanetScale * 2.16f;
            if (Type == PlanetType.Oceanic) pscale *= 1.1f;
            if (Type == PlanetType.Ice) pscale *= 1.05f;

            Matrix4 pScaleM = Matrix4.CreateScale(pscale);
            Matrix4 pTranslationM = Matrix4.CreateTranslation(-0.5f, -0.5f, 0f);
            prog.SetUniform("model", pTranslationM * pScaleM);

            prog.SetUniform("flatColour", new Vector4(1f, 1f, 1f, 1f));
            prog.SetUniform("textureEnabled", true);
            prog.SetUniform("lightEnabled", false);
            prog.SetUniform("texPos", 0f, 0f);
            prog.SetUniform("texScale", 1f, 1f);

            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, Textures.iPlanetHalo);

            GL.UseProgram(prog.ShaderProgramHandle);
            Square.Textured.BindAndDraw();
        }

        // Generate the moons for this planet
        public void GenerateMoons(int pdensity, int minMoons = 0) {
            Random rnd = new Random(Seed);
            Moons.Clear();
            // Get number of moons, based on planet density setting and planet size
            int nmn = rnd.Next(pdensity + 1) + rnd.Next(pdensity + 1) + rnd.Next(pdensity + 1) - 3;
            if (Temperature > 350) nmn -= rnd.Next(2);
            if (Temperature > 400) nmn -= rnd.Next(2);
            if (Temperature > 450) nmn -= rnd.Next(2);
            if (Temperature > 500) nmn -= rnd.Next(2);
            if (Temperature > 550) nmn -= rnd.Next(2);
            if (Temperature > 600) nmn -= rnd.Next(2);
            if (Temperature < 170) nmn -= rnd.Next(2);
            if (Temperature < 150) nmn -= rnd.Next(2);
            if (Temperature < 130) nmn -= rnd.Next(2);
            if (Temperature < 110) nmn -= rnd.Next(2);
            if (Radius < 4 * Const.Million) nmn -= rnd.Next(2) + 1;
            if (Radius < 3.5 * Const.Million) nmn -= rnd.Next(2) + 1;
            if (Radius < 3 * Const.Million) nmn -= rnd.Next(2) + 1;
            if (Radius < 2.5 * Const.Million) nmn -= rnd.Next(2) + 1;
            if (Radius < 2 * Const.Million) nmn -= rnd.Next(2) + 1;
            if (Type != PlanetType.Gas) nmn /= 3;
            if (nmn > 7) nmn = 7 + rnd.Next(2);
            if (nmn < minMoons) nmn = minMoons;

            // Generate moons
            for (int n = 0; n < nmn; n++) {
                Moon mn = new Moon(rnd.Next(10000000), this, n);

                do {
                    mn.Radius = Utils.NextGaussian(rnd, Const.MoonRadius, Const.MoonRadiusSigma);
                } while (mn.Radius < Const.MoonRadiusMin);

                mn.OrbitalDistance = Utils.NextGaussian(rnd, Const.MoonOrbit * (double)(n + 1), Const.MoonOrbitSigma);
                mn.OrbitalDistance += Radius;
                bool bOK = true;
                do {
                    mn.Temperature = Temperature - 40; // Base = planet's temperature minus 40 degrees
                    double tempmod = 0.0;
                    bOK = true;
                    if (mn.Temperature > 180 && mn.Temperature < 320 && rnd.Next(4) == 0) { mn.Type = PlanetType.Oceanic; tempmod = Utils.NextGaussian(rnd, 40, 5); }
                    else {
                        int r = rnd.Next(25);
                        if (r < 2) { mn.Type = PlanetType.Oceanic; tempmod = Utils.NextGaussian(rnd, 40, 5); }
                        else if (r < 5) { mn.Type = PlanetType.Desert; }
                        else if (r < 9) { mn.Type = PlanetType.Volcanic; tempmod = Utils.NextGaussian(rnd, 40, 5); }
                        else { mn.Type = PlanetType.Rocky; tempmod = Utils.NextGaussian(rnd, 5, 5); }
                    }
                    mn.Temperature += (int)tempmod;

                    // Check that this is ok
                    if (mn.Temperature > 320 && mn.Type == PlanetType.Oceanic) bOK = false;
                    if (mn.Temperature < 210 && mn.Type == PlanetType.Oceanic) bOK = false;
                    if (mn.Temperature < 270 && mn.Type == PlanetType.Oceanic) mn.Type = PlanetType.Ice;
                    if (mn.Temperature < 160 && mn.Type == PlanetType.Volcanic) bOK = false;
                } while (bOK != true);

                mn.colour = Const.PlanetTypeToCol2(mn.Type);

                // Orbital period
                double prot = Utils.NextGaussian(rnd, Const.MoonOrbitalPeriod, Const.MoonOrbitalPeriodSigma);
                prot /= (mn.OrbitalDistance / Const.MoonOrbit) * Math.Pow(mn.Radius / Const.MoonRadius, 0.5);
                mn.OrbitalPeriod = (int)prot;

                // Axial rotation
                double arot = Utils.NextGaussian(rnd, prot, prot / 15f);
                mn.AxialRotationPeriod = (int)arot;

                Moons.Add(mn);
            }
        }

        // Randomly expand a moon base (as part of system generation)
        public int ExpandMoonBase(Random rand, Race rc) {
            if (Moons.Count == 0) return 0;
            int mno = rand.Next(Moons.Count);
            Moon mn = Moons[mno];
            if (mn.BaseSize == 4) return 0;
            double tdiff = mn.TDiff(rc);
            if (mn.Colony is not null) tdiff -= 5; // More likely to expand an existing colony
            if (rand.NextDouble() * 100.0 > tdiff) {
                return mn.ExpandBase(rc, rand);
            }
            return 0;
        }

        // Check if the moons for this planet have been changed in any way, or if they can be recreated from the random seed
        private bool HasBeenEdited() {
            if (Moons.Count == 0) return false;
            foreach (Moon mn in Moons) {
                if (!string.IsNullOrEmpty(mn.Name)) return true;
                if (mn.Colony is not null) return true;
            }
            return false;
        }

        // Overrides
        public override void DrawBaseIcon(ShaderProgram prog) {
            if (Colony is null) return;
            float scale = Const.PlanetScale * 1.8f;
            Colony.DrawBaseIcon(prog, scale);
        }
        public override void DrawSelected(ShaderProgram prog, int Level = 8) {
            float scale = DrawScale * Const.PlanetScale;
            Matrix4 pScaleM = Matrix4.CreateScale(scale);
            Matrix4 pTurnM = Matrix4.CreateRotationY((float)Const.ElapsedSeconds * 2f * (float)Math.PI / (float)AxialRotationPeriod);
            Matrix4 pRotateM = Matrix4.CreateRotationX((float)Math.PI / 2f);
            Matrix4 modelM = pRotateM * pTurnM * pScaleM;
            prog.SetUniform("model", modelM);

            prog.SetUniform("lightEnabled", true);
            prog.SetUniform("textureEnabled", true);
            prog.SetUniform("texPos", 0f, 0f);
            prog.SetUniform("texScale", 1f, 1f);
            GL.ActiveTexture(TextureUnit.Texture0);
            SetupTextureMap(64, 32);
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
            Textures.SetParameters();
        }
        public override void ClearData() {
            GL.DeleteTexture(iTexture);
            iTexture = -1;
            texture = null;
            foreach (Moon mn in Moons) {
                mn.ClearData();
            }
        }
        public override void SetName(string str) {
            Name = str;
        }
        public override Star GetSystem() {
            if (Parent is Star st) return st;
            throw new Exception($"Parent of Planet was not a star. Was {Parent?.GetType()}");
        }
        public override string PrintCoordinates() {
            return Parent.PrintCoordinates() + "." + ID;
        }
        public override int GetPopulation() {
            int pop = Colony?.BaseSize ?? 0;
            foreach (Moon mn in Moons) {
                pop += mn.GetPopulation();
            }
            return pop;
        }
    }
}
