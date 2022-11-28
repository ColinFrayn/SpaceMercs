using OpenTK.Graphics.OpenGL;

namespace SpaceMercs.Graphics {
  internal class VertexInfo {
    public readonly Type Type;
    public readonly int SizeInBytes;
    public readonly VertexAttribute[] VertexAttributes;

    public VertexInfo(Type type, params VertexAttribute[] attributes) {
      Type = type;
      SizeInBytes = 0;
      VertexAttributes = attributes;
      foreach (VertexAttribute attr in attributes) {
        SizeInBytes += attr.ComponentCount * sizeof(float);
      }
    }

    public void SetupVertexAttribs() {
      foreach (VertexAttribute attr in VertexAttributes) {
        GL.VertexAttribPointer(attr.Index, attr.ComponentCount, VertexAttribPointerType.Float, false, SizeInBytes, attr.Offset * sizeof(float));
        GL.EnableVertexAttribArray(attr.Index);
      }
    }
  }
}
