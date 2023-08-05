using OpenTK.Graphics.OpenGL;

namespace SpaceMercs.Graphics {
    internal sealed class VertexArray : IDisposable {
        private bool isDisposed;
        public readonly VertexBuffer VertexBuffer;
        public readonly int VertexArrayHandle;

        public VertexArray(VertexBuffer vertexBuffer) {
            isDisposed = false;
            if (vertexBuffer is null) {
                throw new ArgumentNullException(nameof(vertexBuffer));
            }
            VertexBuffer = vertexBuffer;
            VertexArrayHandle = GL.GenVertexArray();
            GL.BindVertexArray(VertexArrayHandle);
            GL.BindBuffer(BufferTarget.ArrayBuffer, VertexBuffer.VertexBufferHandle);
            VertexBuffer.SetupVertexAttribs();
            GL.BindVertexArray(0);
        }

        ~VertexArray() {
            Dispose();
        }

        public void Dispose() {
            if (isDisposed) return;
            //GL.BindVertexArray(0); // Crashes here
            //GL.DeleteVertexArray(VertexArrayHandle); // Crashes here
            isDisposed = true;
            GC.SuppressFinalize(this);
        }
    }
}
