using OpenTK.Mathematics;

namespace SpaceMercs.Graphics {
    internal class VertexPos3DNorm : IVertex {
        public readonly Vector3 Position;
        public readonly Vector3 Normal;

        private static readonly VertexInfo _VertexInfo = new VertexInfo(typeof(VertexPos3DNorm), new VertexAttribute(nameof(Position), 0, 3, 0), new VertexAttribute(nameof(Normal), 1, 3, 3));
        public VertexInfo VertexInfo { get { return _VertexInfo; } }
        public int SizeInBytes { get { return _VertexInfo.SizeInBytes; } }
        public float[] Flattened { get { return new float[6] { Position.X, Position.Y, Position.Z, Normal.X, Normal.Y, Normal.Z }; } }
        public VertexPos3DNorm(Vector3 position, Vector3 normal) {
            Position = position;
            Normal = normal;
        }
    }
}