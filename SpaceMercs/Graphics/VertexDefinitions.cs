using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;

namespace SpaceMercs.Graphics
{

    public readonly struct VertexAttribute
    {
        public readonly string Name;
        public readonly int Index;
        public readonly int ComponentCount;
        public readonly int Offset;
        public VertexAttribute(string name, int index, int componentCount, int offset)
        {
            Name = name;
            Index = index;
            ComponentCount = componentCount;
            Offset = offset;
        }
    }

    public class VertexInfo
    {
        public readonly Type Type;
        public readonly int SizeInBytes;
        public readonly VertexAttribute[] VertexAttributes;

        public VertexInfo(Type type, params VertexAttribute[] attributes)
        {
            Type = type;
            SizeInBytes = 0;
            VertexAttributes = attributes;
            foreach (VertexAttribute attr in attributes)
            {
                SizeInBytes += attr.ComponentCount * sizeof(float);
            }
        }

        public void SetupVertexAttribs()
        {
            foreach (VertexAttribute attr in VertexAttributes)
            {
                GL.VertexAttribPointer(attr.Index, attr.ComponentCount, VertexAttribPointerType.Float, false, SizeInBytes, attr.Offset * sizeof(float));
                GL.EnableVertexAttribArray(attr.Index);
            }
        }
    }

    public readonly struct VertexPosCol
    {
        public readonly Vector2 Position;
        public readonly Color4 Colour;

        public static readonly VertexInfo VertexInfo = new VertexInfo(typeof(VertexPosCol), new VertexAttribute(nameof(Position), 0, 2, 0), new VertexAttribute(nameof(Colour), 1, 4, 2));
        public static int SizeInBytes { get { return VertexInfo.SizeInBytes; } }
        public static void SetupVertexAttribs() => VertexInfo.SetupVertexAttribs();

        public VertexPosCol(Vector2 position, Color4 colour)
        {
            Position = position;
            Colour = colour;
        }
    }
}
