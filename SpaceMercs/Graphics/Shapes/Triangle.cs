using OpenTK.Mathematics;

namespace SpaceMercs.Graphics.Shapes {
    internal static class Triangle {
        private static GLShape? _triangle = null;
        private static GLShape? _triangleNorm = null;
        public static GLShape Flat { get { if (_triangle is null) { _triangle = Build(); } return _triangle; } }
        public static GLShape Norm { get { if (_triangleNorm is null) { _triangleNorm = BuildNorm(); } return _triangleNorm; } }

        private static GLShape Build() {
            VertexPos3D[] vertices = new VertexPos3D[] {
                new VertexPos3D(new Vector3(-1f, 0f, 0f)),
                new VertexPos3D(new Vector3( 0f, 1f, 0f)),
                new VertexPos3D(new Vector3( 1f, 0f, 0f)),
              };
            int[] indices = new int[3] { 0, 1, 2 };
            return new GLShape(vertices, indices);
        }
        private static GLShape BuildNorm() {
            VertexPos3DNorm[] vertices = new VertexPos3DNorm[] {
                new VertexPos3DNorm(new Vector3(-1f, 0f, 0f), new Vector3(0f, 0f, 1f)),
                new VertexPos3DNorm(new Vector3( 0f, 1f, 0f), new Vector3(0f, 0f, 1f)),
                new VertexPos3DNorm(new Vector3( 1f, 0f, 0f), new Vector3(0f, 0f, 1f)),
              };
            int[] indices = new int[3] { 0, 1, 2 };
            return new GLShape(vertices, indices);
        }
    }
}
