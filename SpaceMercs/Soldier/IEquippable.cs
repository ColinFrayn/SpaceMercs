using System;

namespace SpaceMercs {
    public interface IEquippable : IItem {
        double Rarity { get; } // Rarity of this thing (1 = ubiquitous; lower = rarer)
        int Level { get; }  // Level of this thing (0-5). Irrelevant for generic items.
        double UpgradeCost { get; }  // Cost to upgrade this from its current level to the next one
        ItemType BaseType { get; }
        public void EndOfTurn(); // Stuff to do each turn e.g. recharge
        public double GetRange(Soldier s) {
            if (BaseType?.ItemEffect == null) return 0.0;
            double r = BaseType.ItemEffect.Range;
            if (BaseType.ItemEffect.Teleport) {
                r -= s.MassTeleportRangePenalty;
            }
            return Math.Max(0d,r);
        }
        double BuildDiff { get; }
    }
}
