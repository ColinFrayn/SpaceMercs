using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;

namespace SpaceMercs {
  class GraphicsObject {
    List<Face> faces;
    Dictionary<IGraphicsContext,int> iDisplayList;
    
    // Construct a default empty object
    public GraphicsObject() {
      faces = new List<Face>();
      iDisplayList = new Dictionary<IGraphicsContext, int>();
    }

    // Add a new face to this object
    public void AddFace(Face f) {
      if (f != null) faces.Add(f);
    }
    
    // Draw this object
    public void Draw(IGraphicsContext currentContext) {
      if (!iDisplayList.ContainsKey(currentContext) || iDisplayList[currentContext] == -1) SetupDisplayList(currentContext);
      GL.CallList(iDisplayList[currentContext]);
    }
    public void DrawFaces() {
      GL.Begin(BeginMode.Triangles);
      for (int p = 0; p < faces.Count; p++) {
        GL.Normal3(faces[p].n1);
        GL.TexCoord2(faces[p].t1);
        GL.Vertex3(faces[p].p1);
        GL.Normal3(faces[p].n2);
        GL.TexCoord2(faces[p].t2);
        GL.Vertex3(faces[p].p2);
        GL.Normal3(faces[p].n3);
        GL.TexCoord2(faces[p].t3);
        GL.Vertex3(faces[p].p3);
      }
      GL.End();
    }

    private void SetupDisplayList(IGraphicsContext currentContext) {
      // Create the id for the list
      iDisplayList[currentContext] = GL.GenLists(1);
      // Start list
      GL.NewList(iDisplayList[currentContext], ListMode.Compile);
      DrawFaces();
      GL.EndList();
    }
  }

  // A triangular face on a sphere
  class Face {
    public Vector3d p1 { get; set; }
    public Vector3d p2 { get; set; }
    public Vector3d p3 { get; set; }
    public Vector2d t1 { get; set; }
    public Vector2d t2 { get; set; }
    public Vector2d t3 { get; set; }
    public Vector3d n1 { get; set; }
    public Vector3d n2 { get; set; }
    public Vector3d n3 { get; set; }
    public Face(Vector3d _p1, Vector3d _p2, Vector3d _p3) {
      p1 = _p1;
      p2 = _p2;
      p3 = _p3;
    }
  }

}
