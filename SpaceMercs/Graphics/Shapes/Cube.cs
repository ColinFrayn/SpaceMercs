using OpenTK.Mathematics;

namespace SpaceMercs.Graphics.Shapes {
    internal static class Cube {
        private static GLShape? _cubeFlat = null, _cubeNorm = null;
        public static GLShape Flat { get { _cubeFlat ??= BuildFlat(); return _cubeFlat; } }
        public static GLShape Norm { get { _cubeNorm ??= BuildNorm(); return _cubeNorm; } }

        private static GLShape BuildFlat() {
            VertexPos3D[] vertices = new VertexPos3D[] {
                new VertexPos3D(new Vector3( 0.5f,  0.5f,  0.5f)),
                new VertexPos3D(new Vector3(-0.5f,  0.5f,  0.5f)),
                new VertexPos3D(new Vector3(-0.5f, -0.5f,  0.5f)),
                new VertexPos3D(new Vector3( 0.5f, -0.5f,  0.5f)),
                new VertexPos3D(new Vector3( 0.5f,  0.5f, -0.5f)),
                new VertexPos3D(new Vector3(-0.5f,  0.5f, -0.5f)),
                new VertexPos3D(new Vector3(-0.5f, -0.5f, -0.5f)),
                new VertexPos3D(new Vector3( 0.5f, -0.5f, -0.5f)),
              };
            int[] indices = new int[36] { 0, 1, 2, 0, 2, 3, 0, 4, 1, 4, 5, 1, 1, 5, 2, 5, 6, 2, 2, 6, 3, 6, 7, 3, 3, 7, 0, 7, 4, 0, 4, 6, 5, 4, 7, 6 };
            return new GLShape(vertices, indices);
        }

        private static GLShape BuildNorm() {
            List<IVertex> vertexData = [];
            List<Vector3> vertices = [
                new Vector3( 0.5f,  0.5f,  0.5f),
                new Vector3(-0.5f,  0.5f,  0.5f),
                new Vector3(-0.5f, -0.5f,  0.5f),
                new Vector3( 0.5f, -0.5f,  0.5f),
                new Vector3( 0.5f,  0.5f, -0.5f),
                new Vector3(-0.5f,  0.5f, -0.5f),
                new Vector3(-0.5f, -0.5f, -0.5f),
                new Vector3( 0.5f, -0.5f, -0.5f)
            ];
            int[] indices = new int[36] { 0, 1, 2, 0, 2, 3, 0, 4, 1, 4, 5, 1, 1, 5, 2, 5, 6, 2, 2, 6, 3, 6, 7, 3, 3, 7, 0, 7, 4, 0, 4, 6, 5, 4, 7, 6 };
            Vector3[] normals = [
                new Vector3( 0f, 0f, 1f),
                new Vector3( 0f, 1f, 0f),
                new Vector3(-1f, 0f, 0f),
                new Vector3( 0f,-1f, 0f),
                new Vector3( 1f, 0f, 0f),
                new Vector3( 0f, 0f,-1f)
            ];
            Vector2 tex = new Vector2(0f, 0f); // Ignore
            for (int face = 0; face < 12; face++) {
                Vector3 p1 = vertices[indices[face * 3 + 0]];
                Vector3 p2 = vertices[indices[face * 3 + 1]];
                Vector3 p3 = vertices[indices[face * 3 + 2]];
                Vector3 norm = normals[face / 2];
                vertexData.Add(new VertexPos3DTexNorm(p1, tex, norm));
                vertexData.Add(new VertexPos3DTexNorm(p2, tex, norm));
                vertexData.Add(new VertexPos3DTexNorm(p3, tex, norm));
            }
            return new GLShape(vertexData.ToArray(), Enumerable.Range(0, vertexData.Count).ToArray());
        }
    }
}
