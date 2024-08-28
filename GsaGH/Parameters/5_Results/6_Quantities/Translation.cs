using GsaAPI;

using OasysUnits;

using LengthUnit = OasysUnits.Units.LengthUnit;

namespace GsaGH.Parameters.Results {
  public class Translation : ITranslation {
    public Length X { get; internal set; }
    public Length Y { get; internal set; }
    public Length Z { get; internal set; }
    public Length Xyz { get; internal set; }

    internal Translation(Double3 result) {
      X = new Length(result.X, LengthUnit.Meter);
      Y = new Length(result.Y, LengthUnit.Meter);
      Z = new Length(result.Z, LengthUnit.Meter);
      Xyz = QuantityUtility.PythagoreanQuadruple(X, Y, Z);
    }
  }
}
