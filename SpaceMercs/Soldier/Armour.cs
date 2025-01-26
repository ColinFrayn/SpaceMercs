using System.IO;
using System.Text;
using System.Xml;

namespace SpaceMercs {
    public class Armour : IEquippable {
        // IEquipment
        public string Name { get { return Material.Name + " " + Type.Name + " [" + Utils.LevelToDescription(Level) + "]"; } }
        public double Mass { get { return MassAtLevel(Level); } }
        public double Cost { get { return CalculateCost(Level); } }
        public string Description {
            get {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine(Type.Description);
                sb.AppendLine("Quality : " + Utils.LevelToDescription(Level));
                sb.AppendLine("Mass : " + Mass.ToString("0.##") + "kg");
                sb.AppendLine("Material : " + Material.Name);
                sb.AppendLine("Armour : " + BaseArmour.ToString("0.#"));
                if (Shields != 0.0) sb.AppendLine("Shields : " + Shields.ToString("0.#"));
                foreach (KeyValuePair<WeaponType.DamageType, double> tp in GetAllResistances()) {
                    sb.AppendLine("Resist (" + tp.Key + ") : " + tp.Value.ToString("0.#") + "%");
                }
                if (Type.Strength != 0) sb.AppendLine("Strength : " + Type.Strength.ToString("0.#"));
                if (Type.Agility != 0) sb.AppendLine("Agility : " + Type.Agility.ToString("0.#"));
                if (Type.Insight != 0) sb.AppendLine("Insight : " + Type.Insight.ToString("0.#"));
                if (Type.Toughness != 0) sb.AppendLine("Toughness : " + Type.Toughness.ToString("0.#"));
                if (Type.Endurance != 0) sb.AppendLine("Endurance : " + Type.Endurance.ToString("0.#"));
                if (Type.Health != 0) sb.AppendLine("Health : " + Type.Health.ToString("0.#"));
                if (Type.Stamina != 0) sb.AppendLine("Stamina : " + Type.Stamina.ToString("0.#"));
                if (Type.Attack != 0) sb.AppendLine("Attack : " + Type.Attack.ToString("0.#"));
                if (Type.Defence != 0) sb.AppendLine("Defence : " + Type.Defence.ToString("0.#"));
                if (Type.Speed != 1.0) {
                    if (Type.Speed > 1.0) sb.AppendLine($"Speed : +{((Type.Speed - 1.0) * 100).ToString("0")}%");
                    else sb.AppendLine($"Speed : -{((1.0 - Type.Speed) * 100).ToString("0")}%");
                }
                ICollection<Soldier.UtilitySkill> allSkillBoosts = Type.SkillBoosts.Keys.Union(Material.SkillBoosts.Keys).ToList();
                foreach (Soldier.UtilitySkill sk in allSkillBoosts) {
                    int val = GetUtilitySkill(sk);
                    sb.AppendLine(sk.ToString() + " : +" + val);
                }
                return sb.ToString();
            }
        }
        public double Rarity { get { return Type.Rarity * Material.Rarity * Math.Pow(Const.EquipmentLevelRarityScale, Level); } }
        public int Level { get; private set; }
        public double UpgradeCost {
            get {
                double NewCost = CalculateCost(Level + 1);
                return (NewCost - Cost) * Const.UpgradeCostModifier;
            }
        }
        public ItemType BaseType { get { return Type; } }
        public void SaveToFile(StreamWriter file) {
            file.WriteLine("<Armour Type=\"" + BaseType.Name + "\" Material=\"" + Material.Name + "\" Level=\"" + Level.ToString() + "\"/>");
        }

        // Armour-specific properties
        private MaterialType Material;
        public ArmourType Type { get; private set; }
        public double BaseArmour { get { return ArmourAtLevel(Level); } }
        public double Shields { get { return ShieldsAtLevel(Level); } }

        public double GetDamageReductionByDamageType(WeaponType.DamageType type) {
            double red = 0.0;
            if (Type.BonusArmour.ContainsKey(type)) {
                red += Type.BonusArmour[type];
            }
            if (Material.BonusArmour.ContainsKey(type)) {
                foreach (BodyPart bp in Type.Locations) {
                    red += Material.BonusArmour[type] * Utils.BodyPartToArmourScale(bp);
                }
            }
            return red;
        }
        public Dictionary<WeaponType.DamageType, double> GetAllResistances() {
            Dictionary<WeaponType.DamageType, double> AllRes = new Dictionary<WeaponType.DamageType, double>();
            foreach (WeaponType.DamageType tp in Type.BonusArmour.Keys) {
                if (AllRes.ContainsKey(tp)) AllRes[tp] = AllRes[tp] + Type.BonusArmour[tp];
                else AllRes.Add(tp, Type.BonusArmour[tp]);
            }
            foreach (WeaponType.DamageType tp in Material.BonusArmour.Keys) {
                foreach (BodyPart bp in Type.Locations) {
                    if (AllRes.ContainsKey(tp)) AllRes[tp] = AllRes[tp] + Material.BonusArmour[tp] * Utils.BodyPartToArmourScale(bp);
                    else AllRes.Add(tp, Material.BonusArmour[tp] * Utils.BodyPartToArmourScale(bp));
                }
            }
            return AllRes;
        }

        public Armour(ArmourType tp, MaterialType mat, int lvl) {
            Type = tp;
            Material = mat;
            Level = lvl;
        }
        public Armour(IEquippable eq, int iNewLevel) {
            Armour ar = eq as Armour ?? throw new Exception($"Attempting to clone Armour with object of type {eq.GetType()}");
            Type = ar.Type;
            Material = ar.Material;
            Level = iNewLevel;
        }
        public Armour(XmlNode xml) {
            Level = xml.GetAttributeInt("Level");
            string strType = xml.GetAttributeText("Type");
            Type = StaticData.GetArmourTypeByName(strType) ?? throw new Exception("Could not ID armour type : " + strType);
            string strMat = xml.GetAttributeText("Material");
            Material = StaticData.GetMaterialTypeByName(strMat) ?? throw new Exception("Could not ID armour material : " + strMat);
        }

        // Equality comparers so that this can be used in a Dictionary/HashSet properly
        public override int GetHashCode() => HashCode.Combine(Level, Type, Material);

        public override bool Equals(object? obj) {
            if (obj is null || GetType() != obj.GetType() || obj is not Armour ar) return false;
            return (ar.Level == Level && ar.Material == Material && ar.Type == Type);
        }
        public override string ToString() {
            return Name;
        }

        // Misc
        private double ArmourAtLevel(int lev) {
            return (double)Type.BaseArmour * Material.ArmourMod * (1d + ((double)lev / 5d));
        }
        private double ShieldsAtLevel(int lev) {
            return (double)Type.Shields * (1d + ((double)lev / 4d));
        }
        private double MassAtLevel(int lev) {
            return Type.Mass * Material.MassMod * (1.0 - (lev / 10.0));
        }
        private double CalculateCost(int lev) {
            double cost = Type.Cost;
            double armourFact = ArmourAtLevel(lev) / (double)Type.BaseArmour;
            double shieldFact = Type.Shields == 0 ? 1 : ShieldsAtLevel(lev) / (double)Type.Shields;
            double massFact = Type.Mass / MassAtLevel(lev);
            cost *= Math.Pow(armourFact, Const.ArmourCostExponent) * Math.Pow(shieldFact, Const.ShieldCostExponent) * Math.Pow(massFact, Const.MassCostExponent);

            // Modifier for extra damage resistance from the material
            double bonusProt = 0.0;
            foreach (WeaponType.DamageType tp in Material.BonusArmour.Keys) {
                double bonusPerPoint = tp == WeaponType.DamageType.Physical ? Const.BonusPhysicalArmourValue : Const.BonusOtherArmourValue;
                foreach (BodyPart bp in Type.Locations) {
                    bonusProt += Material.BonusArmour[tp] * bonusPerPoint;
                }
            }
            cost *= 1.0 + bonusProt;

            return cost;

        }
        public void UpgradeArmour(Race? rc, int entityLevel) {
            // Upgrade Level, Material or Type
            Random rnd = new Random();
            double r = rnd.NextDouble();
            if (r < 0.2) {
                if (Level < 3 && entityLevel > 2 && Level * 2 < entityLevel) Level++;
                return;
            }
            else if (r < 0.85) { // Upgrade mats, if possible
                MaterialType? matNew = null;
                // Pick the next best material
                foreach (MaterialType mat2 in StaticData.Materials.Where(m => m.CanBuild(rc) && m.ArmourMod > 0)) {
                    if (mat2.IsScavenged || !mat2.IsArmourMaterial) continue;
                    // Is this material better (more valuable) and the cheapest such upgrade possible?
                    if (mat2.ItemCost > Material.ItemCost && (matNew is null || mat2.ItemCost <= matNew.ItemCost)) {
                        matNew = mat2;
                    }
                }
                Material = matNew ?? Material;
                return;
            }
            else { // Upgrade type, if possible
                ArmourType atnew = Type;
                foreach (ArmourType at2 in StaticData.ArmourTypes) {
                    if (!at2.CanBuild(rc)) continue;
                    if (at2.Locations.SetEquals(Type.Locations) && at2.Cost > Type.Cost && (atnew == Type || at2.Cost < atnew.Cost)) {
                        atnew = at2;
                    }
                }
                Type = atnew;
                return;
            }
        }
        public int ModifiedAgility {
            get {
                double agi = (double)Type.Agility;
                agi *= Material.MassMod * (1.0 - (Level / 20.0));
                return (int)Math.Floor(agi);
            }
        }
        public void EndOfTurn() {
            // Nothing to do yet
        }
        public int GetUtilitySkill(Soldier.UtilitySkill sk) {
            int val = Type.GetUtilitySkill(sk);
            // Material properties - 
            int matval = Material.GetUtilitySkill(sk);
            double scale = 0d;
            foreach (BodyPart bp in Type.Locations) {
                scale += Utils.BodyPartToArmourScale(bp);
            }
            val += (int)(scale * matval / 3.0); // Material contribution, scaled down.
            return val;
        }
    }
}