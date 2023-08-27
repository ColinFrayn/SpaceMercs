using System.Xml;

namespace SpaceMercs {
    public class ShipEngine : ShipEquipment {
        public double Range { get; private set; } // Range in metres
        public double Speed { get; private set; } // Max speed in m/s
        public double Accel { get; private set; } // Max accel in system in m/s/s

        public ShipEngine(XmlNode xml) : base(xml, ShipEquipment.RoomSize.Engine) {
            Range = xml.SelectNodeDouble("Range") * Const.LightYear;
            Speed = xml.SelectNodeDouble("Speed") * Const.SpeedOfLight;
            Accel = xml.SelectNodeDouble("Accel");
        }

        public override IEnumerable<string> GetHoverText(Ship? sh = null) {
            List<string> strList = new List<string>(base.GetHoverText(sh));
            if (Range > 0) {
                if (Range > Const.LightYear) strList.Add($"Range: {Math.Round(Range/Const.LightYear,1)}ly");
                else if (Range > Const.AU) strList.Add($"Range: {Math.Round(Range / Const.AU, 1)}AU");
                else strList.Add($"Range: {Math.Round(Range / Const.Million, 1)}Mm");
            }
            if (Speed > 0) {
                if (Speed > Const.SpeedOfLight) strList.Add($"Speed: {Math.Round(Speed / Const.SpeedOfLight, 1)}c");
                else strList.Add($"Speed: {Math.Round(Speed / 1000.0, 1)}km/s");
            }
            if (Accel > 0) strList.Add($"Accel: {Math.Round(Accel,1)}m/s/s");
            return strList;
        }
    }
}
