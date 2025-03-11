using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using SpaceMercs.Graphics;
using SpaceMercs.Graphics.Shapes;
using System.IO;
using System.Xml;

namespace SpaceMercs {
    public class Planet : HabitableAO {
        [Flags]
        public enum PlanetType { Rocky = 0x1, Desert = 0x2, Volcanic = 0x4, Gas = 0x8, Oceanic = 0x10, Ice = 0x20, Star = 0x40, HyperGate = 0x80, SpaceHulk = 0x100 };
        public readonly List<Moon> Moons;
        public double BaseTemp { get; private set; }
        public override float DrawScale { get { return (float)Math.Pow(Radius / 1000.0, 0.4) / 25f; } }
        public bool IsHomeworld { get; private set; }
        public bool IsPrecursor { get; private set; }

        public Planet() {
            Parent = Star.Empty;
            Moons = new List<Moon>();
        }
        public Planet(int _seed, Star parent, int npl) {
            Parent = parent;
            ID = parent.Planets.Count;
            Moons = new List<Moon>();
            _MissionList = null;
            Name = string.Empty;
            Seed = _seed;
            GeneratePlanetDetails(npl);
        }
        public Planet(XmlNode xml, Star parent) : base(xml, parent) {
            BaseTemp = xml.SelectNodeDouble("TempBase");
            IsHomeworld = (xml.SelectSingleNode("Homeworld") is not null);
            IsPrecursor = (xml.SelectSingleNode("Precursor") is not null);
            Moons = new List<Moon>();
            XmlNode? xmlMoons = xml.SelectSingleNode("Moons");
            if (IsHomeworld) {
                GenerateMoons(Const.HomeworldPDensity, Const.HomeworldMinMoons);
            }
            else GenerateMoons(Parent.GetSystem().Sector.ParentMap.PlanetDensity);

            if (xmlMoons != null) {
                foreach (XmlNode xmlm in xmlMoons.ChildNodes) {
                    string id = xmlm.Attributes?["ID"]?.Value ?? throw new Exception($"Could not read Moon ID : Planet {this}");
                    int mno = Int32.Parse(id);
                    // We might be loading an old game save, in which case just kill this moon (and any after it) and hope we can cope.
                    // This is because old game saves might not regenerate moon systems identically to the current code version.
                    // Note, even if this moon has an ID that previously existed, we could still have generated the moon system differently from expectations.
                    // But seeing as all moons are habitable, we can deal with it.
                    // If the player happened to be at this location then they're screwed. I guess I'll attempt to fix that in the player team loader??
                    if (mno >= Moons.Count) {
                        break;
                    }
                    Moon mn = Moons[mno];
                    mn.ExpandFromXml(xmlm);
                }
            }
            BaseColour = Const.PlanetTypeToCol2(Type);
        }

        public static Planet Empty { get { return new Planet(); } }

        public static Planet MakeFromSeed(Star parent, int seed, int npl) {
            Planet pl = new Planet() {
                Parent = parent,
                ID = parent.Planets.Count,
                Name = string.Empty,
                Seed = seed
            };
            pl.GeneratePlanetDetails(npl);
            return pl;
        }

        // Save this planet to an Xml file
        public override void SaveToFile(StreamWriter file, GlobalClock clock) {
            if (!PlanetOrMoonsHaveBeenEdited()) {
                file.WriteLine($"<Planet Seed=\"{Seed}\" />");
                return;
            }
            file.WriteLine($"<Planet ID=\"{ID}\">");
            base.SaveToFile(file, clock);
            // Write planet details to file
            if (IsHomeworld) file.WriteLine("<Homeworld/>");
            if (IsPrecursor) file.WriteLine("<Precursor/>");
            file.WriteLine($"<TempBase>{BaseTemp}</TempBase>");
            // Now write out any moons necessary
            bool writeMoons = false;
            foreach (Moon mn in Moons) {
                if (mn.HasBeenEdited()) {
                    if (!writeMoons) file.WriteLine("<Moons>");
                    mn.SaveToFile(file, clock);
                    writeMoons = true;
                }
            }
            if (writeMoons) file.WriteLine("</Moons>");
            file.WriteLine("</Planet>");
        }

        private void GeneratePlanetDetails(int npl) {
            Random rnd = new Random(Seed);

            Ox = rnd.Next(Const.SeedBuffer);
            Oy = rnd.Next(Const.SeedBuffer);
            Oz = rnd.Next(Const.SeedBuffer);

            Star st = Parent as Star ?? throw new Exception("Planet parent is not a star!");
            double porbit = st.Planets.LastOrDefault()?.OrbitalDistance - st.Radius * 2d ?? Const.PlanetOrbit * st.Mass;

            // Get orbit for this planet
            do {
                porbit *= Utils.NextGaussian(rnd, Const.PlanetOrbitFactor, Const.PlanetOrbitFactorSigma);
            } while (rnd.Next(npl + 2) == 0);

            // Generate the planet details
            bool bOK = true;
            do {
                // Calculate temperature at this location
                // 280K * (T/Ts) * (R/Rs)^1/2 * (1-A)^1/4 / a^1/2  + modifier_for_atmosphere
                // (a = orbit in AU, A = albedo, T = star temperature, R = stellar radius)
                BaseTemp = 300.0 / Math.Pow(porbit / Const.AU, 0.5);
                BaseTemp *= (Parent.Temperature / Const.SunTemperature) * Math.Pow(Parent.Radius / Const.SunRadius, 0.5); // Scale by the star's properties

                double tempmod = 0.0;
                double albedo = 0.3;
                bOK = true;
                if (BaseTemp > 180 && BaseTemp < 350 && rnd.Next(3) == 0) { _type = Planet.PlanetType.Oceanic; albedo = 0.3; tempmod = Utils.NextGaussian(rnd, 30, 5); }
                else {
                    int r = rnd.Next(30);
                    if (r < 2) { _type = Planet.PlanetType.Oceanic; albedo = 0.3; tempmod = Utils.NextGaussian(rnd, 30, 5); }
                    else if (r < 5) { _type = Planet.PlanetType.Desert; albedo = 0.4; }
                    else if (r < 9) { _type = Planet.PlanetType.Volcanic; albedo = 0.18; tempmod = Utils.NextGaussian(rnd, 10, 2); }
                    else if (r < 15) { _type = Planet.PlanetType.Rocky; albedo = 0.25; }
                    else { _type = Planet.PlanetType.Gas; albedo = 0.5; }
                }
                BaseTemp *= Math.Pow(1.0 - albedo, 0.25);
                BaseTemp += tempmod;
                Temperature = (int)BaseTemp;

                // Check that this is ok
                if (BaseTemp > 400 && Type == Planet.PlanetType.Gas) bOK = false;
                if (BaseTemp > 320 && Type == Planet.PlanetType.Oceanic) bOK = false;
                if (BaseTemp < 160 && Type == Planet.PlanetType.Oceanic) bOK = false;
                if (BaseTemp < 270 && Type == Planet.PlanetType.Oceanic) _type = Planet.PlanetType.Ice;
                if (BaseTemp < 180 && Type == Planet.PlanetType.Volcanic) bOK = false;
            } while (bOK == false);

            // Get radius based on type
            do {
                Radius = Utils.NextGaussian(rnd, Const.PlanetSize, Const.PlanetSizeSigma);
            } while (Radius < Const.PlanetSizeMin);
            if (Type == Planet.PlanetType.Gas) {
                Radius *= Utils.NextGaussian(rnd, Const.GasGiantScale, Const.GasGiantScaleSigma);
            }
            OrbitalDistance = porbit + (Parent.Radius * 2.0);

            BaseColour = Const.PlanetTypeToCol2(Type);

            // Orbital period
            double prot = Utils.NextGaussian(rnd, Const.EarthOrbitalPeriod, Const.EarthOrbitalPeriodSigma);
            prot /= ((OrbitalDistance / Const.AU) * Math.Pow(Radius / Const.PlanetSize, 0.5));
            OrbitalPeriod = (int)prot;

            // Axial rotation period (i.e. a day length)
            double arot = Utils.NextGaussian(rnd, Const.DayLength, Const.DayLengthSigma);
            AxialRotationPeriod = (int)(arot * (Radius / Const.PlanetSize));

            // Is this a precursor location? Needs to be in third ring or further.
            Star star = GetSystem();
            Sector sector = star.Sector;
            if (Math.Abs(sector.SectorX) > 2 || Math.Abs(sector.SectorY) > 2) {
                if (Type is not PlanetType.Gas && BaseTemp >= 180 && BaseTemp <= 400) {
                    if (star.IsStableMainSequence()) {
                        if (rnd.NextDouble() > 0.8) {
                            IsPrecursor = true;
                        }
                    }
                }
            }

            GenerateMoons(GetSystem().Sector.ParentMap.PlanetDensity);
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
                mn.SetupRandom(rnd);
                Moons.Add(mn);
            }
        }

        // Randomly expand a moon base (as part of system generation)
        public int ExpandMoonBase(Random rand, Race rc, GlobalClock clock) {
            if (Moons.Count == 0) return 0;
            int mno = rand.Next(Moons.Count);
            Moon mn = Moons[mno];
            if (mn.BaseSize == 4) return 0;
            double tdiff = mn.TDiff(rc);
            if (mn.Colony is not null) tdiff -= 5; // More likely to expand an existing colony
            if (rand.NextDouble() * 100.0 > tdiff) {
                return mn.ExpandBase(rc, rand, clock);
            }
            return 0;
        }

        // Check if the moons for this planet have been changed in any way, or if they can be recreated from the random seed
        private bool MoonsHaveBeenEdited() {
            foreach (Moon mn in Moons) {
                if (mn.HasBeenEdited()) return true;
            }
            return false;
        }
        public bool PlanetOrMoonsHaveBeenEdited() {
            if (MoonsHaveBeenEdited()) return true;
            if (!string.IsNullOrEmpty(Name)) return true;
            if (Colony is not null) return true;
            if (Scanned) return true;
            if (CountMissions > 0) return true;
            return false;
        }

        public void SetAsHomeworld() {
            IsHomeworld = true;
        }

        public void SetupPrecursorMissions(Random rnd) {
            Mission mp = Mission.CreatePrecursorMission(this, rnd);
            AddMission(mp);
            Mission? ma = Mission.TryCreatePrecursorArtifactMission(this, rnd);
            if (ma is not null) AddMission(ma);
        }

        // Overrides
        public override void DrawBaseIcon(ShaderProgram prog) {
            if (Colony is null) return;
            float scale = Const.PlanetScale * 1.8f;
            Colony.DrawBaseIcon(prog, scale);
        }
        public override void DrawSelected(ShaderProgram prog, int Level, double elapsedSeconds) {
            float scale = DrawScale * Const.PlanetScale;
            Matrix4 pScaleM = Matrix4.CreateScale(scale);
            Matrix4 pTurnM = Matrix4.CreateRotationY(RotationAngle(elapsedSeconds));
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
