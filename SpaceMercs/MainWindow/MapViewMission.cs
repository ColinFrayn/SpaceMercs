using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;
using SpaceMercs.Dialogs;
using SpaceMercs.Graphics;
using SpaceMercs.Graphics.Shapes;
using System.Diagnostics;
using System.Text;
using System.Windows.Threading;
using Keys = OpenTK.Windowing.GraphicsLibraryFramework.Keys;

namespace SpaceMercs.MainWindow {
    // Partial class including functions for handling missions
    partial class MapView {
        public enum MissionResult { Victory, Defeat, Evacuated, Aborted };
        public enum SoldierAction { None, Attack, Item };
        public MissionResult MissionOutcome { get; private set; } // Did we win?
        private int hoverx, hovery;
        private float fMissionViewX, fMissionViewY, fMissionViewZ;
        private GUIPanel gpSelect;
        private GUIIconButton? gbZoomTo1, gbZoomTo2, gbZoomTo3, gbZoomTo4;
        private GUIIconButton? gbWest, gbEast, gbNorth, gbSouth, gbAttack, gbInventory, gbUseItem, gbSearch;
        private GUIButton? gbEndTurn, gbTransition, gbEndMission;
        private readonly List<GUIIconButton> lButtons = new List<GUIIconButton>();
        private bool bGUIButtonsInitialised = false;
        private Mission? ThisMission;
        private MissionLevel CurrentLevel;
        private IEntity? SelectedEntity = null;
        private Soldier? soldierPanelCurrentHover = null;
        private bool bDragging = false;
        private int[,] DistMap;
        private bool[,] TargetMap;
        private bool[,] AoEMap;
        private bool[,] DetectionMap;
        private int AoERadius = -1;
        private List<Point>? lCurrentPath;
        private SoldierAction CurrentAction = SoldierAction.None;
        private readonly List<VisualEffect> Effects = new List<VisualEffect>();
        private readonly Stopwatch sw = Stopwatch.StartNew();
        private IEquippable? ActionItem = null;
        private Dispatcher? ThisDispatcher = null;
        private bool bAIRunning = false;
        private VertexBuffer? vbGrid = null, vbPath = null;
        private VertexArray? vaGrid = null, vaPath = null;
        private IndexBuffer? ibGrid = null;
        private HashSet<Soldier> lastSoldiers = new HashSet<Soldier>();
        private object oInitialiseLock = new object();

        // GUIPanel actions
        public const uint I_OpenDoor = 10001;
        public const uint I_CloseDoor = 10002;
        public const uint I_LockDoor = 10003;
        public const uint I_UnlockDoor = 10004;
        public const uint I_Attack = 10005;
        public const uint I_GoTo = 10006;
        public const uint I_UseItem = 10007;

        // Display parameters
        const float FrameBorder = 0.002f;
        const float ButtonSize = 0.022f;
        const float ButtonGap = 0.004f;

        // Run a mission
        public bool RunMission(Mission m) {
            if (m == null) throw new Exception("Starting MissionView with empty mission!");
            bool bCanAbort = (m.Type != Mission.MissionType.RepelBoarders);
            bool bInProgress = m.Soldiers.Any();
            lastSoldiers.Clear(); // Make sure we delete any hangover data from a previosu loaded game
            ThisMission = m;
            if (!bInProgress) ThisMission.Initialise();

            // Force buttons to reinitialise to relink new soldiers
            bGUIButtonsInitialised = false;

            // Choose the soldiers to deploy
            if (!bInProgress) {
                ChooseSoldiers cs = new ChooseSoldiers(PlayerTeam, ThisMission.MaximumSoldiers, bCanAbort);
                cs.ShowDialog();
                if (cs.Soldiers.Count == 0) {
                    MissionOutcome = MissionResult.Aborted;
                    ThisMission = null;
                    return false;
                }
                ThisMission.AddSoldiers(cs.Soldiers);
                CurrentLevel = ThisMission.GetOrCreateCurrentLevel(); // Create here as the number of creatures depends on the number of soldiers deployed in the mission
                CurrentLevel.AddSoldiers(cs.Soldiers); // Add them at the correct starting location, or in a room if ship defence
                Random rnd = new Random();
                Const.dtTime.AddHours(4.0 + rnd.NextDouble() * 2.0); // Time taken to get to the mission location & set up
            }
            else CurrentLevel = ThisMission.GetOrCreateCurrentLevel(); // Should definitely exist, so should return from cache

            // Centre around the first soldier
            CentreView(ThisMission.Soldiers[0]);
            fMissionViewZ = Const.InitialMissionViewZ;
            PlayerTeam.SetCurrentMission(ThisMission);
            view = ViewMode.ViewMission;
            SelectedEntity = null;

            // Set up maps
            TargetMap = new bool[CurrentLevel.Width, CurrentLevel.Height];
            DistMap = new int[CurrentLevel.Width, CurrentLevel.Height];
            AoEMap = new bool[CurrentLevel.Width, CurrentLevel.Height];
            DetectionMap = new bool[CurrentLevel.Width, CurrentLevel.Height];

            return true;
        }
        private void FailMission() {
            MissionOutcome = MissionResult.Defeat;
            CeaseMission();
        }
        private void CeaseMission() {
            if (PlayerTeam is null) throw new Exception("PlayerTeam is null in CeaseMission()");
            if (PlayerTeam.CurrentPosition is null) throw new Exception("PlayerTeam.CurrentPosition is null in CeaseMission()");
            if (ThisMission is null) throw new Exception("ThisMission is null in CeaseMission()");

            PlayerTeam.CeaseMission();
            Random rnd = new Random();

            // Didn't complete the mission so put it back on the pile
            if (MissionOutcome != MissionResult.Victory && TravelDetails == null) {
                ThisMission.ResetMission();
                if (PlayerTeam.CurrentPositionHAO?.Colony != null) PlayerTeam.CurrentPositionHAO.Colony.AddMission(ThisMission);
                else PlayerTeam.CurrentPositionHAO?.AddMission(ThisMission);
            }

            view = ViewMode.ViewSystem;

            // Resolve the mission (either victory or destruction)
            if (MissionOutcome == MissionResult.Victory) {
                if (TravelDetails != null) {
                    if (ThisMission.ShipTarget is null) throw new Exception("ThisMission.ShipTarget is null in CeaseMission()");
                    double dBounty = ThisMission.ShipTarget.EstimatedBountyValue * (0.5 + rnd.NextDouble());
                    if (ThisMission.Type == Mission.MissionType.RepelBoarders) {
                        msgBox.PopupMessage("You have repelled the boarders.\nYou search the attacking vessel and retrieve " + dBounty.ToString("N2") + " credits in bounty");
                        PlayerTeam.Cash += dBounty;
                    }
                    else if (ThisMission.Type == Mission.MissionType.BoardingParty) {
                        msgBox.PopupMessage("You successfully neutralise the enemy crew\nYou receive " + dBounty.ToString("N2") + " credits in bounty");
                        PlayerTeam.Cash += dBounty;
                    }
                    else throw new NotImplementedException();
                }
                else {
                    if (ThisMission.Goal == Mission.MissionGoal.Gather) {
                        msgBox.PopupMessage("You returned safely to your ship\nYou can sell any gathered " + ThisMission.MItem + "s at the nearest Colony");
                    }
                    else {
                        if (ThisMission.Goal == Mission.MissionGoal.FindItem) {
                            msgBox.PopupMessage("You return the " + ThisMission.MItem + " to the mission agent\nCash Reward = " + ThisMission.Reward + "cr\nBonus Experience = " + ThisMission.Experience + "xp each");
                            if (!PlayerTeam.RemoveItemFromStoresOrSoldiers(ThisMission.MItem)) throw new Exception("Could not find quest item on Team");
                        }
                        else msgBox.PopupMessage("You were victorious\nCash Reward = " + ThisMission.Reward + "cr\nBonus Experience = " + ThisMission.Experience + "xp each");
                        PlayerTeam.Cash += ThisMission.Reward;
                        foreach (Soldier s in ThisMission.Soldiers) {
                            s.AddExperience(ThisMission.Experience);
                            s.CheckForLevelUp(AnnounceMessage);
                        }
                    }
                }
            }
            else if (MissionOutcome == MissionResult.Evacuated) {
                // Remove any mission items so they can't be sold and the mission repeated ad infinitum
                if (ThisMission.MItem != null) PlayerTeam.RemoveItemFromStoresOrSoldiers(ThisMission.MItem, 10000);
            }

            Const.dtTime.AddHours(4.0 + rnd.NextDouble() * 2.0); // Time taken to return home from the mission

            if (TravelDetails != null) {
                // What to do if defeated??
                TravelDetails.ResumeTravelling();
            }
        }
        private void MissionClockTick() {
            // Soldiers auto moving
            List<Soldier> lSoldiers = new List<Soldier>(ThisMission!.Soldiers); // In case any die in the middle of the loop
            foreach (Soldier s in lSoldiers) {
                if (s.GoTo == s.Location) s.GoTo = Point.Empty;
                if (s.GoTo != Point.Empty) {
                    if (s.TravelRange == 0) { s.GoTo = Point.Empty; continue; } // May happen if something that occurs during movement alters stamina / movement points remaining
                    List<Point>? path = CurrentLevel.ShortestPath(s, s.Location, s.GoTo, 20, true, 0);
                    if (path is null || path.Count == 0) { s.GoTo = Point.Empty; continue; }  // e.g. if some other soldier moved in the way and blocked the route

                    // Open any door in the way
                    if (CurrentLevel.Map[path[0].X, path[0].Y] == MissionLevel.TileType.DoorHorizontal || CurrentLevel.Map[path[0].X, path[0].Y] == MissionLevel.TileType.DoorVertical) {
                        OpenDoor(path[0].X, path[0].Y, s);
                        continue;
                    }

                    // Step along the path
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
        private void DrawMission() {
            if (!bLoaded || ThisMission is null) return;
            if (SelectedEntity is Soldier s && s.PlayerTeam is null) {
                // If selected soldier has died since the last tick, deselect it
                SelectedEntity = null;
            }
            // If any soldier has died, update the buttons
            if (lastSoldiers.Count != ThisMission!.Soldiers.Count) {
                IEnumerable<Soldier> deadSoldiers = lastSoldiers.Except(ThisMission.Soldiers);
                lastSoldiers = new HashSet<Soldier>(ThisMission!.Soldiers);
                SetupZoomToButtons();
                // Work out which soldier(s) died
                foreach (Soldier recentlyDied in deadSoldiers) {
                    msgBox.PopupMessage($"Soldier {recentlyDied.Name} was killed!");
                }
                // All soldiers on mission are dead -> fail
                if (!ThisMission!.Soldiers.Any()) {
                    msgBox.PopupMessage("Your mission team was defeated!", FailMission);
                }
            }
            // Set up default OpenGL rendering parameters
            PrepareScene();

            // If it's a ship mission then do the starfield;
            if (ThisMission.Type == Mission.MissionType.BoardingParty || ThisMission.Type == Mission.MissionType.RepelBoarders || ThisMission.Type == Mission.MissionType.ShipCombat) {
                Matrix4 projectionUnitM = Matrix4.CreateOrthographicOffCenter(0.0f, 1.0f, 1.0f, 0.0f, -1.0f, 1.0f);
                pos2DCol4ShaderProgram.SetUniform("projection", projectionUnitM);
                pos2DCol4ShaderProgram.SetUniform("view", Matrix4.Identity);
                pos2DCol4ShaderProgram.SetUniform("model", Matrix4.Identity);
                pos2DCol4ShaderProgram.SetUniform("colourFactor", 1f);
                GL.UseProgram(pos2DCol4ShaderProgram.ShaderProgramHandle);
                Starfield.Build.BindAndDraw();
            }

            // Set the correct view location & perspective matrix
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
            Matrix4 projectionM = Matrix4.CreatePerspectiveFieldOfView(Const.MapViewportAngle, Aspect, 0.05f, 5000.0f);
            fullShaderProgram.SetUniform("projection", projectionM);
            pos2DCol4ShaderProgram.SetUniform("projection", projectionM);

            Matrix4 translateM = this.ViewMatrix;
            fullShaderProgram.SetUniform("view", translateM);
            fullShaderProgram.SetUniform("model", Matrix4.Identity);
            fullShaderProgram.SetUniform("flatColour", new Vector4(1f, 1f, 1f, 1f));
            fullShaderProgram.SetUniform("texPos", 0f, 0f);
            fullShaderProgram.SetUniform("texScale", 1f, 1f);
            fullShaderProgram.SetUniform("textureEnabled", false);
            fullShaderProgram.SetUniform("lightEnabled", false);
            pos2DCol4ShaderProgram.SetUniform("view", translateM);

            // Display the scene
            CurrentLevel.DisplayMap(fullShaderProgram);

            // Show any indicators on top of the action in the map view
            ShowMapGUIElements(fullShaderProgram, pos2DCol4ShaderProgram);

            // Show creatures/soldiers
            CurrentLevel.DisplayEntities(fullShaderProgram, PlayerTeam.Mission_ShowLabels, PlayerTeam.Mission_ShowStatBars, PlayerTeam.Mission_ShowEffects, fMissionViewZ, Aspect, translateM);

            // Display visual effects
            for (int n = Effects.Count - 1; n >= 0; n--) {
                if (Effects[n].Display(sw, Aspect, translateM)) Effects.RemoveAt(n);
            }

            // Draw any static GUI elements (overlay)
            ShowOverlayGUI();
        }

        // Input stuff
        private void GetKeyboardInput_Mission() {
            if (bAIRunning) return;
            if (IsKeyPressed(Keys.Escape)) {
                if (CurrentAction == SoldierAction.None) SetSelectedEntity(null);
                CurrentAction = SoldierAction.None;
                ActionItem = null;
            }
            if (SelectedEntity != null && SelectedEntity.GetType() == typeof(Soldier) && CurrentAction == SoldierAction.None) {
                if (SelectedEntity is Soldier s) {
                    if (IsKeyPressed(Keys.Down)) MoveSoldier(s, Utils.Direction.South);
                    if (IsKeyPressed(Keys.Up)) MoveSoldier(s, Utils.Direction.North);
                    if (IsKeyPressed(Keys.Left)) MoveSoldier(s, Utils.Direction.West);
                    if (IsKeyPressed(Keys.Right)) MoveSoldier(s, Utils.Direction.East);
                    if (IsKeyPressed(Keys.V) && (IsKeyDown(Keys.LeftControl) || IsKeyDown(Keys.RightControl))) ScavengeAll(s);
                    if (IsKeyPressed(Keys.S) && (IsKeyDown(Keys.LeftControl) || IsKeyDown(Keys.RightControl))) SelectedSoldierSearch(null);
                    if (IsKeyPressed(Keys.P) && (IsKeyDown(Keys.LeftControl) || IsKeyDown(Keys.RightControl))) PickUpAll(s);
                }
                if (IsKeyPressed(Keys.Space)) EndTurn();
            }
            if (IsKeyPressed(Keys.Tab)) TabToNextSoldier();
            if (IsKeyPressed(Keys.L)) {
                if (PlayerTeam.Mission_ShowLabels) { PlayerTeam.Mission_ShowLabels = false; }
                else { PlayerTeam.Mission_ShowLabels = true; }
            }
            if (IsKeyPressed(Keys.S)) {
                if (PlayerTeam.Mission_ShowStatBars) { PlayerTeam.Mission_ShowStatBars = false; }
                else { PlayerTeam.Mission_ShowStatBars = true; }
            }
            if (IsKeyPressed(Keys.T)) {
                if (PlayerTeam.Mission_ShowTravel) { PlayerTeam.Mission_ShowTravel = false; }
                else { PlayerTeam.Mission_ShowTravel = true; }
            }
            if (IsKeyPressed(Keys.P)) {
                if (PlayerTeam.Mission_ShowPath) { PlayerTeam.Mission_ShowPath = false; }
                else { PlayerTeam.Mission_ShowPath = true; }
            }
            if (IsKeyPressed(Keys.E)) {
                if (PlayerTeam.Mission_ShowEffects) { PlayerTeam.Mission_ShowEffects = false; }
                else { PlayerTeam.Mission_ShowEffects = true; }
            }
            if (IsKeyPressed(Keys.D)) {
                if (PlayerTeam.Mission_ViewDetection) { PlayerTeam.Mission_ViewDetection = false; }
                else { PlayerTeam.Mission_ViewDetection = true; }
            }
        }
        private void CheckHoverMission() {
            if (gpSelect != null && gpSelect.Active) return;
            if (ThisMission is null) return;

            soldierPanelCurrentHover = null;
            // Check GUIPanel first
            if (gpSelect != null && gpSelect.Active && gpSelect.HoverItem != null) return;

            // Check soldier panels
            float sx = 0.99f - Const.GUIPanelWidth, sy = Const.GUIPanelTop;
            double mxfract = MousePosition.X / (double)Size.X;
            double myfract = MousePosition.Y / (double)Size.Y;
            if (mxfract >= sx && mxfract <= (sx + Const.GUIPanelWidth)) {
                foreach (Soldier s in ThisMission.Soldiers) {
                    float step = s.GetGuiPanelHeight(SelectedEntity == s);
                    if (myfract >= sy && myfract <= sy + step) { soldierPanelCurrentHover = s; return; }
                    sy += step + Const.GUIPanelGap;
                }
            }

            // Buttons
            if (gbEndTurn != null && gbEndTurn.IsHover((int)MousePosition.X, (int)MousePosition.Y)) return;
            if (gbTransition != null && gbTransition.IsHover((int)MousePosition.X, (int)MousePosition.Y)) return;
            if (gbZoomTo1 != null && gbZoomTo1.IsHover((int)MousePosition.X, (int)MousePosition.Y)) return;
            if (gbZoomTo2 != null && gbZoomTo2.IsHover((int)MousePosition.X, (int)MousePosition.Y)) return;
            if (gbZoomTo3 != null && gbZoomTo3.IsHover((int)MousePosition.X, (int)MousePosition.Y)) return;
            if (gbZoomTo4 != null && gbZoomTo4.IsHover((int)MousePosition.X, (int)MousePosition.Y)) return;
            if (gbWest != null && gbWest.IsHover((int)MousePosition.X, (int)MousePosition.Y)) return;
            if (gbEast != null && gbEast.IsHover((int)MousePosition.X, (int)MousePosition.Y)) return;
            if (gbNorth != null && gbNorth.IsHover((int)MousePosition.X, (int)MousePosition.Y)) return;
            if (gbSouth != null && gbSouth.IsHover((int)MousePosition.X, (int)MousePosition.Y)) return;
            if (gbAttack != null && gbAttack.IsHover((int)MousePosition.X, (int)MousePosition.Y)) return;
            if (gbUseItem != null && gbUseItem.IsHover((int)MousePosition.X, (int)MousePosition.Y)) return;
            if (gbInventory != null && gbInventory.IsHover((int)MousePosition.X, (int)MousePosition.Y)) return;
            if (gbSearch != null && gbSearch.IsHover((int)MousePosition.X, (int)MousePosition.Y)) return;

            // Calculate the position of the mouse pointer
            double mxpos = ((mxfract - 0.5) * (fMissionViewZ / 1.86) * Aspect) + fMissionViewX;
            double mypos = ((0.5 - myfract) * (fMissionViewZ / 1.86)) + fMissionViewY;
            int oldhoverx = hoverx;
            int oldhovery = hovery;
            hoverx = (int)mxpos;
            hovery = (int)mypos;
            if (hoverx != oldhoverx || hovery != oldhovery) {
                if (SelectedEntity != null && SelectedEntity is Soldier) {
                    if (hoverx >= 0 && hoverx < CurrentLevel.Width && hovery >= 0 && hovery < CurrentLevel.Height && DistMap[hoverx, hovery] > 0) {
                        lCurrentPath = CurrentLevel.ShortestPath(SelectedEntity, SelectedEntity.Location, new Point(hoverx, hovery), 20, true);
                    }
                    else lCurrentPath = null;
                }
                else lCurrentPath = null;
            }
            CurrentLevel.SetHover(hoverx, hovery);
        }
        private void MouseMove_Mission(MouseMoveEventArgs e) {
            if (MouseState.IsButtonDown(MouseButton.Left)) {
                float fScale = Const.MouseMoveScale * fMissionViewZ / 50.0f;
                if (IsKeyDown(Keys.LeftControl) || IsKeyDown(Keys.RightControl)) fScale *= 2.5f;
                else if (IsKeyDown(Keys.LeftShift) || IsKeyDown(Keys.RightShift)) fScale /= 2.5f;
                fMissionViewX -= e.DeltaX * fScale;
                if (fMissionViewX < 0) fMissionViewX = 0;
                if (fMissionViewX > CurrentLevel.Width) fMissionViewX = CurrentLevel.Width;
                fMissionViewY += e.DeltaY * fScale;
                if (fMissionViewY < 0) fMissionViewY = 0;
                if (fMissionViewY > CurrentLevel.Height) fMissionViewY = CurrentLevel.Height;
                bDragging = true;
            }
            else bDragging = false;
            int oldhoverx = hoverx, oldhovery = hovery;
            CheckHoverMission();
            // Mouse has moved to a different square
            if (hoverx != oldhoverx || hovery != oldhovery) {
                if (hoverx > 0 && hoverx < CurrentLevel.Width - 1 && hovery > 0 && hovery < CurrentLevel.Height - 1 && TargetMap[hoverx, hovery]) {
                    if (ActionItem?.BaseType?.ItemEffect is not null) {
                        GenerateAoEMap(hoverx, hovery, ActionItem.BaseType.ItemEffect.Radius, oldhoverx, oldhovery);
                    }
                    else if (SelectedEntity != null && SelectedEntity is Soldier s && s.EquippedWeapon != null && s.EquippedWeapon.Type.Area > 0) {
                        GenerateAoEMap(hoverx, hovery, s.EquippedWeapon.Type.Area, oldhoverx, oldhovery);
                    }
                }
            }
        }
        private async void MouseUp_Mission(MouseButtonEventArgs e) {
            // Check R-button released
            if (bAIRunning) return;
            Soldier? s = null;
            if (SelectedEntity is Soldier sld) s = sld;
            if (e.Button == MouseButton.Right) {
                if (gpSelect != null && gpSelect.Active) {
                    gpSelect.Deactivate();
                    object? oHover = gpSelect.HoverObject;
                    // Process GUIPanel selection
                    if (s != null && oHover is not null && gpSelect.HoverItem!.Enabled) {
                        int iSelectHover = gpSelect.HoverID;
                        if (iSelectHover == I_OpenDoor) {
                            OpenDoor(gpSelect.ClickX, gpSelect.ClickY, s);
                        }
                        if (iSelectHover == I_CloseDoor) {
                            if (CurrentLevel.CloseDoor(gpSelect.ClickX, gpSelect.ClickY)) {
                                SoundEffects.PlaySound("CloseDoor");
                                GenerateDistMap(SelectedEntity as Soldier);
                                GenerateDetectionMap();
                                s.UpdateVisibility(CurrentLevel);
                                CurrentLevel.CalculatePlayerVisibility();
                            }
                        }
                        if (iSelectHover == I_UnlockDoor) {
                            // TODO
                        }
                        if (iSelectHover == I_LockDoor) {
                            // TODO
                        }
                        if (iSelectHover == I_Attack) {
                            bool bAttacked = await Task.Run(() => s.AttackLocation(CurrentLevel, gpSelect.ClickX, gpSelect.ClickY, AddNewEffect, PlaySoundThreaded, AnnounceMessage));
                            if (bAttacked) {
                                if (UpdateDetectionForSoldier(s, Const.FireWeaponExtraDetectionRange) ||
                                    UpdateDetectionForLocation(gpSelect.ClickX, gpSelect.ClickY, 0, Const.BaseDetectionRange)) {
                                    // No alert required
                                }
                            }
                        }
                        if (iSelectHover == I_GoTo) { // Set up GoTo for a selected square
                            s.GoTo = new Point(gpSelect.ClickX, gpSelect.ClickY);
                        }
                        if (iSelectHover == I_UseItem) { // Shortcut to inventory
                            OpenSoldierInventory(s);
                        }
                        if (oHover is IEquippable eqp) {
                            // Clicked on a usable item
                            ActionItem = eqp;
                            CurrentAction = SoldierAction.Item;
                            if (ActionItem?.BaseType?.ItemEffect is not null) {
                                GenerateTargetMap(s, ActionItem.BaseType.ItemEffect.Range);
                                if (SelectedEntity is not null) {
                                    int sy = SelectedEntity.Y;
                                    int sx = SelectedEntity.X;
                                    GenerateAoEMap(sx, sy, ActionItem.BaseType.ItemEffect.Radius);
                                }
                            }
                        }
                    }
                }
            }
            if (e.Button == MouseButton.Left) {
                if (s != null && CurrentAction == SoldierAction.Attack) {
                    CurrentAction = SoldierAction.None;
                    bool bAttacked = await Task.Run(() => s.AttackLocation(CurrentLevel, hoverx, hovery, AddNewEffect, PlaySoundThreaded, AnnounceMessage));
                    //bool bAttacked = s.AttackLocation(level, hoverx, hovery, AddNewEffect, glMissionView.Invalidate, PlaySoundThreaded).Result;
                    if (bAttacked) {
                        if (UpdateDetectionForSoldier(s, Const.FireWeaponExtraDetectionRange) ||
                            UpdateDetectionForLocation(hoverx, hovery, 0, Const.BaseDetectionRange)) {
                            // No alert required
                        }
                    }
                }
                else if (s != null && CurrentAction == SoldierAction.Item) {
                    if (s.Stamina < s.UseItemCost) {
                        msgBox.PopupMessage("You have insufficient Stamina to use Item!");
                        return;
                    }
                    if (ActionItem is null) throw new Exception("Null ActionItem in soldier action");
                    if (ActionItem.BaseType.ItemEffect is null) {
                        throw new Exception("ActionItem without effect!");
                    }
                    if (s.RangeTo(hoverx, hovery) > ActionItem.BaseType.ItemEffect.Range) {
                        msgBox.PopupMessage("Target out of range!");
                        return;
                    }
                    if (ActionItem.BaseType.ItemEffect.Radius == 0 && CurrentLevel.GetEntityAt(hoverx, hovery) is null) {
                        // Single target effect, and no target was selected
                        return;
                    }
                    // Firstly, remove the item from the Soldier in question if it's a single use item
                    IEquippable temp = ActionItem!;
                    s.UseItem(ActionItem!);
                    CurrentAction = SoldierAction.None;
                    ActionItem = null;
                    // Now apply the effect
                    ApplyItemEffectToMap(s, temp, hoverx, hovery);
                    if (UpdateDetectionForLocation(hoverx, hovery, 0, Const.BaseDetectionRange)) {
                        // No alert required
                    }
                }
                else {
                    bool bClicked = false;
                    foreach (GUIIconButton bt in lButtons) bClicked |= bt.CaptureClick((int)MousePosition.X, (int)MousePosition.Y);
                    if (gbEndTurn != null) bClicked |= gbEndTurn.CaptureClick((int)MousePosition.X, (int)MousePosition.Y);
                    if (gbTransition != null) bClicked |= gbTransition.CaptureClick((int)MousePosition.X, (int)MousePosition.Y);
                    if (gbEndMission != null) bClicked |= gbEndMission.CaptureClick((int)MousePosition.X, (int)MousePosition.Y);
                    if (!bClicked) {
                        if (soldierPanelCurrentHover != null) SetSelectedEntity(soldierPanelCurrentHover);
                        else if (!bDragging) SetSelectedEntity(CurrentLevel.GetHoverEntity());
                    }
                }
                CheckMenuClick();
            }
            if (SelectedEntity is Soldier) {
                GenerateDistMap(SelectedEntity as Soldier);
                GenerateDetectionMap();
            }
            CheckHoverMission();
        }
        private void MouseDown_Mission(MouseButtonEventArgs e) {
            if (e.Button == MouseButton.Right) {
                if (SelectedEntity != null && (SelectedEntity is Soldier) && !bAIRunning) SetupContextMenu();
            }
            if (e.Button == MouseButton.Left) {
                // TODO: Left click
            }
        }
        private void DoubleClick_Mission() {
            if (soldierPanelCurrentHover != null) return;
            foreach (GUIIconButton bt in lButtons) {
                if (bt.IsHover((int)MousePosition.X, (int)MousePosition.Y)) return;
            }
            if (gbEndTurn!.IsHover((int)MousePosition.X, (int)MousePosition.Y)) return;
            if (gbTransition!.IsHover((int)MousePosition.X, (int)MousePosition.Y)) return;
            if (gbEndMission!.IsHover((int)MousePosition.X, (int)MousePosition.Y)) return;
            Point pt = CurrentLevel.MouseHover;
            IEntity? he = CurrentLevel.GetHoverEntity();
            if (he != null) {
                using (CreatureView cv = new CreatureView(he, Cursor.X, Cursor.Y)) {
                    cv.ShowDialog();
                }
            }
            else {
                fMissionViewX = pt.X + 0.5f;
                fMissionViewY = pt.Y + 0.5f;
            }
        }
        private void MouseWheel_Mission(float delta) {
            fMissionViewZ += delta;
            if (fMissionViewZ < Const.MinimumMissionViewZ) fMissionViewZ = Const.MinimumMissionViewZ;
            if (fMissionViewZ > Const.MaximumMissionViewZ) fMissionViewZ = Const.MaximumMissionViewZ;
        }

        private void AddNewEffect(VisualEffect.EffectType type, float x, float y, Dictionary<string, object> dict) {
            Effects.Add(new VisualEffect(type, x, y, sw, 1f, dict));
        }

        // Actions
        private void MoveSoldier(Soldier s, Utils.Direction d) {
            if (s.Move(d)) UpdateLevelAfterSoldierMove(s);
        }
        private void UpdateLevelAfterSoldierMove(Soldier s) {
            CheckForTransition();
            if (SelectedEntity is Soldier se) GenerateDistMap(se);
            GenerateDetectionMap();
            // Check if this soldier was detected
            if (DetectionMap[s.X, s.Y]) {
                if (UpdateDetectionForSoldier(s)) {
                    msgBox.PopupMessage(s.Name + " has been detected by the enemy!");
                }
            }
            CheckForTraps(s);
            foreach (Creature cr in CurrentLevel.Creatures) {
                if (cr.CurrentTarget == s && cr.CanSee(s)) cr.SetTarget(s);   // Make sure creature is following us by setting the target (NOP) and updating the Investigation square with Soldier's current position
            }
            // Passive search
            List<string> lFound = s.SearchTheArea(CurrentLevel, true);
            if (lFound.Count > 0) msgBox.PopupMessage(lFound);

        }
        private void CheckForTransition() {
            // Test if all soldiers are now on an entrance/exit square and set up the button if so.
            if (CurrentLevel.CheckAllSoldiersAtEntrance()) {
                gbTransition!.Activate();
                if (CurrentLevel.LevelID == 0) gbTransition.UpdateText("Exit Mission");
                else gbTransition.UpdateText("Ascend to Level " + CurrentLevel.LevelID);  // LevelID starts at zero
            }
            else if (CurrentLevel.CheckAllSoldiersAtExit()) {
                gbTransition!.Activate();
                gbTransition.UpdateText("Descend to Level " + (CurrentLevel.LevelID + 2));  // LevelID starts at zero
            }
            else gbTransition!.Deactivate();
        }
        private void ApplyItemEffectToMap(Soldier s, IEquippable it, int px, int py) {
            ItemEffect? ie = it.BaseType.ItemEffect;
            if (ie is null) return;
            HashSet<IEntity> hsEntities = new HashSet<IEntity>();

            for (int y = (int)Math.Max(py - ie.Radius, 0); y <= (int)Math.Min(py + ie.Radius, CurrentLevel.Height - 1); y++) {
                for (int x = (int)Math.Max(px - ie.Radius, 0); x <= (int)Math.Min(px + ie.Radius, CurrentLevel.Width - 1); x++) {
                    if ((x - px) * (x - px) + (y - py) * (y - py) > ie.Radius * ie.Radius) continue;
                    IEntity? en = CurrentLevel.GetEntityAt(x, y);
                    if (en != null && !hsEntities.Contains(en)) {
                        hsEntities.Add(en); // Make sure we don't double count e.g. large entities
                    }
                }
            }

            // Play a sound, if there is one
            if (!string.IsNullOrEmpty(ie.SoundEffect)) {
                SoundEffects.PlaySound(ie.SoundEffect);
                Thread.Sleep(500);
            }

            // Apply effect to the targets
            foreach (IEntity en in hsEntities) {
                en.ApplyEffectToEntity(s, ie, AddNewEffect);
            }
        }
        private bool CheckForTraps(Soldier s) {
            Trap? tr = CurrentLevel.GetTrapAtPoint(s.X, s.Y);
            if (tr == null) return false;

            // Stop auto-move
            s.GoTo = Point.Empty;

            // Generate Damage
            Dictionary<WeaponType.DamageType, double> AllDam = tr.GenerateDamage();
            double TotalDam = s.InflictDamage(AllDam);
            Thread.Sleep(100);
            SoundEffects.PlaySound("Click");
            Thread.Sleep(100);
            SoundEffects.PlaySound("Grunt");
            AddNewEffect(VisualEffect.EffectType.Damage, s.X + 0.5f, s.Y + 0.5f, new Dictionary<string, object>() { { "Value", TotalDam } });

            if (tr.Hidden) {
                msgBox.PopupMessage("You have triggered a hidden trap!");
                tr.Reveal();
            }

            return true;
        }
        private bool UpdateDetectionForSoldier(Soldier s, double extraRange = 0.0) {
            if (s == null) return false;
            if (extraRange == 0.0 && !DetectionMap[s.X, s.Y]) return false;
            double baseRange = s.DetectionRange + extraRange;
            return UpdateDetectionForLocation(s.X, s.Y, s.Level, baseRange);
        }
        private bool UpdateDetectionForLocation(int x, int y, int sLevel, double baseRange) {
            if (x < 0 || y < 0 || x >= CurrentLevel.Width || y >= CurrentLevel.Height) return false;
            // Check every nearby creature to see if it can detect this soldier
            HashSet<Creature> hsDetected = new HashSet<Creature>();
            foreach (Creature cr in CurrentLevel.Creatures) {
                if (!cr.IsAlert && !hsDetected.Contains(cr)) {
                    double range = baseRange;
                    if (sLevel > 0) range += ((cr.Level - sLevel) / 3.0); // sLevel == 0 means no level bonuses to be applied
                                                                          // Check range
                    double r2 = (x - cr.X) * (x - cr.X) + (y - cr.Y) * (y - cr.Y);
                    if (r2 <= (range * range) && cr.CanSee(x, y)) {
                        hsDetected.Add(cr);
                        cr.Alert();
                        cr.SetTargetInvestigation(x, y);
                    }
                }
            }
            if (hsDetected.Count == 0) return false;
            foreach (Creature crd in hsDetected) {
                foreach (Creature cr in CurrentLevel.Creatures) {
                    if (!cr.IsAlert) {
                        int r2 = (crd.X - cr.X) * (crd.X - cr.X) + (crd.Y - cr.Y) * (crd.Y - cr.Y);
                        if (r2 <= Const.CreatureAlertWarningDistance * Const.CreatureAlertWarningDistance) {
                            if (cr.CanSee(crd)) cr.Alert();
                            cr.SetTargetInvestigation(x, y);
                        }
                    }
                }
            }
            GenerateDetectionMap();
            return true;
        }
        private void ScavengeAll(Soldier s) {
            int sk = s.GetUtilityLevel(Soldier.UtilitySkill.Scavenging);
            if (sk == 0) return;
            Stash? st = CurrentLevel.GetStashAtPoint(s.X, s.Y);
            if (st == null) return;
            Dictionary<IItem, int> Scavenged = new Dictionary<IItem, int>();
            Random rand = new Random();
            foreach (IItem it in st.Items()) {
                if (it is Corpse cp && !cp.IsSoldier) {
                    // Generate stuff
                    for (int n = 0; n < st.GetCount(it); n++) {
                        List<IItem> stuff = cp.Scavenge(sk, rand);
                        foreach (IItem sc in stuff) {
                            s.AddItem(sc);
                            if (Scavenged.ContainsKey(sc)) Scavenged[sc]++;
                            else Scavenged.Add(sc, 1);
                        }
                    }
                    st.Remove(it);
                }
            }
            StringBuilder sb = new StringBuilder();
            if (!Scavenged.Any()) {
                sb.AppendLine("You found nothing useful");
            }
            else {
                sb.AppendLine("Scavenging Results:");
                foreach (IItem sc in Scavenged.Keys) {
                    sb.AppendFormat("{0} [{1}]", sc.Name, Scavenged[sc]);
                    sb.AppendLine();
                }
            }
            msgBox.PopupMessage(sb.ToString());
            if (st.Count == 0) CurrentLevel.ReplaceStashAtPosition(s.X, s.Y, null);
        }
        private void PickUpAll(Soldier s) {
            Stash? st = CurrentLevel.GetStashAtPoint(s.X, s.Y);
            if (st == null) return;
            int count = 0;
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Objects Picked Up:");
            foreach (IItem it in st.Items()) {
                if (it is not Corpse) {
                    int num = st.GetCount(it);
                    s.AddItem(it, num);
                    sb.AppendLine(it.Name + " [" + num + "]");
                    st.Remove(it, num);
                    count++;
                }
            }
            if (st.Count == 0) CurrentLevel.ReplaceStashAtPosition(s.X, s.Y, null);
            if (count == 0) sb.AppendLine("You found nothing useful");
            msgBox.PopupMessage(sb.ToString());
        }
        private void TabToNextSoldier() {
            if (SelectedEntity is null || SelectedEntity is Creature) {
                SetSelectedEntity(ThisMission!.Soldiers[0]);
                CentreView(ThisMission!.Soldiers[0]);
                return;
            }
            else if (SelectedEntity is Soldier s) {
                for (int n = 0; n < ThisMission!.Soldiers.Count; n++) {
                    if (ThisMission.Soldiers[n] == s) {
                        if (n == ThisMission.Soldiers.Count - 1) n = 0;
                        else n++;
                        SetSelectedEntity(ThisMission.Soldiers[n]);
                        CentreView(ThisMission.Soldiers[n]);
                        return;
                    }
                }
            }
            throw new Exception("Couldn't identify tab soldier");
        }
        private void OpenDoor(int x, int y, Soldier s) {
            CurrentLevel.OpenDoor(x, y);
            SoundEffects.PlaySound("OpenDoor");
            GenerateDistMap(s);
            s.UpdateVisibility(CurrentLevel);
            CurrentLevel.CalculatePlayerVisibility();
            GenerateDetectionMap();
            if (UpdateDetectionForSoldier(s)) {
                msgBox.PopupMessage(s.Name + " has been detected by the enemy!");
            }
        }

        // Menu handlers
        private void DisplayMissionDetails() {
            if (ThisMission is null) return;
            string strDesc = ThisMission.GetDescription();
            if (ThisMission.Goal == Mission.MissionGoal.ExploreAll || ThisMission.Goal == Mission.MissionGoal.KillAll || ThisMission.Goal == Mission.MissionGoal.Gather) {
                strDesc += "----------\nProgress:\n";
                for (int n = 0; n < ThisMission.LevelCount; n++) {
                    strDesc += "Level " + n + " : ";
                    MissionLevel? lev = ThisMission.GetLevel(n);
                    if (lev == null) strDesc += "Not explored\n";
                    else {
                        if (ThisMission.Goal == Mission.MissionGoal.ExploreAll) {
                            Tuple<int, int> tp = lev.UnexploredTiles;
                            int rem = tp.Item1;
                            if (rem == 1) strDesc += "1 tile remaining\n";
                            else if (rem == 0) strDesc += "Complete\n";
                            else strDesc += rem.ToString() + " tiles remaining\n";
                        }
                        else if (ThisMission.Goal == Mission.MissionGoal.KillAll) {
                            int rem = lev.Creatures.Count();
                            if (rem == 1) strDesc += "1 enemy remaining\n";
                            else if (rem == 0) strDesc += "Complete\n";
                            else strDesc += rem.ToString() + " enemies remaining\n";
                        }
                        else if (ThisMission.Goal == Mission.MissionGoal.Gather) {
                            int rem = lev.CountMissionItemsRemaining;
                            if (rem == 1) strDesc += "1 item remaining\n";
                            else if (rem == 0) strDesc += "Complete\n";
                            else strDesc += rem.ToString() + " items remaining\n";
                        }
                    }
                }
            }
            msgBox.PopupMessage(strDesc);
        }

        // Show GUI elements in the viewpoint of the map
        private void ShowMapGUIElements(ShaderProgram texProg, ShaderProgram flatProg) {
            // Mouse hover
            GL.Disable(EnableCap.DepthTest);
            Point pt = CurrentLevel.MouseHover;
            if (pt != Point.Empty && CurrentAction == SoldierAction.None) {
                DrawHoverFrame(texProg, pt.X, pt.Y);
            }

            // Selected entity red shadow
            if (SelectedEntity != null) {
                if (CurrentLevel.EntityIsVisible(SelectedEntity))
                { 
                    DrawSelectionTile(texProg, SelectedEntity.Location.X + SelectedEntity.Size / 2f, SelectedEntity.Location.Y + SelectedEntity.Size / 2f, SelectedEntity.Size);
                }
            }

            List<VertexPos2DCol> vertices = new List<VertexPos2DCol>();
            if (CurrentAction == SoldierAction.Attack) {
                vertices.AddRange(DrawTargetGrid());
                if (TargetMap[pt.X, pt.Y] == true) {
                    DrawHoverFrame(texProg, pt.X, pt.Y);
                    if (SelectedEntity != null && SelectedEntity is Soldier soldier && soldier.EquippedWeapon != null && soldier.EquippedWeapon.Type.Area > 0) vertices.AddRange(DrawAoEGrid());
                }
            }
            else if (CurrentAction == SoldierAction.Item) {
                vertices.AddRange(DrawTargetGrid());
                if (TargetMap[pt.X, pt.Y] == true) {
                    DrawHoverFrame(texProg, pt.X, pt.Y);
                    vertices.AddRange(DrawAoEGrid());
                }
            }
            else {
                if (PlayerTeam.Mission_ShowTravel && SelectedEntity != null && SelectedEntity is Soldier) {
                    vertices.AddRange(DrawTravelGrid());
                }
                if (PlayerTeam.Mission_ShowPath && SelectedEntity != null && SelectedEntity is Soldier) {
                    if (hoverx >= 0 && hoverx < CurrentLevel.Width && hovery >= 0 && hovery < CurrentLevel.Height && DistMap[hoverx, hovery] > 0) {
                        DrawTravelPath(texProg);
                    }
                }
            }

            // Set up the overlay grids as an index buffer
            int numGrids = vertices.Count / 4;
            if (numGrids > 0) {
                if (vbGrid is null) vbGrid = new VertexBuffer(vertices.ToArray(), BufferUsageHint.DynamicDraw);
                else vbGrid.SetData(vertices.ToArray());
                if (vaGrid is null) vaGrid = new VertexArray(vbGrid);
                int[] indices = new int[numGrids * 8];
                for (int n = 0; n < numGrids; n++) {
                    indices[n * 8 + 0] = n * 4 + 0;
                    indices[n * 8 + 1] = n * 4 + 1;
                    indices[n * 8 + 2] = n * 4 + 1;
                    indices[n * 8 + 3] = n * 4 + 2;
                    indices[n * 8 + 4] = n * 4 + 2;
                    indices[n * 8 + 5] = n * 4 + 3;
                    indices[n * 8 + 6] = n * 4 + 3;
                    indices[n * 8 + 7] = n * 4 + 0;
                }
                if (ibGrid is null) ibGrid = new IndexBuffer(indices, false);
                else ibGrid.SetData(indices);
                flatProg.SetUniform("colourFactor", 1f);
                GL.UseProgram(flatProg.ShaderProgramHandle);
                GL.BindVertexArray(vaGrid.VertexArrayHandle);
                GL.BindBuffer(BufferTarget.ElementArrayBuffer, ibGrid.IndexBufferHandle);
                GL.DrawElements(PrimitiveType.Lines, ibGrid.IndexCount, DrawElementsType.UnsignedInt, 0);
                GL.BindVertexArray(0);
                GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
            }

            if (PlayerTeam.Mission_ViewDetection) CurrentLevel.DrawDetectionMap(texProg, DetectionMap);
            if (Const.DEBUG_SHOW_SELECTED_ENTITY_VIS && SelectedEntity != null) CurrentLevel.DrawSelectedEntityVis(texProg, SelectedEntity);
            GL.Enable(EnableCap.DepthTest);
        }

        // Show GUI elements in the overlay layer
        private void ShowOverlayGUI() {
            InitialiseGUIButtons();
            Matrix4 projectionM = Matrix4.CreateOrthographicOffCenter(0.0f, 1.0f, 1.0f, 0.0f, -1.0f, 1.0f);
            fullShaderProgram.SetUniform("projection", projectionM);
            fullShaderProgram.SetUniform("view", Matrix4.Identity);
            fullShaderProgram.SetUniform("model", Matrix4.Identity);

            // Show the selected entity details if it's a creature
            if (SelectedEntity != null && SelectedEntity is Creature) {
                ShowSelectedEntityDetails();
            }

            // Details on the squad
            ShowSoldierPanels(fullShaderProgram);

            // Show the context menu and other buttons
            fullShaderProgram.SetUniform("view", Matrix4.Identity);
            fullShaderProgram.SetUniform("model", Matrix4.Identity);
            GL.Disable(EnableCap.DepthTest);
            gpMenu!.Display((int)MousePosition.X, (int)MousePosition.Y, fullShaderProgram);
            gpSelect?.Display((int)MousePosition.X, (int)MousePosition.Y, fullShaderProgram);
            foreach (GUIIconButton bt in lButtons) bt.Display((int)MousePosition.X, (int)MousePosition.Y, fullShaderProgram);
            gbEndTurn?.Display((int)MousePosition.X, (int)MousePosition.Y, fullShaderProgram);
            gbTransition?.Display((int)MousePosition.X, (int)MousePosition.Y, fullShaderProgram);
            if (ThisMission?.IsComplete ?? false) {
                gbEndMission!.Activate();
                gbEndMission.Display((int)MousePosition.X, (int)MousePosition.Y, fullShaderProgram);
            }
            else gbEndMission!.Deactivate();
            DrawMissionToggles();

            // Warn that AI is running?
            if (bAIRunning) {
                DisplayAILabel(fullShaderProgram);
            }
            GL.Enable(EnableCap.DepthTest);
        }
        private void DrawMissionToggles() {
            TextRenderer.DrawAt("L", Alignment.TopRight, toggleScale, Aspect, toggleX, toggleY + toggleStep * 10f, PlayerTeam.Mission_ShowLabels ? Color.White : Color.DimGray);
            TextRenderer.DrawAt("S", Alignment.TopRight, toggleScale, Aspect, toggleX, toggleY + toggleStep * 11f, PlayerTeam.Mission_ShowStatBars ? Color.White : Color.DimGray);
            TextRenderer.DrawAt("T", Alignment.TopRight, toggleScale, Aspect, toggleX, toggleY + toggleStep * 12f, PlayerTeam.Mission_ShowTravel ? Color.White : Color.DimGray);
            TextRenderer.DrawAt("P", Alignment.TopRight, toggleScale, Aspect, toggleX, toggleY + toggleStep * 13f, PlayerTeam.Mission_ShowPath ? Color.White : Color.DimGray);
            TextRenderer.DrawAt("E", Alignment.TopRight, toggleScale, Aspect, toggleX, toggleY + toggleStep * 14f, PlayerTeam.Mission_ShowEffects ? Color.White : Color.DimGray);
            TextRenderer.DrawAt("D", Alignment.TopRight, toggleScale, Aspect, toggleX, toggleY + toggleStep * 15f, PlayerTeam.Mission_ViewDetection ? Color.White : Color.DimGray);
        }
        private void DisplayAILabel(ShaderProgram prog) {
            prog.SetUniform("textureEnabled", false);
            prog.SetUniform("flatColour", new Vector4(0.1f, 0.1f, 0.1f, 1f));
            Matrix4 pTranslateM = Matrix4.CreateTranslation(0.4f, 0.02f, Const.DoodadLayer);
            Matrix4 pScaleM = Matrix4.CreateScale(0.2f, 0.08f, 1f);
            prog.SetUniform("model", pScaleM * pTranslateM);
            GL.UseProgram(prog.ShaderProgramHandle);
            Square.Flat.BindAndDraw();
            TextRenderOptions tro = new TextRenderOptions() {
                Alignment = Alignment.TopMiddle,
                Aspect = Aspect,
                TextColour = Color.White,
                XPos = 0.5f,
                YPos = 0.03f,
                ZPos = 0.02f,
                Scale = 0.04f
            };
            TextRenderer.DrawWithOptions("AI Running", tro);
        }
        private void ShowSelectedEntityDetails() {
            if (SelectedEntity is null) return;
            // Display the stats for the selected entity
            TextRenderOptions tro = new TextRenderOptions() {
                Alignment = Alignment.TopRight,
                Aspect = Aspect,
                TextColour = Color.White,
                XPos = 0.998f,
                YPos = 0.81f - (SelectedEntity.MaxShields > 0f ? 0.0368f : 0f),
                ZPos = Const.GUILayer,
                Scale = 0.04f
            };
            TextRenderer.DrawWithOptions(SelectedEntity.Name, tro);
            tro.YPos += (tro.Scale * 1.2f);
            TextRenderer.DrawWithOptions("Level " + SelectedEntity.Level, tro);
            tro.YPos += (tro.Scale * 1.2f);
            int hp = (int)SelectedEntity.Health;
            if (hp < 1) hp = 1;
            string strStats = $"HP:{hp} / St:{(int)SelectedEntity.Stamina}";
            if (SelectedEntity.MaxShields > 0) strStats += $" / Sh: {(int)SelectedEntity.Shields}";
            TextRenderer.DrawWithOptions(strStats, tro);
        }
        private void ShowSoldierPanels(ShaderProgram prog) {
            if (ThisMission is null) return;
            // Show the details of the soldiers
            float sx = 0.99f - Const.GUIPanelWidth, sy = Const.GUIPanelTop;
            IEntity? hover = CurrentLevel.GetHoverEntity();
            if (soldierPanelCurrentHover != null) hover = soldierPanelCurrentHover;
            for (int sno = 0; sno < ThisMission.Soldiers.Count; sno++) {
                Soldier s = ThisMission.Soldiers[sno];
                float PanelHeight = s.GetGuiPanelHeight(SelectedEntity == s);

                // Place the buttons (Y only, plus on/off if soldier is selected)
                if (sno == 0 && gbZoomTo1 is not null) gbZoomTo1.ButtonY = sy + FrameBorder;
                if (sno == 1 && gbZoomTo2 is not null) gbZoomTo2.ButtonY = sy + FrameBorder;
                if (sno == 2 && gbZoomTo3 is not null) gbZoomTo3.ButtonY = sy + FrameBorder;
                if (sno == 3 && gbZoomTo4 is not null) gbZoomTo4.ButtonY = sy + FrameBorder;
                if (SelectedEntity == s) {
                    gbWest!.ButtonY = sy + PanelHeight - (ButtonSize * 2f + ButtonGap * 2f);
                    gbEast!.ButtonY = sy + PanelHeight - (ButtonSize * 2f + ButtonGap * 2f);
                    gbNorth!.ButtonY = sy + PanelHeight - (ButtonSize * 3f + ButtonGap * 3f);
                    gbSouth!.ButtonY = sy + PanelHeight - (ButtonSize + ButtonGap);
                    gbAttack!.ButtonY = sy + PanelHeight - (ButtonSize * 3f + ButtonGap * 3f);
                    gbUseItem!.ButtonY = sy + PanelHeight - (ButtonSize * 3f + ButtonGap * 3f);
                    gbInventory!.ButtonY = sy + PanelHeight - (ButtonSize * 1.5f + ButtonGap * 1f);
                    gbSearch!.ButtonY = sy + PanelHeight - (ButtonSize * 1.5f + ButtonGap * 1f);
                    if (bAIRunning) gbInventory.Deactivate();
                    else gbInventory.Activate();
                    if (s.Stamina < s.AttackCost || s.GoTo != Point.Empty || bAIRunning || (s.HasMoved && s.EquippedWeapon?.Type?.Stable == true)) gbAttack.Deactivate();
                    else gbAttack.Activate();
                    if (s.Stamina < s.SearchCost || s.GoTo != Point.Empty || bAIRunning) gbSearch.Deactivate();
                    else gbSearch.Activate();
                    if (s.Stamina < s.UseItemCost || s.GoTo != Point.Empty || !s.HasUtilityItems() || bAIRunning) gbUseItem.Deactivate();
                    else gbUseItem.Activate();
                    if (s.Stamina < s.MovementCost || s.GoTo != Point.Empty || bAIRunning) {
                        gbWest.Deactivate();
                        gbEast.Deactivate();
                        gbNorth.Deactivate();
                        gbSouth.Deactivate();
                    }
                    else {
                        gbWest.Activate();
                        gbEast.Activate();
                        gbNorth.Activate();
                        gbSouth.Activate();
                    }
                }

                Matrix4 transM = Matrix4.CreateTranslation(sx, sy, Const.GUILayer);
                prog.SetUniform("view", transM);
                s.DisplaySoldierDetails(prog, sx, sy, SelectedEntity == s, hover == s, Aspect);
                sy += PanelHeight + Const.GUIPanelGap;
            }
        }
        private void InitialiseGUIButtons() {
            gpSubMenu?.GetItem(I_View)?.Disable();
            gpSubMenu?.GetItem(I_Options)?.Disable();
            gpSubMenu?.GetItem(I_Mission)?.Enable();
            // Initialise all on startup
            lock (oInitialiseLock) {
                if (!bGUIButtonsInitialised) {
                    lButtons.Clear();

                    // ZoomTo buttons
                    SetupZoomToButtons();

                    // Direction Control buttons
                    TexSpecs ts = Textures.GetTexCoords(Textures.MiscTexture.Left);
                    gbWest = new GUIIconButton(this, ts, (0.99f - Const.GUIPanelWidth) + FrameBorder, 0f, ButtonSize, ButtonSize, SelectedSoldierMove, Utils.Direction.West);
                    lButtons.Add(gbWest);
                    ts = Textures.GetTexCoords(Textures.MiscTexture.Right);
                    gbEast = new GUIIconButton(this, ts, (0.99f - Const.GUIPanelWidth) + (FrameBorder * 3) + (ButtonSize * 2), 0f, ButtonSize, ButtonSize, SelectedSoldierMove, Utils.Direction.East);
                    lButtons.Add(gbEast);
                    ts = Textures.GetTexCoords(Textures.MiscTexture.Up);
                    gbNorth = new GUIIconButton(this, ts, (0.99f - Const.GUIPanelWidth) + (FrameBorder * 2) + ButtonSize, 0f, ButtonSize, ButtonSize, SelectedSoldierMove, Utils.Direction.North);
                    lButtons.Add(gbNorth);
                    ts = Textures.GetTexCoords(Textures.MiscTexture.Down);
                    gbSouth = new GUIIconButton(this, ts, (0.99f - Const.GUIPanelWidth) + (FrameBorder * 2) + ButtonSize, 0f, ButtonSize, ButtonSize, SelectedSoldierMove, Utils.Direction.South);
                    lButtons.Add(gbSouth);

                    // Misc controls
                    ts = Textures.GetTexCoords(Textures.MiscTexture.Attack);
                    gbAttack = new GUIIconButton(this, ts, (0.99f - Const.GUIPanelWidth) + (FrameBorder * 3f) + (ButtonSize * 3f) + (ButtonGap * 2f), 0f, ButtonSize * 1.5f, ButtonSize * 1.5f, SelectedSoldierAttack, null);
                    lButtons.Add(gbAttack);
                    ts = Textures.GetTexCoords(Textures.MiscTexture.Skills);
                    gbUseItem = new GUIIconButton(this, ts, (0.99f - Const.GUIPanelWidth) + (FrameBorder * 3f) + (ButtonSize * 4.5f) + (ButtonGap * 4f), 0f, ButtonSize * 1.5f, ButtonSize * 1.5f, SelectedSoldierUseItems, null);
                    lButtons.Add(gbUseItem);
                    ts = Textures.GetTexCoords(Textures.MiscTexture.Inventory);
                    gbInventory = new GUIIconButton(this, ts, (0.99f - Const.GUIPanelWidth) + (FrameBorder * 3f) + (ButtonSize * 3f) + (ButtonGap * 2f), 0f, ButtonSize * 1.5f, ButtonSize * 1.5f, SelectedSoldierInventory, null);
                    lButtons.Add(gbInventory);
                    ts = Textures.GetTexCoords(Textures.MiscTexture.Search);
                    gbSearch = new GUIIconButton(this, ts, (0.99f - Const.GUIPanelWidth) + (FrameBorder * 3f) + (ButtonSize * 4.5f) + (ButtonGap * 4f), 0f, ButtonSize * 1.5f, ButtonSize * 1.5f, SelectedSoldierSearch, null);
                    lButtons.Add(gbSearch);

                    // Other buttons
                    gbEndTurn = new GUIButton("End Turn", this, EndTurn);
                    gbEndTurn.Activate();
                    gbEndTurn.SetPosition(0.45f, 0.93f);
                    gbEndTurn.SetSize(0.09f, 0.05f);
                    gbEndTurn.SetBlend(true);

                    gbTransition = new GUIButton("Transition", this, Transition);
                    gbTransition.SetPosition(0.34f, 0.93f);
                    gbTransition.SetSize(0.09f, 0.05f);
                    gbTransition.SetBlend(true);

                    gbEndMission = new GUIButton("End Mission", this, EndMission);
                    gbEndMission.SetPosition(0.56f, 0.93f);
                    gbEndMission.SetSize(0.09f, 0.05f);
                    gbEndMission.SetBlend(true);

                    // Done
                    bGUIButtonsInitialised = true;

                    // Set button state as required
                    CheckForTransition();
                }
            }

            // Switch control buttons on/off when required
            if (SelectedEntity == null || SelectedEntity is not Soldier) {
                gbWest?.Deactivate();
                gbEast?.Deactivate();
                gbNorth?.Deactivate();
                gbSouth?.Deactivate();
                gbAttack?.Deactivate();
                gbSearch?.Deactivate();
                gbUseItem?.Deactivate();
                gbInventory?.Deactivate();
            }
            else {
                gbWest!.Activate();
                gbEast!.Activate();
                gbNorth!.Activate();
                gbSouth!.Activate();
                gbAttack!.Activate();
                gbSearch!.Activate();
                gbUseItem!.Activate();
                gbInventory!.Activate();
            }
        }
        private void SetupZoomToButtons() {
            if (ThisMission is null) return;
            TexSpecs ts = Textures.GetTexCoords(Textures.MiscTexture.Eye);
            if (ThisMission.Soldiers.Count >= 1) {
                if (gbZoomTo1 == null || gbZoomTo1.InternalData != ThisMission.Soldiers[0]) {
                    gbZoomTo1 = new GUIIconButton(this, ts, 0.99f - (ButtonSize + FrameBorder), 0f, ButtonSize, ButtonSize, ZoomToSoldier, ThisMission.Soldiers[0]);
                }
                if (!lButtons.Contains(gbZoomTo1)) lButtons.Add(gbZoomTo1);
                if (ThisMission.Soldiers.Count >= 2) {
                    if (gbZoomTo2 == null || gbZoomTo2.InternalData != ThisMission.Soldiers[1]) {
                        gbZoomTo2 = new GUIIconButton(this, ts, 0.99f - (ButtonSize + FrameBorder), 0f, ButtonSize, ButtonSize, ZoomToSoldier, ThisMission.Soldiers[1]);
                    }
                    if (!lButtons.Contains(gbZoomTo2)) lButtons.Add(gbZoomTo2);
                    if (ThisMission.Soldiers.Count >= 3) {
                        if (gbZoomTo3 == null || gbZoomTo3.InternalData != ThisMission.Soldiers[2]) {
                            gbZoomTo3 = new GUIIconButton(this, ts, 0.99f - (ButtonSize + FrameBorder), 0f, ButtonSize, ButtonSize, ZoomToSoldier, ThisMission.Soldiers[2]);
                        }
                        if (!lButtons.Contains(gbZoomTo3)) lButtons.Add(gbZoomTo3);
                        if (ThisMission.Soldiers.Count >= 4) {
                            if (gbZoomTo4 == null || gbZoomTo4.InternalData != ThisMission.Soldiers[3]) {
                                gbZoomTo4 = new GUIIconButton(this, ts, 0.99f - (ButtonSize + FrameBorder), 0f, ButtonSize, ButtonSize, ZoomToSoldier, ThisMission.Soldiers[3]);
                            }
                            if (!lButtons.Contains(gbZoomTo4)) lButtons.Add(gbZoomTo4);
                        }
                    }
                }
            }
            if (ThisMission.Soldiers.Count < 4) {
                if (gbZoomTo4 is not null && lButtons.Contains(gbZoomTo4)) lButtons.Remove(gbZoomTo4);
                gbZoomTo4 = null;
            }
            if (ThisMission.Soldiers.Count < 3) {
                if (gbZoomTo3 is not null && lButtons.Contains(gbZoomTo3)) lButtons.Remove(gbZoomTo3);
                gbZoomTo3 = null;
            }
            if (ThisMission.Soldiers.Count < 2) {
                if (gbZoomTo2 is not null && lButtons.Contains(gbZoomTo2)) lButtons.Remove(gbZoomTo2);
                gbZoomTo2 = null;
            }
            if (ThisMission.Soldiers.Count == 0) {
                if (gbZoomTo1 is not null && lButtons.Contains(gbZoomTo1)) lButtons.Remove(gbZoomTo1);
                gbZoomTo1 = null;
            }
        }
        private List<VertexPos2DCol> DrawTargetGrid() {
            List<VertexPos2DCol> vertices = new List<VertexPos2DCol>();
            Color4 col = new Color4(1f, 0f, 0f, 1f);
            for (int y = 0; y < CurrentLevel.Height; y++) {
                for (int x = 0; x < CurrentLevel.Width; x++) {
                    if (!TargetMap[x, y]) continue;
                    vertices.Add(new VertexPos2DCol(new Vector2(x, y), col));
                    vertices.Add(new VertexPos2DCol(new Vector2(x + 1f, y), col));
                    vertices.Add(new VertexPos2DCol(new Vector2(x + 1f, y + 1f), col));
                    vertices.Add(new VertexPos2DCol(new Vector2(x, y + 1f), col));
                }
            }
            return vertices;
        }
        private List<VertexPos2DCol> DrawAoEGrid() {
            List<VertexPos2DCol> vertices = new List<VertexPos2DCol>();
            Color4 col = new Color4(1f, 0f, 0f, 1f);
            for (int y = (int)Math.Max(hovery - AoERadius, 0); y < (int)Math.Min(hovery + AoERadius + 1, CurrentLevel.Height); y++) {
                for (int x = (int)Math.Max(hoverx - AoERadius, 0); x < (int)Math.Min(hoverx + AoERadius + 1, CurrentLevel.Width); x++) {
                    if (!AoEMap[x, y]) continue;
                    vertices.Add(new VertexPos2DCol(new Vector2(x, y), col));
                    vertices.Add(new VertexPos2DCol(new Vector2(x + 1f, y), col));
                    vertices.Add(new VertexPos2DCol(new Vector2(x + 1f, y + 1f), col));
                    vertices.Add(new VertexPos2DCol(new Vector2(x, y + 1f), col));
                }
            }
            return vertices;
        }
        private List<VertexPos2DCol> DrawTravelGrid() {
            List<VertexPos2DCol> vertices = new List<VertexPos2DCol>();
            Color4 col = new Color4(0f, 1f, 0f, 1f);
            for (int y = 0; y < CurrentLevel.Height; y++) {
                for (int x = 0; x < CurrentLevel.Width; x++) {
                    if (DistMap[x, y] == -1) continue;
                    vertices.Add(new VertexPos2DCol(new Vector2(x, y), col));
                    vertices.Add(new VertexPos2DCol(new Vector2(x + 1f, y), col));
                    vertices.Add(new VertexPos2DCol(new Vector2(x + 1f, y + 1f), col));
                    vertices.Add(new VertexPos2DCol(new Vector2(x, y + 1f), col));
                }
            }
            return vertices;
        }
        private void DrawTravelPath(ShaderProgram prog) {
            if (lCurrentPath == null || lCurrentPath.Count == 0) return;
            if (SelectedEntity == null || !(SelectedEntity is Soldier)) return;

            List<VertexPos3D> vertices = new List<VertexPos3D>();

            prog.SetUniform("textureEnabled", false);
            prog.SetUniform("flatColour", new Vector4(0f, 1f, 0f, 1f));

            vertices.Add(new VertexPos3D(new Vector3(SelectedEntity.X + 0.5f, SelectedEntity.Y + 0.5f, Const.DoodadLayer)));
            foreach (Point pt in lCurrentPath) {
                vertices.Add(new VertexPos3D(new Vector3(pt.X + 0.5f, pt.Y + 0.5f, Const.DoodadLayer)));
            }
            vertices.Add(new VertexPos3D(new Vector3(hoverx + 0.5f, hovery + 0.5f, Const.DoodadLayer)));

            if (vbPath is null) vbPath = new VertexBuffer(vertices.ToArray(), BufferUsageHint.DynamicDraw);
            else vbPath.SetData(vertices.ToArray());
            if (vaPath is null) vaPath = new VertexArray(vbPath);
            prog.SetUniform("model", Matrix4.Identity);
            GL.UseProgram(prog.ShaderProgramHandle);
            GL.BindVertexArray(vaPath.VertexArrayHandle);
            GL.DrawArrays(PrimitiveType.LineStrip, 0, vbPath.VertexCount);
            GL.BindVertexArray(0);

            prog.SetUniform("flatColour", new Vector4(1f, 1f, 0f, 1f));
            Matrix4 pTranslateM = Matrix4.CreateTranslation(hoverx + 0.45f, hovery + 0.45f, Const.DoodadLayer);
            Matrix4 pScaleM = Matrix4.CreateScale(0.1f);
            prog.SetUniform("model", pScaleM * pTranslateM);
            GL.UseProgram(prog.ShaderProgramHandle);
            Square.Flat.BindAndDraw();
        }
        private void DrawHoverFrame(ShaderProgram prog, int xpos, int ypos) {
            float px = xpos + 0.5f, py = ypos + 0.5f;
            float xSize = 1f, ySize = 1f;
            // Hovering over a large entity
            IEntity? en = CurrentLevel.GetEntityAt(xpos, ypos);
            if (en != null && en.Size > 1) {
                px = en.X + (en.Size / 2f);
                py = en.Y + (en.Size / 2f);
                xSize = en.Size;
                ySize = en.Size;
            }
            // Hovering over a door
            else if (CurrentLevel.Map[xpos, ypos] == MissionLevel.TileType.DoorHorizontal || CurrentLevel.Map[xpos, ypos] == MissionLevel.TileType.OpenDoorHorizontal) {
                int endx = xpos, startx = xpos;
                while (startx - 1 > 0 && CurrentLevel.Map[startx - 1, ypos] == CurrentLevel.Map[xpos, ypos]) startx--;
                while (endx + 1 < CurrentLevel.Width - 1 && CurrentLevel.Map[endx + 1, ypos] == CurrentLevel.Map[xpos, ypos]) endx++;
                px = (startx + endx) / 2f + 0.5f;
                xSize = Math.Abs(startx - endx) + 1f;
            }
            else if (CurrentLevel.Map[xpos, ypos] == MissionLevel.TileType.DoorVertical || CurrentLevel.Map[xpos, ypos] == MissionLevel.TileType.OpenDoorVertical) {
                int endy = ypos, starty = ypos;
                while (starty - 1 > 0 && CurrentLevel.Map[xpos, starty - 1] == CurrentLevel.Map[xpos, ypos]) starty--;
                while (endy + 1 < CurrentLevel.Height - 1 && CurrentLevel.Map[xpos, endy + 1] == CurrentLevel.Map[xpos, ypos]) endy++;
                py = (starty + endy) / 2f + 0.5f;
                ySize = Math.Abs(starty - endy) + 1f;
            }
            if (CurrentAction == SoldierAction.Attack || CurrentAction == SoldierAction.Item) prog.SetUniform("flatColour", new Vector4(1f, 0f, 0f, 1f));
            else prog.SetUniform("flatColour", new Vector4(0f, 1f, 0f, 1f));
            float dx = -(xSize / 2f);
            float dy = -(ySize / 2f);

            prog.SetUniform("textureEnabled", false);
            Matrix4 pTranslateM = Matrix4.CreateTranslation(px + dx, py + dy, Const.EntityLayer);
            Matrix4 pScaleM = Matrix4.CreateScale(xSize, ySize, 1f);
            prog.SetUniform("model", pScaleM * pTranslateM);
            GL.UseProgram(prog.ShaderProgramHandle);
            SquareRing.Flat.BindAndDraw();
        }
        private static void DrawSelectionTile(ShaderProgram prog, float px, float py, float dSize) {
            float d = -(dSize / 2f);
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
            prog.SetUniform("textureEnabled", true);
            prog.SetUniform("flatColour", new Vector4(1f, 1f, 1f, 1f));
            GL.BindTexture(TextureTarget.Texture2D, Textures.SelectionTexture);
            prog.SetUniform("texPos", 0f, 0f);
            prog.SetUniform("texScale", 1f, 1f);
            Matrix4 pTranslateM = Matrix4.CreateTranslation(px + d, py + d, Const.DoodadLayer);
            Matrix4 pScaleM = Matrix4.CreateScale(dSize);
            prog.SetUniform("model", pScaleM * pTranslateM);
            GL.UseProgram(prog.ShaderProgramHandle);
            Square.Textured.BindAndDraw();
        }

        // Setup mouse-hover context menu on right-click
        private void SetupContextMenu() {
            float px = (float)MousePosition.X / (float)Size.X + 0.01f;
            float py = (float)MousePosition.Y / (float)Size.Y + 0.01f;
            gpSelect ??= new GUIPanel(this, px, py);
            gpSelect.Reset();
            gpSelect.SetClick(hoverx, hovery);
            gpSelect.SetIconScale(0.7f);
            gpSelect.SetPosition(px, py);
            CheckHoverMission();

            if (SelectedEntity is not Soldier s) {
                gpSelect.Deactivate();
                return;
            }
            if (s.GoTo != Point.Empty) return; // Walking somewhere so skip creating the context menu

            if (hoverx < 0 || hoverx >= CurrentLevel.Width || hovery < 0 || hovery >= CurrentLevel.Height) {
                gpSelect.Deactivate();
                return;
            }

            // Open doors
            if (CurrentLevel.Map[hoverx, hovery] == MissionLevel.TileType.DoorHorizontal) {
                bool bEntityIsAdjacent = CurrentLevel.EntityIsAdjacentToDoor(SelectedEntity, hoverx, hovery);
                TexSpecs ts = Textures.GetTexCoords(Textures.MiscTexture.OpenDoor);
                gpSelect.InsertIconItem(I_OpenDoor, ts, bEntityIsAdjacent, null);
            }
            if (CurrentLevel.Map[hoverx, hovery] == MissionLevel.TileType.DoorVertical) {
                bool bEntityIsAdjacent = CurrentLevel.EntityIsAdjacentToDoor(SelectedEntity, hoverx, hovery);
                TexSpecs ts = Textures.GetTexCoords(Textures.MiscTexture.OpenDoor);
                gpSelect.InsertIconItem(I_OpenDoor, ts, bEntityIsAdjacent, null);
            }

            // Close doors
            if (CurrentLevel.Map[hoverx, hovery] == MissionLevel.TileType.OpenDoorHorizontal && CurrentLevel.GetEntityAt(hoverx, hovery) == null) {
                bool bEntityIsAdjacent = CurrentLevel.EntityIsAdjacentToDoor(SelectedEntity, hoverx, hovery);
                TexSpecs ts = Textures.GetTexCoords(Textures.MiscTexture.CloseDoor);
                gpSelect.InsertIconItem(I_CloseDoor, ts, bEntityIsAdjacent, null);
            }
            if (CurrentLevel.Map[hoverx, hovery] == MissionLevel.TileType.OpenDoorVertical && CurrentLevel.GetEntityAt(hoverx, hovery) == null) {
                bool bEntityIsAdjacent = CurrentLevel.EntityIsAdjacentToDoor(SelectedEntity, hoverx, hovery);
                TexSpecs ts = Textures.GetTexCoords(Textures.MiscTexture.CloseDoor);
                gpSelect.InsertIconItem(I_CloseDoor, ts, bEntityIsAdjacent, null);
            }

            // Passable square
            if (Utils.IsPassable(CurrentLevel.Map[hoverx, hovery])) {
                IEntity? en = CurrentLevel.GetEntityAt(hoverx, hovery);
                // Walk to this point
                if (en == null) {
                    TexSpecs ts = Textures.GetTexCoords(Textures.MiscTexture.Walk);
                    bool bIsInRange = DistMap[hoverx, hovery] != -1;
                    gpSelect.InsertIconItem(I_GoTo, ts, bIsInRange, null);
                }
                // Attack a creature
                else if (en is Creature) {
                    TexSpecs tsa = Textures.GetTexCoords(Textures.MiscTexture.Attack);
                    TexSpecs tsm = Textures.GetTexCoords(Textures.MiscTexture.Moved);
                    bool bIsInRange = SelectedEntity.CanSee(en) && SelectedEntity.RangeTo(en) <= SelectedEntity.AttackRange;
                    bool bEnabled = bIsInRange && (s.Stamina >= s.AttackCost);
                    if (s.EquippedWeapon is not null && s.EquippedWeapon.Type.Area > 0) bEnabled = false;
                    if (s.EquippedWeapon is not null && s.EquippedWeapon.Type.Stable && s.HasMoved) {
                        gpSelect.InsertIconItem(I_Attack, tsm, false, null);
                    }
                    else gpSelect.InsertIconItem(I_Attack, tsa, bEnabled, null);
                }
            }

            // Click on self
            if (hoverx == s.X && hovery == s.Y) {
                bool bHasUtilityItems = s.HasUtilityItems();
                GUIPanel? gpItems = null;
                if (bHasUtilityItems) {
                    TexSpecs tsr = Textures.GetTexCoords(Textures.MiscTexture.Reuse);
                    TexSpecs tss = Textures.GetTexCoords(Textures.MiscTexture.Stopwatch);
                    bool bEnabled = s.Stamina >= s.UseItemCost;
                    gpItems = new GUIPanel(this);
                    // Set up list of items
                    foreach (Equipment eq in s.GetUtilityItems()) {
                        TexSpecs ts = Textures.GetTexCoords(eq.BaseType);
                        PanelItem ip = gpItems.InsertIconItem(eq, ts, bEnabled && (eq.Recharge == 0), null);
                        if (eq.BaseType.ItemEffect != null && !eq.BaseType.ItemEffect.SingleUse) {
                            if (eq.Recharge == 0) ip.SetOverlay(tsr, new Vector4(0.7f, 0.0f, 0.3f, 0.3f));
                            else ip.SetOverlay(tss, new Vector4(0.7f, 0.0f, 0.3f, 0.3f));
                        }
                    }
                }
                TexSpecs tsi = Textures.GetTexCoords(Textures.MiscTexture.Inventory);
                gpSelect.InsertIconItem(I_UseItem, tsi, bHasUtilityItems, gpItems);
            }
            if (gpSelect.Count == 0) {
                gpSelect.Deactivate();
                return;
            }
            gpSelect.Activate();
        }

        #region ButtonHandlers
        private void CentreView(IEntity ent) {
            fMissionViewX = ent.X;
            fMissionViewY = ent.Y;
        }
        private void CentreViewForceRedraw(IEntity ent) {
            fMissionViewX = ent.X;
            fMissionViewY = ent.Y;
        }
        private void ZoomToSoldier(GUIIconButton sender) {
            if (bAIRunning) return;
            if (sender.InternalData is not Soldier s) {
                throw new Exception("ZoomToSoldier: GUIIconButton did not have Soldier set as internal data");
            }
            SetSelectedEntity(s);
            CentreView(s);
        }
        private void SelectedSoldierMove(GUIIconButton sender) {
            if (bAIRunning) return;
            if (sender?.InternalData is null) throw new Exception("MoveSelectedSoldier: GUIIconButton did not have Direction set as internal data");
            Utils.Direction d = (Utils.Direction)sender.InternalData;
            if (SelectedEntity == null || SelectedEntity is not Soldier s) throw new Exception("SelectedSoldierMove: SelectedSoldier not set!");
            MoveSoldier(s, d);
            CurrentAction = SoldierAction.None;
            ActionItem = null;
            if (s.PlayerTeam != null) { // Not dead
                GenerateDistMap(SelectedEntity as Soldier);
                GenerateDetectionMap();
                CheckForTraps(s);
            }
        }
        private void SelectedSoldierAttack(GUIIconButton sender) {
            if (bAIRunning) return;
            if (SelectedEntity == null || !(SelectedEntity is Soldier)) throw new Exception("SelectedSoldierAttack: SelectedSoldier not set!");
            Soldier s = (Soldier)SelectedEntity;
            GenerateTargetMap(s, s.AttackRange);
            CurrentAction = SoldierAction.Attack;
            if (s.EquippedWeapon != null && s.EquippedWeapon.Type.Area > 0) {
                GenerateAoEMap(s.X, s.Y, s.EquippedWeapon.Type.Area);
            }
        }
        private void SelectedSoldierUseItems(GUIIconButton sender) {
            if (bAIRunning) return;
            if (SelectedEntity is not Soldier s) throw new Exception("SelectedSoldierUseItems: SelectedSoldier not set!");
            using (UseItem ui = new UseItem(s)) {
                ui.ShowDialog();
                if (ui.ChosenItem != null) {
                    ActionItem = ui.ChosenItem;
                    CurrentAction = SoldierAction.Item;
                    if (ActionItem.BaseType.ItemEffect is not null) {
                        GenerateTargetMap(s, ActionItem.BaseType.ItemEffect.Range);
                        int sy = SelectedEntity.Y;
                        int sx = SelectedEntity.X;
                        GenerateAoEMap(sx, sy, ActionItem.BaseType.ItemEffect.Radius);
                    }
                }
            }
        }
        private void SelectedSoldierInventory(GUIIconButton sender) {
            if (bAIRunning) return;
            if (SelectedEntity == null || SelectedEntity is not Soldier ss) throw new Exception("SelectedSoldierInventory: SelectedSoldier not set!");
            OpenSoldierInventory(ss);
        }
        private void OpenSoldierInventory(Soldier ss) {
            int sy = ss.Y;
            int sx = ss.X;
            Stash? st = CurrentLevel.GetStashAtPoint(sx, sy);
            if (st is null) st = new Stash(new Point(sx, sy));
            EquipmentView eqv = new EquipmentView(ss, st);
            eqv.ShowDialog();
            CurrentLevel.ReplaceStashAtPosition(sx, sy, st);
        }
        private void SelectedSoldierSearch(GUIIconButton? sender) {
            if (bAIRunning) return;
            if (SelectedEntity is not Soldier ss) return;
            if (ss.Stamina < ss.SearchCost) return;
            List<string> lFound = ss.PerformActiveSearch(CurrentLevel);
            if (lFound.Count == 0) msgBox.PopupMessage("Despite a thorough search, you found nothing");
            else msgBox.PopupMessage(lFound);
            GenerateDistMap(ss);
            GenerateDetectionMap();
            CurrentLevel.CalculatePlayerVisibility();
        }
        private async void EndTurn() {
            if (bAIRunning) return;
            foreach (Soldier s in CurrentLevel.Soldiers) {
                if (s.GoTo != Point.Empty) return; // Can't end turn if soldiers are still moving
            }

            bAIRunning = true;

            // Handle periodic effects not aimed at an entity e.g. burning floor, delayed-timer explosives
            // TODO

            // Reset stamina, do periodic effects etc.
            List<Soldier> lSoldiers = new List<Soldier>(ThisMission!.Soldiers); // In case one dies
            foreach (Soldier s in lSoldiers) {
                await Task.Run(() => s.EndOfTurn(AddNewEffect, CentreViewForceRedraw, PlaySoundThreaded, AnnounceMessage));
                if (s.PlayerTeam == null && SelectedEntity == s) SelectedEntity = null;
            }

            // Creature AI
            await Task.Run(() => CurrentLevel.RunCreatureTurn(AddNewEffect, CentreViewForceRedraw, PostMoveCheck, PlaySoundThreaded, AnnounceMessage));

            // All done
            if (SelectedEntity != null && SelectedEntity is Soldier se && se.PlayerTeam == null) SelectedEntity = null;
            GenerateDistMap(SelectedEntity as Soldier);
            GenerateDetectionMap();
            SelectedEntity?.UpdateVisibility(CurrentLevel);
            CurrentLevel.CalculatePlayerVisibility();
            bAIRunning = false;
            Const.dtTime.AddSeconds(Const.TurnLength);
        }
        private void PlaySoundThreaded(string strSound) {
            ThisDispatcher?.BeginInvoke((Action)(() => { SoundEffects.PlaySound(strSound); }));
            //ThisDispatcher.Invoke(() => { SoundEffects.PlaySound(strSound); });
        }
        private void AnnounceMessage(string strMsg, Action? _onClick) {
            ThisDispatcher?.Invoke(() => { msgBox.PopupMessage(strMsg, _onClick); });
        }
        private void PostMoveCheck(IEntity en) {
            // Stuff we need to check after a creature moves
            GenerateDetectionMap();
            foreach (Soldier s in CurrentLevel.Soldiers) {
                if (UpdateDetectionForSoldier(s)) {
                    AnnounceMessage(s.Name + " has been detected by the enemy!", null);
                }
            }
        }
        private void Transition() {
            if (bAIRunning) return;
            int oldlev = ThisMission!.CurrentLevel;
            int newlev = oldlev;
            // Check if we're going up or down, or exiting
            if (CurrentLevel.CheckAllSoldiersAtEntrance()) {
                newlev--;
            }
            else if (CurrentLevel.CheckAllSoldiersAtExit()) {
                newlev++;
            }
            else return;

            // Leave the dungeon?
            if (newlev == -1) {
                MissionOutcome = MissionResult.Evacuated;
                CeaseMission();
                return;
            }
            if (newlev >= ThisMission.LevelCount) throw new Exception("Attempting to transition to illegal level");
            CurrentLevel.RemoveAllSoldiers();
            ThisMission.SetCurrentLevel(newlev);
            CurrentLevel = ThisMission.GetOrCreateCurrentLevel();

            // Redim the various maps
            TargetMap = new bool[CurrentLevel.Width, CurrentLevel.Height];
            DistMap = new int[CurrentLevel.Width, CurrentLevel.Height];
            AoEMap = new bool[CurrentLevel.Width, CurrentLevel.Height];
            DetectionMap = new bool[CurrentLevel.Width, CurrentLevel.Height];

            // Update all soldiers
            foreach (Soldier s in ThisMission.Soldiers) {
                if (newlev > oldlev) CurrentLevel.AddSoldier(s);
                else CurrentLevel.AddSoldierAtExit(s);
                UpdateLevelAfterSoldierMove(s);
            }
            CentreView(ThisMission.Soldiers[0]);
        }
        private void EndMission() {
            if (bAIRunning) return;
            if (CurrentLevel.AlertedEnemies) {
                msgBox.PopupMessage("You cannot leave this mission\nThere are enemies alerted to your presence");
                return;
            }
            if (ThisMission!.Type == Mission.MissionType.RepelBoarders) {
                msgBox.PopupConfirmation("Deactivate ship defence systems and return to your posts?", EndMission_Victory);
            }
            else {
                msgBox.PopupConfirmation("Leave this mission and return to the ship?", EndMission_Victory);
            }
        }
        private void EndMission_Victory() {
            MissionOutcome = MissionResult.Victory;
            CeaseMission();
        }
        #endregion //  ButtonHandlers

        // Generate maps for overlays
        private void SetSelectedEntity(IEntity? en) {
            SelectedEntity = en;
            GenerateDetectionMap();
            if (en != null && en is Soldier s) {
                GenerateDistMap(s);
            }
        }
        private void GenerateTargetMap(Soldier s, double range) {
            // Check every square
            for (int y = 0; y < CurrentLevel.Height; y++) {
                for (int x = 0; x < CurrentLevel.Width; x++) {
                    if (!Utils.IsPassable(CurrentLevel.Map[x, y])) {
                        TargetMap[x, y] = false;
                    }
                    else {
                        TargetMap[x, y] = s.CanSee(x, y);
                        if (TargetMap[x, y]) {
                            // Check range
                            double r2 = (x - s.X) * (x - s.X) + (y - s.Y) * (y - s.Y);
                            TargetMap[x, y] = (r2 <= (range * range));
                        }
                    }
                }
            }
        }
        private void GenerateAoEMap(int px, int py, double range, int ox = -1, int oy = -1) {
            int startx = 0, starty = 0, endx = CurrentLevel.Width - 1, endy = CurrentLevel.Height - 1;
            // Check every square
            if (ox >= 0 && oy >= 0) {
                startx = (int)Math.Max(Math.Min(ox, px) - range, 0);
                starty = (int)Math.Max(Math.Min(oy, py) - range, 0);
                endx = (int)Math.Min(Math.Max(ox, px) + range, CurrentLevel.Width - 1);
                endy = (int)Math.Min(Math.Max(oy, py) + range, CurrentLevel.Height - 1);
            }
            for (int y = starty; y <= endy; y++) {
                for (int x = startx; x <= endx; x++) {
                    if (!Utils.IsPassable(CurrentLevel.Map[x, y])) {
                        AoEMap[x, y] = false;
                    }
                    else {
                        double r2 = (x - px) * (x - px) + (y - py) * (y - py);
                        AoEMap[x, y] = (r2 <= (range * range));
                    }
                }
            }
            AoERadius = (int)Math.Ceiling(range);
        }
        private void GenerateDistMap(Soldier? s) {
            if (s == null) return;
            for (int y = 0; y < CurrentLevel.Height; y++) {
                for (int x = 0; x < CurrentLevel.Width; x++) {
                    DistMap[x, y] = -1;
                }
            }
            int maxdist = s.TravelRange;
            DistMap[s.X, s.Y] = 0;
            if (s.X > 0) FloodFillDist(s.X - 1, s.Y, 1, maxdist);
            if (s.X < CurrentLevel.Width - 1) FloodFillDist(s.X + 1, s.Y, 1, maxdist);
            if (s.Y > 0) FloodFillDist(s.X, s.Y - 1, 1, maxdist);
            if (s.Y < CurrentLevel.Height - 1) FloodFillDist(s.X, s.Y + 1, 1, maxdist);
        }
        private void FloodFillDist(int x, int y, int dist, int maxdist) {
            if (!CurrentLevel.Explored[x, y]) return;
            if (!Utils.IsPassable(CurrentLevel.Map[x, y])) return;
            if (CurrentLevel.GetEntityAt(x, y) != null) return;
            Trap? tr = CurrentLevel.GetTrapAtPoint(x, y);
            if (tr != null && !tr.Hidden) return;
            if (dist > maxdist) return;
            DistMap[x, y] = dist;
            if (dist == maxdist) return;
            if (x > 0 && (DistMap[x - 1, y] == -1 || DistMap[x - 1, y] > dist)) FloodFillDist(x - 1, y, dist + 1, maxdist);
            if (x < CurrentLevel.Width - 1 && (DistMap[x + 1, y] == -1 || DistMap[x + 1, y] > dist)) FloodFillDist(x + 1, y, dist + 1, maxdist);
            if (y > 0 && (DistMap[x, y - 1] == -1 || DistMap[x, y - 1] > dist)) FloodFillDist(x, y - 1, dist + 1, maxdist);
            if (y < CurrentLevel.Height - 1 && (DistMap[x, y + 1] == -1 || DistMap[x, y + 1] > dist)) FloodFillDist(x, y + 1, dist + 1, maxdist);
        }
        private void GenerateDetectionMap() {
            // Clear the grid
            for (int y = 0; y < CurrentLevel.Height; y++) {
                for (int x = 0; x < CurrentLevel.Width; x++) {
                    DetectionMap[x, y] = false;
                }
            }
            Soldier? s = SelectedEntity as Soldier;
            if (s == null) return;

            // Check every nearby entity
            foreach (Creature cr in CurrentLevel.Creatures) {
                if (!cr.IsAlert && (CurrentLevel.Visible[cr.X, cr.Y] || Const.DEBUG_VISIBLE_ALL)) {
                    double range = cr.SoldierVisibilityRange(s);
                    for (int y = Math.Max(0, (int)Math.Floor(cr.Y - range)); y <= Math.Min(CurrentLevel.Height - 1, (int)Math.Ceiling(cr.Y + range)); y++) {
                        for (int x = Math.Max(0, (int)Math.Floor(cr.X - range)); x <= Math.Min(CurrentLevel.Width - 1, (int)Math.Ceiling(cr.X + range)); x++) {
                            if (Utils.IsPassable(CurrentLevel.Map[x, y]) && !DetectionMap[x, y]) {
                                // Check range
                                double r2 = (x - cr.X) * (x - cr.X) + (y - cr.Y) * (y - cr.Y);
                                if (r2 <= (range * range)) { // Check range properly
                                    DetectionMap[x, y] |= cr.CanSee(x, y);
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}