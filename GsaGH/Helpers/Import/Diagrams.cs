using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Threading.Tasks;

using GsaAPI;

using Rhino.Geometry;

using Line = Rhino.Geometry.Line;

namespace GsaGH.Helpers.Import {
  /// <summary>
  ///   Class containing functions to import various object types from GSA
  /// </summary>
  internal static class Diagrams {
    internal static Line ConvertLine(GsaAPI.Line line, double scaleFactor) {
      return new Line(
        ConvertPoint(line.Start, scaleFactor),
        ConvertPoint(line.End, scaleFactor));
    }

    internal static Mesh CreateMeshFromTriangles(
      ReadOnlyCollection<Triangle> triangles, double scaleFactor, Color customColor) {
      var faces = new ConcurrentBag<Mesh>();
      Parallel.ForEach(triangles, tri => {
        var face = new Mesh();
        var col = (Color)tri.Colour;
        if (!customColor.IsEmpty) {
          col = customColor;
        }

        foreach (Vector3 verticy in tri.Vertices) {
          face.Vertices.Add(ConvertPoint(verticy, scaleFactor));
          face.VertexColors.Add(col);
        }

        face.Faces.AddFace(0, 1, 2);
        faces.Add(face);
      });
      var mesh = new Mesh();
      mesh.Append(faces);
      mesh.Vertices.CombineIdentical(true, false);
      return mesh;
    }

    private static Point3d ConvertPoint(Vector3 vector3, double scaleFactor) {
      return new Point3d(
        vector3.X * scaleFactor,
        vector3.Y * scaleFactor,
        vector3.Z * scaleFactor);
    }
  }
}
