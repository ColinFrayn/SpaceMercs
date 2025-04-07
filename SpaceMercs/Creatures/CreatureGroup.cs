using OpenTK.Graphics.OpenGL;
using SpaceMercs.Graphics;
using System.Drawing.Imaging;
using System.Xml;

namespace SpaceMercs {
    public class CreatureGroup {
        public string Name { get; private set; }
        public string Filename { get; private set; }
        public bool FoundInShips { get; private set; }
        public bool RaceSpecific { get; private set; } // True = this is a group of creatures that must be assigned to a specific alien race. False = This is just random creatures/unknown aliens
        public double QuantityScale { get; private set; } // Multiple of the standard number of creatures found in a level
        public int MinSectorRange { get; private set; } // Cannot be found in sectors less than this range away from human home
        public HashSet<Planet.PlanetType> FoundIn { get; private set; }
        public int MaxRelations { get; private set; }  // Won't show up against any race with which racial relations are better than this value.
        public readonly List<CreatureType> CreatureTypes = new List<CreatureType>();
        public bool HasBoss { get { return (Boss != null); } }
        public CreatureType? Boss {
            get {
                foreach (CreatureType tp in CreatureTypes) {
                    if (tp.IsBoss) return tp;
                }
                return null;
            }
        }
        private readonly Random rand; // Let's set this here because it's easier than passing one through (and just setting it each time risks getting the exact same seed if insufficient time has passed)
        private BitmapData? TextureData;
        private int iTextureId = -1;
        public bool HasTexture => TextureData != null;

        public CreatureGroup(XmlNode xml) {
            Name = xml.Attributes!["Name"]?.InnerText ?? throw new Exception("Missing Name for creature group");
            Filename = xml.SelectNodeText("Filename");
            string strLocation = xml.SelectNodeText("Locations");
            HashSet<string> Locs = new HashSet<string>(strLocation.Split(',').ToList());
            FoundIn = new HashSet<Planet.PlanetType>();
            FoundInShips = Locs.Contains("Ship");
            RaceSpecific = (xml.SelectSingleNode("Racial") is not null);
            QuantityScale = xml.SelectNodeDouble("QuantityScale", 1.0);
            MinSectorRange = xml.SelectNodeInt("MinSectorRange", 0);
            if (RaceSpecific) {
                string strMaxRel = xml.SelectSingleNode("Racial")?.Attributes?["MaxRelations"]?.Value ?? string.Empty;
                if (!string.IsNullOrEmpty(strMaxRel)) MaxRelations = int.Parse(strMaxRel);
                else MaxRelations = 100; // Ignore
            }
            foreach (Planet.PlanetType pt in Enum.GetValues(typeof(Planet.PlanetType))) {
                if (Locs.Contains(pt.ToString())) FoundIn.Add(pt);
            }
            rand = new Random();
        }

        public Creature? GenerateRandomBoss(Race? ra, int diff, MissionLevel lev) {
            CreatureType? tp = Boss;

            // Generate a suitable level
            if (tp is null || tp.LevelMin > diff) return null; // Can't make one
            int lvl = diff;
            if (rand.NextDouble() < 0.2 && lvl > tp.LevelMin) lvl--;
            if (rand.NextDouble() < 0.2 && lvl < tp.LevelMax) lvl++;

            return new Creature(tp, lvl, lev, ra);
        }
        public Creature GenerateRandomCreature(Race? ra, int diff, MissionLevel lev) {
            // Get a creature type at random that's suitable for the difficulty and not the boss
            CreatureType tp = GetRandomNonBossCreatureType(diff);

            // Generate a suitable level
            int lvl = diff;
            if (rand.NextDouble() < 0.2 && lvl > tp.LevelMin) lvl--;
            if (rand.NextDouble() < 0.2 && lvl < tp.LevelMax) lvl++;

            return new Creature(tp, lvl, lev, ra);
        }
        private CreatureType GetRandomNonBossCreatureType(int diff) {
            int total = 0;
            Dictionary<CreatureType, int> Candidates = new();

            foreach (CreatureType tp in CreatureTypes) {
                if (!tp.IsBoss && tp.LevelMin <= diff && tp.LevelMax >= diff) {
                    total += tp.FrequencyModifier;
                    Candidates.Add(tp,tp.FrequencyModifier);
                }
            }
            int pick = rand.Next(total);
            foreach (CreatureType tp in Candidates.Keys) {
                pick -= Candidates[tp];
                if (pick < 0) return tp;
            }
            throw new Exception("Could not find random creature fitting requirements");
        }

        public void SetTextureBitmap(Bitmap bmp) {
            TextureData = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
        }
        public TexSpecs? GetTexDetails(int x, int y, int sz) {
            if (TextureData is null || x == -1 || y == -1 || sz == -1) return null;
            if (iTextureId == -1) {
                iTextureId = GL.GenTexture();
                GL.BindTexture(TextureTarget.Texture2D, iTextureId);
                GL.TexEnv(TextureEnvTarget.TextureEnv, TextureEnvParameter.TextureEnvMode, (float)TextureEnvMode.Modulate);

                // Make white colours all transparent
                int texture_size = TextureData.Width * TextureData.Height * 4;
                byte[] bytes = new byte[texture_size];
                System.Runtime.InteropServices.Marshal.Copy(TextureData.Scan0, bytes, 0, texture_size);
                for (int i = 0; i < TextureData.Width * TextureData.Height; i++) {
                    if (bytes[i * 4 + 0] == 255 && bytes[i * 4 + 1] == 255 && bytes[i * 4 + 2] == 255) bytes[i * 4 + 3] = 0; 
                }
                GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, TextureData.Width, TextureData.Height, 0, OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, bytes);

                Textures.SetTextureParameters();
            }
            float tx = (float)x / ((float)TextureData.Width / 64f);
            float ty = (float)y / ((float)TextureData.Height / 64f);
            float tw = (float)sz / ((float)TextureData.Width / 64f);
            float th = (float)sz / ((float)TextureData.Height / 64f);
            return new TexSpecs(iTextureId, tx, ty, tw, th);
        }
    }
}