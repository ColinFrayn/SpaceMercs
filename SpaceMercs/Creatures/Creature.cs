using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using SpaceMercs.Graphics;
using SpaceMercs.Graphics.Shapes;
using System.IO;
using System.Xml;

namespace SpaceMercs {
    class Creature : IEntity {
        // IEntity stuff
        public int X { get; private set; }
        public int Y { get; private set; }
        public int Level { get; private set; }
        public double Health { get; private set; }
        public double MaxHealth { get { return Type.HealthBase * (1.0 + Const.CreatureLevelHealthStep * (Level - 1)); } }
        public double Stamina { get; private set; }
        public double MaxStamina { get { return Type.StaminaBase + ((Level - 1) * Const.CreatureLevelStaminaStep) + StatBonuses(StatType.Stamina); } }
        public double Shields { get; private set; }
        public double MaxShields { get { return Type.ShieldsBase * (1.0 + Const.CreatureLevelShieldsStep * (Level - 1)); } }
        public double Attack { get { return Type.AttackBase * (1.0 + Const.CreatureLevelAttackStep * (Level - 1)) + StatBonuses(StatType.Attack); } }
        public double Defence { get { return Type.DefenceBase * (1.0 + Const.CreatureLevelDefenceStep * (Level - 1)) + StatBonuses(StatType.Defence); } }
        public int TravelRange { get { return (int)Stamina; } }
        public Weapon? EquippedWeapon { get; private set; }
        public double AttackRange { get { return (EquippedWeapon == null) ? 1.0 : EquippedWeapon.Range; } }
        public string Name { get { if (OverrideRace != null) return OverrideRace.Name + " " + Type.Name; else return Type.Name; } }
        public int Size { get { return Type.Size; } }
        public double Facing { get; set; }
        public Point Location { get { return new Point(X, Y); } }
        public Point Investigate { get; set; } = Point.Empty;
        private readonly List<Effect> _Effects = new List<Effect>();
        public IEnumerable<Effect> Effects { get { return _Effects.AsReadOnly(); } }
        private bool[,] Visible;
        public bool CanSee(int x, int y) { if (x < 0 || y < 0 || x > Visible.GetLength(0) || y > Visible.GetLength(1)) return false; return Visible[x, y]; }
        public bool CanSee(IEntity en) {
            if (en == null) return false;
            for (int yy = en.Y; yy < en.Y + en.Size; yy++) {
                for (int xx = en.X; xx < en.X + en.Size; xx++) {
                    if (Visible[xx, yy]) return true;
                }
            }
            return false;
        }
        public void UpdateVisibility(MissionLevel m) {
            Visible = m.CalculateVisibilityFromEntity(this);
        }
        public void SetLocation(Point p) {
            //double ang = Math.Atan2(p.Y - Y, p.X - X) * 180.0 / Math.PI;
            //Facing = Utils.AngleToDirection(ang);
            X = p.X;
            Y = p.Y;
        }
        public void SetFacing(Utils.Direction d) { Facing = Utils.DirectionToAngle(d); }
        public void SetFacing(double d) { Facing = d; }
        public void Display(ShaderProgram prog, bool bLabel, bool bStatBars, bool bShowEffects, float fViewHeight, float aspect, Matrix4 viewM) {
            int itexid = -1;
            if (Shields > 0.0) {
                if (Type.TextureShieldsID == -1) Type.GenerateTexture(true);
                itexid = Type.TextureShieldsID;
            }
            else {
                if (Type.TextureID == -1) Type.GenerateTexture(false);
                itexid = Type.TextureID;
            }
            GL.BindTexture(TextureTarget.Texture2D, itexid);

            prog.SetUniform("textureEnabled", true);
            prog.SetUniform("texPos", 0f, 0f);
            prog.SetUniform("texScale", 1f, 1f);
            Matrix4 pTranslateM = Matrix4.CreateTranslation(X + 0.5f, Y + 0.5f, Const.EntityLayer);
            Matrix4 pScaleM = Matrix4.CreateScale((float)Type.Scale);
            Matrix4 pRotateM = Matrix4.CreateRotationZ((float)((Facing+180)*Math.PI/180));
            prog.SetUniform("model", pRotateM * pScaleM * pTranslateM);
            prog.SetUniform("flatColour", new Vector4(1f, 1f, 1f, 1f));
            GL.UseProgram(prog.ShaderProgramHandle);
            Square.TexturedCentred.BindAndDraw();
            prog.SetUniform("textureEnabled", false);

            if (bLabel) {
                TextRenderOptions tro = new TextRenderOptions() {
                    Alignment = Alignment.BottomMiddle,
                    Aspect = 1f,
                    TextColour = Color.White,
                    XPos = X + 0.5f,
                    YPos = Y - 0f,
                    ZPos = 0.02f,
                    Scale = 0.35f,
                    FlipY = true,
                    Projection = Matrix4.CreatePerspectiveFieldOfView(Const.MapViewportAngle, aspect, 0.05f, 5000.0f),
                    View = viewM
                };
                TextRenderer.DrawWithOptions(Name, tro);
                GL.Translate(X + ((float)Type.Size / 2), Y - (0.015 * fViewHeight), Const.GUILayer);
                double lScale = fViewHeight / 50.0;
                GL.Scale(lScale, lScale, lScale);
                TextRenderer.Draw(Name, Alignment.BottomMiddle);
            }
            if (bStatBars) {
                prog.SetUniform("textureEnabled", false);
                GraphicsFunctions.DisplayBicolourFractBar(prog, X, Y - 0.1f, 1.0f, 0.09f, (float)(Health / MaxHealth), new Vector4(0.3f, 1f, 0.3f, 1f), new Vector4(1f, 0f, 0f, 1f));
                GraphicsFunctions.DisplayBicolourFractBar(prog, X, Y - 0.25f, 1.0f, 0.09f, (float)(Stamina / MaxStamina), new Vector4(1f, 1f, 1f, 1f), new Vector4(0.6f, 0.6f, 0.6f, 1f));
                if (MaxShields > 0) GraphicsFunctions.DisplayBicolourFractBar(prog, X, Y - 0.4f, 1.0f, 0.09f, (float)(Shields / MaxShields), new Vector4(0.2f, 0.5f, 1f, 1f), new Vector4(0.2f, 0.2f, 0.2f, 1f));
            }
            if (bShowEffects && _Effects.Any()) {
                //GL.Translate(X + (float)Type.Size, Y + ((float)Type.Size * 0.25f), Const.EntityLayer);
                //GL.Scale(Type.Scale, Type.Scale, 1.0);
                int nEffects = _Effects.Count;
                if (nEffects > 4) nEffects = 4;
                pScaleM = Matrix4.CreateScale(0.08f);
                Matrix4 pbScaleM = Matrix4.CreateScale(0.1f);
                pRotateM = Matrix4.CreateRotationZ((float)Math.PI / 4f);
                for (int i = 0; i < nEffects; i++) {
                    pTranslateM = Matrix4.CreateTranslation(X + 0.94f, Y + (i * 0.2f) - 0.01f, Const.EntityLayer);
                    prog.SetUniform("model", pRotateM * pbScaleM * pTranslateM);
                    prog.SetUniform("flatColour", new Vector4(0.5f, 0.2f, 0.2f, 0.5f));
                    GL.UseProgram(prog.ShaderProgramHandle);
                    Square.Flat.BindAndDraw();
                    pTranslateM = Matrix4.CreateTranslation(X + 0.95f, Y + (i * 0.2f), Const.EntityLayer);
                    prog.SetUniform("model", pRotateM * pScaleM * pTranslateM);
                    prog.SetUniform("flatColour", new Vector4(1f, 0f, 0f, 1f));
                    GL.UseProgram(prog.ShaderProgramHandle);
                    Square.Flat.BindAndDraw();
                }
                if (_Effects.Count > 4) {
                    pTranslateM = Matrix4.CreateTranslation(X + 0.91f, Y + 0.8f, Const.EntityLayer);
                    prog.SetUniform("model", pScaleM * pTranslateM);
                    prog.SetUniform("flatColour", new Vector4(1f, 0f, 0f, 1f));
                    GL.UseProgram(prog.ShaderProgramHandle);
                    Cross.Flat.BindAndDraw();
                }
            }
        }
        public void ResetForBattle() {
            Health = MaxHealth;
            Stamina = MaxStamina;
            Shields = MaxShields;
            Random rand = new Random(X + Y * 1000);
            Facing = rand.Next(360);
            CurrentTarget = null;
        }
        public bool CanOpenDoors { get { return Type.Interact; } }
        public double RangeTo(IEntity en) {
            int dx = (en.X > X) ? en.X - (X + Size - 1) : X - (en.X + en.Size - 1);
            if (dx < 0) dx = 0;
            int dy = (en.Y > Y) ? en.Y - (Y + Size - 1) : Y - (en.Y + en.Size - 1);
            if (dy < 0) dy = 0;
            return Math.Sqrt((dx * dx) + (dy * dy));
        }
        public double RangeTo(int tx, int ty) {
            int dx = (tx > X) ? tx - X : X - tx;
            if (dx < 0) dx = 0;
            int dy = (ty > Y) ? ty - Y : Y - ty;
            if (dy < 0) dy = 0;
            return Math.Sqrt((dx * dx) + (dy * dy));
        }
        public double RangeTo(Point pt) {
            return RangeTo(pt.X, pt.Y);
        }
        public double BaseArmour { get { return Type.ArmourBase * (1.0 + Const.CreatureLevelArmourStep * (Level - 1)); } }
        public double GetDamageReductionByDamageType(WeaponType.DamageType type) {
            double red = 100.0;
            if (Type.Resistances.ContainsKey(type)) red -= Type.Resistances[type];
            return Utils.ArmourReduction(BaseArmour) * red / 100.0;
        }
        public double InflictDamage(Dictionary<WeaponType.DamageType, double> AllDam) {
            if (!AllDam.Any()) return 0.0;

            // Shields? Reduce only physical damage
            if (Shields > 0.0) {
                double PhysDam = 0.0;
                if (AllDam.ContainsKey(WeaponType.DamageType.Physical)) PhysDam = AllDam[WeaponType.DamageType.Physical];
                if (PhysDam > 0.0) {
                    if (Shields > PhysDam) {
                        Shields -= PhysDam;
                        AllDam.Remove(WeaponType.DamageType.Physical);
                    }
                    else {
                        AllDam[WeaponType.DamageType.Physical] -= Shields;
                        Shields = 0.0;
                    }
                }
            }

            // Loop through all damage types and calculate all their combined effects
            double TotalDam = 0.0;
            foreach (WeaponType.DamageType type in AllDam.Keys) {
                double dam = AllDam[type];

                // Armour reduces damage
                TotalDam += dam * GetDamageReductionByDamageType(type);
            }

            // Do the damage
            Health -= TotalDam;

            // Is the creature dead?
            if (Health <= 0.0) KillEntity();
            return TotalDam;
        }
        public Stash GenerateStash() {
            Stash st = new Stash(Location);

            // Add corpse
            if (Type.Corporeal) {
                Corpse cp = new Corpse(this);
                st.Add(cp);
            }

            if (EquippedWeapon != null && EquippedWeapon.Type.IsUsable && rnd.NextDouble() < 0.5) st.Add(EquippedWeapon); // Weapon has 50% chance of surviving intact

            // Generate other dropped items
            if (OverrideRace != null) {
                double dnum = rnd.NextDouble();
                double lfrac = (double)(Level - Type.LevelMin) / (double)(Type.LevelMax - Type.LevelMin);
                dnum += rnd.NextDouble() * lfrac;
                int num = (int)Math.Round(dnum);
                for (int n = 0; n < num; n++) {
                    // Generate a random item suitable for this creature
                    IItem? eq = Utils.GenerateRandomItem(rnd, this.Level);
                    if (eq is not null) st.Add(eq);
                }
            }

            return st;
        }
        public void KillEntity() {
            Health = 0.0;
            CurrentLevel.KillCreature(this);
        }
        public Dictionary<WeaponType.DamageType, double> GenerateDamage() {
            Dictionary<WeaponType.DamageType, double> AllDam = new Dictionary<WeaponType.DamageType, double>();
            if (EquippedWeapon == null) {
                double dam = rnd.NextDouble() * (Level + 5) + Level + 5;
                dam *= Const.CreatureMeleeDamageScale * Const.AttackDamageScale;
                AllDam.Add(WeaponType.DamageType.Physical, dam);
            }
            else {
                double dam = rnd.NextDouble() * EquippedWeapon.DMod + EquippedWeapon.DBase;
                double hmod = 0.9 + (Attack / 10.0);
                hmod *= Const.AttackDamageScale;
                //if (EquippedWeapon.Type.IsMeleeWeapon) dam *= (Level + 4) / 5.0;
                AllDam.Add(EquippedWeapon.Type.DType, dam * hmod);
                foreach (KeyValuePair<WeaponType.DamageType, double> bdam in EquippedWeapon.GetBonusDamage()) {
                    if (AllDam.ContainsKey(bdam.Key)) AllDam[bdam.Key] += bdam.Value * hmod;
                    else AllDam.Add(bdam.Key, bdam.Value * hmod);
                }
            }

            return AllDam;
        }
        public void ApplyEffectToEntity(IEntity src, ItemEffect ie, VisualEffect.EffectFactory fact) {
            Dictionary<WeaponType.DamageType, double> AllDam = new Dictionary<WeaponType.DamageType, double>();
            foreach (Effect eff in ie.Effects) {
                if (eff.Duration == 0) {
                    // Do the action now, whatever it is
                    double dmod = 1.0;
                    if (ie.AssociatedSkill != Soldier.UtilitySkill.Unspent && src is Soldier s) {
                        int sk = s.GetUtilityLevel(ie.AssociatedSkill);
                        if (ie.SkillRequired && sk == 0) throw new Exception("Attempting to perform unskilled application of effect");
                        if (sk == 0) dmod /= 2.0; // Unskilled use
                        else dmod += Math.Pow(sk - 1, 1.5) / 10.0;
                    }
                    if (AllDam.ContainsKey(eff.DamageType)) AllDam[eff.DamageType] += eff.Damage * dmod;
                    else AllDam.Add(eff.DamageType, eff.Damage * dmod);

                }
                else {
                    _Effects.Add(new Effect(eff));
                }
            }
            float TotalDam = (float)InflictDamage(AllDam);
            if (TotalDam > 0.0) fact(VisualEffect.EffectType.Damage, X + (Size / 2f), Y + (Size / 2f), new Dictionary<string, object>() { { "Value", TotalDam } });
            else if (TotalDam < 0.0) fact(VisualEffect.EffectType.Healing, X + (Size / 2f), Y + (Size / 2f), new Dictionary<string, object>() { { "Value", -TotalDam } });
        }

        // Creature-specific
        public CreatureType Type { get; private set; }
        public Race? OverrideRace { get; private set; }  // If this is a humanoid then create a race-specific version (if the Type can be overridden)
        public IEntity? CurrentTarget { get; private set; }
        private double MovementCost { get { return Type.MovementCost / SpeedModifier(); } }
        public double AttackCost { get { if (EquippedWeapon == null) return Const.MeleeCost; return EquippedWeapon.StaminaCost; } }
        private readonly MissionLevel CurrentLevel;
        private readonly Random rnd = new Random();
        public int Experience {
            get {
                double exp = Math.Pow(1.1, Level - 1) * Const.CreatureExperienceScale;
                exp *= (Stamina + Attack + Defence + Health + Shields + BaseArmour);
                foreach (double res in Type.Resistances.Values) exp *= (100.0 + res) / 100.0;
                if (Type.IsBoss) exp *= 1.5;
                return (int)exp;
            }
        }
        public readonly int TX = -1, TY = -1;
        public int StatBonuses(StatType st) {
            int bonus = 0;
            foreach (Effect e in Effects) {
                bonus += e.GetStatMod(st);
            }
            return bonus;
        }
        private double SpeedModifier() {
            double mod = 1.0;
            foreach (Effect e in Effects) {
                mod *= e.SpeedMod;
            }
            return mod;
        }
        public bool IsAlert { get; private set; }
        public bool QuestItem { get; private set; }

        // Constructors
        public Creature(CreatureType ct, int lvl, MissionLevel lev, Race? Override = null) {
            X = -1;
            Y = -1;
            Type = ct;
            Level = lvl;
            CurrentLevel = lev;
            Health = MaxHealth;
            Stamina = MaxStamina;
            Shields = MaxShields;
            OverrideRace = Override;
            EquippedWeapon = Type.GenerateRandomWeapon();
            CurrentTarget = null;
            Investigate = Point.Empty;
            TX = -1;
            TY = -1;
            QuestItem = false;
        }
        public Creature(XmlNode xml, MissionLevel lev) {
            CurrentLevel = lev;
            string strName = xml.Attributes["Type"].Value ?? string.Empty;
            Type = StaticData.GetCreatureTypeByName(strName) ?? throw new Exception("Could not ID Type for Creature : " + strName);

            XmlNode? xmll = xml.SelectSingleNode("Location") ?? throw new Exception("Could not ID Location for Creature : " + strName);
            X = int.Parse(xmll.Attributes["X"].Value);
            Y = int.Parse(xmll.Attributes["Y"].Value);
            Level = int.Parse(xml.SelectSingleNode("Level").InnerText);
            if (double.TryParse(xml.SelectSingleNode("Facing").InnerText, out double fac)) {
                Facing = fac;
            }
            else {
                SetFacing((Utils.Direction)Enum.Parse(typeof(Utils.Direction), xml.SelectSingleNode("Facing").InnerText));
            }
            if (xml.SelectSingleNode("Health") != null) Health = Double.Parse(xml.SelectSingleNode("Health").InnerText);
            else Health = MaxHealth;
            if (xml.SelectSingleNode("Stamina") != null) Stamina = Double.Parse(xml.SelectSingleNode("Stamina").InnerText);
            else Stamina = MaxStamina;
            if (xml.SelectSingleNode("Shields") != null) Shields = Double.Parse(xml.SelectSingleNode("Shields").InnerText);
            else Shields = MaxShields;

            if (xml.SelectSingleNode("OverrideRace") != null) OverrideRace = StaticData.GetRaceByName(xml.SelectSingleNode("OverrideRace").InnerText);

            IsAlert = (xml.SelectSingleNode("Alert") != null);
            QuestItem = (xml.SelectSingleNode("QuestItem") != null);

            // Load equipped weapon
            if (xml.SelectSingleNode("Weapon") != null) {
                WeaponType tp = StaticData.GetWeaponTypeByName(xml.SelectSingleNode("Weapon").InnerText) ?? throw new Exception("Failed to load creature " + Name + " : Unknown weapon type " + xml.SelectSingleNode("Weapon").InnerText);
                EquippedWeapon = new Weapon(tp, 0) ?? throw new Exception("Failed to load creature " + Name + " : Unknown weapon type " + xml.SelectSingleNode("Weapon").InnerText);
            }
            if (EquippedWeapon is null) {
                if (Type.Weapons.Count == 1)  EquippedWeapon = Type.GenerateRandomWeapon();
                else if (Type.Weapons.Count > 1) throw new Exception("Missing ambiguous weapon in creature : " + Name);
            }

            // Current target
            XmlNode? xnt = xml.SelectSingleNode("Target");
            if (xnt is not null) {
                TX = int.Parse(xnt.Attributes["X"].Value);
                TY = int.Parse(xnt.Attributes["Y"].Value);
            }
            else {
                TX = -1;
                TY = -1;
            }
            XmlNode? xni = xml.SelectSingleNode("Investigate");
            if (xni is not null) {
                Investigate = new Point(int.Parse(xni.Attributes["X"].Value), int.Parse(xni.Attributes["Y"].Value));
            }
            else Investigate = Point.Empty;

            // Effects
            XmlNode? xmlef = xml.SelectSingleNode("Effects");
            _Effects.Clear();
            if (xmlef is not null) {
                foreach (XmlNode xef in xmlef.ChildNodes) {
                    Effect e = new Effect(xef);
                    _Effects.Add(e);
                }
            }
        }

        public void SaveToFile(StreamWriter file) {
            file.WriteLine("<Creature Type=\"" + Type.Name + "\">");
            file.WriteLine(" <Location X=\"" + X + "\" Y=\"" + Y + "\"/>");
            if (Health != MaxHealth) file.WriteLine(" <Health>" + Health.ToString("N2") + "</Health>");
            if (Stamina != MaxStamina) file.WriteLine(" <Stamina>" + Stamina.ToString("N2") + "</Stamina>");
            if (Shields != MaxShields) file.WriteLine(" <Shields>" + Shields.ToString("N2") + "</Shields>");
            file.WriteLine(" <Level>" + Level + "</Level>");
            file.WriteLine(" <Facing>" + Facing + "</Facing>");
            if (OverrideRace != null) file.WriteLine(" <OverrideRace>" + OverrideRace.Name + "</OverrideRace>");
            if (EquippedWeapon != null && Type.Weapons.Count > 1) file.WriteLine(" <Weapon>" + EquippedWeapon.Type.Name + "</Weapon>"); // Save only if ambiguous
            if (CurrentTarget != null) {
                file.WriteLine(" <Target X=\"" + CurrentTarget.X + "\" Y=\"" + CurrentTarget.Y + "\"/>");
            }
            if (Investigate != Point.Empty) file.WriteLine(" <Investigate X=\"" + Investigate.X + "\" Y=\"" + Investigate.Y + "\"/>");
            if (_Effects.Count > 0) {
                file.WriteLine(" <Effects>");
                foreach (Effect e in Effects) {
                    e.SaveToFile(file);
                }
                file.WriteLine(" </Effects>");
            }
            if (IsAlert) file.WriteLine(" <Alert/>");
            if (QuestItem) file.WriteLine(" <QuestItem/>");
            file.WriteLine("</Creature>");
        }

        // Actions
        public void Move(Utils.Direction d, Action<string> playSound) {
            if (Stamina < MovementCost) return;
            SetFacing(d);
            int oldx = X, oldy = Y;
            if (d == Utils.Direction.West && X > 0) {
                if ((CurrentLevel.Map[X - 1, Y] == MissionLevel.TileType.DoorHorizontal || CurrentLevel.Map[X - 1, Y] == MissionLevel.TileType.DoorVertical) && CanOpenDoors) {
                    CurrentLevel.OpenDoor(X - 1, Y);
                    if (CurrentLevel.Visible[X - 1, Y]) playSound("OpenDoor");
                }
                for (int i = 0; i < Size; i++) {
                    if (MissionLevel.IsObstruction(CurrentLevel.Map[X - 1, Y + i]) || CurrentLevel.GetEntityAt(X - 1, Y + i) != null) return;
                }
                CurrentLevel.MoveEntityTo(this, new Point(X - 1, Y));
            }
            if (d == Utils.Direction.East && X < CurrentLevel.Width - Size) {
                if ((CurrentLevel.Map[X + 1, Y] == MissionLevel.TileType.DoorHorizontal || CurrentLevel.Map[X + 1, Y] == MissionLevel.TileType.DoorVertical) && CanOpenDoors) {
                    CurrentLevel.OpenDoor(X + 1, Y);
                    if (CurrentLevel.Visible[X + 1, Y]) playSound("OpenDoor");
                }
                for (int i = 0; i < Size; i++) {
                    if (MissionLevel.IsObstruction(CurrentLevel.Map[X + Size, Y + i]) || CurrentLevel.GetEntityAt(X + Size, Y + i) != null) return;
                }
                CurrentLevel.MoveEntityTo(this, new Point(X + 1, Y));
            }
            if (d == Utils.Direction.North && Y < CurrentLevel.Height - Size) {
                if ((CurrentLevel.Map[X, Y + 1] == MissionLevel.TileType.DoorHorizontal || CurrentLevel.Map[X, Y + 1] == MissionLevel.TileType.DoorVertical) && CanOpenDoors) {
                    CurrentLevel.OpenDoor(X, Y + 1);
                    if (CurrentLevel.Visible[X, Y + 1]) playSound("OpenDoor");
                }
                for (int i = 0; i < Size; i++) {
                    if (MissionLevel.IsObstruction(CurrentLevel.Map[X + i, Y + Size]) || CurrentLevel.GetEntityAt(X + i, Y + Size) != null) return;
                }
                CurrentLevel.MoveEntityTo(this, new Point(X, Y + 1));
            }
            if (d == Utils.Direction.South && Y > 0) {
                if ((CurrentLevel.Map[X, Y - 1] == MissionLevel.TileType.DoorHorizontal || CurrentLevel.Map[X, Y - 1] == MissionLevel.TileType.DoorVertical) && CanOpenDoors) {
                    CurrentLevel.OpenDoor(X, Y - 1);
                    if (CurrentLevel.Visible[X, Y - 1]) playSound("OpenDoor");
                }
                for (int i = 0; i < Size; i++) {
                    if (MissionLevel.IsObstruction(CurrentLevel.Map[X + i, Y - 1]) || CurrentLevel.GetEntityAt(X + i, Y - 1) != null) return;
                }
                CurrentLevel.MoveEntityTo(this, new Point(X, Y - 1));
            }
            if (oldx != X || oldy != Y) {
                Stamina -= MovementCost;
            }
        }
        private void MoveTo(Point pt, Action<string> playSound) { // Should be adjacent
            if (pt.X == X && pt.Y == Y) return; // Weird - no move
            if (pt.X == X && pt.Y == Y - 1) Move(Utils.Direction.South, playSound);
            else if (pt.X == X && pt.Y == Y + 1) Move(Utils.Direction.North, playSound);
            else if (pt.X == X - 1 && pt.Y == Y) Move(Utils.Direction.West, playSound);
            else if (pt.X == X + 1 && pt.Y == Y) Move(Utils.Direction.East, playSound);
            else throw new Exception("Cannot move to non-adjacent point in one step");
        }
        public void AttackEntity(IEntity en, VisualEffect.EffectFactory effectFactory, Action<string> playSound) {
            if (en == null) return;
            if (RangeTo(en) > AttackRange) return;
            Stamina -= AttackCost;

            // Rotate creature
            float dx = X - en.X;
            float dy = Y - en.Y;
            SetFacing(180.0 + Math.Atan2(dy, dx) * (180.0 / Math.PI));
            // TODO refreshView();
            Thread.Sleep(100);

            // Play weapon sound & draw shot
            if (EquippedWeapon == null) playSound("Punches");
            else playSound(EquippedWeapon.Type.SoundEffect);
            if (EquippedWeapon != null && !EquippedWeapon.Type.IsMeleeWeapon) {
                float pow = (float)(EquippedWeapon.DBase + (EquippedWeapon.DMod / 2.0));
                effectFactory(VisualEffect.EffectType.Shot, X, Y, new Dictionary<string, object>() { { "FX", X + 0.5f }, { "TX", en.X + 0.5f }, { "FY", Y + 0.5f }, { "TY", en.Y + 0.5f }, { "Power", pow }, { "Colour", Color.FromArgb(255, 200, 200, 200) } });
            }

            // Do the attack
            double hit = Utils.GenerateHitRoll(this, en);
            if (hit <= 0.0) return;
            double TotalDam = en.InflictDamage(GenerateDamage());

            // Graphics for damage
            int delay = (int)(RangeTo(en) * 25.0);
            if (EquippedWeapon == null || EquippedWeapon.Type.IsMeleeWeapon) delay += 250;
            Thread.Sleep(delay);
            effectFactory(VisualEffect.EffectType.Damage, en.X + (en.Size / 2f), en.Y + (en.Size / 2f), new Dictionary<string, object>() { { "Value", TotalDam } });

            // Play sound
            if (EquippedWeapon != null && EquippedWeapon.Type.Area == 0) playSound("Smash");

            // Apply effect?
            if (EquippedWeapon != null) {
                if (EquippedWeapon.Type.ItemEffect != null) {
                    en.ApplyEffectToEntity(this, EquippedWeapon.Type.ItemEffect, effectFactory);
                }
            }
        }
        public void AIStep(VisualEffect.EffectFactory fact, Action<IEntity> postMoveCheck, Action<string> playSound, Action<IEntity> centreView) {
            int nsteps = 0;
            bool bFleeing = false;
            if (!IsAlert) return;
            // Flee if really damaged?
            if (Health * 4.0 < MaxHealth && CurrentTarget != null && Health <= CurrentTarget.Health && Level <= CurrentTarget.Level && !Type.IsBoss) {
                if (rnd.NextDouble() * Health / MaxHealth < 0.1) {
                    // Flee
                    bFleeing = true;
                    while (Stamina >= MovementCost && ++nsteps < 20 && rnd.NextDouble() * Health / MaxHealth < 0.2) {
                        int oldx = X, oldy = Y;
                        MoveAwayFrom(CurrentTarget, playSound);
                        if (X != oldx || Y != oldy) {
                            postMoveCheck(this);
                            // TODO refreshView();
                            if (CurrentLevel.Visible[X, Y]) Thread.Sleep(Const.AITickSpeed);
                        }
                    }
                    CurrentTarget = null;
                }
            }
            do {
                // No target, or current target is not visible, so see if we can find another
                if (CurrentTarget == null) {// || !CanSee(CurrentTarget.X, CurrentTarget.Y)) {
                    SetBestTarget();
                }
                // Do we have a target? If so then behave appropriately
                if (CurrentTarget != null && rnd.NextDouble() < 0.1) {
                    IEntity lastTarg = CurrentTarget;
                    SetBestTarget(); // Occasionally ensure we're on the right target
                    if (CurrentTarget == null && lastTarg.Health > 0.0) CurrentTarget = lastTarg; // Maybe that we lost sight of them because we're pathing a complex route. Stay on track.
                }
                if (CurrentTarget != null) {
                    double atr = AttackRange;
                    double r = RangeTo(CurrentTarget);
                    // Can we attack? If not then maybe move a bit closer for a better shot?
                    if (r <= atr) {
                        if (!CanSee(CurrentTarget)) {
                            List<Point>? path = CurrentLevel.ShortestPath(this, Location, CurrentTarget.Location, 20, false);
                            if (path is null || path.Count == 0) {
                                CurrentTarget = null; // No way of getting close enough to see target
                                return;
                            }
                            MoveTo(path[0], playSound);
                            postMoveCheck(this);
                            // TODO refreshView();
                            if (CurrentLevel.Visible[X, Y]) Thread.Sleep(Const.AITickSpeed);
                        }
                        else if (Stamina < AttackCost) {
                            // Optionally move closer?
                            if (r > 5.0 && Stamina >= MovementCost && r > 1.0 && r > atr * 0.8 && rnd.NextDouble() < 0.3 && !bFleeing) {
                                List<Point>? path = CurrentLevel.ShortestPath(this, Location, CurrentTarget.Location, 10, false, (int)Math.Floor(atr));
                                if (path is null || path.Count == 0) return; // Could be ok - path to target is blocked but can still attack from range. Or else target is adjacent.
                                MoveTo(path[0], playSound);
                                postMoveCheck(this);
                                // TODO refreshView();
                                if (CurrentLevel.Visible[X, Y]) Thread.Sleep(Const.AITickSpeed);
                            }
                            return;
                        }
                        else {
                            // Do the attack
                            centreView(this);
                            Thread.Sleep(250);
                            AttackEntity(CurrentTarget, fact, playSound);
                            //refreshView();
                            Thread.Sleep(Const.AITickSpeed);
                        }
                    }
                    else {
                        if (bFleeing) return;
                        // Close the distance
                        if (Stamina < MovementCost) return;
                        List<Point>? path = CurrentLevel.ShortestPath(this, Location, CurrentTarget.Location, 50, false, (int)Math.Floor(atr));
                        if (path is null || path.Count == 0) CurrentTarget = null; // No way of getting close enough to hit target
                        else {
                            MoveTo(path[0], playSound);
                            postMoveCheck(this);
                            // TODO refreshView();
                            if (CurrentLevel.Visible[X, Y]) Thread.Sleep(Const.AITickSpeed);
                        }
                    }
                }
                // No target and couldn't find one. Maybe investigate nearby if we heard something
                else if (Investigate != Point.Empty) {
                    if (Math.Abs(Investigate.X - X) < Size && Math.Abs(Investigate.Y - Y) < Size) {
                        Investigate = Point.Empty;
                        return;
                    }
                    if (Stamina < MovementCost) return;
                    List<Point>? path = CurrentLevel.ShortestPath(this, Location, Investigate, 30, false, 1); // Go to this square or nearby
                    if (path is null || path.Count == 0) {
                        Investigate = Point.Empty;
                        return;
                    }
                    else {
                        MoveTo(path[0], playSound);
                        postMoveCheck(this);
                        // TODO refreshView();
                        if (CurrentLevel.Visible[X, Y]) Thread.Sleep(Const.AITickSpeed);
                    }
                }
                else {
                    IsAlert = false;
                    return; // no target, no hope of finding one, just give up
                }
            } while (++nsteps < 20 && Stamina >= MovementCost);
        }
        private void SetBestTarget() {
            double bestscore = -10000.0;
            CurrentTarget = null;
            foreach (Soldier s in CurrentLevel.Soldiers) {
                if (CanSee(s)) {
                    double score = 100.0 / RangeTo(s);
                    if (s == CurrentTarget) score += 5.0;
                    if (AttackRange < this.RangeTo(s)) {
                        List<Point>? path = CurrentLevel.ShortestPath(this, Location, s.Location, 50, false, (int)Math.Floor(AttackRange));
                        if (path is null) continue;
                        else score -= path.Count;
                    }
                    if (score > bestscore) {
                        bestscore = score;
                        SetTarget(s);
                    }
                }
            }
        }
        private void MoveAwayFrom(IEntity en, Action<string> playSound) {
            if (en == null) return;
            Utils.Direction d1, d2;
            if (en.X > X) d1 = Utils.Direction.West;
            else if (en.X < X) d1 = Utils.Direction.East;
            else if (rnd.NextDouble() < 0.5) d1 = Utils.Direction.West;
            else d1 = Utils.Direction.East;
            if (en.Y > Y) d2 = Utils.Direction.South;
            else if (en.Y < Y) d2 = Utils.Direction.North;
            else if (rnd.NextDouble() < 0.5) d2 = Utils.Direction.North;
            else d2 = Utils.Direction.South;
            if (rnd.NextDouble() < 0.5) Move(d1, playSound);
            else Move(d2, playSound);
        }
        public void EndOfTurn(VisualEffect.EffectFactory fact, Action<IEntity> centreView, Action<string> playSound, Action<string> showMessage) {
            Stamina = MaxStamina;

            // Handle periodic effects
            foreach (Effect e in Effects) {
                bool bZoom = (!String.IsNullOrEmpty(e.SoundEffect) || e.Damage != 0.0);
                if (bZoom) {
                    // Zoom to this creature & redraw
                    centreView(this);
                    Thread.Sleep(250);
                }
                if (!String.IsNullOrEmpty(e.SoundEffect)) playSound(e.SoundEffect);
                if (e.Damage != 0.0) {
                    Dictionary<WeaponType.DamageType, double> AllDam = new Dictionary<WeaponType.DamageType, double> { { e.DamageType, e.Damage } };
                    double TotalDam = InflictDamage(AllDam);
                    fact(VisualEffect.EffectType.Damage, X + (Size / 2f), Y + (Size / 2f), new Dictionary<string, object>() { { "Value", TotalDam } });
                    if (TotalDam > 0.0) playSound("Grunt");
                    if (Health <= 0.0) break; // If this effect killed this creature, stop here
                }
                if (bZoom) Thread.Sleep(750);
                e.ReduceDuration(1);
            }
            _Effects.RemoveAll(e => e.Duration <= 0);
            if (Health < 0.0) _Effects.Clear();
        }
        public void SetTarget(IEntity? tg) {
            CurrentTarget = tg;
            if (tg == null) Investigate = Point.Empty;
            else Investigate = tg.Location;
        }
        public void SetTargetInvestigation(int x, int y) {
            if (x < 0 || y < 0) Investigate = Point.Empty;
            else Investigate = new Point(x, y);
        }
        public void SetHasQuestItem() {
            QuestItem = true;
        }
        public void Alert() {
            IsAlert = true;
        }
        public void CheckChangeTarget(double totalDam, Soldier attacker) {
            if (CurrentTarget == attacker) return;
            if (attacker == null) return;
            if (CurrentTarget == null) {
                SetTarget(attacker);
                return;
            }
            double dCur = RangeTo(CurrentTarget);
            double dNew = RangeTo(attacker);
            if (dCur * (4.0 + rnd.NextDouble()) * totalDam / MaxHealth > dNew) { // Swap if new attacker is nearer, or if they did a lot of damage
                SetTarget(attacker);
            }
        }
        public double SoldierVisibilityRange(Soldier s) {
            return s.DetectionRange + ((Level - s.Level) / 3.0);
        }

    }
}