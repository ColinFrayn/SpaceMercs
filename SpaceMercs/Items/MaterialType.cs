using System;
using System.Collections.Generic;
using System.Xml;

namespace SpaceMercs {
  class MaterialType {
    public string Name { get; private set; }
    public double MassMod { get; private set; } // Modifier (default = 1.0)
    public double CostMod { get; private set; } // Modifier (default = 1.0)
    public double ArmourMod { get; private set; } // Modifier (default = 0.0)
    public double Rarity { get; private set; } // Modifier (default = 1.0)
    public string Desc { get; private set; }
    public double UnitMass { get; private set; } // Mass of one unit, in kg. This is the amount that is sold
    public bool IsArmourMaterial {  get { return (ArmourMod > 0.0); } }
    public readonly Dictionary<WeaponType.DamageType, double> BonusArmour = new Dictionary<WeaponType.DamageType, double>();
    public double ConstructionChanceModifier {  get { return Math.Log(Rarity > 0 ? Rarity : 0.0001) * 10.0;  } }

    public MaterialType(XmlNode xml) {
      Name = xml.Attributes["Name"].InnerText;
      if (xml.SelectSingleNode("CostMod") != null) CostMod = double.Parse(xml.SelectSingleNode("CostMod").InnerText);
      else CostMod = 1.0;
      if (xml.SelectSingleNode("MassMod") != null) MassMod = double.Parse(xml.SelectSingleNode("MassMod").InnerText);
      else MassMod = 1.0;
      if (xml.SelectSingleNode("Rarity") != null) Rarity = double.Parse(xml.SelectSingleNode("Rarity").InnerText);
      else Rarity = 1.0;
      if (xml.SelectSingleNode("ArmourMod") != null) ArmourMod = double.Parse(xml.SelectSingleNode("ArmourMod").InnerText);
      else ArmourMod = 0.0;
      if (xml.SelectSingleNode("UnitMass") != null) UnitMass = double.Parse(xml.SelectSingleNode("UnitMass").InnerText);
      else UnitMass = 1.0;
      if (xml.SelectSingleNode("Desc") != null) Desc = xml.SelectSingleNode("Desc").InnerText;
      else Desc = "";
      Desc.Trim();

      // Special resistances if this is made into armour
      if (xml.SelectSingleNode("BonusArmour") != null) {
        foreach (XmlNode xn in xml.SelectNodes("BonusArmour/Bonus")) {
          WeaponType.DamageType type = (WeaponType.DamageType)Enum.Parse(typeof(WeaponType.DamageType), xn.Attributes["Type"].Value);
          double val = Double.Parse(xn.Attributes["Amount"].Value);
          BonusArmour.Add(type, val);
        }
      }
    }
  }
}
