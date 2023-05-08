using System;
using System.IO;
using System.Xml;

namespace SpaceMercs {
  class Material : IItem {
    public string Name { get { return BaseType.Name; } }
    public string Desc { get { return BaseType.Desc; } }
    public double Mass { get { return BaseType.UnitMass; } }
    public double Cost { get { return BaseType.CostMod; } }
    public MaterialType BaseType { get; private set; }

    public void SaveToFile(StreamWriter file) {
      file.WriteLine("<Material Type=\"" + BaseType.Name + "\"/>");
    }

    public Material(MaterialType mt) {
      BaseType = mt;
    }
    public Material(XmlNode xml) {
      string strType = xml.Attributes["Type"].Value;
      MaterialType mt = StaticData.GetMaterialTypeByName(strType);
      if (mt == null) throw new Exception("Could not find base type for material \"" + strType + "\"");
      BaseType = mt;
    }

    // Equality comparers so that this can be used in a Dictionary/HashSet properly
    public override int GetHashCode() {
      unchecked {
        int hash = 17;
        hash = hash * 37 + BaseType.GetHashCode();
        return hash;
      }
    }
    public override bool Equals(object? obj) {
      if (obj == null || GetType() != obj.GetType()) return false;
      return (((Material)obj).BaseType == BaseType);
    }
    public override string ToString() {
      return Name;
    }
  }
}
