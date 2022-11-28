namespace SpaceMercs.Graphics {
  internal readonly struct VertexAttribute {
    public readonly string Name;
    public readonly int Index;
    public readonly int ComponentCount;
    public readonly int Offset;
    public VertexAttribute(string name, int index, int componentCount, int offset) {
      Name = name;
      Index = index;
      ComponentCount = componentCount;
      Offset = offset;
    }
  }
}
