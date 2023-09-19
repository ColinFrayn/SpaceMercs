using System.IO;
using System.Text;
using System.Xml;

namespace SpaceMercs {
    public class Weapon : IEquippable {
        // IEquippable
        public string Name { get { return Type.Name + " [" + Utils.LevelToDescription(Level) + "] "; } }
        public double Mass { get { return Type.Mass * (1.0 - (Level / 10.0)); } }
        public double Cost { get { return Type.Cost * Utils.ItemLevelToCostMod(Level); } }
        public string Desc {
            get {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine(Type.Desc);
                sb.AppendLine("Quality : " + Utils.LevelToDescription(Level));
                sb.AppendLine("Mass : " + Mass.ToString("0.##") + "kg");
                if (Range > 1) sb.AppendLine("Range : " + Range.ToString("0.##") + "m");
                sb.AppendLine("Damage : " + DBase.ToString("0.#") + " + R" + DMod.ToString("0.#"));
                sb.AppendLine("Stamina : " + StaminaCost.ToString("0.#"));
                sb.AppendLine($"Accuracy : {AttackBonus.ToString()} - {Type.DropOff}/m");
                if (Type.Area > 0) sb.AppendLine($"Area : {Type.Area}m rad");
                //if (AttackBonus != 0) sb.AppendLine("Attack : " + AttackBonus.ToString("+#;-#"));
                if (Type.Shots > 1) sb.AppendLine($"MultiShot : {Type.Shots}");
                foreach (KeyValuePair<Soldier.UtilitySkill, int> kvp in Type.SkillBoosts) {
                    sb.AppendLine(kvp.Key.ToString() + " : +" + kvp.Value);
                }
                if (Type.Stable) sb.AppendLine("Stable Weapon");
                return sb.ToString();
            }
        }
        public double Rarity { get { return Type.Rarity * Math.Pow(Const.EquipmentLevelRarityScale, Level); } }
        public double Range { get { if (Type.IsMeleeWeapon) return 1.0; return Type.Range * (1.0 + (Level * 0.05)); } }
        public double UpgradeCost {
            get {
                return CalculateCost(Level + 1) - CalculateCost(Level);
            }
        }
        public ItemType BaseType { get { return Type; } }
        public double DBase { get { return Type.DBase * (1.0 + (Level / 5.0)); } }
        public double DMod { get { return Type.DMod * (1.0 + (Level / 5.0)); } }
        public int Level { get; private set; }
        public void SaveToFile(StreamWriter file) {
            file.Write("<Weapon Type=\"" + BaseType.Name + "\" Level=\"" + Level.ToString() + "\"");
            file.WriteLine("/>");
        }

        // Weapon-specific properties
        public WeaponType Type { get; private set; }
        public double StaminaCost { get { return Type.Speed * (1.0 - (Level / 20.0)); } }
        public double AttackBonus { get { return Type.Accuracy + (Level * 0.5); } }
        public Dictionary<WeaponType.DamageType, double> GetBonusDamage() {
            Dictionary<WeaponType.DamageType, double> bdam = new Dictionary<WeaponType.DamageType, double>();
            return bdam;
        } // TODO Implement bonus damage
        private double CalculateCost(int lev) {
            return Type.Cost * Utils.ItemLevelToCostMod(lev);
        }

        public Weapon(WeaponType tp, int lvl) {
            Type = tp;
            Level = lvl;
        }
        public Weapon(IEquippable eq, int iNewLevel) {
            Weapon? wp = eq as Weapon;
            if (wp == null) throw new Exception("Attempting to clone Weapon with object of type " + eq.GetType());
            Type = wp.Type;
            Level = iNewLevel;
        }
        public Weapon(XmlNode xml) {
            Level = xml.GetAttributeInt("Level");
            string strType = xml.GetAttributeText("Type");
            Type = StaticData.GetWeaponTypeByName(strType) ?? throw new Exception($"Could not identify weapon type {strType}");
            if (Type == null) throw new Exception("Could not ID weapon type : " + strType);
        }
        public Weapon(Weapon wp) {
            Type = wp.Type;
            Level = wp.Level;
        }

        // Equality comparers so that this can be used in a Dictionary/HashSet properly
        public override int GetHashCode() {
            unchecked {
                int hash = 17;
                hash = hash * 23 + Level.GetHashCode();
                hash = hash * 37 + Type.GetHashCode();
                return hash;
            }
        }
        public override bool Equals(object? obj) {
            if (obj is null) return false;
            if (obj is not Weapon wp) return false;
            if (wp.Level != Level || wp.Type != Type) return false;
            return true;
        }
        public override string ToString() {
            return Name;
        }
        public void EndOfTurn() {
            // Nothing to do yet
        }

    }
}
