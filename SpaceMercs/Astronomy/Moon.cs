using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using SpaceMercs.Graphics;
using SpaceMercs.Graphics.Shapes;
using System.IO;
using System.Xml;

namespace SpaceMercs {
    class Moon : HabitableAO {
        public Planet Parent { get; set; }
        public override float DrawScale { get { return (float)Math.Sqrt(radius / 1000.0) / 50f; } }

        public Moon(int _seed) {
            Seed = _seed;
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
            Type = (Planet.PlanetType)Enum.Parse(typeof(Planet.PlanetType), xml.SelectSingleNode("Type").InnerText);
            XmlNode xmlc = xml.SelectSingleNode("Colony");
            if (xmlc != null) SetColony(new Colony(xmlc, this));
            colour = Const.PlanetTypeToCol2(Type);
            LoadMissions(xml);
        }

        // Save this moon to an Xml file
        public void SaveToFile(StreamWriter file) {
            file.WriteLine("<Moon ID=\"" + ID.ToString() + "\">");
            WriteAODetailsToFile(file);
            if (Colony != null) Colony.SaveToFile(file);
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
            prog.SetUniform("lightEnabled", true);
            prog.SetUniform("textureEnabled", true);
            prog.SetUniform("texPos", 0f, 0f);
            prog.SetUniform("texScale", 1f, 1f);

            float scale = DrawScale * Const.PlanetScale * Const.MoonScale;
            Matrix4 pScaleM = Matrix4.CreateScale(scale);
            Matrix4 pRotateM = Matrix4.CreateRotationY((float)Const.ElapsedSeconds * 2f * (float)Math.PI / (float)AxialRotationPeriod);
            Matrix4 modelM = pRotateM * pScaleM;
            prog.SetUniform("model", modelM);

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

    }
}
