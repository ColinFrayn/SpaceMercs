using System.IO;
using System.Xml;

namespace SpaceMercs {
    public class MissionItem : IItem {
        public string Name { get; private set; }
        public string Desc { get { return Name; } }
        public double Mass { get; private set; }
        public double Cost { get; private set; }
        public void SaveToFile(StreamWriter file) {
            file.WriteLine("<MissionItem Mass=\"" + Mass.ToString("N2") + "\" Cost=\"" + Cost.ToString("N2") + "\">" + Name + "</MissionItem>");
        }

        private static readonly string[] ItemTypes = new string[] { "Egg", "Diamond", "Emerald", "Geode", "Bone", "Skull", "Crystal", "Ruby", "Statue", "Medal", "Device" };
        private static readonly string[] GoalAdjectives = new string[] { "Giant", "Monstrous", "Superb", "Ethereal", "Mysterious", "Ancient", "Fine" };
        private static readonly string[] GatherAdjectives = new string[] { "Alien", "Golden", "Luminous", "Shiny", "Transparent", "Glowing", "Pulsating", "Ornate" };

        private static readonly string[] ObjectiveAdjectives = new string[] { "historically significant", "potentially valuable", "fascinating", "strategically important", "mysterious", "delicate", "scientifically interesting", "large", "key" };
        private static readonly string[] ObjectiveTypes = new string[] { "structure", "crystal formation", "geologic area", "relic", "mineral deposit", "fossil deposit", "alien artefact" };


        public MissionItem(string strName, double m, double c) {
            Name = strName;
            Mass = m;
            Cost = c;
        }
        public MissionItem(XmlNode xml) {
            Mass = xml.GetAttributeDouble("Mass");
            Cost = xml.GetAttributeDouble("Cost");
            Name = xml.InnerText;
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
            double c = (2.0 + rand.NextDouble()) * Math.Pow(1.25, diff - 1) * 0.8;
            return new MissionItem(strName, m, c);
        }
        public static MissionItem GenerateRandomDefendItem(Random rand) {
            string strName = $"{ObjectiveAdjectives[rand.Next(ObjectiveAdjectives.Length)]} {ObjectiveTypes[rand.Next(ObjectiveTypes.Length)]}";
            return new MissionItem(strName, 0.0, 0.0);
        }

        // Equality comparers so that this can be used in a Dictionary/HashSet properly
        public override int GetHashCode() {
            unchecked {
                int hash = 17;
                hash = hash * 37 + Cost.GetHashCode();
                hash = hash * 37 + Mass.GetHashCode();
                if (!String.IsNullOrEmpty(Name)) hash = hash * 23 + Name.GetHashCode();
                return hash;
            }
        }
        public override bool Equals(object? obj) {
            if (obj == null || GetType() != obj.GetType()) return false;
            if (((MissionItem)obj).Name != Name) return false;
            if (((MissionItem)obj).Cost != Cost) return false;
            if (((MissionItem)obj).Mass != Mass) return false;
            return true;
        }
        public override string ToString() {
            return Name;
        }
    }
}
