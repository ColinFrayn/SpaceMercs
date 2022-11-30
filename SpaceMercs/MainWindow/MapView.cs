using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using SpaceMercs.Dialogs;
using SpaceMercs.Graphics;
using SpaceMercs.Graphics.Shapes;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Threading;
using Keys = OpenTK.Windowing.GraphicsLibraryFramework.Keys;

namespace SpaceMercs.MainWindow {
    // Partial class including core functions and definitions
    partial class MapView : GameWindow {
        public enum ViewMode { ViewMap, ViewSystem, ViewMission, ViewShip };

        private const int DoubleClickTime = 250; // Milliseconds
        private bool bLoaded = false;
        private int mx, my;
        private readonly LinkedList<Star> RecentlyVisited = new LinkedList<Star>();
        private Matrix4 perspective = new Matrix4();
        private ViewMode view = ViewMode.ViewMap;
        private AstronomicalObject? aoSelected = null, aoHover = null;
        private Map GalaxyMap = new Map();
        private bool bShowLabels = true;
        private bool bJustLoaded = false;
        private Team? PlayerTeam = null;
        private int MinSectorX, MaxSectorX, MinSectorY, MaxSectorY;
        private readonly Stopwatch swLastTick = new Stopwatch();
        private DateTime lastLoad = DateTime.MinValue;
        private readonly Stopwatch swLastClick = new Stopwatch();

        public GUIMessageBox msgBox { get; private set; }
        public Travel TravelDetails { get; private set; }

        // Macros
        public float Aspect { get { return bLoaded ? (float)Size.X / (float)Size.Y : 1f; } }
        private Star? CurrentSystem { get { return PlayerTeam?.CurrentPosition?.GetSystem(); } }

        // DEBUGGING
        private GLShape? squares;
        private ShaderProgram shaderProgram;
        private int frameCount = 0;
        private bool DEMO_MODE = false;  // -----=====## DEMO MODE ##=====-----

        // Initialise the game
        public MapView(GameWindowSettings gws, NativeWindowSettings nws) : base(gws, nws) {
            CenterWindow();
            InitialiseGUIElements();
            DisableMenus();

            // Load static data and close if this fails
            if (!StaticData.LoadAll()) this.Close();

            swLastTick.Start();
            swLastClick.Start();

            WindowState = WindowState.Normal;
        }

        #region GameWindow Triggers
        // Initialise the window after it has been loaded
        protected override void OnLoad() {
            IsVisible = true;
            fMapViewZ = Const.InitialMapViewZ;
            fMapViewX = fMapViewY = 0;
            bShowGridlines = true;
            bFadeUnvisited = true;
            bShowRangeCircles = true;
            bShowTradeRoutes = true;
            MakeCurrent();
            //SetupMapTextures();
            //Planet.BuildPlanetHalo();
            //GraphicsFunctions.Initialise();
            //Terrain.GenerateSeedMap();
            //SetupGUIElements();
            bLoaded = true;
            SetupViewport();
            //SetupOptionsMenu();
            //TODO this.missionToolStripMenuItem.Enabled = false;
            ThisDispatcher = Dispatcher.CurrentDispatcher;
            msgBox = new GUIMessageBox(this);

            if (DEMO_MODE) SetupDemo(); // DEBUG

            base.OnLoad();
        }

        private void SetupDemo() {
            frameCount = 0;
            Random rnd = new Random();
            int boxCount = 100;
            int vxCount = 0, ixCount = 0;
            VertexPos2DCol[] vertices = new VertexPos2DCol[boxCount * 4];
            int[] indices = new int[boxCount * 6];
            for (int i = 0; i < boxCount; i++) {
                int w = rnd.Next(32, 128);
                int h = rnd.Next(32, 128);
                int x = rnd.Next(0, Size.X - 128);
                int y = rnd.Next(0, Size.Y - 128);
                float r = (float)rnd.NextDouble();
                float g = (float)rnd.NextDouble();
                float b = (float)rnd.NextDouble();
                vertices[vxCount++] = new VertexPos2DCol(new Vector2(x, y + h), new Color4(r, g, b, 1f));
                vertices[vxCount++] = new VertexPos2DCol(new Vector2(x + w, y + h), new Color4(r, g, b, 1f));
                vertices[vxCount++] = new VertexPos2DCol(new Vector2(x + w, y), new Color4(r, g, b, 1f));
                vertices[vxCount++] = new VertexPos2DCol(new Vector2(x, y), new Color4(r, g, b, 1f));
                indices[ixCount++] = 0 + (i * 4);
                indices[ixCount++] = 1 + (i * 4);
                indices[ixCount++] = 2 + (i * 4);
                indices[ixCount++] = 0 + (i * 4);
                indices[ixCount++] = 2 + (i * 4);
                indices[ixCount++] = 3 + (i * 4);
            }

            squares = new GLShape(vertices, indices);
            shaderProgram = new ShaderProgram(ShaderCode.VertexShaderPos2Col4, ShaderCode.PixelShaderColourFactor);
            shaderProgram.SetUniform("model", Matrix4.Identity);
        }

        // Free stuff when the window is being closed
        protected override void OnUnload() {
            squares?.Dispose();
            shaderProgram?.Dispose();
            base.OnUnload();
        }

        // This gets called every 1/60 of a second. AI updates etc.
        protected override void OnUpdateFrame(FrameEventArgs e) {
            base.OnUpdateFrame(e);

            frameCount++; // DEBUG

            // Check keypresses
            GetKeyboardInput();

            if (bJustLoaded) {  // resuming a mission after a load
                bJustLoaded = false;
                if (TravelDetails != null) TravelDetails.ResumeMissionAfterReload();
                else if (PlayerTeam.CurrentMission != null) {
                    RunMission(PlayerTeam.CurrentMission);
                }
                return;
            }

            // No game loaded so nothing to do
            if (!bLoaded) return;

            if (view == ViewMode.ViewMission) {
                if (swLastTick.ElapsedMilliseconds > 300) {
                    swLastTick.Restart();
                    // --- Resolve clock tick stuff
                    // Soldiers auto moving
                    List<Soldier> lSoldiers = new List<Soldier>(ThisMission.Soldiers); // In case any die in the middle of the loop
                    foreach (Soldier s in lSoldiers) {
                        if (s.GoTo == s.Location) s.GoTo = Point.Empty;
                        if (s.GoTo != Point.Empty) {
                            if (s.TravelRange == 0) { s.GoTo = Point.Empty; continue; } // May happen if something that occurs during movement alters stamina / movement points remaining
                            List<Point> path = CurrentLevel.ShortestPath(s, s.Location, s.GoTo, 20, true, 0);
                            if (path == null || path.Count == 0) { s.GoTo = Point.Empty; continue; }  // e.g. if some other soldier moved in the way and blocked the route
                            if (path[0].X == s.X && path[0].Y == s.Y + 1) MoveSoldier(s, Utils.Direction.North);
                            else if (path[0].X == s.X && path[0].Y == s.Y - 1) MoveSoldier(s, Utils.Direction.South);
                            else if (path[0].Y == s.Y && path[0].X == s.X - 1) MoveSoldier(s, Utils.Direction.West);
                            else if (path[0].Y == s.Y && path[0].X == s.X + 1) MoveSoldier(s, Utils.Direction.East);
                            else throw new Exception("Next path point is not adjacent to soldier!");
                            if (SelectedEntity == s) { // Soldier is still alive and selected
                                if (hoverx >= 0 && hoverx < CurrentLevel.Width && hovery >= 0 && hovery < CurrentLevel.Height && DistMap[hoverx, hovery] > 0) {
                                    lCurrentPath = CurrentLevel.ShortestPath(SelectedEntity, SelectedEntity.Location, new Point(hoverx, hovery), 20, true);
                                }
                                else lCurrentPath = null;
                            }
                        }
                    }
                }
                return;
            }

            // Not on mission screen, so tick the clock & check if we died
            if (GalaxyMap.bMapSetup) {
                Const.dtTime = Const.dtTime.AddMilliseconds(swLastTick.ElapsedMilliseconds);
                swLastTick.Restart();
                if (PlayerTeam.PlayerShip.Hull <= 0.0) GameOver();
            }
        }

        // Main window is resized so setup viewport again
        protected override void OnResize(ResizeEventArgs e) {
            if (!bLoaded) return;
            SetupViewport();
            base.OnResize(e);
        }

        private void RunDemo() {
            Matrix4 projectionM = Matrix4.CreateOrthographicOffCenter(0.0f, Size.X, Size.Y, 0.0f, -1.0f, 1.0f);
            shaderProgram.SetUniform("projection", projectionM);
            float ang = (float)frameCount * (float)Math.PI / 240f;
            Matrix4 rotationM = Matrix4.CreateRotationZ(ang);
            Matrix4 translateM = Matrix4.CreateTranslation(Size.X / 2, Size.Y / 2, 0.0f);
            Matrix4 modelM = translateM.Inverted() * rotationM * translateM;
            shaderProgram.SetUniform("model", modelM);
            float fract = (float)(Math.Abs(Math.Sin((double)frameCount * 2.0 * Math.PI / 120) / 2.0 + 0.5)) * 0.9f + 0.1f;
            shaderProgram.SetUniform("colourFactor", fract);
            GL.UseProgram(shaderProgram.ShaderProgramHandle);
            squares?.Bind();
            squares?.Draw();
            squares?.Unbind();
        }

        // Draw the main view, whatever state it's in
        protected override void OnRenderFrame(FrameEventArgs e) {
            if (!bLoaded) return;

            // Set up default OpenGL rendering parameters
            PrepareScene();

            if (!GalaxyMap.bMapSetup) {
                if (DEMO_MODE) RunDemo();
                else DisplayWelcomeScreen();
                SwapBuffers();
                return;
            }

            if (view == ViewMode.ViewMap) {
                DrawMap();
            }
            else if (view == ViewMode.ViewSystem) {
                DrawSystem();
            }
            else if (view == ViewMode.ViewMission) {
                DrawMission();
            }
            else if (view == ViewMode.ViewShip) {
                DrawShip();
            }
            else throw new NotImplementedException();

            // Any message? If so then display it here. Should affect all mapview modes
            msgBox.Display(mx, my);

            // Swap rendered surface to front
            SwapBuffers();
        }

        // Closing the window - shut down stuff
        protected override void OnClosing(CancelEventArgs e) {
            bLoaded = false;
        }
        #endregion // GameWindow Triggers

        #region Rendering
        private void InitialiseGUIElements() {
            // 
            // MapView
            // 

            //this.FormClosing += new FormClosingEventHandler(this.glMapView_Close);
            //this.KeyDown += new KeyEventHandler(this.MapView_KeyDown);
            //this.KeyUp += new KeyEventHandler(this.glMapView_KeyUp);

            // 
            // fileToolStripMenuItem
            // 
            //this.newToolStripMenuItem.Click += new System.EventHandler(this.newToolStripMenuItem_Click);
            //this.loadToolStripMenuItem.Click += new System.EventHandler(this.loadToolStripMenuItem_Click);
            //this.saveToolStripMenuItem.Click += new System.EventHandler(this.saveToolStripMenuItem_Click);
            //this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);

            // 
            // optionsToolStripMenuItem
            // 
            //this.showLabelsToolStripMenuItem.Click += new System.EventHandler(this.showLabelsToolStripMenuItem_Click);
            //this.showMapGridToolStripMenuItem.Click += new System.EventHandler(this.showMapGridToolStripMenuItem_Click);
            //this.fadeUnvisitedStarsToolStripMenuItem.Click += new System.EventHandler(this.fadeUnvisitedStarsToolStripMenuItem_Click);
            //this.showRangeCirclesToolStripMenuItem.Click += new System.EventHandler(this.showRangeCirclesToolStripMenuItem_Click);
            //this.showTradeRoutesToolStripMenuItem.Click += new System.EventHandler(this.showTradeRoutesToolStripMenuItem_Click);
            //this.showFlagsToolStripMenuItem.Click += new System.EventHandler(this.showFlagsToolStripMenuItem_Click);
            //this.showColoniesToolStripMenuItem.Click += new System.EventHandler(this.showColoniesToolStripMenuItem_Click);

            // 
            // viewToolStripMenuItem
            // 
            //this.shipToolStripMenuItem.Click += new System.EventHandler(this.shipToolStripMenuItem_Click);
            //this.teamToolStripMenuItem.Click += new System.EventHandler(this.teamToolStripMenuItem_Click);
            //this.colonyToolStripMenuItem.Click += new System.EventHandler(this.colonyToolStripMenuItem_Click);
            //this.racesToolStripMenuItem.Click += new System.EventHandler(this.racesToolStripMenuItem_Click);

            // 
            // missionToolStripMenuItem
            // 
            //this.labelsToolStripMenuItem.Click += new System.EventHandler(this.labelsToolStripMenuItem_Click);
            //this.healthBarsToolStripMenuItem.Click += new System.EventHandler(this.healthBarsToolStripMenuItem_Click);
            //this.travelDistanceToolStripMenuItem.Click += new System.EventHandler(this.travelDistanceToolStripMenuItem_Click);
            //this.movementPathToolStripMenuItem.Click += new System.EventHandler(this.movementPathToolStripMenuItem_Click);
            //this.viewEffectsToolStripMenuItem.Click += new System.EventHandler(this.viewEffectsToolStripMenuItem_Click);
            //this.viewDetectionRadiiToolStripMenuItem.Click += new System.EventHandler(this.viewDetectionRadiiToolStripMenuItem_Click);
            //this.detailsToolStripMenuItem.Click += new System.EventHandler(this.detailsToolStripMenuItem_Click);
        }

        // Display a welcome screen on startup
        private void DisplayWelcomeScreen() {
            // Set up scene
            GL.DepthMask(false);
            GL.ClearColor(Color.Black);
            GL.Clear(ClearBufferMask.ColorBufferBit);

            // Display welcome message
            TextRenderOptions tro = new() { Alignment = Alignment.TopMiddle, XPos = 0.5f, YPos = 0.2f, Scale = 0.05f };
            TextRenderer.Draw("Welcome to SpaceMercs v" + Const.strVersion, tro, Aspect);
            TextRenderOptions tro2 = new() { Alignment = Alignment.TopMiddle, XPos = 0.5f, YPos = 0.45f, Scale = 0.03f };
            TextRenderer.Draw("Select An Option From The File Menu", tro2, Aspect);
        }

        // Set up the scene ready for rendering
        private void PrepareScene() {
            MakeCurrent();
            GL.ClearColor(Color.Black);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            GL.CullFace(CullFaceMode.Back);
            GL.ShadeModel(ShadingModel.Smooth);
            GL.Enable(EnableCap.DepthTest);
            GL.Disable(EnableCap.Blend);
            GL.DepthFunc(DepthFunction.Lequal);
            GL.DepthMask(true);
        }

        // Setup the OpenGL viewport
        private void SetupViewport() {
            MakeCurrent();
            GL.Viewport(0, 0, Size.X, Size.Y);
        }
        #endregion // Rendering

        #region Input Handling
        // Key handlers
        private void GetKeyboardInput() {
            if (msgBox.Active) {
                if (IsKeyPressed(Keys.Enter)) msgBox.DefaultAction();
                return;
            }
            if (IsKeyReleased(Keys.F1)) { // New Game
                if (GalaxyMap.bMapSetup) {
                    msgBox.PopupConfirmation("You are in the middle of a game.\nGenerating a new game will lose all unsaved progress.\nAre you sure you want to continue?", NewGame_Continue);
                }
                else NewGame_Continue();
                return;
            }
            if (IsKeyReleased(Keys.F5)) { // Save Game
                SaveGame();
                return;
            }
            if (IsKeyReleased(Keys.F9)) { // Load Game
                LoadGame();
                return;
            }
            if (IsKeyReleased(Keys.Escape)) { // Quit Game
                if (GalaxyMap.bMapSetup == true) {
                    msgBox.PopupConfirmation("You are in the middle of a game.\nExiting will lose all unsaved progress.\nAre you sure you want to continue?", this.Close);
                }
                return;
            }
            if (view == ViewMode.ViewShip) {
                GetKeyboardInput_Ship();
                return;
            }
            if (view == ViewMode.ViewMission) {
                GetKeyboardInput_Mission();
                return;
            }

            // -- Applies to all views except ship & mission
            {
                if (IsKeyPressed(Keys.C)) { // Centre on selected AO
                    if (aoSelected == null) {
                        fMapViewX = CurrentSystem.MapPos.X;
                        fMapViewY = CurrentSystem.MapPos.Y;
                    }
                    else {
                        fMapViewX = aoSelected.GetSystem().MapPos.X;
                        fMapViewY = aoSelected.GetSystem().MapPos.Y;
                    }
                }
                if (IsKeyPressed(Keys.L)) { // Toggle on/off showing labels for stars, planets & moons
                    if (bShowLabels) { bShowLabels = false; }
                    else { bShowLabels = true; }
                }
                if (IsKeyPressed(Keys.Escape)) { // Deselect
                    if (aoSelected != null) aoSelected = null;
                    else {
                        if (view == ViewMode.ViewSystem) view = ViewMode.ViewMap;
                    }
                }
            }

            if (view == ViewMode.ViewMap) {
                GetKeyboardInput_MapView();
                return;
            }

            if (view == ViewMode.ViewSystem) {
                GetKeyboardInput_SystemView();
                return;
            }
        }
        private void GetKeyboardInput_MapView() {
            if (IsKeyPressed(Keys.G)) {  // Toggle on/off gridlines
                if (bShowGridlines) { bShowGridlines = false; }
                else { bShowGridlines = true; }
            }
            if (IsKeyPressed(Keys.V)) {  // Toggle on/off fading of unvisited stars
                if (bFadeUnvisited) { bFadeUnvisited = false; }
                else { bFadeUnvisited = true; }
            }
            if (IsKeyPressed(Keys.R)) {  // Toggle on/off range circles
                if (bShowRangeCircles) { bShowRangeCircles = false; }
                else { bShowRangeCircles = true; }
            }
            if (IsKeyPressed(Keys.A)) {  // Toggle on/off trade routes
                if (bShowTradeRoutes) { bShowTradeRoutes = false; }
                else { bShowTradeRoutes = true; }
            }
            if (IsKeyPressed(Keys.F)) {  // Toggle on/off ownership flags
                if (bShowFlags) { bShowFlags = false; }
                else { bShowFlags = true; }
            }
            MapHover();
        }

        // Mouse handling
        protected override void OnMouseMove(MouseMoveEventArgs e) {
            if (PlayerTeam is null) return;
            if (msgBox.Active) {
                mx = (int)e.X;
                my = (int)e.Y;
                return;
            }
            if (view == ViewMode.ViewShip) {
                MouseMove_Ship(e);
                return;
            }
            if (view == ViewMode.ViewMission) {
                MouseMove_Mission(e);
                return;
            }
            if ((DateTime.Now - lastLoad).TotalMilliseconds < 1000.0) return; // Just loaded. Don't process extra clicks and shift the view.
            if (MouseState.IsButtonDown(MouseButton.Left)) {
                float fScale = Const.MouseMoveScale * (float)fMapViewZ / 27.0f; // To make sure that the scrolling is a sensible speed regardless of zoom level
                if (IsKeyDown(Keys.LeftControl) || IsKeyDown(Keys.RightControl)) fScale *= 2.5f;
                else if (IsKeyDown(Keys.LeftShift) || IsKeyDown(Keys.RightShift)) fScale /= 2.5f;
                fMapViewX -= (float)(e.X - mx) * fScale;
                fMapViewY += (float)(e.Y - my) * fScale;
                if (fMapViewX < (CurrentSystem.MapPos.X - Const.MaximumScrollRange)) fMapViewX = (CurrentSystem.MapPos.X - Const.MaximumScrollRange);
                if (fMapViewX > (CurrentSystem.MapPos.X + Const.MaximumScrollRange)) fMapViewX = (CurrentSystem.MapPos.X + Const.MaximumScrollRange);
                if (fMapViewY < (CurrentSystem.MapPos.Y - Const.MaximumScrollRange)) fMapViewY = (CurrentSystem.MapPos.Y - Const.MaximumScrollRange);
                if (fMapViewY > (CurrentSystem.MapPos.Y + Const.MaximumScrollRange)) fMapViewY = (CurrentSystem.MapPos.Y + Const.MaximumScrollRange);
                // TODO glMapView.Invalidate();
            }

            // Hover over GUI objects
            bool b1 = gbRenameObject.IsHover(mx, my);
            bool b2 = gbFlyTo.IsHover(mx, my);
            bool b3 = gbViewColony.IsHover(mx, my);
            bool b4 = gbScan.IsHover(mx, my);
            mx = (int)e.X;
            my = (int)e.Y;
            bool bUpdate = false;
            if (gbRenameObject.IsHover(mx, my) != b1) bUpdate = true;
            if (gbFlyTo.IsHover(mx, my) != b2) bUpdate = true;
            if (gbViewColony.IsHover(mx, my) != b3) bUpdate = true;
            if (gbScan.IsHover(mx, my) != b4) bUpdate = true;
            // TODO if (bUpdate) glMapView.Invalidate();
            if (view == ViewMode.ViewMap) MapHover();
            if (view == ViewMode.ViewSystem) SystemHover();
        }
        protected override void OnMouseDown(MouseButtonEventArgs e) {
            if (PlayerTeam == null) return;
            if (msgBox.Active) return;
            mx = (int)MousePosition.X;
            my = (int)MousePosition.Y;
            if (view == ViewMode.ViewMission) {
                MouseDown_Mission(e);
                return;
            }
            if (view == ViewMode.ViewShip) {
                MouseDown_Ship(e);
                return;
            }
        }
        protected override void OnMouseUp(MouseButtonEventArgs e) {
            if (PlayerTeam == null) return;
            if (msgBox.Active) {
                msgBox.CaptureClick(mx, my);
                return;
            }
            long clickGap = swLastClick.ElapsedMilliseconds;
            swLastClick.Restart();
            if (e.Button == MouseButton.Left && clickGap < DoubleClickTime) {
                DoubleClickHandler(e);
                return;
            }
            if (view == ViewMode.ViewMission) {
                MouseUp_Mission(e);
                return;
            }
            if (view == ViewMode.ViewShip) {
                MouseUp_Ship(e);
                return;
            }
            if (e.Button == MouseButton.Right) {
                // Add right click stuff here, if needed
            }
            if (e.Button == MouseButton.Left) {
                if (gbRenameObject.CaptureClick(mx, my)) return;
                if (gbFlyTo.CaptureClick(mx, my)) return;
                if (gbViewColony.CaptureClick(mx, my)) return;
                if (gbScan.CaptureClick(mx, my)) return;
                if (aoHover != null) aoSelected = aoHover;
                SetSelection();
            }
        }
        protected override void OnMouseWheel(MouseWheelEventArgs e) {
            if (PlayerTeam == null) return;
            if (msgBox.Active) return;

            float delta;
            if (IsKeyDown(Keys.LeftControl) || IsKeyDown(Keys.RightControl)) delta = -e.OffsetX / 50.0f;
            else if (IsKeyDown(Keys.LeftShift) || IsKeyDown(Keys.RightShift)) delta = -e.OffsetX / 500.0f;
            else delta = -e.OffsetX / 150.0f;

            if (view == ViewMode.ViewMission) {
                MouseWheel_Mission(delta);
                return;
            }
            if (view == ViewMode.ViewShip) {
                MouseWheel_Ship(delta);
                return;
            }

            fMapViewZ += delta;
            if (fMapViewZ < Const.MinimumMapViewZ) fMapViewZ = Const.MinimumMapViewZ;
            if (fMapViewZ > Const.MaximumMapViewZ) fMapViewZ = Const.MaximumMapViewZ;
        }
        private void DoubleClickHandler(MouseButtonEventArgs e) {
            if (view == ViewMode.ViewMission) {
                DoubleClick_Mission();
                return;
            }
            if (view == ViewMode.ViewShip) {
                DoubleClick_Ship();
                return;
            }
            if (e.Button == MouseButton.Left) {
                if (aoHover != null) aoSelected = aoHover;
                // Zoom in to system
                if (view == ViewMode.ViewMap && aoSelected != null && aoSelected.AOType == AstronomicalObject.AstronomicalObjectType.Star) {
                    Star st = (Star)aoSelected;
                    if (st.Visited) {
                        SystemStar = st;
                        st.GeneratePlanets(GalaxyMap.PlanetDensity);
                        view = ViewMode.ViewSystem;
                        aoSelected = null;
                        gbRenameObject.Deactivate();
                        gbFlyTo.Deactivate();
                        gbViewColony.Deactivate();
                        gbScan.Deactivate();
                    }
                    else {
                        msgBox.PopupMessage("You have not yet visited that system");
                    }
                }
            }
        }
        #endregion // Input Handling

        #region Game Management
        // Setup the details for a new game
        private void SetupNewGame(NewGame ng) {
            fMapViewZ = Const.InitialMapViewZ;
            aoSelected = null;
            aoHover = null;
            RecentlyVisited.Clear();
            bLoaded = true;
            CloseAllDialogs();
            PlayerTeam = new Team(ng, StaticData.Races[0]);
            if (PlayerTeam.CurrentPosition == null || PlayerTeam.CurrentPosition.Colony == null) throw new Exception("Did not set up PlayerTeam correctly - not at home planet!");
            PlayerTeam.CurrentPosition.Colony.UpdateStock(PlayerTeam);
            SetupOptionsMenu();
            // TODO glMapView.Invalidate();
            // TODO saveToolStripMenuItem.Enabled = true;
        }

        // External triggers
        public void RefreshView() {
            // TODO glMapView.Invalidate();
        }
        private void NewGame_Continue() {
            // Set up the new game dialog box
            NewGame ng = new NewGame();
            DialogResult res = ng.ShowDialog();
            if (res == DialogResult.OK) {
                try {
                    MakeCurrent();
                    GalaxyMap.Generate(ng);
                    SetupNewGame(ng);
                    EnableMenus();
                    SetAOButtonsOnGUI(aoSelected);
                }
                catch (Exception ex) {
                    MessageBox.Show(ex.ToString());
                }
            }
        }
        public void SaveGame() {
            if (GalaxyMap.bMapSetup == false) return;
            bool bTick = false;
            if (TravelDetails != null) bTick = TravelDetails.StopTimer();

            // Get the filename
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "SpaceMercs Savegame Files (*.sve)|*.sve|All files (*.*)|*.*";
            sfd.FilterIndex = 1;
            DialogResult result = sfd.ShowDialog();
            if (result == DialogResult.OK) {
                try {
                    SaveGame(sfd.FileName);
                }
                catch (Exception ex) {
                    MessageBox.Show(ex.ToString(), "Save Game Failed", MessageBoxButtons.OK);
                    return;
                }
                finally {
                    if (TravelDetails != null && bTick) TravelDetails.StartTimer();
                }
            }
        }
        public void LoadGame() {
            // Need to check to see if we already have a map set up
            if (GalaxyMap.bMapSetup == true) {
                msgBox.PopupConfirmation("You are in the middle of a game.\nLoading a game will lose all unsaved progress.\nAre you sure you want to continue?", LoadGame_Continue);
            }
            else LoadGame_Continue();
        }
        private void LoadGame_Continue() {
            // Get the filename
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "SpaceMercs Savegame Files (*.sve)|*.sve|All files (*.*)|*.*";
            ofd.FilterIndex = 1;
            DialogResult result = ofd.ShowDialog();
            if (result == DialogResult.OK) {
                try {
                    Tuple<Map, Team, Travel> tp = LoadGame(ofd.FileName);
                    // If we get here then it loaded OK, so overwrite everything
                    GalaxyMap = tp.Item1;
                    PlayerTeam = tp.Item2;
                    TravelDetails = tp.Item3;

                    // Setup GUI
                    aoSelected = null;
                    aoHover = null;
                    fMapViewZ = Const.InitialMapViewZ;
                    EnableMenus();
                    bLoaded = true;
                    RecentlyVisited.Clear();

                    // Close down any windows
                    CloseAllDialogs();
                    SetupOptionsMenu();

                    // Zoom to current location
                    SystemStar = PlayerTeam.CurrentPosition.GetSystem();
                    fMapViewX = PlayerTeam.CurrentPosition.GetMapLocation().X;
                    fMapViewY = PlayerTeam.CurrentPosition.GetMapLocation().Y;
                    lastLoad = DateTime.Now;
                    SetAOButtonsOnGUI(aoSelected);
                    // TODO glMapView.Invalidate();
                    view = ViewMode.ViewMap;

                    // If we're on a mission then set it up
                    // Do this by configuring a flag that is consumed at the top of OnUpdateFrame()
                    bJustLoaded = true;
                    return;
                }
                catch (Exception ex) {
                    MessageBox.Show(ex.ToString(), "Load Game Failed", MessageBoxButtons.OK);
                    return;
                }
            }
        }
        private void GameOver() {
            bLoaded = false;
            msgBox.PopupMessage("Game Over!\nPlease start a new game or load a saved game to continue playing");
        }
        #endregion // Game Management

        // Finish travelling
        public void ArriveAt(AstronomicalObject aoTo) {
            // Close colonyview if open
            foreach (Form f in Application.OpenForms) {
                if (f.GetType() == typeof(ColonyView)) { f.Close(); break; }
            }
            aoTo.GetSystem().SetVisited(true);
            aoTo.GetSystem().UpdateColonies();
            msgBox.PopupMessage("You have arrived at your destination");
            PlayerTeam.CurrentPosition = TravelDetails.Destination;
            if (PlayerTeam.CurrentPosition.Colony != null) PlayerTeam.CurrentPosition.Colony.UpdateStock(PlayerTeam); // Make sure we get up to date with what this colony has in store
            TravelDetails = null;
            SetAOButtonsOnGUI(aoSelected);
        }

        // Close down all dialogs
        private void CloseAllDialogs() {
            List<Form> FormsToClose = new List<Form>();
            foreach (Form f in Application.OpenForms) {
                if (f.GetType() != typeof(MapView)) FormsToClose.Add(f);
            }
            foreach (Form f in FormsToClose) {
                f.Close();
            }
        }

        // Setup the current selection as the true selection. Set up text, textures etc.
        private void SetSelection() {
            if (aoSelected != null) {
                if (aoSelected.AOType == AstronomicalObject.AstronomicalObjectType.Star) {
                    Star st = (Star)aoSelected;
                    //if (!String.IsNullOrEmpty(st.Name)) tlSel1.UpdateText(st.Name);
                    //else tlSel1.UpdateText("Unnamed Star");
                    //tlSel2.UpdateText("M = " + Math.Round(st.Mass, 2).ToString() + " Sol");
                    //tlSel3.UpdateText("R = " + Math.Round(st.radius / Const.Million, 0).ToString() + " Mm");
                    //tlSel4.UpdateText("Type " + st.StarType);
                    //tlSel5.UpdateText("(" + Math.Round(st.MapPos.X, 1) + "," + Math.Round(st.MapPos.Y, 1) + ")");
                }
                if (aoSelected.AOType == AstronomicalObject.AstronomicalObjectType.Planet) {
                    Planet pl = (Planet)aoSelected;
                    //if (!String.IsNullOrEmpty(pl.Name)) tlSel1.UpdateText(pl.Name);
                    //else tlSel1.UpdateText("Unnamed Planet");
                    //tlSel2.UpdateText(pl.Type.ToString());
                    //tlSel3.UpdateText("R = " + Math.Round(pl.radius / 1000.0, 0).ToString() + "km");
                    //string strTemp = "Temp = " + pl.Temperature + "K";
                    ////strTemp += "  <" + pl.GetTempRange(Const.races[0]).ToString() + ">";
                    //tlSel4.UpdateText(strTemp);
                    //tlSel5.UpdateText("Orbit = " + Math.Round(pl.orbit / Const.AU, 2).ToString() + " AU");
                    //tw.Update(iTextSelected4, "Dist = " + Math.Round(GraphicsFunctions.ViewDistance(pl), 3).ToString() + "Gm");
                }
                if (aoSelected.AOType == AstronomicalObject.AstronomicalObjectType.Moon) {
                    Moon mn = (Moon)aoSelected;
                    //tlSel1.UpdateText("Moon " + (mn.ID + 1).ToString());
                    //tlSel2.UpdateText(mn.Type.ToString());
                    //tlSel3.UpdateText("R = " + Math.Round(mn.radius / 1000.0, 0).ToString() + "km");
                    //tlSel4.UpdateText("Temp = " + mn.Temperature + "K");
                    //tlSel5.UpdateText("Orbit = " + Math.Round(mn.orbit / Const.Million, 0).ToString() + " Mm");
                }
            }
            SetAOButtonsOnGUI(aoSelected);
        }

        #region Menu Code
        // Disable/enable the game menus
        private void DisableMenus() {
            // TODO saveToolStripMenuItem.Enabled = false;
            // TODO shipToolStripMenuItem.Enabled = false;
            // TODO teamToolStripMenuItem.Enabled = false;
            // TODO colonyToolStripMenuItem.Enabled = false;
            // TODO racesToolStripMenuItem.Enabled = false;
        }
        private void EnableMenus() {
            // TODO saveToolStripMenuItem.Enabled = true;
            // TODO shipToolStripMenuItem.Enabled = true;
            // TODO teamToolStripMenuItem.Enabled = true;
            // TODO colonyToolStripMenuItem.Enabled = true;
            // TODO racesToolStripMenuItem.Enabled = true;
        }

        // Display the "About" box
        private void aboutToolStripMenuItem_Click(object sender, EventArgs e) {
            MessageBox.Show($"SpaceMercs v{Const.strVersion}\n(c) 2016-{DateTime.Now.Year} Colin Frayn\nwww.frayn.net", "About SpaceMercs", MessageBoxButtons.OK);
        }

        #region File Menu
        private void newToolStripMenuItem_Click(object sender, EventArgs e) {
            // Need to check to see if we already have a map set up
            if (GalaxyMap.bMapSetup) {
                msgBox.PopupConfirmation("You are in the middle of a game.\nGenerating a new game will lose all unsaved progress.\nAre you sure you want to continue?", NewGame_Continue);
            }
        }
        private void loadToolStripMenuItem_Click(object sender, EventArgs e) {
            LoadGame();
        }
        private void saveToolStripMenuItem_Click(object sender, EventArgs e) {
            SaveGame();
        }
        private void exitToolStripMenuItem_Click(object sender, EventArgs e) {
            // Need to check to see if we already have a map set up
            if (GalaxyMap.bMapSetup == true) {
                msgBox.PopupConfirmation("You are in the middle of a game.\nExiting will lose all unsaved progress.\nAre you sure you want to continue?", this.Close);
            }
            this.Close();
        }
        #endregion

        #region Options Menu
        private void SetupOptionsMenu() {
            // TODO Enable menus
            //if (bShowLabels) showLabelsToolStripMenuItem.Checked = true;
            //else showLabelsToolStripMenuItem.Checked = false;
            //if (bShowGridlines) showMapGridToolStripMenuItem.Checked = true;
            //else showMapGridToolStripMenuItem.Checked = false;
            //if (bFadeUnvisited) fadeUnvisitedStarsToolStripMenuItem.Checked = true;
            //else fadeUnvisitedStarsToolStripMenuItem.Checked = false;
            //if (bShowRangeCircles) showRangeCirclesToolStripMenuItem.Checked = true;
            //else showRangeCirclesToolStripMenuItem.Checked = false;
            //if (bShowTradeRoutes) showTradeRoutesToolStripMenuItem.Checked = true;
            //else showTradeRoutesToolStripMenuItem.Checked = false;
            //if (bShowFlags) showFlagsToolStripMenuItem.Checked = true;
            //else showFlagsToolStripMenuItem.Checked = false;
            //if (bShowColonies) showColoniesToolStripMenuItem.Checked = true;
            //else showColoniesToolStripMenuItem.Checked = false;
        }
        private void showLabelsToolStripMenuItem_Click(object sender, EventArgs e) {
            if (bShowLabels) {
                bShowLabels = false;
                // TODO showLabelsToolStripMenuItem.Checked = false;
            }
            else {
                bShowLabels = true;
                // TODO showLabelsToolStripMenuItem.Checked = true;
            }
            // TODO glMapView.Invalidate();
        }
        private void showMapGridToolStripMenuItem_Click(object sender, EventArgs e) {
            if (bShowGridlines) {
                bShowGridlines = false;
                // TODO showMapGridToolStripMenuItem.Checked = false;
            }
            else {
                bShowGridlines = true;
                // TODO showMapGridToolStripMenuItem.Checked = true;
            }
            // TODO glMapView.Invalidate();
        }
        private void fadeUnvisitedStarsToolStripMenuItem_Click(object sender, EventArgs e) {
            if (bFadeUnvisited) {
                bFadeUnvisited = false;
                // TODO fadeUnvisitedStarsToolStripMenuItem.Checked = false;
            }
            else {
                bFadeUnvisited = true;
                // TODO fadeUnvisitedStarsToolStripMenuItem.Checked = true;
            }
        }
        private void showRangeCirclesToolStripMenuItem_Click(object sender, EventArgs e) {
            if (bShowRangeCircles) {
                bShowRangeCircles = false;
                // TODO showRangeCirclesToolStripMenuItem.Checked = false;
            }
            else {
                bShowRangeCircles = true;
                // TODO showRangeCirclesToolStripMenuItem.Checked = true;
            }
        }
        private void showTradeRoutesToolStripMenuItem_Click(object sender, EventArgs e) {
            if (bShowTradeRoutes) {
                bShowTradeRoutes = false;
                // TODO showTradeRoutesToolStripMenuItem.Checked = false;
            }
            else {
                bShowTradeRoutes = true;
                // TODO showTradeRoutesToolStripMenuItem.Checked = true;
            }
        }
        private void showFlagsToolStripMenuItem_Click(object sender, EventArgs e) {
            if (bShowFlags) {
                bShowFlags = false;
                // TODO showFlagsToolStripMenuItem.Checked = false;
            }
            else {
                bShowFlags = true;
                // TODO showFlagsToolStripMenuItem.Checked = true;
            }
        }
        private void showColoniesToolStripMenuItem_Click(object sender, EventArgs e) {
            if (bShowColonies) {
                bShowColonies = false;
                // TODO showColoniesToolStripMenuItem.Checked = false;
            }
            else {
                bShowColonies = true;
                // TODO showColoniesToolStripMenuItem.Checked = true;
            }
        }

        #endregion

        #region View Menu
        private void shipToolStripMenuItem_Click(object sender, EventArgs e) {
            if (!GalaxyMap.bMapSetup) return;
            SetupShipView();
        }
        private void teamToolStripMenuItem_Click(object sender, EventArgs e) {
            if (!GalaxyMap.bMapSetup) return;
            TeamView tv = new TeamView(PlayerTeam);
            tv.ShowDialog();
        }
        private void colonyToolStripMenuItem_Click(object sender, EventArgs e) {
            if (!GalaxyMap.bMapSetup) return;
            if (PlayerTeam!.CurrentPosition.BaseSize == 0) return;
            ColonyView cv = new ColonyView(PlayerTeam, RunMission);
            cv.Show();
        }
        private void racesToolStripMenuItem_Click(object sender, EventArgs e) {
            if (!GalaxyMap.bMapSetup) return;
            RaceView rv = new RaceView();
            rv.Show();
        }
        #endregion // View Menu


        #endregion // Menu Code

    }
}