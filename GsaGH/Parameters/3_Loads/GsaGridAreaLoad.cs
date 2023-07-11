using GsaAPI;
using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GsaGH.Parameters {
  public class GsaGridAreaLoad : IGsaLoad {
    public GridAreaLoad GridAreaLoad { get; set; } = new GridAreaLoad();
    public GsaGridPlaneSurface GridPlaneSurface { get; set; } = new GsaGridPlaneSurface();
    public ReferenceType ReferenceType => GridPlaneSurface._referenceType;
    public GsaList ReferenceList => GridPlaneSurface._refList;
    public Guid RefObjectGuid => GridPlaneSurface._refObjectGuid;

    internal List<Point3d> Points { get; set; } = new List<Point3d>();
    public LoadType LoadType => LoadType.GridArea;

    public GsaGridAreaLoad() {
      GridAreaLoad.Type = GridAreaPolyLineType.PLANE;
    }

    public int CaseId() {
      return GridAreaLoad.Case;
    }

    public IGsaLoad Duplicate() {
      var dup = new GsaGridAreaLoad {
        GridAreaLoad = {
          AxisProperty = GridAreaLoad.AxisProperty,
          Case = GridAreaLoad.Case,
          Direction = GridAreaLoad.Direction,
          GridSurface = GridAreaLoad.GridSurface,
          IsProjected = GridAreaLoad.IsProjected,
          Name = GridAreaLoad.Name.ToString(),
          PolyLineDefinition = GridAreaLoad.PolyLineDefinition.ToString(),
          PolyLineReference = GridAreaLoad.PolyLineReference,
          Type = GridAreaLoad.Type,
          Value = GridAreaLoad.Value,
        },
        GridPlaneSurface = GridPlaneSurface.Duplicate(),
        Points = Points.ToList(),
      };
      return dup;
    }

    public override string ToString() {
      return string.Join(" ", LoadType.ToString().Trim(), GridAreaLoad.Name.Trim()).Trim().Replace("  ", " ");
    }
  }
}
