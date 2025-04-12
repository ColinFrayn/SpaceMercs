using SpaceMercs.Graphics;
using System.Xml;

namespace SpaceMercs {
    public class CreatureType {
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
        public bool Corporeal { get; private set; }
        public bool Interact { get; private set; }  // Can interact with objects? (i.e. open doors) Default = false
        public double MovementCost { get; private set; }
        public int FrequencyModifier { get; private set; }
        public readonly Dictionary<WeaponType, (int Weight, int MinLev, int MaxLev)> Weapons = new Dictionary<WeaponType, (int Weight, int MinLev, int MaxLev)>();
        public readonly Dictionary<WeaponType.DamageType, double> Resistances = new Dictionary<WeaponType.DamageType, double>();
        public readonly Dictionary<MaterialType, double> Scavenge = new Dictionary<MaterialType, double>();
        public ItemEffect? OnDeathEffect { get; private set; }
        // Texture stuff
        public int TexX { get; private set; }
        public int TexY { get; private set; }
        public int TexSz { get; private set; }

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

            FrequencyModifier = xml.SelectNodeInt("Frequency", 1);

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

            // Texture details
            XmlNode? xtex = xml.SelectSingleNode("Tex");
            TexSz = TexX = TexY = -1;
            if (xtex is not null) {
                string strTex = xtex.InnerText;
                TexSz = xtex.GetAttributeInt("Size", 1);
                if (!string.IsNullOrEmpty(strTex)) {
                    string[] texBits = strTex.Split(',');
                    if (texBits.Length != 2) throw new Exception($"Illegal Tex string : {strTex}");
                    TexX = int.Parse(texBits[0]) - 1;
                    TexY = int.Parse(texBits[1]) - 1;
                }
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

        public TexSpecs GetTexture(bool bShields) {
            TexSpecs? ts = Group.GetTexDetails(TexX, TexY, TexSz);
            if (ts is not null) return ts.Value;
            int itexid = -1;
            if (bShields) {
                if (TextureShieldsID == -1) GenerateTexture(true);
                itexid = TextureShieldsID;
            }
            else {
                if (TextureID == -1) GenerateTexture(false);
                itexid = TextureID;
            }
            return new TexSpecs(itexid, 0f, 0f, 1f, 1f);
        }
        public void GenerateTexture(bool bShields) {
            if (bShields) {
                if (TextureShieldsID != -1) return;
                TextureShieldsID = Textures.GenerateDefaultCreatureTexture(this, true);
            }
            else {
                if (TextureID != -1) return;
                TextureID = Textures.GenerateDefaultCreatureTexture(this, false);
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