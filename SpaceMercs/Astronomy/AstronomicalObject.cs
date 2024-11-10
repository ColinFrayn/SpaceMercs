using OpenTK.Mathematics;
using SpaceMercs.Graphics;
using System.IO;
using System.Xml;

namespace SpaceMercs {
    public abstract class AstronomicalObject {
        public enum AstronomicalObjectType { Star, Planet, Moon, HyperGate, Unknown };
        public string Name { get; protected set; }
        public double Radius; // In metres
        public double OrbitalDistance; // In metres
        public double OrbitalPeriod; // Period of orbit (seconds)
        public double AxialRotationPeriod; // Period of axial rotation (seconds)
        public int Temperature { get; set; } // Kelvin
        public Vector3 colour;
        public int Seed;
        public int Ox, Oy, Oz; // Perlin seed offsets
        protected byte[]? texture;
        protected int iTexture;
        public int ID;
        public abstract float DrawScale { get; }

        public AstronomicalObject() {
            iTexture = -1;
            Name = "Unnamed";
        }

        public virtual AstronomicalObjectType AOType { get { return AstronomicalObjectType.Unknown; } }
        public abstract string PrintCoordinates();
        public abstract void DrawSelected(ShaderProgram prog, int Level);
        public abstract void SetupTextureMap(int width, int height);
        public abstract void ClearData();
        public abstract void SetName(string str);
        public abstract Star GetSystem();   // Parent Star (or self, if star)
        public abstract int GetPopulation();
        public Vector3 GetMapLocation() {
            return this.GetSystem().MapPos;
        }

        // Calculate the distance between two AOs
        public static double CalculateDistance(AstronomicalObject ao1, AstronomicalObject ao2) {
            double dist = 0.0;
            if (ao1 == null || ao2 == null) return -1.0;
            if (ao1 == ao2) return 0.0;
            Star st1 = ao1.GetSystem();
            Star st2 = ao2.GetSystem();
            if (st1 == st2) {
                if (ao1.AOType == AstronomicalObjectType.Star || ao2.AOType == AstronomicalObjectType.Star) return 0.0;
                AstronomicalObject pl1, pl2;
                if (ao1.AOType is AstronomicalObjectType.Planet or AstronomicalObjectType.HyperGate) pl1 = ao1;
                else pl1 = ((Moon)ao1).Parent;
                if (ao2.AOType is AstronomicalObjectType.Planet or AstronomicalObjectType.HyperGate) pl2 = ao2;
                else pl2 = ((Moon)ao2).Parent;
                if (pl1 == pl2) {
                    if (ao1.AOType == AstronomicalObjectType.Moon) dist = ((Moon)ao1).OrbitalDistance;
                    if (ao2.AOType == AstronomicalObjectType.Moon) dist -= ((Moon)ao2).OrbitalDistance;
                    dist = Math.Abs(dist);
                }
                else {
                    dist = Math.Abs(pl1.OrbitalDistance - pl2.OrbitalDistance);
                    if (ao1.AOType == AstronomicalObjectType.Moon) dist += ((Moon)ao1).OrbitalDistance;
                    if (ao2.AOType == AstronomicalObjectType.Moon) dist += ((Moon)ao2).OrbitalDistance;
                }
            }
            else {
                dist = (st1.MapPos - st2.MapPos).Length * Const.LightYear;
            }
            return dist;
        }

        // Load generic AO details from an XML file
        protected void LoadAODetailsFromFile(XmlNode xml) {
            iTexture = -1;
            ID = xml.GetAttributeInt("ID");
            Name = xml.SelectNodeText("Name", string.Empty);
            Radius = xml.SelectNodeDouble("Radius");

            OrbitalDistance = xml.SelectNodeDouble("Orbit", 0.0);

            OrbitalPeriod = xml.SelectNodeDouble("PRot");
            // Default here is an approximation, for backwards compatibility with versions where I didn't save this
            AxialRotationPeriod = xml.SelectNodeDouble("ARot", Const.DayLength * (Radius / Const.PlanetSize));
            Temperature = xml.SelectNodeInt("Temp");
            Seed = xml.SelectNodeInt("Seed");
            Random rnd = new Random(Seed);
            Ox = rnd.Next(Const.SeedBuffer);
            Oy = rnd.Next(Const.SeedBuffer);
            Oz = rnd.Next(Const.SeedBuffer);
        }

        // Save this planet to an Xml file
        protected void WriteAODetailsToFile(StreamWriter file) {
            if (!string.IsNullOrEmpty(Name) && !string.Equals(Name,"Unnamed")) file.WriteLine("<Name>" + Name + "</Name>");
            if (OrbitalDistance != 0.0) file.WriteLine("<Orbit>" + Math.Round(OrbitalDistance, 0).ToString() + "</Orbit>");
            file.WriteLine("<Radius>" + Math.Round(Radius, 0).ToString() + "</Radius>");
            file.WriteLine("<PRot>" + Math.Round(OrbitalPeriod, 0).ToString() + "</PRot>");
            file.WriteLine("<ARot>" + Math.Round(AxialRotationPeriod, 0).ToString() + "</ARot>");
            file.WriteLine("<Temp>" + Temperature.ToString() + "</Temp>");
            file.WriteLine("<Seed>" + Seed + "</Seed>");
        }

        // Get a random difficulty level for this location (e.g. for a mercenary or a mission)
        public int GetRandomMissionDifficulty(Random rand) {
            double dDist = AstronomicalObject.CalculateDistance(StaticData.Races[0].HomePlanet, this);
            double dDistLY = dDist / Const.LightYear;
            double dLevel = 1d + rand.NextDouble() + rand.NextDouble() + rand.NextDouble();

            // Base difficulty scales up as we go further from home
            // Scale more steeply in home sector (Yes I know sectors are not circular.)
            double innerDist = Math.Min(Const.EncounterLevelScalingInnerRadius, dDistLY);
            dLevel += Math.Pow(innerDist / Const.EncounterLevelScalingDistanceInner, Const.EncounterLevelScalingExponentInner);
            // Scale more shallowly outside home sector
            if (dDistLY > Const.EncounterLevelScalingInnerRadius) {
                dLevel += Math.Pow((dDistLY-Const.EncounterLevelScalingInnerRadius) / Const.EncounterLevelScalingDistanceOuter, Const.EncounterLevelScalingExponentOuter);
            }

            // Home planet is a nursery zone
            if (this == StaticData.Races[0].HomePlanet) dLevel -= 1.0 + rand.NextDouble();

            // Increase the difficulty based on where we are within this system
            if (this is HabitableAO hao) {
                Planet? pl = null;
                if (hao is Planet pla) pl = pla;
                else if (hao is Moon mn) pl = mn.Parent;
                if (pl != null) {
                    // Planet/moon without local colony -> dangerous         
                    if (hao.Colony == null && pl.Colony == null) dLevel += rand.NextDouble();
                    // More distant planets can be more hostile
                    double pDist = Math.Sqrt(pl.ID);
                    dLevel += pDist * (rand.NextDouble() + 1d) / 2d;
                }
            }

            // This is a home system so reduce danger level
            Star sys = GetSystem();
            foreach (Race r in StaticData.Races) {
                if (sys == r.HomePlanet.GetSystem()) dLevel -= rand.NextDouble() + rand.NextDouble();

            }

            int iLevel = Math.Max(1, (int)dLevel);
            return iLevel;
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
            return AOType switch {
                AstronomicalObjectType.Star => str + ":" + ID,
                AstronomicalObjectType.Planet => str + ":" + GetSystem().ID + ":" + ID,
                AstronomicalObjectType.Moon => str + ":" + GetSystem().ID + ":" + ((Moon)this).Parent.ID + "." + ID,
                _ => throw new NotImplementedException(),
            };
        }
    }
}
