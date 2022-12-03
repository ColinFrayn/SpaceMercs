using OpenTK.Mathematics;

namespace SpaceMercs.Graphics {
    internal class VertexPos3DTexNorm : IVertex {
        public readonly Vector3 Position;
        public readonly Vector2 Texture;
        public readonly Vector3 Normal;

        private static readonly VertexInfo _VertexInfo = new VertexInfo(typeof(VertexPos3DTexNorm), new VertexAttribute(nameof(Position), 0, 3, 0), new VertexAttribute(nameof(Texture), 1, 2, 3), new VertexAttribute(nameof(Normal), 2, 3, 5));
        public VertexInfo VertexInfo { get { return _VertexInfo; } }
        public int SizeInBytes { get { return _VertexInfo.SizeInBytes; } }
        public float[] Flattened { get { return new float[8] { Position.X, Position.Y, Position.Z, Texture.X, Texture.Y, Normal.X, Normal.Y, Normal.Z }; } }
        public VertexPos3DTexNorm(Vector3 position, Vector2 texture, Vector3 normal) {
            Position = position;
            Texture = texture;
            Normal = normal;
        }
    }
}