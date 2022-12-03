using OpenTK.Mathematics;

namespace SpaceMercs.Graphics.Shapes {
    internal static class Sphere {
        private static IDictionary<int, GLShape> _spheres = new Dictionary<int, GLShape>();
        public static GLShape Build(int detail) {
            if (detail < 0 || detail > 10) throw new ArgumentException($"Values for {nameof(detail)} must be between 1 .. 10");
            if (!_spheres.ContainsKey(detail)) {
                _spheres.Add(detail, SetupSphere(2 * detail + 3, 2 * detail + 4));
            }
            return _spheres[detail];
        }

        // Setup the sphere by generating rings of triangles
        private static GLShape SetupSphere(int nphi, int ntheta) {
            List<VertexPos3D> vertices = new List<VertexPos3D>();
            for (int phi = 0; phi < nphi; phi++) {
                for (int theta = 0; theta < ntheta; theta++) {
                    // Upper triangle
                    if (phi > 0) {
                        Vector2 v1 = new Vector2((float)theta / (float)ntheta, (float)phi / (float)nphi);
                        Vector2 v2 = new Vector2((float)(theta + 1) / (float)ntheta, (float)phi / (float)nphi);
                        Vector2 v3 = new Vector2((float)theta / (float)ntheta, (float)(phi + 1) / (float)nphi);
                        vertices.AddRange(SetupSphereFace_Pos(v1, v2, v3));
                    }
                    // Lower triangle
                    if (phi + 1 < nphi) {
                        Vector2 v1 = new Vector2((float)(theta + 1) / (float)ntheta, (float)phi / (float)nphi);
                        Vector2 v2 = new Vector2((float)(theta + 1) / (float)ntheta, (float)(phi + 1) / (float)nphi);
                        Vector2 v3 = new Vector2((float)theta / (float)ntheta, (float)(phi + 1) / (float)nphi);
                        vertices.AddRange(SetupSphereFace_Pos(v1, v2, v3));
                    }
                }
            }
            return new GLShape(vertices.ToArray(), Enumerable.Range(0, vertices.Count).ToArray());
        }

        // Set up a face for the planet
        private static IEnumerable<VertexPos3D> SetupSphereFace_Pos(Vector2 v1, Vector2 v2, Vector2 v3) {
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
            if (pv1 == pv2 || pv2 == pv3 || pv1 == pv3) return Array.Empty<VertexPos3D>();

            return new VertexPos3D[] {
                new VertexPos3D(pv1),
                new VertexPos3D(pv2),
                new VertexPos3D(pv3)
            };
        }

        // Set up a face for the planet
        private static IEnumerable<VertexPos3DTexNorm> SetupSphereFace_PosTexNorm(Vector2 v1, Vector2 v2, Vector2 v3) {
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
            if (pv1 == pv2 || pv2 == pv3 || pv1 == pv3) return Array.Empty<VertexPos3DTexNorm>(); ;

            // Calculate the normal
            Vector3 n1 = new Vector3(pv1);
            Vector3 n2 = new Vector3(pv2);
            Vector3 n3 = new Vector3(pv3);

            // Texture coordinates
            Vector2 t1 = new Vector2(1.0f - v1.X, v1.Y);
            Vector2 t2 = new Vector2(1.0f - v2.X, v2.Y);
            Vector2 t3 = new Vector2(1.0f - v3.X, v3.Y);

            return new VertexPos3DTexNorm[] {
                new VertexPos3DTexNorm(pv1, t1, n1),
                new VertexPos3DTexNorm(pv2, t2, n2),
                new VertexPos3DTexNorm(pv3, t3, n3)
            };
        }

        //_spheres.Add(SetupSphere(3, 4));
        //_spheres.Add(SetupSphere(5, 6));
        //_spheres.Add(SetupSphere(7, 8));
        //_spheres.Add(SetupSphere(9, 12));
        //_spheres.Add(SetupSphere(13, 16));
        //_spheres.Add(SetupSphere(19, 24));
        //_spheres.Add(SetupSphere(25, 36));
        //_spheres.Add(SetupSphere(35, 48));
    }
}
