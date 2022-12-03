using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using SpaceMercs.Graphics;
using System.ComponentModel;
using System.IO;
using System.Xml;

namespace SpaceMercs {
  class Planet : HabitableAO {
    [Flags]
    public enum PlanetType { Rocky = 0x1, Desert = 0x2, Volcanic = 0x4, Gas = 0x8, Oceanic = 0x10, Ice = 0x20, Star = 0x40 };
    public Star Parent { get; set; }
    private readonly List<Moon> Moons;
    public double tempbase;
    private readonly BackgroundWorker bw;
    private bool bDrawing = false, bGenerating = false;
    public override double DrawScale { get { return Math.Pow(radius / 1000.0, 0.4) / 25.0; } }

    public Planet(int _seed) {
      Moons = new List<Moon>();
      _MissionList = null;
      strName = "";
      Seed = _seed;
      Random rnd = new Random(Seed);
      Ox = rnd.Next(Const.SeedBuffer);
      Oy = rnd.Next(Const.SeedBuffer);
      Oz = rnd.Next(Const.SeedBuffer);
      bw = new BackgroundWorker();
      bw.DoWork += new DoWorkEventHandler(bw_DoWork);
      bw.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bw_RunWorkerCompleted);
    }
    public Planet(XmlNode xml, Star parent) {
      Parent = parent;
      // Load this planet from the given Xml node
      // Start with generic AO stuff
      LoadAODetailsFromFile(xml);

      XmlNode xmlc = xml.SelectSingleNode("Colony");
      if (xmlc != null) SetColony(new Colony(xmlc, this));

      // Load planet-specific stuff
      tempbase = double.Parse(xml.SelectSingleNode("TempBase").InnerText);
      Type = (Planet.PlanetType)Enum.Parse(typeof(Planet.PlanetType), xml.SelectSingleNode("Type").InnerText);

      Moons = new List<Moon>();
      XmlNode xmlMoons = xml.SelectSingleNode("Moons");
      if (xmlMoons != null) {
        foreach (XmlNode xmlm in xmlMoons.ChildNodes) {
          Moon mn = new Moon(xmlm, this);
          mn.Parent = this;
          Moons.Add(mn);
        }
      }
      colour = Const.PlanetTypeToCol1(Type);

      LoadMissions(xml);

      // Trigger the terrain generation
      bw = new BackgroundWorker();
      bw.DoWork += new DoWorkEventHandler(bw_DoWork);
      bw.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bw_RunWorkerCompleted);
    }

    // Save this planet to an Xml file
    public void SaveToFile(StreamWriter file) {
      file.WriteLine("<Planet ID=\"" + ID.ToString() + "\">");
      // Write generic AO details
      WriteAODetailsToFile(file);
      if (Colony != null) Colony.SaveToFile(file);
      // Write planet details to file
      file.WriteLine("<TempBase>" + tempbase.ToString() + "</TempBase>");
      file.WriteLine("<Type>" + Type.ToString() + "</Type>");
      SaveMissions(file);

      // Now write out all planets
      file.WriteLine("<Moons>");
      foreach (Moon mn in Moons) {
        mn.SaveToFile(file);
      }
      file.WriteLine("</Moons>");
      file.WriteLine("</Planet>");
    }

    // Retrieve a moon from this system by ID
    public Moon GetMoonByID(int ID) {
      if (ID < 0 || ID >= Moons.Count) return null;
      return Moons[ID];
    }

    // Draw this planet plus moons on the system view
    public void DrawSystem(ShaderProgram prog, AstronomicalObject aoSelected, AstronomicalObject aoHover, AstronomicalObject aoCurrentPosition, bool bShowLabels, bool bShowColonies) {
      if (radius > 7 * Const.Million) DrawSelected(prog, 7);
      else DrawSelected(prog, 6);
      DrawHalo();
      if (bShowLabels) DrawNameLabel();
      if (aoHover == this) GraphicsFunctions.DrawHoverReticule(DrawScale * 1.1);
      if (aoSelected == this) GraphicsFunctions.DrawSelectedReticule(DrawScale * 1.1);
      if (bShowColonies) DrawBaseIcon();
      if (aoCurrentPosition == this) GraphicsFunctions.DrawLocationIcon(DrawScale * 1.0);

      GL.Translate(0.0, Const.MoonGap, 0.0);
      GL.Scale(Const.MoonScale, Const.MoonScale, Const.MoonScale); // Make moons noticeably smaller than planets
      foreach (Moon mn in Moons) {
        if (bShowColonies) mn.DrawBaseIcon();
        mn.DrawSelected(prog, 5);
        if (aoHover == mn) GraphicsFunctions.DrawHoverReticule(mn.DrawScale * 1.2);
        if (aoSelected == mn) GraphicsFunctions.DrawSelectedReticule(mn.DrawScale * 1.2);
        if (aoCurrentPosition == mn) GraphicsFunctions.DrawLocationIcon(mn.DrawScale * 1.25);
        GL.Translate(0.0, 4.5, 0.0);
      }
    }

    // Are we hovering over anything here?
    public AstronomicalObject GetHover(double mousex, double mousey, double px, double py) {
      py += Const.MoonGap * Const.PlanetScale;
      foreach (Moon mn in Moons) {
        if (Math.Abs(px - mousex) < (mn.DrawScale * Const.PlanetScale * Const.MoonScale * Const.SystemViewSelectionTolerance) && Math.Abs(py - mousey) < (mn.DrawScale * Const.PlanetScale * Const.MoonScale * Const.SystemViewSelectionTolerance)) return mn;
        py += (4.5 * Const.PlanetScale * Const.MoonScale);
      }
      return null;
    }

    // Draw the planet's name label
    private void DrawNameLabel() {
      if (Name == null || Name.Length == 0) return;
      SetName(Name);
      GL.PushMatrix();
      GL.Disable(EnableCap.Lighting);
      GL.Color3(1.0, 1.0, 1.0);
      double dskip = (Colony != null) ? 0.7 : 0.06;
      if ((ID & 1) == 0) GL.Translate(0.0, -(DrawScale * 1.06) - dskip, 0.0);
      else GL.Translate(0.0, (DrawScale * 1.06) + 0.06, 0.0);
      GL.Scale(1.7, 1.7, 1.7);
      GL.Rotate(180.0, Vector3d.UnitX);
      //if ((ID & 1) == 0) tlName.Draw(TextLabel.Alignment.BottomMiddle);
      //else tlName.Draw(TextLabel.Alignment.TopMiddle);
      GL.PopMatrix();
    }

    // Build texture maps for the planets' radial brightness maps
    public static void BuildPlanetHalo() {
      double RMin = 0.8;
      Textures.iPlanetHalo = GL.GenTexture();
      Textures.bytePlanetHalo = new byte[Textures.PlanetHaloTextureSize * Textures.PlanetHaloTextureSize * 3];
      for (int y = 0; y < Textures.PlanetHaloTextureSize; y++) {
        double dy = ((double)Textures.PlanetHaloTextureSize / 2.0) - ((double)y + 0.5);
        for (int x = 0; x < Textures.PlanetHaloTextureSize; x++) {
          double dx = ((double)Textures.PlanetHaloTextureSize / 2.0) - ((double)x + 0.5);
          double r = Math.Pow((dx * dx) + (dy * dy), 0.5) * 2.0 / (double)Textures.PlanetHaloTextureSize;
          byte val;
          if (r > 1.0) val = 0;
          else if (r < RMin) val = 255;
          else val = (byte)((1.0 - ((r - RMin) / (1.0 - RMin))) * 255.0);
          Textures.bytePlanetHalo[((y * Textures.PlanetHaloTextureSize) + x) * 3 + 0] = val;
          Textures.bytePlanetHalo[((y * Textures.PlanetHaloTextureSize) + x) * 3 + 1] = val;
          Textures.bytePlanetHalo[((y * Textures.PlanetHaloTextureSize) + x) * 3 + 2] = val;
        }
      }
    }

    // Draw a halo-type atmosphere effect round a planet
    public void DrawHalo() {
      if (Type == PlanetType.Gas) return;
      GL.DepthMask(false);
      GL.Disable(EnableCap.Lighting);
      GL.Enable(EnableCap.Blend);
      GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.One);
      GL.Enable(EnableCap.Texture2D);
      GL.BindTexture(TextureTarget.Texture2D, Textures.iPlanetHalo);
      GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgb, Textures.PlanetHaloTextureSize, Textures.PlanetHaloTextureSize, 0, PixelFormat.Rgb, PixelType.UnsignedByte, Textures.bytePlanetHalo);
      Textures.SetParameters();
      GL.TexEnv(TextureEnvTarget.TextureEnv, TextureEnvParameter.TextureEnvMode, (float)TextureEnvMode.Modulate);
      GL.PushMatrix();
      double pscale = DrawScale * 1.08;
      if (Type == PlanetType.Oceanic) pscale *= 1.1;
      if (Type == PlanetType.Ice) pscale *= 1.05;
      GL.Scale(pscale, pscale, pscale);
      GL.Color3(1.0, 1.0, 1.0);
      
      // Draw the halo
      GL.Begin(BeginMode.Quads);
      GL.TexCoord2(0.0, 1.0);
      GL.Vertex3(-1.0, 1.0, 0.0);
      GL.TexCoord2(1.0, 1.0);
      GL.Vertex3(1.0, 1.0, 0.0);
      GL.TexCoord2(1.0, 0.0);
      GL.Vertex3(1.0, -1.0, 0.0);
      GL.TexCoord2(0.0, 0.0);
      GL.Vertex3(-1.0, -1.0, 0.0);
      GL.End();
      GL.PopMatrix();
      GL.Disable(EnableCap.Texture2D);
      GL.Disable(EnableCap.Blend);
      GL.DepthMask(true);
    }

    // Generate the moons for this planet
    public void GenerateMoons(Random rnd, int pdensity, int minMoons = 0) {
      Moons.Clear();
      // Get number of moons, based on planet density setting and planet size
      int nmn = 0;
      nmn = rnd.Next(pdensity + 1) + rnd.Next(pdensity + 1) + rnd.Next(pdensity + 1) - 3;
      if (Temperature > 350) nmn -= rnd.Next(2);
      if (Temperature > 400) nmn -= rnd.Next(2);
      if (Temperature > 450) nmn -= rnd.Next(2);
      if (Temperature > 500) nmn -= rnd.Next(2);
      if (Temperature > 550) nmn -= rnd.Next(2);
      if (Temperature > 600) nmn -= rnd.Next(2);
      if (Temperature < 170) nmn -= rnd.Next(2);
      if (Temperature < 150) nmn -= rnd.Next(2);
      if (Temperature < 130) nmn -= rnd.Next(2);
      if (Temperature < 110) nmn -= rnd.Next(2);
      if (radius < 4 * Const.Million) nmn -= rnd.Next(2) + 1;
      if (radius < 3.5 * Const.Million) nmn -= rnd.Next(2) + 1;
      if (radius < 3 * Const.Million) nmn -= rnd.Next(2) + 1;
      if (radius < 2.5*Const.Million) nmn -= rnd.Next(2) + 1;
      if (radius < 2 * Const.Million) nmn -= rnd.Next(2) + 1;
      if (Type != PlanetType.Gas) nmn /= 3;
      if (nmn > 8) nmn = 8 + rnd.Next(2);
      if (nmn < minMoons) nmn = minMoons;

      // Generate moons
      for (int n = 0; n < nmn; n++) {
        Moon mn = new Moon(rnd.Next(10000000));
        mn.ID = n;

        do {
          mn.radius = Utils.NextGaussian(rnd, Const.MoonRadius, Const.MoonRadiusSigma);
        } while (mn.radius < Const.MoonRadiusMin);

        mn.orbit = Utils.NextGaussian(rnd, Const.MoonOrbit * (double)(n + 1), Const.MoonOrbitSigma);
        mn.orbit += radius;
        mn.Parent = this;
        bool bOK = true;
        do {
          mn.Temperature = Temperature - 40; // Base = planet's temperature minus 40 degrees
          double tempmod = 0.0;
          bOK = true;
          if (mn.Temperature > 180 && mn.Temperature < 320 && rnd.Next(4) == 0) { mn.Type = PlanetType.Oceanic; tempmod = Utils.NextGaussian(rnd, 40, 5); }
          else {
            int r = rnd.Next(25);
            if (r < 2) { mn.Type = PlanetType.Oceanic; tempmod = Utils.NextGaussian(rnd, 40, 5); }
            else if (r < 5) { mn.Type = PlanetType.Desert; }
            else if (r < 9) { mn.Type = PlanetType.Volcanic; tempmod = Utils.NextGaussian(rnd, 40, 5); }
            else { mn.Type = PlanetType.Rocky; tempmod = Utils.NextGaussian(rnd, 5, 5); }
          }
          mn.Temperature += (int)tempmod;

          // Check that this is ok
          if (mn.Temperature > 320 && mn.Type == PlanetType.Oceanic) bOK = false;
          if (mn.Temperature < 210 && mn.Type == PlanetType.Oceanic) bOK = false;
          if (mn.Temperature < 270 && mn.Type == PlanetType.Oceanic) mn.Type = PlanetType.Ice;
          if (mn.Temperature < 160 && mn.Type == PlanetType.Volcanic) bOK = false;
        } while (bOK != true);

        mn.colour = Const.PlanetTypeToCol1(mn.Type);

        // Rotational/orbital parameters
        double pmass; // Rough estimate of planet's mass / 10^18
        pmass = Math.Pow((radius / Const.EarthRadius), 3.0) * 6000000.0;
        if (Type == PlanetType.Gas) pmass /= 4.0;

        Moons.Add(mn);
      }
    }

    // Set up more detailed texture maps if necessary
    // If this is a large one then do it in a thread so we don't freeze the main GUI
    private void bw_DoWork(object sender, DoWorkEventArgs e) {
      int width = ((KeyValuePair<int, int>)e.Argument).Key;
      int height = ((KeyValuePair<int, int>)e.Argument).Value;
      TexResult tr = new TexResult();
      tr.width = width;
      tr.height = height;
      tr.texture = Terrain.GenerateMap(this, width, height);
      e.Result = tr;
    }
    private void bw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e) {
      TexResult tr = (TexResult)e.Result;
      while (bDrawing);
      texture = tr.texture;
      GL.BindTexture(TextureTarget.Texture2D, iTexture);
      GL.TexEnv(TextureEnvTarget.TextureEnv, TextureEnvParameter.TextureEnvMode, (float)TextureEnvMode.Modulate);
      GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgb, tr.width, tr.height, 0, PixelFormat.Rgb, PixelType.UnsignedByte, texture);
      Textures.SetParameters();
      bGenerating = false;
    }

    // Randomly expand a moon base
    public int ExpandMoonBase(Random rand, Race rc) {
      if (Moons.Count == 0) return 0;
      int mno = rand.Next(Moons.Count);
      Moon mn = Moons[mno];
      if (mn.BaseSize == 4) return 0;
      double tdiff = mn.TDiff(rc);
      if (rand.NextDouble() * 100.0 > tdiff) {
        return mn.ExpandBase(rc, rand);
      }
      return 0;
    }
    public void CheckGrowth() {
      if (Colony != null) Colony.CheckGrowth();
      foreach (Moon mn in Moons) {
        if (mn.Colony != null) mn.Colony.CheckGrowth();
      }
    }

    // Overrides
    public override AstronomicalObjectType AOType { get { return AstronomicalObjectType.Planet; } }
    public override void DrawBaseIcon() {
      if (BaseSize == 0) return;
      GL.Disable(EnableCap.DepthTest);
      GL.PushMatrix();
      // Opposite the name tag
      //GL.Translate(0.0, (DrawScale * 1.3) + 0.2, 0.0); // Below planet icon
      GL.Translate(0.0, -(DrawScale * 1.3) - 0.2, 0.0); // Above planet icon
      GL.Scale(1.0, 1.0, 1.0);
      GL.Translate(-((BaseSize - 1) * 0.7) / 2.0, 0.0, 0.0);
      if ((Base & Colony.BaseType.Outpost) != 0) {
        GL.Color3(1.0, 1.0, 0.0);
        GraphicsFunctions.DrawRhomboid(0.65);
        GL.Translate(0.7, 0.0, 0.0);
      }
      if ((Base & Colony.BaseType.Colony) != 0) {
        GL.Color3(0.0, 1.0, 0.0);
        GraphicsFunctions.DrawRhomboid(0.65);
        GL.Translate(0.7, 0.0, 0.0);
      }
      if ((Base & Colony.BaseType.Trading) != 0) {
        GL.Color3(0.0, 1.0, 1.0);
        GraphicsFunctions.DrawRhomboid(0.65);
        GL.Translate(0.7, 0.0, 0.0);
      }
      if ((Base & Colony.BaseType.Research) != 0) {
        GL.Color3(0.0, 0.0, 1.0);
        GraphicsFunctions.DrawRhomboid(0.65);
        GL.Translate(0.7, 0.0, 0.0);
      }
      if ((Base & Colony.BaseType.Military) != 0) {
        GL.Color3(1.0, 0.0, 0.0);
        GraphicsFunctions.DrawRhomboid(0.65);
        GL.Translate(0.7, 0.0, 0.0);
      }
      if ((Base & Colony.BaseType.Metropolis) != 0) {
        GL.Color3(1.0, 1.0, 1.0);
        GraphicsFunctions.DrawRhomboid(0.65);
        GL.Translate(0.7, 0.0, 0.0);
      }
      // Frame
      GL.Color3(1.0, 1.0, 1.0);
      GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line);
      GL.Begin(BeginMode.Quads);
      GL.Vertex3(-0.3 - (BaseSize * 0.7), -0.4, 0.0);
      GL.Vertex3(-0.4, -0.4, 0.0);
      GL.Vertex3(-0.4, 0.4, 0.0);
      GL.Vertex3(-0.3 - (BaseSize * 0.7), 0.4, 0.0);
      GL.Vertex3(-0.3 - (BaseSize * 0.7), -0.4, 0.0);
      GL.Vertex3(-0.4, -0.4, 0.0);
      GL.Vertex3(-0.4, 0.4, 0.0);
      GL.Vertex3(-0.3 - (BaseSize * 0.7), 0.4, 0.0);
      GL.End();
      GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);
      GL.PopMatrix();
      GL.Enable(EnableCap.DepthTest);
    }
    public override void DrawSelected(ShaderProgram prog, int Level = 7) {
      // Draw this planet
      SetupTextureMap(64, 32);
      GL.PushMatrix();
      GL.Enable(EnableCap.Texture2D);
      GL.BindTexture(TextureTarget.Texture2D, iTexture);
      GL.Color3(1.0, 1.0, 1.0);
      double pscale = DrawScale;
      GL.Scale(pscale, pscale, pscale);
      GL.Rotate(90.0, Vector3d.UnitY);
      //GL.Rotate(Const.dSeconds * 360.0 / prot, Vector3d.UnitZ);
      //GraphicsFunctions.sphere(Level).Draw();
      GL.Disable(EnableCap.Texture2D);
      GL.PopMatrix();
    }
    public override void SetupTextureMap(int width, int height) {
      if (iTexture == -1 || texture == null) iTexture = GL.GenTexture();
      else {
        if (texture.Length >= (width * height * 3)) return;
      }
      if (width < 1000) {
        texture = Terrain.GenerateMap(this, width, height);
        GL.BindTexture(TextureTarget.Texture2D, iTexture);
        GL.TexEnv(TextureEnvTarget.TextureEnv, TextureEnvParameter.TextureEnvMode, (float)TextureEnvMode.Modulate);
        GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgb, width, height, 0, PixelFormat.Rgb, PixelType.UnsignedByte, texture);
        Textures.SetParameters();
      }
      else {
        if (!bGenerating) {
          bGenerating = true;
          bw.RunWorkerAsync(new KeyValuePair<int, int>(width, height));
        }
      }
    }
    public override void ClearData() {
      GL.DeleteTexture(iTexture);
      iTexture = -1;
      texture = null;
      foreach (Moon mn in Moons) {
        mn.ClearData();
      }
    }
    public override void SetName(string str) {
      strName = str;
    }
    public override Star GetSystem() {
      return Parent;
    }
    public override string PrintCoordinates() {
      return Parent.PrintCoordinates() + "." + ID;
    }  

    struct TexResult {
      public int width;
      public int height;
      public byte[] texture;
    }
  }

}
