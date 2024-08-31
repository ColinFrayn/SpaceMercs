using System.Xml;
using static SpaceMercs.ShipEquipment;

namespace SpaceMercs.Items {
    public class Requirements {
        private readonly int MinLevel;
        private readonly int MinSystems;
        private readonly int MinPop;
        private readonly int CashCost;
        private readonly IDictionary<Race, int> RequiredRaceRelations = new Dictionary<Race, int>();
        private readonly IDictionary<MaterialType, int> RequiredMaterials = new Dictionary<MaterialType, int>();
        private readonly ICollection<RoomAbilities> RequiredFacilities = new HashSet<RoomAbilities>();

        public Requirements(XmlNode xml) {
            MinLevel = xml.SelectNodeInt("MinLevel", 1);
            MinSystems = xml.SelectNodeInt("MinSystems", 1);
            MinPop = xml.SelectNodeInt("MinPop", 1);
            CashCost = xml.SelectNodeInt("CashCost", 0);

            // This item is restricted unless the player race has relations this good or better with the specified race(s)
            foreach (XmlNode xn in xml.SelectNodesToList("Relations")) {
                string strName = xn.Attributes!["Name"]?.InnerText ?? "Missing";
                Race? requiredRace = StaticData.GetRaceByName(strName);
                if (requiredRace == null) {
                    throw new Exception($"Could not find required race \"{strName}\"");
                }
                int iVal = int.Parse(xn.InnerText);
                RequiredRaceRelations.Add(requiredRace, iVal);
            }

            // This item is restricted unless the player owns the following materials
            foreach (XmlNode xn in xml.SelectNodesToList("Material")) {
                string strName = xn.Attributes!["Name"]?.InnerText ?? "Missing";
                MaterialType? requiredMaterial = StaticData.GetMaterialTypeByName(strName);
                if (requiredMaterial == null) {
                    throw new Exception($"Could not find required material \"{strName}\"");
                }
                int iVal = int.Parse(xn.InnerText);
                RequiredMaterials.Add(requiredMaterial, iVal);
            }

            // Room facilities required to research this
            foreach (XmlNode xn in xml.SelectNodesToList("Facility")) {
                RoomAbilities ab = (RoomAbilities)Enum.Parse(typeof(RoomAbilities), xn.InnerText);
                if (RequiredFacilities.Contains(ab)) throw new Exception("Duplicate room facility requirement");
                RequiredFacilities.Add(ab);
            }
        }

        // Any race : Have they expanded enough to research this?
        public bool MeetsRequirements(Race race) {
            if (race.SystemCount < MinSystems) return false;
            if (race.Population < MinPop) return false;

            // Race relations: Player race passes automatically, because we test this properly elsewhere
            if (race == StaticData.Races[0]) return true;

            // If this isn't the player race then only pass if the requirements don't include any race other than this one.
            return RequiredRaceRelations.Count == 0 ||
                (RequiredRaceRelations.Count == 1 && RequiredRaceRelations.ContainsKey(race));
        }

        // The player team & player race : Can they actually initiate research?
        public bool MeetsRequirements(Team team) {
            if (team.Cash < CashCost) return false;
            foreach (Race rc in RequiredRaceRelations.Keys) {
                if (team.GetRelations(rc) < RequiredRaceRelations[rc]) return false;
            }
            if (team.MaximumSoldierLevel < MinLevel) return false;
            foreach (RoomAbilities ra in RequiredFacilities) {
                if (!team.PlayerShip.HasFacility(ra)) return false;
            }
            foreach (KeyValuePair<MaterialType, int> kvp in RequiredMaterials) {
                if (team.CountMaterial(kvp.Key) < kvp.Value) return false;
            }
            return MeetsRequirements(StaticData.Races[0]);
        }

        // The player team & player race : Meets requirements for viewing this tech, but not necessarily researching it
        // i.e. ignoring Cash and Material requirements
        public bool MeetsBasicRequirements(Team team) {
            foreach (Race rc in RequiredRaceRelations.Keys) {
                if (!rc.Known) return false;
                if (team.GetRelations(rc) < RequiredRaceRelations[rc]) return false;
            }
            if (team.MaximumSoldierLevel < MinLevel) return false;
            foreach (RoomAbilities ra in RequiredFacilities) {
                if (!team.PlayerShip.HasFacility(ra)) return false;
            }
            return MeetsRequirements(StaticData.Races[0]);
        }

        // How difficult is this (makes it harder for alien races to research it)
        // 1.0 is base difficulty. Higher number is harder.
        public double Difficulty {
            get {
                double diff = 0.9 + (MinLevel * MinLevel / 200d) + (MinSystems / 20d) + (MinPop / 100d);
                diff += RequiredRaceRelations.Count / 10.0;
                diff += RequiredMaterials.Count / 20.0;
                return diff;
            }
        }
    }
}
