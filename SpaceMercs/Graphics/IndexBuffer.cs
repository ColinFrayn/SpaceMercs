using OpenTK.Graphics.OpenGL;

namespace SpaceMercs.Graphics {
    public sealed class IndexBuffer : IDisposable {
        private bool isDisposed;
        private const int MinIndexCount = 2;
        private const int MaxIndexCount = 250_000;

        public readonly int IndexBufferHandle;
        public int IndexCount { get; private set; }

        public IndexBuffer(int[] data, bool isStatic = true) {
            if (data is null) {
                throw new ArgumentNullException(nameof(data));
            }
            IndexCount = data.Length;
            if (IndexCount < MinIndexCount || IndexCount > MaxIndexCount) {
                throw new ArgumentOutOfRangeException(nameof(IndexCount));
            }
            IndexBufferHandle = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, IndexBufferHandle);
            GL.BufferData(BufferTarget.ElementArrayBuffer, IndexCount * sizeof(int), data, isStatic ? BufferUsageHint.StaticDraw : BufferUsageHint.StreamDraw);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0); // Unbind
        }

        public void SetData(int[] data) {
            if (data is null || data.Length == 0) {
                throw new ArgumentNullException(nameof(data));
            }
            IndexCount = data.Length;
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, IndexBufferHandle);
            GL.BufferData(BufferTarget.ElementArrayBuffer, IndexCount * sizeof(int), data, BufferUsageHint.StreamDraw);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0); // Unbind
        }

        ~IndexBuffer() {
            Dispose();
        }

        public void Dispose() {
            if (isDisposed) return;
            //GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0); // Crashes here
            //GL.DeleteBuffer(IndexBufferHandle); // Crashes here
            isDisposed = true;
            GC.SuppressFinalize(this);
        }
    }
}
