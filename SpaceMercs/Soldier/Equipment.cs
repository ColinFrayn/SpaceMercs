using System;
using System.IO;
using System.Xml;

namespace SpaceMercs {
  // Soldier equipment
  class Equipment : IEquippable {
    // IEquipment
    public string Name { get { return BaseType.Name; } }
    public double Mass { get { return BaseType.Mass; } }
    public double Cost { get { return BaseType.Cost; } }
    public string Desc { get { return BaseType.Desc; } }
    public double Rarity { get { return BaseType.Rarity; } }
    public int Level { get { return 0; } } // Ignored here
    public double UpgradeCost { get { return 0.0; } } // Ignored here
    public ItemType BaseType { get; private set; }
    public bool IsMedical {  get { return false; } }

    public void SaveToFile(StreamWriter file) {
      file.WriteLine("<Equipment Type=\"" + BaseType.Name + "\"/>");
    }

    public Equipment(ItemType tp) {
      BaseType = tp;
    }
    public Equipment(XmlNode xml) {
      string strType = xml.Attributes["Type"].Value;
      ItemType tp = StaticData.GetItemTypeByName(strType);
      if (tp == null) throw new Exception("Could not ID equipment type : " + strType);
      BaseType = tp;
    }

    public override int GetHashCode() {
      unchecked {
        int hash = 17;
        hash = hash * 37 + BaseType.GetHashCode();
        return hash;
      }
    }
    public override bool Equals(object obj) {
      if (obj == null || GetType() != obj.GetType()) return false;
      Equipment wp = obj as Equipment;
      return (wp.BaseType == BaseType);
    }
    public override string ToString() {
      return Name;
    }

  }
}
