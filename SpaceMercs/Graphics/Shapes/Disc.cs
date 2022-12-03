using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;

namespace SpaceMercs.Graphics.Shapes {
    internal static class Disc {
        private static GLShape? _disc32 = null, _disc64 = null;
        public static GLShape Disc32 {
            get {
                if (_disc32 is null) _disc32 = BuildFlat(32);
                return _disc32;
            }
        }
        public static GLShape Disc64 {
            get {
                if (_disc64 is null) _disc64 = BuildFlat(64);
                return _disc64;
            }
        }

        public static GLShape BuildFlat(int count) {
            VertexPos3D[] vertices = new VertexPos3D[count+2];
            vertices[0] = new VertexPos3D(new Vector3(0f, 0f, 0f));
            for (int i = 0; i <= count; i++) {
                double ang = (double)i * Math.PI * 2 / (double)count;
                vertices[i+1] = new VertexPos3D(new Vector3((float)Math.Cos(ang), (float)Math.Sin(ang), 0f));
            }
            return new GLShape(vertices, Enumerable.Range(0, vertices.Length).ToArray(), PrimitiveType.TriangleFan);
        }
    }
}
