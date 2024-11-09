using OpenTK.Mathematics;
using System.Xml;
using static SpaceMercs.Soldier;

namespace SpaceMercs {
    public static class Utils {
        public enum Direction { West, East, North, South, NorthWest, NorthEast, SouthWest, SouthEast };
        private static readonly Random rnd = new Random();

        public static string RangeToString(double range) {
            double rly = range / Const.LightYear;
            if (rly >= 0.01) return (Math.Round(rly, 2)).ToString() + " Light Years";
            else if (range >= Const.Billion) return (Math.Round(range / Const.Billion, 2)).ToString() + " Gm";
            else if (range >= Const.Million) return (Math.Round(range / Const.Million, 2)).ToString() + " Mm";
            return (Math.Round(range / 1000.0, 2)).ToString() + " km";
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

        public static string PrintDistance(double dist) {
            if (dist > Const.LightYear / 20.0) {
                return Math.Round(dist / Const.LightYear, 2).ToString() + "ly";
            }
            if (dist > Const.AU * 1000.0) {
                return Math.Round(dist / (Const.AU * 1000.0), 2).ToString() + "kAU";
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

        public static string MapSizeToDescription(int sz) {
            return sz switch {
                1 => "Tiny",
                2 => "Small",
                3 => "Medium",
                4 => "Large",
                5 => "Huge",
                6 => "Enormous",
                7 => "Gargantuan",
                _ => throw new Exception($"Unexpected Mission Size {sz}")
            };
        }

        public static float DirectionToAngle(Direction d) {
            return d switch {
                Direction.East => 0.0f,
                Direction.NorthEast => 45.0f,
                Direction.North => 90.0f,
                Direction.NorthWest => 135.0f,
                Direction.West => 180.0f,
                Direction.SouthWest => -135.0f,
                Direction.South => -90.0f,
                Direction.SouthEast => -45.0f,
                _ => throw new NotImplementedException(),
            };
        }

        public static Direction AngleToDirection(double ang) {
            if (ang < 0) ang += 360.0;
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

        public static string RunLengthEncode(string str) {
            return str; // TODO
        }
        public static string RunLengthDecode(string str) {
            return str; // TODO
        }

        public static string MissionGoalToString(Mission.MissionGoal mg) {
            return mg switch {
                Mission.MissionGoal.ExploreAll => "Explore",
                Mission.MissionGoal.KillAll => "Kill All",
                Mission.MissionGoal.KillBoss => "Assassination",
                Mission.MissionGoal.Gather => "Gathering",
                Mission.MissionGoal.FindItem => "Treasure Hunt",
                Mission.MissionGoal.Defend => "Defend Objective",
                _ => "Unknown",
            };
        }

        public static double RoundSF(double d, int digits) {
            if (d == 0) return 0;
            decimal scale = (decimal)Math.Pow(10, Math.Floor(Math.Log10(Math.Abs(d))) + 1);
            return (double)(scale * Math.Round((decimal)d / scale, digits));
        }

        #region Soldier Functions
        public static string RelationsToString(int r) {
            return r switch {
                -5 => "Despised",
                -4 => "Hated",
                -3 => "Hostile",
                -2 => "Suspicious",
                -1 => "Unfriendly",
                0 => "Neutral",
                1 => "Friendly",
                2 => "Allied",
                3 => "Revered",
                4 => "Heroic",
                5 => "Exalted",
                _ => throw new Exception($"Unexpected relation level : {r}")
            };
        }

        public static double RelationsToCostMod(int r) {
            return r switch {
                -5 => 10d,
                -4 => 5d,
                -3 => 3d,
                -2 => 2.0d,
                -1 => 1.2d,
                0 => 1d,
                1 => 0.95d,
                2 => 0.9d,
                3 => 0.85d,
                4 => 0.8d,
                5 => 0.75d,
                _ => throw new Exception($"Unexpected relation level : {r}")
            };
        }

        public static int ExperienceToRelations(int xp) {
            double d = xp / Const.RaceRelationsExperienceScale;
            // Exp = Lev * (Lev+1) * Scale if Lev >= 0
            // Exp = -Lev * (Lev+1) * Scale if Lev < 0
            if (d < 0) return Math.Max(-5, -(int)Math.Floor(Math.Sqrt(-d + 0.25) - 0.5) - 1);
            return Math.Min(5, (int)Math.Floor(Math.Sqrt(d + 0.25) - 0.5));
        }

        public static int ExperienceToSkillLevel(int xp) {
            double d = xp / Const.WeaponSkillBase;
            // Inverse triangular numbers ;)
            return (int)Math.Floor((Math.Sqrt(1 + d * 8) - 1) / 2);
        }

        public static int SkillLevelToExperience(int lvl) {
            return lvl * (lvl + 1) * Const.WeaponSkillBase / 2;
        }

        public static double GenerateHitRoll(IEntity from, IEntity to) {
            double att = from.Attack + (from.EquippedWeapon?.AccuracyBonus ?? 0);
            double def = to.Defence;
            double dist = from.RangeTo(to);
            double size = to.Size;
            double dropoff = (from.EquippedWeapon == null) ? 0.0 : from.EquippedWeapon.DropOff;
            double encumbrancePenalty = from.Encumbrance * Const.EncumbranceHitPenalty;
            double hit = Const.HitBias
                         + ((rnd.NextDouble() - 0.5) * Const.RandomHitScale)
                         + ((att - def) * Const.GuaranteedHitScale)
                         + (rnd.NextDouble() * att * Const.AttackScale)
                         - (rnd.NextDouble() * def * Const.DefenceScale)
                         + Math.Pow(size, Const.HitSizePowerLaw)
                         - encumbrancePenalty;
            double dropoffmod = (dist * dropoff);
            if (from is Soldier s) {
                hit += Const.SoldierHitBias;
                int sharpshooter = s.GetUtilityLevel(Soldier.UtilitySkill.Sharpshooter);
                dropoffmod *= Math.Pow(Const.SharpshooterRangeMod, sharpshooter);
            }
            if (dropoff > 0.0) hit -= dropoffmod; // Harder to hit at long range. 0.0 = melee weapon.
            if (to is Creature cre && !cre.IsAlert) {
                hit += Const.SurpriseHitMod;
            }
            return hit;
        }

        public static double BodyPartToArmourScale(BodyPart bp) {
            return bp switch {
                BodyPart.Chest => 3.0,
                BodyPart.Legs => 2.0,
                BodyPart.Arms => 2.0,
                BodyPart.Feet => 1.0,
                BodyPart.Hands => 1.0,
                BodyPart.Head => 1.0,
                _ => throw new Exception("Unhandled body part in BodyPartToArmourScale : " + bp),
            };
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
            return sk switch {
                Soldier.UtilitySkill.Medic => 10,
                _ => 0,
            };
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

        public static bool IsPassable(MissionLevel.TileType tp) {
            return tp switch {
                MissionLevel.TileType.Floor => true,
                MissionLevel.TileType.OpenDoorHorizontal => true,
                MissionLevel.TileType.OpenDoorVertical => true,
                _ => false,
            };
        }
        #endregion

        #region Item Functions
        public static Dictionary<IItem, int> DismantleEquipment(IEquippable eq, int lvl) {
            Dictionary<IItem, int> dRemains = new Dictionary<IItem, int>();
            Random rand = new Random();
            double diff = ((double)eq.BaseType.BaseRarity / 2.0) + (double)eq.Level;
            double fract = (double)lvl / diff;
            Dictionary<MaterialType, int> dMats = new (eq.BaseType.Materials);
            if (eq is Weapon wp && wp.Mod is not null) {
                foreach (MaterialType mmat in wp.Mod.Materials.Keys) {
                    if (dMats.ContainsKey(mmat)) dMats[mmat] += wp.Mod.Materials[mmat];
                    else dMats.Add(mmat, wp.Mod.Materials[mmat]);
                }
            }
            foreach (MaterialType mat in dMats.Keys) {
                int iQuantity = dMats[mat];
                int iRecovered = (int)(rand.NextDouble() * fract * (double)(iQuantity + 1.0));
                if (iRecovered > iQuantity) iRecovered = iQuantity;
                if (iRecovered > 0) dRemains.Add(new Material(mat), iRecovered);
            }

            return dRemains;
        }

        public static Color LevelToColour(int lvl) {
            return lvl switch {
                0 => Color.FromArgb(255, 255, 255, 255),
                1 => Color.FromArgb(255, 255, 255, 0),
                2 => Color.FromArgb(255, 50, 255, 100),
                3 => Color.FromArgb(255, 50, 100, 255),
                4 => Color.FromArgb(255, 170, 45, 255),
                5 => Color.FromArgb(255, 255, 100, 70),
                _ => throw new Exception("Unexpected equipment level : " + lvl)
            };
        }

        public static IItem? LoadItem(XmlNode? xml) {
            if (xml == null) return null;
            if (xml.Name.Equals("Armour")) return new Armour(xml);
            if (xml.Name.Equals("Weapon")) return new Weapon(xml);
            if (xml.Name.Equals("Equipment")) return new Equipment(xml);
            if (xml.Name.Equals("Corpse")) return new Corpse(xml);
            if (xml.Name.Equals("Material")) return new Material(xml);
            if (xml.Name.Equals("MissionItem")) return new MissionItem(xml);
            throw new Exception("Attempting to load IItem of unknown type : " + xml.Name);
        }

        public static string LevelToDescription(int lvl) {
            return lvl switch {
                0 => "Basic",
                1 => "Good",
                2 => "Fine",
                3 => "Superb",
                4 => "Epic",
                5 => "Legendary",
                _ => throw new Exception($"Unexpected equipment level : {lvl}")
            };
        }

        public static double CalculateMass(Dictionary<IItem, int> dEquip) {
            double Mass = 0.0;
            foreach (KeyValuePair<IItem, int> kvp in dEquip) {
                Mass += kvp.Key.Mass * kvp.Value;
            }
            return Mass;
        }

        public static double ArmourReduction(double ar) {
            return Math.Pow(0.5, ar / Const.ArmourScale);
        }

        public static int GenerateDroppedItemLevel(int lev, bool boss) {
            int ilev = (int)((rnd.NextDouble() * lev) / 4.0);
            if (boss) {
                int ilev2 = (int)((rnd.NextDouble() * lev) / 4.0);
                if (ilev2 > ilev) ilev = ilev2;
            }
            return ilev;
        }

        public static double ItemLevelToCostMod(int lev) {
            return Math.Pow(lev + 1, Const.EquipmentLevelCostExponent) * Math.Pow(Const.EquipmentLevelCostBaseExponent, lev);
        }

        public static IItem? GenerateRandomItem(Random rnd, int lvl, Race? race, bool bAddEquipment = true) {
            // Armour
            double rnum = rnd.NextDouble();
            if (!bAddEquipment) rnum = 1.0;
            if (rnum < 0.2) { // Random weapon
                return GenerateRandomWeapon(rnd, lvl, race);
            }
            if (rnum < 0.7) { // Random armour
                return GenerateRandomArmour(rnd, lvl, race);
            }
            if (rnum < 0.8) { // Random material
                double best = 0.0;
                MaterialType? mbest = null;
                foreach (MaterialType mat in StaticData.Materials.Where(m => m.CanBuild(race))) {
                    double r = rnd.NextDouble() * Math.Pow(mat.Rarity, 5.0 / (lvl + 4.0));
                    if (r > best) {
                        best = r;
                        mbest = mat;
                    }
                }
                if (mbest is null) return null;
                return new Material(mbest);
            }
            else { // Random item
                double best = 0.0;
                ItemType? ibest = null;
                foreach (ItemType it in StaticData.ItemTypes) {
                    if (it.BaseRarity > lvl + 5) continue;
                    if (!it.CanBuild(race)) continue;
                    double r = rnd.NextDouble() * Math.Pow(it.Rarity, 5.0 / (lvl + 4.0));
                    if (r > best) {
                        best = r;
                        ibest = it;
                    }
                }
                if (ibest is null) return null;
                return new Equipment(ibest);
            }
        }

        public static Weapon? GenerateRandomWeapon(Random rnd, int Level, Race? race) {
            List<WeaponType> wts = new List<WeaponType>();
            double trar = 0.0;
            foreach (WeaponType tp in StaticData.WeaponTypes) {
                if (!tp.CanBuild(race)) continue;
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

        public static Armour? GenerateRandomArmour(Random rnd, int Level, Race? race) {
            // Choose between all single-location armour pieces
            double best = 0.0;
            ArmourType? abest = null;
            MaterialType? mbest = null;
            foreach (ArmourType at in StaticData.ArmourTypes) {
                if (!at.CanBuild(race)) continue;
                if (at.Locations.Count == 1) {
                    foreach (MaterialType mat in StaticData.Materials.Where(mat => mat.CanBuild(race))) {
                        if (!mat.IsArmourMaterial) continue;
                        if (race is not null && mat.RequiredRace is not null && race != mat.RequiredRace) continue;
                        double r = rnd.NextDouble() * at.Rarity * mat.Rarity;
                        if (r > best) {
                            best = r;
                            abest = at;
                            mbest = mat;
                        }
                    }
                }
            }
            if (abest is null || mbest is null) return null;
            Armour ar = new Armour(abest, mbest, 0);

            // Scale to level
            for (int n = 0; n < Level / 2; n++) {
                if (rnd.NextDouble() < 0.6) ar.UpgradeArmour(race);
            }
            if (Level % 1 == 1 && rnd.NextDouble() < 0.3) ar.UpgradeArmour(race);

            return ar;
        }

        public static string UtilitySkillToDesc(UtilitySkill sk) {
            return sk switch {
                UtilitySkill.Armoursmith => "The ability to make and modify pieces of armour.",
                UtilitySkill.Avoidance => "The ability to dodge incoming attacks, increasing your defence.",
                UtilitySkill.Bladesmith => "The ability to make and modify bladed weapons such as swords and axes.",
                UtilitySkill.Engineer => "The ability to make and modify items of equipment.",
                UtilitySkill.Sharpshooter => "The ability to aim accurately over long distances, reducing the penalty for range.",
                UtilitySkill.Gunsmith => "The ability to make and modify guns.",
                UtilitySkill.Medic => "The ability to heal injuries using a medikit.",
                UtilitySkill.Perception => "The ability to spot hidden traps, treasure stashes and secret doors.",
                UtilitySkill.Scavenging => "The ability to obtain valuable materials from the remains of downed enemies.",
                UtilitySkill.Stealth => "The ability to move silently and avoid being spotted by enemy creatures.",
                _ => "Unknown"
            };
        }
        #endregion

        #region Xml Parsing Utility Functions
        public static IEnumerable<XmlNode> SelectNodesToList(this XmlNode root, string path) {
            List<XmlNode> nodes = new List<XmlNode>();
            if (string.IsNullOrEmpty(path) || root is null) return nodes;
            XmlNodeList? nodeList = root.SelectNodes(path);
            if (nodeList is null) return nodes;
            foreach (XmlNode node in nodeList) {
                nodes.Add(node);
            }
            return nodes;
        }

        public static string SelectNodeText(this XmlNode root, string path, string? defaultValue = null) {
            if (string.IsNullOrEmpty(path) || root is null) return defaultValue ?? string.Empty;
            return root.SelectSingleNode(path)?.InnerText ?? defaultValue ?? string.Empty;
        }
        public static double SelectNodeDouble(this XmlNode root, string path, double? defaultValue = null) {
            if (string.IsNullOrEmpty(path) || root is null) return defaultValue ?? throw new Exception($"Could not find double data for path {path}");
            string? strText = root.SelectSingleNode(path)?.InnerText;
            if (string.IsNullOrEmpty(strText)) return defaultValue ?? throw new Exception($"Found empty double data for path {path}");
            if (!double.TryParse(strText, out double dVal)) {
                throw new Exception($"Could not parse double data for path {path} : {strText}");
            }
            return dVal;
        }
        public static int SelectNodeInt(this XmlNode root, string path, int? defaultValue = null) {
            if (string.IsNullOrEmpty(path) || root is null) return defaultValue ?? throw new Exception($"Could not find int data for path {path}");
            string? strText = root.SelectSingleNode(path)?.InnerText;
            if (string.IsNullOrEmpty(strText)) return defaultValue ?? throw new Exception($"Found empty int data for path {path}");
            if (!int.TryParse(strText, out int iVal)) {
                throw new Exception($"Could not parse int data for path {path} : {strText}");
            }
            return iVal;
        }

        public static EnumType SelectNodeEnum<EnumType>(this XmlNode root, string path, EnumType defaultValue) {
            if (string.IsNullOrEmpty(path) || root is null) return defaultValue;
            string? strText = root.SelectSingleNode(path)?.InnerText;
            if (string.IsNullOrEmpty(strText)) return defaultValue;
            return InterpretEnum<EnumType>(strText);
        }
        public static EnumType SelectNodeEnum<EnumType>(this XmlNode root, string path) {
            if (string.IsNullOrEmpty(path) || root is null) throw new Exception($"Path not found : {path}");
            string? strText = root.SelectSingleNode(path)?.InnerText;
            if (string.IsNullOrEmpty(strText)) throw new Exception($"Path was empty : {path}");
            return InterpretEnum<EnumType>(strText);
        }
        private static EnumType InterpretEnum<EnumType>(string strText) {
            if (!Enum.TryParse(typeof(EnumType), strText, out object? oVal)) {
                throw new Exception($"Could not parse string {strText} to enum of type {typeof(EnumType).FullName}");
            }
            if (oVal is null) throw new Exception($"Enum string {strText} converted to a null object");
            if (oVal is not EnumType eVal) throw new Exception($"Could not convert enum {oVal} to enum of type {typeof(EnumType).FullName}");
            return eVal;

        }

        public static string GetAttributeText(this XmlNode root, string attributeName, string defaultValue = "") {
            if (string.IsNullOrEmpty(attributeName) || root is null) return defaultValue;
            return root.Attributes?[attributeName]?.Value ?? defaultValue;
        }
        public static double GetAttributeDouble(this XmlNode root, string attributeName, double? defaultValue = null) {
            if (string.IsNullOrEmpty(attributeName) || root is null) return defaultValue ?? throw new Exception($"Could not find double data for path {attributeName}");
            string? strText = root.Attributes?[attributeName]?.Value;
            if (string.IsNullOrEmpty(strText)) return defaultValue ?? throw new Exception($"Found empty double data for path {attributeName}");
            if (!double.TryParse(strText, out double dVal)) {
                throw new Exception($"Could not parse double data for path {attributeName} : {strText}");
            }
            return dVal;
        }
        public static int GetAttributeInt(this XmlNode root, string attributeName, int? defaultValue = null) {
            if (string.IsNullOrEmpty(attributeName) || root is null) return defaultValue ?? throw new Exception($"Could not find int data for path {attributeName}");
            string? strText = root.Attributes?[attributeName]?.Value;
            if (string.IsNullOrEmpty(strText)) return defaultValue ?? throw new Exception($"Found empty int data for path {attributeName}");
            if (!int.TryParse(strText, out int iVal)) {
                throw new Exception($"Could not parse int data for path {attributeName} : {strText}");
            }
            return iVal;
        }
        #endregion
    }
}
