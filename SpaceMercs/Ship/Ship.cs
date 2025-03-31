using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using SpaceMercs.Graphics;
using SpaceMercs.Graphics.Shapes;
using System.IO;
using System.Xml;
using static SpaceMercs.Delegates;

namespace SpaceMercs {
    public class Ship {
        public ShipType Type { get; private set; }
        public string Name { get; private set; }
        public double Hull { get; private set; } // Amount of health left
        public double Shield { get; private set; } // Amount of shield left
        private readonly Dictionary<int, Tuple<ShipEquipment, bool>> Equipment = new Dictionary<int, Tuple<ShipEquipment, bool>>();
        public Team? Owner { get; private set; }
        public int Seed { get; private set; }
        public string ClassName {  
            get {
                if (Type is null) return "unknown";
                if (string.IsNullOrEmpty(Type.Name)) return Type.SizeString();
                return $"{Type.Name}-class"; 
            }
        }

        // Feed-through properties
        public ShipEngine? Engine {
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
                    if (Equipment[id].Item1 is ShipWeapon sw) lw.Add(sw);
                }
                return lw;
            }
        }
        public IEnumerable<int> AllWeaponRooms {
            get {
                foreach (int id in Equipment.Keys) {
                    if (Equipment[id].Item1 is ShipWeapon) yield return id;
                }
            }
        }
        public ShipArmour? ArmourType { get; private set; }
        public double Range { get { if (!EngineEnabled) return 0.0; return Engine?.Range ?? 0.0; } } // Range in metres
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
        public float HullFract {
            get {
                return (float)(Hull / Type.MaxHull);
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
                if (Engine is null || Owner is null) return false;
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
                if (ArmourType is not null && ArmourType.Repair > 0) return true; 
                foreach (Tuple<ShipEquipment, bool> tp in Equipment.Values) {
                    if (tp.Item2 && tp.Item1.Repair > 0) return true;
                }
                return false;
            }
        }
        public int RepairRate {
            get {
                int rep = ArmourType?.Repair ?? 0;
                foreach (Tuple<ShipEquipment, bool> tp in Equipment.Values) {
                    if (tp.Item2) rep += tp.Item1.Repair;
                }
                return rep;
            }
        }
        public int AIBoost {
            get {
                int boost = 0;
                foreach (Tuple<ShipEquipment, bool> tp in Equipment.Values) {
                    if (tp.Item2 && tp.Item1.AIModule) boost++;
                }
                return boost * Const.SkillBoostPerAIModule;
            }
        }
        public bool HasEngineering {
            get {
                foreach (Tuple<ShipEquipment, bool> tp in Equipment.Values) {
                    if (tp.Item2 && tp.Item1.Engineering) return true;
                }
                return false;
            }
        }
        public bool HasFacility(ShipEquipment.RoomAbilities ability) =>
            ability switch {
                ShipEquipment.RoomAbilities.Medlab => HasMedlab,
                ShipEquipment.RoomAbilities.Armoury => HasArmoury,
                ShipEquipment.RoomAbilities.Workshop => HasWorkshop,
                ShipEquipment.RoomAbilities.Engineering => HasEngineering,
                _ => false
            };
        public bool IsConcealed {
            get {
                foreach (Tuple<ShipEquipment, bool> tp in Equipment.Values) {
                    if (tp.Item2 && tp.Item1.Conceal) return true;
                }
                return false;
            }
        }
        public double EstimatedBountyValue {
            get {
                double value = Type.MaxHull * Type.MaxHull;
                // Bounty is some measure of what a captain can pay, so ignore equipment and just scale it off the size of ship they own
                return value * Const.ShipBountyScale;
            }
        }
        public double EstimatedStrength {
            get {
                double strength = (Type.MaxHull * Hull); // Scale a bit by the current level of damage
                strength *= (RepairRate + 10.0) / 10.0;  // +10% for each repair tick
                strength *= (Armour + 20.0) / 20.0; // +5% for each armour point
                strength += (Shield * MaxShield * 1.5); // Shields are more valuable than hull as they regen automatically
                strength *= (Defence + 20d) / 20d;
                foreach (ShipWeapon sw in AllWeapons) {
                    strength += (sw.Attack + Attack) * sw.Range / (150.0 * sw.Rate);
                }
                return strength;
            }
        }

        // Pre-calculated combat properties
        public int MaxShield { get; private set; }
        public int Attack { get; private set; }
        public int Defence { get; private set; }
        public int Armour { get { return Type.Armour + ArmourType?.BaseArmour ?? 0; } }
        public double WeaponRange { get; private set; }

        public static Ship GenerateStarterShip(Team tm, ShipType? st = null) {
            Ship sh = new Ship(st ?? StaticData.GetStarterShip() ?? throw new Exception("Could not find ship type!"));
            sh.Owner = tm;
            sh.Name = "Player Ship";
            sh.SetEngine(StaticData.GetShipEngineByName("Thrusters"));
            sh.AddBuiltEquipmentAutoSlot(StaticData.GetShipEquipmentByName("Fission Core"));
            sh.AddBuiltEquipmentAutoSlot(StaticData.GetShipWeaponByName("Chain Gun"));
            sh.InitialiseForBattle();
            return sh;
        }

        public Ship() {
            Type = ShipType.Empty;
            Name = "Uninitialised";
        }
        public Ship(ShipType st) {
            Type = st;
            Hull = Type.MaxHull;
            Name = "Unnamed Ship";
            InitialiseForBattle();
            Random rand = new Random();
            Seed = rand.Next(1000000);
        }
        public Ship(XmlNode xml, Team? owner) {
            Owner = owner;
            Name = xml.SelectNodeText("Name");
            string strType = xml.GetAttributeText("Type");
            if (!string.IsNullOrEmpty(strType)) Type = StaticData.GetShipTypeByName(strType) ?? throw new Exception($"Could not identify ship type {strType} for ship {Name}");
            else {
                int seed = xml.GetAttributeInt("Seed");
                double diff = xml.GetAttributeDouble("Diff");
                Type = ShipType.SetupRandomShipType(diff, seed);
            }
            if (Type is null) throw new Exception($"Could not ID Ship Type for ship {Name}");
            Hull = xml.SelectNodeDouble("Hull");
            Seed = xml.SelectNodeInt("Seed");
            Equipment.Clear();

            // Compatibility mode
            foreach (XmlNode xr in xml.SelectNodesToList("Room")) {
                XmlNode xn = xr.SelectSingleNode("ShipRoom/Equipment") ?? throw new Exception("Could not find ShipRoom/Equipment details in savegame");
                ShipEquipment? se = StaticData.GetShipEquipmentByName(xn.InnerText) ?? throw new Exception("Could not find room type : " + xn.InnerText);
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
            foreach (XmlNode xr in xml.SelectNodesToList("Eqp")) {
                int id = xr.GetAttributeInt("ID");
                string eqpName = xr.InnerText;
                if (eqpName == "Targetting AI") eqpName = "Military Computer"; // Backwards compatibility
                ShipEquipment ? se = StaticData.GetShipEquipmentByName(eqpName) ?? throw new Exception($"Found unknown ShipEquipment {xr.InnerText} in savegame");
                bool bActive = bool.Parse(xr.GetAttributeText("Active"));
                Equipment.Add(id, new Tuple<ShipEquipment, bool>(se, bActive));
            }
            string strArm = xml.SelectNodeText("Armour");
            if (!string.IsNullOrEmpty(strArm)) {
                ShipArmour? sa = StaticData.GetShipArmourByName(strArm);
                ArmourType = sa ?? throw new Exception("Couldn't find ShipArmour type " + strArm);
            }
            else ArmourType = null;
            InitialiseForBattle();
            Shield = xml.SelectNodeDouble("Shield"); // Do this after InitialiseForBattle() because that method resets the shield
        }
        public static Ship Empty { get { return new Ship(); } }

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
        public void SetEngine(ShipEngine? se) {
            if (se is null) return;
            int n = 0;
            while (Type.Rooms[n].Size != ShipEquipment.RoomSize.Engine) n++;
            AddBuiltEquipmentAutoSlot(se, n); // Overwrite
        }
        public void SetOwner(Team pt) {
            Owner = pt;
        }

        // Add to this ship equipment of the given type, in the first available slot. FULLY BUILT.
        public void AddBuiltEquipmentAutoSlot(ShipEquipment? se, int RID = -1) {
            if (se is null) return;
            int n = RID;
            if (RID == -1) {
                n = 0;
                while (n < Type.Rooms.Count && (Type.Rooms[n].Size != se.Size || Equipment.ContainsKey(n))) n++;
            }
            if (n < 0 || n >= Type.Rooms.Count) throw new Exception("Illegal room ID!");
            Equipment.Add(n, new Tuple<ShipEquipment, bool>(se, true));
        }

        // Draw this ship on the ShipView dialog, including room types etc.
        public void DrawSchematic(ShaderProgram prog, int iHover, bool bHoverHull) {
            double maxy = 0.0;
            TexSpecs? ts = null;
            if (ArmourType != null) ts = Textures.GetTexCoords(ArmourType);

            // Shields
            if (MaxShield > 0.0) {
                prog.SetUniform("flatColour", new Vector4(0f, 0f, 1f, 1f));
                Type.DrawShields(prog);
            }

            // Background / corridors
            foreach (ShipRoomDesign r in Type.Rooms) {
                prog.SetUniform("flatColour", new Vector4(0.2f, 0.2f, 0.2f, 1f));
                if (r.Size == ShipEquipment.RoomSize.Weapon || r.Size == ShipEquipment.RoomSize.Engine) { // No border
                    Matrix4 pTranslateM = Matrix4.CreateTranslation(r.XPos, r.YPos, 0f);
                    Matrix4 pScaleM = Matrix4.CreateScale(r.Width, r.Height, 0f);
                    prog.SetUniform("model", pScaleM * pTranslateM);
                    GL.UseProgram(prog.ShaderProgramHandle);
                    Square.Flat.BindAndDraw();
                }
                else {
                    if (ArmourType != null && ts != null) {
                        prog.SetUniform("textureEnabled", true);
                        GL.ActiveTexture(TextureUnit.Texture0);
                        GL.BindTexture(TextureTarget.Texture2D, ts.Value.ID);
                        prog.SetUniform("flatColour", new Vector4(1f, 1f, 1f, 0.7f));

                        float tx = ts.Value.X, ty = ts.Value.Y, tw = ts.Value.W, th = ts.Value.H;
                        for (int y = r.YPos - 1; y <= r.YPos + r.Height; y++) {
                            for (int x = r.XPos - 1; x <= r.XPos + r.Width; x++) {
                                if (y == r.YPos - 1 || x == r.XPos - 1 || y == r.YPos + r.Height || x == r.XPos + r.Width) {
                                    prog.SetUniform("texPos", tx, ty);
                                    prog.SetUniform("texScale", tw, th);
                                    Matrix4 paTranslateM = Matrix4.CreateTranslation(x, y, 0f);
                                    Matrix4 paScaleM = Matrix4.CreateScale(1f, 1f, 0f);
                                    prog.SetUniform("model", paScaleM * paTranslateM);
                                    GL.UseProgram(prog.ShaderProgramHandle);
                                    Square.Textured.BindAndDraw();
                                }
                            }
                        }
                        GL.BindTexture(TextureTarget.Texture2D, 0);
                        prog.SetUniform("textureEnabled", false);
                        Matrix4 pTranslateM = Matrix4.CreateTranslation(r.XPos, r.YPos, 0f);
                        Matrix4 pScaleM = Matrix4.CreateScale(r.Width, r.Height, 0f);
                        prog.SetUniform("model", pScaleM * pTranslateM);
                        prog.SetUniform("flatColour", new Vector4(0.2f, 0.2f, 0.2f, 1f));
                        GL.UseProgram(prog.ShaderProgramHandle);
                        Square.Flat.BindAndDraw();
                    }
                    else {
                        Matrix4 pTranslateM = Matrix4.CreateTranslation(r.XPos - 1f, r.YPos - 1f, 0f);
                        Matrix4 pScaleM = Matrix4.CreateScale(r.Width + 2f, r.Height + 2f, 0f);
                        prog.SetUniform("model", pScaleM * pTranslateM);
                        prog.SetUniform("flatColour", new Vector4(0.2f, 0.2f, 0.2f, 1f));
                        GL.UseProgram(prog.ShaderProgramHandle);
                        Square.Flat.BindAndDraw();
                    }
                }
                if (r.YPos + r.Height > maxy) maxy = r.YPos + r.Height;
            }

            // Display fillers
            if (Type.Fillers.Any()) {
                Matrix4 pScaleM = Matrix4.CreateScale(1f, 1f, 0f);
                foreach (Point pt in Type.Fillers) {
                    Matrix4 pTranslateM = Matrix4.CreateTranslation(pt.X, pt.Y, 0f);
                    prog.SetUniform("model", pScaleM * pTranslateM);
                    if (ArmourType is null || ts is null) {
                        prog.SetUniform("flatColour", new Vector4(0.2f, 0.2f, 0.2f, 1f));
                        Square.Flat.BindAndDraw();
                    }
                    else {
                        float tx = ts.Value.X, ty = ts.Value.Y, tw = ts.Value.W, th = ts.Value.H;
                        GL.BindTexture(TextureTarget.Texture2D, ts.Value.ID);
                        prog.SetUniform("textureEnabled", true);
                        prog.SetUniform("texPos", tx, ty);
                        prog.SetUniform("texScale", tw, th);
                        prog.SetUniform("flatColour", new Vector4(1f, 1f, 1f, 1f));
                        Square.Textured.BindAndDraw();
                        prog.SetUniform("textureEnabled", false);
                    }
                }
            }

            // Display boundary for currently hovering room
            if (iHover >= 0 && iHover < Type.Rooms.Count) {
                ShipRoomDesign rHover = Type.Rooms[iHover];
                Matrix4 pTranslateM = Matrix4.CreateTranslation(rHover.XPos - 0.2f, rHover.YPos - 0.2f, 0f);
                Matrix4 pScaleM = Matrix4.CreateScale(rHover.Width + 0.4f, rHover.Height + 0.4f, 0f);
                prog.SetUniform("model", pScaleM * pTranslateM);
                prog.SetUniform("flatColour", new Vector4(1f, 0f, 0f, 1f));
                GL.UseProgram(prog.ShaderProgramHandle);
                Square.Lines.BindAndDraw();
            }
            // Display all rooms
            for (int n = 0; n < Type.Rooms.Count; n++) {
                ShipRoomDesign r = Type.Rooms[n];
                Matrix4 pTranslateM = Matrix4.CreateTranslation(r.XPos, r.YPos, 0f);
                Matrix4 pScaleM = Matrix4.CreateScale(r.Width, r.Height, 0f);
                prog.SetUniform("model", pScaleM * pTranslateM);
                prog.SetUniform("flatColour", new Vector4(0.6f, 0.6f, 0.6f, 1f));
                GL.UseProgram(prog.ShaderProgramHandle);
                Square.Lines.BindAndDraw();
                if (Equipment.ContainsKey(n)) DrawEquipment(prog, n);
            }

            // Display ship perimeter
            if (bHoverHull) prog.SetUniform("flatColour", new Vector4(1f, 1f, 1f, 1f));
            else prog.SetUniform("flatColour", new Vector4(0.4f, 0.4f, 0.4f, 1f));
            prog.SetUniform("model", Matrix4.Identity);
            Type.DrawPerimeter(prog);
        }

        // Draw this ship when in battle
        public void DrawBattle(ShaderProgram prog) {
            // Shields
            if (Shield > 0.0) {
                float shieldVis = (ShieldFract > 0.6) ? 1.0f : (float)(ShieldFract * 0.8 / 0.6) + 0.2f;
                prog.SetUniform("flatColour", new Vector4(0f, Math.Max(0.0f, shieldVis - 0.6f), shieldVis, 1.0f));
                prog.SetUniform("model", Matrix4.CreateScale(1.0f));
                Type.DrawShields(prog);
            }

            for (int rno = 0; rno < Type.Rooms.Count; rno++) {
                ShipRoomDesign r = Type.Rooms[rno];
                if (r.Size != ShipEquipment.RoomSize.Weapon && r.Size != ShipEquipment.RoomSize.Engine) { // draw a border?
                    prog.SetUniform("flatColour", new Vector4(0.2f, 0.2f, 0.2f, 1f));
                    Matrix4 piTranslateM = Matrix4.CreateTranslation(r.XPos - 1f, r.YPos - 1f, 0f);
                    Matrix4 piScaleM = Matrix4.CreateScale(r.Width + 2f, r.Height + 2f, 0f);
                    prog.SetUniform("model", piScaleM * piTranslateM);
                    GL.UseProgram(prog.ShaderProgramHandle);
                    Square.Flat.BindAndDraw();
                }
                if (Equipment.ContainsKey(rno)) {
                    if (Equipment[rno].Item2) prog.SetUniform("flatColour", new Vector4(0.4f, 0.4f, 0.4f, 1f));
                    else prog.SetUniform("flatColour", new Vector4(0.3f, 0.3f, 0.3f, 1f));                    
                }
                else prog.SetUniform("flatColour", new Vector4(0.2f, 0.2f, 0.2f, 1f));
                Matrix4 pTranslateM = Matrix4.CreateTranslation(r.XPos, r.YPos, 0f);
                Matrix4 pScaleM = Matrix4.CreateScale(r.Width, r.Height, 0f);
                prog.SetUniform("model", pScaleM * pTranslateM);
                GL.UseProgram(prog.ShaderProgramHandle);
                Square.Flat.BindAndDraw();
            }

            // Display fillers
            prog.SetUniform("flatColour", new Vector4(0.2f, 0.2f, 0.2f, 1f));
            if (Type.Fillers.Any()) {
                foreach (Point pt in Type.Fillers) {
                    Matrix4 pTranslateM = Matrix4.CreateTranslation(pt.X, pt.Y, 0f);
                    prog.SetUniform("model", pTranslateM);
                    GL.UseProgram(prog.ShaderProgramHandle);
                    Square.Flat.BindAndDraw();
                }
            }

            prog.SetUniform("flatColour", new Vector4(1f, 1f, 1f, 1f));
            prog.SetUniform("model", Matrix4.Identity);
            Type.DrawPerimeter(prog);
        }

        // Draw the equipment in a room
        private void DrawEquipment(ShaderProgram prog, int id) {
            if (!Equipment.ContainsKey(id)) throw new Exception("Attempting to draw non-existent ship equipment!");
            ShipEquipment se = Equipment[id].Item1;
            ShipRoomDesign rd = Type.Rooms[id];
            float fIconSize = (float)Math.Min(rd.Width, rd.Height);
            if (fIconSize > 1f) fIconSize = 1f + (fIconSize - 1f) / 2f;
            else fIconSize = 0.85f;
            float sx = rd.XPos + (rd.Width - fIconSize) / 2f;
            float sy = rd.YPos + (rd.Height - fIconSize) / 2f;
            TexSpecs ts = Textures.GetTexCoords(se);

            Matrix4 pScaleM = Matrix4.CreateScale(fIconSize);
            Matrix4 pTranslateM = Matrix4.CreateTranslation(sx, sy, 0f);
            prog.SetUniform("model", pScaleM * pTranslateM);
            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, ts.ID);
            prog.SetUniform("textureEnabled", true);
            prog.SetUniform("texPos", ts.X, ts.Y + ts.H);
            prog.SetUniform("texScale", ts.W, -ts.H);
            prog.SetUniform("flatColour", new Vector4(1f, 1f, 1f, 1f));
            GL.UseProgram(prog.ShaderProgramHandle);
            Square.Textured.BindAndDraw();
            prog.SetUniform("textureEnabled", false);

            if (!Equipment[id].Item2) {
                Matrix4 piSquashM = Matrix4.CreateScale(1.2f, 0.2f, 1f);
                Matrix4 piRotateM = Matrix4.CreateRotationZ((float)Math.PI / 4f);
                Matrix4 piTranslateM = Matrix4.CreateTranslation(sx + (fIconSize*0.1f), sy + (fIconSize * 0.0f), 0f);
                prog.SetUniform("model", piSquashM * piRotateM * pScaleM * piTranslateM);
                prog.SetUniform("flatColour", new Vector4(1f, 0f, 0f, 1f));
                GL.UseProgram(prog.ShaderProgramHandle);
                Square.Flat.BindAndDraw();
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
        public void BuildEquipment(int iRoomID, ShipEquipment se, ShowMessageDecisionDelegate showMessage) {
            if (iRoomID == -1 || iRoomID >= Type.Rooms.Count || se == null) return;
            if (se.Size != Type.Rooms[iRoomID].Size) return;
            string strMessage = string.Empty;
            if (Owner is null) throw new Exception("Ship owner is null!");
            double cost = CostToBuildEquipment(se);
            double salvage = 0d;
            bool bReplace = Equipment.TryGetValue(iRoomID, out Tuple<ShipEquipment, bool>? oldRoom);
            if (bReplace && oldRoom?.Item1 is ShipEquipment seOld) {
                salvage = SalvageValue(seOld);
                cost -= salvage;
                strMessage = string.Format($"Really replace {seOld.Name} with {se.Name} for {cost:F2} credits?\n(Includes rebate of {salvage:F2} credits)");
            }
            else {
                strMessage = string.Format($"Really build {se.Name} for {cost:F2} credits?");
            }
            if (cost > Owner.Cash) return;
            showMessage(strMessage, () => ContinueBuildRoom(cost, bReplace, iRoomID, se), null);
        }
        private void ContinueBuildRoom(double cost, bool bReplace, int iRoomID, ShipEquipment se) {
            if (Owner is null) return;
            Owner.Cash -= cost;
            if (bReplace) {
                Equipment[iRoomID] = new Tuple<ShipEquipment, bool>(se, true);
            }
            else Equipment.Add(iRoomID, new Tuple<ShipEquipment, bool>(se, true));
        }
        public void UpgradeHull(ShipEquipment se, ShowMessageDecisionDelegate showMessage) {
            if (se.Size != ShipEquipment.RoomSize.Armour) return;
            if (se is not ShipArmour sa) return;
            if (Owner is null) throw new Exception("Ship owner is null!");
            double cost = CostToBuildEquipment(se);
            if (cost > Owner.Cash) return;
            string strMessage = string.Format("Really build {0} for {1:F2} credits?", se.Name, cost);
            showMessage(strMessage, () => ContinueUpgradeHull(cost, sa), null);
        }
        private void ContinueUpgradeHull(double cost, ShipArmour sa) {
            if (Owner is null) return;
            Owner.Cash -= cost;
            ArmourType = sa;
        }
        public void ActivateRoom(int iRoomID) {
            if (!Equipment.ContainsKey(iRoomID)) return;
            Equipment[iRoomID] = new Tuple<ShipEquipment, bool>(Equipment[iRoomID].Item1, true);
        }
        public void DeactivateRoom(int iRoomID) {
            if (!Equipment.ContainsKey(iRoomID)) return;
            Equipment[iRoomID] = new Tuple<ShipEquipment, bool>(Equipment[iRoomID].Item1, false);
        }
        public ShipEquipment? GetEquipmentByRoomID(int id) {
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
        public bool CanFoundColony(HabitableAO hao) {
            foreach (Tuple<ShipEquipment, bool> tp in Equipment.Values) {
                if (tp.Item2 && tp.Item1.BuildColony != ShipEquipment.ColoniseAbility.None) {
                    if (tp.Item1.BuildColony == ShipEquipment.ColoniseAbility.Basic && hao.Type == Planet.PlanetType.Oceanic) return true;
                    if (tp.Item1.BuildColony == ShipEquipment.ColoniseAbility.Advanced && hao.Type != Planet.PlanetType.Gas) return true;
                    if (tp.Item1.BuildColony == ShipEquipment.ColoniseAbility.Cloud && hao.Type == Planet.PlanetType.Gas) return true;
                }
            }
            return false;
        }
        public void RemoveColonyBuilder(HabitableAO hao) {
            int bestID = -1;
            foreach (int r in Equipment.Keys) {
                Tuple<ShipEquipment, bool> tp = Equipment[r];
                if (tp.Item2 && tp.Item1.BuildColony != ShipEquipment.ColoniseAbility.None) {
                    if (tp.Item1.BuildColony == ShipEquipment.ColoniseAbility.Basic && hao.Type == Planet.PlanetType.Oceanic) bestID = r;
                    if (tp.Item1.BuildColony == ShipEquipment.ColoniseAbility.Advanced && hao.Type != Planet.PlanetType.Gas && bestID == -1) bestID = r; // Don't override a basic colony builder, if there are several
                    if (tp.Item1.BuildColony == ShipEquipment.ColoniseAbility.Cloud && hao.Type == Planet.PlanetType.Gas) bestID = r;
                }
            }
            if (bestID >= 0) Equipment.Remove(bestID);
            else throw new Exception("Couldn't find colony builder to remove!");
        }

        // User wants to salvage this room
        public void SalvageRoom(int iRoomID, ShowMessageDecisionDelegate showMessage) {
            if (!Equipment.ContainsKey(iRoomID)) return;
            if (Owner is null) throw new Exception("Ship owner is null!");
            double rebate = SalvageValue(Equipment[iRoomID].Item1);
            string strMessage = string.Format("Really salvage this room ({0})? You will recover {1:F2} credits", Equipment[iRoomID].Item1.Name, rebate);
            showMessage(strMessage, () => ContinueSalvageRoom(rebate, iRoomID), null);
        }
        private void ContinueSalvageRoom(double rebate, int iRoomID) {
            if (Owner is null) return;
            Owner.Cash += rebate;
            Equipment.Remove(iRoomID);
        }
        public void SalvageHull(ShowMessageDecisionDelegate showMessage) {
            if (ArmourType == null) return;
            if (Owner is null) throw new Exception("Ship owner is null!");
            double rebate = SalvageValue(ArmourType);
            string strMessage = string.Format("Really salvage your ship armour ({0})? You will recover {1:F2} credits", ArmourType.Name, rebate);
            showMessage(strMessage, () => ContinueSalvageArmour(rebate), null);
        }
        private void ContinueSalvageArmour(double rebate) {
            if (Owner is null) return;
            Owner.Cash += rebate;
            ArmourType = null;
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
        public Dictionary<IItem, int> GenerateSalvage(bool bDestroyed, Race? ra) {
            Dictionary<IItem, int> dSalvage = new Dictionary<IItem, int>();
            IEnumerable<MaterialType> dMats = StaticData.Materials.Where(m => m.Requirements is null || (ra is not null && m.Requirements.RaceMeetsRequirements(ra)));
            Random rand = new Random();
            foreach (MaterialType mat in dMats) {
                double quantity = Type.MaxHull * mat.Rarity * rand.NextDouble() / 3.0;
                if (bDestroyed) quantity *= (rand.NextDouble() + 2.0) / 5.0;
                int num = (int)quantity;
                if (num > 0) dSalvage.Add(new Material(mat), num);
            }

            return dSalvage;
        }

        // Generate a ship for the given race, of the given difficulty
        public static Ship GenerateRandomShipOfDifficulty(double dDiff, ShipEngine? minDrive) {
            Random rand = new Random();
            Ship sh = new Ship(ShipType.SetupRandomShipType(dDiff, rand.Next()));

            // Set up the ship itself
            sh.Owner = null;
            sh.Hull = sh.Type.MaxHull;
            sh.Name = "Enemy Ship";
            sh.Seed = rand.Next(1000000);

            // Set up equipment (we only care about weapons, equipment, armour, engine)
            double dCash = ((Math.Pow(1.5d,dDiff) * 40d) + (sh.Type.MaxHull * dDiff * 4d) + 20d) / 5d;
            ShipEngine? seng = StaticData.GetRandomShipItemOfMaximumCost(StaticData.ShipEngines.ToList<ShipEquipment>(), ShipEquipment.RoomSize.Engine, dCash / 5.0, rand) as ShipEngine;
            if (minDrive != null && (seng == null || seng.Range < minDrive.Range)) seng = minDrive;
            if (seng != null) sh.SetEngine(seng);

            bool bIsArmed = false;
            for (int iRoomID = 0; iRoomID < sh.Type.Rooms.Count; iRoomID++) {
                ShipRoomDesign rd = sh.Type.Rooms[iRoomID];
                if (rd.Size == ShipEquipment.RoomSize.Weapon) {
                    int iTries = 0;
                    do {
                        ShipEquipment? se = StaticData.GetRandomShipItemOfMaximumCost(StaticData.ShipWeapons.ToList<ShipEquipment>(), rd.Size, dCash * ((double)iTries + 10d) / 10d, rand);
                        if (se is null) iTries++;
                        else {
                            sh.AddBuiltEquipmentAutoSlot(se, iRoomID);
                            bIsArmed = true;
                        }
                    } while (!bIsArmed); // Ensure that the ship has at least one weapon
                }
                // Fill in some of the other rooms to make the ship look better
                else if (rd.Size == ShipEquipment.RoomSize.Small && rand.NextDouble() > 0.2) {
                    ShipEquipment? se = StaticData.GetShipEquipmentByName("Cabin");
                    sh.AddBuiltEquipmentAutoSlot(se, iRoomID);
                }
                else if (rd.Size == ShipEquipment.RoomSize.Medium && rand.NextDouble() > 0.3) {
                    ShipEquipment? se = StaticData.GetShipEquipmentByName("Bunk Room");
                    sh.AddBuiltEquipmentAutoSlot(se, iRoomID);
                }
                else if (rd.Size == ShipEquipment.RoomSize.Large && rand.NextDouble() > 0.4) {
                    ShipEquipment? se = StaticData.GetShipEquipmentByName("Large Bunk Room");
                    sh.AddBuiltEquipmentAutoSlot(se, iRoomID);
                }
            }
            if (dDiff + (rand.NextDouble() * 3d) > 5d) {
                ShipEquipment? arm = StaticData.GetRandomShipItemOfMaximumCost(StaticData.ShipArmours.ToList<ShipEquipment>(), ShipEquipment.RoomSize.Armour, dCash, rand);
                if (arm is ShipArmour sArm) {
                    sh.ArmourType = sArm;
                }
            }

            sh.InitialiseForBattle();
            return sh;
        }
        public static Ship GenerateSpaceHulkShip(int iDiff) {
            Random rand = new Random();
            Ship sh = new Ship(ShipType.SetupRandomShipType(iDiff, rand.Next(), true));
            sh.Owner = null;
            sh.Hull = sh.Type.MaxHull;
            sh.Name = "Abandoned Hulk";
            sh.Seed = rand.Next(1000000);
            return sh;
        }

        // Handle combat
        public void InitialiseForBattle() {
            MaxShield = 0;
            Attack = Type.Attack;
            Defence = ArmourType?.Defence ?? 0; // Armour that offers defensive benefits e.g. chameleon plating
            WeaponRange = 0d;
            foreach (Tuple<ShipEquipment, bool> tp in Equipment.Values) {
                if (tp.Item2) {
                    MaxShield += tp.Item1.Shield;
                    if (tp.Item1 is ShipWeapon weapon) {
                        weapon.Cooldown = 0.0;
                        if (weapon.Range > WeaponRange) WeaponRange = weapon.Range;
                    }
                    else Attack += tp.Item1.Attack; // Any ship rooms that add to attack such as targetting systems
                    Defence += tp.Item1.Defence; // Defensive items such as a cloaking device
                }
            }
            Shield = MaxShield;
            if (CanRepair) RepairHull(); // Hull repair is quick enough that it can fully repair ships between battles
        }
        public double DamageShip(double dmg) {
            if (dmg <= 0d) return 0d;
            if (Shield > 0d) {
                if (Shield >= dmg) {
                    Shield -= dmg;
                    return 0d;
                }
                Shield = 0d;
                dmg -= Shield;
            }
            dmg *= (double)(100 - Armour) / 100d; // Ship armour is a %age reduction
            if (dmg <= 0d) return 0d;
            Hull -= dmg;
            return dmg;
        }
        public void RepairHull() {
            Hull = Type.MaxHull;
        }
        public void BattleUpdate() {
            if (Hull < Type.MaxHull) {
                int repair = RepairRate;
                if (repair > 0) {
                    Hull += ((float)repair * Const.ShipRepairRate);
                }
            }
            if (Hull > Type.MaxHull) {
                Hull = Type.MaxHull;
            }
            if (Shield < MaxShield) {
                Shield += Const.ShipShieldRegenRate;
            }
            if (Shield > MaxShield) {
                Shield = MaxShield;
            }
        }

        // Calculate salvage value of this entire ship
        public double CalculateSalvageValue() {
            double Value = Type.Cost * Const.SalvageRate / (Owner?.GetLocalPriceModifier() ?? 1.0); // Value of the base ship

            // Add in salvage value of all equipment/rooms
            foreach (Tuple<ShipEquipment, bool> tp in Equipment.Values) {
                Value += SalvageValue(tp.Item1);
            }

            // Modify based on salvage rate & race relations etc.
            return Value;
        }

        // Calculate how much it would cost to repair this ship
        public double CalculateRepairCost() {
            double Value = Type.Cost; // Value of the base ship

            // Take damage into account
            Value *= HullFract * Const.ShipRepairCostScale;

            return Value;
        }

        // Can we build more than one of these?
        public bool CanBuildAnother(ShipEquipment se) {
            if (se is ShipWeapon) return true;
            foreach (Tuple<ShipEquipment, bool> tp in Equipment.Values) {
                if (tp.Item2 && tp.Item1.Size != ShipEquipment.RoomSize.Weapon) {
                    if (tp.Item1.Shield > 0 && se.Shield > 0) return false; // Shield generators
                    if (tp.Item1.Attack > 0 && se.Attack > 0) return false; // Targetting AI systems
                    if (tp.Item1.Defence > 0 && se.Defence > 0) return false; // Cloaking system
                }
            }
            return true;
        }

        // Fabrication stuff
        public double CostToBuildEquipment(ShipEquipment se) {
            if (Owner is null) throw new Exception("Ship owner is null!");
            double cost = se.Cost * Owner.GetLocalPriceModifier();
            if (se.Size == ShipEquipment.RoomSize.Armour) cost *= Type.MaxHull / Const.HullUpgradeCost;
            return Math.Round(cost, 2);
        }
        public double SalvageValue(ShipEquipment se) {
            double priceMod = Owner?.GetLocalPriceModifier() ?? 1.0;
            double rebate = se.Cost * Const.SalvageRate / priceMod;
            if (se.Size == ShipEquipment.RoomSize.Armour) rebate *= Type.MaxHull / Const.HullUpgradeCost;
            return rebate;
        }
        public bool CanBuildItem(IItem it) {
            // Can we build this item in ths ship?
            // This required (1) the right room equipment and (2) a soldier with the right skill
            if (Owner is null) throw new Exception("Ship owner is null!");
            Equipment? eqi = (it is Equipment eqp) ? eqp : null;
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
