namespace SpaceMercs {
    public class ShotResult {
        public Dictionary<WeaponType.DamageType, double>? Damage { get; init; }
        public IEntity? Source { get; init; }
        public IEntity? Target { get; init; }

        public ShotResult(IEntity? src, IEntity? tgt, Dictionary<WeaponType.DamageType, double>? dmg) {
            Damage = dmg;
            Source = src;
            Target = tgt;
        }
    }
}
