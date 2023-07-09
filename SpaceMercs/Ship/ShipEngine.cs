using System.Xml;

namespace SpaceMercs {
    class ShipEngine : ShipEquipment {
        public double Range { get; private set; } // Range in metres
        public double Speed { get; private set; } // Max speed in c
        public double Accel { get; private set; } // Max accel in system in m/s/s

        public ShipEngine(XmlNode xml) : base(xml, ShipEquipment.RoomSize.Engine) {
            Range = xml.SelectNodeDouble("Range") * Const.LightYear;
            Speed = xml.SelectNodeDouble("Speed") * Const.SpeedOfLight;
            Accel = xml.SelectNodeDouble("Accel");
        }
    }
}
