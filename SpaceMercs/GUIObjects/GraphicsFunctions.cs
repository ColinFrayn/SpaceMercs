using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;

namespace SpaceMercs {
  // Common graphics elements
  static class GraphicsFunctions {
    // Setup a graphics object representing a sphere
    public static GraphicsObject SetupToroid(int nslices, int ntheta, float radius) {
      GraphicsObject obj = new GraphicsObject();
      
      double majorStep = 2.0 * Math.PI / (double)nslices;
      double minorStep = 2.0 * Math.PI / (double)ntheta;

      for (int slice = 0; slice < nslices; slice++) {
        double a0 = slice * majorStep;
        double a1 = a0 + majorStep;
        double x0 = Math.Cos(a0);
        double y0 = Math.Sin(a0);
        double x1 = Math.Cos(a1);
        double y1 = Math.Sin(a1);

        for (int theta = 0; theta < ntheta; theta++) {
          double b = theta * minorStep;
          double c1 = Math.Cos(b);
          double c2 = Math.Cos(b + minorStep);
          double r1 = radius * c1 + 1.0f;
          double r2 = radius * c2 + 1.0f;
          double z1 = radius * Math.Sin(b);
          double z2 = radius * Math.Sin(b + minorStep);

          // Setup the first face
          Face newFace = new Face(new Vector3d(x0 * r1, y0 * r1, z1), new Vector3d(x0 * r2, y0 * r2, z2), new Vector3d(x1 * r2, y1 * r2, z2));

          // Calculate the normal
          newFace.n1 = new Vector3d(x0 * r1 - x0, y0 * r1 - y0, z1);
          newFace.n2 = new Vector3d(x0 * r2 - x0, y0 * r2 - y0, z2);
          newFace.n3 = new Vector3d(x1 * r2 - x1, y1 * r2 - y1, z2);
          newFace.n1.Normalize();
          newFace.n2.Normalize();
          newFace.n3.Normalize();

          // Texture coordinates
          newFace.t1 = new Vector2d(1.0f - ((double)slice / (double)nslices), (double)theta / (double)ntheta);
          newFace.t2 = new Vector2d(1.0f - ((double)slice / (double)nslices), (double)(theta+1) / (double)ntheta);
          newFace.t3 = new Vector2d(1.0f - ((double)(slice+1) / (double)nslices), (double)(theta+1) / (double)ntheta);

          obj.AddFace(newFace);

          // Setup the first face
          newFace = new Face(new Vector3d(x0 * r1, y0 * r1, z1), new Vector3d(x1 * r2, y1 * r2, z2), new Vector3d(x1 * r1, y1 * r1, z1));

          // Calculate the normal
          newFace.n1 = new Vector3d(x0 * r1 - x0, y0 * r1 - y0, z1);
          newFace.n2 = new Vector3d(x1 * r2 - x1, y1 * r2 - y1, z2);
          newFace.n3 = new Vector3d(x1 * r1 - x1, y1 * r1 - y1, z1);
          newFace.n1.Normalize();
          newFace.n2.Normalize();
          newFace.n3.Normalize();

          // Texture coordinates
          newFace.t1 = new Vector2d(1.0f - ((double)slice / (double)nslices), (double)theta / (double)ntheta);
          newFace.t2 = new Vector2d(1.0f - ((double)(slice + 1) / (double)nslices), (double)(theta + 1) / (double)ntheta);
          newFace.t3 = new Vector2d(1.0f - ((double)(slice + 1) / (double)nslices), (double)theta / (double)ntheta);

          obj.AddFace(newFace);

        }
      }

      return obj;
    }

    // Setup a graphics object representing a cylinder
    public static GraphicsObject SetupCylinder(double radius, double depth, int slices) {
      GraphicsObject obj = new GraphicsObject();

      // Make cylinders grow down
      depth = -depth;

      double theta2 = 0.0, theta1;
      for (int sno = 1; sno <= slices; sno++) {
        theta1 = theta2;
        theta2 = (double)sno * 2.0 * Math.PI / (double)slices;
        double x1 = Math.Cos(theta1);
        double y1 = Math.Sin(theta1);
        double x2 = Math.Cos(theta2);
        double y2 = Math.Sin(theta2);
        Face ftop = new Face(new Vector3d(0.0, 0.0, 0.0), new Vector3d(x1 * radius, y1 * radius, 0.0), new Vector3d(x2 * radius, y2 * radius, 0.0));
        ftop.n1 = new Vector3d(0.0, 0.0, 1.0);
        ftop.n2 = new Vector3d(0.0, 0.0, 1.0);
        ftop.n3 = new Vector3d(0.0, 0.0, 1.0);
        obj.AddFace(ftop);
        Face fbase = new Face(new Vector3d(0.0, 0.0, depth), new Vector3d(x2 * radius, y2 * radius, depth), new Vector3d(x1 * radius, y1 * radius, depth));
        fbase.n1 = new Vector3d(0.0, 0.0, -1.0);
        fbase.n2 = new Vector3d(0.0, 0.0, -1.0);
        fbase.n3 = new Vector3d(0.0, 0.0, -1.0);
        obj.AddFace(fbase);
        Face fside1 = new Face(new Vector3d(x2 * radius, y2 * radius, 0.0), new Vector3d(x1 * radius, y1 * radius, 0.0), new Vector3d(x1 * radius, y1 * radius, depth));
        fside1.n1 = new Vector3d(x2, y2, 0.0);
        fside1.n2 = new Vector3d(x1, y1, 0.0);
        fside1.n3 = new Vector3d(x1, y1, 0.0);
        obj.AddFace(fside1);
        Face fside2 = new Face(new Vector3d(x1 * radius, y1 * radius, depth), new Vector3d(x2 * radius, y2 * radius, depth), new Vector3d(x2 * radius, y2 * radius, 0.0));
        fside2.n1 = new Vector3d(x1, y1, 0.0);
        fside2.n2 = new Vector3d(x2, y2, 0.0);
        fside2.n3 = new Vector3d(x2, y2, 0.0);
        obj.AddFace(fside2);
      }

      return obj;
    }

    // Draw a cuboid at the current location
    public static void DrawCuboid(float scalex, float scaley, float scalez) {
      Vector3[] vertex = new Vector3[8];
      vertex[0] = new Vector3(-scalex, -scaley, scalez);
      vertex[1] = new Vector3(-scalex, scaley, scalez);
      vertex[2] = new Vector3(scalex, scaley, scalez);
      vertex[3] = new Vector3(scalex, -scaley, scalez);
      vertex[4] = new Vector3(-scalex, -scaley, -scalez);
      vertex[5] = new Vector3(-scalex, scaley, -scalez);
      vertex[6] = new Vector3(scalex, scaley, -scalez);
      vertex[7] = new Vector3(scalex, -scaley, -scalez);

      Vector3[] normal = new Vector3[6];
      normal[0] = new Vector3(0.0f, 0.0f, 1.0f);
      normal[1] = new Vector3(0.0f, -1.0f, 0.0f);
      normal[2] = new Vector3(-1.0f, 0.0f, 0.0f);
      normal[3] = new Vector3(0.0f, 1.0f, 0.0f);
      normal[4] = new Vector3(1.0f, 0.0f, 0.0f);
      normal[5] = new Vector3(0.0f, 0.0f, -1.0f);

      // Details of the face members
      int[,] faces = { { 0, 3, 2, 1 }, { 0, 4, 7, 3 }, { 4, 0, 1, 5 }, { 2, 6, 5, 1 }, { 3, 7, 6, 2 }, { 7, 4, 5, 6 } };

      // Draw the cuboid itself
      GL.Begin(BeginMode.Quads);
      for (int n = 0; n < 6; n++) {
        normal[n].Normalize();
        GL.Normal3(normal[n]);
        for (int v = 0; v < 4; v++) {
          GL.Vertex3(vertex[(faces[n, v])]);
        }
      }
      GL.End();
    }

    // Draw a prism.
    // Points on first prism are clockwise as viewed from outside
    // Points on second face are anticlockwise, so that they match up on index
    public static void DrawPrism(List<Vector3> face1, List<Vector3> face2) {
      if (face1.Count != face2.Count) {
        throw new Exception("Trying to create a prism with unequal faces");
      }

      // Draw face 1
      {
        Vector3 norm = Vector3.Cross(Vector3.Subtract(face1[2], face1[1]), Vector3.Subtract(face1[1], face1[0]));
        norm.Normalize();
        GL.Normal3(norm);
        GL.Begin(BeginMode.Polygon);
        foreach (Vector3 v in face1) {
          GL.Vertex3(v);
        }
        GL.End();
      }

      // Draw face 2
      {
        Vector3 norm = Vector3.Cross(Vector3.Subtract(face2[1], face2[0]), Vector3.Subtract(face2[2], face2[1]));
        norm.Normalize();
        GL.Normal3(norm);
        GL.Begin(BeginMode.Polygon);
        // Draw these points clockwise, too
        for (int i=face2.Count-1; i>=0; i--) {
          GL.Vertex3(face2[i]);
        }
        GL.End();
      }

      // Draw joining faces
      {
        GL.Begin(BeginMode.Quads);
        for (int n = 0; n < face1.Count; n++) {
          int nextid = n + 1;
          if (nextid == face1.Count) nextid = 0;
          Vector3 norm = Vector3.Cross(Vector3.Subtract(face1[n], face2[n]), Vector3.Subtract(face1[nextid], face1[n]));
          norm.Normalize();
          GL.Normal3(norm);
          GL.Vertex3(face1[n]);
          GL.Vertex3(face2[n]);
          GL.Vertex3(face2[nextid]);
          GL.Vertex3(face1[nextid]);
        }
        GL.End();
      }
    }

    // Setup a hemispherical framework
    public static GraphicsObject SetupFramework(int level) {
      GraphicsObject obj = new GraphicsObject();
      double hsqrt2 = Math.Sqrt(2.0)/2.0;

      // Coordinates for the vertices of a regular tetrahedron
      Vector3d[] vertex = new Vector3d[5];
      vertex[0] = new Vector3d(0.0, 0.0, 1.0);
      vertex[1] = new Vector3d(-hsqrt2, hsqrt2, 0.0);
      vertex[2] = new Vector3d(hsqrt2, hsqrt2, 0.0);
      vertex[3] = new Vector3d(hsqrt2, -hsqrt2, 0.0);
      vertex[4] = new Vector3d(-hsqrt2, -hsqrt2, 0.0);

      // Recurse
      SubdivideFrame(vertex[1], vertex[0], vertex[4], level, obj);
      SubdivideFrame(vertex[2], vertex[0], vertex[1], level, obj);
      SubdivideFrame(vertex[3], vertex[0], vertex[2], level, obj);
      SubdivideFrame(vertex[4], vertex[0], vertex[3], level, obj);

      return obj;
    }

    // Recursive subdivision of a triangular surface into framework.  Start
    // with a tetrahedron and then recursively subdivide to the required depth.
    private static void SubdivideFrame(Vector3d v1, Vector3d v2, Vector3d v3, int depth, GraphicsObject obj) {
      Vector3d v12, v23, v31;

      if (depth == 0) {
        Face f = new Face(v1, v2, v3);
        f.n1 = v1;
        f.n2 = v2;
        f.n3 = v3;
        obj.AddFace(f);
        return;
      }

      v12 = (v1 + v2) / 2.0;
      v23 = (v2 + v3) / 2.0;
      v31 = (v3 + v1) / 2.0;

      v12.Normalize();
      v23.Normalize();
      v31.Normalize();
      SubdivideFrame(v1, v12, v31, depth - 1, obj);
      SubdivideFrame(v2, v23, v12, depth - 1, obj);
      SubdivideFrame(v3, v31, v23, depth - 1, obj);
      SubdivideFrame(v12, v23, v31, depth - 1, obj);
    }

    // Selection icons
    public static void DrawHoverReticule(double scale) {
      GL.PushMatrix();
      GL.Scale(scale, scale, scale);
      GL.Color3(1.0, 1.0, 1.0);
      GL.Begin(BeginMode.QuadStrip);
      for (int n = 0; n <= 32; n++) {
        double ang = n * Math.PI / 16.0;
        GL.Vertex3(Math.Cos(ang), Math.Sin(ang), 0.0);
        GL.Vertex3(Math.Cos(ang) * 1.2, Math.Sin(ang) * 1.2, 0.0);
      }
      GL.End();
      GL.PopMatrix();
    }
    public static void DrawSelectedReticule(double scale) {
      GL.PushMatrix();
      GL.Scale(scale, scale, scale);
      GL.Color3(0.2, 1.0, 0.4);
      GL.Begin(BeginMode.QuadStrip);
      for (int n = 0; n <= 32; n++) {
        double ang = n * Math.PI / 16.0;
        GL.Vertex3(Math.Cos(ang), Math.Sin(ang), 0.0);
        GL.Vertex3(Math.Cos(ang) * 1.25, Math.Sin(ang) * 1.25, 0.0);
      }
      GL.End();
      GL.PopMatrix();
    }
    public static void DrawLocationIcon(double scale) {
      GL.PushMatrix();
      GL.Scale(scale, scale, scale);
      //GL.Rotate(45.0, Vector3d.UnitZ);
      GL.Disable(EnableCap.DepthTest);
      GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);
      GL.Color3(1.0, 1.0, 1.0);
      GL.Begin(BeginMode.Triangles);
      GL.Vertex2(-0.8, -0.7);
      GL.Vertex2(0.8, 0.0);
      GL.Vertex2(-0.8, 0.7);
      GL.End();
      GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line);
      GL.Color3(0.0, 0.0, 0.0);
      GL.Begin(BeginMode.Triangles);
      GL.Vertex2(-0.8, -0.7);
      GL.Vertex2(0.8, 0.0);
      GL.Vertex2(-0.8, 0.7);
      GL.End();
      GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);
      GL.Enable(EnableCap.DepthTest);
      //GL.Begin(BeginMode.Triangles);
      //GL.Vertex2(-0.25, -1.5);
      //GL.Vertex2(0.25, -1.5);
      //GL.Vertex2(0.0, -1.0);
      //GL.Vertex2(-0.25, 1.5);
      //GL.Vertex2(0.25, 1.5);
      //GL.Vertex2(0.0, 1.0);
      //GL.Vertex2(-1.5, -0.25);
      //GL.Vertex2(-1.5, 0.25);
      //GL.Vertex2(-1.0, 0.0);
      //GL.Vertex2(1.5, -0.25);
      //GL.Vertex2(1.5, 0.25);
      //GL.Vertex2(1.0, 0.0);
      //GL.End();
      GL.PopMatrix();
    }
    public static void DrawInactiveIcon(double scale) {
      GL.PushMatrix();
      GL.Scale(scale / 2.0, scale / 2.0, scale / 2.0);
      GL.Color3(1.0, 0.0, 0.0);
      //GL.Begin(BeginMode.QuadStrip);
      //for (int n = 0; n <= 16; n++) {
      //  double ang = n * Math.PI / 8.0;
      //  GL.Vertex3(Math.Cos(ang), Math.Sin(ang), 0.0);
      //  GL.Vertex3(Math.Cos(ang) * 1.2, Math.Sin(ang) * 1.2, 0.0);
      //}
      //GL.End();
      double sr2 = Math.Sqrt(2.0) / 2.0;
      GL.Begin(BeginMode.Quads);
      GL.Vertex3(0.9, 1.0, 0.0);
      GL.Vertex3(1.0, 0.9, 0.0);
      GL.Vertex3(-0.9, -1.0, 0.0);
      GL.Vertex3(-1.0, -0.9, 0.0);
      //GL.Vertex3(sr2, sr2, 0.0);
      //GL.Vertex3(sr2 + 0.1, sr2 + 0.1, 0.0);
      //GL.Vertex3(-sr2, -sr2, 0.0);
      //GL.Vertex3(-(sr2 + 0.1), -(sr2 + 0.1), 0.0);
      GL.End();
      GL.PopMatrix();
    }

    // Other random 2D geometry
    public static void DrawRhomboid(double scale) {
      GL.Begin(BeginMode.Quads);
      GL.Vertex3(0.0, -scale / 2.0, 0.0);
      GL.Vertex3(scale / 2.0, 0.0, 0.0);
      GL.Vertex3(0.0, scale / 2.0, 0.0);
      GL.Vertex3(-scale / 2.0, 0.0, 0.0);
      GL.End();
    }
    public static void DrawCross(double scale) {
      GL.Begin(BeginMode.Quads);
      GL.Vertex3(-scale * 0.15, -scale * 0.5, 0.0);
      GL.Vertex3(scale * 0.15, -scale * 0.5, 0.0);
      GL.Vertex3(scale * 0.15, scale * 0.5, 0.0);
      GL.Vertex3(-scale * 0.15, scale * 0.5, 0.0);
      GL.Vertex3(scale * 0.5, -scale * 0.15, 0.0);
      GL.Vertex3(scale * 0.5, scale * 0.15, 0.0);
      GL.Vertex3(-scale * 0.5, scale * 0.15, 0.0);
      GL.Vertex3(-scale * 0.5, -scale * 0.15, 0.0);
      GL.End();
    }

    // A timer icon
    public static void DrawTimer(double dFract) {
      const double ostep = Math.PI / 16.0;
      GL.Color3(0.0, 0.4, 1.0);
      GL.Begin(BeginMode.TriangleFan);
      GL.Vertex3(0.0, 0.0, 0.0);
      for (double n = 0.0; n <= 2.0 * Math.PI * dFract; n += ostep) {
        GL.Vertex3(Math.Sin(n), Math.Cos(n), 0.0);
      }
      GL.End();
      GL.Color3(1.0, 1.0, 1.0);
      GL.Begin(BeginMode.LineLoop);
      for (double n = 0.0; n < 2.0 * Math.PI; n += ostep) {
        GL.Vertex3(Math.Sin(n), Math.Cos(n), 0.0);
      }
      GL.End();
      GL.Begin(BeginMode.Lines);
      for (double n = 0.0; n < 2.0 * Math.PI; n += Math.PI / 6.0) {
        GL.Vertex3(Math.Sin(n) * 0.9, Math.Cos(n) * 0.9, 0.0);
        GL.Vertex3(Math.Sin(n), Math.Cos(n), 0.0);
      }
      GL.End();
    }

    // A fractional bar
    public static void DisplayFractBar(Color col, Color bg, double startx, double starty, double width, double height, double val, double maxval) {
      if (maxval <= 0.0) return;
      double fract = val / maxval;
      if (fract < 0.0f) fract = 0.0f;
      if (fract > 1.0f) fract = 1.0f;
      GL.Begin(BeginMode.Quads);
      double endx = startx + (width * fract);
      if (fract < 1.0f) {
        GL.Color4(bg.R / 255.0, bg.G / 255.0, bg.B / 255.0, Const.GUIAlpha);
        GL.Vertex2(endx, starty);
        GL.Vertex2(startx + width, starty);
        GL.Vertex2(startx + width, starty + height);
        GL.Vertex2(endx, starty + height);
      }
      GL.Color4(col.R / 255.0, col.G / 255.0, col.B / 255.0, Const.GUIAlpha);
      GL.Vertex2(startx, starty);
      GL.Vertex2(endx, starty);
      GL.Vertex2(endx, starty + height);
      GL.Vertex2(startx, starty + height);
      GL.End();
    }

  }
}
