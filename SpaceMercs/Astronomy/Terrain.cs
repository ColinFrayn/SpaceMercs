using OpenTK.Mathematics;

namespace SpaceMercs {
    static class Terrain {
        private static double[,,] dSeedMap;
        private static double[] dWeights = { 0.6, 0.8, 1.0, 0.8, 0.6, 0.4, 0.20, 0.14, 0.1 };
        public delegate Vector3 HeightToColor(int height);
        private static Vector3 col1, col2, col3, col4, cdiff1, cdiff2;

        // Generate the new map
        public static byte[] GenerateMap(AstronomicalObject ao, int width, int height) {
            int[,] iMap = new int[width, height];
            Random rnd = new Random(ao.Seed);
            HeightToColor htc;
            Planet.PlanetType pt = Planet.PlanetType.Star;

            // Get the anchor colours for the map
            if (ao.AOType == AstronomicalObject.AstronomicalObjectType.Star) {
                Star st = (Star)ao;
                col1 = Vector3.Multiply(st.colour, 0.7f);
                col2 = st.colour;
                col3 = st.colour;
                col4 = Vector3.Multiply(st.colour, 0.9f);
                htc = HeightToColor_Default;
            }
            else {
                if (ao.AOType == AstronomicalObject.AstronomicalObjectType.Planet) pt = ((Planet)ao).Type;
                else pt = ((Moon)ao).Type;
                col1 = Const.PlanetTypeToCol1(pt);
                col2 = Const.PlanetTypeToCol2(pt);
                col3 = Const.PlanetTypeToCol3(pt);
                col4 = Const.PlanetTypeToCol4(pt);
                if (pt == Planet.PlanetType.Oceanic) htc = HeightToColor_Oceanic;
                else htc = HeightToColor_Default;
            }
            cdiff1 = Vector3.Subtract(col2, col1);
            cdiff2 = Vector3.Subtract(col4, col3);

            // Get base displacement
            double dBase = dSeedMap[ao.Ox, ao.Oy, ao.Oz] + dSeedMap[ao.Ox + 1, ao.Oy, ao.Oz] + dSeedMap[ao.Ox, ao.Oy + 1, ao.Oz] + dSeedMap[ao.Ox + 1, ao.Oy + 1, ao.Oz];
            dBase += dSeedMap[ao.Ox, ao.Oy, ao.Oz + 1] + dSeedMap[ao.Ox + 1, ao.Oy, ao.Oz + 1] + dSeedMap[ao.Ox, ao.Oy + 1, ao.Oz + 1] + dSeedMap[ao.Ox + 1, ao.Oy + 1, ao.Oz + 1];
            dBase *= Const.TerrainScale / 8.0;

            // Do fractal terrain generation, starting with the whole map
            //System.Diagnostics.Stopwatch sw = System.Diagnostics.Stopwatch.StartNew()
            PerlinMap(iMap, width, height, ao.Ox, ao.Oy, ao.Oz);
            //sw.Stop();

            // Add finishing touches
            Finishing(iMap, rnd, pt, width, height);

            // Convert the height map to a texture
            return BuildTexture(iMap, htc, (int)dBase, width, height);
        }

        // Convert height map to a texture
        private static byte[] BuildTexture(int[,] iMap, HeightToColor htc, int iBase, int width, int height) {
            // Generate the coloured texture map from the height map
            byte[] tex = new byte[width * height * 3];
            int tval = 0;
            for (int y = 0; y < height; y++) {
                for (int x = 0; x < width; x++) {
                    int val = iMap[x, y] - iBase;
                    Vector3 col = htc(val);
                    tex[tval + 0] = (byte)(col.X * 255.0);
                    tex[tval + 1] = (byte)(col.Y * 255.0);
                    tex[tval + 2] = (byte)(col.Z * 255.0);
                    tval += 3;
                }
            }
            return tex;
        }

        // Generate map using Perlin Noise in 3D
        private static void PerlinMap(int[,] iMap, int width, int height, int Ox, int Oy, int Oz) {
            double pw = Math.PI / (double)width;
            for (int y = 0; y < height; y++) {
                double yy = (double)y * Math.PI / (double)height;
                double pz = Math.Cos(yy);
                double sy = Math.Sin(yy);
                double xx = 0;
                for (int x = 0; x < width; x++) {
                    double px = sy * Math.Sin(2 * xx);
                    double py = sy * Math.Cos(2 * xx);
                    iMap[x, y] = GetTerrainHeight(px, py, pz, Ox, Oy, Oz);
                    xx += pw;
                }
            }
        }

        // Generate Perlin noise seed map
        public static void GenerateSeedMap() {
            Random rnd = new Random();
            int SeedSize = (1 << (Const.PerlinOctaves)) + Const.SeedBuffer + 1;
            dSeedMap = new double[SeedSize, SeedSize, SeedSize];
            for (int x = 0; x < SeedSize; x++) {
                for (int y = 0; y < SeedSize; y++) {
                    for (int z = 0; z < SeedSize; z++) {
                        dSeedMap[x, y, z] = Utils.NextGaussian(rnd, 0.0, 1.0);
                    }
                }
            }
        }

        // Get terrain height for a 3D co-ordinate
        private static int GetTerrainHeight(double px, double py, double pz, int Ox, int Oy, int Oz) {
            double p = 0.0;
            px = (px + 1.0) / 2.0;
            py = (py + 1.0) / 2.0;
            pz = (pz + 1.0) / 2.0;
            for (int octave = 0; octave <= Const.PerlinOctaves; octave++) {
                p += dWeights[octave] * Noise(px + (double)Ox, py + (double)Oy, pz + (double)Oz);
                px *= 2.0;
                py *= 2.0;
                pz *= 2.0;
            }
            return (int)(p * Const.TerrainScale);
        }

        // Noise function for Perlin noise map
        private static double Noise(double px, double py, double pz) {
            int x = (int)px;
            int y = (int)py;
            int z = (int)pz;
            double dx = px - (double)x;
            double dy = py - (double)y;
            double dz = pz - (double)z;
            // z0 plane value
            double x0 = dSeedMap[x, y, z] + (dSeedMap[x + 1, y, z] - dSeedMap[x, y, z]) * dx;
            double x1 = dSeedMap[x, y + 1, z] + (dSeedMap[x + 1, y + 1, z] - dSeedMap[x, y + 1, z]) * dx;
            double v0 = x0 + (x1 - x0) * dy;
            // z1 plane value
            x0 = dSeedMap[x, y, z + 1] + (dSeedMap[x + 1, y, z + 1] - dSeedMap[x, y, z + 1]) * dx;
            x1 = dSeedMap[x, y + 1, z + 1] + (dSeedMap[x + 1, y + 1, z + 1] - dSeedMap[x, y + 1, z + 1]) * dx;
            double v1 = x0 + (x1 - x0) * dy;
            // Interpolate
            return v0 + (v1 - v0) * dz;
        }

        // Add finishing touches to planet textures
        private static void Finishing(int[,] iMap, Random rnd, Planet.PlanetType pt, int width, int height) {
            if (width < 32) return; // Not worth it

            if (pt == Planet.PlanetType.Gas) {
                AddGasGiantCloudFeatures(iMap, rnd);
            }

            // Add craters
            //int nCrater = 0;
            //if (pt == Enums.PlanetType.Ice) nCrater = 3 + rnd.Next(4) + rnd.Next(4);
            //if (pt == Enums.PlanetType.Rocky) nCrater = 4 + rnd.Next(7) + rnd.Next(7) + rnd.Next(7);
            //if (pt == Enums.PlanetType.Volcanic) nCrater = 2 + rnd.Next(4) + rnd.Next(4);
            //for (int n = 0; n < nCrater; n++) {
            //  int cSize = rnd.Next(10) + rnd.Next(10) + rnd.Next(10) + 1;
            //  double cRad = (double)cSize * (double)MapHeight / 2000.0;
            //  int cx = rnd.Next(MapWidth);
            //  int cy = rnd.Next(MapHeight * 3 / 4) + (MapHeight / 8);
            //  int cr = (int)(cRad * 1.3);
            //  int cDepth = cSize * 5 + rnd.Next(15) - rnd.Next(15);
            //  if (cDepth < 1) cDepth = 1;
            //  double dDepth = (double)cDepth / 8.0;
            //  for (int yy = -cr; yy <= cr; yy++) {
            //    for (int xx = -cr; xx <= cr; xx++) {
            //      double r = Math.Sqrt((yy * yy) + (xx * xx));
            //      int px = cx + xx;
            //      if (px < 0) px += MapWidth;
            //      if (px >= MapWidth) px -= MapWidth;
            //      double amount = 0.0;
            //      if (r > cRad) {
            //        amount = (cRad / Math.Max(5.0 * (r - cRad), 0.5)) / 2.0;
            //      }
            //      else {
            //        amount = (1.0 / Math.Max(10.0 * (cRad - r) / cRad, 0.5)) - 1.0;
            //      }
            //      iMap[px, cy + yy] += (int)(amount * dDepth);
            //    }
            //  }
            //}
        }

        // Add bands and eyes to gas giants
        private static void AddGasGiantCloudFeatures(int[,] iMap, Random rnd) {
            // Add bands
            // Add eyes
            // Add turbulence
        }

        // Convert height to colour for a star
        private static Vector3 HeightToColor_Default(int height) {
            Vector3 col;
            if (height < 0) {
                float fract = (float)(height + 100) / 100.0f;
                if (fract < 0.0f) fract = 0.0f;
                col = Vector3.Add(col1, Vector3.Multiply(cdiff1, fract));
            }
            else {
                float fract = (float)height / 100.0f;
                if (fract > 1.0f) fract = 1.0f;
                col = Vector3.Add(col3, Vector3.Multiply(cdiff2, fract));
            }
            return col;
        }

        // Convert height to colour for an oceanic world
        private static Vector3 HeightToColor_Oceanic(int height) {
            Vector3 col;
            if (height < 0) {
                float fract = (float)(height + 100) / 100.0f;
                if (fract < 0.9f) {
                    col.X = col.Y = 0.0f;
                    col.Z = 0.4f;
                }
                else {
                    col.X = col.Y = 0.0f;
                    //col.X = (fract - 0.9f) * 1.0f;
                    //col.Y = (fract - 0.9f) * 1.5f;
                    col.Z = (fract - 0.9f) * 6.0f + 0.4f;
                }
            }
            else {
                float fract = (float)height / 100.0f;
                if (fract < 0.4f) {
                    col.X = col.Z = 0.0f;
                    col.Y = 0.6f + (fract * 1.0f);
                }
                //else if (fract < 0.4f) {
                //  col.X = (fract - 0.3f) * 1.0f;
                //  col.Y = 1.0f;
                //  col.Z = (fract - 0.3f) * 2.0f;
                //}
                else if (fract < 0.5f) {
                    col.X = (fract - 0.4f) * 7.0f;
                    col.Y = 1.0f - (fract - 0.4f) * 4.5f;
                    col.Z = (fract - 0.4f) * 4.0f;
                }
                else if (fract < 0.8f) {
                    col.X = (fract - 0.5f) * 0.9f + 0.7f;
                    col.Y = (fract - 0.5f) * 0.8f + 0.55f;
                    col.Z = (fract - 0.5f) * 0.7f + 0.4f;
                }
                else if (fract < 0.9f) {
                    col.X = (fract - 0.8f) * 7.0f + 0.3f;
                    col.Y = (fract - 0.8f) * 7.0f + 0.3f;
                    col.Z = (fract - 0.8f) * 7.0f + 0.3f;
                }
                else {
                    col.X = col.Y = col.Z = 1.0f;
                }
            }
            return col;
        }
    }
}
