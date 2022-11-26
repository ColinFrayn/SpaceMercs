using OpenTK.Graphics.OpenGL;

namespace SpaceMercs.Graphics {
  public sealed class IndexBuffer : IDisposable {
    private bool isDisposed;
    private const int MinIndexCount = 2;
    private const int MaxIndexCount = 250_000;

    public readonly int IndexBufferHandle;
    public readonly int IndexCount;

    public IndexBuffer(int indexCount, bool isStatic = true) {
      if (indexCount < MinIndexCount || indexCount > MaxIndexCount) {
        throw new ArgumentOutOfRangeException(nameof(indexCount));
      }
      IndexCount = indexCount;
      IndexBufferHandle = GL.GenBuffer();
      GL.BindBuffer(BufferTarget.ElementArrayBuffer, IndexBufferHandle);
      GL.BufferData(BufferTarget.ElementArrayBuffer, indexCount * sizeof(int), IntPtr.Zero, isStatic ? BufferUsageHint.StaticDraw : BufferUsageHint.StreamDraw);
      GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0); // Unbind
    }

    public void SetData(int[] data) {
      if (data is null) {
        throw new ArgumentNullException(nameof(data));
      }
      if (data.Length != IndexCount) {
        throw new ArgumentOutOfRangeException(nameof(data));
      }

      GL.BindBuffer(BufferTarget.ElementArrayBuffer, IndexBufferHandle);
      GL.BufferSubData(BufferTarget.ElementArrayBuffer, IntPtr.Zero, data.Length * sizeof(int), data);
      GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0); // Unbind
    }

    ~IndexBuffer() {
      Dispose();
    }

    public void Dispose() {
      if (isDisposed) return;
      GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
      GL.DeleteBuffer(IndexBufferHandle);
      isDisposed = true;
      GC.SuppressFinalize(this);
    }
  }
}
