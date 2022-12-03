using OpenTK.Compute.OpenCL;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using SpaceMercs.Graphics;
using SpaceMercs.Graphics.Shapes;

namespace SpaceMercs.MainWindow {
    // Partial class including functions for drawing the full galaxymap view
    partial class MapView {
        private float fMapViewZ, fMapViewX, fMapViewY;
        private bool bShowGridlines, bFadeUnvisited, bShowRangeCircles, bShowTradeRoutes, bShowFlags;
        private int iMiscTexture = -1, iBuildingTexture = -1;
        VertexBuffer? mapLinesBuffer = null, hoverLinkBuffer = null;
        VertexArray? mapLinesArray = null, hoverLinkArray = null;
        private int lastMinX = -1, lastMinY = -1, lastMaxX = -1, lastMaxY = -1;

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
            Matrix4 projectionM = Matrix4.CreatePerspectiveFieldOfView(Const.MapViewportAngle, (float)Aspect, 0.05f, 5000.0f);
            flatColourShaderProgram.SetUniform("projection", projectionM);
            pos2DCol4ShaderProgram.SetUniform("projection", projectionM);

            Matrix4 translateM = Matrix4.CreateTranslation(fMapViewX, -fMapViewY, -fMapViewZ);
            Matrix4 viewM = translateM;
            flatColourShaderProgram.SetUniform("view", viewM);
            flatColourShaderProgram.SetUniform("model", Matrix4.Identity);
            pos2DCol4ShaderProgram.SetUniform("view", viewM);
            pos2DCol4ShaderProgram.SetUniform("model", Matrix4.Identity);

            GL.ClearColor(Color.Black);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            // Display the map
            if (bShowGridlines) DrawMapGrid();
            if (bShowRangeCircles) DrawRangeCircles();
            if (bShowTradeRoutes) DrawTradeRoutes();
            DrawStars();
            DrawMapHoverLink();
            DrawMapSelectionIcons();
            DrawGUI();
        }

        // Highlight aoSelected and aoHover, if they exist; Highlight current location
        private void DrawMapSelectionIcons() {
            if (aoHover != null) {
                Matrix4 translateM = Matrix4.CreateTranslation(((Star)aoHover).MapPos);
                double StarScale = Const.MapStarScale * Math.Pow(aoHover.radius / Const.Million, 0.28) + 0.05;
                if (StarScale < Const.MapStarScale * 0.5) StarScale = Const.MapStarScale * 0.5;
                Matrix4 scaleM = Matrix4.CreateScale((float)StarScale * 1.1f);
                Matrix4 modelM = scaleM * translateM;
                flatColourShaderProgram.SetUniform("model", modelM);
                flatColourShaderProgram.SetUniform("flatColour", new Vector4(1f,1f,1f,1f));
                GL.UseProgram(flatColourShaderProgram.ShaderProgramHandle);
                Annulus.Annulus16.Bind();
                Annulus.Annulus16.Draw();
                Annulus.Annulus16.Unbind();
            }

            if (aoSelected != null) {
                Matrix4 translateM = Matrix4.CreateTranslation(((Star)aoSelected).MapPos);
                double StarScale = Const.MapStarScale * Math.Pow(aoSelected.radius / Const.Million, 0.28) + 0.05;
                if (StarScale < Const.MapStarScale * 0.5) StarScale = Const.MapStarScale * 0.5;
                Matrix4 scaleM = Matrix4.CreateScale((float)StarScale);
                Matrix4 modelM = scaleM * translateM;
                flatColourShaderProgram.SetUniform("model", modelM);
                flatColourShaderProgram.SetUniform("flatColour", new Vector4(0.2f, 1f, 0.4f, 1f));
                GL.UseProgram(flatColourShaderProgram.ShaderProgramHandle);
                Annulus.Annulus16.Bind();
                Annulus.Annulus16.Draw();
                Annulus.Annulus16.Unbind();
            }

            {
                Matrix4 translateM = Matrix4.CreateTranslation(CurrentSystem.MapPos);
                double StarScale = Const.MapStarScale * Math.Pow(CurrentSystem.radius / Const.Million, 0.28) + 0.05;
                if (StarScale < Const.MapStarScale * 0.5) StarScale = Const.MapStarScale * 0.5;
                Matrix4 scaleM = Matrix4.CreateScale((float)StarScale);
                Matrix4 modelM = scaleM * translateM;
                flatColourShaderProgram.SetUniform("model", modelM);
                flatColourShaderProgram.SetUniform("flatColour", new Vector4(1f, 1f, 1f, 1f));
                GL.UseProgram(flatColourShaderProgram.ShaderProgramHandle);
                // TODO: What? A triangle?
                //GraphicsFunctions.DrawLocationIcon(StarScale);
            }
        }

        // If we have any trade routes between systems, show them here
        private void DrawTradeRoutes() {
            pos2DCol4ShaderProgram.SetUniform("colourFactor", 1f);

            for (int sy = MinSectorY; sy <= MaxSectorY; sy++) {
                for (int sx = MinSectorX; sx <= MaxSectorX; sx++) {
                    Tuple<int, int> tp = new Tuple<int, int>(sx, sy);
                    Sector sc = GalaxyMap.GetSector(tp);
                    sc.DrawTradeRoutes(pos2DCol4ShaderProgram);
                }
            }
        }

        // Draw the stars on the map screen
        private void DrawStars() {
            if (GalaxyMap.bMapSetup == false) return;
            return;

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
            flatColourShaderProgram.SetUniform("flatColour", new Vector4(0.5f, 0.5f, 0.5f, 0.7f));

            // If limits unchanged then don't recreate
            if (MinSectorX != lastMinX || MaxSectorX != lastMaxX || MinSectorY != lastMinY || MaxSectorY != lastMaxY || mapLinesBuffer is null) {
                List<VertexPos3D> vertices = new List<VertexPos3D>();
                // Draw the map grid
                for (int x = MinSectorX; x <= MaxSectorX + 1; x++) {
                    vertices.Add(new VertexPos3D(new Vector3((x * Const.SectorSize) - (Const.SectorSize / 2), (MinSectorY * Const.SectorSize) - (Const.SectorSize / 2), 0f)));
                    vertices.Add(new VertexPos3D(new Vector3((x * Const.SectorSize) - (Const.SectorSize / 2), (MaxSectorY * Const.SectorSize) + (Const.SectorSize / 2), 0f)));
                }
                for (int y = MinSectorY; y <= MaxSectorY + 1; y++) {
                    vertices.Add(new VertexPos3D(new Vector3((MinSectorX * Const.SectorSize) - (Const.SectorSize / 2), (y * Const.SectorSize) - (Const.SectorSize / 2), 0f)));
                    vertices.Add(new VertexPos3D(new Vector3((MaxSectorX * Const.SectorSize) + (Const.SectorSize / 2), (y * Const.SectorSize) - (Const.SectorSize / 2), 0f)));
                }

                // Create the buffers/arrays if not already done
                if (!vertices.Any()) return;
                if (mapLinesBuffer is null) mapLinesBuffer = new VertexBuffer(vertices.ToArray(), BufferUsageHint.DynamicDraw);
                else mapLinesBuffer.SetData(vertices.ToArray());

                // Store new values so we can check later if they change
                lastMinX = MinSectorX;
                lastMinY = MinSectorY;
                lastMaxX = MaxSectorX;
                lastMaxY = MaxSectorY;
            }
            if (mapLinesArray is null) mapLinesArray = new VertexArray(mapLinesBuffer);

            GL.UseProgram(flatColourShaderProgram.ShaderProgramHandle);
            GL.BindVertexArray(mapLinesArray.VertexArrayHandle);
            GL.DrawArrays(PrimitiveType.Lines, 0, mapLinesBuffer.VertexCount);
            GL.BindVertexArray(0);
        }

        // Draw range circles from the current location
        private void DrawRangeCircles() {
            Matrix4 translateM = Matrix4.CreateTranslation(CurrentSystem!.MapPos);
            for (int range = 2; range <= 14; range += 2) {
                float col = 0.8f - ((float)range / 20.0f);
                flatColourShaderProgram.SetUniform("flatColour", new Vector4(col, col, col, 1f));
                Matrix4 scaleM = Matrix4.CreateScale(range, range, range);
                Matrix4 modelM = scaleM * translateM;
                flatColourShaderProgram.SetUniform("model", modelM);
                GL.UseProgram(flatColourShaderProgram.ShaderProgramHandle);
                if (fMapViewZ + range > 20) {
                    Circle.Circle64.Bind();
                    Circle.Circle64.Draw();
                    Circle.Circle64.Unbind();
                }
                else {
                    Circle.Circle32.Bind();
                    Circle.Circle32.Draw();
                    Circle.Circle32.Unbind();
                }
            }
            GL.BindVertexArray(0);
        }

        // Join hover star with selected star
        private void DrawMapHoverLink() {
            if (aoHover == null || aoSelected == null || aoHover == aoSelected) return;
            if (Control.ModifierKeys != Keys.Alt) return; // Only display if Alt is held down            

            VertexPos3D[] vertices = new VertexPos3D[2] {
                new VertexPos3D(aoSelected.GetMapLocation()),
                new VertexPos3D(aoHover.GetMapLocation())
            };

            if (hoverLinkBuffer is null) hoverLinkBuffer = new VertexBuffer(vertices.ToArray(), BufferUsageHint.StreamDraw);
            else hoverLinkBuffer.SetData(vertices.ToArray());
            if (hoverLinkArray is null) hoverLinkArray = new VertexArray(hoverLinkBuffer);

            // Join selected and hover AOs if necessary
            Vector4 col = new Vector4(1f, 0f, 0f, 1f);
            if (PlayerTeam.PlayerShip.Range >= aoSelected.GetSystem().DistanceTo(aoHover.GetSystem())) col = new Vector4(0f, 1f, 0f, 1f);
            flatColourShaderProgram.SetUniform("flatColour", col);

            GL.UseProgram(flatColourShaderProgram.ShaderProgramHandle);
            GL.BindVertexArray(hoverLinkArray.VertexArrayHandle);
            GL.DrawArrays(PrimitiveType.Lines, 0, 2);
            GL.BindVertexArray(0);
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
