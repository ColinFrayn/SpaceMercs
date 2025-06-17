using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using SpaceMercs.Graphics;
using SpaceMercs.Graphics.Shapes;
using System.IO;
using System.Xml;
using static SpaceMercs.Delegates;

namespace SpaceMercs {
    public class ResourceNode : IEntity {
        // IEntity stuff
        public int X { get; private set; }
        public int Y { get; private set; }
        public int Level { get; private set; }
        public double Health { get; private set; }
        public double MaxHealth { get { return Const.ResourceNodeHealth * (1d + ((double)Level / 4d)); } }
        public double Stamina => 0d;
        public double MaxStamina => 0d;
        public double Shields => 0d;
        public double MaxShields => 0d;
        public double Attack => 0d;
        public double Defence => 0d;
        public int TravelRange => 0;
        public Weapon? EquippedWeapon => null;
        public double AttackRange => 0d;
        public string Name { get { return $"{ResourceType.Name} Node"; } }
        public int Size => 1;
        public double Facing => 0d;
        public double Shred => 0d;
        public Point Location { get { return new Point(X, Y); } }
        private readonly List<Effect> _Effects = [];
        public IEnumerable<Effect> Effects { get { return _Effects.AsReadOnly(); } }
        public bool HasMoved => false;
        private readonly MaterialType ResourceType;
        private readonly MissionLevel? CurrentLevel;

        public bool CanSee(int x, int y) { return false; }
        public bool CanSee(IEntity en) { return false; }
        public void UpdateVisibility(MissionLevel m) { } // NOP
        public void SetLocation(Point p) {
            X = p.X;
            Y = p.Y;
        }
        public void SetFacing(Utils.Direction d) { } // NOP
        public void SetFacing(double d) { } // NOP
        private TexSpecs GetTexture() {
            return ResourceType.GetTexture();
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
            Matrix4 pTranslateM = Matrix4.CreateTranslation(X + 0.5f, Y + 0.5f, Const.EntityLayer);
            Matrix4 pScaleM = Matrix4.CreateScale(1f);
            prog.SetUniform("model", pScaleM * pTranslateM);
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
                prog.SetUniform("textureEnabled", false);
                GraphicsFunctions.DisplayBicolourFractBar(prog, X, Y - 0.1f, 1f, 0.09f, (float)(Health / MaxHealth), new Vector4(0.3f, 1f, 0.3f, 1f), new Vector4(1f, 0f, 0f, 1f));
            }
        }
        public void ResetForBattle() {
            Health = MaxHealth;
        }
        public bool CanOpenDoors => false;
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
        public double BaseArmour => Const.ResourceNodeArmour * (1d + Const.ResourceNodeLevelArmourScale * (Level - 1));
        public double GetDamageReductionByDamageType(WeaponType.DamageType type) {
            double red = 100d;
            if (type is WeaponType.DamageType.Poison or WeaponType.DamageType.Psychic or WeaponType.DamageType.Void) red = 0d;
            else if (type is WeaponType.DamageType.Electrical or WeaponType.DamageType.Fire or WeaponType.DamageType.Cold) red = 30d;
            return Utils.ArmourReduction(BaseArmour) * red / 100d;
        }
        public double CalculateDamage(Dictionary<WeaponType.DamageType, double> AllDam) {
            return InflictDamage_Internal(AllDam, null, null, null, false);
        }
        public double InflictDamage(Dictionary<WeaponType.DamageType, double> AllDam, ItemEffect.ApplyItemEffect applyEffect, VisualEffect.EffectFactory? fact, IEntity? source) {
            return InflictDamage_Internal(AllDam, applyEffect, fact, source, true);
        }
        private double InflictDamage_Internal(Dictionary<WeaponType.DamageType, double> AllDam, ItemEffect.ApplyItemEffect? applyEffect, VisualEffect.EffectFactory? fact, IEntity? source, bool applyDamage) {
            if (!AllDam.Any() || Health <= 0.0) return 0.0;

            // Take a copy as we're modifying it
            Dictionary<WeaponType.DamageType, double> damInternal = new Dictionary<WeaponType.DamageType, double>(AllDam);

            // Loop through all damage types and calculate all their combined effects
            double TotalDam = 0.0;
            foreach (WeaponType.DamageType type in damInternal.Keys) {
                double dam = damInternal[type];

                // Armour reduces damage
                TotalDam += dam * GetDamageReductionByDamageType(type);
            }

            if (!applyDamage) return TotalDam;

            // Do the damage
            Health -= TotalDam;

            // Is the node destroyed?
            if (Health <= 0.0) KillEntity(applyEffect!, fact!, source!);
            return TotalDam;
        }
        public void ShredArmour(double shred) { } // NOP
        public Stash GenerateStash() {
            return GenerateStash(null);
        }
        public Stash GenerateStash(IEntity? miner) { 
            Stash st = new Stash(Location);
            Random rnd = new Random();
            // How much ore?
            double minerMod = 1d;
            if (miner is Soldier s) {
                minerMod = 1d + ((double)s.GetUtilityLevel(Soldier.UtilitySkill.Miner) / 5d);
            }
            double levelMod = 1d + ((Level - ResourceType.NodeMin) / 10d);
            double dnum = (1d + rnd.NextDouble()) * minerMod * levelMod;
            int num = (int)Math.Round(dnum);
            st.Add(new Material(ResourceType), num);
            return st;
        }
        public void KillEntity(ItemEffect.ApplyItemEffect applyEffect, VisualEffect.EffectFactory? fact) { 
            KillEntity(applyEffect, fact, null);
        }
        public void KillEntity(ItemEffect.ApplyItemEffect applyEffect, VisualEffect.EffectFactory? fact, IEntity? src) {
            Health = 0.0;
            CurrentLevel?.DestroyNode(this, src);
        }
        public Dictionary<WeaponType.DamageType, double> GenerateDamage() {
            throw new NotImplementedException();
        }
        public void ApplyEffectToEntity(IEntity? src, ItemEffect ie, VisualEffect.EffectFactory fact, ItemEffect.ApplyItemEffect applyEffect) { } // NOP
        public bool IsInjured { get { return Health < MaxHealth; } }
        public double Encumbrance => 0.0;
        public void EndOfTurn(VisualEffect.EffectFactory fact, Action<IEntity> centreView, PlaySoundDelegate playSound, ShowMessageDelegate showMessage, ItemEffect.ApplyItemEffect applyEffect) { } // NOP

        // Constructors
        public ResourceNode(MaterialType mat, int lvl, MissionLevel lev) {
            X = -1;
            Y = -1;
            Level = lvl;
            ResourceType = mat;
            Health = MaxHealth;
            CurrentLevel = lev;
        }
        public ResourceNode(XmlNode xml, MissionLevel lev) {
            string strName = xml.GetAttributeText("Type", string.Empty);
            ResourceType = StaticData.GetMaterialTypeByName(strName) ?? throw new Exception($"Could not identify resource node type: {strName}");
            Level = xml.SelectNodeInt("Level");
            XmlNode? xmll = xml.SelectSingleNode("Location") ?? throw new Exception("Could not ID Location for ResourceNode : " + strName);
            X = xmll.GetAttributeInt("X");
            Y = xmll.GetAttributeInt("Y");
            Health = xml.SelectNodeDouble("Health", MaxHealth);
            CurrentLevel = lev;
        }

        public void SaveToFile(StreamWriter file) {
            file.WriteLine("<ResourceNode Type=\"" + ResourceType.Name + "\">");
            file.WriteLine(" <Level>" + Level + "</Level>");
            file.WriteLine(" <Location X=\"" + X + "\" Y=\"" + Y + "\"/>");
            if (Health != MaxHealth) file.WriteLine($" <Health>{Health:N2}</Health>");
            file.WriteLine("</ResourceNode>");
        }
    }
}