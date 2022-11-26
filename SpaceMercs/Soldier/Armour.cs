using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;

namespace SpaceMercs {
  class Armour : IEquippable {
    // IEquipment
    public string Name { get { return Material.Name + " " + Type.Name + " [" + Utils.LevelToDescription(Level) + "]"; } }
    public double Mass { get { return Type.Mass * Material.MassMod * (1.0 - (Level / 10.0)); } }
    public double Cost { get { return CalculateCost(Level); } }
    public string Desc {
      get {
        StringBuilder sb = new StringBuilder();
        sb.AppendLine(Type.Desc);
        sb.AppendLine("Quality : " + Utils.LevelToDescription(Level));
        sb.AppendLine("Mass : " + Mass.ToString("0.##") + "kg");
        sb.AppendLine("Material : " + Material.Name);
        sb.AppendLine("Armour : " + BaseArmour.ToString("0.#"));
        if (Shields != 0.0) sb.AppendLine("Shields : " + Shields.ToString("0.#"));
        foreach (KeyValuePair<WeaponType.DamageType,double> tp in GetAllResistances()) {
          sb.AppendLine("Resist (" + tp.Key + ") : " + tp.Value.ToString("0.#") + "%");
        }
        if (Type.Strength != 0.0) sb.AppendLine("Strength : " + Type.Strength.ToString("0.#"));
        if (Type.Agility != 0.0) sb.AppendLine("Agility : " + Type.Agility.ToString("0.#"));
        if (Type.Intellect != 0.0) sb.AppendLine("Intellect : " + Type.Intellect.ToString("0.#"));
        if (Type.Toughness != 0.0) sb.AppendLine("Toughness : " + Type.Toughness.ToString("0.#"));
        if (Type.Endurance != 0.0) sb.AppendLine("Endurance : " + Type.Endurance.ToString("0.#"));
        if (Type.Health != 0.0) sb.AppendLine("Health : " + Type.Health.ToString("0.#"));
        if (Type.Stamina != 0.0) sb.AppendLine("Stamina : " + Type.Stamina.ToString("0.#"));
        if (Type.Attack != 0.0) sb.AppendLine("Attack : " + Type.Attack.ToString("0.#"));
        if (Type.Defence != 0.0) sb.AppendLine("Defence : " + Type.Defence.ToString("0.#"));
        foreach (KeyValuePair<Soldier.UtilitySkill, int> kvp in Type.SkillBoosts) {
          sb.AppendLine(kvp.Key.ToString() + " : +" + kvp.Value);
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
    public double BaseArmour {  get { return Type.BaseArmour * Material.ArmourMod * (1 + (Level / 5.0)); } }
    public double Shields { get { return Type.Shields * (1 + (Level / 4)); } }
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
    public Dictionary<WeaponType.DamageType,double> GetAllResistances() {
      Dictionary<WeaponType.DamageType, double> AllRes = new Dictionary<WeaponType.DamageType, double>();
      foreach (WeaponType.DamageType tp in Type.BonusArmour.Keys) {
        if (AllRes.ContainsKey(tp)) AllRes[tp] += Type.BonusArmour[tp];
        else AllRes.Add(tp, Type.BonusArmour[tp]);
      }
      foreach (WeaponType.DamageType tp in Material.BonusArmour.Keys) {
        foreach (BodyPart bp in Type.Locations) {
          if (AllRes.ContainsKey(tp)) AllRes[tp] += Material.BonusArmour[tp] * Utils.BodyPartToArmourScale(bp);
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
      Armour ar = eq as Armour;
      if (ar == null) throw new Exception("Attempting to clone Armour with object of type " + eq.GetType());
      Type = ar.Type;
      Material = ar.Material;
      Level = iNewLevel;
    }
    public Armour(XmlNode xml) {
      Level = Int32.Parse(xml.Attributes["Level"].Value);
      string strType = xml.Attributes["Type"].Value;
      Type = StaticData.GetArmourTypeByName(strType);
      if (Type == null) throw new Exception("Could not ID armour type : " + strType);
      string strMat = xml.Attributes["Material"].Value;
      Material = StaticData.GetMaterialTypeByName(strMat);
      if (Material == null) throw new Exception("Could not ID armour material : " + strMat);
    }

    // Equality comparers so that this can be used in a Dictionary/HashSet properly
    public override int GetHashCode() {
      unchecked {
        int hash = 17;
        hash = hash * 23 + Level.GetHashCode();
        hash = hash * 37 + Type.GetHashCode();
        hash = hash * 31 + Material.GetHashCode();
        return hash;
      }
    }
    public override bool Equals(object obj) {
      if (obj == null || GetType() != obj.GetType()) return false;
      Armour ar = obj as Armour;
      return (ar.Level == Level && ar.Material == Material && ar.Type == Type);
    }
    public override string ToString() {
      return Name;
    }

    // Misc
    private double CalculateCost(int lev) {
      return Type.Cost * Material.CostMod * Utils.ItemLevelToCostMod(lev);
    }
    public void UpgradeArmour() {
      // Upgrade Level, Material or Type
      Random rnd = new Random();
      double r = rnd.NextDouble();
      if (r < 0.2 && Level < 3) Level++;
      else if (r < 0.85) { // Upgrade mats, if possible
        MaterialType matnew = Material;
        foreach (MaterialType mat2 in StaticData.Materials) {
          if (mat2.IsArmourMaterial && mat2.CostMod > Material.CostMod && (matnew == Material || mat2.CostMod < matnew.CostMod)) {
            matnew = mat2;
          }
        }
        Material = matnew;
      }
      else { // Upgrade type, if possible
        ArmourType atnew = Type;
        foreach (ArmourType at2 in StaticData.ArmourTypes) {
          if (at2.Locations.SetEquals(Type.Locations) && at2.Cost > Type.Cost && (atnew == Type || at2.Cost < atnew.Cost)) {
            atnew = at2;
          }
        }
        Type = atnew;
      }
    }
  }
}