using OpenTK.Mathematics;

namespace SpaceMercs.Graphics
{
    internal class VertexPos2DTex : IVertex
    {
        public readonly Vector2 Position;
        public readonly Vector2 Texture;

        private static readonly VertexInfo _VertexInfo = new VertexInfo(typeof(VertexPos2DTex), new VertexAttribute(nameof(Position), 0, 2, 0), new VertexAttribute(nameof(Texture), 1, 2, 2));
        public VertexInfo VertexInfo {  get { return _VertexInfo; } }
        public int SizeInBytes { get { return _VertexInfo.SizeInBytes; } }
        public float[] Flattened {  get {  return new float[4] { Position.X, Position.Y, Texture.X, Texture.Y }; } }

        public VertexPos2DTex(Vector2 position, Vector2 texture)
        {
            Position = position;
            Texture = texture;
        }
    }
}
