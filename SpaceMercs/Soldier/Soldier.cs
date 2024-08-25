using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using SpaceMercs.Dialogs;
using SpaceMercs.Graphics;
using SpaceMercs.Graphics.Shapes;
using System.Collections.ObjectModel;
using System.IO;
using System.Xml;

namespace SpaceMercs {
    public class Soldier : IEntity {
        public enum UtilitySkill { Unspent, Medic, Engineer, Gunsmith, Armoursmith, Bladesmith, Avoidance, Stealth, Scavenging, Perception, Sharpshooter }

        // Generic stuff
        public Team? PlayerTeam; // Could be null (if an unhired mercenary)
        public AstronomicalObject? aoLocation; // Usually ignored, unless soldier is deactivated
        private int iTextureID = -1;
        public bool IsActive { get; private set; }
        public string SoldierID {
            get {
                return Name + "/" + Level + "/" + Race.Name + "/" + Gender + "/" + (BaseStrength ^ BaseAgility ^ BaseInsight ^ BaseToughness ^ BaseEndurance);
            }
        }
        public Point GoTo { get; set; }
        public bool OnMission { get; set; }
        private readonly Random rnd;
        public bool HasMoved { get; private set; } = false;

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
        public Weapon? EquippedWeapon { get; private set; }
        public int Size { get { return 1; } }
        public int TravelRange { get { /* if (Encumbrance >= 1.0) return 0; */ return (int)Math.Floor(Stamina / MovementCost); } }
        public double AttackRange { get { if (EquippedWeapon == null) return 1.0; return EquippedWeapon.Range; } }
        public double Facing { get; private set; }
        public Point Location { get { return new Point(X, Y); } }
        private readonly List<Effect> _Effects = new List<Effect>();
        public IEnumerable<Effect> Effects { get { return _Effects.AsReadOnly(); } }
        private bool[,] SightMap;
        public Color PrimaryColor { get; private set; }
        public bool IsInjured { get { return Health < MaxHealth; } }

        public bool CanSee(int x, int y) { if (x < 0 || y < 0 || x >= SightMap.GetLength(0) || y >= SightMap.GetLength(1)) return false; return SightMap[x, y]; }
        public bool CanSee(IEntity? en) {
            if (en == null) return false;
            for (int yy = en.Y; yy < en.Y + en.Size; yy++) {
                for (int xx = en.X; xx < en.X + en.Size; xx++) {
                    if (SightMap[xx, yy]) return true;
                }
            }
            return false;
        }
        public bool CouldSeeEntityAtLocation(IEntity en, Point p) {
            for (int yy = p.Y; yy < p.Y + en.Size; yy++) {
                for (int xx = p.X; xx < p.X + en.Size; xx++) {
                    if (SightMap[xx, yy]) return true;
                }
            }
            return false;
        }
        public void UpdateVisibility(MissionLevel m) {
            SightMap = m.CalculateVisibilityFromEntity(this);
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
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

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
            HasMoved = false;
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
            double red = 100.0; // TODO CHECK
            foreach (Armour ar in EquippedArmour) {
                red -= ar.GetDamageReductionByDamageType(type);
            }
            if (red < 0.0) red = 0.0; // Clamp this for Soldiers (i.e. no reducing the damage so much it flips to healing)
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
                if (dam > 0.0) TotalDam += dam * GetDamageReductionByDamageType(type); // Actual damage
                else TotalDam += dam; // Negative damage = healing
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
            foreach (IGrouping<IItem, IItem> g in Inventory.GroupBy(x => x)) {
                st.Add(g.Key, g.Count());
            }
            Inventory.Clear();

            return st;
        }
        public void KillEntity() {
            Health = 0.0;
            IsActive = false;
            CurrentLevel.KillSoldier(this);
        }
        public Dictionary<WeaponType.DamageType, double> GenerateDamage(int nhits) {
            // Utils.HitToMod(hit); // Damage modifier for quality of hit
            Dictionary<WeaponType.DamageType, double> AllDam = new Dictionary<WeaponType.DamageType, double>();
            if (EquippedWeapon == null) {
                double dam = 0.0;
                for (int n = 0; n < nhits; n++) {
                    dam += (rnd.NextDouble() + 0.5) * Const.SoldierAttackDamageScale * Attack;  // Unarmed melee does rubbish damage, in general
                }
                AllDam.Add(WeaponType.DamageType.Physical, dam);
            }
            else {
                double hmod = Attack * Const.SoldierAttackDamageScale; // 10% damage bonus per attack point (includes weapon skill etc.)
                double dam = 0.0;
                for (int n = 0; n < nhits; n++) {
                    dam += EquippedWeapon.DBase + (rnd.NextDouble() * EquippedWeapon.DMod);
                }
                AllDam.Add(EquippedWeapon.Type.DType, dam * hmod);
                foreach (KeyValuePair<WeaponType.DamageType, double> bdam in EquippedWeapon.GetBonusDamage()) {
                    if (AllDam.ContainsKey(bdam.Key)) AllDam[bdam.Key] += bdam.Value * hmod * nhits;
                    else AllDam.Add(bdam.Key, bdam.Value * hmod * nhits);
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
            float TotalDam = (float)InflictDamage(AllDam);
            if (TotalDam > 0.0) fact(VisualEffect.EffectType.Damage, X + (Size / 2f), Y + (Size / 2f), new Dictionary<string, object>() { { "Value", TotalDam } });
            else if (TotalDam < 0.0) fact(VisualEffect.EffectType.Healing, X + (Size / 2f), Y + (Size / 2f), new Dictionary<string, object>() { { "Value", -TotalDam } });
            if (ie.CurePoison) {
                _Effects.RemoveAll(e => e.DamageType == WeaponType.DamageType.Poison);
            }
            CalculateMaxStats();
        }

        // Inventory
        private readonly List<IItem> Inventory = new List<IItem>();
        public ReadOnlyDictionary<IItem, int> InventoryGrouped { 
            get 
            {
                return new ReadOnlyDictionary<IItem, int>(Inventory.GroupBy(x => x).ToDictionary(x => x.Key, x => x.Count())); 
            } 
        }
        public List<Armour> EquippedArmour = new List<Armour>();

        // Base stats
        public GenderType Gender { get; private set; }
        public Race Race { get; private set; }
        public int BaseStrength { get; private set; }
        public int BaseAgility { get; private set; }
        public int BaseInsight { get; private set; }
        public int BaseToughness { get; private set; }
        public int BaseEndurance { get; private set; }
        public int Experience { get; private set; }

        // Calculated stats
        public int Strength { get { return Math.Max(0, BaseStrength + StatBonuses(StatType.Strength)); } }
        public int Agility { get { return Math.Max(0, BaseAgility + StatBonuses(StatType.Agility)); } }
        public int Insight { get { return Math.Max(0, BaseInsight + StatBonuses(StatType.Insight)); } }
        public int Toughness { get { return Math.Max(0, BaseToughness + StatBonuses(StatType.Toughness)); } }
        public int Endurance { get { return Math.Max(0, BaseEndurance + StatBonuses(StatType.Endurance)); } }
        public int BaseAttack { get { return (MeleeAttacker ? Strength : Insight) + Level + 2; } }
        public int BaseDefence { get { return Agility + Level + 2; } }
        public int BaseHealth { get { return Toughness + Level + 10; } }
        public int BaseStamina { get { return Endurance + Level + 10; } }
        public double StaminaRegen {  get { return MaxStamina - Level; } }
        public double MaximumCarry { get { return (Math.Pow(Strength, 1.55) * Const.MaxCarryScale) + 4.0; } }
        public double Encumbrance {
            get {
                double m = CalculateInventoryMass();
                if (m * 2.0 <= MaximumCarry) return 0.0;
                return Math.Min(1.0, (m - (MaximumCarry / 2.0)) * 2.0 / MaximumCarry);
            }
        }
        public bool MeleeAttacker { get { return (EquippedWeapon is null || EquippedWeapon.Type.IsMeleeWeapon); } }
        public double MovementCost { get { return Const.MovementCost * (1.0 + Encumbrance) / SpeedModifier(); } }
        public double SearchCost { get { return Const.SearchCost; } }
        public double AttackCost { get { if (EquippedWeapon == null) return Const.MeleeCost; return EquippedWeapon.StaminaCost; } }
        public double UseItemCost { get { return Const.UseItemCost; } }
        private MissionLevel CurrentLevel { get { return PlayerTeam?.CurrentMission?.GetOrCreateCurrentLevel() ?? throw new Exception("CurrentLevel doesn't exist"); } }
        public double SearchRadius { get { return Const.BaseSearchRadius + GetUtilityLevel(UtilitySkill.Perception) * Const.PerceptionSearchRadiusBoost; } }
        public double PassiveSearchRadius { get { return Const.PassiveSearchRadius + GetUtilityLevel(UtilitySkill.Perception) * Const.PerceptionSearchRadiusBoost; } }
        public double BaseSearchChance { get { return Const.BaseSearchChance + GetUtilityLevel(UtilitySkill.Perception) * Const.SearchBoostPerSkill + Insight; } }
        public double PassiveSearchChance { get { return Const.PassiveSearchChance + GetUtilityLevel(UtilitySkill.Perception) * Const.SearchBoostPerSkill + Insight; } }
        private void CalculateMaxStats() {
            MaxHealth = BaseHealth + StatBonuses(StatType.Health);
            if (Health > MaxHealth) Health = MaxHealth;
            MaxStamina = BaseStamina + StatBonuses(StatType.Stamina);
            if (Stamina > MaxStamina) Stamina = MaxStamina;
            MaxShields = ShieldsFromItems();
            if (Shields > MaxShields) Shields = MaxShields;
            Attack = BaseAttack + StatBonuses(StatType.Attack) + GetSoldierSkillWithWeapon(EquippedWeapon?.Type);
            Defence = BaseDefence + GetUtilityLevel(UtilitySkill.Avoidance) + StatBonuses(StatType.Defence);
        }

        // Skills & experience
        private readonly Dictionary<WeaponType.WeaponClass, int> WeaponExperience = new Dictionary<WeaponType.WeaponClass, int>();
        private readonly Dictionary<UtilitySkill, int> UtilitySkills = new Dictionary<UtilitySkill, int>();
        public IReadOnlyList<WeaponType.WeaponClass> SkilledWeaponClasses {
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
        public int GetSoldierSkillWithWeaponClass(WeaponType.WeaponClass wp) {
            if (!WeaponExperience.ContainsKey(wp)) return 0;
            int lvl = Utils.ExperienceToSkillLevel(WeaponExperience[wp]);
            if (lvl > Level) return Level;
            else return lvl;
        }
        public int GetSoldierSkillWithWeapon(WeaponType? wp) {
            if (wp is null) return 0;
            if (!WeaponExperience.ContainsKey(wp.WClass)) return 0;
            int lvl = Utils.ExperienceToSkillLevel(WeaponExperience[wp.WClass]);
            if (lvl > Level) return Level;
            else return lvl;
        }
        public int GetUtilityLevel(UtilitySkill sk) {
            int val = GetRawUtilityLevel(sk);
            if (EquippedWeapon != null) val += EquippedWeapon.GetUtilitySkill(sk);
            foreach (Armour ar in EquippedArmour) {
                val += ar.GetUtilitySkill(sk);
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
        public void CheckForLevelUp(Action<string, Action?> showMessage) {
            if (Experience >= ExperienceRequiredToReachNextLevel()) {
                Level++;
                AddUtilitySkill(UtilitySkill.Unspent);
                showMessage($"Congratulations! Soldier {Name} has reached level {Level}", () => UpgradeStat(this));
            }
        }
        private static void UpgradeStat(Soldier s) {
            ChooseStat cs = new ChooseStat(s);
            cs.ShowDialog(new Form { TopMost = true });
            s.CalculateMaxStats();
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
                object randomSkill = values.GetValue(rnd.Next(values.Length)) ?? throw new Exception("Could not identify random utility skill");
                UtilitySkill sk = (UtilitySkill)randomSkill;
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
            if (EquippedWeapon != null) WeaponExperience.Add(EquippedWeapon.Type.WClass, val);
        }
        public void IncreaseStat(StatType tp, int val = 1) {
            switch (tp) {
                case StatType.Strength: BaseStrength += val; break;
                case StatType.Agility: BaseAgility += val; break;
                case StatType.Insight: BaseInsight += val; break;
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
            BaseInsight = Int;
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
        public Soldier(XmlNode xml, Team? pt) {
            PlayerTeam = pt;
            iTextureID = -1;
            Name = xml.GetAttributeText("Name");
            rnd = new Random(Name.GetHashCode());
            XmlNode? xac = xml.SelectSingleNode("Inactive");
            if (xac is null) {
                IsActive = true;
                if (pt is not null) aoLocation = pt.CurrentPosition;
            }
            else {
                IsActive = false;
                string location = xac.Attributes!["Loc"]?.Value ?? throw new Exception($"Could not ID location of soldier {Name} = (null)");
                if (pt is not null) aoLocation = pt.CurrentPosition.GetSystem().Sector.ParentMap.GetAOFromLocationString(location);
                if (aoLocation is null) throw new Exception($"Could not ID location of soldier {Name} = {location}");
            }
            OnMission = (xml.SelectSingleNode("OnMission") != null);

            XmlNode? xmll = xml.SelectSingleNode("Location");
            if (xmll is not null) {
                X = xmll.GetAttributeInt("X");
                Y = xmll.GetAttributeInt("Y");
            }
            else X = Y = 0;
            Level = xml.SelectNodeInt("Level");
            Gender = xml.SelectNodeEnum<GenderType>("Gender");
            string raceName = xml.SelectSingleNode("Race")?.InnerText ?? "";
            Race = StaticData.GetRaceByName(raceName) ?? throw new Exception($"Unrecognised Race in Soldier data : {raceName}");
            if (Race == null) throw new Exception("Could not ID Soldier " + Name + " Race : " + xml.SelectNodeText("Race"));
            string[] stats = xml.SelectNodeText("Stats").Split(',');
            if (stats.Length != 5) throw new Exception($"Could not understand stats string for Soldier {Name}");
            BaseStrength = int.Parse(stats[0]);
            BaseAgility = int.Parse(stats[1]);
            BaseInsight = int.Parse(stats[2]);
            BaseToughness = int.Parse(stats[3]);
            BaseEndurance = int.Parse(stats[4]);
            Experience = xml.SelectNodeInt("XP");
            Health = xml.SelectNodeDouble("Health", MaxHealth);
            Stamina = xml.SelectNodeDouble("Stamina", MaxStamina);
            Shields = xml.SelectNodeDouble("Health", MaxHealth);
            // Facing - backwards compatibility in case it's an angle or an enum
            if (xml.SelectSingleNode("Facing") != null) {
                if (double.TryParse(xml.SelectNodeText("Facing"), out double fac)) {
                    Facing = fac;
                }
                else {
                    SetFacing((Utils.Direction)Enum.Parse(typeof(Utils.Direction), xml.SelectNodeText("Facing")));
                }
            }
            else Facing = 90.0;

            if (xml.SelectSingleNode("Colour") != null) {
                PrimaryColor = ColorTranslator.FromHtml(xml.SelectNodeText("Colour"));
            }
            else PrimaryColor = Color.Blue;

            HasMoved = (xml.SelectSingleNode("Moved") != null);

            XmlNode? xg = xml.SelectSingleNode("GoTo");
            if (xg is not null) {
                int gx = xg.GetAttributeInt("X");
                int gy = xg.GetAttributeInt("Y");
                GoTo = new Point(gx, gy);
            }
            else GoTo = Point.Empty;

            XmlNode? xmli = xml.SelectSingleNode("Inventory");
            Inventory.Clear();
            if (xmli is not null) {
                foreach (XmlNode xi in xmli.ChildNodes) {
                    int count = xi.GetAttributeInt("Count");
                    IItem? eq = Utils.LoadItem(xi.FirstChild);
                    if (eq is not null) {
                        Inventory.AddRange(Enumerable.Repeat(eq, count));
                    }
                }
            }

            XmlNode? xmlar = xml.SelectSingleNode("EquippedArmour");
            EquippedArmour.Clear();
            if (xmlar is not null) {
                foreach (XmlNode xar in xmlar.ChildNodes) {
                    Armour ar = new Armour(xar);
                    EquippedArmour.Add(ar);
                }
            }

            XmlNode? xmlwp = xml.SelectSingleNode("EquippedWeapon");
            EquippedWeapon = null;
            if (xmlwp?.FirstChild is not null) {
                EquippedWeapon = new Weapon(xmlwp.FirstChild);
            }

            XmlNode? wex = xml.SelectSingleNode("WeaponExperience");
            WeaponExperience.Clear();
            if (wex is not null) {
                foreach (XmlNode xw in wex.SelectNodesToList("Exp")) {
                    int exp = int.Parse(xw.InnerText);
                    // Previously we stored this per weapon. Now we do it per WeaponClass, so for reverse compatibility, we attempt to fallback to weapontype.
                    if (Enum.TryParse<WeaponType.WeaponClass>(xw.GetAttributeText("Type"), out var wc)) {
                        if (WeaponExperience.ContainsKey(wc)) WeaponExperience[wc] += exp;
                        else WeaponExperience.Add(wc, exp);
                    }
                    else {
                        WeaponType? tp = StaticData.GetWeaponTypeByName(xw.GetAttributeText("Type")) ?? throw new Exception("Could not ID WeaponType : " + xw.GetAttributeText("Type"));
                        if (WeaponExperience.ContainsKey(tp.WClass)) WeaponExperience[tp.WClass] += exp;
                        else WeaponExperience.Add(tp.WClass, exp);
                    }
                }
            }

            XmlNode? wut = xml.SelectSingleNode("UtilityExperience");
            UtilitySkills.Clear();
            int totsk = 0;
            if (wut is not null) {
                foreach (XmlNode xu in wut.SelectNodesToList("Exp")) {
                    string skillName = xu.GetAttributeText("Skill");
                    if (skillName == "Sniper") skillName = nameof(UtilitySkill.Sharpshooter); // Backwards compatibility
                    if (skillName == "Eyesight") skillName = nameof(UtilitySkill.Sharpshooter); // Backwards compatibility
                    UtilitySkill sk = (UtilitySkill)Enum.Parse(typeof(UtilitySkill), skillName);
                    int lvl = int.Parse(xu.InnerText);
                    totsk += lvl;
                    UtilitySkills.Add(sk, lvl);
                }
            }
            if (totsk < Level + 1) {
                AddUtilitySkill(UtilitySkill.Unspent, Level + 1 - totsk); // Make sure we've got our unspent points
            }

            XmlNode? xmlef = xml.SelectSingleNode("Effects");
            _Effects.Clear();
            if (xmlef is not null) {
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
            Stats[2] = cl.Owner.Insight;
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
            if (!IsActive) file.WriteLine(" <Inactive Loc=\"" + aoLocation?.PrintCoordinates() + "\"/>");
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
            file.WriteLine(" <Stats>" + BaseStrength + "," + BaseAgility + "," + BaseInsight + "," + BaseToughness + "," + BaseEndurance + "</Stats>");
            if (GoTo != Point.Empty && GoTo != Location) file.WriteLine(" <GoTo X=\"" + GoTo.X + "\" Y=\"" + GoTo.Y + "\"/>");
            file.WriteLine(" <Colour>" + ColorTranslator.ToHtml(PrimaryColor) + "</Colour>");

            if (Inventory.Count > 0) {
                file.WriteLine(" <Inventory>");
                foreach (KeyValuePair<IItem, int> kvp in InventoryGrouped) {
                    file.WriteLine($"  <Inv Count=\"{kvp.Value}\">");
                    kvp.Key.SaveToFile(file);
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
                foreach (WeaponType.WeaponClass tp in WeaponExperience.Keys) {
                    file.WriteLine($"  <Exp Type=\"{tp}\">{WeaponExperience[tp]}</Exp>");
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

            if (HasMoved) file.WriteLine(" <Moved/>");

            file.WriteLine("</Soldier>");
        }

        // How much to hire this soldier as a mercenary? (ignoring race relations)
        public double HireCost() {
            double dBase = Level * Math.Pow(Const.MercenaryCostBase, Const.MercenaryCostExponent * Level) * Const.MercenaryCostScale;
            
            // Bonus cost if they are better than average
            int Bonus = (BaseStrength + BaseAgility + BaseInsight + BaseToughness + BaseEndurance) - (Race.Strength + Race.Agility + Race.Insight + Race.Toughness + Race.Endurance);
            if (Bonus < 1) Bonus = 0;
            else Bonus++;
            double dStats = 1.0 + ((Bonus * Bonus) / 30.0);

            // Bonuses for large individual scores
            if (BaseStrength - Race.Strength > 3) dStats += Math.Pow(BaseStrength - Race.Strength - 2, 2) / 25.0;
            if (BaseAgility - Race.Agility > 3) dStats += Math.Pow(BaseAgility - Race.Agility - 2, 2) / 25.0;
            if (BaseInsight - Race.Insight > 3) dStats += Math.Pow(BaseInsight - Race.Insight - 2, 2) / 25.0;
            if (BaseToughness - Race.Toughness > 3) dStats += Math.Pow(BaseToughness - Race.Toughness - 2, 2) / 25.0;
            if (BaseEndurance - Race.Endurance > 3) dStats += Math.Pow(BaseEndurance - Race.Endurance - 2, 2) / 25.0;

            // Additional cost for teh mercenary's equipment
            double dKit = EquipmentCost();

            // Final cost
            return (dBase * dStats) + (dKit * Const.MercenaryKitValueScale);
        }
        public double EquipmentCost() {
            double dCost = 0.0;
            foreach (IItem eq in Inventory) {
                dCost += eq.Cost;
            }
            foreach (Armour ar in EquippedArmour) {
                dCost += ar.Cost;
            }
            if (EquippedWeapon != null) dCost += EquippedWeapon.Cost;
            return dCost;
        }

        #region Inventory
        public Armour? GetArmourAtLocation(BodyPart bp) {
            foreach (Armour ar in EquippedArmour) {
                if (ar.Type.Locations.Contains(bp)) return ar;
            }
            return null;
        }
        public double CalculateInventoryMass() {
            double w = 0.0;
            foreach (IItem eq in Inventory) w += eq.Mass;
            foreach (Armour ar in EquippedArmour) w += ar.Mass;
            if (EquippedWeapon != null) w += EquippedWeapon.Mass;
            return w;
        }
        public bool Equip(IEquippable eq) {
            if (!Inventory.Contains(eq)) return false;
            if (!(eq is Armour || eq is Weapon)) return false;
            Inventory.Remove(eq);
            if (eq is Armour ar) {
                HashSet<Armour> hsToUnequip = new HashSet<Armour>();
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
        public void Unequip(IEquippable? eq) {
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
            for (int i = 0; i < Count; i++) {
                if (Inventory.Contains(it)) {
                    PlayerTeam?.AddItem(it);
                    Inventory.Remove(it);
                }
            }
            CalculateMaxStats();
        }
        public void DropAll(IItem it) {
            while (Inventory.Contains(it)) { 
                PlayerTeam?.AddItem(it);
                Inventory.Remove(it);
            }
            CalculateMaxStats();
        }
        public void DestroyItem(IItem it, int Count = 1) {
            int tally = 0;
            while (Inventory.Contains(it)) {
                Inventory.Remove(it);
                tally++;
                if (tally >= Count) return;
            }
            CalculateMaxStats();
        }
        public void RemoveItemByType(ItemType it, int Count = 1) {
            int tally = 0;
            foreach (IEquippable eq in Inventory.Where(e => e is IEquippable)) {
                if (eq.BaseType == it) {
                    DestroyItem(eq);
                    tally++;
                    if (tally >= Count) return;
                }
            }
        }
        public void AddItem(IItem it, int Count = 1) {
            if (it == null || Count < 1) return;
            Inventory.AddRange(Enumerable.Repeat(it,Count));
            CalculateMaxStats();
        }
        public double BaseArmour {
            get {
                double arm = 0.0;
                foreach (Armour ar in EquippedArmour) {
                    arm += ar.BaseArmour;
                }
                foreach (Effect eff in Effects) {
                    arm += eff.ArmourMod;
                }
                if (arm < 0.0) arm = 0.0;
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
                EquippedWeapon = Utils.GenerateRandomWeapon(rnd, Level, Race);
            } while (EquippedWeapon is null || EquippedWeapon.StaminaCost > MaxStamina);

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
                IItem? eq = Utils.GenerateRandomItem(rnd, Level, Race, false);
                if (eq is not null) AddItem(eq);
            }
        }
        private void SetupBasicArmour() {
            Armour? chest = PickRandomBaseArmourForLocation(BodyPart.Chest);
            if (chest is not null && rnd.NextDouble() < 0.8) EquippedArmour.Add(chest);
            foreach (BodyPart bp in Enum.GetValues(typeof(BodyPart))) {
                if (rnd.NextDouble() < 0.3 && GetArmourAtLocation(bp) == null) {
                    Armour? arm = PickRandomBaseArmourForLocation(bp);
                    if (arm is not null) EquippedArmour.Add(arm);
                }
            }
        }
        private Armour? PickRandomBaseArmourForLocation(BodyPart bp) {
            MaterialType? mat = null;
            // Get base material
            foreach (MaterialType m in StaticData.Materials) {
                if (m.IsArmourMaterial && (mat is null || m.Rarity > mat.Rarity)) mat = m; // Pick the most common material
            }
            if (mat is null) return null;
            // Get base armour for this location
            ArmourType? choice = null;
            foreach (ArmourType at in StaticData.ArmourTypes) {
                if (at.Locations.Contains(bp) && (choice is null || at.Rarity > choice.Rarity)) {
                    bool bOK = true;
                    foreach (BodyPart abp in at.Locations) {
                        if (GetArmourAtLocation(abp) is not null) { bOK = false; break; }
                    }
                    if (bOK) choice = at;
                }
            }
            if (choice is null) return null;
            return new Armour(choice, mat, 0);
        }
        private void UpgradeArmourAtRandom() {
            // Pick a random location
            BodyPart bp = Utils.GetRandomBodyPart();
            Armour? ar = GetArmourAtLocation(bp);
            // Try to fill in empty armour slots if at all possible by having another go at finding an empty slot.
            if (ar is not null) {
                bp = Utils.GetRandomBodyPart();
                ar = GetArmourAtLocation(bp);
            }
            // If armour doesn't exist at this location, then generate something
            if (ar is null) {
                ar = PickRandomBaseArmourForLocation(bp);
                if (ar is not null) EquippedArmour.Add(ar);
            }
            // Otherwise upgrade what's there
            else {
                ar.UpgradeArmour(Race);
                CalculateMaxStats();
            }
        }
        public bool HasItem(IItem it) {
            return Inventory.Contains(it);
        }
        #endregion // Inventory

        // Is this soldier active in the team?
        public void Activate() {
            if (aoLocation != PlayerTeam?.CurrentPosition) return;
            IsActive = true;
            // TODO: Anything else we want to do?
        }
        public void Deactivate() {
            IsActive = false;
            aoLocation = PlayerTeam?.CurrentPosition;
            // TODO: Anything else we want to do?
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
                HasMoved = true;
                return true;
            }
            return false;
        }
        public bool AttackLocation(MissionLevel level, int tx, int ty, VisualEffect.EffectFactory effectFactory, Action<string> playSound, Action<string, Action?> showMessage) {
            if (level is null) throw new Exception("Null level in AttackLocation");
            if (Stamina < AttackCost) return false;
            // Check that we're attacking a square in range, or an entity part of which is in range
            if (RangeTo(tx, ty) > AttackRange) {
                IEntity? en = level.GetEntityAt(tx, ty);
                if (en == null) return false;
                if (RangeTo(en) > AttackRange) return false;
            }
            if (!SightMap[tx, ty]) return false;

            // Don't attack a friendly square
            if (level.IsFriendlyAt(tx, ty)) {
                return false;
            }

            Stamina -= AttackCost;
            HasMoved = true;

            // Rotate soldier
            float dx = X - tx;
            float dy = Y - ty;
            SetFacing(180.0 + Math.Atan2(dy, dx) * (180.0 / Math.PI));
            Thread.Sleep(100);

            // Play weapon sound
            if (EquippedWeapon == null) playSound("Punches");
            else playSound(EquippedWeapon.Type.SoundEffect);

            // Show the shot
            if (EquippedWeapon != null && !EquippedWeapon.Type.IsMeleeWeapon) {
                float pow = (float)(EquippedWeapon.DBase + (EquippedWeapon.DMod / 2.0));
                effectFactory(VisualEffect.EffectType.Shot, X, Y, new Dictionary<string, object>() { { "FX", X + 0.5f }, { "TX", tx + 0.5f }, { "FY", Y + 0.5f }, { "TY", ty + 0.5f }, { "Power", pow }, { "Colour", Color.FromArgb(255, 200, 200, 200) } });
            }

            int r = 0;
            HashSet<IEntity> hsAttacked = new HashSet<IEntity>();
            if (EquippedWeapon != null) r = (int)Math.Ceiling(EquippedWeapon.Type.Area);
            for (int y = Math.Max(0, ty - r); y <= Math.Min(level.Height - 1, ty + r); y++) {
                for (int x = Math.Max(0, tx - r); x <= Math.Min(level.Width - 1, tx + r); x++) {
                    int dr2 = (ty - y) * (ty - y) + (tx - x) * (tx - x);
                    if (dr2 > r * r) continue;
                    IEntity? en = level.GetEntityAt(x, y);
                    if (en != null && !hsAttacked.Contains(en)) {
                        AttackEntity(en, effectFactory, playSound, showMessage);
                        hsAttacked.Add(en);
                    }
                }
            }
            return true;
        }
        private bool AttackEntity(IEntity targetEntity, VisualEffect.EffectFactory effectFactory, Action<string> playSound, Action<string, Action?> showMessage) {
            HasMoved = true;
            int nhits = 0, nshots = EquippedWeapon?.Type?.Shots ?? 1;
            for (int n = 0; n < nshots; n++) {                
                double hit = Utils.GenerateHitRoll(this, targetEntity);
                if (hit > 0.0) nhits++;
            }
            if (nhits == 0) {
                if (targetEntity is Creature cre) cre.CheckChangeTarget(0.0, this);
                return false;
            }
            double TotalDam = targetEntity.InflictDamage(GenerateDamage(nhits));
            if (targetEntity is Creature cr) cr.CheckChangeTarget(TotalDam, this);

            // Graphics for damage
            int delay = (int)(RangeTo(targetEntity) * 25.0);
            if (EquippedWeapon == null || EquippedWeapon.Type.IsMeleeWeapon) delay += 250;
            Thread.Sleep(delay);
            effectFactory(VisualEffect.EffectType.Damage, targetEntity.X + (targetEntity.Size / 2f), targetEntity.Y + (targetEntity.Size / 2f), new Dictionary<string, object>() { { "Value", TotalDam } });

            // Play sound
            if (EquippedWeapon != null && EquippedWeapon.Type.Area == 0) playSound("Smash");

            // Apply effect?
            if (EquippedWeapon != null) {
                if (EquippedWeapon.Type.ItemEffect != null) {
                    targetEntity.ApplyEffectToEntity(this, EquippedWeapon.Type.ItemEffect, effectFactory);
                }
            }

            // Add weapon experience
            if (EquippedWeapon != null) {
                int exp = Math.Max(1, targetEntity.Level - Level) * Const.DEBUG_WEAPON_SKILL_MOD;
                int maxExp = Utils.SkillLevelToExperience(Level);
                int oldlvl = 0;
                if (WeaponExperience.ContainsKey(EquippedWeapon.Type.WClass)) {
                    oldlvl = Utils.ExperienceToSkillLevel(WeaponExperience[EquippedWeapon.Type.WClass]);
                    WeaponExperience[EquippedWeapon.Type.WClass] += exp;
                }
                else WeaponExperience.Add(EquippedWeapon.Type.WClass, exp);
                if (WeaponExperience[EquippedWeapon.Type.WClass] > maxExp) WeaponExperience[EquippedWeapon.Type.WClass] = maxExp; // Clamp at maximum for this level
                int newlvl = Utils.ExperienceToSkillLevel(WeaponExperience[EquippedWeapon.Type.WClass]);
                if (newlvl > oldlvl) {
                    showMessage($"Soldier {Name} has gained level {newlvl} proficiency in {EquippedWeapon.Type.WClass}", null);
                }
            }

            return true;
        }
        public void EndOfTurn(VisualEffect.EffectFactory fact, Action<IEntity> centreView, Action<string> playSound, Action<string, Action?> showMessage) {
            CheckForLevelUp(showMessage);
            // Increase Stamina by Endurance + 10 + Bonuses. (i.e. MaxStamina - Level)
            // *OR* Just set to max, instead of recovering a subset of stamina each turn based on Endurance & equipment??
            Stamina = Math.Min(Stamina + StaminaRegen, MaxStamina); 

            // Handle periodic effects
            foreach (Effect e in Effects) {
                bool bZoom = (!String.IsNullOrEmpty(e.SoundEffect) || e.Damage != 0.0);
                if (bZoom) {
                    // Zoom to this soldier & redraw
                    centreView(this);
                    Thread.Sleep(250);
                }
                if (!string.IsNullOrEmpty(e.SoundEffect)) playSound(e.SoundEffect);
                if (e.Damage != 0.0) {
                    Dictionary<WeaponType.DamageType, double> AllDam = new Dictionary<WeaponType.DamageType, double> { { e.DamageType, e.Damage } };
                    double TotalDam = InflictDamage(AllDam);
                    fact(VisualEffect.EffectType.Damage, X + (Size / 2f), Y + (Size / 2f), new Dictionary<string, object>() { { "Value", TotalDam } });
                    if (Health <= 0.0) return; // Dead. Abandon update.
                }
                if (bZoom) Thread.Sleep(750);
                e.ReduceDuration(1);
            }
            _Effects.RemoveAll(e => e.Duration <= 0);

            // Handle any items in inventory that reset/recharge etc.
            foreach (IEquippable eq in Inventory.OfType<IEquippable>()) {
                eq.EndOfTurn();
            }

            CalculateMaxStats(); // Just in case
            HasMoved = false;
        }
        public List<string> PerformActiveSearch(MissionLevel level) {
            HasMoved = true;
            Stamina -= SearchCost;
            return SearchTheArea(level, false);
        }
        public List<string> SearchTheArea(MissionLevel level, bool bPassive) { 
            List<string> lFound = new List<string>();
            double rad = bPassive ? PassiveSearchRadius : SearchRadius;
            double dPenalty = Const.EncumbranceSearchPenalty * Encumbrance;
            double baseChance = (bPassive ? PassiveSearchChance : BaseSearchChance) - dPenalty;
            Random rand = new Random();
            for (int y = Math.Max(0, Y - (int)rad); y <= Math.Min(Y + rad, level.Height - 1); y++) {
                for (int x = Math.Max(0, X - (int)rad); x <= Math.Min(X + rad, level.Width - 1); x++) {
                    if ((x - X) * (x - X) + (y - Y) * (y - Y) <= (rad * rad)) {
                        // Is there something hidden here?
                        bool bFound = false;
                        if (level.Map[x, y] == MissionLevel.TileType.SecretDoorHorizontal || level.Map[x, y] == MissionLevel.TileType.SecretDoorVertical) bFound = true;
                        Stash? st = level.GetStashAtPoint(x, y);
                        if (st is not null && st.Hidden) bFound = true;
                        Trap? tr = level.GetTrapAtPoint(x, y);
                        if (tr is not null && tr.Hidden) bFound = true;

                        // TODO Add hidden creatures

                        if (!bFound) continue;

                        // If so, do we have a line of sight?
                        if (!level.CanSee(Location, new Point(x, y))) continue;

                        // If so then check if we spot it
                        double chance = baseChance;
                        chance -= Math.Sqrt((x - X) * (x - X) + (y - Y) * (y - Y)) * Const.SearchReduction;
                        chance -= level.ParentMission.Diff * Const.MissionDifficultySearchScale;
                        if (rand.NextDouble() * 100.0 <= chance) {
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
        public void UseItem(IEquippable? actionItem) {
            if (actionItem?.BaseType?.ItemEffect?.SingleUse ?? false) DestroyItem(actionItem);
            if (actionItem is Equipment eq) {
                int recharge = actionItem?.BaseType?.ItemEffect?.Recharge ?? 0;
                if (recharge > 0) eq.SetRecharge(recharge);
            }
            Stamina -= UseItemCost;
            HasMoved = true;
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
                    case StatType.Insight: bonus += ar.Type.Insight; break;
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
            foreach (Armour ar in EquippedArmour) {
                mod *= ar.Type.Speed;
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
            foreach (IEquippable eq in Inventory.Where(e => e is IEquippable)) {
                ItemEffect? ie = eq.BaseType.ItemEffect;
                if (ie is not null) {
                    if (ie.AssociatedSkill == UtilitySkill.Unspent || !ie.SkillRequired || GetUtilityLevel(ie.AssociatedSkill) > 0) {
                        return true;
                    }
                }
            }
            return false;
        }
        public IEnumerable<Equipment> GetUtilityItems() {
            foreach (Equipment eq in Inventory.OfType<Equipment>()) {
                ItemEffect? ie = eq.BaseType.ItemEffect;
                if (ie is not null) {
                    if (ie.AssociatedSkill == UtilitySkill.Unspent || !ie.SkillRequired || GetUtilityLevel(ie.AssociatedSkill) > 0) {
                        yield return eq;
                    }
                }
            }
        }
        public double DetectionRange {
            get {
                double range = Const.BaseDetectionRange;
                range += (10.0 - Agility) / 5.0; // Agility has a very minor effect
                range -= (GetUtilityLevel(UtilitySkill.Stealth) / 2.0); // Stealth makes you harder to spot
                range += Encumbrance * 2.0;  // Encumbrance = [0,1]. Easily spotted if heavily encumbered.
                if (range < 1.0) range = 1.0;
                return range;
            }
        }

        // Display options
        public void DisplaySoldierDetails(ShaderProgram prog, float px, float py, bool bSelected, bool bHover, float aspect) {
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
                Aspect = aspect,
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

            tro.XPos = px + (Const.GUIPanelWidth * 0.4f);
            tro.YPos = py + TopBar - 0.0015f;
            tro.Alignment = Alignment.TopMiddle;
            tro.Scale = TextScale * 0.7f;
            tro.TextColour = Color.Gray;
            TextRenderer.DrawWithOptions($"{(int)Health}/{(int)MaxHealth}", tro);
            tro.YPos += 0.02f;
            TextRenderer.DrawWithOptions($"{(int)Stamina}/{(int)MaxStamina}", tro);
            if (MaxShields > 0) {
                tro.YPos += 0.02f;
                TextRenderer.DrawWithOptions($"{(int)Shields}/{(int)MaxShields}", tro);
            }

            // Encumbrance icon
            // TODO - Make it look like a weight again
            float encX = Const.GUIPanelWidth - 0.024f;
            float encY = 0.055f;
            float fract = (float)Math.Min(1.0, CalculateInventoryMass() / MaximumCarry);
            prog.SetUniform("textureEnabled", false);
            prog.SetUniform("flatColour", new Vector4(0.1f, 1f, 0.2f, Const.GUIAlpha));
            Matrix4 pTranslateM = Matrix4.CreateTranslation(encX, encY, Const.DoodadLayer);
            pScaleM = Matrix4.CreateScale(0.021f, -0.021f * (float)Math.Min(0.5,fract) * 2f, 1f);
            prog.SetUniform("model", pScaleM * pTranslateM);
            GL.UseProgram(prog.ShaderProgramHandle);
            Square.Flat.BindAndDraw();
            
            if (fract > 0.5) {
                prog.SetUniform("flatColour", new Vector4(1f, 0.1f, 0.1f, Const.GUIAlpha));
                pScaleM = Matrix4.CreateScale(0.021f, -0.021f * (fract - 0.5f) * 2f, 1f);
                prog.SetUniform("model", pScaleM * pTranslateM);
                GL.UseProgram(prog.ShaderProgramHandle);
                Square.Flat.BindAndDraw();
            }
            prog.SetUniform("flatColour", new Vector4(1f, 1f, 1f, Const.GUIAlpha));
            pScaleM = Matrix4.CreateScale(0.021f, -0.021f, 1f);
            prog.SetUniform("model", pScaleM * pTranslateM);
            GL.UseProgram(prog.ShaderProgramHandle);
            Square.Lines.BindAndDraw();
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
