using SpaceMercs;

namespace UnitTests {
    public class TestColony {
        private Planet planet;

        [SetUp]
        public void Setup() {
            Assert.IsTrue(StaticData.LoadAll());
            planet = new Planet();
        }

        [Test]
        public void Test_SetupNewOutpost() {
            Race humanRace = StaticData.GetRaceByName("Human") ?? throw new Exception("Could not find Human Race");
            Colony cl = new Colony(humanRace, 1, 0, planet);
            Assert.NotNull(cl);
            Assert.AreEqual(humanRace, cl.Owner);
            Assert.True(cl.HasBaseType(Colony.BaseType.Outpost));
            Assert.False(cl.HasBaseType(Colony.BaseType.Colony));
            Assert.False(cl.HasBaseType(Colony.BaseType.Trading));
            Assert.False(cl.HasBaseType(Colony.BaseType.Military));
            Assert.False(cl.HasBaseType(Colony.BaseType.Research));
            Assert.False(cl.HasBaseType(Colony.BaseType.Metropolis));
            Assert.True(cl.CanGrow);
            Assert.AreEqual(1, humanRace.ColonyCount);
            Assert.AreEqual(1, humanRace.SystemCount);
            Assert.AreEqual(1, humanRace.Population);
        }

        [Test]
        public void Test_SetupNewMetropolis() {
            Race humanRace = StaticData.GetRaceByName("Human") ?? throw new Exception("Could not find Human Race");
            Colony cl = new Colony(humanRace, 6, 0, planet);
            Assert.NotNull(cl);
            Assert.True(cl.HasBaseType(Colony.BaseType.Outpost));
            Assert.True(cl.HasBaseType(Colony.BaseType.Colony));
            Assert.True(cl.HasBaseType(Colony.BaseType.Trading));
            Assert.True(cl.HasBaseType(Colony.BaseType.Military));
            Assert.True(cl.HasBaseType(Colony.BaseType.Research));
            Assert.True(cl.HasBaseType(Colony.BaseType.Metropolis));
            Assert.False(cl.CanGrow);
            Assert.AreEqual(1, humanRace.ColonyCount);
            Assert.AreEqual(1, humanRace.SystemCount);
            Assert.AreEqual(6, humanRace.Population);
        }

        [Test]
        public void Test_SetupNewSize5() {
            Race humanRace = StaticData.GetRaceByName("Human") ?? throw new Exception("Could not find Human Race");
            Colony cl = new Colony(humanRace, 5, 0, planet);
            Assert.NotNull(cl);
            Assert.True(cl.HasBaseType(Colony.BaseType.Outpost));
            Assert.True(cl.HasBaseType(Colony.BaseType.Colony));
            Assert.True(cl.HasBaseType(Colony.BaseType.Trading));
            Assert.True(cl.HasBaseType(Colony.BaseType.Military));
            Assert.True(cl.HasBaseType(Colony.BaseType.Research));
            Assert.False(cl.HasBaseType(Colony.BaseType.Metropolis));
            Assert.True(cl.CanGrow);
            Assert.AreEqual(1, humanRace.ColonyCount);
            Assert.AreEqual(1, humanRace.SystemCount);
            Assert.AreEqual(5, humanRace.Population);
        }

        [Test]
        public void Test_ExpandBase_FixedType() {
            Race humanRace = StaticData.GetRaceByName("Human") ?? throw new Exception("Could not find Human Race");
            Colony cl = new Colony(humanRace, 1, 0, planet);
            Assert.True(cl.HasBaseType(Colony.BaseType.Outpost));
            Assert.False(cl.HasBaseType(Colony.BaseType.Colony));
            cl.ExpandBase(Colony.BaseType.Colony);
            Assert.True(cl.HasBaseType(Colony.BaseType.Colony));
        }

        [Test]
        public void Test_ExpandBase_Random() {
            Race humanRace = StaticData.GetRaceByName("Human") ?? throw new Exception("Could not find Human Race");
            Colony cl = new Colony(humanRace, 1, 0, planet);
            Random rand = new Random();
            Assert.True(cl.HasBaseType(Colony.BaseType.Outpost));
            Assert.False(cl.HasBaseType(Colony.BaseType.Colony));
            cl.ExpandBase(rand);
            Assert.AreEqual(2, cl.BaseSize);
            Assert.AreEqual(2, humanRace.Population);
        }
    }
}