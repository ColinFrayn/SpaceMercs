using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using SpaceMercs.Graphics;
using SpaceMercs.Graphics.Shapes;
using SpaceMercs.MainWindow;
using System.IO;
using System.Xml;

namespace SpaceMercs {
    class Star : AstronomicalObject {
        private double Age;

        public readonly List<Planet> Planets;
        public double Mass { get; private set; }  // In solar masses (2 * 10^30 kg)
        public string StarType { get; private set; }
        public Race? Owner { get; private set; }
        public Vector3 MapPos { get; private set; }
        public bool bGenerated { get; private set; }
        public bool Visited { get; private set; }
        public override float DrawScale { get { return (float)Math.Pow(radius / Const.Billion, 0.3) * 2f; } }
        public Sector Sector { get; private set; }
        public bool Scanned {
            get {
                foreach (Planet pl in Planets) {
                    if (pl.Scanned) return true;
                }
                return false;
            }
        }

        private static Vector3 cHottest = new Vector3(1.0f, 1.0f, 1.0f);
        private static Vector3 cMid = new Vector3(1.0f, 0.8f, 0.0f);
        private static Vector3 cCoolest = new Vector3(0.8f, 0.2f, 0.1f);

        public readonly List<Star> TradeRoutes = new List<Star>();
        public void AddTradeRoute(Star st) {
            if (!TradeRoutes.Contains(st)) TradeRoutes.Add(st);
            if (!st.TradeRoutes.Contains(this)) st.AddTradeRoute(this);
        }

        public Star() {
            StarType = "No Type";
            Sector = Sector.Empty;
            Planets = new List<Planet>();
        }
        public Star(float X, float Y, int _seed, Sector s, int sno) {
            MapPos = new Vector3(X, Y, 0f);
            Planets = new List<Planet>();
            Visited = false;
            Sector = s;
            strName = "";
            Seed = _seed;
            StarType = "";
            ID = sno;
            Random rnd = new Random(Seed);
            Ox = rnd.Next(Const.SeedBuffer);
            Oy = rnd.Next(Const.SeedBuffer);
            Oz = rnd.Next(Const.SeedBuffer);
            Generate();
        }
        public Star(XmlNode xml, Sector sect) {
            Sector = sect;
            LoadAODetailsFromFile(xml);

            XmlNode? xmln = xml.SelectSingleNode("Mass");
            if (xmln == null) throw new Exception("Could not locate Mass for Star with ID = " + ID);
            Mass = Double.Parse(xmln.InnerText);

            XmlNode? xmlpos = xml.SelectSingleNode("MapPos");
            if (xmlpos == null) throw new Exception("Could not locate MapPos for Star with ID = " + ID);
            float X = float.Parse(xmlpos.Attributes["X"].Value);
            float Y = float.Parse(xmlpos.Attributes["Y"].Value);
            MapPos = new Vector3(X, Y, 0f);

            Visited = (xml.SelectSingleNode("Visited") != null);

            XmlNode? xmlown = xml.SelectSingleNode("Owner");
            if (xmlown == null) Owner = null;
            else {
                Owner = StaticData.GetRaceByName(xmlown.InnerText);
                Owner.AddSystem(this);
                if (Const.DEBUG_VIEW_ALL_CIVS) SetVisited(true);
            }

            TradeRoutes.Clear();
            foreach (XmlNode xmlt in xml.SelectNodes("TradeRoute")) {
                AstronomicalObject? aotr = sect.ParentMap.GetAOFromLocationString(xmlt.InnerText);
                if (aotr == null) { // It could be that the target is in the current sector (which hasn't been added to the Map yet), so check that
                    aotr = sect.GetAOFromLocationString(xmlt.InnerText);
                }
                if (aotr != null) { // If the target system has been created, then create the trade routes. If it hasn't, then when we load the target system, it will set the route up from there.
                    if (aotr.AOType != AstronomicalObjectType.Star) throw new Exception("Illegal TradeRoute destination (was " + aotr.AOType + " ) at " + xmlt.InnerText);
                    Star sttr = (Star)aotr;
                    TradeRoutes.Add(sttr);
                    sttr.TradeRoutes.Add(this);
                }
            }

            Planets = new List<Planet>();
            foreach (XmlNode xmlp in xml.SelectNodes("Planets/Planet")) {
                Planet pl = new Planet(xmlp, this);
                pl.Parent = this;
                Planets.Add(pl);
            }
            if (Planets.Count == 0) GeneratePlanets(sect.ParentMap.PlanetDensity); // Didn't save them, so regenerate here
            SetupColour();
            StarType = SetupType();
        }
        public static Star Empty { get { return new Star(); } }

        // Save this star to an Xml file
        public void SaveToFile(StreamWriter file) {
            file.WriteLine("<Star ID=\"" + ID.ToString() + "\">");
            // Write generic AO details
            WriteAODetailsToFile(file);

            // Write star details to file
            file.WriteLine(" <Mass>" + Math.Round(Mass, 6).ToString() + "</Mass>");
            file.WriteLine(" <MapPos X=\"" + Math.Round(MapPos.X, 2).ToString() + "\" Y=\"" + Math.Round(MapPos.Y, 2).ToString() + "\"/>");
            if (Owner != null) file.WriteLine(" <Owner>" + Owner.Name + "</Owner>");

            foreach (Star st in TradeRoutes) {
                file.WriteLine(" <TradeRoute>" + st.PrintCoordinates() + "</TradeRoute>");
            }

            // Now write out all planets, but only do this if the system is owned by someone (i.e. it might have colonies in it). Otherwise we can procedurally re-generate it identically on loading.
            // Also save if this system has been scanned at all
            if (Owner != null || Scanned) {
                file.WriteLine(" <Planets>");
                foreach (Planet pl in Planets) {
                    pl.SaveToFile(file);
                }
                file.WriteLine(" </Planets>");
            }

            if (Visited) file.Write(" <Visited/>");

            file.WriteLine("</Star>");
        }

        // Retrieve a planet from this system by ID
        public Planet? GetPlanetByID(int ID) {
            if (ID < 0 || ID >= Planets.Count) return null;
            return Planets[ID];
        }

        // Are we hovering over anything here?
        public AstronomicalObject? GetHover(double aspect, double mousex, double mousey) {
            double px = 8.6 * aspect;
            double py = 2.0;
            foreach (Planet pl in Planets) {
                px -= pl.DrawScale * Const.PlanetScale * aspect * 0.6;
                double buffer = (pl.DrawScale * Const.PlanetScale * Const.SystemViewSelectionTolerance);
                if (mousex > px + buffer) return null; // Too far right. Abort
                if (Math.Abs(px - mousex) < buffer && Math.Abs(py - mousey) < buffer) return pl;
                if (mousey > py + buffer) {
                    AstronomicalObject? aoHover = pl.GetHover(mousex, mousey, px, py);
                    if (aoHover != null) return aoHover;
                }
                px -= ((pl.DrawScale * Const.PlanetScale) + 0.3) * aspect * 0.6;
            }
            return null;
        }

        // Draw a simplified version on the map
        public void DrawMap(ShaderProgram prog, int Level = 7) {
            // Sort out scaling and rotation
            Matrix4 scaleM = Matrix4.CreateScale(Const.StarScale * DrawScale);
            prog.SetUniform("model", scaleM);

            // Setup the colour & draw it
            prog.SetUniform("flatColour", new Vector4(colour, 1f));
            prog.SetUniform("lightEnabled", true);
            GL.UseProgram(prog.ShaderProgramHandle);
            Sphere.CachedBuildAndDraw(Level, true);
        }

        // Draw a name label (and construct one if it doesn't exist yet)
        public void DrawName() {
            if (strName.Length == 0) return;
            TextRenderer.Draw(strName, Alignment.TopMiddle);
        }

        // Generate a star's details according to the required IMF etc.
        public void Generate() {
            Random rnd = new Random(Seed);
            // Try to get valid stars, based on a sensible IMF
            // Keep repeating until we get stars that are not older than their own max. lifetime.
            // This ensures that we actually use sensible initial mass distributions, and sensible lifetimes based on a
            // roughly ELS/62 style monolithic collapse and subsequent low-level triggered SF.
            do {
                // Get stellar mass
                double r = rnd.NextDouble();
                if (r < 0.5) {
                    double A = 2.7 / (3.35 * 1.35);
                    Mass = Math.Pow(1 - (r / A), -1.35);
                }
                else {
                    Mass = r;
                }

                // Age
                // Assume 1/3 of stars created in burst of star formation between 12Gyr and 11Gyr and remainder evenly between 11Gyr and 0Gyr
                // Store age in Gyr
                r = rnd.NextDouble();
                if (r < 0.3333) {
                    Age = (11.0 + r * 3.0);
                }
                else {
                    Age = (r - 0.3333) * 11.0 * 1.5;
                }

            } while (Age > Lifetime(Mass) && Mass > 1.1);  // Try again if this star likely ended in a supernova. However, we do generate white dwarfs

            // Calculate what lifestage the star is in
            // Based on this, set up the star's radius and colour
            double lifestage = Age / Lifetime(Mass);
            radius = Mass * 700.0 * Const.Million;
            // Main sequence
            if (lifestage <= 0.8) {
                radius *= 1.0 + ((lifestage - 0.4) / 2.0);
                float lsc = (float)(lifestage / 0.8);
                Temperature = (int)((double)(8000 - 3500) * lifestage / 0.8) + 3500;
            }
            // Giant branch (simplifying this as one stage atm)
            else if (lifestage <= 1.0) {
                radius *= 1.2 * Math.Pow(2, (lifestage - 0.8) * 20.0);
                float lsc = (float)(lifestage - 0.8) / 0.2f;
                Temperature = (int)((double)(3000 - 6000) * (lifestage - 0.8) * 5.0) + 6000;
            }
            // White dwarf
            else {
                radius = 6.0 * Const.Million / Math.Pow(Mass, 0.33333);
                Temperature = (int)(10000.0 / (lifestage * lifestage));
            }

            // Setup the star's type & colour based on temperature, mass, lifetime etc.
            SetupType();
            SetupColour();

            // Stellar type
            if (lifestage <= 0.8) StarType += " Star";
            else if (lifestage <= 0.9) StarType += " Subgiant";
            else if (lifestage <= 1.0) {
                if (radius < 10 * Const.Million) StarType += " Giant";
                else StarType += " Supergiant";
            }
            else StarType += " Dwarf";

            // Axial rotation period
            do {
                AxialRotationPeriod = (int)Utils.NextGaussian(rnd, Const.StarRotation, Const.StarRotationSigma);
            } while (AxialRotationPeriod < Const.StarRotationMin);
            OrbitalPeriod = 1; // Irrelevant, but avoiding zero :)

            // Make sure we don't have any planets. We generate them later
            Planets.Clear();
        }

        // Set up the stellar type
        private string SetupType() {
            // Set up the stellar type 
            if (Temperature > 40000) return "O0";
            else if (Temperature >= 33000) return "O" + (9 - ((Temperature - 33000) / 700)).ToString();
            else if (Temperature >= 10000) return "B" + (9 - ((Temperature - 10000) / 2300)).ToString();
            else if (Temperature >= 7500) return "A" + (9 - ((Temperature - 7500) / 250)).ToString();
            else if (Temperature >= 6000) return "F" + (9 - ((Temperature - 6000) / 150)).ToString();
            else if (Temperature >= 5200) return "G" + (9 - ((Temperature - 5200) / 80)).ToString();
            else if (Temperature >= 3700) return "K" + (9 - ((Temperature - 3700) / 150)).ToString();
            else if (Temperature >= 2800) return "M" + (9 - ((Temperature - 2800) / 90)).ToString();
            else return "R";
        }

        // Set up the star's colour
        private void SetupColour() {
            if (Temperature < 6500) {
                colour = Vector3.Subtract(cMid, cCoolest);
                colour = Vector3.Multiply(colour, (float)(Temperature - 3000) / (6500 - 3000));
                colour = Vector3.Add(colour, cCoolest);
            }
            else {
                colour = Vector3.Subtract(cHottest, cMid);
                colour = Vector3.Multiply(colour, (float)(Temperature - 6500) / (10000 - 6500));
                colour = Vector3.Add(colour, cMid);
            }
        }

        // Generate this system as a home system
        public void GenerateHomeSystem(Race rc) {
            Random rand = new Random(Seed);
            Planet? plHome = null;
            double dTempBest = Const.TempTolerance;
            do {
                Seed = rand.Next(10000000);
                Generate();
                GeneratePlanets(9);
                foreach (Planet pl in Planets) {
                    if (pl.Type == rc.PlanetType && Math.Abs(pl.Temperature - rc.BaseTemp) < dTempBest) {
                        dTempBest = Math.Abs(pl.Temperature - rc.BaseTemp);
                        plHome = pl;
                    }
                }
            } while (plHome == null);
            plHome.GenerateMoons(rand, 9, 2); // Make sure that this planet has at least two moons
            rc.SetHomePlanet(plHome);
            rc.Colonise(this);
            InsertColoniesForRace(rc, Const.HomeSysColonyCount);
            plHome.SetName("Homeworld");
        }

        // Generate the rest of the solar system
        public void GeneratePlanets(int pdensity) {
            if (Planets.Count > 0) return;
            Random rnd = new Random(Seed);
            // Number of planets
            int npl = (rnd.Next(pdensity + 1) + rnd.Next(pdensity + 4) + rnd.Next(pdensity + 4)) / 2;
            if (radius < 30.0 * Const.Million) npl = (npl * 2) / 3; // Reduce npl for white dwarfs
            if (npl > Const.MaxPlanetsPerSystem) npl = Const.MaxPlanetsPerSystem;
            if (npl < 1) npl = 1;
            double porbit = Const.PlanetOrbit * Mass;

            // Generate all planets
            for (int n = 0; n < npl; n++) {
                Planet pl = new Planet(rnd.Next(10000000), this);
                pl.Parent = this;
                pl.ID = n;

                // Get orbit
                do {
                    porbit *= Utils.NextGaussian(rnd, Const.PlanetOrbitFactor, Const.PlanetOrbitFactorSigma);
                } while (rnd.Next(npl + 2) == 0);
                pl.orbit = porbit + (radius * 2.0);

                // Work out planet type
                bool bOK = true;
                do {
                    // Calculate temperature at this location
                    // 280K * (T/Ts) * (R/Rs)^1/2 * (1-A)^1/4 / a^1/2  + modifier_for_atmosphere
                    // (a = orbit in AU, A = albedo, T = star temperature, R = stellar radius)
                    pl.tempbase = 300.0 / Math.Pow(porbit / Const.AU, 0.5);
                    pl.tempbase *= (Temperature / Const.SunTemperature) * Math.Pow(radius / Const.SunRadius, 0.5); // Scale by the star's properties

                    double tempmod = 0.0;
                    double albedo = 0.3;
                    bOK = true;
                    if (pl.tempbase > 180 && pl.tempbase < 350 && rnd.Next(3) == 0) { pl.Type = Planet.PlanetType.Oceanic; albedo = 0.3; tempmod = Utils.NextGaussian(rnd, 30, 5); }
                    else {
                        int r = rnd.Next(30);
                        if (r < 2) { pl.Type = Planet.PlanetType.Oceanic; albedo = 0.3; tempmod = Utils.NextGaussian(rnd, 30, 5); }
                        else if (r < 5) { pl.Type = Planet.PlanetType.Desert; albedo = 0.4; }
                        else if (r < 9) { pl.Type = Planet.PlanetType.Volcanic; albedo = 0.18; tempmod = Utils.NextGaussian(rnd, 10, 2); }
                        else if (r < 15) { pl.Type = Planet.PlanetType.Rocky; albedo = 0.25; }
                        else { pl.Type = Planet.PlanetType.Gas; albedo = 0.5; }
                    }
                    pl.tempbase *= Math.Pow(1.0 - albedo, 0.25);
                    pl.tempbase += tempmod;
                    pl.Temperature = (int)pl.tempbase;

                    // Check that this is ok
                    if (pl.tempbase > 400 && pl.Type == Planet.PlanetType.Gas) bOK = false;
                    if (pl.tempbase > 320 && pl.Type == Planet.PlanetType.Oceanic) bOK = false;
                    if (pl.tempbase < 160 && pl.Type == Planet.PlanetType.Oceanic) bOK = false;
                    if (pl.tempbase < 270 && pl.Type == Planet.PlanetType.Oceanic) pl.Type = Planet.PlanetType.Ice;
                    if (pl.tempbase < 180 && pl.Type == Planet.PlanetType.Volcanic) bOK = false;
                } while (bOK == false);

                // Get radius based on type
                do {
                    pl.radius = Utils.NextGaussian(rnd, Const.PlanetSize, Const.PlanetSizeSigma);
                } while (pl.radius < Const.PlanetSizeMin);
                if (pl.Type == Planet.PlanetType.Gas) {
                    pl.radius *= Utils.NextGaussian(rnd, Const.GasGiantScale, Const.GasGiantScaleSigma);
                }
                pl.colour = Const.PlanetTypeToCol2(pl.Type);

                // Orbital period
                double prot = Utils.NextGaussian(rnd, Const.AverageOrbitalPeriod, Const.AverageOrbitalPeriodSigma);
                prot /= ((pl.orbit / Const.AU) * Math.Pow(pl.radius / (6.0 * Const.Million), 0.5));
                pl.OrbitalPeriod = (int)prot;

                // Axial rotation period (i.e. a day length)
                double arot = Utils.NextGaussian(rnd, Const.DayLength, Const.DayLengthSigma);
                pl.AxialRotationPeriod = (int)(arot * (pl.radius / Const.PlanetSize));

                pl.GenerateMoons(rnd, pdensity);
                Planets.Add(pl);
            }
            bGenerated = true;
        }

        // Max lifetime = 10^10 yrs / mass^3 (valid for stars < 10Ms and roughly valid above that. Ish.)
        public static double Lifetime(double stmass) {
            return 10.0 / Math.Pow(stmass, 3.0);
        }

        // Distance in LY between two stars
        public double DistanceTo(Star st) {
            double dx = st.MapPos.X - MapPos.X;
            double dy = st.MapPos.Y - MapPos.Y;
            double d2 = ((dx * dx) + (dy * dy));
            return Math.Sqrt(d2);
        }

        // Setters
        public void SetOwner(Race rc) {
            Owner = rc;
            if (rc == StaticData.Races[0]) Visited = true;
            if (Planets.Count == 0 && rc != null) GeneratePlanets(Sector.ParentMap.PlanetDensity);
        }
        public void SetVisited(bool v) {
            Visited = v;
        }

        // For a few habitable planets in this system (or moons), insert a colony for the given race
        public void InsertColoniesForRace(Race rc, int Count) {
            int tries = 0, inserted = 0, iterations = 0;
            if (Planets.Count == 0) return;
            Random rand = new Random();
            // Just so we don't get the same place every time, randomise a bit
            do {
                int pno = rand.Next(Planets.Count);
                Planet pl = Planets[pno];
                if (pl.BaseSize == 0) break;
            } while (++iterations < 10);
            // Now add the colonies
            do {
                tries++;
                int pno = rand.Next(Planets.Count);
                Planet pl = Planets[pno];
                double tdiff = pl.TDiff(rc);
                if (rand.NextDouble() * 100.0 > tdiff) {
                    if (rand.NextDouble() > 0.5) {
                        if (pl.Type != Planet.PlanetType.Desert || pl.BaseSize < 4) inserted += pl.ExpandBase(rc, rand);
                    }
                    else inserted += pl.ExpandMoonBase(rand, rc);
                }
            } while (inserted < Count && tries < 1000);
        }

        // When we arrive in this system (or move about the system) update colony growth
        public void UpdateColonies() {
            foreach (Planet pl in Planets) {
                pl.CheckGrowth();
            }
        }

        public Planet? GetOutermostPlanet() {
            if (Planets.Count == 0) return null;
            return Planets.Last();
        }

        // Overrides
        public override AstronomicalObjectType AOType { get { return AstronomicalObjectType.Star; } }
        public override void DrawSelected(ShaderProgram prog, int Level = 8) {
            // Sort out scaling and rotation
            float scale = Const.StarScale * DrawScale;
            Matrix4 pScaleM = Matrix4.CreateScale(scale);
            Matrix4 pTurnM = Matrix4.CreateRotationY((float)Const.ElapsedSeconds * 2f * (float)Math.PI / (float)AxialRotationPeriod);
            Matrix4 pRotateM = Matrix4.CreateRotationX((float)Math.PI / 2f);
            Matrix4 modelM = pRotateM * pTurnM * pScaleM;
            prog.SetUniform("model", modelM);

            // Setup the texture & draw it
            prog.SetUniform("lightEnabled", true);
            prog.SetUniform("textureEnabled", true);
            prog.SetUniform("texPos", 0f, 0f);
            prog.SetUniform("texScale", 1f, 1f);
            SetupTextureMap(64, 32);
            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, iTexture);
            GL.UseProgram(prog.ShaderProgramHandle);
            Sphere.CachedBuildAndDraw(Level, true);
            GL.BindTexture(TextureTarget.Texture2D, 0);
        }
        public override void SetupTextureMap(int width, int height) {
            if (iTexture == -1 || texture == null) iTexture = GL.GenTexture();
            else {
                // Have we already built a texture of sufficient detail?
                if (texture.Length >= (width * height * 3)) return;
            }
            texture = Terrain.GenerateMap(this, width, height);
            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, iTexture);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgb, width, height, 0, PixelFormat.Rgb, PixelType.UnsignedByte, texture);
            GL.TexEnv(TextureEnvTarget.TextureEnv, TextureEnvParameter.TextureEnvMode, (float)TextureEnvMode.Modulate);
            Textures.SetParameters();
        }
        public override void ClearData() {
            GL.DeleteTexture(iTexture);
            texture = null;
            iTexture = -1;
            foreach (Planet pl in Planets) {
                pl.ClearData();
            }
        }
        public override void SetName(string str) {
            strName = str;
        }
        public override Star GetSystem() {
            return this;
        }
        public override string PrintCoordinates() {
            return Sector.PrintCoordinates() + ID;
        }
        // Detail level to display this star, given the view distance
        public int GetDetailLevel(float fMapViewX, float fMapViewY, float fMapViewZ) {
            float dx = fMapViewX - MapPos.X;
            float dy = fMapViewY - MapPos.Y;
            float dist2 = ((dx * dx) + (dy * dy) + (fMapViewZ * fMapViewZ));

            float DetailScale = (float)(Math.Sqrt(dist2) / (DrawScale * 0.1f));

            if (DetailScale < 25.0) return 8;
            else if (DetailScale < 32.0) return 7;
            else if (DetailScale < 40.0) return 6;
            else if (DetailScale < 80.0) return 5;
            else if (DetailScale < 150.0) return 4;
            else if (DetailScale < 300.0) return 3;
            else if (DetailScale < 600.0) return 2;
            return 1;
        }
    }
}
