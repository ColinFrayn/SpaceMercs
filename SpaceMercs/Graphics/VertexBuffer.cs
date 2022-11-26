using OpenTK.Graphics.OpenGL;

namespace SpaceMercs.Graphics {
  public sealed class VertexBuffer : IDisposable {
    private bool isDisposed;
    private const int MinVertexCount = 2;
    private const int MaxVertexCount = 100_000;

    public readonly int VertexBufferHandle;
    public readonly int VertexCount;
    public readonly VertexInfo VertexInfo;
    public readonly bool IsStatic;

    public VertexBuffer(VertexInfo vertexInfo, int vertexCount, bool isStatic = true) {
      isDisposed = false;
      if (vertexCount < MinVertexCount || vertexCount > MaxVertexCount) {
        throw new ArgumentOutOfRangeException(nameof(vertexCount));
      }
      VertexInfo = vertexInfo;
      VertexCount = vertexCount;
      IsStatic = isStatic; 
      VertexBufferHandle = GL.GenBuffer();
      GL.BindBuffer(BufferTarget.ArrayBuffer, VertexBufferHandle);
      GL.BufferData(BufferTarget.ArrayBuffer, VertexCount * VertexInfo.SizeInBytes, IntPtr.Zero, isStatic ? BufferUsageHint.StaticDraw : BufferUsageHint.StreamDraw);
      GL.BindBuffer(BufferTarget.ArrayBuffer, 0); // Unbind
    }

    public void SetData<T>(T[] data) where T : struct {
      if (typeof(T) != VertexInfo.Type) {
        throw new ArgumentException($"Generic Type {typeof(T).FullName} does not match the configured vertex type of the vertex buffer");
      }
      if (data is null) {
        throw new ArgumentNullException(nameof(data));
      }

      if (data.Length != VertexCount) {
        throw new ArgumentOutOfRangeException(nameof(data));
      }

      GL.BindBuffer(BufferTarget.ArrayBuffer, VertexBufferHandle);
      GL.BufferSubData(BufferTarget.ArrayBuffer, IntPtr.Zero, data.Length * VertexInfo.SizeInBytes, data);
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
