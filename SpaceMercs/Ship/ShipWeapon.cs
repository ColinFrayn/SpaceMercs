using System.Xml;

namespace SpaceMercs {
    public class ShipWeapon : ShipEquipment {
        public double Range { get; private set; } // Range in metres
        public double Rate { get; private set; } // Time in seconds between shots
        public double Cooldown { get; set; }  // In a dogfight, how long before we can fire again (sec)

        public ShipWeapon(XmlNode xml) : base(xml, ShipEquipment.RoomSize.Weapon) {
            Range = xml.SelectNodeDouble("Range");
            Rate = xml.SelectNodeDouble("Rate");
        }

        public double FireWeapon(Ship source, Ship target, Random rand) {
            int attackScore = source.Attack + Attack;
            int defenceScore = target.Defence;
            double damage = (rand.NextDouble() * attackScore) - (rand.NextDouble() * defenceScore);
            Cooldown = Rate + (rand.NextDouble() * 0.1); // Reset cooldown, plus some randomness
            if (damage < 0.0) damage = 0.0;
            return target.DamageShip(damage);
        }
    }
}