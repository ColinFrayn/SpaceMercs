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

            Radius = xml.SelectNodeDouble("Radius", 0.0);
            Range = xml.SelectNodeDouble("Range", 1.0);

            SingleUse = (xml.SelectSingleNode("SingleUse") is not null);
            XmlNode? xsk = xml.SelectSingleNode("Skill");
            if (xsk is not null) {
                if (xsk.Attributes?["Required"]?.Value is null) SkillRequired = false;
                else SkillRequired = bool.Parse(xsk.Attributes!["Required"]!.Value);
                AssociatedSkill = xml.SelectNodeEnum<Soldier.UtilitySkill>("Skill");
            }
            else {
                AssociatedSkill = Soldier.UtilitySkill.Unspent;
                SkillRequired = false;
            }
            SoundEffect = xml.SelectNodeText("Sound");

            Effects = new List<Effect>();
            foreach (XmlNode xn in xml.SelectNodesToList("Effect")) {
                Effect eff = new Effect(xn);
                Effects.Add(eff);
            }
            if (!Effects.Any()) throw new Exception("No Effect(s) in ItemEffect");
        }
    }
}
