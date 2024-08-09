using System.Collections.Generic;

using Rhino.Geometry;

namespace GsaGH.Parameters {
  /// <summary>
  /// <para>Refer to <see href="https://docs.oasys-software.com/structural/gsa/references/memberaxes.html">1D member axes</see> to read more.</para>
  /// </summary>
  public class LocalAxes {
    public Vector3d X { get; }
    public Vector3d Y { get; }
    public Vector3d Z { get; }

    public LocalAxes(IReadOnlyList<double> collection) {
      X = new Vector3d(collection[0], collection[3], collection[6]);
      Y = new Vector3d(collection[1], collection[4], collection[7]);
      Z = new Vector3d(collection[2], collection[5], collection[8]);
    }
  }
}
