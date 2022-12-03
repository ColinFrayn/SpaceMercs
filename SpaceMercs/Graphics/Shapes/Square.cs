using OpenTK.Mathematics;

namespace SpaceMercs.Graphics.Shapes {
    internal static class Square {
        private static GLShape? _square = null;
        private static GLShape? _squareNorm = null;
        private static GLShape? _squareLines = null;
        private static GLShape? _squareTex = null;
        public static GLShape Flat { get { if (_square is null) { _square = Build(Alignment.TopLeft); } return _square; } }
        public static GLShape Norm { get { if (_squareNorm is null) { _squareNorm = BuildNorm(Alignment.TopLeft); } return _squareNorm; } }
        public static GLShape Lines { get { if (_squareLines is null) { _squareLines = BuildLines(Alignment.TopLeft); } return _squareLines; } }
        public static GLShape Textured { get { if (_squareTex is null) { _squareTex = BuildTextured(Alignment.TopLeft); } return _squareTex; } }

        private static GLShape Build(Alignment ali) {
            float tlcx = 0f, tlcy = 0f;
            if (ali == Alignment.TopMiddle || ali == Alignment.CentreMiddle || ali == Alignment.BottomMiddle) tlcx = -0.5f;
            if (ali == Alignment.TopRight || ali == Alignment.CentreRight || ali == Alignment.BottomRight) tlcx = -1.0f;
            if (ali == Alignment.CentreLeft || ali == Alignment.CentreMiddle || ali == Alignment.CentreRight) tlcy = -0.5f;
            if (ali == Alignment.BottomLeft || ali == Alignment.BottomMiddle || ali == Alignment.BottomRight) tlcy = -1.0f;
            VertexPos3D[] vertices = new VertexPos3D[] {
                new VertexPos3D(new Vector3(tlcx,    tlcy,    0f)),
                new VertexPos3D(new Vector3(tlcx,    tlcy+1f, 0f)),
                new VertexPos3D(new Vector3(tlcx+1f, tlcy+1f, 0f)),
                new VertexPos3D(new Vector3(tlcx+1f, tlcy,    0f)),
              };
            int[] indices = new int[6] { 0, 1, 2, 0, 2, 3 };
            return new GLShape(vertices, indices);
        }
        private static GLShape BuildNorm(Alignment ali) {
            float tlcx = 0f, tlcy = 0f;
            if (ali == Alignment.TopMiddle || ali == Alignment.CentreMiddle || ali == Alignment.BottomMiddle) tlcx = -0.5f;
            if (ali == Alignment.TopRight || ali == Alignment.CentreRight || ali == Alignment.BottomRight) tlcx = -1.0f;
            if (ali == Alignment.CentreLeft || ali == Alignment.CentreMiddle || ali == Alignment.CentreRight) tlcy = -0.5f;
            if (ali == Alignment.BottomLeft || ali == Alignment.BottomMiddle || ali == Alignment.BottomRight) tlcy = -1.0f;
            VertexPos3DNorm[] vertices = new VertexPos3DNorm[] {
                new VertexPos3DNorm(new Vector3(tlcx,    tlcy,    0f), new Vector3(0f, 0f, 1f)),
                new VertexPos3DNorm(new Vector3(tlcx,    tlcy+1f, 0f), new Vector3(0f, 0f, 1f)),
                new VertexPos3DNorm(new Vector3(tlcx+1f, tlcy+1f, 0f), new Vector3(0f, 0f, 1f)),
                new VertexPos3DNorm(new Vector3(tlcx+1f, tlcy,    0f), new Vector3(0f, 0f, 1f)),
              };
            int[] indices = new int[6] { 0, 1, 2, 0, 2, 3 };
            return new GLShape(vertices, indices);
        }
        private static GLShape BuildLines(Alignment ali) {
            float tlcx = 0f, tlcy = 0f;
            if (ali == Alignment.TopMiddle || ali == Alignment.CentreMiddle || ali == Alignment.BottomMiddle) tlcx = -0.5f;
            if (ali == Alignment.TopRight || ali == Alignment.CentreRight || ali == Alignment.BottomRight) tlcx = -1.0f;
            if (ali == Alignment.CentreLeft || ali == Alignment.CentreMiddle || ali == Alignment.CentreRight) tlcy = -0.5f;
            if (ali == Alignment.BottomLeft || ali == Alignment.BottomMiddle || ali == Alignment.BottomRight) tlcy = -1.0f;
            VertexPos3D[] vertices = new VertexPos3D[] {
                new VertexPos3D(new Vector3(tlcx,    tlcy,    0f)),
                new VertexPos3D(new Vector3(tlcx,    tlcy+1f, 0f)),
                new VertexPos3D(new Vector3(tlcx+1f, tlcy+1f, 0f)),
                new VertexPos3D(new Vector3(tlcx+1f, tlcy,    0f)),
              };
            int[] indices = new int[4] { 0, 1, 2, 3 };
            return new GLShape(vertices, indices, OpenTK.Graphics.OpenGL.PrimitiveType.LineLoop);
        }
        private static GLShape BuildTextured(Alignment ali) {
            float tlcx = 0f, tlcy = 0f;
            if (ali == Alignment.TopMiddle || ali == Alignment.CentreMiddle || ali == Alignment.BottomMiddle) tlcx = -0.5f;
            if (ali == Alignment.TopRight || ali == Alignment.CentreRight || ali == Alignment.BottomRight) tlcx = -1.0f;
            if (ali == Alignment.CentreLeft || ali == Alignment.CentreMiddle || ali == Alignment.CentreRight) tlcy = -0.5f;
            if (ali == Alignment.BottomLeft || ali == Alignment.BottomMiddle || ali == Alignment.BottomRight) tlcy = -1.0f;
            VertexPos2DTex[] vertices = new VertexPos2DTex[] {
                new VertexPos2DTex(new Vector2(tlcx,    tlcy), new Vector2(0f, 0f)),
                new VertexPos2DTex(new Vector2(tlcx,    tlcy+1f),    new Vector2(0f, 1f)),
                new VertexPos2DTex(new Vector2(tlcx+1f, tlcy+1f),    new Vector2(1f, 1f)),
                new VertexPos2DTex(new Vector2(tlcx+1f, tlcy), new Vector2(1f, 0f)),
              };
            int[] indices = new int[6] { 0, 1, 2, 0, 2, 3 };
            return new GLShape(vertices, indices);
        }
    }
}
