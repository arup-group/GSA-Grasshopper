using GsaAPI;
using OasysUnits;
using LengthUnit = OasysUnits.Units.LengthUnit;

namespace GsaGH.Parameters._5_Results.Quantities {
  public interface ITranslation : IResult{
    public Length X { get; }
    public Length Xyz { get; }
    public Length Y { get; }
    public Length Z { get; }

    public void SetLengthUnit(Double6 result, LengthUnit unit);
    public void SetLengthUnit(Length x, Length y, Length z, Length xyz);
  }
}
