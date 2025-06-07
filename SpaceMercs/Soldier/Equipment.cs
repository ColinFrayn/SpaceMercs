using System.IO;
using System.Xml;

namespace SpaceMercs {
    // Soldier equipment
    public class Equipment : IEquippable {
        // IEquipment
        public string Name { get { return BaseType.Name; } }
        public double Mass { get { return BaseType.Mass; } }
        public double Cost { get { return BaseType.Cost; } }
        public string Description { get { return BaseType.Description; } }
        public double Rarity { get { return BaseType.Rarity; } }
        public int Level { get { return 0; } } // Ignored here
        public double UpgradeCost { get { return 0d; } } // Ignored here
        public ItemType BaseType { get; private set; }
        public static bool IsMedical { get { return false; } }
        public int Recharge { get; private set; }

        public void SaveToFile(StreamWriter file) {
            string rst = "";
            if (Recharge > 0) rst = $" Recharge=\"{Recharge}\"";
            file.WriteLine($"<Equipment Type=\"{BaseType.Name}\"{rst}/>");
        }

        public Equipment(ItemType tp) {
            BaseType = tp;
        }
        public Equipment(XmlNode xml) {
            string strType = xml.Attributes!["Type"]!.Value;
            ItemType? tp = StaticData.GetItemTypeByName(strType);
            BaseType = tp ?? throw new Exception("Could not ID equipment type : " + strType);
            Recharge = xml.GetAttributeInt("Recharge", 0);
        }

        public void SetRecharge(int r) {
            Recharge = r;
        }
        public void EndOfTurn() {
            if (Recharge > 0) Recharge--;
        }
        public double BuildDiff {
            get {
                double diff = BaseType.Requirements?.MinLevel ?? 0;
                diff += Level * 3d;
                diff += Mass / 10d; // Heavier items have more parts and are more difficult to build
                return diff;
            }
        }

        public override int GetHashCode() {
            return HashCode.Combine(BaseType);
        }

        public override bool Equals(object? obj) {
            if (obj == null || GetType() != obj.GetType()) return false;
            if (obj is Equipment eqp) {
                return eqp.BaseType == BaseType &&
                       eqp.Recharge == Recharge;
            }
            return false;
        }
        public override string ToString() {
            return Name;
        }

    }
}
