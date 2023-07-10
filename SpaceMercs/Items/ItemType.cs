using System.Xml;

namespace SpaceMercs {
    // A type of soldier item
    class ItemType {
        public enum ItemSource { None = 0, Workshop = 1, Medlab = 2, Armoury = 3 }

        public string Name { get; private set; }
        public double Mass { get; private set; }
        public double Cost { get; private set; }
        public string Desc { get; private set; }
        public Dictionary<MaterialType, int> Materials { get; private set; }
        public int BaseRarity { get; private set; }
        public double Rarity { get; private set; }
        public int GetUtilitySkill(Soldier.UtilitySkill sk) {
            if (_SkillBoosts.ContainsKey(sk)) return _SkillBoosts[sk];
            return 0;
        }
        public ItemEffect? ItemEffect { get; private set; }
        public int TextureX { get; private set; }
        public int TextureY { get; private set; }
        public uint ItemID { get; private set; }
        public ItemSource Source { get; private set; }
        public Dictionary<Soldier.UtilitySkill, int> _SkillBoosts { get; private set; } = new Dictionary<Soldier.UtilitySkill, int>();
        public IReadOnlyDictionary<Soldier.UtilitySkill, int> SkillBoosts { get { return _SkillBoosts; } }
        private static uint NextID = Const.ItemIDBase;
        public double ConstructionChance { get { return 120.0 - (BaseRarity * 5.0); } }

        public ItemType(XmlNode xml) {
            Name = xml.GetAttributeText("Name");
            Cost = xml.SelectNodeDouble("Cost", 0.0);
            BaseRarity = xml.SelectNodeInt("Rarity", 0);
            Rarity = (100.0 / ((Math.Pow(BaseRarity, 1.5)) + 1.0));
            Mass = xml.SelectNodeDouble ("Mass", 0.0);
            Desc = xml.SelectNodeText("Desc").Trim();

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

            // Texture coords (optional)
            TextureX = TextureY = -1;
            string strTex = xml.SelectNodeText("Tex");
            if (!string.IsNullOrEmpty(strTex)) {                
                string[] texBits = strTex.Split(',');
                if (texBits.Length != 2) throw new Exception($"Illegal Tex string : {strTex}");
                TextureX = int.Parse(texBits[0]) - 1;
                TextureY = int.Parse(texBits[1]) - 1;
            }

            // Update ItemID
            ItemID = NextID;
            NextID++;
        }

        public override string ToString() {
            return Name;
        }
    }
}
