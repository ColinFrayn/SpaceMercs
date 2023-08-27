using SpaceMercs;

namespace UnitTests {
    public class Tests {
        private Soldier TestSoldier;

        [SetUp]
        public void Setup() {
            Race? rc = StaticData.GetRaceByName("Human");
            Assert.NotNull(rc);
            TestSoldier = new Soldier("Bob", rc, 10, 10, 10, 10, 10, GenderType.Male, 1, 0);
        }

        [Test]
        public void Test_AddItem() {
            Assert.AreEqual(0, TestSoldier.InventoryGrouped.Count);
            ItemType? typ = StaticData.GetItemTypeByName("Medikit");
            Assert.NotNull(typ);
            Equipment eq = new Equipment(typ);
            Assert.NotNull(eq);
            TestSoldier.AddItem(eq);
            Assert.That(TestSoldier.InventoryGrouped.Count, Is.EqualTo(1));
        }

    }
}