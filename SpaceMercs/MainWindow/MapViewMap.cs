using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using SpaceMercs.Graphics;
using SpaceMercs.Graphics.Shapes;

namespace SpaceMercs.MainWindow {
    // Partial class including functions for drawing the full galaxymap view
    partial class MapView {
        private float fMapViewZ, fMapViewX, fMapViewY;
        private bool bShowGridLines = true, bFadeUnvisited = false, bShowRangeCircles = false, bShowTradeRoutes = false, bShowFlags = true, bShowPop = false;
        VertexBuffer? mapLinesBuffer = null, hoverLinkBuffer = null;
        VertexArray? mapLinesArray = null, hoverLinkArray = null;
        private int lastMinX = -1, lastMinY = -1, lastMaxX = -1, lastMaxY = -1;

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

            // Set the correct view location & perspective matrices for each shader program
            Matrix4 projectionM = Matrix4.CreatePerspectiveFieldOfView(Const.MapViewportAngle, (float)Aspect, 0.05f, 5000.0f);
            flatColourShaderProgram.SetUniform("projection", projectionM);
            flatColourLitShaderProgram.SetUniform("projection", projectionM);
            pos2DCol4ShaderProgram.SetUniform("projection", projectionM);
            fullShaderProgram.SetUniform("projection", projectionM);

            Matrix4 translateM = Matrix4.CreateTranslation(-fMapViewX, -fMapViewY, -fMapViewZ);
            Matrix4 viewM = translateM;
            flatColourShaderProgram.SetUniform("view", viewM);
            flatColourShaderProgram.SetUniform("model", Matrix4.Identity);
            pos2DCol4ShaderProgram.SetUniform("view", viewM);
            pos2DCol4ShaderProgram.SetUniform("model", Matrix4.Identity);
            fullShaderProgram.SetUniform("view", viewM);
            fullShaderProgram.SetUniform("model", Matrix4.Identity);
            flatColourLitShaderProgram.SetUniform("view", viewM);
            flatColourLitShaderProgram.SetUniform("model", Matrix4.Identity);

            GL.ClearColor(Color.Black);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            // Display the map
            if (bShowGridLines) DrawMapGrid();
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
                double StarScale = Const.MapStarScale * Math.Pow(aoHover.Radius / Const.Million, 0.28) + 0.05;
                if (StarScale < Const.MapStarScale * 0.5) StarScale = Const.MapStarScale * 0.5;
                Matrix4 scaleM = Matrix4.CreateScale((float)StarScale * 1.1f);
                Matrix4 modelM = scaleM * translateM;
                flatColourShaderProgram.SetUniform("model", modelM);
                flatColourShaderProgram.SetUniform("flatColour", new Vector4(1f,1f,1f,1f));
                GL.UseProgram(flatColourShaderProgram.ShaderProgramHandle);
                Annulus.Annulus16.BindAndDraw();
            }

            if (aoSelected != null) {
                Matrix4 translateM = Matrix4.CreateTranslation(((Star)aoSelected).MapPos);
                double StarScale = Const.MapStarScale * Math.Pow(aoSelected.Radius / Const.Million, 0.28) + 0.05;
                if (StarScale < Const.MapStarScale * 0.5) StarScale = Const.MapStarScale * 0.5;
                Matrix4 scaleM = Matrix4.CreateScale((float)StarScale);
                Matrix4 modelM = scaleM * translateM;
                flatColourShaderProgram.SetUniform("model", modelM);
                flatColourShaderProgram.SetUniform("flatColour", new Vector4(0.2f, 1f, 0.4f, 1f));
                GL.UseProgram(flatColourShaderProgram.ShaderProgramHandle);
                Annulus.Annulus16.BindAndDraw();
            }

            if (CurrentSystem is not null) {
                double StarScale = Const.MapStarScale * Math.Pow(CurrentSystem.Radius / Const.Million, 0.28) + 0.05;
                if (StarScale < Const.MapStarScale * 0.5) StarScale = Const.MapStarScale * 0.5;
                Matrix4 translateM = Matrix4.CreateTranslation(CurrentSystem.MapPos.X, CurrentSystem.MapPos.Y, 0f);
                Matrix4 scaleM = Matrix4.CreateScale((float)StarScale, (float)StarScale, 1f);
                Matrix4 modelM = scaleM * translateM;
                flatColourShaderProgram.SetUniform("model", modelM);
                flatColourShaderProgram.SetUniform("flatColour", new Vector4(1f, 1f, 1f, 1f));
                GL.UseProgram(flatColourShaderProgram.ShaderProgramHandle);
                TriangleFocus.Flat.BindAndDraw();
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
            if (GalaxyMap.MapIsInitialised == false) return;

            // Setup basic lighting parameters (light colour is White by default)
            fullShaderProgram.SetUniform("lightPos", 100000f, 100000f, 10000f);
            fullShaderProgram.SetUniform("ambient", 0.25f);
            fullShaderProgram.SetUniform("textureEnabled", false);
            fullShaderProgram.SetUniform("lightEnabled", true);
            fullShaderProgram.SetUniform("lightCol", new Vector3(1f, 1f, 1f));
            fullShaderProgram.SetUniform("flatColour", new Vector4(1f, 1f, 1f, 1f));
            fullShaderProgram.SetUniform("texPos", 0f, 0f);
            fullShaderProgram.SetUniform("texScale", 1f, 1f);

            // Display all stars by sector
            for (int sy = MinSectorY; sy <= MaxSectorY; sy++) {
                for (int sx = MinSectorX; sx <= MaxSectorX; sx++) {
                    Sector sc = GalaxyMap.GetSector(sx, sy);
                    sc.Draw(fullShaderProgram, bFadeUnvisited, bShowLabels, bShowFlags, bShowPop, fMapViewX, fMapViewY, fMapViewZ, Aspect);
                }
            }
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
                    Circle.Circle64.BindAndDraw();
                }
                else {
                    Circle.Circle32.BindAndDraw();
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
            aoHover = null;
            if (!GalaxyMap.MapIsInitialised) return;

            // Work out what we're hovering over (pick the closest one!)
            double mxfract = (double)MousePosition.X / (double)Size.X;
            double myfract = (double)MousePosition.Y / (double)Size.Y;
            double mxpos = ((mxfract - 0.5) * (fMapViewZ / 18.6) * (double)Const.SectorSize * Aspect) + fMapViewX;
            double mypos = ((0.5 - myfract) * (fMapViewZ / 18.6) * (double)Const.SectorSize) + fMapViewY;
            int sx = (int)Math.Floor((mxpos / (double)Const.SectorSize) + 0.5);
            int sy = (int)Math.Floor((mypos / (double)Const.SectorSize) + 0.5);
            Tuple<int, int> tp = new Tuple<int, int>(sx, sy);
            Sector sc = GalaxyMap.GetSector(tp);
            aoHover = sc.CheckHover(mxpos, mypos, fMapViewZ);
        }
    }
}
