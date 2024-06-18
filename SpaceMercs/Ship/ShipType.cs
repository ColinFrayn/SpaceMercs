using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using SpaceMercs.Graphics;
using SpaceMercs.Graphics.Shapes;
using System.Xml;

namespace SpaceMercs {
    using RoomSize = ShipEquipment.RoomSize;
    public class ShipType {
        public string Name { get; private set; } = string.Empty;
        public string AKA { get; private set; } = string.Empty; // Backwards compatibility for names
        public double Cost { get; private set; }
        public string Description { get; private set; } = string.Empty;
        public readonly List<ShipRoomDesign> Rooms = new List<ShipRoomDesign>();
        public int Length { get; private set; }
        public int Width { get; private set; }
        public int Armour { get; private set; }
        public double MaxHull { get { return (Small * 2.0) + (Medium * 4.0) + (Large * 8.0) + (Weapon * 1.0) + 4.0; } }
        public int Cargo { get { return (Small * 150) + (Medium * 500) + (Large * 1500) - (Weapon * 50) + 50; } } // Carrying capacity in kg. Weapon rooms need ammo, supplies etc. hence removing space
        public string RoomConfigString { get { return Small + "/" + Medium + "/" + Large + "/" + Weapon; } }
        public List<Vector2> Perimeter { get; private set; }
        public List<Point> Fillers { get; private set; }
        public List<Vector2> ShieldShape { get; private set; }
        public int EngineRoomID { get; private set; } = -1;
        public int PowerCoreRoomID { get; private set; } = -1;
        public Race? RequiredRace { get; private set; }

        // Autogen stuff
        public int Seed { get; private set; }
        public double Diff { get; private set; } // For enemy ships on missions

        // Ship definition
        public int Small { get; private set; }
        public int Medium { get; private set; }
        public int Large { get; private set; }
        public int Weapon { get; private set; }

        // Internal stuff
        private readonly bool[,] Layout = new bool[201, 201]; // Midpoint @ (100,100)
        private const double dTheta = Math.PI / 8;
        private const double dRad = 0.25;
        private int MinX = 1000, MaxX = -1000;
        private int MinY = 1000, MaxY = -1000;
        private GLShape? _glPerimeter = null;
        private GLShape? GLPerimeter {
            get {
                if (_glPerimeter is null) {
                    // Set up the render list
                    List<VertexPos3D> vertices = new List<VertexPos3D>();
                    foreach (Vector2 pt in Perimeter) {
                        vertices.Add(new VertexPos3D(new Vector3(pt.X, pt.Y, 0f)));
                    }
                    _glPerimeter = new GLShape(vertices.ToArray(), Enumerable.Range(0, vertices.Count).ToArray(), PrimitiveType.LineLoop);
                }
                return _glPerimeter;
            }
        }
        private GLShape? _glShields = null;
        private GLShape? GLShields {
            get {
                if (_glShields is null) {
                    // Triangulate the shield perimeter
                    List<Vector2> triangles = GraphicsUtils.Triangulate(ShieldShape);
                    List<VertexPos3D> vertices = new List<VertexPos3D>();
                    // Set up the render list
                    foreach (Vector2 pt in triangles) {
                        vertices.Add(new VertexPos3D(new Vector3(pt.X, pt.Y, 0f)));
                    }
                    _glShields = new GLShape(vertices.ToArray(), Enumerable.Range(0, vertices.Count).ToArray(), PrimitiveType.Triangles);
                }
                return _glShields;
            }
        }

        public ShipType() {
            Description = "No description";
            Fillers = new List<Point>();
        }
        public ShipType(int seed, double diff) {
            Seed = seed;
            Diff = diff;
            Fillers = new List<Point>();
        }
        public ShipType(XmlNode xml) {
            Name = xml.Attributes!["Name"]?.Value ?? string.Empty;
            AKA = xml.SelectNodeText("AKA");
            Cost = xml.SelectNodeDouble("Cost");
            Small = xml.SelectNodeInt("Small");
            Medium = xml.SelectNodeInt("Medium");
            Large = xml.SelectNodeInt("Large");
            Weapon = xml.SelectNodeInt("Weapon");
            Description = xml.SelectNodeText("Desc");
            Fillers = new List<Point>();
            Armour = xml.SelectNodeInt("Armour",0);
            // Load the race that this ship type is restricted to (default null)
            if (xml.SelectSingleNode("Race") != null) {
                RequiredRace = StaticData.GetRaceByName(xml.SelectNodeText("Race"));
                if (RequiredRace == null) {
                    throw new Exception("Could not find restricted race \"" + xml.SelectNodeText("Race") + "\" for equipment " + Name);
                }
            }

            SetupLayout();
        }
        public static ShipType Empty { get { return new ShipType(); } }

        public int MaximumRoomsOfSize(ShipEquipment.RoomSize sz) {
            if (sz == ShipEquipment.RoomSize.Small) return Small;
            if (sz == ShipEquipment.RoomSize.Medium) return Medium;
            if (sz == ShipEquipment.RoomSize.Large) return Large;
            if (sz == ShipEquipment.RoomSize.Core) return 1;
            if (sz == ShipEquipment.RoomSize.Engine) return 1;
            if (sz == ShipEquipment.RoomSize.Weapon) return Weapon;
            if (sz == ShipEquipment.RoomSize.Armour) return 1;
            return 0;
        }

        // Work out the location for all the rooms
        private void SetupLayout() {
            // https://www.reddit.com/r/gamedev/comments/1z676s/how_i_generate_a_layout_for_space_ships_quick/
            Rooms.Clear();
            Random rand = new Random(0);
            PlacePowerCore();

            // Place the rooms at random
            int LargeLeft = Large, MediumLeft = Medium, SmallLeft = Small;
            do {
                int r = rand.Next(SmallLeft + MediumLeft + LargeLeft); // Pick a random room size to place
                if (r < LargeLeft) LargeLeft -= SetupRoom(RoomSize.Large, (LargeLeft % 2) == 0, rand);
                else if (r < LargeLeft + MediumLeft) MediumLeft -= SetupRoom(RoomSize.Medium, (MediumLeft % 2) == 0, rand);
                else SmallLeft -= SetupRoom(RoomSize.Small, (SmallLeft % 2) == 0, rand);
            } while (SmallLeft + MediumLeft + LargeLeft > 0);

            PlaceWeapons(rand);
            PlaceEngine();

            Length = MaxX - MinX;
            Width = MaxY - MinY;
            int XShift = ((MaxX + MinX) / 2) - 100;
            foreach (ShipRoomDesign rd in Rooms) {
                rd.Shift(-XShift, 0);
            }
            SetupShipPerimeterBox(XShift);
            SetupShieldShape(XShift);
            SetupFillers(XShift);
        }

        // Given the current grid, place one or two rooms symmetrically
        private int SetupRoom(RoomSize sz, bool bPair, Random rand) {
            if (!bPair) {
                SetupSingleRoom(sz, rand);
                return 1;
            }
            // Start at a random point along the central axis
            int x0 = 60 + rand.Next(71); // Slightly biased to left
            int y0 = 101;
            double theta = 0.0, r = 0.0;
            bool bOK = false;
            do {
                int x = x0 + (int)(r * Math.Cos(theta));
                int y = y0 + (int)(r * Math.Sin(theta));

                bool bOKNoRot = false, bOKRot = false;
                bOKNoRot = CanPlaceRoom(sz, x, y, false);
                bOKRot = CanPlaceRoom(sz, x, y, true);
                bOK = bOKNoRot | bOKRot;
                if (bOK) {
                    bool bRotate = false;
                    if (!bOKNoRot || (bOKNoRot && bOKRot && rand.Next(3) == 0)) bRotate = true;

                    // First shuffle towards the centre line in Y (note: Y is always above the centre line)
                    do {
                        if (!CanPlaceRoom(sz, x, y - 1, bRotate)) break;
                        if (y > 101) y--;
                    } while (y > 101);

                    // Now shuffle in X towards the centre
                    do {
                        int x2 = x;
                        if (x == 100) break;
                        if (x > 100) x2--;
                        else x2++;
                        if (!CanPlaceRoom(sz, x2, y, bRotate)) break;
                        x = x2;
                    } while (x != 100);

                    // Now place the rooms
                    PlaceRoom(sz, x, y, bRotate, true);
                }
                else {
                    r += dRad;
                    theta += dTheta;
                    if (theta > Math.PI) theta -= Math.PI; // Only do one side (because it's symmetrical)
                }
            } while (!bOK);
            return 2;
        }

        // Place a single room along the central axis
        private void SetupSingleRoom(RoomSize sz, Random rand) {
            // Make sure we rotate so that the odd-numbered length is across
            bool bRotate = false;
            if (sz == ShipEquipment.RoomSize.Small || (sz == ShipEquipment.RoomSize.Medium && rand.Next(2) == 0)) bRotate = true;

            // Start at a random point along the central axis, either very far front or back
            int x = 100 + ((rand.Next(2) * 180) - 90);
            int y = (201 - ShipRoomDesign.RoomHeight(sz, bRotate)) / 2;

            // As long as this location is OK, shuffle towards the middle
            do {
                if (x == 100) break;
                int x2 = x;
                if (x > 100) x2--;
                else x2++;
                if (!CanPlaceRoom(sz, x2, y, bRotate)) break;
                x = x2;
            } while (x != 100);

            // Place this room
            PlaceRoom(sz, x, y, bRotate, false);
        }

        // Place a single room along the central axis
        private void PlacePowerCore() {
            // Place the room and surrounding corridor
            ShipRoomDesign rd = new ShipRoomDesign(99, 99, false, ShipEquipment.RoomSize.Core);
            for (int h = 98; h <= 102; h++) {
                for (int w = 98; w <= 102; w++) {
                    Layout[w, h] = true;
                }
            }
            Rooms.Add(rd);
            MinX = 98;
            MaxX = MaxY = 102;
        }

        // Place weapons randomly around the ship, towards the front, without corridors
        private void PlaceWeapons(Random rand) {
            int nw = Weapon;
            // Odd number - place one right at the front
            if (nw % 2 == 1) {
                int y = 100;
                int x = 10;
                while (!Layout[x + 1, y]) x++;
                PlaceRoom(ShipEquipment.RoomSize.Weapon, x, y, false, false);
                nw--;
            }

            // Place the remainder in pairs, biased towards the front of the ship
            while (nw > 0) {
                // Start at a random point along the central axis
                int x = 65 + rand.Next(45);
                int y = 150;
                if (!Layout[x, y]) { // This square is empty?
                                     // Shuffle towards the centre line in Y (note: Y is always above the centre line)
                    while (y > 100 && !Layout[x, y - 1]) y--;

                    // Only place weapons if we've found a proper point on the hull
                    if (y == 100) continue;

                    // Now place the weapons
                    PlaceRoom(ShipEquipment.RoomSize.Weapon, x, y, false, true);
                    nw -= 2;
                }
            }
        }

        // Place the engine at the far right of the ship
        private void PlaceEngine() {
            // Start far to the right
            int y = (201 - ShipRoomDesign.RoomHeight(ShipEquipment.RoomSize.Engine, false)) / 2;
            int x = 190;

            // As long as this location is OK, shuffle towards the middle
            while (CanPlaceRoom(ShipEquipment.RoomSize.Engine, x - 1, y, false)) x--;

            // Place this room
            PlaceRoom(ShipEquipment.RoomSize.Engine, x, y, false, false);
        }

        // Can we place a room in this orientation at these coordinates?
        private bool CanPlaceRoom(RoomSize sz, int x, int y, bool bRotate) {
            // Calculate the room dimensions
            int dx = ShipRoomDesign.RoomWidth(sz, bRotate), dy = ShipRoomDesign.RoomHeight(sz, bRotate);

            // Check if the room can go here
            for (int h = y; h < y + dy; h++) {
                for (int w = x; w < x + dx; w++) {
                    if (Layout[w, h]) return false;
                }
            }
            return true;
        }

        // Place a room at this location with this orientation
        private void PlaceRoom(RoomSize sz, int x, int y, bool bRotate, bool bPair) {
            // Place the room and surrounding corridor
            ShipRoomDesign rd = new ShipRoomDesign(x, y, bRotate, sz);
            int gap = 1;
            if (sz == RoomSize.Weapon || sz == RoomSize.Engine) gap = 0;
            for (int h = y - gap; h < y + rd.Height + gap; h++) {
                for (int w = x - gap; w < x + rd.Width + gap; w++) {
                    Layout[w, h] = true;
                }
            }
            Rooms.Add(rd);
            if (sz == RoomSize.Engine) EngineRoomID = Rooms.Count - 1;
            else if (sz == RoomSize.Core) PowerCoreRoomID = Rooms.Count - 1;

            if (x - gap < MinX) MinX = x - gap;
            if (x + rd.Width + gap > MaxX) MaxX = x + rd.Width + gap;
            if (y - gap < MinY) MinY = y - gap;
            if (y + rd.Height + gap > MaxY) MaxY = y + rd.Height + gap;

            // Only placing a single room on-axis so return
            if (!bPair) return;

            // If this is a pair then also place the mirror image room (& corridor)
            int y2 = (201 - y) - rd.Height;
            ShipRoomDesign rd2 = new ShipRoomDesign(x, y2, bRotate, sz);
            for (int h = (200 - y) - gap; h < (200 - (y + rd2.Height)) + gap; h++) {
                for (int w = x - gap; w < x + rd2.Width + gap; w++) {
                    Layout[w, h] = true;
                }
            }
            if (y2 - gap < MinY) MinY = y2 - gap;
            if (y2 + rd2.Height + gap > MaxY) MaxY = y2 + rd2.Height + gap;
            Rooms.Add(rd2);
        }

        // Setup a random ship type
        public static ShipType SetupRandomShipType(double dDiff, int seed) {
            Random rand = new Random(seed);
            ShipType tp = new ShipType(seed, dDiff);
            int size = 1;
            while (rand.NextDouble() > (1.0 + ((double)size / 20.0) - (Math.Sqrt(dDiff) / 6.0))) size++;
            size = 2 + (int)((rand.NextDouble() + 1.0) * (double)size * 4.0);

            while (size > 0) {
                int r = rand.Next(3);
                if (r == 0) { tp.Small++; size -= 2; }
                if (r == 1) { tp.Medium++; size -= 5; }
                if (r == 2) { tp.Large++; size -= 10; }
            }
            tp.Weapon = 1;
            if (dDiff > 4.0 + (rand.NextDouble() * 2.0)) tp.Weapon++;
            if (dDiff > 10.0 + (rand.NextDouble() * 5.0)) tp.Weapon++;
            if (dDiff > 15.0 + (rand.NextDouble() * 7.0)) tp.Weapon++;
            tp.SetupLayout();

            return tp;
        }

        // Calculate the points along the perimeter of this ship
        private void SetupShipPerimeterBox(int XShift) {
            if (!Rooms.Any()) return;
            Perimeter = GeneratePerimeter(Layout, XShift);
        }
        public void DrawPerimeter(ShaderProgram prog) {
            if (GLPerimeter is null) return;
            GL.UseProgram(prog.ShaderProgramHandle);
            GLPerimeter.BindAndDraw();
        }

        // Calculate the shape of shields, if any are installed
        private void SetupShieldShape(int XShift) {
            if (!Rooms.Any()) return;

            // Firstly, make a new layout with shield rooms on the edge of the ship
            bool[,] ShieldLayout = new bool[201, 201]; // Midpoint @ (100,100)
            for (int y = 0; y <= 200; y++) {
                for (int x = 0; x <= 200; x++) {
                    ShieldLayout[x, y] = Layout[x, y];
                    if (!Layout[x, y]) {
                        if (x > 0 && Layout[x - 1, y]) ShieldLayout[x, y] = true;
                        if (x < 200 && Layout[x + 1, y]) ShieldLayout[x, y] = true;
                        if (y > 0 && Layout[x, y - 1]) ShieldLayout[x, y] = true;
                        if (y < 200 && Layout[x, y + 1]) ShieldLayout[x, y] = true;
                        if (x > 0 && y > 0 && Layout[x - 1, y - 1]) ShieldLayout[x, y] = true;
                        if (x < 200 && y > 0 && Layout[x + 1, y - 1]) ShieldLayout[x, y] = true;
                        if (x > 0 && y < 200 && Layout[x - 1, y + 1]) ShieldLayout[x, y] = true;
                        if (x < 200 && y < 200 && Layout[x + 1, y + 1]) ShieldLayout[x, y] = true;
                    }
                }
            }

            ShieldShape = GeneratePerimeter(ShieldLayout, XShift);
        }
        public void DrawShields(ShaderProgram prog) {
            if (GLShields is null) return;
            GL.UseProgram(prog.ShaderProgramHandle);
            GLShields.BindAndDraw();
        }

        // Calculate perimeter of arbitrary layout
        private static List<Vector2> GeneratePerimeter(bool [,] blocked, int XShift) {
            List<Vector2> Points = new List<Vector2>();
            int dx = -XShift - 100, dy = -99;
            int x = 0, y = 100;
            while (!blocked[x, y]) x++;
            int dir = blocked[x, y + 1] ? 2 : 1;
            Points.Add(new Vector2(x + dx, y + dy));
            // Trace the edge anti-clockwise
            do {
                if (dir == 0) {
                    y--;
                    if (blocked[x, y]) {
                        dir = 1;
                        Points.Add(new Vector2(x + dx, y + dy));
                    }
                    else if (!blocked[x - 1, y]) {
                        dir = 3;
                        Points.Add(new Vector2(x + dx, y + dy));
                    }
                }
                else if (dir == 1) {
                    x++;
                    if (blocked[x, y + 1]) {
                        dir = 2;
                        Points.Add(new Vector2(x + dx, y + dy));
                    }
                    else if (!blocked[x, y]) {
                        dir = 0;
                        Points.Add(new Vector2(x + dx, y + dy));
                    }
                }
                else if (dir == 2) {
                    y++;
                    if (blocked[x - 1, y + 1]) {
                        dir = 3;
                        Points.Add(new Vector2(x + dx, y + dy));
                    }
                    else if (!blocked[x, y + 1]) {
                        dir = 1;
                        Points.Add(new Vector2(x + dx, y + dy));
                    }
                }
                else if (dir == 3) {
                    x--;
                    if (blocked[x - 1, y]) {
                        dir = 0;
                        Points.Add(new Vector2(x + dx, y + dy));
                    }
                    else if (!blocked[x - 1, y + 1]) {
                        dir = 2;
                        Points.Add(new Vector2(x + dx, y + dy));
                    }
                }
                if (Points.Count > 100) throw new Exception("Could not trace ship perimeter");
            } while (y >= 100);

            // Add in bottom half
            int np = Points.Count;
            for (int n = np - 1; n >= 0; n--) {
                Vector2 pt = Points[n];
                Points.Add(new Vector2(pt.X, -pt.Y + 1));
            }
            return Points;
        }

        // Fill in any gaps
        private void SetupFillers(int XShift) {
            int dx = -XShift - 100, dy = -100;
            // Flood fill outside ship
            bool[,] Outside = new bool[201, 201];
            Queue<Point> qBacklog = new Queue<Point>();
            do {
                Point pt = (qBacklog.Any()) ? qBacklog.Dequeue() : new Point(0, 100);
                if (!Layout[pt.X, pt.Y]) {
                    Outside[pt.X, pt.Y] = true;
                    if (pt.X >= MinX && !Outside[pt.X - 1, pt.Y] && !Layout[pt.X - 1, pt.Y] && !qBacklog.Contains(new Point(pt.X - 1, pt.Y))) qBacklog.Enqueue(new Point(pt.X - 1, pt.Y));
                    if (pt.X <= MaxX && !Outside[pt.X + 1, pt.Y] && !Layout[pt.X + 1, pt.Y] && !qBacklog.Contains(new Point(pt.X + 1, pt.Y))) qBacklog.Enqueue(new Point(pt.X + 1, pt.Y));
                    if (pt.Y > 100 && !Outside[pt.X, pt.Y - 1] && !Layout[pt.X, pt.Y - 1] && !qBacklog.Contains(new Point(pt.X, pt.Y - 1))) qBacklog.Enqueue(new Point(pt.X, pt.Y - 1));
                    if (pt.Y <= MaxY && !Outside[pt.X, pt.Y + 1] && !Layout[pt.X, pt.Y + 1] && !qBacklog.Contains(new Point(pt.X, pt.Y + 1))) qBacklog.Enqueue(new Point(pt.X, pt.Y + 1));
                }
            } while (qBacklog.Any());

            for (int y = 100; y <= MaxY; y++) {
                for (int x = MinX; x <= MaxX; x++) {
                    if (!Outside[x, y] && !Layout[x, y]) {
                        Fillers.Add(new Point(x + dx, y + dy));
                        Fillers.Add(new Point(x + dx, -(y + dy)));
                    }
                }
            }
        }

        // Get a string that describes the size of this ship
        public string SizeString() {
            double hull = MaxHull;
            if (hull < 13.0) return "Tiny";
            if (hull < 25.0) return "Small";
            if (hull < 40.0) return "Medium";
            if (hull < 70.0) return "Large";
            if (hull < 100.0) return "Huge";
            return "Enormous";
        }
    }
}
