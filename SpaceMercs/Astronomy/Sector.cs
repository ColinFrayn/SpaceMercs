using OpenTK.Compute.OpenCL;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using SpaceMercs.Graphics;
using SpaceMercs.Graphics.Shapes;
using System.IO;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;

namespace SpaceMercs {
    class Sector {
        private readonly List<Star> Stars = new List<Star>();
        public Race? Inhabitant = null;
        public int SectorX { get; private set; }
        public int SectorY { get; private set; }
        public Map ParentMap { get; private set; }
        VertexBuffer? tradeRoutesBuffer = null;
        VertexArray? tradeRoutesArray = null;

        public Sector() {
            ParentMap = Map.Empty;
        }
        public Sector(int sx, int sy, Map map) {
            SectorX = sx;
            SectorY = sy;
            ParentMap = map;
            int seed = map.MapSeed ^ ((sx * 85091) + (sy * 29527)) ^ ((sx * 34501) + (sy * 61819)); // Non-random seed; repeatable
            Random rand = new Random(seed);
            // Setup the stars in this sector
            float yoffset = (float)(((sy * 2) - 1) * Const.SectorSize) / 2.0f;
            float xoffset = (float)(((sx * 2) - 1) * Const.SectorSize) / 2.0f;
            for (int sno = 0; sno < map.StarsPerSector; sno++) {
                float X, Y;
                do {
                    // Create a star somewhere in the sector, but avoid the edges (so we don't end up overlapping with other stars in other sectors.)
                    // Note we can't really just check the neighbouring sectors, because then the details of this sector would depend on which other sectors were built first, which would make this unrepeatable.
                    X = (float)rand.NextDouble() * ((float)Const.SectorSize * 0.96f) + xoffset + ((float)Const.SectorSize * 0.02f);
                    Y = (float)rand.NextDouble() * ((float)Const.SectorSize * 0.96f) + yoffset + ((float)Const.SectorSize * 0.02f);
                } while (CheckProximity(X, Y));
                Star st = new Star(X, Y, rand.Next(10000000), this, sno);
                Stars.Add(st);
            }
            // Done
        }
        public Sector(XmlNode xml, Map map) {
            SectorX = xml.GetAttributeInt("X");
            SectorY = xml.GetAttributeInt("Y");
            ParentMap = map;
            Inhabitant = StaticData.GetRaceByName(xml.GetAttributeText("Inhabitant"));

            foreach (XmlNode xmls in xml.ChildNodes) {
                Star st = new Star(xmls, this);
                Stars.Add(st);
            }
        }
        public static Sector Empty { get { return new Sector(); } }

        // Save this sector to an Xml file
        public void SaveToFile(StreamWriter file) {
            file.WriteLine(" <Sector X=\"" + SectorX + "\" Y=\"" + SectorY + "\" Inhabitant=\"" + ((Inhabitant == null) ? "" : Inhabitant.Name) + "\">");
            foreach (Star st in Stars) {
                st.SaveToFile(file);
            }
            file.WriteLine(" </Sector>");
        }

        // Is this star too close to another? Note that we need to check neighbouring sectors, too...
        private bool CheckProximity(double x, double y) {
            double MD2 = Const.MinStarDistance * Const.MinStarDistance;
            foreach (Star st in Stars) {
                double dx = st.MapPos.X - x;
                double dy = st.MapPos.Y - y;
                double r2 = (dx * dx) + (dy * dy);
                if (r2 < MD2) return true;
            }
            return false;
        }

        public void Draw(ShaderProgram prog, bool bFadeUnvisited, bool bShowLabels, bool bShowFlags, bool bShowPop, float fMapViewX, float fMapViewY, float fMapViewZ, float aspect) {
            TextRenderOptions tro = new TextRenderOptions() {
                Alignment = Alignment.TopMiddle,
                Aspect = 1.0f,
                TextColour = Color.White,
                XPos = 0.0f,
                YPos = -0.5f,
                ZPos = 0.02f,
                Scale = 0.3f,
                FlipY = true,
                Projection = Matrix4.CreatePerspectiveFieldOfView(Const.MapViewportAngle, aspect, 0.05f, 5000.0f),
            };

            foreach (Star st in Stars) {
                // Translate into the star frame
                Matrix4 translateM = Matrix4.CreateTranslation(st.MapPos.X - fMapViewX, st.MapPos.Y - fMapViewY, -fMapViewZ);

                // Work out the degree of detail to show in this star
                int iLevel = st.GetDetailLevel(fMapViewX, fMapViewY, fMapViewZ);

                // If the star is close to the viewer and not faded then show the textured sphere
                if ((!bFadeUnvisited || st.Visited) && iLevel >= 4) {
                    Matrix4 scaleM = Matrix4.CreateScale(0.5f);
                    Matrix4 viewM = scaleM * translateM;
                    prog.SetUniform("view", viewM);
                    prog.SetUniform("flatColour", new Vector4(1f, 1f, 1f, 1f));
                    st.DrawSelected(prog, iLevel);
                }
                else {
                    // Fade out unvisited stars (if set to do so)
                    float fade = 1f;
                    if (bFadeUnvisited && !st.Visited) fade = 4f;
                    Vector4 col = new Vector4(st.colour.X / fade, st.colour.Y / fade, st.colour.Z / fade, 1.0f);

                    Matrix4 scaleM = Matrix4.CreateScale(st.DrawScale / 8f);
                    Matrix4 viewM = scaleM * translateM;
                    prog.SetUniform("view", viewM);
                    prog.SetUniform("model", Matrix4.Identity);
                    prog.SetUniform("textureEnabled", false);
                    prog.SetUniform("lightEnabled", false);
                    prog.SetUniform("flatColour", col);
                    GL.UseProgram(prog.ShaderProgramHandle);
                    if (iLevel > 6) Disc.Disc32.BindAndDraw();
                    else Disc.Disc16.BindAndDraw();
                }

                // Draw the name label for this star
                if (bShowLabels && st.Visited && !string.IsNullOrEmpty(st.Name)) {
                    tro.View = Matrix4.CreateScale(0.5f) * translateM;
                    tro.YPos = -0.5f;
                    tro.TextColour = Color.White;
                    TextRenderer.DrawWithOptions(st.Name, tro);
                }

                // Display whether this system has been colonised with a flag
                if (bShowFlags && st.Visited && st.Owner != null) {
                    Matrix4 translateM2 = Matrix4.CreateTranslation(0.0f, 0.5f, 0f);
                    Matrix4 scaleM = Matrix4.CreateScale(0.05f, 1.0f, 0.01f);
                    prog.SetUniform("model", scaleM * translateM * translateM2);
                    prog.SetUniform("textureEnabled", false);
                    prog.SetUniform("lightEnabled", false);
                    prog.SetUniform("flatColour", new Vector4(0.6f, 0.4f, 0.2f, 1.0f));
                    GL.UseProgram(prog.ShaderProgramHandle);
                    Square.Flat.BindAndDraw();
                    Matrix4 translateM3 = Matrix4.CreateTranslation(0.0f, 1.1f, 0f);
                    Matrix4 scaleM2 = Matrix4.CreateScale(0.6f, 0.4f, 0.01f);
                    prog.SetUniform("model", scaleM2 * translateM * translateM3);
                    prog.SetUniform("flatColour", new Vector4(st.Owner.Colour.R, st.Owner.Colour.G, st.Owner.Colour.B, 1.0f));
                    GL.UseProgram(prog.ShaderProgramHandle);
                    Square.Flat.BindAndDraw();
                }

                // Draw the system population
                if (bShowPop && st.Visited) {
                    int pop = st.GetPopulation();
                    if (pop == 0) continue;
                    if (bShowLabels) tro.YPos = -1.0f; // Offset under system name
                    else tro.YPos = -0.5f;
                    tro.View = Matrix4.CreateScale(0.5f) * translateM;
                    tro.TextColour = Color.Green;
                    TextRenderer.DrawWithOptions(pop.ToString(), tro);
                }
            }
        }

        public Star? CheckHover(double x, double y, double fMapViewZ) {
            foreach (Star st in Stars) {
                double dx = x - st.MapPos.X;
                double dy = y - st.MapPos.Y;
                double d2 = (dx * dx) + (dy * dy);
                double StarScale = (st.DrawScale * 0.1) + 0.03; // Scale to our view units. Add a border to make this a bit more forgiving
                if (d2 < (StarScale * StarScale)) return st;
            }
            return null;
        }

        public Star? GetMostCentralStar() {
            double AveX = 0.0, AveY = 0.0;
            foreach (Star st in Stars) {
                AveX += st.MapPos.X;
                AveY += st.MapPos.Y;
            }
            AveX /= Stars.Count;
            AveY /= Stars.Count;
            Star? stClosest = null;
            double dClosest = 100000.0;
            foreach (Star st in Stars) {
                double dx = st.MapPos.X - AveX;
                double dy = st.MapPos.Y - AveY;
                double d2 = ((dx * dx) + (dy * dy));
                if (stClosest == null || d2 < dClosest) {
                    dClosest = d2;
                    stClosest = st;
                }
            }
            return stClosest;
        }

        public Star? GetClosestNonColonisedSystemTo(Star stCentral) {
            Star? stClosest = null;
            double dClosest = 100000.0;
            foreach (Star st in Stars) {
                if (st.Owner != null) continue;
                double dx = st.MapPos.X - stCentral.MapPos.X;
                double dy = st.MapPos.Y - stCentral.MapPos.Y;
                double d2 = ((dx * dx) + (dy * dy));
                if (stClosest == null || d2 < dClosest) {
                    dClosest = d2;
                    stClosest = st;
                }
            }
            return stClosest;
        }

        public void DrawTradeRoutes(ShaderProgram prog) {
            List<VertexPos2DCol> vertices = new List<VertexPos2DCol>();
            foreach (Star st in Stars) {
                foreach (Star targ in st.TradeRoutes) {
                    // Only draw them from left to right, in order not to draw each one twice
                    if (st.MapPos.X < targ.MapPos.X && (st.Visited || targ.Visited)) {
                        // Add this line
                        Color4 colFrom = (st.Owner is null) ? Color4.LightGray : new Color4(st.Owner.Colour.R, st.Owner.Colour.G, st.Owner.Colour.B, 1f);
                        Color4 colTo = (targ.Owner is null) ? Color4.LightGray : new Color4(targ.Owner.Colour.R, targ.Owner.Colour.G, targ.Owner.Colour.B, 1f);
                        VertexPos2DCol v1 = new VertexPos2DCol(new Vector2(st.MapPos.X, st.MapPos.Y), colFrom);
                        VertexPos2DCol v2 = new VertexPos2DCol(new Vector2(targ.MapPos.X, targ.MapPos.Y), colTo);
                        vertices.Add(v1);
                        vertices.Add(v2);
                    }
                }
            }
            if (!vertices.Any()) return;
            // Create the buffer and array if not already created, or if list has changed
            // It's lilely only going to change by adding or subtracting routes, and very unlikely to change
            // but keep the same number of routes.
            if (tradeRoutesBuffer is null || tradeRoutesBuffer.VertexCount != vertices.Count) { 
                tradeRoutesBuffer = new VertexBuffer(vertices.ToArray(), BufferUsageHint.DynamicDraw);
            }
            tradeRoutesArray ??= new VertexArray(tradeRoutesBuffer);

            // Draw the lines
            GL.UseProgram(prog.ShaderProgramHandle);
            GL.BindVertexArray(tradeRoutesArray.VertexArrayHandle);
            GL.DrawArrays(PrimitiveType.Lines, 0, tradeRoutesBuffer.VertexCount);
            GL.BindVertexArray(0);
        }

        public Star? GetStarByID(int id) {
            if (id < 0 || id >= Stars.Count) return null;
            return Stars[id];
        }

        public AstronomicalObject? GetAOFromLocationString(string strLoc) {
            if (!strLoc.StartsWith("(") || !strLoc.Contains(")")) throw new Exception("Illegal location string:" + strLoc);
            string strMapLoc = strLoc.Substring(0, strLoc.IndexOf(")") + 1);
            string[] bits = strMapLoc.Replace("(", "").Replace(")", "").Split(',');
            if (bits.Length != 2) throw new Exception("Couldn't parse location string : " + strLoc + " - Sector location invalid : " + strMapLoc);
            int sX = int.Parse(bits[0]);
            int sY = int.Parse(bits[1]);
            if (sX != SectorX || sY != SectorY) return null;
            return GetAOFromLocationWithinSector(strLoc.Substring(strLoc.IndexOf(")") + 1));
        }
        public AstronomicalObject? GetAOFromLocationWithinSector(string strAOID) {
            string[] bits = strAOID.Split('.');
            if (bits.Length == 0 || bits.Length > 3) throw new Exception("Illegal location within sector : \"" + strAOID + "\"");
            int sno = int.Parse(bits[0]);
            Star? st = GetStarByID(sno);
            if (st == null || bits.Length == 1) return st;
            int pno = int.Parse(bits[1]);
            Planet? pl = st.GetPlanetByID(pno);
            if (pl == null || bits.Length == 2) return pl;
            int mno = int.Parse(bits[2]);
            Moon? mn = pl.GetMoonByID(mno);
            return mn;
        }

        public string PrintCoordinates() {
            return "(" + SectorX + "," + SectorY + ")";
        }
    }
}
