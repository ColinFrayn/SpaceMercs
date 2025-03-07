using System.Xml;

namespace SpaceMercs {
    public class WeaponType : ItemType {
        public enum DamageType { Physical, Electrical, Fire, Cold, Acid, Poison };
        public enum ShotType { Single, Cone, ConeMulti, Grenade };
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
        public double Width { get; private set; } // Cone width if cone-type shot
        public bool IsMeleeWeapon { get { return WClass == WeaponClass.Melee; } }
        public string SoundEffect { get; private set; }
        public bool IsUsable { get; private set; }
        public bool Stable { get; private set; }  // This weapon requires stability i.e. you can't move in the same turn before firing
        public int Shots { get; private set; } // Number of shots
        public int NoiseLevel { get; private set; } // Noise level is the number of squares away this weapon can be heard
        public bool Modifiable { get; private set; }
        public double Recoil { get; private set; } // Hit penalty for subsequent shots after first.
        public double Delay { get; private set; } // Delay between shots
        public double BaseDelay { get; private set; } // Delay before the first shot
        public double ShotLength { get; private set; }
        public double ShotSpeed { get; private set; }
        public Color ShotColor { get; private set; }
        public ShotType WeaponShotType { get; private set; }
        public int Recharge { get; private set; }

        public WeaponType(XmlNode xml) : base(xml) {
            string wclass = xml.GetAttributeText("Class", "Other");
            if (!Enum.TryParse<WeaponClass>(wclass, out WeaponClass wc)) {
                throw new Exception($"Could not find weapon class {wclass}");
            }
            WClass = wc;

            XmlNode nRange = xml.SelectSingleNode("Range") ?? throw new Exception($"Could not find range setting for weapon type {Name}");
            Range = double.Parse(nRange.InnerText);
            Accuracy = nRange.GetAttributeDouble("Accuracy", 0.0);
            DropOff = nRange.GetAttributeDouble("DropOff", 0.0);
            ShotLength = nRange.GetAttributeDouble("Length", 0.0);
            ShotSpeed = nRange.GetAttributeDouble("Speed", 1.0);
            string strCol = nRange.GetAttributeText("Colour", string.Empty);
            ShotColor = Color.FromArgb(255, 200, 200, 200);
            if (!string.IsNullOrEmpty(strCol)) {
                string[] cbits = strCol.Split(',');
                if (cbits.Length != 3) throw new Exception($"Could not parse Colour string \"{strCol}\" in weapon {Name}");
                int r = (int)(double.Parse(cbits[0]) * 255.0);
                int g = (int)(double.Parse(cbits[1]) * 255.0);
                int b = (int)(double.Parse(cbits[2]) * 255.0);
                ShotColor = Color.FromArgb(255, r, g, b);
            }

            Speed = xml.SelectNodeDouble("Speed");
            SoundEffect = xml.SelectNodeText("Sound");
            NoiseLevel = xml.GetAttributeInt("Noise", 0);
            XmlNode nDam = xml.SelectSingleNode("Damage") ?? throw new Exception($"Could not find damage setting for weapon {Name}"); ;
            string strDam = nDam.InnerText;
            string[] bits = strDam.Split('+');
            if (bits.Length != 2) throw new Exception($"Could not parse damage string \"{strDam}\" in weapon {Name}");
            DBase = double.Parse(bits[0]);
            DMod = double.Parse(bits[1]);
            Area = nDam.GetAttributeDouble("Area", 0.0);
            Width = nDam.GetAttributeDouble("Width", 0.0);
            WeaponShotType = nDam.GetAttributeEnum<ShotType>("Type", ShotType.Single);

            DType = xml.SelectNodeEnum<DamageType>("Type", DamageType.Physical);
            IsUsable = (xml.SelectSingleNode("Hidden") == null);
            Stable = (xml.SelectSingleNode("Stable") != null);
            Shots = xml.SelectNodeInt("Shots", 1);
            Modifiable = (xml.SelectSingleNode("Unmodifiable") == null);
            Recharge = xml.SelectNodeInt("Recharge", 0);
            Recoil = xml.SelectNodeDouble("Recoil", 0.0);
            Delay = xml.SelectNodeDouble("Delay", 0.0);
            BaseDelay = xml.SelectNodeDouble("BaseDelay", 0.0);
        }

        public override string ToString() {
            return Name;
        }
    }
}