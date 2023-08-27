using System.IO;
using System.Xml;

namespace SpaceMercs {
    public class Corpse : IItem {
        private readonly CreatureType? Type;
        private readonly string? SoldierName;
        private readonly int Level;
        public string Name { get { return "Corpse of " + ((Type == null) ? SoldierName : Type.Name); } }
        public string Desc { get { return Name; } }
        public double Mass { get { return (Type == null) ? 60.0 : Type.Scale * Type.Scale * Type.Scale * 60.0; } }
        public double Cost { get { return 0.0; } }

        public void SaveToFile(StreamWriter file) {
            if (Type != null) file.WriteLine("<Corpse Type=\"" + Type.Name + "\" Level=\"" + Level + "\"/>");
            else file.WriteLine("<Corpse Type=\"Soldier:" + Name + "\" Level=\"" + Level + "\"/>");
        }
        public bool IsSoldier { get { return !String.IsNullOrEmpty(SoldierName); } }

        public Corpse(Creature cr) {
            Type = cr.Type;
            Level = cr.Level;
            SoldierName = null;
        }
        public Corpse(Soldier s) {
            SoldierName = s.Name;
            Type = null;
            Level = s.Level;
        }
        public Corpse(XmlNode xml) {
            string strType = xml.Attributes?["Type"]?.Value ?? string.Empty;
            if (strType.StartsWith("Soldier:")) {
                SoldierName = strType.Substring(8);
                Type = null;
            }
            else {
                Type = StaticData.GetCreatureTypeByName(strType) ?? throw new Exception("Could not ID creature type for corpse : " + strType);
                SoldierName = null;
            }
            if (xml.Attributes?["Level"] is not null) {
                Level = xml.GetAttributeInt("Level");
            }
            else if (Type != null) {
                Level = Type.LevelMin;
            }
            else Level = 1;
        }

        public List<IItem> Scavenge(int lvl, Random rand) {
            List<IItem> stuff = new List<IItem>();
            if (Type == null) return stuff;
            if (Type.Scavenge.Count == 0) return stuff;
            double lvlscale = (double)(Level - Type.LevelMin) / (double)(Type.LevelMax - Type.LevelMin);

            foreach (MaterialType mt in Type.Scavenge.Keys) {
                double chance = Type.Scavenge[mt] * (0.5 + lvlscale) * Math.Pow(1.05, lvl - 1);
                int total = 0;
                while (rand.NextDouble() < chance) {
                    total++;
                    chance -= 1.0;
                }
                for (int i = 0; i < total; i++) stuff.Add(new Material(mt));
            }
            return stuff;
        }

        // Equality comparers so that this can be used in a Dictionary/HashSet properly
        public override int GetHashCode() {
            unchecked {
                int hash = 17;
                if (Type != null) hash = hash * 37 + Type.GetHashCode();
                if (!string.IsNullOrEmpty(SoldierName)) hash = hash * 23 + SoldierName.GetHashCode();
                return hash;
            }
        }
        public override bool Equals(object? obj) {
            if (obj is null || GetType() != obj.GetType()) return false;
            if (obj is not Corpse cp) return false;
            return (cp.SoldierName == SoldierName && cp.Type == Type);
        }
        public override string ToString() {
            return Name;
        }
    }
}
