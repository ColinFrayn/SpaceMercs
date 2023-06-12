using System;
using System.Collections.Generic;
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
      Name = xml.Attributes["Name"]?.Value ?? "<No Name>";
      if (xml.SelectSingleNode("Radius") != null) Radius = double.Parse(xml.SelectSingleNode("Radius").InnerText);
      else Radius = 0.0;
      if (xml.SelectSingleNode("Range") != null) Range = double.Parse(xml.SelectSingleNode("Range").InnerText);
      else Range = 1.0;
      SingleUse = (xml.SelectSingleNode("SingleUse") != null);
      XmlNode? xsk = xml.SelectSingleNode("Skill");
      if (xsk is not null) {
        if (xsk.Attributes["Required"] == null) SkillRequired = false;
        else SkillRequired = bool.Parse(xsk.Attributes["Required"].Value);
        AssociatedSkill = (Soldier.UtilitySkill)Enum.Parse(typeof(Soldier.UtilitySkill), xml.SelectSingleNode("Skill").InnerText);
      }
      else {
        AssociatedSkill = Soldier.UtilitySkill.Unspent;
        SkillRequired = false;
      }
      if (xml.SelectSingleNode("Sound") != null) SoundEffect = xml.SelectSingleNode("Sound").InnerText;
      else SoundEffect = "";

      XmlNodeList? xes = xml.SelectNodes("Effect");
      if (xes is null || xes.Count == 0) throw new Exception("No Effect(s) in ItemEffect");
      Effects = new List<Effect>();
      foreach (XmlNode xn in xes) {
        Effect eff = new Effect(xn);
        Effects.Add(eff);
      }
    }
  }
}
