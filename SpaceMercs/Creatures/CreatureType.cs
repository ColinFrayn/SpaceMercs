using System.Xml;

namespace SpaceMercs {
    public class CreatureType {
        public enum BodyType { Humanoid, Bug, Snake, Lizard, Beetle, Arachnid, Xenomorph, Centipede, Scorpion, Dragon, Plant, Slime, ShadowBeast, VoidBeast, Fungoid, Mechanoid, Gremlin, Tank, Crystalline, Mephit, Elemental }
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
        public readonly Dictionary<WeaponType, (int Weight, int MinLev, int MaxLev)> Weapons = new Dictionary<WeaponType, (int Weight, int MinLev, int MaxLev)>();
        public readonly Dictionary<WeaponType.DamageType, double> Resistances = new Dictionary<WeaponType.DamageType, double>();
        public readonly Dictionary<MaterialType, double> Scavenge = new Dictionary<MaterialType, double>();
        public ItemEffect? OnDeathEffect { get; private set; }

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
                    int wgt = xn.GetAttributeInt("Weight", 1);
                    int minlev = xn.GetAttributeInt("MinLev", 1);
                    int maxlev = xn.GetAttributeInt("MaxLev", 100);
                    if (Weapons.ContainsKey(wpt)) throw new Exception("Creature " + Name + " has repeated weapons : " + wpt.Name);
                    Weapons.Add(wpt, new(wgt,minlev,maxlev));
                }
            }
            else {
                WeaponType? wtyp = StaticData.GetWeaponTypeByName("Bite");
                if (wtyp != null) Weapons.Add(wtyp, (1,1,100));
            }
            // Check that we have valid weapons at all levels. Yeah this algorithm is inefficient. :)
            for (int n = LevelMin; n <= LevelMax; n++) {
                bool bOK = false;
                foreach ((WeaponType _, (int _, int mn, int mx)) in Weapons) {
                    if (mn <= n && mx >= n) {
                        bOK = true; break;
                    }
                }
                if (!bOK) throw new Exception($"Creature {Name} does not have a valid weapon at level {n}");
            }

            // Special effect on death?
            XmlNode? xie = xml.SelectSingleNode("OnDeathEffect");
            if (xie is not null) {
                OnDeathEffect = new ItemEffect(xie);
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

        public Weapon? GenerateRandomWeapon(int lev) {
            var validWeapons = Weapons.Where(x => x.Value.MinLev <= lev && x.Value.MaxLev >= lev).ToDictionary();
            int total = validWeapons.Sum(x => x.Value.Weight);
            Random rnd = new Random();
            int r = rnd.Next(total);
            foreach (WeaponType tp in validWeapons.Keys) {
                if (r < Weapons[tp].Weight) return new Weapon(tp, 0);
                r -= Weapons[tp].Weight;
            }
            return null;
        }

        public override string ToString() {
            return Name;
        }
    }
}