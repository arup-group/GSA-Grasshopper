using GsaAPI;

using OasysUnits;

using AngleUnit = OasysUnits.Units.AngleUnit;
using LengthUnit = OasysUnits.Units.LengthUnit;

namespace GsaGH.Parameters.Results {
  public class Drift : IDrift<Length> {
    public Length X { get; internal set; }
    public Length Y { get; internal set; }
    public Length Xy { get; internal set; }

    internal Drift(AssemblyDriftResult result) {
      X = new Length(result.X, LengthUnit.Meter);
      Y = new Length(result.Y, LengthUnit.Meter);
      Xy = new Length(result.XY, LengthUnit.Meter);
    }
  }
}
