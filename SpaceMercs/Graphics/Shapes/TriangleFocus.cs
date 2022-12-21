using OpenTK.Mathematics;

namespace SpaceMercs.Graphics.Shapes {
    internal static class TriangleFocus {
        private static GLShape? _triangleFocus = null;
        public static GLShape Flat { get { if (_triangleFocus is null) { _triangleFocus = Build(); } return _triangleFocus; } }

        private static GLShape Build() {
            VertexPos3D[] vertices = new VertexPos3D[] {
                new VertexPos3D(new Vector3( 0f,    0.5f, 0f)),
                new VertexPos3D(new Vector3(-0.5f,  1f,   0f)),
                new VertexPos3D(new Vector3( 0.5f,  1f,   0f)),
                new VertexPos3D(new Vector3( 0f,   -0.5f, 0f)),
                new VertexPos3D(new Vector3( 0.5f, -1f,   0f)),
                new VertexPos3D(new Vector3(-0.5f, -1f,   0f)),
                new VertexPos3D(new Vector3( 0.5f,  0f,   0f)),
                new VertexPos3D(new Vector3( 1f,  0.5f,   0f)),
                new VertexPos3D(new Vector3( 1f, -0.5f,   0f)),
                new VertexPos3D(new Vector3(-0.5f,  0f,   0f)),
                new VertexPos3D(new Vector3(-1f, -0.5f,   0f)),
                new VertexPos3D(new Vector3(-1f,  0.5f,   0f)),
              };
            int[] indices = new int[12] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11 };
            return new GLShape(vertices, indices);
        }
    }
}
