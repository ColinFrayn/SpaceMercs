using SpaceMercs;
using System.Text;
using System.Xml;

namespace UnitTests {
    public class TestTeam {
        private Race humanRace = null!;
        private ItemType medikit = null!;
        private ItemType grenade = null!;
        private ItemType medsystem = null!;
        private MaterialType fabric = null!;
        private WeaponType pistol = null!;
        private WeaponType shortblade = null!;
        private ArmourType helmet = null!;

        [SetUp]
        public void Setup() {
            Assert.IsTrue(StaticData.LoadAll());
            humanRace = StaticData.GetRaceByName("Human") ?? throw new Exception("Could not find Human Race");
            medikit = StaticData.GetItemTypeByName("Medikit") ?? throw new Exception("Could not find Medikit");
            grenade = StaticData.GetItemTypeByName("Frag Grenade") ?? throw new Exception("Could not find Frag Grenade");
            medsystem = StaticData.GetItemTypeByName("Medsystem") ?? throw new Exception("Could not find Medsystem");
            fabric = StaticData.GetMaterialTypeByName("Fabric") ?? throw new Exception("Could not find Fabric");
            pistol = StaticData.GetWeaponTypeByName("Pistol") ?? throw new Exception("Could not find Pistol");
            shortblade = StaticData.GetWeaponTypeByName("Shortblade") ?? throw new Exception("Could not find Shortblade");
            helmet = StaticData.GetArmourTypeByName("Military Helmet") ?? throw new Exception("Could not find Military Helmet");
        }

        private static Soldier MakeSoldier(string name = "Bob", int level = 1) {
            Race rc = StaticData.GetRaceByName("Human") ?? throw new Exception("Could not find Human Race");
            return new Soldier(name, rc, 10, 10, 10, 10, 10, GenderType.Male, level, 0);
        }

        private static IItem FabricItem() {
            XmlDocument xml = new XmlDocument();
            xml.LoadXml("<Material Type=\"Fabric\"/>");
            return Utils.LoadItem(xml.DocumentElement) ?? throw new Exception("Could not load Fabric");
        }

        #region Construction

        [Test]
        public void Empty_HasNoSoldiers() {
            Team emptyTeam = Team.Empty();
            Assert.IsNotNull(emptyTeam);
            Assert.IsEmpty(emptyTeam.SoldiersRO);
            Assert.AreEqual(0, emptyTeam.SoldierCount);
            Assert.AreEqual(0, emptyTeam.ActiveSoldiers);
        }

        [Test]
        public void AddCash_IncreasesBalance() {
            Team team = TestUtils.TestTeam();
            double before = team.Cash;
            team.AddCash(12.5);
            Assert.AreEqual(before + 12.5, team.Cash, 0.0001);
        }

        [Test]
        public void AddCash_Negative_Throws() {
            Team team = TestUtils.TestTeam();
            Assert.Throws<ArgumentOutOfRangeException>(() => team.AddCash(-1));
        }

        [Test]
        public void TrySpend_Sufficient_DeductsAndReturnsTrue() {
            Team team = TestUtils.TestTeam();
            double before = team.Cash;
            Assert.True(team.TrySpend(10));
            Assert.AreEqual(before - 10, team.Cash, 0.0001);
        }

        [Test]
        public void TrySpend_Insufficient_IsNoOp() {
            Team team = TestUtils.TestTeam();
            double before = team.Cash;
            Assert.False(team.TrySpend(before + 1));
            Assert.AreEqual(before, team.Cash);
        }

        #endregion

        #region Inventory

        [Test]
        public void AddItem_SingleItem_AddsToInventory() {
            Team team = TestUtils.TestTeam();
            Assert.AreEqual(0, team.Inventory.Count);
            Equipment eq = new Equipment(medikit);
            Assert.AreEqual(0, team.AddItem(eq));
            Assert.AreEqual(1, team.Inventory.Count);
            Assert.True(team.Inventory.ContainsKey(eq));
            Assert.AreEqual(1, team.Inventory[eq]);
        }

        [Test]
        public void AddItem_WithCount_Stacks() {
            Team team = TestUtils.TestTeam();
            Equipment eq = new Equipment(medikit);
            Assert.AreEqual(0, team.AddItem(eq, 20));
            Assert.AreEqual(1, team.Inventory.Count);
            Assert.True(team.Inventory.ContainsKey(eq));
            Assert.AreEqual(20, team.Inventory[eq]);
        }

        [Test]
        public void AddItem_TwoTypes_StackSeparately() {
            Team team = TestUtils.TestTeam();
            Equipment eq1 = new Equipment(medikit);
            Equipment eq2 = new Equipment(grenade);
            team.AddItem(eq1);
            team.AddItem(eq2);
            team.AddItem(eq1);
            team.AddItem(eq2);
            team.AddItem(eq1);
            Assert.AreEqual(2, team.Inventory.Count);
            Assert.True(team.Inventory.ContainsKey(eq1));
            Assert.True(team.Inventory.ContainsKey(eq2));
            Assert.AreEqual(3, team.Inventory[eq1]);
            Assert.AreEqual(2, team.Inventory[eq2]);
        }

        [Test]
        public void AddItem_ClonesSoCallerHandleIsIndependent() {
            Team team = TestUtils.TestTeam();
            Equipment eq = new Equipment(medsystem);
            Assert.AreEqual(0, team.AddItem(eq, 3));
            eq.SetRecharge(99);
            Equipment stored = team.Inventory.Keys.OfType<Equipment>().Single(e => e.BaseType == medsystem);
            Assert.AreEqual(0, stored.Recharge);
            Assert.AreEqual(3, team.CountItemsInStores(new Equipment(medsystem)));
            Assert.AreEqual(0, team.CountItemsInStores(eq));
        }

        [Test]
        public void AddItem_CountZeroOrNegative_IsNoOp() {
            Team team = TestUtils.TestTeam();
            Equipment eq = new Equipment(medikit);
            Assert.AreEqual(0, team.AddItem(eq, 0));
            Assert.AreEqual(-3, team.AddItem(eq, -3));
            Assert.AreEqual(0, team.Inventory.Count);
        }

        [Test]
        public void AddItem_Null_ReturnsCount() {
            Team team = TestUtils.TestTeam();
            Assert.AreEqual(4, team.AddItem(null, 4));
            Assert.AreEqual(0, team.Inventory.Count);
        }

        [Test]
        public void AddItem_ExceedsCapacity_ReturnsRemainder() {
            Team team = TestUtils.TestTeam();
            Equipment eq = new Equipment(medikit);
            int capacity = team.PlayerShip.Type.Capacity;
            int maxFit = (int)Math.Floor(capacity / eq.Mass);
            int leftover = team.AddItem(eq, maxFit + 10);
            Assert.AreEqual(10, leftover);
            Assert.AreEqual(maxFit, team.Inventory[eq]);
        }

        [Test]
        public void AddItems_PartialFit_ReturnsRemainder() {
            Team team = TestUtils.TestTeam();
            Equipment eq = new Equipment(medikit);
            int capacity = team.PlayerShip.Type.Capacity;
            int maxFit = (int)Math.Floor(capacity / eq.Mass);
            Dictionary<IItem, int> incoming = new() { { eq, maxFit + 5 } };
            Dictionary<IItem, int> rem = team.AddItems(incoming);
            Assert.AreEqual(1, rem.Count);
            Assert.AreEqual(5, rem[eq]);
            Assert.AreEqual(maxFit, team.Inventory[eq]);
        }

        [Test]
        public void CargoMass_SumsItemMassTimesCount() {
            Team team = TestUtils.TestTeam();
            Equipment kit = new Equipment(medikit);
            Equipment nade = new Equipment(grenade);
            team.AddItem(kit, 3);
            team.AddItem(nade, 2);
            Assert.AreEqual(kit.Mass * 3 + nade.Mass * 2, team.CargoMass, 0.0001);
        }

        [Test]
        public void RemoveItemFromStores_MatchingItem_Removes() {
            Team team = TestUtils.TestTeam();
            Equipment eq = new Equipment(medikit);
            team.AddItem(eq);
            team.RemoveItemFromStores(eq);
            Assert.AreEqual(0, team.Inventory.Count);
        }

        [Test]
        public void RemoveItemFromStores_WrongType_IsNoOp() {
            Team team = TestUtils.TestTeam();
            Equipment eq = new Equipment(medikit);
            Equipment eq2 = new Equipment(grenade);
            team.AddItem(eq);
            team.RemoveItemFromStores(eq2);
            Assert.AreEqual(1, team.Inventory.Count);
            Assert.True(team.Inventory.ContainsKey(eq));
            Assert.AreEqual(1, team.Inventory[eq]);
        }

        [Test]
        public void RemoveItemFromStores_PartialCount_Decrements() {
            Team team = TestUtils.TestTeam();
            Equipment eq = new Equipment(medikit);
            team.AddItem(eq, 5);
            team.RemoveItemFromStores(eq, 2);
            Assert.AreEqual(3, team.Inventory[eq]);
        }

        [Test]
        public void CountItemsInStores_IgnoresSoldierInventory() {
            Team team = TestUtils.TestTeam();
            Equipment eq = new Equipment(medikit);
            team.AddItem(eq, 2);
            Soldier s = team.SoldiersRO.First();
            s.AddItem(new Equipment(medikit), 3);
            Assert.AreEqual(2, team.CountItemsInStores(eq));
            Assert.AreEqual(5, team.CountItems(eq));
        }

        [Test]
        public void RemoveItemFromStoresOrSoldiers_TakesFromStoresFirst() {
            Team team = TestUtils.TestTeam();
            Equipment eq = new Equipment(medikit);
            team.AddItem(eq, 2);
            Soldier s = team.SoldiersRO.First();
            s.AddItem(new Equipment(medikit), 3);
            Assert.True(team.RemoveItemFromStoresOrSoldiers(eq, 2));
            Assert.AreEqual(0, team.CountItemsInStores(eq));
            Assert.AreEqual(3, s.CountItem(eq));
        }

        [Test]
        public void RemoveItemFromStoresOrSoldiers_TakesFromSoldierWhenStoresEmpty() {
            Team team = TestUtils.TestTeam();
            Equipment eq = new Equipment(medikit);
            Soldier s = team.SoldiersRO.First();
            s.AddItem(new Equipment(medikit), 3);
            Assert.True(team.RemoveItemFromStoresOrSoldiers(eq, 2));
            Assert.AreEqual(1, s.CountItem(eq));
        }

        [Test]
        public void RemoveItemFromStoresOrSoldiers_MissingItem_ReturnsFalse() {
            Team team = TestUtils.TestTeam();
            Assert.False(team.RemoveItemFromStoresOrSoldiers(new Equipment(medikit)));
        }

        [Test]
        public void CountMaterial_IncludesStoresAndActiveSoldiers() {
            Team team = TestUtils.TestTeam();
            IItem mat = FabricItem();
            team.AddItem(mat, 4);
            Soldier s = team.SoldiersRO.First();
            s.AddItem(FabricItem(), 3);
            Assert.AreEqual(7, team.CountMaterial(fabric));
        }

        [Test]
        public void CountMaterial_IgnoresInactiveSoldiers() {
            Team team = TestUtils.TestTeam();
            Soldier s = team.SoldiersRO.First();
            s.AddItem(FabricItem(), 3);
            s.Deactivate();
            Assert.AreEqual(0, team.CountMaterial(fabric));
        }

        [Test]
        public void RemoveMaterial_FromStores_RemovesRequestedAmount() {
            Team team = TestUtils.TestTeam();
            team.AddItem(FabricItem(), 5);
            team.RemoveMaterial(fabric, 3);
            Assert.AreEqual(2, team.CountMaterial(fabric));
        }

        [Test]
        public void RemoveMaterial_FromActiveSoldier_WhenStoresEmpty() {
            Team team = TestUtils.TestTeam();
            Soldier s = team.SoldiersRO.First();
            s.AddItem(FabricItem(), 4);
            team.RemoveMaterial(fabric, 2);
            Assert.AreEqual(2, team.CountMaterial(fabric));
            Assert.AreEqual(2, s.CountItem(FabricItem()));
        }

        [Test]
        public void RemoveMaterial_SplitBetweenStoresAndSoldier_RemovesAll() {
            Team team = TestUtils.TestTeam();
            Soldier s = team.SoldiersRO.First();
            team.AddItem(FabricItem(), 3);
            s.AddItem(FabricItem(), 2);
            Assert.DoesNotThrow(() => team.RemoveMaterial(fabric, 5));
            Assert.AreEqual(0, team.CountMaterial(fabric));
            Assert.AreEqual(0, s.CountItem(FabricItem()));
        }

        [Test]
        public void RemoveMaterial_Insufficient_Throws() {
            Team team = TestUtils.TestTeam();
            team.AddItem(FabricItem(), 1);
            Assert.Throws<Exception>(() => team.RemoveMaterial(fabric, 5));
        }

        #endregion

        #region Soldiers and berths

        [Test]
        public void AddSoldier_SetsPlayerTeamAndCounts() {
            Team team = TestUtils.TestTeam();
            int before = team.SoldierCount;
            Soldier s = MakeSoldier();
            team.AddSoldier(s);
            Assert.AreEqual(before + 1, team.SoldierCount);
            Assert.AreSame(team, s.PlayerTeam);
            Assert.AreEqual(before + 1, team.ActiveSoldiers);
        }

        [Test]
        public void AddSoldier_NoSpareBerths_Throws() {
            Team team = TestUtils.TestTeam();
            while (team.GetSpareBerths() > 0) {
                team.AddSoldier(MakeSoldier($"Extra{team.SoldierCount}"));
            }
            Assert.AreEqual(0, team.GetSpareBerths());
            Assert.Throws<Exception>(() => team.AddSoldier(MakeSoldier("OneTooMany")));
        }

        [Test]
        public void RemoveSoldier_ClearsPlayerTeam() {
            Team team = TestUtils.TestTeam();
            Soldier s = MakeSoldier();
            team.AddSoldier(s);
            team.RemoveSoldier(s);
            Assert.False(team.SoldiersRO.Contains(s));
            Assert.IsNull(s.PlayerTeam);
        }

        [Test]
        public void GetSpareBerths_Deactivate_FreesBerth() {
            Team team = TestUtils.TestTeam();
            int spare = team.GetSpareBerths();
            team.SoldiersRO.First().Deactivate();
            Assert.AreEqual(spare + 1, team.GetSpareBerths());
            Assert.AreEqual(0, team.ActiveSoldiers);
        }

        [Test]
        public void SetPosition_UpdatesActiveSoldiersOnly() {
            Team team = TestUtils.TestTeam();
            Soldier extra = MakeSoldier();
            team.AddSoldier(extra);
            extra.Deactivate();
            AstronomicalObject? oldLoc = extra.aoLocation;
            Planet dest = Planet.Empty;
            team.SetPosition(dest);
            Assert.AreSame(dest, team.CurrentPosition);
            Assert.AreSame(dest, team.SoldiersRO.First(s => s.IsActive).aoLocation);
            Assert.AreEqual(oldLoc, extra.aoLocation);
        }

        #endregion

        #region Skills

        [Test]
        public void HasSkill_ActiveSoldier_ReturnsTrue() {
            Team team = TestUtils.TestTeam();
            Soldier s = team.SoldiersRO.First();
            Assert.False(team.HasSkill(Soldier.UtilitySkill.Engineer));
            s.AddUtilitySkill(Soldier.UtilitySkill.Engineer, 2);
            Assert.True(team.HasSkill(Soldier.UtilitySkill.Engineer));
            Assert.AreEqual(2, team.MaxSkillLevel(Soldier.UtilitySkill.Engineer));
            Assert.AreSame(s, team.MaxSkillSoldier(Soldier.UtilitySkill.Engineer));
        }

        [Test]
        public void HasSkill_InactiveSoldier_IsIgnored() {
            Team team = TestUtils.TestTeam();
            Soldier s = team.SoldiersRO.First();
            s.AddUtilitySkill(Soldier.UtilitySkill.Engineer, 3);
            s.Deactivate();
            Assert.False(team.HasSkill(Soldier.UtilitySkill.Engineer));
            Assert.AreEqual(0, team.MaxSkillLevel(Soldier.UtilitySkill.Engineer));
        }

        [Test]
        public void MaxSkillSoldier_NoActiveSoldiers_Throws() {
            Team team = TestUtils.TestTeam();
            team.SoldiersRO.First().Deactivate();
            Assert.Throws<Exception>(() => team.MaxSkillSoldier(Soldier.UtilitySkill.Engineer));
        }

        [Test]
        public void GetMaxSkillByItemType_UsesMatchingUtilitySkill() {
            Team team = TestUtils.TestTeam();
            Soldier s = team.SoldiersRO.First();
            s.AddUtilitySkill(Soldier.UtilitySkill.Medic, 2);
            s.AddUtilitySkill(Soldier.UtilitySkill.Gunsmith, 3);
            s.AddUtilitySkill(Soldier.UtilitySkill.Bladesmith, 4);
            s.AddUtilitySkill(Soldier.UtilitySkill.Armoursmith, 5);
            Assert.AreEqual(2, team.GetMaxSkillByItemType(medikit));
            Assert.AreEqual(3, team.GetMaxSkillByItemType(pistol));
            Assert.AreEqual(4, team.GetMaxSkillByItemType(shortblade));
            Assert.AreEqual(5, team.GetMaxSkillByItemType(helmet));
            Assert.AreEqual(2, team.GetMaxSkillByItem(new Equipment(medikit)));
            Assert.AreSame(s, team.GetSoldierWithMaxSkillByItemType(medikit));
        }

        [Test]
        public void MaximumSoldierLevel_UsesHighestActive() {
            Team team = TestUtils.TestTeam();
            Assert.AreEqual(1, team.MaximumSoldierLevel);
            team.AddSoldier(MakeSoldier("High", 7));
            Assert.AreEqual(7, team.MaximumSoldierLevel);
            team.SoldiersRO.First(s => s.Level == 7).Deactivate();
            Assert.AreEqual(1, team.MaximumSoldierLevel);
        }

        #endregion

        #region Relations

        [Test]
        public void GetRelations_HomeRace_UsesStartingExperience() {
            Team team = TestUtils.TestTeam();
            Assert.AreEqual(Utils.ExperienceToRelations(Const.StartingRelationsWithHomeRace), team.GetRelations(humanRace));
        }

        [Test]
        public void GetRelations_UnmetRace_UsesBaseAttitude() {
            Team team = TestUtils.TestTeam();
            Race gokan = StaticData.GetRaceByName("Gokan") ?? throw new Exception("Could not find Gokan");
            Assert.AreEqual(Utils.ExperienceToRelations(gokan.BaseAttitude), team.GetRelations(gokan));
        }

        [Test]
        public void ImproveRelations_NullRace_IsNoOp() {
            Team team = TestUtils.TestTeam();
            Assert.DoesNotThrow(() => team.ImproveRelations(null, 1000, (_, _) => throw new Exception("should not message")));
        }

        [Test]
        public void ImproveRelations_SmallChange_DoesNotMessage() {
            Team team = TestUtils.TestTeam();
            List<string> msgs = new();
            team.ImproveRelations(humanRace, 1, (s, _) => msgs.Add(s));
            Assert.IsEmpty(msgs);
            Assert.AreEqual(Utils.ExperienceToRelations(Const.StartingRelationsWithHomeRace), team.GetRelations(humanRace));
        }

        [Test]
        public void ImproveRelations_Drop_UsesWorsenReason() {
            Team team = TestUtils.TestTeam();
            List<string> msgs = new();
            team.ImproveRelations(humanRace, Const.FoundColonyInContestedSystemRelationsPenalty, (s, _) => msgs.Add(s), "your colony in their claimed system");
            Assert.AreEqual(1, msgs.Count);
            Assert.True(msgs[0].Contains("your colony in their claimed system"));
            Assert.True(msgs[0].Contains("worsened"));
            Assert.Less(team.GetRelations(humanRace), Utils.ExperienceToRelations(Const.StartingRelationsWithHomeRace));
        }

        [Test]
        public void ImproveRelations_Drop_DefaultReason() {
            Team team = TestUtils.TestTeam();
            List<string> msgs = new();
            team.ImproveRelations(humanRace, Const.FoundColonyInContestedSystemRelationsPenalty, (s, _) => msgs.Add(s));
            Assert.AreEqual(1, msgs.Count);
            Assert.True(msgs[0].Contains("your actions"));
        }

        [Test]
        public void SetBaselineRelations_UnmetRace_DoesNotOverwriteLater() {
            Team team = TestUtils.TestTeam();
            Race gokan = StaticData.GetRaceByName("Gokan") ?? throw new Exception("Could not find Gokan");
            team.SetBaselineRelations(gokan);
            Assert.AreEqual(Utils.ExperienceToRelations(gokan.BaseAttitude), team.GetRelations(gokan));
            team.SetBaselineRelations(gokan);
            team.ImproveRelations(gokan, 1, (_, _) => { });
            team.SetBaselineRelations(gokan);
            Assert.AreEqual(Utils.ExperienceToRelations(gokan.BaseAttitude + 1), team.GetRelations(gokan));
        }

        #endregion

        #region Missions and cores

        [Test]
        public void RegisterMissionCompletion_IncrementsAndTracksToughest() {
            Team team = TestUtils.TestTeam();
            team.RegisterMissionCompletion(Mission.CreateIgnoreMission());
            Assert.AreEqual(1, team.CompletedMissions);
            Assert.AreEqual(0, team.ToughestMission);
            team.RegisterMissionCompletion(Mission.CreateRepelBoardersMission(humanRace, 7, team.PlayerShip));
            Assert.AreEqual(2, team.CompletedMissions);
            Assert.AreEqual(7, team.ToughestMission);
            team.RegisterMissionCompletion(Mission.CreateRepelBoardersMission(humanRace, 3, team.PlayerShip));
            Assert.AreEqual(3, team.CompletedMissions);
            Assert.AreEqual(7, team.ToughestMission);
        }

        [Test]
        public void SetCurrentMission_ThenCeaseMission_ClearsMission() {
            Team team = TestUtils.TestTeam();
            Mission miss = Mission.CreateIgnoreMission();
            team.SetCurrentMission(miss);
            Assert.AreSame(miss, team.CurrentMission);
            team.CeaseMission();
            Assert.IsNull(team.CurrentMission);
        }

        [Test]
        public void RegisterMissionItem_TracksPrecursorAndSpaceHulkCores() {
            Team team = TestUtils.TestTeam();
            team.RegisterMissionItem(MissionItem.GeneratePrecursorCore(4));
            team.RegisterMissionItem(MissionItem.GeneratePrecursorCore(2));
            team.RegisterMissionItem(MissionItem.GenerateSpaceHulkCore(6));
            Assert.AreEqual(2, team.PrecursorCoreCount);
            Assert.AreEqual(4, team.PrecursorCoreRecord);
            Assert.AreEqual(1, team.SpaceHulkCoreCount);
            Assert.AreEqual(6, team.SpaceHulkCoreRecord);
        }

        [Test]
        public void HasPrecursorCore_TrueWhenInStores() {
            Team team = TestUtils.TestTeam();
            Assert.False(team.HasPrecursorCore);
            team.AddItem(MissionItem.GeneratePrecursorCore(1));
            Assert.True(team.HasPrecursorCore);
        }

        [Test]
        public void HasSpaceHulkCore_TrueWhenOnActiveSoldier() {
            Team team = TestUtils.TestTeam();
            Assert.False(team.HasSpaceHulkCore);
            team.SoldiersRO.First().AddItem(MissionItem.GenerateSpaceHulkCore(1));
            Assert.True(team.HasSpaceHulkCore);
            team.SoldiersRO.First().Deactivate();
            Assert.False(team.HasSpaceHulkCore);
        }

        [Test]
        public void ShowStatistics_IncludesCompletedMissions() {
            Team team = TestUtils.TestTeam();
            team.RegisterMissionCompletion(Mission.CreateRepelBoardersMission(humanRace, 5, team.PlayerShip));
            string? text = null;
            team.ShowStatistics((s, _) => text = s);
            Assert.IsNotNull(text);
            Assert.True(text!.Contains("Completed Missions : 1"));
            Assert.True(text.Contains("Toughest Mission : Level 5"));
        }

        [Test]
        public void SaveToFile_WritesMissionStats() {
            Team team = TestUtils.TestTeam();
            team.RegisterMissionCompletion(Mission.CreateRepelBoardersMission(humanRace, 5, team.PlayerShip));
            team.RegisterMissionItem(MissionItem.GeneratePrecursorCore(3));
            using MemoryStream stream = new MemoryStream();
            using StreamWriter sw = new StreamWriter(stream);
            team.SaveToFile(sw);
            sw.Flush();
            string xml = Encoding.UTF8.GetString(stream.ToArray());
            Assert.True(xml.Contains("<CompletedMissions>1</CompletedMissions>"));
            Assert.True(xml.Contains("<ToughestMission>5</ToughestMission>"));
            Assert.True(xml.Contains("<PrecursorCoreCount>1</PrecursorCoreCount>"));
            Assert.True(xml.Contains("<PrecursorCoreRecord>3</PrecursorCoreRecord>"));
        }

        #endregion
    }
}
