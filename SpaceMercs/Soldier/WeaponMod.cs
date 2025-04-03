using System.Xml;

namespace SpaceMercs {
    public class WeaponMod {
        public string Name { get; private set; }
        public string Desc { get; private set; }
        public double Mass { get; private set; }
        public double CostMod { get; private set; }
        public bool IsMelee { get; private set; }
        public double DropoffMod { get; private set; }
        public int Range { get; private set; }
        public int Silencer { get; private set; }
        public double Accuracy { get; private set; }
        public double Damage { get; private set; }
        public double Shred { get; private set; }
        public double RecoilMod { get; private set; }
        public string Char { get; private set; }
        public Dictionary<MaterialType, int> Materials { get; private set; }

        public WeaponMod(XmlNode xml) {
            Name = xml.GetAttributeText("Name");
            Char = xml.GetAttributeText("Char");
            Mass = xml.SelectNodeDouble("Mass");
            CostMod = xml.SelectNodeDouble("CostMod");
            Desc = xml.SelectNodeText("Desc").Trim();

            IsMelee = xml.SelectSingleNode("Melee") != null;

            DropoffMod = xml.SelectNodeDouble("DropoffMod", 1d);
            Range = xml.SelectNodeInt("Range", 0);
            Silencer = xml.SelectNodeInt("Silencer", 0);
            Accuracy = xml.SelectNodeDouble("Accuracy", 0d);
            Damage = xml.SelectNodeDouble("Damage", 0d);
            RecoilMod = xml.SelectNodeDouble("RecoilMod", 1d);
            Shred = xml.SelectNodeDouble("Shred", 0d);
        }

        public bool CanBeFittedTo(Weapon wp) {
            if (IsMelee != wp.Type.IsMeleeWeapon) return false;
            if (Name.Equals(wp.Mod?.Name)) return false;
            if (wp.Type.WeaponShotType == WeaponType.ShotType.Cone) {
                if (DropoffMod != 1.0) return false;
                if (Accuracy != 0.0) return false;
            }
            if (wp.Type.Shots == 1) {
                if (RecoilMod != 1.0) return false;
            }
            return true;
        }
    }
}
