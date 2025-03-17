using System.Xml;

namespace SpaceMercs {
    public class ItemEffect {
        public string Name { get; private set; }
        public double Radius { get; private set; } // Default to zero (single target)
        public double Range { get; private set; } // Default to one (touch)
        public bool SingleUse { get; private set; } // Destroyed after use?
        public Soldier.UtilitySkill AssociatedSkill { get; private set; }
        public bool SkillRequired { get; private set; }  // Is the AssociatedSkill required to be >0?
        public List<Effect> Effects { get; private set; }
        public string SoundEffect { get; private set; }
        public int Recharge { get; private set; } // Number of turns to recharge (2 = usable every other round, 1 = one use per round, 0 = unlimited uses)
        public bool CurePoison { get; private set; }
        public int NoiseLevel { get; private set; } // Noise level is the number of squares away this effect can be heard
        public bool Scan { get; private set; }
        public bool Instant { get; private set; }

        // Delegate
        public delegate void ApplyItemEffect(IEntity? source, ItemEffect ie, int px, int py);

        public ItemEffect(XmlNode xml) {
            Name = xml.Attributes?["Name"]?.Value ?? "<No Name>";

            Radius = xml.SelectNodeDouble("Radius", 0.0);
            Range = xml.SelectNodeDouble("Range", 1.0);
            Recharge = xml.SelectNodeInt("Recharge", 0);
            SingleUse = xml.SelectSingleNode("SingleUse") != null;
            NoiseLevel = xml.GetAttributeInt("Noise", 0);
            CurePoison = xml.SelectSingleNode("CurePoison") != null;

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

            Scan = xml.SelectSingleNode("Scan") != null;
            Instant = xml.SelectSingleNode("Instant") != null;

            if (!Scan && !Effects.Any()) throw new Exception("No Effect(s) in ItemEffect");
        }
    }
}
