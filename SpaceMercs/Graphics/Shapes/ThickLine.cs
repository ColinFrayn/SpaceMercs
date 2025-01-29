using OpenTK.Mathematics;

namespace SpaceMercs.Graphics.Shapes {
    internal class ThickLine {
        private const int splits = 4;

        private readonly GLShape? _line = null;

        private ThickLine(GLShape? line) {
            _line = line;
        }

        public static ThickLine Make_VertexPos2DCol(float fx, float fy, float tx, float ty, float thickness, Color4 col) {
            List<VertexPos2DCol> vertices = new List<VertexPos2DCol>();
            Vector2 perp = new Vector2(ty-fy, fx-tx); // Perpendicular vector

            perp *= thickness / perp.Length;

            for (int n=0; n<=splits; n++) 
            {
                vertices.Add(new VertexPos2DCol(new Vector2(fx + ((tx - fx) * n) / splits + perp.X, fy + ((ty - fy) * n) / splits + perp.Y), col));
                vertices.Add(new VertexPos2DCol(new Vector2(fx + ((tx - fx) * n) / splits - perp.X, fy + ((ty - fy) * n) / splits - perp.Y), col));
            };

            int[] indices = Enumerable.Range(0, (splits + 1) * 2).ToArray();

            GLShape line = new GLShape(vertices.ToArray(), indices, OpenTK.Graphics.OpenGL.PrimitiveType.TriangleStrip);
            return new(line);
        }

        public static ThickLine Make_Vertex3D(float fx, float fy, float fz, float tx, float ty, float tz, float thickness) {
            List<VertexPos3D> vertices = new List<VertexPos3D>();
            Vector3 perp = new Vector3(ty - fy, fx - tx, 0f); // Perpendicular vector

            perp *= thickness / perp.Length;

            for (int n = 0; n <= splits; n++) {
                vertices.Add(new VertexPos3D(new Vector3(fx + ((tx - fx) * n) / splits + perp.X, fy + ((ty - fy) * n) / splits + perp.Y, fz + ((tz - fz) * n) / splits + perp.Z)));
                vertices.Add(new VertexPos3D(new Vector3(fx + ((tx - fx) * n) / splits - perp.X, fy + ((ty - fy) * n) / splits - perp.Y, fz + ((tz - fz) * n) / splits - perp.Z)));
            };

            int[] indices = Enumerable.Range(0, (splits + 1) * 2).ToArray();

            GLShape line = new GLShape(vertices.ToArray(), indices, OpenTK.Graphics.OpenGL.PrimitiveType.TriangleStrip);
            return new(line);
        }

        public void BindAndDraw() => _line?.BindAndDraw();
    }
}
