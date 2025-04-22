using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using SpaceMercs.Graphics.Shapes;
using Keys = OpenTK.Windowing.GraphicsLibraryFramework.Keys;

namespace SpaceMercs.MainWindow {
    // Partial class including functions for drawing the full galaxymap view
    partial class MapView {
        private Star? SystemStar = null;
        private bool bShowColonies = true;
        private Vector4 SelectionAnnulusColour = new Vector4(0.8f, 0.8f, 0.8f, 1f);

        // Root call for displaying the system when zoomed in
        private void DrawSystem() {
            // Make sure we haven't still got the star selected
            if (aoSelected is Star) aoSelected = null;
            if (aoHover is Star) aoHover = null;

            // Set the correct view location & perspective matrices for each shader program
            Matrix4 projectionM = Matrix4.CreateOrthographicOffCenter(0.0f, 1.0f, 1.0f, 0.0f, -1.0f, 1.0f);
            fullShaderProgram.SetUniform("projection", projectionM);
            fullShaderProgram.SetUniform("view", Matrix4.Identity);
            fullShaderProgram.SetUniform("model", Matrix4.Identity);
            flatColourShaderProgram.SetUniform("projection", projectionM);
            flatColourShaderProgram.SetUniform("view", Matrix4.Identity);

            fullShaderProgram.SetUniform("ambient", 0.25f);
            fullShaderProgram.SetUniform("lightCol", new Vector3(1f, 1f, 1f));
            fullShaderProgram.SetUniform("flatColour", new Vector4(1f, 1f, 1f, 1f));
            fullShaderProgram.SetUniform("texPos", 0f, 0f);
            fullShaderProgram.SetUniform("texScale", 1f, 1f);

            GL.ClearColor(Color.Black);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            // Display the scene
            DrawAstronomicalObjects(Aspect, PlayerTeam.CurrentPosition, bShowLabels, bShowColonies, Clock.ElapsedSeconds());
            DrawSystemText();
            SetupGUIHoverInfo();
            DrawGUI();
        }

        // Draw all objects in this system
        private void DrawAstronomicalObjects(float aspect, AstronomicalObject aoCurrentPosition, bool bShowLabels, bool bShowColonies, double elapsedSeconds) {
            fullShaderProgram.SetUniform("textureEnabled", false);
            fullShaderProgram.SetUniform("lightEnabled", true);
            Matrix4 squashM = Matrix4.CreateScale(1f / aspect, 1f, 1f);
            Matrix4 translateM = Matrix4.CreateTranslation(0.9f + (SystemStar!.DrawScale * Const.StarScale / aspect), 0.5f, 0f);
            fullShaderProgram.SetUniform("view", squashM * translateM);
            fullShaderProgram.SetUniform("lightPos", -100000f, 0f, 10000f);

            // Draw the star
            SystemStar.DrawSelected(fullShaderProgram, 12, elapsedSeconds);

            // Draw system
            aoHover = null;
            float px = 0.8f;
            float py = 0.25f;
            float mx = (float)MousePosition.X / (float)Size.X;
            float my = (float)MousePosition.Y / (float)Size.Y;
            fullShaderProgram.SetUniform("lightPos", 100000f, 10000f, 10000f);
            foreach (Planet pl in SystemStar.Planets) {
                float scale = pl.DrawScale * Const.PlanetScale;
                Matrix4 pTranslateM = Matrix4.CreateTranslation(px, py, 0f);
                fullShaderProgram.SetUniform("view", squashM * pTranslateM);
                flatColourShaderProgram.SetUniform("view", squashM * pTranslateM);

                // Draw the Planet
                int Level = pl.Radius > 7 * Const.Million ? 10 : 9;
                pl.DrawSelected(fullShaderProgram, Level, elapsedSeconds);

                // Draw all other Planet icons
                pl.DrawHalo(fullShaderProgram);

                if (bShowLabels && !string.IsNullOrEmpty(pl.Name)) {
                    float dskip = (pl.Colony != null) ? 0.06f : 0.01f;
                    float yskip = (scale * 1.06f) + 0.01f;
                    TextRenderOptions tro = new TextRenderOptions() {
                        Alignment = Alignment.TopMiddle,
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
                    flatColourShaderProgram.SetUniform("flatColour", SelectionAnnulusColour);
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
                if (aoCurrentPosition == pl) {
                    Matrix4 pScaleM = Matrix4.CreateScale(scale * 1.4f);
                    flatColourShaderProgram.SetUniform("model", pScaleM);
                    flatColourShaderProgram.SetUniform("flatColour", new Vector4(1f, 1f, 1f, 1f));
                    GL.UseProgram(flatColourShaderProgram.ShaderProgramHandle);
                    TriangleFocus.Flat.BindAndDraw();
                }

                if (bShowColonies && pl.Colony is not null) {
                    pl.DrawBaseIcon(flatColourShaderProgram);
                    TextRenderOptions tro = new TextRenderOptions() {
                        Alignment = Alignment.CentreMiddle,
                        Aspect = aspect,
                        TextColour = Color.White,
                        XPos = px,
                        YPos = py - (Const.PlanetScale * 0.12f),
                        ZPos = 0.0f,
                        Scale = Const.PlanetScale
                    };
                    GL.Disable(EnableCap.DepthTest);
                    TextRenderer.DrawWithOptions(pl.Colony.BaseSize.ToString(), tro);
                    GL.Enable(EnableCap.DepthTest);                   
                }

                // Draw the moons
                float moony = py + 0.15f;
                foreach (Moon mn in pl.Moons) {
                    scale = mn.DrawScale * Const.PlanetScale * Const.MoonScale;
                    Matrix4 mTranslateM = Matrix4.CreateTranslation(px, moony, 0f);
                    fullShaderProgram.SetUniform("view", squashM * mTranslateM);
                    flatColourShaderProgram.SetUniform("view", squashM * mTranslateM);

                    mn.DrawSelected(fullShaderProgram, 7, elapsedSeconds);

                    dist2 = ((px - mx) * (px - mx) * (aspect * aspect)) + (moony - my) * (moony - my);
                    if (dist2 <= (pl.DrawScale * Const.PlanetScale * Const.MoonScale) * (pl.DrawScale * Const.PlanetScale * Const.MoonScale)) {
                        aoHover = mn;
                        Matrix4 pScaleM = Matrix4.CreateScale(scale * 1.2f);
                        flatColourShaderProgram.SetUniform("model", pScaleM);
                        flatColourShaderProgram.SetUniform("flatColour", SelectionAnnulusColour);
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

                    if (bShowColonies && mn.Colony is not null) {
                        mn.DrawBaseIcon(flatColourShaderProgram);
                        TextRenderOptions tro = new TextRenderOptions() {
                            Alignment = Alignment.CentreMiddle,
                            Aspect = aspect,
                            TextColour = Color.White,
                            XPos = px,
                            YPos = moony - (Const.PlanetScale * 0.12f),
                            ZPos = 0.0f,
                            Scale = Const.PlanetScale * 0.8f
                        };
                        GL.Disable(EnableCap.DepthTest);
                        TextRenderer.DrawWithOptions(mn.Colony.BaseSize.ToString(), tro);
                        GL.Enable(EnableCap.DepthTest);
                    }

                    moony += 0.07f;
                }

                px -= (pl.DrawScale * Const.PlanetScale + 0.05f) * 0.8f;
            }

            // Draw the HyperGate, if there is one
            if (SystemStar.HasHyperGate) {
                HyperGate hg = SystemStar.GetHyperGate()!;
                float scale = Const.PlanetScale;
                Matrix4 pTranslateM = Matrix4.CreateTranslation(px, py, 0f);
                fullShaderProgram.SetUniform("view", squashM * pTranslateM);
                flatColourShaderProgram.SetUniform("view", squashM * pTranslateM);
                HyperGate.DrawHyperGate(fullShaderProgram);
                float dist2 = ((px - mx) * (px - mx) * (aspect * aspect)) + (py - my) * (py - my);
                if (dist2 <= scale * scale) {
                    aoHover = hg;
                    Matrix4 pScaleM = Matrix4.CreateScale(scale * 1.2f);
                    flatColourShaderProgram.SetUniform("model", pScaleM);
                    flatColourShaderProgram.SetUniform("flatColour", SelectionAnnulusColour);
                    GL.UseProgram(flatColourShaderProgram.ShaderProgramHandle);
                    Annulus.Annulus32.BindAndDraw();
                }
                if (aoSelected == hg) {
                    Matrix4 pScaleM = Matrix4.CreateScale(scale * 1.1f);
                    flatColourShaderProgram.SetUniform("model", pScaleM);
                    flatColourShaderProgram.SetUniform("flatColour", new Vector4(0.2f, 1f, 0.4f, 1f));
                    GL.UseProgram(flatColourShaderProgram.ShaderProgramHandle);
                    Annulus.Annulus32.BindAndDraw();
                }
                if (aoCurrentPosition == hg) {
                    Matrix4 pScaleM = Matrix4.CreateScale(scale * 1.4f);
                    flatColourShaderProgram.SetUniform("model", pScaleM);
                    flatColourShaderProgram.SetUniform("flatColour", new Vector4(1f, 1f, 1f, 1f));
                    GL.UseProgram(flatColourShaderProgram.ShaderProgramHandle);
                    TriangleFocus.Flat.BindAndDraw();
                }
                px -= (Const.PlanetScale + 0.05f) * 0.8f;
            }

            // Draw the SpaceHulk, if there is one
            if (SystemStar.SpaceHulk is not null) {
                SpaceHulk sh = SystemStar.SpaceHulk;
                float scale = Const.PlanetScale * sh.DrawScale;
                Matrix4 pTranslateM = Matrix4.CreateTranslation(px, py, 0f);
                fullShaderProgram.SetUniform("view", squashM * pTranslateM);
                flatColourShaderProgram.SetUniform("view", squashM * pTranslateM);
                sh.DrawSelected(fullShaderProgram, 0, elapsedSeconds);
                float dist2 = ((px - mx) * (px - mx) * (aspect * aspect)) + (py - my) * (py - my);
                if (dist2 <= scale * scale) {
                    aoHover = sh;
                    Matrix4 pScaleM = Matrix4.CreateScale(scale * 1.2f);
                    flatColourShaderProgram.SetUniform("model", pScaleM);
                    flatColourShaderProgram.SetUniform("flatColour", SelectionAnnulusColour);
                    GL.UseProgram(flatColourShaderProgram.ShaderProgramHandle);
                    Annulus.Annulus32.BindAndDraw();
                }
                if (aoSelected == sh) {
                    Matrix4 pScaleM = Matrix4.CreateScale(scale * 1.1f);
                    flatColourShaderProgram.SetUniform("model", pScaleM);
                    flatColourShaderProgram.SetUniform("flatColour", new Vector4(0.2f, 1f, 0.4f, 1f));
                    GL.UseProgram(flatColourShaderProgram.ShaderProgramHandle);
                    Annulus.Annulus32.BindAndDraw();
                }
                if (aoCurrentPosition == sh) {
                    Matrix4 pScaleM = Matrix4.CreateScale(scale * 1.4f);
                    flatColourShaderProgram.SetUniform("model", pScaleM);
                    flatColourShaderProgram.SetUniform("flatColour", new Vector4(1f, 1f, 1f, 1f));
                    GL.UseProgram(flatColourShaderProgram.ShaderProgramHandle);
                    TriangleFocus.Flat.BindAndDraw();
                }
            }
        }

        // Draw the system name and affiliation on the system view
        private void DrawSystemText() {
            string strSystem = "Unnamed Star";
            if (!string.IsNullOrEmpty(SystemStar?.Name)) strSystem = SystemStar.Name;
            string strOwner = "No Owner";
            if (SystemStar?.Owner is not null) {
                if (SystemStar.Owner.Known) {
                    strOwner = SystemStar.Owner.Name;
                }
                else strOwner = "Unknown Alien Race";
            }
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

        // Keyboard handler specific for this view
        private void GetKeyboardInput_SystemView() {
            if (IsKeyPressed(Keys.C)) {  // Toggle on/off colony icons
                if (bShowColonies) { bShowColonies = false; }
                else { bShowColonies = true; }
            }
        }

        // Double clcik on colony to view
        private void DoubleClick_System() {
            if (aoHover != null) aoSelected = aoHover;
            if (aoSelected != PlayerTeam.CurrentPosition) return;
            if (aoSelected is HabitableAO hao && hao.Colony is not null) {
                OpenColonyViewDialog();
            }
        }
    }
}
