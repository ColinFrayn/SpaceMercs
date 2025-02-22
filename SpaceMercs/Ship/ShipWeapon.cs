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

        public double FireWeapon(Ship source, Ship? target, Random rand) {
            if (target is null) return 0d;
            int attackScore = source.Attack + Attack;
            int defenceScore = target.Defence;
            double hit = (rand.NextDouble() * attackScore) - (rand.NextDouble() * defenceScore);
            Cooldown = Rate + (rand.NextDouble() * 0.1d); // Reset cooldown, plus some randomness
            if (hit <= 0d) return 0d;
            double damage = (1d + rand.NextDouble()) * Attack / 2d;
            if (damage <= 0.0) return 0d;
            return target.DamageShip(damage);
        }

        public override IEnumerable<string> GetHoverText(Ship? sh = null) {
            List<string> strList = new List<string>(base.GetHoverText(sh));
            strList.Add($"Range: {Range}m");
            strList.Add($"Delay: {Rate}s");
            return strList;
        }
    }
}