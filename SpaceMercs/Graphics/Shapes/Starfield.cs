using OpenTK.Mathematics;

namespace SpaceMercs.Graphics.Shapes {
    internal static class Starfield {
        private static GLShape? _starfield = null;
        public static GLShape Build { get { if (_starfield is null) { _starfield = BuildStarfield(); } return _starfield; } }
        private const int StarfieldCount = 4000;

        private static GLShape BuildStarfield() {
            List<VertexPos2DCol> vertices = new List<VertexPos2DCol>();
            Random rnd = new Random();
            for (int n=0; n < StarfieldCount; n++) {
                Vector2 pos = new Vector2((float)rnd.NextDouble(), (float)rnd.NextDouble());
                float c = 1.0f;
                for (int i=0; i<5; i++) {
                    if (rnd.NextDouble() > 0.3) c *= 0.75f;
                }
                Color4 col = new Color4(c, c, c, 1f);
                vertices.Add(new VertexPos2DCol(pos, col));
            }
            return new GLShape(vertices.ToArray(), Enumerable.Range(0, vertices.Count).ToArray(), OpenTK.Graphics.OpenGL.PrimitiveType.Points);
        }
    }
}
