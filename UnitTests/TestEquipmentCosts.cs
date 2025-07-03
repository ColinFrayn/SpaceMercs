using SpaceMercs;

namespace UnitTests {
    internal class TestEquipmentCosts {
        private TestContext testContextInstance;

        /// <summary>
        /// Gets or sets the test context which provides
        /// information about and functionality for the current test run.
        /// </summary>
        public TestContext TestContext {
            get { return testContextInstance; }
            set { testContextInstance = value; }
        }

        [SetUp]
        public void Setup() {
            Assert.IsTrue(StaticData.LoadAll());
        }

        [Test]
        public void TestItemCosts() {
            foreach (ItemType it in StaticData.ItemTypes) {
                Equipment eq = new Equipment(it);
                double cost = eq.Cost;
                double matCost = 0d;
                foreach ((MaterialType mat, int count) in it.Materials) {
                    matCost += mat.UnitCost * count;
                }
                double ratio = cost / matCost;
                if (ratio < 0.75) {
                    TestContext.WriteLine($"ItemType {it.Name} : Cost = {cost}cr : MatCost = {matCost}cr : Ratio = {ratio:N2} ** SOLD CHEAPLY **");
                }
                else if (ratio > 2.0) {
                    TestContext.WriteLine($"ItemType {it.Name} : Cost = {cost}cr : MatCost = {matCost}cr : Ratio = {ratio:N2} ** EASY PROFIT **");
                }
                else {
                    TestContext.WriteLine($"ItemType {it.Name} : Cost = {cost}cr : MatCost = {matCost}cr : Ratio = {ratio:N2}");
                }
            }
        }

        [Test]
        public void TestWeaponCosts() {
            foreach (WeaponType wt in StaticData.WeaponTypes) {
                if (!wt.IsUsable) continue; // Not usable = creature weapons
                if (wt.Requirements?.Researchable == false) continue; // Unresearchable = precursor tech
                Weapon wp = new Weapon(wt, 0);
                double cost = wp.Cost;
                double matCost = 0d;
                foreach ((MaterialType mat, int count) in wt.Materials) {
                    matCost += mat.UnitCost * count;
                }
                double ratio = cost / matCost;
                if (ratio < 0.75) {
                    TestContext.WriteLine($"WeaponType {wt.Name} : Cost = {cost}cr : MatCost = {matCost}cr : Ratio = {ratio:N2} ** SOLD CHEAPLY **");
                }
                else if (ratio > 2.0) {
                    TestContext.WriteLine($"WeaponType {wt.Name} : Cost = {cost}cr : MatCost = {matCost}cr : Ratio = {ratio:N2} ** EASY PROFIT **");
                }
                else {
                    TestContext.WriteLine($"WeaponType {wt.Name} : Cost = {cost}cr : MatCost = {matCost}cr : Ratio = {ratio:N2}");
                }
            }
        }

        [Test]
        public void TestArmourCosts() {
            foreach (ArmourType at in StaticData.ArmourTypes) {
                if (at.Requirements?.Researchable == false) continue; // Unresearchable = precursor tech
                //if (!at.Name.Contains("Powered Arm Plating")) continue; // TODO
                foreach (MaterialType mt in StaticData.Materials) {
                    if (!mt.IsArmourMaterial) continue;
                    if (mt.MaxLevel < at.MinMatLvl) continue;
                    Armour ar = new Armour(at, mt, 0);
                    double cost = ar.Cost;
                    double matCost = mt.UnitCost * at.BaseMaterialRequirements; 
                    foreach ((MaterialType mat, int count) in at.Materials) {
                        matCost += mat.UnitCost * count;
                    }
                    double ratio = cost / matCost;
                    if (ratio < 0.75) {
                        TestContext.WriteLine($"ArmourType {mt.Name} {at.Name} : Cost = {cost:N2}cr : MatCost = {matCost:N2}cr : Ratio = {ratio:N2} ** SOLD CHEAPLY **");
                    }
                    else if (ratio > 2.0) {
                        TestContext.WriteLine($"ArmourType {mt.Name} {at.Name} : Cost = {cost:N2}cr : MatCost = {matCost:N2}cr : Ratio = {ratio:N2} ** EASY PROFIT **");
                    }
                    else {
                        TestContext.WriteLine($"ArmourType {mt.Name} {at.Name} : Cost = {cost:N2}cr : MatCost = {matCost:N2}cr : Ratio = {ratio:N2}");
                    }
                }
            }
        }
    }
}
