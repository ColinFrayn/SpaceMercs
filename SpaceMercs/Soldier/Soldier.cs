using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using SharpFont;
using SpaceMercs.Dialogs;
using SpaceMercs.Graphics;
using SpaceMercs.Graphics.Shapes;
using System.Collections.ObjectModel;
using System.IO;
using System.Xml;

namespace SpaceMercs {
    class Soldier : IEntity {
        public enum UtilitySkill { Unspent, Medic, Engineer, Gunsmith, Armoursmith, Bladesmith, Avoidance, Stealth, Scavenging, Perception, Sniper }

        // Generic stuff
        public Team PlayerTeam;
        public AstronomicalObject aoLocation; // Usually ignored, unless soldier is deactivated
        private int iTextureID = -1;
        public bool IsActive { get; private set; }
        public string SoldierID {
            get {
                return Name + "/" + Level + "/" + Race.Name + "/" + Gender + "/" + (BaseStrength ^ BaseAgility ^ BaseIntellect ^ BaseToughness ^ BaseEndurance);
            }
        }
        public Point GoTo { get; set; }
        public bool OnMission { get; set; }
        private readonly Random rnd;
        private bool bHasMoved = false;

        // IEntity Stuff
        public int X { get; private set; }
        public int Y { get; private set; }
        public int Level { get; private set; }
        public double Health { get; private set; }
        public double MaxHealth { get; private set; }
        public double Stamina { get; private set; }
        public double MaxStamina { get; private set; }
        public double Shields { get; private set; }
        public double MaxShields { get; private set; }
        public double Attack { get; private set; }
        public double Defence { get; private set; }
        public string Name { get; private set; }
        public Weapon EquippedWeapon { get; private set; }
        public int Size { get { return 1; } }
        public int TravelRange { get { /* if (Encumbrance >= 1.0) return 0; */ return (int)Math.Floor(Stamina / MovementCost); } }
        public double AttackRange { get { if (EquippedWeapon == null) return 1.0; return EquippedWeapon.Range; } }
        public double Facing { get; private set; }
        public Point Location { get { return new Point(X, Y); } }
        private readonly List<Effect> _Effects = new List<Effect>();
        public IEnumerable<Effect> Effects { get { return _Effects.AsReadOnly(); } }
        private bool[,] Visible;
        public Color PrimaryColor { get; private set; }

        public bool CanSee(int x, int y) { if (x < 0 || y < 0 || x >= Visible.GetLength(0) || y >= Visible.GetLength(1)) return false; return Visible[x, y]; }
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
            if (iTextureID > -1 && (bShieldsInTexture != (Shields > 0.0))) { GL.DeleteTexture(iTextureID); iTextureID = -1; }
            if (iTextureID == -1) {
                iTextureID = Textures.GenerateSoldierTexture(this);
                bShieldsInTexture = (Shields > 0.0);
            }
            GL.BindTexture(TextureTarget.Texture2D, iTextureID);
            GL.Enable(EnableCap.Blend);
            GL.Disable(EnableCap.DepthTest);

            prog.SetUniform("textureEnabled", true);
            prog.SetUniform("texPos", 0f, 0f);
            prog.SetUniform("texScale", 1f, 1f);
            Matrix4 pTranslateM = Matrix4.CreateTranslation(X + 0.5f, Y + 0.5f, Const.EntityLayer);
            Matrix4 pScaleM = Matrix4.CreateScale((float)Race.Scale);
            Matrix4 pRotateM = Matrix4.CreateRotationZ((float)((Facing + 180) * Math.PI / 180));
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
            }
            if (bStatBars) {
                GraphicsFunctions.DisplayBicolourFractBar(prog, X, Y - 0.1f, 1.0f, 0.09f, (float)(Health / MaxHealth), new Vector4(0.3f, 1f, 0.3f, 1f), new Vector4(1f, 0f, 0f, 1f));
                GraphicsFunctions.DisplayBicolourFractBar(prog, X, Y - 0.25f, 1.0f, 0.09f, (float)(Stamina / MaxStamina), new Vector4(1f, 1f, 1f, 1f), new Vector4(0.6f, 0.6f, 0.6f, 1f));
                if (MaxShields > 0) GraphicsFunctions.DisplayBicolourFractBar(prog, X, Y - 0.4f, 1.0f, 0.09f, (float)(Shields / MaxShields), new Vector4(0.2f, 0.5f, 1f, 1f), new Vector4(0.2f, 0.2f, 0.2f, 1f));
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
            _Effects.Clear();
            if (iTextureID != -1) {
                GL.DeleteTexture(iTextureID);
                iTextureID = -1;
            }
            Facing = 0.0;
        }
        public bool CanOpenDoors { get { return true; } }
        public double RangeTo(IEntity en) {
            int dx = (en.X > X) ? en.X - X : X - (en.X + en.Size - 1);
            if (dx < 0) dx = 0;
            int dy = (en.Y > Y) ? en.Y - Y : Y - (en.Y + en.Size - 1);
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
        public double GetDamageReductionByDamageType(WeaponType.DamageType type) {
            double red = 100.0;
            foreach (Armour ar in EquippedArmour) {
                red -= ar.GetDamageReductionByDamageType(type);
            }
            if (red < 0.0) red = 0.0; // Clamp this for Soldiers (i.e. no healing)
            return Utils.ArmourReduction(BaseArmour) * red / 100.0;
        }
        public double InflictDamage(Dictionary<WeaponType.DamageType, double> AllDam) {
            if (!AllDam.Any()) return 0.0;

            // Shields?
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

                // Armour reduces damage but doesn't reduce healing
                if (dam > 0.0) TotalDam += dam * GetDamageReductionByDamageType(type); // Damage
                else TotalDam += dam; // Healing
            }

            // Do the damage / healing
            Health -= TotalDam;

            // Is the soldier dead?
            if (Health <= 0.0) KillEntity();
            return TotalDam;
        }
        public Stash GenerateStash() {
            Stash st = new Stash(Location);

            // Add corpse
            Corpse cp = new Corpse(this);
            st.Add(cp);

            // Generate dropped items
            foreach (IItem eq in Inventory.Keys) {
                st.Add(eq, Inventory[eq]);
            }

            return st;
        }
        public void KillEntity() {
            Health = 0.0;
            CurrentLevel.KillSoldier(this);
        }
        public Dictionary<WeaponType.DamageType, double> GenerateDamage() {
            double hmod = 1.0; // Utils.HitToMod(hit); // Damage modifier for quality of hit
            Dictionary<WeaponType.DamageType, double> AllDam = new Dictionary<WeaponType.DamageType, double>();
            WeaponType.DamageType type = WeaponType.DamageType.Physical;
            if (EquippedWeapon == null) {
                double dam = (rnd.NextDouble() + 1.0) * Const.AttackDamageScale * Strength / 15.0;  // Melee does rubbish damage, in general
                AllDam.Add(WeaponType.DamageType.Physical, dam * hmod * Const.SoldierDamageModifier);
            }
            else {
                double dam = EquippedWeapon.DBase + (rnd.NextDouble() * EquippedWeapon.DMod);
                //if (EquippedWeapon.Type.IsMeleeWeapon) dam *= Strength / 10.0;
                hmod = Attack / 10.0; // 10% damage bonus per attack point (includes weapon skill etc.)
                hmod *= Const.AttackDamageScale;
                type = EquippedWeapon.Type.DType;
                AllDam.Add(type, dam * hmod * Const.SoldierDamageModifier);
                foreach (KeyValuePair<WeaponType.DamageType, double> bdam in EquippedWeapon.GetBonusDamage()) {
                    if (AllDam.ContainsKey(bdam.Key)) AllDam[bdam.Key] += bdam.Value * hmod * Const.SoldierDamageModifier;
                    else AllDam.Add(bdam.Key, bdam.Value * hmod * Const.SoldierDamageModifier);
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
                    if (ie.AssociatedSkill != UtilitySkill.Unspent && src is Soldier) {
                        int sk = ((Soldier)src).GetUtilityLevel(ie.AssociatedSkill);
                        if (sk == 0 && ie.SkillRequired) throw new Exception("Attempting to perform unskilled application of effect");
                        if (sk == 0) dmod /= 2.0; // Unskilled use
                        else dmod += Math.Pow(sk - 1, 1.5) / 10.0;
                    }
                    if (AllDam.ContainsKey(eff.DamageType)) AllDam[eff.DamageType] += eff.Damage * dmod;
                    else AllDam.Add(eff.DamageType, eff.Damage * dmod);
                }
                else {
                    // Make a copy of the effect and apply it
                    _Effects.Add(new Effect(eff));
                }
            }
            double TotalDam = InflictDamage(AllDam);
            if (TotalDam > 0.0) fact(VisualEffect.EffectType.Damage, X + (Size / 2.0), Y + (Size / 2.0), new Dictionary<string, object>() { { "Value", TotalDam } });
            else if (TotalDam < 0.0) fact(VisualEffect.EffectType.Healing, X + (Size / 2.0), Y + (Size / 2.0), new Dictionary<string, object>() { { "Value", -TotalDam } });
            CalculateMaxStats();
        }

        // Inventory
        private readonly Dictionary<IItem, int> Inventory = new Dictionary<IItem, int>();
        public ReadOnlyDictionary<IItem, int> InventoryRO { get { return new ReadOnlyDictionary<IItem, int>(Inventory); } }
        public List<Armour> EquippedArmour = new List<Armour>();

        // Base stats
        public GenderType Gender { get; private set; }
        public Race Race { get; private set; }
        public int BaseStrength { get; private set; }
        public int BaseAgility { get; private set; }
        public int BaseIntellect { get; private set; }
        public int BaseToughness { get; private set; }
        public int BaseEndurance { get; private set; }
        public int Experience { get; private set; }

        // Calculated stats
        public int Strength { get { return Math.Max(0, BaseStrength + StatBonuses(StatType.Strength)); } }
        public int Agility { get { return Math.Max(0, BaseAgility + StatBonuses(StatType.Agility)); } }
        public int Intellect { get { return Math.Max(0, BaseIntellect + StatBonuses(StatType.Intelligence)); } }
        public int Toughness { get { return Math.Max(0, BaseToughness + StatBonuses(StatType.Toughness)); } }
        public int Endurance { get { return Math.Max(0, BaseEndurance + StatBonuses(StatType.Endurance)); } }
        public int BaseAttack { get { return (MeleeWeaponEquipped ? Strength : Intellect) + Level + 2; } }
        public int BaseDefence { get { return Agility + Level + 2; } }
        public int BaseHealth { get { return Toughness + Level + 10; } }
        public int BaseStamina { get { return Endurance + Level + 10; } }
        public double MaximumCarry { get { return (Math.Pow(Strength, 1.55) * Const.MaxCarryScale) + 4.0; } }
        public double Encumbrance {
            get {
                double m = CalculateInventoryMass();
                if (m * 2.0 <= MaximumCarry) return 0.0;
                return Math.Min(1.0, (m - (MaximumCarry / 2.0)) * 2.0 / MaximumCarry);
            }
        }
        public bool MeleeWeaponEquipped { get { return (EquippedWeapon != null && EquippedWeapon.Type.IsMeleeWeapon); } }
        public double MovementCost { get { return Const.MovementCost * (1.0 + Encumbrance) / SpeedModifier(); } }
        public double SearchCost { get { return Math.Min(MaxStamina, Const.SearchCost * (1.0 + Encumbrance)); } }
        public double AttackCost { get { if (EquippedWeapon == null) return Const.MeleeCost; if (bHasMoved && EquippedWeapon.Type.Stable) return 999.0; return EquippedWeapon.StaminaCost * (1.0 + Encumbrance); } }
        public double UseItemCost { get { return Math.Min(MaxStamina, Const.UseItemCost * (1.0 + Encumbrance)); } }
        private MissionLevel CurrentLevel { get { return PlayerTeam.CurrentMission.GetOrCreateCurrentLevel(); } }
        public int SearchRadius { get { return Const.BaseSearchRadius + GetUtilityLevel(UtilitySkill.Perception); } }
        public double BaseSearchChance { get { return Const.BaseSearchChance + GetUtilityLevel(UtilitySkill.Perception) * Const.SearchBoostPerSkill; } }
        private void CalculateMaxStats() {
            MaxHealth = BaseHealth + StatBonuses(StatType.Health);
            if (Health > MaxHealth) Health = MaxHealth;
            MaxStamina = BaseStamina + StatBonuses(StatType.Stamina);
            if (Stamina > MaxStamina) Stamina = MaxStamina;
            MaxShields = ShieldsFromItems();
            if (Shields > MaxShields) Shields = MaxShields;
            Attack = BaseAttack + StatBonuses(StatType.Attack) + ((EquippedWeapon != null) ? (EquippedWeapon.AttackBonus + GetSoldierSkillWithWeapon(EquippedWeapon.Type)) : 0);
            Defence = BaseDefence + GetUtilityLevel(UtilitySkill.Avoidance) + StatBonuses(StatType.Defence);
        }

        // Skills & experience
        private readonly Dictionary<WeaponType, int> WeaponExperience = new Dictionary<WeaponType, int>();
        private readonly Dictionary<UtilitySkill, int> UtilitySkills = new Dictionary<UtilitySkill, int>();
        public List<WeaponType> SkilledWeapons {
            get {
                return WeaponExperience.Keys.Where(x => Utils.ExperienceToSkillLevel(WeaponExperience[x]) > 0).ToList();
            }
        }
        public void AddUtilitySkill(UtilitySkill sk, int lvls = 1) {
            if (lvls > 0) {
                if (UtilitySkills.ContainsKey(sk)) UtilitySkills[sk] += lvls;
                else UtilitySkills.Add(sk, lvls);
            }
            else {
                if (!UtilitySkills.ContainsKey(sk)) return; // Error
                if (UtilitySkills[sk] + lvls < 0) return; // Error
                UtilitySkills[sk] += lvls;
                if (UtilitySkills[sk] == 0) UtilitySkills.Remove(sk);
            }
            CalculateMaxStats();
        }
        public int GetSoldierSkillWithWeapon(WeaponType wp) {
            if (!WeaponExperience.ContainsKey(wp)) return 0;
            int lvl = Utils.ExperienceToSkillLevel(WeaponExperience[wp]);
            if (lvl > Level) return Level;
            else return lvl;
        }
        public int GetUtilityLevel(UtilitySkill sk) {
            int val = GetRawUtilityLevel(sk);
            if (EquippedWeapon != null) val += EquippedWeapon.Type.GetUtilitySkill(sk);
            foreach (Armour ar in EquippedArmour) {
                val += ar.Type.GetUtilitySkill(sk);
            }
            return val;
        }
        public int GetRawUtilityLevel(UtilitySkill sk) {
            if (!UtilitySkills.ContainsKey(sk)) return 0;
            return UtilitySkills[sk];
        }
        public bool HasAllUtilitySkills() {
            int nsk = UtilitySkills.Keys.Count;
            if (nsk == Enum.GetValues(typeof(UtilitySkill)).Length) return true;
            if (nsk == Enum.GetValues(typeof(UtilitySkill)).Length - 1 && !UtilitySkills.ContainsKey(UtilitySkill.Unspent)) return true;
            return false;
        }
        public List<UtilitySkill> UnknownUtilitySkills() {
            List<UtilitySkill> lSkills = new List<UtilitySkill>();
            Array values = Enum.GetValues(typeof(UtilitySkill));
            foreach (UtilitySkill sk in values) {
                if (!UtilitySkills.ContainsKey(sk) && sk != UtilitySkill.Unspent) lSkills.Add(sk);
            }
            return lSkills;
        }
        public void AddExperience(int exp) {
            Experience += exp * Const.DEBUG_EXPERIENCE_MOD;
        }
        public void CheckForLevelUp(Action<string> showMessage) {
            if (Experience >= ExperienceRequiredToReachNextLevel()) {
                Level++;
                AddUtilitySkill(UtilitySkill.Unspent);
                showMessage("Congratulations! Soldier " + Name + " has reached level " + Level);
                ChooseStat cs = new ChooseStat(this);
                cs.ShowDialog(new Form { TopMost = true });
                CalculateMaxStats();
            }
        }
        public int ExperienceRequiredToReachNextLevel() {
            return (int)(Const.SoldierLevelExperience * ((Math.Pow(Const.SoldierLevelExponent, Level - 1) * Const.SoldierLevelScale) - (Const.SoldierLevelScale - 1.0)));
        }
        public void GenerateRandomUtilitySkills() {
            UtilitySkills.Clear();
            int nsk = Level + 1;
            if (EquippedWeapon != null) {
                if (EquippedWeapon.Type.IsMeleeWeapon) {
                    AddUtilitySkill(UtilitySkill.Bladesmith);
                    AddUtilitySkill(UtilitySkill.Avoidance);
                    nsk -= 2;
                }
                else {
                    AddUtilitySkill(UtilitySkill.Gunsmith);
                    nsk--;

                }
            }
            Array values = Enum.GetValues(typeof(UtilitySkill));
            while (nsk > 0) {
                UtilitySkill sk = (UtilitySkill)values.GetValue(rnd.Next(values.Length));
                if (sk == UtilitySkill.Unspent) continue;
                else if (UtilitySkills.ContainsKey(sk) && UtilitySkills[sk] >= Level) continue;
                else {
                    nsk--;
                    AddUtilitySkill(sk);
                    if (nsk > 0 && rnd.Next(3) > 0) {
                        nsk--;
                        AddUtilitySkill(sk);
                    }
                }
            }
        }
        private void GenerateSuitableWeaponSkills() {
            WeaponExperience.Clear();
            int val = Utils.SkillLevelToExperience(Level);
            val = rnd.Next(val / 2) + (val * 2 / 3);
            if (EquippedWeapon != null) WeaponExperience.Add(EquippedWeapon.Type, val);
        }
        public void IncreaseStat(StatType tp, int val = 1) {
            switch (tp) {
                case StatType.Strength: BaseStrength += val; break;
                case StatType.Agility: BaseAgility += val; break;
                case StatType.Intelligence: BaseIntellect += val; break;
                case StatType.Toughness: BaseToughness += val; break;
                case StatType.Endurance: BaseEndurance += val; break;
                default: throw new NotImplementedException("Attemptign to increase non-primary stat : " + tp.ToString());
            }

            CalculateMaxStats();
        }

        // Display stuff
        private bool bShieldsInTexture = false;

        // CTORs
        public Soldier(string strName, Race rc, int Str, int Agi, int Int, int Tou, int End, GenderType G, int ilvl, int randseed) {
            Name = strName;
            Race = rc;
            BaseStrength = Str;
            BaseAgility = Agi;
            BaseIntellect = Int;
            BaseToughness = Tou;
            BaseEndurance = End;
            Gender = G;
            Level = ilvl;
            IsActive = true;
            Experience = 0;
            GoTo = Point.Empty;
            OnMission = false;
            UtilitySkills.Add(UtilitySkill.Unspent, ilvl + 1);
            CalculateMaxStats();
            rnd = new Random(randseed);
            PrimaryColor = Color.Blue;
        }
        public Soldier(XmlNode xml, Team pt) {
            PlayerTeam = pt;
            iTextureID = -1;
            Name = xml.Attributes["Name"].Value;
            rnd = new Random(Name.GetHashCode());
            XmlNode xac = xml.SelectSingleNode("Inactive");
            if (xac == null) {
                IsActive = true;
                if (pt != null) aoLocation = pt.CurrentPosition;
            }
            else {
                IsActive = false;
                if (pt != null) aoLocation = pt.CurrentPosition.GetSystem().Sector.ParentMap.GetAOFromLocationString(xac.Attributes["Loc"].Value);
                if (aoLocation == null) throw new Exception("Could not ID location of soldier : " + xac.Attributes["Loc"].Value);
            }
            OnMission = (xml.SelectSingleNode("OnMission") != null);

            XmlNode xmll = xml.SelectSingleNode("Location");
            if (xmll != null) {
                X = Int32.Parse(xmll.Attributes["X"].Value);
                Y = Int32.Parse(xmll.Attributes["Y"].Value);
            }
            else X = Y = 0;
            Level = Int32.Parse(xml.SelectSingleNode("Level").InnerText);
            Gender = (GenderType)Enum.Parse(typeof(GenderType), xml.SelectSingleNode("Gender").InnerText);
            Race = StaticData.GetRaceByName(xml.SelectSingleNode("Race").InnerText);
            if (Race == null) throw new Exception("Could not ID Soldier " + Name + " Race : " + xml.SelectSingleNode("Race").InnerText);
            XmlNode xmls = xml.SelectSingleNode("Stats");
            string[] stats = xmls.InnerText.Split(',');
            if (stats.Length != 5) throw new Exception("Could not understand stats string for Soldier " + Name + " : " + xmls.InnerText);
            BaseStrength = Int32.Parse(stats[0]);
            BaseAgility = Int32.Parse(stats[1]);
            BaseIntellect = Int32.Parse(stats[2]);
            BaseToughness = Int32.Parse(stats[3]);
            BaseEndurance = Int32.Parse(stats[4]);
            Experience = Int32.Parse(xml.SelectSingleNode("XP").InnerText);
            if (xml.SelectSingleNode("Health") != null) Health = Double.Parse(xml.SelectSingleNode("Health").InnerText);
            else Health = MaxHealth;
            if (xml.SelectSingleNode("Stamina") != null) Stamina = Double.Parse(xml.SelectSingleNode("Stamina").InnerText);
            else Stamina = MaxStamina;
            if (xml.SelectSingleNode("Shields") != null) Shields = Double.Parse(xml.SelectSingleNode("Shields").InnerText);
            else Shields = MaxShields;
            if (xml.SelectSingleNode("Facing") != null) {
                if (Double.TryParse(xml.SelectSingleNode("Facing").InnerText, out double fac)) {
                    Facing = fac;
                }
                else {
                    SetFacing((Utils.Direction)Enum.Parse(typeof(Utils.Direction), xml.SelectSingleNode("Facing").InnerText));
                }
            }
            else Facing = 90.0;

            if (xml.SelectSingleNode("Colour") != null) {
                PrimaryColor = ColorTranslator.FromHtml(xml.SelectSingleNode("Colour").InnerText);
            }
            else PrimaryColor = Color.Blue;

            bHasMoved = (xml.SelectSingleNode("Moved") != null);

            XmlNode xg = xml.SelectSingleNode("GoTo");
            if (xg != null) {
                int gx = Int32.Parse(xg.Attributes["X"].Value);
                int gy = Int32.Parse(xg.Attributes["Y"].Value);
                GoTo = new Point(gx, gy);
            }
            else GoTo = Point.Empty;

            XmlNode xmli = xml.SelectSingleNode("Inventory");
            Inventory.Clear();
            if (xmli != null) {
                foreach (XmlNode xi in xmli.ChildNodes) {
                    int count = Int32.Parse(xi.Attributes["Count"].Value);
                    IItem eq = Utils.LoadItem(xi.FirstChild);
                    if (Inventory.ContainsKey(eq)) Inventory[eq] += count; // Ideally shouldn't happen, but we might as well tidy it up here if it does...
                    else Inventory.Add(eq, count);
                }
            }

            XmlNode xmlar = xml.SelectSingleNode("EquippedArmour");
            EquippedArmour.Clear();
            if (xmlar != null) {
                foreach (XmlNode xar in xmlar.ChildNodes) {
                    Armour ar = new Armour(xar);
                    EquippedArmour.Add(ar);
                }
            }

            XmlNode xmlwp = xml.SelectSingleNode("EquippedWeapon");
            EquippedWeapon = null;
            if (xmlwp != null) {
                EquippedWeapon = new Weapon(xmlwp.FirstChild);
            }

            XmlNode wex = xml.SelectSingleNode("WeaponExperience");
            WeaponExperience.Clear();
            if (wex != null) {
                foreach (XmlNode xw in wex.SelectNodes("Exp")) {
                    WeaponType tp = StaticData.GetWeaponTypeByName(xw.Attributes["Type"].Value);
                    if (tp == null) throw new Exception("Could not ID WeaponType : " + xw.Attributes["Type"].Value);
                    int exp = Int32.Parse(xw.InnerText);
                    WeaponExperience.Add(tp, exp);
                }
            }

            XmlNode wut = xml.SelectSingleNode("UtilityExperience");
            UtilitySkills.Clear();
            int totsk = 0;
            if (wut != null) {
                foreach (XmlNode xu in wut.SelectNodes("Exp")) {
                    UtilitySkill sk = (UtilitySkill)Enum.Parse(typeof(UtilitySkill), xu.Attributes["Skill"].Value);
                    int lvl = Int32.Parse(xu.InnerText);
                    totsk += lvl;
                    UtilitySkills.Add(sk, lvl);
                }
            }
            if (totsk < Level + 1) {
                AddUtilitySkill(UtilitySkill.Unspent, Level + 1 - totsk); // Make sure we've got our unspent points
            }

            XmlNode xmlef = xml.SelectSingleNode("Effects");
            _Effects.Clear();
            if (xmlef != null) {
                foreach (XmlNode xef in xmlef.ChildNodes) {
                    Effect e = new Effect(xef);
                    _Effects.Add(e);
                }
            }

            CalculateMaxStats();
        }
        public static Soldier GenerateRandomMercenary(Colony cl, Random rand) {
            int[] Stats = new int[5];
            int Bonus = Math.Max(rand.Next(8) - 2, 0); // Some mercs are just better than others
            Stats[0] = cl.Owner.Strength;
            Stats[1] = cl.Owner.Agility;
            Stats[2] = cl.Owner.Intellect;
            Stats[3] = cl.Owner.Toughness;
            Stats[4] = cl.Owner.Endurance;
            for (int n = 0; n < 20; n++) Stats[rand.Next(5)]--;
            for (int n = 0; n < 20 + Bonus; n++) Stats[rand.Next(5)]++;
            for (int n = 0; n < 5; n++) {
                if (Stats[n] < 5) Stats[n] = 5;
            }
            Race r = cl.Location.GetRandomRace(rand);
            GenderType gt = r.GenerateRandomGender(rand);
            string strName = r.GenerateRandomName(rand, gt);
            int iLevel = cl.Location.GetRandomMissionDifficulty(rand);

            Soldier s = new Soldier(strName, cl.Owner, Stats[0], Stats[1], Stats[2], Stats[3], Stats[4], gt, iLevel, rand.Next());
            s.GenerateRandomItems();
            s.GenerateRandomUtilitySkills();
            s.GenerateSuitableWeaponSkills();
            s.CalculateMaxStats();
            return s;
        }

        // Save this soldier to an Xml file
        public void SaveToFile(StreamWriter file) {
            file.WriteLine("<Soldier Name=\"" + Name + "\">");
            if (!IsActive) file.WriteLine(" <Inactive Loc=\"" + aoLocation.PrintCoordinates() + "\"/>");
            if (OnMission) file.WriteLine(" <OnMission/>");
            if (PlayerTeam != null) file.WriteLine(" <Location X=\"" + X + "\" Y=\"" + Y + "\"/>");
            if (PlayerTeam != null) file.WriteLine(" <Health>" + Health + "</Health>");
            if (PlayerTeam != null) file.WriteLine(" <Stamina>" + Stamina + "</Stamina>");
            if (PlayerTeam != null) file.WriteLine(" <Shields>" + Shields + "</Shields>");
            if (PlayerTeam != null) file.WriteLine(" <Facing>" + Facing + "</Facing>");
            file.WriteLine(" <Level>" + Level + "</Level>");
            file.WriteLine(" <XP>" + Experience + "</XP>");
            file.WriteLine(" <Gender>" + Gender + "</Gender>");
            file.WriteLine(" <Race>" + Race.Name + "</Race>");
            file.WriteLine(" <Stats>" + BaseStrength + "," + BaseAgility + "," + BaseIntellect + "," + BaseToughness + "," + BaseEndurance + "</Stats>");
            if (GoTo != Point.Empty && GoTo != Location) file.WriteLine(" <GoTo X=\"" + GoTo.X + "\" Y=\"" + GoTo.Y + "\"/>");
            file.WriteLine(" <Colour>" + ColorTranslator.ToHtml(PrimaryColor) + "</Colour>");

            if (Inventory.Count > 0) {
                file.WriteLine(" <Inventory>");
                foreach (IItem it in Inventory.Keys) {
                    file.WriteLine("  <Inv Count=\"" + Inventory[it] + "\">");
                    it.SaveToFile(file);
                    file.WriteLine("  </Inv>");
                }
                file.WriteLine(" </Inventory>");
            }

            if (EquippedArmour.Count > 0) {
                file.WriteLine(" <EquippedArmour>");
                foreach (Armour ar in EquippedArmour) ar.SaveToFile(file);
                file.WriteLine(" </EquippedArmour>");
            }

            if (EquippedWeapon != null) {
                file.WriteLine(" <EquippedWeapon>");
                EquippedWeapon.SaveToFile(file);
                file.WriteLine(" </EquippedWeapon>");
            }

            if (WeaponExperience.Count > 0) {
                file.WriteLine(" <WeaponExperience>");
                foreach (WeaponType tp in WeaponExperience.Keys) {
                    file.WriteLine("  <Exp Type=\"" + tp.Name + "\">" + WeaponExperience[tp] + "</Exp>");
                }
                file.WriteLine(" </WeaponExperience>");
            }

            if (UtilitySkills.Count > 0) {
                file.WriteLine(" <UtilityExperience>");
                foreach (UtilitySkill sk in UtilitySkills.Keys) {
                    file.WriteLine("  <Exp Skill=\"" + sk + "\">" + UtilitySkills[sk] + "</Exp>");
                }
                file.WriteLine(" </UtilityExperience>");
            }

            if (_Effects.Count > 0) {
                file.WriteLine(" <Effects>");
                foreach (Effect e in Effects) {
                    e.SaveToFile(file);
                }
                file.WriteLine(" </Effects>");
            }

            if (bHasMoved) file.WriteLine(" <Moved/>");

            file.WriteLine("</Soldier>");
        }

        // How much to hire this soldier as a mercenary? (ignoring race relations)
        public double HireCost() {
            double dBase = (Level + 3) * (Level + 1) * Const.MercenaryCostScale;
            double dKit = EquipmentCost();
            int Bonus = (BaseStrength + BaseAgility + BaseIntellect + BaseToughness + BaseEndurance) - (Race.Strength + Race.Agility + Race.Intellect + Race.Toughness + Race.Endurance);
            if (Bonus < 1) Bonus = 0;
            else Bonus++;
            double dStats = 1.0 + ((Bonus * Bonus) / 30.0); // Bonus cost if they are better than average

            // Bonuses for large individual scores
            if (BaseStrength - Race.Strength > 3) dStats += Math.Pow(BaseStrength - Race.Strength - 2, 2) / 25.0;
            if (BaseAgility - Race.Agility > 3) dStats += Math.Pow(BaseAgility - Race.Agility - 2, 2) / 25.0;
            if (BaseIntellect - Race.Intellect > 3) dStats += Math.Pow(BaseIntellect - Race.Intellect - 2, 2) / 25.0;
            if (BaseToughness - Race.Toughness > 3) dStats += Math.Pow(BaseToughness - Race.Toughness - 2, 2) / 25.0;
            if (BaseEndurance - Race.Endurance > 3) dStats += Math.Pow(BaseEndurance - Race.Endurance - 2, 2) / 25.0;

            // Final cost
            return (dBase * dStats) + dKit;
        }
        public double EquipmentCost() {
            double dCost = 0.0;
            foreach (IItem eq in Inventory.Keys) {
                dCost += eq.Cost * Inventory[eq];
            }
            foreach (Armour ar in EquippedArmour) {
                dCost += ar.Cost;
            }
            if (EquippedWeapon != null) dCost += EquippedWeapon.Cost;
            return dCost;
        }

        #region Inventory
        public Armour GetArmourAtLocation(BodyPart bp) {
            foreach (Armour ar in EquippedArmour) {
                if (ar.Type.Locations.Contains(bp)) return ar;
            }
            return null;
        }
        public double CalculateInventoryMass() {
            double w = 0.0;
            foreach (IItem eq in Inventory.Keys) w += eq.Mass * Inventory[eq];
            foreach (Armour ar in EquippedArmour) w += ar.Mass;
            if (EquippedWeapon != null) w += EquippedWeapon.Mass;
            return w;
        }
        public bool Equip(IEquippable eq) {
            if (!Inventory.ContainsKey(eq)) return false;
            if (!(eq is Armour || eq is Weapon)) return false;
            Inventory[eq]--;
            if (Inventory[eq] == 0) Inventory.Remove(eq);
            if (eq is Armour) {
                HashSet<Armour> hsToUnequip = new HashSet<Armour>();
                Armour ar = eq as Armour;
                foreach (BodyPart bp in ar.Type.Locations) {
                    foreach (Armour aeq in EquippedArmour) {
                        if (aeq.Type.Locations.Contains(bp)) {
                            if (!hsToUnequip.Contains(aeq)) hsToUnequip.Add(aeq);
                        }
                    }
                }
                foreach (Armour aeq in hsToUnequip) Unequip(aeq);
                EquippedArmour.Add(ar);
                CalculateMaxStats();
                return true;
            }
            if (eq is Weapon) {
                Unequip(EquippedWeapon);
                EquippedWeapon = eq as Weapon;
                CalculateMaxStats();
                return true;
            }
            return false;
        }
        public void Unequip(IEquippable eq) {
            if (eq == null) return;
            if (eq == EquippedWeapon) {
                EquippedWeapon = null;
                AddItem(eq);
                CalculateMaxStats();
                return;
            }
            else if (eq is Armour) {
                foreach (Armour ar in EquippedArmour) {
                    if (ar == eq) {
                        EquippedArmour.Remove(ar);
                        AddItem(eq);
                        CalculateMaxStats();
                        return;
                    }
                }
            }
            else {
                CalculateMaxStats(); // Not equipped, but might be an item that affects stats
            }
        }
        public void DropItem(IItem it, int Count = 1) {
            if (Inventory.ContainsKey(it)) {
                if (Count > Inventory[it]) Count = Inventory[it];
                PlayerTeam.AddItem(it, Count);
                Inventory[it] -= Count;
                if (Inventory[it] <= 0) Inventory.Remove(it);
            }
            CalculateMaxStats();
        }
        public void DropAll(IItem it) {
            if (Inventory.ContainsKey(it)) {
                PlayerTeam.AddItem(it, Inventory[it]);
                Inventory.Remove(it);
            }
            CalculateMaxStats();
        }
        public void DestroyItem(IItem it, int Count = 1) {
            if (Inventory.ContainsKey(it)) {
                if (Count > Inventory[it]) Count = Inventory[it];
                Inventory[it] -= Count;
                if (Inventory[it] <= 0) Inventory.Remove(it);
            }
            CalculateMaxStats();
        }
        public void RemoveItemByType(ItemType it, int Count = 1) {
            foreach (IEquippable eq in Inventory.Keys.Where(e => e is IEquippable)) {
                if (eq.BaseType == it) {
                    DestroyItem(eq);
                    return;
                }
            }
        }
        public void AddItem(IItem it, int Count = 1) {
            if (it == null || Count < 1) return;
            if (Inventory.ContainsKey(it)) Inventory[it] += Count;
            else Inventory.Add(it, Count);
            CalculateMaxStats();
        }
        public double BaseArmour {
            get {
                double arm = 0.0;
                foreach (Armour ar in EquippedArmour) {
                    arm += ar.BaseArmour;
                }
                return arm;
            }
        }
        public Dictionary<WeaponType.DamageType, double> GetAllResistances() {
            Dictionary<WeaponType.DamageType, double> AllRes = new Dictionary<WeaponType.DamageType, double>();
            foreach (Armour ar in EquippedArmour) {
                Dictionary<WeaponType.DamageType, double> Res = ar.GetAllResistances();
                foreach (WeaponType.DamageType dt in Res.Keys) {
                    if (AllRes.ContainsKey(dt)) AllRes[dt] += Res[dt];
                    else AllRes.Add(dt, Res[dt]);
                }
            }
            return AllRes;
        }
        private void GenerateRandomItems() {
            // Firstly, generate a weapon
            do {
                EquippedWeapon = Utils.GenerateRandomWeapon(rnd, Level);
            } while (EquippedWeapon.StaminaCost > MaxStamina);

            // Next generate armour
            // Start with a base set of kit, then incrementally improve it
            EquippedArmour.Clear();
            SetupBasicArmour();
            for (int i = 0; i < Level; i++) {
                UpgradeArmourAtRandom();
                if (rnd.NextDouble() < 0.8) UpgradeArmourAtRandom();
            }

            // Now generate other random stuff
            int num = (rnd.Next(Level) + 3 + rnd.Next(3)) / 2;
            for (int n = 0; n < num; n++) {
                // Generate a random item suitable for this soldier (not a weapon or armour)
                IItem eq = Utils.GenerateRandomItem(rnd, Level, false);
                if (eq != null) AddItem(eq);
            }
        }
        private void SetupBasicArmour() {
            Armour chest = PickRandomBaseArmourForLocation(BodyPart.Chest);
            if (chest != null && rnd.NextDouble() < 0.8) EquippedArmour.Add(chest);
            foreach (BodyPart bp in Enum.GetValues(typeof(BodyPart))) {
                if (rnd.NextDouble() < 0.3 && GetArmourAtLocation(bp) == null) {
                    Armour arm = PickRandomBaseArmourForLocation(bp);
                    if (arm != null) EquippedArmour.Add(arm);
                }
            }
        }
        private Armour PickRandomBaseArmourForLocation(BodyPart bp) {
            MaterialType mat = null;
            // Get base material
            foreach (MaterialType m in StaticData.Materials) {
                if (m.IsArmourMaterial && (mat == null || m.CostMod < mat.CostMod)) mat = m;
            }
            // Get base armour for this location
            ArmourType choice = null;
            foreach (ArmourType at in StaticData.ArmourTypes) {
                if (at.Locations.Contains(bp) && (choice == null || at.Rarity > choice.Rarity)) {
                    bool bOK = true;
                    foreach (BodyPart abp in at.Locations) {
                        if (GetArmourAtLocation(abp) != null) { bOK = false; break; }
                    }
                    if (bOK) choice = at;
                }
            }
            return new Armour(choice, mat, 0);
        }
        private void UpgradeArmourAtRandom() {
            // Pick a random location
            BodyPart bp = Utils.GetRandomBodyPart();
            Armour ar = GetArmourAtLocation(bp);
            // Try to fill in empty armour slots if at all possible by having another go at finding an empty slot.
            if (ar != null) {
                bp = Utils.GetRandomBodyPart();
                ar = GetArmourAtLocation(bp);
            }
            // If armour doesn't exist at this location, then generate something
            if (ar == null) {
                ar = PickRandomBaseArmourForLocation(bp);
                EquippedArmour.Add(ar);
            }
            // Otherwise upgrade what's there
            else {
                ar.UpgradeArmour();
                //EquippedArmour.Remove(ar);
                //EquippedArmour.Add(new Armour(at, mat, lvl));
                CalculateMaxStats();
            }
        }
        public bool HasItem(IItem it) {
            return Inventory.ContainsKey(it);
        }
        #endregion // Inventory

        // Is this soldier active in the team?
        public void Activate() {
            if (aoLocation != PlayerTeam.CurrentPosition) return;
            IsActive = true;
            // Anything else we want to do?
        }
        public void Deactivate() {
            IsActive = false;
            aoLocation = PlayerTeam.CurrentPosition;
            // Anything else we want to do?
        }

        // Actions
        public bool Move(Utils.Direction d) {
            if (Stamina < MovementCost) return false;
            SetFacing(d);
            double oldStamina = Stamina;
            if (d == Utils.Direction.West) {
                if (X > 0 && !MissionLevel.IsObstruction(CurrentLevel.Map[X - 1, Y]) && CurrentLevel.GetEntityAt(X - 1, Y) == null) {
                    CurrentLevel.MoveEntityTo(this, new Point(X - 1, Y));
                    Stamina -= MovementCost;
                }
            }
            if (d == Utils.Direction.East) {
                if (X < CurrentLevel.Width - 1 && !MissionLevel.IsObstruction(CurrentLevel.Map[X + 1, Y]) && CurrentLevel.GetEntityAt(X + 1, Y) == null) {
                    CurrentLevel.MoveEntityTo(this, new Point(X + 1, Y));
                    Stamina -= MovementCost;
                }
            }
            if (d == Utils.Direction.North) {
                if (Y < CurrentLevel.Height - 1 && !MissionLevel.IsObstruction(CurrentLevel.Map[X, Y + 1]) && CurrentLevel.GetEntityAt(X, Y + 1) == null) {
                    CurrentLevel.MoveEntityTo(this, new Point(X, Y + 1));
                    Stamina -= MovementCost;
                }
            }
            if (d == Utils.Direction.South) {
                if (Y > 0 && !MissionLevel.IsObstruction(CurrentLevel.Map[X, Y - 1]) && CurrentLevel.GetEntityAt(X, Y - 1) == null) {
                    CurrentLevel.MoveEntityTo(this, new Point(X, Y - 1));
                    Stamina -= MovementCost;
                }
            }
            if (Stamina < oldStamina) {
                bHasMoved = true;
                return true;
            }
            return false;
        }
        public bool AttackLocation(MissionLevel level, int tx, int ty, VisualEffect.EffectFactory effectFactory, Action<string> playSound, Action<string> showMessage) {
            // Check that we're attacking a square in range, or an entity part of which is in range
            if (RangeTo(tx, ty) > AttackRange) {
                IEntity en = level.GetEntityAt(tx, ty);
                if (en == null) return false;
                if (RangeTo(en) > AttackRange) return false;
            }
            if (!Visible[tx, ty]) return false;

            // Don't attack a friendly square
            if (level.IsFriendlyAt(tx, ty)) {
                return false;
            }

            Stamina -= AttackCost;
            bHasMoved = true;

            // Rotate soldier
            float dx = X - tx;
            float dy = Y - ty;
            SetFacing(180.0 + Math.Atan2(dy, dx) * (180.0 / Math.PI));
            // TODO refreshView();
            Thread.Sleep(100);

            // Play weapon sound
            if (EquippedWeapon == null) playSound("Punches");
            else playSound(EquippedWeapon.Type.SoundEffect);

            // Show the shot
            if (EquippedWeapon != null && !EquippedWeapon.Type.IsMeleeWeapon) {
                double pow = EquippedWeapon.DBase + (EquippedWeapon.DMod / 2.0);
                effectFactory(VisualEffect.EffectType.Shot, X, Y, new Dictionary<string, object>() { { "FX", X + 0.5 }, { "TX", tx + 0.5 }, { "FY", Y + 0.5 }, { "TY", ty + 0.5 }, { "Power", pow }, { "Colour", Color.FromArgb(255, 200, 200, 200) } });
            }

            int r = 0;
            HashSet<IEntity> hsAttacked = new HashSet<IEntity>();
            if (EquippedWeapon != null) r = (int)Math.Ceiling(EquippedWeapon.Type.Area);
            for (int y = Math.Max(0, ty - r); y <= Math.Min(level.Height - 1, ty + r); y++) {
                for (int x = Math.Max(0, tx - r); x <= Math.Min(level.Width - 1, tx + r); x++) {
                    int dr2 = (ty - y) * (ty - y) + (tx - x) * (tx - x);
                    if (dr2 > r * r) continue;
                    IEntity en = level.GetEntityAt(x, y);
                    if (en != null && !hsAttacked.Contains(en)) {
                        AttackEntity(en, effectFactory, playSound, showMessage);
                        hsAttacked.Add(en);
                    }
                }
            }
            return true;
        }
        private bool AttackEntity(IEntity en, VisualEffect.EffectFactory effectFactory, Action<string> playSound, Action<string> showMessage) {
            bHasMoved = true;
            double hit = Utils.GenerateHitRoll(this, en);
            if (hit <= 0.0) {
                if (en is Creature cre) cre.CheckChangeTarget(0.0, this);
                return false;
            }
            double TotalDam = en.InflictDamage(GenerateDamage());
            if (en is Creature cr) cr.CheckChangeTarget(TotalDam, this);

            // Graphics for damage
            int delay = (int)(RangeTo(en) * 25.0);
            if (EquippedWeapon == null || EquippedWeapon.Type.IsMeleeWeapon) delay += 250;
            Thread.Sleep(delay);
            effectFactory(VisualEffect.EffectType.Damage, en.X + (en.Size / 2.0), en.Y + (en.Size / 2.0), new Dictionary<string, object>() { { "Value", TotalDam } });

            // Play sound
            if (EquippedWeapon != null && EquippedWeapon.Type.Area == 0) playSound("Smash");

            // Apply effect?
            if (EquippedWeapon != null) {
                if (EquippedWeapon.Type.ItemEffect != null) {
                    en.ApplyEffectToEntity(this, EquippedWeapon.Type.ItemEffect, effectFactory);
                }
            }

            // Add weapon experience
            if (EquippedWeapon != null) {
                int exp = Math.Max(1, en.Level - Level) * Const.DEBUG_WEAPON_SKILL_MOD;
                int maxExp = Utils.SkillLevelToExperience(Level);
                int oldlvl = 0;
                if (WeaponExperience.ContainsKey(EquippedWeapon.Type)) {
                    oldlvl = Utils.ExperienceToSkillLevel(WeaponExperience[EquippedWeapon.Type]);
                    WeaponExperience[EquippedWeapon.Type] += exp;
                }
                else WeaponExperience.Add(EquippedWeapon.Type, exp);
                if (WeaponExperience[EquippedWeapon.Type] > maxExp) WeaponExperience[EquippedWeapon.Type] = maxExp; // Clamp at maximum for this level
                int newlvl = Utils.ExperienceToSkillLevel(WeaponExperience[EquippedWeapon.Type]);
                if (newlvl > oldlvl) {
                    showMessage("Soldier " + Name + " has gained level " + newlvl + " proficiency in " + EquippedWeapon.Type.Name);
                }
            }

            return true;
        }
        public void EndOfTurn(VisualEffect.EffectFactory fact, Action<IEntity> centreView, Action<string> playSound, Action<string> showMessage) {
            CheckForLevelUp(showMessage);
            //foreach (Effect e in Effects) {
            //  dStam += e.GetStatMod(StatType.Stamina);
            //}
            Stamina = MaxStamina;

            // Handle periodic effects
            foreach (Effect e in Effects) {
                bool bZoom = (!String.IsNullOrEmpty(e.SoundEffect) || e.Damage != 0.0);
                if (bZoom) {
                    // Zoom to this soldier & redraw
                    centreView(this);
                    Thread.Sleep(250);
                }
                if (!String.IsNullOrEmpty(e.SoundEffect)) playSound(e.SoundEffect);
                if (e.Damage != 0.0) {
                    Dictionary<WeaponType.DamageType, double> AllDam = new Dictionary<WeaponType.DamageType, double> { { e.DamageType, e.Damage } };
                    double TotalDam = InflictDamage(AllDam);
                    fact(VisualEffect.EffectType.Damage, X + (Size / 2.0), Y + (Size / 2.0), new Dictionary<string, object>() { { "Value", TotalDam } });
                    if (Health <= 0.0) return; // Dead. Abandon update.
                }
                if (bZoom) Thread.Sleep(750);
                e.ReduceDuration(1);
            }
            _Effects.RemoveAll(e => e.Duration <= 0);

            CalculateMaxStats(); // Just in case
            bHasMoved = false;
        }
        public List<string> PerformSearch(MissionLevel level) {
            bHasMoved = true;
            Stamina -= SearchCost;
            List<string> lFound = new List<string>();
            int rad = SearchRadius;
            Random rand = new Random();
            for (int y = Math.Max(0, Y - rad); y <= Math.Min(Y + rad, level.Height - 1); y++) {
                for (int x = Math.Max(0, X - rad); x <= Math.Min(X + rad, level.Width - 1); x++) {
                    if ((x - X) * (x - X) + (y - Y) * (y - Y) <= (rad * rad)) {
                        // Is there something hidden here?
                        bool bFound = false;
                        if (level.Map[x, y] == MissionLevel.TileType.SecretDoorHorizontal || level.Map[x, y] == MissionLevel.TileType.SecretDoorVertical) bFound = true;
                        Stash st = level.GetStashAtPoint(x, y);
                        if (st != null && st.Hidden) bFound = true;
                        Trap tr = level.GetTrapAtPoint(x, y);
                        if (tr != null && tr.Hidden) bFound = true;

                        // TODO Add hidden creatures, traps

                        if (!bFound) continue;

                        // If so, do we have a line of sight?
                        if (!level.CanSee(Location, new Point(x, y))) continue;

                        // If so then check if we spot it
                        double chance = BaseSearchChance - Math.Sqrt((x - X) * (x - X) + (y - Y) * (y - Y)) * Const.SearchReduction;
                        if (rand.NextDouble() * 100 <= chance) {
                            // Spotted
                            // Hidden door
                            if (level.Map[x, y] == MissionLevel.TileType.SecretDoorHorizontal) {
                                lFound.Add("You found a hidden door");
                                int xx = x;
                                while (xx > 0 && level.Map[xx, y] == MissionLevel.TileType.SecretDoorHorizontal) level.Map[xx--, y] = MissionLevel.TileType.DoorHorizontal;
                                xx = x + 1;
                                while (xx < level.Width && level.Map[xx, y] == MissionLevel.TileType.SecretDoorHorizontal) level.Map[xx++, y] = MissionLevel.TileType.DoorHorizontal;
                            }
                            if (level.Map[x, y] == MissionLevel.TileType.SecretDoorVertical) {
                                lFound.Add("You found a hidden door");
                                int yy = y;
                                while (yy > 0 && level.Map[x, yy] == MissionLevel.TileType.SecretDoorVertical) level.Map[x, yy--] = MissionLevel.TileType.DoorVertical;
                                yy = y + 1;
                                while (yy < level.Height && level.Map[x, yy] == MissionLevel.TileType.SecretDoorVertical) level.Map[x, yy++] = MissionLevel.TileType.DoorVertical;
                            }

                            // Hidden creature
                            // TODO

                            // Hidden treasure
                            if (st != null && st.Hidden) {
                                lFound.Add("You found a hidden stash of items");
                                st.Reveal();
                            }

                            // Hidden traps
                            if (tr != null && tr.Hidden) {
                                lFound.Add("You found a hidden trap");
                                tr.Reveal();
                            }
                        }
                    }
                }
            }
            return lFound;
        }
        public void UseItem(ItemType ActionItem) {
            if (ActionItem.ItemEffect.SingleUse) RemoveItemByType(ActionItem);
            Stamina -= UseItemCost;
            bHasMoved = true;
        }
        public void StopMission() {
            OnMission = false;
            _Effects.Clear();
        }

        // Bonuses from equipped items, effects etc.
        public int StatBonuses(StatType st) {
            int bonus = 0;
            foreach (Armour ar in EquippedArmour) {
                switch (st) {
                    case StatType.Strength: bonus += ar.Type.Strength; break;
                    case StatType.Agility: bonus += ar.Type.Agility; break;
                    case StatType.Intelligence: bonus += ar.Type.Intellect; break;
                    case StatType.Toughness: bonus += ar.Type.Toughness; break;
                    case StatType.Endurance: bonus += ar.Type.Endurance; break;
                    case StatType.Health: bonus += ar.Type.Health; break;
                    case StatType.Stamina: bonus += ar.Type.Stamina; break;
                    case StatType.Attack: bonus += ar.Type.Attack; break;
                    case StatType.Defence: bonus += ar.Type.Defence; break;
                    default: throw new NotImplementedException();
                }
            }

            // Check effects
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
        private double ShieldsFromItems() {
            double bonus = 0;
            foreach (Armour ar in EquippedArmour) {
                bonus += ar.Shields;
            }
            return bonus;
        }
        public bool HasUtilityItems() {
            foreach (IEquippable eq in Inventory.Keys.Where(e => e is IEquippable)) {
                ItemEffect ie = eq.BaseType.ItemEffect;
                if (ie != null) {
                    if (ie.AssociatedSkill == UtilitySkill.Unspent || !ie.SkillRequired || GetUtilityLevel(ie.AssociatedSkill) > 0) {
                        return true;
                    }
                }
            }
            return false;
        }
        public List<ItemType> GetUtilityItems() {
            List<ItemType> lItems = new List<ItemType>();
            foreach (IEquippable eq in Inventory.Keys.Where(e => e is IEquippable)) {
                if (eq is Weapon || eq is Armour) continue;
                ItemEffect ie = eq.BaseType.ItemEffect;
                if (ie != null) {
                    if (ie.AssociatedSkill == UtilitySkill.Unspent || !ie.SkillRequired || GetUtilityLevel(ie.AssociatedSkill) > 0) {
                        if (!lItems.Contains(eq.BaseType)) lItems.Add(eq.BaseType);
                    }
                }
            }
            return lItems;
        }
        public double DetectionRange {
            get {
                double range = Const.BaseDetectionRange + ((10.0 - Agility) / 2.0);
                range -= (GetUtilityLevel(UtilitySkill.Stealth) / 2.0);
                range += Encumbrance * 4.0;  // Encumbrance = [0,1]. Easily spotted if heavily encumbered.
                if (range < 1.0) range = 1.0;
                return range;
            }
        }

        // Display options
        public void DisplaySoldierDetails(ShaderProgram prog, float px, float py, bool bSelected, bool bHover) {
            float PanelHeight = GetGuiPanelHeight(bSelected);
            GL.Enable(EnableCap.Blend);

            // Background selection colour, if selected
            prog.SetUniform("textureEnabled", false);
            if (bSelected) prog.SetUniform("flatColour", new Vector4(0.3f, 0.3f, 0.3f, 0.8f));
            else prog.SetUniform("flatColour", new Vector4(0f, 0f, 0f, 0.6f));
            Matrix4 pScaleM = Matrix4.CreateScale(Const.GUIPanelWidth, PanelHeight, 1f);
            prog.SetUniform("model", pScaleM);
            GL.UseProgram(prog.ShaderProgramHandle);
            Square.Flat.BindAndDraw();

            // Hovering over this one
            if (bHover) {
                Matrix4 phTransM = Matrix4.CreateTranslation(-0.005f, -0.005f, 0f);
                Matrix4 phScaleM = Matrix4.CreateScale(Const.GUIPanelWidth + 0.01f, PanelHeight + 0.01f, 1f);
                prog.SetUniform("model", phScaleM * phTransM);
                prog.SetUniform("flatColour", new Vector4(0.2f, 1f, 0.4f, Const.GUIAlpha));
                GL.UseProgram(prog.ShaderProgramHandle);
                SquareRing.Thin.BindAndDraw();
            }

            // Frame
            prog.SetUniform("model", pScaleM);
            prog.SetUniform("flatColour", new Vector4(1f, 1f, 1f, Const.GUIAlpha));
            GL.UseProgram(prog.ShaderProgramHandle);
            Square.Lines.BindAndDraw();

            // Health/Stamina bars
            float TopBar = 0.045f;
            if (EquippedWeapon != null) TopBar += 0.02f;
            GraphicsFunctions.DisplayBicolourFractBar(prog, Const.GUIPanelWidth * 0.02f, TopBar, Const.GUIPanelWidth * 0.75f, 0.01f, (float)(Health / MaxHealth), new Vector4(0.3f, 1f, 0.3f, 1f), new Vector4(1f, 0f, 0f, 1f));
            GraphicsFunctions.DisplayBicolourFractBar(prog, Const.GUIPanelWidth * 0.02f, TopBar + 0.02f, Const.GUIPanelWidth * 0.75f, 0.01f, (float)(Stamina / MaxStamina), new Vector4(1f, 1f, 1f, 1f), new Vector4(0.6f, 0.6f, 0.6f, 1f));
            if (MaxShields > 0) GraphicsFunctions.DisplayBicolourFractBar(prog, Const.GUIPanelWidth * 0.02f, TopBar + 0.04f, Const.GUIPanelWidth * 0.75f, 0.01f, (float)(Shields / MaxShields), new Vector4(0.2f, 0.5f, 1f, 1f), new Vector4(0.2f, 0.2f, 0.2f, 1f));

            // Text summary
            const float TextScale = 0.015f;
            TextRenderOptions tro = new TextRenderOptions() {
                Alignment = Alignment.TopLeft,
                Aspect = PlayerTeam.CurrentMission.CurrentMapView.Aspect,
                TextColour = Color.White,
                XPos = px + 0.002f,
                YPos = py,
                ZPos = 0.015f,
                Scale = TextScale
            };
            TextRenderer.DrawWithOptions(Name, tro);
            tro.YPos += TextScale * 1.2f;
            TextRenderer.DrawWithOptions($"Level {Level} {Race.Name}", tro);
            if (EquippedWeapon != null) {
                tro.YPos += tro.Scale * 1.2f;
                tro.TextColour = Utils.LevelToColour(EquippedWeapon.Level);
                TextRenderer.DrawWithOptions(EquippedWeapon.Type.Name, tro);
            }

            tro.XPos = px + (Const.GUIPanelWidth / 2f);
            tro.YPos = py + TopBar + 0.02f;
            tro.Alignment = Alignment.TopMiddle;
            tro.Scale = TextScale * 0.7f;
            TextRenderer.DrawWithOptions($"{(int)Health}/{(int)MaxHealth}", tro);
            tro.YPos += 0.02f;
            TextRenderer.DrawWithOptions($"{(int)Stamina}/{(int)MaxStamina}", tro);
            if (MaxShields > 0) {
                tro.YPos += 0.02f;
                TextRenderer.DrawWithOptions($"{(int)Shields}/{(int)MaxShields}", tro);
            }

            return;

            // Encumbrance icon
            float encX = Const.GUIPanelWidth - 0.024f;
            float encY = 0.055f;
            double fract = Math.Min(1.0, CalculateInventoryMass() / MaximumCarry);
            GL.PushMatrix();
            GL.Translate(encX, encY, 0.0);
            GL.Color4(0.1, 1.0, 0.2, Const.GUIAlpha);
            GL.Begin(BeginMode.Quads);
            DrawEncumbranceSymbol(Math.Min(1.0, fract * 2.0));
            GL.End();
            if (fract > 0.5) {
                GL.Color4(1.0, 0.1, 0.0, Const.GUIAlpha);
                GL.Begin(BeginMode.Quads);
                DrawEncumbranceSymbol((fract - 0.5) * 2.0);
                GL.End();
            }
            GL.Color4(1.0, 1.0, 1.0, Const.GUIAlpha);
            GL.Begin(BeginMode.LineLoop);
            DrawEncumbranceSymbol(1.0);
            GL.End();
            GL.PopMatrix();

            GL.Disable(EnableCap.Blend);
        }
        private void DrawEncumbranceSymbol(double fract) {
            GL.Vertex2(0.0, 0.0);
            GL.Vertex2(0.021, 0.0);
            GL.Vertex2(0.021 - (0.003 * fract), -0.024 * fract);
            GL.Vertex2(0.003 * fract, -0.024 * fract);
        }
        public float GetGuiPanelHeight(bool bSelected) {
            int nrows = bSelected ? 8 : 4;
            if (MaxShields > 0) nrows++;
            if (EquippedWeapon != null) nrows++;
            return Const.GUIPanelRowHeight * nrows;
        }
        public override string ToString() {
            return Name;
        }
        public void SetPrimaryColour(Color col) {
            PrimaryColor = col;
        }
    }
}