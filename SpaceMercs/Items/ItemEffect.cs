using System.Xml;

namespace SpaceMercs {
    class ItemEffect {
        public string Name { get; private set; }
        public double Radius { get; private set; }
        public double Range { get; private set; } // Default to one (touch)
        public bool SingleUse { get; private set; } // Destroyed after use?
        public Soldier.UtilitySkill AssociatedSkill { get; private set; }
        public bool SkillRequired { get; private set; }  // Is the AssociatedSkill required to be >0?
        public List<Effect> Effects { get; private set; }
        public string SoundEffect { get; private set; }

        public ItemEffect(XmlNode xml) {
            Name = xml.Attributes?["Name"]?.Value ?? "<No Name>";

            string strRadius = xml.SelectSingleNode("Radius")?.InnerText ?? string.Empty;
            if (!string.IsNullOrEmpty(strRadius)) Radius = double.Parse(strRadius);
            else Radius = 0.0;

            string strRange = xml.SelectSingleNode("Range")?.InnerText ?? string.Empty;
            if (!string.IsNullOrEmpty(strRange)) Range = double.Parse(strRange);
            else Range = 1.0;

            SingleUse = (xml.SelectSingleNode("SingleUse") is not null);
            XmlNode? xsk = xml.SelectSingleNode("Skill");
            if (xsk is not null) {
                if (xsk.Attributes?["Required"]?.Value is null) SkillRequired = false;
                else SkillRequired = bool.Parse(xsk.Attributes!["Required"]!.Value);
                AssociatedSkill = (Soldier.UtilitySkill)Enum.Parse(typeof(Soldier.UtilitySkill), xml.SelectSingleNode("Skill")?.InnerText ?? string.Empty);
            }
            else {
                AssociatedSkill = Soldier.UtilitySkill.Unspent;
                SkillRequired = false;
            }
            SoundEffect = xml.SelectSingleNode("Sound")?.InnerText ?? string.Empty;

            Effects = new List<Effect>();
            foreach (XmlNode xn in xml.SelectNodesToList("Effect")) {
                Effect eff = new Effect(xn);
                Effects.Add(eff);
            }
            if (!Effects.Any()) throw new Exception("No Effect(s) in ItemEffect");
        }
    }
}
