using OpenTK.Mathematics;
using System.Windows.Media.Media3D;
using System.Xml;
using static SpaceMercs.Delegates;
using static SpaceMercs.Soldier;
using static SpaceMercs.VisualEffect;

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

        public static string MissionGoalToString(Mission.MissionGoal mg) {
            return mg switch {
                Mission.MissionGoal.ExploreAll => "Explore",
                Mission.MissionGoal.KillAll => "Kill All",
                Mission.MissionGoal.KillBoss => "Assassination",
                Mission.MissionGoal.Gather => "Gathering",
                Mission.MissionGoal.FindItem => "Treasure Hunt",
                Mission.MissionGoal.Defend => "Defend Objective",
                Mission.MissionGoal.Artifact => "Artifact Hunt",
                Mission.MissionGoal.Countdown => "Time Critical",
                _ => "Unknown",
            };
        }

        public static double RoundSF(double d, int digits) {
            if (d == 0) return 0;
            decimal scale = (decimal)Math.Pow(10, Math.Floor(Math.Log10(Math.Abs(d))) + 1);
            return (double)(scale * Math.Round((decimal)d / scale, digits));
        }

        // When shooting a weapon, create shot particles
        public static void CreateShots(Weapon EquippedWeapon, IEntity from, int tx, int ty, int tSize, List<ShotResult> results, double shotRange, EffectFactory effectFactory, float baseDelay) {
            float sdelay = baseDelay;
            float avDam = (float)(EquippedWeapon.AverageDamage);
            float shotSize = (float)avDam / Const.ShotSizeScale;
            float duration = (float)shotRange * Const.ShotDurationScale / (float)EquippedWeapon.Type.ShotSpeed;
            float sLength = (float)EquippedWeapon.Type.ShotLength; // Length of a shot, not the weapon itself
            if (sLength == 0.0f) duration = avDam * Const.ShotDurationScale; // This is not a projectile e.g. arc rifle, so just leave the whole distance highlighted for a while
            float scatter = (float)EquippedWeapon.DropOff * (float)shotRange * Const.ShotScatterScale;
            Random rand = new Random();
            Color col = EquippedWeapon?.Type?.ShotColor ?? Color.FromArgb(255, 200, 200, 200);
            foreach (ShotResult result in results) {
                float scatterMod = result.Hit ? 0.05f : scatter;
                float sx = (float)Utils.NextGaussian(rand, 0, scatterMod);
                float sy = (float)Utils.NextGaussian(rand, 0, scatterMod);
                effectFactory(EffectType.Shot, tx, ty, new Dictionary<string, object>() { { "Result", result }, { "FX", from.X + 0.5f }, { "TX", tx + ((float)tSize / 2f) + sx }, { "FY", from.Y + 0.5f }, { "TY", ty + ((float)tSize / 2f) + sy }, { "Delay", sdelay }, { "Length", sLength }, { "Duration", duration }, { "Size", shotSize }, { "Colour", col } });
                sdelay += (float)(EquippedWeapon?.Type?.Delay ?? 0d);
            }
        }

        // Resolve Melee weapon hits
        public static void CreateMeleeHits(Weapon? equippedWeapon, int tx, int ty, List<ShotResult> results, double sneakMod, EffectFactory effectFactory) {
            float sdelay = 0f;
            foreach (ShotResult result in results) {
                effectFactory(EffectType.Melee, tx, ty, new Dictionary<string, object>() { { "Result", result }, { "Delay", sdelay }, { "SneakMod", sneakMod } });
                sdelay += (float)(equippedWeapon?.Type?.Delay ?? 0d);
            }
        }

        // Resolve multiple weapon impacts / AoE. Note this is only used for primary weapon hits, not e.g. thrown grenades.
        public static void ResolveHits(IEnumerable<IEntity> hsAttacked, Weapon? wp, IEntity? source, EffectFactory effectFactory, ItemEffect.ApplyItemEffect applyEffect, ShowMessageDelegate showMessage, double sneakDmgMod = 1d) {
            if (source is null) return;
            Random rand = new Random();

            // Note that this enumerable can contain duplicates
            foreach (IEntity tgt in hsAttacked) {
                if (tgt.Health > 0.0) {
                    Dictionary<WeaponType.DamageType, double> hitDmg = source.GenerateDamage();
                    // Melee sneak attack
                    if (sneakDmgMod != 1d && hitDmg.TryGetValue(WeaponType.DamageType.Physical, out double physDmg)) {
                        physDmg *= sneakDmgMod;
                        hitDmg[WeaponType.DamageType.Physical] = physDmg;
                    }

                    // Mining skill damage bonus
                    if (tgt is ResourceNode nd && source is Soldier s && hitDmg.TryGetValue(WeaponType.DamageType.Physical, out double physDmgRn)) {
                        int miningSkill = s.GetUtilityLevel(UtilitySkill.Miner);
                        physDmgRn *= 1d + ((double)miningSkill / 5d);
                        hitDmg[WeaponType.DamageType.Physical] = physDmgRn;
                    }

                    double TotalDam = tgt.CalculateDamage(hitDmg);
                    float xshift = (float)(rand.NextDouble() - 0.5d) / 3f;
                    if (Math.Abs(TotalDam) > 0.1d) {
                        if (TotalDam > 0d) {
                            effectFactory(EffectType.Damage, (float)tgt.X + ((float)tgt.Size / 2f) + xshift, (float)tgt.Y + ((float)tgt.Size / 2f), new Dictionary<string, object>() { { "Value", TotalDam } });
                        }
                        else {
                            effectFactory(EffectType.Healing, (float)tgt.X + ((float)tgt.Size / 2f) + xshift, (float)tgt.Y + ((float)tgt.Size / 2f), new Dictionary<string, object>() { { "Value", -TotalDam } });
                        }
                    }
                    tgt.InflictDamage(hitDmg, applyEffect, effectFactory, source);
                    if (wp != null && wp.Shred > 0d) tgt.ShredArmour(wp.Shred);

                    // Apply effect?
                    if (wp != null) {
                        if (wp.Type.ItemEffect != null) {
                            tgt.ApplyEffectToEntity(source, wp.Type.ItemEffect, effectFactory, applyEffect);
                        }
                    }

                    // Soldier hits creature -> check change target, and maybe register kill
                    if (tgt is Creature cre) {
                        if (cre.Health <= 0d) {
                            if (source is Soldier sKiller) sKiller.RegisterKill(cre, showMessage);
                        }
                        else {
                            cre.CheckChangeTarget(TotalDam, source);
                        }
                    }

                    // Add weapon experience if shot was with a weapon and from a soldier
                    if (source is Soldier sd && wp != null && tgt is not null) {
                        int exp = Math.Max(1, tgt.Level - sd.Level) * Const.DEBUG_WEAPON_SKILL_MOD / hsAttacked.Count();
                        sd.AddWeaponExperience(wp, exp, showMessage);
                    }
                }
            }
        }

        #region Experience Functions
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
        public static double RelationsToHyperspaceCost(int r) {
            return r switch {
                2 => 6d,
                3 => 4d,
                4 => 2d,
                5 => 1d,
                _ => 0d
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
            double d = (double)xp / (double)Const.RaceRelationsExperienceScale;
            // Exp = Lev * (Lev+1) * Scale if Lev >= 0
            // Exp = -Lev * (Lev+1) * Scale if Lev < 0
            if (d < 0) return Math.Max(-5, -(int)Math.Floor(Math.Sqrt(-d + 0.25) - 0.5) - 1);
            return Math.Min(5, (int)Math.Floor(Math.Sqrt(d + 0.25) - 0.5));
        }
        public static double ExperienceToRelationsFraction(int xp) {
            int lev = ExperienceToRelations(xp);
            int current = RelationsLevelToExperience(lev);
            int next = RelationsLevelToExperience(lev + 1);
            return (double)(xp - current) / (double)(next - current);
        }
        public static int RelationsLevelToExperience(int level) {
            if (level < 0) return level * (1 - level) * Const.RaceRelationsExperienceScale;
            return level * (level + 1) * Const.RaceRelationsExperienceScale;
        }

        public static int ExperienceToSkillLevel(int xp) {
            double d = xp / Const.WeaponSkillBase;
            // Inverse triangular numbers ;)
            return (int)Math.Floor((Math.Sqrt(1 + d * 8) - 1) / 2);
        }
        public static int SkillLevelToExperience(int lvl) {
            return lvl * (lvl + 1) * Const.WeaponSkillBase / 2;
        }
        #endregion

        #region Soldier Functions
        public static double GenerateHitRoll(IEntity from, IEntity to) {
            double att = from.Attack + (from.EquippedWeapon?.AccuracyBonus ?? 0);
            double def = to.Defence;
            double dist = from.RangeTo(to);

            // Melee attack mods:
            if (from.EquippedWeapon is null || from.EquippedWeapon.Type.IsMeleeWeapon) {
                att -= from.Encumbrance * Const.MeleeEncumbranceAttackPenalty;
                def -= to.Encumbrance * Const.MeleeEncumbranceDefencePenalty;
                // Melee defending against melee
                if (to.EquippedWeapon is not null && to.EquippedWeapon.Type.IsMeleeWeapon) {
                    // Target is wielding a melee weapon and being attacked in melee. Add a defence bonus.
                    if (Const.EnableCreatureMeleeDefenceBonus && to is Creature cr && cr.IsAlert) {
                        def += cr.Level;
                    }
                    else if (Const.EnableSoldierMeleeDefenceBonus && to is Soldier sd) {
                        def += sd.GetSoldierSkillWithWeaponClass(WeaponType.WeaponClass.Melee);
                    }
                }
                // Heavy weapon defending against melee (penalty)
                if (to.EquippedWeapon is not null && to.EquippedWeapon.Type.WClass == WeaponType.WeaponClass.Heavy) {
                    def -= Const.HeavyWeaponMeleeDefencePenalty;
                }
                // Short melee weapon attacking diagonally (penalty)
                if (from.EquippedWeapon is not null && !from.EquippedWeapon.Type.LongWeapon && dist > 1.1) {
                    att -= Const.DiagonalMeleePenalty;
                }
            }

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

        public static BodyPart GetRandomBodyPartScaledByArmourCoverage() {
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
        public static Dictionary<IItem, int> DismantleEquipment(IEquippable eq, int lvl, int count = 1) {
            Dictionary<IItem, int> dRemains = new Dictionary<IItem, int>();
            Random rand = new Random();
            double diff = (5d + (double)(eq.BaseType.Requirements?.MinLevel ?? 1) / 2d) + (double)eq.Level;
            double fract = (double)lvl / diff;
            Dictionary<MaterialType, int> dMats = new (eq.BaseType.Materials);
            if (eq is Weapon wp && wp.Mod is not null) {
                foreach (MaterialType mmat in wp.Mod.Materials.Keys) {
                    if (dMats.ContainsKey(mmat)) dMats[mmat] += wp.Mod.Materials[mmat];
                    else dMats.Add(mmat, wp.Mod.Materials[mmat]);
                }
            }
            foreach (MaterialType mat in dMats.Keys) {
                int iQuantity = dMats[mat] * count;
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
            return Math.Pow(Const.ArmourReductionBase, Math.Pow(ar / Const.ArmourReductionExponentialScale, Const.ArmourReductionSecondExponent));
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

        public static IItem? GenerateRandomItem(Random rnd, int lvl, Race? race, bool bAddEquipment = true, bool bIncludeWeapons = true) {
            // Armour
            double rnum = rnd.NextDouble();
            if (!bAddEquipment) rnum = 1.0;
            if (!bIncludeWeapons) rnum = rnd.NextDouble() * 0.8d + 0.2d;
            if (rnum < 0.2) { // Random weapon
                return GenerateRandomWeaponByLevel(rnd, lvl, race);
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
                    if ((it.Requirements?.MinLevel ?? 0) + rnd.Next(3) > lvl) continue;
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

        public static Weapon? GenerateRandomWeaponForSoldier(Random rnd, Soldier s) {
            List<WeaponType> wts = new List<WeaponType>();
            double trar = 0.0;
            foreach (WeaponType tp in StaticData.WeaponTypes) {
                if (!tp.CanBuild(s.Race)) continue;
                if (s.SoldierClass is SoldierType.Skirmisher or SoldierType.Assault && tp.WClass != WeaponType.WeaponClass.Rifle) continue;
                if (s.SoldierClass is SoldierType.Assassin or SoldierType.Brute && tp.WClass != WeaponType.WeaponClass.Melee) continue;
                if (s.SoldierClass is SoldierType.Heavy && tp.WClass != WeaponType.WeaponClass.Heavy) continue;
                if (s.SoldierClass is SoldierType.Grenadier && tp.WClass != WeaponType.WeaponClass.Launcher) continue;
                if (s.SoldierClass is SoldierType.Pistoleer && tp.WClass != WeaponType.WeaponClass.Pistol) continue;
                if (s.SoldierClass is SoldierType.Chemicals && tp.WClass != WeaponType.WeaponClass.Emitter) continue;
                if (s.SoldierClass is SoldierType.Sniper && tp.WClass != WeaponType.WeaponClass.Sniper) continue;
                if (s.SoldierClass is SoldierType.Enforcer && tp.WClass != WeaponType.WeaponClass.Shotgun) continue;
                if ((tp.Requirements?.MinLevel ?? 1) <= s.Level && tp.IsUsable) {
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
                    int wpLevel = 0;
                    if (s.Level > 2) {
                        while (wpLevel < 3 && rnd.NextDouble() < 0.5 && (tp.Requirements?.MinLevel ?? 1) + (wpLevel * 3) <= s.Level) wpLevel++;
                    }
                    return new Weapon(tp, wpLevel);
                }
            }
            return null;
        }

        public static Weapon? GenerateRandomWeaponByLevel(Random rnd, int level, Race? race) {
            List<WeaponType> wts = new List<WeaponType>();
            double trar = 0.0;
            foreach (WeaponType tp in StaticData.WeaponTypes) {
                if (!tp.CanBuild(race)) continue;
                if ((tp.Requirements?.MinLevel ?? 1) <= level && tp.IsUsable) {
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
                    int wpLevel = 0;
                    if (level > 2) {
                        while (wpLevel < 3 && rnd.NextDouble() < 0.5 && (tp.Requirements?.MinLevel ?? 1) + (wpLevel * 3) <= level) wpLevel++;
                    }
                    return new Weapon(tp, wpLevel);
                }
            }
            return null;
        }

        public static Weapon? GenerateRandomLegendaryWeapon(Random rnd, int missionLevel) {
            Dictionary<WeaponType, double> wts = new();
            double totalWeight = 0.0;
            foreach (WeaponType tp in StaticData.WeaponTypes) {
                int minLevel = (tp.Requirements?.MinLevel ?? 1) + Const.LegendaryItemLevelDiff;
                if (tp.IsUsable && minLevel <= missionLevel && minLevel + 8 > missionLevel) {
                    double weight = 1.0;
                    if (minLevel + 3 < missionLevel) weight /= (missionLevel - (minLevel + 3)); // Weaker weapons are deprioritised
                    wts.Add(tp, weight);
                    totalWeight += weight;
                }
            }
            if (wts.Count == 0) return null;
            double rar = rnd.NextDouble() * totalWeight;
            WeaponType? wtChosen = null;
            foreach (WeaponType wt in wts.Keys) { 
                rar -= wts[wt];
                if (rar <= 0d) {
                    wtChosen = wt;
                    break;
                }
            }
            if (wtChosen != null) {
                if (missionLevel - (wtChosen.Requirements?.MinLevel ?? 1) - Const.LegendaryItemLevelDiff >= 4) {
                    return new Weapon(wtChosen, 5); // Legendary
                }
                return new Weapon(wtChosen, 4); // Epic
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
                        if (mat.MaxLevel < at.MinMatLvl) continue;
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
                if (ar.Level >= ar.Material.MaxLevel) break;
                if (rnd.NextDouble() < 0.6) ar.UpgradeArmour(race, Level);
            }
            if (Level % 1 == 1 && rnd.NextDouble() < 0.3) ar.UpgradeArmour(race, Level);

            return ar;
        }

        public static Armour? GenerateRandomLegendaryArmour(Random rnd, int missionLevel) {
            // Choose between all armour pieces that are sufficiently strong
            Dictionary<ArmourType, double> ats = new();
            double totalWeight = 0.0;
            foreach (ArmourType at in StaticData.ArmourTypes) {
                int minLevel = (at.Requirements?.MinLevel ?? 1) + Const.LegendaryItemLevelDiff;
                if (minLevel <= missionLevel) {
                    double weight = 1.0;
                    if (minLevel + 4 < missionLevel) weight /= (missionLevel - (minLevel + 4)); // Weaker armour is deprioritised
                    weight *= (30 - at.Size); // Larger armour is rarer
                    ats.Add(at, weight);
                    totalWeight += weight;
                }
            }
            if (ats.Count == 0) return null;
            double rar = rnd.NextDouble() * totalWeight;
            ArmourType? atChosen = null;
            foreach (ArmourType at in ats.Keys) {
                rar -= ats[at];
                if (rar <= 0d) {
                    atChosen = at;
                    break;
                }
            }
            if (atChosen is null) return null;

            // Pick a material
            Dictionary<MaterialType, double> mts = new();
            totalWeight = 0.0;
            foreach (MaterialType mat in StaticData.Materials) {
                if (!mat.IsArmourMaterial) continue;
                if (mat.MaxLevel < 4) continue;
                if (mat.MaxLevel < atChosen.MinMatLvl) continue;
                int minLevel = (mat.Requirements?.MinLevel ?? 1) + Const.LegendaryItemLevelDiff;
                if (minLevel <= missionLevel) {
                    double weight = 1.0;
                    if (minLevel + 4 < missionLevel) weight /= (missionLevel - (minLevel + 4)); // Weaker matrials are deprioritised
                    mts.Add(mat, weight);
                    totalWeight += weight;
                }
            }
            if (mts.Count == 0) return null;
            rar = rnd.NextDouble() * totalWeight;
            MaterialType? mtChosen = null;
            foreach (MaterialType mt in mts.Keys) {
                rar -= mts[mt];
                if (rar <= 0d) {
                    mtChosen = mt;
                    break;
                }
            }
            if (mtChosen is null) return null;

            // Scale to level
            if (missionLevel - (atChosen.Requirements?.MinLevel ?? 1) - Const.LegendaryItemLevelDiff >= 4 && mtChosen.MaxLevel >= 5) {
                return new Armour(atChosen, mtChosen, 5); // Legendary
            }
            return new Armour(atChosen, mtChosen, 4); // Epic
        }

        public static SoldierType GetClassFromStats(int[] Stats, Random rand) {
            if (Stats.Length != 5) throw new Exception("Illegal stat array!");
            int str = Stats[0];
            int agi = Stats[1];
            int ins = Stats[2];
            int tou = Stats[3];
            int end = Stats[4];
            bool TopInsight = ins > str && ins > agi && ins > tou && ins > end;
            bool TopStrength = str > agi && str > ins && str > tou && str > end;
            int r = rand.Next(100);
            if (TopInsight) {
                if (str + 3 > ins) {
                    if (r < 40) return SoldierType.Assault;
                    if (r < 46) return SoldierType.Grenadier;
                    if (r < 52) return SoldierType.Chemicals;
                    return SoldierType.Heavy;
                }
                if (str + 3 < ins) {
                    if (r < 60 && end > 10) return SoldierType.Sniper;
                    return SoldierType.Skirmisher;
                }
                if (r < 60 && (end <= 12 || str > ins || tou > 12)) return SoldierType.Skirmisher;
                return SoldierType.Sniper;
            }
            if (ins > str) {
                if (str > 15) return SoldierType.Heavy;
                if (tou > agi + 3) {
                    if (r < 50) return SoldierType.Assault;
                    else return SoldierType.Enforcer;
                }
                if (agi > tou + 3) {
                    if (r < 50) return SoldierType.Skirmisher;
                    if (r < 70 && end > 9) return SoldierType.Sniper;
                    if (r < 85) return SoldierType.Grenadier;
                    return SoldierType.Chemicals;
                }
            }
            if (TopStrength) {
                if (ins > 12) return SoldierType.Heavy;
                if (agi > tou + 3) return SoldierType.Assassin;
                if (tou > agi + 3) return SoldierType.Brute;
                if (r < 30) return SoldierType.Assassin;
                if (r < 60) return SoldierType.Brute;
            }
            if (str + 5 < ins && end > 9) {
                if (r < 40 && end > 9) return SoldierType.Sniper;
                if (r < 80) return SoldierType.Skirmisher;
                return SoldierType.Pistoleer;
            }
            // Not great at anything. Random generic class
            if (r < 35) return SoldierType.Skirmisher;
            if (r < 70) return SoldierType.Assault;
            if (r < 82) return SoldierType.Grenadier;
            if (r < 94) return SoldierType.Chemicals;
            return SoldierType.Pistoleer;
        }

        public static string UtilitySkillToDesc(UtilitySkill sk) {
            return sk switch {
                UtilitySkill.Armoursmith => "The ability to make and modify pieces of armour.",
                UtilitySkill.Avoidance => "The ability to dodge incoming attacks, increasing your defence.",
                UtilitySkill.Bladesmith => "The ability to make and modify bladed weapons such as swords and axes.",
                UtilitySkill.Engineer => "The ability to make and modify items of equipment.",
                UtilitySkill.Gunsmith => "The ability to make and modify guns.",
                UtilitySkill.Medic => "The ability to heal injuries using a medikit.",
                UtilitySkill.Perception => "The ability to spot hidden traps, treasure stashes and secret doors.",
                UtilitySkill.Scavenging => "The ability to obtain valuable materials from the remains of downed enemies.",
                UtilitySkill.Stealth => "The ability to move silently and avoid being spotted by enemy creatures.",
                _ => "Unknown"
            };
        }

        public static double ConstructionChance(double ItemLevel, double SkillLevel) {
            double diff = SkillLevel - ItemLevel;
            // Standard sigmoid (in percent)
            double chance = 100d / (1d + Math.Exp(-diff * Const.ConstructionChanceScale));
            if (chance > 99.0) return 99.0;
            if (chance < 1.0) return 1.0;
            return chance;
        }
        #endregion

        #region Load Save Utility Functions
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
        public static EnumType GetAttributeEnum<EnumType>(this XmlNode root, string attributeName, EnumType defaultValue) {
            if (string.IsNullOrEmpty(attributeName) || root is null) throw new Exception($"Attribute not found : {attributeName}");
            string? strText = root.Attributes?[attributeName]?.Value;
            if (string.IsNullOrEmpty(strText)) return defaultValue;
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

        public static string RunLengthEncode(string str) {
            return str; // TODO
        }
        public static string RunLengthDecode(string str) {
            return str; // TODO
        }
        #endregion
    }
}
