using System.Xml;

namespace SpaceMercs {
    public class ShipArmour : ShipEquipment {
        public int BaseArmour { get; private set; } // %age damage reduction

        public ShipArmour(XmlNode xml) : base(xml, ShipEquipment.RoomSize.Armour) {
            BaseArmour = xml.SelectNodeInt("BaseArmour");
        }

        public override IEnumerable<string> GetHoverText(Ship? sh = null) {
            List<string> strList = new List<string>(base.GetHoverText(sh));
            if (BaseArmour > 0) strList.Add("Armour: " + BaseArmour);
            return strList;
        }
    }
}
