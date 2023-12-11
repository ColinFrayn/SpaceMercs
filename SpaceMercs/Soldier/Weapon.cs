using System.IO;
using System.Text;
using System.Xml;

namespace SpaceMercs {
    public class Weapon : IEquippable {
        // IEquippable
        public string Name { get { return Type.Name + " [" + Utils.LevelToDescription(Level) + "]" + ModChar(); } }
        public double Mass { get { return Type.Mass * (1.0 - (Level / 12.0)); } }
        public double Cost { get { return CalculateCost(); } }
        public string Desc {
            get {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine(Type.Desc);
                sb.AppendLine("Class : " + Type.WClass.ToString());
                sb.AppendLine("Quality : " + Utils.LevelToDescription(Level));
                sb.AppendLine("Mass : " + Mass.ToString("0.##") + "kg");
                if (Range > 1) sb.AppendLine("Range : " + Range.ToString("0.##") + "m");
                sb.AppendLine("Damage : " + DBase.ToString("0.#") + " + R" + DMod.ToString("0.#"));
                sb.AppendLine("Stamina : " + StaminaCost.ToString("0.#"));
                sb.AppendLine($"Accuracy : {AccuracyBonus.ToString()} - {Type.DropOff}/m");
                if (Type.Area > 0) sb.AppendLine($"Area : {Type.Area}m rad");
                if (Type.Shots > 1) sb.AppendLine($"MultiShot : {Type.Shots}");
                foreach (KeyValuePair<Soldier.UtilitySkill, int> kvp in Type.SkillBoosts) {
                    sb.AppendLine(kvp.Key.ToString() + " : +" + kvp.Value);
                }
                if (Type.Stable) sb.AppendLine("Stable Weapon");
                if (Mod is not null) sb.AppendLine($"Mod: {Mod.Name}");
                return sb.ToString();
            }
        }
        public double Rarity { get { return Type.Rarity * Math.Pow(Const.EquipmentLevelRarityScale, Level); } }
        public double Range { get { if (Type.IsMeleeWeapon) return 1.0; return Type.Range * (1.0 + (Level * 0.05)) + (Mod is not null ? Mod.Range : 0); } }
        public double UpgradeCost {
            get {
                // Do not include the Mod cost in here
                return Type.Cost * (Utils.ItemLevelToCostMod(Level+1) - Utils.ItemLevelToCostMod(Level));
            }
        }
        public ItemType BaseType { get { return Type; } }
        public int Level { get; private set; }
        public void SaveToFile(StreamWriter file) {
            file.Write($"<Weapon Type=\"{BaseType.Name}\" Level=\"{Level}\"");
            if (Mod != null) file.Write($" Mod=\"{Mod.Name}\"");
            file.WriteLine(" />");
        }

        // Weapon-specific properties
        public WeaponType Type { get; private set; }
        public WeaponMod? Mod { get; private set; }
        public double DBase { get { return Type.DBase * (1.0 + (Level / 10.0)) + (Mod is not null ? Mod.Damage : 0); } }
        public double DMod { get { return Type.DMod * (1.0 + (Level / 10.0)); } }
        public double StaminaCost { get { return Type.Speed * (1.0 - (Level / 20.0)); } }
        public double AccuracyBonus { get { return Type.Accuracy + (Level * 0.5) + (Mod is not null ? Mod.Accuracy : 0); } }
        public double DropOff { get { return Type.DropOff * Math.Pow(0.95, Level) * (Mod is not null ? Mod.DropoffMod : 1.0); } }
        public double NoiseLevel { get { return Math.Max(0, Type.NoiseLevel - (Mod is not null ? Mod.Silencer : 0.0)); } }
        public Dictionary<WeaponType.DamageType, double> GetBonusDamage() {
            Dictionary<WeaponType.DamageType, double> bdam = new Dictionary<WeaponType.DamageType, double>();
            // TODO Implement bonus damage
            return bdam;
        }
        public double UnmodifiedCost {
            get {
                return Type.Cost * Utils.ItemLevelToCostMod(Level);
            }
        }
        private double CalculateCost() {
            return UnmodifiedCost;
            // TODO Add Mod
        }
        private string ModChar() {
            return Mod is null ? "" : $" {{{Mod.Char}}}";
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
            string strMod = xml.GetAttributeText("Mod");
            if (!string.IsNullOrEmpty(strMod)) {
                Mod = StaticData.GetWeaponModByName(strMod);
                if (Mod is null) throw new Exception($"Could not find weapon mod {strMod}");
            }
        }
        public Weapon(Weapon wp) {
            Type = wp.Type;
            Level = wp.Level;
        }

        public void SetModifier(WeaponMod mod) {
            Mod = mod;
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
            if (wp.Mod != Mod) return false;
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
