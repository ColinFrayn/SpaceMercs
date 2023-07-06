using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using SpaceMercs.Graphics;
using SpaceMercs.Graphics.Shapes;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

namespace SpaceMercs {
    class MissionLevel {
        // "Gap" means that there is no wall, but there is a hole here so that you cannot walk across it, but may cross it if you have a jetpack (e.g.)
        // NOTE - if there are ever > 26 of these types then the current load/save mechanism will fail because it relies on characters
        public enum TileType { Floor, Wall, Gap, Void, Machinery, DoorVertical, DoorHorizontal, OpenDoorVertical, OpenDoorHorizontal, SecretDoorVertical, SecretDoorHorizontal }

        private readonly int Diff;
        private readonly bool bInitialised;
        private int[,] TextureCoords;
        private readonly HashSet<Point> EntryLocations = new HashSet<Point>(); //  = "To/From above" if multi-level
        private readonly HashSet<Point> ExitLocations = new HashSet<Point>(); //  = "To/From below" if multi-level
        private readonly List<IEntity> Entities = new List<IEntity>();
        private readonly Dictionary<Point, Stash> Items = new Dictionary<Point, Stash>();
        private readonly Dictionary<Point, Trap> Traps = new Dictionary<Point, Trap>();
        private IEntity?[,] EntityMap;
        private int HoverX = -1, HoverY = -1;
        private Random rand = new Random();
        private const float TexEpsilon = 0.01f;

        public TileType[,] Map { get; private set; }
        public bool[,] Explored { get; private set; }
        public bool[,] Visible { get; private set; }
        public int Width { get; private set; }
        public int Height { get; private set; }
        public Point MidPoint { get { if (!bInitialised) return new Point(0, 0); return new Point(Width / 2, Height / 2); } }
        public int MaximumSoldiers { get { if (EntryLocations.Any()) return EntryLocations.Count; return 4; } }
        public Mission ParentMission { get; private set; }
        public int LevelID { get; private set; }
        public Point MouseHover {
            get {
                if (HoverX < 0 || HoverX >= Width || HoverY < 0 || HoverY >= Height) return Point.Empty;
                if (!Visible[HoverX, HoverY]) return Point.Empty;
                return new Point(HoverX, HoverY);
            }
        }
        public bool AllEnemiesKilled {
            get {
                return (!Creatures.Any());
            }
        }
        public bool AlertedEnemies {
            get {
                foreach (Creature cr in Creatures) {
                    if (cr.IsAlert) return true;
                }
                return false;
            }
        }
        public bool FullyExplored {
            get {
                for (int y = 0; y < Height; y++) {
                    for (int x = 0; x < Width; x++) {
                        if (Map[x, y] != TileType.Wall && !Explored[x, y]) return false;
                    }
                }
                return true;
            }
        }
        public int CountMissionItemsRemaining {
            get {
                int count = 0;
                foreach (Creature cr in Creatures) {
                    if (cr.QuestItem) count++;
                }
                foreach (Stash st in Items.Values) {
                    count += st.GetCount(ParentMission.MItem);
                }
                return count;
            }
        }
        public Tuple<int, int> UnexploredTiles {
            get {
                int count = 0, total = 0;
                for (int y = 0; y < Height; y++) {
                    for (int x = 0; x < Width; x++) {
                        if (Map[x, y] != TileType.Wall) {
                            total++;
                            if (!Explored[x, y]) count++;
                        }
                    }
                }
                return new Tuple<int, int>(count, total);
            }
        }

        // Public access to entities sorted by type
        public IEnumerable<Creature> Creatures { get { return Entities.OfType<Creature>().Select(e => e as Creature).ToList<Creature>().AsReadOnly(); } }
        public IEnumerable<Soldier> Soldiers { get { return Entities.OfType<Soldier>().Select(e => e as Soldier).ToList().AsReadOnly(); } }

        // These are used for map generation only (can be ignored after this point)
        private int[,] RoomMap; // Only used for dungeon map generation
        private int StartX = -1, StartY = -1, EndX = -1, EndY = -1; // Only used for map generation

        // CTORs
        public MissionLevel(Mission parentMission, int diff, int level) {
            ParentMission = parentMission;
            Diff = diff;
            LevelID = level;
            bInitialised = false;
            GenerateMap();
            // All done
            bInitialised = true;
        }
        public MissionLevel(XmlNode xml, Mission parentMission) {
            ParentMission = parentMission;
            HoverX = HoverY = -1;
            int w = int.Parse(xml.Attributes["Width"].Value);
            int h = int.Parse(xml.Attributes["Height"].Value);
            Width = w;
            Height = h;
            Diff = int.Parse(xml.SelectSingleNode("Diff").InnerText);
            LevelID = int.Parse(xml.SelectSingleNode("Level").InnerText);
            Map = new TileType[w, h];
            Explored = new bool[w, h];
            Visible = new bool[w, h];
            EntityMap = new IEntity[w, h];
            StartX = int.Parse(xml.SelectSingleNode("Start").Attributes["X"].Value);
            StartY = int.Parse(xml.SelectSingleNode("Start").Attributes["Y"].Value);
            SetupEntryLocations();
            EndX = int.Parse(xml.SelectSingleNode("End").Attributes["X"].Value);
            EndY = int.Parse(xml.SelectSingleNode("End").Attributes["Y"].Value);
            SetupExitLocations();

            // Map
            XmlNode xmlm = xml.SelectSingleNode("Map") ?? throw new Exception("Could not identify Map node");
            string strMap = Utils.RunLengthDecode(xmlm.InnerText);
            for (int y = 0; y < Height; y++) {
                for (int x = 0; x < Width; x++) {
                    Map[x, y] = (TileType)(strMap.ElementAt<char>((y * Width) + x) - (char)'A');
                }
            }

            // Explored
            XmlNode xmlx = xml.SelectSingleNode("Explored") ?? throw new Exception("Could not identify Explored node");
            string strExp = Utils.RunLengthDecode(xmlx.InnerText);
            for (int y = 0; y < Height; y++) {
                for (int x = 0; x < Width; x++) {
                    Explored[x, y] = strExp.ElementAt<char>((y * Width) + x) == '1';
                }
            }

            // Creatures
            XmlNode xmlc = xml.SelectSingleNode("Creatures") ?? throw new Exception("Could not identify Creatures node");
            foreach (XmlNode xc in xmlc.SelectNodes("Creature")) {
                Creature cr = new Creature(xc, this);
                AddCreatureWithoutReset(cr);
            }

            Items.Clear();
            // Item stashes
            XmlNode? xmli = xml.SelectSingleNode("Items"); // Old format
            if (xmli is not null) {
                foreach (XmlNode xn in xmli.SelectNodes("Stack")) {
                    Dictionary<IItem, int> dict = new Dictionary<IItem, int>();
                    int x = int.Parse(xn.Attributes["X"].Value);
                    int y = int.Parse(xn.Attributes["Y"].Value);
                    Point pt = new Point(x, y);
                    foreach (XmlNode xnn in xn.SelectNodes("StackItem")) {
                        int n = int.Parse(xnn.Attributes["N"].Value);
                        IItem it = Utils.LoadItem(xnn.FirstChild) ?? throw new Exception($"Could not load item from stack : {xnn.FirstChild?.InnerText ?? "null"}");
                        dict.Add(it, n);
                    }
                    Items.Add(pt, new Stash(dict, pt));
                }
            }
            XmlNode? xmls = xml.SelectSingleNode("Stashes"); // New format
            if (xmls is not null) {
                foreach (XmlNode xn in xmls.SelectNodes("Stash")) {
                    Stash s = new Stash(xn);
                    Items.Add(s.Location, s);
                }
            }

            // Traps
            Traps.Clear();
            XmlNode? xmlt = xml.SelectSingleNode("Traps");
            if (xmlt is not null) {
                foreach (XmlNode xn in xmlt.SelectNodes("Trap")) {
                    Trap t = new Trap(xn);
                    Traps.Add(t.Location, t);
                }
            }

            bInitialised = true;
        }

        // After loading a level, set all creature targets properly (from the X,Y loaded to an IEntity)
        public void SetCreatureTargets() {
            foreach (IEntity ent in Entities) {
                if (ent is Creature cr) {
                    if (cr.TX < 0 || cr.TX >= Width || cr.TY < 0 || cr.TY >= Height) continue;
                    cr.SetTarget(EntityMap[cr.TX, cr.TY]);
                }
            }
        }

        public void SaveToFile(StreamWriter file) {
            file.WriteLine("<MissionLevel Width=\"" + Width + "\" Height=\"" + Height + "\">");
            file.WriteLine(" <Diff>" + Diff + "</Diff>");
            file.WriteLine(" <Level>" + LevelID + "</Level>");
            file.WriteLine(" <Start X=\"" + StartX + "\" Y=\"" + StartY + "\"/>");
            file.WriteLine(" <End X=\"" + EndX + "\" Y=\"" + EndY + "\"/>");

            // Save the map itself
            if (Map != null) {
                StringBuilder sb = new StringBuilder("");
                for (int y = 0; y < Height; y++) {
                    for (int x = 0; x < Width; x++) {
                        sb.Append((char)((int)Map[x, y] + 'A'));
                    }
                }
                file.WriteLine(" <Map>" + Utils.RunLengthEncode(sb.ToString()) + "</Map>");
            }

            // Explored bitmap
            if (Explored != null) {
                StringBuilder sb = new StringBuilder("");
                for (int y = 0; y < Height; y++) {
                    for (int x = 0; x < Width; x++) {
                        sb.Append(Explored[x, y] ? "1" : "0");
                    }
                }
                file.WriteLine(" <Explored>" + Utils.RunLengthEncode(sb.ToString()) + "</Explored>");
            }

            // Save creatures
            file.WriteLine("<Creatures>");
            foreach (IEntity en in Entities) {
                if (en is Creature cr) {
                    cr.SaveToFile(file);
                }
            }
            file.WriteLine("</Creatures>");

            // Save item stashes
            if (Items.Count > 0) {
                file.WriteLine("<Stashes>");
                foreach (Point pt in Items.Keys) {
                    Items[pt].SaveToFile(file, pt);
                }
                file.WriteLine("</Stashes>");
            }

            // Save traps
            if (Traps.Count > 0) {
                file.WriteLine("<Traps>");
                foreach (Point pt in Traps.Keys) {
                    Traps[pt].SaveToFile(file, pt);
                }
                file.WriteLine("</Traps>");
            }

            file.WriteLine("</MissionLevel>");
        }

        #region Display
        public void DisplayMap(ShaderProgram prog) {
            if (!bInitialised) return;
            prog.SetUniform("textureEnabled", true);
            GL.ActiveTexture(TextureUnit.Texture0);
            GL.Disable(EnableCap.DepthTest);

            DisplayTiles(prog);            
            DisplayDoors(prog);

            // Entry/exit locations
            foreach (Point p in EntryLocations) DrawEntryLocation(prog, p);
            foreach (Point p in ExitLocations) DrawExitLocation(prog, p);

            DrawFogOfWar(prog);
        }
        private void DisplayDoors(ShaderProgram prog) {
            // Draw all doors
            prog.SetUniform("textureEnabled", false);
            GL.Enable(EnableCap.Blend);
            for (int y = 0; y < Height; y++) {
                for (int x = 0; x < Width; x++) {
                    if (!Const.DEBUG_VISIBLE_ALL && !Explored[x, y]) continue;                    
                    if (Map[x, y] == TileType.DoorVertical || Map[x, y] == TileType.DoorHorizontal) prog.SetUniform("flatColour", new Vector4(0.6f, 0.4f, 0.1f, 1f));
                    else if (Map[x, y] == TileType.SecretDoorVertical || Map[x, y] == TileType.SecretDoorHorizontal) prog.SetUniform("flatColour", new Vector4(0.9f, 0.8f, 0f, 1f));
                    else if (Map[x, y] == TileType.OpenDoorVertical || Map[x, y] == TileType.OpenDoorHorizontal) prog.SetUniform("flatColour", new Vector4(0.6f, 0.4f, 0.1f, 0.2f));
                    // Draw it
                    if (Map[x, y] == TileType.DoorVertical || Map[x, y] == TileType.OpenDoorVertical || (Const.DEBUG_VISIBLE_ALL && Map[x, y] == TileType.SecretDoorVertical)) {
                        Matrix4 pTranslateM = Matrix4.CreateTranslation(x + 0.2f, y, Const.DoodadLayer);
                        Matrix4 pScaleM = Matrix4.CreateScale(0.6f, 1f, 1f);
                        prog.SetUniform("model", pScaleM * pTranslateM);
                        GL.UseProgram(prog.ShaderProgramHandle);
                        Square.Flat.BindAndDraw();
                    }
                    else if (Map[x, y] == TileType.DoorHorizontal || Map[x, y] == TileType.OpenDoorHorizontal || (Const.DEBUG_VISIBLE_ALL && Map[x, y] == TileType.SecretDoorHorizontal)) {
                        Matrix4 pTranslateM = Matrix4.CreateTranslation(x, y + 0.2f, Const.DoodadLayer);
                        Matrix4 pScaleM = Matrix4.CreateScale(1f, 0.6f, 1f);
                        prog.SetUniform("model", pScaleM * pTranslateM);
                        GL.UseProgram(prog.ShaderProgramHandle);
                        Square.Flat.BindAndDraw();
                    }
                }
            }
            GL.Disable(EnableCap.Blend);
        }
        private void DisplayTiles(ShaderProgram prog) {
            if (TextureCoords is null) GenerateTextures();
            GL.Disable(EnableCap.Blend);

            // Draw the visible terrain
            int iLastID = -1, tw = 0, th = 0;
            for (int y = 0; y < Height; y++) {
                for (int x = 0; x < Width; x++) {
                    if (!Const.DEBUG_VISIBLE_ALL && !Explored[x, y]) continue;
                    TexDetails det;
                    if (Map[x, y] == TileType.Floor || Map[x, y] == TileType.DoorVertical || Map[x, y] == TileType.DoorHorizontal || Map[x, y] == TileType.OpenDoorVertical || Map[x, y] == TileType.OpenDoorHorizontal) {
                        det = Textures.GenerateFloorTexture(this);
                    }
                    else if (Map[x, y] == TileType.Wall || Map[x, y] == TileType.SecretDoorHorizontal || Map[x, y] == TileType.SecretDoorVertical) {
                        Textures.WallSide ws = GetWallSides(x, y);
                        det = Textures.GenerateWallTexture(this, ws);
                    }
                    else {
                        throw new Exception("Unhandled texture requested : " + Map[x, y]);
                    }
                    int iTexID = det.ID;
                    tw = det.W / Textures.TileSize;
                    th = det.H / Textures.TileSize;
                    if (iTexID != iLastID) {
                        GL.BindTexture(TextureTarget.Texture2D, iTexID);
                        iLastID = iTexID;
                    }
                    // Draw the square using the correct texture
                    int itc = TextureCoords[x, y];
                    float tx = (float)((itc & 3) % tw) / (float)tw;
                    float ty = (float)((itc / 4) % th) / (float)th;
                    prog.SetUniform("texPos", tx + TexEpsilon, ty + TexEpsilon);
                    prog.SetUniform("texScale", (1f / tw) - (TexEpsilon*2f), (1f / th) - (TexEpsilon * 2f));
                    Matrix4 pTranslateM = Matrix4.CreateTranslation(x, y, Const.TileLayer);
                    prog.SetUniform("model", pTranslateM);
                    GL.UseProgram(prog.ShaderProgramHandle);
                    Square.Textured.BindAndDraw();
                }
            }
        }
        private void GenerateTextures() {
            TextureCoords = new int[Width, Height];
            for (int y = 0; y < Height; y++) {
                for (int x = 0; x < Width; x++) {
                    TextureCoords[x, y] = rand.Next(16);
                }
            }
        }
        private Textures.WallSide GetWallSides(int x, int y) {
            // Catch unexpected values
            if (x < 0 || x >= Width || y < 0 || y >= Height) return Textures.AllSides;
            if (!LooksLikeAWall(x, y)) return Textures.AllSides;

            // Is there a floor tile in the eight neighbouring cells?
            Textures.WallSide ws = 0;
            if (x > 0 && !LooksLikeAWall(x - 1, y)) ws |= Textures.WallSide.Left;
            if (x < Width - 1 && !LooksLikeAWall(x + 1, y)) ws |= Textures.WallSide.Right;
            if (y > 0 && !LooksLikeAWall(x, y - 1)) ws |= Textures.WallSide.Up;
            if (y < Height - 1 && !LooksLikeAWall(x, y + 1)) ws |= Textures.WallSide.Down;
            if (x > 0 && y > 0 && !LooksLikeAWall(x - 1, y - 1)) ws |= Textures.WallSide.UpLeft;
            if (x < Width - 1 && y > 0 && !LooksLikeAWall(x + 1, y - 1)) ws |= Textures.WallSide.UpRight;
            if (x > 0 && y < Height - 1 && !LooksLikeAWall(x - 1, y + 1)) ws |= Textures.WallSide.DownLeft;
            if (x < Width - 1 && y < Height - 1 && !LooksLikeAWall(x + 1, y + 1)) ws |= Textures.WallSide.DownRight;
            return ws;
        }
        private void DrawEntryLocation(ShaderProgram prog, Point p) {
            if (!Const.DEBUG_VISIBLE_ALL && !Explored[p.X, p.Y]) return;
            prog.SetUniform("textureEnabled", false);
            prog.SetUniform("flatColour", new Vector4(1f, 1f, 0f, 1f));
            Matrix4 pTranslateM = Matrix4.CreateTranslation(p.X + 0.1f, p.Y + 0.1f, Const.DoodadLayer);
            Matrix4 pScaleM = Matrix4.CreateScale(0.8f);
            prog.SetUniform("model", pScaleM * pTranslateM);
            GL.UseProgram(prog.ShaderProgramHandle);
            Square.Flat.BindAndDraw();

            prog.SetUniform("flatColour", new Vector4(0.3f, 0.3f, 0.3f, 1f));
            pTranslateM = Matrix4.CreateTranslation(p.X + 0.5f, p.Y + 0.3f, Const.DoodadLayer);
            pScaleM = Matrix4.CreateScale(0.3f, 0.4f, 1f);
            prog.SetUniform("model", pScaleM * pTranslateM);
            GL.UseProgram(prog.ShaderProgramHandle);
            Triangle.Flat.BindAndDraw();
        }
        private void DrawExitLocation(ShaderProgram prog, Point p) {
            if (!Const.DEBUG_VISIBLE_ALL && !Explored[p.X, p.Y]) return;
            prog.SetUniform("textureEnabled", false);
            prog.SetUniform("flatColour", new Vector4(1f, 1f, 0f, 1f));
            Matrix4 pTranslateM = Matrix4.CreateTranslation(p.X + 0.1f, p.Y + 0.1f, Const.DoodadLayer);
            Matrix4 pScaleM = Matrix4.CreateScale(0.8f);
            prog.SetUniform("model", pScaleM * pTranslateM);
            GL.UseProgram(prog.ShaderProgramHandle);
            Square.Flat.BindAndDraw();

            prog.SetUniform("flatColour", new Vector4(0.3f, 0.3f, 0.3f, 1f));
            pTranslateM = Matrix4.CreateTranslation(p.X + 0.5f, p.Y + 0.65f, Const.DoodadLayer);
            pScaleM = Matrix4.CreateScale(0.3f, -0.4f, 1f);
            prog.SetUniform("model", pScaleM * pTranslateM);
            GL.UseProgram(prog.ShaderProgramHandle);
            Triangle.Flat.BindAndDraw();
        }
        private void DrawFogOfWar(ShaderProgram prog) {
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
            prog.SetUniform("textureEnabled", true);
            GL.BindTexture(TextureTarget.Texture2D, Textures.FogOfWarTexture);
            prog.SetUniform("texPos", 0f, 0f);
            prog.SetUniform("texScale", 1f, 1f);
            GL.DepthMask(false);
            for (int y = 0; y < Height; y++) {
                for (int x = 0; x < Width; x++) {
                    if (Map[x, y] == TileType.Void) continue;
                    if (!Const.DEBUG_VISIBLE_ALL && Explored[x, y] && !Visible[x, y]) {
                        Matrix4 pTranslateM = Matrix4.CreateTranslation(x, y, Const.EntityLayer);
                        prog.SetUniform("model", pTranslateM);
                        GL.UseProgram(prog.ShaderProgramHandle);
                        Square.Textured.BindAndDraw();
                    }
                }
            }
            GL.DepthMask(true);
            GL.Disable(EnableCap.Blend);
        }
        public void DisplayEntities(ShaderProgram prog, bool bShowLabels, bool bShowStatBars, bool bShowEffects, float fViewHeight) {
            // Display soldiers & creatures
            GL.Enable(EnableCap.Blend);
            GL.Disable(EnableCap.DepthTest);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
            // To handle concurrent access to these lists, create RO copies first, and loop over those
            IEnumerable<Point> lItems = new List<Point>(Items.Keys).AsReadOnly();
            foreach (Point pt in lItems) {
                if (Visible[pt.X, pt.Y] || Const.DEBUG_VISIBLE_ALL) DisplayItemStack(prog, pt);
            }
            IEnumerable<Point> lTraps = new List<Point>(Traps.Keys).AsReadOnly();
            foreach (Point pt in lTraps) {
                if (Visible[pt.X, pt.Y] || Const.DEBUG_VISIBLE_ALL) DisplayTrap(prog, pt);
            }
            IEnumerable<IEntity> lEntities = Entities.ToList().AsReadOnly();
            float aspect = ParentMission.CurrentMapView?.Aspect ?? 1f;
            Matrix4 viewM = ParentMission.CurrentMapView?.ViewMatrix ?? Matrix4.Identity;
            foreach (IEntity e in lEntities) {
                if (Visible[e.X, e.Y] || Const.DEBUG_VISIBLE_ALL) e.Display(prog, bShowLabels, bShowStatBars, bShowEffects, fViewHeight, aspect, viewM);
            }
        }
        private void DisplayItemStack(ShaderProgram prog, Point pt) {
            if (Items[pt].Hidden) return;
            TexSpecs ts;
            if (Items[pt].ContainsOnlyCorpses) ts = Textures.GetTexCoords(Textures.MiscTexture.Bones, true);
            else if (Items[pt].Count < 6) ts = Textures.GetTexCoords(Textures.MiscTexture.Coins, true);
            else ts = Textures.GetTexCoords(Textures.MiscTexture.Treasure, true);

            GL.BindTexture(TextureTarget.Texture2D, ts.ID);
            prog.SetUniform("flatColour", new Vector4(1f, 1f, 1f, 1f));
            prog.SetUniform("textureEnabled", true);
            prog.SetUniform("texPos", ts.X, ts.Y);
            prog.SetUniform("texScale", ts.W, ts.H);
            Matrix4 pTranslateM = Matrix4.CreateTranslation(pt.X + 0.1f, pt.Y + 0.9f, Const.EntityLayer);
            Matrix4 pScaleM = Matrix4.CreateScale(0.8f, -0.8f, 1f);
            prog.SetUniform("model", pScaleM * pTranslateM);
            GL.UseProgram(prog.ShaderProgramHandle);
            Square.Textured.BindAndDraw();
        }
        private void DisplayTrap(ShaderProgram prog, Point pt) {
            if (Traps[pt].Hidden) return;
            TexSpecs ts = Textures.GetTexCoords(Textures.MiscTexture.Trap, true);
            GL.BindTexture(TextureTarget.Texture2D, ts.ID);
            prog.SetUniform("flatColour", new Vector4(1f, 1f, 1f, 1f));
            prog.SetUniform("textureEnabled", true);
            prog.SetUniform("texPos", ts.X, ts.Y);
            prog.SetUniform("texScale", ts.W, ts.H);
            Matrix4 pTranslateM = Matrix4.CreateTranslation(pt.X + 0.1f, pt.Y + 0.9f, Const.EntityLayer);
            Matrix4 pScaleM = Matrix4.CreateScale(0.8f, -0.8f, 1f);
            prog.SetUniform("model", pScaleM * pTranslateM);
            GL.UseProgram(prog.ShaderProgramHandle);
            Square.Textured.BindAndDraw();
        }
        public void DrawDetectionMap(ShaderProgram prog, bool[,] DetectionMap) {
            TexSpecs ts = Textures.GetTexCoords(Textures.MiscTexture.Alert, true);
            GL.BindTexture(TextureTarget.Texture2D, ts.ID);
            prog.SetUniform("flatColour", new Vector4(1f, 1f, 1f, 1f));
            prog.SetUniform("textureEnabled", true);
            prog.SetUniform("texPos", ts.X, ts.Y);
            prog.SetUniform("texScale", ts.W, ts.H);
            Matrix4 pScaleM = Matrix4.CreateScale(0.8f, -0.8f, 1f);
            for (int y = 0; y < Height; y++) {
                for (int x = 0; x < Width; x++) {
                    if (!Const.DEBUG_VISIBLE_ALL && !Explored[x, y]) continue;
                    if (!DetectionMap[x, y]) continue;
                    Matrix4 pTranslateM = Matrix4.CreateTranslation(x + 0.1f, y + 0.9f, Const.GUILayer);
                    prog.SetUniform("model", pScaleM * pTranslateM);
                    GL.UseProgram(prog.ShaderProgramHandle);
                    Square.Textured.BindAndDraw();
                }
            }
        }
        public void DrawSelectedEntityVis(ShaderProgram prog, IEntity en) {
            prog.SetUniform("flatColour", new Vector4(1f, 1f, 1f, 0.5f));
            prog.SetUniform("textureEnabled", false);
            Matrix4 pScaleM = Matrix4.CreateScale(0.4f);
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
            for (int y = 0; y < Height; y++) {
                for (int x = 0; x < Width; x++) {
                    if (!en.CanSee(x, y)) continue;
                    Matrix4 pTranslateM = Matrix4.CreateTranslation(x + 0.3f, y + 0.3f, Const.GUILayer);
                    prog.SetUniform("model", pScaleM * pTranslateM);
                    GL.UseProgram(prog.ShaderProgramHandle);
                    Square.Textured.BindAndDraw();
                }
            }
        }
        #endregion // Display

        #region Map Generation
        public void GenerateMap() {
            rand = new Random(ParentMission.Seed + LevelID); // Make it repeatable :)
            SetupMapDimensions();
            if (ParentMission.IsShipMission) GenerateShipMap();
            else if (ParentMission.Type == Mission.MissionType.AbandonedCity) GenerateDungeonMap();
            else if (ParentMission.Type == Mission.MissionType.Mines) GenerateDungeonMap(true);
            else if (ParentMission.Type == Mission.MissionType.Caves) GenerateCaveMap();
            else if (ParentMission.Type == Mission.MissionType.Surface) GenerateSurfaceMap();
            else throw new NotImplementedException();
            Explored = new bool[Width, Height];
            Visible = new bool[Width, Height];
            EntityMap = new IEntity[Width, Height];
            GenerateCreatures();
            // Add goal if required
            if (ParentMission.Goal == Mission.MissionGoal.FindItem) {
                if (ParentMission.MItem is null) throw new Exception("No goal item type set up for FindItem mission");
                if (LevelID == ParentMission.LevelCount - 1) {
                    InsertGoalItem(ParentMission.MItem);
                }
            }
            if (ParentMission.Goal == Mission.MissionGoal.Gather) {
                if (ParentMission.MItem is null) throw new Exception("No goal item type set up for Gather mission");
                int nitems = ParentMission.Size + 2 + LevelID;
                for (int n = 0; n < nitems; n++) {
                    InsertGoalItem(ParentMission.MItem);
                }
            }
        }
        private void SetupMapDimensions() {
            if (ParentMission.IsShipMission) {
                if (ParentMission.ShipTarget is null) throw new Exception("No ship item set up for Ship mission");
                Width = (ParentMission.ShipTarget.Type.Length * 4) + 9;
                Height = (ParentMission.ShipTarget.Type.Width * 4) + 9;
            }
            else if (ParentMission.Type == Mission.MissionType.AbandonedCity || ParentMission.Type == Mission.MissionType.Mines) {
                Width = 12 + (int)(Math.Pow(1.5, ParentMission.Size - 1) * 12) + rand.Next((ParentMission.Size * 3) + 3);
                Height = 12 + (int)(Math.Pow(1.5, ParentMission.Size - 1) * 12) + rand.Next((ParentMission.Size * 3) + 3);
            }
            else if (ParentMission.Type == Mission.MissionType.Caves) {
                Width = 12 + (int)(Math.Pow(1.5, ParentMission.Size - 1) * 12) + rand.Next((ParentMission.Size * 3) + 3);
                Height = 12 + (int)(Math.Pow(1.5, ParentMission.Size - 1) * 12) + rand.Next((ParentMission.Size * 3) + 3);
            }
            else if (ParentMission.Type == Mission.MissionType.Surface) {
                Width = 12 + (int)(Math.Pow(1.5, ParentMission.Size - 1) * 16) + rand.Next((ParentMission.Size * 3) + 3);
                Height = 12 + (int)(Math.Pow(1.5, ParentMission.Size - 1) * 16) + rand.Next((ParentMission.Size * 3) + 3);
            }
            else throw new NotImplementedException();
            Map = new TileType[Width, Height];
        }
        private void GenerateShipMap() {
            Ship sh = ParentMission.ShipTarget ?? throw new Exception("No ship target found in mission");
            int tx = (sh.Type.Length * 2) + 4;
            int ty = (sh.Type.Width * 2) + 4;

            // Set up all as void
            for (int x = 0; x < Width; x++) {
                for (int y = 0; y < Height; y++) Map[x, y] = TileType.Void;
            }

            // Background / corridors
            foreach (ShipRoomDesign r in sh.Type.Rooms) {
                if (r.Size != ShipEquipment.RoomSize.Weapon && r.Size != ShipEquipment.RoomSize.Engine) { // No border
                    for (int x = -3; x < (r.Width * 3) + 3; x++) {
                        for (int y = -3; y < (r.Height * 3) + 3; y++) {
                            Map[r.XPos * 3 + tx + x, r.YPos * 3 + ty + y] = TileType.Wall;
                        }
                    }
                }
            }
            // Rooms
            for (int n = 0; n < sh.Type.Rooms.Count; n++) {
                ShipRoomDesign r = sh.Type.Rooms[n];
                for (int x = 0; x < r.Width * 3; x++) {
                    for (int y = 0; y < r.Height * 3; y++) {
                        if (x == 0 || y == 0 || x == r.Width * 3 - 1 || y == r.Height * 3 - 1) Map[r.XPos * 3 + tx + x, r.YPos * 3 + ty + y] = TileType.Wall;
                        else if (r.Size == ShipEquipment.RoomSize.Weapon || r.Size == ShipEquipment.RoomSize.Engine) {
                            Map[r.XPos * 3 + tx + x, r.YPos * 3 + ty + y] = TileType.Machinery;
                        }
                        else {
                            Map[r.XPos * 3 + tx + x, r.YPos * 3 + ty + y] = TileType.Floor;
                        }
                    }
                }
            }

            // Corridor pass
            List<Point> lCorridor = new List<Point>();
            for (int x = 0; x < Width; x++) {
                for (int y = 0; y < Height; y++) {
                    if (Map[x, y] == TileType.Wall) {
                        if (x > 0 && Map[x - 1, y] != TileType.Wall) continue;
                        if (y > 0 && Map[x, y - 1] != TileType.Wall) continue;
                        if (x < Width - 1 && Map[x + 1, y] != TileType.Wall) continue;
                        if (y < Height - 1 && Map[x, y + 1] != TileType.Wall) continue;
                        if (x > 0 && y > 0 && Map[x - 1, y - 1] != TileType.Wall) continue;
                        if (x < Width - 1 && y > 0 && Map[x + 1, y - 1] != TileType.Wall) continue;
                        if (x < Width - 1 && y < Height - 1 && Map[x + 1, y + 1] != TileType.Wall) continue;
                        if (x > 0 && y < Height - 1 && Map[x - 1, y + 1] != TileType.Wall) continue;
                        lCorridor.Add(new Point(x, y));
                    }
                }
            }
            foreach (Point p in lCorridor) {
                Map[p.X, p.Y] = TileType.Floor;
            }

            // Insert doors
            Random rand = new Random(sh.Seed);
            for (int n = 0; n < sh.Type.Rooms.Count; n++) {
                ShipRoomDesign r = sh.Type.Rooms[n];
                if (r.Size == ShipEquipment.RoomSize.Weapon || r.Size == ShipEquipment.RoomSize.Engine) continue;
                // Place at random in the middle of a few walls
                int nDoors = 1;
                if (rand.NextDouble() < 0.7) nDoors++;
                if (rand.NextDouble() < 0.5) nDoors++;
                if (rand.NextDouble() < 0.3) nDoors++;
                bool[] DoorExists = new bool[4];
                for (int d = 0; d < nDoors; d++) {
                    int side = rand.Next(4); // LURD
                    if (DoorExists[side]) continue;
                    DoorExists[side] = true;
                    if (side == 0) {
                        int y = 1 + rand.Next(r.Height * 3 - 3);
                        Map[r.XPos * 3 + tx, r.YPos * 3 + ty + y] = TileType.DoorVertical;
                        Map[r.XPos * 3 + tx, r.YPos * 3 + ty + y + 1] = TileType.DoorVertical;
                    }
                    else if (side == 1) {
                        int x = 1 + rand.Next(r.Width * 3 - 3);
                        Map[r.XPos * 3 + tx + x, r.YPos * 3 + ty] = TileType.DoorHorizontal;
                        Map[r.XPos * 3 + tx + x + 1, r.YPos * 3 + ty] = TileType.DoorHorizontal;
                    }
                    else if (side == 2) {
                        int y = 1 + rand.Next(r.Height * 3 - 3);
                        Map[r.XPos * 3 + tx + r.Width * 3 - 1, r.YPos * 3 + ty + y] = TileType.DoorVertical;
                        Map[r.XPos * 3 + tx + r.Width * 3 - 1, r.YPos * 3 + ty + y + 1] = TileType.DoorVertical;
                    }
                    else if (side == 3) {
                        int x = 1 + rand.Next(r.Width * 3 - 3);
                        Map[r.XPos * 3 + tx + x, r.YPos * 3 + ty + r.Height * 3 - 1] = TileType.DoorHorizontal;
                        Map[r.XPos * 3 + tx + x + 1, r.YPos * 3 + ty + r.Height * 3 - 1] = TileType.DoorHorizontal;
                    }
                }
            }
            GenerateTransitionLocations();
            //GenerateHiddenTreasure(1);
            //GenerateTraps(1);
        }
        private void GenerateDungeonMap(bool bMines = false) {
            int niter = 0;
            RoomMap = new int[Width, Height];
            int nRooms = 0, MinRooms;

            do { // We may have to try this multiple times because the algo is tricky and stochastic.
                 // Clear the dungeon
                for (int x = 0; x < Width; x++) {
                    for (int y = 0; y < Height; y++) {
                        Map[x, y] = TileType.Wall;
                        RoomMap[x, y] = 0;
                    }
                }

                PlaceRandomDungeonEntranceAndExit();
                GenerateRandomTunnels();
                nRooms = BuildRooms(bMines);

                // Check that this map has enough rooms and redo if so (maximum number of tries)
                MinRooms = ((Width * Height) / (180 + niter++));
                if (bMines) MinRooms /= 2;
            } while (nRooms < MinRooms);

            CheckConnectivity(); // Added this here as mines are sometimes disconnected
            if (!bMines) GenerateDoors();
            GenerateHiddenTreasure(bMines ? 1 : 2);
            GenerateTraps(bMines ? 1 : 2);
        }
        private void GenerateCaveMap() {
            double fract = Const.AutomataCaveFract + (rand.NextDouble() / 50.0);
            if (ParentMission.GetLargestCreatureSize() > 2) fract -= (ParentMission.GetLargestCreatureSize() - 2) / 35.0;
            for (int x = 0; x < Width; x++) {
                for (int y = 0; y < Height; y++) {
                    if (x == 0 || y == 0 || x == Width - 1 || y == Height - 1) Map[x, y] = TileType.Wall;
                    else {
                        if (rand.NextDouble() > fract) Map[x, y] = TileType.Floor;
                        else Map[x, y] = TileType.Wall;
                    }
                }
            }
            for (int i = 0; i < Const.AutomataIterations; i++) RunAutomata();
            CheckConnectivity();
            GenerateTransitionLocations();
            GenerateHiddenTreasure(1);
            GenerateTraps(1);
        }
        private void GenerateSurfaceMap() {
            double fract = Const.AutomataSurfaceFract + (rand.NextDouble() / 50.0);
            for (int x = 0; x < Width; x++) {
                for (int y = 0; y < Height; y++) {
                    if (x == 0 || y == 0 || x == Width - 1 || y == Height - 1) Map[x, y] = TileType.Wall;
                    else {
                        if (rand.NextDouble() > fract) Map[x, y] = TileType.Floor;
                        else Map[x, y] = TileType.Wall;
                    }
                }
            }
            for (int i = 0; i < Const.AutomataIterations + 2; i++) RunAutomata();
            CheckConnectivity();
            GenerateTransitionLocations();
        }

        // Set up the items and locations in this map
        private void GenerateTransitionLocations() {
            bool bOK = true;
            do {
                StartX = rand.Next(Width - 8) + 4;
                StartY = rand.Next(Height - 8) + 4;
                bOK = (Map[StartX, StartY] == TileType.Floor && Map[StartX + 1, StartY] == TileType.Floor && Map[StartX + 1, StartY + 1] == TileType.Floor && Map[StartX, StartY + 1] == TileType.Floor);
            } while (!bOK);
            SetupEntryLocations();

            if (LevelID < ParentMission.LevelCount - 1) {
                int furthest = 0;
                int sx2, sy2;
                EndX = EndY = -1;
                for (int i = 0; i < 30; i++) {
                    bOK = true;
                    do {
                        sx2 = rand.Next(Width - 8) + 4;
                        sy2 = rand.Next(Height - 8) + 4;
                        bOK = (Map[sx2, sy2] == TileType.Floor && Map[sx2 + 1, sy2] == TileType.Floor && Map[sx2 + 1, sy2 + 1] == TileType.Floor && Map[sx2, sy2 + 1] == TileType.Floor);
                    } while (!bOK);
                    int d = Math.Abs(StartX - sx2) + Math.Abs(StartY - sy2);
                    if (d > furthest) {
                        EndX = sx2;
                        EndY = sy2;
                        furthest = d;
                    }
                }
                SetupExitLocations();
            }
        }
        private void SetupEntryLocations() {
            EntryLocations.Clear();
            if (StartX < 0 || StartY < 0) return;
            if (ParentMission.Type == Mission.MissionType.RepelBoarders) return; // No specific entry points for RepelBoarders missions
            EntryLocations.Add(new Point(StartX, StartY));
            EntryLocations.Add(new Point(StartX + 1, StartY));
            EntryLocations.Add(new Point(StartX, StartY + 1));
            EntryLocations.Add(new Point(StartX + 1, StartY + 1));
            Map[StartX, StartY] = Map[StartX + 1, StartY] = Map[StartX, StartY + 1] = Map[StartX + 1, StartY + 1] = TileType.Floor;
        }
        private void SetupExitLocations() {
            ExitLocations.Clear();
            if (EndX < 0 || EndY < 0) return;
            ExitLocations.Add(new Point(EndX, EndY));
            ExitLocations.Add(new Point(EndX + 1, EndY));
            ExitLocations.Add(new Point(EndX, EndY + 1));
            ExitLocations.Add(new Point(EndX + 1, EndY + 1));
            Map[EndX, EndY] = Map[EndX + 1, EndY] = Map[EndX, EndY + 1] = Map[EndX + 1, EndY + 1] = TileType.Floor;
        }
        private void GenerateHiddenTreasure(int scale) {
            int size = Width * Height;
            int nTreasure = (rand.Next(size * scale) * 2 + (size * scale)) / 1500;
            int ntries = 0;
            while (Items.Count < nTreasure && ++ntries < (nTreasure * 10 + 10)) {
                // Pick a random location
                int x = rand.Next(Width - 4) + 2;
                int y = rand.Next(Height - 4) + 2;
                Point pt = new Point(x, y);
                if (Map[x, y] != TileType.Floor) continue;
                if (Items.ContainsKey(pt)) continue;
                if (EntryLocations.Contains(pt)) continue;
                if (ExitLocations.Contains(pt)) continue;

                // Set up treasure & add to map
                Stash st = new Stash(ParentMission.Diff, pt, rand);
                st.Hide();
                if (st.Count > 0) Items.Add(pt, st);
            }
        }
        private void GenerateTraps(int scale) {
            int size = Width * Height;
            int nTraps = (rand.Next(size * scale) * 2 + (size * scale)) / 1500;
            int ntries = 0;
            while (Traps.Count < nTraps && ++ntries < (nTraps * 5 + 5)) {
                // Pick a random location
                int x = rand.Next(Width - 4) + 2;
                int y = rand.Next(Height - 4) + 2;
                Point pt = new Point(x, y);
                if (Map[x, y] != TileType.Floor) continue;
                if (Items.ContainsKey(pt)) continue;
                if (Traps.ContainsKey(pt)) continue;
                if (EntryLocations.Contains(pt)) continue;
                if (ExitLocations.Contains(pt)) continue;

                // Set up trap here
                Trap tr = new Trap(pt, ParentMission.Diff, rand);
                tr.Hide();
                Traps.Add(pt, tr);
            }
        }
        private void InsertGoalItem(MissionItem mi) {
            int r = rand.Next(100);

            if (r < 30) {
                // Add to boss if exists
                foreach (Creature cr in Creatures) {
                    if (cr.Type.IsBoss) {
                        cr.SetHasQuestItem();
                        return;
                    }
                }
            }
            if (r < 60) {
                // Add to random creature
                if (Creatures.Any()) {
                    Creature cr = Creatures.ElementAt(rand.Next(Creatures.Count()));
                    if (!cr.QuestItem) {
                        cr.SetHasQuestItem();
                        return;
                    }
                }

            }
            if (r < 80) {
                // Add to a stash if possible
                if (Items.Count > 0) {
                    List<Stash> lSt = new List<Stash>(Items.Values);
                    Stash st = lSt[rand.Next(lSt.Count)];
                    st.Add(mi);
                    return;
                }
            }
            // Hide it in new stash
            int ntries = 0;
            while (++ntries < 20) {
                // Pick a random location
                int x = rand.Next(Width - 4) + 2;
                int y = rand.Next(Height - 4) + 2;
                Point pt = new Point(x, y);
                if (Map[x, y] != TileType.Floor) continue;
                if (Items.ContainsKey(pt)) continue;
                if (EntryLocations.Contains(pt)) continue;
                if (ExitLocations.Contains(pt)) continue;

                // Set up treasure & add to map
                Stash st = new Stash(pt);
                st.Add(mi);
                st.Hide();
                Items.Add(pt, st);
                return;
            }
            throw new Exception("Could not find suitable location for MissionItem!");
        }

        // Add entities to the map
        public void AddSoldier(Soldier s) {
            if (ParentMission.Type == Mission.MissionType.RepelBoarders) {
                AddSoldierInRandomRoom(s);
                return;
            }
            foreach (Point pt in EntryLocations) {
                if (EntityMap[pt.X, pt.Y] == null) {
                    s.SetLocation(pt);
                    s.SetFacing(Utils.Direction.North);
                    if (!s.OnMission) s.ResetForBattle();
                    s.UpdateVisibility(this);
                    Entities.Add(s);
                    EntityMap[pt.X, pt.Y] = s;
                    if (!ParentMission.Soldiers.Contains(s)) ParentMission.Soldiers.Add(s);
                    CalculatePlayerVisibility();
                    s.OnMission = true;
                    return;
                }
            }
            throw new Exception("Cannot place soldier"); // Can't do it
        }
        public void AddSoldierAtExit(Soldier s) {
            foreach (Point pt in ExitLocations) {
                if (EntityMap[pt.X, pt.Y] == null) {
                    s.SetLocation(pt);
                    s.SetFacing(Utils.Direction.North);
                    if (!s.OnMission) s.ResetForBattle();
                    s.UpdateVisibility(this);
                    Entities.Add(s);
                    EntityMap[pt.X, pt.Y] = s;
                    if (!ParentMission.Soldiers.Contains(s)) ParentMission.Soldiers.Add(s);
                    CalculatePlayerVisibility();
                    s.OnMission = true;
                    return;
                }
            }
            throw new Exception("Cannot place soldier"); // Can't do it
        }
        public void AddSoldierAtCurrentLocation(Soldier s) {
            s.OnMission = true;
            s.UpdateVisibility(this);
            Entities.Add(s);
            EntityMap[s.X, s.Y] = s;
            ParentMission.Soldiers.Add(s);
            CalculatePlayerVisibility();
        }
        public void RemoveAllSoldiers() {
            List<Soldier> lSoldiers = new List<Soldier>(Soldiers);
            foreach (Soldier s in lSoldiers) {
                EntityMap[s.X, s.Y] = null;
                Entities.Remove(s);
            }
        }
        private void AddSoldierInRandomRoom(Soldier s) {
            s.OnMission = true;
            bool bOK = true;
            double bestscore = 0;
            int bx = -1, by = -1;
            for (int n = 0; n < 10; n++) {
                int x, y;
                do {
                    x = rand.Next(Width - 8) + 4;
                    y = rand.Next(Height - 8) + 4;
                    bOK = (Map[x, y] == TileType.Floor) && (EntityMap[x, y] == null);
                } while (!bOK);
                double score = 1000.0;
                foreach (Creature cr in Creatures) {
                    if (cr.CanSee(x, y)) {
                        score -= cr.Level / cr.RangeTo(x, y);
                    }
                }
                if (score > bestscore) {
                    bestscore = score;
                    bx = x;
                    by = y;
                }
            }
            if (bx == -1 || by == -1) throw new Exception("Could not find legitimate starting location for Soldier " + s.Name);
            Point pt = new Point(bx, by);
            s.SetLocation(pt);
            s.SetFacing(Utils.Direction.North);
            s.ResetForBattle();
            s.UpdateVisibility(this);
            Entities.Add(s);
            EntityMap[pt.X, pt.Y] = s;
            ParentMission.Soldiers.Add(s);
            CalculatePlayerVisibility();
        }
        private void AddCreature(Creature cr) {
            cr.ResetForBattle();
            AddCreatureWithoutReset(cr);
        }
        private void AddCreatureWithoutReset(Creature cr) {
            cr.UpdateVisibility(this);
            if (Entities.Contains(cr)) throw new Exception("Attempting to add duplicate creature");
            Entities.Add(cr);
            for (int y = cr.Y; y < cr.Y + cr.Size; y++) {
                for (int x = cr.X; x < cr.X + cr.Size; x++) {
                    if (EntityMap[x, y] != null) throw new Exception("Attempting to add creature on top of another");
                    EntityMap[x, y] = cr;
                }
            }
        }
        private bool CreatureCanGo(Creature cr, int x, int y) {
            for (int cy = y; cy < y + cr.Size; cy++) {
                if (cy < 0 || cy >= Height) return false;
                for (int cx = x; cx < x + cr.Size; cx++) {
                    if (cx < 0 || cx >= Width) return false;
                    if (Map[cx, cy] != TileType.Floor || EntityMap[cx, cy] != null) return false;
                }
            }
            return true;
        }

        // Adding the (enemy) creatures to the map
        private void GenerateCreatures() {
            int nFloorTiles = 0;
            for (int x = 0; x < Width; x++) {
                for (int y = 0; y < Height; y++) {
                    if (Map[x, y] == TileType.Floor) nFloorTiles++;
                }
            }
            int nCreatures = (int)(Math.Pow(nFloorTiles, Const.CreatureCountExponent) / (Const.CreatureCountScale - (ParentMission.Soldiers.Count * 2)));
            if (ParentMission.Type == Mission.MissionType.Surface) nCreatures = (nCreatures * 4) / 5;
            if (nCreatures < 3) nCreatures = 3;
            int nLeft = nCreatures;

            // Who are we fighting against?
            CreatureGroup cg = ParentMission.PrimaryEnemy;
            Race? ra = ParentMission.RacialOpponent;
            if (ra is not null && cg is null) cg = GenerateCreatureGroupForRacialOpponent(ra);
            int niter = 0;

            // Add all creatures
            while (nLeft > 0 && niter < 1000) {
                List<Creature> cGroup = new List<Creature>();
                int iGroupSize = 1 + rand.Next(2);
                if (nCreatures > 15) iGroupSize += rand.Next(nCreatures / 15);
                if (ParentMission.Soldiers.Count > 1) iGroupSize += rand.Next(ParentMission.Soldiers.Count);
                if (Entities.Count == 0 && cg.HasBoss && (nCreatures > 8 || ParentMission.Goal == Mission.MissionGoal.KillBoss) && LevelID == ParentMission.LevelCount - 1) {
                    Creature? cr = cg.GenerateRandomBoss(ra, Diff, this);
                    if (cr is not null) {
                        if (!PlaceFirstCreatureInGroup(cr, true)) { niter++; continue; }
                        iGroupSize++;
                        cGroup.Add(cr);
                    }
                    else if (ParentMission.Goal == Mission.MissionGoal.KillBoss) throw new Exception("Couldn't generate required boss for assassination quest");
                }
                if (iGroupSize > nLeft) iGroupSize = nLeft;
                for (int i = cGroup.Count; i < iGroupSize; i++) {
                    Creature? cr = cg.GenerateRandomCreature(ra, Diff, this);
                    if (cGroup.Count == 0) {
                        if (PlaceFirstCreatureInGroup(cr, ParentMission.Type == Mission.MissionType.Surface)) cGroup.Add(cr);
                        else break;
                    }
                    else {
                        if (PlaceCreatureNearGroup(cr, cGroup, iGroupSize)) cGroup.Add(cr);
                    }
                }
                if (cGroup.Count == iGroupSize || (niter > 10 && cGroup.Count > 0)) { // We managed to place the entire group, or we've tried a few times and failed but at least we placed something!
                    nLeft -= cGroup.Count;
                    foreach (Creature cr in cGroup) AddCreature(cr);
                }
                else niter++;
            }
            if (Entities.Count == 0) {
                throw new Exception("Couldn't place creatures!");
            }
        }
        private bool PlaceFirstCreatureInGroup(Creature cr, bool bFarFromEntryPoint) {
            if (cr == null) return false;
            int nTries = 2 + (bFarFromEntryPoint ? 2 : 0);
            int MinDist = Const.MinimumCreatureDistanceFromStartLocation + (bFarFromEntryPoint ? (Width + Height) / 4 : 0);
            double BestScore = -1;
            Point best = Point.Empty;
            for (int n = 0; n < nTries; n++) {
                bool bOK = true;
                double score = 0.0;
                int x, y, niter = 0;
                do {
                    score = 0.0;
                    int dist = 0;
                    Point ptStart = Point.Empty;
                    if (EntryLocations.Any()) ptStart = EntryLocations.First<Point>();
                    do {
                        x = rand.Next(Width - 5) + 2;
                        y = rand.Next(Height - 5) + 2;
                        if (ptStart == Point.Empty) dist = MinDist + 1;
                        else dist = Math.Abs(x - ptStart.X) + Math.Abs(y - ptStart.Y);
                    } while (!CreatureCanGo(cr, x, y) || (dist < MinDist && CanSee(ptStart, new Point(x, y))));
                    // Attempt to put all creatures far from the EntryLocations
                    score += dist;

                    // Large creatures in cave/mine levels should not be blocked (but this doesn't achieve that. Could still have places where they're blockable.)
                    //if (cr.Size > 1) {
                    //  int sdist = ShortestPathLength(cr, new Point(x, y), ptStart, 50);
                    //  // What is sdist if creature can't get to entrance?
                    //}

                    // Avoid other groups of creatures
                    bOK = true;
                    foreach (IEntity e in Entities) {
                        int d = Math.Abs(e.X - x) + Math.Abs(e.Y - y);
                        if (d < 6) { bOK = false; niter++; break; }
                        score += (d / 4);
                    }
                } while (!bOK && niter < 100);
                if (bOK && (score > BestScore || best == Point.Empty)) {
                    BestScore = score;
                    best = new Point(x, y);
                }
            }
            if (best == Point.Empty) return false;
            cr.SetLocation(best);
            return true;
        }
        private bool PlaceCreatureNearGroup(Creature cr, List<Creature> cGroup, int iGroupSize) {
            int x, y, tries = 0;
            if (cGroup.Count == 0) return false;
            Creature centre = cGroup[0];
            bool bOK = false;
            do {
                bOK = true;
                if (++tries > 150) return false; // Failed
                x = centre.X + rand.Next(iGroupSize * 2 + 3) - iGroupSize;
                y = centre.Y + rand.Next(iGroupSize * 2 + 3) - iGroupSize;
                if (!CreatureCanGo(cr, x, y)) bOK = false;
                else if (RoomMap != null && RoomMap[x, y] != RoomMap[centre.X, centre.Y]) bOK = false; // Same room as centre of group
                else {
                    foreach (Creature c2 in cGroup) {
                        bool xClash = false, yClash = false;
                        if (x <= c2.X && c2.X - x < cr.Type.Size) xClash = true;
                        if (y <= c2.Y && c2.Y - y < cr.Type.Size) yClash = true;
                        if (x >= c2.X && x - c2.X < c2.Type.Size) xClash = true;
                        if (y >= c2.Y && y - c2.Y < c2.Type.Size) yClash = true;
                        if (xClash && yClash) { bOK = false; break; }
                    }
                }
            } while (!bOK);
            cr.SetLocation(new Point(x, y));
            return true;
        }
        private CreatureGroup GenerateCreatureGroupForRacialOpponent(Race ra) {
            List<CreatureGroup> cgList = new List<CreatureGroup>();
            foreach (CreatureGroup cg in StaticData.CreatureGroups) {
                if (cg.RaceSpecific) {
                    if (ra.Relations <= cg.MaxRelations) {
                        cgList.Add(cg);
                    }
                }
            }
            if (cgList.Count == 0) throw new Exception("Found no suitable CreatureGroups for racial opponent : " + ra.Name);
            if (cgList.Count == 1) return cgList[0];
            return cgList[rand.Next(cgList.Count)];
        }

        #region Generate Caves
        private void RunAutomata() {
            TileType[,] OldMap = new TileType[Width, Height];
            for (int y = 0; y < Height; y++) {
                for (int x = 0; x < Width; x++) OldMap[x, y] = Map[x, y];
            }

            // Run a B678/345678 algorithm
            for (int y = 0; y < Height; y++) {
                for (int x = 0; x < Width; x++) {
                    int neighbours = 0;
                    for (int a = -1; a <= 1; a++) {
                        if (a + x >= 0 && a + x < Width) {
                            for (int b = -1; b <= 1; b++) {
                                if (b + y >= 0 && b + y < Height) {
                                    if (a != 0 || b != 0) {
                                        if (OldMap[a + x, b + y] == TileType.Wall) neighbours++;
                                    }
                                }
                                else neighbours = 8;
                            }
                        }
                        else neighbours = 8;
                    }
                    // Determine next state for this cell
                    if (OldMap[x, y] == TileType.Wall) {
                        if (neighbours < 3) Map[x, y] = TileType.Floor;
                    }
                    else {
                        if (neighbours > 5) Map[x, y] = TileType.Wall;
                    }
                }
            }
        }
        private void CheckConnectivity() {
            // Make sure everything is connected up

            // Set up a flood fill map to count the regions
            int[,] FloodFill = new int[Width, Height];
            for (int y = 0; y < Height; y++) {
                for (int x = 0; x < Width; x++) FloodFill[x, y] = 0;
            }

            // FloodFill the map and count sub-regions
            int MaxRegion = 0;
            for (int y = 0; y < Height; y++) {
                for (int x = 0; x < Width; x++) {
                    if (Map[x, y] == TileType.Floor && FloodFill[x, y] == 0) {
                        FloodFillRegion(x, y, FloodFill, MaxRegion + 1);
                        MaxRegion++;
                    }
                }
            }
            // Only one region, so we're done
            if (MaxRegion == 1) return;

            // We had multiple regions, so we need to connect them up.
            // Start by getting lists of cells in each region
            Dictionary<int, List<Tuple<int, int>>> dRegions = new Dictionary<int, List<Tuple<int, int>>>();
            for (int y = 0; y < Height; y++) {
                for (int x = 0; x < Width; x++) {
                    if (FloodFill[x, y] > 0) {
                        if (!dRegions.ContainsKey(FloodFill[x, y])) dRegions.Add(FloodFill[x, y], new List<Tuple<int, int>>());
                        dRegions[FloodFill[x, y]].Add(new Tuple<int, int>(x, y));
                    }
                }
            }

            // Loop through all regions > 1 and make sure they're connected to region 1
            for (int region = 2; region <= MaxRegion; region++) {
                if (!dRegions.ContainsKey(region)) {
                    throw new Exception("Unknown region " + region);
                }
                if (dRegions[region].Count == 0) {
                    //throw new Exception("Empty region, for some reason!");
                    continue;
                }
                int x = (dRegions[region])[0].Item1;
                int y = (dRegions[region])[0].Item2;
                if (FloodFill[x, y] == 1) continue; // it might have been connected since the first iteration
                                                    // Tiny region, so just fill it in
                if (dRegions[region].Count == 1) {
                    Map[x, y] = TileType.Wall;
                    FloodFill[x, y] = 0;
                    continue;
                }
                // Find two close cells in R1 and Rn
                int best = 10000;
                Tuple<int, int>? b1 = null, b2 = null;
                for (int c = 0; c < 250; c++) {
                    Tuple<int, int> c1 = (dRegions[1])[rand.Next(dRegions[1].Count)];
                    int x1 = c1.Item1;
                    int y1 = c1.Item2;
                    Tuple<int, int> c2 = (dRegions[region])[rand.Next(dRegions[region].Count)];
                    int x2 = c2.Item1;
                    int y2 = c2.Item2;
                    int d = Math.Abs(x1 - x2) + Math.Abs(y1 - y2);
                    if (c == 0 || d < best) {
                        best = d;
                        b1 = c1;
                        b2 = c2;
                    }
                    if (best < 4) break;
                }
                JoinCells(b1, b2, FloodFill, dRegions);
            }
        }
        private void FloodFillRegion(int x, int y, int[,] FloodFill, int region) {
            // Flood fill a connected region
            FloodFill[x, y] = region;
            if (x > 0 && Map[x - 1, y] == TileType.Floor && FloodFill[x - 1, y] == 0) FloodFillRegion(x - 1, y, FloodFill, region);
            if (x < Width - 1 && Map[x + 1, y] == TileType.Floor && FloodFill[x + 1, y] == 0) FloodFillRegion(x + 1, y, FloodFill, region);
            if (y > 0 && Map[x, y - 1] == TileType.Floor && FloodFill[x, y - 1] == 0) FloodFillRegion(x, y - 1, FloodFill, region);
            if (y < Height - 1 && Map[x, y + 1] == TileType.Floor && FloodFill[x, y + 1] == 0) FloodFillRegion(x, y + 1, FloodFill, region);
        }
        private void JoinCells(Tuple<int, int>? b1, Tuple<int, int>? b2, int[,] FloodFill, Dictionary<int, List<Tuple<int, int>>> dRegions) {
            if (b1 is null || b2 is null) return;
            // Join two cells and therefore join the relevant regions containing them
            int rtgt = FloodFill[b2.Item1, b2.Item2];
            int sx = b1.Item1;
            int sy = b1.Item2;
            int tx = b2.Item1;
            int ty = b2.Item2;
            int x = sx, y = sy;
            bool bOK = false;
            do {
                int oldy = y;
                if (tx > x) {
                    x++;
                    y = sy + (int)(0.5 + (x - sx) * (ty - sy) / (tx - sx));
                }
                else if (tx < x) {
                    x--;
                    y = sy + (int)(0.5 + (x - sx) * (ty - sy) / (tx - sx));
                    if (oldy != y) {
                        for (int dy = Math.Min(oldy, y); dy <= Math.Max(oldy, y); dy++) {
                            if (FloodFill[x, dy] == rtgt) bOK = true;
                            TunnelTo(x, dy, FloodFill, dRegions);
                        }
                    }
                }
                else y = ty;
                for (int dy = Math.Min(oldy, y); dy <= Math.Max(oldy, y); dy++) {
                    if (FloodFill[x, dy] == rtgt) bOK = true;
                    TunnelTo(x, dy, FloodFill, dRegions);
                }
            } while (!bOK && (x != tx || y != ty));
        }
        private void TunnelTo(int x, int y, int[,] FloodFill, Dictionary<int, List<Tuple<int, int>>> dRegions) {
            // Draw a tunnel in a cave map. If we break through to a new region then fill it with region 1
            Map[x, y] = TileType.Floor;
            int r = FloodFill[x, y];
            if (r > 1) {
                for (int i = 0; i < dRegions[r].Count; i++) {
                    FloodFill[dRegions[r][i].Item1, dRegions[r][i].Item2] = 1;
                    dRegions[1].Add(dRegions[r][i]);
                }
                dRegions[r].Clear();
            }
        }
        #endregion // Generate Caves

        #region Generate Dungeon
        private void PlaceRandomDungeonEntranceAndExit() {
            StartX = rand.Next(Width - 8) + 4;
            StartY = rand.Next(Height - 8) + 4;
            SetupEntryLocations();

            if (LevelID < ParentMission.LevelCount - 1) { // Add stairs down!
                int furthest = 0;
                EndX = EndY = -1;
                for (int i = 0; i < 30; i++) {
                    int sx2 = rand.Next(Width - 8) + 4;
                    int sy2 = rand.Next(Height - 8) + 4;
                    int d = Math.Abs(StartX - sx2) + Math.Abs(StartY - sy2);
                    if (d > furthest) {
                        EndX = sx2;
                        EndY = sy2;
                        furthest = d;
                    }
                }
                SetupExitLocations();
            }
        }
        private void GenerateRandomTunnels() {
            int nTunnels = 0;

            // Start a tunnel from the start location
            InitialiseTunnel(StartX, StartY, ref nTunnels);

            // Add loads more tunnels at random
            while (nTunnels < (Width * Height / 5)) {
                bool bOK = true;
                int x, y;
                do {
                    bOK = true;
                    x = (int)(rand.NextDouble() * (Width - 3)) + 1;
                    y = (int)(rand.NextDouble() * (Height - 3)) + 1;
                    if (Map[x, y] != TileType.Floor || Map[x + 1, y] != TileType.Floor || Map[x, y + 1] != TileType.Floor || Map[x + 1, y + 1] != TileType.Floor) bOK = false;
                } while (!bOK);
                InitialiseTunnel(x, y, ref nTunnels);
            }

            // Also start a tunnel from the stairs down, if there are any and they aren't already connected
            if (LevelID == ParentMission.LevelCount - 1) return;
            if (Map[EndX - 1, EndY] == TileType.Floor || Map[EndX - 1, EndY + 1] == TileType.Floor) return; // Left passage
            if (Map[EndX, EndY - 1] == TileType.Floor || Map[EndX + 1, EndY - 1] == TileType.Floor) return; // Up passage
            if (Map[EndX + 2, EndY] == TileType.Floor || Map[EndX + 2, EndY + 1] == TileType.Floor) return; // Right passage
            if (Map[EndX, EndY + 2] == TileType.Floor || Map[EndX + 1, EndY + 2] == TileType.Floor) return; // Down passage
            InitialiseTunnel(EndX, EndY, ref nTunnels);
        }
        private void InitialiseTunnel(int sx, int sy, ref int nTunnels) {
            // Start a tunnel in a random direction from the given location
            if (rand.NextDouble() < 0.5) {
                // Horizontal
                if (rand.NextDouble() * Width > sx) DigTunnel(sx + 1, sy, 1, 0, 2, ref nTunnels); // Right
                else DigTunnel(sx, sy, -1, 0, 2, ref nTunnels); // Left
            }
            else {
                // Vertical
                if (rand.NextDouble() * Height > sy) DigTunnel(sx, sy + 1, 0, 1, 2, ref nTunnels); // Down
                else DigTunnel(sx, sy, 0, -1, 2, ref nTunnels); // Up
            }
        }
        private void DigTunnel(int sx, int sy, int dx, int dy, int tw, ref int nTunnels) {
            // Dig a tunnel in the given direction, starting from the given point
            int tlen = 0;

            // Calculate vector from sx,sy to other cells at tunnel front (sx,sy is always TLC)
            int tx = 0, ty = 0;
            if (dx == 0) tx = 1;
            if (dy == 0) ty = 1;

            // Check if we can tunnel here
            int nextx = sx + dx;
            int nexty = sy + dy;
            if (Map[nextx, nexty] == TileType.Floor || nextx == 0 || nextx + (tx * (tw - 1)) >= Width - 1 || nexty == 0 || nexty + (ty * (tw - 1)) >= Height - 1) return;

            // Check if we've been asked to start a tunnel (because of a junction, probably) in a direction that has since become impossible
            if (Map[nextx + dx, nexty + dy] == TileType.Wall && Map[nextx + dx + (tx * (tw - 1)), nexty + dy + (ty * (tw - 1))] == TileType.Wall) {
                if (Map[nextx + dx - tx, nexty + dy - ty] == TileType.Floor) return;
                if (Map[nextx + dx + (tx * tw), nexty + dy + (ty * tw)] == TileType.Floor) return;
            }

            // Dig the tunnel
            bool bOK = false, bCollision = false;
            int rstop = 0;
            do {
                bOK = true;
                sx += dx;
                sy += dy;
                nTunnels += tw;
                tlen++;

                // Check tunnel validity here (are we about to run over another tunnel on a tangent?)
                // Only do this if we're not about to join with another tunnel
                if (Map[sx + dx, sy + dy] == TileType.Wall && Map[sx + dx + tx, sy + dy + ty] == TileType.Wall) {
                    if (Map[sx + dx - tx, sy + dy - ty] == TileType.Floor) break;
                    if (Map[sx + dx + (tx * tw), sy + dy + (ty * tw)] == TileType.Floor) break;
                }

                // Set the tunnel
                for (int i = 0; i < tw; i++) {
                    // If the square we are tunneling through is already a tunnel, and it's of a higher tunnel ID
                    // Then that guarantees that it is already linked up, so we're therefore also linked
                    if (Map[sx + (tx * i), sy + (ty * i)] == TileType.Wall) Map[sx + (tx * i), sy + (ty * i)] = TileType.Floor;
                    RoomMap[sx + (tx * i), sy + (ty * i)] = -1;
                }

                // Iterate
                nextx = sx + dx;
                nexty = sy + dy;
                rstop = (int)(rand.NextDouble() * (12 + tw - tlen));  // 12 = DUNGEON_TUNNEL_LENGTH
                if (nTunnels < 3) rstop = 100; //First tunnel should always exist!
                bCollision = false;
                if (Map[nextx, nexty] == TileType.Floor) bCollision = true;
                if (tw == 2 && Map[nextx + tx, nexty + ty] == TileType.Floor) bCollision = true;
            } while (bOK && !bCollision && nextx > 0 && nextx < (Width - 1) && nexty > 0 && nexty < (Height - 1) && rstop >= 1);
            if (bCollision) return;

            // Junction
            do {
                bOK = true;
                int junc = rand.Next(100);
                if (junc < 30) { // CornerL
                    bOK = TunnelLeftTurn(sx, sy, dx, dy, tw, ref nTunnels);
                }
                else if (junc < 60) { // CornerR
                    bOK = TunnelRightTurn(sx, sy, dx, dy, tw, ref nTunnels);
                }
                else if (junc < 68) { // T-Junction LR
                    TunnelLeftTurn(sx, sy, dx, dy, tw, ref nTunnels);
                    TunnelRightTurn(sx, sy, dx, dy, tw, ref nTunnels);
                }
                else if (junc < 76) { // T-Junction LS
                    if (sx + dx >= Width - 1 || sx + dx < 1 || sy + dy >= Height - 1 || sy + dy < 1) bOK = false;
                    else {
                        TunnelLeftTurn(sx, sy, dx, dy, tw, ref nTunnels);
                        DigTunnel(sx, sy, dx, dy, GetRandomTunnelWidth(), ref nTunnels);
                    }
                }
                else if (junc < 84) { // T-Junction RS
                    if (sx + dx >= Width - 1 || sx + dx < 1 || sy + dy >= Height - 1 || sy + dy < 1) bOK = false;
                    else {
                        TunnelRightTurn(sx, sy, dx, dy, tw, ref nTunnels);
                        DigTunnel(sx, sy, dx, dy, GetRandomTunnelWidth(), ref nTunnels);
                    }
                }
                else if (junc < 92) { // Crossroads
                    TunnelLeftTurn(sx, sy, dx, dy, tw, ref nTunnels);
                    TunnelRightTurn(sx, sy, dx, dy, tw, ref nTunnels);
                    DigTunnel(sx, sy, dx, dy, GetRandomTunnelWidth(), ref nTunnels);
                }
                else { // Dead end
                    if (nTunnels < (Width * Height / 20)) bOK = false;
                }
            } while (!bOK);
        }
        private bool TunnelLeftTurn(int sx, int sy, int dx, int dy, int tw, ref int nTunnels) {
            int newtw = GetRandomTunnelWidth();
            if (dx == -1) { // Dir=Left
                if (sy + newtw + 4 > Height) return false;
                DigTunnel(sx, sy + tw - 1, 0, 1, newtw, ref nTunnels);
            }
            if (dx == 1) { // Dir=Right
                if (sy < 3 + newtw) return false;
                DigTunnel(sx + 1 - newtw, sy, 0, -1, newtw, ref nTunnels);
            }
            if (dy == -1) { // Dir=Up
                if (sx < 3 + newtw) return false;
                DigTunnel(sx, sy, -1, 0, newtw, ref nTunnels);
            }
            if (dy == 1) { // Dir=Down
                if (sx + newtw + 4 > Width) return false;
                DigTunnel(sx + tw - 1, sy + 1 - newtw, 1, 0, newtw, ref nTunnels);
            }
            return true;
        }
        private bool TunnelRightTurn(int sx, int sy, int dx, int dy, int tw, ref int nTunnels) {
            int newtw = GetRandomTunnelWidth();
            if (dx == -1) { // Dir=Left
                if (sy < 3 + newtw) return false;
                DigTunnel(sx, sy, 0, -1, newtw, ref nTunnels);
            }
            if (dx == 1) { // Dir=Right
                if (sy + newtw + 4 > Height) return false;
                DigTunnel(sx + 1 - newtw, sy + tw - 1, 0, 1, newtw, ref nTunnels);
            }
            if (dy == -1) { // Dir=Up
                if (sx + newtw + 4 > Width) return false;
                DigTunnel(sx + tw - 1, sy, 1, 0, newtw, ref nTunnels);
            }
            if (dy == 1) { // Dir=Down
                if (sx < 3 + newtw) return false;
                DigTunnel(sx, sy + 1 - newtw, -1, 0, newtw, ref nTunnels);
            }
            return true;
        }
        private int GetRandomTunnelWidth() {
            if (ParentMission.GetLargestCreatureSize() > 1) return ParentMission.GetLargestCreatureSize();
            int r = rand.Next(100);
            if (r < 86) return 2;
            if (r < 92) return 3;
            return 1;
        }
        private int BuildRooms(bool bMines) {
            // Attempt to create lots of rooms. As many as possible. Fewer if mines.
            bool bOK = false;
            int nRooms = 0;
            do {
                int h = 0, w = 0, x = 0, y = 0;
                int MaxTries = (bMines ? 50 - (nRooms * 5) : 150);
                for (int i = 0; i < MaxTries; i++) { // Attempt to place a new room a number of times
                    bOK = false;
                    // Generate a random room size
                    h = (int)(4.0 + (rand.NextDouble() * 3.0) + (rand.NextDouble() * 3.0) + (rand.NextDouble() * 2.0));
                    w = h + (int)((rand.NextDouble() * 3.0) + (rand.NextDouble() * 2.0) - (rand.NextDouble() * 3.0) - (rand.NextDouble() * 2.0));
                    if (w < 4) w = 4;
                    // Place this room somewhere where it's guaranteed not to overlap the edge of the map.
                    x = (int)(rand.NextDouble() * (Width - (w + 2))) + 1;
                    y = (int)(rand.NextDouble() * (Height - (h + 2))) + 1;
                    // Check room corner wall spaces to make sure we don't get ugly rooms brushing passages at corners, parallel passages etc.
                    if (Map[x - 1, y - 1] == TileType.Floor) continue;
                    if (Map[x + w, y - 1] == TileType.Floor) continue;
                    if (Map[x - 1, y + h] == TileType.Floor) continue;
                    if (Map[x + w, y + h] == TileType.Floor) continue;

                    // Check if this room clashes with another, and that it has a tunnel in it
                    bool bTunnel = false;
                    bool bBadLocation = false;
                    for (int ty = y - 1; ty <= y + h; ty++) {
                        for (int tx = x - 1; tx <= x + w; tx++) {
                            Point pt = new Point(tx, ty);
                            if (EntryLocations.Contains(pt) || ExitLocations.Contains(pt) || RoomMap[tx, ty] > 0 || (Map[tx, ty] != TileType.Wall && Map[tx, ty] != TileType.Floor)) {
                                bBadLocation = true;
                                break;
                            }
                            if (Map[tx, ty] == TileType.Floor) bTunnel = true; // This isn't a room therefore it must be a passage
                        }
                        if (bBadLocation) break;
                    }
                    bOK = (bTunnel & !bBadLocation);
                    if (bOK) break;
                }

                // Build this room
                if (bOK) {
                    nRooms++;
                    for (int ty = y - 1; ty <= y + h; ty++) {
                        for (int tx = x - 1; tx <= x + w; tx++) {
                            if (ty == y - 1 || ty == y + h || tx == x - 1 || tx == x + w) {
                                RoomMap[tx, ty] = -1;
                            }
                            else {
                                RoomMap[tx, ty] = nRooms;
                                Map[tx, ty] = TileType.Floor;
                            }
                        }
                    }
                }

            } while (bOK);
            return nRooms;
        }
        private void GenerateDoors() {
            for (int y = 1; y < Height - 1; y++) {
                for (int x = 1; x < Width - 1; x++) {
                    if (RoomMap[x, y] > 0) {
                        if (Map[x - 1, y] == TileType.Floor && RoomMap[x - 1, y] < 0) Map[x - 1, y] = TileType.DoorVertical;
                        if (Map[x + 1, y] == TileType.Floor && RoomMap[x + 1, y] < 0) Map[x + 1, y] = TileType.DoorVertical;
                        if (Map[x, y - 1] == TileType.Floor && RoomMap[x, y - 1] < 0) Map[x, y - 1] = TileType.DoorHorizontal;
                        if (Map[x, y + 1] == TileType.Floor && RoomMap[x, y + 1] < 0) Map[x, y + 1] = TileType.DoorHorizontal;
                    }
                }
            }

            // Remove all doors that are >2 in width (and any remaining stragglers that are door fragments)
            for (int y = 1; y < Height - 1; y++) {
                for (int x = 1; x < Width - 1; x++) {
                    if (Map[x, y] == TileType.DoorHorizontal) {
                        if (Map[x - 1, y] == TileType.DoorHorizontal && Map[x + 1, y] == TileType.DoorHorizontal) {
                            Map[x - 1, y] = Map[x, y] = Map[x + 1, y] = TileType.Floor;
                        }
                        else if (Map[x - 1, y] == TileType.Floor || Map[x + 1, y] == TileType.Floor) Map[x, y] = TileType.Floor;
                    }
                    else if (Map[x, y] == TileType.DoorVertical) {
                        if (Map[x, y - 1] == TileType.DoorVertical && Map[x, y + 1] == TileType.DoorVertical) {
                            Map[x, y - 1] = Map[x, y] = Map[x, y + 1] = TileType.Floor;
                        }
                        else if (Map[x, y - 1] == TileType.Floor || Map[x, y + 1] == TileType.Floor) Map[x, y] = TileType.Floor;
                    }
                }
            }

            // Make some doors secret and remove some others
            for (int y = 1; y < Height - 1; y++) {
                for (int x = 1; x < Width - 1; x++) {
                    if (Map[x, y] == TileType.DoorHorizontal) {
                        if (Map[x - 1, y] == TileType.Floor) Map[x, y] = TileType.Floor;
                        else if (Map[x - 1, y] == TileType.SecretDoorHorizontal) Map[x, y] = TileType.SecretDoorHorizontal;
                        else if (Map[x - 1, y] == TileType.Wall) {
                            int sd = rand.Next(100);
                            // Create a secret door, but only for doors of width 1 or 2 and only occasionally
                            if (sd < 12 && (Map[x - 1, y] == TileType.Wall || Map[x + 1, y] == TileType.Wall)) Map[x, y] = TileType.SecretDoorHorizontal;
                            else if (sd < 35) Map[x, y] = TileType.Floor;
                        }
                    }
                    else if (Map[x, y] == TileType.DoorVertical) {
                        if (Map[x, y - 1] == TileType.Floor) Map[x, y] = TileType.Floor;
                        else if (Map[x, y - 1] == TileType.SecretDoorVertical) Map[x, y] = TileType.SecretDoorVertical;
                        else if (Map[x, y - 1] == TileType.Wall) {
                            int sd = rand.Next(100);
                            // Create a secret door, but only for doors of width 1 or 2 and only occasionally
                            if (sd < 12 && (Map[x - 1, y] == TileType.Wall || Map[x + 1, y] == TileType.Wall)) Map[x, y] = TileType.SecretDoorVertical;
                            else if (sd < 35) Map[x, y] = TileType.Floor;
                        }
                    }
                }
            }

        }
        #endregion // Generate Dungeon

        #endregion // Map Generation

        // Actions
        public void MoveEntityTo(IEntity en, Point pt) {
            if (!Entities.Contains(en)) throw new Exception("Attempting to place an entity that isn't in this level!");
            for (int y = en.Y; y < en.Y + en.Size; y++) {
                for (int x = en.X; x < en.X + en.Size; x++) {
                    if (EntityMap[x, y] == null) throw new Exception("Attempting to move an entity with a corrupt location map");
                    EntityMap[x, y] = null;
                }
            }
            for (int y = pt.Y; y < pt.Y + en.Size; y++) {
                for (int x = pt.X; x < pt.X + en.Size; x++) {
                    if (EntityMap[x, y] != null) throw new Exception("Attempting to place an entity on top of another!");
                    EntityMap[x, y] = en;
                }
            }
            en.SetLocation(pt);
            en.UpdateVisibility(this);
            if (en is Soldier s) {
                CalculatePlayerVisibility();
                // Check if any creatures want to attack the soldier
                foreach (Creature cr in Creatures) {
                    if (cr.IsAlert && cr.CurrentTarget == null && cr.CanSee(en) && cr.RangeTo(en) <= cr.SoldierVisibilityRange(s)) {
                        if (cr.Investigate == Point.Empty || cr.RangeTo(en) + (rand.NextDouble() * 3.0) < cr.RangeTo(cr.Investigate) || rand.NextDouble() > 0.9) {
                            cr.SetTarget(en);
                        }
                    }
                }
            }
        }
        public void OpenDoor(int xpos, int ypos) {
            if (Map[xpos, ypos] == TileType.DoorHorizontal) {
                Map[xpos, ypos] = TileType.OpenDoorHorizontal;
                int dx = xpos;
                while (dx - 1 > 0 && Map[dx - 1, ypos] == TileType.DoorHorizontal) { dx--; Map[dx, ypos] = TileType.OpenDoorHorizontal; }
                while (dx + 1 < Width - 1 && Map[dx + 1, ypos] == TileType.DoorHorizontal) { dx++; Map[dx, ypos] = TileType.OpenDoorHorizontal; }
            }
            else if (Map[xpos, ypos] == TileType.DoorVertical) {
                Map[xpos, ypos] = TileType.OpenDoorVertical;
                int dy = ypos;
                while (dy - 1 > 0 && Map[xpos, dy - 1] == TileType.DoorVertical) { dy--; Map[xpos, dy] = TileType.OpenDoorVertical; }
                while (dy + 1 < Height - 1 && Map[xpos, dy + 1] == TileType.DoorVertical) { dy++; Map[xpos, dy] = TileType.OpenDoorVertical; }
            }
            else throw new Exception("Attempting to open a thing that isn't a closed door!");
            // Update all visibility
            foreach (IEntity en in Entities) {
                en.UpdateVisibility(this);
            }
        }
        public bool CloseDoor(int xpos, int ypos) {
            if (Map[xpos, ypos] == TileType.OpenDoorHorizontal) {
                if (EntityMap[xpos, ypos] != null) return false;
                int dx = xpos;
                while (dx - 1 > 0 && Map[dx - 1, ypos] == TileType.OpenDoorHorizontal) { dx--; if (EntityMap[dx, ypos] != null) return false; }
                dx = xpos;
                while (dx + 1 < Width - 1 && Map[dx + 1, ypos] == TileType.OpenDoorHorizontal) { dx++; if (EntityMap[dx, ypos] != null) return false; }
                Map[xpos, ypos] = TileType.DoorHorizontal;
                dx = xpos;
                while (dx - 1 > 0 && Map[dx - 1, ypos] == TileType.OpenDoorHorizontal) { dx--; Map[dx, ypos] = TileType.DoorHorizontal; }
                dx = xpos;
                while (dx + 1 < Width - 1 && Map[dx + 1, ypos] == TileType.OpenDoorHorizontal) { dx++; Map[dx, ypos] = TileType.DoorHorizontal; }
            }
            else if (Map[xpos, ypos] == TileType.OpenDoorVertical) {
                if (EntityMap[xpos, ypos] != null) return false;
                int dy = ypos;
                while (dy - 1 > 0 && Map[xpos, dy - 1] == TileType.OpenDoorVertical) { dy--; if (EntityMap[xpos, dy] != null) return false; }
                dy = ypos;
                while (dy + 1 < Height - 1 && Map[xpos, dy + 1] == TileType.OpenDoorVertical) { dy++; if (EntityMap[xpos, dy] != null) return false; }
                Map[xpos, ypos] = TileType.DoorVertical;
                dy = ypos;
                while (dy - 1 > 0 && Map[xpos, dy - 1] == TileType.OpenDoorVertical) { dy--; Map[xpos, dy] = TileType.DoorVertical; }
                dy = ypos;
                while (dy + 1 < Height - 1 && Map[xpos, dy + 1] == TileType.OpenDoorVertical) { dy++; Map[xpos, dy] = TileType.DoorVertical; }
            }
            else throw new Exception("Attempting to close a thing that isn't an open door!");
            foreach (IEntity en in Entities) en.UpdateVisibility(this);
            return true;
        }
        public void RunCreatureTurn(VisualEffect.EffectFactory fact, Action<IEntity> centreView, Action<IEntity> postMoveCheck, Action<string> playSound, Action<string> showMessage) {
            List<Creature> lCreatures = new List<Creature>(Creatures); // In case one dies...
            foreach (Creature cr in lCreatures) {
                cr.AIStep(fact, postMoveCheck, playSound, centreView);
                cr.EndOfTurn(fact, centreView, playSound, showMessage);
            }
        }
        public void KillCreature(Creature cr) {
            Entities.Remove(cr);
            for (int y = cr.Y; y < cr.Y + cr.Size; y++) {
                for (int x = cr.X; x < cr.X + cr.Size; x++) {
                    if (EntityMap[x, y] == null) throw new Exception("Attempting to remove creature from empty cell");
                    EntityMap[x, y] = null;
                }
            }
            Stash st = cr.GenerateStash();
            if (cr.QuestItem) st.Add(ParentMission.MItem);
            AddToStashAtPosition(cr.X, cr.Y, st);

            // Experience
            int exp = (cr.Experience * 5) / (ParentMission.Soldiers.Count + 4);
            foreach (Soldier s in ParentMission.Soldiers) {
                s.AddExperience(exp);
            }
        }
        public void KillSoldier(Soldier s) {
            // Remove soldier from Team & Mission
            Entities.Remove(s);
            ParentMission.Soldiers.Remove(s);
            s.PlayerTeam?.RemoveSoldier(s);
            EntityMap[s.X, s.Y] = null;

            // Generate corpse & drop items
            AddToStashAtPosition(s.X, s.Y, s.GenerateStash());

            // Make sure nothing is targeting it
            foreach (Creature ct in Entities.OfType<Creature>()) {
                if (ct.CurrentTarget == s) ct.SetTarget(null);
            }
        }

        // Utility functions
        public static bool IsObstruction(TileType t) {
            if (t == TileType.DoorHorizontal || t == TileType.DoorVertical || t == TileType.Wall) return true;
            if (t == TileType.SecretDoorHorizontal || t == TileType.SecretDoorVertical) return true;
            return false;
        }
        public bool LooksLikeAWall(int x, int y) {
            if (!Explored[x, y]) return true;
            TileType t = Map[x, y];
            if (t == TileType.Wall) return true;
            if (t == TileType.SecretDoorHorizontal || t == TileType.SecretDoorVertical) return true;
            return false;
        }
        public bool CheckAllSoldiersAtEntrance() {
            foreach (Soldier s in ParentMission.Soldiers) {
                if (!EntryLocations.Contains(s.Location)) return false;
            }
            return true;
        }
        public bool CheckAllSoldiersAtExit() {
            foreach (Soldier s in ParentMission.Soldiers) {
                if (!ExitLocations.Contains(s.Location)) return false;
            }
            return true;
        }
        public void CalculatePlayerVisibility() {
            if (!bInitialised) return;
            for (int x = 0; x < Width; x++) {
                for (int y = 0; y < Height; y++) {
                    Visible[x, y] = false;
                    foreach (Soldier s in ParentMission.Soldiers) {
                        Visible[x, y] |= s.CanSee(x, y);
                        Explored[x, y] |= s.CanSee(x, y);
                    }
                }
            }
//            UpdateTileVertexArray();
        }
        public bool[,] CalculateVisibilityFromEntity(IEntity en) {
            // This is crazily inefficient. If it turns out that it's too slow then I'll have to do it more efficiently somehow
            bool[,] VisibleMap = new bool[Width, Height];
            VisibleMap[en.X, en.Y] = true;

            // Run round perimeter and do a line from entity location to perimeter, stopping as soon as an obstacle is hit
            for (int ey = 0; ey < en.Size; ey++) {
                for (int ex = 0; ex < en.Size; ex++) {
                    for (int x = 0; x <= Width; x++) {
                        CastVisibilityLine(VisibleMap, en.X + ex, en.Y + ey, x, 0);
                        CastVisibilityLine(VisibleMap, en.X + ex, en.Y + ey, x, Height - 1);
                    }
                    for (int y = 0; y <= Height; y++) {
                        CastVisibilityLine(VisibleMap, en.X + ex, en.Y + ey, 0, y);
                        CastVisibilityLine(VisibleMap, en.X + ex, en.Y + ey, Width - 1, y);
                    }
                    // Cast to all other entities
                    foreach (IEntity en2 in Entities) {
                        for (int ey2 = 0; ey2 < en2.Size; ey2++) {
                            for (int ex2 = 0; ex2 < en2.Size; ex2++) {
                                CastVisibilityLine(VisibleMap, en.X + ex, en.Y + ey, en2.X + ex2, en2.Y + ey2);
                            }
                        }
                    }
                }
            }


            // Improve this to get the bits we missed
            for (int x = 0; x < Width; x++) {
                for (int y = 0; y < Height; y++) {
                    if (IsObstruction(Map[x, y]) && !VisibleMap[x, y]) {
                        int nVis = 0;
                        if (x > 0 && VisibleMap[x - 1, y]) nVis++;
                        if (y > 0 && VisibleMap[x, y - 1]) nVis++;
                        if (x < Width - 1 && VisibleMap[x + 1, y]) nVis++;
                        if (y < Height - 1 && VisibleMap[x, y + 1]) nVis++;
                        if (nVis > 1) {
                            int nOpen = 0;
                            if (x > 0 && y > 0 && !IsObstruction(Map[x - 1, y - 1])) nOpen++;
                            if (x < Width - 1 && y > 0 && !IsObstruction(Map[x + 1, y - 1])) nOpen++;
                            if (x > 0 && y < Height - 1 && !IsObstruction(Map[x - 1, y + 1])) nOpen++;
                            if (x < Width - 1 && y < Height - 1 && !IsObstruction(Map[x + 1, y + 1])) nOpen++;
                            if (nOpen > 0) VisibleMap[x, y] = true;
                        }
                    }
                }
            }
            return VisibleMap;
        }
        private void CastVisibilityLine(bool[,] VisibleMap, int fx, int fy, int tx, int ty) {
            // Shamefully stolen from DungeonDelveXL
            double dx = tx - fx;
            double dy = ty - fy;
            double r = Math.Sqrt((dx * dx) + (dy * dy));
            if (r == 0.0) return;
            dx /= r;
            dy /= r;
            bool bBlocked = false;
            double overstep = 0.000001;
            double x = fx + 0.5;
            double y = fy + 0.5;

            do {
                VisibleMap[(int)x, (int)y] = true;
                if (IsObstruction(Map[(int)x, (int)y])) bBlocked = true;

                // Get the next square. Are we near the horizontal or vertical gridlines?
                double dv = 10000.0, dh = 10000.0;
                if (dx > 0) dv = (1.0 - (x - (int)x)) / dx;
                else if (dx < 0) dv = ((int)x - x) / dx;
                if (dy > 0) dh = (1.0 - (y - (int)y)) / dy;
                else if (dy < 0) dh = ((int)y - y) / dy;
                if (dh <= dv) {  // Horizontal line up/down is nearer than the vertical line left/right, or they are equidistant
                    x += (dx * (dh + overstep));
                    y += (dy * (dh + overstep));
                }
                else {  // Vertical line left/right is nearer than the horizontal line up/down
                    x += (dx * (dv + overstep));
                    y += (dy * (dv + overstep));
                }

                // We exactly hit a gridpoint - sort out squares either side then continue
                if (dh == dv && !bBlocked) {
                    // Note that it is impossible for us to be here and either dx = 0 or dy = 0
                    // It is also impossible for either of the diagonal squares to be outside the board unless the target square is
                    if ((int)x < 0 || (int)y < 0 || (int)x >= Width || (int)y >= Height) bBlocked = true;
                    else bBlocked = IsObstruction(Map[(int)x, (int)y]);

                    if (!bBlocked && (int)x >= 0 && (int)x < Width && (int)y >= 0 && (int)y < Height) {
                        if (dx < 0) {
                            VisibleMap[(int)x + 1, (int)y] = true;
                            if (IsObstruction(Map[(int)x + 1, (int)y])) bBlocked = true;
                        }
                        else {
                            VisibleMap[(int)x - 1, (int)y] = true;
                            if (IsObstruction(Map[(int)x - 1, (int)y])) bBlocked = true;
                        }
                        if (dy < 0) {
                            VisibleMap[(int)x, (int)y + 1] = true;
                            if (IsObstruction(Map[(int)x, (int)y + 1])) bBlocked = true;
                        }
                        else {
                            VisibleMap[(int)x, (int)y - 1] = true;
                            if (IsObstruction(Map[(int)x, (int)y - 1])) bBlocked = true;
                        }
                    }
                }
            } while (!bBlocked && x >= 0.0 && y >= 0.0 && (int)x < Width && (int)y < Height);
        }
        public Trap? GetTrapAtPoint(int x, int y) {
            if (x < 0 || x >= Width || y < 0 || y >= Height) throw new Exception("Attemptign to get trap at illegal point!");
            Point pt = new Point(x, y);
            if (Traps.ContainsKey(pt)) return Traps[pt];
            return null;
        }

        // Item functions
        public Stash? GetStashAtPoint(int x, int y) {
            if (x < 0 || x >= Width || y < 0 || y >= Height) throw new Exception("Attempting to get stash at illegal point!");
            Point pt = new Point(x, y);
            if (Items.ContainsKey(pt)) return Items[pt];
            return null;
        }
        public void AddItemToMapStash(int x, int y, IItem it) {
            if (x < 0 || x >= Width || y < 0 || y >= Height) throw new Exception("Attempting to add item at illegal point!");
            Point pt = new Point(x, y);
            if (Items.ContainsKey(pt)) {
                Items[pt].Add(it);
            }
            else {
                Stash st = new Stash(pt);
                st.Add(it);
                Items.Add(pt, st);
            }
        }
        public void ReplaceStashAtPosition(int x, int y, Stash? st) {
            if (x < 0 || x >= Width || y < 0 || y >= Height) return;
            Point pt = new Point(x, y);
            if (st != null) st.SetLocation(pt);
            if (!Items.ContainsKey(pt)) {
                if (st == null || st.Count == 0) return;
                else Items.Add(pt, st);
            }
            else {
                if (st == null || st.Count == 0) Items.Remove(pt);
                else Items[pt] = st;
            }
        }
        public void AddToStashAtPosition(int x, int y, Stash st) {
            if (x < 0 || x >= Width || y < 0 || y >= Height) return;
            if (st.IsEmpty) return;
            Point pt = new Point(x, y);
            if (!Items.ContainsKey(pt)) {
                Items.Add(pt, st);
                return;
            }
            Items[pt].Add(st);
        }

        // Set where the mouse is currently hovering
        public void SetHover(int hx, int hy) {
            if (hx < 0 || hx >= Width || hy < 0 || hy >= Height) return;
            HoverX = hx;
            HoverY = hy;
        }
        public IEntity? GetHoverEntity() {
            return GetEntityAt(HoverX, HoverY);
        }
        public IEntity? GetEntityAt(int x, int y) {
            if (x >= 0 && x < Width && y >= 0 && y < Height) {
                return EntityMap[x, y];
            }
            return null;
        }
        public bool IsFriendlyAt(int x, int y) {
            if (x >= 0 && x < Width && y >= 0 && y < Height) {
                IEntity? en = EntityMap[x, y];
                if (en is null) return false;
                if (en is Soldier) return true;
            }
            return false;
        }

        // ---- Pathfinding
        public List<Point>? ShortestPath(IEntity en, Point start, Point end, int PruningModifier, bool bOnlyExploredCells, int mindist = 1) {
            int[,] AStarG = new int[Width, Height];
            Dictionary<Point, int> lOpen = new Dictionary<Point, int>();
            Dictionary<Point, int> lClosed = new Dictionary<Point, int>();
            const int BAD_ROUTE = 100000;
            const int CREATURE_ASTAR_PRUNING_DIST = 5;

            // Pointless search
            if (start == end || start == Point.Empty || end == Point.Empty) return null;

            //  Initial guess is the initial naive taxicab distance estimate between source and target
            int guess = Math.Abs(start.X - end.X) + Math.Abs(start.Y - end.Y);
            if (guess <= mindist) return new List<Point>();  // Already close enough!
            lOpen.Add(start, guess);

            //  Main A-Star Loop
            do {
                Point pt = GetLowest(lOpen);
                if (pt == Point.Empty) throw new Exception("Error in AStar algorithm!");
                lClosed.Add(pt, lOpen[pt]);
                lOpen.Remove(pt);
                int[] D = new int[4];

                D[0] = D[1] = D[2] = D[3] = BAD_ROUTE;

                // Can move left?
                if (pt.X > 1) {
                    bool bOK = true;
                    int extra = 0;
                    for (int i = 0; i < en.Size; i++) {
                        if (IsObstruction(Map[pt.X - 1, pt.Y + i]) || (EntityMap[pt.X - 1, pt.Y + i] != null && EntityMap[pt.X - 1, pt.Y + i] != en)) {
                            if (en.CanOpenDoors && Map[pt.X - 1, pt.Y + i] == TileType.DoorVertical) extra = 1;
                            else { bOK = false; break; }
                        }
                        if (Traps.ContainsKey(new Point(pt.X - 1, pt.Y + i)) && (!Traps[new Point(pt.X - 1, pt.Y + i)].Hidden || en is Creature)) { bOK = false; break; }
                    }
                    if (bOnlyExploredCells && !Explored[pt.X - 1, pt.Y]) bOK = false;

                    if (bOK) {
                        D[0] = Math.Abs((pt.X - 1) - end.X) + Math.Abs(pt.Y - end.Y) + extra;
                    }
                }
                // Can move right?
                if (pt.X + en.Size < Width - 1) {
                    bool bOK = true;
                    int extra = 0;
                    for (int i = 0; i < en.Size; i++) {
                        if (IsObstruction(Map[pt.X + en.Size, pt.Y + i]) || (EntityMap[pt.X + en.Size, pt.Y + i] != null && EntityMap[pt.X + en.Size, pt.Y + i] != en)) {
                            if (en.CanOpenDoors && Map[pt.X + en.Size, pt.Y + i] == TileType.DoorVertical) extra = 1;
                            else { bOK = false; break; }
                        }
                        if (Traps.ContainsKey(new Point(pt.X + 1, pt.Y + i)) && (!Traps[new Point(pt.X + 1, pt.Y + i)].Hidden || en is Creature)) { bOK = false; break; }
                    }
                    if (bOnlyExploredCells && !Explored[pt.X + 1, pt.Y]) bOK = false;

                    if (bOK) {
                        D[1] = Math.Abs((pt.X + 1) - end.X) + Math.Abs(pt.Y - end.Y) + extra;
                    }
                }
                // Can move down?
                if (pt.Y > 1) {
                    bool bOK = true;
                    int extra = 0;
                    for (int i = 0; i < en.Size; i++) {
                        if (IsObstruction(Map[pt.X + i, pt.Y - 1]) || (EntityMap[pt.X + i, pt.Y - 1] != null && EntityMap[pt.X + 1, pt.Y - 1] != en)) {
                            if (en.CanOpenDoors && Map[pt.X + i, pt.Y - 1] == TileType.DoorHorizontal) extra = 1;
                            else { bOK = false; break; }
                        }
                        if (Traps.ContainsKey(new Point(pt.X + i, pt.Y - 1)) && (!Traps[new Point(pt.X + i, pt.Y - 1)].Hidden || en is Creature)) { bOK = false; break; }
                    }
                    if (bOnlyExploredCells && !Explored[pt.X, pt.Y - 1]) bOK = false;

                    if (bOK) {
                        D[2] = Math.Abs(pt.X - end.X) + Math.Abs((pt.Y - 1) - end.Y) + extra;
                    }
                }
                // Can move up?
                if (pt.Y + en.Size < Height - 1) {
                    bool bOK = true;
                    int extra = 0;
                    for (int i = 0; i < en.Size; i++) {
                        if (IsObstruction(Map[pt.X + i, pt.Y + en.Size]) || (EntityMap[pt.X + i, pt.Y + en.Size] != null && EntityMap[pt.X + 1, pt.Y + en.Size] != en)) {
                            if (en.CanOpenDoors && Map[pt.X + i, pt.Y + en.Size] == TileType.DoorHorizontal) extra = 1;
                            else { bOK = false; break; }
                        }
                        if (Traps.ContainsKey(new Point(pt.X + i, pt.Y + 1)) && (!Traps[new Point(pt.X + i, pt.Y + 1)].Hidden || en is Creature)) { bOK = false; break; }
                    }
                    if (bOnlyExploredCells && !Explored[pt.X, pt.Y + 1]) bOK = false;

                    if (bOK) {
                        D[3] = Math.Abs(pt.X - end.X) + Math.Abs((pt.Y + 1) - end.Y) + extra;
                    }
                }

                // Update the state
                for (int i = 0; i <= 3; i++) {
                    if (D[i] != BAD_ROUTE) {
                        int dx = pt.X, dy = pt.Y;
                        int dist = AStarG[dx, dy] + 1;
                        switch (i) {
                            case (0): dx--; break;
                            case (1): dx++; break;
                            case (2): dy--; break;
                            case (3): dy++; break;
                        }
                        int mintaxicab = 100000;
                        for (int sdy = dy; sdy < dy + en.Size; sdy++) {
                            for (int sdx = dx; sdx < dx + en.Size; sdx++) {
                                int h = Math.Abs(end.X - sdx) + Math.Abs(end.Y - sdy); // Naive estimated taxicab distance to target
                                if (h < mintaxicab) mintaxicab = h;
                            }
                        }
                        //int h = Math.Abs(end.X - dx) + Math.Abs(end.Y-dy); // Naive estimated taxicab distance to target
                        if (dist + mintaxicab < guess + CREATURE_ASTAR_PRUNING_DIST + PruningModifier) { // We're not way off-track
                            Point pt2 = new Point(dx, dy);
                            // Arrived?
                            if (mintaxicab <= mindist) {
                                AStarG[dx, dy] = dist;
                                return ExtractPath(start, pt2, AStarG);
                            }
                            // In closed queue i.e. already searched, so skip this one
                            if (lClosed.ContainsKey(pt2)) continue;
                            // In open queue i.e. about to be searched
                            if (lOpen.ContainsKey(pt2)) {
                                // If we got here more quickly than before then update the score.
                                if (dist + mintaxicab < lOpen[pt2]) {
                                    AStarG[dx, dy] = dist;
                                    lOpen[pt2] = dist + mintaxicab;
                                }
                            }
                            // Not in either list, so make sure we add it to the open list and set the distance to this point.
                            else {
                                AStarG[dx, dy] = dist;
                                lOpen.Add(pt2, dist + mintaxicab);
                            }
                        }
                    }
                }
            } while (lOpen.Any());
            return null; // No possible path
        }
        private Point GetLowest(Dictionary<Point, int> lpt) {
            int best = 1000;
            Point bpt = Point.Empty;
            foreach (Point pt in lpt.Keys) {
                if (lpt[pt] < best) {
                    bpt = pt;
                    best = lpt[pt];
                }
            }
            return bpt;
        }
        private int ShortestPathLength(IEntity en, Point start, Point end, int PruningModifier) {
            List<Point>? lpt = ShortestPath(en, start, end, PruningModifier, false);
            if (lpt == null) return int.MaxValue;
            return lpt.Count;
        }
        private List<Point> ExtractPath(Point start, Point end, int[,] AStarG) {
            List<Point> lpt = new List<Point>();
            Point pt = end;
            lpt.Add(pt);
            do {
                // Are we there yet?
                if ((pt.X == start.X && Math.Abs(pt.Y - start.Y) <= 1) || (pt.Y == start.Y && Math.Abs(pt.X - start.X) <= 1)) {
                    lpt.Reverse();
                    return lpt;
                }
                // Find the adjacent square with the lowest distance
                int dx = pt.X, dy = pt.Y;
                int best = AStarG[dx, dy];
                if (AStarG[pt.X - 1, pt.Y] == best - 1) pt.X--;
                else if (AStarG[pt.X + 1, pt.Y] == best - 1) pt.X++;
                else if (AStarG[pt.X, pt.Y - 1] == best - 1) pt.Y--;
                else if (AStarG[pt.X, pt.Y + 1] == best - 1) pt.Y++;
                else throw new Exception("No path found!");
                lpt.Add(pt);
            } while (true);
        }
        public bool EntityIsAdjacentToDoor(IEntity en, int xpos, int ypos) {
            if (en == null) return false;
            // Hovering over a door
            if (Map[xpos, ypos] == MissionLevel.TileType.DoorHorizontal || Map[xpos, ypos] == MissionLevel.TileType.OpenDoorHorizontal) {
                if (Math.Abs(en.Y - ypos) != 1) return false;
                if (en.X == xpos) return true;
                int dx = xpos;
                if (en.X < xpos) {
                    while (dx - 1 > 0 && Map[dx - 1, ypos] == Map[xpos, ypos]) {
                        dx--;
                        if (dx == en.X) return true;
                    }
                }
                else {
                    while (dx + 1 < Width - 1 && Map[dx + 1, ypos] == Map[xpos, ypos]) {
                        dx++;
                        if (dx == en.X) return true;
                    }
                }
            }
            else if (Map[xpos, ypos] == MissionLevel.TileType.DoorVertical || Map[xpos, ypos] == MissionLevel.TileType.OpenDoorVertical) {
                if (Math.Abs(en.X - xpos) != 1) return false;
                if (en.Y == ypos) return true;
                int dy = ypos;
                if (en.Y < ypos) {
                    while (dy - 1 > 0 && Map[xpos, dy - 1] == Map[xpos, ypos]) {
                        dy--;
                        if (dy == en.Y) return true;
                    }
                }
                else {
                    while (dy + 1 < Height - 1 && Map[xpos, dy + 1] == Map[xpos, ypos]) {
                        dy++;
                        if (dy == en.Y) return true;
                    }
                }
            }
            return false;
        }

        // -- Sight lines
        public bool CanSee(Point from, Point to) {
            // Shamefully stolen from CastVisibilityLine
            double dx = to.X - from.X;
            double dy = to.Y - from.Y;
            double r = Math.Sqrt((dx * dx) + (dy * dy));
            if (r <= 1.0) return true;
            dx /= r;
            dy /= r;
            double overstep = 0.000001;
            double x = from.X + 0.5;
            double y = from.Y + 0.5;

            do {
                if (IsObstruction(Map[(int)x, (int)y])) return false;

                // Get the next square. Are we near the horizontal or vertical gridlines?
                double dv = 10000.0, dh = 10000.0;
                if (dx > 0) dv = (1.0 - (x - (int)x)) / dx;
                else if (dx < 0) dv = ((int)x - x) / dx;
                if (dy > 0) dh = (1.0 - (y - (int)y)) / dy;
                else if (dy < 0) dh = ((int)y - y) / dy;
                if (dh <= dv) {  // Horizontal line up/down is nearer than the vertical line left/right, or they are equidistant
                    x += (dx * (dh + overstep));
                    y += (dy * (dh + overstep));
                }
                else {  // Vertical line left/right is nearer than the horizontal line up/down
                    x += (dx * (dv + overstep));
                    y += (dy * (dv + overstep));
                }
                if ((int)x == to.X && (int)y == to.Y) return true;
                // We exactly hit a gridpoint - sort out squares either side then continue
                if (dh == dv) {
                    if ((int)x < 0 || (int)y < 0 || (int)x >= Width || (int)y >= Height) return false;
                    if (IsObstruction(Map[(int)x, (int)y])) return false;
                    if (dx < 0 && IsObstruction(Map[(int)x + 1, (int)y])) return false;
                    if (dx >= 0 && IsObstruction(Map[(int)x - 1, (int)y])) return false;
                    if (dy < 0 && IsObstruction(Map[(int)x, (int)y + 1])) return false;
                    if (dy >= 0 && IsObstruction(Map[(int)x, (int)y - 1])) return false;
                }
            } while (x >= 0.0 && y >= 0.0 && (int)x < Width && (int)y < Height);
            return false;
        }

        // Debugging
        private void WriteOutMap(int id) {
            using (StreamWriter file = new StreamWriter(@"C:\temp\Map" + id + ".txt")) {
                for (int y = Height - 1; y >= 0; y--) {
                    StringBuilder sb = new StringBuilder();
                    for (int x = 0; x < Width; x++) {
                        switch (Map[x, y]) {
                            case TileType.Wall: sb.Append("#"); break;
                            case TileType.Floor: sb.Append(" "); break;
                            default: sb.Append("?"); break;
                        }
                    }
                    file.WriteLine(sb.ToString());
                }
            }
        }
        private void WriteOutMapRooms(int id) {
            using (StreamWriter file = new StreamWriter(@"C:\temp\Map" + id + ".txt")) {
                for (int y = Height - 1; y >= 0; y--) {
                    StringBuilder sb = new StringBuilder();
                    for (int x = 0; x < Width; x++) {
                        switch (Map[x, y]) {
                            case TileType.Wall: sb.Append("#"); break;
                            case TileType.Floor: if (RoomMap[x, y] < 0) sb.Append(" "); else sb.Append(Convert.ToChar(RoomMap[x, y] + 64)); break;
                            default: sb.Append("?"); break;
                        }
                    }
                    file.WriteLine(sb.ToString());
                }
            }
        }
        private void WriteOutAStar(int[,] AStarG) {
            using (StreamWriter file = new StreamWriter(@"C:\temp\AStarG.csv")) {
                for (int y = Height - 1; y >= 0; y--) {
                    StringBuilder sb = new StringBuilder();
                    for (int x = 0; x < Width; x++) {
                        if (x > 0) sb.Append(",");
                        if (Map[x, y] == TileType.Wall) sb.Append("#");
                        else sb.Append(AStarG[x, y]);
                    }
                    file.WriteLine(sb.ToString());
                }
            }
        }
    }
}
