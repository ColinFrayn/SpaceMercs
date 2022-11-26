using System;

namespace SpaceMercs {
  interface IEquippable : IItem {
    double Rarity { get; } // Rarity of this thing (1 = ubiquitous; lower = rarer)
    int Level { get; }  // Level of this thing (0-5). Irrelevant for generic items.
    double UpgradeCost { get; }  // Cost to upgrade this from its current level to the next one
    ItemType BaseType { get; }
  }
}
