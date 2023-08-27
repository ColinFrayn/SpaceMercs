using System.Xml;

namespace SpaceMercs {
    public class ArmourType : ItemType {
        public int Strength { get; private set; } // Bonus
        public int Agility { get; private set; } // Bonus
        public int Intellect { get; private set; } // Bonus
        public int Toughness { get; private set; } // Bonus
        public int Endurance { get; private set; } // Bonus
        public int Health { get; private set; } // Bonus
        public int Stamina { get; private set; } // Bonus
        public int Shields { get; private set; } // Bonus
        public int Attack { get; private set; } // Bonus
        public int Defence { get; private set; } // Bonus
        public double Speed { get; private set; } // Divides movment cost -> higher number = faster
        public int BaseArmour { get; private set; }
        public int Size {
            get {
                double sz = 0.0;
                foreach (BodyPart bp in Locations) sz += Utils.BodyPartToArmourScale(bp);
                return (int)Math.Ceiling(sz);
            }
        }
        public readonly HashSet<BodyPart> Locations = new HashSet<BodyPart>();
        public readonly Dictionary<WeaponType.DamageType, double> BonusArmour = new Dictionary<WeaponType.DamageType, double>();

        public ArmourType(XmlNode xml) : base(xml) {
            Strength = xml.SelectNodeInt("Strength", 0);
            Agility = xml.SelectNodeInt("Agility", 0);
            Intellect = xml.SelectNodeInt("Intellect", 0);
            Toughness = xml.SelectNodeInt("Toughness", 0);
            Endurance = xml.SelectNodeInt("Endurance", 0);
            Health = xml.SelectNodeInt("Health", 0);
            Stamina = xml.SelectNodeInt("Stamina", 0);
            Shields = xml.SelectNodeInt("Shields", 0);
            Attack = xml.SelectNodeInt("Attack", 0);
            Defence = xml.SelectNodeInt("Defence", 0);
            BaseArmour = xml.SelectNodeInt("BaseArmour", 0);
            Speed = xml.SelectNodeDouble("Speed", 1.0);
            if (xml.SelectSingleNode("BonusArmour") != null) {
                foreach (XmlNode xn in xml.SelectNodesToList("BonusArmour/Bonus")) {
                    WeaponType.DamageType type = (WeaponType.DamageType)Enum.Parse(typeof(WeaponType.DamageType), xn.GetAttributeText("Type"));
                    double val = xn.GetAttributeDouble("Amount");
                    BonusArmour.Add(type, val);
                }
            }
            string strLocation = xml.SelectNodeText("Location");
            foreach (string strLoc in strLocation.Split(',')) {
                BodyPart bp = (BodyPart)Enum.Parse(typeof(BodyPart), strLoc);
                if (Locations.Contains(bp)) throw new Exception("Duplicate body part covered by armour type " + Name);
                Locations.Add(bp);
            }
        }

        public override string ToString() {
            return Name;
        }
    }
}
