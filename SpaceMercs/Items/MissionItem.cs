using System.IO;
using System.Xml;

namespace SpaceMercs {
    public class MissionItem : IItem {
        public string Name {
            get {
                return LegendaryItem?.Name ?? _name;
            }
        }
        private readonly string _name;
        public string Description { get { return Name; } }
        public double Mass { get; private set; }
        public double Cost { get; private set; }
        public int Level { get; private set; }
        public IItem? LegendaryItem { get; private set; }
        public bool IsPrecursorCore { get; private set; }
        public bool IsSpaceHulkCore { get; private set; }

        private static readonly string[] ItemTypes = new string[] { "Egg", "Diamond", "Emerald", "Geode", "Bone", "Skull", "Crystal", "Ruby", "Statue", "Medal", "Device" };
        private static readonly string[] GoalAdjectives = new string[] { "Giant", "Monstrous", "Superb", "Ethereal", "Mysterious", "Ancient", "Fine" };
        private static readonly string[] GatherAdjectives = new string[] { "Alien", "Golden", "Luminous", "Shiny", "Transparent", "Glowing", "Pulsating", "Ornate" };

        private static readonly string[] ObjectiveAdjectives = new string[] { "historically significant", "potentially valuable", "fascinating", "strategically important", "mysterious", "delicate", "scientifically interesting", "large", "key" };
        private static readonly string[] ObjectiveTypes = new string[] { "structure", "crystal formation", "geologic area", "relic", "mineral deposit", "fossil deposit", "alien artefact" };

        public MissionItem(string strName, double mass, double cost) {
            _name = strName;
            Mass = mass;
            Cost = cost;
        }
        public MissionItem(IItem item) {
            LegendaryItem = item;
        }
        public MissionItem(XmlNode xml) {
            if (xml is null) throw new Exception("Null Mission");
            Mass = xml.GetAttributeDouble("Mass");
            Cost = xml.GetAttributeDouble("Cost");
            Level = xml.GetAttributeInt("Level", 0);
            IsPrecursorCore = xml.SelectSingleNode("Precursor") != null;
            IsSpaceHulkCore = xml.SelectSingleNode("Hulk") != null;
            if (xml.Attributes?["Name"] is null) {
                _name = xml.InnerText;
            }
            else {
                _name = xml.GetAttributeText("Name", string.Empty);
                XmlNode? xmlni = xml.FirstChild;
                if (!IsPrecursorCore && !IsSpaceHulkCore && xmlni is not null) {
                    LegendaryItem = Utils.LoadItem(xmlni);
                }
            }
        }

        public static MissionItem GeneratePrecursorCore(int iDiff) {
            MissionItem it = new MissionItem("Precursor Core", (double)iDiff, (double)iDiff * 150d);
            it.IsPrecursorCore = true;
            it.Level = iDiff;
            return it;
        }

        public static MissionItem GenerateSpaceHulkCore(int iDiff) {
            MissionItem it = new MissionItem("SpaceHulk Core", (double)iDiff, (double)iDiff * 80d);
            it.IsSpaceHulkCore = true;
            it.Level = iDiff;
            return it;
        }

        public void SaveToFile(StreamWriter file) {
            if (Level > 0) {
                file.Write($"<MissionItem Mass=\"{Mass:N2}\" Cost=\"{Cost:N2}\" Name=\"{Name}\" Level=\"{Level}\">");
            }
            else {
                file.Write($"<MissionItem Mass=\"{Mass:N2}\" Cost=\"{Cost:N2}\" Name=\"{Name}\">");
            }
            if (LegendaryItem is not null) {
                file.WriteLine();
                LegendaryItem.SaveToFile(file);
            }
            else if (IsSpaceHulkCore) file.Write("<Hulk/>");
            else if (IsPrecursorCore) file.Write("<Precursor/>");

            file.WriteLine("</MissionItem>");
        }

        public static MissionItem GenerateRandomGoalItem(int diff, Random rand) {
            string strName = GoalAdjectives[rand.Next(GoalAdjectives.Length)] + " " + GatherAdjectives[rand.Next(GatherAdjectives.Length)] + " " + ItemTypes[rand.Next(ItemTypes.Length)];
            double m = 3.0 + (rand.NextDouble() * 2.0);
            double c = (5.0 + rand.NextDouble()) * Math.Pow(1.25, diff - 1) * 8.0;
            return new MissionItem(strName, m, c);
        }
        public static MissionItem GenerateRandomGatherItem(int diff, Random rand) {
            string strName = GatherAdjectives[rand.Next(GatherAdjectives.Length)] + " " + ItemTypes[rand.Next(ItemTypes.Length)];
            double m = 0.8 + (rand.Next(10) / 10.0);
            double c = (2.0 + rand.NextDouble()) * Math.Pow(1.25, diff - 1) * 1.5;
            return new MissionItem(strName, m, c);
        }
        public static MissionItem GenerateRandomDefendItem(Random rand) {
            string strName = $"{ObjectiveAdjectives[rand.Next(ObjectiveAdjectives.Length)]} {ObjectiveTypes[rand.Next(ObjectiveTypes.Length)]}";
            return new MissionItem(strName, 0.0, 0.0);
        }
        public static MissionItem? TryGenerateRandomLegendaryWeapon(Random rand, int diff, Race? rc) {
            IItem? it = Utils.GenerateRandomLegendaryWeapon(rand, diff, rc);
            if (it is null) return null;
            return new MissionItem(it);
        }

        // Equality comparers so that this can be used in a Dictionary/HashSet properly
        public override int GetHashCode() {
            unchecked {
                int hash = 17;
                hash = hash * 37 + Cost.GetHashCode();
                hash = hash * 41 + Mass.GetHashCode();
                if (!string.IsNullOrEmpty(Name)) hash = hash * 23 + Name.GetHashCode();
                if (LegendaryItem is not null) hash = hash * 47 + LegendaryItem.GetHashCode();
                if (IsPrecursorCore) hash = hash ^ 131;
                if (IsSpaceHulkCore) hash = hash ^ 997;
                return hash;
            }
        }
        public override bool Equals(object? obj) {
            if (obj == null || obj is not MissionItem mi) return false;
            if (mi.Name != Name) return false;
            if (mi.Cost != Cost) return false;
            if (mi.Mass != Mass) return false;
            if ((mi.LegendaryItem is null) != (LegendaryItem is null)) return false;
            if (mi.LegendaryItem is not null && mi.LegendaryItem.GetHashCode() != LegendaryItem!.GetHashCode()) return false;
            if (IsPrecursorCore != mi.IsPrecursorCore) return false;
            if (IsSpaceHulkCore != mi.IsSpaceHulkCore) return false;
            return true;
        }
        public override string ToString() {
            return Name;
        }
    }
}
