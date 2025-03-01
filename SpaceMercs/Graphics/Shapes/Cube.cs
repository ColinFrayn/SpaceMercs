using OpenTK.Mathematics;

namespace SpaceMercs.Graphics.Shapes {
    internal static class Cube {
        private static GLShape? _cube = null;
        public static GLShape Flat { get { if (_cube is null) { _cube = Build(); } return _cube; } }

        private static GLShape Build() {
            VertexPos3D[] vertices = new VertexPos3D[] {
                new VertexPos3D(new Vector3( 0.5f,  0.5f,  0.5f)),
                new VertexPos3D(new Vector3(-0.5f,  0.5f,  0.5f)),
                new VertexPos3D(new Vector3(-0.5f, -0.5f,  0.5f)),
                new VertexPos3D(new Vector3( 0.5f, -0.5f,  0.5f)),
                new VertexPos3D(new Vector3( 0.5f,  0.5f, -0.5f)),
                new VertexPos3D(new Vector3(-0.5f,  0.5f, -0.5f)),
                new VertexPos3D(new Vector3(-0.5f, -0.5f, -0.5f)),
                new VertexPos3D(new Vector3( 0.5f, -0.5f, -0.5f)),
              };
            int[] indices = new int[36] { 0, 1, 2, 0, 2, 3, 0, 4, 1, 4, 5, 1, 1, 5, 2, 5, 6, 2, 2, 6, 3, 6, 7, 3, 3, 7, 0, 7, 4, 0, 4, 6, 5, 4, 7, 6 };
            return new GLShape(vertices, indices);
        }
    }
}
