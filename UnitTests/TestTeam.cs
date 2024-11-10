using SpaceMercs;

namespace UnitTests {
    public class TestTeam {
        private ItemType typ1, typ2;

        [SetUp]
        public void Setup() {
            Assert.IsTrue(StaticData.LoadAll());
            typ1 = StaticData.GetItemTypeByName("Medikit") ?? throw new Exception("Could not set up ItemType for Item1");
            typ2 = StaticData.GetItemTypeByName("Frag Grenade") ?? throw new Exception("Could not set up ItemType for Item2");
        }

        [Test]
        public void Test_SetupEmpty() {
            Team emptyTeam = Team.Empty();
            Assert.IsNotNull(emptyTeam);
            Assert.IsEmpty(emptyTeam.SoldiersRO);
            Assert.AreEqual(0, emptyTeam.SoldierCount);
        }

        [Test]
        public void Test_AddItem() {
            Team team = TestUtils.TestTeam();
            Assert.AreEqual(0, team.Inventory.Count);
            Equipment eq = new Equipment(typ1);
            team.AddItem(eq);
            Assert.AreEqual(1, team.Inventory.Count);
            Assert.Contains(eq, team.Inventory.Keys);
            Assert.AreEqual(1, team.Inventory[eq]);
        }

        [Test]
        public void Test_AddMultipleItems() {
            Team team = TestUtils.TestTeam();
            Equipment eq = new Equipment(typ1);
            team.AddItem(eq, 20);
            Assert.AreEqual(1, team.Inventory.Count);
            Assert.Contains(eq, team.Inventory.Keys);
            Assert.AreEqual(20, team.Inventory[eq]);
        }

        [Test]
        public void Test_AddMultipleItems_ShouldStackSeparately() {
            Team team = TestUtils.TestTeam();
            Equipment eq1 = new Equipment(typ1);
            Equipment eq2 = new Equipment(typ2);
            team.AddItem(eq1);
            team.AddItem(eq2);
            team.AddItem(eq1);
            team.AddItem(eq2);
            team.AddItem(eq1);
            Assert.AreEqual(2, team.Inventory.Count);
            Assert.Contains(eq1, team.Inventory.Keys);
            Assert.Contains(eq2, team.Inventory.Keys);
            Assert.AreEqual(3, team.Inventory[eq1]);
            Assert.AreEqual(2, team.Inventory[eq2]);
        }

        [Test]
        public void Test_RemoveItemByType() {
            Team team = TestUtils.TestTeam();
            Equipment eq = new Equipment(typ1);
            team.AddItem(eq);
            team.RemoveItemFromStores(eq);
            Assert.AreEqual(0, team.Inventory.Count);
        }

        [Test]
        public void Test_RemoveItemByType_WrongType_ShouldBeNOP() {
            Team team = TestUtils.TestTeam();
            Equipment eq = new Equipment(typ1);
            Equipment eq2 = new Equipment(typ2);
            team.AddItem(eq);
            team.RemoveItemFromStores(eq2);
            Assert.AreEqual(1, team.Inventory.Count);
            Assert.Contains(eq, team.Inventory.Keys);
            Assert.AreEqual(1, team.Inventory[eq]);
        }
    }
}