using System;
using System.Collections.Generic;
using OpenTK.Graphics.OpenGL;
using System.IO;
using System.Xml;
using OpenTK.Windowing.Common;

namespace SpaceMercs {
  class Sector {
    private readonly List<Star> Stars = new List<Star>();
    public Race Inhabitant = null;
    public int SectorX { get; private set; }
    public int SectorY { get; private set; }
    public Map ParentMap { get; private set; }

    public Sector(int sx, int sy, Map map) {
      SectorX = sx;
      SectorY = sy;
      ParentMap = map;
      int seed = map.MapSeed ^ ((sx * 85091) + (sy * 29527)) ^ ((sx * 34501) + (sy * 61819)); // Non-random seed; repeatable
      Random rand = new Random(seed);
      // Setup the stars in this sector
      double yoffset = (((sy * 2) - 1) * Const.SectorSize) / 2.0;
      double xoffset = (((sx * 2) - 1) * Const.SectorSize) / 2.0;
      for (int sno = 0; sno < map.StarsPerSector; sno++) {
        double X, Y;
        do {
          // Create a star somewhere in the sector, but avoid the edges (so we don't end up overlapping with other stars in other sectors.)
          // Note we can't really just check the neighbouring sectors, because then the details of this sector would depend on which other sectors were built first, which would make this unrepeatable.
          X = rand.NextDouble() * ((double)Const.SectorSize * 0.96) + xoffset + ((double)Const.SectorSize * 0.02);
          Y = rand.NextDouble() * ((double)Const.SectorSize * 0.96) + yoffset + ((double)Const.SectorSize * 0.02);
        } while (CheckProximity(X, Y));
        Star st = new Star(X, Y, rand.Next(10000000), map.PlanetDensity, this);
        st.ID = sno;
        st.Generate();
        Stars.Add(st);
      }
      // Done
    }
    public Sector(XmlNode xml, Map map) {
      SectorX = Int32.Parse(xml.Attributes["X"].Value);
      SectorY = Int32.Parse(xml.Attributes["Y"].Value);
      ParentMap = map;
      Inhabitant = StaticData.GetRaceByName(xml.Attributes["Inhabitant"].Value);

      foreach (XmlNode xmls in xml.ChildNodes) {
        Star st = new Star(xmls, this);
        Stars.Add(st);
      }
    }

    // Save this sector to an Xml file
    public void SaveToFile(StreamWriter file) {
      file.WriteLine(" <Sector X=\"" + SectorX + "\" Y=\"" + SectorY + "\" Inhabitant=\"" + ((Inhabitant == null) ? "" : Inhabitant.Name) + "\">");
      foreach (Star st in Stars) {
        st.SaveToFile(file);
      }
      file.WriteLine(" </Sector>");
    }

    // Is this star too close to another? Note that we need to check neighbouring sectors, too...
    private bool CheckProximity(double x, double y) {
      double MD2 = Const.MinStarDistance * Const.MinStarDistance;
      foreach (Star st in Stars) {
        double dx = st.MapPos.X - x;
        double dy = st.MapPos.Y - y;
        double r2 = (dx * dx) + (dy * dy);
        if (r2 < MD2) return true;
      }
      return false;
    }

    public void Draw(IGraphicsContext currentContext, bool bFadeUnvisited, bool bShowLabels, bool bShowFlags, double fMapViewX, double fMapViewY, double fMapViewZ) {
      foreach (Star st in Stars) {
        // Rotate into the star frame
        GL.PushMatrix();
        GL.Translate(st.MapPos);

        // Display star 
        double StarScale = st.DrawScale * 0.1;

        // Fade out unvisited stars
        if (bFadeUnvisited && !st.Visited) GL.Color3(st.colour.X / 3.0, st.colour.Y / 3.0, st.colour.Z / 3.0);
        else GL.Color3(st.colour.X, st.colour.Y, st.colour.Z);

        // Work out the degree of detail to show in this star
        double dx = fMapViewX - st.MapPos.X;
        double dy = fMapViewY - st.MapPos.Y;
        double dist2 = ((dx * dx) + (dy * dy) + (fMapViewZ * fMapViewZ));
        double DetailScale = Math.Sqrt(dist2) / StarScale;
        int iLevel = 1;
        if (DetailScale < 25.0) iLevel = 7;
        else if (DetailScale < 40.0) iLevel = 6;
        else if (DetailScale < 80.0) iLevel = 5;
        else if (DetailScale < 150.0) iLevel = 4;
        else if (DetailScale < 300.0) iLevel = 3;
        else if (DetailScale < 600.0) iLevel = 2;
        else iLevel = 1;

        // Display the star. If close and not faded then show the texture
        if ((!bFadeUnvisited || st.Visited) && iLevel >= 4) {
          GL.PushMatrix();
          GL.Scale(0.1, 0.1, 0.1);
          st.DrawSelected(currentContext, iLevel);
          GL.PopMatrix();
        }
        else {
          GL.PushMatrix();
          GL.Scale(StarScale, StarScale, StarScale);
          GraphicsFunctions.spheres[iLevel].Draw(currentContext);
          GL.PopMatrix();
        }

        // Test the LOD code
        //if (iLevel == 1) GL.Color3(1.0, 0.0, 0.0);
        //if (iLevel == 2) GL.Color3(0.0, 1.0, 0.0);
        //if (iLevel == 3) GL.Color3(0.0, 0.0, 1.0);
        //if (iLevel == 4) GL.Color3(1.0, 1.0, 0.0);
        //if (iLevel == 5) GL.Color3(0.0, 1.0, 1.0);
        //if (iLevel == 6) GL.Color3(1.0, 0.0, 1.0);
        //if (iLevel == 7) GL.Color3(1.0, 1.0, 1.0);
        //GL.PushMatrix();
        //GL.Scale(StarScale * 10.0, StarScale * 10.0, StarScale * 10.0);
        //GraphicsFunctions.spheres[iLevel].Draw();
        //GL.PopMatrix();

        // Draw the name label
        if (bShowLabels && st.Visited && !String.IsNullOrEmpty(st.Name)) {
          GL.PushMatrix();
          GL.Translate(0.0, -(StarScale + 0.02), 0.01);
          double scale = 0.02 * fMapViewZ;
          GL.Scale(scale, scale, scale);
          st.DrawName();
          GL.PopMatrix();
        }

        // Display whether this system has been colonised with a flag
        if (bShowFlags && st.Visited && st.Owner != null) {
          GL.PushMatrix();
          GL.Translate(0.0, StarScale, 0.01);
          double scale = 0.01 * fMapViewZ;
          GL.Scale(scale, scale, scale);
          GL.Color3(st.Owner.Colour);
          GL.Begin(BeginMode.Quads);
          GL.Vertex3(0.0, 0.5, 0.0);
          GL.Vertex3(0.7, 0.5, 0.0);
          GL.Vertex3(0.7, 1.0, 0.0);
          GL.Vertex3(0.0, 1.0, 0.0);
          GL.End();
          GL.Color3(0.7, 0.45, 0.2);
          GL.Begin(BeginMode.Lines);
          GL.Vertex3(0.0, 0.0, 0.0);
          GL.Vertex3(0.0, 1.0, 0.0);
          GL.End();
          GL.PopMatrix();
        }

        GL.PopMatrix();
      }
    }

    public Star CheckHover(double x, double y, double fMapViewZ) {
      foreach (Star st in Stars) {
        double dx = x - st.MapPos.X;
        double dy = y - st.MapPos.Y;
        double d2 = (dx * dx) + (dy * dy);
        double StarScale = (st.DrawScale * 0.1) + 0.03; // Scale to our view units. Add a border to make this a bit more forgiving
        if (d2 < (StarScale * StarScale)) return st;
      }
      return null;
    }

    public Star GetMostCentralStar() {
      double AveX = 0.0, AveY = 0.0;
      foreach (Star st in Stars) {
        AveX += st.MapPos.X;
        AveY += st.MapPos.Y;
      }
      AveX /= Stars.Count;
      AveY /= Stars.Count;
      Star stClosest = null;
      double dClosest = 100000.0;
      foreach (Star st in Stars) {
        double dx = st.MapPos.X - AveX;
        double dy = st.MapPos.Y - AveY;
        double d2 = ((dx * dx) + (dy * dy));
        if (stClosest == null || d2 < dClosest) {
          dClosest = d2;
          stClosest = st;
        }
      }
      return stClosest;
    }

    public Star GetClosestNonColonisedSystemTo(Star stCentral) {
      Star stClosest = null;
      double dClosest = 100000.0;
      foreach (Star st in Stars) {
        if (st.Owner != null) continue;
        double dx = st.MapPos.X - stCentral.MapPos.X;
        double dy = st.MapPos.Y - stCentral.MapPos.Y;
        double d2 = ((dx * dx) + (dy * dy));
        if (stClosest == null || d2 < dClosest) {
          dClosest = d2;
          stClosest = st;
        }
      }
      return stClosest;
    }

    public void DrawTradeRoutes() {
      GL.Disable(EnableCap.Lighting);
      GL.LineWidth(2.0f);
      GL.Color3(0.6, 0.6, 0.6);
      GL.Enable(EnableCap.LineStipple);
      GL.LineStipple(4, 0x5555);
      GL.Begin(BeginMode.Lines);
      // Only draw them from left to right, in order not to draw each one twice
      foreach (Star st in Stars) {
        foreach (Star targ in st.TradeRoutes) {
          if (st.MapPos.X < targ.MapPos.X && (st.Visited || targ.Visited)) {
            GL.Color3(st.Owner.Colour);
            GL.Vertex2(st.MapPos.X, st.MapPos.Y);
            GL.Color3(targ.Owner.Colour);
            GL.Vertex2(targ.MapPos.X, targ.MapPos.Y);
          }
        }
      }
      GL.End();
      GL.Disable(EnableCap.LineStipple);
      GL.LineWidth(1.0f);
    }

    public Star GetStarByID(int id) {
      if (id < 0 || id >= Stars.Count) return null;
      return Stars[id];
    }

    public AstronomicalObject GetAOFromLocationString(string strLoc) {
      if (!strLoc.StartsWith("(") || !strLoc.Contains(")")) throw new Exception("Illegal location string:" + strLoc);
      string strMapLoc = strLoc.Substring(0, strLoc.IndexOf(")") + 1);
      string[] bits = strMapLoc.Replace("(", "").Replace(")", "").Split(',');
      if (bits.Length != 2) throw new Exception("Couldn't parse location string : " + strLoc + " - Sector location invalid : " + strMapLoc);
      int sX = Int32.Parse(bits[0]);
      int sY = Int32.Parse(bits[1]);
      if (sX != SectorX || sY != SectorY) return null;
      return GetAOFromLocationWithinSector(strLoc.Substring(strLoc.IndexOf(")") + 1));
    }
    public AstronomicalObject GetAOFromLocationWithinSector(string strAOID) {
      string[] bits = strAOID.Split('.');
      if (bits.Length == 0 || bits.Length > 3) throw new Exception("Illegal location within sector : \"" + strAOID + "\"");
      int sno = Int32.Parse(bits[0]);
      Star st = GetStarByID(sno);
      if (st == null || bits.Length == 1) return st;
      int pno = Int32.Parse(bits[1]);
      Planet pl = st.GetPlanetByID(pno);
      if (pl == null || bits.Length == 2) return pl;
      int mno = Int32.Parse(bits[2]);
      Moon mn = pl.GetMoonByID(mno);
      return mn;
    }

    public string PrintCoordinates() {
      return "(" + SectorX + "," + SectorY + ")";
    }
  }    
}
