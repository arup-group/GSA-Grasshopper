using System;
using System.Collections.Generic;
using System.Linq;
using GsaAPI;
using Rhino.Geometry;

namespace GsaGH.Parameters {
  public class GsaGridLineLoad : IGsaLoad {
    public GridLineLoad GridLineLoad { get; set; } = new GridLineLoad();
    public GsaGridPlaneSurface GridPlaneSurface { get; set; } = new GsaGridPlaneSurface();
    public ReferenceType ReferenceType => GridPlaneSurface._referenceType;
    public GsaList ReferenceList => GridPlaneSurface._refList;
    public Guid RefObjectGuid => GridPlaneSurface._refObjectGuid;
    internal List<Point3d> Points { get; set; } = new List<Point3d>();
    public LoadType LoadType => LoadType.GridLine;

    public GsaGridLineLoad() {
      GridLineLoad.PolyLineReference = 0;
    }

    public int CaseId() {
      return GridLineLoad.Case;
    }

    public IGsaLoad Duplicate() {
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

    public override string ToString() {
      return string.Join(" ", LoadType.ToString().Trim(), GridLineLoad.Name.Trim()).Trim().Replace("  ", " ");
    }
  }
}
