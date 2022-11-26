using System;
using System.Collections.Generic;
using System.Drawing;

namespace SpaceMercs {
  public enum StatType { Strength = 1, Agility = 2, Intelligence = 3, Toughness = 4, Endurance = 5, Health = 6, Stamina = 7, Attack = 8, Defence = 9 };
  public enum GenderType { Male, Female, Neuter };
  public enum BodyPart { Head, Chest, Arms, Legs, Feet, Hands }

  interface IEntity {
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

    // Methods
    bool CanSee(int x, int y);
    bool CanSee(IEntity en);
    void UpdateVisibility(MissionLevel m);
    void SetLocation(Point p);
    void SetFacing(Utils.Direction d);
    void SetFacing(double d);
    void Display(bool bLabel, bool bStatBars, bool bShowEffects, float fViewHeight);
    void ResetForBattle();
    double RangeTo(IEntity en);
    double RangeTo(Point pt);
    double RangeTo(int tx, int ty);
    void EndOfTurn(VisualEffect.EffectFactory fact, Action<IEntity> centreView, Action<string> playSound, Action<string> showMessage);
    double InflictDamage(Dictionary<WeaponType.DamageType, double> AllDam);
    Stash GenerateStash();
    double GetDamageReductionByDamageType(WeaponType.DamageType type);
    void KillEntity();
    Dictionary<WeaponType.DamageType, double> GenerateDamage();
    void ApplyEffectToEntity(IEntity src, ItemEffect ie, VisualEffect.EffectFactory fact);

    // Actions
    //void Move(Utils.Direction d);
    //void AttackEntity(IEntity en, VisualEffect.EffectFactory fact, Action refresh);
  }
}
