using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;

namespace SpaceMercs.MainWindow {
  // Partial class including functions for drawing the full galaxymap view
  partial class MapView {
    private double fMapViewZ, fMapViewX, fMapViewY;
    private bool bShowGridlines, bFadeUnvisited, bShowRangeCircles, bShowTradeRoutes, bShowFlags;
    private int iMiscTexture = -1, iBuildingTexture = -1;

    // Build texture maps for the stars' radial brightness maps
    private void SetupMapTextures() {
      double D = -0.7, D2 = -0.6;
      double RMin = 0.05;
      double RScale = Math.Pow(RMin, D) - 1.0;
      double RScale2 = Math.Pow(RMin, D2) - 1.0;
      Textures.iStarTexture = GL.GenTexture();
      Textures.iShipHalo = GL.GenTexture();
      Textures.byteStarTexture = new byte[Textures.StarTextureSize * Textures.StarTextureSize * 3];
      Textures.byteShipHaloTexture = new byte[Textures.ShipHaloTextureSize * Textures.ShipHaloTextureSize * 3];
      for (int y = 0; y < Textures.StarTextureSize; y++) {
        double dy = ((double)Textures.StarTextureSize / 2.0) - ((double)y + 0.5);
        for (int x = 0; x < Textures.StarTextureSize; x++) {
          double dx = ((double)Textures.StarTextureSize / 2.0) - ((double)x + 0.5);
          double r = Math.Pow((dx * dx) + (dy * dy), 0.5) * 2.0 / (double)Textures.StarTextureSize;
          double r2 = r * 0.8;
          byte val, val2;
          if (r > 1.0) val = 0;
          else if (r < RMin) val = 255;
          else val = (byte)((Math.Pow(r, D) - 1.0) * 255.0 / RScale);
          if (r2 > 1.0) val2 = 0;
          else if (r2 < RMin) val2 = 255;
          else val2 = (byte)((Math.Pow(r2, D2) - 1.0) * 255.0 / RScale2);
          if (val2 > 150) val2 = 150;
          Textures.byteStarTexture[((y * Textures.StarTextureSize) + x) * 3 + 0] = val;
          Textures.byteStarTexture[((y * Textures.StarTextureSize) + x) * 3 + 1] = val;
          Textures.byteStarTexture[((y * Textures.StarTextureSize) + x) * 3 + 2] = val;
          Textures.byteShipHaloTexture[((y * Textures.ShipHaloTextureSize) + x) * 3 + 0] = val2;
          Textures.byteShipHaloTexture[((y * Textures.ShipHaloTextureSize) + x) * 3 + 1] = val2;
          Textures.byteShipHaloTexture[((y * Textures.ShipHaloTextureSize) + x) * 3 + 2] = val2;
        }
      }
      iMiscTexture = Textures.GetMiscTexture();
      iBuildingTexture = Textures.GetBuildingTexture();
    }

    // Display the galaxy map on the screen
    private void DrawMap() {
       // What are the extents of the map that we can show here?
      int cx = (int)Math.Floor(fMapViewX / (double)(Const.SectorSize));
      int cy = (int)Math.Floor(fMapViewY / (double)(Const.SectorSize));
      int cwx = (int)Math.Floor(fMapViewZ / (double)(Const.SectorSize) * 2.5);
      int cwy = (int)Math.Floor(fMapViewZ / (double)(Const.SectorSize) * 3.5);
      MinSectorX = cx - cwx;
      MaxSectorX = cx + cwx;
      MinSectorY = cy - cwy;
      MaxSectorY = cy + cwy;

      //-- Display the scene

      // Set the correct view location & perspective matrix
      GL.MatrixMode(MatrixMode.Projection);
      GL.LoadMatrix(ref perspective);
      GL.ClearColor(Color.Black);
      GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
      GL.MatrixMode(MatrixMode.Modelview);
      GL.LoadIdentity();
      GL.Translate(-fMapViewX, -fMapViewY, -fMapViewZ);

      // Display the map
      if (bShowGridlines) DrawMapGrid();
      if (bShowRangeCircles) DrawRangeCircles();
      if (bShowTradeRoutes) DrawTradeRoutes();
      DrawStars();
      DrawMapHoverLink();
      DrawMapSelectionIcons();
      SetupGUIHoverInfo();
      DrawGUI();
    }

    // Highlight aoSelected and aoHover, if they exist; Highlight current location
    private void DrawMapSelectionIcons() {
      if (aoHover != null) {
        GL.PushMatrix();
        GL.Translate(((Star)aoHover).MapPos);
        double StarScale = Const.MapStarScale * Math.Pow(aoHover.radius / Const.Million, 0.28) + 0.05;
        if (StarScale < Const.MapStarScale * 0.5) StarScale = Const.MapStarScale * 0.5;
        GraphicsFunctions.DrawHoverReticule(StarScale*1.1);
        GL.PopMatrix();
      }

      if (aoSelected != null) {
        GL.PushMatrix();
        GL.Translate(((Star)aoSelected).MapPos);
        double StarScale = Const.MapStarScale * Math.Pow(aoSelected.radius / Const.Million, 0.28) + 0.05;
        if (StarScale < Const.MapStarScale * 0.5) StarScale = Const.MapStarScale * 0.5;
        GraphicsFunctions.DrawSelectedReticule(StarScale);
        GL.PopMatrix();
      }

      {
        GL.PushMatrix();
        GL.Translate(CurrentSystem.MapPos);
        double StarScale = Const.MapStarScale * Math.Pow(CurrentSystem.radius / Const.Million, 0.28) + 0.05;
        if (StarScale < Const.MapStarScale * 0.5) StarScale = Const.MapStarScale * 0.5;
        GraphicsFunctions.DrawLocationIcon(StarScale);
        GL.PopMatrix();
      }
    }

    // If we have any trade routes between systems, show them here
    private void DrawTradeRoutes() {
      for (int sy = MinSectorY; sy <= MaxSectorY; sy++) {
        for (int sx = MinSectorX; sx <= MaxSectorX; sx++) {
          Tuple<int, int> tp = new Tuple<int, int>(sx, sy);
          Sector sc = GalaxyMap.GetSector(tp);
          sc.DrawTradeRoutes();
        }
      }
    }

    // Draw the stars on the map screen
    private void DrawStars() {
      if (GalaxyMap.bMapSetup == false) return;
      
      // Display all stars by sector
      SetupMapLighting();
      GL.Enable(EnableCap.Lighting);
      for (int sy = MinSectorY; sy <= MaxSectorY; sy++) {
        for (int sx = MinSectorX; sx <= MaxSectorX; sx++) {
          Tuple<int, int> tp = new Tuple<int, int>(sx, sy);
          Sector sc = GalaxyMap.GetSector(tp);
          sc.Draw(Context, bFadeUnvisited, bShowLabels, bShowFlags, fMapViewX, fMapViewY, fMapViewZ);
        }
      }
      GL.Disable(EnableCap.Lighting);

    }

    // Draw the grid for the map screen
    private void DrawMapGrid() {
      GL.DepthMask(false);
      GL.LineWidth(1.0f);
      GL.Color4(0.5, 0.5, 0.5, 0.7);

      // Draw the map grid
      GL.Begin(BeginMode.Lines);
      for (int x = MinSectorX; x <= MaxSectorX + 1; x++) {
        GL.Vertex2((x * Const.SectorSize) - (Const.SectorSize / 2), (MinSectorY * Const.SectorSize) - (Const.SectorSize / 2));
        GL.Vertex2((x * Const.SectorSize) - (Const.SectorSize / 2), (MaxSectorY * Const.SectorSize) + (Const.SectorSize / 2));
      }
      for (int y = MinSectorY; y <= MaxSectorY + 1; y++) {
        GL.Vertex2((MinSectorX * Const.SectorSize) - (Const.SectorSize / 2), (y * Const.SectorSize) - (Const.SectorSize / 2));
        GL.Vertex2((MaxSectorX * Const.SectorSize) + (Const.SectorSize / 2), (y * Const.SectorSize) - (Const.SectorSize / 2));
      }
      GL.End();

      // Reset OpenGL Parameters
      GL.DepthMask(true);
    }

    // Draw range circles from the current location
    private void DrawRangeCircles() {
      GL.PushMatrix();
      GL.Translate(CurrentSystem!.MapPos);
      GL.LineWidth(1.0f);
      for (int range = 2; range <= 14; range += 2) {
        double col = 0.8 - ((double)range / 20.0);
        GL.Color3(col,col,col);
        GL.PushMatrix();
        GL.Scale(range, range, range);
        GL.CallList(GraphicsFunctions.iOrbitDL);
        GL.PopMatrix();
      }
      GL.PopMatrix();
    }

    // Join hover star with selected star
    private void DrawMapHoverLink() {
      if (aoHover == null || aoSelected == null || aoHover == aoSelected) return;
      if (Control.ModifierKeys != Keys.Alt) return; // Only display if Alt is held down

      // Join selected and hover AOs if necessary
      GL.Disable(EnableCap.Blend);
      GL.Disable(EnableCap.Texture2D);
      if (PlayerTeam.PlayerShip.Range >= aoSelected.GetSystem().DistanceTo(aoHover.GetSystem())) GL.Color3(0.0, 1.0, 0.0);
      else GL.Color3(1.0, 0.0, 0.0);
      GL.Begin(BeginMode.Lines);
      GL.Vertex3(aoSelected.GetMapLocation());
      GL.Vertex3(aoHover.GetMapLocation());
      GL.End();
    }

    // Get the system under the mouse pointer
    private void MapHover() {
      AstronomicalObject aoHoverOld = aoHover;
      aoHover = null;
      if (GalaxyMap.bMapSetup == false) return;

      // Work out what we're hovering over (pick the closest one!)
      double mxfract = (double)mx / (double)Size.X;
      double myfract = (double)my / (double)Size.Y;
      double mxpos = ((mxfract - 0.5) * (fMapViewZ / 18.6) * (double)Const.SectorSize * Aspect) + fMapViewX;
      double mypos = ((0.5 - myfract) * (fMapViewZ / 18.6) * (double)Const.SectorSize) + fMapViewY;
      int sx = (int)Math.Floor((mxpos / (double)Const.SectorSize) + 0.5);
      int sy = (int)Math.Floor((mypos / (double)Const.SectorSize) + 0.5);
      Tuple<int, int> tp = new Tuple<int, int>(sx, sy);
      Sector sc = GalaxyMap.GetSector(tp);
      aoHover = sc.CheckHover(mxpos, mypos, fMapViewZ);
      // TODO if (aoHover != aoHoverOld) glMapView.Invalidate();
    }

    // Configure and locate the light from the star
    private void SetupMapLighting() {
      GL.Enable(EnableCap.Normalize);
      // Setup parallel light source
      GL.Light(LightName.Light0, LightParameter.Position, new float[] { 100000.0f, 100000.0f, 10000.0f, 1.0f });
      GL.Light(LightName.Light0, LightParameter.Ambient, new float[] { 0.3f, 0.3f, 0.3f, 1.0f });
      GL.Light(LightName.Light0, LightParameter.Diffuse, new float[] { 0.8f, 0.8f, 0.8f, 1.0f });
      GL.Light(LightName.Light0, LightParameter.Specular, new float[] { 0.0f, 0.0f, 0.0f, 1.0f });
      GL.Enable(EnableCap.Light0);

      // Material properties
      GL.Material(MaterialFace.Front, MaterialParameter.Specular, new float[] { 0.1f, 0.1f, 0.1f, 1.0f });
      GL.Material(MaterialFace.Front, MaterialParameter.Ambient, new float[] { 0.5f, 0.5f, 0.5f, 1.0f });
      GL.Material(MaterialFace.Front, MaterialParameter.Diffuse, new float[] { 1.0f, 1.0f, 1.0f, 1.0f });

      // Global ambient light level
      GL.LightModel(LightModelParameter.LightModelAmbient, 0.25f);

      // Set colouring
      GL.Enable(EnableCap.ColorMaterial);
      GL.ColorMaterial(MaterialFace.Front, ColorMaterialParameter.AmbientAndDiffuse);
    }

  }
}
