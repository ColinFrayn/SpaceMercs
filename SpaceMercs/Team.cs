using SpaceMercs.Dialogs;
using System.IO;
using System.Xml;

namespace SpaceMercs {
    public class Team {
        public AstronomicalObject CurrentPosition { get; set; }
        public HabitableAO? CurrentPositionHAO {  get { if (CurrentPosition is HabitableAO hao) return hao; return null; } }
        private readonly List<Soldier> _Soldiers = new List<Soldier>();
        public IEnumerable<Soldier> SoldiersRO { get { return _Soldiers.AsReadOnly(); } }
        public int SoldierCount { get { return _Soldiers.Count; } }
        public Ship PlayerShip { get; private set; }
        public double Cash { get; set; }
        private readonly Dictionary<Race, int> Relations = new Dictionary<Race, int>();  // 0 = neutral, -5 = war, +5 = worshipful
        public readonly Dictionary<IItem, int> Inventory = new Dictionary<IItem, int>();
        public Mission? CurrentMission { get; private set; }
        public bool Mission_ShowLabels { get; set; }
        public bool Mission_ShowStatBars { get; set; }
        public bool Mission_ShowTravel { get; set; }
        public bool Mission_ShowPath { get; set; }
        public bool Mission_ShowEffects { get; set; }
        public bool Mission_ViewDetection { get; set; }

        // Calculated
        public double CargoMass { get { double c = 0.0; foreach (KeyValuePair<IItem, int> kvp in Inventory) c += kvp.Key.Mass * kvp.Value; return c; } }
        public int ActiveSoldiers { get { return _Soldiers.Where(x => x.IsActive).Count(); } }

        public Team() {
            CurrentPosition = Planet.Empty;
            PlayerShip = Ship.Empty;
        }
        public Team(NewGame newGame, Race playerRace) {
            CurrentPosition = playerRace.HomePlanet;
            Random rand = new Random();
            Soldier s = new Soldier(newGame.PlayerName, playerRace, newGame.Strength, newGame.Agility, newGame.Intellect, newGame.Toughness, newGame.Endurance, GenderType.Male, 1, rand.Next());
            s.PlayerTeam = this;
            _Soldiers.Add(s);
            PlayerShip = Ship.GenerateStarterShip(this);
            Cash = Const.InitialCash;
            Relations.Add(playerRace, 2);
            Mission_ShowLabels = Mission_ShowStatBars = false;
            Mission_ShowTravel = Mission_ShowPath = true;
        }
        public Team(XmlNode xml, Map map) {
            XmlNode? xmll = xml.SelectSingleNode("Pos");
            if (xmll is null) throw new Exception("Could not locate Team Position node");
            AstronomicalObject? ao = map.GetAOFromLocationString(xmll.InnerText);
            if (ao is null) throw new Exception("Could not decode Team Position : " + xmll.InnerText);
            CurrentPosition = ao;

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

            file.WriteLine(" <Relations>");
            foreach (Race rc in Relations.Keys) {
                file.WriteLine("  <Relation Race=\"" + rc.Name + "\" Value=\"" + Relations[rc].ToString() + "\"/>");
            }
            file.WriteLine(" </Relations>");

            file.WriteLine(" <Inventory>");
            foreach (IItem it in Inventory.Keys) {
                file.WriteLine("  <Inv Count=\"" + Inventory[it] + "\">");
                it.SaveToFile(file);
                file.WriteLine("  </Inv>");
            }
            file.WriteLine(" </Inventory>");

            if (CurrentMission != null) CurrentMission.SaveToFile(file);

            file.WriteLine("</Team>");
        }

        public bool CanTravel(AstronomicalObject aoTarget) {
            double dist = AstronomicalObject.CalculateDistance(CurrentPosition, aoTarget);
            if (dist < 0.0) return false; // Error
            double range = PlayerShip.Range;
            return (range >= dist);
        }

        public double GetPriceModifier(Race? rc) {
            if (rc is null || !Relations.ContainsKey(rc)) return 100.0;
            return Utils.RelationsToCostMod(Relations[rc]);
        }
        public double GetLocalPriceModifier() {
            return GetPriceModifier(CurrentPosition?.GetSystem()?.Owner);
        }

        public double GetRelations(Race rc) {
            if (rc == null) return 0;
            if (!Relations.ContainsKey(rc)) return 0;
            return Relations[rc];
        }
        public double GetRelations(AstronomicalObject ao) {
            if (ao == null) return 0;
            Star sys = ao.GetSystem();
            if (sys.Owner is null) return 0;
            return GetRelations(sys.Owner);
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
            int CargoMax = PlayerShip.Type.Cargo;
            if (CargoMax - CargoMass < equip.Mass) return Count;
            if (!Inventory.ContainsKey(equip)) Inventory.Add(equip, 0);
            int NumToTake = Math.Min((int)Math.Floor((CargoMax - CargoMass) / equip.Mass), Count);
            Inventory[equip] += NumToTake;
            return Count - NumToTake;
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

        // Skill stuff
        public bool HasSkill(Soldier.UtilitySkill sk) {
            foreach (Soldier s in _Soldiers.Where(s => s.aoLocation == CurrentPosition)) {
                if (s.GetUtilityLevel(sk) > 0) return true;
            }
            return false;
        }
        private int MaxSkillLevel(Soldier.UtilitySkill sk) {
            int lvl = 0;
            foreach (Soldier s in _Soldiers.Where(s => s.aoLocation == CurrentPosition)) {
                if (s.GetUtilityLevel(sk) > lvl) lvl = s.GetUtilityLevel(sk);
            }
            return lvl;
        }
        private Soldier MaxSkillSoldier(Soldier.UtilitySkill sk) {
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


    }
}