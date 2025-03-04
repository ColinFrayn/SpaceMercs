using SpaceMercs.Dialogs;
using SpaceMercs.Items;
using System.IO;
using System.Text;
using System.Xml;
using static SpaceMercs.Delegates;

namespace SpaceMercs {
    public class Team {
        public AstronomicalObject CurrentPosition { get; private set; }
        public HabitableAO? CurrentPositionHAO {  get { if (CurrentPosition is HabitableAO hao) return hao; return null; } }
        private readonly List<Soldier> _Soldiers = new List<Soldier>();
        public IEnumerable<Soldier> SoldiersRO { get { return _Soldiers.AsReadOnly(); } }
        public int SoldierCount { get { return _Soldiers.Count; } }
        public Ship PlayerShip { get; private set; }
        public double Cash { get; set; }
        private readonly Dictionary<Race, int> Relations = new Dictionary<Race, int>();  // This is the experience level
        public readonly Dictionary<IItem, int> Inventory = new Dictionary<IItem, int>();
        public Mission? CurrentMission { get; private set; }
        public bool Mission_ShowLabels { get; set; }
        public bool Mission_ShowStatBars { get; set; }
        public bool Mission_ShowTravel { get; set; }
        public bool Mission_ShowPath { get; set; }
        public bool Mission_ShowEffects { get; set; }
        public bool Mission_ViewDetection { get; set; }
        public bool Mission_FastAI { get; set; }

        // Calculated
        public double CargoMass { get { double c = 0.0; foreach (KeyValuePair<IItem, int> kvp in Inventory) c += kvp.Key.Mass * kvp.Value; return c; } }
        public int ActiveSoldiers { get { return _Soldiers.Where(x => x.IsActive).Count(); } }

        // Stats
        public int PrecursorCoreCount { get; private set; }
        public int PrecursorCoreRecord { get; private set; }
        public int SpaceHulkCoreCount { get; private set; }
        public int SpaceHulkCoreRecord { get; private set; }
        public int CompletedMissions { get; private set; }
        public int ToughestMission { get; private set; }

        public Team() {
            CurrentPosition = Planet.Empty;
            PlayerShip = Ship.Empty;
        }
        public Team(NewGame newGame, Race playerRace) {
            CurrentPosition = playerRace.HomePlanet;
            Random rand = new Random();
            Soldier s = new Soldier(newGame.PlayerName, playerRace, newGame.Strength, newGame.Agility, newGame.Insight, newGame.Toughness, newGame.Endurance, GenderType.Male, 1, rand.Next());
            s.PlayerTeam = this;
            _Soldiers.Add(s);
            PlayerShip = Ship.GenerateStarterShip(this);
            Cash = Const.InitialCash + Const.DEBUG_ADDITIONAL_STARTING_CASH;
            Relations.Add(playerRace, Const.StartingRelationsWithHomeRace);
            Mission_ShowLabels = Mission_ShowStatBars = false;
            Mission_ShowTravel = Mission_ShowPath = true;
        }
        public Team(XmlNode xml, Map map) {
            XmlNode? xmll = xml.SelectSingleNode("Pos") ?? throw new Exception("Could not locate Team Position node");
            string strLoc = xmll.InnerText;
            try {
                AstronomicalObject? ao = map.GetAOFromLocationString(strLoc);
                if (ao is null) throw new NullReferenceException($"Could not decode Team Position : {xmll.InnerText} : Got null location");
                CurrentPosition = ao;
            }
            catch (NullReferenceException) {
                throw; // This is really bad
            }
            catch {
                // Here we couldn't identify the location. We might be able to rescue this in case it was a missing moon that was regenerated differently.
                // In this case, skip back to the parent planet
                int pos = strLoc.IndexOf('.');
                if (pos == -1) throw; // OK we're totally screwed as this was not a moon
                strLoc = strLoc.Substring(0, pos); // This was a moon, so try the parent planet
                AstronomicalObject? ao = map.GetAOFromLocationString(strLoc);
                if (ao is null) throw new NullReferenceException($"Could not decode Team Position : {xmll.InnerText} : Also could not move back to parent planet. Aborting game load.");
                CurrentPosition = ao;
            }

            Cash = xml.SelectNodeDouble("Cash");

            XmlNode xmls = xml.SelectSingleNode("Soldiers") ?? throw new Exception("Could not find team Soldiers data");
            foreach (XmlNode xs in xmls.ChildNodes) {
                Soldier s = new Soldier(xs, this);
                _Soldiers.Add(s);
            }

            PlayerShip = new Ship(xml.SelectSingleNode("Ship") ?? throw new Exception("Could not find team Ship data"), this);

            // Mission GUI options
            Mission_ShowLabels = (xml.SelectSingleNode("Mission_ShowLabels") != null);
            Mission_ShowStatBars = (xml.SelectSingleNode("Mission_ShowStatBars") != null);
            Mission_ShowTravel = (xml.SelectSingleNode("Mission_ShowTravel") != null);
            Mission_ShowPath = (xml.SelectSingleNode("Mission_ShowPath") != null);
            Mission_ShowEffects = (xml.SelectSingleNode("Mission_ShowEffects") != null);
            Mission_ViewDetection = (xml.SelectSingleNode("Mission_ViewDetection") != null);
            Mission_FastAI = (xml.SelectSingleNode("Mission_FastAI") != null);

            XmlNode xmlr = xml.SelectSingleNode("Relations") ?? throw new Exception("Could not find team Relations data");
            Relations.Clear();
            foreach (XmlNode xr in xmlr.ChildNodes) {
                string strRace = xr.GetAttributeText("Race");
                Race rc = StaticData.GetRaceByName(strRace) ?? throw new Exception($"Found unknown Race : {strRace}");
                int rel = xr.GetAttributeInt("Value");
                Relations.Add(rc, rel);
            }

            XmlNode xmli = xml.SelectSingleNode("Inventory") ?? throw new Exception("Could not find team Inventory data");
            Inventory.Clear();
            foreach (XmlNode xi in xmli.ChildNodes) {
                int count = xi.GetAttributeInt("Count");
                IItem eq = Utils.LoadItem(xi.FirstChild) ?? throw new Exception("Could not load inventory item!");
                if (Inventory.ContainsKey(eq)) Inventory[eq] += count;
                else Inventory.Add(eq, count);
            }

            XmlNode? xMission = xml.SelectSingleNode("Mission");
            if (xMission is not null) {
                CurrentMission = new Mission(xMission, CurrentPositionHAO!);
                // If it's a tactical mission then insert all participating soldiers
                if (CurrentMission.IsTacticalMission) {
                    foreach (Soldier s in _Soldiers) {
                        if (s.OnMission) CurrentMission.GetOrCreateCurrentLevel().AddSoldierAtCurrentLocation(s);
                    }
                    CurrentMission.SetCreatureTargets();
                }
            }

            // Stats
            PrecursorCoreCount = xml.SelectNodeInt("PrecursorCoreCount", 0);
            SpaceHulkCoreCount = xml.SelectNodeInt("SpaceHulkCoreCount", 0);
            PrecursorCoreRecord = xml.SelectNodeInt("PrecursorCoreRecord", 0);
            SpaceHulkCoreRecord = xml.SelectNodeInt("SpaceHulkCoreRecord", 0);
            CompletedMissions = xml.SelectNodeInt("CompletedMissions", 0);
            ToughestMission = xml.SelectNodeInt("ToughestMission", 0);
        }

        public static Team Empty() => new Team();

        // Save this team to an Xml file
        public void SaveToFile(StreamWriter file) {
            file.WriteLine("<Team>");
            file.WriteLine(" <Pos>" + CurrentPosition.PrintCoordinates() + "</Pos>");
            file.WriteLine(" <Cash>" + Cash.ToString() + "</Cash>");

            file.WriteLine(" <Soldiers>");
            foreach (Soldier s in _Soldiers) {
                s.SaveToFile(file);
            }
            file.WriteLine(" </Soldiers>");

            PlayerShip.SaveToFile(file);

            if (Mission_ShowLabels) file.WriteLine(" <Mission_ShowLabels/>");
            if (Mission_ShowStatBars) file.WriteLine(" <Mission_ShowStatBars/>");
            if (Mission_ShowTravel) file.WriteLine(" <Mission_ShowTravel/>");
            if (Mission_ShowPath) file.WriteLine(" <Mission_ShowPath/>");
            if (Mission_ShowEffects) file.WriteLine(" <Mission_ShowEffects/>");
            if (Mission_ViewDetection) file.WriteLine(" <Mission_ViewDetection/>");
            if (Mission_FastAI) file.WriteLine(" <Mission_FastAI/>");

            file.WriteLine(" <Relations>");
            foreach (Race rc in Relations.Keys) {
                file.WriteLine($"  <Relation Race=\"{rc.Name}\" Value=\"{Relations[rc]}\"/>");
            }
            file.WriteLine(" </Relations>");

            file.WriteLine(" <Inventory>");
            foreach (IItem it in Inventory.Keys) {
                file.WriteLine($"  <Inv Count=\"{Inventory[it]}\">");
                it.SaveToFile(file);
                file.WriteLine("  </Inv>");
            }
            file.WriteLine(" </Inventory>");

            if (CurrentMission != null) CurrentMission.SaveToFile(file);

            // Statistics
            if (SpaceHulkCoreCount > 0) file.WriteLine($" <SpaceHulkCoreCount>{SpaceHulkCoreCount}</SpaceHulkCoreCount>");
            if (PrecursorCoreCount > 0) file.WriteLine($" <PrecursorCoreCount>{PrecursorCoreCount}</PrecursorCoreCount>");
            if (SpaceHulkCoreRecord > 0) file.WriteLine($" <SpaceHulkCoreRecord>{SpaceHulkCoreRecord}</SpaceHulkCoreRecord>");
            if (PrecursorCoreRecord > 0) file.WriteLine($" <PrecursorCoreRecord>{PrecursorCoreRecord}</PrecursorCoreRecord>");

            file.WriteLine("</Team>");
        }

        public bool CanTravel(AstronomicalObject aoTarget) {
            double dist = AstronomicalObject.CalculateDistance(CurrentPosition, aoTarget);
            if (dist < 0.0) return false; // Error
            double range = PlayerShip.Range;
            return (range >= dist);
        }
        public void SetPosition(AstronomicalObject aoTarget) {
            CurrentPosition = aoTarget;
            foreach (Soldier s in _Soldiers) {
                if (s.IsActive) s.aoLocation = aoTarget;
            }
        }

        public double GetPriceModifier(Race? rc, Star st) {
            // We have not met this race, or it doesn't exist, so impossible to trade
            if (rc is null || !Relations.ContainsKey(rc)) return 100.0;
            // Cost mod based on relations with the owning race
            double mod = Utils.RelationsToCostMod(GetRelations(rc));
            // Unconnected systems are expensive because it's hard to get goods there
            if (!st.TradeRoutes.Any()) mod *= Const.UnconnectedColonyCostMod;
            return mod;
        }
        public double GetLocalPriceModifier() {
            return GetPriceModifier(CurrentPosition.GetSystem().Owner, CurrentPosition.GetSystem());
        }

        public int GetRelations(Race rc) {
            if (rc == null) return 0;
            int exp = Relations.ContainsKey(rc) ? Relations[rc] : rc.BaseAttitude;
            return Utils.ExperienceToRelations(exp);
        }
        public double GetRelationsProgress(Race rc) {
            if (rc == null) return 0;
            int exp = Relations.ContainsKey(rc) ? Relations[rc] : rc.BaseAttitude;
            double fract = Utils.ExperienceToRelationsFraction(exp);
            return fract;
        }
        public int GetRelations(AstronomicalObject ao) {
            if (ao == null) return 0;
            Star sys = ao.GetSystem();
            if (sys.Owner is null) return 0;
            return GetRelations(sys.Owner);
        }
        public void ImproveRelations(Race? rc, int exp, ShowMessageDelegate showMessage) {
            if (rc == null) return;
            int oldRelations = GetRelations(rc);
            // Get all currently unresearchable techs
            HashSet<IResearchable> oldUnresearchable = UnresearchableItems.ToHashSet();
            if (!Relations.ContainsKey(rc)) {
                Relations.Add(rc, rc.BaseAttitude + exp);
            }
            else Relations[rc] += exp;
            int newRelations = GetRelations(rc);
            if (newRelations > oldRelations) {
                showMessage($"Thanks to your efforts, relations with the {rc.Name} race have improved\nYou are now considered {Utils.RelationsToString(newRelations)}", null);
                IEnumerable<IResearchable> newResearchable = oldUnresearchable.Except(UnresearchableItems);
                if (newResearchable.Any()) {
                    string msg = $"Collaboration with {rc.Name} scientists have made available new technological advances.\n";
                    if (newResearchable.OfType<BaseItemType>().Any()) {
                        msg += $"The following new item research is now available:\n{String.Join("\n", newResearchable.OfType<BaseItemType>().Select(it => it.Name))}\n";
                    }
                    if (newResearchable.OfType<MaterialType>().Any()) {
                        msg += $"The following new material research is now available:\n{String.Join("\n", newResearchable.OfType<MaterialType>().Select(it => it.Name))}";
                    }
                    showMessage(msg, null);
                }
            }
            if (newRelations < oldRelations) {
                showMessage($"Because of your persistent attacks, relations with the {rc.Name} race have worsened\nYou are now considered {Utils.RelationsToString(newRelations)}", null);
            }
        }
        public void SetBaselineRelations(Race rc) {
            if (!Relations.ContainsKey(rc)) {
                Relations.Add(rc, rc.BaseAttitude);
            }
        }

        // Add a list of items to the team's inventory if we have space
        public Dictionary<IItem, int> AddItems(Dictionary<IItem, int> dEquip) {
            Dictionary<IItem, int> dRemainder = new Dictionary<IItem, int>();
            foreach (IItem e in dEquip.Keys) {
                int rem = AddItem(e, dEquip[e]);
                if (rem > 0) dRemainder.Add(e, rem);
            }
            return dRemainder;
        }

        // Add a single item, if we can, and return how many were left over
        public int AddItem(IItem? equip, int Count = 1) {
            if (equip is null) return Count;
            int CargoMax = PlayerShip.Type.Capacity;
            if (CargoMax - CargoMass < equip.Mass) return Count;
            if (!Inventory.ContainsKey(equip)) Inventory.Add(equip, 0);
            int NumToTake = Math.Min((int)Math.Floor((CargoMax - CargoMass) / equip.Mass), Count);
            Inventory[equip] += NumToTake;
            return Count - NumToTake;
        }

        public int GetSpareBerths() {
            int total = PlayerShip.TotalBerths;
            foreach (Soldier s in _Soldiers) {
                if (s.IsActive) total--;
            }
            return total;
        }
        public void AddSoldier(Soldier merc) {
            _Soldiers.Add(merc);
            merc.PlayerTeam = this;
        }
        public void RemoveSoldier(Soldier s) {
            _Soldiers.Remove(s);
            s.PlayerTeam = null;
        }
        public void SetTeamShip(ShipType st) {
            PlayerShip = new Ship(st);
            PlayerShip.SetOwner(this);
            PlayerShip.AddBuiltEquipmentAutoSlot(StaticData.GetShipEquipmentByName("Fission Core"));
            PlayerShip.AddBuiltEquipmentAutoSlot(StaticData.GetShipWeaponByName("Chain Gun"));
            PlayerShip.AddBuiltEquipmentAutoSlot(StaticData.GetShipEngineByName("Thrusters"));
            PlayerShip.InitialiseForBattle();
        }

        public void SetCurrentMission(Mission? miss) {
            CurrentMission = miss;
        }
        public void CeaseMission() {
            foreach (Soldier s in _Soldiers) {
                s.StopMission();
            }
            CurrentMission = null;
        }
        public void RegisterMissionCompletion(Mission miss) {
            CompletedMissions++;
            if (miss.Diff > ToughestMission) ToughestMission = miss.Diff;
        }
        public void RegisterMissionItem(MissionItem mitem) {
            if (mitem.IsPrecursorCore) {
                PrecursorCoreCount++;
                if (mitem.Level > PrecursorCoreRecord) PrecursorCoreRecord = mitem.Level;
            }
            else if (mitem.IsSpaceHulkCore) {
                SpaceHulkCoreCount++;
                if (mitem.Level > SpaceHulkCoreRecord) SpaceHulkCoreRecord = mitem.Level;
            }
        }

        public void RemoveItemFromStores(IItem? equip, int Count = 1) {
            if (equip is null) return;
            if (!Inventory.ContainsKey(equip)) return;
            if (Count >= Inventory[equip]) Inventory.Remove(equip);
            else Inventory[equip] -= Count;
        }
        public bool RemoveItemFromStoresOrSoldiers(IItem? equip, int Count = 1) {
            if (equip is null) return false;
            if (Inventory.ContainsKey(equip)) {
                if (Count >= Inventory[equip]) {
                    Count -= Inventory[equip];
                    Inventory.Remove(equip);
                    if (Count == 0) return true;
                }
                else {
                    Inventory[equip] -= Count;
                    return true;
                }
            }
            foreach (Soldier s in _Soldiers) {
                if (s.HasItem(equip)) {
                    if (Count >= s.InventoryGrouped[equip]) {
                        int n = s.InventoryGrouped[equip];
                        s.DestroyItem(equip, Count);
                        Count -= n;
                        if (Count == 0) return true;
                    }
                    else {
                        s.DestroyItem(equip, Count);
                        return true;
                    }

                }
            }
            return false;
        }
        public int CountMaterial(MaterialType mat) {
            int count = 0;
            foreach (IItem it in Inventory.Keys) {
                if (it is Material m) {
                    if (m.BaseType == mat) count += Inventory[it];
                }
            }
            foreach (Soldier s in _Soldiers.Where(s => s.aoLocation == CurrentPosition)) {
                foreach (IItem it in s.InventoryGrouped.Keys) {
                    if (it is Material m) {
                        if (m.BaseType == mat) count += s.InventoryGrouped[it];
                    }
                }
            }
            return count;
        }
        public void RemoveMaterial(MaterialType mat, int num) {
            // Remove what we can from ship's inventory
            foreach (IItem it in Inventory.Keys) {
                if (it is Material m) {
                    if (m.BaseType == mat) {
                        int left = num - Inventory[it];
                        RemoveItemFromStores(it, num);
                        if (left <= 0) return;
                        num = left;
                    }
                }
            }

            // If there's any left, take it from soldiers
            foreach (Soldier s in _Soldiers.Where(s => s.aoLocation == CurrentPosition)) {
                foreach (IItem it in s.InventoryGrouped.Keys) {
                    if (it is Material m) {
                        if (m.BaseType == mat) {
                            int left = num - s.InventoryGrouped[it];
                            s.DestroyItem(it, num);
                            if (left <= 0) return;
                            num = left;
                        }
                    }
                }
            }

            // If there's any left, error
            throw new Exception("Couldn't delete materials from Team - insufficient avaialble");
        }
        public int CountItems(IItem itFind) {
            int count = 0;
            if (Inventory.ContainsKey(itFind)) count += Inventory[itFind];
            foreach (Soldier s in _Soldiers.Where(s => s.aoLocation == CurrentPosition)) {
                if (s.InventoryGrouped.ContainsKey(itFind)) count += s.InventoryGrouped[itFind];
            }
            return count;
        }
        public int CountItemsInStores(IItem itFind) {
            if (Inventory.ContainsKey(itFind)) return Inventory[itFind];
            return 0;
        }
        public IEnumerable<IResearchable> UnresearchableItems {
            get {
                List<IResearchable> items = new List<IResearchable>();
                Race humanRace = StaticData.HumanRace;
                foreach (BaseItemType it in StaticData.ResearchableBaseItems) {
                    if (humanRace.HasResearched(it)) continue;
                    if (it.Requirements?.TeamMeetsBasicRequirements(this) == false) {
                        items.Add(it);
                    }
                }
                foreach (MaterialType mat in StaticData.ResearchableMaterialTypes) {
                    if (humanRace.HasResearched(mat)) continue;
                    if (mat.Requirements?.TeamMeetsBasicRequirements(this) == false) {
                        items.Add(mat);
                    }
                }
                return items;
            }
        }
        public IEnumerable<IResearchable> ResearchableItems {
            get {
                List<IResearchable> items = new List<IResearchable>();
                Race humanRace = StaticData.HumanRace;
                foreach (BaseItemType it in StaticData.ResearchableBaseItems) {
                    if (humanRace.HasResearched(it)) continue;
                    if (it.Requirements?.TeamMeetsBasicRequirements(this) == true) {
                        items.Add(it);
                    }
                }
                foreach (MaterialType mat in StaticData.ResearchableMaterialTypes) {
                    if (humanRace.HasResearched(mat)) continue;
                    if (mat.Requirements?.TeamMeetsBasicRequirements(this) == true) {
                        items.Add(mat);
                    }
                }
                return items;
            }
        }

        // Skill stuff
        public bool HasSkill(Soldier.UtilitySkill sk) {
            foreach (Soldier s in _Soldiers.Where(s => s.aoLocation == CurrentPosition)) {
                if (s.GetUtilityLevel(sk) > 0) return true;
            }
            return false;
        }
        public int MaxSkillLevel(Soldier.UtilitySkill sk) {
            int lvl = 0;
            foreach (Soldier s in _Soldiers.Where(s => s.aoLocation == CurrentPosition)) {
                if (s.GetUtilityLevel(sk) > lvl) lvl = s.GetUtilityLevel(sk);
            }
            return lvl;
        }
        public Soldier MaxSkillSoldier(Soldier.UtilitySkill sk) {
            Soldier? sbest = null;
            foreach (Soldier s in _Soldiers.Where(s => s.aoLocation == CurrentPosition)) {
                if (sbest is null || s.GetUtilityLevel(sk) > sbest.GetUtilityLevel(sk)) sbest = s;
            }
            if (sbest is null) throw new Exception($"Couldn't find any soldier with the skill : {sk}");
            return sbest!;
        }
        public int GetMaxSkillByItemType(ItemType newType) {
            if (newType is ArmourType) return MaxSkillLevel(Soldier.UtilitySkill.Armoursmith);
            else if (newType is WeaponType) {
                if (((WeaponType)newType).IsMeleeWeapon) return MaxSkillLevel(Soldier.UtilitySkill.Bladesmith);
                else return MaxSkillLevel(Soldier.UtilitySkill.Gunsmith);
            }
            if (newType.Source == ItemType.ItemSource.Medlab) return MaxSkillLevel(Soldier.UtilitySkill.Medic);
            else if (newType.Source == ItemType.ItemSource.Workshop) return MaxSkillLevel(Soldier.UtilitySkill.Engineer);
            throw new Exception("Unknown skill required in item construction: " + newType.Source.ToString());
        }
        public Soldier GetSoldierWithMaxSkillByItemType(ItemType newType) {
            if (newType is ArmourType) return MaxSkillSoldier(Soldier.UtilitySkill.Armoursmith);
            else if (newType is WeaponType) {
                if (((WeaponType)newType).IsMeleeWeapon) return MaxSkillSoldier(Soldier.UtilitySkill.Bladesmith);
                else return MaxSkillSoldier(Soldier.UtilitySkill.Gunsmith);
            }
            if (newType.Source == ItemType.ItemSource.Medlab) return MaxSkillSoldier(Soldier.UtilitySkill.Medic);
            else if (newType.Source == ItemType.ItemSource.Workshop) return MaxSkillSoldier(Soldier.UtilitySkill.Engineer);
            throw new Exception("Unknown skill required in item construction: " + newType.Source.ToString());
        }
        public int GetMaxSkillByItem(IItem it) {
            if (it is Armour) return MaxSkillLevel(Soldier.UtilitySkill.Armoursmith);
            else if (it is Weapon wp) {
                if (wp.Range == 0) return MaxSkillLevel(Soldier.UtilitySkill.Bladesmith);
                else return MaxSkillLevel(Soldier.UtilitySkill.Gunsmith);
            }
            if (it is Equipment eq) {
                if (eq.BaseType.Source == ItemType.ItemSource.Medlab) return MaxSkillLevel(Soldier.UtilitySkill.Medic);
                else if (eq.BaseType.Source == ItemType.ItemSource.Workshop) return MaxSkillLevel(Soldier.UtilitySkill.Engineer);
            }
            throw new Exception("Unknown skill required in item construction: " + it.ToString());
        }
        public int MaximumSoldierLevel => _Soldiers.Where(s => s.aoLocation == CurrentPosition)?.MaxBy(s => s.Level)?.Level ?? 0;

        // Utility
        public void ShowStatistics(ShowMessageDelegate showMessage) {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Team Statistics");
            sb.AppendLine($"Completed Missions : {CompletedMissions}");
            if (ToughestMission > 0) sb.AppendLine($"Toughest Mission : Level {ToughestMission}");
            if (SpaceHulkCoreCount > 0) sb.AppendLine($"SpaceHulk Cores Found : {SpaceHulkCoreCount}");
            if (SpaceHulkCoreRecord > 0) sb.AppendLine($"Highest Difficulty SpaceHulk Core : {SpaceHulkCoreRecord}");
            if (PrecursorCoreCount > 0) sb.AppendLine($"Precursor Cores Found : {PrecursorCoreCount}");
            if (PrecursorCoreRecord > 0) sb.AppendLine($"Highest Difficulty Precursor Core : {PrecursorCoreRecord}");
            showMessage(sb.ToString(), null);
        }
    }
}