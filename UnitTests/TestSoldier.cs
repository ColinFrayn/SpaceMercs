using SpaceMercs;
using System.Collections.ObjectModel;

namespace UnitTests {
    public class TestSoldier {
        private Race humanRace;
        private ItemType typ1, typ2;

        [SetUp]
        public void Setup() {
            Assert.IsTrue(StaticData.LoadAll());
            humanRace = StaticData.GetRaceByName("Human") ?? throw new Exception("Could not find Human Race");
            typ1 = StaticData.GetItemTypeByName("Medikit") ?? throw new Exception("Could not set up ItemType for Item1");
            typ2 = StaticData.GetItemTypeByName("Frag Grenade") ?? throw new Exception("Could not set up ItemType for Item2");
        }

        [Test]
        public void Test_AddItem() {
            Soldier soldier = new Soldier("Bob", humanRace, 10, 10, 10, 10, 10, GenderType.Male, 1, 0);
            Assert.AreEqual(0, soldier.InventoryGrouped.Count);
            Equipment eq = new Equipment(typ1);
            soldier.AddItem(eq);
            ReadOnlyDictionary<IItem, int> inventoryGrouped = soldier.InventoryGrouped;
            Assert.AreEqual(1, inventoryGrouped.Count);
            Assert.Contains(eq, inventoryGrouped.Keys);
            Assert.AreEqual(1, inventoryGrouped[eq]);
        }

        [Test]
        public void Test_AddMultipleItems() {
            Soldier soldier = new Soldier("Bob", humanRace, 10, 10, 10, 10, 10, GenderType.Male, 1, 0);
            Equipment eq = new Equipment(typ1);
            soldier.AddItem(eq,20);
            ReadOnlyDictionary<IItem, int> inventoryGrouped = soldier.InventoryGrouped;
            Assert.AreEqual(1, inventoryGrouped.Count);
            Assert.Contains(eq, inventoryGrouped.Keys);
            Assert.AreEqual(20, inventoryGrouped[eq]);
        }

        [Test]
        public void Test_AddMultipleItems_ShouldStackSeparately() {
            Soldier soldier = new Soldier("Bob", humanRace, 10, 10, 10, 10, 10, GenderType.Male, 1, 0);
            Equipment eq1 = new Equipment(typ1);
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
            Equipment eq = new Equipment(typ1);
            soldier.AddItem(eq);
            soldier.RemoveItemByType(typ1);
            ReadOnlyDictionary<IItem, int> inventoryGrouped = soldier.InventoryGrouped;
            Assert.AreEqual(0, inventoryGrouped.Count);
        }

        [Test]
        public void Test_RemoveItemByType_WrongType_ShouldBeNOP() {
            Soldier soldier = new Soldier("Bob", humanRace, 10, 10, 10, 10, 10, GenderType.Male, 1, 0);
            Equipment eq = new Equipment(typ1);
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
            Equipment eq = new Equipment(typ1);
            Equipment eq2 = new Equipment(typ2);
            soldier.AddItem(eq);
            Assert.True(soldier.HasItem(eq));
            Assert.False(soldier.HasItem(eq2));
        }

        [Test]
        public void Test_GenerateStash() {
            Soldier soldier = new Soldier("Bob", humanRace, 10, 10, 10, 10, 10, GenderType.Male, 1, 0);
            Equipment eq1 = new Equipment(typ1);
            Equipment eq2 = new Equipment(typ2);
            soldier.AddItem(eq1,3);
            soldier.AddItem(eq2,2);
            Stash st = soldier.GenerateStash();
            Assert.AreEqual(6, st.Count); // 5 items plus corpse
            Assert.AreEqual(soldier.Location, st.Location);
            Assert.False(st.IsEmpty);
            Assert.False(st.Hidden);
            Assert.False(st.ContainsOnlyCorpses);
            Assert.AreEqual(3, st.GetCount(eq1));
            Assert.AreEqual(2, st.GetCount(eq2));
            bool bFoundCorpse = false;
            foreach (IItem it in st.Items()) {
                if (it is Corpse co) {
                    Assert.True(co.IsSoldier);
                    Assert.AreEqual($"Corpse of {soldier.Name}", co.Name);
                    bFoundCorpse = true;
                }
            }
            Assert.True(bFoundCorpse);
        }
    }
}