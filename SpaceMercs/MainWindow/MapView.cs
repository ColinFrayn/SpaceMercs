using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using SpaceMercs.Dialogs;
using SpaceMercs.Graphics;
using SpaceMercs.Graphics.Shapes;
using SpaceMercs.Items;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Threading;
using Keys = OpenTK.Windowing.GraphicsLibraryFramework.Keys;

namespace SpaceMercs.MainWindow {
    // Partial class including core functions and definitions
    partial class MapView : GameWindow {
        public enum ViewMode { ViewMap, ViewSystem, ViewMission, ViewShip };

        private const int DoubleClickTime = 250; // Milliseconds
        private bool bLoaded = false;
        private readonly LinkedList<Star> RecentlyVisited = new LinkedList<Star>();
        private ViewMode view = ViewMode.ViewMap;
        private AstronomicalObject? aoSelected = null, aoHover = null;
        private Map GalaxyMap = new Map();
        private bool bShowLabels = true;
        private bool bJustLoaded = false;
        private Team PlayerTeam = Team.Empty();
        private int MinSectorX, MaxSectorX, MinSectorY, MaxSectorY;
        private DateTime lastLoad = DateTime.MinValue;
        private readonly Stopwatch swLastTick = new Stopwatch();
        private readonly Stopwatch swLastClick = new Stopwatch();
        private GUIButton? gbLoadGame, gbNewGame, gbExitGame;
        private double MXPos {
            get {
                double mxfract = MousePosition.X / (double)Size.X;
                double mxpos = ((mxfract - 0.5) * (fMissionViewZ / 1.86) * Aspect) + fMissionViewX;
                return mxpos;
            }
        }
        private double MYPos {
            get {
                double myfract = MousePosition.Y / (double)Size.Y;
                double mypos = ((0.5 - myfract) * (fMissionViewZ / 1.86)) + fMissionViewY;
                return mypos;
            }
        }
        public static GlobalClock Clock = new(Const.StartingDate);

        public GUIMessageBox msgBox { get; private set; }
        public Travel? TravelDetails { get; private set; }

        // Macros
        public float Aspect { get { return (float)Size.X / (float)Size.Y; } }
        private Star? CurrentSystem { get { return PlayerTeam?.CurrentPosition?.GetSystem(); } }

        // Graphics
        private ShaderProgram flatColourShaderProgram;
        private ShaderProgram pos2DCol4ShaderProgram;
        private ShaderProgram fullShaderProgram;
        private ShaderProgram flatColourLitShaderProgram;
        private BitmapData? BackgroundTextureData;
        private int iBackgroundTextureId = -1;

        // Initialise the game
        public MapView(GameWindowSettings gws, NativeWindowSettings nws) : base(gws, nws) {
            CenterWindow();

            // Load static data and close if this fails
            if (!StaticData.LoadAll()) this.Close();

            // Load background data
            string strBackgroundBitmapFile = Path.Combine(StaticData.GraphicsLocation, @"Backgrounds\Background1.bmp");
            if (File.Exists(strBackgroundBitmapFile)) {
                Bitmap TextureBitmap = new Bitmap(strBackgroundBitmapFile);
                BackgroundTextureData = TextureBitmap.LockBits(new Rectangle(0, 0, TextureBitmap.Width, TextureBitmap.Height), ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            }

            swLastTick.Start();
            swLastClick.Start();

            WindowState = WindowState.Normal;
            msgBox = new GUIMessageBox(this);
        }

        #region GameWindow Triggers
        // Initialise the window after it has been loaded
        protected override void OnLoad() {
            IsVisible = true;
            fMapViewZ = Const.InitialMapViewZ;
            fMapViewX = fMapViewY = 0;
            bShowGridLines = true;
            bFadeUnvisited = true;
            bShowRangeCircles = true;
            bShowTradeRoutes = true;
            MakeCurrent();
            //SetupMapTextures(); // Not implemented
            Planet.BuildPlanetHalo();
            SetupGUIElements();
            bLoaded = true;
            ThisDispatcher = Dispatcher.CurrentDispatcher;

            // Setup the default shader programs
            pos2DCol4ShaderProgram = new ShaderProgram(ShaderCode.VertexShaderPos2Col4, ShaderCode.PixelShaderColourFactor);
            pos2DCol4ShaderProgram.SetUniform("model", Matrix4.Identity);
            pos2DCol4ShaderProgram.SetUniform("view", Matrix4.Identity);
            pos2DCol4ShaderProgram.SetUniform("projection", Matrix4.Identity);
            pos2DCol4ShaderProgram.SetUniform("colourFactor", 1f);
            flatColourShaderProgram = new ShaderProgram(ShaderCode.VertexShaderPos3, ShaderCode.PixelShaderFlatColour);
            flatColourShaderProgram.SetUniform("model", Matrix4.Identity);
            flatColourShaderProgram.SetUniform("view", Matrix4.Identity);
            flatColourShaderProgram.SetUniform("projection", Matrix4.Identity);
            flatColourShaderProgram.SetUniform("flatColour", new Vector4(1f, 1f, 1f, 1f));
            fullShaderProgram = new ShaderProgram(ShaderCode.VertexShaderPos3TexNorm, ShaderCode.PixelShaderFull);
            fullShaderProgram.SetUniform("model", Matrix4.Identity);
            fullShaderProgram.SetUniform("view", Matrix4.Identity);
            fullShaderProgram.SetUniform("projection", Matrix4.Identity);
            fullShaderProgram.SetUniform("lightCol", new Vector3(1f, 1f, 1f));
            fullShaderProgram.SetUniform("flatColour", new Vector4(1f, 1f, 1f, 1f));
            fullShaderProgram.SetUniform("lightEnabled", true);
            fullShaderProgram.SetUniform("textureEnabled", true);
            fullShaderProgram.SetUniform("texPos", 0f, 0f);
            fullShaderProgram.SetUniform("texScale", 1f, 1f);
            flatColourLitShaderProgram = new ShaderProgram(ShaderCode.VertexShaderPos3FlatNorm, ShaderCode.PixelShaderLitFlatColour);
            flatColourLitShaderProgram.SetUniform("model", Matrix4.Identity);
            flatColourLitShaderProgram.SetUniform("view", Matrix4.Identity);
            flatColourLitShaderProgram.SetUniform("projection", Matrix4.Identity);
            flatColourLitShaderProgram.SetUniform("flatColour", new Vector4(1f, 1f, 1f, 1f));

            base.OnLoad();
        }

        // Free stuff when the window is being closed
        protected override void OnUnload() {
            pos2DCol4ShaderProgram?.Dispose();
            base.OnUnload();
        }

        // This gets called every 1/60 of a second. AI updates etc.
        protected override void OnUpdateFrame(FrameEventArgs e) {
            base.OnUpdateFrame(e);

            // Check keypresses
            GetKeyboardInput();

            if (bJustLoaded) {  // Resuming a mission after a load
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
                if (ThisMission is not null && swLastTick.ElapsedMilliseconds > 300) {
                    swLastTick.Restart();
                    // --- Resolve clock tick stuff
                    MissionClockTick();
                }
                return;
            }

            // Any travel/combat to update?
            if (TravelDetails is not null) TravelDetails.ClockTickProcessor();

            // Not on mission screen, so tick the clock & check if we died
            if (GalaxyMap.MapIsInitialised) {
                Clock.AddMilliseconds(swLastTick.ElapsedMilliseconds);
                swLastTick.Restart();
                // Is our ship destroyed?
                if (PlayerTeam.PlayerShip.Hull <= 0.0) GameOver();
                // Did all our soldiers die?
                if (PlayerTeam.SoldierCount == 0) GameOver();
            }
        }

        // Main window is resized so setup viewport again
        protected override void OnResize(ResizeEventArgs e) {
            if (!bLoaded) return;
            // Annoyingly this isn't called first when maximising the window, so we have to setup the viewport in the main render loop
            base.OnResize(e);
        }

        // Draw the main view, whatever state it's in
        protected override void OnRenderFrame(FrameEventArgs e) {
            if (!bLoaded) return;

            // Set up default OpenGL rendering parameters
            PrepareScene();

            if (!GalaxyMap.MapIsInitialised) {
                DisplayWelcomeScreen();
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
            msgBox.Display((int)MousePosition.X, (int)MousePosition.Y, flatColourShaderProgram);

            // Swap rendered surface to front
            SwapBuffers();
        }

        // Closing the window - shut down stuff
        protected override void OnClosing(CancelEventArgs e) {
            if (bLoaded && PlayerTeam.SoldierCount > 0) { 
                msgBox.PopupConfirmation("Really close the game? You will lose unsaved progress!", ReallyClose, null);
                // Cancel closing - keep the window open
                e.Cancel = true;
                return;
            }
            bLoaded = false;
            base.OnClosing(e);
        }
        private void ReallyClose() {
            bLoaded = false;
            this.Close();
        }
        #endregion // GameWindow Triggers

        #region Rendering

        private static Matrix4 flatProjectionM = Matrix4.CreateOrthographicOffCenter(0.0f, 1.0f, 1.0f, 0.0f, -1.0f, 1.0f);
        private static Matrix4 zOffsetTranslateM = Matrix4.CreateTranslation(0f, 0f, 0.01f);

        // Display a welcome screen on startup
        private void DisplayWelcomeScreen() {
            flatColourShaderProgram.SetUniform("projection", flatProjectionM);
            flatColourShaderProgram.SetUniform("view", Matrix4.Identity);
            flatColourShaderProgram.SetUniform("model", Matrix4.Identity);

            GL.ClearColor(Color.Black);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            if (iBackgroundTextureId == -1 && BackgroundTextureData is not null) {
                GL.Enable(EnableCap.Texture2D);
                iBackgroundTextureId = GL.GenTexture();
                GL.BindTexture(TextureTarget.Texture2D, iBackgroundTextureId);
                GL.TexEnv(TextureEnvTarget.TextureEnv, TextureEnvParameter.TextureEnvMode, (float)TextureEnvMode.Modulate);
                GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgb, BackgroundTextureData.Width, BackgroundTextureData.Height, 0, OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, BackgroundTextureData.Scan0);
                Textures.SetTextureParameters();
            }

            if (iBackgroundTextureId != -1) {
                fullShaderProgram.SetUniform("projection", flatProjectionM);
                fullShaderProgram.SetUniform("view", Matrix4.Identity);
                fullShaderProgram.SetUniform("model", zOffsetTranslateM);
                GL.Enable(EnableCap.Texture2D);
                GL.Disable(EnableCap.DepthTest);
                fullShaderProgram.SetUniform("textureEnabled", true);
                fullShaderProgram.SetUniform("lightEnabled", false);
                fullShaderProgram.SetUniform("flatColour", new Vector4(1f, 1f, 1f, 1f));
                fullShaderProgram.SetUniform("texPos", 0f, 0f);
                fullShaderProgram.SetUniform("texScale", 1f, 1f);
                GL.ActiveTexture(TextureUnit.Texture0);
                GL.BindTexture(TextureTarget.Texture2D, iBackgroundTextureId);              
                GL.UseProgram(fullShaderProgram.ShaderProgramHandle);
                Square.Textured.BindAndDraw();
            }

            TextRenderOptions tro = new() { Alignment = Alignment.TopMiddle, XPos = 0.5f, YPos = 0.2f, Scale = 0.07f, Aspect = Aspect };
            TextRenderer.DrawWithOptions($"Welcome to SpaceMercs v{Const.strVersion}", tro);

            gbLoadGame!.Display((int)MousePosition.X, (int)MousePosition.Y, flatColourShaderProgram);
            gbNewGame!.Display((int)MousePosition.X, (int)MousePosition.Y, flatColourShaderProgram);
            gbExitGame!.Display((int)MousePosition.X, (int)MousePosition.Y, flatColourShaderProgram);
        }

        // Display a set of circles at incremental radii to debug positions
        private void DisplayDebuggingCircles() {
            Matrix4 projectionM = Matrix4.CreateOrthographicOffCenter(0.0f, 1.0f, 1.0f, 0.0f, -1.0f, 1.0f);
            flatColourShaderProgram.SetUniform("projection", projectionM);
            flatColourShaderProgram.SetUniform("view", Matrix4.CreateTranslation(0.5f, 0.5f, 0.0f));
            flatColourShaderProgram.SetUniform("model", Matrix4.CreateScale(0.5f));
            flatColourShaderProgram.SetUniform("flatColour", new Vector4(1f, 1f, 1f, 1f));
            GL.UseProgram(flatColourShaderProgram.ShaderProgramHandle);
            Circle.Circle64.Bind();
            Circle.Circle64.Draw();
            flatColourShaderProgram.SetUniform("model", Matrix4.CreateScale(0.45f));
            GL.UseProgram(flatColourShaderProgram.ShaderProgramHandle);
            Circle.Circle64.Draw();
            flatColourShaderProgram.SetUniform("model", Matrix4.CreateScale(0.4f));
            GL.UseProgram(flatColourShaderProgram.ShaderProgramHandle);
            Circle.Circle64.Draw();
            flatColourShaderProgram.SetUniform("model", Matrix4.CreateScale(0.35f));
            GL.UseProgram(flatColourShaderProgram.ShaderProgramHandle);
            Circle.Circle64.Draw();
            flatColourShaderProgram.SetUniform("model", Matrix4.CreateScale(0.3f));
            GL.UseProgram(flatColourShaderProgram.ShaderProgramHandle);
            Circle.Circle64.Draw();
            flatColourShaderProgram.SetUniform("model", Matrix4.CreateScale(0.25f));
            GL.UseProgram(flatColourShaderProgram.ShaderProgramHandle);
            Circle.Circle64.Draw();
            flatColourShaderProgram.SetUniform("model", Matrix4.CreateScale(0.2f));
            GL.UseProgram(flatColourShaderProgram.ShaderProgramHandle);
            Circle.Circle64.Draw();
            flatColourShaderProgram.SetUniform("model", Matrix4.CreateScale(0.15f));
            GL.UseProgram(flatColourShaderProgram.ShaderProgramHandle);
            Circle.Circle64.Draw();
            flatColourShaderProgram.SetUniform("model", Matrix4.CreateScale(0.1f));
            GL.UseProgram(flatColourShaderProgram.ShaderProgramHandle);
            Circle.Circle64.Draw();
            Circle.Circle64.Unbind();
        }

        // Set up the scene ready for rendering
        private void PrepareScene() {
            MakeCurrent();
            GL.Viewport(0, 0, Size.X, Size.Y);
            GL.ClearColor(Color.Black);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            GL.CullFace(CullFaceMode.Back);
            GL.ShadeModel(ShadingModel.Smooth);
            GL.Enable(EnableCap.DepthTest);
            GL.Disable(EnableCap.Blend);
            GL.DepthFunc(DepthFunction.Lequal);
            GL.DepthMask(true);
        }

        public Matrix4 ViewMatrix {
            get {
                return Matrix4.CreateTranslation(-fMissionViewX, -fMissionViewY, -fMissionViewZ);
            }
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
                if (GalaxyMap.MapIsInitialised) {
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
                    if (aoSelected is null) {
                        fMapViewX = CurrentSystem?.MapPos.X ?? 0.0f;
                        fMapViewY = CurrentSystem?.MapPos.Y ?? 0.0f;
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
                bShowGridLines = !bShowGridLines;
            }
            if (IsKeyPressed(Keys.V)) {  // Toggle on/off fading of unvisited stars
                bFadeUnvisited = !bFadeUnvisited;
            }
            if (IsKeyPressed(Keys.R)) {  // Toggle on/off range circles
                bShowRangeCircles = !bShowRangeCircles;
            }
            if (IsKeyPressed(Keys.A)) {  // Toggle on/off trade routes
                bShowTradeRoutes = !bShowTradeRoutes;
            }
            if (IsKeyPressed(Keys.F)) {  // Toggle on/off ownership flags
                bShowFlags = !bShowFlags;
            }
            if (IsKeyPressed(Keys.P)) {  // Toggle on/off system population
                bShowPop = !bShowPop;
            }
            MapHover();
        }

        // Mouse handling
        protected override void OnMouseMove(MouseMoveEventArgs e) {
            if (!GalaxyMap.MapIsInitialised) {
                gbLoadGame?.IsHover((int)e.X, (int)e.Y);
                gbNewGame?.IsHover((int)e.X, (int)e.Y);
                gbExitGame?.IsHover((int)e.X, (int)e.Y);
                return;
            }
            if (PlayerTeam is null) return;
            if (msgBox.Active) {
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
                fMapViewX -= (float)(e.DeltaX) * fScale;
                fMapViewY += (float)(e.DeltaY) * fScale;
                if (CurrentSystem is not null) {
                    if (fMapViewX < (CurrentSystem.MapPos.X - Const.MaximumScrollRange)) fMapViewX = (CurrentSystem.MapPos.X - Const.MaximumScrollRange);
                    if (fMapViewX > (CurrentSystem.MapPos.X + Const.MaximumScrollRange)) fMapViewX = (CurrentSystem.MapPos.X + Const.MaximumScrollRange);
                    if (fMapViewY < (CurrentSystem.MapPos.Y - Const.MaximumScrollRange)) fMapViewY = (CurrentSystem.MapPos.Y - Const.MaximumScrollRange);
                    if (fMapViewY > (CurrentSystem.MapPos.Y + Const.MaximumScrollRange)) fMapViewY = (CurrentSystem.MapPos.Y + Const.MaximumScrollRange);
                }
            }

            // Hover over GUI objects
            gbRenameObject?.IsHover((int)e.X, (int)e.Y);
            gbFlyTo?.IsHover((int)e.X, (int)e.Y);
            gbViewColony?.IsHover((int)e.X, (int)e.Y);
            gbScan?.IsHover((int)e.X, (int)e.Y);
            gbHyperspace?.IsHover((int)e.X, (int)e.Y);
            if (view == ViewMode.ViewMap) MapHover();
        }
        protected override void OnMouseDown(MouseButtonEventArgs e) {
            if (PlayerTeam == null) return;
            if (msgBox.Active) return;
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
            if (!GalaxyMap.MapIsInitialised) {
                if (e.Button == MouseButton.Left) {
                    if (gbLoadGame?.CaptureClick((int)MousePosition.X, (int)MousePosition.Y) ?? false) return;
                    if (gbNewGame?.CaptureClick((int)MousePosition.X, (int)MousePosition.Y) ?? false) return;
                    if (gbExitGame?.CaptureClick((int)MousePosition.X, (int)MousePosition.Y) ?? false) return;
                }
                return;
            }
            if (PlayerTeam == null) return;
            if (msgBox is not null && msgBox.Active) {
                msgBox.CaptureClick((int)MousePosition.X, (int)MousePosition.Y);
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
                if (gbRenameObject!.CaptureClick((int)MousePosition.X, (int)MousePosition.Y)) return;
                if (gbFlyTo!.CaptureClick((int)MousePosition.X, (int)MousePosition.Y)) return;
                if (gbViewColony!.CaptureClick((int)MousePosition.X, (int)MousePosition.Y)) return;
                if (gbScan!.CaptureClick((int)MousePosition.X, (int)MousePosition.Y)) return;
                if (gbHyperspace!.CaptureClick((int)MousePosition.X, (int)MousePosition.Y)) return;
                if (aoHover != null) aoSelected = aoHover;
                SetSelection();
                CheckMenuClick();
            }
        }
        protected override void OnMouseWheel(MouseWheelEventArgs e) {
            if (PlayerTeam == null) return;
            if (msgBox.Active) return;

            float delta;
            if (IsKeyDown(Keys.LeftControl) || IsKeyDown(Keys.RightControl)) delta = -e.OffsetY * 2.0f;
            else if (IsKeyDown(Keys.LeftShift) || IsKeyDown(Keys.RightShift)) delta = -e.OffsetY / 4.0f;
            else delta = -e.OffsetY / 1.5f;

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
            if (view == ViewMode.ViewSystem) {
                DoubleClick_System();
                return;
            }
            if (e.Button == MouseButton.Left) {
                if (aoHover != null) aoSelected = aoHover;
                // Zoom in to system
                if (view == ViewMode.ViewMap && aoSelected != null && aoSelected is Star st) {
                    if (st.Visited) {
                        SystemStar = st;
                        if (!st.bGenerated) st.GeneratePlanets(GalaxyMap.PlanetDensity);
                        view = ViewMode.ViewSystem;
                        aoSelected = null;
                        gbRenameObject?.Deactivate();
                        gbFlyTo?.Deactivate();
                        gbViewColony?.Deactivate();
                        gbScan?.Deactivate();
                        gbHyperspace?.Deactivate();
                    }
                    else {
                        msgBox.PopupMessage("You have not yet visited that system");
                    }
                }
            }
        }
        private void CheckMenuClick() {
            if (gpMenu!.HoverID == -1) return;
            switch ((uint)gpMenu.HoverID) {
                case I_New:
                    if (GalaxyMap.MapIsInitialised) {
                        msgBox.PopupConfirmation("You are in the middle of a game.\nGenerating a new game will lose all unsaved progress.\nAre you sure you want to continue?", NewGame_Continue);
                    }
                    else NewGame_Continue();
                    return;
                case I_Load: LoadGame(); return;
                case I_Save: SaveGame(); return;
                case I_Exit:
                    if (GalaxyMap.MapIsInitialised == true) {
                        msgBox.PopupConfirmation("You are in the middle of a game.\nExiting will lose all unsaved progress.\nAre you sure you want to continue?", this.Close);
                    }
                    return;
                case I_ViewShip:
                    if (GalaxyMap.MapIsInitialised) SetupShipView();
                    return;
                case I_ViewTeam:
                    if (!GalaxyMap.MapIsInitialised) return;
                    TeamView tv = new TeamView(PlayerTeam);
                    tv.ShowDialog();
                    return;
                case I_ViewColony:
                    if (!GalaxyMap.MapIsInitialised) return;
                    if (PlayerTeam.CurrentPosition is not HabitableAO hao || hao.BaseSize == 0) return;
                    // Close existing colonyview if open
                    foreach (Form f in Application.OpenForms) {
                        if (f.GetType() == typeof(ColonyView)) { f.Close(); break; }
                    }
                    ColonyView cv = new ColonyView(PlayerTeam, RunMission, Clock);
                    cv.Show();
                    return;
                case I_ViewRaces:
                    if (!GalaxyMap.MapIsInitialised) return;
                    RaceView rv = new RaceView(PlayerTeam);
                    rv.Show();
                    return;
                case I_OptionsLabels: bShowLabels = !bShowLabels; return;
                case I_OptionsGridLines: bShowGridLines = !bShowGridLines; return;
                case I_OptionsFadeUnvisited: bFadeUnvisited = !bFadeUnvisited; return;
                case I_OptionsRangeCircles: bShowRangeCircles = !bShowRangeCircles; return;
                case I_OptionsTradeRoutes: bShowTradeRoutes = !bShowTradeRoutes; return;
                case I_OptionsFlags: bShowFlags = !bShowFlags; return;
                case I_OptionsColonies: bShowColonies = !bShowColonies; return;
                case I_OptionsPopulation: bShowPop = !bShowPop; return;
                case I_MissionDetails: DisplayMissionDetails();  return;
                case I_MissionLabels: PlayerTeam.Mission_ShowLabels = !PlayerTeam.Mission_ShowLabels; return;
                case I_MissionStatBars: PlayerTeam.Mission_ShowStatBars = !PlayerTeam.Mission_ShowStatBars; return;
                case I_MissionTravel: PlayerTeam.Mission_ShowTravel = !PlayerTeam.Mission_ShowTravel; return;
                case I_MissionPath: PlayerTeam.Mission_ShowPath = !PlayerTeam.Mission_ShowPath; return;
                case I_MissionEffects: PlayerTeam.Mission_ShowEffects = !PlayerTeam.Mission_ShowEffects; return;
                case I_MissionDetection: PlayerTeam.Mission_ViewDetection = !PlayerTeam.Mission_ViewDetection; return;
                case I_MissionFastAI: PlayerTeam.Mission_FastAI = !PlayerTeam.Mission_FastAI; return;
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
            PlayerTeam = new Team(ng, StaticData.HumanRace);
            if (PlayerTeam.CurrentPosition is not HabitableAO hao || hao.Colony == null) throw new Exception("Did not set up PlayerTeam correctly - not at home planet!");
        }

        // External triggers
        private void NewGame_Continue() {
            // Set up the new game dialog box
            NewGame ng = new NewGame();
            DialogResult res = ng.ShowDialog();
            if (res == DialogResult.OK) {
                try {
                    MakeCurrent();
                    GalaxyMap.Generate(ng, Clock);
                    SetupNewGame(ng);
                    view = ViewMode.ViewMap;
                    SetAOButtonsOnGUI(aoSelected);
                    fMapViewZ = Const.InitialMapViewZ;
                    fMapViewX = fMapViewY = 0;
                }
                catch (Exception ex) {
                    msgBox.PopupMessage(ex.ToString());
                }
            }
        }
        public void SaveGame() {
            if (GalaxyMap.MapIsInitialised == false) return;
            if (TravelDetails != null) TravelDetails.Pause();

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
                    msgBox.PopupMessage(ex.ToString());
                }
            }
            TravelDetails?.Unpause();
        }
        public void LoadGame() {
            // Need to check to see if we already have a map set up
            if (GalaxyMap.MapIsInitialised == true) {
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
                    Tuple<Map, Team, Travel?> tp = LoadGame(ofd.FileName);
                    // If we get here then it loaded OK, so overwrite everything
                    GalaxyMap = tp.Item1;
                    PlayerTeam = tp.Item2;
                    TravelDetails = tp.Item3;

                    // Setup GUI
                    aoSelected = null;
                    aoHover = null;
                    fMapViewZ = Const.InitialMapViewZ;
                    bLoaded = true;
                    RecentlyVisited.Clear();

                    // Close down any windows
                    CloseAllDialogs();

                    // Zoom to current location
                    SystemStar = PlayerTeam.CurrentPosition.GetSystem();
                    fMapViewX = PlayerTeam.CurrentPosition.GetMapLocation().X;
                    fMapViewY = PlayerTeam.CurrentPosition.GetMapLocation().Y;
                    lastLoad = DateTime.Now;
                    SetAOButtonsOnGUI(aoSelected);
                    view = ViewMode.ViewMap;
                    msgBox?.Clear();

                    // If we're on a mission then set it up
                    // Do this by configuring a flag that is consumed at the top of OnUpdateFrame()
                    bJustLoaded = true;
                    return;
                }
                catch (Exception ex) {
                    MessageBox.Show(ex.ToString());
                }
            }
        }
        private void GameOver() {
            msgBox.PopupMessage("Game Over!\nPlease start a new game or load a saved game to continue playing", ClearGame);
        }
        private void ClearGame() {
            GalaxyMap.Unload();
            PlayerTeam = Team.Empty();
            msgBox?.Clear();
        }
        #endregion // Game Management

        #region Clock Tick
        // Update the global clock
        public void UpdateCurrentTime(DateTime newTime) {
            DateTime oldTime = Clock.CurrentTime;
            Clock.SetTime(newTime);
            TimeSpan tDiff = (newTime - oldTime);
            RunClockTickUpdate(tDiff);
        }

        private void RunClockTickUpdate(TimeSpan tDiff) {
            CheckPopulationGrowth(tDiff);
            // TODO
        }

        // Finish travelling
        public void ArriveAt(AstronomicalObject aoTo) {
            // Close colonyview if open
            foreach (Form f in Application.OpenForms) {
                if (f.GetType() == typeof(ColonyView)) { f.Close(); break; }
            }
            aoTo.GetSystem().SetVisited();
            msgBox.PopupMessage("You have arrived at your destination");
            PlayerTeam.SetPosition(aoTo);
            if (PlayerTeam.CurrentPosition is HabitableAO hao && hao.Colony != null) {
                if (!hao.Colony.Owner.Known) {
                    msgBox.PopupMessage($"You arrive at a planet colonised by a previously unknown alien race!\nThey announce themselves to be called {hao.Colony.Owner.Name}");
                    hao.Colony.Owner.SetAsKnownBy(PlayerTeam);
                }
            }
            TravelDetails = null;
            SystemStar = PlayerTeam.CurrentPosition.GetSystem();
            fMapViewX = PlayerTeam.CurrentPosition.GetMapLocation().X;
            fMapViewY = PlayerTeam.CurrentPosition.GetMapLocation().Y;
            SetAOButtonsOnGUI(aoSelected);
        }

        private void CheckPopulationGrowth(TimeSpan tDiff) {
            // tDiff in fraction of a year
            int oldHumanPop = StaticData.HumanRace.Population;

            // Get all currently unresearchable techs
            HashSet<IResearchable> oldUnresearchable = PlayerTeam.UnresearchableItems.ToHashSet();

            int maxLevel = PlayerTeam.MaximumSoldierLevel;

            // Go through all races and check colonies for growth. etc.
            foreach (Race rc in StaticData.Races) {
                if (!rc.IsPlayer) rc.CheckForNewColonySystems(msgBox, Clock);
                rc.CheckGrowthForAllColonies(Clock);
                rc.CheckColonySeeds(msgBox, tDiff, Clock);
                rc.CheckResearch(tDiff, maxLevel);
            }

            // Announce pop growth
            int newHumanPop = StaticData.HumanRace.Population;
            if (newHumanPop != oldHumanPop) {
                msgBox.PopupMessage($"The Human population has increased to {newHumanPop}");
            }

            // See if we can now research any of the previously unresearchable techs. If so then announce it.
            IEnumerable<IResearchable> newResearchable = oldUnresearchable.Except(PlayerTeam.UnresearchableItems);

            if (newResearchable.Any()) {
                string msg = $"Human scientists have made technological advances.\n";
                if (newResearchable.OfType<BaseItemType>().Any()) {
                    msg += $"The following new research is now available:\n{String.Join("\n", newResearchable.OfType<BaseItemType>().Select(it => it.Name))}\n";
                }
                if (newResearchable.OfType<MaterialType>().Any()) {
                    msg += $"The following new materials have been discovered:\n{String.Join("\n", newResearchable.OfType<MaterialType>().Select(it => it.Name))}";
                }
                msgBox.PopupMessage(msg);
            }
        }
        #endregion // Clock Tick

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
            SetAOButtonsOnGUI(aoSelected);
        }
    }
}