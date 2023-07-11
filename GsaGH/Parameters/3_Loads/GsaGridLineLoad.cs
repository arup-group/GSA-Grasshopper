using GsaAPI;
using Rhino.Geometry;
using System.Collections.Generic;
using System.Linq;

namespace GsaGH.Parameters {
  public class GsaGridLineLoad {
    public GridLineLoad GridLineLoad { get; set; } = new GridLineLoad();
    public GsaGridPlaneSurface GridPlaneSurface { get; set; } = new GsaGridPlaneSurface();
    internal List<Point3d> Points { get; set; } = new List<Point3d>();

    public GsaGridLineLoad() {
      GridLineLoad.PolyLineReference = 0;
    }
    public GsaGridLineLoad Duplicate() {
      var dup = new GsaGridLineLoad {
        GridLineLoad = {
          AxisProperty = GridLineLoad.AxisProperty,
          Case = GridLineLoad.Case,
          Direction = GridLineLoad.Direction,
          GridSurface = GridLineLoad.GridSurface,
          IsProjected = GridLineLoad.IsProjected,
          Name = GridLineLoad.Name.ToString(),
          PolyLineDefinition = GridLineLoad.PolyLineDefinition.ToString(),
          PolyLineReference = GridLineLoad.PolyLineReference,
          Type = GridLineLoad.Type,
          ValueAtStart = GridLineLoad.ValueAtStart,
          ValueAtEnd = GridLineLoad.ValueAtEnd,
        },
        GridPlaneSurface = GridPlaneSurface.Duplicate(),
        Points = Points.ToList(),
      };
      return dup;
    }
  }
}
