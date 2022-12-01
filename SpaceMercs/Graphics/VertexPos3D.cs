using OpenTK.Mathematics;

namespace SpaceMercs.Graphics {
  internal class VertexPos3D : IVertex {
    public readonly Vector3 Position;

    private static readonly VertexInfo _VertexInfo = new VertexInfo(typeof(VertexPos3D), new VertexAttribute(nameof(Position), 0, 3, 0));
    public VertexInfo VertexInfo { get { return _VertexInfo; } }
    public int SizeInBytes { get { return _VertexInfo.SizeInBytes; } }
    public float[] Flattened { get { return new float[3] { Position.X, Position.Y, Position.Z }; } }
    public VertexPos3D(Vector3 position) {
      Position = position;
    }
  }
}
