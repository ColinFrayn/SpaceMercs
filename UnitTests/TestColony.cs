using Moq;
using SpaceMercs;
using System.Xml;

namespace UnitTests {
    public class TestColony {
        private Planet planet = null!;
        private Race humanRace = null!;
        private ItemType medikit = null!;

        [SetUp]
        public void Setup() {
            Assert.IsTrue(StaticData.LoadAll());
            planet = new Planet();
            planet.Temperature = 290;
            humanRace = StaticData.GetRaceByName("Human") ?? throw new Exception("Could not find Human Race");
            medikit = StaticData.GetItemTypeByName("Medikit") ?? throw new Exception("Could not find Medikit");
        }

        private Colony MakeColony(int size) {
            return new Colony(humanRace, size, 0, planet, new GlobalClock(DateTime.Now));
        }

        private Colony LoadColony(string xmlText) {
            XmlDocument xml = new XmlDocument();
            xml.LoadXml(xmlText);
            return new Colony(xml.DocumentElement!, planet);
        }

        #region Construction

        [Test]
        public void Constructor_NewOutpost_HasOnlyOutpostType() {
            Colony cl = MakeColony(1);
            Assert.NotNull(cl);
            Assert.AreEqual(humanRace, cl.Owner);
            Assert.True(cl.HasBaseType(Colony.BaseType.Outpost));
            Assert.False(cl.HasBaseType(Colony.BaseType.Colony));
            Assert.False(cl.HasBaseType(Colony.BaseType.Trading));
            Assert.False(cl.HasBaseType(Colony.BaseType.Military));
            Assert.False(cl.HasBaseType(Colony.BaseType.Research));
            Assert.False(cl.HasBaseType(Colony.BaseType.Metropolis));
            Assert.True(cl.CanGrow);
            Assert.AreEqual(1, cl.BaseSize);
            Assert.AreEqual(1, humanRace.ColonyCount);
            Assert.AreEqual(1, humanRace.SystemCount);
            Assert.AreEqual(1, humanRace.Population);
        }

        [Test]
        public void Constructor_NewMetropolis_HasAllBaseTypes() {
            Colony cl = MakeColony(6);
            Assert.True(cl.HasBaseType(Colony.BaseType.Outpost));
            Assert.True(cl.HasBaseType(Colony.BaseType.Colony));
            Assert.True(cl.HasBaseType(Colony.BaseType.Trading));
            Assert.True(cl.HasBaseType(Colony.BaseType.Military));
            Assert.True(cl.HasBaseType(Colony.BaseType.Research));
            Assert.True(cl.HasBaseType(Colony.BaseType.Metropolis));
            Assert.False(cl.CanGrow);
            Assert.AreEqual(6, cl.BaseSize);
            Assert.AreEqual(1, humanRace.ColonyCount);
            Assert.AreEqual(1, humanRace.SystemCount);
            Assert.AreEqual(6, humanRace.Population);
        }

        [Test]
        public void Constructor_NewSize5_HasAllTypesExceptMetropolis() {
            Colony cl = MakeColony(5);
            Assert.True(cl.HasBaseType(Colony.BaseType.Outpost));
            Assert.True(cl.HasBaseType(Colony.BaseType.Colony));
            Assert.True(cl.HasBaseType(Colony.BaseType.Trading));
            Assert.True(cl.HasBaseType(Colony.BaseType.Military));
            Assert.True(cl.HasBaseType(Colony.BaseType.Research));
            Assert.False(cl.HasBaseType(Colony.BaseType.Metropolis));
            Assert.True(cl.CanGrow);
            Assert.AreEqual(5, cl.BaseSize);
            Assert.AreEqual(1, humanRace.ColonyCount);
            Assert.AreEqual(1, humanRace.SystemCount);
            Assert.AreEqual(5, humanRace.Population);
        }

        [Test]
        public void Constructor_Size3_HasThreeBaseTypes() {
            Colony cl = MakeColony(3);
            Assert.AreEqual(3, cl.BaseSize);
            Assert.True(cl.HasBaseType(Colony.BaseType.Outpost));
            Assert.True(cl.CanGrow);
        }

        [Test]
        public void Constructor_OnMoon_CannotGrow() {
            Moon moon = new Moon(1, planet, 0);
            Colony cl = new Colony(humanRace, 1, 0, moon, new GlobalClock(DateTime.Now));
            Assert.False(cl.CanGrow);
            Assert.AreEqual(DateTime.MaxValue, cl.dtNextGrowth);
        }

        [Test]
        public void HasBaseType_None_IsTrue() {
            Colony cl = MakeColony(1);
            Assert.True(cl.HasBaseType(Colony.BaseType.None));
        }

        #endregion

        #region Expand / facilities

        [Test]
        public void ExpandBase_FixedType_AddsColony() {
            Colony cl = MakeColony(1);
            Assert.True(cl.HasBaseType(Colony.BaseType.Outpost));
            Assert.False(cl.HasBaseType(Colony.BaseType.Colony));
            cl.ExpandBase(Colony.BaseType.Colony);
            Assert.True(cl.HasBaseType(Colony.BaseType.Colony));
            Assert.AreEqual(2, cl.BaseSize);
        }

        [Test]
        public void ExpandBase_Random_IncreasesSize() {
            Colony cl = MakeColony(1);
            var mockRandom = new Mock<Random>();
            mockRandom.Setup(x => x.Next(It.IsAny<int>())).Returns(1);
            Assert.True(cl.HasBaseType(Colony.BaseType.Outpost));
            Assert.False(cl.HasBaseType(Colony.BaseType.Colony));
            cl.ExpandBase(mockRandom.Object);
            Assert.AreEqual(2, cl.BaseSize);
            Assert.AreEqual(2, humanRace.Population);
        }

        [Test]
        public void ExpandBase_OnMoon_ReturnsZero() {
            Moon moon = new Moon(1, planet, 0);
            Colony cl = new Colony(humanRace, 1, 0, moon, new GlobalClock(DateTime.Now));
            int grown = cl.ExpandBase(new Random(1));
            Assert.AreEqual(0, grown);
            Assert.AreEqual(1, cl.BaseSize);
        }

        [Test]
        public void CanSeed_Outpost_IsFalse() {
            Assert.False(MakeColony(1).CanSeed);
        }

        [Test]
        public void CanSeed_Size5_IsTrue() {
            Assert.True(MakeColony(5).CanSeed);
        }

        [Test]
        public void CanRepairShips_Outpost_IsFalse() {
            Assert.False(MakeColony(1).CanRepairShips);
        }

        [Test]
        public void CanRepairShips_Size5_IsTrue() {
            Assert.True(MakeColony(5).CanRepairShips);
        }

        [Test]
        public void CostModifier_Outpost_IsTwo() {
            Assert.AreEqual(2.0, MakeColony(1).CostModifier);
        }

        #endregion

        #region Inventory

        [Test]
        public void GetAvailability_UnknownItem_ReturnsZero() {
            Colony cl = MakeColony(1);
            Assert.AreEqual(0, cl.GetAvailability(new Equipment(medikit)));
        }

        [Test]
        public void ResetStock_PopulatesInventory() {
            Colony cl = MakeColony(5);
            cl.ResetStock();
            Assert.IsNotEmpty(cl.InventoryList().ToList());
        }

        [Test]
        public void RemoveItem_LastUnit_RemovesFromStock() {
            Colony cl = MakeColony(5);
            cl.ResetStock();
            IItem item = cl.InventoryList().First();
            int count = cl.GetAvailability(item);
            Assert.Greater(count, 0);
            for (int n = 0; n < count; n++) cl.RemoveItem(item);
            Assert.AreEqual(0, cl.GetAvailability(item));
        }

        [Test]
        public void XmlConstructor_DuplicateInventoryRows_MergesCounts() {
            Colony cl = LoadColony("""
                <Colony>
                 <Owner>Human</Owner>
                 <Inventory>
                  <Inv Count="2"><Equipment Type="Medikit"/></Inv>
                  <Inv Count="3"><Equipment Type="Medikit"/></Inv>
                 </Inventory>
                </Colony>
                """);
            Assert.AreEqual(5, cl.GetAvailability(new Equipment(medikit)));
        }

        [Test]
        public void XmlConstructor_UnknownInventoryItem_Throws() {
            Assert.Throws<Exception>(() => LoadColony("""
                <Colony>
                 <Owner>Human</Owner>
                 <Inventory>
                  <Inv Count="1"><NotAnItem Type="Medikit"/></Inv>
                 </Inventory>
                </Colony>
                """));
        }

        #endregion

        #region Mercenaries / missions

        [Test]
        public void ResetMercenaries_Size5_AddsMercenaries() {
            Colony cl = MakeColony(5);
            cl.ResetMercenaries();
            Assert.IsNotEmpty(cl.MercenariesList().ToList());
        }

        [Test]
        public void RemoveMercenary_Unknown_Throws() {
            Colony cl = MakeColony(1);
            Soldier bob = new Soldier("Bob", humanRace, 10, 10, 10, 10, 10, GenderType.Male, 1, 0);
            Assert.Throws<Exception>(() => cl.RemoveMercenary(bob));
        }

        [Test]
        public void AddMission_Null_IsNoOp() {
            Colony cl = MakeColony(1);
            cl.AddMission(null!);
            Assert.IsEmpty(cl.MissionsList().ToList());
        }

        #endregion
    }
}
