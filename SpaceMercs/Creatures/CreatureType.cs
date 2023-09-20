using System.Xml;

namespace SpaceMercs {
    public class CreatureType {
        public enum BodyType { Humanoid, Bug, Snake, Lizard, Beetle, Arachnid, Xenomorph, Centipede, Scorpion, Dragon, Plant, Slime, ShadowBeast, VoidBeast }
        public enum LootType { Soldier, Trader, Exotic }
        public string Name { get; private set; }
        public int Size { get; private set; } // Default = 1
        public double Scale { get; private set; } // Default = (double)Size
        public int LevelMin { get; private set; }
        public int LevelMax { get; private set; }
        public int HealthBase { get; private set; }
        public int StaminaBase { get; private set; }
        public int ShieldsBase { get; private set; }
        public int AttackBase { get; private set; }
        public int DefenceBase { get; private set; }
        public int ArmourBase { get; private set; }
        public string Description { get; private set; }
        public CreatureGroup Group { get; private set; }
        public bool IsBoss { get; private set; } // Default = false
        public int TextureID { get; private set; }
        public int TextureShieldsID { get; private set; }
        public BodyType Body { get; private set; } // Default = humanoid
        public bool Corporeal { get; private set; }
        public bool Interact { get; private set; }  // Can interact with objects? (i.e. open doors) Default = false
        public double MovementCost { get; private set; }
        public int FrequencyModifier { get; private set; }
        public Color Col1 { get; private set; }
        public Color Col2 { get; private set; }
        public Color Col3 { get; private set; }
        public readonly Dictionary<WeaponType, int> Weapons = new Dictionary<WeaponType, int>();
        public readonly Dictionary<WeaponType.DamageType, double> Resistances = new Dictionary<WeaponType.DamageType, double>();
        public readonly Dictionary<MaterialType, double> Scavenge = new Dictionary<MaterialType, double>();
        private readonly int WeaponTotalWeight = 0;

        public CreatureType(XmlNode xml, CreatureGroup gp) {
            Name = xml.Attributes?["Name"]?.InnerText ?? throw new Exception("CreatureType missing Name");
            Size = xml.SelectNodeInt("Size", 1);
            Scale = xml.SelectNodeDouble("Scale", (double)Size);

            string strLevel = xml.SelectNodeText("LevelRange");
            string[] bits = strLevel.Split(',');
            if (bits.Length != 2) throw new Exception(Name + " : Could not parse level range string : " + strLevel);
            LevelMin = int.Parse(bits[0]);
            LevelMax = int.Parse(bits[1]);

            HealthBase = xml.SelectNodeInt("Health");
            StaminaBase = xml.SelectNodeInt("Stamina");
            ArmourBase = xml.SelectNodeInt("Armour");

            ShieldsBase = xml.SelectNodeInt("Shields", 0);

            MovementCost = xml.SelectNodeDouble("MovementCost", Const.MovementCost);

            AttackBase = xml.SelectNodeInt("Attack");
            DefenceBase = xml.SelectNodeInt("Defence");

            Body = xml.SelectNodeEnum<BodyType>("BodyType", BodyType.Humanoid);

            FrequencyModifier = xml.SelectNodeInt("Frequency", 1);

            // Base colours
            Col1 = Color.Black;
            Col2 = Color.Gray;
            Col3 = Color.SaddleBrown;
            if (xml.SelectSingleNode("Col1") is not null) {
                bits = xml.SelectNodeText("Col1").Split(',');
                if (bits.Length != 3) throw new Exception("Could not parse Colour1 string : " + xml.SelectNodeText("Col1"));
                Col1 = Color.FromArgb(255, int.Parse(bits[0]), int.Parse(bits[1]), int.Parse(bits[2]));
            }
            if (xml.SelectSingleNode("Col2") is not null) {
                bits = xml.SelectNodeText("Col2").Split(',');
                if (bits.Length != 3) throw new Exception("Could not parse Colour2 string : " + xml.SelectNodeText("Col2"));
                Col2 = Color.FromArgb(255, int.Parse(bits[0]), int.Parse(bits[1]), int.Parse(bits[2]));
            }
            if (xml.SelectSingleNode("Col3") is not null) {
                bits = xml.SelectNodeText("Col3").Split(',');
                if (bits.Length != 3) throw new Exception("Could not parse Colour3 string : " + xml.SelectNodeText("Col3"));
                Col3 = Color.FromArgb(255, int.Parse(bits[0]), int.Parse(bits[1]), int.Parse(bits[2]));
            }

            // Special resistances
            if (xml.SelectSingleNode("Resistances") is not null) {
                foreach (XmlNode xn in xml.SelectNodesToList("Resistances/Resistance")) {
                    WeaponType.DamageType tp = (WeaponType.DamageType)Enum.Parse(typeof(WeaponType.DamageType), xn.Attributes!["Type"]?.Value ?? string.Empty);
                    if (Resistances.ContainsKey(tp)) throw new Exception("Duplicate resistance of type " + tp + " for creature " + Name);
                    double res = xn.GetAttributeDouble("Value");
                    Resistances.Add(tp, res);
                }
            }

            // Scavenging results
            if (xml.SelectSingleNode("Scavenge") is not null) {
                foreach (XmlNode xn in xml.SelectNodesToList("Scavenge/Item")) {
                    double amount = xn.GetAttributeDouble("Amount");
                    MaterialType ? tp = StaticData.GetMaterialTypeByName(xn.InnerText);
                    if (tp == null) throw new Exception("Couldn't identify scavenging material " + xn.InnerText + " in creature type " + Name);
                    if (Scavenge.ContainsKey(tp)) throw new Exception("Duplicate scavenging result of type " + tp + " for creature " + Name);
                    Scavenge.Add(tp, amount);
                }
            }

            // Weapons
            if (xml.SelectSingleNode("Weapons") is not null) {
                foreach (XmlNode xn in xml.SelectNodesToList("Weapons/Weapon")) {
                    WeaponType? wpt = StaticData.GetWeaponTypeByName(xn.InnerText);
                    if (wpt == null) throw new Exception("Creature " + Name + " has unknown weapon : " + xn.InnerText);
                    int wgt = xn.GetAttributeInt("Weight");
                    if (Weapons.ContainsKey(wpt)) throw new Exception("Creature " + Name + " has repeated weapons : " + wpt.Name);
                    Weapons.Add(wpt, wgt);
                    WeaponTotalWeight += wgt;
                }
            }
            else {
                WeaponTotalWeight = 1;
                WeaponType? wtyp = StaticData.GetWeaponTypeByName("Bite");
                if (wtyp != null) Weapons.Add(wtyp, 1);
            }

            // Misc
            Description = xml.SelectNodeText("Desc");
            IsBoss = (xml.SelectSingleNode("Boss") is not null);
            Corporeal = (xml.SelectSingleNode("NonCorporeal") is null);
            Interact = (xml.SelectSingleNode("Interact") is not null);
            Group = gp;
            TextureID = -1;
            TextureShieldsID = -1;
        }

        public void GenerateTexture(bool bShields) {
            if (bShields) {
                if (TextureShieldsID != -1) return;
                TextureShieldsID = Textures.GenerateCreatureTexture(this, true);
            }
            else {
                if (TextureID != -1) return;
                TextureID = Textures.GenerateCreatureTexture(this, false);
            }
        }

        public Weapon? GenerateRandomWeapon() {
            if (WeaponTotalWeight == 0) return null;
            Random rnd = new Random();
            int r = rnd.Next(WeaponTotalWeight);
            foreach (WeaponType tp in Weapons.Keys) {
                if (r < Weapons[tp]) return new Weapon(tp, 0);
                r -= Weapons[tp];
            }
            return null;
        }

        public override string ToString() {
            return Name;
        }
    }
}