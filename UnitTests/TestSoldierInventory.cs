using SpaceMercs;
using System.Collections.ObjectModel;
using System.Drawing;
using System.IO;
using System.Text;
using System.Xml;

namespace UnitTests {
    public class TestSoldierInventory {
        private Race humanRace = null!;
        private ItemType medikit = null!;
        private ItemType grenade = null!;
        private ItemType medsystem = null!;
        private WeaponType pistol = null!;
        private ArmourType helmet = null!;
        private ArmourType chestplate = null!;
        private MaterialType fabric = null!;

        [SetUp]
        public void Setup() {
            Assert.IsTrue(StaticData.LoadAll());
            humanRace = StaticData.GetRaceByName("Human") ?? throw new Exception("Could not find Human Race");
            medikit = StaticData.GetItemTypeByName("Medikit") ?? throw new Exception("Could not find Medikit");
            grenade = StaticData.GetItemTypeByName("Frag Grenade") ?? throw new Exception("Could not find Frag Grenade");
            medsystem = StaticData.GetItemTypeByName("Medsystem") ?? throw new Exception("Could not find Medsystem");
            pistol = StaticData.GetWeaponTypeByName("Pistol") ?? throw new Exception("Could not find Pistol");
            helmet = StaticData.GetArmourTypeByName("Military Helmet") ?? throw new Exception("Could not find Military Helmet");
            chestplate = StaticData.GetArmourTypeByName("Chestplate") ?? throw new Exception("Could not find Chestplate");
            fabric = StaticData.GetMaterialTypeByName("Fabric") ?? throw new Exception("Could not find Fabric");
            Assert.True(medikit.ItemEffect?.SingleUse ?? false, "Medikit is expected to be single-use");
            Assert.False(medsystem.ItemEffect?.SingleUse ?? false, "Medsystem is expected to be reusable");
            Assert.Greater(medsystem.ItemEffect?.Recharge ?? 0, 0, "Medsystem is expected to recharge");
        }

        private Soldier MakeSoldier(string name = "Bob") {
            return new Soldier(name, humanRace, 10, 10, 10, 10, 10, GenderType.Male, 1, 0);
        }

        private static Soldier RoundTrip(Soldier s, Team? team = null) {
            using MemoryStream stream = new MemoryStream();
            using StreamWriter sw = new StreamWriter(stream);
            s.SaveToFile(sw);
            sw.Flush();
            stream.Position = 0;
            XmlDocument xml = new XmlDocument();
            string doc = Encoding.UTF8.GetString(stream.ToArray());
            xml.LoadXml(doc);
            return new Soldier(xml.DocumentElement!, team);
        }

        private static void UseReadyItem(Soldier s, ItemType type) {
            Equipment eq = s.InventoryGrouped.Keys.OfType<Equipment>().First(e => e.BaseType == type && e.Recharge == 0);
            s.UseItem(eq);
        }

        private static void EndTurn(Soldier s) {
            s.EndOfTurn((_, _, _, _) => { }, _ => { }, _ => { }, (_, _) => { }, (_, _, _, _) => { });
        }

        private static (int Stacks, int Ready, int Recharging) CountByRecharge(Soldier s, ItemType type) {
            int stacks = 0, ready = 0, recharging = 0;
            foreach (KeyValuePair<IItem, int> kvp in s.InventoryGrouped) {
                if (kvp.Key is not Equipment eq || eq.BaseType != type) continue;
                stacks++;
                if (eq.Recharge > 0) recharging += kvp.Value;
                else ready += kvp.Value;
            }
            return (stacks, ready, recharging);
        }

        #region Adding and stacking

        [Test]
        public void DistinctInstancesOfSameType_StackInGroupedInventory() {
            Soldier s = MakeSoldier();
            s.AddItem(new Equipment(medikit));
            s.AddItem(new Equipment(medikit));
            ReadOnlyDictionary<IItem, int> grouped = s.InventoryGrouped;
            Assert.AreEqual(1, grouped.Count);
            Assert.AreEqual(2, grouped.Values.Single());
        }

        [Test]
        public void DistinctInstancesOfDifferentTypes_DoNotStack() {
            Soldier s = MakeSoldier();
            s.AddItem(new Equipment(medikit));
            s.AddItem(new Equipment(grenade));
            Assert.AreEqual(2, s.InventoryGrouped.Count);
            Assert.AreEqual(1, s.CountItem(new Equipment(medikit)));
            Assert.AreEqual(1, s.CountItem(new Equipment(grenade)));
        }

        [Test]
        public void AddItem_CountZeroOrNegative_IsNoOp() {
            Soldier s = MakeSoldier();
            Equipment eq = new Equipment(medikit);
            s.AddItem(eq, 0);
            s.AddItem(eq, -3);
            Assert.AreEqual(0, s.InventoryGrouped.Count);
            Assert.False(s.HasItem(eq));
        }

        [Test]
        public void HasItemAndCountItem_MatchEqualInstanceNotInList() {
            Soldier s = MakeSoldier();
            s.AddItem(new Equipment(medikit), 3);
            Equipment lookup = new Equipment(medikit);
            Assert.True(s.HasItem(lookup));
            Assert.AreEqual(3, s.CountItem(lookup));
            Assert.False(s.HasItem(new Equipment(grenade)));
            Assert.AreEqual(0, s.CountItem(new Equipment(grenade)));
        }

        [Test]
        public void AddItemWithCount_ThenCountItem() {
            Soldier s = MakeSoldier();
            Equipment eq = new Equipment(medikit);
            s.AddItem(eq, 4);
            Assert.AreEqual(4, s.CountItem(eq));
            Assert.AreEqual(1, s.InventoryGrouped.Count);
        }

        #endregion

        #region Use / recharge

        [Test]
        public void UseSingleUseMedikit_TwoDistinctInstances_ConsumesOnlyOne() {
            Soldier s = MakeSoldier();
            s.AddItem(new Equipment(medikit));
            s.AddItem(new Equipment(medikit));
            UseReadyItem(s, medikit);
            Assert.AreEqual(1, s.CountItem(new Equipment(medikit)));
            Assert.AreEqual(0, CountByRecharge(s, medikit).Recharging);
        }

        [Test]
        public void UseSingleUseMedikit_AddedWithCount_ConsumesOnlyOne() {
            Soldier s = MakeSoldier();
            s.AddItem(new Equipment(medikit), 2);
            UseReadyItem(s, medikit);
            Assert.AreEqual(1, s.CountItem(new Equipment(medikit)));
        }

        [Test]
        public void UseRechargeable_TwoDistinctInstances_OnlyUsedInstanceRecharges() {
            Soldier s = MakeSoldier();
            Equipment a = new Equipment(medsystem);
            Equipment b = new Equipment(medsystem);
            s.AddItem(a);
            s.AddItem(b);
            UseReadyItem(s, medsystem);

            (int stacks, int ready, int recharging) = CountByRecharge(s, medsystem);
            Assert.AreEqual(1, ready, "one medsystem should still be ready");
            Assert.AreEqual(1, recharging, "only the used medsystem should be recharging");
            Assert.AreEqual(2, stacks, "ready and recharging copies should be separate stacks");
            Assert.AreEqual(medsystem.ItemEffect!.Recharge, s.InventoryGrouped.Keys.OfType<Equipment>().Single(e => e.BaseType == medsystem && e.Recharge > 0).Recharge);
        }

        [Test]
        public void UseRechargeable_AddedWithCount_OnlyOneRecharges() {
            Soldier s = MakeSoldier();
            Equipment eq = new Equipment(medsystem);
            s.AddItem(eq, 2);
            UseReadyItem(s, medsystem);

            (int stacks, int ready, int recharging) = CountByRecharge(s, medsystem);
            Assert.AreEqual(1, ready, "one copy should still be ready");
            Assert.AreEqual(1, recharging, "only one copy should be recharging");
            Assert.AreEqual(2, ready + recharging);
            Assert.AreEqual(2, stacks, "ready and recharging copies should be separate stacks");
        }

        [Test]
        public void UseRechargeable_UIGroupedKey_WithTwoDistinctInstances_LeavesOneReady() {
            // Mirrors UseItem dialog: it binds the list row to InventoryGrouped.Keys, not a unique instance.
            Soldier s = MakeSoldier();
            s.AddItem(new Equipment(medsystem));
            s.AddItem(new Equipment(medsystem));
            IItem groupedKey = s.InventoryGrouped.Keys.OfType<Equipment>().Single(e => e.BaseType == medsystem);
            s.UseItem((IEquippable)groupedKey);

            (_, int ready, int recharging) = CountByRecharge(s, medsystem);
            Assert.AreEqual(1, ready);
            Assert.AreEqual(1, recharging);
        }

        [Test]
        public void EndOfTurn_TwoRechargingDistinctInstances_EachLosesOneCharge() {
            Soldier s = MakeSoldier();
            s.AddItem(new Equipment(medsystem));
            s.AddItem(new Equipment(medsystem));
            UseReadyItem(s, medsystem);
            UseReadyItem(s, medsystem);
            int cooldown = medsystem.ItemEffect!.Recharge;
            EndTurn(s);
            foreach (Equipment eq in s.InventoryGrouped.Keys.OfType<Equipment>().Where(e => e.BaseType == medsystem)) {
                Assert.AreEqual(cooldown - 1, eq.Recharge);
            }
            Assert.AreEqual(2, CountByRecharge(s, medsystem).Recharging);
        }

        [Test]
        public void EndOfTurn_RechargeableAddedWithCount_CooldownDropsByOneNotByStackSize() {
            Soldier s = MakeSoldier();
            s.AddItem(new Equipment(medsystem), 3);
            UseReadyItem(s, medsystem);
            UseReadyItem(s, medsystem);
            UseReadyItem(s, medsystem);
            int cooldown = medsystem.ItemEffect!.Recharge;
            EndTurn(s);
            foreach (Equipment eq in s.InventoryGrouped.Keys.OfType<Equipment>().Where(e => e.BaseType == medsystem)) {
                Assert.AreEqual(cooldown - 1, eq.Recharge, "a stack of 3 should not tick recharge three times in one turn");
            }
            (_, _, int recharging) = CountByRecharge(s, medsystem);
            Assert.AreEqual(3, recharging);
        }

        [Test]
        public void StopMission_ClearsRechargeOnInventoryEquipment() {
            Soldier s = MakeSoldier();
            s.AddItem(new Equipment(medsystem));
            UseReadyItem(s, medsystem);
            Assert.Greater(CountByRecharge(s, medsystem).Recharging, 0);
            s.StopMission();
            Assert.AreEqual(0, CountByRecharge(s, medsystem).Recharging);
            Assert.AreEqual(0, s.InventoryGrouped.Keys.OfType<Equipment>().Single(e => e.BaseType == medsystem).Recharge);
        }

        #endregion

        #region Destroy / remove / drop

        [Test]
        public void DestroyItem_PartialCount() {
            Soldier s = MakeSoldier();
            Equipment eq = new Equipment(medikit);
            s.AddItem(eq, 5);
            s.DestroyItem(eq, 2);
            Assert.AreEqual(3, s.CountItem(eq));
        }

        [Test]
        public void DestroyItem_MoreThanCarried_RemovesAll() {
            Soldier s = MakeSoldier();
            Equipment eq = new Equipment(medikit);
            s.AddItem(eq, 2);
            s.DestroyItem(eq, 10);
            Assert.AreEqual(0, s.CountItem(eq));
            Assert.AreEqual(0, s.InventoryGrouped.Count);
        }

        [Test]
        public void RemoveItemByType_LeavesOtherTypes() {
            Soldier s = MakeSoldier();
            s.AddItem(new Equipment(medikit), 2);
            s.AddItem(new Equipment(grenade), 1);
            s.RemoveItemByType(medikit, 1);
            Assert.AreEqual(1, s.CountItem(new Equipment(medikit)));
            Assert.AreEqual(1, s.CountItem(new Equipment(grenade)));
        }

        [Test]
        public void RemoveItemByType_TwoDistinctInstances_DoesNotThrow() {
            Soldier s = MakeSoldier();
            s.AddItem(new Equipment(medikit));
            s.AddItem(new Equipment(medikit));
            s.AddItem(new Equipment(grenade));
            Assert.DoesNotThrow(() => s.RemoveItemByType(medikit, 2));
            Assert.AreEqual(0, s.CountItem(new Equipment(medikit)));
            Assert.AreEqual(1, s.CountItem(new Equipment(grenade)));
        }

        [Test]
        public void DropItem_MovesToTeamInventory() {
            Team team = TestUtils.TestTeam();
            Soldier s = MakeSoldier();
            team.AddSoldier(s);
            Equipment eq = new Equipment(medikit);
            s.AddItem(eq, 3);
            s.DropItem(eq, 2);
            Assert.AreEqual(1, s.CountItem(eq));
            Assert.AreEqual(2, team.Inventory[eq]);
        }

        [Test]
        public void DropAll_MovesEntireStackToTeam() {
            Team team = TestUtils.TestTeam();
            Soldier s = MakeSoldier();
            team.AddSoldier(s);
            Equipment eq = new Equipment(medikit);
            s.AddItem(eq, 4);
            s.DropAll(eq);
            Assert.AreEqual(0, s.CountItem(eq));
            Assert.AreEqual(4, team.Inventory[eq]);
        }

        [Test]
        public void DropItem_WithoutTeam_LeavesItemInInventory() {
            Soldier s = MakeSoldier();
            Equipment eq = new Equipment(medikit);
            s.AddItem(eq, 2);
            s.DropItem(eq, 1);
            Assert.AreEqual(2, s.CountItem(eq));
        }

        #endregion

        #region Equip / unequip

        [Test]
        public void EquipWeapon_RemovesFromInventory() {
            Soldier s = MakeSoldier();
            Weapon wp = new Weapon(pistol, 0);
            s.AddItem(wp);
            Assert.True(s.Equip(wp));
            Assert.AreEqual(wp, s.EquippedWeapon);
            Assert.False(s.HasItem(wp));
            Assert.AreEqual(0, s.InventoryGrouped.Count);
        }

        [Test]
        public void UnequipWeapon_ReturnsToInventory() {
            Soldier s = MakeSoldier();
            Weapon wp = new Weapon(pistol, 0);
            s.Equip(wp);
            s.Unequip(wp);
            Assert.IsNull(s.EquippedWeapon);
            Assert.True(s.HasItem(wp));
            Assert.AreEqual(1, s.CountItem(wp));
        }

        [Test]
        public void EquipSecondWeapon_FirstGoesToInventory() {
            Soldier s = MakeSoldier();
            Weapon first = new Weapon(pistol, 0);
            Weapon second = new Weapon(pistol, 1);
            s.Equip(first);
            s.Equip(second);
            Assert.AreEqual(second, s.EquippedWeapon);
            Assert.True(s.HasItem(first));
            Assert.False(s.HasItem(second));
        }

        [Test]
        public void EquipArmour_ThenUnequip_ReturnsToInventory() {
            Soldier s = MakeSoldier();
            Armour ar = new Armour(helmet, fabric, 0);
            s.AddItem(ar);
            Assert.True(s.Equip(ar));
            Assert.AreEqual(ar, s.GetArmourAtLocation(BodyPart.Head));
            Assert.False(s.HasItem(ar));
            s.Unequip(ar);
            Assert.IsNull(s.GetArmourAtLocation(BodyPart.Head));
            Assert.True(s.HasItem(ar));
        }

        [Test]
        public void EquipOverlappingArmour_PreviousPieceGoesToInventory() {
            Soldier s = MakeSoldier();
            Armour first = new Armour(helmet, fabric, 0);
            Armour second = new Armour(helmet, fabric, 1);
            s.Equip(first);
            s.Equip(second);
            Assert.AreEqual(second, s.GetArmourAtLocation(BodyPart.Head));
            Assert.True(s.HasItem(first));
            Assert.AreEqual(1, s.EquippedArmour.Count);
        }

        [Test]
        public void EquipHelmetAndChestplate_BothStayEquipped() {
            Soldier s = MakeSoldier();
            Armour head = new Armour(helmet, fabric, 0);
            Armour chest = new Armour(chestplate, fabric, 0);
            s.Equip(head);
            s.Equip(chest);
            Assert.AreEqual(head, s.GetArmourAtLocation(BodyPart.Head));
            Assert.AreEqual(chest, s.GetArmourAtLocation(BodyPart.Chest));
            Assert.AreEqual(2, s.EquippedArmour.Count);
        }

        [Test]
        public void EquippedItems_AreNotCountedInInventory() {
            Soldier s = MakeSoldier();
            Weapon wp = new Weapon(pistol, 0);
            Armour ar = new Armour(helmet, fabric, 0);
            s.Equip(wp);
            s.Equip(ar);
            Assert.False(s.HasItem(wp));
            Assert.False(s.HasItem(ar));
            Assert.AreEqual(0, s.InventoryGrouped.Count);
        }

        [Test]
        public void CalculateInventoryMass_IncludesCarriedAndEquipped() {
            Soldier s = MakeSoldier();
            Equipment kit = new Equipment(medikit);
            Weapon wp = new Weapon(pistol, 0);
            Armour ar = new Armour(helmet, fabric, 0);
            s.AddItem(kit);
            s.Equip(wp);
            s.Equip(ar);
            Assert.AreEqual(kit.Mass + wp.Mass + ar.Mass, s.CalculateInventoryMass(), 0.0001);
        }

        #endregion

        #region Loadout

        [Test]
        public void AddAndRemoveLoadout_TracksQuantity() {
            Soldier s = MakeSoldier();
            Equipment eq = new Equipment(medikit);
            Assert.AreEqual(1, s.AddLoadout(eq));
            Assert.AreEqual(2, s.AddLoadout(eq));
            Assert.AreEqual(1, s.RemoveLoadout(eq));
            Assert.AreEqual(0, s.RemoveLoadout(eq));
            Assert.AreEqual(0, s.RemoveLoadout(eq));
        }

        [Test]
        public void LoadoutComplete_FalseUntilInventoryMatches() {
            Soldier s = MakeSoldier();
            Equipment eq = new Equipment(medikit);
            s.AddLoadout(eq);
            s.AddLoadout(eq);
            Assert.False(s.LoadoutComplete);
            s.AddItem(new Equipment(medikit));
            Assert.False(s.LoadoutComplete);
            s.AddItem(new Equipment(medikit));
            Assert.True(s.LoadoutComplete);
        }

        [Test]
        public void Refill_TakesMatchingItemsFromTeam() {
            Team team = TestUtils.TestTeam();
            Soldier s = MakeSoldier();
            team.AddSoldier(s);
            Equipment loadoutItem = new Equipment(medikit);
            s.AddLoadout(loadoutItem);
            s.AddLoadout(loadoutItem);
            Equipment teamStack = new Equipment(medikit);
            team.AddItem(teamStack, 5);
            s.Refill();
            Assert.True(s.LoadoutComplete);
            Assert.AreEqual(2, s.CountItem(new Equipment(medikit)));
            Assert.AreEqual(3, team.Inventory[teamStack]);
        }

        [Test]
        public void SetLoadout_DropsExcessThenRefills() {
            Team team = TestUtils.TestTeam();
            Soldier s = MakeSoldier();
            team.AddSoldier(s);
            Equipment eq = new Equipment(medikit);
            s.AddItem(eq, 5);
            s.AddLoadout(eq);
            s.AddLoadout(eq);
            s.SetLoadout();
            Assert.AreEqual(2, s.CountItem(eq));
            Assert.AreEqual(3, team.Inventory[eq]);
            Assert.True(s.LoadoutComplete);
        }

        #endregion

        #region Save / load / stash

        [Test]
        public void SaveLoad_PreservesStackedCounts() {
            Soldier s = MakeSoldier();
            s.AddItem(new Equipment(medikit), 3);
            s.AddItem(new Equipment(grenade), 2);
            Soldier loaded = RoundTrip(s);
            Assert.AreEqual(3, loaded.CountItem(new Equipment(medikit)));
            Assert.AreEqual(2, loaded.CountItem(new Equipment(grenade)));
        }

        [Test]
        public void SaveLoad_TwoDistinctRechargeables_UsingOneOnlyRechargesOne() {
            Soldier s = MakeSoldier();
            s.AddItem(new Equipment(medsystem));
            s.AddItem(new Equipment(medsystem));
            Soldier loaded = RoundTrip(s);
            UseReadyItem(loaded, medsystem);
            (_, int ready, int recharging) = CountByRecharge(loaded, medsystem);
            Assert.AreEqual(1, ready, "after save/load, using one medsystem should leave the other ready");
            Assert.AreEqual(1, recharging);
        }

        [Test]
        public void SaveLoad_LoadoutAndEquippedWeapon() {
            Soldier s = MakeSoldier();
            Weapon wp = new Weapon(pistol, 2);
            s.Equip(wp);
            Equipment kit = new Equipment(medikit);
            s.AddLoadout(kit);
            s.AddLoadout(kit);
            Soldier loaded = RoundTrip(s);
            Assert.IsNotNull(loaded.EquippedWeapon);
            Assert.AreEqual(pistol, loaded.EquippedWeapon!.Type);
            Assert.AreEqual(2, loaded.EquippedWeapon.Level);
            Assert.AreEqual(2, loaded.Loadout[new Equipment(medikit)]);
        }

        [Test]
        public void GenerateStash_DropsInventoryAndEquipment() {
            Soldier s = MakeSoldier();
            Equipment kit = new Equipment(medikit);
            Weapon wp = new Weapon(pistol, 0);
            Armour ar = new Armour(helmet, fabric, 0);
            s.AddItem(kit, 2);
            s.Equip(wp);
            s.Equip(ar);
            Stash st = s.GenerateStash();
            Assert.AreEqual(0, s.InventoryGrouped.Count);
            Assert.IsNull(s.EquippedWeapon);
            Assert.AreEqual(0, s.EquippedArmour.Count);
            Assert.AreEqual(2, st.GetCount(kit));
            Assert.AreEqual(1, st.GetCount(wp));
            Assert.AreEqual(1, st.GetCount(ar));
        }

        [Test]
        public void PickupAllFromStash_ThenUseRechargeable_OnlyOneRecharges() {
            Soldier s = MakeSoldier();
            Stash st = new Stash(new Point(0, 0));
            Equipment stack = new Equipment(medsystem);
            st.Add(stack, 2);
            foreach (IItem it in st.Items()) {
                s.AddItem(it, st.GetCount(it));
            }
            UseReadyItem(s, medsystem);
            (_, int ready, int recharging) = CountByRecharge(s, medsystem);
            Assert.AreEqual(1, ready, "picking up a stacked stash item should still allow using one copy");
            Assert.AreEqual(1, recharging);
        }

        #endregion
    }
}
