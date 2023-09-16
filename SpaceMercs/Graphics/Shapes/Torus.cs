using OpenTK.Mathematics;

namespace SpaceMercs.Graphics.Shapes {
    internal static class Torus {
        private static readonly IDictionary<(int, int, bool), GLShape> _tori = new Dictionary<(int, int, bool), GLShape>();

        public static void CachedBuildAndDraw(int detail, int ratio, bool bTexture = false) {
            GLShape torus = Build(detail, ratio, bTexture);
            torus.BindAndDraw();
        }

        public static GLShape Build(int detail, int ratio, bool bTexture = false) {
            if (detail < 1 || detail > 12) throw new ArgumentException($"Values for {nameof(detail)} must be between 1 .. 12");
            if (!_tori.ContainsKey((detail, ratio, bTexture))) {
                _tori.Add((detail, ratio, bTexture), SetupTorus(3 * detail + 2, ((3 * detail)/2) + 2, ratio, bTexture));
            }
            return _tori[(detail, ratio, bTexture)];
        }

        // Setup the torus by generating rings of triangles
        private static GLShape SetupTorus(int nphi, int ntheta, int ratio, bool bTexture = false) {
            List<IVertex> vertices = new List<IVertex>();

            // Ratio here is the minor_radius * 100 / major_radius
            double r2 = ratio / 100.0;

            double dTheta = Math.PI * 2.0 / ntheta;
            double dPhi = Math.PI * 2.0 / nphi;

            for (int phi = 0; phi < nphi; phi++) {
                double Phi = phi * dPhi;
                // Point in the middle of the torus at this phi
                Vector2d pt = new Vector2d(Math.Cos(Phi), Math.Sin(Phi));
                Vector2d pt2 = new Vector2d(Math.Cos(Phi + dPhi), Math.Sin(Phi + dPhi));
                // Run round the minor ring
                for (int theta = 0; theta <= ntheta; theta++) {
                    double Theta = theta * dTheta;
                    {
                        // Upper triangle 
                        Vector4d v1 = new Vector4d(pt.X, pt.Y, Theta, Phi); // X, Y, Theta, Phi
                        Vector4d v2 = new Vector4d(pt.X, pt.Y, Theta + dTheta, Phi);
                        Vector4d v3 = new Vector4d(pt2.X, pt2.Y, Theta + dTheta, Phi + dPhi);
                        vertices.AddRange(Setup3DFace(v1, v2, v3, r2, bTexture));
                    }
                    {
                        // Lower triangle
                        Vector4d v1 = new Vector4d(pt.X, pt.Y, Theta, Phi); // X, Y, Theta, Phi
                        Vector4d v2 = new Vector4d(pt2.X, pt2.Y, Theta + dTheta, Phi + dPhi);
                        Vector4d v3 = new Vector4d(pt2.X, pt2.Y, Theta, Phi + dPhi);
                        vertices.AddRange(Setup3DFace(v1, v2, v3, r2, bTexture));
                    }
                }
            }
            return new GLShape(vertices.ToArray(), Enumerable.Range(0, vertices.Count).ToArray());
        }

        // Set up a face
        public static IEnumerable<IVertex> Setup3DFace(Vector4d v1, Vector4d v2, Vector4d v3, double r2, bool bTexture) {
            Vector3 pv1 = new Vector3((float)v1.X + (float)(r2 * Math.Cos(v1.W) * Math.Cos(v1.Z)), (float)v1.Y + (float)(r2 * Math.Sin(v1.W) * Math.Cos(v1.Z)), (float)(r2 * Math.Sin(v1.Z)));
            Vector3 pv2 = new Vector3((float)v2.X + (float)(r2 * Math.Cos(v2.W) * Math.Cos(v2.Z)), (float)v2.Y + (float)(r2 * Math.Sin(v2.W) * Math.Cos(v2.Z)), (float)(r2 * Math.Sin(v2.Z)));
            Vector3 pv3 = new Vector3((float)v3.X + (float)(r2 * Math.Cos(v3.W) * Math.Cos(v3.Z)), (float)v3.Y + (float)(r2 * Math.Sin(v3.W) * Math.Cos(v3.Z)), (float)(r2 * Math.Sin(v3.Z)));

            // Calculate the normal
            Vector3 n1 = new Vector3((float)(Math.Cos(v1.Z) * Math.Cos(v1.W)), (float)(Math.Sin(v1.Z) * Math.Cos(v1.W)), (float)(Math.Sin(v1.Z)));
            Vector3 n2 = new Vector3((float)(Math.Cos(v2.Z) * Math.Cos(v2.W)), (float)(Math.Sin(v2.Z) * Math.Cos(v2.W)), (float)(Math.Sin(v2.Z)));
            Vector3 n3 = new Vector3((float)(Math.Cos(v3.Z) * Math.Cos(v3.W)), (float)(Math.Sin(v3.Z) * Math.Cos(v3.W)), (float)(Math.Sin(v3.Z)));

            if (bTexture) {
                // Texture coordinates
                Vector2 t1 = new Vector2((float)(v1.Z / (Math.PI * 2.0)), (float)(v1.W / (Math.PI * 2.0)));
                Vector2 t2 = new Vector2((float)(v2.Z / (Math.PI * 2.0)), (float)(v2.W / (Math.PI * 2.0)));
                Vector2 t3 = new Vector2((float)(v3.Z / (Math.PI * 2.0)), (float)(v3.W / (Math.PI * 2.0)));

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
