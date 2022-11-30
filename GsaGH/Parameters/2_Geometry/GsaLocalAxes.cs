using Rhino.Geometry;
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

    public GsaLocalAxes(ReadOnlyCollection<double> collection)
    {
      this.X = new Vector3d(collection[0], collection[1], collection[2]);
      this.Y = new Vector3d(collection[3], collection[4], collection[5]);
      this.Z = new Vector3d(collection[6], collection[7], collection[8]);
    }
  }
}
