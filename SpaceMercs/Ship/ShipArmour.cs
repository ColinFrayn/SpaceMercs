using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace SpaceMercs {
  class ShipArmour : ShipEquipment {
    public int BaseArmour { get; private set; } // %age damage reduction
    public double HealRate { get; private set; }

    public ShipArmour(XmlNode xml) : base(xml, ShipEquipment.RoomSize.Armour) {
      BaseArmour = Int32.Parse(xml.SelectSingleNode("BaseArmour").InnerText);
      if (xml.SelectSingleNode("Heal") != null) HealRate = double.Parse(xml.SelectSingleNode("Heal").InnerText);
    }
  }

}
