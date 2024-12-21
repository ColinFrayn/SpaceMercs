using OpenTK.Mathematics;
using SpaceMercs.Graphics;
using static SpaceMercs.Delegates;

namespace SpaceMercs {
    public enum StatType { Strength = 1, Agility = 2, Insight = 3, Toughness = 4, Endurance = 5, Health = 6, Stamina = 7, Attack = 8, Defence = 9 };
    public enum GenderType { Male, Female, Neuter };
    public enum BodyPart { Head, Chest, Arms, Legs, Feet, Hands }

    public interface IEntity {
        int X { get; }
        int Y { get; }
        double Health { get; }
        double MaxHealth { get; }
        double Stamina { get; }
        double MaxStamina { get; }
        double Shields { get; }
        double MaxShields { get; }
        int Level { get; }
        double Attack { get; }
        double Defence { get; }
        string Name { get; }
        int Size { get; }
        int TravelRange { get; }
        double AttackRange { get; }
        Point Location { get; }
        double Facing { get; }
        bool CanOpenDoors { get; }
        Weapon? EquippedWeapon { get; }
        double BaseArmour { get; }
        IEnumerable<Effect> Effects { get; }
        bool IsInjured { get; }
        double Encumbrance { get; }
        bool HasMoved { get; }

        // Methods
        bool CanSee(int x, int y);
        bool CanSee(IEntity en);
        void UpdateVisibility(MissionLevel m);
        void SetLocation(Point p);
        void SetFacing(Utils.Direction d);
        void SetFacing(double d);
        void Display(ShaderProgram prog, bool bLabel, bool bStatBars, bool bShowEffects, float fViewHeight, float aspect, Matrix4 viewM);
        void ResetForBattle();
        double RangeTo(IEntity en);
        double RangeTo(Point pt);
        double RangeTo(int tx, int ty);
        void EndOfTurn(VisualEffect.EffectFactory fact, Action<IEntity> centreView, PlaySoundDelegate playSound, ShowMessageDelegate showMessage, ItemEffect.ApplyItemEffect applyEffect);
        double InflictDamage(Dictionary<WeaponType.DamageType, double> AllDam, ItemEffect.ApplyItemEffect applyEffect);
        double CalculateDamage(Dictionary<WeaponType.DamageType, double> AllDam);
        Stash GenerateStash();
        double GetDamageReductionByDamageType(WeaponType.DamageType type);
        void KillEntity(ItemEffect.ApplyItemEffect applyEffect);
        Dictionary<WeaponType.DamageType, double> GenerateDamage(int nhits);
        void ApplyEffectToEntity(IEntity? src, ItemEffect ie, VisualEffect.EffectFactory fact, ItemEffect.ApplyItemEffect applyEffect);
    }
}