using OpenTK.Graphics.OpenGL;

namespace SpaceMercs.Graphics {
  internal sealed class VertexBuffer : IDisposable {
    private bool isDisposed;
    private const int MinVertexCount = 2;
    private const int MaxVertexCount = 100_000;

    public readonly int VertexBufferHandle;
    public readonly int VertexCount;
    public readonly VertexInfo VertexInfo;
    public readonly bool IsStatic;

    public VertexBuffer(VertexPos2DTex[] vertices, bool isStatic = true) {
      isDisposed = false;
      if (vertices is null) {
        throw new ArgumentNullException(nameof(vertices));
      }
      VertexCount = vertices.Length;
      if (VertexCount < MinVertexCount || VertexCount > MaxVertexCount) {
        throw new ArgumentOutOfRangeException(nameof(vertices));
      }
      IsStatic = isStatic;
      VertexInfo = VertexPos2DTex.VertexInfo;
      VertexBufferHandle = GL.GenBuffer();

      GL.BindBuffer(BufferTarget.ArrayBuffer, VertexBufferHandle);
      GL.BufferData(BufferTarget.ArrayBuffer, VertexCount * VertexInfo.SizeInBytes, vertices, isStatic ? BufferUsageHint.StaticDraw : BufferUsageHint.StreamDraw);
      GL.BindBuffer(BufferTarget.ArrayBuffer, 0); // Unbind
    }

    public VertexBuffer(VertexPos2DCol[] vertices, bool isStatic = true) {
      isDisposed = false;
      if (vertices is null) {
        throw new ArgumentNullException(nameof(vertices));
      }
      VertexCount = vertices.Length;
      if (VertexCount < MinVertexCount || VertexCount > MaxVertexCount) {
        throw new ArgumentOutOfRangeException(nameof(vertices));
      }
      IsStatic = isStatic;
      VertexInfo = VertexPos2DCol.VertexInfo;
      VertexBufferHandle = GL.GenBuffer();

      GL.BindBuffer(BufferTarget.ArrayBuffer, VertexBufferHandle);
      GL.BufferData(BufferTarget.ArrayBuffer, VertexCount * VertexInfo.SizeInBytes, vertices, isStatic ? BufferUsageHint.StaticDraw : BufferUsageHint.StreamDraw);
      GL.BindBuffer(BufferTarget.ArrayBuffer, 0); // Unbind
    }

    public void SetupVertexAttribs() => VertexInfo.SetupVertexAttribs();

    ~VertexBuffer() {
      Dispose();
    }

    public void Dispose() {
      if (isDisposed) return;
      GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
      GL.DeleteBuffer(VertexBufferHandle);
      isDisposed = true;
      GC.SuppressFinalize(this);
    }
  }
}
