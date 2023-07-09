using System.Xml;

namespace SpaceMercs {
    class ShipWeapon : ShipEquipment {
        public double Range { get; private set; } // Range in metres
        public double Rate { get; private set; } // Time in seconds between shots
        public double Cooldown { get; set; }  // In a dogfight, how long before we can fire again (sec)

        public ShipWeapon(XmlNode xml) : base(xml, ShipEquipment.RoomSize.Weapon) {
            Range = xml.SelectNodeDouble("Range");
            Rate = xml.SelectNodeDouble("Rate");
        }
    }
}