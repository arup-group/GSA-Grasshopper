
using OasysGH.Units;

using LengthUnit = OasysUnits.Units.LengthUnit;
namespace GsaGH.Helpers.GH {
  public abstract class GsaGeometryBase {
    public LengthUnit LengthUnit { get; set; }
    protected GsaGeometryBase() {
      LengthUnit = DefaultUnits.LengthUnitGeometry;
    }
    protected GsaGeometryBase(LengthUnit unit) {
      LengthUnit = unit;
    }
  }
}
