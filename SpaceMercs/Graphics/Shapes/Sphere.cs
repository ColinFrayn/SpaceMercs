using OpenTK.Mathematics;

namespace SpaceMercs.Graphics.Shapes {
    internal static class Sphere {
        private static IDictionary<(int,bool), GLShape> _spheres = new Dictionary<(int, bool), GLShape>();

        public static void CachedBuildAndDraw(int detail, bool bTexture = false) {
            GLShape sphere = Build(detail, bTexture);
            sphere.BindAndDraw();
        }

        public static GLShape Build(int detail, bool bTexture = false) {
            if (detail < 1 || detail > 12) throw new ArgumentException($"Values for {nameof(detail)} must be between 1 .. 12");
            if (!_spheres.ContainsKey((detail, bTexture))) {
                _spheres.Add((detail, bTexture), SetupSphere(2 * detail + 3, 4 * detail + 2, bTexture));
            }
            return _spheres[(detail, bTexture)];
        }

        // Setup the sphere by generating rings of triangles
        private static GLShape SetupSphere(int nphi, int ntheta, bool bTexture = false) {
            List<IVertex> vertices = new List<IVertex>();
            for (int phi = 0; phi < nphi; phi++) {
                for (int theta = 0; theta < ntheta; theta++) {
                    // Upper triangle
                    if (phi > 0) {
                        Vector2 v1 = new Vector2((float)theta / (float)ntheta, (float)phi / (float)nphi);
                        Vector2 v2 = new Vector2((float)(theta + 1) / (float)ntheta, (float)phi / (float)nphi);
                        Vector2 v3 = new Vector2((float)theta / (float)ntheta, (float)(phi + 1) / (float)nphi);
                        vertices.AddRange(SetupSphereFace(v1, v2, v3, bTexture));
                    }
                    // Lower triangle
                    if (phi + 1 < nphi) {
                        Vector2 v1 = new Vector2((float)(theta + 1) / (float)ntheta, (float)phi / (float)nphi);
                        Vector2 v2 = new Vector2((float)(theta + 1) / (float)ntheta, (float)(phi + 1) / (float)nphi);
                        Vector2 v3 = new Vector2((float)theta / (float)ntheta, (float)(phi + 1) / (float)nphi);
                        vertices.AddRange(SetupSphereFace(v1, v2, v3, bTexture));
                    }
                }
            }
            return new GLShape(vertices.ToArray(), Enumerable.Range(0, vertices.Count).ToArray());
        }

        // Set up a face for the planet
        private static IEnumerable<IVertex> SetupSphereFace(Vector2 v1, Vector2 v2, Vector2 v3, bool bTexture) {
            // Convert from three 2D vectors in the map, to their theta/phi coordinates
            double tr1 = v1.X * 2.0 * Math.PI;
            double pr1 = v1.Y * 1.0 * Math.PI;
            double tr2 = v2.X * 2.0 * Math.PI;
            double pr2 = v2.Y * 1.0 * Math.PI;
            double tr3 = v3.X * 2.0 * Math.PI;
            double pr3 = v3.Y * 1.0 * Math.PI;

            // Calculate 3D positions of the three corners
            Vector3 pv1 = new Vector3((float)(Math.Sin(pr1) * Math.Sin(tr1)), (float)(Math.Sin(pr1) * Math.Cos(tr1)), (float)Math.Cos(pr1));
            Vector3 pv2 = new Vector3((float)(Math.Sin(pr2) * Math.Sin(tr2)), (float)(Math.Sin(pr2) * Math.Cos(tr2)), (float)Math.Cos(pr2));
            Vector3 pv3 = new Vector3((float)(Math.Sin(pr3) * Math.Sin(tr3)), (float)(Math.Sin(pr3) * Math.Cos(tr3)), (float)Math.Cos(pr3));

            // Zero area face at the poles? Skip it.
            if (pv1 == pv2 || pv2 == pv3 || pv1 == pv3) return bTexture ? Array.Empty<VertexPos3DTexNorm>() : Array.Empty<VertexPos3DNorm>();

            // Calculate the normal
            Vector3 n1 = new Vector3(pv1);
            Vector3 n2 = new Vector3(pv2);
            Vector3 n3 = new Vector3(pv3);

            // Texture coordinates
            Vector2 t1 = new Vector2(1f - v1.X, v1.Y);
            Vector2 t2 = new Vector2(1f - v2.X, v2.Y);
            Vector2 t3 = new Vector2(1f - v3.X, v3.Y);

            if (bTexture) {
                return new VertexPos3DTexNorm[] {
                    new VertexPos3DTexNorm(pv1, t1, n1),
                    new VertexPos3DTexNorm(pv2, t2, n2),
                    new VertexPos3DTexNorm(pv3, t3, n3)
                };
            }
            return new VertexPos3DNorm[] {
                    new VertexPos3DNorm(pv1, n1),
                    new VertexPos3DNorm(pv2, n2),
                    new VertexPos3DNorm(pv3, n3)
                };
        }
    }
}
