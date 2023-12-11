using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;
using SpaceMercs.Dialogs;
using SpaceMercs.Graphics;
using SpaceMercs.Graphics.Shapes;
using Keys = OpenTK.Windowing.GraphicsLibraryFramework.Keys;

namespace SpaceMercs.MainWindow {
    // Partial class including functions for viewing the team ship
    partial class MapView {
        private float fShipViewX, fShipViewY, fShipViewZ = Const.MinimumShipViewZ;
        private int irHover = -1, irContextRoom = -1, irSelected = -1;
        private PanelItem? piHoverItem;
        private GUIButton gbRepair, gbFabricate, gbBack;
        private bool bHoverHull = false, bContextHull = false;
        private ViewMode PreviousViewMode = ViewMode.ViewMap;

        // Possible icon IDs (must be larger than the number of ShipEquipment)
        public const uint I_Build = 10000;
        public const uint I_Cancel = 10001;
        public const uint I_Salvage = 10002;
        public const uint I_Timer = 10003;
        public const uint I_Disconnect = 10004;
        public const uint I_Connect = 10005;
        public const uint I_None = 10006;

        private void SetupShipView() {
            if (view == ViewMode.ViewShip) return;
            PreviousViewMode = view;
            view = ViewMode.ViewShip;
            fShipViewX = fShipViewY = 0.0f;
            fShipViewZ = PlayerTeam.PlayerShip.Length * 2;
            if (fShipViewZ < Const.MinimumShipViewZ) fShipViewZ = Const.MinimumShipViewZ;
            if (gpSelect == null) gpSelect = new GUIPanel(this);
            if (gbRepair == null) gbRepair = new GUIButton("Repair", this, RepairShip);
            if (gbBack == null) gbBack = new GUIButton("Back", this, BackToMap);
            if (gbFabricate == null) gbFabricate = new GUIButton("Fabricate", this, FabricateItems);
            SetupUtilityButtons();
        }
        private void SetupUtilityButtons() {
            gbRepair.Deactivate();
            gbFabricate.Deactivate();

            // "Back to last view" button
            gbBack.SetPosition(0.01f, 0.05f);
            gbBack.SetSize(0.06f, 0.03f);
            gbBack.SetBlend(false);
            gbBack.Activate();

            // Not at a base
            if (PlayerTeam.CurrentPositionHAO is null) return;

            // "Repair Ship" button
            gbRepair.SetPosition(0.89f, 0.85f);
            gbRepair.SetSize(0.08f, 0.03f);
            gbRepair.SetBlend(false);
            if (PlayerTeam.PlayerShip.HullFract < 1.0 && // Ship is damaged
                PlayerTeam.CurrentPositionHAO.Colony is not null &&
                PlayerTeam.GetRelations(PlayerTeam.CurrentPositionHAO.Colony.Owner) >= Const.RaceRelationsLevelToAllowShipRepair && // Neutral or better with the owner race
                (PlayerTeam.CurrentPositionHAO.Base & (Colony.BaseType.Colony | Colony.BaseType.Metropolis | Colony.BaseType.Military)) != 0)  // Only colonies, metropolis or military bases can repair
            {
                gbRepair.Activate();
            }

            // Fabrication Button
            gbFabricate.SetPosition(0.89f, 0.9f);
            gbFabricate.SetSize(0.08f, 0.03f);
            gbFabricate.SetBlend(false);
            if (PlayerTeam.PlayerShip.HasArmoury) gbFabricate.Activate();
            else if (PlayerTeam.PlayerShip.HasWorkshop) gbFabricate.Activate();
            else if (PlayerTeam.PlayerShip.HasMedlab) gbFabricate.Activate();
            else if (PlayerTeam.PlayerShip.HasEngineering) gbFabricate.Activate();
        }

        private void DrawShip() {
            if (!bLoaded || PlayerTeam?.PlayerShip is null) return;

            // Set the correct view location & perspective matrix
            Matrix4 projectionUnitM = Matrix4.CreateOrthographicOffCenter(0.0f, 1.0f, 1.0f, 0.0f, -1.0f, 1.0f);
            pos2DCol4ShaderProgram.SetUniform("projection", projectionUnitM);
            pos2DCol4ShaderProgram.SetUniform("view", Matrix4.Identity);
            pos2DCol4ShaderProgram.SetUniform("model", Matrix4.Identity);

            Matrix4 projectionM = Matrix4.CreatePerspectiveFieldOfView(Const.MapViewportAngle, (float)Aspect, 0.05f, 5000.0f);
            fullShaderProgram.SetUniform("projection", projectionM);
            fullShaderProgram.SetUniform("model", Matrix4.Identity);
            fullShaderProgram.SetUniform("flatColour", new Vector4(1f, 1f, 1f, 1f));
            fullShaderProgram.SetUniform("texPos", 0f, 0f);
            fullShaderProgram.SetUniform("texScale", 1f, 1f);
            fullShaderProgram.SetUniform("textureEnabled", false);
            fullShaderProgram.SetUniform("lightEnabled", false);

            GL.ClearColor(Color.Black);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            GL.Enable(EnableCap.DepthTest);

            // Draw the starfield (unmoving)
            pos2DCol4ShaderProgram.SetUniform("colourFactor", 1f);
            GL.UseProgram(pos2DCol4ShaderProgram.ShaderProgramHandle);
            Starfield.Build.BindAndDraw();

            // Shift to the correct location
            Matrix4 translateM = Matrix4.CreateTranslation(-fShipViewX, -fShipViewY, -fShipViewZ);
            fullShaderProgram.SetUniform("view", translateM);

            // Draw the ship
            GL.Disable(EnableCap.DepthTest);
            PlayerTeam.PlayerShip.DrawSchematic(fullShaderProgram, irHover, bHoverHull);

            // Draw any static GUI elements
            ShowShipGUI();
        }

        private void GetKeyboardInput_Ship() {
            if (bAIRunning) return;
            if (IsKeyPressed(Keys.Escape)) {
                view = PreviousViewMode;
            }
        }

        private void BackToMap() {
            view = PreviousViewMode;
        }

        // Mouse stuff
        private void MouseMove_Ship(MouseMoveEventArgs e) {
            if (MouseState.IsButtonDown(MouseButton.Left)) {
                float fScale = Const.MouseMoveScale * fShipViewZ / 50.0f;
                if (IsKeyDown(Keys.LeftControl) || IsKeyDown(Keys.RightControl)) fScale *= 2.5f;
                else if (IsKeyDown(Keys.LeftShift) || IsKeyDown(Keys.RightShift)) fScale /= 2.5f;
                fShipViewX -= e.DeltaX * fScale;
                if (fShipViewX < -PlayerTeam.PlayerShip.Length / 1.9f) fShipViewX = -PlayerTeam.PlayerShip.Length / 1.9f;
                if (fShipViewX > PlayerTeam.PlayerShip.Length / 1.9f) fShipViewX = PlayerTeam.PlayerShip.Length / 1.9f;
                fShipViewY += e.DeltaY * fScale;
                if (fShipViewY < -PlayerTeam.PlayerShip.Width / 1.9f) fShipViewY = -PlayerTeam.PlayerShip.Width / 1.9f;
                if (fShipViewY > PlayerTeam.PlayerShip.Width / 1.9f) fShipViewY = PlayerTeam.PlayerShip.Width / 1.9f;
            }
            gbRepair.IsHover((int)MousePosition.X, (int)MousePosition.Y);
            gbFabricate.IsHover((int)MousePosition.X, (int)MousePosition.Y);
            gbBack.IsHover((int)MousePosition.X, (int)MousePosition.Y);
            CheckHover_Ship();
        }
        private void MouseUp_Ship(MouseButtonEventArgs e) {
            // Check R-button released
            if (e.Button == MouseButton.Right) {
                if (gpSelect.Active) {
                    gpSelect.Deactivate();
                    int iSelectHover = gpSelect.HoverID;
                    // Process GUIPanel selection
                    if (iSelectHover >= 0) {
                        if (iSelectHover < StaticData.ShipEquipment.Count) {
                            if (bContextHull) PlayerTeam.PlayerShip.UpgradeHull(StaticData.ShipEquipment[iSelectHover]);
                            else PlayerTeam.PlayerShip.BuildEquipment(irContextRoom, StaticData.ShipEquipment[iSelectHover]);
                        }
                        if (iSelectHover == I_Salvage) {
                            if (bContextHull) PlayerTeam.PlayerShip.SalvageHull();
                            else PlayerTeam.PlayerShip.SalvageRoom(irContextRoom);
                        }
                        if (iSelectHover == I_Disconnect) {
                            PlayerTeam.PlayerShip.DeactivateRoom(irContextRoom);
                        }
                        if (iSelectHover == I_Connect) {
                            PlayerTeam.PlayerShip.ActivateRoom(irContextRoom);
                        }
                    }
                }
                irContextRoom = -1;
                bContextHull = false;
            }
            if (e.Button == MouseButton.Left) {
                if (gbRepair.CaptureClick((int)MousePosition.X, (int)MousePosition.Y)) return;
                if (gbFabricate.CaptureClick((int)MousePosition.X, (int)MousePosition.Y)) return;
                if (gbBack.CaptureClick((int)MousePosition.X, (int)MousePosition.Y)) return;
            }
            CheckHover_Ship();
            SetupUtilityButtons();
        }
        private void MouseDown_Ship(MouseButtonEventArgs e) {
            if (e.Button == MouseButton.Right) {
                SetupContextMenu_Ship();
            }
            if (e.Button == MouseButton.Left) {
                irSelected = irHover;
            }
        }
        private void DoubleClick_Ship() {
            fShipViewX = fShipViewY = 0.0f;
            fShipViewZ = PlayerTeam.PlayerShip.Length * 2;
            if (fShipViewZ < Const.MinimumShipViewZ) fShipViewZ = Const.MinimumShipViewZ;
        }
        private void MouseWheel_Ship(float delta) {
            fShipViewZ += delta;
            if (fShipViewZ < Const.MinimumShipViewZ) fShipViewZ = Const.MinimumShipViewZ;
            if (fShipViewZ > Const.MaximumShipViewZ) fShipViewZ = Const.MaximumShipViewZ;
        }

        // Check if we're hovering on a specific room
        private void CheckHover_Ship() {
            irHover = -1;
            piHoverItem = null;
            // Check GUIPanel first
            if (gpSelect.Active && gpSelect.HoverItem != null) {
                piHoverItem = gpSelect.HoverItem;
                return;
            }
            // Calculate the position of the mouse pointer
            double mxfract = (double)MousePosition.X / (double)Size.X;
            double myfract = (double)MousePosition.Y / (double)Size.Y;
            double mxpos = ((mxfract - 0.5) * (fShipViewZ / 1.86) * Aspect) + fShipViewX;
            double mypos = ((0.5 - myfract) * (fShipViewZ / 1.86)) + fShipViewY;
            irHover = PlayerTeam.PlayerShip.CheckHoverRoom(mxpos, mypos, out bHoverHull);
        }

        // Show static GUI elements
        private void ShowShipGUI() {
            Matrix4 projectionM = Matrix4.CreateOrthographicOffCenter(0.0f, 1.0f, 1.0f, 0.0f, -1.0f, 1.0f);
            fullShaderProgram.SetUniform("projection", projectionM);
            fullShaderProgram.SetUniform("view", Matrix4.Identity);
            fullShaderProgram.SetUniform("model", Matrix4.Identity);
            flatColourShaderProgram.SetUniform("projection", projectionM);
            flatColourShaderProgram.SetUniform("view", Matrix4.Identity);
            flatColourShaderProgram.SetUniform("model", Matrix4.Identity);

            // Show the context menu, if it's available
            gpSelect.Display((int)MousePosition.X, (int)MousePosition.Y, fullShaderProgram);

            // Display the current date and time
            TextRenderer.DrawAt(Const.dtTime.ToString("F"), Alignment.TopLeft, 0.03f, Aspect, 0.01f, 0.01f);

            // Display the player's remaining cash reserves
            TextRenderer.DrawAt($"{PlayerTeam.Cash.ToString("F2")} credits", Alignment.TopRight, 0.03f, Aspect, 0.99f, 0.01f);

            if (irSelected != -1) DisplaySelectionText_Ship();

            DrawHullCondition();
            DrawPowerBar();

            // Display all buttons
            gbRepair.Display((int)MousePosition.X, (int)MousePosition.Y, fullShaderProgram);
            gbFabricate.Display((int)MousePosition.X, (int)MousePosition.Y, fullShaderProgram);
            gbBack.Display((int)MousePosition.X, (int)MousePosition.Y, fullShaderProgram);

            // Show hover text
            DrawShipHoverInfo();
        }

        // Setup a mini window to show details of the current hover room or context menu icon
        private IEnumerable<string>? GetShipHoverText() {
            if (gpSelect.Active) {
                int ID = gpSelect.HoverID;
                if (ID >= 0) {
                    if (ID < StaticData.ShipEquipment.Count) {
                        ShipEquipment se = StaticData.ShipEquipment[ID];
                        return se.GetHoverText(PlayerTeam.PlayerShip);
                    }
                    else {
                        if (ID == (int)I_Build) return new List<string>() { "Build" };
                        if (ID == (int)I_Cancel) return new List<string>() { "Cancel" };
                        if (ID == (int)I_Salvage) return new List<string>() { "Salvage" };
                        if (ID == (int)I_Timer) return new List<string>() { "Sleep" };
                        if (ID == (int)I_Disconnect) return new List<string>() { "Deactivate" };
                        if (ID == (int)I_Connect) return new List<string>() { "Activate" };
                    }
                    return null;
                }
            }

            if (bHoverHull || irHover > -1) {
                ShipEquipment? se = bHoverHull ? PlayerTeam.PlayerShip.ArmourType : PlayerTeam.PlayerShip.GetEquipmentByRoomID(irHover);
                if (se == null) {
                    if (bHoverHull) return new List<string>() { "<No Armour>" };
                    else return new List<string>() { "<Empty>" };
                }

                if (se != null) {
                    if (bHoverHull || PlayerTeam.PlayerShip.GetIsRoomActive(irHover)) {
                        return se.GetHoverText();
                    }
                    else return new List<string>() { se.Name + " (Deactivated)" };
                }
                return null;
            }
            return null;
        }

        // Draw the hover info when hovering over a room
        private void DrawShipHoverInfo() {
            if (irHover == -1 && !bHoverHull && (!gpSelect.Active || gpSelect.HoverItem == null)) return; // No label to show

            float xx = (float)MousePosition.X / (float)Size.X;
            float yy = (float)MousePosition.Y / (float)Size.Y;

            IEnumerable<string>? txtList = GetShipHoverText();
            if (txtList is null || !txtList.Any()) return;
            string longest = string.Empty;
            // Approximate longest string on display is string with most chars. Might not be, but whatever.
            foreach (string str in txtList) {
                if (str.Length > longest.Length) longest = str;
            }

            float tx = xx > 0.5 ? xx - 0.02f : xx + 0.02f;
            float ty = yy > 0.5 ? yy - 0.02f : yy + 0.02f;
            float px = tx, py = ty;
            float ph = 0.03f, phb = ph * 0.2f;
            TextMeasure tm = TextRenderer.MeasureText(longest);
            float pw = ph * tm.Width / (Aspect * TextRenderer.FontSize);

            // Draw the hover text
            Alignment al = Alignment.BottomLeft;
            if (xx > 0.5) {
                px -= pw;
                if (yy > 0.5) al = Alignment.BottomRight;
                else al = Alignment.TopRight;
            }
            else {
                if (yy > 0.5) al = Alignment.BottomLeft;
                else al = Alignment.TopLeft;
            }
            if (yy > 0.5) py -= (ph*txtList.Count());

            fullShaderProgram.SetUniform("textureEnabled", false);
            fullShaderProgram.SetUniform("lightEnabled", false);
            fullShaderProgram.SetUniform("view", Matrix4.Identity);
            Matrix4 pTranslateM = Matrix4.CreateTranslation(px, py, 0f);
            Matrix4 pScaleM = Matrix4.CreateScale(pw, (ph * txtList.Count()) + (phb * 2f), 1f);
            fullShaderProgram.SetUniform("model", pScaleM * pTranslateM);
            fullShaderProgram.SetUniform("flatColour", new Vector4(0.3f, 0.3f, 0.3f, 1f));
            GL.UseProgram(fullShaderProgram.ShaderProgramHandle);
            Square.Flat.BindAndDraw();

            TextRenderOptions tro = new TextRenderOptions() {
                Alignment = al,
                Aspect = Aspect,
                TextColour = Color.LightGray,
                XPos = tx,
                YPos = ty,
                ZPos = 0.015f,
                Scale = ph
            };
            TextRenderer.DrawWithOptions(txtList, tro);
        }

        // Setup mouse-hover context menu
        private void SetupContextMenu_Ship() {
            irContextRoom = -1;
            bContextHull = false;
            gpSelect.Reset();
            gpSelect.SetPosition((float)MousePosition.X / (float)Size.X + 0.02f, (float)MousePosition.Y / (float)Size.Y + 0.01f);
            if (irHover != -1 || bHoverHull) {
                irContextRoom = irHover;
                bContextHull = bHoverHull;
                ShipEquipment? seHover = bHoverHull ? PlayerTeam.PlayerShip.ArmourType : PlayerTeam.PlayerShip.GetEquipmentByRoomID(irContextRoom);
                if (seHover == null) {
                    if (PlayerTeam.CurrentPositionHAO?.BaseSize > 0) {
                        GUIPanel? buildingPanel = GenerateBuildingList();
                        TexSpecs ts = Textures.GetTexCoords(Textures.MiscTexture.Build);
                        gpSelect.InsertIconItem(I_Build, ts, true, buildingPanel);
                    }
                }
                else {
                    if (PlayerTeam.CurrentPositionHAO?.BaseSize > 0) {
                        TexSpecs ts = Textures.GetTexCoords(Textures.MiscTexture.Salvage);
                        gpSelect.InsertIconItem(I_Salvage, ts, true, null);
                    }
                    if (!bHoverHull && PlayerTeam.PlayerShip.GetCanDeactivateRoom(irContextRoom)) {
                        TexSpecs ts = Textures.GetTexCoords(Textures.MiscTexture.Disconnect);
                        gpSelect.InsertIconItem(I_Disconnect, ts, true, null);
                    }
                    else if (!bHoverHull && !PlayerTeam.PlayerShip.GetIsRoomActive(irContextRoom)) {
                        TexSpecs ts = Textures.GetTexCoords(Textures.MiscTexture.Connect);
                        gpSelect.InsertIconItem(I_Connect, ts, true, null);
                    }
                }
                gpSelect.Activate();
            }
        }

        // Generate a building icon list for the sub-panel when building a new room
        private GUIPanel? GenerateBuildingList() {
            if (!bHoverHull && (irContextRoom < 0 || irContextRoom > PlayerTeam.PlayerShip.Type.Rooms.Count)) return null;
            ShipRoomDesign? rd = bHoverHull ? null : PlayerTeam.PlayerShip.Type.Rooms[irContextRoom];
            ShipEquipment.RoomSize roomSize = rd?.Size ?? ShipEquipment.RoomSize.Armour;
            int count = 0;
            GUIPanel gp = new GUIPanel(this);
            List<int> lIDs = Enumerable.Range(0, StaticData.ShipEquipment.Count).ToList<int>();
            lIDs.Sort((a, b) => StaticData.ShipEquipment[a].Cost.CompareTo(StaticData.ShipEquipment[b].Cost));
            foreach (int eno in lIDs) {
                ShipEquipment se = StaticData.ShipEquipment[eno];
                // Can we build this at the current location?
                if (PlayerTeam.CurrentPositionHAO is null) continue; // No base
                if ((se.Available & PlayerTeam.CurrentPositionHAO!.Base) == 0) continue; // Not the correct facilities
                if (se.RequiredRace != null && PlayerTeam.CurrentPosition.GetSystem().Owner != se.RequiredRace) continue; // Not the correct race
                if (se.RequiredRace != null && PlayerTeam.GetRelations(se.RequiredRace) < Const.RaceRelationsLevelToAllowSpecialisedEquipmentSale) continue; // Correct race, but player team is not >= friendly

                // Is it the right size?
                if (se.Size != roomSize) continue;

                // Can we afford it?
                bool bAfford = true;
                if (PlayerTeam.PlayerShip.CostToBuildEquipment(se) > PlayerTeam.Cash) bAfford = false;

                // Insert this icon & add to the tally
                TexSpecs ts = Textures.GetTexCoords(se);
                gp.InsertIconItem((uint)eno, ts, bAfford, null);
                count++;
            }
            if (count == 0) {
                TexSpecs ts = Textures.GetTexCoords(Textures.MiscTexture.None);
                gp.InsertIconItem(I_None, ts, false, null);
            }
            return gp;
        }

        // Condition bars
        private void DrawHullCondition() {
            // Show the hull condition
            if (PlayerTeam == null) return;
            float fract = PlayerTeam.PlayerShip.HullFract;
            float barWidth = 0.2f, barHeight = 0.03f;
            fullShaderProgram.SetUniform("view", Matrix4.Identity);
            Matrix4 pTranslateM = Matrix4.CreateTranslation(0.79f, 0.09f, 0f);
            Matrix4 pScaleM = Matrix4.CreateScale(barWidth, barHeight, 1f);
            fullShaderProgram.SetUniform("model", pScaleM * pTranslateM);

            // Display fractional progress bar
            pos2DCol4ShaderProgram.SetUniform("view", Matrix4.Identity);
            pos2DCol4ShaderProgram.SetUniform("model", pScaleM * pTranslateM);
            GL.UseProgram(pos2DCol4ShaderProgram.ShaderProgramHandle);
            Spectrum.Flat.BindAndDraw();

            // Black out the unused bit
            if (fract < 1.0f) {
                Matrix4 pbTranslateM = Matrix4.CreateTranslation(0.79f + (fract * barWidth), 0.09f, 0f);
                fullShaderProgram.SetUniform("model", pScaleM * pbTranslateM);
                fullShaderProgram.SetUniform("flatColour", new Vector4(0f, 0f, 0f, 1f));
                GL.UseProgram(fullShaderProgram.ShaderProgramHandle);
                Square.Flat.BindAndDraw();
            }

            // Now draw the frame
            fullShaderProgram.SetUniform("model", pScaleM * pTranslateM);
            fullShaderProgram.SetUniform("flatColour", new Vector4(1f, 1f, 1f, 1f));
            GL.UseProgram(fullShaderProgram.ShaderProgramHandle);
            Square.Lines.BindAndDraw();

            // Show the label
            TextRenderOptions tro = new TextRenderOptions() {
                Alignment = Alignment.TopRight,
                Aspect = Aspect,
                TextColour = Color.White,
                XPos = 0.78f,
                YPos = 0.09f,
                ZPos = 0.015f,
                Scale = 0.02f
            };
            TextRenderer.DrawWithOptions("Hull", tro);

            // Show the value
            string strHull = PlayerTeam.PlayerShip.Hull.ToString("0.#") + " / " + PlayerTeam.PlayerShip.Type.MaxHull.ToString("0.#");
            tro.Alignment = Alignment.TopMiddle;
            tro.XPos = 0.89f;
            tro.YPos = 0.09f;
            TextRenderer.DrawWithOptions(strHull, tro);
        }
        private void DrawPowerBar() {
            if (PlayerTeam == null) return;
            // Show the power bar
            fullShaderProgram.SetUniform("textureEnabled", false);
            fullShaderProgram.SetUniform("lightEnabled", false);
            fullShaderProgram.SetUniform("view", Matrix4.Identity);
            Matrix4 pTranslateM = Matrix4.CreateTranslation(0.79f, 0.15f, 0f);
            int pg = PlayerTeam.PlayerShip.PowerGeneration;
            int pc = PlayerTeam.PlayerShip.PowerConsumption;
            float barWidth = 0.2f, barHeight = 0.03f;
            Matrix4 pScaleM = Matrix4.CreateScale(barWidth, barHeight, 1f);

            if (pg > 0) {
                float fract = (float)pc / (float)pg;
                if (fract < 1.0) {
                    Matrix4 pfScaleM = Matrix4.CreateScale(barWidth * fract, barHeight, 1f);
                    fullShaderProgram.SetUniform("model", pfScaleM * pTranslateM);
                    fullShaderProgram.SetUniform("flatColour", new Vector4(0f, 1f, 0f, 1f));
                    GL.UseProgram(fullShaderProgram.ShaderProgramHandle);
                    Square.Flat.BindAndDraw();
                }
                else {
                    Matrix4 pfScaleM = Matrix4.CreateScale(barWidth * (1f/fract), barHeight, 1f);
                    fullShaderProgram.SetUniform("model", pfScaleM * pTranslateM);
                    fullShaderProgram.SetUniform("flatColour", new Vector4(0f, 1f, 0f, 1f));
                    GL.UseProgram(fullShaderProgram.ShaderProgramHandle);
                    Square.Flat.BindAndDraw();
                    Matrix4 p2TranslateM = Matrix4.CreateTranslation(0.79f + (0.2f/fract), 0.12f, 0f);
                    pfScaleM = Matrix4.CreateScale(barWidth * (1f - (1f / fract)), barHeight, 1f);
                    fullShaderProgram.SetUniform("model", pfScaleM * p2TranslateM);
                    fullShaderProgram.SetUniform("flatColour", new Vector4(1f, 0f, 0f, 1f));
                    GL.UseProgram(fullShaderProgram.ShaderProgramHandle);
                    Square.Flat.BindAndDraw();
                }
            }
            else {
                fullShaderProgram.SetUniform("model", pScaleM * pTranslateM);
                fullShaderProgram.SetUniform("flatColour", new Vector4(1f, 0f, 0f, 1f));
                GL.UseProgram(fullShaderProgram.ShaderProgramHandle);
                Square.Flat.BindAndDraw();
            }

            // Now draw the frame
            fullShaderProgram.SetUniform("model", pScaleM * pTranslateM);
            fullShaderProgram.SetUniform("flatColour", new Vector4(1f, 1f, 1f, 1f));
            GL.UseProgram(fullShaderProgram.ShaderProgramHandle);
            Square.Lines.BindAndDraw();

            // Show the label
            TextRenderOptions tro = new TextRenderOptions() {
                Alignment = Alignment.TopRight,
                Aspect = Aspect,
                TextColour = Color.White,
                XPos = 0.78f,
                YPos = 0.15f,
                ZPos = 0.015f,
                Scale = 0.02f
            };
            TextRenderer.DrawWithOptions("Power", tro);

            // Show the value
            string strPower = "Using " + pc + " / " + pg;
            tro.Alignment = Alignment.TopMiddle;
            tro.XPos = 0.89f;
            tro.YPos = 0.15f;
            TextRenderer.DrawWithOptions(strPower, tro);
        }

        // Display the text labels required for the GUI
        private void DisplaySelectionText_Ship() {
            float dTLScale = 0.03f;
            float dXMargin = 0.01f;
            float dYStart = 0.82f;
            float dYGap = 0.04f;

            // Get the text strings
            if (irSelected == -1) return;

            string tl1 = "", tl2 = "", tl3 = "", tl4 = "";

            ShipEquipment? se = PlayerTeam.PlayerShip.GetEquipmentByRoomID(irSelected);
            if (se == null) {
                tl1 = "<Empty>";
            }
            else {
                tl1 = se.Name;

                if (se is ShipArmour arm) {
                    tl2 = "Armour : " + arm.BaseArmour + "%";
                    if (se.Defence > 0) tl3 = "Defence Bonus : " + se.Defence;
                    if (arm.Repair > 0) tl4 = "Heal Rate : " + arm.Repair;
                }
                else if (se is ShipEngine eng) {
                    if (eng.Range >= Const.LightYear) tl2 = "Range : " + Math.Round(((ShipEngine)se).Range / Const.LightYear, 1) + "ly";
                    else tl2 = "Range : System";
                    tl3 = "Speed : " + Math.Round(eng.Speed / Const.SpeedOfLight, 1) + "c";
                    tl4 = "Accel : " + Math.Round(eng.Accel / 10.0, 1) + "g"; // Yeah I know g =~9.8, but whatever
                }
                else if (se is ShipEquipment) {
                    if (se.Generate > 0) tl2 = "Generate : " + se.Generate;
                    else tl2 = "Power : " + se.Power;
                    if (se.Attack > 0) tl3 = "Attack Bonus : " + se.Attack;
                    else if (se.Defence > 0) tl3 = "Defence Bonus : " + se.Defence;
                    if (se.Capacity > 0) tl4 = "Support : " + se.Capacity;
                    else if (se.Medlab) tl4 = "Medbay";
                    else if (se.Armoury) tl4 = "Armoury";
                    else if (se.Workshop) tl4 = "Workshop";
                }
                else if (se is ShipWeapon) {
                    tl2 = "Power : " + se.Power;
                    tl3 = "Attack Bonus : " + se.Attack;
                }
                else {
                    tl2 = "Unknown Room Type : " + se.GetType();
                }
            }
            // Display the text details of the selected object
            if (!string.IsNullOrEmpty(tl1)) TextRenderer.DrawAt(tl1, Alignment.TopLeft, dTLScale, Aspect, dXMargin, dYStart);
            if (!string.IsNullOrEmpty(tl2)) TextRenderer.DrawAt(tl2, Alignment.TopLeft, dTLScale, Aspect, dXMargin, dYStart + dYGap);
            if (!string.IsNullOrEmpty(tl3)) TextRenderer.DrawAt(tl3, Alignment.TopLeft, dTLScale, Aspect, dXMargin, dYStart + dYGap * 2f);
            if (!string.IsNullOrEmpty(tl4)) TextRenderer.DrawAt(tl4, Alignment.TopLeft, dTLScale, Aspect, dXMargin, dYStart + dYGap * 3f);
        }

        // Ship function buttons
        public void RepairShip() {
            double cost = PlayerTeam.PlayerShip.CalculateRepairCost();
            if (cost == 0.0) return;
            cost = Math.Round(cost, 2);
            msgBox.PopupConfirmation("Repair would cost " + cost + " credits. Proceed?", () => RepairShip_Continue(cost));
        }
        private void RepairShip_Continue(double c) {
            double cost = (double)c;
            PlayerTeam.Cash -= cost;
            PlayerTeam.PlayerShip.RepairHull();
            gbRepair.Deactivate();
        }
        public void FabricateItems() {
            FabricateItems fi = new FabricateItems(PlayerTeam);
            fi.ShowDialog();
        }
    }
}
