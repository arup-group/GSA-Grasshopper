using Rhino.Collections;

namespace GsaGH.Helpers.GH {
  public static class Point3dListExtension {
    public static bool IsClosed(this Point3dList points) {
      return points[0] == points[points.Count - 1];
    }
  }
}
