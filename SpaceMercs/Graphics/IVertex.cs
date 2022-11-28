namespace SpaceMercs.Graphics {
  internal interface IVertex {
    VertexInfo VertexInfo { get; }
    int SizeInBytes { get; }
    float[] Flattened { get; }
  }
}
