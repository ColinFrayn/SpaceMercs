using OpenTK.Mathematics;

namespace SpaceMercs.Graphics.Shapes {
    internal static class Spectrum {
        private static GLShape? _spect = null;
        public static GLShape Flat { get { if (_spect is null) { _spect = Build(); } return _spect; } }

        private static GLShape Build() {
            VertexPos2DCol[] vertices = new VertexPos2DCol[] {
                new VertexPos2DCol(new Vector2(0.0f, 0f), new Color4(0.6f,0.0f,0.0f,1f)),
                new VertexPos2DCol(new Vector2(0.0f, 1f), new Color4(0.6f,0.0f,0.0f,1f)),
                new VertexPos2DCol(new Vector2(0.3f, 0f), new Color4(1.0f,0.5f,0.0f,1f)),
                new VertexPos2DCol(new Vector2(0.3f, 1f), new Color4(1.0f,0.5f,0.0f,1f)),
                new VertexPos2DCol(new Vector2(0.5f, 0f), new Color4(1.0f,1.0f,0.0f,1f)),
                new VertexPos2DCol(new Vector2(0.5f, 1f), new Color4(1.0f,1.0f,0.0f,1f)),
                new VertexPos2DCol(new Vector2(0.8f, 0f), new Color4(0.0f,1.0f,0.0f,1f)),
                new VertexPos2DCol(new Vector2(0.8f, 1f), new Color4(0.0f,1.0f,0.0f,1f)),
                new VertexPos2DCol(new Vector2(1.0f, 0f), new Color4(0.0f,0.0f,0.7f,1f)),
                new VertexPos2DCol(new Vector2(1.0f, 1f), new Color4(0.0f,0.0f,0.7f,1f)),
              };
            return new GLShape(vertices, Enumerable.Range(0, vertices.Length).ToArray(), OpenTK.Graphics.OpenGL.PrimitiveType.TriangleStrip);
        }
    }
}
