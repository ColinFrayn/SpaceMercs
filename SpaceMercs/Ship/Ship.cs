using OpenTK.Graphics.OpenGL;
using SpaceMercs.Graphics;
using System.IO;
using System.Xml;

namespace SpaceMercs {
    class Ship {
        public ShipType Type { get; private set; }
        public string Name { get; private set; }
        public double Hull { get; private set; } // Amount of health left
        public double Shield { get; private set; } // Amount of shield left
        private readonly Dictionary<int, Tuple<ShipEquipment, bool>> Equipment = new Dictionary<int, Tuple<ShipEquipment, bool>>();
        public Team Owner { get; private set; }
        public int Seed { get; private set; }

        // Feed-through properties
        public ShipEngine Engine {
            get {
                if (Type.EngineRoomID == -1) return null;
                if (!Equipment.ContainsKey(Type.EngineRoomID)) return null;
                return Equipment[Type.EngineRoomID].Item1 as ShipEngine;
            }
        }
        public bool EngineEnabled {
            get {
                if (Type.EngineRoomID == -1) return false;
                if (!Equipment.ContainsKey(Type.EngineRoomID)) return false;
                if (Equipment[Type.EngineRoomID].Item1 == null) return false;
                return Equipment[Type.EngineRoomID].Item2;
            }
        }
        public IEnumerable<ShipWeapon> AllWeapons {
            get {
                List<ShipWeapon> lw = new List<ShipWeapon>();
                foreach (int id in Equipment.Keys) {
                    if (Equipment[id].Item1 is ShipWeapon) lw.Add(Equipment[id].Item1 as ShipWeapon);
                }
                return lw;
            }
        }
        public IEnumerable<int> AllWeaponRooms {
            get {
                List<int> lw = new List<int>();
                foreach (int id in Equipment.Keys) {
                    if (Equipment[id].Item1 is ShipWeapon) lw.Add(id);
                }
                return lw;
            }
        }
        public ShipArmour ArmourType { get; private set; }
        public double Range { get { if (!EngineEnabled) return 0.0; return Engine.Range; } } // Range in metres
        public int Length { get { return Type.Length; } }
        public int Width { get { return Type.Width; } }
        public int MaxCapacity {
            get {
                int Cap = 1; // Ship has default of 1 space for commander
                foreach (Tuple<ShipEquipment, bool> tp in Equipment.Values) {
                    Cap += tp.Item1.Capacity;
                }
                return Cap;
            }
        } // Maximum number of soldiers
        public double HullFract {
            get {
                return Hull / Type.MaxHull;
            }
        }
        public double ShieldFract {
            get {
                if (MaxShield == 0.0) return 0.0;
                return Shield / MaxShield;
            }
        }
        public int PowerGeneration {
            get {
                int g = 0;
                foreach (Tuple<ShipEquipment, bool> tp in Equipment.Values) {
                    if (tp.Item2) g += tp.Item1.Generate;
                }
                return g;
            }
        }
        public int PowerConsumption {
            get {
                int p = 0;
                foreach (Tuple<ShipEquipment, bool> tp in Equipment.Values) {
                    if (tp.Item2) p += tp.Item1.Power;
                }
                return p;
            }
        }
        public int TotalBerths {
            get {
                int g = 1; // Start with one space for the initial soldier
                foreach (Tuple<ShipEquipment, bool> tp in Equipment.Values) {
                    if (tp.Item2) g += tp.Item1.Capacity;
                }
                return g;
            }
        }
        public bool CanFly {
            get {
                if (PowerConsumption > PowerGeneration) return false;
                if (Engine == null) return false;
                if (Owner.ActiveSoldiers > MaxCapacity) return false;
                return true;
            }
        }
        public bool CanScan {
            get {
                foreach (Tuple<ShipEquipment, bool> tp in Equipment.Values) {
                    if (tp.Item2 && tp.Item1.Scanner) return true;
                }
                return false;
            }
        }
        public bool HasArmoury {
            get {
                foreach (Tuple<ShipEquipment, bool> tp in Equipment.Values) {
                    if (tp.Item2 && tp.Item1.Armoury) return true;
                }
                return false;
            }
        }
        public bool HasWorkshop {
            get {
                foreach (Tuple<ShipEquipment, bool> tp in Equipment.Values) {
                    if (tp.Item2 && tp.Item1.Workshop) return true;
                }
                return false;
            }
        }
        public bool HasMedlab {
            get {
                foreach (Tuple<ShipEquipment, bool> tp in Equipment.Values) {
                    if (tp.Item2 && tp.Item1.Medlab) return true;
                }
                return false;
            }
        }
        public bool CanRepair {
            get {
                foreach (Tuple<ShipEquipment, bool> tp in Equipment.Values) {
                    if (tp.Item2 && tp.Item1.Repair) return true;
                }
                return false;
            }
        }
        public bool CanResearch {
            get {
                foreach (Tuple<ShipEquipment, bool> tp in Equipment.Values) {
                    if (tp.Item2 && tp.Item1.Research) return true;
                }
                return false;
            }
        }
        public bool CanFoundColony {
            get {
                foreach (Tuple<ShipEquipment, bool> tp in Equipment.Values) {
                    if (tp.Item2 && tp.Item1.BuildColony) return true;
                }
                return false;
            }
        }
        public double EstimatedBountyValue {
            get {
                double value = Type.MaxHull * Type.MaxHull / 5.0;
                // Bounty is some measure of what a captain can pay, so ignore equipment and just scale it off the size of ship they own
                return value * Const.ShipBountyScale;
            }
        }

        // Pre-calculated combat properties
        public int MaxShield { get; private set; }
        public int Attack { get; private set; }
        public int Defence { get; private set; }
        public int Armour { get { if (ArmourType == null) return 0; return ArmourType.BaseArmour; } }

        public static Ship GenerateStarterShip(Team tm, ShipType st = null) {
            Ship sh = new Ship(st != null ? st : StaticData.GetStarterShip());
            sh.Owner = tm;
            sh.Name = "Player Ship";
            sh.SetEngine(StaticData.GetShipEngineByName("Thrusters"));
            sh.AddBuiltEquipmentAutoSlot(StaticData.GetShipEquipmentByName("Fission Core"));
            sh.AddBuiltEquipmentAutoSlot(StaticData.GetShipWeaponByName("Chain Gun"));
            sh.InitialiseForBattle();
            return sh;
        }

        public Ship(ShipType st) {
            Type = st;
            Hull = Type.MaxHull;
            Name = "Unnamed Ship";
            InitialiseForBattle();
            Random rand = new Random();
            Seed = rand.Next(1000000);
        }
        public Ship(XmlNode xml, Team owner) {
            Owner = owner;
            Name = xml.SelectSingleNode("Name").InnerText;
            if (xml.Attributes["Type"] != null) Type = StaticData.GetShipTypeByName(xml.Attributes["Type"].Value);
            else {
                int seed = Int32.Parse(xml.Attributes["Seed"].Value);
                double diff = Double.Parse(xml.Attributes["Diff"].Value);
                Type = ShipType.SetupRandomShipType(diff, seed);
            }
            if (Type == null) throw new Exception("Could not ID Ship Type : " + xml.Attributes["Type"].Value);
            Hull = Double.Parse(xml.SelectSingleNode("Hull").InnerText);
            Seed = Int32.Parse(xml.SelectSingleNode("Seed").InnerText);
            Equipment.Clear();

            // Compatibility mode
            foreach (XmlNode xr in xml.SelectNodes("Room")) {
                //int id = Int32.Parse(xr.Attributes["ID"].Value);
                XmlNode xn = xr.SelectSingleNode("ShipRoom/Equipment");
                ShipEquipment se = StaticData.GetShipEquipmentByName(xn.InnerText);
                if (se == null) throw new Exception("Could not find room type : " + xn.InnerText);
                bool bActive = (xr.SelectSingleNode("ShipRoom/Active") != null);
                if (se is ShipArmour) ArmourType = se as ShipArmour;
                else {
                    int id = 0;
                    while (id < Type.Rooms.Count && (Type.Rooms[id].Size != se.Size || Equipment.ContainsKey(id))) id++;
                    if (id < 0 || id >= Type.Rooms.Count) throw new Exception("Could not find suitable room for equipment");
                    Equipment.Add(id, new Tuple<ShipEquipment, bool>(se, bActive));
                }
            }

            // New-style Equipment loading
            foreach (XmlNode xr in xml.SelectNodes("Eqp")) {
                int id = Int32.Parse(xr.Attributes["ID"].Value);
                ShipEquipment se = StaticData.GetShipEquipmentByName(xr.InnerText);
                bool bActive = bool.Parse(xr.Attributes["Active"].Value);
                Equipment.Add(id, new Tuple<ShipEquipment, bool>(se, bActive));
            }
            if (xml.SelectSingleNode("Armour") != null) {
                string strArm = xml.SelectSingleNode("Armour").InnerText;
                ShipArmour sa = StaticData.GetShipArmourByName(strArm);
                if (sa == null) throw new Exception("Couldn't find ShipArmour type " + strArm);
                ArmourType = sa;
            }
            else ArmourType = null;
            InitialiseForBattle();
            Shield = Double.Parse(xml.SelectSingleNode("Shield").InnerText); // Do this after InitialiseForBattle() because that method resets the shield
        }

        // Save this Ship to an Xml file
        public void SaveToFile(StreamWriter file) {
            if (String.IsNullOrEmpty(Type.Name)) file.WriteLine("<Ship Seed=\"" + Type.Seed + "\" Diff=\"" + Type.Diff + "\">");
            else file.WriteLine("<Ship Type=\"" + Type.Name + "\">");
            file.WriteLine(" <Name>" + Name + "</Name>");
            file.WriteLine(" <Hull>" + Hull + "</Hull>");
            file.WriteLine(" <Shield>" + Shield + "</Shield>");
            if (ArmourType != null) file.WriteLine(" <Armour>" + ArmourType.Name + "</Armour>");
            file.WriteLine(" <Seed>" + Seed + "</Seed>");
            foreach (int id in Equipment.Keys) {
                file.WriteLine(" <Eqp ID=\"" + id + "\" Active=\"" + Equipment[id].Item2 + "\">" + Equipment[id].Item1.Name + "</Eqp>");
            }
            file.WriteLine("</Ship>");
        }

        // Configure the ship
        public void SetEngine(ShipEngine se) {
            int n = 0;
            while (Type.Rooms[n].Size != ShipEquipment.RoomSize.Engine) n++;
            AddBuiltEquipmentAutoSlot(se, n); // Overwrite
        }
        public void SetOwner(Team pt) {
            Owner = pt;
        }

        // Add to this ship equipment of the given type, in the first available slot. FULLY BUILT.
        public void AddBuiltEquipmentAutoSlot(ShipEquipment se, int RID = -1) {
            if (se == null) return;
            int n = RID;
            if (RID == -1) {
                n = 0;
                while (n < Type.Rooms.Count && (Type.Rooms[n].Size != se.Size || Equipment.ContainsKey(n))) n++;
            }
            if (n < 0 || n >= Type.Rooms.Count) throw new Exception("Illegal room ID!");
            Equipment.Add(n, new Tuple<ShipEquipment, bool>(se, true));
        }

        // Draw this ship on the ShipView dialog, including room types etc.
        public void DrawSchematic(int iHover, bool bHoverHull) {
            double maxy = 0.0;
            TexSpecs? ts = null;
            if (ArmourType != null) ts = Textures.GetTexCoords(ArmourType);
            // Background / corridors
            foreach (ShipRoomDesign r in Type.Rooms) {
                GL.Color3(0.2, 0.2, 0.2);
                if (r.Size == ShipEquipment.RoomSize.Weapon || r.Size == ShipEquipment.RoomSize.Engine) { // No border
                    GL.Begin(BeginMode.Quads);
                    GL.Vertex3(r.XPos, r.YPos, 0.0);
                    GL.Vertex3(r.XPos + r.Width, r.YPos, 0.0);
                    GL.Vertex3(r.XPos + r.Width, r.YPos + r.Height, 0.0);
                    GL.Vertex3(r.XPos, r.YPos + r.Height, 0.0);
                    GL.End();
                }
                else {
                    if (ArmourType != null && ts != null) {
                        GL.BindTexture(TextureTarget.Texture2D, ts.Value.ID);
                        GL.Enable(EnableCap.Texture2D);
                        GL.Color4(1.0, 1.0, 1.0, 0.7);
                        GL.Begin(BeginMode.Quads);
                        float tx = ts.Value.X, ty = ts.Value.Y, tw = ts.Value.W, th = ts.Value.H;
                        for (int y = r.YPos - 1; y <= r.YPos + r.Height; y++) {
                            for (int x = r.XPos - 1; x <= r.XPos + r.Width; x++) {
                                if (y == r.YPos - 1 || x == r.XPos - 1 || y == r.YPos + r.Height || x == r.XPos + r.Width) {
                                    GL.TexCoord2(tx, ty + th);
                                    GL.Vertex3(x, y, 0.0);
                                    GL.TexCoord2(tx + tw, ty + th);
                                    GL.Vertex3(x + 1, y, 0.0);
                                    GL.TexCoord2(tx + tw, ty);
                                    GL.Vertex3(x + 1, y + 1, 0.0);
                                    GL.TexCoord2(tx, ty);
                                    GL.Vertex3(x, y + 1, 0.0);
                                }
                            }
                        }
                        GL.End();
                        GL.Color3(0.2, 0.2, 0.2);
                        GL.Disable(EnableCap.Texture2D);
                        GL.Begin(BeginMode.Quads);
                        GL.Vertex3(r.XPos, r.YPos, 0.0);
                        GL.Vertex3(r.XPos + r.Width, r.YPos, 0.0);
                        GL.Vertex3(r.XPos + r.Width, r.YPos + r.Height, 0.0);
                        GL.Vertex3(r.XPos, r.YPos + r.Height, 0.0);
                        GL.End();
                    }
                    else {
                        GL.Begin(BeginMode.Quads);
                        GL.Vertex3(r.XPos - 1, r.YPos - 1, 0.0);
                        GL.Vertex3(r.XPos + r.Width + 1, r.YPos - 1, 0.0);
                        GL.Vertex3(r.XPos + r.Width + 1, r.YPos + r.Height + 1, 0.0);
                        GL.Vertex3(r.XPos - 1, r.YPos + r.Height + 1, 0.0);
                        GL.End();
                    }
                }
                if (r.YPos + r.Height > maxy) maxy = r.YPos + r.Height;
            }

            // Display fillers
            if (Type.Fillers.Any()) {
                if (ArmourType != null) {
                    GL.BindTexture(TextureTarget.Texture2D, ts.Value.ID);
                    GL.Enable(EnableCap.Texture2D);
                }
                else GL.Color3(0.2, 0.2, 0.2);
                float tx = ts.Value.X, ty = ts.Value.Y, tw = ts.Value.W, th = ts.Value.H;
                GL.Begin(BeginMode.Quads);
                foreach (Point pt in Type.Fillers) {
                    if (ArmourType != null) GL.TexCoord2(tx, ty + th);
                    GL.Vertex3(pt.X, pt.Y, 0.0);
                    if (ArmourType != null) GL.TexCoord2(tx + tw, ty + th);
                    GL.Vertex3(pt.X + 1, pt.Y, 0.0);
                    if (ArmourType != null) GL.TexCoord2(ty + tw, ty);
                    GL.Vertex3(pt.X + 1, pt.Y + 1, 0.0);
                    if (ArmourType != null) GL.TexCoord2(ty, ty);
                    GL.Vertex3(pt.X, pt.Y + 1, 0.0);
                }
                GL.End();
                if (ArmourType != null) GL.Disable(EnableCap.Texture2D);
            }

            // Hover boundary
            if (iHover >= 0 && iHover < Type.Rooms.Count) {
                ShipRoomDesign rHover = Type.Rooms[iHover];
                GL.Color3(1.0, 0.0, 0.0);
                GL.Begin(BeginMode.Quads);
                GL.Vertex3(rHover.XPos - 0.2, rHover.YPos - 0.2, 0.1);
                GL.Vertex3(rHover.XPos + rHover.Width + 0.2, rHover.YPos - 0.2, 0.1);
                GL.Vertex3(rHover.XPos + rHover.Width + 0.2, rHover.YPos + rHover.Height + 0.2, 0.1);
                GL.Vertex3(rHover.XPos - 0.2, rHover.YPos + rHover.Height + 0.2, 0.1);
                GL.End();
            }
            // Rooms
            for (int n = 0; n < Type.Rooms.Count; n++) {
                ShipRoomDesign r = Type.Rooms[n];
                GL.Color3(0.6, 0.6, 0.6);
                GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line);
                GL.Begin(BeginMode.Quads);
                GL.Vertex3(r.XPos, r.YPos, 0.0);
                GL.Vertex3(r.XPos + r.Width, r.YPos, 0.0);
                GL.Vertex3(r.XPos + r.Width, r.YPos + r.Height, 0.0);
                GL.Vertex3(r.XPos, r.YPos + r.Height, 0.0);
                GL.End();
                GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);
                if (Equipment.ContainsKey(n)) DrawEquipment(n, ts.Value.ID);
            }

            // Perimeter
            if (bHoverHull) GL.Color3(1.0, 1.0, 1.0);
            else GL.Color3(0.4, 0.4, 0.4);
            GL.Begin(BeginMode.LineLoop);
            foreach (Point pt in Type.Perimeter) {
                GL.Vertex3(pt.X, pt.Y, 0.0);
            }
            GL.End();
        }

        // Draw this ship when in battle
        public void DrawBattle() {
            GL.Begin(BeginMode.Quads);
            for (int rno = 0; rno < Type.Rooms.Count; rno++) {
                ShipRoomDesign r = Type.Rooms[rno];
                if (r.Size != ShipEquipment.RoomSize.Weapon && r.Size != ShipEquipment.RoomSize.Engine) { // draw a border?
                    GL.Color3(0.2, 0.2, 0.2);
                    GL.Vertex3(r.XPos - 1, r.YPos - 1, 0.0);
                    GL.Vertex3(r.XPos + r.Width + 1, r.YPos - 1, 0.0);
                    GL.Vertex3(r.XPos + r.Width + 1, r.YPos + r.Height + 1, 0.0);
                    GL.Vertex3(r.XPos - 1, r.YPos + r.Height + 1, 0.0);
                }
                if (Equipment.ContainsKey(rno)) {
                    if (Equipment[rno].Item2) GL.Color3(0.4, 0.4, 0.4);
                    else GL.Color3(0.3, 0.3, 0.3);
                }
                else GL.Color3(0.2, 0.2, 0.2);
                GL.Vertex3(r.XPos, r.YPos, 0.0);
                GL.Vertex3(r.XPos + r.Width, r.YPos, 0.0);
                GL.Vertex3(r.XPos + r.Width, r.YPos + r.Height, 0.0);
                GL.Vertex3(r.XPos, r.YPos + r.Height, 0.0);
            }
            GL.End();

            // Display fillers
            if (Type.Fillers.Any()) {
                GL.Color3(0.2, 0.2, 0.2);
                GL.Begin(BeginMode.Quads);
                foreach (Point pt in Type.Fillers) {
                    GL.Vertex3(pt.X, pt.Y, 0.0);
                    GL.Vertex3(pt.X + 1, pt.Y, 0.0);
                    GL.Vertex3(pt.X + 1, pt.Y + 1, 0.0);
                    GL.Vertex3(pt.X, pt.Y + 1, 0.0);
                }
                GL.End();
            }

            GL.Color3(0.5, 0.5, 0.5);
            GL.Begin(BeginMode.LineLoop);
            foreach (Point pt in Type.Perimeter) {
                GL.Vertex3(pt.X, pt.Y, 0.0);
            }
            GL.End();
            GL.Color3(1.0, 1.0, 1.0);
            GL.Begin(BeginMode.LineLoop);
            foreach (Point pt in Type.Perimeter) {
                GL.Vertex3(pt.X, pt.Y, 0.0);
            }
            GL.End();
        }

        // Draw the equipment in a room
        private void DrawEquipment(int id, int iBuildingTexture) {
            if (!Equipment.ContainsKey(id)) throw new Exception("Attempting to draw non-existent ship equipment!");
            ShipEquipment se = Equipment[id].Item1;
            ShipRoomDesign rd = Type.Rooms[id];
            double dIconSize = Math.Min(rd.Width, rd.Height);
            if (dIconSize > 1.0) dIconSize = 1.0 + (dIconSize - 1.0) / 2.0;
            else dIconSize = 0.85;
            double sx = rd.XPos + (rd.Width - dIconSize) / 2.0;
            double sy = rd.YPos + (rd.Height - dIconSize) / 2.0;
            TexSpecs ts = Textures.GetTexCoords(se);
            GL.BindTexture(TextureTarget.Texture2D, iBuildingTexture);
            GL.Enable(EnableCap.Texture2D);
            GL.Begin(BeginMode.Quads);
            GL.TexCoord2(ts.X, ts.Y + ts.H);
            GL.Vertex3(sx, sy, 0.0);
            GL.TexCoord2(ts.X + ts.W, ts.Y + ts.H);
            GL.Vertex3(sx + dIconSize, sy, 0.0);
            GL.TexCoord2(ts.X + ts.W, ts.Y);
            GL.Vertex3(sx + dIconSize, sy + dIconSize, 0.0);
            GL.TexCoord2(ts.X, ts.Y);
            GL.Vertex3(sx, sy + dIconSize, 0.0);
            GL.End();
            GL.Disable(EnableCap.Texture2D);
            if (!Equipment[id].Item2) {
                GL.PushMatrix();
                GL.Translate(sx + dIconSize / 2.0, sy + dIconSize / 2.0, 0.0);
                GraphicsFunctions.DrawInactiveIcon(dIconSize);
                GL.PopMatrix();
            }
        }

        // See if we're hovering over a room
        public int CheckHoverRoom(double xpos, double ypos, out bool bHoverHull) {
            bHoverHull = false;
            for (int n = 0; n < Type.Rooms.Count; n++) {
                ShipRoomDesign r = Type.Rooms[n];
                if (xpos >= r.XPos && xpos < r.XPos + r.Width && ypos >= r.YPos && ypos < r.YPos + r.Height) return n;
                if (r.Size != ShipEquipment.RoomSize.Weapon && r.Size != ShipEquipment.RoomSize.Engine) {
                    if (xpos >= r.XPos - 1 && xpos < r.XPos + r.Width + 1 && ypos >= r.YPos - 1 && ypos < r.YPos + r.Height + 1) { bHoverHull = true; return -1; }
                }
            }
            foreach (Point pt in Type.Fillers) {
                if (xpos >= pt.X && xpos < pt.X + 1 && ypos >= pt.Y && ypos < pt.Y + 1) { bHoverHull = true; return -1; }
            }
            return -1;
        }

        // User wants to build some equipment in this room
        public void BuildEquipment(int iRoomID, ShipEquipment se) {
            if (iRoomID == -1 || iRoomID >= Type.Rooms.Count || se == null) return;
            if (se.Size != Type.Rooms[iRoomID].Size) return;
            if (Equipment.ContainsKey(iRoomID)) return;
            double cost = CostToBuildEquipment(se);
            if (cost > Owner.Cash) return;
            string strMessage = String.Format("Really build {0} for {1:F2} credits?", se.Name, cost);
            if (MessageBox.Show(new Form { TopMost = true }, strMessage, "Build Room", MessageBoxButtons.YesNo) == DialogResult.Yes) {
                Owner.Cash -= cost;
                Equipment.Add(iRoomID, new Tuple<ShipEquipment, bool>(se, true));
            }
        }
        public void UpgradeHull(ShipEquipment se) {
            if (se.Size != ShipEquipment.RoomSize.Armour) return;
            if (!(se is ShipArmour)) return;
            double cost = CostToBuildEquipment(se);
            if (cost > Owner.Cash) return;
            string strMessage = String.Format("Really build {0} for {1:F2} credits?", se.Name, cost);
            if (MessageBox.Show(new Form { TopMost = true }, strMessage, "Upgrade Hull", MessageBoxButtons.YesNo) == DialogResult.Yes) {
                Owner.Cash -= cost;
                ArmourType = se as ShipArmour;
            }
        }
        public void ActivateRoom(int iRoomID) {
            if (!Equipment.ContainsKey(iRoomID)) return;
            Equipment[iRoomID] = new Tuple<ShipEquipment, bool>(Equipment[iRoomID].Item1, true);
        }
        public void DeactivateRoom(int iRoomID) {
            if (!Equipment.ContainsKey(iRoomID)) return;
            Equipment[iRoomID] = new Tuple<ShipEquipment, bool>(Equipment[iRoomID].Item1, false);
        }
        public ShipEquipment GetEquipmentByRoomID(int id) {
            if (!Equipment.ContainsKey(id)) return null;
            return Equipment[id].Item1;
        }
        public bool GetIsRoomActive(int id) {
            if (!Equipment.ContainsKey(id)) return false;
            return Equipment[id].Item2;
        }
        public bool GetCanDeactivateRoom(int id) {
            if (!Equipment.ContainsKey(id)) return false;
            if (!GetIsRoomActive(id)) return false;
            if (Equipment[id].Item1.Power == 0) return false;
            return true;
        }
        public ShipRoomDesign PickRandomRoom(Random rand) {
            return Type.Rooms[rand.Next(Type.Rooms.Count)];
        }
        public void RemoveColonyBuilder() {
            foreach (int r in Equipment.Keys) {
                Tuple<ShipEquipment, bool> tp = Equipment[r];
                if (tp.Item2 && tp.Item1.BuildColony) {
                    Equipment.Remove(r);
                    return;
                }
            }
            throw new Exception("Couldn't find colony builder to remove!");
        }

        // User wants to salvage this room
        public void SalvageRoom(int iRoomID) {
            if (!Equipment.ContainsKey(iRoomID)) return;
            double rebate = SalvageValue(Equipment[iRoomID].Item1);
            string strMessage = String.Format("Really salvage this room ({0})? You will recover {1:F2} credits", Equipment[iRoomID].Item1.Name, rebate);
            if (MessageBox.Show(new Form { TopMost = true }, strMessage, "Salvage Room", MessageBoxButtons.YesNo) == DialogResult.Yes) {
                Owner.Cash += rebate;
                Equipment.Remove(iRoomID);
            }
        }
        public void SalvageHull() {
            if (ArmourType == null) return;
            double rebate = SalvageValue(ArmourType);
            string strMessage = String.Format("Really salvage your ship armour ({0})? You will recover {1:F2} credits", ArmourType.Name, rebate);
            if (MessageBox.Show(new Form { TopMost = true }, strMessage, "Salvage Armour", MessageBoxButtons.YesNo) == DialogResult.Yes) {
                Owner.Cash += rebate;
                ArmourType = null;
            }
        }

        public double CalculateTravelTime(AstronomicalObject aoFrom, AstronomicalObject aoTo) {
            if (Engine == null) return Double.MaxValue;
            if (!EngineEnabled) return Double.MaxValue;
            double dist = AstronomicalObject.CalculateDistance(aoFrom, aoTo);
            if (dist < double.Epsilon) return 0.0;
            if (dist > Engine.Range) return Double.MaxValue;

            // Calculate time
            double dTime = 2.0 * Math.Sqrt(dist / Engine.Accel);  // S = dist/2, solve for T for half distance, then double it.
            double MaxSpeed = (dTime / 2.0) * Engine.Accel;
            if (MaxSpeed <= Engine.Speed) return dTime; // Everything is OK

            // The given time is invalid as we will exceed the max speed of this engine. So recalculate.
            double dAccelTime = Engine.Speed / Engine.Accel;
            double dDist = Engine.Accel * (dAccelTime * dAccelTime); // (2.0 * 1/2at^2);
            double dRemainder = dist - dDist;
            double dExtraTime = dRemainder / Engine.Speed;
            return (dAccelTime * 2.0) + dExtraTime;
        }

        // Can we outrun this ship?
        public bool CanOutrun(Ship sh) {
            // Must be at least as good in Accel and Speed, and superior in at least one
            if (Engine == null) return false;
            if (sh.Engine == null) return true;
            if (Engine.Accel < sh.Engine.Accel) return false;
            if (Engine.Speed < sh.Engine.Speed) return false;
            if (Engine.Accel > sh.Engine.Accel) return true;
            if (Engine.Speed > sh.Engine.Speed) return true;
            return false;
        }

        // Generate what salvage this ship contains (if it has been destroyed, then generate less)
        public Dictionary<IItem, int> GenerateSalvage(bool bDestroyed) {
            Dictionary<IItem, int> dSalvage = new Dictionary<IItem, int>();
            Dictionary<string, double> dMats = new Dictionary<string, double>() { { "Steel", 0.4 }, { "Aluminium", 0.8 }, { "Rubber", 0.1 }, { "Carbon Fibre", 0.5 }, { "Composite", 0.6 }, { "Rare Earth Metals", 0.1 }, { "Copper", 0.1 }, { "Fabric", 0.1 }, { "Plastic", 0.1 } };
            Random rand = new Random();
            foreach (string strMat in dMats.Keys) {
                double dmult = dMats[strMat];
                MaterialType mat = StaticData.GetMaterialTypeByName(strMat);
                int num = (int)(Type.MaxHull * dmult * (rand.NextDouble() + 2.0) / 5.0);
                if (bDestroyed) num = (int)(num * (rand.NextDouble() + 2.0) / 5.0);
                if (num > 0) dSalvage.Add(new Material(mat), num);
            }

            return dSalvage;
        }

        // Generate a ship for the given race, of the given difficulty
        public static Ship GenerateRandomShipOfRace(Race rc, double dDiff, ShipEngine minDrive) {
            Random rand = new Random();
            Ship sh = new Ship(ShipType.SetupRandomShipType(dDiff, rand.Next()));

            // Set up the ship itself
            sh.Owner = null;
            sh.Hull = sh.Type.MaxHull;
            sh.Name = "Enemy Ship";

            // Set up equipment (we only care about weapons, equipment, armour, engine)
            double dCash = (dDiff * (40.0 + sh.Equipment.Count * 6.0)) + 20.0;
            ShipEngine seng = StaticData.GetRandomShipItemOfMaximumCost(StaticData.ShipEngines.ToList<ShipEquipment>(), ShipEquipment.RoomSize.Engine, rc, dCash / 5.0, rand) as ShipEngine;
            if (minDrive != null && (seng == null || seng.Range < minDrive.Range)) seng = minDrive;
            if (seng != null) sh.SetEngine(seng); // Engine doesn't come out of the cash reserves

            bool bIsArmed = false;
            for (int iRoomID = 0; iRoomID < sh.Type.Rooms.Count; iRoomID++) {
                ShipRoomDesign rd = sh.Type.Rooms[iRoomID];
                if (rd.Size == ShipEquipment.RoomSize.Weapon) {
                    double dCashToSpend = dCash / 5.0;
                    int iTries = 0;
                    do {
                        ShipEquipment se = StaticData.GetRandomShipItemOfMaximumCost(StaticData.ShipWeapons.ToList<ShipEquipment>(), rd.Size, rc, dCashToSpend, rand);
                        sh.AddBuiltEquipmentAutoSlot(se, iRoomID);
                        if (se == null) {
                            if (iTries++ > 5) dCashToSpend *= 1.1; // If we didn't get a weapon after a few tries then allow us to spend more until we get one.
                        }
                        else {
                            dCash -= se.Cost;
                            bIsArmed = true;
                        }
                    } while (!bIsArmed); // Ensure that the ship has at least one weapon
                }
                else if (rd.Size == ShipEquipment.RoomSize.Armour) {
                    ShipEquipment se = StaticData.GetRandomShipItemOfMaximumCost(StaticData.ShipArmours.ToList<ShipEquipment>(), rd.Size, rc, dCash / 5.0, rand);
                    sh.AddBuiltEquipmentAutoSlot(se, iRoomID);
                    if (se != null) dCash -= se.Cost;
                }
                else if (rd.Size == ShipEquipment.RoomSize.Small || rd.Size == ShipEquipment.RoomSize.Medium || rd.Size == ShipEquipment.RoomSize.Large) {
                    ShipEquipment se = StaticData.GetRandomShipItemOfMaximumCost(StaticData.ShipEquipment, rd.Size, rc, dCash / 5.0, rand);
                    sh.AddBuiltEquipmentAutoSlot(se, iRoomID);
                    if (se != null) dCash -= se.Cost;
                }
            }

            sh.Seed = rand.Next(1000000);
            sh.InitialiseForBattle();
            return sh;
        }

        // Set up the ship's combat statistics correctly
        public void InitialiseForBattle() {
            MaxShield = 0;
            Attack = 0;
            Defence = 0;
            foreach (Tuple<ShipEquipment, bool> tp in Equipment.Values) {
                if (tp.Item2) {
                    MaxShield += tp.Item1.Shield;
                    if (tp.Item1 is ShipWeapon) ((ShipWeapon)tp.Item1).Cooldown = 0.0;
                    Attack += tp.Item1.Attack;
                    Defence += tp.Item1.Defence;
                }
            }
            Shield = MaxShield;
        }

        // Cause damage to this ship
        public double DamageShip(double dmg) {
            if (dmg <= 0.0) return 0.0;
            if (Shield > 0.0) {
                if (Shield >= dmg) {
                    Shield -= dmg;
                    return 0.0;
                }
                dmg -= Shield;
            }
            dmg *= (double)(100 - Armour) / 100.0; // Ship armour is a %age reduction
            if (dmg <= 0.0) return 0.0;
            Hull -= dmg;
            return dmg;
        }
        public void RepairHull() {
            Hull = Type.MaxHull;
        }

        // Calculate salvage value of this entire ship
        public double CalculateSalvageValue() {
            double Value = Type.Cost; // Value of the base ship

            // Add in salvage value of all equipment/rooms
            foreach (Tuple<ShipEquipment, bool> tp in Equipment.Values) {
                Value += SalvageValue(tp.Item1);
            }

            // Modify based on salvage rate & race relations etc.
            return Value * Const.SalvageRate / Owner.GetPriceModifier(Owner.CurrentPosition.GetSystem().Owner);
        }

        // Calculate how much it would cost to repair this ship
        public double CalculateRepairCost() {
            double Value = Type.Cost; // Value of the base ship

            // Add in salvage value of all equipment/rooms
            foreach (Tuple<ShipEquipment, bool> tp in Equipment.Values) {
                Value += CostToBuildEquipment(tp.Item1) * Const.ShipEquipmentRepairFract;  // Most damage will be the hull
            }

            // Take damage into account
            Value *= HullFract * Const.ShipRepairCostScale;

            return Value;
        }

        // Fabrication stuff
        public double CostToBuildEquipment(ShipEquipment se) {
            double cost = se.Cost * Owner.GetPriceModifier(Owner.CurrentPosition.GetSystem().Owner);
            if (se.Size == ShipEquipment.RoomSize.Armour) cost *= Type.MaxHull / Const.HullUpgradeCost;
            return Math.Round(cost, 2);
        }
        public double SalvageValue(ShipEquipment se) {
            double rebate = se.Cost * Const.SalvageRate / Owner.GetPriceModifier(Owner.CurrentPosition.GetSystem().Owner);
            if (se.Size == ShipEquipment.RoomSize.Armour) rebate *= Type.MaxHull / Const.HullUpgradeCost;
            return rebate;
        }
        public bool CanBuildItem(IItem it) {
            // Can we build this item in ths ship?
            // This required (1) the right room equipment and (2) a soldier with the right skill
            Equipment eqi = (it is Equipment eqp) ? eqp : null;
            if ((!HasMedlab || !Owner.HasSkill(Soldier.UtilitySkill.Medic)) && eqi != null && eqi.BaseType.Source == ItemType.ItemSource.Medlab) return false;
            if ((!HasWorkshop || !Owner.HasSkill(Soldier.UtilitySkill.Engineer)) && eqi != null && eqi.BaseType.Source == ItemType.ItemSource.Workshop) return false;
            if (it is Weapon || it is Armour) {
                if (!HasArmoury) return false;
                if (it is Armour && !Owner.HasSkill(Soldier.UtilitySkill.Armoursmith)) return false;
                if (it is Weapon wp) {
                    if (wp.Range == 0 && !Owner.HasSkill(Soldier.UtilitySkill.Bladesmith)) return false;
                    if (wp.Range > 0 && !Owner.HasSkill(Soldier.UtilitySkill.Gunsmith)) return false;
                }
            }
            return true;
        }

    }
}
