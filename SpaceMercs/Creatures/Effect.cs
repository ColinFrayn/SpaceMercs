using System.IO;
using System.Xml;

namespace SpaceMercs {
    class Effect {
        public string Name { get; private set; }
        public double Damage { get; private set; }
        public WeaponType.DamageType DamageType { get; private set; }
        public int Duration { get; private set; }
        public string SoundEffect { get; private set; }
        private readonly Dictionary<StatType, int> StatMods;
        public double SpeedMod { get; private set; }

        public Effect(XmlNode xml) {
            Name = xml.Attributes!["Name"]?.Value ?? "";
            Damage = 0.0;
            DamageType = WeaponType.DamageType.Physical;
            if (xml.SelectSingleNode("Damage") is not null) {
                Damage = double.Parse(xml.SelectNodeText("Damage"));
                DamageType = (WeaponType.DamageType)Enum.Parse(typeof(WeaponType.DamageType), xml.SelectSingleNode("Damage")!.Attributes!["Type"]?.Value ?? string.Empty);
            }
            Duration = xml.SelectNodeInt("Duration", 0);
            if (Duration > 0 && string.IsNullOrEmpty(Name)) throw new Exception("If Effect duration > 0 then effect must have a name");

            SoundEffect = xml.SelectNodeText("Sound");
            
            SpeedMod = xml.SelectNodeDouble("SpeedMod", 1.0);

            StatMods = new Dictionary<StatType, int>();
            XmlNode? sm = xml.SelectSingleNode("StatMods");
            if (sm != null) {
                foreach (XmlNode xn in sm.SelectNodesToList("Mod")) {
                    string strMod = xn.Attributes!["Stat"]!.Value;
                    StatType st = (StatType)Enum.Parse(typeof(StatType), strMod);
                    int val = int.Parse(xn.InnerText);
                    if (StatMods.ContainsKey(st)) throw new Exception("Loading Effect : " + Name + "; Duplicate stat mod : " + st);
                    StatMods.Add(st, val);
                }
            }
        }
        public Effect(Effect e) {
            Name = e.Name;
            Damage = e.Damage;
            DamageType = e.DamageType;
            Duration = e.Duration;
            SoundEffect = e.SoundEffect;
            SpeedMod = e.SpeedMod;
            StatMods = new Dictionary<StatType, int>(e.StatMods);
        }

        public void SaveToFile(StreamWriter file) {
            file.WriteLine("<Effect Name=\"" + Name + "\">");
            if (Damage != 0.0) file.WriteLine(" <Damage Type=\"" + DamageType.ToString() + "\">" + Damage + "</Damage>");
            file.WriteLine(" <Duration>" + Duration + "</Duration>");
            if (!string.IsNullOrEmpty(SoundEffect)) file.WriteLine(" <Sound>" + SoundEffect + "</Sound>");
            if (SpeedMod != 1.0) file.WriteLine(" <SpeedMod>" + SpeedMod + "</SpeedMod>");
            if (StatMods.Count > 0) {
                file.WriteLine(" <StatMods>");
                foreach (StatType st in StatMods.Keys) {
                    file.WriteLine("  <Mod Stat=\"" + st + "\">" + StatMods[st] + "</Mod>");
                }
                file.WriteLine(" </StatMods>");
            }
            file.WriteLine("</Effect>");
        }

        public void ReduceDuration(int d = 1) {
            Duration -= d;
        }

        public void AddStatMod(StatType st, int mod) {
            if (st == StatType.Attack || st == StatType.Defence || st == StatType.Stamina) {
                if (StatMods.ContainsKey(st)) StatMods[st] += mod;
                else StatMods.Add(st, mod);
            }
            else throw new Exception("Effects can only affect Att, Def, Sta");
        }

        public int GetStatMod(StatType st) {
            //if (st == StatType.Attack || st == StatType.Defence || st == StatType.Stamina) {
            if (StatMods.ContainsKey(st)) return StatMods[st];
            //}
            return 0;
        }

        public override bool Equals(object? obj) {
            if (obj is not Effect eff) return false;
            return Name.Equals(eff.Name);
        }

        public override int GetHashCode() {
            return base.GetHashCode();
        }
    }
}
