using GsaAPI;
using OasysUnits;
using Rhino.Geometry;
using LengthUnit = OasysUnits.Units.LengthUnit;

namespace GsaGH.Parameters {
  public class GsaGridPointLoad {
    public GsaGridPlaneSurface GridPlaneSurface { get; set; } = new GsaGridPlaneSurface();
    public GridPointLoad GridPointLoad { get; set; } = new GridPointLoad();
    public GsaGridPointLoad() { }

    public GsaGridPointLoad Duplicate() {
      var dup = new GsaGridPointLoad {
        GridPointLoad = {
          AxisProperty = GridPointLoad.AxisProperty,
          Case = GridPointLoad.Case,
          Direction = GridPointLoad.Direction,
          GridSurface = GridPointLoad.GridSurface,
          Name = GridPointLoad.Name.ToString(),
          X = GridPointLoad.X,
          Y = GridPointLoad.Y,
          Value = GridPointLoad.Value,
        },
        GridPlaneSurface = GridPlaneSurface.Duplicate(),
      };
      return dup;
    }

    internal Point3d GetPoint(LengthUnit unit) {
      LengthUnit m = LengthUnit.Meter;
      return new Point3d(
              new Length(GridPointLoad.X, m).As(unit),
              new Length(GridPointLoad.Y, m).As(unit),
              new Length(GridPlaneSurface.Plane.OriginZ, m).As(unit));
    }
  }
}
