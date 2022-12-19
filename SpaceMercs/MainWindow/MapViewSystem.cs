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
            if (aoSelected?.AOType == AstronomicalObject.AstronomicalObjectType.Star) aoSelected = null;
            if (aoHover?.AOType == AstronomicalObject.AstronomicalObjectType.Star) aoHover = null;

            // Set the correct view location & perspective matrices for each shader program
            Matrix4 projectionM = Matrix4.CreateOrthographicOffCenter(0.0f, 1.0f, 1.0f, 0.0f, -1.0f, 1.0f);
            fullShaderProgram.SetUniform("projection", projectionM);
            fullShaderProgram.SetUniform("view", Matrix4.Identity);
            fullShaderProgram.SetUniform("model", Matrix4.Identity);

            fullShaderProgram.SetUniform("lightPos", 100000f, 100000f, 10000f);
            fullShaderProgram.SetUniform("ambient", 0.25f);
            fullShaderProgram.SetUniform("lightCol", new Vector3(1f, 1f, 1f));
            fullShaderProgram.SetUniform("flatColour", new Vector4(1f, 1f, 1f, 1f));

            GL.ClearColor(Color.Black);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            // Display the scene
            SystemStar.DrawSystem(fullShaderProgram, Aspect, aoSelected, aoHover, PlayerTeam!.CurrentPosition, bShowLabels, bShowColonies);
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
            double mousex = ((double)MousePosition.X / (double)Size.X) * 10.0 * Aspect;
            double mousey = ((double)MousePosition.Y / (double)Size.Y) * 10.0;
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
            TextRenderOptions tro = new TextRenderOptions() {
                Alignment = Alignment.TopMiddle,
                Aspect = Aspect,
                TextColour = Color.White,
                XPos = 0.5f,
                YPos = 0.02f,
                ZPos = 0.01f,
                Scale = 0.035f
            };
            TextRenderer.DrawWithOptions(strSystem, tro);

            tro.YPos = 0.1f;
            tro.Scale = 0.025f;
            if (SystemStar?.Owner is null) tro.TextColour = Color.Gray;
            else tro.TextColour = SystemStar.Owner.Colour;
            TextRenderer.DrawWithOptions(strOwner, tro);
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
