using SpaceMercs.Graphics;
using SpaceMercs.Items;
using System.Xml;
using static SpaceMercs.Planet;

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
        public int MaxLevel { get; private set; } // maximum level of armour possible with this material
        public Requirements? Requirements { get; private set; }
        public bool IsArmourMaterial { get { return (ArmourMod > 0.0); } }
        public IReadOnlyDictionary<Soldier.UtilitySkill, int> SkillBoosts { get; private set; }
        public readonly IDictionary<WeaponType.DamageType, double> BonusArmour = new Dictionary<WeaponType.DamageType, double>();
        public double ConstructionDiffModifier { get { return (MaxLevel - 1) + (IsScavenged ? 0.5d : ((Requirements?.MinLevel ?? 0d) * 0.15d)); } }
        public int GetUtilitySkill(Soldier.UtilitySkill sk) {
            if (SkillBoosts.ContainsKey(sk)) return SkillBoosts[sk];
            return 0;
        }
        public int NodeMin { get; private set; } // Minimum level of resource node
        public int NodeMax { get; private set; } // Maximum level of resource node
        public readonly ICollection<PlanetType> PlanetTypes = new HashSet<PlanetType>();

        // Texture stuff for resource nodes
        public int TextureID { get; private set; }
        public int TexX { get; private set; }
        public int TexY { get; private set; }

        public MaterialType(XmlNode xml) {
            Name = xml.GetAttributeText("Name");
            UnitCost = xml.SelectNodeDouble("UnitCost", 1.0);
            MassMod = xml.SelectNodeDouble("MassMod", 1.0);
            Rarity = xml.SelectNodeDouble("Rarity", 1.0);
            ArmourMod = xml.SelectNodeDouble("ArmourMod", 0.0);
            UnitMass = xml.SelectNodeDouble("UnitMass", 1.0);
            Description = xml.SelectNodeText("Desc").Trim();
            IsScavenged = xml.SelectSingleNode("Scavenged") != null;
            MaxLevel = xml.SelectNodeInt("MaxLevel", 5);

            // Special resistances if this is made into armour
            foreach (XmlNode xn in xml.SelectNodesToList("BonusArmour/Bonus")) {
                WeaponType.DamageType type = (WeaponType.DamageType)Enum.Parse(typeof(WeaponType.DamageType), xn.GetAttributeText("Type"));
                double val = xn.GetAttributeDouble("Amount");
                BonusArmour.Add(type, val);
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

            // Check for possibility of this material being in a resource node
            XmlNode? nRes = xml.SelectSingleNode("Node");
            TexX = TexY = -1;
            if (nRes is not null) {
                NodeMin = nRes.GetAttributeInt("LevelMin", 0);
                NodeMax = nRes.GetAttributeInt("LevelMax", 999);
                string strTypes = nRes.GetAttributeText("Type", string.Empty);
                string[] types = strTypes.Split(',');
                foreach (string type in types) {
                    PlanetType? ptp = Enum.Parse<Planet.PlanetType>(type);
                    if (ptp == null) {
                        throw new Exception($"Could not identify required planet type \"{type}\"");
                    }
                    PlanetTypes.Add(ptp.Value);
                }
                TexX = nRes.GetAttributeInt("TexX") - 1;
                TexY = nRes.GetAttributeInt("TexY") - 1;
            }
            else {
                NodeMin = NodeMax = 0;
            }

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
            if (race is null) return Requirements is null;
            if (Requirements is null) return true;
            return race.HasResearched(this);
        }

        public TexSpecs GetTexture() {
            return Textures.GetTexCoords(this);
        }
    }
}
