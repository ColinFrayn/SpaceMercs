using System.Xml;

namespace SpaceMercs {
    class ShipArmour : ShipEquipment {
        public int BaseArmour { get; private set; } // %age damage reduction
        public double HealRate { get; private set; }

        public ShipArmour(XmlNode xml) : base(xml, ShipEquipment.RoomSize.Armour) {
            BaseArmour = xml.SelectNodeInt("BaseArmour");
            HealRate = xml.SelectNodeDouble("Heal", 0.0);
        }
    }
}
