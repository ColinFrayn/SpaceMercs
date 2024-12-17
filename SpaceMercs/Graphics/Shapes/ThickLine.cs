using OpenTK.Mathematics;

namespace SpaceMercs.Graphics.Shapes {
    internal class ThickLine2D {
        private const int splits = 4;

        private GLShape? _line = null;

        public ThickLine2D(float fx, float fy, float tx, float ty, float thickness, Color4 col) {
            List<VertexPos2DCol> vertices = new List<VertexPos2DCol>();
            Vector2 perp = new Vector2(ty-fy, fx-tx); // Perpendicular vector

            perp *= thickness / perp.Length;

            for (int n=0; n<=splits; n++) 
            {
                vertices.Add(new VertexPos2DCol(new Vector2(fx + ((tx - fx) * n) / splits + perp.X, fy + ((ty - fy) * n) / splits + perp.Y), col));
                vertices.Add(new VertexPos2DCol(new Vector2(fx + ((tx - fx) * n) / splits - perp.X, fy + ((ty - fy) * n) / splits - perp.Y), col));
            };

            int[] indices = Enumerable.Range(0, (splits + 1) * 2).ToArray();

            _line = new GLShape(vertices.ToArray(), indices, OpenTK.Graphics.OpenGL.PrimitiveType.TriangleStrip);
        }

        public void BindAndDraw() => _line?.BindAndDraw();
    }
}
