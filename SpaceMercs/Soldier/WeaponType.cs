using System.Xml;

namespace SpaceMercs {
    public class WeaponType : ItemType {
        public enum DamageType { Physical, Electrical, Fire, Cold, Acid, Poison };
        public enum WeaponClass { Rifle, Shotgun, Pistol, Melee, Launcher, Sniper, Heavy, Other };
        public WeaponClass WClass { get; private set; }
        public DamageType DType { get; private set; } // Damage type
        public double Range { get; private set; } // Maximum range for this weapon
        public double Accuracy { get; private set; } // Accuracy bonus for this weapon (default = 0)
        public double DropOff { get; private set; } // For every metres distance to target, suffer a -X penalty to hit.
        public double DBase { get; private set; } // Base damage per hit
        public double DMod { get; private set; } // Random damage per hit
        public double Speed { get; private set; }  // Stamina cost to fire
        public double Area { get; private set; } // Is area-of-effect?
        public bool IsMeleeWeapon { get { return Range <= 1; } }
        public string SoundEffect { get; private set; }
        public bool IsUsable { get; private set; }
        public bool Stable { get; private set; }  // This weapon requires stability i.e. you can't move in the same turn before firing
        public int Shots { get; private set; } // Number of shots
        public int NoiseLevel { get; private set; } // Noise level is the number of squares away this weapon can be heard
        public bool Modifiable { get; private set; }
        public double Recoil { get; private set; } // Hit penalty for subsequent shots after first.
        public double Delay { get; private set; } // Delay between shots

        public WeaponType(XmlNode xml) : base(xml) {
            XmlNode nRange = xml.SelectSingleNode("Range") ?? throw new Exception("Could not find range setting for weapon type");
            string wclass = xml.GetAttributeText("Class", "Other");
            if (!Enum.TryParse<WeaponClass>(wclass, out WeaponClass wc)) {
                throw new Exception($"Could not find weapon class {wclass}");
            }
            WClass = wc;
            Range = double.Parse(nRange.InnerText);
            Speed = xml.SelectNodeDouble("Speed");
            SoundEffect = xml.SelectNodeText("Sound");
            Accuracy = nRange.GetAttributeDouble("Accuracy", 0.0);
            DropOff = nRange.GetAttributeDouble("DropOff", 0.0);
            Area = xml.GetAttributeDouble("Damage/Area", 0.0);
            NoiseLevel = xml.GetAttributeInt("Noise", 0);
            string strDam = xml.SelectNodeText("Damage");
            string[] bits = strDam.Split('+');
            if (bits.Length != 2) throw new Exception("Could not parse damage string : " + strDam);
            DBase = double.Parse(bits[0]);
            DMod = double.Parse(bits[1]);
            DType = xml.SelectNodeEnum<DamageType>("Type", DamageType.Physical);
            IsUsable = (xml.SelectSingleNode("Hidden") == null);
            Stable = (xml.SelectSingleNode("Stable") != null);
            Shots = xml.SelectNodeInt("Shots", 1);
            Modifiable = (xml.SelectSingleNode("Unmodifiable") == null);
            Recoil = xml.SelectNodeDouble("Recoil", 0.0);
            Delay = xml.SelectNodeDouble("Delay", 0.0);
        }

        public override string ToString() {
            return Name;
        }
    }
}