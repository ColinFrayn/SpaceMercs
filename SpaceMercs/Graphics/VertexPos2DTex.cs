using OpenTK.Mathematics;

namespace SpaceMercs.Graphics
{
    internal readonly struct VertexPos2DTex
    {
        public readonly Vector2 Position;
        public readonly Vector2 Texture;

        public static readonly VertexInfo VertexInfo = new VertexInfo(typeof(VertexPos2DTex), new VertexAttribute(nameof(Position), 0, 2, 0), new VertexAttribute(nameof(Texture), 1, 2, 2));
        public static int SizeInBytes { get { return VertexInfo.SizeInBytes; } }

        public VertexPos2DTex(Vector2 position, Vector2 texture)
        {
            Position = position;
            Texture = texture;
        }
    }
}
