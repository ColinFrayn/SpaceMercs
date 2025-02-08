using SpaceMercs.Items;
using System.Xml;

namespace SpaceMercs {
    public class MaterialType : IResearchable {
        public string Name { get; private set; }
        public double MassMod { get; private set; } // Modifier (default = 1.0)
        public double ArmourMod { get; private set; } // Modifier (default = 1.0)
        public double Rarity { get; private set; } // Modifier (default = 1.0)
        public bool IsScavenged { get; private set; } // Can this material only be obtained from dead creatures?
        public string Description { get; private set; }
        public double UnitMass { get; private set; } // Mass of one unit, in kg. This is the amount that is sold (default = 1.0)
        public double UnitCost { get; private set; } // Cost of buying one unit of this item, base, credits
        public Race? RequiredRace { get; private set; }
        public Requirements? Requirements { get; private set; }
        public bool IsArmourMaterial { get { return (ArmourMod > 0.0); } }
        public IReadOnlyDictionary<Soldier.UtilitySkill, int> SkillBoosts { get; private set; }
        public readonly Dictionary<WeaponType.DamageType, double> BonusArmour = new Dictionary<WeaponType.DamageType, double>();
        public double ConstructionChanceModifier { get { return -((Requirements?.MinLevel ?? 0d) * 2d) - (10d * (ArmourMod - 1d)) - (15d * (1d - MassMod)) - (Math.Sqrt(UnitCost) * 2d) + (IsScavenged ? -2d : 3d); } }
        public int GetUtilitySkill(Soldier.UtilitySkill sk) {
            if (SkillBoosts.ContainsKey(sk)) return SkillBoosts[sk];
            return 0;
        }

        public MaterialType(XmlNode xml) {
            Name = xml.GetAttributeText("Name");
            UnitCost = xml.SelectNodeDouble("UnitCost", 1.0);
            MassMod = xml.SelectNodeDouble("MassMod", 1.0);
            Rarity = xml.SelectNodeDouble("Rarity", 1.0);
            ArmourMod = xml.SelectNodeDouble("ArmourMod", 0.0);
            UnitMass = xml.SelectNodeDouble("UnitMass", 1.0);
            Description = xml.SelectNodeText("Desc").Trim();
            IsScavenged = xml.SelectSingleNode("Scavenged") != null;

            // Special resistances if this is made into armour
            foreach (XmlNode xn in xml.SelectNodesToList("BonusArmour/Bonus")) {
                WeaponType.DamageType type = (WeaponType.DamageType)Enum.Parse(typeof(WeaponType.DamageType), xn.GetAttributeText("Type"));
                double val = xn.GetAttributeDouble("Amount");
                BonusArmour.Add(type, val);
            }

            // Load the race that this equipment is restricted to (default null), or otherwise fail
            if (xml.SelectSingleNode("Race") != null) {
                RequiredRace = StaticData.GetRaceByName(xml.SelectNodeText("Race"));
                if (RequiredRace == null) {
                    throw new Exception("Could not find restricted race \"" + xml.SelectNodeText("Race") + "\" for equipment " + Name);
                }
            }

            // Load in skill boosts
            Dictionary<Soldier.UtilitySkill, int> skillBoostsTemp = new();
            XmlNode? nUtility = xml.SelectSingleNode("Utility");
            if (nUtility is not null) {
                foreach (XmlNode xn in nUtility.ChildNodes) {
                    string strSkill = xn.Name;
                    Soldier.UtilitySkill sk = (Soldier.UtilitySkill)Enum.Parse(typeof(Soldier.UtilitySkill), strSkill);
                    int iVal = int.Parse(xn.InnerText);
                    if (skillBoostsTemp.ContainsKey(sk)) throw new Exception("Duplicate Utility Boost in material type " + Name);
                    skillBoostsTemp.Add(sk, iVal);
                }
            }
            SkillBoosts = new Dictionary<Soldier.UtilitySkill, int>(skillBoostsTemp);
            Requirements = null;
        }

        public void LoadRequirements(XmlNode xml) {
            // Do we have any requirements to be able to research this material type. (If not then we get it by default and no research required)
            if (xml.SelectSingleNode("Requirements") != null) {
                try {
                    Requirements = new Requirements(xml.SelectSingleNode("Requirements")!);
                }
                catch (Exception ex) {
                    throw new Exception($"Error loading requirements for material type {Name} : {ex.Message}");
                }
            }
        }

        public bool CanBuild(Race? race) {
            if (race is null) return RequiredRace is null && Requirements is null;
            if (RequiredRace != null && RequiredRace != race) return false;
            if (Requirements is null) return true;
            return race.HasResearched(this);
        }

    }
}
