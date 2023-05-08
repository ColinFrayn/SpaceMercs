using System;
using System.Collections.Generic;
using System.Xml;
using OpenTK.Mathematics;
using System.Drawing;
using System.Linq;

namespace SpaceMercs {
  static class Utils {
    public enum Direction { West, East, North, South, NorthWest, NorthEast, SouthWest, SouthEast };
    private static readonly Random rnd = new Random();
    public static string RangeToString(double range) {
      string strRange = "";
      double rly = range / Const.LightYear;
      if (rly >= 0.01) strRange = (Math.Round(rly, 2)).ToString() + " Light Years";
      else if (range >= Const.Billion) strRange = (Math.Round(range / Const.Billion, 2)).ToString() + " Gm";
      else if (range >= Const.Million) strRange = (Math.Round(range / Const.Million, 2)).ToString() + " Mm";
      else strRange = (Math.Round(range / 1000.0, 2)).ToString() + " km";
      return strRange;
    }
    public static string TimeToString(double t, bool bDisplayAsClock) {
      if (!bDisplayAsClock) {
        if (t < 10.0) return (Math.Round(t, 1)).ToString() + " sec";
        else if (t < 1000.0) return ((int)t).ToString() + " sec";
        else if (t < 10000.0) return ((int)Math.Round(t / 1000.0, 1)).ToString() + " ksec";
        else if (t < Const.Million) return ((int)(t / 1000.0)).ToString() + " ksec";
        else if (t < 10.0 * Const.Million) return ((int)Math.Round(t / Const.Million, 1)).ToString() + " Msec";
        else if (t < Const.Billion) return ((int)(t / Const.Million)).ToString() + " Msec";
        else return ((int)Math.Round(t / Const.Billion, 1)).ToString() + " Gsec";
      }
      int iClock1 = (int)(t / 10000000.0) % 100;
      int iClock2 = (int)(t / 100000.0) % 100;
      int iClock3 = (int)(t / 1000.0) % 100;
      int iClock4 = (int)(t / 100.0) % 10;
      string strClock = "";
      bool bStarted = false;
      if (iClock1 > 0 || bDisplayAsClock || bStarted) {
        if (iClock1 < 10) strClock += "0";
        strClock += iClock1.ToString() + ":";
        bStarted = true;
      }
      if (iClock2 > 0 || bDisplayAsClock || bStarted) {
        if (iClock2 < 10) strClock += "0";
        strClock += iClock2.ToString() + ":";
        bStarted = true;
      }
      if (bDisplayAsClock || bStarted) {
        if (iClock3 < 10) strClock += "0";
      }
      strClock += iClock3.ToString() + "." + iClock4.ToString();
      return strClock;
    }
    public static Matrix4 GeneratePickMatrix(float x, float y, float width, float height, int[] viewport) {
      Matrix4 result = Matrix4.Identity;

      float translateX = (viewport[2] - (2.0f * (x - viewport[0]))) / width;
      float translateY = (viewport[3] - (2.0f * (y - viewport[1]))) / height;
      result = Matrix4.Mult(Matrix4.CreateTranslation(translateX, translateY, 0.0f), result);

      float scaleX = viewport[2] / width;
      float scaleY = viewport[3] / height;
      result = Matrix4.Mult(Matrix4.CreateScale(scaleX, scaleY, 1.0f), result);

      return result;
    }
    public static double NextGaussian(Random rnd, double mean, double stdDev) {
      double u1 = rnd.NextDouble();
      double u2 = rnd.NextDouble();
      double randStdNormal = Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Sin(2.0 * Math.PI * u2);
      return mean + stdDev * randStdNormal;
    }
    public static Vector3d GetRandomSphericalDisplacement(double rmin, double rmax) {
      double x, y, z, phi, theta, radius;
      phi = Math.Acos((2.0 * rnd.NextDouble()) - 1.0);
      theta = rnd.NextDouble() * Math.PI * 2.0;
      radius = Math.Sqrt((rnd.NextDouble() * (rmax * rmax - rmin * rmin)) + (rmin * rmin));  // Radius between rmin and rmax
      x = radius * Math.Sin(phi) * Math.Sin(theta);
      y = radius * Math.Sin(phi) * Math.Cos(theta);
      z = radius * Math.Cos(phi);
      return new Vector3d(x, y, z);
    }
    public static string RelationsToString(int r) {
      if (r == -5) return "Despised";
      if (r == -4) return "Hated";
      if (r == -3) return "Hostile";
      if (r == -2) return "Suspicious";
      if (r == -1) return "Unfriendly";
      if (r ==  0) return "Neutral";
      if (r ==  1) return "Friendly";
      if (r ==  2) return "Allied";
      if (r ==  3) return "Revered";
      if (r ==  4) return "Heroic";
      if (r ==  5) return "Exalted";
      return "ILLEGAL_RELATION_VALUE : " + r;
    }
    public static double RelationsToCostMod(int r) {
      if (r==0) return 1.0;
      if (r > 0) return 1.0 - (double)((r + 2) * r) / 100.0;
      if (r == -1) return 1.1;
      if (r == -2) return 1.5;
      if (r == -3) return 3.0;
      return 100.0;
    }
    public static string PrintDistance(double dist) {
      if (dist > Const.LightYear / 20.0) {
        return Math.Round(dist / Const.LightYear, 2).ToString() + "ly";
      }
      if (dist > Const.AU * 1000.0) {
        return Math.Round(dist / (Const.AU*1000.0), 2).ToString() + "kAU";
      }
      if (dist > Const.AU / 10.0) {
        return Math.Round(dist / Const.AU, 2).ToString() + "AU";
      }
      if (dist > 1E8) {
        return Math.Round(dist / Const.Billion, 2).ToString() + "Gm";
      }
      if (dist > 1E5) {
        return Math.Round(dist / Const.Million, 2).ToString() + "Mm";
      }
      return Math.Round(dist / 1000.0, 2).ToString() + "km";
    }
    public static double CalculateMass(Dictionary<IItem, int> dEquip) {
      double Mass = 0.0;
      foreach (KeyValuePair<IItem, int> kvp in dEquip) {
        Mass += kvp.Key.Mass * kvp.Value; 
      }
      return Mass;
    }
    public static string LevelToDescription(int lvl) {
      if (lvl == 0) return "Basic";
      if (lvl == 1) return "Fine";
      if (lvl == 2) return "Good";
      if (lvl == 3) return "Superb";
      if (lvl == 4) return "Epic";
      if (lvl == 5) return "Legendary";
      throw new Exception("Unexpected equipment level : " + lvl);
    }
    public static string MapSizeToDescription(int sz) {
      if (sz < 1) throw new Exception("Mission Size < 1");
      if (sz == 1) return "Tiny";
      if (sz == 2) return "Small";
      if (sz == 3) return "Medium";
      if (sz == 4) return "Large";
      if (sz == 5) return "Huge";
      if (sz == 6) return "Enormous";
      return "Gargantuan";
    }
    public static Color LevelToColour(int lvl) {
      if (lvl == 0) return Color.FromArgb(255, 255, 255, 255);
      if (lvl == 1) return Color.FromArgb(255, 255, 255, 0); 
      if (lvl == 2) return Color.FromArgb(255, 50, 255, 100); 
      if (lvl == 3) return Color.FromArgb(255, 50, 100, 255);
      if (lvl == 4) return Color.FromArgb(255, 170, 45,  255);
      if (lvl == 5) return Color.FromArgb(255, 255, 100, 70);
      throw new Exception("Unexpected equipment level : " + lvl);
    }
    public static Dictionary<IItem, int> DismantleEquipment(IEquippable eq, int lvl) {
      Dictionary<IItem, int> dRemains = new Dictionary<IItem, int>();
      Random rand = new Random();
      double diff = ((double)eq.BaseType.BaseRarity / 2.0) + (double)eq.Level;
      double fract = (double)lvl / diff;
      foreach (MaterialType mat in eq.BaseType.Materials.Keys) {
        int iQuantity = eq.BaseType.Materials[mat];
        int iRecovered = (int)((rand.NextDouble() * fract) * (double)(iQuantity + 1.0));
        if (iRecovered > iQuantity) iRecovered = iQuantity;
        if (iRecovered > 0) dRemains.Add(new Material(mat), iRecovered);
      }
      
      return dRemains;
    }
    public static int ExperienceToSkillLevel(int xp) {
      double d = xp / Const.WeaponSkillBase;
      // Inverse triangular numbers ;)
      return (int)Math.Floor((Math.Sqrt(1 + d * 8) - 1) / 2);
    }
    public static int SkillLevelToExperience(int lvl) {
      return lvl * (lvl + 1) * Const.WeaponSkillBase / 2;
    }
    public static double ArmourReduction(double ar) {
      return Math.Pow(0.5, ar / Const.ArmourScale);
    }
    public static float DirectionToAngle(Direction d) {
      switch (d) {
        case Direction.East: return 0.0f;
        case Direction.NorthEast: return 45.0f;
        case Direction.North: return 90.0f;
        case Direction.NorthWest: return 135.0f;
        case Direction.West: return 180.0f;
        case Direction.SouthWest: return -135.0f;
        case Direction.South: return -90.0f;
        case Direction.SouthEast: return -45.0f;
      }
      throw new NotImplementedException();
    }
    public static Direction AngleToDirection(double ang) {
      if (ang < 0) ang += 360.0 ;
      ang = ang % 360.0;
      if (ang < 22.5) return Direction.East;
      if (ang < 67.5) return Direction.NorthEast;
      if (ang < 112.5) return Direction.North;
      if (ang < 157.5) return Direction.NorthWest;
      if (ang < 202.5) return Direction.West;
      if (ang < 247.5) return Direction.SouthWest;
      if (ang < 292.5) return Direction.South;
      if (ang < 337.5) return Direction.SouthEast;
      return Direction.East;
    }
    public static IItem? LoadItem(XmlNode xml) {
      if (xml == null) return null;
      if (xml.Name.Equals("Armour")) return new Armour(xml);
      if (xml.Name.Equals("Weapon")) return new Weapon(xml);
      if (xml.Name.Equals("Equipment")) return new Equipment(xml);
      if (xml.Name.Equals("Corpse")) return new Corpse(xml);
      if (xml.Name.Equals("Material")) return new Material(xml);
      if (xml.Name.Equals("MissionItem")) return new MissionItem(xml);
      throw new Exception("Attempting to load IItem of unknown type : " + xml.Name);
    }
    public static string RunLengthEncode(string str) {
      return str;
    }
    public static string RunLengthDecode(string str) {
      return str;
    }
    public static double GenerateHitRoll(IEntity from, IEntity to) {
      double att = from.Attack;
      double def = to.Defence;
      double dist = from.RangeTo(to);
      double size = to.Size;
      double dropoff = (from.EquippedWeapon == null) ? 0.0 : from.EquippedWeapon.Type.DropOff;
      double hit = Const.HitBias + ((rnd.NextDouble() - 0.5) * Const.HitScale) + ((att - def) * Const.GuaranteedHitScale) + (rnd.NextDouble() * att * Const.AttackScale) - (rnd.NextDouble() * def * Const.DefenceScale) + Math.Pow(size, Const.HitSizePowerLaw);
      double dropoffmod = (dist * dropoff);
      if (from is Soldier s) {
        hit += Const.SoldierHitBias;
        int sniper = s.GetUtilityLevel(Soldier.UtilitySkill.Sniper);
        dropoffmod *= 1.0 - ((double)sniper / 10.0);
        if (dropoffmod < 0.0) dropoffmod = 0.0;
      }
      if (dropoff > 0.0) hit -= dropoffmod; // Harder to hit at long range. 0.0 = melee weapon.
      return hit;
    }
    public static int GenerateDroppedItemLevel(int lev, bool boss) {
      int ilev = (int)((rnd.NextDouble() * lev) / 4.0);
      if (boss) {
        int ilev2 = (int)((rnd.NextDouble() * lev) / 4.0);
        if (ilev2 > ilev) ilev = ilev2;
      }
      return ilev;
    }
    public static double BodyPartToArmourScale(BodyPart bp) {
      switch (bp) {
        case BodyPart.Chest: return 3.0;
        case BodyPart.Legs: return 2.0;
        case BodyPart.Arms: return 2.0;
        case BodyPart.Feet: return 1.0;
        case BodyPart.Hands: return 1.0;
        case BodyPart.Head: return 1.0;
      }
      throw new Exception("Unhandled body part in BodyPartToArmourScale : " + bp);
    }
    public static BodyPart GetRandomBodyPart() {
      double tot = 0.0;
      foreach (BodyPart bp in Enum.GetValues(typeof(BodyPart))) {
        tot += BodyPartToArmourScale(bp);
      }
      double r = rnd.NextDouble() * tot;
      foreach (BodyPart bp in Enum.GetValues(typeof(BodyPart))) {
        if (r <= BodyPartToArmourScale(bp)) return bp;
        r -= BodyPartToArmourScale(bp);
      }
      throw new Exception("Could not get random body part");
    }
    public static double HitToMod(double hit) {
      if (hit < 0.0) return 0.0;
      if (hit < 5.0) {
        return 0.5 + (hit / 10.0);
      }
      return 1.0 + Math.Sqrt((hit - 5.0) / 20.0);  // (hit,hmod) = (0.0,0.5),(5.0,1.0),(25.0,2.0),(85.0,3.0),(185.0,4.0)

    }
    public static int StaminaCostOfUtilitySkill(Soldier.UtilitySkill sk) {
      switch (sk) {
        case Soldier.UtilitySkill.Medic: return 10;
        //case Soldier.UtilitySkill.Stealth: return 12;
      }
      return 0;
    }
    public static Dictionary<WeaponType.DamageType, double> CombineDamage(Dictionary<WeaponType.DamageType, double> d1, Dictionary<WeaponType.DamageType, double> d2) {
      if (d1 == null) return d2;
      if (d2 == null) return d1;
      Dictionary<WeaponType.DamageType, double> dict = new Dictionary<WeaponType.DamageType, double>(d1);
      foreach (WeaponType.DamageType dt in d2.Keys) {
        if (dict.ContainsKey(dt)) dict[dt] += d2[dt];
        else dict.Add(dt, d2[dt]);
      }
      return dict;
    }
    public static IItem GenerateRandomItem(Random rnd, int lvl, bool bAddEquipment = true) {
      // Armour
      double rnum = rnd.NextDouble();
      if (!bAddEquipment) rnum = 1.0;
      if (rnum < 0.2) { // Random weapon
        return GenerateRandomWeapon(rnd, lvl);
      }
      if (rnum < 0.7) { // Random armour
        return GenerateRandomArmour(rnd, lvl);
      }
      if (rnum < 0.8) { // Random material
        double best = 0.0;
        MaterialType mbest = null;
        foreach (MaterialType mat in StaticData.Materials) {
          double r = rnd.NextDouble() * Math.Pow(mat.Rarity, 5.0 / (lvl + 4.0));
          if (r > best) {
            best = r;
            mbest = mat;
          }
        }
        if (mbest == null) return null;
        return new Material(mbest);
      }
      else { // Random item
        double best = 0.0;
        ItemType ibest = null;
        foreach (ItemType it in StaticData.ItemTypes) {
          if (it.BaseRarity > lvl + 5) continue;
          double r = rnd.NextDouble() * Math.Pow(it.Rarity, 5.0 / (lvl + 4.0));
          if (r > best) {
            best = r;
            ibest = it;
          }
        }
        if (ibest == null) return null;
        return new Equipment(ibest);
      }
    }
    public static Weapon GenerateRandomWeapon(Random rnd, int Level) {
      List<WeaponType> wts = new List<WeaponType>();
      double trar = 0.0;
      foreach (WeaponType tp in StaticData.WeaponTypes) {
        if (tp.BaseRarity <= Level + 5 && tp.IsUsable) {
          wts.Add(tp);
          trar += tp.Rarity;
        }
      }
      if (wts.Any()) {
        double rar = rnd.NextDouble() * trar;
        int pos = 0;
        while (pos < wts.Count && rar > wts[pos].Rarity) {
          if (pos == wts.Count - 1) break;
          rar -= wts[pos].Rarity;
          pos++;
        }
        WeaponType tp = wts[pos];
        if (tp != null) {
          int lvl = 0;
          if (Level > 1) {
            while (lvl < 4 && rnd.NextDouble() < 0.5 && tp.BaseRarity + (lvl * 4) <= (Level + 1)) lvl++;
          }
          return new Weapon(tp, lvl);
        }
      }
      return null;
    }
    public static Armour GenerateRandomArmour(Random rnd, int Level) {
      // Choose between all single-location armour pieces
      double best = 0.0;
      ArmourType abest = null;
      MaterialType mbest = null;
      foreach (ArmourType at in StaticData.ArmourTypes) {
        if (at.Locations.Count == 1) {
          foreach (MaterialType mat in StaticData.Materials) {
            if (!mat.IsArmourMaterial) continue;
            double r = rnd.NextDouble() * at.Rarity * mat.Rarity;
            if (r > best) {
              best = r;
              abest = at;
              mbest = mat;
            }
          }
        }
      }
      if (abest == null || mbest == null) return null;
      Armour ar = new Armour(abest, mbest, 0);

      // Scale to level
      for (int n=0; n<Level/2; n++) {
        if (rnd.NextDouble() < 0.6) ar.UpgradeArmour();
      }
      if (Level % 1 == 1 && rnd.NextDouble() < 0.3) ar.UpgradeArmour();

      return ar;
    }
    public static bool IsPassable(MissionLevel.TileType tp) {
      switch (tp) {
        case MissionLevel.TileType.Floor: return true;
        case MissionLevel.TileType.OpenDoorHorizontal: return true;
        case MissionLevel.TileType.OpenDoorVertical: return true;
      }
      return false;
    }
    public static string MissionGoalToString(Mission.MissionGoal mg) {
      switch (mg) {
        case Mission.MissionGoal.ExploreAll: return "Explore";
        case Mission.MissionGoal.KillAll: return "Kill All";
        case Mission.MissionGoal.KillBoss: return "Assassination";
        case Mission.MissionGoal.Gather: return "Gathering";
        case Mission.MissionGoal.FindItem: return "Treasure Hunt";
      }
      return "Unknown";
    }
    public static double ItemLevelToCostMod(int lev) {
      return Math.Pow(lev + 1, Const.EquipmentLevelCostExponent) * Math.Pow(Const.EquipmentLevelCostBaseExponent, lev);
    }
    public static double RoundSF(double d, int digits) {
      if (d == 0) return 0;
      decimal scale = (decimal)Math.Pow(10, Math.Floor(Math.Log10(Math.Abs(d))) + 1);
      return (double)(scale * Math.Round((decimal)d / scale, digits));
    }
  }
}
