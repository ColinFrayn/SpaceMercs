using OpenTK.Mathematics;

namespace SpaceMercs.Graphics.Shapes {
  internal static class Square {
    public static GLShape BuildTextured(Alignment ali) {
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
