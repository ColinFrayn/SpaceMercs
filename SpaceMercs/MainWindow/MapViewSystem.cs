using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using SpaceMercs.Graphics;
using SpaceMercs.Graphics.Shapes;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Tab;
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
            flatColourShaderProgram.SetUniform("projection", projectionM);
            flatColourShaderProgram.SetUniform("view", Matrix4.Identity);

            fullShaderProgram.SetUniform("lightPos", 100000f, 100000f, 10000f);
            fullShaderProgram.SetUniform("ambient", 0.25f);
            fullShaderProgram.SetUniform("lightCol", new Vector3(1f, 1f, 1f));
            fullShaderProgram.SetUniform("flatColour", new Vector4(1f, 1f, 1f, 1f));
            fullShaderProgram.SetUniform("texPos", 0f, 0f);
            fullShaderProgram.SetUniform("texScale", 1f, 1f);

            GL.ClearColor(Color.Black);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            // Display the scene
            DrawAstronomicalObjects(Aspect, PlayerTeam!.CurrentPosition, bShowLabels, bShowColonies);
            DrawSystemText();
            SetupGUIHoverInfo();
            DrawGUI();
        }

        private void DrawAstronomicalObjects(float aspect, AstronomicalObject aoCurrentPosition, bool bShowLabels, bool bShowColonies) {
            fullShaderProgram.SetUniform("textureEnabled", false);
            fullShaderProgram.SetUniform("lightEnabled", true);
            Matrix4 squashM = Matrix4.CreateScale(1f / aspect, 1f, 1f);
            Matrix4 translateM = Matrix4.CreateTranslation(0.8f + (SystemStar.DrawScale * Const.StarScale), 0.5f, 0f);
            fullShaderProgram.SetUniform("view", squashM * translateM);

            // Draw the star
            SystemStar.DrawSelected(fullShaderProgram, 12);

            // Draw system
            aoHover = null;
            float px = 0.8f;
            float py = 0.25f;
            float mx = (float)MousePosition.X / (float)Size.X;
            float my = (float)MousePosition.Y / (float)Size.Y;
            bool bOdd = true;
            foreach (Planet pl in SystemStar.Planets) {
                float scale = pl.DrawScale * Const.PlanetScale;
                Matrix4 pTranslateM = Matrix4.CreateTranslation(px, py, 0f);
                fullShaderProgram.SetUniform("view", squashM * pTranslateM);
                flatColourShaderProgram.SetUniform("view", squashM * pTranslateM);

                // Draw the Planet
                int Level = pl.radius > 7 * Const.Million ? 10 : 9;
                pl.DrawSelected(fullShaderProgram, Level);

                // Draw all other Planet icons
                pl.DrawHalo(fullShaderProgram);

                if (bShowLabels) {
                    float dskip = (pl.Colony != null) ? 0.06f : 0.01f;
                    float yskip = bOdd ? (scale * 1.06f) + 0.01f : -(scale * 1.06f) - dskip;
                    TextRenderOptions tro = new TextRenderOptions() {
                        Alignment = bOdd ? Alignment.TopMiddle : Alignment.BottomMiddle,
                        Aspect = aspect,
                        TextColour = Color.White,
                        XPos = px,
                        YPos = py + yskip,
                        ZPos = 0.015f,
                        Scale = Const.PlanetScale
                    };
                    TextRenderer.DrawWithOptions(pl.Name, tro);
                }
                float dist2 = ((px - mx) * (px - mx) * (aspect * aspect)) + (py - my) * (py - my);
                if (dist2 <= scale*scale) {
                    aoHover = pl;
                    Matrix4 pScaleM = Matrix4.CreateScale(scale * 1.2f);
                    flatColourShaderProgram.SetUniform("model", pScaleM);
                    flatColourShaderProgram.SetUniform("flatColour", new Vector4(1f, 1f, 1f, 1f));
                    GL.UseProgram(flatColourShaderProgram.ShaderProgramHandle);
                    Annulus.Annulus32.BindAndDraw();
                }
                if (aoSelected == pl) {
                    Matrix4 pScaleM = Matrix4.CreateScale(scale * 1.1f);
                    flatColourShaderProgram.SetUniform("model", pScaleM);
                    flatColourShaderProgram.SetUniform("flatColour", new Vector4(0.2f, 1f, 0.4f, 1f));
                    GL.UseProgram(flatColourShaderProgram.ShaderProgramHandle);
                    Annulus.Annulus32.BindAndDraw();
                }
                //if (bShowColonies) pl.DrawBaseIcon();  // TODO
                if (aoCurrentPosition == pl) {
                    Matrix4 pScaleM = Matrix4.CreateScale(scale * 1.4f);
                    flatColourShaderProgram.SetUniform("model", pScaleM);
                    flatColourShaderProgram.SetUniform("flatColour", new Vector4(1f, 1f, 1f, 1f));
                    GL.UseProgram(flatColourShaderProgram.ShaderProgramHandle);
                    TriangleFocus.Flat.BindAndDraw();
                }

                // Draw the moons
                float moony = py + 0.15f;
                foreach (Moon mn in pl.Moons) {
                    scale = mn.DrawScale * Const.PlanetScale * Const.MoonScale;
                    Matrix4 mTranslateM = Matrix4.CreateTranslation(px, moony, 0f);
                    fullShaderProgram.SetUniform("view", squashM * mTranslateM);
                    flatColourShaderProgram.SetUniform("view", squashM * mTranslateM);

                    //if (bShowColonies) mn.DrawBaseIcon();  // TODO
                    mn.DrawSelected(fullShaderProgram, 7);

                    dist2 = ((px - mx) * (px - mx) * (aspect * aspect)) + (moony - my) * (moony - my);
                    if (dist2 <= (pl.DrawScale * Const.PlanetScale * Const.MoonScale) * (pl.DrawScale * Const.PlanetScale * Const.MoonScale)) {
                        aoHover = mn;
                        Matrix4 pScaleM = Matrix4.CreateScale(scale * 1.2f);
                        flatColourShaderProgram.SetUniform("model", pScaleM);
                        flatColourShaderProgram.SetUniform("flatColour", new Vector4(1f, 1f, 1f, 1f));
                        GL.UseProgram(flatColourShaderProgram.ShaderProgramHandle);
                        Annulus.Annulus16.BindAndDraw();
                    }
                    if (aoSelected == mn) {
                        Matrix4 pScaleM = Matrix4.CreateScale(scale * 1.2f);
                        flatColourShaderProgram.SetUniform("model", pScaleM);
                        flatColourShaderProgram.SetUniform("flatColour", new Vector4(0.2f, 1f, 0.4f, 1f));
                        GL.UseProgram(flatColourShaderProgram.ShaderProgramHandle);
                        Annulus.Annulus16.BindAndDraw();
                    }
                    if (aoCurrentPosition == mn) {
                        Matrix4 pScaleM = Matrix4.CreateScale(scale * 1.5f);
                        flatColourShaderProgram.SetUniform("model", pScaleM);
                        flatColourShaderProgram.SetUniform("flatColour", new Vector4(1f, 1f, 1f, 1f));
                        GL.UseProgram(flatColourShaderProgram.ShaderProgramHandle);
                        TriangleFocus.Flat.BindAndDraw();
                    }
                    moony += 0.07f;
                }

                px -= (pl.DrawScale * Const.PlanetScale + 0.1f) * 0.6f;
            }
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
        }

    }
}
