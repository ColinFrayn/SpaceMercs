using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using SpaceMercs.Graphics;
using SpaceMercs.Graphics.Shapes;
using System.IO;
using System.Text;
using System.Xml;

namespace SpaceMercs {
    public class Star : AstronomicalObject {
        public IReadOnlyList<Planet> Planets => _planets;
        private readonly List<Planet> _planets = new List<Planet>();
        private double Age;
        public double Mass { get; private set; }  // In solar masses (2 * 10^30 kg)
        public string StarType { get; private set; }
        public Race? Owner { get; private set; }
        public Vector3 MapPos { get; private set; }
        public bool bGenerated { get; private set; } = false;
        public bool Visited { get; private set; }
        public override float DrawScale { get { return (float)Math.Pow(Radius / Const.Billion, 0.3) * 2f; } }
        public Sector Sector { get; private set; }
        public bool Scanned {
            get {
                if (HasHyperGate) return true;
                if (SpaceHulk is not null && SpaceHulk.Scanned) return true;
                foreach (Planet pl in _planets) {
                    if (pl.Scanned) return true;
                    foreach (Moon mn in pl.Moons) {
                        if (mn.Scanned) return true;
                    }
                }
                return false;
            }
        }
        public bool Renamed {
            get {
                foreach (Planet pl in _planets) {
                    if (!string.IsNullOrEmpty(pl.Name)) return true;
                    foreach (Moon mn in pl.Moons) {
                        if (!string.IsNullOrEmpty(mn.Name)) return true;
                    }
                }
                return false;
            }
        }
        public bool HasHyperGate { get { return TradeRoutes.Count > 0; } }
        public SpaceHulk? SpaceHulk { get; private set; }
        public double HyperGateOrbit {
            get {
                if (!bGenerated) GeneratePlanets(Sector.ParentMap.PlanetDensity);
                Planet pl = _planets.Last();
                return pl.OrbitalDistance * 1.5;
            }
        }
        public double SpaceHulkOrbit {
            get {
                if (!bGenerated) GeneratePlanets(Sector.ParentMap.PlanetDensity);
                Planet pl = _planets.Last();
                return pl.OrbitalDistance * 2.0;
            }
        }
        private HyperGate _hypergate;
        private readonly Lock _hglock = new();
        public HyperGate? GetHyperGate() {
            if (!HasHyperGate) return null;
            lock (_hglock) {
                _hypergate ??= new HyperGate(this, HyperGateOrbit);
                return _hypergate;
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
        }
        public Star(float X, float Y, int _seed, Sector s, int sno) {
            MapPos = new Vector3(X, Y, 0f);
            Visited = false;
            Sector = s;
            Name = "";
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

            Seed = xml.SelectNodeInt("Seed");
            ID = xml.GetAttributeInt("ID");
            Name = xml.SelectNodeText("Name", string.Empty);

            // Shortcut - no details saved for the star itself so regenerate them (identically) from the seed
            Generate();

            XmlNode xmlpos = xml.SelectSingleNode("MapPos") ?? throw new Exception($"Could not locate MapPos for Star with ID = {ID}");
            float X = float.Parse(xmlpos.Attributes!["X"]?.Value ?? throw new Exception($"Could not identify Star X-Coord with ID = {ID}"));
            float Y = float.Parse(xmlpos.Attributes!["Y"]?.Value ?? throw new Exception($"Could not identify Star Y-Coord with ID = {ID}"));
            MapPos = new Vector3(X, Y, 0f);

            Visited = (xml.SelectSingleNode("Visited") is not null);

            XmlNode? xmlown = xml.SelectSingleNode("Owner");
            Owner = null;
            if (xmlown is not null) {
                Owner = StaticData.GetRaceByName(xmlown.InnerText) ?? throw new Exception($"Could not identify system owner race {xmlown.InnerText}");
                Owner.AddSystem(this);
                if (Const.DEBUG_VIEW_ALL_CIVS) SetVisited();
                if (string.IsNullOrEmpty(Name)) {
                    Name = GenerateAlienSystemName(Seed);
                }
            }

            TradeRoutes.Clear();
            foreach (XmlNode xmlt in xml.SelectNodesToList("TradeRoute")) {
                AstronomicalObject? aotr = sect.ParentMap.GetAOFromLocationString(xmlt.InnerText);
                if (aotr is null) { // It could be that the target is in the current sector (which hasn't been added to the Map yet), so check that
                    aotr = sect.GetAOFromLocationString(xmlt.InnerText);
                }
                if (aotr is not null) { // If the target system has been created, then create the trade routes. If it hasn't, then when we load the target system, it will set the route up from there.
                    if (aotr is not Star) throw new Exception($"Illegal TradeRoute destination (was {aotr.GetType()}) at {xmlt.InnerText}");
                    Star sttr = (Star)aotr;
                    TradeRoutes.Add(sttr);
                    sttr.TradeRoutes.Add(this);
                }
            }

            // Load any planets that might have been saved specially
            var planets = xml.SelectNodesToList("Planets/Planet");
            if (planets.Any()) _planets.Clear();
            foreach (XmlNode xmlp in planets) {
                if (xmlp.Attributes!["Seed"]?.Value is string strSeed) {
                    int seed = Int32.Parse(strSeed);
                    Planet pl = Planet.MakeFromSeed(this, seed, planets.Count());
                    _planets.Add(pl);
                }
                else {
                    Planet pl = new Planet(xmlp, this);
                    _planets.Add(pl);
                }
                bGenerated = true;
                if (SpaceHulk != null) SpaceHulk.OrbitalDistance = SpaceHulkOrbit;
            }

            SetupColour();
            StarType = SetupType();

            XmlNode? xsh = xml.SelectSingleNode("SpaceHulk");
            if (xsh is not null) {
                if (SpaceHulk is null) {
                    throw new Exception("Weird - system has SpaceHulk data but no actual space hulk");
                }
                SpaceHulk.LoadFromFile(this, xsh);
            }
        }

        public static Star Empty { get { return new Star(); } }

        // Save this star to an Xml file
        public override void SaveToFile(StreamWriter file, GlobalClock clock) {
            file.WriteLine("<Star ID=\"" + ID.ToString() + "\">");
            file.WriteLine("<Seed>" + Seed + "</Seed>");
            if (!string.IsNullOrEmpty(Name) && !string.Equals(Name, "Unnamed")) file.WriteLine("<Name>" + Name + "</Name>");
            file.WriteLine(" <MapPos X=\"" + Math.Round(MapPos.X, 2).ToString() + "\" Y=\"" + Math.Round(MapPos.Y, 2).ToString() + "\"/>");
            if (Owner != null) file.WriteLine(" <Owner>" + Owner.Name + "</Owner>");

            foreach (Star st in TradeRoutes.OrderBy(t => t.ID)) {
                file.WriteLine(" <TradeRoute>" + st.PrintCoordinates() + "</TradeRoute>");
            }

            // Now write out all planets, but only do this if the system is owned by someone (i.e. it might have colonies in it). Otherwise we can procedurally re-generate it identically on loading.
            // Also save if this system has been scanned at all or any planets have been renamed
            if (_planets.Any() && HasBeenEdited()) {
                file.WriteLine(" <Planets>");
                foreach (Planet pl in _planets) {
                    pl.SaveToFile(file, clock);
                }
                file.WriteLine(" </Planets>");
            }

            if (Visited) file.Write(" <Visited/>");

            if (SpaceHulk is not null && SpaceHulk.Scanned) {
                SpaceHulk.SaveToFile(file, clock);
            }

            file.WriteLine("</Star>");
        }

        // Retrieve a planet from this system by ID
        public Planet? GetPlanetByID(int ID) {
            if (!bGenerated) GeneratePlanets(Sector.ParentMap.PlanetDensity);
            if (ID < 0 || ID >= _planets.Count) return null;
            return _planets[ID];
        }

        // Draw a simplified version on the map
        public void DrawMap(ShaderProgram prog, int Level = 7) {
            // Sort out scaling and rotation
            Matrix4 scaleM = Matrix4.CreateScale(Const.StarScale * DrawScale);
            prog.SetUniform("model", scaleM);

            // Setup the colour & draw it
            prog.SetUniform("flatColour", new Vector4(BaseColour, 1f));
            prog.SetUniform("lightEnabled", true);
            GL.UseProgram(prog.ShaderProgramHandle);
            Sphere.CachedBuildAndDraw(Level, true);
        }

        // Draw a name label (and construct one if it doesn't exist yet)
        public void DrawName() {
            if (string.IsNullOrEmpty(Name)) return;
            TextRenderer.Draw(Name, Alignment.TopMiddle);
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
            Radius = Mass * 700.0 * Const.Million;
            // Main sequence
            if (lifestage <= 0.8) {
                Radius *= 1.0 + ((lifestage - 0.4) / 2.0);
                Temperature = (int)((double)(8000 - 3500) * lifestage / 0.8) + 3500;
            }
            // Giant branch (simplifying this as one stage atm)
            else if (lifestage <= 1.0) {
                Radius *= 1.2 * Math.Pow(2, (lifestage - 0.8) * 20.0);
                Temperature = (int)((double)(3000 - 6000) * (lifestage - 0.8) * 5.0) + 6000;
            }
            // White dwarf
            else {
                Radius = 6.0 * Const.Million / Math.Pow(Mass, 0.33333);
                Temperature = (int)(10000.0 / (lifestage * lifestage));
            }

            // Setup the star's type & colour based on temperature, mass, lifetime etc.
            StarType = SetupType();
            SetupColour();

            // Stellar type
            if (lifestage <= 0.8) StarType += " Star";
            else if (lifestage <= 0.9) StarType += " Subgiant";
            else if (lifestage <= 1.0) {
                if (Radius < 10 * Const.Million) StarType += " Giant";
                else StarType += " Supergiant";
            }
            else StarType += " Dwarf";

            // Axial rotation period
            do {
                AxialRotationPeriod = (int)Utils.NextGaussian(rnd, Const.StarRotation, Const.StarRotationSigma);
            } while (AxialRotationPeriod < Const.StarRotationMin);

            // Finally, clear out the planets
            _planets?.Clear();

            // Does it have a space hulk?
            SpaceHulk = null;
            if (Sector is not null) {
                int ring = Math.Max(Math.Abs(Sector.SectorX), Math.Abs(Sector.SectorY));
                if (ring >= 2) {
                    // Use Star ID & sector Seed to get a more predictable but repeatable fraction of hulks
                    if ((ID + Sector.Seed) % (2 + ring) == 0) {
                        SpaceHulk = new SpaceHulk(this);
                    }
                }
            }

            bGenerated = false;
        }

        // This star is stable and on the main sequence i.e. not a giant/dwarf or young star
        public bool IsStableMainSequence() {
            double lifestage = Age / Lifetime(Mass);
            return (Age > 2.0) && (lifestage < 0.8);
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
                BaseColour = Vector3.Subtract(cMid, cCoolest);
                BaseColour = Vector3.Multiply(BaseColour, (float)(Temperature - 3000) / (6500 - 3000));
                BaseColour = Vector3.Add(BaseColour, cCoolest);
            }
            else {
                BaseColour = Vector3.Subtract(cHottest, cMid);
                BaseColour = Vector3.Multiply(BaseColour, (float)(Temperature - 6500) / (10000 - 6500));
                BaseColour = Vector3.Add(BaseColour, cMid);
            }
        }

        // Generate this system as a home system
        public void GenerateHomeSystem(Race rc, GlobalClock clock) {
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
            plHome.GenerateMoons(Const.HomeworldPDensity, Const.HomeworldMinMoons); // Make sure that this planet has at least two moons
            rc.Colonise(this);
            rc.SetHomePlanet(plHome, clock);
            plHome.SetName("Homeworld");
        }

        // Generate the rest of the solar system
        public void GeneratePlanets(int pdensity) {
            _planets.Clear();
            Random rnd = new Random(Seed);
            // Number of planets
            int npl = (rnd.Next(pdensity + 1) + rnd.Next(pdensity + 4) + rnd.Next(pdensity + 4)) / 2;
            if (Radius < 30.0 * Const.Million) npl = (npl * 2) / 3; // Reduce npl for white dwarfs
            if (npl > Const.MaxPlanetsPerSystem) npl = Const.MaxPlanetsPerSystem;
            if (npl < 1) npl = 1;

            // Generate all planets
            for (int n = 0; n < npl; n++) {
                Planet pl = new Planet(rnd.Next(10000000), this, npl);
                _planets.Add(pl);
            }
            bGenerated = true;
            if (SpaceHulk != null) SpaceHulk.OrbitalDistance = SpaceHulkOrbit;
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
            if (rc == StaticData.HumanRace) Visited = true;
            if (!_planets.Any() && rc != null) GeneratePlanets(Sector.ParentMap.PlanetDensity);
            if (string.IsNullOrEmpty(Name) && rc != StaticData.HumanRace) Name = GenerateAlienSystemName(Seed);
        }
        public void SetVisited() {
            Visited = true;
        }

        // Either expand an existing base or add a new one in this system
        public bool AddPopulationInSystem(Race rc, Random rand, GlobalClock clock) {
            int tries = 0;
            if (!bGenerated) GeneratePlanets(Sector.ParentMap.PlanetDensity);

            // Firstly colonise any Oceanic planets and maybe some moons
            int plcols = 0;
            foreach (Planet pl in Planets) {
                if (pl.Colony is not null) plcols++;
                if (pl.Type == Planet.PlanetType.Oceanic) {
                    if (pl.ExpandBase(rc, rand, clock) > 0) return true;
                    if (rand.NextDouble() > 0.5) {
                        if (pl.ExpandMoonBase(rand, rc, clock) > 0) return true;
                    }
                }
            }

            // Now add any remaining colonies
            do {
                tries++;
                int pno = rand.Next(_planets.Count);
                Planet pl = _planets[pno];
                double tdiff = pl.TDiff(rc);
                if (pl.Type == Planet.PlanetType.Gas) {
                    tdiff += 15.0; // Make it less likely to colonise Gas giants.
                    if (plcols == 0) tdiff += 30; // Much less likely if this is the first colony in this system
                    if (plcols == 1) tdiff += 15; // Quite a bit less likely if this is the second colony in this system
                }
                if (rand.NextDouble() * 150.0 > tdiff) {
                    if (rand.NextDouble() > 0.5) {
                        if (pl.Type != Planet.PlanetType.Gas || pl.BaseSize < 4) {
                            if (pl.ExpandBase(rc, rand, clock) > 0) return true;
                        }
                    }
                    else if (pl.ExpandMoonBase(rand, rc, clock) > 0) return true;
                }
            } while (tries < 50);
            return false;
        }

        public bool MaybeAddNewColony(Race rc, Random rand, GlobalClock clock) {
            int plcols = 0;
            foreach (Planet pl in Planets) {
                if (pl.Colony is not null) plcols++;
            }
            int maxTries = Planets.Count * 4 + 6;
            int tries = 0;
            do {
                int pno = rand.Next(_planets.Count);
                Planet pl = _planets[pno];
                double tdiff = pl.TDiff(rc);
                if (pl.Type == Planet.PlanetType.Gas) {
                    tdiff += 20d; // Make it less likely to colonise Gas giants.
                    if (plcols == 0) tdiff += 40d; // Much less likely if this is the first planet colony in this system
                    if (plcols == 1) tdiff += 20d; // Quite a bit less likely if this is the second planet colony in this system
                }
                if (rand.NextDouble() * rand.NextDouble() * 200.0 > tdiff) {
                    if (pl.Colony is null) {
                        if (pl.ExpandBase(rc, rand, clock) > 0) return true;
                    }
                    else if (pl.Moons.Count > 0) {
                        int mno = rand.Next(pl.Moons.Count);
                        Moon mn = pl.Moons[mno];
                        int moonPop = pl.GetPopulation() - pl.Colony.BaseSize;
                        if (mn.Colony is null) {
                            double chance = 40d + (plcols * 2d) - (moonPop * 15d);
                            if (mn.Type == Planet.PlanetType.Oceanic) chance += 25d;
                            if (mn.Type == Planet.PlanetType.Volcanic || mn.Type == Planet.PlanetType.Ice) chance -= 15d;
                            if (rand.NextDouble() * 100d < chance) {
                                if (mn.ExpandBase(rc, rand, clock) > 0) return true;
                            }
                        }

                    }
                }
            } while (++tries < maxTries);
            return false;
        }

        // Maybe build trade routes for this system based on the largest colony size and number of colonies for the given race
        public void CheckBuildTradeRoutes(Race rc) {
            int colonyCount = 0;
            int maxColonySize = 0;
            int totalPop = 0;
            foreach (Planet planet in Planets) {
                if (planet.Colony?.Owner == rc) {
                    colonyCount++;
                    if (planet.BaseSize > maxColonySize) maxColonySize = planet.BaseSize;
                    totalPop += planet.BaseSize;
                }
                foreach (Moon mn in planet.Moons) {
                    if (mn.Colony?.Owner == rc) {
                        colonyCount++;
                        if (mn.BaseSize > maxColonySize) maxColonySize = mn.BaseSize;
                        totalPop += mn.BaseSize;
                    }
                }
            }

            // The more populous the system, the more trade routes it can support
            int expectedTradeRoutes = 0;
            if (maxColonySize >= 4 || (maxColonySize == 3 && totalPop >= 6)) expectedTradeRoutes = 1;
            if (maxColonySize >= 5 && colonyCount >= 2 && totalPop >= 7) expectedTradeRoutes = 2;
            if (maxColonySize == 4 && colonyCount >= 4 && totalPop >= 10) expectedTradeRoutes = 2;
            if (maxColonySize == 6 && colonyCount >= 4 && totalPop >= 13) expectedTradeRoutes = 3;
            if (maxColonySize == 5 && colonyCount >= 5 && totalPop >= 15) expectedTradeRoutes = 3;
            if (maxColonySize == 6 && colonyCount >= 6 && totalPop >= 20) expectedTradeRoutes = 4;
            if (TradeRoutes.Count < expectedTradeRoutes) {
                MaybeAddTradeRoute(rc, true);
            }
        }
        public void MaybeAddTradeRoute(Race rc, bool onlyToExistingTradeRouteDestinations) {
            Star? stClosest = null;
            double best = Const.BasicTradeRouteLength;
            int pop = CountPopulation();
            if (pop > 4) best += (double)(pop - 4) / 4.0; // Highly populated systems can have longer trade routes
            if (best > Const.MaxTradeRouteLength) best = Const.MaxTradeRouteLength;
            foreach (Star st2 in rc.Systems) {
                if (st2 == this) continue;
                if (onlyToExistingTradeRouteDestinations && !st2.TradeRoutes.Any()) continue;
                double d = DistanceTo(st2);
                if (d < best && !TradeRoutes.Contains(st2)) {
                    stClosest = st2;
                    best = d;
                }
            }
            if (stClosest != null) {
                AddTradeRoute(stClosest);
            }
        }

        // Utility
        public Planet? GetOutermostPlanet() {
            if (!bGenerated) GeneratePlanets(Sector.ParentMap.PlanetDensity);
            return _planets.Last();
        }
        public int CountPopulation() {
            int pop = 0;
            foreach (Planet planet in Planets) {
                pop += planet.Colony?.BaseSize ?? 0;
                foreach (Moon mn in planet.Moons) {
                    pop += mn.Colony?.BaseSize ?? 0;
                }
            }
            return pop;
        }

        // Overrides
        public override void DrawSelected(ShaderProgram prog, int Level, double elapsedSeconds) {
            // Sort out scaling and rotation
            float scale = Const.StarScale * DrawScale;
            Matrix4 pScaleM = Matrix4.CreateScale(scale);
            Matrix4 pTurnM = Matrix4.CreateRotationY(RotationAngle(elapsedSeconds));
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
            Textures.SetTextureParameters();
        }
        public override void ClearData() {
            GL.DeleteTexture(iTexture);
            texture = null;
            iTexture = -1;
            foreach (Planet pl in _planets) {
                pl.ClearData();
            }
        }
        public override void SetName(string str) {
            Name = str;
        }
        public override Star GetSystem() {
            return this;
        }
        public override string PrintCoordinates() {
            return Sector.PrintCoordinates() + ID;
        }
        public override int GetPopulation() {
            int pop = 0;
            foreach (Planet pl in _planets) {
                pop += pl.GetPopulation();
            }
            return pop;
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

        // If this system has been edited in any way (and hence potentially has to be saved in full)
        private bool HasBeenEdited() {
            if (!bGenerated) return false;
            if (Renamed) return true;
            foreach (Planet pl in Planets) {
                if (pl.PlanetOrMoonsHaveBeenEdited()) return true;
            }
            return false;
        }

        // Generate a random system name for this system
        private static string GenerateAlienSystemName(int seed) {
            Random rnd = new Random(seed);
            string[] Vowels = ["A", "E", "I", "O", "U"];
            string[] HardConsonants = ["B", "C", "D", "F", "G", "H", "J", "K", "L", "M", "N", "P", "R", "S", "T", "V", "W", "Y", "Z"];
            string[] SecondaryConsonants = ["l", "r", "h"];
            string[] Terminals = ["a", "b", "ck", "d", "g", "i", "k", "l", "m", "n", "o", "p", "r", "s", "t", "v", "z"];
            string[] Final = ["a", "h", "x", "i", "k"];
            string[] SystemNameX = ["Alpha", "Beta", "Gamma", "Delta", "Epsilon", "Prime", "Major", "Minor", "Core"];

            StringBuilder sbName = new StringBuilder();

            if (rnd.NextDouble() < 0.3) sbName.Append(Vowels[rnd.Next(Vowels.Length)]);
            else {
                sbName.Append(HardConsonants[rnd.Next(HardConsonants.Length)]);
                if (rnd.NextDouble() < 0.4) {
                    sbName.Append(SecondaryConsonants[rnd.Next(SecondaryConsonants.Length)]);
                }
                sbName.Append(Vowels[rnd.Next(Vowels.Length)].ToLower());
            }
            if (rnd.NextDouble() < 0.6) sbName.Append(HardConsonants[rnd.Next(HardConsonants.Length)].ToLower());
            else sbName.Append(HardConsonants[rnd.Next(HardConsonants.Length)]);
            sbName.Append(SecondaryConsonants[rnd.Next(SecondaryConsonants.Length)]);
            sbName.Append(Vowels[rnd.Next(Vowels.Length)].ToLower());
            sbName.Append(Terminals[rnd.Next(Terminals.Length)]);
            if (rnd.NextDouble() < 0.3) sbName.Append(Vowels[rnd.Next(Vowels.Length)].ToLower());

            if (rnd.NextDouble() < 0.7) {
                sbName.Append(" ");
                sbName.Append(SystemNameX[rnd.Next(SystemNameX.Length)]);
            }

            return sbName.ToString();
        }
    }
}
