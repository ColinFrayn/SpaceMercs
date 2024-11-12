using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;

namespace SpaceMercs.Graphics.Shapes {
    internal static class Annulus {
        private static GLShape? _annulus16 = null, _annulus32 = null;
        public static GLShape Annulus16 {
            get {
                _annulus16 ??= BuildFlat(16, 1.2f);
                return _annulus16;
            }
        }
        public static GLShape Annulus32 {
            get {
                _annulus32 ??= BuildFlat(32, 1.2f);
                return _annulus32;
            }
        }

        public static GLShape BuildFlat(int count, float outerRadius) {
            VertexPos3D[] vertices = new VertexPos3D[(count+1)*2];
            for (int i = 0; i <= count; i++) {
                double ang = (double)i * Math.PI * 2 / (double)count;
                vertices[i * 2 + 0] = new VertexPos3D(new Vector3((float)Math.Cos(ang) * outerRadius, (float)Math.Sin(ang) * outerRadius, 0f));
                vertices[i * 2 + 1] = new VertexPos3D(new Vector3((float)Math.Cos(ang), (float)Math.Sin(ang), 0f));
            }
            return new GLShape(vertices, Enumerable.Range(0, vertices.Length).ToArray(), PrimitiveType.TriangleStrip);
        }
    }
}
