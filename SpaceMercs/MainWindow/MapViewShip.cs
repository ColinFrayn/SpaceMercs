using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;
using SpaceMercs.Dialogs;
using Keys = OpenTK.Windowing.GraphicsLibraryFramework.Keys;

namespace SpaceMercs.MainWindow {
  // Partial class including functions for viewing the team ship
  partial class MapView {
    private float fShipViewX, fShipViewY, fShipViewZ = Const.MinimumShipViewZ;
    private int irHover = -1, irContextRoom = -1, irSelected = -1;
    private IPanelItem piHoverItem;
    private GUIButton gbRepair, gbFabricate;
    private TextLabel tlHull, tlPower, tlHullValue, tlPowerValue;
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
      if (gbFabricate == null) gbFabricate = new GUIButton("Fabricate", this, FabricateItems);
      if (tlCash == null) {
        tlCash = new TextLabel(PlayerTeam.Cash.ToString("F2") + " credits");
        tlCash.TextColour = Color.White;
      }
      if (tlHull == null) tlHull = new TextLabel("Hull");
      if (tlPower == null) tlPower = new TextLabel("Power");
      if (tlHullValue == null) tlHullValue = new TextLabel();
      if (tlPowerValue == null) tlPowerValue = new TextLabel();
      // TODO shipToolStripMenuItem.Enabled = false;
      SetupUtilityButtons();
    }

    private void SetupUtilityButtons() {
      // "Repair Ship" button
      gbRepair.SetPosition(0.89, 0.85);
      gbRepair.SetSize(0.08, 0.03);
      gbRepair.SetBlend(false);
      if (PlayerTeam.PlayerShip.HullFract < 1.0) gbRepair.Activate();
      else gbRepair.Deactivate();
      if (PlayerTeam.CurrentPosition.PriceModifier > 1.0) gbRepair.Deactivate(); // Not Neutral or better with the owner race
      if ((PlayerTeam.CurrentPosition.Base & (Colony.BaseType.Colony | Colony.BaseType.Metropolis | Colony.BaseType.Military)) == 0) gbRepair.Deactivate(); // Only colonies, metropolis or military bases can repair

      // Fabrication Button
      gbFabricate.SetPosition(0.89, 0.9);
      gbFabricate.SetSize(0.08, 0.03);
      gbFabricate.SetBlend(false);
      gbFabricate.Deactivate();
      if (PlayerTeam.PlayerShip.HasArmoury) gbFabricate.Activate();
      else if (PlayerTeam.PlayerShip.HasWorkshop) gbFabricate.Activate();
      else if (PlayerTeam.PlayerShip.HasMedlab) gbFabricate.Activate();
    }
    private void DrawShip() {
      if (!bLoaded) return;

      // Set up default OpenGL rendering parameters
      GL.DepthMask(false);
      GL.Disable(EnableCap.Lighting);
      GL.Disable(EnableCap.Texture2D);
      GL.Disable(EnableCap.DepthTest);

      // Set the correct view location & perspective matrix
      GL.MatrixMode(MatrixMode.Projection);
      GL.LoadMatrix(ref perspective);
      GL.MatrixMode(MatrixMode.Modelview);
      GL.LoadIdentity();

      // Draw the starfield (unmoving)
      if (iStarfieldDL == -1) SetupStarfield();
      GL.CallList(iStarfieldDL);

      // Shift to the correct location
      GL.Translate(-fShipViewX, -fShipViewY, -fShipViewZ);

      // Draw the ship
      PlayerTeam.PlayerShip.DrawSchematic(irHover, iBuildingTexture, bHoverHull);

      // Draw any static GUI elements
      ShowShipGUI();
    }

    private void GetKeyboardInput_Ship() {
      if (bAIRunning) return;
      if (IsKeyPressed(Keys.Escape)) {
        view = PreviousViewMode;
        // TODO shipToolStripMenuItem.Enabled = true;
        // TODO glMapView.Invalidate();
      }
    }

    // Mouse stuff
    private void MouseMove_Ship(MouseMoveEventArgs e) {
      if (MouseState.IsButtonDown(MouseButton.Left)) {
        float fScale = Const.MouseMoveScale * fShipViewZ / 50.0f;
        if (IsKeyDown(Keys.LeftControl) || IsKeyDown(Keys.RightControl)) fScale *= 2.5f;
        else if (IsKeyDown(Keys.LeftShift) || IsKeyDown(Keys.RightShift)) fScale /= 2.5f;
        fShipViewX -= (float)(e.X - mx) * fScale;
        if (fShipViewX < -PlayerTeam.PlayerShip.Length / 1.9f) fShipViewX = -PlayerTeam.PlayerShip.Length / 1.9f;
        if (fShipViewX > PlayerTeam.PlayerShip.Length / 1.9f) fShipViewX = PlayerTeam.PlayerShip.Length / 1.9f;
        fShipViewY += (float)(e.Y - my) * fScale;
        if (fShipViewY < -PlayerTeam.PlayerShip.Width / 1.9f) fShipViewY = -PlayerTeam.PlayerShip.Width / 1.9f;
        if (fShipViewY > PlayerTeam.PlayerShip.Width / 1.9f) fShipViewY = PlayerTeam.PlayerShip.Width / 1.9f;
        // TODO glMapView.Invalidate();
      }
      bool bRep = gbRepair.IsHover(mx, my);
      bool bFab = gbFabricate.IsHover(mx, my);
      mx = (int)e.X;
      my = (int)e.Y;
      int rOldHover = irHover;
      IPanelItem piOldItem = piHoverItem;
      CheckHover_Ship();
      // TODO glMapView.Invalidate();
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
        if (gbRepair.CaptureClick(mx, my)) return;
        if (gbFabricate.CaptureClick(mx, my)) return;
      }
      CheckHover_Ship();
      SetupUtilityButtons();
    }
    private void MouseDown_Ship(MouseButtonEventArgs e) {
      mx = (int)MousePosition.X;
      my = (int)MousePosition.Y;
      if (e.Button == MouseButton.Right) {
        SetupContextMenu_Ship();
      }
      if (e.Button == MouseButton.Left) {
        SetSelection(irHover);
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
      double mxfract = (double)mx / (double)Size.X;
      double myfract = (double)my / (double)Size.Y;
      double mxpos = ((mxfract - 0.5) * (fShipViewZ / 1.86) * Aspect) + fShipViewX;
      double mypos = ((0.5 - myfract) * (fShipViewZ / 1.86)) + fShipViewY;
      irHover = PlayerTeam.PlayerShip.CheckHoverRoom(mxpos, mypos, out bHoverHull);
    }

    // Show static GUI elements
    private void ShowShipGUI() {
      GL.MatrixMode(MatrixMode.Projection);
      GL.LoadMatrix(ref ortho_projection);
      GL.MatrixMode(MatrixMode.Modelview);
      GL.LoadIdentity();
      GL.Disable(EnableCap.Lighting);
      GL.Clear(ClearBufferMask.DepthBufferBit);

      // Show the context menu, if it's available
      gpSelect.Display(mx, my);

      // Show hover text
      SetupRoomHoverInfo();
      DrawRoomHoverInfo();

      // Show the cash
      GL.PushMatrix();
      GL.Translate(0.01, 0.0, 0.1);
      GL.Color3(1.0, 1.0, 1.0);
      GL.Scale(0.04 / Aspect, 0.04, 0.04);
      GL.Rotate(180.0, Vector3d.UnitX);
      tlCash.UpdateText(PlayerTeam.Cash.ToString("F2") + " credits");
      //tlCash.Draw(TextLabel.Alignment.TopLeft);
      GL.PopMatrix();

      DrawHullCondition();
      DrawPowerBar();
      if (irSelected != -1) DisplaySelectionText_Ship();

      // Display all buttons
      gbRepair.Display(mx, my);
      gbFabricate.Display(mx, my);
    }

    // Setup a mini window to show details of the current hover room or context menu icon
    private void SetupRoomHoverInfo() {
      if (tlHover == null) {
        tlHover = new TextLabel("...");
        tlHover.Border = 1;
        tlHover.BorderColour = Color.LightGray;
        tlHover.BackgroundColour = Color.Black;
      }
      if (gpSelect.Active) {
        int ID = gpSelect.HoverID;
        if (ID >= 0) {
          if (ID < StaticData.ShipEquipment.Count) {
            ShipEquipment se = StaticData.ShipEquipment[ID];
            tlHover.UpdateTextFromList(se.GetHoverText(PlayerTeam.PlayerShip));
          }
          else {
            if (ID == (int)I_Build) tlHover.UpdateText("Build");
            if (ID == (int)I_Cancel) tlHover.UpdateText("Cancel");
            if (ID == (int)I_Salvage) tlHover.UpdateText("Salvage");
            if (ID == (int)I_Timer) tlHover.UpdateText("Sleep");
            if (ID == (int)I_Disconnect) tlHover.UpdateText("Deactivate");
            if (ID == (int)I_Connect) tlHover.UpdateText("Activate");
          }
          return;
        }
      }

      if (bHoverHull || irHover > -1) {
        ShipEquipment se = bHoverHull ? PlayerTeam.PlayerShip.ArmourType : PlayerTeam.PlayerShip.GetEquipmentByRoomID(irHover);
        if (se == null) {
          if (bHoverHull) tlHover.UpdateText("<No Armour>");
          else tlHover.UpdateText("<Empty>");
          return;
        }

        if (se != null) {
          if (bHoverHull || PlayerTeam.PlayerShip.GetIsRoomActive(irHover)) {
            tlHover.UpdateTextFromList(se.GetHoverText());
          }
          else tlHover.UpdateText(se.Name + " (Deactivated)");
        }
        return;
      }
    }

    // Draw the hover info when hovering over a room
    private void DrawRoomHoverInfo() {
      if (irHover == -1 && !bHoverHull && (!gpSelect.Active || gpSelect.HoverItem == null)) return; // No label to show
      // Draw the hover text
      double xx = (double)mx / (double)Size.X;
      double yy = (double)my / (double)Size.Y;
      double thHeight = 0.04 * tlHover.Lines;
      double thWidth = thHeight * (double)tlHover.Width / (double)tlHover.Height;
      double xSep = 0.01, ySep = 0.01;
      if (xx > 0.5) {
        thWidth = -thWidth;
        xSep = -0.01;
      }
      if (yy < 0.5) {
        thHeight = -thHeight;
        ySep = -0.01;
      }

      // Draw the hover text
      GL.Color3(0.7, 0.7, 0.7);
      GL.PushMatrix();
      if (thWidth < 0.0) GL.Translate(xx + thWidth + xSep, 0.0, 0.3);
      else GL.Translate(xx + xSep, 0.0, 0.3);
      if (thHeight > 0.0) GL.Translate(0.0, yy - ySep, 0.0);
      else GL.Translate(0.0, yy - thHeight - ySep, 0.0);
      GL.Rotate(180.0, Vector3d.UnitX);
      //tlHover.Draw(TextLabel.Alignment.BottomLeft, Math.Abs(thWidth), Math.Abs(thHeight));
      GL.PopMatrix();
    }

    // Setup mouse-hover context menu
    private void SetupContextMenu_Ship() {
      irContextRoom = -1;
      bContextHull = false;
      gpSelect.Reset();
      gpSelect.PanelX = (double)mx / (double)Size.X + 0.02;
      gpSelect.PanelY = (double)my / (double)Size.Y + 0.01;
      if (irHover != -1 || bHoverHull) {
        irContextRoom = irHover;
        bContextHull = bHoverHull;
        ShipEquipment seHover = bHoverHull ? PlayerTeam.PlayerShip.ArmourType : PlayerTeam.PlayerShip.GetEquipmentByRoomID(irContextRoom);
        if (seHover == null) {
          if (PlayerTeam.CurrentPosition.BaseSize > 0) {
            GUIPanel buildingPanel = GenerateBuildingList();
            Tuple<double, double> tp = Textures.GetTexCoords(Textures.MiscTexture.Build);
            gpSelect.InsertIcon(I_Build, iMiscTexture, tp.Item1, tp.Item2, Textures.MiscTextureWidth, Textures.MiscTextureHeight, true, buildingPanel);
          }
        }
        else {
          if (PlayerTeam.CurrentPosition.BaseSize > 0) {
            Tuple<double, double> tp = Textures.GetTexCoords(Textures.MiscTexture.Salvage);
            gpSelect.InsertIcon(I_Salvage, iMiscTexture, tp.Item1, tp.Item2, Textures.MiscTextureWidth, Textures.MiscTextureHeight, true, null);
          }
          if (!bHoverHull && PlayerTeam.PlayerShip.GetCanDeactivateRoom(irContextRoom)) {
            Tuple<double, double> tp = Textures.GetTexCoords(Textures.MiscTexture.Disconnect);
            gpSelect.InsertIcon(I_Disconnect, iMiscTexture, tp.Item1, tp.Item2, Textures.MiscTextureWidth, Textures.MiscTextureHeight, true, null);
          }
          else if (!bHoverHull && !PlayerTeam.PlayerShip.GetIsRoomActive(irContextRoom)) {
            Tuple<double, double> tp = Textures.GetTexCoords(Textures.MiscTexture.Connect);
            gpSelect.InsertIcon(I_Connect, iMiscTexture, tp.Item1, tp.Item2, Textures.MiscTextureWidth, Textures.MiscTextureHeight, true, null);
          }
        }
        gpSelect.Activate();
      }
      // TODO glMapView.Invalidate();
    }

    // Generate a building icon list for the sub-panel when building a new room
    private GUIPanel GenerateBuildingList() {
      if (!bHoverHull && (irContextRoom < 0 || irContextRoom > PlayerTeam.PlayerShip.Type.Rooms.Count)) return null;
      ShipRoomDesign rd = bHoverHull ? null : PlayerTeam.PlayerShip.Type.Rooms[irContextRoom];
      ShipEquipment.RoomSize roomSize = bHoverHull ? ShipEquipment.RoomSize.Armour : rd.Size;
      int count = 0;
      GUIPanel gp = new GUIPanel(this);
      List<int> lIDs = Enumerable.Range(0, StaticData.ShipEquipment.Count).ToList<int>();
      lIDs.Sort((a, b) => StaticData.ShipEquipment[a].Cost.CompareTo(StaticData.ShipEquipment[b].Cost));
      foreach (int eno in lIDs) {
        ShipEquipment se = StaticData.ShipEquipment[eno];
        // Can we build this at the current location?
        if ((se.Available & PlayerTeam.CurrentPosition.Base) == 0) continue;
        if (se.RequiredRace != null && PlayerTeam.CurrentPosition.GetSystem().Owner != se.RequiredRace) continue; // Not the correct race
        if (se.RequiredRace != null && PlayerTeam.CurrentPosition.PriceModifier >= 1.0) continue; // Correct race, but player team is not at least friendly

        // Is it the right size?
        if (se.Size != roomSize) continue;

        // Can we afford it?
        bool bAfford = true;
        if (PlayerTeam.PlayerShip.CostToBuildEquipment(se) > PlayerTeam.Cash) bAfford = false;

        // Insert this icon & add to the tally
        Tuple<double, double> tp = Textures.GetTexCoords(se);
        gp.InsertIcon((uint)eno, iBuildingTexture, tp.Item1, tp.Item2, Textures.BuildingTextureWidth, Textures.BuildingTextureHeight, bAfford, null);
        count++;
      }
      if (count == 0) {
        Tuple<double, double> tp = Textures.GetTexCoords(Textures.MiscTexture.None);
        gp.InsertIcon(I_None, iMiscTexture, tp.Item1, tp.Item2, Textures.MiscTextureWidth, Textures.MiscTextureHeight, false, null);
      }
      return gp;
    }

    // Condition bars
    private void DrawHullCondition() {
      // Show the hull condition
      GL.Disable(EnableCap.Texture2D);
      GL.Enable(EnableCap.Blend);
      double dFract = PlayerTeam.PlayerShip.HullFract;
      GL.PushMatrix();
      GL.Translate(0.79, 0.01, 0.1);
      GL.Scale(0.2, 0.2, 1.0);
      GL.Begin(BeginMode.Quads);
      GL.Color3(0.6, 0.0, 0.0);
      GL.Vertex3(0.0, 0.0, 0.0);
      GL.Vertex3(0.0, 0.1, 0.0);
      GL.Color3(1.0, 0.5, 0.0);
      GL.Vertex3(Math.Min(dFract,0.3), 0.1, 0.0);
      GL.Vertex3(Math.Min(dFract, 0.3), 0.0, 0.0);
      //
      if (dFract > 0.3) {
        GL.Color3(1.0, 0.5, 0.0);
        GL.Vertex3(0.3, 0.0, 0.0);
        GL.Vertex3(0.3, 0.1, 0.0);
        GL.Color3(1.0, 1.0, 0.0);
        GL.Vertex3(Math.Min(dFract, 0.5), 0.1, 0.0);
        GL.Vertex3(Math.Min(dFract, 0.5), 0.0, 0.0);
      }
      //
      if (dFract > 0.5) {
        GL.Color3(1.0, 1.0, 0.0);
        GL.Vertex3(0.5, 0.0, 0.0);
        GL.Vertex3(0.5, 0.1, 0.0);
        GL.Color3(0.0, 1.0, 0.0);
        GL.Vertex3(Math.Min(dFract, 0.8), 0.1, 0.0);
        GL.Vertex3(Math.Min(dFract, 0.8), 0.0, 0.0);
      }
      //
      if (dFract > 0.8) {
        GL.Color3(0.0, 1.0, 0.0);
        GL.Vertex3(0.8, 0.0, 0.0);
        GL.Vertex3(0.8, 0.1, 0.0);
        GL.Color3(0.0, 0.0, 0.7);
        GL.Vertex3(Math.Min(dFract, 1.0), 0.1, 0.0);
        GL.Vertex3(Math.Min(dFract, 1.0), 0.0, 0.0);
      }
      GL.End();

      // Now draw the frame
      GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line);
      GL.Color3(1.0, 1.0, 1.0);
      GL.Begin(BeginMode.Quads);
      GL.Vertex3(0.0, 0.0, 0.1);
      GL.Vertex3(1.0, 0.0, 0.1);
      GL.Vertex3(1.0, 0.1, 0.1);
      GL.Vertex3(0.0, 0.1, 0.1);
      GL.End();
      GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);
      GL.PopMatrix();

      // Show the label
      GL.PushMatrix();
      GL.Translate(0.78, 0.0, 0.1);
      GL.Color3(1.0, 1.0, 1.0);
      GL.Scale(0.04 / Aspect, 0.04, 0.04);
      GL.Rotate(180.0, Vector3d.UnitX);
      //tlHull.Draw(TextLabel.Alignment.TopRight);
      GL.PopMatrix();

      // Show the value
      string strHull = PlayerTeam.PlayerShip.Hull.ToString("0.#") + " / " + PlayerTeam.PlayerShip.Type.MaxHull.ToString("0.#");
      tlHullValue.UpdateText(strHull);
      GL.PushMatrix();
      GL.Translate(0.89, 0.03, 0.1);
      GL.Color3(1.0, 1.0, 1.0);
      GL.Scale(0.03 / Aspect, 0.03, 0.03);
      GL.Rotate(180.0, Vector3d.UnitX);
      //tlHullValue.Draw(TextLabel.Alignment.TopMiddle);
      GL.PopMatrix();
    }
    private void DrawPowerBar() {
      // Show the power bar
      GL.Disable(EnableCap.Texture2D);
      double dFract = 0.0;
      GL.PushMatrix();
      GL.Translate(0.79, 0.12, 0.1);
      GL.Scale(0.2, 0.2, 1.0);
      int pg = PlayerTeam.PlayerShip.PowerGeneration;
      int pc = PlayerTeam.PlayerShip.PowerConsumption;
      if (pg > 0) {
        dFract = (double)pc / (double)pg;
        if (dFract < 1.0) {
          GL.Color3(0.0, 1.0, 0.0);
          GL.Begin(BeginMode.Quads);
          GL.Vertex3(0.0, 0.0, 0.0);
          GL.Vertex3(dFract, 0.0, 0.0);
          GL.Vertex3(dFract, 0.1, 0.0);
          GL.Vertex3(0.0, 0.1, 0.0);
          GL.End();
        }
        else {
          GL.Color3(0.0, 1.0, 0.0);
          GL.Begin(BeginMode.Quads);
          GL.Vertex3(0.0, 0.0, 0.0);
          GL.Vertex3(1 / dFract, 0.0, 0.0);
          GL.Vertex3(1 / dFract, 0.1, 0.0);
          GL.Vertex3(0.0, 0.1, 0.0);
          GL.Color3(1.0, 0.0, 0.0);
          GL.Vertex3(1 / dFract, 0.0, 0.0);
          GL.Vertex3(1.0, 0.0, 0.0);
          GL.Vertex3(1.0, 0.1, 0.0);
          GL.Vertex3(1 / dFract, 0.1, 0.0);
          GL.End();
        }
      }
      else {
        GL.Color3(1.0, 0.0, 0.0);
        GL.Begin(BeginMode.Quads);
        GL.Vertex3(0.0, 0.0, 0.0);
        GL.Vertex3(1.0, 0.0, 0.0);
        GL.Vertex3(1.0, 0.1, 0.0);
        GL.Vertex3(0.0, 0.1, 0.0);
        GL.End();
      }

      // Now draw the frame
      GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line);
      GL.Color3(1.0, 1.0, 1.0);
      GL.Begin(BeginMode.Quads);
      GL.Vertex3(0.0, 0.0, 0.1);
      GL.Vertex3(1.0, 0.0, 0.1);
      GL.Vertex3(1.0, 0.1, 0.1);
      GL.Vertex3(0.0, 0.1, 0.1);
      GL.End();
      GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);
      GL.PopMatrix();

      // Show the label
      GL.PushMatrix();
      GL.Translate(0.78, 0.11, 0.1);
      GL.Color3(1.0, 1.0, 1.0);
      GL.Scale(0.04 / Aspect, 0.04, 0.04);
      GL.Rotate(180.0, Vector3d.UnitX);
      //tlPower.Draw(TextLabel.Alignment.TopRight);
      GL.PopMatrix();

      // Show the value
      string strPower = "Using " + pc + " / " + pg;
      tlPowerValue.UpdateText(strPower);
      GL.PushMatrix();
      GL.Translate(0.89, 0.14, 0.1);
      GL.Color3(1.0, 1.0, 1.0);
      GL.Scale(0.03 / Aspect, 0.03, 0.03);
      GL.Rotate(180.0, Vector3d.UnitX);
      //tlPowerValue.Draw(TextLabel.Alignment.TopMiddle);
      GL.PopMatrix();

    }

    // Display the text labels required for the GUI
    private void DisplaySelectionText_Ship() {
      double dTLScale = 0.035;
      double dXMargin = 0.01;
      double dYStart = 0.83;
      double dYGap = 0.0325;
      // Display the text details of the selected object
      GL.PushMatrix();
      GL.Translate(dXMargin, dYStart, 0.1);
      GL.Scale(dTLScale / Aspect, dTLScale, dTLScale);
      GL.Rotate(180.0, Vector3d.UnitX);
      //tlSel1.Draw(TextLabel.Alignment.CentreLeft);
      GL.PopMatrix();
      GL.PushMatrix();
      GL.Translate(dXMargin, dYStart + dYGap, 0.1);
      GL.Scale(dTLScale / Aspect, dTLScale, dTLScale);
      GL.Rotate(180.0, Vector3d.UnitX);
      //tlSel2.Draw(TextLabel.Alignment.CentreLeft);
      GL.PopMatrix();
      GL.PushMatrix();
      GL.Translate(dXMargin, dYStart + dYGap * 2.0, 0.1);
      GL.Scale(dTLScale / Aspect, dTLScale, dTLScale);
      GL.Rotate(180.0, Vector3d.UnitX);
      //tlSel3.Draw(TextLabel.Alignment.CentreLeft);
      GL.PopMatrix();
      GL.PushMatrix();
      GL.Translate(dXMargin, dYStart + dYGap * 3.0, 0.1);
      GL.Scale(dTLScale / Aspect, dTLScale, dTLScale);
      GL.Rotate(180.0, Vector3d.UnitX);
      //tlSel4.Draw(TextLabel.Alignment.CentreLeft);
      GL.PopMatrix();
    }

    // Ship function buttons
    public void RepairShip() {
      double cost = PlayerTeam.PlayerShip.CalculateRepairCost();
      if (cost == 0.0) return;
      cost = Math.Round(cost, 2);
      msgBox.PopupConfirmation("Repair would cost " + cost + " credits. Proceed?", RepairShip_Continue, cost);
    }
    private void RepairShip_Continue(object c) {
      double cost = (double)c;
      PlayerTeam.Cash -= cost;
      PlayerTeam.PlayerShip.RepairHull();
      gbRepair.Deactivate();
    }
    public void FabricateItems() {
      FabricateItems fi = new FabricateItems(PlayerTeam);
      fi.ShowDialog();
    }

    // Setup the details for the selected room
    private void SetSelection(int iRoomID) {
      ShipEquipment se = PlayerTeam.PlayerShip.GetEquipmentByRoomID(iRoomID);
      if (se == null) {
        tlSel1.UpdateText("<Empty>");
        tlSel2.bEnabled = false;
        tlSel3.bEnabled = false;
        tlSel4.bEnabled = false;
        return;
      }

      tlSel1.UpdateText(se.Name);
      if (se is ShipArmour) {
        tlSel2.UpdateText("Armour : " + ((ShipArmour)se).BaseArmour + "%");
        if (se.Defence > 0) tlSel3.UpdateText("Defence Bonus : " + se.Defence);
        else tlSel3.bEnabled = false;
        if (((ShipArmour)se).HealRate > 0) tlSel4.UpdateText("Heal Rate : " + ((ShipArmour)se).HealRate);
        else tlSel4.bEnabled = false;
      }
      else if (se is ShipEngine) {
        if (((ShipEngine)se).Range >= Const.LightYear) tlSel2.UpdateText("Range : " + Math.Round(((ShipEngine)se).Range / Const.LightYear, 1) + "ly");
        else tlSel2.UpdateText("Range : System");
        tlSel3.UpdateText("Speed : " + Math.Round(((ShipEngine)se).Speed / Const.SpeedOfLight, 1) + "c");
        tlSel4.UpdateText("Accel : " + Math.Round(((ShipEngine)se).Accel / 10.0, 1) + "g"); // Yeah I know g =~9.8, but whatever
      }
      else if (se is ShipEquipment) {
        if (se.Generate > 0) tlSel2.UpdateText("Generate : " + se.Generate);
        else tlSel2.UpdateText("Power : " + se.Power);
        if (se.Attack > 0) tlSel3.UpdateText("Attack Bonus : " + se.Attack);
        else if (se.Defence > 0) tlSel3.UpdateText("Defence Bonus : " + se.Defence);
        else tlSel3.bEnabled = false;
        string strDesc = "";
        if (se.Capacity > 0) strDesc = "Support : " + se.Capacity;
        else if (se.Medlab) strDesc += "Medbay";
        else if (se.Armoury) strDesc += "Armoury";
        else if (se.Workshop) strDesc += "Workshop";
        else tlSel4.bEnabled = false;
        if (!String.IsNullOrEmpty(strDesc)) tlSel4.UpdateText(strDesc);
      }
      else if (se is ShipWeapon) {
        tlSel2.UpdateText("Power : " + se.Power);
        tlSel3.UpdateText("Attack Bonus : " + se.Attack);
        tlSel4.bEnabled = false;
      }
      else {
        tlSel2.UpdateText("Unknown Room Type : " + se.GetType());
        tlSel3.bEnabled = false;
        tlSel4.bEnabled = false;
      }
    }

  }
}
