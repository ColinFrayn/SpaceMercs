using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;

namespace SpaceMercs.Graphics.Shapes {
    internal static class Circle {
        private static GLShape? _circle32 = null, _circle64 = null;
        public static GLShape Circle32 {
            get {
                if (_circle32 is null) _circle32 = BuildFlat(32);
                return _circle32;
            }
        }
        public static GLShape Circle64 {
            get {
                if (_circle64 is null) _circle64 = BuildFlat(64);
                return _circle64;
            }
        }

        public static GLShape BuildFlat(int count) {
            VertexPos3D[] vertices = new VertexPos3D[count];
            for (int i = 0; i < count; i++) {
                double ang = (double)i * Math.PI * 2 / (double)count;
                vertices[i] = new VertexPos3D(new Vector3((float)Math.Cos(ang), (float)Math.Sin(ang), 0f));
            }
            return new GLShape(vertices, Enumerable.Range(0, count - 1).ToArray(), PrimitiveType.LineLoop);
        }
    }
}
