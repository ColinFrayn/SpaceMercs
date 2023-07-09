using System.Xml;

namespace SpaceMercs {
    class MaterialType {
        public string Name { get; private set; }
        public double MassMod { get; private set; } // Modifier (default = 1.0)
        public double CostMod { get; private set; } // Modifier (default = 1.0)
        public double ArmourMod { get; private set; } // Modifier (default = 0.0)
        public double Rarity { get; private set; } // Modifier (default = 1.0)
        public string Desc { get; private set; }
        public double UnitMass { get; private set; } // Mass of one unit, in kg. This is the amount that is sold (default = 1.0)
        public bool IsArmourMaterial { get { return (ArmourMod > 0.0); } }
        public readonly Dictionary<WeaponType.DamageType, double> BonusArmour = new Dictionary<WeaponType.DamageType, double>();
        public double ConstructionChanceModifier { get { return Math.Log(Rarity > 0 ? Rarity : 0.0001) * 10.0; } }

        public MaterialType(XmlNode xml) {
            Name = xml.GetAttributeValue("Name");
            CostMod = xml.SelectNodeDouble("CostMod", 1.0);
            MassMod = xml.SelectNodeDouble("MassMod", 1.0);
            Rarity = xml.SelectNodeDouble("Rarity", 1.0);
            ArmourMod = xml.SelectNodeDouble("ArmourMod", 0.0);
            UnitMass = xml.SelectNodeDouble("UnitMass", 1.0);
            Desc = xml.SelectNodeText("Desc").Trim();

            // Special resistances if this is made into armour
            foreach (XmlNode xn in xml.SelectNodesToList("BonusArmour/Bonus")) {
                WeaponType.DamageType type = (WeaponType.DamageType)Enum.Parse(typeof(WeaponType.DamageType), xn.GetAttributeValue("Type"));
                double val = double.Parse(xn.GetAttributeValue("Amount"));
                BonusArmour.Add(type, val);
            }
        }
    }
}
