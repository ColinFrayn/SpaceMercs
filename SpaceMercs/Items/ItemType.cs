using System;
using System.Collections.Generic;
using System.Xml;

namespace SpaceMercs {
  // A type of soldier item
  class ItemType {
    public enum ItemSource { None = 0, Workshop = 1, Medlab = 2, Armoury = 3 }

    public string Name { get; private set; }
    public double Mass { get; private set; }
    public double Cost { get; private set; }
    public string Desc { get; private set; }
    public Dictionary<MaterialType,int> Materials { get; private set; }
    public int BaseRarity { get; private set; }
    public double Rarity { get; private set; }
    public int GetUtilitySkill(Soldier.UtilitySkill sk) {
      if (_SkillBoosts.ContainsKey(sk)) return _SkillBoosts[sk];
      return 0;
    }
    public ItemEffect ItemEffect { get; private set; }
    public int TextureX { get; private set; }
    public int TextureY { get; private set; }
    public uint ItemID { get; private set; }
    public ItemSource Source { get; private set; }
    public Dictionary<Soldier.UtilitySkill, int> _SkillBoosts { get; private set; } = new Dictionary<Soldier.UtilitySkill, int>();
    public IReadOnlyDictionary<Soldier.UtilitySkill, int> SkillBoosts {  get { return _SkillBoosts; } }
    private static uint NextID = Const.ItemIDBase;
    public double ConstructionChance { get { return 120.0 - (BaseRarity * 5.0); } }

    public ItemType(XmlNode xml) {
      Name = xml.Attributes["Name"].InnerText;
      if (xml.SelectSingleNode("Cost") != null) {
        Cost = Double.Parse(xml.SelectSingleNode("Cost").InnerText);
      }
      else Cost = 0.0;
      if (xml.SelectSingleNode("Rarity") != null) {
        BaseRarity = int.Parse(xml.SelectSingleNode("Rarity").InnerText);
        Rarity = (100.0 / ((Math.Pow(BaseRarity,1.5)) + 1.0));
      }
      else Rarity = 0.0;
      if (xml.SelectSingleNode("Mass") != null) Mass = double.Parse(xml.SelectSingleNode("Mass").InnerText);
      else Mass = 0.0;
      if (xml.SelectSingleNode("Desc") != null) Desc = xml.SelectSingleNode("Desc").InnerText;
      else Desc = "";
      Desc.Trim();
      Materials = new Dictionary<MaterialType, int>();
      if (xml.SelectSingleNode("Materials") != null) {
        foreach (XmlNode xn in xml.SelectNodes("Materials/Material")) {
          string strMat = xn.Attributes["Name"].Value;
          int iVal = int.Parse(xn.Attributes["Amount"].Value);
          MaterialType m = StaticData.GetMaterialTypeByName(strMat);
          if (m == null) throw new Exception("Could not identify material " + strMat + " required for item " + Name);
          if (Materials.ContainsKey(m)) throw new Exception("Duplicate material in item description : " + Name);
          Materials.Add(m, iVal);
        }
      }
      if (xml.SelectSingleNode("Utility") != null) {
        foreach (XmlNode xn in xml.SelectSingleNode("Utility").ChildNodes) {
          string strSkill = xn.Name;
          Soldier.UtilitySkill sk = (Soldier.UtilitySkill)Enum.Parse(typeof(Soldier.UtilitySkill), strSkill);
          int iVal = int.Parse(xn.InnerText);
          if (_SkillBoosts.ContainsKey(sk)) throw new Exception("Duplicate Utility Boost in item " + Name);
          _SkillBoosts.Add(sk, iVal);
        }
      }

      XmlNode xie = xml.SelectSingleNode("ItemEffect");
      if (xie != null) {
        ItemEffect = new ItemEffect(xie);
      }

      // Source (where can it be made?)
      XmlNode xs = xml.SelectSingleNode("Source");
      if (xs != null) {
        Source = (ItemSource)Enum.Parse(typeof(ItemSource), xs.InnerText);
      }
      else Source = ItemSource.None;

      // Texture coords (optional)
      TextureX = TextureY = -1;
      if (xml.SelectSingleNode("Tex") != null) {
        string strTex = xml.SelectSingleNode("Tex").InnerText;
        string[] TexBits = strTex.Split(',');
        TextureX = int.Parse(TexBits[0]) - 1;
        TextureY = int.Parse(TexBits[1]) - 1;
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
