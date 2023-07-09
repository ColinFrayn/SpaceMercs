using System.Xml;

namespace SpaceMercs {
    class ShipEquipment {
        public string Name { get; private set; }
        public double Cost { get; private set; } // Cost per unit ship size
        public int Defence { get; private set; } // Modifier (additive)
        public int Attack { get; private set; } // Modifier (additive)
        public int Shield { get; private set; } // Modifier (additive)
        public int Power { get; private set; } // Power requirement
        public int Generate { get; private set; } // Power generation capability
        public bool Scanner { get; private set; }  // Can scan surfaces of terrestrial planets for missions
        public string Description { get; private set; }
        public RoomSize Size { get; private set; }
        public Colony.BaseType Available { get; private set; }
        public int TextureX { get; private set; }
        public int TextureY { get; private set; }
        public Race? RequiredRace { get; private set; }
        public int Capacity { get; private set; } // How many soldiers can it support? 
        public bool Medlab { get; private set; }
        public bool Armoury { get; private set; }
        public bool Workshop { get; private set; }
        public bool Repair { get; private set; }
        public bool Research { get; private set; }
        public bool BuildColony { get; private set; }
        public enum RoomSize { Weapon, Small, Medium, Large, Core, Engine, Armour };

        public ShipEquipment(XmlNode xml) : this(xml, ShipEquipment.RoomSize.Small) {
            // Nothing to see
        }
        public ShipEquipment(XmlNode xml, RoomSize sz = RoomSize.Small) {
            Name = xml.Attributes["Name"].InnerText;
            Cost = xml.SelectNodeDouble("Cost");
            Description = xml.SelectNodeText("Desc");
            string strTex = xml.SelectNodeText("Tex");
            string[] TexBits = strTex.Split(',');
            TextureX = int.Parse(TexBits[0]) - 1;
            TextureY = int.Parse(TexBits[1]) - 1;

            // Optional stuff
            if (xml.SelectSingleNode("Size") != null) {
                string strSize = xml.SelectNodeText("Size");
                Size = ParseRoomSize(strSize);
            }
            else Size = sz;
            Generate = xml.SelectNodeInt("Generate", 0);
            Defence = xml.SelectNodeInt("Defence", 0);
            Power = xml.SelectNodeInt("Power", 0);
            Attack = xml.SelectNodeInt("Attack", 0);
            Shield = xml.SelectNodeInt("Shield", 0);
            Scanner = (xml.SelectSingleNode("Scanner") != null);
            Capacity = xml.SelectNodeInt("Capacity", 0);
            Medlab = (xml.SelectSingleNode("Medlab") != null);
            Armoury = (xml.SelectSingleNode("Armoury") != null);
            Workshop = (xml.SelectSingleNode("Workshop") != null);
            Repair = (xml.SelectSingleNode("Repair") != null);
            Research = (xml.SelectSingleNode("Research") != null);
            BuildColony = (xml.SelectSingleNode("BuildColony") != null);

            // If Avail tag doesn't exist then this is avaialble everywhere. Otherwise, parse it.
            if (xml.SelectSingleNode("Avail") != null) {
                string strAvail = xml.SelectNodeText("Avail");
                Available = Colony.BaseType.None;
                if (strAvail.Contains("C")) Available |= Colony.BaseType.Colony;
                if (strAvail.Contains("E")) Available |= Colony.BaseType.Metropolis;
                if (strAvail.Contains("M")) Available |= Colony.BaseType.Military;
                if (strAvail.Contains("R")) Available |= Colony.BaseType.Research;
                if (strAvail.Contains("T")) Available |= Colony.BaseType.Trading;
                if (strAvail.Contains("O")) Available |= Colony.BaseType.Outpost;
            }
            else Available = Colony.BaseType.Outpost | Colony.BaseType.Colony | Colony.BaseType.Metropolis | Colony.BaseType.Military | Colony.BaseType.Research | Colony.BaseType.Trading;

            // Load the race that this equipment is restricted to (default null), or otherwise fail
            if (xml.SelectSingleNode("Race") != null) {
                RequiredRace = StaticData.GetRaceByName(xml.SelectNodeText("Race"));
                if (RequiredRace == null) {
                    throw new Exception("Could not find restricted race \"" + xml.SelectNodeText("Race") + "\" for equipment " + Name);
                }
            }
        }

        // Conversion utility
        public static RoomSize ParseRoomSize(string strSize) {
            if (strSize == "S") return RoomSize.Small;
            if (strSize == "M") return RoomSize.Medium;
            if (strSize == "L") return RoomSize.Large;
            if (strSize == "E") return RoomSize.Engine;
            if (strSize == "C") return RoomSize.Core;
            if (strSize == "T") return RoomSize.Weapon;
            if (strSize == "A") return RoomSize.Armour;
            throw new Exception("Unknown room size : " + strSize);
        }

        public IEnumerable<string> GetHoverText(Ship? sh = null) {
            List<string> strList = new List<string> { Name };
            if (sh != null) {
                double cost = sh.CostToBuildEquipment(this);
                strList.Add("Cost: " + cost + "cr");
            }
            if (Power > 0) strList.Add("Power: " + Power);
            if (Generate > 0) strList.Add("Generate: +" + Generate);
            if (Attack > 0) strList.Add("Attack: " + Attack);
            if (Defence > 0) strList.Add("Defence: " + Defence);
            if (Capacity > 0) strList.Add("Berths: " + Capacity);
            if (Shield > 0) strList.Add("Shields: " + Shield);
            if (Armoury) strList.Add("Ability: Armoury");
            if (Medlab) strList.Add("Ability: Medlab");
            if (Repair) strList.Add("Ability: Repair");
            if (Research) strList.Add("Ability: Research");
            if (Scanner) strList.Add("Ability: Scanner");
            if (Workshop) strList.Add("Ability: Workshop");
            return strList;
        }

    }
}
