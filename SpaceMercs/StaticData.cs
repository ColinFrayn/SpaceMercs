using System.Collections;
using System.IO;
using System.Reflection;
using System.Xml;

namespace SpaceMercs {
    public static class StaticData {
        private static readonly string strBitmapsDir, strDataDir, strRootDir;
        public static Race HumanRace { get; private set; }
        public static string GraphicsLocation => strRootDir + @"\Graphics\";

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
        public static List<WeaponMod> WeaponMods;
#pragma warning restore 0649

        // Static constructor sets up the root directory where all data are to be found
        static StaticData() {
            strRootDir = Path.GetDirectoryName(Application.ExecutablePath) ?? string.Empty;
            strDataDir = strRootDir + @"\Data\";
            strBitmapsDir = strRootDir + @"\Graphics\Bitmaps\";
        }

        // Load up all static data types via reflection
        public static bool LoadAll() {
            // Get a list of all public List<>s in this class
            Type tpThis = MethodBase.GetCurrentMethod()?.DeclaringType ?? throw new Exception("Could not get declaring type");
            foreach (FieldInfo fi in tpThis.GetFields(BindingFlags.Public | BindingFlags.Static)) {
                if (fi.FieldType.IsGenericType) {
                    if (fi.FieldType.GetGenericArguments() != null && fi.FieldType.GetGenericArguments().Length == 1) {
                        if (!LoadDataFile(fi)) return false;
                    }
                }
            }
            CombineAllShipEquipment();
            if (!LoadCreatureTypes()) return false;
            if (!LoadMaterialRequirements()) return false; // Have to reload the file after the first pass because requirements might refer to other materials.

            try {
                Textures.LoadTextureFiles(strBitmapsDir);
            }
            catch (Exception ex) {
                MessageBox.Show("Failed to load texture data : " + ex.Message, "Texture error", MessageBoxButtons.OK);
                return false;
            }

            Race? human = GetRaceByName("Human");
            if (human is null) {
                MessageBox.Show("Failed to ID HumanRace in static data", "Static Data Initialisation error", MessageBoxButtons.OK);
                return false;
            }
            HumanRace = human;

            return true;
        }
        private static bool LoadDataFile(FieldInfo fi) {
            Type tp = fi.FieldType.GetGenericArguments()[0];
            if (tp.Name.Equals("CreatureType")) return true; // We load these in a special way later
            Type genericListType = typeof(List<>).MakeGenericType(tp);
            IList newList = Activator.CreateInstance(genericListType) as IList ?? throw new Exception("Could not create generic list");

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

            ConstructorInfo? ctor = tp.GetConstructor(new[] { typeof(XmlNode) });
            if (ctor is null) {
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

            ConstructorInfo? ctor = tp.GetConstructor(new[] { typeof(XmlNode), typeof(CreatureGroup) });
            if (ctor is null) {
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
                    if (ct.IsBoss) gp.AddBoss(ct);
                }
                catch (Exception ex) {
                    MessageBox.Show("Error loading " + tp.Name + " data element " + Count.ToString() + ". Error:" + ex.Message, "Error in Static Data Load", MessageBoxButtons.OK);
                    return false;
                }
                Count++;
            }

            // Attempt to load texture file, if it exists
            string strCreatureTextureFile = Path.Combine(strBitmapsDir, $"{gp.Filename.Replace(".xml",".bmp")}");
            if (File.Exists(strCreatureTextureFile)) {
                Bitmap TextureBitmap = new Bitmap(strCreatureTextureFile);
                gp.SetTextureBitmap(TextureBitmap);                
            }

            return true;
        }

        // Utility functions
        public static ItemType? GetItemTypeByName(string strName) {
            foreach (ItemType it in ItemTypes) {
                if (it.Name.Equals(strName)) return it;
            }
            return null;
        }
        public static ItemType? GetItemTypeById(int id) {
            foreach (ItemType it in ItemTypes) {
                if (it.ItemID == id) return it;
            }
            return null;
        }
        public static Race? GetRaceByName(string strName) {
            foreach (Race rc in Races) {
                if (rc.Name.Equals(strName)) return rc;
            }
            return null;
        }
        public static ShipType? GetShipTypeByName(string strName) {
            foreach (ShipType st in ShipTypes) {
                if (st.Name.Equals(strName)) return st;
                if (st.AKA.Equals(strName)) return st;
            }
            return null;
        }
        public static ShipType? GetStarterShip() {
            ShipType? stCheapest = null;
            double best = 1000000.0;
            foreach (ShipType st in ShipTypes) {
                if (stCheapest == null || st.Cost < best) {
                    stCheapest = st;
                    best = st.Cost;
                }
            }
            return stCheapest;
        }
        public static ShipEngine? GetShipEngineByName(string strName) {
            foreach (ShipEngine se in ShipEngines) {
                if (se.Name.Equals(strName)) return se;
            }
            return null;
        }
        public static ShipEngine? GetMinimumDriveByDistance(double dist) {
            double cheapest = 10000.0;
            ShipEngine? seBest = null;
            foreach (ShipEngine se in ShipEngines) {
                if (se.Range >= dist && (seBest == null || se.Cost < cheapest)) {
                    cheapest = se.Cost;
                    seBest = se;
                }
            }
            return seBest;
        }
        public static ShipWeapon? GetShipWeaponByName(string strName) {
            foreach (ShipWeapon sw in ShipWeapons) {
                if (sw.Name.Equals(strName)) return sw;
            }
            return null;
        }
        public static ShipArmour? GetShipArmourByName(string strName) {
            foreach (ShipArmour sa in ShipArmours) {
                if (sa.Name.Equals(strName)) return sa;
            }
            return null;
        }
        public static MaterialType? GetMaterialTypeByName(string? strName) {
            if (string.IsNullOrEmpty(strName)) return null;
            foreach (MaterialType m in Materials) {
                if (m.Name.Equals(strName)) return m;
            }
            return null;
        }
        public static ShipEquipment? GetShipEquipmentByName(string strName) {
            foreach (ShipEquipment se in ShipEquipment) {
                if (se.Name.Equals(strName)) return se;
            }
            return null;
        }
        public static WeaponType? GetWeaponTypeByName(string strName) {
            foreach (WeaponType wt in WeaponTypes) {
                if (wt.Name.Equals(strName)) return wt;
            }
            return null;
        }
        public static ArmourType? GetArmourTypeByName(string strName) {
            if (strName == "Full Armour") strName = "Core Armour"; // Backwards compatibility
            foreach (ArmourType at in ArmourTypes) {
                if (at.Name.Equals(strName)) return at;
            }
            return null;
        }
        public static CreatureType? GetCreatureTypeByName(string strName) {
            foreach (CreatureType ct in CreatureTypes) {
                if (ct.Name.Equals(strName)) return ct;
            }
            return null;
        }
        public static CreatureGroup? GetCreatureGroupByName(string strName) {
            foreach (CreatureGroup cg in CreatureGroups) {
                if (cg.Name.Equals(strName)) return cg;
            }
            return null;
        }
        public static WeaponMod? GetWeaponModByName(string strName) {
            foreach (WeaponMod wm in WeaponMods) {
                if (wm.Name.Equals(strName)) return wm;
            }
            return null;
        }
        public static BaseItemType? GetBaseItemByName(string strName) {
            if (GetItemTypeByName(strName) is BaseItemType it) return it;
            if (GetWeaponTypeByName(strName) is BaseItemType wp) return wp;
            if (GetArmourTypeByName(strName) is BaseItemType ar) return ar;
            if (GetShipEquipmentByName(strName) is BaseItemType se) return se;
            return null;
        }

        // Put all equipment in one list
        private static void CombineAllShipEquipment() {
            ShipEquipment.AddRange(ShipEngines);
            ShipEquipment.AddRange(ShipWeapons);
            ShipEquipment.AddRange(ShipArmours);
        }

        // Once we've loaded all material types we have to load the file again to pull in requirements.
        // This is because some requirements refer to materialTypes.
        private static bool LoadMaterialRequirements() {
            string strFile = strDataDir + "MaterialType.xml";
            XmlDocument xDoc = new XmlDocument();
            try {
                xDoc.Load(strFile);
            }
            catch (Exception ex) {
                MessageBox.Show("Error loading datafile " + strFile + ". Error:" + ex.Message, "Error in MaterialType Re-Load", MessageBoxButtons.OK);
                return false;
            }

            // Load all elements in this file
            foreach (XmlNode node in xDoc.GetElementsByTagName("MaterialType")) {
                try {
                    // Get the material type
                    var name = node!.Attributes!["Name"]!.InnerText;
                    // Load the requirements, if any
                    var mat = GetMaterialTypeByName(name);
                    mat!.LoadRequirements(node);
                }
                catch (Exception ex) {
                    MessageBox.Show($"Error loading requirements for MaterialType {node.Name}. Error:" + ex.Message, "Error in Static Data Load", MessageBoxButtons.OK);
                    return false;
                }
            }
            return true;
        }

        public static IEnumerable<BaseItemType> AllBaseItems {
            get {
                foreach (ItemType it in ItemTypes) {
                    yield return it;
                }
                foreach (WeaponType wp in WeaponTypes) {
                    yield return wp;
                }
                foreach (ArmourType ar in ArmourTypes) {
                    yield return ar;
                }
                foreach (ShipEquipment se in ShipEquipment) {
                    yield return se;
                }
            }
        }
        public static IEnumerable<BaseItemType> ResearchableBaseItems {
            get {
                return AllBaseItems.Where(b => b.Requirements is not null);
            }
        }
        public static IEnumerable<MaterialType> ResearchableMaterialTypes {
            get {
                return Materials.Where(b => b.Requirements is not null);
            }
        }

        // Given the budget and constraints, see if we can (randomly) pick something
        public static ShipEquipment? GetRandomShipItemOfMaximumCost(List<ShipEquipment> lItems, ShipEquipment.RoomSize size, double dCash, Random rand) {
            ShipEquipment? ChosenItem = null;

            foreach (ShipEquipment item in lItems) {
                if (item.Size == size) {
                    if ((rand.NextDouble() + 0.2) * dCash / 1.2 > item.Cost) { // Choose it with a reducing probability as the cost increases
                        ChosenItem = item;
                    }
                }
            }

            return ChosenItem;
        }

    }
}

