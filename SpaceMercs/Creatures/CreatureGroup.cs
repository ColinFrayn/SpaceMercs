using System.Xml;

namespace SpaceMercs {
    public class CreatureGroup {
        public string Name { get; private set; }
        public string Filename { get; private set; }
        public bool FoundInCaves { get; private set; }
        public bool FoundInShips { get; private set; }
        public bool RaceSpecific { get; private set; } // True = this is a group of creatures that must be assigned to a specific alien race. False = This is just random creatures/unknown aliens
        public double QuantityScale { get; private set; } // Multiple of the standard number of creatures found in a level
        public HashSet<Planet.PlanetType> FoundIn { get; private set; }
        public int MaxRelations { get; private set; }  // Won't show up against any race with which racial relations are better than this value.
        public readonly List<CreatureType> CreatureTypes = new List<CreatureType>();
        public bool HasBoss { get { return (Boss != null); } }
        public CreatureType? Boss {
            get {
                foreach (CreatureType tp in CreatureTypes) {
                    if (tp.IsBoss) return tp;
                }
                return null;
            }
        }
        private readonly Random rand; // Let's set this here because it's easier than passing one through (and just setting it each time risks getting the exact same seed if insufficient time has passed)

        public CreatureGroup(XmlNode xml) {
            Name = xml.Attributes!["Name"]?.InnerText ?? throw new Exception("Missing Name for creature group");
            Filename = xml.SelectNodeText("Filename");
            string strLocation = xml.SelectNodeText("Locations");
            HashSet<string> Locs = new HashSet<string>(strLocation.Split(',').ToList());
            FoundIn = new HashSet<Planet.PlanetType>();
            FoundInShips = Locs.Contains("Ship");
            FoundInCaves = Locs.Contains("Cave");
            RaceSpecific = (xml.SelectSingleNode("Racial") is not null);
            QuantityScale = xml.SelectNodeDouble("QuantityScale", 1.0);
            if (RaceSpecific) {
                string strMaxRel = xml.SelectSingleNode("Racial")?.Attributes?["MaxRelations"]?.Value ?? string.Empty;
                if (!string.IsNullOrEmpty(strMaxRel)) MaxRelations = int.Parse(strMaxRel);
                else MaxRelations = 100; // Ignore
            }
            foreach (Planet.PlanetType pt in Enum.GetValues(typeof(Planet.PlanetType))) {
                if (Locs.Contains(pt.ToString())) FoundIn.Add(pt);
            }
            rand = new Random();
        }

        public Creature? GenerateRandomBoss(Race? ra, int diff, MissionLevel lev) {
            CreatureType? tp = Boss;

            // Generate a suitable level
            if (tp is null || tp.LevelMin > diff) return null; // Can't make one
            int lvl = diff;
            if (rand.NextDouble() < 0.2 && lvl > tp.LevelMin) lvl--;
            if (rand.NextDouble() < 0.2 && lvl < tp.LevelMax) lvl++;

            return new Creature(tp, lvl, lev, ra);
        }
        public Creature GenerateRandomCreature(Race? ra, int diff, MissionLevel lev) {
            // Get a creature type at random that's suitable for the difficulty and not the boss
            CreatureType tp = GetRandomNonBossCreatureType(diff);

            // Generate a suitable level
            int lvl = diff;
            if (rand.NextDouble() < 0.2 && lvl > tp.LevelMin) lvl--;
            if (rand.NextDouble() < 0.2 && lvl < tp.LevelMax) lvl++;

            return new Creature(tp, lvl, lev, ra);
        }
        private CreatureType GetRandomNonBossCreatureType(int diff) {
            int total = 0;

            foreach (CreatureType tp in CreatureTypes) {
                if (!tp.IsBoss) {
                    total += tp.FrequencyModifier;
                }
            }
            int pick = rand.Next(total);
            foreach (CreatureType tp in CreatureTypes) {
                if (!tp.IsBoss) {
                    pick -= tp.FrequencyModifier;
                    if (pick < 0) return tp;
                }
            }
            throw new Exception("Could not find random creature fitting requirements");
        }
    }
}