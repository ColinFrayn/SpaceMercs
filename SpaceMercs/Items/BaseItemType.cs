using SpaceMercs.Items;
using System.Xml;

namespace SpaceMercs {
    // A type of item, either equippable or ship equipment
    public class BaseItemType : IResearchable {
        public string Name { get; private set; }
        public double Cost { get; private set; }
        public string Description { get; private set; }
        public int TextureX { get; private set; }
        public int TextureY { get; private set; }
        public Requirements? Requirements { get; private set; }

        public BaseItemType(XmlNode xml) {
            Name = xml.GetAttributeText("Name");
            Cost = xml.SelectNodeDouble("Cost", 0.0);
            Description = xml.SelectNodeText("Desc").Trim();

            // Texture coords (optional)
            TextureX = TextureY = -1;
            string strTex = xml.SelectNodeText("Tex");
            if (!string.IsNullOrEmpty(strTex)) {
                string[] texBits = strTex.Split(',');
                if (texBits.Length != 2) throw new Exception($"Illegal Tex string : {strTex}");
                TextureX = int.Parse(texBits[0]) - 1;
                TextureY = int.Parse(texBits[1]) - 1;
            }

            // TODO DELETE ME TEMP
            if (xml.SelectSingleNode("Race") != null) {
                throw new Exception("Race tag is no longer valid");
            }

            // Do we have any requirements to be able to research this item. (If not then we get it by default and no research required)
            Requirements = null;
            if (xml.SelectSingleNode("Requirements") != null) {
                try {
                    Requirements = new Requirements(xml.SelectSingleNode("Requirements")!);
                }
                catch (Exception ex) {
                    throw new Exception($"Error loading requirements for item {Name} : {ex.Message}");
                }
            }
        }

        public override string ToString() {
            return Name;
        }

        public bool CanBuild(Race? race) {
            if (race is null) return Requirements is null;
            if (Requirements is null) return true;
            return race.HasResearched(this);
        }
    }
}
