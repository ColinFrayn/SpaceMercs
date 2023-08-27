using System.IO;
using System.Xml;

namespace SpaceMercs {
    public class ShipRoomDesign {
        public int XPos, YPos;
        public bool Rotated;
        public ShipEquipment.RoomSize Size;
        public int Width {
            get {
                return RoomWidth(Size, Rotated);
            }
        }
        public int Height {
            get {
                return RoomHeight(Size, Rotated);
            }
        }

        public ShipRoomDesign(int x, int y, bool bRotated, ShipEquipment.RoomSize sz) {
            XPos = x - 100;
            YPos = y - 100;
            Rotated = bRotated;
            Size = sz;
        }
        public ShipRoomDesign(XmlNode xml) {
            XPos = xml.GetAttributeInt("X");
            YPos = xml.GetAttributeInt("Y");
            Size = (ShipEquipment.RoomSize)Enum.Parse(typeof(ShipEquipment.RoomSize), xml.GetAttributeText("Size"));
            Rotated = (xml.Attributes?["Rotated"] != null);
        }

        // Save this ShipRoom to an Xml file
        public void SaveToFile(StreamWriter file) {
            file.WriteLine("<ShipRoomDesign X=\"" + XPos + "\" Y=\"" + YPos + "\" Size=\"" + Size.ToString() + "\"" + (Rotated ? " Rotated=\"true\"" : "") + " />");
        }

        // Shift this room about a bit
        public void Shift(int dx, int dy) {
            XPos += dx;
            YPos += dy;
        }

        public static int RoomWidth(ShipEquipment.RoomSize sz, bool bRotated) {
            if (sz == ShipEquipment.RoomSize.Weapon) return 1;
            if (sz == ShipEquipment.RoomSize.Small) {
                if (bRotated) return Const.SmallRoomHeight;
                return Const.SmallRoomWidth;
            }
            if (sz == ShipEquipment.RoomSize.Medium) {
                if (bRotated) return Const.MediumRoomHeight;
                return Const.MediumRoomWidth;
            }
            if (sz == ShipEquipment.RoomSize.Large) {
                if (bRotated) return Const.LargeRoomHeight;
                return Const.LargeRoomWidth;
            }
            if (sz == ShipEquipment.RoomSize.Core) return 3;
            if (sz == ShipEquipment.RoomSize.Engine) return 3;
            if (sz == ShipEquipment.RoomSize.Armour) return 3;
            return 0;
        }
        public static int RoomHeight(ShipEquipment.RoomSize sz, bool bRotated) {
            if (sz == ShipEquipment.RoomSize.Weapon) return 1;
            if (sz == ShipEquipment.RoomSize.Small) {
                if (!bRotated) return Const.SmallRoomHeight;
                return Const.SmallRoomWidth;
            }
            if (sz == ShipEquipment.RoomSize.Medium) {
                if (!bRotated) return Const.MediumRoomHeight;
                return Const.MediumRoomWidth;
            }
            if (sz == ShipEquipment.RoomSize.Large) {
                if (!bRotated) return Const.LargeRoomHeight;
                return Const.LargeRoomWidth;
            }
            if (sz == ShipEquipment.RoomSize.Core) return 3;
            if (sz == ShipEquipment.RoomSize.Engine) return 5;
            if (sz == ShipEquipment.RoomSize.Armour) return 3;
            return 0;
        }
    }
}