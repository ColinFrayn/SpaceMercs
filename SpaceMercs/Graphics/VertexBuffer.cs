using OpenTK.Graphics.OpenGL;

namespace SpaceMercs.Graphics {
    internal sealed class VertexBuffer : IDisposable {
        private bool isDisposed;
        private const int MinVertexCount = 2;
        private const int MaxVertexCount = 100_000;

        public readonly int VertexBufferHandle;
        public int VertexCount { get; private set; }
        public readonly VertexInfo VertexInfo;
        public readonly BufferUsageHint BufferUsageHint;

        public VertexBuffer(IVertex[] vertices, BufferUsageHint hint = BufferUsageHint.StaticDraw) {
            isDisposed = false;
            if (vertices is null) {
                throw new ArgumentNullException(nameof(vertices));
            }
            var types = vertices.Select(x => x.GetType()).Distinct();
            if (types.Count() != 1) {
                throw new ArgumentException("Vertex array must all be of the same type");
            }
            VertexCount = vertices.Length;
            if (VertexCount < MinVertexCount || VertexCount > MaxVertexCount) {
                throw new ArgumentOutOfRangeException(nameof(vertices));
            }
            BufferUsageHint = hint;
            VertexInfo = vertices.First().VertexInfo;
            VertexBufferHandle = GL.GenBuffer();

            // Flatten this structure into a single array that can be passed simply to OpenGL.
            // Note: I could have stored everything in structs and passed them in without flattening, but that makes the code ugly.
            float[] flattened = vertices.SelectMany(x => x.Flattened).ToArray();

            GL.BindBuffer(BufferTarget.ArrayBuffer, VertexBufferHandle);
            GL.BufferData(BufferTarget.ArrayBuffer, VertexCount * VertexInfo.SizeInBytes, flattened, hint);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0); // Unbind
        }

        public void SetData(IVertex[] vertices) {
            if (vertices is null) {
                throw new ArgumentNullException(nameof(vertices));
            }
            var types = vertices.Select(x => x.GetType()).Distinct();
            if (types.Count() != 1) {
                throw new ArgumentException("Vertex array must all be of the same type");
            }
            VertexCount = vertices.Length;

            // Flatten this structure into a single array that can be passed simply to OpenGL.
            // Note: I could have stored everything in structs and passed them in without flattening, but that makes the code ugly.
            float[] flattened = vertices.SelectMany(x => x.Flattened).ToArray();

            GL.BindBuffer(BufferTarget.ArrayBuffer, VertexBufferHandle);
            GL.BufferData(BufferTarget.ArrayBuffer, VertexCount * VertexInfo.SizeInBytes, flattened, BufferUsageHint);
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
