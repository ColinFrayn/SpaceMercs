using System.Xml;

namespace SpaceMercs {
    public class MaterialType {
        public string Name { get; private set; }
        public double MassMod { get; private set; } // Modifier (default = 1.0)
        public double ItemCost { get; private set; } // Modifier (default = 1.0)
        public double ArmourMod { get; private set; } // Modifier (default = 0.0)
        public double Rarity { get; private set; } // Modifier (default = 1.0)
        public bool IsScavenged { get; private set; } // Can this material only be obtained from dead creatures?
        public string Desc { get; private set; }
        public double UnitMass { get; private set; } // Mass of one unit, in kg. This is the amount that is sold (default = 1.0)
        public Race? RequiredRace { get; private set; }
        public bool IsArmourMaterial { get { return (ArmourMod > 0.0); } }
        public readonly Dictionary<WeaponType.DamageType, double> BonusArmour = new Dictionary<WeaponType.DamageType, double>();
        public double ConstructionChanceModifier { get { return Math.Log(Rarity > 0 ? Rarity : 0.0001) * 10.0; } }

        public MaterialType(XmlNode xml) {
            Name = xml.GetAttributeText("Name");
            ItemCost = xml.SelectNodeDouble("ItemCost", 1.0);
            MassMod = xml.SelectNodeDouble("MassMod", 1.0);
            Rarity = xml.SelectNodeDouble("Rarity", 1.0);
            ArmourMod = xml.SelectNodeDouble("ArmourMod", 0.0);
            UnitMass = xml.SelectNodeDouble("UnitMass", 1.0);
            Desc = xml.SelectNodeText("Desc").Trim();
            IsScavenged = xml.SelectSingleNode("Scavenged") != null;

            // Special resistances if this is made into armour
            foreach (XmlNode xn in xml.SelectNodesToList("BonusArmour/Bonus")) {
                WeaponType.DamageType type = (WeaponType.DamageType)Enum.Parse(typeof(WeaponType.DamageType), xn.GetAttributeText("Type"));
                double val = xn.GetAttributeDouble("Amount");
                BonusArmour.Add(type, val);
            }

            // Load the race that this equipment is restricted to (default null), or otherwise fail
            if (xml.SelectSingleNode("Race") != null) {
                RequiredRace = StaticData.GetRaceByName(xml.SelectNodeText("Race"));
                if (RequiredRace == null) {
                    throw new Exception("Could not find restricted race \"" + xml.SelectNodeText("Race") + "\" for equipment " + Name);
                }
            }
        }
    }
}
