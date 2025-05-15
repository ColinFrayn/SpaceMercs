using OpenTK.Graphics.OpenGL;
using SpaceMercs.Graphics;
using System.Drawing.Imaging;
using System.IO;

namespace SpaceMercs {
    public struct TexDetails {
        public int ID;
        public int W, H;
    }
    public static class Textures {
        [Flags]
        public enum WallSide { UpLeft = 1, Up = 2, UpRight = 4, Left = 8, Right = 16, DownLeft = 32, Down = 64, DownRight = 128 }; // Do we need a wall on this side of the cell? (i.e. is the cell in this direction a floor tile?)
        public static WallSide AllSides = WallSide.UpLeft | WallSide.Up | WallSide.UpRight | WallSide.Left | WallSide.Right | WallSide.DownLeft | WallSide.Down | WallSide.DownRight;
        private static readonly Random rand = new Random();

        // Internal texture IDs
        private static int MiscTextureID = -1;
        private static int MiscTextureTransparentID = -1;
        private static int BuildingTextureID = -1;
        private static int ItemTextureID = -1;

        // Texture dimensions
        private static float MiscTextureWidth { get { return Textures.MiscTextureData is null ? 0 : (float)(Textures.TexSize - 1) / Textures.MiscTextureData.Width; } }
        private static float MiscTextureHeight { get { return Textures.MiscTextureData is null ? 0 : (float)(Textures.TexSize - 1) / Textures.MiscTextureData.Height; } }
        private static float BuildingTextureWidth { get { return Textures.BuildingTextureData is null ? 0 : (float)(Textures.TexSize - 1) / Textures.BuildingTextureData.Width; } }
        private static float BuildingTextureHeight { get { return Textures.BuildingTextureData is null ? 0 : (float)(Textures.TexSize - 1) / Textures.BuildingTextureData.Height; } }
        private static float ItemTextureWidth { get { return Textures.ItemTextureData is null ? 0 : (float)(Textures.TexSize - 1) / Textures.ItemTextureData.Width; } }
        private static float ItemTextureHeight { get { return Textures.ItemTextureData is null ? 0 : (float)(Textures.TexSize - 1) / Textures.ItemTextureData.Height; } }

        // Texture coordinates
        public enum MiscTexture { Build = 0, Salvage = 1, Up = 2, Right = 3, Cancel = 4, None = 5, Down = 6, Left = 7, Timer = 8, Connect = 9, Attack = 10, Search = 11, Disconnect = 12, Eye = 13, Inventory = 14, Skills = 15, Walk = 16,
                                  Unlock = 17, Lock = 18, Treasure = 19, Coins = 20, OpenDoor = 21, CloseDoor = 22, Bones = 23, Trap = 24, Alert = 25, Reuse = 26, Menu = 27, File = 28, Mission = 29, Stopwatch = 30, Moved = 31,
                                  FrameRed = 32, FrameRedThick = 33, FrameGreen = 34, FrameGreenThick = 35, Replace = 36, FullAttack = 37 };
        private const int TexSize = 64;
        public static TexSpecs GetTexCoords(MiscTexture tex, bool bTransparent = true) {
            if (bTransparent) {
                if (MiscTextureTransparentID == -1) {
                    SetupMiscTextureTransparent();
                    SetTextureParameters();
                }
            }
            else {
                if (MiscTextureID == -1) {
                    SetupMiscTexture();
                    SetTextureParameters();
                }
            }
            int xpos = (int)tex & 3, ypos = (int)tex / 4;
            if (Textures.MiscTextureData is null) throw new Exception("Misc texture data is null");
            float tx = (float)((xpos * Textures.TexSize) + 0.5) / Textures.MiscTextureData.Width;
            float ty = (float)((ypos * Textures.TexSize) + 0.5) / Textures.MiscTextureData.Height;
            return new(bTransparent ? MiscTextureTransparentID : MiscTextureID, tx, ty, MiscTextureWidth, MiscTextureHeight);
        }
        public static TexSpecs GetTexCoords(ShipEquipment se) {
            if (BuildingTextureID == -1) {
                SetupBuildingTexture();
                SetTextureParameters();
            }
            if (Textures.BuildingTextureData is null) throw new Exception("Building texture data is null");
            float tx = (float)((se.TextureX * Textures.TexSize) + 0.5) / Textures.BuildingTextureData.Width;
            float ty = (float)((se.TextureY * Textures.TexSize) + 0.5) / Textures.BuildingTextureData.Height;
            return new(BuildingTextureID, tx, ty, BuildingTextureWidth, BuildingTextureHeight);
        }
        public static TexSpecs GetTexCoords(ItemType it) {
            if (ItemTextureID == -1) {
                SetupItemTexture();
                SetTextureParameters();
            }
            if (it.TextureX == -1 || it.TextureY == -1) throw new Exception("Attempting to generate texture coordinates for item with no configured texture");
            if (Textures.ItemTextureData is null) throw new Exception("Item texture data is null");
            float tx = (float)((it.TextureX * Textures.TexSize) + 0.5) / Textures.ItemTextureData.Width;
            float ty = (float)((it.TextureY * Textures.TexSize) + 0.5) / Textures.ItemTextureData.Height;
            return new(ItemTextureID, tx, ty, ItemTextureWidth, ItemTextureHeight);
        }

        // Details for built-in BMP textures
        private readonly static string BuildingTextureFile = "Building.bmp";
        private readonly static string MiscTextureFile = "MiscTextures.bmp";
        private readonly static string ItemTextureFile = "ItemTextures.bmp";
        private static BitmapData? BuildingTextureData;
        private static BitmapData? MiscTextureData;
        private static BitmapData? ItemTextureData;

        // Halo textures
        public const int StarTextureSize = 128;
        public const int ShipHaloTextureSize = 128;
        public const int PlanetHaloTextureSize = 256;
        public static int iPlanetHalo = -1;
        public static byte[]? bytePlanetHalo;
        public static int iStarTexture = -1;
        public static int iShipHalo = -1;

        // Mission textures
        public const int TileSize = 32;

        // Initialise textures from the built-in BMPs
        public static void LoadTextureFiles(string strGraphicsDir) {
            string strBuildingTextureFile = Path.Combine(strGraphicsDir, BuildingTextureFile);
            if (!File.Exists(strBuildingTextureFile)) throw new Exception("Could not find Building Texture File!");
            Bitmap TextureBitmap1 = new Bitmap(strBuildingTextureFile);
            BuildingTextureData = TextureBitmap1.LockBits(new Rectangle(0, 0, TextureBitmap1.Width, TextureBitmap1.Height), ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            string strMiscTextureFile = Path.Combine(strGraphicsDir, MiscTextureFile);
            if (!File.Exists(strMiscTextureFile)) throw new Exception("Could not find Misc Texture File!");
            Bitmap TextureBitmap2 = new Bitmap(strMiscTextureFile);
            MiscTextureData = TextureBitmap2.LockBits(new Rectangle(0, 0, TextureBitmap2.Width, TextureBitmap2.Height), ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            string strItemTextureFile = Path.Combine(strGraphicsDir, ItemTextureFile);
            if (!File.Exists(strItemTextureFile)) throw new Exception("Could not find Item Texture File!");
            Bitmap TextureBitmap3 = new Bitmap(strItemTextureFile);
            ItemTextureData = TextureBitmap3.LockBits(new Rectangle(0, 0, TextureBitmap3.Width, TextureBitmap3.Height), ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
        }
        public static void SetTextureParameters() {
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);
        }

        #region Utility Textures
        private static int _fogOfWarTexture = -1;
        public static int FogOfWarTexture { get { if (_fogOfWarTexture == -1) _fogOfWarTexture = GenerateFogOfWarTexture(); return _fogOfWarTexture; } }
        private static int _selectionTexture = -1;
        public static int SelectionTexture { get { if (_selectionTexture == -1) _selectionTexture = GenerateSelectionTexture(); return _selectionTexture; } }

        private static int GenerateHighlightTexture_UNUSED() {
            byte[,,] image = new byte[Textures.TileSize, Textures.TileSize, 4];
            Color col = Color.FromArgb(255, 255, 100, 100);
            for (int y = 0; y < Textures.TileSize; y++) {
                for (int x = 0; x < Textures.TileSize; x++) {
                    image[x, y, 0] = col.R;
                    image[x, y, 1] = col.G;
                    image[x, y, 2] = col.B;
                    int dist = Math.Min(Math.Min(x, Textures.TileSize - x), Math.Min(y, Textures.TileSize - y));
                    image[x, y, 3] = (byte)Math.Max(0, (255 - dist * 40));
                }
            }
            return BindEntityTexture(image);
        }
        private static int GenerateHoverTexture_UNUSED() {
            byte[,,] image = new byte[Textures.TileSize, Textures.TileSize, 4];
            Color col = Color.FromArgb(255, 0, 255, 50);
            for (int y = 0; y < Textures.TileSize; y++) {
                for (int x = 0; x < Textures.TileSize; x++) {
                    image[x, y, 0] = col.R;
                    image[x, y, 1] = col.G;
                    image[x, y, 2] = col.B;
                    double dfract = Math.Max(0.0, Math.Sqrt((Textures.TileSize / 2 - x) * (Textures.TileSize / 2 - x) + (Textures.TileSize / 2 - y) * (Textures.TileSize / 2 - y)) * 4.0 / Textures.TileSize - 1.0);
                    image[x, y, 3] = (byte)Math.Max(0, 255 - (180 * dfract));
                    if (dfract > 1.0) image[x, y, 0] = image[x, y, 1] = image[x, y, 2] = image[x, y, 3] = 0;
                }
            }
            return BindEntityTexture(image);
        }
        private static int GenerateSelectionTexture() {
            byte[,,] image = new byte[Textures.TileSize, Textures.TileSize, 4];
            Color col = Color.FromArgb(255, 255, 50, 0);
            for (int y = 0; y < Textures.TileSize; y++) {
                for (int x = 0; x < Textures.TileSize; x++) {
                    image[x, y, 0] = col.R;
                    image[x, y, 1] = col.G;
                    image[x, y, 2] = col.B;
                    double dfract = Math.Max(0.0, Math.Sqrt((Textures.TileSize / 2 - x) * (Textures.TileSize / 2 - x) + (Textures.TileSize / 2 - y) * (Textures.TileSize / 2 - y)) * 4.0 / Textures.TileSize - 1.0);
                    image[x, y, 3] = (byte)Math.Max(0, 255 - (180 * dfract));
                    if (dfract > 1.0) image[x, y, 0] = image[x, y, 1] = image[x, y, 2] = image[x, y, 3] = 0;
                }
            }
            return BindEntityTexture(image);
        }
        private static int GenerateFogOfWarTexture() {
            byte[,,] image = new byte[Textures.TileSize, Textures.TileSize, 4];
            for (int y = 0; y < Textures.TileSize; y++) {
                for (int x = 0; x < Textures.TileSize; x++) {
                    byte val = 0x00;
                    if ((x % 4 == 0 || x % 4 == 1) && (y % 4 == 0 || y % 4 == 1)) val = 0xf0;
                    else if ((x % 4 == 2 || x % 4 == 3) && (y % 4 == 2 || y % 4 == 3)) val = 0xf0;
                    image[x, y, 0] = image[x, y, 1] = image[x, y, 2] = 0;
                    image[x, y, 3] = val; // 0 = Transparent background
                }
            }
            return BindEntityTexture(image);
        }
        #endregion // Utility Textures

        #region Wall And Floor Textures
        private enum WallType { Caves, Desert, Ice, Oceanic, Rocky, Volcanic, Ship, City };
        private static readonly Dictionary<(WallType, WallSide), TexDetails> dWallTextures = new Dictionary<(WallType, WallSide), TexDetails>();
        private static readonly Dictionary<WallType, TexDetails> dFloorTextures = new Dictionary<WallType, TexDetails>();
        public static TexDetails GenerateFloorTexture(MissionLevel lev) {
            WallType wt = WallTypeFromMissionLevel(lev);
            if (dFloorTextures.TryGetValue(wt, out TexDetails texDetails)) {
                return texDetails;
            }
            TexDetails newTexDet = BindTexture(GenerateFloorTextureMap(wt));
            dFloorTextures.Add(wt, newTexDet);
            return newTexDet;
        }
        private static byte[,,] GenerateFloorTextureMap(WallType wt) {
            switch (wt) {
                case WallType.Ship: return GenerateMetalFloorTile(Color.FromArgb(255, 180, 180, 180), 3, Textures.TileSize, Textures.TileSize);
                case WallType.Caves: return GenerateAlignedVarianceMap(Color.FromArgb(255, 150, 150, 150), 20, Textures.TileSize * 2, Textures.TileSize * 2);               
                case WallType.Desert: return GenerateAlignedVarianceMap(Color.FromArgb(255, 230, 210, 60), 15, Textures.TileSize * 2, Textures.TileSize * 2); // Sand
                case WallType.Ice: return GenerateAlignedVarianceMap(Color.FromArgb(255, 200, 210, 255), 15, Textures.TileSize * 2, Textures.TileSize * 2); // Ice/snow
                case WallType.Oceanic: return GenerateAlignedVarianceMap(Color.FromArgb(255, 70, 180, 90), 20, Textures.TileSize * 2, Textures.TileSize * 2); // Grass
                case WallType.Rocky: return GenerateAlignedVarianceMap(Color.FromArgb(255, 180, 180, 180), 20, Textures.TileSize * 2, Textures.TileSize * 2); // Rocks
                case WallType.Volcanic: return GenerateAlignedVarianceMap(Color.FromArgb(255, 200, 150, 140), 30, Textures.TileSize * 2, Textures.TileSize * 2); // Igneous
                case WallType.City: return GenerateStoneFloorTile(Textures.TileSize, Textures.TileSize);
            }
            throw new NotImplementedException();
        }
        public static TexDetails GenerateWallTexture(MissionLevel lev, WallSide ws) {
            WallType wt = WallTypeFromMissionLevel(lev);
            if (dWallTextures.ContainsKey((wt, ws))) return dWallTextures[(wt, ws)];
            byte[,,] baseImage = wt switch {
                WallType.Caves => FadeWallImage(GenerateRockWallTexture(Color.FromArgb(255, 150, 150, 150), Color.FromArgb(255, 160, 160, 160), 3, Textures.TileSize * 2, Textures.TileSize * 2, wt), ws),
                WallType.Desert => GenerateRockWallTexture(Color.FromArgb(255, 230, 210, 60), Color.FromArgb(255, 220, 160, 50), 3, Textures.TileSize * 2, Textures.TileSize * 2, wt),
                WallType.Ice => GenerateRockWallTexture(Color.FromArgb(255, 200, 210, 255), Color.FromArgb(255, 180, 190, 255), 3, Textures.TileSize * 2, Textures.TileSize * 2, wt),
                WallType.Oceanic => GenerateRockWallTexture(Color.FromArgb(255, 100, 255, 120), Color.FromArgb(255, 160, 160, 160), 3, Textures.TileSize * 2, Textures.TileSize * 2, wt),
                WallType.Rocky => GenerateRockWallTexture(Color.FromArgb(255, 180, 180, 180), Color.FromArgb(255, 180, 180, 180), 3, Textures.TileSize * 2, Textures.TileSize * 2, wt),
                WallType.Volcanic => GenerateRockWallTexture(Color.FromArgb(255, 200, 150, 140), Color.FromArgb(255, 160, 160, 160), 3, Textures.TileSize * 2, Textures.TileSize * 2, wt),
                WallType.Ship => GenerateMetalWall(Color.FromArgb(255, 150, 150, 150), 3, Textures.TileSize, Textures.TileSize, ws),
                WallType.City => GenerateBrickWall(Color.FromArgb(255, 132, 31, 29), Color.FromArgb(255, 90, 90, 80), 3, Textures.TileSize, Textures.TileSize, ws),
                _ => throw new NotImplementedException()
            };
            TexDetails TexID = BindTexture(baseImage);
            //if (TexID == null) TexID = BindTexture(GenerateAlignedVarianceMap(Color.FromArgb(255, 150, 150, 150), 10, Textures.TileSize, Textures.TileSize));
            dWallTextures.TryAdd((wt, ws), TexID);
            return TexID;
        }
        private static WallType WallTypeFromMissionLevel(MissionLevel lev) {
            if (lev.Type is Mission.MissionType.Caves or Mission.MissionType.Mines) return WallType.Caves;
            if (lev.Type is Mission.MissionType.Surface) {
                switch (lev.ParentMission.Location.Type) {
                    case Planet.PlanetType.Desert: return WallType.Desert;
                    case Planet.PlanetType.Ice: return WallType.Ice;
                    case Planet.PlanetType.Oceanic: return WallType.Oceanic;
                    case Planet.PlanetType.Rocky: return WallType.Rocky;
                    case Planet.PlanetType.Volcanic: return WallType.Volcanic;
                    case Planet.PlanetType.Precursor: return WallType.Oceanic;
                    default: throw new NotImplementedException();
                }
            }
            if (lev.ParentMission.IsShipMission || lev.Type == Mission.MissionType.SpaceHulk) return WallType.Ship;
            if (lev.Type is Mission.MissionType.AbandonedCity) return WallType.City;
            throw new NotImplementedException();
        }

        // Individual algorithms
        private static byte[,,] GenerateVarianceMap(Color col, int Range, int Width, int Height) {
            byte[,,] image = new byte[Width, Height, 3];
            for (int y = 0; y < Height; y++) {
                for (int x = 0; x < Width; x++) {
                    image[x, y, 0] = (byte)Math.Min(255, Math.Max(0, col.R + rand.Next(Range * 2 + 1) - Range));
                    image[x, y, 1] = (byte)Math.Min(255, Math.Max(0, col.G + rand.Next(Range * 2 + 1) - Range));
                    image[x, y, 2] = (byte)Math.Min(255, Math.Max(0, col.B + rand.Next(Range * 2 + 1) - Range));
                }
            }
            return image;
        }
        private static byte[,,] GenerateAlignedVarianceMap(Color col, int Range, int Width, int Height) {
            byte[,,] image = new byte[Width, Height, 3];
            for (int y = 0; y < Height; y++) {
                for (int x = 0; x < Width; x++) {
                    int r = rand.Next(Range * 2 + 1) - Range;
                    image[x, y, 0] = (byte)Math.Min(255, Math.Max(0, col.R + r));
                    image[x, y, 1] = (byte)Math.Min(255, Math.Max(0, col.G + r));
                    image[x, y, 2] = (byte)Math.Min(255, Math.Max(0, col.B + r));
                }
            }
            return image;
        }
        private static byte[,,] GenerateMetalFloorTile(Color col, int Range, int Width, int Height) {
            byte[,,] image = new byte[Width, Height, 3];
            for (int y = 0; y < Height; y++) {
                for (int x = 0; x < Width; x++) {
                    int mod = 0, var = Range;
                    if (x == 0 || y == 0) { mod = 20; var = Range / 2; }
                    else if (x == Width - 1 || y == Height - 1) { mod = -20; var = Range / 2; }
                    else if ((x == 3 || x == Width - 4) && (y == 3 || y == Height - 4)) { mod = -60; var = Range / 2; }
                    image[x, y, 0] = (byte)Math.Min(255, Math.Max(0, col.R + mod + rand.Next(var * 2 + 1) - var));
                    image[x, y, 1] = (byte)Math.Min(255, Math.Max(0, col.G + mod + rand.Next(var * 2 + 1) - var));
                    image[x, y, 2] = (byte)Math.Min(255, Math.Max(0, col.B + mod + rand.Next(var * 2 + 1) - var));
                }
            }
            return image;
        }
        private static byte[,,] GenerateStoneFloorTile(int Width, int Height) {
            byte[,,] image = new byte[Width, Height, 3];
            int var = 3, stonex = 6, gap = 2, stoney = 6;
            Color col = Color.FromArgb(255, 170, 170, 170);
            for (int y = 0; y < Height; y++) {
                for (int x = 0; x < Width; x++) {
                    int mod = 0;
                    int xpos = x % (stonex + gap);
                    if (xpos == 0 || xpos == (stonex + gap - 1)) mod = -50;
                    int colno = x / (stonex + gap);
                    int ypos = (y + (colno * ((stoney + gap) / 2))) % (stoney + gap);
                    if (ypos == 0 || ypos == (stoney + gap - 1)) mod = -50;
                    image[x, y, 0] = (byte)Math.Min(255, Math.Max(0, col.R + mod + rand.Next(var * 2 + 1) - var));
                    image[x, y, 1] = (byte)Math.Min(255, Math.Max(0, col.G + mod + rand.Next(var * 2 + 1) - var));
                    image[x, y, 2] = (byte)Math.Min(255, Math.Max(0, col.B + mod + rand.Next(var * 2 + 1) - var));
                }
            }
            return image;
        }
        private static byte[,,] GenerateMetalWall(Color col, int Range, int Width, int Height, WallSide ws) {
            byte[,,] image = new byte[Width, Height, 3];
            for (int y = 0; y < Height; y++) {
                for (int x = 0; x < Width; x++) {
                    int var = Range;
                    int d = DistanceFromNearestFloor(x % Textures.TileSize, y % Textures.TileSize, ws);
                    int mod = (d > 10 ? -1000 : -40);
                    image[x, y, 0] = (byte)Math.Min(255, Math.Max(0, col.R + mod + rand.Next(var * 2 + 1) - var));
                    image[x, y, 1] = (byte)Math.Min(255, Math.Max(0, col.G + mod + rand.Next(var * 2 + 1) - var));
                    image[x, y, 2] = (byte)Math.Min(255, Math.Max(0, col.B + mod + rand.Next(var * 2 + 1) - var));
                }
            }
            return image;
        }
        private static byte[,,] GenerateRockWallTexture(Color colBase, Color colWall, int Range, int Width, int Height, WallType wt) {
            byte[,,] image = GenerateFloorTextureMap(wt);
            int scale = 12;
            for (int imx = 0; imx < (Width / Textures.TileSize); imx++) {
                for (int imy = 0; imy < (Height / Textures.TileSize); imy++) {
                    for (int n = 0; n < (Textures.TileSize * Textures.TileSize / 10); n++) {
                        int psize2 = 6 + rand.Next(12) + rand.Next(12);
                        double ex = 1.0 + (rand.NextDouble() - rand.NextDouble()) / 4.0;
                        double ey = 1.0 + (rand.NextDouble() - rand.NextDouble()) / 4.0;
                        int psizex = (int)(Math.Sqrt(psize2) * ex);
                        int psizey = (int)(Math.Sqrt(psize2) * ey);
                        int cx = rand.Next(Textures.TileSize - (psizex * 2)) + psizex;
                        int cy = rand.Next(Textures.TileSize - (psizey * 2)) + psizey;
                        int colmod = rand.Next(scale) + rand.Next(scale);
                        Color pcol = Color.FromArgb(255, Math.Max(0, colWall.R - colmod), Math.Max(0, colWall.G - colmod), Math.Max(0, colWall.B - colmod));
                        for (int y = cy - psizey; y <= cy + psizey; y++) {
                            for (int x = cx - psizex; x <= cx + psizex; x++) {
                                double r2 = ((x - cx) * (x - cx) / (ex * ex)) + ((y - cy) * (y - cy) / (ey * ey));
                                if (r2 < psize2) {
                                    double r = Math.Sqrt(r2) / Math.Sqrt(psize2);
                                    double rmod = 1.0;
                                    if (r > 0.4) rmod = Math.Pow(0.8, (r - 0.4) * 3);
                                    image[x + (imx * Textures.TileSize), y + (imy * Textures.TileSize), 0] = (byte)(pcol.R * rmod);
                                    image[x + (imx * Textures.TileSize), y + (imy * Textures.TileSize), 1] = (byte)(pcol.G * rmod);
                                    image[x + (imx * Textures.TileSize), y + (imy * Textures.TileSize), 2] = (byte)(pcol.B * rmod);
                                }
                            }
                        }
                    }
                }
            }
            return image;
        }
        private static byte[,,] GenerateBrickWall(Color colBrick, Color colMortar, int Range, int Width, int Height, WallSide ws) {
            byte[,,] image = new byte[Width, Height, 3];
            int var = Range, brickl = 7, brickw = 4, wallwidth = brickw * 2 + 3;
            for (int y = 0; y < Height; y++) {
                for (int x = 0; x < Width; x++) {
                    Color col = Color.Black;
                    int d = DistanceFromNearestFloor(x % Textures.TileSize, y % Textures.TileSize, ws);

                    if ((ws & WallSide.UpLeft) == WallSide.UpLeft && x < wallwidth && y < wallwidth) {
                        col = colBrick;
                        if (x == 0 || x == (brickw + 1) || x == wallwidth - 1) col = colMortar;
                        if (y == 0 || y == (brickw + 1) || y == wallwidth - 1) col = colMortar;
                    }
                    if ((ws & WallSide.UpRight) == WallSide.UpRight && x >= Width - wallwidth && y < wallwidth) {
                        col = colBrick;
                        if (x == Width - 1 || x == Width - (brickw + 2) || x == Width - wallwidth) col = colMortar;
                        if (y == 0 || y == (brickw + 1) || y == wallwidth - 1) col = colMortar;
                    }
                    if ((ws & WallSide.DownLeft) == WallSide.DownLeft && x < wallwidth && y >= Height - wallwidth) {
                        col = colBrick;
                        if (x == 0 || x == (brickw + 1) || x == wallwidth - 1) col = colMortar;
                        if (y == Height - 1 || y == Height - (brickw + 2) || y == Height - wallwidth) col = colMortar;
                    }
                    if ((ws & WallSide.DownRight) == WallSide.DownRight && x >= Width - wallwidth && y >= Height - wallwidth) {
                        col = colBrick;
                        if (x == Width - 1 || x == Width - (brickw + 2) || x == Width - wallwidth) col = colMortar;
                        if (y == Height - 1 || y == Height - (brickw + 2) || y == Height - wallwidth) col = colMortar;
                    }
                    if ((ws & WallSide.Left) == WallSide.Left && x < wallwidth) {
                        col = colBrick;
                        int brickpos = (y + ((x > brickw + 1) ? 4 : 0)) % (brickl + 1);
                        if (x == 0 || x == (brickw + 1) || x == wallwidth - 1 || brickpos == brickl) col = colMortar;
                    }
                    if ((ws & WallSide.Right) == WallSide.Right && x >= Width - wallwidth) {
                        col = colBrick;
                        int brickpos = (y + (((Width - 1 - x) > brickw + 1) ? 4 : 0)) % (brickl + 1);
                        if (x == Width - 1 || x == Width - (brickw + 2) || x == Width - wallwidth || brickpos == brickl) col = colMortar;
                    }
                    if ((ws & WallSide.Up) == WallSide.Up && y < wallwidth) {
                        col = colBrick;
                        int brickpos = (x + ((y > brickw + 1) ? 4 : 0)) % (brickl + 1);
                        if (y == 0 || y == (brickw + 1) || y == wallwidth - 1 || brickpos == brickl) col = colMortar;
                    }
                    if ((ws & WallSide.Down) == WallSide.Down && y >= Height - wallwidth) {
                        col = colBrick;
                        int brickpos = (x + (((Height - 1 - y) > brickw + 1) ? 4 : 0)) % (brickl + 1);
                        if (y == Height - 1 || y == Height - (brickw + 2) || y == Height - wallwidth || brickpos == brickl) col = colMortar;
                    }
                    image[x, y, 0] = (byte)Math.Min(255, Math.Max(0, col.R + rand.Next(var * 2 + 1) - var));
                    image[x, y, 1] = (byte)Math.Min(255, Math.Max(0, col.G + rand.Next(var * 2 + 1) - var));
                    image[x, y, 2] = (byte)Math.Min(255, Math.Max(0, col.B + rand.Next(var * 2 + 1) - var));
                }
            }
            return image;
        }
        private static byte[,,] SmoothMap(byte[,,] map, int size) {
            int Width = map.GetLength(0), Height = map.GetLength(1);
            double[,,] image = new double[Width, Height, 3];
            for (int y = 0; y < Height; y++) {
                for (int x = 0; x < Width; x++) {
                    image[x, y, 0] = image[x, y, 1] = image[x, y, 2] = 0.0;
                    double dTotal = 0.0;
                    for (int yy = Math.Max(0, y - size); yy <= Math.Min(Height - 1, y + size); yy++) {
                        for (int xx = Math.Max(0, x - size); xx <= Math.Min(Width - 1, x + size); xx++) {
                            double dDist2 = Math.Abs(x - xx) * Math.Abs(x - xx) + Math.Abs(y - yy) * Math.Abs(y - yy);
                            double dWeight = Math.Exp(-dDist2 / 2);
                            image[x, y, 0] += map[xx, yy, 0] * dWeight;
                            image[x, y, 1] += map[xx, yy, 1] * dWeight;
                            image[x, y, 2] += map[xx, yy, 2] * dWeight;
                            dTotal += dWeight;
                        }
                    }
                    image[x, y, 0] /= dTotal;
                    image[x, y, 1] /= dTotal;
                    image[x, y, 2] /= dTotal;
                }
            }
            for (int y = 0; y < Height; y++) {
                for (int x = 0; x < Width; x++) {
                    map[x, y, 0] = (byte)image[x, y, 0];
                    map[x, y, 1] = (byte)image[x, y, 1];
                    map[x, y, 2] = (byte)image[x, y, 2];
                }
            }
            return map;
        }
        private static byte[,,] FadeWallImage(byte[,,] image, WallSide ws) {
            int Width = image.GetLength(0), Height = image.GetLength(1);
            byte[,,] newImage = new byte[Width, Height, 3];
            for (int y = 0; y < Height; y++) {
                for (int x = 0; x < Width; x++) {
                    int d = DistanceFromNearestFloor(x % Textures.TileSize, y % Textures.TileSize, ws);
                    int mod = (d < 16 ? 0 : (d - 15));
                    newImage[x, y, 0] = (byte)Math.Max(0, ((double)image[x, y, 0] * Math.Pow(0.9, mod) - (mod * 6)));
                    newImage[x, y, 1] = (byte)Math.Max(0, ((double)image[x, y, 1] * Math.Pow(0.9, mod) - (mod * 6)));
                    newImage[x, y, 2] = (byte)Math.Max(0, ((double)image[x, y, 2] * Math.Pow(0.9, mod) - (mod * 6)));
                }
            }
            return newImage;
        }
        #endregion // Wall And Floor Textures

        // ----- Creature/Soldier textures
        public static int GenerateSoldierTexture(Soldier s) {
            byte[,,] image = new byte[Textures.TileSize, Textures.TileSize, 4];
            for (int y = 0; y < Textures.TileSize; y++) {
                for (int x = 0; x < Textures.TileSize; x++) {
                    image[x, y, 3] = 0; // Transparent background
                }
            }
            DrawHumanoid(image, Color.FromArgb(255, 180, 140, 100), s.PrimaryColor, Color.FromArgb(255, 200, 180, 160));
            if (s.Shields > 0.0) AddHaloToImage(image, Color.FromArgb(255, 60, 90, 255));
            return BindEntityTexture(image);
        }
        public static int GenerateDefaultCreatureTexture(CreatureType tp, bool bShields) {
            byte[,,] image = new byte[Textures.TileSize, Textures.TileSize, 4];
            for (int y = 0; y < Textures.TileSize; y++) {
                for (int x = 0; x < Textures.TileSize; x++) {
                    image[x, y, 3] = 0; // Transparent background
                }
            }
            DrawCircleOnTexture(image, Color.Purple, 15.5, 15.5, 10, 10);
            // Rotate it so that up is forward (currently Right = forward)
            // We'll get rid of most of this soon anyway
            byte[,,] imageRot = new byte[Textures.TileSize, Textures.TileSize, 4];
            for (int y = 0; y < Textures.TileSize; y++) {
                for (int x = 0; x < Textures.TileSize; x++) {
                    for (int n = 0; n <= 3; n++) { 
                        imageRot[x, y, 3] = image[Textures.TileSize - y - 1, x, n]; // Rotate 90 degrees CW
                    }
                }
            }
            return BindEntityTexture(imageRot);
        }
        private static void DrawHumanoid(byte[,,] image, Color cFeet, Color cBody, Color cHead) {
            // Feet
            DrawCircleOnTexture(image, cFeet, 10, 12.5, 3.5, 5);
            DrawCircleOnTexture(image, cFeet, 21, 12.5, 3.5, 5);
            // Body
            DrawCircleOnTexture(image, cBody, 15.5, 15.5, 12, 6);
            // Head
            DrawCircleOnTexture(image, cHead, 15.5, 16, 7, 7);
        }

        #region Utility Methods
        private static TexDetails BindTexture(byte[,,] image) {
            int texID;
            GL.Enable(EnableCap.Texture2D);
            texID = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, texID);
            GL.TexEnv(TextureEnvTarget.TextureEnv, TextureEnvParameter.TextureEnvMode, (float)TextureEnvMode.Modulate);
            SetTextureParameters();
            // Flip the image. For reasons I don't understand.
            int Width = image.GetLength(0), Height = image.GetLength(1);
            byte[,,] newImage = new byte[Height, Width, 3];
            for (int y = 0; y < Height; y++) {
                for (int x = 0; x < Width; x++) {
                    newImage[y, x, 0] = image[x, y, 0];
                    newImage[y, x, 1] = image[x, y, 1];
                    newImage[y, x, 2] = image[x, y, 2];
                }
            }
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, Height, Width, 0, OpenTK.Graphics.OpenGL.PixelFormat.Rgb, PixelType.UnsignedByte, newImage);
            return new TexDetails() { ID = texID, W = newImage.GetLength(0), H = newImage.GetLength(1) };
        }
        private static int BindEntityTexture(byte[,,] image) {
            int texID;
            GL.Enable(EnableCap.Texture2D);
            texID = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, texID);
            GL.TexEnv(TextureEnvTarget.TextureEnv, TextureEnvParameter.TextureEnvMode, (float)TextureEnvMode.Modulate);
            SetTextureParameters();
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, image.GetLength(0), image.GetLength(1), 0, OpenTK.Graphics.OpenGL.PixelFormat.Rgba, PixelType.UnsignedByte, image);
            return texID;
        }
        private static int DistanceFromNearestFloor(int xx, int yy, WallSide ws) {
            int d = 100;
            if ((ws & WallSide.Left) == WallSide.Left) d = Math.Min(d, xx);
            if ((ws & WallSide.Right) == WallSide.Right) d = Math.Min(d, (Textures.TileSize - xx) - 1);
            if ((ws & WallSide.Up) == WallSide.Up) d = Math.Min(d, yy);
            if ((ws & WallSide.Down) == WallSide.Down) d = Math.Min(d, (Textures.TileSize - yy) - 1);
            if ((ws & WallSide.UpLeft) == WallSide.UpLeft) d = Math.Min(d, Math.Max(xx, yy));
            if ((ws & WallSide.UpRight) == WallSide.UpRight) d = Math.Min(d, Math.Max((Textures.TileSize - xx) - 1, yy));
            if ((ws & WallSide.DownLeft) == WallSide.DownLeft) d = Math.Min(d, Math.Max(xx, (Textures.TileSize - yy) - 1));
            if ((ws & WallSide.DownRight) == WallSide.DownRight) d = Math.Min(d, Math.Max((Textures.TileSize - xx) - 1, (Textures.TileSize - yy) - 1));
            return d;
        }
        private static void DrawCircleOnTexture(byte[,,] image, Color col, double xpos, double ypos, double rx, double ry) {
            for (int y = (int)Math.Floor(ypos - ry); y <= (int)Math.Ceiling(ypos + ry); y++) {
                for (int x = (int)Math.Floor(xpos - rx); x <= (int)Math.Ceiling(xpos + rx); x++) {
                    double dx = (xpos - x) / rx;
                    double dy = (ypos - y) / ry;
                    double r2 = (dx * dx) + (dy * dy);
                    if (r2 <= 1.0) {
                        image[x, y, 0] = (byte)col.R;
                        image[x, y, 1] = (byte)col.G;
                        image[x, y, 2] = (byte)col.B;
                        image[x, y, 3] = (byte)col.A;
                    }
                }
            }
        }
        private static void SetupMiscTexture() {
            MiscTextureID = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, MiscTextureID);
            GL.TexEnv(TextureEnvTarget.TextureEnv, TextureEnvParameter.TextureEnvMode, (float)TextureEnvMode.Modulate);
            if (MiscTextureData is null) throw new Exception("Misc Texture Data was null");
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, MiscTextureData.Width, MiscTextureData.Height, 0, OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, MiscTextureData.Scan0);
            SetTextureParameters();
        }
        private static void SetupMiscTextureTransparent() {
            MiscTextureTransparentID = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, MiscTextureTransparentID);
            GL.TexEnv(TextureEnvTarget.TextureEnv, TextureEnvParameter.TextureEnvMode, (float)TextureEnvMode.Modulate);
            if (MiscTextureData is null) throw new Exception("Misc Texture Data was null");
            int texture_size = MiscTextureData.Width * MiscTextureData.Height * 4;
            byte[] bytes = new byte[texture_size];
            System.Runtime.InteropServices.Marshal.Copy(MiscTextureData.Scan0, bytes, 0, texture_size);
            for (int i = 0; i < MiscTextureData.Width * MiscTextureData.Height; i++) {
                if (bytes[i * 4 + 0] > 242 && bytes[i * 4 + 1] > 242 && bytes[i * 4 + 2] > 242) bytes[i * 4 + 3] = 0; // Make white-ish colours all transparent
            }
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, MiscTextureData.Width, MiscTextureData.Height, 0, OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, bytes);
            SetTextureParameters();
        }
        private static void SetupBuildingTexture() {
            BuildingTextureID = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, BuildingTextureID);
            GL.TexEnv(TextureEnvTarget.TextureEnv, TextureEnvParameter.TextureEnvMode, (float)TextureEnvMode.Modulate);
            if (BuildingTextureData is null) throw new Exception("Building Texture Data was null");
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, BuildingTextureData.Width, BuildingTextureData.Height, 0, OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, BuildingTextureData.Scan0);
            SetTextureParameters();
        }
        private static void SetupItemTexture() {
            ItemTextureID = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, ItemTextureID);
            GL.TexEnv(TextureEnvTarget.TextureEnv, TextureEnvParameter.TextureEnvMode, (float)TextureEnvMode.Modulate);
            if (ItemTextureData is null) throw new Exception("Item Texture Data was null");
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, ItemTextureData.Width, ItemTextureData.Height, 0, OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, ItemTextureData.Scan0);
        }
        private static void AddHaloToImage(byte[,,] image, Color col) {
            int width = image.GetLength(0);
            int height = image.GetLength(1);
            bool[,] bMask = new bool[width, height];
            for (int y = 0; y < height; y++) {
                for (int x = 0; x < width; x++) {
                    bMask[x, y] = (image[x, y, 3] > 0);
                }
            }
            // Yes, this bit is crazy inefficient
            for (int y = 0; y < height; y++) {
                for (int x = 0; x < width; x++) {
                    if (!bMask[x, y]) { // Transparent pixel
                        if (x > 0 && bMask[x - 1, y]) continue;
                        if (y > 0 && bMask[x, y - 1]) continue;
                        if (x < width - 1 && bMask[x + 1, y]) continue;
                        if (y < height - 1 && bMask[x, y + 1]) continue;
                        int count = 0;
                        for (int yy = Math.Max(0, y - 3); yy <= Math.Min(height - 1, y + 3); yy++) {
                            for (int xx = Math.Max(0, x - 3); xx <= Math.Min(width - 1, x + 3); xx++) {
                                if (bMask[xx, yy]) count++;
                            }
                        }
                        if (count > 0) {
                            if (count > 10) count = 10;
                            image[x, y, 0] = (byte)col.R;
                            image[x, y, 1] = (byte)col.G;
                            image[x, y, 2] = (byte)col.B;
                            image[x, y, 3] = (byte)((col.A * count) / 10);
                        }
                    }
                }
            }
        }
        #endregion // Utility Methods
    }
}

