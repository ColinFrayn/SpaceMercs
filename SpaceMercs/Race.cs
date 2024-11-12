using System.Xml;
using System.IO;
using SpaceMercs.Items;

namespace SpaceMercs {
    public class Race {
        public string Name { get; set; }
        public double Scale { get; set; }
        public int Strength { get; private set; }
        public int Insight { get; private set; }
        public int Toughness { get; private set; }
        public int Agility { get; private set; }
        public int Endurance { get; private set; }
        public int Aggression { get; private set; }
        public int BaseAttitude { get; private set; }
        public double BaseTemp;
        public string Description { get; private set; }
        public Planet.PlanetType PlanetType { get; private set; } // Preferred planet type
        public Planet HomePlanet { get; private set; }
        public bool Known { get; private set; } // Have we met them yet?
        public DateTime LastExpandCheck { get; private set; }
        public readonly List<Star> Systems = new List<Star>();
        private readonly List<Colony> Colonies = new List<Colony>();
        private readonly Dictionary<GenderType, List<string>> FirstNames = new Dictionary<GenderType, List<string>>();
        private readonly List<string> FamilyNames;
        public Color Colour { get; private set; }
        public int ColonyCount { get { return Colonies.Count; } }
        public int SystemCount { get { return Systems.Count; } }
        public int Population { get { return Colonies.Select(x => x.BaseSize).Sum(); } }
        public bool IsPlayer { get { return HomePlanet.GetSystem().Sector.SectorX == 0 && HomePlanet.GetSystem().Sector.SectorY == 0; } }
        private readonly HashSet<IResearchable> ResearchedItems = new HashSet<IResearchable>();

        public Race(XmlNode xml) {
            Name = xml.SelectNodeText("Name");
            Scale = xml.SelectNodeDouble("Scale");
            Insight = xml.SelectNodeInt("Insight"); 
            Strength = xml.SelectNodeInt("Strength");
            Toughness = xml.SelectNodeInt("Toughness");
            Agility = xml.SelectNodeInt("Agility");
            Endurance = xml.SelectNodeInt("Endurance");
            Aggression = xml.SelectNodeInt("Aggression", 0);
            Description = xml.SelectNodeText("Description");
            BaseTemp = xml.SelectNodeInt("BaseTemp");
            BaseAttitude = xml.SelectNodeInt("BaseAttitude", 0);
            PlanetType = xml.SelectNodeEnum<Planet.PlanetType>("PlanetType");
            string strCol = xml.SelectNodeText("Colour");
            string[] bits = strCol.Split(',');
            Colour = Color.FromArgb(255, int.Parse(bits[0]), int.Parse(bits[1]), int.Parse(bits[2]));

            XmlNode nPersonal = xml.SelectSingleNode("Names/Personal") ?? throw new Exception($"Could not find personal names list for Race {Name}");
            foreach (XmlNode xn in nPersonal.ChildNodes) {
                List<string> lNames = xn.InnerText.Split(',').ToList<string>();
                if (lNames.Count == 0) throw new Exception("No names defined for Gender=" + xn.Name + ", Race=" + Name);
                GenderType gt = (GenderType)Enum.Parse(typeof(GenderType), xn.Name);
                if (FirstNames.ContainsKey(gt)) throw new Exception("Duplicate gender key " + xn.Name + " for race names for " + Name + " race");
                FirstNames.Add(gt, lNames);
            }
            FamilyNames = xml.SelectNodeText("Names/Family").Split(',').ToList<string>();
            if (FamilyNames.Count == 0) throw new Exception("No family names defined for race " + Name);
            if (FirstNames.Keys.Count == 0) throw new Exception("No possible genders defined for race " + Name + ". Please add some names!");
            Reset();
        }

        public void SaveAdditionalData(StreamWriter file) {
            file.WriteLine("<Race Name=\"" + Name + "\">");
            file.WriteLine("<HomePlanet>" + HomePlanet.PrintCoordinates() + "</HomePlanet>");
            file.WriteLine("<LastExpand>" + LastExpandCheck.ToBinary() + "</LastExpand>");
            if (Known) file.WriteLine("<Known/>");
            file.WriteLine("<Research>");
            foreach (IResearchable item in ResearchedItems) {
                if (item is BaseItemType it) file.WriteLine($" <Item>{it.Name}</Item>");
                else if (item is MaterialType mat) file.WriteLine($" <Material>{mat.Name}</Material>");
                else throw new Exception($"Unknown IResearchable type : {item.GetType()}");
            }
            file.WriteLine("</Research>");
            file.WriteLine("</Race>");
        }
        public void LoadAdditionalData(XmlNode xml, Map map) {
            AstronomicalObject? aoHome = map.GetAOFromLocationString(xml.SelectNodeText("HomePlanet"));
            if (aoHome is not Planet pl) throw new Exception("Home Planet corrupted in data file (not a planet!)");
            HomePlanet = pl;
            Known = (xml.SelectSingleNode("Known") is not null);

            string strLastExpand = xml.SelectNodeText("LastExpand");
            if (!string.IsNullOrEmpty(strLastExpand)) LastExpandCheck = DateTime.FromBinary(long.Parse(strLastExpand));
            else LastExpandCheck = Const.dtStart;

            XmlNodeList? xns = xml.SelectNodes("Research/Item");
            if (xns is not null) {
                foreach (XmlNode xn in xns) {
                    string strItem = xn.InnerText;
                    if (string.IsNullOrEmpty(strItem)) throw new Exception($"Null item discovered in Race Research list for Race {Name}");
                    BaseItemType? item = StaticData.GetBaseItemByName(strItem);
                    if (item == null) throw new Exception($"Unknown item {strItem} discovered in Race Research list for Race {Name}");
                    if (!ResearchedItems.Add(item)) throw new Exception($"Repeated item {strItem} discovered in Race Research list for Race {Name}");
                }
            }
            XmlNodeList? xmats = xml.SelectNodes("Research/Material");
            if (xmats is not null) {
                foreach (XmlNode xn in xmats) {
                    string strMat = xn.InnerText;
                    if (string.IsNullOrEmpty(strMat)) throw new Exception($"Null material type discovered in Race Research list for Race {Name}");
                    MaterialType? mat = StaticData.GetMaterialTypeByName(strMat);
                    if (mat == null) throw new Exception($"Unknown material type {strMat} discovered in Race Research list for Race {Name}");
                    if (!ResearchedItems.Add(mat)) throw new Exception($"Repeated material type {strMat} discovered in Race Research list for Race {Name}");
                }
            }
        }

        public void SetHomePlanet(Planet pl) {
            HomePlanet = pl;
            pl.GetSystem().SetName(Name + " Home");
            pl.SetupBase(this, 5);
        }

        public void Colonise(Star st) {
            st.SetOwner(this);
            AddSystem(st);
            if (Const.DEBUG_VIEW_ALL_CIVS) st.SetVisited(true);
        }

        public void AddSystem(Star st) {
            if (!Systems.Contains(st)) Systems.Add(st);
            if (st.Owner == null) st.SetOwner(this);
        }
        public void AddColony(Colony cl) {
            if (!Colonies.Contains(cl)) Colonies.Add(cl);
            AddSystem(cl.Location.GetSystem());
        }
        internal void CheckGrowthForAllColonies() {
            // Take a copy as we may modify the original
            List<Colony> backup = new List<Colony>(Colonies);
            foreach (Colony cl in backup) {
                cl.CheckGrowth();
            }
        }
        internal void CheckColonySeeds(GUIMessageBox msgBox, TimeSpan tDiff) {
            // Take a copy as we may modify the original
            List<Colony> backup = new List<Colony>(Colonies);
            foreach (Colony cl in backup) {
                cl.UpdateSeedProgress(msgBox, tDiff);
            }
        }
        internal void CheckForNewColonySystems(GUIMessageBox msgBox) {
            HashSet<Star> nearestUncolonisedStars = new HashSet<Star>();
            double daysSinceLast = (Const.dtTime - LastExpandCheck).TotalDays;
            Star stHome = HomePlanet.GetSystem();
            Sector scHome = stHome.Sector;

            foreach (Star st in Systems) {
                // Get stars nearby this colonised system, and add them to a "maybe colonise" list
                List<Star> systemsInOrderOfDistance = scHome.GetStarsInDistanceOrderFrom(st);
                systemsInOrderOfDistance.Remove(st);
                // If any nearby systems are uncolonised then add them to the list
                int count = 0;
                while (systemsInOrderOfDistance.Any()) { 
                    Star? nearest = systemsInOrderOfDistance.FirstOrDefault();
                    if (nearest is null) break;
                    if ((nearest.Owner is null || nearest.Owner == this) && nearest.GetPopulation() == 0) {
                        nearestUncolonisedStars.Add(nearest);
                        count++;
                        if (count >= 2) break;
                    }
                    systemsInOrderOfDistance.Remove(nearest);
                }
            }

            // Check if any new system has been colonised in this period
            if (nearestUncolonisedStars.Count == 0) return;
            Random rand = new Random();
            bool isPlayer = scHome.SectorX == 0 && scHome.SectorY == 0;
            Star candidate = nearestUncolonisedStars.ElementAt(rand.Next(nearestUncolonisedStars.Count)); // Get a star to check
            do {
                double span = 200.0 + rand.NextDouble() * 100;
                if (daysSinceLast < span) return;
                daysSinceLast -= span;
                LastExpandCheck = LastExpandCheck.AddDays(span);
                if (rand.NextDouble() > 0.9 && candidate.AddPopulationInSystem(this, rand)) {
                    Colonise(candidate);
                    if (isPlayer) {
                        candidate.SetVisited(true);
                        msgBox.PopupMessage($"The {Name} Race has colonised a new system at {candidate.PrintCoordinates()}");
                    }
                    return;
                }
            } while (daysSinceLast > 0);
        }
        internal void CheckResearch(TimeSpan tDiff, int maxLevel) {
            if (IsPlayer) return;

            double nDays = tDiff.TotalSeconds / Const.SecondsPerDay;
            Random rand = new Random();

            // Are there any items that this race can research?
            foreach (BaseItemType it in StaticData.ResearchableBaseItems) {
                if (HasResearched(it)) continue;
                if (maxLevel < (it.Requirements?.MinLevel ?? 0)) continue; // Gate it by player level too so we don't end up with enemy races way more advanced
                if (it.Requirements?.MeetsRequirements(this) == true) {
                    double diff = it.Requirements.Difficulty;
                    double prob = Math.Pow(1.0 - Const.DailyResearchProb, nDays / diff); // Chance of *failure*
                    if (rand.NextDouble() > prob) {
                        ResearchedItems.Add(it);
                    }
                }
            }
            // Are there any material types that this race can research?
            foreach (MaterialType mat in StaticData.ResearchableMaterialTypes) {
                if (HasResearched(mat)) continue;
                if (maxLevel < (mat.Requirements?.MinLevel ?? 0)) continue; // Gate it by player level too so we don't end up with enemy races way more advanced
                if (mat.Requirements?.MeetsRequirements(this) == true) {
                    double diff = mat.Requirements.Difficulty;
                    double prob = Math.Pow(1.0 - Const.DailyResearchProb, nDays / diff); // Chance of *failure*
                    if (rand.NextDouble() > prob) {
                        ResearchedItems.Add(mat);
                    }
                }
            }
        }

        public void Reset() {
            HomePlanet = Planet.Empty;
            Known = false;
            Systems.Clear();
            Colonies.Clear();
            ResearchedItems.Clear();
        }

        public GenderType GenerateRandomGender(Random rand) {
            int total = 0;
            foreach (GenderType gt in FirstNames.Keys) {
                total += FirstNames[gt].Count;
            }
            int r = rand.Next(total);
            foreach (GenderType gt in FirstNames.Keys) {
                r -= FirstNames[gt].Count;
                if (r < 0) return gt;
            }
            throw new Exception("Error in Race.GenerateRandomGender!");
        }
        public string GenerateRandomName(Random rand, GenderType gt) {
            if (!FirstNames.ContainsKey(gt)) throw new Exception("Attempting to generate name for soldier of impossible gender for this race (" + Name + ", " + gt + ")");
            int r = rand.Next(FirstNames[gt].Count);
            string strName = (FirstNames[gt])[r] + " ";
            r = rand.Next(FamilyNames.Count);
            strName += FamilyNames[r];
            return strName;
        }

        public void SetAsKnownBy(Team? pt) {
            Known = true;
            pt?.SetBaselineRelations(this);
        }

        public string RelationsToString(Team t) {
            if (!Known) return "No Contact";
            return Utils.RelationsToString(t.GetRelations(this));
        }

        public Star GetNearestSystemTo(Star st) {
            if (Systems.Contains(st)) return st;
            double closest = 100000.0;
            Star stBest = st;
            foreach (Star st2 in Systems) {
                if (stBest == st || st.DistanceTo(st2) < closest) {
                    closest = st.DistanceTo(st2);
                    stBest = st2;
                }
            }
            return stBest;
        }
        public Star GetNearestSystemToNotIncludingSelf(Star st) {
            double closest = 100000.0;
            Star stBest = st;
            foreach (Star st2 in Systems) {
                if (st2 == st) continue;
                if (stBest == st || st.DistanceTo(st2) < closest) {
                    closest = st.DistanceTo(st2);
                    stBest = st2;
                }
            }
            return stBest;
        }

        public bool HasResearched(IResearchable tp) => ResearchedItems.Contains(tp);
        public void CompleteResearch(IResearchable item) {
            if (HasResearched(item)) throw new Exception($"Researching already researched item {item.Name}!");
            ResearchedItems.Add(item);
        }
    }
}
