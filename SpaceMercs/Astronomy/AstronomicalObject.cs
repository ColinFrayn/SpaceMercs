using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using System.IO;
using System.Xml;

namespace SpaceMercs {
  abstract class AstronomicalObject {
    public enum AstronomicalObjectType { Star, Planet, Moon, Unknown };
    protected string strName = "Unnamed";
    public string Name { get { return strName; } }
    public double radius; // In metres
    public double orbit; // In metres
    public int RotationPeriod; // Period of rotation (seconds)
    public int Temperature { get; set; } // Kelvin
    public Vector3 colour;
    public int Seed;
    public int Ox, Oy, Oz; // Perlin seed offsets
    protected byte[]? texture;
    protected int iTexture;
    public int ID;
    public abstract double DrawScale { get; }

    public AstronomicalObject() {
      iTexture = -1;
    }

    public virtual AstronomicalObjectType AOType { get { return AstronomicalObjectType.Unknown; } }
    public abstract string PrintCoordinates();
    public abstract void DrawSelected(IGraphicsContext currentContext, int Level);
    public abstract void SetupTextureMap(int width, int height);
    public abstract void ClearData();
    public abstract void SetName(string str);
    public abstract Star GetSystem();   // Parent Star (or self, if star)
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
        Planet pl1, pl2;
        if (ao1.AOType == AstronomicalObjectType.Planet) pl1 = (Planet)ao1;
        else pl1 = ((Moon)ao1).Parent;
        if (ao2.AOType == AstronomicalObjectType.Planet) pl2 = (Planet)ao2;
        else pl2 = ((Moon)ao2).Parent;
        if (pl1 == pl2) {
          if (ao1.AOType == AstronomicalObjectType.Moon) dist = ((Moon)ao1).orbit;
          if (ao2.AOType == AstronomicalObjectType.Moon) dist -= ((Moon)ao2).orbit;
          dist = Math.Abs(dist);
        }
        else {
          dist = Math.Abs(pl1.orbit - pl2.orbit);
          if (ao1.AOType == AstronomicalObjectType.Moon) dist += ((Moon)ao1).orbit;
          if (ao2.AOType == AstronomicalObjectType.Moon) dist += ((Moon)ao2).orbit;
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
      ID = Int32.Parse(xml.Attributes["ID"]?.InnerText);
      strName = "";
      strName = xml.SelectSingleNode("Name")?.InnerText;
      radius = double.Parse(xml.SelectSingleNode("Radius").InnerText);
      if (xml.SelectSingleNode("Orbit") != null) orbit = double.Parse(xml.SelectSingleNode("Orbit").InnerText);
      else orbit = 0;
      RotationPeriod = Int32.Parse(xml.SelectSingleNode("PRot").InnerText);
      Temperature = Int32.Parse(xml.SelectSingleNode("Temp").InnerText);
      Seed = Int32.Parse(xml.SelectSingleNode("Seed").InnerText);
      Random rnd = new Random(Seed);
      Ox = rnd.Next(Const.SeedBuffer);
      Oy = rnd.Next(Const.SeedBuffer);
      Oz = rnd.Next(Const.SeedBuffer);
    }

    // Save this planet to an Xml file
    protected void WriteAODetailsToFile(StreamWriter file) {
      if (!String.IsNullOrEmpty(strName)) file.WriteLine("<Name>" + strName + "</Name>");
      if (orbit != 0.0) file.WriteLine("<Orbit>" + Math.Round(orbit, 0).ToString() + "</Orbit>");
      file.WriteLine("<Radius>" + Math.Round(radius,0).ToString() + "</Radius>");
      file.WriteLine("<PRot>" + RotationPeriod.ToString() + "</PRot>");
      file.WriteLine("<Temp>" + Temperature.ToString() + "</Temp>");
      file.WriteLine("<Seed>" + Seed + "</Seed>");
    }

    // Get a random difficulty level for this location (e.g. for a mercenary or a mission)
    public int GetRandomMissionDifficulty(Random rand) {
      double dDist = AstronomicalObject.CalculateDistance(StaticData.Races[0].HomePlanet, this);
      double dDistLY = dDist / Const.LightYear;

      int iLevelBase = 1 + (int)Math.Sqrt(dDistLY / Const.EncounterLevelScalingDistance);
      if (dDist > 0.0) {
        iLevelBase++;  // Not the home planet (but may be a moon)
        if (this.GetSystem() != StaticData.Races[0].HomePlanet.GetSystem()) iLevelBase++;  // Not in home system, so more dangerous
        if (this is HabitableAO hao) {
          Planet pl = null;
          if (hao is Planet) pl = ((Planet)this);
          else if (hao is Moon) pl = ((Moon)this).Parent;
          if (pl != null) {
            if (hao.Colony == null && pl.Colony == null) iLevelBase++; // Planet/moon without local colony -> dangerous         
            if (rand.Next(10) + 2 < pl.ID) iLevelBase++; // Distant planets are more dangerous
          }
        }
      }
      int iLevel = iLevelBase;

      // Create a spread of levels
      double r = rand.NextDouble();
      if (r < 0.5) iLevel += (int)(rand.NextDouble() * 3.0);
      else if (r < 0.8) iLevel += (int)(rand.NextDouble() * 4.0);
      else iLevel += (int)(rand.NextDouble() * 5.0);

      return iLevel;
    }

    // Get a random race suitable for this location
    public Race GetRandomRace(Random rand) {
      if (this is HabitableAO hao) {
        if (hao.Colony != null) return hao.Colony.Owner;
      }
      Race rBest = null;
      double dBestScore = 0.0;
      foreach (Race r in StaticData.Races) {
        //if (!r.Known) continue;
        double dDist = AstronomicalObject.CalculateDistance(GetSystem(), r.HomePlanet.GetSystem()) / Const.LightYear;
        if (dDist < 0.2) dDist = 0.2; // Just being careful... Even in a home system, there is a tiny chance of getting aliens
        double dScore = (rand.NextDouble() + 0.1) / (dDist * dDist);
        if (GetSystem().Sector == r.HomePlanet.GetSystem().Sector) dScore *= 1.5; // More likely to encounter race if it's their home sector
        if (rBest == null || dScore > dBestScore) {
          dBestScore = dScore;
          rBest = r;
        }
      }
      return rBest;
    }

    // Location to string
    public override string ToString() {
      string str = "(" + GetSystem().Sector.SectorX.ToString() + "," + GetSystem().Sector.SectorY.ToString() + ")";
      switch (AOType) {
        case AstronomicalObjectType.Star: return str + ":" + ID;
        case AstronomicalObjectType.Planet: return str + ":" + GetSystem().ID + ":" + ID;
        case AstronomicalObjectType.Moon: return str + ":" + GetSystem().ID + ":" + ((Moon)this).Parent.ID + "." + ID;
      }
      throw new NotImplementedException();
    }

  }
}
