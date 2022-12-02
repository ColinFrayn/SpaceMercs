using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;

namespace SpaceMercs.Graphics.Shapes {
    internal static class Circle {
        public static GLShape BuildFlat(int count) {
            VertexPos3D[] vertices = new VertexPos3D[count];
            for (int i = 0; i < count; i++) {
                double ang = (double)i * Math.PI * 2 / (double)count;
                vertices[i] = new VertexPos3D(new Vector3((float)Math.Cos(ang), (float)Math.Sin(ang), 0f));
            }
            return new GLShape(vertices, Enumerable.Range(0, count - 1).ToArray(), PrimitiveType.LineLoop);
        }
    }
}
