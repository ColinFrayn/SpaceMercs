using SpaceMercs.Dialogs;
using System.IO;
using System.Xml;

namespace SpaceMercs {
    public class Map {
        private readonly Dictionary<Tuple<int, int>, Sector> dSectors = new Dictionary<Tuple<int, int>, Sector>();
        public bool MapIsInitialised { get; private set; }
        private const int INITIAL_MAP_SIZE = 3; // Just make sure that you set up an inital map of a reasonable size. This must be >= 1!
        public int MapSeed { get; private set; }
        public int StarsPerSector { get; private set; }
        public int PlanetDensity { get; private set; }

        public Map() {
            // We're good
        }
        public Map(XmlNode xml) {
            if (xml.Attributes?["Seed"] is null) throw new Exception("Map seed is missing from save file");
            if (xml.Attributes?["SPS"] is null) throw new Exception("Map StarsPerSector is missing from save file");
            if (xml.Attributes?["PD"] is null) throw new Exception("Map PlanetDensity is missing from save file");
            MapSeed = xml.GetAttributeInt("Seed");
            StarsPerSector = xml.GetAttributeInt("SPS");
            PlanetDensity = xml.GetAttributeInt("PD");

            dSectors.Clear();
            foreach (XmlNode xmls in xml.SelectNodesToList("Sector")) {
                Sector sect = new Sector(xmls, this);
                dSectors.Add(new Tuple<int, int>(sect.SectorX, sect.SectorY), sect);
            }
            if (!dSectors.Any()) throw new Exception("Could not locate sector nodes in save file");

            MapIsInitialised = true;
        }
        public static Map Empty { get { return new Map(); } }

        // Generate a map with the given races
        public void Generate(NewGame ng) {
            // Get the new values and reset our map
            Random rand = new Random(ng.Seed);
            MapIsInitialised = false;
            dSectors.Clear();
            MapSeed = ng.Seed;
            StarsPerSector = ng.StarsPerSector;
            PlanetDensity = ng.PlanetDensity;

            // Reset all races
            foreach (Race rc in StaticData.Races) rc.Reset();

            // Generate the stars in the first few sectors, visible from home
            for (int sy = -INITIAL_MAP_SIZE; sy <= INITIAL_MAP_SIZE; sy++) {
                for (int sx = -INITIAL_MAP_SIZE; sx <= INITIAL_MAP_SIZE; sx++) {
                    dSectors.Add(new Tuple<int, int>(sx, sy), new Sector(sx, sy, this));
                }
            }

            // Setup Race starting locations by modifying existing map to create habitable systems
            foreach (Race rc in StaticData.Races) {
                if (rc == StaticData.Races[0]) {
                    SetupHomeSector(rc, dSectors[new Tuple<int, int>(0, 0)], ng);
                    rc.SetAsKnownBy(null);
                }
                else {
                    Sector sc;
                    do {
                        int sx = rand.Next(3) - 1;
                        int sy = rand.Next(3) - 1;
                        sc = dSectors[new Tuple<int, int>(sx, sy)];
                    } while (sc.Inhabitant != null);
                    SetupHomeSector(rc, sc, ng);
                }
            }

            // Flag everything as done
            MapIsInitialised = true;
        }

        // Setup the given sector as the home for this race
        private static void SetupHomeSector(Race rc, Sector sc, NewGame ng) {
            // Get the most central star to be the home system
            sc.Inhabitant = rc;
            Star? stHome = sc.GetMostCentralStar();
            if (stHome is null) {
                throw new Exception("Could not find most central star");
            }
            bool isPlayer = (sc.SectorX==0 && sc.SectorY==0);
            int maxSize = isPlayer ? ng.CivSize : ng.AlienCivSize;

            stHome.GenerateHomeSystem(rc);

            // Setup other stars in this sector, allied to this race, based on the maximum civ size
            Random rand = new Random(ng.Seed);

            // Force multiple systems based on civ size
            List<Star> SystemsInOrderOfDistance = sc.GetStarsInDistanceOrderFrom(stHome);
            SystemsInOrderOfDistance.Remove(stHome);
            int nsys = maxSize / 5;
            do {
                if (SystemsInOrderOfDistance.Count == 0) break;
                Star st = SystemsInOrderOfDistance.First(); // Get nearest uncolonised system
                for (int ntry = 0; ntry < 50; ntry++) { // Attempt lots of times to colonise it with a single size 1 colony.
                    if (st.AddPopulationInSystem(rc, rand)) {
                        rc.Colonise(st);
                        stHome.AddTradeRoute(st);
                        if (isPlayer) st.SetVisited(true);
                        break;
                    }
                }
                SystemsInOrderOfDistance.Remove(st);
            } while (rc.SystemCount < nsys);

            // Add extra colonies up to required population size
            while (rc.Population < maxSize) {
                Star st = stHome;
                int r = rand.Next(100);
                if (r < 10) st = sc.GetClosestNonColonisedSystemTo(stHome) ?? stHome;
                else if (r < 80) st = sc.GetRandomColonisedSystem(rand) ?? stHome;
                if (st is null) continue;
                if (st.AddPopulationInSystem(rc, rand)) {
                    rc.Colonise(st);
                    if (st != stHome) stHome.AddTradeRoute(st); // Doesn't add duplicates so no worries
                    if (isPlayer) st.SetVisited(true);
                }
            }

            // Set up extra trade routes between nearest neighbours
            foreach (Star st1 in rc.Systems) {
                if (st1 == stHome) continue;
                st1.MaybeAddTradeRoute(rc, false);
            }

        }

        // Save this map to an Xml file
        public void SaveToFile(StreamWriter file) {
            file.WriteLine("<Map Seed=\"" + MapSeed + "\" SPS=\"" + StarsPerSector + "\" PD=\"" + PlanetDensity + "\">");
            // Save the sectors
            foreach (Sector sec in dSectors.Values) {
                if (sec.ShouldBeSaved()) sec.SaveToFile(file);
            }
            file.WriteLine("</Map>");
        }

        // Get a sector that exists, or create a new one
        public Sector GetSector(Tuple<int, int> tp) {
            if (dSectors.TryGetValue(tp, out Sector? sect)) return sect;
            Sector sc = new Sector(tp.Item1, tp.Item2, this);
            dSectors.Add(tp, sc);
            return sc;
        }

        // Get a sector that exists, or create a new one
        public Sector GetSector(int sx, int sy) {
            Tuple<int, int> tp = new Tuple<int, int>(sx, sy);
            if (dSectors.TryGetValue(tp, out Sector? sect)) return sect;
            Sector sc = new Sector(tp.Item1, tp.Item2, this);
            dSectors.Add(tp, sc);
            return sc;
        }

        public AstronomicalObject? GetAOFromLocationString(string strLoc) {
            if (!strLoc.StartsWith("(") || !strLoc.Contains(')')) throw new Exception("Illegal location string:" + strLoc);
            string strMapLoc = strLoc.Substring(0, strLoc.IndexOf(")") + 1);
            string[] bits = strMapLoc.Replace("(", "").Replace(")", "").Split(',');
            if (bits.Length != 2) throw new Exception("Couldn't parse location string : " + strLoc + " - Sector location invalid : " + strMapLoc);
            int sX = int.Parse(bits[0]);
            int sY = int.Parse(bits[1]);
            // If target sector hasn't yet been initialised then skip out of here
            if (!dSectors.ContainsKey(new Tuple<int, int>(sX, sY))) return null;
            Sector sec = dSectors[new Tuple<int, int>(sX, sY)];
            return sec.GetAOFromLocationWithinSector(strLoc.Substring(strLoc.IndexOf(")") + 1));
        }

        // Unload this map
        public void Unload() {
            MapIsInitialised = false;
            dSectors.Clear();
        }

    }
}
