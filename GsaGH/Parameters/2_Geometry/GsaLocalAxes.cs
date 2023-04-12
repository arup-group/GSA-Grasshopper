using System.Collections.Generic;
using Rhino.Geometry;

namespace GsaGH.Parameters {
  public class GsaLocalAxes {
    public Vector3d X { get; }
    public Vector3d Y { get; }
    public Vector3d Z { get; }

    public GsaLocalAxes(Vector3d x, Vector3d y, Vector3d z) {
      X = x;
      Y = y;
      Z = z;
    }

    public GsaLocalAxes(IReadOnlyList<double> collection) {
      X = new Vector3d(collection[0], collection[3], collection[6]);
      Y = new Vector3d(collection[1], collection[4], collection[7]);
      Z = new Vector3d(collection[2], collection[5], collection[8]);
    }
  }
}
