using System;
using System.Collections.Generic;
using System.Xml;
using System.IO;
using SpaceMercs.Dialogs;

namespace SpaceMercs {
    class Map {
        private readonly Dictionary<Tuple<int, int>, Sector> dSectors = new Dictionary<Tuple<int, int>, Sector>();
        public bool bMapSetup = false;
        static Random rnd = new Random();
        private const int INITIAL_MAP_SIZE = 3; // Just make sure that you set up an inital map of a reasonable size. This must be >= 1!
        public int MapSeed { get; private set; }
        public int StarsPerSector { get; private set; }
        public int PlanetDensity { get; private set; }

        public Map() {
            // We're good
        }
        public Map(XmlNode xml) {
            MapSeed = Int32.Parse(xml.Attributes["Seed"].Value);
            StarsPerSector = Int32.Parse(xml.Attributes["SPS"].Value);
            PlanetDensity = Int32.Parse(xml.Attributes["PD"].Value);

            dSectors.Clear();
            foreach (XmlNode xmls in xml.SelectNodes("Sector")) {
                Sector sect = new Sector(xmls, this);
                dSectors.Add(new Tuple<int, int>(sect.SectorX, sect.SectorY), sect);
            }

            bMapSetup = true;
        }

        // Generate a map with the given races
        public void Generate(NewGame ng) {
            // Get the new values and reset our map
            Random rand = new Random(ng.Seed);
            bMapSetup = false;
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
                    SetupHomeSector(rc, dSectors[new Tuple<int, int>(0, 0)], ng, rand);
                    rc.SetAsKnown();
                }
                else {
                    Sector sc;
                    do {
                        int sx = rand.Next(3) - 1;
                        int sy = rand.Next(3) - 1;
                        sc = dSectors[new Tuple<int, int>(sx, sy)];
                    } while (sc.Inhabitant != null);
                    SetupHomeSector(rc, sc, ng, rand);
                }
            }

            // Flag everything as done
            bMapSetup = true;
        }

        // Setup the given sector as the home for this race
        private void SetupHomeSector(Race rc, Sector sc, NewGame ng, Random rand) {
            // Get the most central star to be the home system
            sc.Inhabitant = rc;
            Star stHome = sc.GetMostCentralStar();
            stHome.GenerateHomeSystem(rc);

            // Setup other stars in this sector, allied to this race, if specified in NewGame dialog.
            for (int sno = 2; sno <= ng.CivSize; sno++) {
                Star st = sc.GetClosestNonColonisedSystemTo(stHome);
                rc.Colonise(st);
                stHome.AddTradeRoute(st);
            }

            // Set up extra trade routes between nearest neighbours
            foreach (Star st1 in rc.Systems) {
                if (st1 == stHome) continue;
                Star stClosest = null;
                double best = st1.DistanceTo(stHome);
                foreach (Star st2 in rc.Systems) {
                    if (st2 == stHome) continue;
                    if (st2 == st1) continue;
                    double d = st1.DistanceTo(st2);
                    if (d < best && !st1.TradeRoutes.Contains(st2)) {
                        stClosest = st2;
                        best = d;
                    }
                }
                if (stClosest != null) {
                    st1.AddTradeRoute(stClosest);
                }
            }

        }

        // Save this map to an Xml file
        public void SaveToFile(StreamWriter file) {
            file.WriteLine("<Map Seed=\"" + MapSeed + "\" SPS=\"" + StarsPerSector + "\" PD=\"" + PlanetDensity + "\">");
            // Save the sectors
            foreach (Sector sec in dSectors.Values) {
                sec.SaveToFile(file);
            }
            file.WriteLine("</Map>");
        }

        // Get a sector that exists, or create a new one
        public Sector GetSector(Tuple<int, int> tp) {
            if (dSectors.ContainsKey(tp)) return dSectors[tp];
            Sector sc = new Sector(tp.Item1, tp.Item2, this);
            dSectors.Add(tp, sc);
            return sc;
        }

        // Get a sector that exists, or create a new one
        public Sector GetSector(int sx, int sy) {
            Tuple<int, int> tp = new Tuple<int, int>(sx, sy);
            if (dSectors.ContainsKey(tp)) return dSectors[tp];
            Sector sc = new Sector(tp.Item1, tp.Item2, this);
            dSectors.Add(tp, sc);
            return sc;
        }

        public AstronomicalObject GetAOFromLocationString(string strLoc) {
            if (!strLoc.StartsWith("(") || !strLoc.Contains(")")) throw new Exception("Illegal location string:" + strLoc);
            string strMapLoc = strLoc.Substring(0, strLoc.IndexOf(")") + 1);
            string[] bits = strMapLoc.Replace("(", "").Replace(")", "").Split(',');
            if (bits.Length != 2) throw new Exception("Couldn't parse location string : " + strLoc + " - Sector location invalid : " + strMapLoc);
            int sX = Int32.Parse(bits[0]);
            int sY = Int32.Parse(bits[1]);
            // If target sector hasn't yet been initialised then skip out of here
            if (!dSectors.ContainsKey(new Tuple<int, int>(sX, sY))) return null;
            Sector sec = dSectors[new Tuple<int, int>(sX, sY)];
            return sec.GetAOFromLocationWithinSector(strLoc.Substring(strLoc.IndexOf(")") + 1));
        }

    }
}
