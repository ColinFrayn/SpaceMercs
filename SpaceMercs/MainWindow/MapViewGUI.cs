using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using SpaceMercs.Dialogs;
using SpaceMercs.Graphics.Shapes;

namespace SpaceMercs.MainWindow {
  partial class MapView {
    private GUIButton gbRenameObject, gbFlyTo, gbViewColony, gbScan;
    //private TextRenderer tlSel1, tlSel2, tlSel3, tlSel4, tlSel5;
    //private TextRenderer tlF, tlG, tlL, tlR, tlA, tlV, tlC;
    //private TextRenderer tlHover, tlClock, tlCash, tlWelcome1, tlWelcome2;
    private readonly int iGUIHoverN = -1; // What is this??
    private static readonly double toggleY = 0.1, toggleX = 0.002, toggleStep = 0.03, toggleScale = 0.05;
    private static Matrix4 ortho_projection = Matrix4.CreateOrthographicOffCenter(0, 1, 1, 0, -1, 1);
    private AstronomicalObject lastAOHover = null;
    private bool bShowGUIHover = false;

    // Draw the GUI elements
    private void DrawGUI() {
      GL.MatrixMode(MatrixMode.Projection);
      GL.LoadMatrix(ref ortho_projection);
      GL.MatrixMode(MatrixMode.Modelview);
      GL.LoadIdentity();

      // Set up scene
      GL.Disable(EnableCap.Lighting);
      GL.DepthMask(true);
      GL.Clear(ClearBufferMask.DepthBufferBit);

      // Draw details of currently selected object
      if (aoSelected != null) {
        DisplaySelectionText();
      }

      // Display the current date and time
      GL.PushMatrix();
      GL.Translate(toggleX, 0.01, 0.1);
      GL.Color3(1.0, 1.0, 1.0);
      GL.Scale(0.04 / Aspect, 0.04, 0.04);
      GL.Rotate(180.0, Vector3d.UnitX);
      TextRenderer.Draw(Const.dtTime.ToString("F"), Alignment.TopLeft);
      GL.PopMatrix();

      // Draw stuff that's only visible when there's a game underway
      if (bLoaded && GalaxyMap.bMapSetup) {
        // Display the player's remaining cash reserves
        GL.PushMatrix();
        GL.Translate(0.99, 0.01, 0.1);
        GL.Color3(1.0, 1.0, 1.0);
        GL.Scale(0.04 / Aspect, 0.04, 0.04);
        GL.Rotate(180.0, Vector3d.UnitX);
        TextRenderer.Draw(PlayerTeam.Cash.ToString("F2") + " credits", Alignment.TopRight, Color.White);
        GL.PopMatrix();
        // Toggles
        DrawToggles();
        if (view == ViewMode.ViewMap) DrawMapToggles();
        if (view == ViewMode.ViewSystem) DrawSystemToggles();
        DrawGUIHoverInfo();
      }

      // If we're travelling then display that
      if (TravelDetails != null) {
        if (TravelDetails.GameOver) TravelDetails = null;
        else TravelDetails.Display(Context);
      }
      // Display the various GUI Buttons
      else {
        SetAOButtonsOnGUI(aoSelected);
        gbRenameObject.Display(mx, my);
        gbFlyTo.Display(mx, my);
        gbViewColony.Display(mx, my);
        gbScan.Display(mx, my);
      }

      // Colony?
      // TODO Enable
      //if (TravelDetails != null) colonyToolStripMenuItem.Enabled = false;
      //else {
      //  if (PlayerTeam.CurrentPosition.BaseSize > 0) colonyToolStripMenuItem.Enabled = true;
      //  else colonyToolStripMenuItem.Enabled = false;
      //}
    }

    // Display the text labels required for the GUI
    private void DisplaySelectionText() {
      double dTLScale = 0.035;
      double dXMargin = 0.01;
      double dYStart = 0.84;
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
      GL.PushMatrix();
      GL.Translate(dXMargin, dYStart + dYGap * 4.0, 0.1);
      GL.Scale(dTLScale / Aspect, dTLScale, dTLScale);
      GL.Rotate(180.0, Vector3d.UnitX);
      //tlSel5.Draw(TextLabel.Alignment.CentreLeft);
      GL.PopMatrix();
    }

    // Draw toggles for all screens (L)
    private void DrawToggles() {
      GL.PushMatrix();
      GL.Translate(toggleX, toggleY + toggleStep, 0.1);
      GL.Scale(toggleScale / Aspect, toggleScale, toggleScale);
      GL.Rotate(180.0, Vector3d.UnitX);
      TextRenderer.Draw("L", Alignment.CentreLeft, bShowLabels ? Color.White : Color.DimGray);
      GL.PopMatrix();
    }

    // Draw toggles for the System View (C)
    private void DrawSystemToggles() {
      GL.PushMatrix();
      GL.Translate(toggleX, toggleY + toggleStep * 2, 0.1);
      GL.Scale(toggleScale / Aspect, toggleScale, toggleScale);
      GL.Rotate(180.0, Vector3d.UnitX);
      TextRenderer.Draw("C", Alignment.CentreLeft, bShowColonies ? Color.White : Color.DimGray);
      GL.PopMatrix();
    }

    // Draw toggles for the map screen (RFGAV)
    private void DrawMapToggles() {
      GL.PushMatrix();
      GL.Translate(toggleX, toggleY + toggleStep * 2, 0.1);
      GL.Scale(toggleScale / Aspect, toggleScale, toggleScale);
      GL.Rotate(180.0, Vector3d.UnitX);
      TextRenderer.Draw("A", Alignment.CentreLeft, bShowTradeRoutes ? Color.White : Color.DimGray);
      GL.PopMatrix();
      GL.PushMatrix();
      GL.Translate(toggleX, toggleY + toggleStep * 3, 0.1);
      GL.Scale(toggleScale / Aspect, toggleScale, toggleScale);
      GL.Rotate(180.0, Vector3d.UnitX);
      TextRenderer.Draw("F", Alignment.CentreLeft, bShowFlags ? Color.White : Color.DimGray);
      GL.PopMatrix();
      GL.PushMatrix();
      GL.Translate(toggleX, toggleY + toggleStep * 4, 0.1);
      GL.Scale(toggleScale / Aspect, toggleScale, toggleScale);
      GL.Rotate(180.0, Vector3d.UnitX);
      TextRenderer.Draw("G", Alignment.CentreLeft, bShowGridlines ? Color.White : Color.DimGray);
      GL.PopMatrix();
      GL.PushMatrix();
      GL.Translate(toggleX, toggleY + toggleStep * 5, 0.1);
      GL.Scale(toggleScale / Aspect, toggleScale, toggleScale);
      GL.Rotate(180.0, Vector3d.UnitX);
      TextRenderer.Draw("R", Alignment.CentreLeft, bShowRangeCircles ? Color.White : Color.DimGray);
      GL.PopMatrix();
      GL.PushMatrix();
      GL.Translate(toggleX, toggleY + toggleStep * 6, 0.1);
      GL.Scale(toggleScale / Aspect, toggleScale, toggleScale);
      GL.Rotate(180.0, Vector3d.UnitX);
      TextRenderer.Draw("V", Alignment.CentreLeft, bFadeUnvisited ? Color.White : Color.DimGray);
      GL.PopMatrix();
    }

    // Setup a mini window to show details of the current hover target
    private void SetupGUIHoverInfo() {
      bShowGUIHover = false;
      if (Control.ModifierKeys != Keys.Alt) return; // Only display if Alt is held down
      if (aoHover == null && iGUIHoverN == -1) return;
      bShowGUIHover = true;
      List<string> strHoverText = new List<string>();

      // Check for AO hover.
      if (aoHover != null && aoHover != lastAOHover) {
        double dist = AstronomicalObject.CalculateDistance(aoSelected, aoHover);
        if (aoHover.AOType == AstronomicalObject.AstronomicalObjectType.Star) {
          Star stHover = (Star)aoHover;
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
        else if (aoHover.AOType == AstronomicalObject.AstronomicalObjectType.Planet) {
          Planet plHover = (Planet)aoHover;
          if (plHover.Name.Length == 0) {
            strHoverText.Add("<Unnamed>");
          }
          else {
            strHoverText.Add(plHover.Name);
          }
          strHoverText.Add(plHover.Type.ToString());
          if (aoSelected != null) {
            if (aoSelected.GetSystem() == aoHover.GetSystem()) {
              strHoverText.Add("Dist: " + Math.Round(dist / Const.Billion, 1).ToString() + " Gm");
            }
            else {
              strHoverText.Add("Dist: " + Math.Round(dist / Const.LightYear, 1).ToString() + " ly");
            }
          }
        }
        else if (aoHover.AOType == AstronomicalObject.AstronomicalObjectType.Moon) {
          Moon mnHover = (Moon)aoHover;
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
      }
      //if (strHoverText.Count > 0) {
      //  tlHover.UpdateTextFromList(strHoverText);
      //}
    }

    // Draw the hover info when hovering over an object with "Alt" pressed
    private void DrawGUIHoverInfo() {
      if (!bShowGUIHover) return;
      // Draw the mouse hover text
      double xx = (double)mx / (double)Size.X;
      double yy = (double)my / (double)Size.Y;
      double thHeight = 0.06;
      double thWidth = thHeight * 1.0; // (double)tlHover.Width / (double)tlHover.Height;
      double xSep = 0.01, ySep = 0.01;
      if (xx > 0.5) {
        thWidth = -thWidth;
        xSep = -0.01;
      }
      if (yy < 0.5) {
        thHeight = -thHeight;
        ySep = -0.01;
      }
      GL.Color3(0.7, 0.7, 0.7);
      //GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line);
      GL.Begin(BeginMode.LineLoop);
      GL.Vertex3(xx + xSep, yy - ySep, 0.21);
      GL.Vertex3(xx + xSep + thWidth, yy - ySep, 0.21);
      GL.Vertex3(xx + xSep + thWidth, yy - (ySep + thHeight), 0.21);
      GL.Vertex3(xx + xSep, yy - (ySep + thHeight), 0.21);
      GL.End();
      GL.Color3(0.0, 0.0, 0.0);
      GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);
      GL.Begin(BeginMode.Quads);
      GL.Vertex3(xx + xSep, yy - ySep, 0.2);
      GL.Vertex3(xx + xSep + thWidth, yy - ySep, 0.2);
      GL.Vertex3(xx + xSep + thWidth, yy - (ySep + thHeight), 0.2);
      GL.Vertex3(xx + xSep, yy - (ySep + thHeight), 0.2);
      GL.End();
      // Draw the hover text
      GL.Color3(0.7, 0.7, 0.7);
      GL.PushMatrix();
      if (thWidth < 0.0) GL.Translate(xx + thWidth + xSep, 0.0, 0.3);
      else GL.Translate(xx + xSep, 0.0, 0.3);
      if (thHeight > 0.0) GL.Translate(0.0, yy - ySep, 0.0);
      else GL.Translate(0.0, yy - thHeight - ySep, 0.0);
      GL.Rotate(180.0, Vector3d.UnitX);
      //tlHover.Draw(TextLabel.Alignment.TopLeft, Math.Abs(thWidth), Math.Abs(thHeight));
      GL.PopMatrix();
    }

    // Set up the various GUI elements of class GUIObject that need to be initialised
    private void SetupGUIElements() {
      // "View the colony at the current location" button
      gbViewColony = new GUIButton("Colony", this, OpenColonyViewDialog);
      gbViewColony.SetPosition(0.01, 0.07);
      gbViewColony.SetSize(0.065, 0.035);
      gbViewColony.SetBlend(false);

      // "Rename this planet" button
      gbRenameObject = new GUIButton("Rename", this, OpenRenameObjectDialog);
      gbRenameObject.SetPosition(0.08, 0.07);
      gbRenameObject.SetSize(0.065, 0.035);
      gbRenameObject.SetBlend(false);

      // "Fly to this target" button
      gbFlyTo = new GUIButton("Fly To", this, OpenFlyToDialog);
      gbFlyTo.SetPosition(0.15, 0.07);
      gbFlyTo.SetSize(0.065, 0.035);
      gbFlyTo.SetBlend(false);

      // "Scan this planet" button
      gbScan = new GUIButton("Scan", this, OpenScanPlanetDialog);
      gbScan.SetPosition(0.23, 0.07);
      gbScan.SetSize(0.065, 0.035);
      gbScan.SetBlend(false);
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
      msgBox.PopupConfirmation(String.Format("Really travel? Journey time = {0:%d}d {0:%h}h {0:%m}m {0:%s}s", ts), FlyTo_Continue, dJourneyTime);
    }
    private void FlyTo_Continue(object jt) {
      // Set up that we're travelling
      double dJourneyTime = (double)jt;
      TravelDetails = new Travel(PlayerTeam.CurrentPosition, aoSelected, dJourneyTime, PlayerTeam, this);
      // TODO glMapView.Invalidate();
      // TODO colonyToolStripMenuItem.Enabled = false;
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
      if (PlayerTeam.CurrentPosition == null) {
        msgBox.PopupMessage("Cannot scan current location\nScanner only works on terrestrial palnets and moons.");
        return;
      }
      if (PlayerTeam.CurrentPosition?.BaseSize > 0) {
        msgBox.PopupMessage("Please visit colony for missions from this location");
        return;
      }
      // Open the ScanPlanet dialog
      ScanPlanet sp = new ScanPlanet(PlayerTeam.CurrentPosition, PlayerTeam, RunMission);
      sp.ShowDialog();
      SetAOButtonsOnGUI(aoSelected);
    }

    // Set the button relevant for the selected AO
    public void SetAOButtonsOnGUI(AstronomicalObject ao) {
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
