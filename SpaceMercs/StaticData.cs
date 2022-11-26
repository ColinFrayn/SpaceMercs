using System.Collections;
using System.IO;
using System.Reflection;
using System.Xml;

namespace SpaceMercs {
  static class StaticData {
    private static readonly string[] strRNbeg = { "Tar", "Mim", "Cen", "Dar", "Dy", "Ka", "Tha", "Mol", "He", "Ri", "Der", "Har", "Tai", "Ke", "An", "Ak", "Uth", "Tal", "Al", "Ul", "Gel" };
    private static readonly string[] strRNmid = { "", "", "", "'", "-", "k", "ta", "ba", "th", "gr", "li", "mra", "gor", "sh", "ch", "sg", "n'", "to", "ti", "lo", "bu", "bo", "vi", "fi", "sa" };
    private static readonly string[] strRNend = { "on", "un", "en", "in", "an", "ri", "ak", "ahk", "eth", "lin", "lon", "dum", "il", "el", "oth", "ag", "ul", "ish", "uth", "ekh", "esk", "kah" };
    private static readonly string strGraphicsDir, strDataDir;

    // All loaded data : Loads through reflection in this order!
#pragma warning disable 0649
    public static List<Race> Races;
    public static List<MaterialType> Materials;
    public static List<WeaponType> WeaponTypes;
    public static List<ItemType> ItemTypes;
    public static List<ArmourType> ArmourTypes;
    public static List<CreatureType> CreatureTypes;
    public static List<ShipType> ShipTypes;
    public static List<ShipEngine> ShipEngines;
    public static List<ShipEquipment> ShipEquipment;
    public static List<ShipWeapon> ShipWeapons;
    public static List<ShipArmour> ShipArmours;
    public static List<CreatureGroup> CreatureGroups;
#pragma warning restore 0649

    // Static constructor sets up the root directory where all data are to be found
    static StaticData() {
      string strRootDir = Path.GetDirectoryName(Application.ExecutablePath);
      strDataDir = strRootDir + @"\Data\";
      strGraphicsDir = strRootDir + @"\Graphics\";
    }

    // Load up all static data types via reflection
    public static bool LoadAll() {
      // Get a list of all public List<>s in this class
      Type tpThis = MethodBase.GetCurrentMethod().DeclaringType;
      foreach (FieldInfo fi in tpThis.GetFields(BindingFlags.Public | BindingFlags.Static)) {
        if (fi.FieldType.IsGenericType) {
          if (fi.FieldType.GetGenericArguments() != null && fi.FieldType.GetGenericArguments().Length == 1) {
            if (!LoadDataFile(fi)) return false;
          }
        }
      }
      CombineAllShipEquipment();
      if (!LoadCreatureTypes()) return false;

      try {
        Textures.InitialiseTextures(strGraphicsDir);
      }
      catch (Exception ex) {
        MessageBox.Show("Failed to load texture data : " + ex.Message, "Texture error", MessageBoxButtons.OK);
        return false;
      }
      return true;
    }
    private static bool LoadDataFile(FieldInfo fi) {
      Type tp = fi.FieldType.GetGenericArguments()[0];
      if (tp.Name.Equals("CreatureType")) return true; // We load these in a special way later
      Type genericListType = typeof(List<>).MakeGenericType(tp);
      IList newList = (IList)Activator.CreateInstance(genericListType);

      // Load in the static data root file (and potentially recurse through includes)
      string strFile = strDataDir + tp.Name + ".xml";
      XmlDocument xDoc = new XmlDocument();
      try {
        xDoc.Load(strFile);
      }
      catch (Exception ex) {
        MessageBox.Show("Error loading datafile " + strFile + ". Error:" + ex.Message, "Error in Static Data Load", MessageBoxButtons.OK);
        return false;
      }

      if (xDoc.GetElementsByTagName(tp.Name).Count == 0) {
        MessageBox.Show("Could not find any elements labelled " + tp.Name + " in data file " + strFile, "Error in Static Data Load", MessageBoxButtons.OK);
        return false;
      }

      ConstructorInfo ctor = tp.GetConstructor(new[] { typeof(XmlNode) });
      if (ctor == null) {
        MessageBox.Show("Could not find a constructor for type " + tp.Name + " that takes XmlNode", "Error in Static Data Load", MessageBoxButtons.OK);
        return false;
      }

      // Load all elements in this file
      foreach (XmlNode node in xDoc.GetElementsByTagName(tp.Name)) {
        try {
          object instance = ctor.Invoke(new object[] { node });
          newList.Add(instance);
        }
        catch (Exception ex) {
          MessageBox.Show("Error loading " + tp.Name + " data element " + newList.Count.ToString() + ". Error:" + ex.Message, "Error in Static Data Load", MessageBoxButtons.OK);
          return false;
        }
      }

      fi.SetValue(null, newList);
      return true;
    }
    private static bool LoadCreatureTypes() {
      Type tp = typeof(CreatureType);
      CreatureTypes = new List<CreatureType>();

      ConstructorInfo ctor = tp.GetConstructor(new[] { typeof(XmlNode), typeof(CreatureGroup) });
      if (ctor == null) {
        MessageBox.Show("Could not find a constructor for type " + tp.Name + " that takes (XmlNode,string)", "Error in Static Data Load", MessageBoxButtons.OK);
        return false;
      }

      // Load all creature groups in turn
      foreach (CreatureGroup gp in CreatureGroups) {
        if (!LoadCreatureGroup(Path.Combine(strDataDir, "Creatures", gp.Filename), gp)) return false;
      }
      return true;
    }
    private static bool LoadCreatureGroup(string strFile, CreatureGroup gp) {
      Type tp = typeof(CreatureType);
      XmlDocument xDoc = new XmlDocument();
      strFile = Path.ChangeExtension(strFile, ".xml"); // Make sure we have the correct extension
      try {
        xDoc.Load(strFile);
      }
      catch (Exception ex) {
        MessageBox.Show("Error loading datafile " + strFile + ". Error:" + ex.Message, "Error in Static Data Load", MessageBoxButtons.OK);
        return false;
      }
      if (xDoc.GetElementsByTagName(tp.Name).Count == 0) {
        MessageBox.Show("Could not find any elements labelled " + tp.Name + " in data file " + strFile, "Error in Static Data Load", MessageBoxButtons.OK);
        return false;
      }

      int Count = 0;
      foreach (XmlNode node in xDoc.GetElementsByTagName(tp.Name)) {
        try {
          CreatureType ct = new CreatureType(node, gp);
          CreatureTypes.Add(ct);
          gp.CreatureTypes.Add(ct);
        }
        catch (Exception ex) {
          MessageBox.Show("Error loading " + tp.Name + " data element " + Count.ToString() + ". Error:" + ex.Message, "Error in Static Data Load", MessageBoxButtons.OK);
          return false;
        }
        Count++;
      }

      return true;
    }

    // Utility functions
    public static ItemType GetItemTypeByName(string strName) {
      foreach (ItemType it in ItemTypes) {
        if (it.Name.Equals(strName)) return it;
      }
      return null;
    }
    public static ItemType GetItemTypeById(int id) {
      foreach (ItemType it in ItemTypes) {
        if (it.ItemID == id) return it;
      }
      return null;
    }
    public static Race GetRaceByName(string strName) {
      foreach (Race rc in Races) {
        if (rc.Name.Equals(strName)) return rc;
      }
      return null;
    }
    public static ShipType GetShipTypeByName(string strName) {
      foreach (ShipType st in ShipTypes) {
        if (st.Name.Equals(strName)) return st;
        if (st.AKA.Equals(strName)) return st;
      }
      return null;
    }
    public static ShipType GetStarterShip() {
      ShipType stCheapest = null;
      double best = 1000000.0;
      foreach (ShipType st in ShipTypes) {
        if (stCheapest == null || st.Cost < best) {
          stCheapest = st;
          best = st.Cost;
        }
      }
      return stCheapest;
    }
    public static ShipEngine GetShipEngineByName(string strName) {
      foreach (ShipEngine se in ShipEngines) {
        if (se.Name.Equals(strName)) return se;
      }
      return null;
    }
    public static ShipEngine GetMinimumDriveByDistance(double dist) {
      double cheapest = 10000.0;
      ShipEngine seBest = null;
      foreach (ShipEngine se in ShipEngines) {
        if (se.Range >= dist && (seBest == null || se.Cost < cheapest)) {
          cheapest = se.Cost;
          seBest = se;
        }
      }
      return seBest;
    }
    public static ShipWeapon GetShipWeaponByName(string strName) {
      foreach (ShipWeapon sw in ShipWeapons) {
        if (sw.Name.Equals(strName)) return sw;
      }
      return null;
    }
    public static ShipArmour GetShipArmourByName(string strName) {
      foreach (ShipArmour sa in ShipArmours) {
        if (sa.Name.Equals(strName)) return sa;
      }
      return null;
    }
    public static MaterialType GetMaterialTypeByName(string strName) {
      foreach (MaterialType m in Materials) {
        if (m.Name.Equals(strName)) return m;
      }
      return null;
    }
    public static ShipEquipment GetShipEquipmentByName(string strName) {
      foreach (ShipEquipment se in ShipEquipment) {
        if (se.Name.Equals(strName)) return se;
      }
      return null;
    }
    public static WeaponType GetWeaponTypeByName(string strName) {
      foreach (WeaponType wt in WeaponTypes) {
        if (wt.Name.Equals(strName)) return wt;
      }
      return null;
    }
    public static ArmourType GetArmourTypeByName(string strName) {
      foreach (ArmourType at in ArmourTypes) {
        if (at.Name.Equals(strName)) return at;
      }
      return null;
    }
    public static CreatureType GetCreatureTypeByName(string strName) {
      foreach (CreatureType ct in CreatureTypes) {
        if (ct.Name.Equals(strName)) return ct;
      }
      return null;
    }
    public static CreatureGroup GetCreatureGroupByName(string strName) {
      foreach (CreatureGroup cg in CreatureGroups) {
        if (cg.Name.Equals(strName)) return cg;
      }
      return null;
    }

    // Put all equipment in one list
    private static void CombineAllShipEquipment() {
      ShipEquipment.AddRange(ShipEngines);
      ShipEquipment.AddRange(ShipWeapons);
      ShipEquipment.AddRange(ShipArmours);
    }

    // Given the budget and constraints, see if we can (randomly) pick something
    public static ShipEquipment GetRandomShipItemOfMaximumCost(List<ShipEquipment> lItems, ShipEquipment.RoomSize size, Race rc, double dCash, Random rand) {
      ShipEquipment ChosenItem = null;

      foreach (ShipEquipment item in lItems) {
        if (item.Size == size) {
          if (item.RequiredRace == null || item.RequiredRace == rc) {
            if ((rand.NextDouble() + 0.2) * dCash / 1.2 > item.Cost) { // Choose it with a reducing probability as the cost increases
              ChosenItem = item;
            }
          }
        }
      }

      return ChosenItem;
    }

  }
}

