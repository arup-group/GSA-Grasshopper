
using LengthUnit = OasysUnits.Units.LengthUnit;
namespace GsaGH.Helpers.GH {
  public abstract class GsaGeometryBase {
    public LengthUnit LengthUnit { get; set; } = LengthUnit.Meter;

    protected GsaGeometryBase() { }

    protected GsaGeometryBase(LengthUnit unit) {
      LengthUnit = unit;
    }
  }
}
