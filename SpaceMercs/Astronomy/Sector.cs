using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using SpaceMercs.Graphics;
using SpaceMercs.Graphics.Shapes;
using System.IO;
using System.Xml;

namespace SpaceMercs {
    public class Sector {
        private readonly List<Star> Stars = new List<Star>();
        public Race? Inhabitant = null;
        public int SectorX { get; private set; }
        public int SectorY { get; private set; }
        public Map ParentMap { get; private set; }
        public int Seed => ParentMap.MapSeed ^ ((SectorX* 85091) + (SectorY* 29527)) ^ ((SectorX* 34501) + (SectorY* 61819));
        VertexBuffer? tradeRoutesBuffer = null;
        VertexArray? tradeRoutesArray = null;
        public int SectorRing => Math.Max(Math.Abs(SectorX), Math.Abs(SectorY));

        public Sector() {
            ParentMap = Map.Empty;
        }
        public Sector(int sx, int sy, Map map) {
            SectorX = sx;
            SectorY = sy;
            ParentMap = map;
            Generate();
        }
        public Sector(XmlNode xml, Map map) {
            SectorX = xml.GetAttributeInt("X");
            SectorY = xml.GetAttributeInt("Y");
            ParentMap = map;
            Inhabitant = StaticData.GetRaceByName(xml.GetAttributeText("Inhabitant")); // May be blank

            IEnumerable<XmlNode> xStars = xml.SelectNodesToList("Star");
            if (xStars.Any()) {
                foreach (XmlNode xmls in xStars) {
                    Star st = new Star(xmls, this);
                    Stars.Add(st);
                }
            }
            else {
                Generate();
            }
        }
        public static Sector Empty { get { return new Sector(); } }

        private void Generate() {
            int seed = Seed; // Non-random seed; repeatable
            Random rand = new Random(seed);
            // Setup the stars in this sector
            float yoffset = (float)(((SectorY * 2) - 1) * Const.SectorSize) / 2.0f;
            float xoffset = (float)(((SectorX * 2) - 1) * Const.SectorSize) / 2.0f;
            for (int sno = 0; sno < ParentMap.StarsPerSector; sno++) {
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
        }

        // Save this sector to an Xml file
        public void SaveToFile(StreamWriter file, GlobalClock clock) {
            file.WriteLine($" <Sector X=\"{SectorX}\" Y=\"{SectorY}\" Inhabitant=\"{((Inhabitant == null) ? "" : Inhabitant.Name)}\">");
            foreach (Star st in Stars) {
                st.SaveToFile(file, clock);
            }
            file.WriteLine(" </Sector>");
        }
        public bool ShouldBeSaved() {
            foreach (Star st in Stars) {
                if (st.bGenerated && (st.Owner != null || st.Visited || st.Scanned || st.Renamed)) return true;
            }
            return false;
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

        public void Draw(ShaderProgram prog, bool bFadeUnvisited, bool bShowLabels, bool bShowFlags, bool bShowPop, float fMapViewX, float fMapViewY, float fMapViewZ, float aspect, double elapsedSeconds) {
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

            // Faded stars
            // Draw these separately for speed
            prog.SetUniform("textureEnabled", false);
            prog.SetUniform("lightEnabled", false);
            prog.SetUniform("model", Matrix4.Identity);
            GL.UseProgram(prog.ShaderProgramHandle);
            if (fMapViewZ > 20) Disc.Disc16.Bind();
            else Disc.Disc32.Bind();
            foreach (Star st in Stars) {
                // Work out the degree of detail to show in this star
                int iLevel = st.GetDetailLevel(fMapViewX, fMapViewY, fMapViewZ);

                if ((bFadeUnvisited && !st.Visited) || iLevel <= 2) {
                    // Fade out unvisited stars (if set to do so)
                    float fade = 1f;
                    if (bFadeUnvisited && !st.Visited) fade = 4f;
                    Vector4 col = new Vector4(st.BaseColour.X / fade, st.BaseColour.Y / fade, st.BaseColour.Z / fade, 1.0f);

                    // Translate into the star frame
                    Matrix4 translateM = Matrix4.CreateTranslation(st.MapPos.X - fMapViewX, st.MapPos.Y - fMapViewY, -fMapViewZ);
                    Matrix4 scaleM = Matrix4.CreateScale(st.DrawScale / 8f);
                    Matrix4 viewM = scaleM * translateM;

                    prog.SetUniform("view", viewM);
                    prog.SetUniform("flatColour", col);
                    GL.UseProgram(prog.ShaderProgramHandle);

                    if (fMapViewZ > 20) Disc.Disc16.Draw();
                    else Disc.Disc32.Draw();
                }
            }
            if (fMapViewZ > 20) Disc.Disc16.Unbind();
            else Disc.Disc32.Unbind();

            // Precalc matrices
            Matrix4 scaleText = Matrix4.CreateScale(0.5f);
            Matrix4 scaleFlag = Matrix4.CreateScale(0.6f, 0.4f, 0.01f);
            Matrix4 scaleFlagPole = Matrix4.CreateScale(0.05f, 1.0f, 0.01f);

            // Display remaining stars, text, flags etc.
            foreach (Star st in Stars) {
                // Translate into the star frame
                Matrix4 translateStar = Matrix4.CreateTranslation(st.MapPos.X - fMapViewX, st.MapPos.Y - fMapViewY, -fMapViewZ);

                // Work out the degree of detail to show in this star
                int iLevel = st.GetDetailLevel(fMapViewX, fMapViewY, fMapViewZ);

                // If the star is close to the viewer and not faded then show the textured sphere
                if ((!bFadeUnvisited || st.Visited) && iLevel >= 3) {
                    Matrix4 viewM = scaleText * translateStar;
                    prog.SetUniform("view", viewM);
                    prog.SetUniform("flatColour", new Vector4(1f, 1f, 1f, 1f));
                    st.DrawSelected(prog, iLevel, elapsedSeconds);
                }

                // Draw the name label for this star
                bool hasLabel = false;
                if (bShowLabels && st.Visited && !string.IsNullOrEmpty(st.Name)) {
                    tro.View = Matrix4.CreateScale(0.5f) * translateStar;
                    tro.YPos = -(0.15f + (Const.StarScale * st.DrawScale));
                    tro.TextColour = Color.White;
                    TextRenderer.DrawWithOptions(st.Name, tro);
                    hasLabel = true;
                }

                // Display whether this system has been colonised with a flag
                if (bShowFlags && st.Visited && st.Owner != null) {
                    Matrix4 viewFlag = Matrix4.CreateScale(0.5f) * translateStar;
                    Matrix4 translateFlagPole = Matrix4.CreateTranslation(0.0f, 0.2f + (Const.StarScale * st.DrawScale), 0f);
                    prog.SetUniform("view", viewFlag);
                    prog.SetUniform("model", scaleFlagPole * translateStar * translateFlagPole);
                    prog.SetUniform("textureEnabled", false);
                    prog.SetUniform("lightEnabled", false);
                    prog.SetUniform("flatColour", new Vector4(0.6f, 0.4f, 0.2f, 1.0f));
                    GL.UseProgram(prog.ShaderProgramHandle);
                    Square.Flat.Bind();
                    Square.Flat.Draw();
                    Matrix4 translateFlag = Matrix4.CreateTranslation(0.0f, 0.8f + (Const.StarScale * st.DrawScale), 0f);
                    prog.SetUniform("model", scaleFlag * translateStar * translateFlag);
                    prog.SetUniform("flatColour", new Vector4((float)st.Owner.Colour.R / 255f, (float)st.Owner.Colour.G / 255f, (float)st.Owner.Colour.B / 255f, 1.0f));
                    GL.UseProgram(prog.ShaderProgramHandle);
                    Square.Flat.Draw();
                    Square.Flat.Unbind();
                }

                // Draw the system population
                if (bShowPop && st.Visited) {
                    int pop = st.GetPopulation();
                    if (pop == 0) continue;
                    if (hasLabel) tro.YPos = -0.55f; // Offset under system name
                    else tro.YPos = -0.05f;
                    tro.YPos -= Const.StarScale * st.DrawScale;
                    tro.View = Matrix4.CreateScale(0.5f) * translateStar;
                    tro.TextColour = Color.LightGreen;
                    tro.Scale = 0.5f;
                    TextRenderer.DrawWithOptions(pop.ToString(), tro);
                    tro.Scale = 0.3f;
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
        public Star? GetRandomColonisedSystem(Random rand) {
            List<Star> colonisedStars = new List<Star>();
            foreach (Star st in Stars) {
                if (st.Owner != null) colonisedStars.Add(st);
            }
            if (colonisedStars.Count == 0) return null;
            return colonisedStars[rand.Next(colonisedStars.Count)];
        }
        public List<Star> GetStarsInDistanceOrderFrom(Star stCentral) {
            return Stars.OrderBy(st => (st.MapPos - stCentral.MapPos).LengthFast).ToList();
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
            // It's likely only going to change by adding or subtracting routes, and very unlikely to change
            // whilst keep the same number of routes, so test for a change in route count.
            if (tradeRoutesBuffer is null || tradeRoutesBuffer.VertexCount != vertices.Count) { 
                tradeRoutesBuffer = new VertexBuffer(vertices.ToArray(), BufferUsageHint.DynamicDraw);
                tradeRoutesArray = new VertexArray(tradeRoutesBuffer);
            }
            // Just in case it hasn't been set up yet...
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
            if (!strLoc.StartsWith("(") || !strLoc.Contains(')')) throw new Exception("Illegal location string:" + strLoc);
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
            if (string.Equals(bits[1], "HG")) return st.GetHyperGate() ?? throw new Exception($"Could not find saved HyperGate location {strAOID}");
            if (string.Equals(bits[1], "SH")) return st.SpaceHulk ?? throw new Exception($"Could not find saved SpaceHulk location {strAOID}");
            if (!int.TryParse(bits[1], out int pno)) {
                throw new Exception($"Could not parse planet in location string {strAOID}");
            }
            Planet? pl = st.GetPlanetByID(pno);
            if (pl == null || bits.Length == 2) return pl;
            if (!int.TryParse(bits[2], out int mno)) {
                throw new Exception($"Could not parse moon in location string {strAOID}");
            }
            Moon? mn = pl.GetMoonByID(mno);
            return mn;
        }

        public string PrintCoordinates() {
            return "(" + SectorX + "," + SectorY + ")";
        }
    }
}
