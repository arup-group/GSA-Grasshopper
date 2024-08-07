using GsaAPI;

using OasysUnits;
using OasysUnits.Units;

namespace GsaGH.Parameters.Results {
  public class Shear2d : IShear2d {
    public ForcePerLength Qx { get; internal set; }
    public ForcePerLength Qy { get; internal set; }

    internal Shear2d(Vector2 result) {
      Qx = new ForcePerLength(result.X, ForcePerLengthUnit.NewtonPerMeter);
      Qy = new ForcePerLength(result.Y, ForcePerLengthUnit.NewtonPerMeter);
    }
  }
}
