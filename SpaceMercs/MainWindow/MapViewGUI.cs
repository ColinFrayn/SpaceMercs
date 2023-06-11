using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using SpaceMercs.Dialogs;
using SpaceMercs.Graphics;
using SpaceMercs.Graphics.Shapes;
using Keys = OpenTK.Windowing.GraphicsLibraryFramework.Keys;

namespace SpaceMercs.MainWindow {
    partial class MapView {
        private GUIButton gbRenameObject, gbFlyTo, gbViewColony, gbScan;
        private GUIPanel gpMenu, gpSubMenu, gpFileMenu, gpViewMenu, gpOptionsMenu, gpMissionMenu;
        private static readonly float toggleY = 0.16f, toggleX = 0.99f, toggleStep = 0.04f, toggleScale = 0.035f;
        private AstronomicalObject lastAOHover = null;

        #region Menu Codes
        // GUIPanel for main menu
        public const uint I_Menu = 11000;
        public const uint I_File = 11010;
        public const uint I_New = 11011;
        public const uint I_Load = 11012;
        public const uint I_Save = 11013;
        public const uint I_Exit = 11014;
        public const uint I_View = 11020;
        public const uint I_ViewShip = 11021;
        public const uint I_ViewTeam = 11022;
        public const uint I_ViewColony = 11023;
        public const uint I_ViewRaces = 11024;
        public const uint I_Options = 11030;
        public const uint I_OptionsLabels = 11031;
        public const uint I_OptionsGridLines = 11032;
        public const uint I_OptionsFadeUnvisited = 11033;
        public const uint I_OptionsRangeCircles = 11034;
        public const uint I_OptionsTradeRoutes = 11035;
        public const uint I_OptionsFlags = 11036;
        public const uint I_OptionsColonies = 11037;
        public const uint I_Mission = 11040;
        public const uint I_MissionDetails = 11041;
        public const uint I_MissionLabels = 11042;
        public const uint I_MissionStatBars = 11043;
        public const uint I_MissionTravel = 11044;
        public const uint I_MissionPath = 11045;
        public const uint I_MissionEffects = 11046;
        public const uint I_MissionDetection = 11047;
        #endregion // Menu Codes

        // Draw the GUI elements
        private void DrawGUI() {
            Matrix4 projectionM = Matrix4.CreateOrthographicOffCenter(0.0f, 1.0f, 1.0f, 0.0f, -1.0f, 1.0f);
            flatColourShaderProgram.SetUniform("projection", projectionM);
            flatColourShaderProgram.SetUniform("view", Matrix4.Identity);
            flatColourShaderProgram.SetUniform("model", Matrix4.Identity);
            pos2DCol4ShaderProgram.SetUniform("projection", projectionM);
            pos2DCol4ShaderProgram.SetUniform("view", Matrix4.Identity);
            pos2DCol4ShaderProgram.SetUniform("model", Matrix4.Identity);
            fullShaderProgram.SetUniform("projection", projectionM);
            fullShaderProgram.SetUniform("view", Matrix4.Identity);
            fullShaderProgram.SetUniform("model", Matrix4.Identity);

            // Set up scene
            GL.DepthMask(true);
            GL.Clear(ClearBufferMask.DepthBufferBit);

            // Display the current date and time
            TextRenderer.DrawAt(Const.dtTime.ToString("F"), Alignment.TopLeft, 0.03f, Aspect, 0.01f, 0.01f);

            // Draw stuff that's only visible when there's a game underway
            if (!bLoaded || !GalaxyMap.bMapSetup) return;

            // Draw details of currently selected object
            if (aoSelected != null) {
                DisplaySelectionText();
            }

            // Display the player's remaining cash reserves
            TextRenderer.DrawAt($"{PlayerTeam.Cash.ToString("F2")} credits", Alignment.TopRight, 0.03f, Aspect, 0.99f, 0.01f);

            // Toggles
            DrawToggles();
            if (view == ViewMode.ViewMap) DrawMapToggles();
            if (view == ViewMode.ViewSystem) DrawSystemToggles();

            // Main menu (if not travelling)
            if (TravelDetails == null) {
                gpMenu.Display((int)MousePosition.X, (int)MousePosition.Y, fullShaderProgram);
                gpSubMenu.GetItem(I_View)?.Enable();
                gpSubMenu.GetItem(I_Options)?.Enable();
                gpSubMenu.GetItem(I_Mission)?.Disable();
            }

            // Hover info for the current setup
            DrawGUIHoverInfo();

            // If we're travelling then display that
            if (TravelDetails != null) {
                if (TravelDetails.GameOver) TravelDetails = null;
                else TravelDetails.Display(fullShaderProgram, pos2DCol4ShaderProgram);
            }
            // Display the various GUI Buttons
            else {
                SetAOButtonsOnGUI(aoSelected);
                gbRenameObject.Display((int)MousePosition.X, (int)MousePosition.Y, flatColourShaderProgram);
                gbFlyTo.Display((int)MousePosition.X, (int)MousePosition.Y, flatColourShaderProgram);
                gbViewColony.Display((int)MousePosition.X, (int)MousePosition.Y, flatColourShaderProgram);
                gbScan.Display((int)MousePosition.X, (int)MousePosition.Y, flatColourShaderProgram);
            }
        }

        // Display the text labels required for the GUI
        private void DisplaySelectionText() {
            float dTLScale = 0.03f;
            float dXMargin = 0.01f;
            float dYStart = 0.78f;
            float dYGap = 0.04f;

            // Get the text strings
            if (aoSelected == null) return;

            string tl1 = "", tl2 = "", tl3 = "", tl4 = "", tl5 = "";

            if (aoSelected.AOType == AstronomicalObject.AstronomicalObjectType.Star) {
                Star st = (Star)aoSelected;
                if (!string.IsNullOrEmpty(st.Name)) tl1 = st.Name;
                else tl1 = "Unnamed Star";
                tl2 = "M = " + Math.Round(st.Mass, 2).ToString() + " Sol";
                tl3 = "R = " + Math.Round(st.radius / Const.Million, 0).ToString() + " Mm";
                tl4 = "Type " + st.StarType;
                tl5 = "(" + Math.Round(st.MapPos.X, 1) + "," + Math.Round(st.MapPos.Y, 1) + ")";
            }
            if (aoSelected.AOType == AstronomicalObject.AstronomicalObjectType.Planet) {
                Planet pl = (Planet)aoSelected;
                if (!string.IsNullOrEmpty(pl.Name)) tl1 = pl.Name;
                else tl1 = "Unnamed Planet";
                tl2 = pl.Type.ToString();
                tl3 = "R = " + Math.Round(pl.radius / 1000.0, 0).ToString() + "km";
                tl4 = $"Temp = {pl.Temperature}K";
                tl5 = "Orbit = " + Math.Round(pl.orbit / Const.AU, 2).ToString() + " AU";
                //tl4 = "Dist = " + Math.Round(GraphicsFunctions.ViewDistance(pl), 3).ToString() + "Gm";
            }
            if (aoSelected.AOType == AstronomicalObject.AstronomicalObjectType.Moon) {
                Moon mn = (Moon)aoSelected;
                tl1 = "Moon " + (mn.ID + 1).ToString();
                tl2 = mn.Type.ToString();
                tl3 = "R = " + Math.Round(mn.radius / 1000.0, 0).ToString() + "km";
                tl4 = $"Temp = {mn.Temperature}K";
                tl5 = "Orbit = " + Math.Round(mn.orbit / Const.Million, 0).ToString() + " Mm";
            }

            // Display the text details of the selected object
            TextRenderer.DrawAt(tl1, Alignment.TopLeft, dTLScale, Aspect, dXMargin, dYStart);
            TextRenderer.DrawAt(tl2, Alignment.TopLeft, dTLScale, Aspect, dXMargin, dYStart + dYGap);
            TextRenderer.DrawAt(tl3, Alignment.TopLeft, dTLScale, Aspect, dXMargin, dYStart + dYGap * 2f);
            TextRenderer.DrawAt(tl4, Alignment.TopLeft, dTLScale, Aspect, dXMargin, dYStart + dYGap * 3f);
            TextRenderer.DrawAt(tl5, Alignment.TopLeft, dTLScale, Aspect, dXMargin, dYStart + dYGap * 4f);
        }

        // Draw toggles for all screens (L)
        private void DrawToggles() {
            TextRenderer.DrawAt("L", Alignment.TopRight, toggleScale, Aspect, toggleX, toggleY + toggleStep, bShowLabels ? Color.White : Color.DimGray);
        }

        // Draw toggles for the System View (C)
        private void DrawSystemToggles() {
            TextRenderer.DrawAt("C", Alignment.TopRight, toggleScale, Aspect, toggleX, toggleY + toggleStep * 2f, bShowColonies ? Color.White : Color.DimGray);
        }

        // Draw toggles for the map screen (RFGAV)
        private void DrawMapToggles() {
            TextRenderer.DrawAt("A", Alignment.TopRight, toggleScale, Aspect, toggleX, toggleY + toggleStep * 2f, bShowTradeRoutes ? Color.White : Color.DimGray);
            TextRenderer.DrawAt("F", Alignment.TopRight, toggleScale, Aspect, toggleX, toggleY + toggleStep * 3f, bShowFlags ? Color.White : Color.DimGray);
            TextRenderer.DrawAt("G", Alignment.TopRight, toggleScale, Aspect, toggleX, toggleY + toggleStep * 4f, bShowGridLines ? Color.White : Color.DimGray);
            TextRenderer.DrawAt("R", Alignment.TopRight, toggleScale, Aspect, toggleX, toggleY + toggleStep * 5f, bShowRangeCircles ? Color.White : Color.DimGray);
            TextRenderer.DrawAt("V", Alignment.TopRight, toggleScale, Aspect, toggleX, toggleY + toggleStep * 6f, bFadeUnvisited ? Color.White : Color.DimGray);
        }

        // Setup a mini window to show details of the current hover target
        private List<string> SetupGUIHoverInfo() {
            List<string> strHoverText = new List<string>();

            // Check for AO hover.
            if (aoHover is null || aoSelected is null) return strHoverText;
            double dist = AstronomicalObject.CalculateDistance(aoSelected, aoHover);
            if (aoHover.AOType == AstronomicalObject.AstronomicalObjectType.Star && aoHover is Star stHover) {
                if (stHover.Name.Length == 0) {
                    strHoverText.Add("<Unnamed>");
                }
                else {
                    strHoverText.Add(stHover.Name);
                }
                strHoverText.Add("Type " + stHover.StarType);
                if (dist > 0.0 && aoHover.GetSystem() != aoSelected.GetSystem()) {
                    strHoverText.Add("Dist: " + Math.Round(dist / Const.LightYear, 1).ToString() + " ly");
                }
            }
            else if (aoHover.AOType == AstronomicalObject.AstronomicalObjectType.Planet && aoHover is Planet plHover) {
                if (string.IsNullOrEmpty(plHover?.Name)) {
                    strHoverText.Add("<Unnamed>");
                }
                else {
                    strHoverText.Add(plHover.Name);
                }
                strHoverText.Add(plHover.Type.ToString());
                if (aoSelected is not null) {
                    if (aoSelected.GetSystem() == aoHover.GetSystem()) {
                        strHoverText.Add("Dist: " + Math.Round(dist / Const.Billion, 1).ToString() + " Gm");
                    }
                    else {
                        strHoverText.Add("Dist: " + Math.Round(dist / Const.LightYear, 1).ToString() + " ly");
                    }
                }
            }
            else if (aoHover.AOType == AstronomicalObject.AstronomicalObjectType.Moon && aoHover is Moon mnHover) {
                strHoverText.Add(mnHover.Type.ToString() + " Moon");
                if (aoSelected != null) {
                    if (aoSelected.GetSystem() == aoHover.GetSystem()) {
                        strHoverText.Add("Dist: " + Math.Round(dist / Const.Billion, 1).ToString() + " Gm");
                    }
                    else {
                        strHoverText.Add("Dist: " + Math.Round(dist / Const.LightYear, 1).ToString() + " ly");
                    }
                }
            }
            lastAOHover = aoHover;
            return strHoverText;
        }

        // Draw the hover info when hovering over an object with "Alt" pressed
        private void DrawGUIHoverInfo() {
            if (aoHover == null) return;
            if (!IsKeyDown(Keys.LeftAlt) && !IsKeyDown(Keys.RightAlt)) return; // Only display if Alt is held down

            List<string> strHoverText = SetupGUIHoverInfo();
            if (!strHoverText.Any()) return;

            // -- Draw a dark grey box with a black background, and overlay the relevant text
            float thWidth = 0f;
            foreach (string str in strHoverText) {
                TextMeasure size = TextRenderer.MeasureText(str);
                if (size.Width > thWidth) thWidth = size.Width;
            }
            float hoverTextScale = 0.03f;
            thWidth *= hoverTextScale / TextRenderer.FontSize;
            float thHeight = hoverTextScale * strHoverText.Count() * 1.3f;

            // First, calculate box dimensions
            float xx = MousePosition.X / (float)Size.X;
            float yy = MousePosition.Y / (float)Size.Y;
            float xSep = 0.01f, ySep = 0.01f;
            float dx = (xx > 0.5) ? xx - xSep - thWidth : xx + xSep; // (xx > 0.5) ? xx - thWidth + xSep : xx + xSep;
            float dy = (yy > 0.5) ? yy - thHeight - ySep : yy + ySep + (hoverTextScale * 0.5f);

            Matrix4 translateM = Matrix4.CreateTranslation(dx, dy, -0.1f);
            Matrix4 scaleM = Matrix4.CreateScale(thWidth / Aspect, thHeight, 1f);
            Matrix4 modelM = scaleM * translateM;
            flatColourShaderProgram.SetUniform("model", modelM);
            flatColourShaderProgram.SetUniform("flatColour", new Vector4(0f, 0f, 0f, 1.0f));
            GL.UseProgram(flatColourShaderProgram.ShaderProgramHandle);
            Square.Flat.BindAndDraw();

            translateM = Matrix4.CreateTranslation(dx, dy, -0.05f);
            modelM = scaleM * translateM;
            flatColourShaderProgram.SetUniform("model", modelM);
            flatColourShaderProgram.SetUniform("flatColour", new Vector4(0.7f, 0.7f, 0.7f, 1.0f));
            GL.UseProgram(flatColourShaderProgram.ShaderProgramHandle);
            Square.Lines.BindAndDraw();

            foreach (string str in strHoverText) {
                TextRenderer.DrawAt(str, Alignment.TopLeft, hoverTextScale, Aspect, dx, dy, Color.LightGray);
                dy += hoverTextScale * 1.15f;
            }
        }

        // Set up the various GUI elements of class GUIObject that need to be initialised
        private void SetupGUIElements() {
            // "View the colony at the current location" button
            gbViewColony = new GUIButton("Colony", this, OpenColonyViewDialog);
            gbViewColony.SetPosition(0.01f, 0.07f);
            gbViewColony.SetSize(0.065f, 0.035f);

            // "Rename this planet" button
            gbRenameObject = new GUIButton("Rename", this, OpenRenameObjectDialog);
            gbRenameObject.SetPosition(0.08f, 0.07f);
            gbRenameObject.SetSize(0.065f, 0.035f);

            // "Fly to this target" button
            gbFlyTo = new GUIButton("Fly To", this, OpenFlyToDialog);
            gbFlyTo.SetPosition(0.15f, 0.07f);
            gbFlyTo.SetSize(0.065f, 0.035f);

            // "Scan this planet" button
            gbScan = new GUIButton("Scan", this, OpenScanPlanetDialog);
            gbScan.SetPosition(0.23f, 0.07f);
            gbScan.SetSize(0.065f, 0.035f);

            // "Start a new game" button
            gbNewGame = new GUIButton("Start New Game", this, NewGame_Continue);
            gbNewGame.SetPosition(0.35f, 0.4f);
            gbNewGame.SetSize(0.3f, 0.08f);
            gbNewGame.Activate();

            // "Load a saved game" button
            gbLoadGame = new GUIButton("Load Saved Game", this, LoadGame);
            gbLoadGame.SetPosition(0.35f, 0.55f);
            gbLoadGame.SetSize(0.3f, 0.08f);
            gbLoadGame.Activate();

            // "Exit the game" button
            gbExitGame = new GUIButton("Exit Game", this, ExitTheGame);
            gbExitGame.SetPosition(0.35f, 0.7f);
            gbExitGame.SetSize(0.3f, 0.08f);
            gbExitGame.Activate();

            // --- Configure the main menu ---

            // File menu
            gpFileMenu = new GUIPanel(this, direction: GUIPanel.PanelDirection.Vertical);
            gpFileMenu.InsertTextItem(I_New, "New Game", Aspect);
            gpFileMenu.InsertTextItem(I_Load, "Load Game", Aspect);
            gpFileMenu.InsertTextItem(I_Save, "Save Game", Aspect);
            gpFileMenu.InsertTextItem(I_Exit, "Exit Game", Aspect);

            // View menu
            gpViewMenu = new GUIPanel(this, direction: GUIPanel.PanelDirection.Vertical);
            gpViewMenu.InsertTextItem(I_ViewShip, "View Ship", Aspect);
            gpViewMenu.InsertTextItem(I_ViewTeam, "View Team", Aspect);
            gpViewMenu.InsertTextItem(I_ViewColony, "View Colony", Aspect);
            gpViewMenu.InsertTextItem(I_ViewRaces, "View Races", Aspect);

            // Skills menu
            gpOptionsMenu = new GUIPanel(this, direction: GUIPanel.PanelDirection.Vertical);
            gpOptionsMenu.InsertTextItem(I_OptionsLabels, "Labels", Aspect, () => bShowLabels);
            gpOptionsMenu.InsertTextItem(I_OptionsGridLines, "Grid Lines", Aspect, () => bShowGridLines);
            gpOptionsMenu.InsertTextItem(I_OptionsFadeUnvisited, "Fade Unvisited", Aspect, () => bFadeUnvisited);
            gpOptionsMenu.InsertTextItem(I_OptionsRangeCircles, "Range Circles", Aspect, () => bShowRangeCircles);
            gpOptionsMenu.InsertTextItem(I_OptionsTradeRoutes, "Trade Routes", Aspect, () => bShowTradeRoutes);
            gpOptionsMenu.InsertTextItem(I_OptionsFlags, "Flags", Aspect, () => bShowFlags);
            gpOptionsMenu.InsertTextItem(I_OptionsColonies, "Colonies", Aspect, () => bShowColonies);

            // Mission menu
            gpMissionMenu = new GUIPanel(this, direction: GUIPanel.PanelDirection.Vertical);
            gpMissionMenu.InsertTextItem(I_MissionDetails, "Mission Details", Aspect);
            gpMissionMenu.InsertTextItem(I_MissionLabels, "Labels", Aspect, () => bShowEntityLabels);
            gpMissionMenu.InsertTextItem(I_MissionStatBars, "Stat Bars", Aspect, () => bShowStatBars);
            gpMissionMenu.InsertTextItem(I_MissionTravel, "Travel Distance", Aspect, () => bShowTravel);
            gpMissionMenu.InsertTextItem(I_MissionPath, "Best Path", Aspect, () => bShowPath);
            gpMissionMenu.InsertTextItem(I_MissionEffects, "Effects", Aspect, () => bShowEffects);
            gpMissionMenu.InsertTextItem(I_MissionDetection, "Detection Area", Aspect, () => bViewDetection);

            // First level menus
            gpSubMenu = new GUIPanel(this, direction: GUIPanel.PanelDirection.Vertical);
            TexSpecs ts = Textures.GetTexCoords(Textures.MiscTexture.File);
            gpSubMenu.InsertIconItem(I_File, ts, true, gpFileMenu);
            ts = Textures.GetTexCoords(Textures.MiscTexture.Eye);
            gpSubMenu.InsertIconItem(I_View, ts, true, gpViewMenu);
            ts = Textures.GetTexCoords(Textures.MiscTexture.Skills);
            gpSubMenu.InsertIconItem(I_Options, ts, true, gpOptionsMenu);
            ts = Textures.GetTexCoords(Textures.MiscTexture.Mission);
            gpSubMenu.InsertIconItem(I_Mission, ts, false, gpMissionMenu);

            // Top level menu
            gpMenu = new GUIPanel(this, 0.01f, 0.15f);
            ts = Textures.GetTexCoords(Textures.MiscTexture.Menu);
            gpMenu.InsertIconItem(I_Menu, ts, true, gpSubMenu);
            gpMenu.Activate();
            gpSubMenu.Activate();

        }

        // Dialog action handlers
        public void OpenRenameObjectDialog() {
            if (aoSelected == null || aoSelected.AOType == AstronomicalObject.AstronomicalObjectType.Moon) return;
            GetString gs = new GetString();
            gs.UpdateString(aoSelected.Name);
            if (gs.ShowDialog() == DialogResult.OK && gs.strText.CompareTo(aoSelected.Name) != 0) {
                aoSelected.SetName(gs.strText);
                SetSelection();
            }
        }
        public void OpenFlyToDialog() {
            if (aoSelected == null) return;

            if (!PlayerTeam.PlayerShip.CanFly) {
                if (PlayerTeam.PlayerShip.PowerConsumption > PlayerTeam.PlayerShip.PowerGeneration) msgBox.PopupMessage("You cannot take off - power consumption exceeds generation!");
                else if (PlayerTeam.PlayerShip.Engine == null) msgBox.PopupMessage("Your ship doesn't have an engine installed!");
                else if (!PlayerTeam.PlayerShip.EngineEnabled) msgBox.PopupMessage("Your ship's engine is powered down!");
                else if (PlayerTeam.GetSpareBerths() < 0) msgBox.PopupMessage("You can't travel as you don't have enough accommodation\nfor all your active soldiers!");
                else msgBox.PopupMessage("Cannot take off - Unspecified reason (possible bug?)");
                return;
            }

            // Get journey time & check if it's ok
            double dJourneyTime = PlayerTeam.PlayerShip.CalculateTravelTime(PlayerTeam.CurrentPosition, aoSelected);
            TimeSpan ts = TimeSpan.FromSeconds(dJourneyTime);
            msgBox.PopupConfirmation(String.Format("Really travel? Journey time = {0:%d}d {0:%h}h {0:%m}m {0:%s}s", ts), () => FlyTo_Continue(dJourneyTime));
        }
        private void FlyTo_Continue(double jt) {
            // Set up that we're travelling somewhere
            if (aoSelected is null) throw new Exception("Attempting to set up travel to a null target");
            TravelDetails = new Travel(PlayerTeam.CurrentPosition, aoSelected, (float)jt, PlayerTeam, this);
        }
        private void OpenColonyViewDialog() {
            if (!GalaxyMap.bMapSetup) return;
            if (PlayerTeam.CurrentPosition?.BaseSize == 0) {
                if (PlayerTeam.CurrentPosition != null && PlayerTeam.CurrentPosition is HabitableAO hao && hao.Type != Planet.PlanetType.Gas && PlayerTeam.PlayerShip.CanFoundColony) {
                    if (!hao.Scanned) {
                        msgBox.PopupMessage("Before you found a colony you need to scan the planet\nand clear all discovered missions");
                        return;
                    }
                    if (hao.CountMissions > 0) {
                        msgBox.PopupMessage("Before you found a colony you need to clear all\navailable scanner missions");
                        return;
                    }
                    msgBox.PopupConfirmation("Really Found A Colony Here?", FoundColony_Continue);
                    return;
                }
                else {
                    msgBox.PopupMessage("No colony exists at current location");
                }
                return;
            }
            ColonyView cv = new ColonyView(PlayerTeam, RunMission);
            cv.Show();
        }
        private void FoundColony_Continue() {
            if (!(PlayerTeam.CurrentPosition is HabitableAO hao)) return;
            hao.SetupBase(StaticData.Races[0], 1);
            if (hao.GetSystem().Owner == null) {
                StaticData.Races[0].AddSystem(hao.GetSystem());
            }
            Star stTR = StaticData.Races[0].GetNearestSystemToNotIncludingSelf(hao.GetSystem());
            if (stTR != null && stTR.DistanceTo(hao.GetSystem()) <= Const.MaxTradeRouteDistInLY) {
                hao.GetSystem().AddTradeRoute(stTR);
            }
            PlayerTeam.PlayerShip.RemoveColonyBuilder();
            msgBox.PopupMessage("Colony Founded");
            SetAOButtonsOnGUI(hao);
        }
        private void OpenScanPlanetDialog() {
            if (!GalaxyMap.bMapSetup) return;
            if (PlayerTeam.CurrentPosition is null) {
                msgBox.PopupMessage("Cannot scan current location\nScanner only works on terrestrial palnets and moons.");
                return;
            }
            if (PlayerTeam.CurrentPosition?.BaseSize > 0) {
                msgBox.PopupMessage("Please visit colony for missions from this location");
                return;
            }
            // Open the ScanPlanet dialog
            ScanPlanet sp = new ScanPlanet(PlayerTeam!.CurrentPosition!, PlayerTeam!, RunMission);
            sp.ShowDialog();
            SetAOButtonsOnGUI(aoSelected);
        }
        private void ExitTheGame() {
            this.Close();
        }

        // Set the button relevant for the selected AO
        public void SetAOButtonsOnGUI(AstronomicalObject? ao) {
            gbRenameObject.Deactivate();
            gbFlyTo.Deactivate();
            if (ao == null) return;
            if (ao.AOType != AstronomicalObject.AstronomicalObjectType.Moon) {
                gbRenameObject.Activate();
            }
            if (PlayerTeam.CanTravel(ao) && ao != PlayerTeam.CurrentPosition) gbFlyTo.Activate();

            gbViewColony.Deactivate();
            if (view == ViewMode.ViewSystem && ao == PlayerTeam.CurrentPosition) {
                if (PlayerTeam.CurrentPosition?.BaseSize > 0) {
                    gbViewColony.UpdateText("Colony");
                    gbViewColony.Activate();
                }
                else if (PlayerTeam.CurrentPosition != null && PlayerTeam.CurrentPosition is HabitableAO hao && hao.Type != Planet.PlanetType.Gas) {
                    if (PlayerTeam.PlayerShip.CanFoundColony) {
                        gbViewColony.UpdateText("Colonise");
                        gbViewColony.Activate();
                    }
                }
            }

            bool bCanScanHere = false;
            if (view == ViewMode.ViewSystem && PlayerTeam.CurrentPosition != null && PlayerTeam.PlayerShip.CanScan && ao == PlayerTeam.CurrentPosition) {
                if (PlayerTeam.CurrentPosition?.BaseSize == 0) {
                    if (PlayerTeam.CurrentPosition.AOType == AstronomicalObject.AstronomicalObjectType.Planet) {
                        if (((Planet)PlayerTeam.CurrentPosition).Type != Planet.PlanetType.Gas) {
                            bCanScanHere = true;
                        }
                    }
                    if (PlayerTeam.CurrentPosition.AOType == AstronomicalObject.AstronomicalObjectType.Moon) {
                        bCanScanHere = true;
                    }
                }
            }
            if (bCanScanHere) {
                gbScan.Activate();
                if (!PlayerTeam.CurrentPosition.Scanned) gbScan.UpdateText("Scan");
                else gbScan.UpdateText("Missions");
            }
            else gbScan.Deactivate();
        }
    }
}
