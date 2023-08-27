using SpaceMercs;
using System.Collections.ObjectModel;

namespace UnitTests {
    public class TestSoldier {
        private Race humanRace;

        [SetUp]
        public void Setup() {
            Assert.IsTrue(StaticData.LoadAll());
            humanRace = StaticData.GetRaceByName("Human") ?? throw new Exception("Could not find Human Race");
        }

        [Test]
        public void Test_AddItem() {
            Soldier soldier = new Soldier("Bob", humanRace, 10, 10, 10, 10, 10, GenderType.Male, 1, 0);
            Assert.AreEqual(0, soldier.InventoryGrouped.Count);
            ItemType? typ = StaticData.GetItemTypeByName("Medikit");
            Assert.NotNull(typ);
            Equipment eq = new Equipment(typ);
            soldier.AddItem(eq);
            ReadOnlyDictionary<IItem, int> inventoryGrouped = soldier.InventoryGrouped;
            Assert.AreEqual(1, inventoryGrouped.Count);
            Assert.Contains(eq, inventoryGrouped.Keys);
            Assert.AreEqual(1, inventoryGrouped[eq]);
        }

        [Test]
        public void Test_AddMultipleItems() {
            Soldier soldier = new Soldier("Bob", humanRace, 10, 10, 10, 10, 10, GenderType.Male, 1, 0);
            ItemType? typ = StaticData.GetItemTypeByName("Medikit");
            Assert.NotNull(typ);
            Equipment eq = new Equipment(typ);
            soldier.AddItem(eq,20);
            ReadOnlyDictionary<IItem, int> inventoryGrouped = soldier.InventoryGrouped;
            Assert.AreEqual(1, inventoryGrouped.Count);
            Assert.Contains(eq, inventoryGrouped.Keys);
            Assert.AreEqual(20, inventoryGrouped[eq]);
        }

        [Test]
        public void Test_AddMultipleItems_ShouldStackSeparately() {
            Soldier soldier = new Soldier("Bob", humanRace, 10, 10, 10, 10, 10, GenderType.Male, 1, 0);
            ItemType? typ1 = StaticData.GetItemTypeByName("Medikit");
            Assert.NotNull(typ1);
            Equipment eq1 = new Equipment(typ1);
            ItemType? typ2 = StaticData.GetItemTypeByName("Frag Grenade");
            Assert.NotNull(typ2);
            Equipment eq2 = new Equipment(typ2);
            soldier.AddItem(eq1);
            soldier.AddItem(eq2);
            soldier.AddItem(eq1);
            soldier.AddItem(eq2);
            soldier.AddItem(eq1);
            ReadOnlyDictionary<IItem, int> inventoryGrouped = soldier.InventoryGrouped;
            Assert.AreEqual(2, inventoryGrouped.Count);
            Assert.Contains(eq1, inventoryGrouped.Keys);
            Assert.Contains(eq2, inventoryGrouped.Keys);
            Assert.AreEqual(3, inventoryGrouped[eq1]);
            Assert.AreEqual(2, inventoryGrouped[eq2]);
        }

        [Test]
        public void Test_RemoveItemByType() {
            Soldier soldier = new Soldier("Bob", humanRace, 10, 10, 10, 10, 10, GenderType.Male, 1, 0);
            ItemType? typ = StaticData.GetItemTypeByName("Medikit");
            Assert.NotNull(typ);
            Equipment eq = new Equipment(typ);
            soldier.AddItem(eq);
            soldier.RemoveItemByType(typ);
            ReadOnlyDictionary<IItem, int> inventoryGrouped = soldier.InventoryGrouped;
            Assert.AreEqual(0, inventoryGrouped.Count);
        }

        [Test]
        public void Test_RemoveItemByType_WrongType_ShouldBeNOP() {
            Soldier soldier = new Soldier("Bob", humanRace, 10, 10, 10, 10, 10, GenderType.Male, 1, 0);
            ItemType? typ = StaticData.GetItemTypeByName("Medikit");
            Assert.NotNull(typ);
            ItemType? typ2 = StaticData.GetItemTypeByName("Frag Grenade");
            Assert.NotNull(typ2);
            Equipment eq = new Equipment(typ);
            soldier.AddItem(eq);
            soldier.RemoveItemByType(typ2);
            ReadOnlyDictionary<IItem, int> inventoryGrouped = soldier.InventoryGrouped;
            Assert.AreEqual(1, inventoryGrouped.Count);
            Assert.Contains(eq, inventoryGrouped.Keys);
            Assert.AreEqual(1, inventoryGrouped[eq]);
        }

        [Test]
        public void Test_HasItem() {
            Soldier soldier = new Soldier("Bob", humanRace, 10, 10, 10, 10, 10, GenderType.Male, 1, 0);
            ItemType? typ = StaticData.GetItemTypeByName("Medikit");
            Equipment eq = new Equipment(typ);
            ItemType? typ2 = StaticData.GetItemTypeByName("Frag Grenade");
            Assert.NotNull(typ2);
            Equipment eq2 = new Equipment(typ2);
            soldier.AddItem(eq);
            Assert.True(soldier.HasItem(eq));
            Assert.False(soldier.HasItem(eq2));
        }
    }
}