using System.Xml;

namespace SpaceMercs {
    public class WeaponMod {
        public string Name { get; private set; }
        public string Desc { get; private set; }
        public double Mass { get; private set; }
        public double CostMod { get; private set; }
        public bool IsMelee { get; private set; }
        public double DropoffMod { get; private set; }
        public int Range { get; private set; }
        public int Silencer { get; private set; }
        public double Accuracy { get; private set; }
        public double Damage { get; private set; }
        public string Char { get; private set; }
        public Dictionary<MaterialType, int> Materials { get; private set; }

        public WeaponMod(XmlNode xml) {
            Name = xml.GetAttributeText("Name");
            Char = xml.GetAttributeText("Char");
            Mass = xml.SelectNodeDouble("Mass");
            CostMod = xml.SelectNodeDouble("CostMod");
            Desc = xml.SelectNodeText("Desc").Trim();

            IsMelee = xml.SelectSingleNode("Melee") != null;

            DropoffMod = xml.SelectNodeDouble("DropoffMod", 1.0);
            Range = xml.SelectNodeInt("Range", 0);
            Silencer = xml.SelectNodeInt("Silencer", 0);
            Accuracy = xml.SelectNodeDouble("Accuracy", 0);
            Damage = xml.SelectNodeDouble("Damage", 0);
        }
    }
}
