using OpenTK.Mathematics;

namespace SpaceMercs.Graphics.Shapes {
    internal static class Grid {
        private static GLShape? _grid = null;
        public static GLShape Lines { get { if (_grid is null) { _grid = BuildLines(Alignment.TopLeft); } return _grid; } }

        private static GLShape BuildLines(Alignment ali) {
            List<VertexPos3D> vertices = new List<VertexPos3D>();
            for (int x = 0; x <= 10; x++) {
                vertices.Add(new VertexPos3D(new Vector3((float)x / 10f, 0f, 0f)));
                vertices.Add(new VertexPos3D(new Vector3((float)x / 10f, 1f, 0f)));
            }
            for (int y = 0; y <= 10; y++) {
                vertices.Add(new VertexPos3D(new Vector3(0f, (float)y / 10f, 0f)));
                vertices.Add(new VertexPos3D(new Vector3(1f, (float)y / 10f, 0f)));
            }
            return new GLShape(vertices.ToArray(), Enumerable.Range(0, vertices.Count).ToArray(), OpenTK.Graphics.OpenGL.PrimitiveType.Lines);
        }
    }
}
