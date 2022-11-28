using OpenTK.Mathematics;

namespace SpaceMercs.Graphics {
  internal class VertexPos2DCol : IVertex {
    public readonly Vector2 Position;
    public readonly Color4 Colour;

    private static readonly VertexInfo _VertexInfo = new VertexInfo(typeof(VertexPos2DCol), new VertexAttribute(nameof(Position), 0, 2, 0), new VertexAttribute(nameof(Colour), 1, 4, 2));
    public VertexInfo VertexInfo { get { return _VertexInfo; } }
    public int SizeInBytes { get { return _VertexInfo.SizeInBytes; } }
    public float[] Flattened { get { return new float[6] { Position.X, Position.Y, Colour.R, Colour.G, Colour.B, Colour.A }; } }
    public VertexPos2DCol(Vector2 position, Color4 colour) {
      Position = position;
      Colour = colour;
    }
  }
}
