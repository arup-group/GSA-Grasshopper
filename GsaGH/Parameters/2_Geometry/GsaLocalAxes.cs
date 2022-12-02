using Rhino.Geometry;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace GsaGH.Parameters
{
  public class GsaLocalAxes
  {
    public Vector3d X { get; }
    public Vector3d Y { get; }
    public Vector3d Z { get; }

    public GsaLocalAxes(Vector3d x, Vector3d y, Vector3d z)
    {
      this.X = x;
      this.Y = y;
      this.Z = z;
    }

    public GsaLocalAxes(List<double> collection, int id)
    {
      this.X = new Vector3d(collection[0], collection[3], collection[6]);
      this.Y = new Vector3d(collection[1], collection[4], collection[7]);
      this.Z = new Vector3d(collection[2], collection[5], collection[8]);
    }
  }
}
