using SpaceMercs.MainWindow;
using System.IO;
using System.Text;
using System.Xml;

namespace SpaceMercs {
    // Used for storing details of a mission to be undertaken
    class Mission {
        public enum MissionType { ShipCombat, RepelBoarders, BoardingParty, Surface, Caves, Mines, AbandonedCity, Repair, Salvage, Ignore }
        public enum MissionGoal { KillAll, ExploreAll, KillBoss, FindItem, Gather }
        public MissionType Type { get; private set; }
        public MissionGoal Goal { get; private set; }
        public Ship? ShipTarget { get; private set; }
        public Race? RacialOpponent { get; private set; }
        public CreatureGroup PrimaryEnemy { get; private set; }
        public HabitableAO Location { get; private set; }
        public float TimeCost { get; private set; }
        public double Reward { get; private set; }
        public int Diff { get; private set; }
        public int Size { get; private set; } // Relevant for caves, mines, dungeon & surface maps
        public int LevelCount { get; private set; }
        public int CurrentLevel { get; private set; }
        public bool IsShipMission { get { if (Type == MissionType.Surface || Type == MissionType.Caves || Type == MissionType.Mines || Type == MissionType.AbandonedCity) return false; else return true; } }
        public bool IsTacticalMission { get { if (Type == MissionType.Repair || Type == MissionType.Salvage || Type == MissionType.Ignore || Type == MissionType.ShipCombat) return false; else return true; } }
        public string Summary {
            get {
                switch (Type) {
                    case MissionType.RepelBoarders: return "Repel Boarders";
                    case MissionType.BoardingParty: return "Derelict Hulk";
                    case MissionType.Surface: return "Surface Mission";
                    case MissionType.Caves: return "Cave System";
                    case MissionType.Mines: return "Abandoned Mines";
                    case MissionType.ShipCombat: return "Ship Combat";
                    case MissionType.AbandonedCity: return "Ruined Structure";
                    case MissionType.Repair: return "Repair Ship";
                    case MissionType.Salvage: return "Salvage Empty Ship";
                    case MissionType.Ignore: return "Ignore";
                }
                return "Unknown Mission Type";
            }
        }
        public int Seed { get; private set; }
        public bool IsComplete {
            get {
                if (Levels.Count != LevelCount) return false; // Not on lowest level yet

                if (Goal == MissionGoal.KillAll) {
                    // Check that all enemies have been killed
                    foreach (MissionLevel lev in Levels.Values) {
                        if (!lev.AllEnemiesKilled) return false;
                    }
                    return true;
                }
                if (Goal == MissionGoal.KillBoss) {
                    if (Levels[LevelCount - 1] == null) return false;
                    foreach (Creature cr in Levels[LevelCount - 1].Creatures) {
                        if (cr.Type.IsBoss) return false;
                    }
                    return true;
                }
                if (Goal == MissionGoal.ExploreAll) {
                    foreach (MissionLevel lev in Levels.Values) {
                        if (!lev.FullyExplored) return false;
                    }
                    return true;
                }
                if (Goal == MissionGoal.FindItem) { // You can quit whenever you pick up the one single item
                    if (MItem is null) throw new Exception("No mission item in FindItem mission");
                    foreach (Soldier s in Soldiers) {
                        if (s.HasItem(MItem)) return true;
                    }
                    return false;
                }
                if (Goal == MissionGoal.Gather) { // You can quit whenever you pick up at least one item
                    if (MItem is null) throw new Exception("No mission item in Gather mission");
                    foreach (Soldier s in Soldiers) {
                        if (s.HasItem(MItem)) return true;
                    }
                    return false;
                }
                throw new Exception("Unknown mission goal");
            }
        }
        public int Experience { get { if (Goal == MissionGoal.Gather) return 0; return (Diff + 1) * (Diff + 1) * LevelCount * (Size * 2 + 8); } }
        public MissionItem? MItem { get; private set; }
        private string FullDescription;
        private Dictionary<int, MissionLevel> Levels = new Dictionary<int, MissionLevel>();
        public readonly List<Soldier> Soldiers = new List<Soldier>();
        public MapView CurrentMapView { get; private set; }

        public Mission(MissionType t, int dif, int sd = 0) {
            Goal = MissionGoal.KillAll;
            CurrentLevel = 0;
            LevelCount = 1;
            Diff = dif;
            Type = t;
            Seed = sd;
            Random rand = new Random(Seed);
            if (IsShipMission) {
                Size = 1;
            }
            else {
                Size = 100 + rand.Next(80) + rand.Next(80) + Math.Min((Diff * 5), 50) + rand.Next(Diff * 15);
                if (rand.Next(80) < Diff * 2 + 10) Size += 50;
                if (rand.Next(100) < Diff * 2 + 10) Size += 50;
                Size /= 100;
                if (Size < 1) Size = 1;
            }
            LevelCount = 1;
            if (Diff > 5.0 && !IsShipMission && Type != MissionType.Surface) {
                double d = Diff;
                int lc = 1;
                while (rand.NextDouble() * 5.0 < d) {
                    d -= 5.0;
                    lc++;
                }
                LevelCount = lc;
            }
            TimeCost = 0.0f;
            ShipTarget = null;
        }
        public Mission(XmlNode xml, HabitableAO loc) {
            Location = loc;
            Type = (MissionType)Enum.Parse(typeof(MissionType), xml.SelectNodeText("Type"));

            XmlNode? sn = xml.SelectSingleNode("Ship");
            if (sn is not null) {
                ShipTarget = new Ship(sn, null);
            }

            XmlNode? xg = xml.SelectSingleNode("Goal");
            if (xg is not null) {
                Goal = (MissionGoal)Enum.Parse(typeof(MissionGoal), xg.InnerText);
            }
            else Goal = MissionGoal.KillAll;

            XmlNode? xmi = xml.SelectSingleNode("MissionItem");
            if (xmi is not null) {
                MItem = new MissionItem(xmi);
            }
            else MItem = null;

            string? strOpp = xml.SelectNodeText("Opponent");
            if (!string.IsNullOrEmpty(strOpp)) {
                RacialOpponent = StaticData.GetRaceByName(strOpp) ?? throw new Exception("Could not ID RacialOpponent : " + strOpp);
            }

            string? strEn = xml.SelectNodeText("Enemy");
            if (!string.IsNullOrEmpty(strEn)) {
                PrimaryEnemy = StaticData.GetCreatureGroupByName(strEn) ?? throw new Exception("Could not ID PrimaryEnemy : " + strEn);
            }

            TimeCost = float.Parse(xml.SelectNodeText("TimeCost"));
            Reward = xml.SelectNodeDouble("Reward");
            Diff = xml.SelectNodeInt("Diff");
            LevelCount = xml.SelectNodeInt("LevelCount");
            CurrentLevel = xml.SelectNodeInt("CurrentLevel");

            Seed = xml.SelectNodeInt("Seed", 0);
            Size = xml.SelectNodeInt("Size", 1);

            foreach (XmlNode xl in xml.SelectNodesToList("Level")) {
                int id = xl.GetAttributeInt("ID");
                if (xl.FirstChild is not null) {
                    MissionLevel lev = new MissionLevel(xl.FirstChild, this);
                    Levels.Add(id, lev);
                }
            }
            GetDescription();
        }

        public void SaveToFile(StreamWriter file) {
            file.WriteLine("<Mission>");
            file.WriteLine(" <Type>" + Type + "</Type>");
            file.WriteLine(" <Goal>" + Goal + "</Goal>");
            if (MItem != null) MItem.SaveToFile(file);
            if (ShipTarget != null) ShipTarget.SaveToFile(file);
            if (RacialOpponent != null) file.WriteLine(" <Opponent>" + RacialOpponent.Name + "</Opponent>");
            if (PrimaryEnemy != null) file.WriteLine(" <Enemy>" + PrimaryEnemy.Name + "</Enemy>");
            file.WriteLine(" <TimeCost>" + TimeCost + "</TimeCost>");
            file.WriteLine(" <Reward>" + Reward + "</Reward>");
            file.WriteLine(" <Diff>" + Diff + "</Diff>");
            file.WriteLine(" <LevelCount>" + LevelCount + "</LevelCount>");
            file.WriteLine(" <CurrentLevel>" + CurrentLevel + "</CurrentLevel>");
            if (Seed != 0) file.WriteLine(" <Seed>" + Seed + "</Seed>");
            if (Size != 1) file.WriteLine(" <Size>" + Size + "</Size>");
            foreach (int lev in Levels.Keys) {
                file.WriteLine(" <Level ID=\"" + lev + "\">");
                Levels[lev].SaveToFile(file);
                file.WriteLine(" </Level>");
            }
            file.WriteLine("</Mission>");
        }

        public static Mission CreateIgnoreMission() {
            Mission m = new Mission(MissionType.Ignore, 0);
            return m;
        }
        public static Mission CreateRepairMission(Race rc, float fTime, double reward) {
            Mission m = new Mission(MissionType.Repair, 0);
            m.TimeCost = fTime;
            m.Reward = reward;
            m.RacialOpponent = rc;
            return m;
        }
        public static Mission CreateSalvageMission(Race rc, int dDiff, float fTime) {
            Mission m = new Mission(MissionType.Salvage, dDiff);
            m.RacialOpponent = rc;
            m.TimeCost = fTime;
            m.ShipTarget = Ship.GenerateRandomShipOfRace(rc, dDiff, null);
            return m;
        }
        public static Mission CreateBoardingPartyMission(Race rc, int dDiff) {
            Mission m = new Mission(MissionType.BoardingParty, dDiff);
            m.RacialOpponent = rc;
            m.ShipTarget = Ship.GenerateRandomShipOfRace(rc, dDiff, null);
            return m;
        }
        public static Mission CreateRepelBoardersMission(Race? rc, int dDiff, Ship sh) {
            Mission m = new Mission(MissionType.RepelBoarders, dDiff);
            m.RacialOpponent = rc;
            m.ShipTarget = sh;
            return m;
        }
        public static Mission CreateShipCombatMission(Race rc, int dDiff, ShipEngine minDrive) {
            Mission m = new Mission(MissionType.ShipCombat, dDiff);
            m.RacialOpponent = rc;
            m.ShipTarget = Ship.GenerateRandomShipOfRace(rc, dDiff, minDrive);
            return m;
        }
        public static Mission CreateRandomColonyMission(Colony cl, Random rand) {
            int iDiff = cl.Location.GetRandomMissionDifficulty(rand);
            MissionType tp = GenerateRandomColonyMissionType(cl.Location, rand);
            Mission m = new Mission(tp, iDiff, rand.Next());
            m.Location = cl.Location;
            if (rand.NextDouble() > 0.4) m.RacialOpponent = null; // Enemy is not a major race e.g. wildlife
            else m.RacialOpponent = cl.Location.GetRandomRace(rand);
            m.PrimaryEnemy = GetPrimaryEnemy(m, rand) ?? throw new Exception("Unable to get PrimaryEnemy for random Colony mission");
            if (m.Type == MissionType.BoardingParty) {
                if (m.RacialOpponent is null) throw new Exception("Attempting to create a boarding party mission with no opponent");
                m.ShipTarget = Ship.GenerateRandomShipOfRace(m.RacialOpponent, m.Diff, null);
                int sz = m.ShipTarget.Type.Width * m.ShipTarget.Type.Length;
                m.Size = (int)Math.Floor((Math.Log((double)sz / 100.0) / Math.Log(2))) + 1;
                if (m.Size < 1) m.Size = 1;
            }
            else {
                // Set mission goal
                Tuple<MissionGoal, MissionItem?> mgtp = GetRandomMissionGoal(m, rand);
                m.Goal = mgtp.Item1;
                m.MItem = mgtp.Item2;
            }
            m.Reward = Math.Round((rand.NextDouble() + (m.Size + 3.0)) * (m.Diff + 2.0) * (m.Diff + 2.0) * m.LevelCount / 5.0, 2);
            if (m.Goal == MissionGoal.KillBoss) m.Reward *= 0.9;
            if (m.Goal == MissionGoal.Gather) m.Reward = 0.0;
            return m;
        }
        public static Mission CreateRandomScannerMission(HabitableAO loc, Random rand) {
            int iDiff = loc.GetRandomMissionDifficulty(rand);
            MissionType tp = GenerateRandomScannerMissionType(rand);
            Mission m = new Mission(tp, iDiff, rand.Next());
            m.Location = loc;
            if (rand.NextDouble() > 0.4) m.RacialOpponent = null; // Enemy is not a major race e.g. wildlife
            else m.RacialOpponent = loc.GetRandomRace(rand);
            m.PrimaryEnemy = GetPrimaryEnemy(m, rand) ?? throw new Exception("Unable to get PrimaryEnemy for random Scanner mission");
            m.Reward = Math.Round((rand.NextDouble() + (m.Size + 3.0)) * (m.Diff + 2.0) * (m.Diff + 2.0) * m.LevelCount / 5.0, 2);
            if (m.Size < 1) m.Size = 1;

            // Set mission goal
            Tuple<MissionGoal, MissionItem?> mgtp = GetRandomMissionGoal(m, rand);
            m.Goal = mgtp.Item1;
            m.MItem = mgtp.Item2;
            if (m.Goal == MissionGoal.KillBoss) m.Reward *= 0.9;
            if (m.Goal == MissionGoal.Gather) m.Reward = 0.0;

            return m;
        }

        public static MissionType GenerateRandomColonyMissionType(AstronomicalObject loc, Random rand) {
            int r = rand.Next(100);
            if (!(loc is HabitableAO)) {
                throw new Exception("Trying to run a mission near an AO that is not a HabitableAO");
                //if (r < 40) return MissionType.BoardingParty;
                //return MissionType.ShipAssault;
            }
            if (((HabitableAO)loc).AOType == AstronomicalObject.AstronomicalObjectType.Planet && ((Planet)loc).Type == Planet.PlanetType.Gas) {
                return MissionType.BoardingParty;
            }
            if (r < 20) return MissionType.AbandonedCity;
            else if (r < 40) return MissionType.Caves;
            else if (r < 60) return MissionType.Mines;
            else if (r < 80) return MissionType.Surface;
            else return MissionType.BoardingParty;
        }
        public static MissionType GenerateRandomScannerMissionType(Random rand) {
            int r = rand.Next(100);
            if (r < 25) return MissionType.AbandonedCity;
            else if (r < 40) return MissionType.Mines;
            else if (r < 85) return MissionType.Caves;
            else return MissionType.Surface;
        }

        // Utility
        private static CreatureGroup? GetPrimaryEnemy(Mission m, Random rand) {
            double dTotalScore = 0.0;

            // If opponent race is null (i.e. a creature group of some type) then get a non-racial enemy
            Dictionary<CreatureGroup, double> dScores = new Dictionary<CreatureGroup, double>();
            foreach (CreatureGroup cg in StaticData.CreatureGroups) {
                if (m.RacialOpponent is null && cg.RaceSpecific) continue;
                if (m.RacialOpponent is not null && !cg.RaceSpecific) continue;
                if (m.IsShipMission && !cg.FoundInShips) continue;
                if ((m.Type == MissionType.Caves || m.Type == MissionType.Mines) && !cg.FoundInCaves) continue;
                if (!m.IsShipMission && m.Type != MissionType.Caves && m.Type != MissionType.Mines && !cg.FoundIn.Contains(m.Location.Type)) continue; // If a surface mission then check planet type is ok
                                                                                                                                                       // Race relations are between each race and the player team. Why would we ever get a mission with the soldiers of a given race? Only ever in space (e.g. ambush if PT is hated), never from colonies
                if (cg.RaceSpecific && cg.MaxRelations < 5) continue; // A colony would never set up a mission agaisnt its own soldiers.

                // Get a score for this CG to assess how well it fits this role
                double score = 0.0;
                // Get the fraction of members of this group that are in the correct level range, and how close to the middle of the range they are
                int count = 0;
                foreach (CreatureType ct in StaticData.CreatureTypes.Where(x => x.Group == cg)) {
                    count++;
                    if (ct.LevelMin <= m.Diff && ct.LevelMax >= m.Diff) {
                        score += 20.0 - Math.Abs(((ct.LevelMin + ct.LevelMax) / 2) - m.Diff); // How far from average for this range?
                    }
                }
                if (score <= 10.0) continue;
                score /= count;
                score -= cg.FoundIn.Count; // The more location-specific this creature group, the more we want to use it for the locations for which it is suitable
                dScores.Add(cg, score);
                dTotalScore += score;
            }

            // If we don't have any possible creatures then we're in trouble!
            if (dScores.Count == 0) throw new Exception("Could not find any suitable creatures!");

            // Now pick a creature group biased by the scores
            double r = rand.NextDouble() * dTotalScore;
            foreach (CreatureGroup cg in dScores.Keys) {
                if (r < dScores[cg]) return cg;
                r -= dScores[cg];
            }

            return null;
        }
        private static Tuple<MissionGoal, MissionItem?> GetRandomMissionGoal(Mission m, Random rand) {
            MissionGoal mg = MissionGoal.KillAll;
            MissionItem? it = null;

            if (m.IsShipMission) return new Tuple<MissionGoal, MissionItem?>(MissionGoal.KillAll, null); // Shouldn't ever happen...

            int r = rand.Next(100);
            if (r < 30) {
                mg = MissionGoal.KillAll;
            }
            else if (r < 50) {
                if (m.Type == MissionType.Surface) mg = MissionGoal.KillAll;
                else mg = MissionGoal.ExploreAll;
            }
            else if (r < 70) {
                if (m.Diff < 4 || !m.PrimaryEnemy.HasBoss) mg = MissionGoal.KillAll;
                else mg = MissionGoal.KillBoss;
            }
            else if (r < 85 && m.Location.Colony != null) {
                mg = MissionGoal.FindItem;
                it = MissionItem.GenerateRandomGoalItem(m.Diff, rand);
            }
            else {
                mg = MissionGoal.Gather;
                it = MissionItem.GenerateRandomGatherItem(m.Diff, rand);
            }

            return new Tuple<MissionGoal, MissionItem?>(mg, it);
        }
        public bool IsSameMissionAs(Mission m) {
            if (m.Seed != Seed) return false;
            if (m.Type != Type) return false;
            if (m.Goal != Goal) return false;
            // Most other params are generated from the seed anyway, so no point comparing them
            return true;
        }

        // Set mission parameters
        public void SetCreatureTargets() {
            foreach (MissionLevel lev in Levels.Values) {
                lev.SetCreatureTargets();
            }
        }

        public MissionLevel GetOrCreateCurrentLevel() {
            if (Levels.ContainsKey(CurrentLevel)) return Levels[CurrentLevel];
            int diff = Diff + CurrentLevel - (LevelCount / 2);
            MissionLevel lvl = new MissionLevel(this, diff, CurrentLevel);
            Levels.Add(CurrentLevel, lvl);
            return lvl;
        }
        public MissionLevel? GetLevel(int n) {
            if (Levels.ContainsKey(n)) return Levels[n];
            return null;
        }
        public int GetLargestCreatureSize() {
            if (PrimaryEnemy == null) return 1;
            int largest = 1;
            foreach (CreatureType ct in StaticData.CreatureTypes.Where(x => x.Group == PrimaryEnemy)) {
                if (ct.Size > largest) largest = ct.Size;
            }
            return largest;
        }
        public string GetDescription() {
            if (!String.IsNullOrEmpty(FullDescription)) return FullDescription;
            Random rand = new Random(Seed);
            int r = rand.Next(100);
            string strSz = "";
            StringBuilder sb = new StringBuilder(Summary);
            sb.AppendLine();
            if (Type == MissionType.AbandonedCity) {
                string strSource = "Our scans of the planet surface indicate";
                if (Location.Colony != null) {
                    if (r % 3 == 0) strSource = "A group of merchants have informed us about ";
                    if (r % 3 == 1) strSource = "We are receiving classified military reports concerning ";
                }
                if (r < 30) sb.AppendLine(strSource + " some abandoned ruins that show unusual energy fluctuations.");
                else if (r < 60) sb.AppendLine(strSource + " some abandoned ruins that have only recently been discovered.");
                else sb.AppendLine(strSource + " some abandoned ruins that show signs of hostile life within.");
                if (Location.Colony == null) sb.Append("You need to ");
                else sb.Append("We would like you to ");
                if (Goal == MissionGoal.KillAll) sb.AppendLine("investigate the ruins and clear out any opposition you find.");
                if (Goal == MissionGoal.KillBoss) sb.AppendLine("investigate the ruins, take care of the " + PrimaryEnemy!.Boss!.Name + " and get out alive.");
                if (Goal == MissionGoal.ExploreAll) sb.AppendLine("investigate the ruins and map every part of it for the official records.");
                if (Goal == MissionGoal.FindItem) sb.AppendLine("investigate the ruins and search out a " + MItem!.Name + " hidden inside.");
                if (Goal == MissionGoal.Gather) sb.AppendLine("investigate the ruins and gather as many " + MItem!.Name + "s as you can.");
                strSz = "Size : " + Utils.MapSizeToDescription(Size);
            }
            else if (Type == MissionType.BoardingParty) {
                string strSource = "Our long range scanners have detected";
                if (r % 3 == 0) strSource = "A group of merchants have informed us about ";
                if (r % 3 == 1) strSource = "We are receiving classified military reports concerning ";
                if (r < 30) sb.AppendLine(strSource + " an apparently lifeless " + ShipTarget!.Name + "-class ship nearing the colony.");
                else if (r < 60) sb.AppendLine(strSource + " an apparently lifeless " + ShipTarget!.Name + "-class ship with potentially valuable cargo onboard.");
                else sb.AppendLine(strSource + " an apparently lifeless " + ShipTarget!.Name + "-class ship in dangerous proximity to a nearby military zone.");
                sb.AppendLine("We would like you to investigate the ship, clear out any opposition and report back to us.");
                strSz = "Size : " + ShipTarget.Name + "-class";
            }
            else if (Type == MissionType.Caves) {
                string strSource = "Our scans of the planet surface indicate";
                if (Location.Colony != null) {
                    if (r % 3 == 0) strSource = "A group of merchants have informed us about ";
                    if (r % 3 == 1) strSource = "We are receiving classified military reports concerning ";
                }
                if (r < 30) sb.AppendLine(strSource + " a large cave system that might contain important mineral resources.");
                else if (r < 60) sb.AppendLine(strSource + " a large cave system that has only recently been discovered.");
                else sb.AppendLine(strSource + " a large cave system that shows signs of hostile life within.");
                if (Location.Colony == null) sb.Append("You need to ");
                else sb.Append("We would like you to ");
                if (Goal == MissionGoal.KillAll) sb.AppendLine("investigate the cave system and clear out any opposition you find.");
                if (Goal == MissionGoal.KillBoss) sb.AppendLine("investigate the cave system, take care of the " + PrimaryEnemy!.Boss!.Name + " and get out alive.");
                if (Goal == MissionGoal.ExploreAll) sb.AppendLine("investigate the cave system and map every part of it for the official records.");
                if (Goal == MissionGoal.FindItem) sb.AppendLine("investigate the cave system and find a " + MItem!.Name + " hidden inside.");
                if (Goal == MissionGoal.Gather) sb.AppendLine("investigate the cave system and gather as many " + MItem!.Name + "s as you can.");
                strSz = "Size : " + Utils.MapSizeToDescription(Size);
            }
            else if (Type == MissionType.Mines) {
                string strSource = "Our scans of the planet surface indicate";
                if (Location.Colony != null) {
                    if (r % 3 == 0) strSource = "A group of merchants have informed us about ";
                    if (r % 3 == 1) strSource = "We are receiving classified military reports concerning ";
                }
                if (r < 30) sb.AppendLine(strSource + " an abandoned mine system that might contain important mineral resources.");
                else if (r < 60) sb.AppendLine(strSource + " an abandoned mine system that has only recently been discovered.");
                else sb.AppendLine(strSource + " an abandoned mine system that shows signs of hostile life within.");
                if (Location.Colony == null) sb.Append("You need to ");
                else sb.Append("We would like you to ");
                if (Goal == MissionGoal.KillAll) sb.AppendLine("investigate the mines and clear out any opposition you find.");
                if (Goal == MissionGoal.KillBoss) sb.AppendLine("investigate the mines, take care of the " + PrimaryEnemy!.Boss!.Name + " and get out alive.");
                if (Goal == MissionGoal.ExploreAll) sb.AppendLine("investigate the mines and map every part of it for the official records.");
                if (Goal == MissionGoal.FindItem) sb.AppendLine("investigate the mines and find a " + MItem!.Name + " hidden inside.");
                if (Goal == MissionGoal.Gather) sb.AppendLine("investigate the mines and gather as many " + MItem!.Name + "s as you can.");
                strSz = "Size : " + Utils.MapSizeToDescription(Size);
            }
            else if (Type == MissionType.Surface) {
                string strSource = "Our scans of the planet surface indicate";
                if (Location.Colony != null) {
                    if (r % 3 == 0) strSource = "A group of merchants have informed us about ";
                    if (r % 3 == 1) strSource = "We are receiving classified military reports concerning ";
                }
                if (r < 30) sb.AppendLine(strSource + " a region of the planet's surface that shows unusual energy fluctuations.");
                else if (r < 60) sb.AppendLine(strSource + " a region of the planet's surface with interesting geographical reports suggesting mineral wealth.");
                else sb.AppendLine(strSource + " a region of the surface with an accumulation of potentially aggressive life forms.");
                if (Location.Colony == null) sb.Append("You need to ");
                else sb.Append("We would like you to ");
                if (Goal == MissionGoal.KillAll) sb.AppendLine("investigate this area and clear out any opposition you find.");
                if (Goal == MissionGoal.KillBoss) sb.AppendLine("investigate this area, take care of the " + PrimaryEnemy!.Boss!.Name + " and get out alive.");
                if (Goal == MissionGoal.ExploreAll) sb.AppendLine("investigate this area and map every part of it for the official records.");
                if (Goal == MissionGoal.FindItem) sb.AppendLine("investigate this area and find a " + MItem!.Name + " hidden inside.");
                if (Goal == MissionGoal.Gather) sb.AppendLine("investigate this area and gather as many " + MItem!.Name + "s as you can.");
                strSz = "Size : " + Utils.MapSizeToDescription(Size);
            }
            else return sb.ToString(); // Travel mission (i.e. description is irrelevant)
            if (RacialOpponent != null) sb.AppendLine("Primary threat will be " + RacialOpponent.Name + " " + PrimaryEnemy.Name);
            else sb.AppendLine("Primary threat will be " + PrimaryEnemy.Name);
            if (!String.IsNullOrEmpty(strSz)) sb.AppendLine(strSz);
            if (LevelCount > 1) {
                sb.AppendLine("Levels : " + LevelCount);
            }
            FullDescription = sb.ToString();
            return FullDescription;
        }

        // Set up this mission to be run on the tactical view
        public void Initialise() {
            CurrentLevel = 0;
            Levels = new Dictionary<int, MissionLevel>();
        }
        public void SetCurrentLevel(int lvl) {
            CurrentLevel = lvl;
        }
        public void SetCurrentMissionView(MapView mv) {
            CurrentMapView = mv;
        }
        public void ResetMission() {
            // Reset the mission so it can be run again
            CurrentLevel = 0;
            Levels.Clear();
            CurrentMapView = null;
            Soldiers.Clear();
        }
    }
}
