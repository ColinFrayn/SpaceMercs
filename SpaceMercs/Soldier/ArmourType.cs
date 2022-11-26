using System;
using System.Xml;
using System.Collections.Generic;

namespace SpaceMercs {
  class ArmourType : ItemType {
    public int Strength { get; private set; } // Bonus
    public int Agility { get; private set; } // Bonus
    public int Intellect { get; private set; } // Bonus
    public int Toughness { get; private set; } // Bonus
    public int Endurance { get; private set; } // Bonus
    public int Health { get; private set; } // Bonus
    public int Stamina { get; private set; } // Bonus
    public int Shields { get; private set; } // Bonus
    public int Attack { get; private set; } // Bonus
    public int Defence { get; private set; } // Bonus
    public int BaseArmour { get; private set; }
    public int Size { 
      get {
        double sz = 0.0;
        foreach (BodyPart bp in Locations) sz += Utils.BodyPartToArmourScale(bp);
        return (int)Math.Ceiling(sz);
      } 
    }
    public readonly HashSet<BodyPart> Locations = new HashSet<BodyPart>();
    public readonly Dictionary<WeaponType.DamageType, double> BonusArmour = new Dictionary<WeaponType.DamageType, double>();

    public ArmourType(XmlNode xml) : base(xml) {
      if (xml.SelectSingleNode("Strength") != null) Strength = Int32.Parse(xml.SelectSingleNode("Strength").InnerText);
      if (xml.SelectSingleNode("Agility") != null) Agility = Int32.Parse(xml.SelectSingleNode("Agility").InnerText);
      if (xml.SelectSingleNode("Intellect") != null) Intellect = Int32.Parse(xml.SelectSingleNode("Intellect").InnerText);
      if (xml.SelectSingleNode("Toughness") != null) Toughness = Int32.Parse(xml.SelectSingleNode("Toughness").InnerText);
      if (xml.SelectSingleNode("Endurance") != null) Endurance = Int32.Parse(xml.SelectSingleNode("Endurance").InnerText);
      if (xml.SelectSingleNode("Health") != null) Health = Int32.Parse(xml.SelectSingleNode("Health").InnerText);
      if (xml.SelectSingleNode("Stamina") != null) Stamina = Int32.Parse(xml.SelectSingleNode("Stamina").InnerText);
      if (xml.SelectSingleNode("Shields") != null) Shields = Int32.Parse(xml.SelectSingleNode("Shields").InnerText);
      if (xml.SelectSingleNode("Attack") != null) Attack = Int32.Parse(xml.SelectSingleNode("Attack").InnerText);
      if (xml.SelectSingleNode("Defence") != null) Defence = Int32.Parse(xml.SelectSingleNode("Defence").InnerText);
      if (xml.SelectSingleNode("BaseArmour") != null) BaseArmour = Int32.Parse(xml.SelectSingleNode("BaseArmour").InnerText);
      if (xml.SelectSingleNode("BonusArmour") != null) {
        foreach (XmlNode xn in xml.SelectNodes("BonusArmour/Bonus")) {
          WeaponType.DamageType type = (WeaponType.DamageType)Enum.Parse(typeof(WeaponType.DamageType), xn.Attributes["Type"].Value);
          double val = Double.Parse(xn.Attributes["Amount"].Value);
          BonusArmour.Add(type, val);
        }
      }
      string strLocation = xml.SelectSingleNode("Location").InnerText;
      foreach (string strLoc in strLocation.Split(',')) {
        BodyPart bp = (BodyPart)Enum.Parse(typeof(BodyPart), strLoc);
        if (Locations.Contains(bp)) throw new Exception("Duplicate body part covered by armour type " + Name);
        Locations.Add(bp);
      }
    }

    public override string ToString() {
      return Name;
    }
  }
}
