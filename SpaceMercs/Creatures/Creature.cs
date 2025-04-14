using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using SpaceMercs.Graphics;
using SpaceMercs.Graphics.Shapes;
using System.IO;
using System.Security.Cryptography;
using System.Xml;
using static SpaceMercs.Delegates;

namespace SpaceMercs {
    public class Creature : IEntity {
        // IEntity stuff
        public int X { get; private set; }
        public int Y { get; private set; }
        public int Level { get; private set; }
        public double Health { get; private set; }
        public double MaxHealth { get { return Type.HealthBase * (1.0 + Const.CreatureLevelHealthScale * (Level - 1)) + (Const.CreatureLevelHealthStep * (Level - 1)); } }
        public double Stamina { get; private set; }
        public double MaxStamina { get { return Type.StaminaBase + ((Level - 1) * Const.CreatureLevelStaminaStep) + StatBonuses(StatType.Stamina); } }
        public double Shields { get; private set; }
        public double MaxShields { get { return Type.ShieldsBase * (1.0 + Const.CreatureLevelShieldsScale * (Level - 1)); } }
        public double Attack { get { return Type.AttackBase * (1.0 + Const.CreatureLevelAttackScale * (Level - 1)) + (Const.CreatureLevelAttackStep * (Level - 1)) + StatBonuses(StatType.Attack); } }
        public double Defence { get { return Type.DefenceBase * (1.0 + Const.CreatureLevelDefenceScale * (Level - 1)) + (Const.CreatureLevelDefenceStep * (Level - 1)) + StatBonuses(StatType.Defence); } }
        public int TravelRange { get { return (int)Stamina; } }
        public Weapon? EquippedWeapon { get; private set; }
        public double AttackRange { get { return (EquippedWeapon == null) ? 1.0 : EquippedWeapon.Range; } }
        public string Name { get { if (OverrideRace != null) return OverrideRace.Name + " " + Type.Name; else return Type.Name; } }
        public int Size { get { return Type.Size; } }
        public double Facing { get; set; }
        public double Shred { get; private set; }
        public int Searching { get; private set; } // Number of turns spent searching
        public Point Location { get { return new Point(X, Y); } }
        public Point Investigate { get; set; } = Point.Empty;
        public Point HidingPlace { get; set; } = Point.Empty;
        private readonly List<Effect> _Effects = new List<Effect>();
        public IEnumerable<Effect> Effects { get { return _Effects.AsReadOnly(); } }
        private bool[,] Visible;
        public bool HasMoved { get; private set; } = false;

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
        private TexSpecs GetTexture() {
            bool bShields = Shields > 0.001d;
            return Type.GetTexture(bShields);
        }

        public void Display(ShaderProgram prog, bool bLabel, bool bStatBars, bool bShowEffects, float fViewHeight, float aspect, Matrix4 viewM) {
            TexSpecs ts = GetTexture();
            GL.BindTexture(TextureTarget.Texture2D, ts.ID);
            GL.Enable(EnableCap.Blend);
            GL.Disable(EnableCap.DepthTest);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

            prog.SetUniform("textureEnabled", true);
            prog.SetUniform("texPos", ts.X, ts.Y);
            prog.SetUniform("texScale", ts.W, ts.H);
            Matrix4 pTranslateM = Matrix4.CreateTranslation(X + ((float)Type.Size / 2.0f), Y + ((float)Type.Size / 2.0f), Const.EntityLayer);
            Matrix4 pScaleM = Matrix4.CreateScale((float)Type.Scale);
            Matrix4 pRotateM = Matrix4.CreateRotationZ((float)((Facing - 90d) * Math.PI / 180d));
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
                    XPos = X + ((float)Type.Size / 2.0f),
                    YPos = Y - 0f,
                    ZPos = 0.02f,
                    Scale = 0.35f,
                    FlipY = true,
                    Projection = Matrix4.CreatePerspectiveFieldOfView(Const.MapViewportAngle, aspect, 0.05f, 5000.0f),
                    View = viewM
                };
                TextRenderer.DrawWithOptions(Name, tro);
            }
            if (bStatBars) {
                prog.SetUniform("textureEnabled", false);
                GraphicsFunctions.DisplayBicolourFractBar(prog, X, Y - 0.1f, (float)Type.Size, 0.09f, (float)(Health / MaxHealth), new Vector4(0.3f, 1f, 0.3f, 1f), new Vector4(1f, 0f, 0f, 1f));
                GraphicsFunctions.DisplayBicolourFractBar(prog, X, Y - 0.25f, (float)Type.Size, 0.09f, (float)(Stamina / MaxStamina), new Vector4(1f, 1f, 1f, 1f), new Vector4(0.6f, 0.6f, 0.6f, 1f));
                if (MaxShields > 0) GraphicsFunctions.DisplayBicolourFractBar(prog, X, Y - 0.4f, (float)Type.Size, 0.09f, (float)(Shields / MaxShields), new Vector4(0.2f, 0.5f, 1f, 1f), new Vector4(0.2f, 0.2f, 0.2f, 1f));
            }
            if (bShowEffects && _Effects.Any()) {
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
            Shred = 0d;
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
        public double BaseArmour { 
            get {
                double arm = Type.ArmourBase * (1.0 + Const.CreatureLevelArmourScale * (Level - 1)) + (Const.CreatureLevelArmourStep * (Level - 1));
                foreach (Effect eff in Effects) {
                    arm += eff.ArmourMod;
                }
                arm -= Shred;
                if (arm < 0.0) arm = 0.0;
                return arm;
            } 
        }
        public double GetDamageReductionByDamageType(WeaponType.DamageType type) {
            double red = 100.0;
            if (Type.Resistances.ContainsKey(type)) red -= Type.Resistances[type];
            return Utils.ArmourReduction(BaseArmour) * red / 100.0;
        }
        public double CalculateDamage(Dictionary<WeaponType.DamageType, double> AllDam) {
            return InflictDamage_Internal(AllDam, null, null, false);
        }
        public double InflictDamage(Dictionary<WeaponType.DamageType, double> AllDam, ItemEffect.ApplyItemEffect applyEffect, VisualEffect.EffectFactory? fact) {
            return InflictDamage_Internal(AllDam, applyEffect, fact, true);
        }
        private double InflictDamage_Internal(Dictionary<WeaponType.DamageType, double> AllDam, ItemEffect.ApplyItemEffect? applyEffect, VisualEffect.EffectFactory? fact, bool applyDamage) {
            if (!AllDam.Any() || Health <= 0.0) return 0.0;

            // Take a copy as we're modifying it
            Dictionary<WeaponType.DamageType, double> damInternal = new Dictionary<WeaponType.DamageType, double>(AllDam);

            // Shields? Reduce only physical damage
            if (Shields > 0.0) {
                double PhysDam = 0.0;
                if (damInternal.ContainsKey(WeaponType.DamageType.Physical)) PhysDam = damInternal[WeaponType.DamageType.Physical];
                if (PhysDam > 0.0) {
                    if (Shields > PhysDam) {
                        if (applyDamage) Shields -= PhysDam;
                        damInternal.Remove(WeaponType.DamageType.Physical);
                    }
                    else {
                        damInternal[WeaponType.DamageType.Physical] -= Shields;
                        if(applyDamage) Shields = 0.0;
                    }
                }
            }

            // Loop through all damage types and calculate all their combined effects
            double TotalDam = 0.0;
            foreach (WeaponType.DamageType type in damInternal.Keys) {
                double dam = damInternal[type];

                // Armour reduces damage
                TotalDam += dam * GetDamageReductionByDamageType(type);
            }

            // Sometimes we heal enemies if they have >100% resistance. Make sure we handle that properly.
            if (Health - TotalDam > MaxHealth) TotalDam = -(MaxHealth - Health);

            if (!applyDamage) return TotalDam;

            // Do the damage
            Health -= TotalDam;

            // Is the creature dead?
            if (Health <= 0.0) KillEntity(applyEffect!, fact!);
            return TotalDam;
        }
        public void ShredArmour(double shred) {
            Shred += shred;
        }
        public Stash GenerateStash() {
            Stash st = new Stash(Location);

            // Add corpse
            if (Type.Corporeal) {
                Corpse cp = new Corpse(this);
                st.Add(cp);
            }

            if (EquippedWeapon != null && EquippedWeapon.Type.IsUsable && rnd.NextDouble() > 0.4) {
                int lvl = EquippedWeapon.Level;
                // Degrade weapon a bit when dropped. use a different rng here because we were getting loads of really bad not-very-random behaviour otherwise
                while (RandomNumberGenerator.GetInt32(10) < 5) {
                    lvl--;
                }
                if (lvl >= 0) st.Add(new Weapon(EquippedWeapon.Type, lvl));
            }

            // Generate other dropped items
            if (OverrideRace != null) {
                double dnum = rnd.NextDouble();
                double lfrac = (double)(Level - Type.LevelMin) / (double)(Type.LevelMax - Type.LevelMin);
                dnum += rnd.NextDouble() * lfrac;
                int num = (int)Math.Round(dnum);
                for (int n = 0; n < num; n++) {
                    // Generate a random item suitable for this creature
                    IItem? eq = Utils.GenerateRandomItem(rnd, this.Level, OverrideRace, bIncludeWeapons:false);
                    if (eq is not null) st.Add(eq);
                }
            }

            return st;
        }
        public void KillEntity(ItemEffect.ApplyItemEffect applyEffect, VisualEffect.EffectFactory? fact) {
            Health = 0.0;
            CurrentLevel?.KillCreature(this, applyEffect, fact);
        }
        public Dictionary<WeaponType.DamageType, double> GenerateDamage() {
            Dictionary<WeaponType.DamageType, double> AllDam = new Dictionary<WeaponType.DamageType, double>();
            double hmod = Const.CreatureAttackDamageBaseMod + Attack * Const.CreatureAttackDamageScale;
            if (EquippedWeapon == null) {
                double dam = rnd.NextDouble() * (Level + 5) + Level + 5;
                dam *= Const.CreatureMeleeDamageScale * hmod;
                AllDam.Add(WeaponType.DamageType.Physical, dam);
            }
            else {
                double dam = EquippedWeapon.DBase + (rnd.NextDouble() * EquippedWeapon.DMod);
                AllDam.Add(EquippedWeapon.Type.DType, dam * hmod);
                foreach (KeyValuePair<WeaponType.DamageType, double> bdam in EquippedWeapon.GetBonusDamage()) {
                    if (AllDam.ContainsKey(bdam.Key)) AllDam[bdam.Key] += bdam.Value * hmod;
                    else AllDam.Add(bdam.Key, bdam.Value * hmod);
                }                
            }

            return AllDam;
        }
        public void ApplyEffectToEntity(IEntity? src, ItemEffect ie, VisualEffect.EffectFactory fact, ItemEffect.ApplyItemEffect applyEffect) {
            if (Health <= 0.0) return;
            Dictionary<WeaponType.DamageType, double> AllDam = new Dictionary<WeaponType.DamageType, double>();
            foreach (Effect eff in ie.Effects) {
                if (eff.Duration == 0) {
                    // Do the action now, whatever it is
                    double dmod = 1.0;
                    if (ie.AssociatedSkill != Soldier.UtilitySkill.Unspent && src is Soldier soldier) {
                        int sk = soldier.GetUtilityLevel(ie.AssociatedSkill);
                        if (ie.SkillRequired && sk == 0) throw new Exception("Attempting to perform unskilled application of effect");
                        if (sk == 0) dmod /= 2.0; // Unskilled use
                        else dmod += Math.Pow(sk - 1, 1.5) / 10.0;
                    }
                    if (AllDam.ContainsKey(eff.DamageType)) AllDam[eff.DamageType] += eff.Damage * dmod;
                    else AllDam.Add(eff.DamageType, eff.Damage * dmod);

                }
                else {
                    _Effects.Add(new Effect(eff, src is Soldier s ? s : null));
                }
            }
            double TotalDam = CalculateDamage(AllDam); 
            if (TotalDam > 0.0) fact(VisualEffect.EffectType.Damage, X + (Size / 2f), Y + (Size / 2f), new Dictionary<string, object>() { { "Value", (float)TotalDam } });
            else if (TotalDam < 0.0) fact(VisualEffect.EffectType.Healing, X + (Size / 2f), Y + (Size / 2f), new Dictionary<string, object>() { { "Value", -(float)TotalDam } });
            if (ie.CurePoison) {
                _Effects.RemoveAll(e => e.DamageType == WeaponType.DamageType.Poison);
            }
            if (ie.Shred > 0d) ShredArmour(ie.Shred);
            InflictDamage(AllDam, applyEffect, fact);
        }
        public bool IsInjured { get { return Health < MaxHealth; } }
        public double Encumbrance => 0.0;

        // Creature-specific
        public CreatureType Type { get; private set; }
        public Race? OverrideRace { get; private set; }  // If this is a humanoid then create a race-specific version (if the Type can be overridden)
        public IEntity? CurrentTarget { get; private set; }
        private double MovementCost { get { return Type.MovementCost / SpeedModifier(); } }
        public double AttackCost { get { if (EquippedWeapon == null) return Const.MeleeCost; return EquippedWeapon.StaminaCost; } }
        private readonly MissionLevel? CurrentLevel;
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
            EquippedWeapon = Type.GenerateRandomWeapon(lvl);
            CurrentTarget = null;
            Investigate = Point.Empty;
            HidingPlace = Point.Empty;
            TX = -1;
            TY = -1;
            QuestItem = false;
            Visible = new bool[0, 0];
            Shred = 0d;
        }
        public Creature(XmlNode xml, MissionLevel? lev) {
            CurrentLevel = lev;
            string strName = xml.GetAttributeText("Type", string.Empty);
            Type = StaticData.GetCreatureTypeByName(strName) ?? throw new Exception("Could not ID Type for Creature : " + strName);
            Level = xml.SelectNodeInt("Level");
            string strOverride = xml.SelectNodeText("OverrideRace");
            if (!string.IsNullOrEmpty(strOverride)) OverrideRace = StaticData.GetRaceByName(strOverride);

            // We're just saving this creature by description, e.g. reduced mode
            if (lev is null) return;

            XmlNode? xmll = xml.SelectSingleNode("Location") ?? throw new Exception("Could not ID Location for Creature : " + strName);
            X = xmll.GetAttributeInt("X");
            Y = xmll.GetAttributeInt("Y");
            string strFacing = xml.SelectNodeText("Facing");
            if (double.TryParse(strFacing, out double fac)) {
                Facing = fac;
            }
            else {
                SetFacing((Utils.Direction)Enum.Parse(typeof(Utils.Direction), strFacing));
            }
            Health = xml.SelectNodeDouble("Health", MaxHealth);
            Stamina = xml.SelectNodeDouble("Stamina", MaxStamina);
            Shields = xml.SelectNodeDouble("Shields", MaxShields);
            Shred = xml.SelectNodeDouble("Shred", 0d);
            Searching = xml.SelectNodeInt("Searching", 0);

            IsAlert = (xml.SelectSingleNode("Alert") is not null);
            QuestItem = (xml.SelectSingleNode("QuestItem") is not null);

            HasMoved = (xml.SelectSingleNode("Moved") != null);

            // Load equipped weapon
            string strWeapon = xml.SelectNodeText("Weapon");
            if (!string.IsNullOrEmpty(strWeapon)) {
                WeaponType tp = StaticData.GetWeaponTypeByName(strWeapon) ?? throw new Exception("Failed to load creature " + Name + " : Unknown weapon type " + strWeapon);
                EquippedWeapon = new Weapon(tp, 0) ?? throw new Exception("Failed to load creature " + Name + " : Unknown weapon type " + strWeapon);
            }
            if (EquippedWeapon is null) {
                if (Type.Weapons.Count == 1)  EquippedWeapon = Type.GenerateRandomWeapon(Level);
                else if (Type.Weapons.Count > 1) throw new Exception("Missing ambiguous weapon in creature : " + Name);
            }

            // Current target
            TX = TY = -1;
            XmlNode? xnt = xml.SelectSingleNode("Target");
            if (xnt is not null) {
                TX = xnt.GetAttributeInt("X");
                TY = xnt.GetAttributeInt("Y");
            }

            XmlNode? xni = xml.SelectSingleNode("Investigate");
            if (xni is not null) {
                int ix = xni.GetAttributeInt("X");
                int iy = xni.GetAttributeInt("Y");
                Investigate = new Point(ix, iy);
            }
            else Investigate = Point.Empty;

            XmlNode? xnh = xml.SelectSingleNode("HidingPlace");
            if (xnh is not null) {
                int hx = xnh.GetAttributeInt("X");
                int hy = xnh.GetAttributeInt("Y");
                HidingPlace = new Point(hx, hy);
            }
            else HidingPlace = Point.Empty;

            // Effects
            XmlNode? xmlef = xml.SelectSingleNode("Effects");
            _Effects.Clear();
            if (xmlef is not null) {
                foreach (XmlNode xef in xmlef.ChildNodes) {
                    Effect e = new Effect(xef);
                    _Effects.Add(e);
                }
            }

            Visible = new bool[0, 0];
        }

        public void SaveToFile(StreamWriter file, bool reducedMode = false) {
            file.WriteLine("<Creature Type=\"" + Type.Name + "\">");
            file.WriteLine(" <Level>" + Level + "</Level>");
            if (OverrideRace != null) file.WriteLine(" <OverrideRace>" + OverrideRace.Name + "</OverrideRace>");
            // We use reduced mode for storing a creature for e.g. the toughest kill stats, but not one that is actually alive in a mission
            if (!reducedMode) {
                file.WriteLine(" <Location X=\"" + X + "\" Y=\"" + Y + "\"/>");
                if (Health != MaxHealth) file.WriteLine(" <Health>" + Health.ToString("N2") + "</Health>");
                if (Stamina != MaxStamina) file.WriteLine(" <Stamina>" + Stamina.ToString("N2") + "</Stamina>");
                if (Shields != MaxShields) file.WriteLine(" <Shields>" + Shields.ToString("N2") + "</Shields>");
                if (Math.Abs(Shred) > 0.01) file.WriteLine($" <Shred>{Shred:N2}</Shred>");
                file.WriteLine(" <Facing>" + Facing + "</Facing>");
                if (EquippedWeapon != null && Type.Weapons.Count > 1) file.WriteLine(" <Weapon>" + EquippedWeapon.Type.Name + "</Weapon>"); // Save only if ambiguous
                if (CurrentTarget != null) {
                    file.WriteLine(" <Target X=\"" + CurrentTarget.X + "\" Y=\"" + CurrentTarget.Y + "\"/>");
                }
                if (Investigate != Point.Empty) file.WriteLine(" <Investigate X=\"" + Investigate.X + "\" Y=\"" + Investigate.Y + "\"/>");
                if (HidingPlace != Point.Empty) file.WriteLine(" <HidingPlace X=\"" + HidingPlace.X + "\" Y=\"" + HidingPlace.Y + "\"/>");
                if (_Effects.Count > 0) {
                    file.WriteLine(" <Effects>");
                    foreach (Effect e in Effects) {
                        e.SaveToFile(file);
                    }
                    file.WriteLine(" </Effects>");
                }
                if (IsAlert) file.WriteLine(" <Alert/>");
                if (QuestItem) file.WriteLine(" <QuestItem/>");
                if (HasMoved) file.WriteLine(" <Moved/>");
                if (Searching != 0) file.WriteLine($" <Searching>{Searching}</Searching>");
            }
            file.WriteLine("</Creature>");
        }

        // Actions
        public void Move(Utils.Direction d, PlaySoundDelegate playSound) {
            if (Stamina < MovementCost || CurrentLevel is null) return;
            SetFacing(d);
            int oldx = X, oldy = Y;
            if (d == Utils.Direction.West && X > 0) {
                if (CurrentLevel.Map[X - 1, Y] == MissionLevel.TileType.DoorVertical && CanOpenDoors) {
                    CurrentLevel.OpenDoor(X - 1, Y);
                    if (CurrentLevel.Visible[X - 1, Y]) playSound("OpenDoor");
                }
                for (int i = 0; i < Size; i++) {
                    if (MissionLevel.IsObstruction(CurrentLevel.Map[X - 1, Y + i]) || CurrentLevel.GetEntityAt(X - 1, Y + i) != null) return;
                }
                CurrentLevel.MoveEntityTo(this, new Point(X - 1, Y));
            }
            if (d == Utils.Direction.East && X < CurrentLevel.Width - Size) {
                if (CurrentLevel.Map[X + Size, Y] == MissionLevel.TileType.DoorVertical && CanOpenDoors) {
                    CurrentLevel.OpenDoor(X + Size, Y);
                    if (CurrentLevel.Visible[X + Size, Y]) playSound("OpenDoor");
                }
                for (int i = 0; i < Size; i++) {
                    if (MissionLevel.IsObstruction(CurrentLevel.Map[X + Size, Y + i]) || CurrentLevel.GetEntityAt(X + Size, Y + i) != null) return;
                }
                CurrentLevel.MoveEntityTo(this, new Point(X + 1, Y));
            }
            if (d == Utils.Direction.North && Y < CurrentLevel.Height - Size) {
                if (CurrentLevel.Map[X, Y + Size] == MissionLevel.TileType.DoorHorizontal && CanOpenDoors) {
                    CurrentLevel.OpenDoor(X, Y + Size);
                    if (CurrentLevel.Visible[X, Y + Size]) playSound("OpenDoor");
                }
                for (int i = 0; i < Size; i++) {
                    if (MissionLevel.IsObstruction(CurrentLevel.Map[X + i, Y + Size]) || CurrentLevel.GetEntityAt(X + i, Y + Size) != null) return;
                }
                CurrentLevel.MoveEntityTo(this, new Point(X, Y + 1));
            }
            if (d == Utils.Direction.South && Y > 0) {
                if (CurrentLevel.Map[X, Y - 1] == MissionLevel.TileType.DoorHorizontal && CanOpenDoors) {
                    CurrentLevel.OpenDoor(X, Y - 1);
                    if (CurrentLevel.Visible[X, Y - 1]) playSound("OpenDoor");
                }
                for (int i = 0; i < Size; i++) {
                    if (MissionLevel.IsObstruction(CurrentLevel.Map[X + i, Y - 1]) || CurrentLevel.GetEntityAt(X + i, Y - 1) != null) return;
                }
                CurrentLevel.MoveEntityTo(this, new Point(X, Y - 1));
            }
            if (oldx != X || oldy != Y) {
                HasMoved = true;
                Stamina -= MovementCost;
            }
        }
        private void MoveTo(Point pt, PlaySoundDelegate playSound) { // Should be adjacent
            if (pt.X == X && pt.Y == Y) return; // Weird - no move

            if (pt.X == X && pt.Y == Y - 1) Move(Utils.Direction.South, playSound);
            else if (pt.X == X && pt.Y == Y + 1) Move(Utils.Direction.North, playSound);
            else if (pt.X == X - 1 && pt.Y == Y) Move(Utils.Direction.West, playSound);
            else if (pt.X == X + 1 && pt.Y == Y) Move(Utils.Direction.East, playSound);
            else throw new Exception("Cannot move to non-adjacent point in one step");
        }
        public void AttackEntity(IEntity en, VisualEffect.EffectFactory effectFactory, PlaySoundDelegate playSound) {
            if (en == null) return;
            double range = RangeTo(en);
            if (range > AttackRange) return;
            Stamina -= AttackCost;

            // Rotate creature
            float dx = X - en.X;
            float dy = Y - en.Y;
            SetFacing(180.0 + Math.Atan2(dy, dx) * (180.0 / Math.PI));
            Thread.Sleep(100);

            // Do the attack
            int nhits = 0;
            int nshots = EquippedWeapon?.Type?.Shots ?? 1;
            double recoil = EquippedWeapon?.Recoil ?? 0d;
            List<ShotResult> results = new List<ShotResult>();
            int r = (int)Math.Ceiling(EquippedWeapon?.Type?.Area ?? 0d);
            for (int n = 0; n < nshots; n++) {
                if (r > 0d) {
                    results.Add(new ShotResult(this, true));
                }
                else {
                    // Single target so roll to hit
                    if (en is null) return;
                    double hit = Utils.GenerateHitRoll(this, en) - (n * recoil);  // Subsequent shots are harder to hit
                    if (hit > 0.0) {
                        nhits++;
                        results.Add(new ShotResult(this, true));
                    }
                    else {
                        results.Add(new ShotResult(this, false));
                    }
                }
            }

            // Play weapon sound
            if (EquippedWeapon == null) playSound("Punches");
            else playSound(EquippedWeapon.Type.SoundEffect);

            // Set up the projectile shots or auto-resolve melee effect
            Utils.CreateShots(EquippedWeapon, this, en.X, en.Y, en.Size, results, range, effectFactory, (float)(EquippedWeapon?.Type.BaseDelay ?? 0d));
        }
        public void AIStep(VisualEffect.EffectFactory fact, Action<IEntity> postMoveCheck, PlaySoundDelegate playSound, Action<IEntity> centreView, bool fastAI) {
            int nsteps = 0;
            if (CurrentLevel is null) return;
            bool isDefendLevel = CurrentLevel.ParentMission.Goal == Mission.MissionGoal.Defend;
            HasMoved = false;
            if (Investigate == Point.Empty) Searching = 0;
            if (!IsAlert) return;

            // Flee if really damaged (unless this is a "Defend the objective" mission, or this creature is a boss)
            if (CurrentTarget != null && AIStep_ShouldFlee() && !isDefendLevel) {
                // Flee
                while (Stamina >= MovementCost && ++nsteps < 20 && rnd.NextDouble() * Health / MaxHealth < 0.2) {
                    int oldx = X, oldy = Y;
                    MoveAwayFrom(CurrentTarget, playSound);
                    if (X != oldx || Y != oldy) {
                        postMoveCheck(this);
                        if (CurrentLevel.EntityIsVisible(this)) Thread.Sleep(fastAI ? Const.FastAITickSpeed : Const.AITickSpeed);
                    }
                }
                CurrentTarget = null;
                return;
            }

            // --- MAIN AI LOOP ---
            do {
                // Make sure we're on the best target
                IEntity? lastTarg = CurrentTarget;
                SetBestTarget();
                if (CurrentTarget == null && lastTarg != null && lastTarg.Health > 0.0) CurrentTarget = lastTarg; // Maybe we lost sight of them because we're pathing a complex route. Stay on track.
                if (isDefendLevel) {
                    // Make sure the objective target is always set and never gets overridden on defend levels
                    Investigate = CurrentLevel.LocationToDefend;
                }

                // We have a target, so attempt to get near enough to attack them, and do so.
                if (CurrentTarget != null) {
                    if (AIStep_HasTarget(fact, postMoveCheck, playSound, centreView, fastAI)) return;
                }

                // No target and couldn't find one. Maybe investigate nearby if we heard something.
                else if (Investigate != Point.Empty) {
                    if (AIStep_Investigate(postMoveCheck, playSound, fastAI)) break;                   
                }

                // Running to hide somewhere i.e. if you are being attacked but can't get within range of the attacker because the way is blocked.
                else if (HidingPlace != Point.Empty) {
                    if (AIStep_Hide(postMoveCheck, playSound, fastAI)) return;
                }

                // Should creature attempt to hide from Soldiers?
                else {
                    if (AIStep_SetHidingPlace()) return;
                }
            } while (++nsteps < 20 && Stamina >= MovementCost);

            // If we're doing a "Defend the objective" mission then never forget the Investigate objective.
            // Otherwise get bored searching after a few rounds.
            if (Investigate != Point.Empty) {
                Searching++;
                if (Searching > 2 && rnd.NextDouble() > 0.5 && !isDefendLevel) {
                    Investigate = Point.Empty;
                    Searching = 0;
                }
            }
        }
        private bool AIStep_ShouldFlee() {
            if (Health * 4.0 > MaxHealth) return false;
            if (CurrentTarget == null) return false;
            if (Health >= CurrentTarget.Health) return false;
            if (Level != CurrentTarget.Level) return false;
            if (Type.IsBoss) return false;
            return rnd.NextDouble() * Health / MaxHealth < 0.1;
        }
        private bool AIStep_HasTarget(VisualEffect.EffectFactory fact, Action<IEntity> postMoveCheck, PlaySoundDelegate playSound, Action<IEntity> centreView, bool fastAI) {
            if (CurrentTarget is null) return false;
            bool isDefendLevel = CurrentLevel!.ParentMission.Goal == Mission.MissionGoal.Defend;
            bool atObjective = isDefendLevel && CurrentLevel.CheckIfLocationIsEntranceTile(Location);
            double atr = AttackRange;
            double r = RangeTo(CurrentTarget);
            // -- Can we attack? If not then maybe move a bit closer for a better shot?
            // We are within range:
            if (r <= atr) {
                // We can't see the target, so maybe attempt to find them
                if (!CanSee(CurrentTarget)) {
                    List<Point>? path = CurrentLevel.ShortestPath(this, Location, CurrentTarget.Location, 20, false);
                    if (path is null || path.Count == 0) {
                        CurrentTarget = null; // No way of getting close enough to see target so remove the target
                        return false;
                    }
                    if (!atObjective) MoveTo(path[0], playSound);
                    postMoveCheck(this);
                    if (CurrentLevel.EntityIsVisible(this)) Thread.Sleep(fastAI ? Const.FastAITickSpeed : Const.AITickSpeed);
                }
                // We can see the target, but can't attack them because we don't have enough stamina
                else if (Stamina < AttackCost) {
                    // Optionally move closer?
                    if (r > 5.0 && Stamina >= MovementCost && r > atr * 0.8 && rnd.NextDouble() < 0.3) {
                        List<Point>? path = CurrentLevel.ShortestPath(this, Location, CurrentTarget.Location, 5, false, (int)Math.Floor(atr));
                        if (path is null || path.Count == 0) {
                            return true; // Could be ok - path to target is blocked but can still attack from range. Or else target is adjacent.
                        }
                        if (!atObjective) MoveTo(path[0], playSound);
                        postMoveCheck(this);
                        if (CurrentLevel.EntityIsVisible(this)) Thread.Sleep(fastAI ? Const.FastAITickSpeed : Const.AITickSpeed);
                    }
                    return true;
                }
                // We can see the target, and have enough stamina to attack, so attack.
                else {
                    centreView(this);
                    Thread.Sleep(fastAI ? 150 : 200);
                    AttackEntity(CurrentTarget, fact, playSound);
                    Thread.Sleep(fastAI ? Const.FastAITickSpeed : Const.AITickSpeed);
                }
            }
            // We are not within range of the target
            else {
                // Close the distance
                if (Stamina < MovementCost) return true;
                List<Point>? path = CurrentLevel.ShortestPath(this, Location, CurrentTarget.Location, 50, false, (int)Math.Floor(atr));
                if (path is null || path.Count == 0) {
                    // No way of getting close enough to hit target so give up and don't investigate.
                    CurrentTarget = null;
                    if (!isDefendLevel) Investigate = Point.Empty;
                }
                else {
                    if (!atObjective) MoveTo(path[0], playSound);
                    postMoveCheck(this);
                    if (CurrentLevel.EntityIsVisible(this)) Thread.Sleep(fastAI ? Const.FastAITickSpeed : Const.AITickSpeed);
                }
            }
            return false;
        }
        private bool AIStep_Investigate(Action<IEntity> postMoveCheck, PlaySoundDelegate playSound, bool fastAI) {
            bool isDefendLevel = CurrentLevel!.ParentMission.Goal == Mission.MissionGoal.Defend;
            if (Math.Abs(Investigate.X - X) < Size && Math.Abs(Investigate.Y - Y) < Size) { // We got to the objective
                if (!isDefendLevel) Investigate = Point.Empty;
                return true;
            }
            if (Stamina < MovementCost) return true;
            // Find a route to the target square or nearby, if possible
            List<Point>? path = CurrentLevel.ShortestPath(this, Location, Investigate, 40, false, 1, ignoreEntities: isDefendLevel);
            // No route available, quit
            if (path is null || path.Count == 0) {
                if (!isDefendLevel) Investigate = Point.Empty;
                Searching = 0;
                return true;
            }
            // There is a route, so take it
            MoveTo(path[0], playSound);
            postMoveCheck(this);
            if (CurrentLevel.EntityIsVisible(this)) Thread.Sleep(fastAI ? Const.FastAITickSpeed : Const.AITickSpeed);
            return false;
        }
        private bool AIStep_Hide(Action<IEntity> postMoveCheck, PlaySoundDelegate playSound, bool fastAI) {
            if (Stamina < MovementCost) return true;
            List<Point>? path = CurrentLevel!.ShortestPath(this, Location, HidingPlace, 30, false, mindist: 0, preciseTarget: true);
            if (path is null || path.Count == 0) {
                HidingPlace = Point.Empty;
                return true;
            }
            else {
                MoveTo(path[0], playSound);
                postMoveCheck(this);
                if (CurrentLevel.EntityIsVisible(this)) Thread.Sleep(fastAI ? Const.FastAITickSpeed : Const.AITickSpeed);
            }
            return false;
        }
        private bool AIStep_SetHidingPlace() {
            Point? hidingPlace = null;
            if (IsInjured && CurrentLevel!.EntityIsVisible(this)) {
                hidingPlace = FindHidingPlace();
            }
            if (hidingPlace is null) {
                IsAlert = false;
                return true; // No target, no hope of finding one, just give up and go back to idling
            }
            else {
                HidingPlace = hidingPlace!.Value; // Go here
            }
            return false;
        }
        private Point? FindHidingPlace() {
            Point? best = null;
            double bestscore = -10000.0;
            for (int n=0; n<50; n++) {
                int range = 3 + (n/15);
                // Random point nearby
                Point? hidingSpot;
                int attempts = 0;
                List<Point>? path = null;
                do {
                    hidingSpot = CurrentLevel!.GetPointNearby(this, range);
                    if (hidingSpot.HasValue) {
                        path = CurrentLevel.ShortestPath(this, Location, hidingSpot.Value, 30, false, mindist: 0, preciseTarget: true);
                        if (path is null || path.Count == 0 || path.Count * MovementCost > Stamina) hidingSpot = null;
                    }
                } while (hidingSpot == null && ++attempts < 10);
                if (!hidingSpot.HasValue) return null; // Too hard to get a nearby location

                // Work out how good a hiding spot this is
                double score = -path!.Count;
                // Can any soldiers see this place?
                foreach (Soldier s in CurrentLevel.Soldiers) {
                    if (s.CouldSeeEntityAtLocation(this, hidingSpot.Value)) {
                        double dist = Math.Sqrt(((s.X - hidingSpot.Value.X) * (s.X - hidingSpot.Value.X)) + ((s.Y - hidingSpot.Value.Y) * (s.Y - hidingSpot.Value.Y)));
                        score -= (40-dist);
                    }
                }
                if (score > bestscore) {
                    bestscore = score;
                    best = hidingSpot;
                }
            }
            return best;
        }
        private void SetBestTarget() {
            double bestscore = -10000.0;
            foreach (Soldier s in CurrentLevel!.Soldiers) {
                if (CanSee(s)) {
                    double score = 100.0 / RangeTo(s);
                    if (s == CurrentTarget) score += 15.0;
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
        private void MoveAwayFrom(IEntity en, PlaySoundDelegate playSound) {
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
        public void EndOfTurn(VisualEffect.EffectFactory fact, Action<IEntity> centreView, PlaySoundDelegate playSound, ShowMessageDelegate showMessage, ItemEffect.ApplyItemEffect applyEffect) {
            Stamina = MaxStamina;

            // Handle periodic effects
            bool bPlayedGrunt = false;
            foreach (Effect e in Effects) {
                bool bZoom = (!string.IsNullOrEmpty(e.SoundEffect) || e.Damage != 0.0);
                if (bZoom) {
                    // Zoom to this creature & redraw
                    centreView(this);
                    Thread.Sleep(200);
                }
                if (!string.IsNullOrEmpty(e.SoundEffect)) playSound(e.SoundEffect);
                if (Math.Abs(e.Damage) > 0.01) {
                    Dictionary<WeaponType.DamageType, double> AllDam = new Dictionary<WeaponType.DamageType, double> { { e.DamageType, e.Damage } };
                    double TotalDam = CalculateDamage(AllDam);
                    fact(VisualEffect.EffectType.Damage, X + (Size / 2f), Y + (Size / 2f), new Dictionary<string, object>() { { "Value", TotalDam } });
                    if (TotalDam > 0.0 && !bPlayedGrunt) {
                        playSound("Grunt");
                        bPlayedGrunt = true;
                    }
                    InflictDamage(AllDam, applyEffect, fact);
                    if (Health <= 0.0) {
                        // If this effect killed this creature, maybe register the kill and then stop here
                        if (!string.IsNullOrEmpty(e.CausedBy)) {
                            Soldier? s = CurrentLevel?.GetSoldierByName(e.CausedBy);
                            if (s is not null) {
                                s.RegisterKill(this, showMessage);
                            }
                        }
                        break; 
                    }
                }
                if (bZoom) Thread.Sleep(250);
                e.ReduceDuration(1);
            }
            _Effects.RemoveAll(e => e.Duration <= 0);
            if (Health < 0.0) _Effects.Clear();

            // If this creature has arrived at the target location we're trying to defend, announce it.
            if (CurrentLevel!.ParentMission.Goal == Mission.MissionGoal.Defend) {
                CheckDefensiveGoal(showMessage);
            }
        }
        private void CheckDefensiveGoal(ShowMessageDelegate showMessage) {
            // Check if this creature has moved on to the entrance squares
            if (!HasMoved) return;
            if (!CurrentLevel!.CheckIfLocationIsEntranceTile(Location)) return;

            // Check if this creature is the first one to reach the target
            if (CurrentLevel.CountCreaturesAtEntrance() > 1) return;

            // Announce the incursion
            showMessage($"A {Name} has reached the objective.\nIf it remains there for a turn then you will be defeated!", null);
        }
        public void SetTarget(IEntity? tg) {
            CurrentTarget = tg;
            if (tg == null) Investigate = Point.Empty;
            else Investigate = tg.Location;
        }
        public void SetTargetInvestigation(int x, int y) {
            if (x <= 0 || y <= 0) Investigate = Point.Empty;
            else Investigate = new Point(x, y);
        }
        public void SetTargetInvestigation(Point pt) {
            Investigate = new Point(pt.X, pt.Y);
        }
        public void SetHasQuestItem() {
            QuestItem = true;
        }
        public void SetAlert() {
            IsAlert = true;
        }
        public void CheckChangeTarget(double totalDam, IEntity? attacker) {
            if (attacker is null) return;
            if (CurrentTarget == attacker) return;
            if (CurrentTarget == null) {
                SetTarget(attacker);
                return;
            }
            // Got a target which is different from the attacker. Swap?
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