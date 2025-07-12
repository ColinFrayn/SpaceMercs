using OpenTK.Mathematics;
using SpaceMercs.Graphics;
using System.IO;
using System.Xml;

namespace SpaceMercs {
    public abstract class AstronomicalObject {
        public enum AstronomicalObjectType { Star, Planet, Moon, HyperGate, Unknown };
        public string Name { get; protected set; }
        public double Radius; // In metres
        public double AxialRotationPeriod; // Period of axial rotation (seconds)
        public double Temperature { get; set; } // Kelvin
        public Vector3 BaseColour;
        public int Seed;
        public int Ox, Oy, Oz; // Perlin seed offsets
        protected byte[]? texture;
        protected int iTexture;
        public int ID;
        public abstract float DrawScale { get; }

        public AstronomicalObject() {
            iTexture = -1;
            Name = string.Empty;
        }
        public AstronomicalObject(XmlNode xml) {
            iTexture = -1;
            ID = xml.GetAttributeInt("ID");
            Name = xml.SelectNodeText("Name", string.Empty);
            Radius = xml.SelectNodeDouble("Radius");

            // Default here is an approximation, for backwards compatibility with versions where I didn't save this
            AxialRotationPeriod = xml.SelectNodeDouble("ARot", Const.DayLength * (Radius / Const.PlanetSize));
            Temperature = xml.SelectNodeDouble("Temp");
            Seed = xml.SelectNodeInt("Seed");
            Random rnd = new Random(Seed);
            Ox = rnd.Next(Const.SeedBuffer);
            Oy = rnd.Next(Const.SeedBuffer);
            Oz = rnd.Next(Const.SeedBuffer);
        }

        public abstract string PrintCoordinates();
        public abstract void DrawSelected(ShaderProgram prog, int Level, double elapsedSeconds);
        public abstract void SetupTextureMap(int width, int height);
        public abstract void ClearData();
        public abstract void SetName(string str);
        public abstract Star GetSystem();   // Parent Star (or self, if star)
        public abstract int GetPopulation(Race? rc = null);
        public Vector3 GetMapLocation() {
            return this.GetSystem().MapPos;
        }
        protected float RotationAngle(double elapsedSeconds) => (float)((elapsedSeconds * 2d * Math.PI / AxialRotationPeriod) % (2d * Math.PI));

        // Calculate the distance between two AOs
        public static double CalculateDistance(AstronomicalObject ao1, AstronomicalObject ao2) {
            double dist = 0.0;
            if (ao1 == null || ao2 == null) return -1.0;
            if (ao1 == ao2) return 0.0;
            Star st1 = ao1.GetSystem();
            Star st2 = ao2.GetSystem();
            if (st1 == st2) {
                if (ao1 is Star) {
                    if (ao2 is OrbitalAO ao2o) return ao2o.DistanceFromStar();
                }
                if (ao2 is Star) {
                    if (ao1 is OrbitalAO ao1o) return ao1o.DistanceFromStar();
                }

                if (ao1 is not OrbitalAO oao1 || ao2 is not OrbitalAO oao2) {
                    throw new Exception($"Unexpected AO types in CalculateDistance: {ao1.GetType()} : {ao2.GetType()}");
                }
                OrbitalAO pl1 = ao1 is Moon m1 ? (Planet)m1.Parent : oao1;
                OrbitalAO pl2 = ao2 is Moon m2 ? (Planet)m2.Parent : oao2;
                if (pl1 == pl2) {
                    if (ao1 is Moon mn1) dist += mn1.OrbitalDistance;
                    if (ao2 is Moon mn2) dist -= mn2.OrbitalDistance;
                    dist = Math.Abs(dist);
                }
                else {
                    dist = Math.Abs(pl1.OrbitalDistance - pl2.OrbitalDistance);
                    if (ao1 is Moon mn1) dist += mn1.OrbitalDistance;
                    if (ao2 is Moon mn2) dist += mn2.OrbitalDistance;
                }
            }
            else {
                dist = (st1.MapPos - st2.MapPos).Length * Const.LightYear;
            }
            return dist;
        }

        // Save this object to an Xml file
        public virtual void SaveToFile(StreamWriter file, GlobalClock clock) {
            if (!string.IsNullOrEmpty(Name) && !string.Equals(Name, "Unnamed")) file.WriteLine("<Name>" + Name + "</Name>");
            file.WriteLine("<Radius>" + Radius.ToString("N0") + "</Radius>");
            file.WriteLine("<ARot>" + AxialRotationPeriod.ToString("N0") + "</ARot>");
            file.WriteLine("<Temp>" + Temperature.ToString("N2") + "</Temp>");
            file.WriteLine("<Seed>" + Seed + "</Seed>");
        }

        // Get a random difficulty level for this location (e.g. for a mercenary or a mission)
        public int GetRandomMissionDifficulty(Random rand) {
            return GetRandomMissionDifficulty(rand.NextDouble);
        }
        public int GetRandomMissionDifficulty(Func<double> nextDouble) {
            Star sys = GetSystem();

            // Human home system so carefully configure the difficulty to be appropriate for low level players
            if (sys == StaticData.HumanRace.HomePlanet.GetSystem()) {
                return GetHomeSystemMissionDifficulty(nextDouble);
            }

            // This is a home system for one of the alien races so set the danger level appropriately
            foreach (Race r in StaticData.Races) {
                if (sys == r.HomePlanet.GetSystem()) {
                    return GetHomeSystemMissionDifficulty(nextDouble, 5);
                }
            }

            // Mission difficulty scales up as we go further from home
            double dLevel = GetBaseDifficultyForSystem() + nextDouble() + nextDouble() + nextDouble();

            // Increase the difficulty based on where we are within this system
            // Only in inner sector rings. Otherwise entire systems are univorm
            if (this is HabitableAO hao && GetSystem().Sector.SectorRing <= 1) {
                // Local colony helps make it safer
                Planet? pl = hao.ParentPlanet();
                if (pl != null) {
                    // Planet/moon without local colony -> dangerous         
                    if (hao.Colony == null && pl.Colony == null) dLevel += nextDouble();
                    // More distant planets can be more hostile
                    double pDist = Math.Sqrt(pl.ID);
                    dLevel += pDist * (nextDouble() + 1d) / 2d;
                }
            }
            else dLevel += 2d;

            return (int)dLevel;
        }
        public int GetPrecursorMissionDifficultyForSystem() {
            Star sys = GetSystem();
            double dLevel = GetBaseDifficultyForSystem() + 3d;

            return (int)dLevel;
        }
        private double GetBaseDifficultyForSystem() {
            // Mission difficulty scales up as we go further from home
            double dDist = AstronomicalObject.CalculateDistance(StaticData.HumanRace.HomePlanet, this);
            double dDistLY = dDist / Const.LightYear;
            double dLevel = 1d;

            // Scale more steeply in home sector (Yes I know sectors are not circular.)
            double innerDist = Math.Min(Const.EncounterLevelScalingInnerRadius, dDistLY);
            dLevel += Math.Pow(innerDist * Const.EncounterLevelScalingDistanceInner, Const.EncounterLevelScalingExponentInner);

            // Scale more shallowly outside home sector
            if (dDistLY > Const.EncounterLevelScalingInnerRadius) {
                dLevel += Math.Pow((dDistLY - Const.EncounterLevelScalingInnerRadius) / Const.EncounterLevelScalingDistanceOuter, Const.EncounterLevelScalingExponentOuter);
            }
            return dLevel;
        }

        // Get the mission difficulty for the home system of the Human race
        private int GetHomeSystemMissionDifficulty(Func<double> nextDouble, int offset = 0) {
            // Home planet is a nursery zone. Mostly 1s with a sprinkling of 2s.
            if (this == StaticData.HumanRace.HomePlanet) {
                return (int)(nextDouble() * 1.7d) + 1 + offset;
            }
            // Planets increase in diff as they get further from home
            if (this is Planet pl) {
                double pdist = Math.Abs(pl.ID - StaticData.HumanRace.HomePlanet.ID);
                double diff = (nextDouble() * 1.5d) + 1.5d + (pdist * (0.3d + (nextDouble() * 0.25d)));
                if (diff < 2d) diff = 2d;
                return (int)diff + offset;
            }
            // Moons increase in diff as they get further from their parent planet, and as the parent gets further from home
            if (this is Moon mn) {
                double pdist = Math.Abs((mn.Parent).ID - StaticData.HumanRace.HomePlanet.ID);
                double diff = (nextDouble() * 1.5d) + 1.5d + (pdist * (0.3d + (nextDouble() * 0.25d)));
                diff += mn.ID * ((nextDouble() * 0.1d) + 0.1d);
                if (diff < 2d) diff = 2d;
                return (int)diff + offset;
            }
            if (this is HyperGate) {
                double diff = (nextDouble() * 1.5d) + 3d + (nextDouble() * 1.25d);
                if (diff < 3d) diff = 3d;
                return (int)diff + offset;
            }
            if (this is SpaceHulk) {
                double diff = (nextDouble() * 1.5d) + 3.5d + (nextDouble() * 1.25d);
                if (diff < 3d) diff = 3d;
                return (int)diff + offset;
            }
            // This *could* happen when flying from one system to another
            if (this is Star) {
                double diff = (nextDouble() * 1.5d) + 3d + nextDouble();
                if (diff < 3d) diff = 3d;
                return (int)diff + offset;
            }
            // Shouldn't get here
            throw new Exception($"Strange AO type {this.GetType().FullName} in {nameof(GetHomeSystemMissionDifficulty)}");
        }

        // Get a random race suitable for this location
        public Race GetRandomRace(Random rand) {
            if (this is HabitableAO hao) {
                if (hao.Colony != null) return hao.Colony.Owner;
            }
            Race? rBest = null;
            double dBestScore = 0.0;
            foreach (Race r in StaticData.Races) {
                //if (!r.Known) continue;
                double dDist = AstronomicalObject.CalculateDistance(GetSystem(), r.HomePlanet.GetSystem()) / Const.LightYear;
                if (dDist < 0.2) dDist = 0.2; // Just being careful... Even in a home system, there is a tiny chance of getting aliens
                double dScore = (rand.NextDouble() + 0.1) / (dDist * dDist);
                if (GetSystem().Sector == r.HomePlanet.GetSystem().Sector) dScore *= 1.5; // More likely to encounter a race if it's their home sector
                if (rBest is null || dScore > dBestScore) {
                    dBestScore = dScore;
                    rBest = r;
                }
            }
            return rBest ?? throw new Exception("Could not generate a random race");
        }

        // Location to string
        public override string ToString() {
            string str = "(" + GetSystem().Sector.SectorX.ToString() + "," + GetSystem().Sector.SectorY.ToString() + ")";
            return this switch {
                Star => str + ":" + ID,
                Planet => str + ":" + GetSystem().ID + ":" + ID,
                Moon mn => str + ":" + GetSystem().ID + ":" + mn.Parent.ID + "." + ID,
                _ => throw new NotImplementedException(),
            };
        }
    }
}
