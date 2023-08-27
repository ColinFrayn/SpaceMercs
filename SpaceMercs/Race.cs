using System.Xml;
using System.IO;

namespace SpaceMercs {
    public class Race {
        public string Name { get; set; }
        public double Scale { get; set; }
        public int Strength { get; private set; }
        public int Intellect { get; private set; }
        public int Toughness { get; private set; }
        public int Agility { get; private set; }
        public int Endurance { get; private set; }
        public double BaseTemp;
        public string Description { get; private set; }
        public Planet.PlanetType PlanetType { get; private set; } // Preferred planet type
        public Planet HomePlanet { get; private set; }
        public int Relations { get; private set; } // Ambassadorial relations between this race the player team. 0 = neutral, >0 = good, <0 = bad
        public bool Known { get; private set; } // Have we met them yet?
        public readonly List<Star> Systems = new List<Star>();
        private readonly List<Colony> Colonies = new List<Colony>();
        private readonly Dictionary<GenderType, List<string>> FirstNames = new Dictionary<GenderType, List<string>>();
        private readonly List<string> FamilyNames;
        public Color Colour { get; private set; }
        public int ColonyCount { get { return Colonies.Count; } }
        public int SystemCount { get { return Systems.Count; } }

        public Race(XmlNode xml) {
            Name = xml.SelectNodeText("Name");
            Scale = xml.SelectNodeDouble("Scale");
            Intellect = xml.SelectNodeInt("Intellect"); 
            Strength = xml.SelectNodeInt("Strength");
            Toughness = xml.SelectNodeInt("Toughness");
            Agility = xml.SelectNodeInt("Agility");
            Endurance = xml.SelectNodeInt("Endurance");
            Description = xml.SelectNodeText("Description");
            BaseTemp = xml.SelectNodeInt("BaseTemp");
            PlanetType = xml.SelectNodeEnum<Planet.PlanetType>("PlanetType");
            string strCol = xml.SelectNodeText("Colour");
            string[] bits = strCol.Split(',');
            Colour = Color.FromArgb(255, int.Parse(bits[0]), int.Parse(bits[1]), int.Parse(bits[2]));
            XmlNode nPersonal = xml.SelectSingleNode("Names/Personal") ?? throw new Exception($"Could not find personal names list for Race {Name}");
            foreach (XmlNode xn in nPersonal.ChildNodes) {
                List<string> lNames = xn.InnerText.Split(',').ToList<string>();
                if (lNames.Count == 0) throw new Exception("No names defined for Gender=" + xn.Name + ", Race=" + Name);
                GenderType gt = (GenderType)Enum.Parse(typeof(GenderType), xn.Name);
                if (FirstNames.ContainsKey(gt)) throw new Exception("Duplicate gender key " + xn.Name + " for race names for " + Name + " race");
                FirstNames.Add(gt, lNames);
            }
            FamilyNames = xml.SelectNodeText("Names/Family").Split(',').ToList<string>();
            if (FamilyNames.Count == 0) throw new Exception("No family names defined for race " + Name);
            if (FirstNames.Keys.Count == 0) throw new Exception("No possible genders defined for race " + Name + ". Please add some names!");
            Reset();
        }

        public void SaveAdditionalData(StreamWriter file) {
            file.WriteLine("<Race Name=\"" + Name + "\">");
            file.WriteLine("<HomePlanet>" + HomePlanet.PrintCoordinates() + "</HomePlanet>");
            if (Known) file.WriteLine("<Known/>");
            file.WriteLine("<Relations>" + Relations + "</Relations>");
            file.WriteLine("</Race>");
        }
        public void LoadAdditionalData(XmlNode xml, Map map) {
            AstronomicalObject? aoHome = map.GetAOFromLocationString(xml.SelectNodeText("HomePlanet"));
            if (aoHome is null || aoHome.AOType != AstronomicalObject.AstronomicalObjectType.Planet) throw new Exception("Home Planet corrupted in data file (not a planet!)");
            HomePlanet = (Planet)aoHome;
            Known = (xml.SelectSingleNode("Known") is not null);
            Relations = xml.SelectNodeInt("Relations");
        }

        public void SetHomePlanet(Planet pl) {
            HomePlanet = pl;
            pl.GetSystem().SetName(Name + " Home");
            pl.SetupBase(this, 5);
        }

        public void Colonise(Star st) {
            st.SetOwner(this);
            st.InsertColoniesForRace(this, Const.InitialColonyCount);
            AddSystem(st);
            if (Const.DEBUG_VIEW_ALL_CIVS) st.SetVisited(true);
        }

        public void AddSystem(Star st) {
            if (!Systems.Contains(st)) Systems.Add(st);
            if (st.Owner == null) st.SetOwner(this);
        }
        public void AddColony(Colony cl) {
            if (!Colonies.Contains(cl)) Colonies.Add(cl);
        }

        public void Reset() {
            HomePlanet = Planet.Empty;
            Relations = 0;
            Known = false;
            Systems.Clear();
        }

        public GenderType GenerateRandomGender(Random rand) {
            int total = 0;
            foreach (GenderType gt in FirstNames.Keys) {
                total += FirstNames[gt].Count;
            }
            int r = rand.Next(total);
            foreach (GenderType gt in FirstNames.Keys) {
                r -= FirstNames[gt].Count;
                if (r < 0) return gt;
            }
            throw new Exception("Error in Race.GenerateRandomGender!");
        }
        public string GenerateRandomName(Random rand, GenderType gt) {
            if (!FirstNames.ContainsKey(gt)) throw new Exception("Attemptign to generate name for soldier of impossible gender for this race (" + Name + ", " + gt + ")");
            int r = rand.Next(FirstNames[gt].Count);
            string strName = (FirstNames[gt])[r] + " ";
            r = rand.Next(FamilyNames.Count);
            strName += FamilyNames[r];
            return strName;
        }

        public void SetAsKnown() {
            Known = true;
            Relations = 0;
        }

        public string RelationsToString() {
            if (!Known) return "No Contact";
            return Utils.RelationsToString(Relations);
        }

        public Star GetNearestSystemTo(Star st) {
            if (Systems.Contains(st)) return st;
            double closest = 100000.0;
            Star stBest = st;
            foreach (Star st2 in Systems) {
                if (stBest == st || st.DistanceTo(st2) < closest) {
                    closest = st.DistanceTo(st2);
                    stBest = st2;
                }
            }
            return stBest;
        }
        public Star GetNearestSystemToNotIncludingSelf(Star st) {
            double closest = 100000.0;
            Star stBest = st;
            foreach (Star st2 in Systems) {
                if (st2 == st) continue;
                if (stBest == st || st.DistanceTo(st2) < closest) {
                    closest = st.DistanceTo(st2);
                    stBest = st2;
                }
            }
            return stBest;
        }
    }
}
