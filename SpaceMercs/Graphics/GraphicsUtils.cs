using OpenTK.Mathematics;
using System.Windows;

namespace SpaceMercs.Graphics {
    internal static class GraphicsUtils {

        // Which side of line v1->v2 is the point pt on?
        private static float Sign(Vector2 pt, Vector2 v1, Vector2 v2) {
            return (pt.X - v2.X) * (v1.Y - v2.Y) - (v1.X - v2.X) * (pt.Y - v2.Y);
        }

        // Is point pt inside triangle v1->v2->v3?
        private static bool PointIsInTriangle(Vector2 pt, Vector2 v1, Vector2 v2, Vector2 v3) {
            float d1, d2, d3;
            bool has_neg, has_pos;

            d1 = Sign(pt, v1, v2);
            d2 = Sign(pt, v2, v3);
            d3 = Sign(pt, v3, v1);

            has_neg = (d1 < 0) || (d2 < 0) || (d3 < 0);
            has_pos = (d1 > 0) || (d2 > 0) || (d3 > 0);

            return !(has_neg && has_pos);
        }

        public static List<Vector2> Triangulate(IReadOnlyList<Vector2> polygon) {
            List<Vector2> vertices = new List<Vector2>(polygon);
            List<Vector2> triangles = new List<Vector2>();
            while (vertices.Count > 2) {
                // Find an ear point
                int c = vertices.Count;
                for (int n=0; n<vertices.Count; n++) {
                    int pi1 = n - 1;
                    int pi2 = n;
                    int pi3 = n + 1;
                    if (pi1 < 0) pi1 = c - 1;
                    if (pi3 >= c) pi3 = 0;

                    //Vector2 s1 = vertices[pi2] - vertices[pi1];
                    //s1.Normalize();
                    //Vector2 s2 = vertices[pi3] - vertices[pi2];
                    //s2.Normalize();
                    //double ang = Math.Acos(s1.X * s2.X + s1.Y * s2.Y);
                    //if (ang >= Math.PI || ang <= 0.0) continue;

                    // Check if this triangle is actually inside the remaining polygon
                    // As we know that we're going through points clockwise, just check if this is a "left turn"
                    if (Sign(vertices[pi3], vertices[pi1], vertices[pi2]) > 0) continue; 

                    // Check this triangle for an ear
                    bool isEar = true;
                    for (int i = 0; i < c; i++) {
                        if (i != pi1 && i != pi2 && i != pi3) {
                            if (PointIsInTriangle(vertices[i], vertices[pi1], vertices[pi2], vertices[pi3])) {
                                isEar = false;
                                break;
                            }
                        }
                    }
                    if (isEar) {
                        triangles.Add(vertices[pi1]);
                        triangles.Add(vertices[pi2]);
                        triangles.Add(vertices[pi3]);
                        vertices.RemoveAt(n);
                        break;
                    }
                }

            }
            return triangles;
        }
    }
}
