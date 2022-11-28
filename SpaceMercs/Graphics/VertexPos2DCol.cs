using OpenTK.Mathematics;

namespace SpaceMercs.Graphics
{
    internal readonly struct VertexPos2DCol
    {
        public readonly Vector2 Position;
        public readonly Color4 Colour;

        public static readonly VertexInfo VertexInfo = new VertexInfo(typeof(VertexPos2DCol), new VertexAttribute(nameof(Position), 0, 2, 0), new VertexAttribute(nameof(Colour), 1, 4, 2));
        public static int SizeInBytes { get { return VertexInfo.SizeInBytes; } }

        public VertexPos2DCol(Vector2 position, Color4 colour)
        {
            Position = position;
            Colour = colour;
        }
    }
}
