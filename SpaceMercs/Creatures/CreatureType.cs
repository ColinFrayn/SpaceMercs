using System;
using System.Xml;
using System.Drawing;
using System.Collections.Generic;

namespace SpaceMercs {
  class CreatureType {
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
    public Color Col1 { get; private set; }
    public Color Col2 { get; private set; }
    public Color Col3 { get; private set; }
    public readonly Dictionary<WeaponType, int> Weapons = new Dictionary<WeaponType, int>();
    public readonly Dictionary<WeaponType.DamageType, double> Resistances = new Dictionary<WeaponType.DamageType, double>();
    public readonly Dictionary<MaterialType, double> Scavenge = new Dictionary<MaterialType, double>();
    private readonly int WeaponTotalWeight = 0;

    public CreatureType(XmlNode xml, CreatureGroup gp) {
      Name = xml.Attributes["Name"].InnerText;
      if (xml.SelectSingleNode("Size") != null) Size = Int32.Parse(xml.SelectSingleNode("Size").InnerText);
      else Size = 1;
      if (xml.SelectSingleNode("Scale") != null) Scale = Double.Parse(xml.SelectSingleNode("Scale").InnerText);
      else Scale = (double)Size;

      string strLevel = xml.SelectSingleNode("LevelRange").InnerText;
      string[] bits = strLevel.Split(',');
      if (bits.Length != 2) throw new Exception(Name + " : Could not parse level range string : " + strLevel);
      LevelMin = Int32.Parse(bits[0]);
      LevelMax = Int32.Parse(bits[1]);

      HealthBase = Int32.Parse(xml.SelectSingleNode("Health").InnerText);
      StaminaBase = Int32.Parse(xml.SelectSingleNode("Stamina").InnerText);
      ArmourBase = Int32.Parse(xml.SelectSingleNode("Armour").InnerText);

      if (xml.SelectSingleNode("Shields") != null) {
        ShieldsBase = Int32.Parse(xml.SelectSingleNode("Shields").InnerText);
      }
      else ShieldsBase = 0;

      if (xml.SelectSingleNode("MovementCost") != null) {
        MovementCost = Double.Parse(xml.SelectSingleNode("MovementCost").InnerText);
      }
      else MovementCost = Const.MovementCost; // Default = 2.0

      AttackBase = Int32.Parse(xml.SelectSingleNode("Attack").InnerText);
      DefenceBase = Int32.Parse(xml.SelectSingleNode("Defence").InnerText);

      if (xml.SelectSingleNode("BodyType") != null) {
        Body = (BodyType)Enum.Parse(typeof(BodyType), xml.SelectSingleNode("BodyType").InnerText);
      }
      else Body = BodyType.Humanoid;

      // Base colours
      Col1 = Color.Black;
      Col2 = Color.Gray;
      Col3 = Color.SaddleBrown;
      if (xml.SelectSingleNode("Col1") != null) {
        bits = xml.SelectSingleNode("Col1").InnerText.Split(',');
        if (bits.Length != 3) throw new Exception("Could not parse Colour1 string : " + xml.SelectSingleNode("Col1").InnerText);
        Col1 = Color.FromArgb(255, Int32.Parse(bits[0]), Int32.Parse(bits[1]), Int32.Parse(bits[2]));
      }
      if (xml.SelectSingleNode("Col2") != null) {
        bits = xml.SelectSingleNode("Col2").InnerText.Split(',');
        if (bits.Length != 3) throw new Exception("Could not parse Colour2 string : " + xml.SelectSingleNode("Col2").InnerText);
        Col2 = Color.FromArgb(255, Int32.Parse(bits[0]), Int32.Parse(bits[1]), Int32.Parse(bits[2]));
      }
      if (xml.SelectSingleNode("Col3") != null) {
        bits = xml.SelectSingleNode("Col3").InnerText.Split(',');
        if (bits.Length != 3) throw new Exception("Could not parse Colour3 string : " + xml.SelectSingleNode("Col3").InnerText);
        Col3 = Color.FromArgb(255, Int32.Parse(bits[0]), Int32.Parse(bits[1]), Int32.Parse(bits[2]));
      }

      // Special resistances
      if (xml.SelectSingleNode("Resistances") != null) {
        foreach (XmlNode xn in xml.SelectNodes("Resistances/Resistance")) {
          WeaponType.DamageType tp = (WeaponType.DamageType)Enum.Parse(typeof(WeaponType.DamageType), xn.Attributes["Type"].Value);
          if (Resistances.ContainsKey(tp)) throw new Exception("Duplicate resistance of type " + tp + " for creature " + Name);
          double res = Double.Parse(xn.Attributes["Value"].Value);
          Resistances.Add(tp, res);
        }
      }

      // Scavenging results
      if (xml.SelectSingleNode("Scavenge") != null) {
        foreach (XmlNode xn in xml.SelectNodes("Scavenge/Item")) {
          double amount = Double.Parse(xn.Attributes["Amount"].Value);
          MaterialType tp = StaticData.GetMaterialTypeByName(xn.InnerText);
          if (tp == null) throw new Exception("Couldn't identify scavenging material " + xn.InnerText + " in creature type " + Name);
          if (Scavenge.ContainsKey(tp)) throw new Exception("Duplicate scavenging result of type " + tp + " for creature " + Name);
          Scavenge.Add(tp, amount);
        }
      }

      // Weapons
      if (xml.SelectSingleNode("Weapons") != null) {
        foreach (XmlNode xn in xml.SelectNodes("Weapons/Weapon")) {
          WeaponType wpt = StaticData.GetWeaponTypeByName(xn.InnerText);
          if (wpt == null) throw new Exception("Creature " + Name + " has unknown weapon : " + xn.InnerText);
          int wgt = Int32.Parse(xn.Attributes["Weight"].Value);
          if (Weapons.ContainsKey(wpt)) throw new Exception("Creature " + Name + " has repeated weapons : " + wpt.Name);
          Weapons.Add(wpt, wgt);
          WeaponTotalWeight += wgt;
        }
      }
      else {
        WeaponTotalWeight = 1;
        Weapons.Add(StaticData.GetWeaponTypeByName("Bite"), 1);
      }
      
      // Misc
      Description = xml.SelectSingleNode("Desc").InnerText;
      IsBoss = (xml.SelectSingleNode("Boss") != null);
      Corporeal = (xml.SelectSingleNode("NonCorporeal") == null);
      Interact = (xml.SelectSingleNode("Interact") != null);
      Group = gp;
      TextureID = -1;
      TextureShieldsID = -1;
    }

    public void GenerateTexture(bool bShields) {
      if (bShields) {
        if (TextureShieldsID != -1) return;
        TextureShieldsID = Textures.GenerateCreatureTexture(this,true);
      }
      else {
        if (TextureID != -1) return;
        TextureID = Textures.GenerateCreatureTexture(this,false);
      }
    }

    public Weapon GenerateRandomWeapon() {
      if (WeaponTotalWeight == 0) return null;
      Random rnd = new Random();
      int r = rnd.Next(WeaponTotalWeight);
      foreach (WeaponType tp in Weapons.Keys) {
        if (r < Weapons[tp]) return new Weapon(tp,0);
        r -= Weapons[tp];
      }
      return null;
    }

    public override string ToString() {
      return Name;
    }

  }
}
