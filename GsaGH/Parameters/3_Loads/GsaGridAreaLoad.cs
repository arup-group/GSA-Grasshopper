using System;

using GsaAPI;

using GsaGH.Parameters.Enums;

using Rhino.Collections;

namespace GsaGH.Parameters {
  public class GsaGridAreaLoad : IGsaGridLoad {
    public GridAreaLoad ApiLoad { get; set; } = new GridAreaLoad();
    public Polyline ApiPolyline { get; internal set; }
    public GsaGridPlaneSurface GridPlaneSurface { get; set; } = new GsaGridPlaneSurface();
    public GsaLoadCase LoadCase { get; set; }
    public ReferenceType ReferenceType => GridPlaneSurface._referenceType;
    public GsaList ReferenceList => GridPlaneSurface._refList;
    public Guid RefObjectGuid => GridPlaneSurface._refObjectGuid;
    public int CaseId {
      get => ApiLoad.Case;
      set => ApiLoad.Case = value;
    }
    public string Name {
      get => ApiLoad.Name;
      set => ApiLoad.Name = value;
    }
    internal Point3dList Points { get; set; } = new Point3dList();

    public GsaGridAreaLoad() {
      ApiLoad.Type = GridAreaPolyLineType.PLANE;
      ApiLoad.Direction = Direction.Z;
    }

    public IGsaLoad Duplicate() {
      var dup = new GsaGridAreaLoad {
        ApiLoad = {
          AxisProperty = ApiLoad.AxisProperty,
          Case = ApiLoad.Case,
          Direction = ApiLoad.Direction,
          GridSurface = ApiLoad.GridSurface,
          IsProjected = ApiLoad.IsProjected,
          Name = ApiLoad.Name.ToString(),
          PolyLineDefinition = ApiLoad.PolyLineDefinition.ToString(),
          PolyLineReference = ApiLoad.PolyLineReference,
          Type = ApiLoad.Type,
          Value = ApiLoad.Value,
        },
        GridPlaneSurface = GridPlaneSurface.Duplicate(),
        Points = Points,
      };

      if (ApiPolyline != null) {
        dup.ApiPolyline = DuplicateApiPolyline();
      }

      if (LoadCase != null) {
        dup.LoadCase = LoadCase;
      }

      return dup;
    }

    private Polyline DuplicateApiPolyline() {
      var polyline = new Polyline(ApiPolyline.Points) {
        Name = ApiPolyline.Name
      };
      return polyline;
    }
  }
}
