using System.IO;
using System.Xml;

namespace SpaceMercs {
    class Material : IItem {
        public string Name { get { return BaseType.Name; } }
        public string Description { get { return BaseType.Description; } }
        public double Mass { get { return BaseType.UnitMass; } }
        public double Cost { get { return BaseType.ItemCost; } }
        public MaterialType BaseType { get; private set; }

        public void SaveToFile(StreamWriter file) {
            file.WriteLine("<Material Type=\"" + BaseType.Name + "\"/>");
        }

        public Material(MaterialType mt) {
            BaseType = mt;
        }
        public Material(XmlNode xml) {
            string strType = xml.Attributes!["Type"]?.Value ?? "<Name Unknown>";
            MaterialType mt = StaticData.GetMaterialTypeByName(strType) ?? throw new Exception($"Could not find base type for material \"{strType}\"");
            BaseType = mt;
        }

        // Equality comparers so that this can be used in a Dictionary/HashSet properly
        public override int GetHashCode() {
            return HashCode.Combine(BaseType);
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