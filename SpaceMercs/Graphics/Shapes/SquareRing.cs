using OpenTK.Mathematics;

namespace SpaceMercs.Graphics.Shapes {
    internal static class SquareRing {
        private static GLShape? _squareRing = null;
        private static GLShape? _thinRing = null;
        public static GLShape Flat { get { if (_squareRing is null) { _squareRing = Build(0.15f); } return _squareRing; } }
        public static GLShape Thin { get { if (_thinRing is null) { _thinRing = Build(0.05f); } return _thinRing; } }

        private static GLShape Build(float border) {
            VertexPos3D[] vertices = new VertexPos3D[] {
                new VertexPos3D(new Vector3(0f, 0f, 0f)),
                new VertexPos3D(new Vector3(1f, 0f, 0f)),
                new VertexPos3D(new Vector3(1f, 1f, 0f)),
                new VertexPos3D(new Vector3(0f, 1f, 0f)),
                new VertexPos3D(new Vector3(border, border, 0f)),
                new VertexPos3D(new Vector3(1f - border, border, 0f)),
                new VertexPos3D(new Vector3(1f - border, 1f - border, 0f)),
                new VertexPos3D(new Vector3(border, 1f - border, 0f)),
              };
            int[] indices = new int[24] { 0, 1, 4, 4, 1, 5, 5, 1, 2, 5, 2, 6, 7, 6, 3, 6, 2, 3, 0, 4, 7, 0, 7, 3 };
            return new GLShape(vertices, indices);
        }
    }
}
