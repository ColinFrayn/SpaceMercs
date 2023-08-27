using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using SpaceMercs.Graphics;
using SpaceMercs.Graphics.Shapes;
using System.IO;
using System.Xml;

namespace SpaceMercs {
    public class Moon : HabitableAO {
        public Planet Parent { get; set; }
        public override float DrawScale { get { return (float)Math.Sqrt(Radius / 1000.0) / 50f; } }

        public Moon(int _seed, Planet parent, int id) {
            ID = id;
            Seed = _seed;
            Parent = parent;
            _MissionList = null;
            Random rnd = new Random(Seed);
            Ox = rnd.Next(Const.SeedBuffer);
            Oy = rnd.Next(Const.SeedBuffer);
            Oz = rnd.Next(Const.SeedBuffer);
        }
        public Moon(XmlNode xml, Planet parent) {
            Parent = parent;
            // Load this moon from the given Xml node
            // Start with generic AO stuff
            LoadAODetailsFromFile(xml);

            // Bugfix - handle some dodgy data saved down because moon orbital period was wrapping as it was miscalculated too large
            if (OrbitalPeriod < 0 || AxialRotationPeriod < 0) {
                Random rnd = new Random();
                OrbitalPeriod = Utils.NextGaussian(rnd, Const.MoonOrbitalPeriod, Const.MoonOrbitalPeriodSigma);
                OrbitalPeriod /= (OrbitalDistance / Const.MoonOrbit) * Math.Pow(Radius / Const.MoonRadius, 0.5);
                AxialRotationPeriod = Utils.NextGaussian(rnd, OrbitalPeriod, OrbitalPeriod / 15f);
            }

            Type = (Planet.PlanetType)Enum.Parse(typeof(Planet.PlanetType), xml.SelectNodeText("Type"));
            XmlNode? xmlc = xml.SelectSingleNode("Colony");
            if (xmlc != null) SetColony(new Colony(xmlc, this));
            colour = Const.PlanetTypeToCol2(Type);
            LoadMissions(xml);
        }

        // Save this moon to an Xml file
        public void SaveToFile(StreamWriter file) {
            file.WriteLine("<Moon ID=\"" + ID.ToString() + "\">");
            WriteAODetailsToFile(file);
            Colony?.SaveToFile(file);
            file.WriteLine("<Type>" + Type.ToString() + "</Type>");
            SaveMissions(file);
            file.WriteLine("</Moon>");
        }

        // Overrides
        public override AstronomicalObjectType AOType { get { return AstronomicalObjectType.Moon; } }
        public override void DrawBaseIcon(ShaderProgram prog) {
            if (Colony is null) return;
            float scale = Const.PlanetScale * Const.MoonScale * 1.5f;
            Colony.DrawBaseIcon(prog, scale);
        }
        public override void DrawSelected(ShaderProgram prog, int Level = 6) {
            float scale = DrawScale * Const.PlanetScale * Const.MoonScale;
            Matrix4 pScaleM = Matrix4.CreateScale(scale);
            Matrix4 pTurnM = Matrix4.CreateRotationY((float)Const.ElapsedSeconds * 2f * (float)Math.PI / (float)AxialRotationPeriod);
            Matrix4 pRotateM = Matrix4.CreateRotationX((float)Math.PI / 2f);
            Matrix4 modelM = pRotateM * pTurnM * pScaleM;
            prog.SetUniform("model", modelM);

            prog.SetUniform("lightEnabled", true);
            prog.SetUniform("textureEnabled", true);
            prog.SetUniform("texPos", 0f, 0f);
            prog.SetUniform("texScale", 1f, 1f);

            SetupTextureMap(32, 16);
            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, iTexture);
            GL.UseProgram(prog.ShaderProgramHandle);
            Sphere.CachedBuildAndDraw(Level, true);
            GL.BindTexture(TextureTarget.Texture2D, 0);
        }
        public override void SetupTextureMap(int width, int height) {
            if (iTexture == -1 || texture == null) iTexture = GL.GenTexture();
            else {
                if (texture.Length >= (width * height * 3)) return;
            }
            texture = Terrain.GenerateMap(this, width, height);
            GL.BindTexture(TextureTarget.Texture2D, iTexture);
            GL.TexEnv(TextureEnvTarget.TextureEnv, TextureEnvParameter.TextureEnvMode, (float)TextureEnvMode.Modulate);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgb, width, height, 0, PixelFormat.Rgb, PixelType.UnsignedByte, texture);
            Textures.SetParameters();
        }
        public override void ClearData() {
            GL.DeleteTexture(iTexture);
            texture = null;
            iTexture = -1;
        }
        public override void SetName(string str) {
            // Meh
        }
        public override Star GetSystem() {
            return Parent.Parent;
        }
        public override string PrintCoordinates() {
            return Parent.PrintCoordinates() + "." + ID;
        }
        public override int GetPopulation() {
            return Colony?.BaseSize ?? 0;
        }
    }
}
