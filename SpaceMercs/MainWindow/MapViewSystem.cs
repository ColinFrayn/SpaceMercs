using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using SpaceMercs.Graphics.Shapes;
using Keys = OpenTK.Windowing.GraphicsLibraryFramework.Keys;

namespace SpaceMercs.MainWindow {
  // Partial class including functions for drawing the full galaxymap view
  partial class MapView {
    private Star SystemStar = null;
    private bool bShowColonies = true;

    // Root call for displaying the system when zoomed in
    private void DrawSystem() {
      // Make sure we haven't still got the star selected
      if (aoSelected != null && aoSelected.AOType == AstronomicalObject.AstronomicalObjectType.Star) aoSelected = null;
      if (aoHover != null && aoHover.AOType == AstronomicalObject.AstronomicalObjectType.Star) aoHover = null;

      // Set the correct view location & perspective matrix
      GL.MatrixMode(MatrixMode.Projection);
      GL.LoadMatrix(ref ortho_projection); //GL.LoadMatrix(ref perspective);
      GL.ClearColor(Color.Black);
      GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
      GL.MatrixMode(MatrixMode.Modelview);
      GL.LoadIdentity();
      GL.Scale(0.1 / Aspect, 0.1, 0.1);

      // Display the scene
      SystemStar.DrawSystem(Context, Aspect, aoSelected, aoHover, PlayerTeam.CurrentPosition, bShowLabels, bShowColonies);
      DrawSystemText();
      SetupGUIHoverInfo();
      DrawGUI();
    }

    // Get the system under the mouse pointer
    private void SystemHover() {
      AstronomicalObject aoHoverOld = aoHover;
      aoHover = null;
      if (GalaxyMap.bMapSetup == false) return;

      // Set aoHover
      double mousex = ((double)mx / (double)Size.X) * 10.0 * Aspect;
      double mousey = ((double)my / (double)Size.Y) * 10.0;
      aoHover = SystemStar.GetHover(Aspect, mousex, mousey);
      // TODO if (aoHover != aoHoverOld) glMapView.Invalidate();
    }

    // Configure and locate the light from the star
    private void SetupSystemLighting() {
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

    // Draw the system name and affiliation on the system view
    private void DrawSystemText() {
      string strSystem = "Unnamed Star";
      if (!string.IsNullOrEmpty(SystemStar?.Name)) strSystem = SystemStar.Name;
      string strOwner = SystemStar?.Owner?.Name ?? "No Owner";
      GL.PushMatrix();
      GL.Disable(EnableCap.Lighting);
      GL.Color3(1.0, 1.0, 1.0);
      GL.Translate(5.0 * Aspect, 0.05, 0.0);
      GL.PushMatrix();
      GL.Scale(0.6, 0.6, 0.6);
      GL.Rotate(180.0, Vector3d.UnitX);
      TextRenderer.Draw(strSystem, Alignment.TopMiddle);
      GL.PopMatrix();
      GL.PushMatrix();
      GL.Translate(0.0, 0.5, 0.0);
      GL.Scale(0.45,0.45,0.45);
      if (SystemStar.Owner == null) GL.Color3(0.5, 0.5, 0.5);
      else GL.Color3(SystemStar.Owner.Colour);
      GL.Rotate(180.0, Vector3d.UnitX);
      TextRenderer.Draw(strOwner, Alignment.TopMiddle);
      GL.PopMatrix();
      GL.PopMatrix();
    }

    private void GetKeyboardInput_SystemView() {
      if (IsKeyPressed(Keys.C)) {  // Toggle on/off colony icons
        if (bShowColonies) { bShowColonies = false; }
        else { bShowColonies = true; }
      }
      SystemHover();
    }

  }
}
