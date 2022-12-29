using OpenTK.Mathematics;

namespace SpaceMercs.Graphics.Shapes {
    internal static class Cross {
        private static GLShape? _cross = null;
        public static GLShape Flat { get { if (_cross is null) { _cross = Build(); } return _cross; } }

        private static GLShape Build() {
            VertexPos3D[] vertices = new VertexPos3D[] {
                new VertexPos3D(new Vector3(0f, 0.6f, 0f)),
                new VertexPos3D(new Vector3(0f, 0.4f, 0f)),
                new VertexPos3D(new Vector3(1f, 0.4f, 0f)),
                new VertexPos3D(new Vector3(1f, 0.6f, 0f)),
                new VertexPos3D(new Vector3(0.4f, 0f, 0f)),
                new VertexPos3D(new Vector3(0.6f, 0f, 0f)),
                new VertexPos3D(new Vector3(0.6f, 1f, 0f)),
                new VertexPos3D(new Vector3(0.4f, 1f, 0f)),
              };
            int[] indices = new int[12] { 0, 1, 2, 1, 2, 3, 4, 5, 6, 4, 6, 7 };
            return new GLShape(vertices, indices);
        }
    }
}
