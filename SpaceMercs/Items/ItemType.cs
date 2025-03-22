using System.Xml;

namespace SpaceMercs {
    // A type of soldier item
    public class ItemType : BaseItemType {
        public enum ItemSource { None = 0, Workshop = 1, Medlab = 2, Armoury = 3 }

        public double Mass { get; private set; }
        public Dictionary<MaterialType, int> Materials { get; private set; }
        public double Rarity { get; private set; }
        public int GetUtilitySkill(Soldier.UtilitySkill sk) {
            if (_SkillBoosts.ContainsKey(sk)) return _SkillBoosts[sk];
            return 0;
        }
        public ItemEffect? ItemEffect { get; private set; }
        public double StaminaCost { get; private set; }
        public uint ItemID { get; private set; }
        public ItemSource Source { get; private set; }
        private Dictionary<Soldier.UtilitySkill, int> _SkillBoosts { get; set; } = new Dictionary<Soldier.UtilitySkill, int>();
        public IReadOnlyDictionary<Soldier.UtilitySkill, int> SkillBoosts { get { return _SkillBoosts; } }
        private static uint NextID = Const.ItemIDBase;
        public double ConstructionChance { get { return 90d - ((Requirements?.MinLevel ?? 0d) * 7d) - Math.Sqrt(Cost); } }

        public ItemType(XmlNode xml) : base(xml) {
            Mass = xml.SelectNodeDouble("Mass", 0.0);
            StaminaCost = xml.SelectNodeDouble("StaminaCost", Const.UseItemCost);

            // Rarity calculations
            Rarity = 10d;
            if (Requirements is not null) {
                Rarity /= Requirements.MinLevel;
                int totalRel = Requirements.RequiredRaceRelations.Sum(x => x.Value + 1);
                Rarity *= Math.Pow(0.75, totalRel);
            }
            Rarity *= Math.Pow(0.8, Mass/10d);

            Materials = new Dictionary<MaterialType, int>();
            if (xml.SelectSingleNode("Materials") is not null) {
                foreach (XmlNode xn in xml.SelectNodesToList("Materials/Material")) {
                    string strMat = xn.GetAttributeText("Name");
                    int iVal = xn.GetAttributeInt("Amount");
                    MaterialType? m = StaticData.GetMaterialTypeByName(strMat);
                    if (m is null) throw new Exception("Could not identify material " + strMat + " required for item " + Name);
                    if (Materials.ContainsKey(m)) throw new Exception("Duplicate material in item description : " + Name);
                    Materials.Add(m, iVal);
                }
            }
            XmlNode? nUtility = xml.SelectSingleNode("Utility");
            if (nUtility is not null) {
                foreach (XmlNode xn in nUtility.ChildNodes) {
                    string strSkill = xn.Name;
                    Soldier.UtilitySkill sk = (Soldier.UtilitySkill)Enum.Parse(typeof(Soldier.UtilitySkill), strSkill);
                    int iVal = int.Parse(xn.InnerText);
                    if (_SkillBoosts.ContainsKey(sk)) throw new Exception("Duplicate Utility Boost in item " + Name);
                    _SkillBoosts.Add(sk, iVal);
                }
            }

            XmlNode? xie = xml.SelectSingleNode("ItemEffect");
            if (xie is not null) {
                ItemEffect = new ItemEffect(xie);
            }

            // Source (where can it be made?)
            Source = xml.SelectNodeEnum<ItemSource>("Source", ItemSource.None);

            // Update ItemID
            ItemID = NextID;
            NextID++;
        }

        public override string ToString() {
            return Name;
        }
    }
}
