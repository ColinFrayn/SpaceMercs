using System.IO;
using System.Security.Cryptography;
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
                if (Type.Strength != 0) sb.AppendLine($"Strength : {FormatDoubleWithOptionalSign(Type.Strength)}");
                if (Type.Agility != 0) sb.AppendLine($"Agility : {FormatDoubleWithOptionalSign(Type.Agility)}");
                if (Type.Insight != 0) sb.AppendLine($"Insight : {FormatDoubleWithOptionalSign(Type.Insight)}");
                if (Type.Toughness != 0) sb.AppendLine($"Toughness : {FormatDoubleWithOptionalSign(Type.Toughness)}");
                if (Type.Endurance != 0) sb.AppendLine($"Endurance : {FormatDoubleWithOptionalSign(Type.Endurance)}");
                if (Type.Health != 0) sb.AppendLine($"Health : {FormatDoubleWithOptionalSign(Type.Health)}");
                if (Type.Stamina != 0) sb.AppendLine($"Stamina : {FormatDoubleWithOptionalSign(Type.Stamina)}");
                if (Type.Attack != 0) sb.AppendLine($"Attack : {FormatDoubleWithOptionalSign(Type.Attack)}");
                if (Type.Defence != 0) sb.AppendLine($"Defence : {FormatDoubleWithOptionalSign(Type.Defence)}");
                if (Type.Speed != 1.0) {
                    if (Type.Speed > 1.0) sb.AppendLine($"Speed : +{((Type.Speed - 1.0) * 100).ToString("0")}%");
                    else sb.AppendLine($"Speed : -{((1.0 - Type.Speed) * 100).ToString("0")}%");
                }
                ICollection<Soldier.UtilitySkill> allSkillBoosts = Type.SkillBoosts.Keys.Union(Material.SkillBoosts.Keys).ToList();
                foreach (Soldier.UtilitySkill sk in allSkillBoosts) {
                    int val = GetUtilitySkill(sk);
                    if (val > 0) sb.AppendLine(sk.ToString() + " : +" + val);
                    else if (val < 0) sb.AppendLine(sk.ToString() + " : " + val);
                }
                return sb.ToString();
            }
        }
        private string FormatDoubleWithOptionalSign(double d) {
            if (d < 0) return d.ToString("0.#");
            return $"+{d:0.#}";
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
        public MaterialType Material { get; private set; }
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
            if (strType == "Helmet") strType = "Military Helmet";
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
            double size = Type.Size;
            double shieldValue = Math.Pow(ShieldsAtLevel(lev), Const.ShieldValueExponent) * Const.ShieldCostMultiplier;

            double baseMass = 1.5d * size;
            double massFactor = Math.Pow(baseMass / MassAtLevel(lev), Const.MassCostExponent);

            double baseArmour = 3d * size;
            double armour = ArmourAtLevel(lev);
            // Modifier for extra damage resistance from the material
            foreach (WeaponType.DamageType tp in Material.BonusArmour.Keys) {
                double bonusPerPoint = tp == WeaponType.DamageType.Physical ? 1d : 0.2d;
                armour += Material.BonusArmour[tp] * bonusPerPoint * size;
            }
            double armourFactor = Math.Pow(armour / baseArmour, Const.ArmourCostExponent);

            double abilityValue = 0d;
            double abilityBoost = Type.Strength + Type.Agility + Type.Insight + Type.Toughness + Type.Endurance;
            abilityValue += abilityBoost >= 0
                ? Math.Pow(abilityBoost, Const.AbilityBonusExponent) * Const.AbilityBonusValue
                : -Math.Pow(-abilityBoost, Const.AbilityBonusExponent) * Const.AbilityBonusValue;

            double propertyBoost = Type.Health + Type.Stamina + Type.Attack + Type.Defence;
            abilityValue += propertyBoost >= 0
                ? Math.Pow(propertyBoost, Const.PropertyBonusExponent) * Const.PropertyBonusValue
                : -Math.Pow(-propertyBoost, Const.PropertyBonusExponent) * Const.PropertyBonusValue;

            abilityValue += (Math.Pow(Type.Speed, Const.ArmourSpeedBonusExponent) - 1d) * Const.ArmourSpeedBonusValue;

            foreach ((Soldier.UtilitySkill sk, int val) in Type.SkillBoosts) {                
                if (sk == Soldier.UtilitySkill.Stealth) abilityValue += val * Const.AbilityBonusValue * Math.Pow(1.1, val) * 5d;
                else abilityValue += val * Const.AbilityBonusValue * Math.Pow(1.1, val) * 3d;
            }

            // Modifier for the complexity of this armour (and therefore rarity)
            double rarityFactor = Math.Pow(1.04, (Type.Requirements?.MinLevel ?? 0));

            // Level factor. A high level item with the same properties as a different low level item is worth less because there is less scope to improve it.
            double levelFactor = Math.Pow(0.97, lev);

            double baseCost = size * Const.BaseArmourCost;
            double cost = baseCost + shieldValue;
            cost *= massFactor * armourFactor * rarityFactor * levelFactor;
            cost += abilityValue;
            if (cost < baseCost / 3d) cost = baseCost / 3d;

            return cost;
        }
        public bool UpgradeArmour(Race? rc, int entityLevel) {
            // Upgrade Level, Material or Type
            int r = RandomNumberGenerator.GetInt32(100);
            if (r < 18) {
                if (Level < 3 && entityLevel > 2 && Level * 2 < entityLevel) {
                    Level++;
                    return true;
                }
                else return false;
            }
            else if (r < 75) { // Upgrade mats, if possible
                MaterialType? matNew = null;
                // Pick the next best material
                foreach (MaterialType mat2 in StaticData.Materials.Where(m => m.CanBuild(rc) && m.ArmourMod > 0)) {
                    if (mat2.IsScavenged || !mat2.IsArmourMaterial || mat2 == Material) continue; // Not usable
                    if (mat2.UnitCost < Material.UnitCost) continue; // not better
                    // Is this material better (more valuable) and the cheapest such upgrade possible?
                    if (matNew is null || mat2.UnitCost <= matNew.UnitCost) {
                        matNew = mat2;
                    }
                }
                if (matNew is null) return false;
                Material = matNew;
                return true;
            }
            else { // Upgrade type, if possible
                ArmourType atnew = Type;
                foreach (ArmourType at2 in StaticData.ArmourTypes) {
                    if (!at2.CanBuild(rc)) continue;
                    if (at2.Locations.SetEquals(Type.Locations) && at2.Cost > Type.Cost && at2.Cost < atnew.Cost) {
                        atnew = at2;
                    }
                }
                if (atnew == Type) return false;
                Type = atnew;
                return true;
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