using GsaAPI;
using OasysUnits;
using OasysUnits.Units;
using AngleUnit = OasysUnits.Units.AngleUnit;
using LengthUnit = OasysUnits.Units.LengthUnit;

namespace GsaGH.Parameters.Results {
  public class Shear2d : IShear2d {
    public ForcePerLength Vx { get; internal set; }
    public ForcePerLength Vy { get; internal set; }

    internal Shear2d(Vector2 result) {
      Vx = new ForcePerLength(result.X, ForcePerLengthUnit.NewtonPerMeter);
      Vy = new ForcePerLength(result.Y, ForcePerLengthUnit.NewtonPerMeter);
    }
  }
}
