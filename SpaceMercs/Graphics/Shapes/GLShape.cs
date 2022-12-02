using OpenTK.Graphics.OpenGL;

namespace SpaceMercs.Graphics.Shapes {
    internal class GLShape : IDisposable {
        protected readonly VertexBuffer vertexBuffer;
        protected readonly IndexBuffer indexBuffer;
        protected readonly VertexArray vertexArray;
        private bool isDisposed = false;
        private PrimitiveType PrimitiveType = PrimitiveType.Triangles;

        public GLShape(IVertex[] vertices, int[] indices, PrimitiveType pt = PrimitiveType.Triangles) {
            vertexBuffer = new VertexBuffer(vertices);
            indexBuffer = new IndexBuffer(indices);
            vertexArray = new VertexArray(vertexBuffer);
            PrimitiveType = pt;
        }

        public void Bind() {
            GL.BindVertexArray(vertexArray.VertexArrayHandle);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, indexBuffer.IndexBufferHandle);
        }

        public void Unbind() {
            GL.BindVertexArray(0);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
        }

        public void Draw() {
            GL.DrawElements(PrimitiveType, indexBuffer.IndexCount, DrawElementsType.UnsignedInt, 0);
        }

        // IDisposable
        ~GLShape() {
            Dispose();
        }

        public void Dispose() {
            if (isDisposed) return;
            isDisposed = true;
            vertexBuffer?.Dispose();
            indexBuffer?.Dispose();
            vertexArray?.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
