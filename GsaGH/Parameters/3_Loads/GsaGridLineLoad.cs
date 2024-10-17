using System;

using GsaAPI;

using GsaGH.Parameters.Enums;

using Rhino.Collections;

namespace GsaGH.Parameters {
  public class GsaGridLineLoad : IGsaGridLoad {
    public GridLineLoad ApiLoad { get; set; } = new GridLineLoad();
    public Polyline ApiPolyline { get; internal set; }
    public GsaGridPlaneSurface GridPlaneSurface { get; set; } = new GsaGridPlaneSurface();
    public GsaLoadCase LoadCase { get; set; }
    public ReferenceType ReferenceType => GridPlaneSurface._referenceType;
    public GsaList ReferenceList => GridPlaneSurface._refList;
    public Guid RefObjectGuid => GridPlaneSurface._refObjectGuid;
    public Point3dList Points { get; set; } = new Point3dList();
    public int CaseId {
      get => ApiLoad.Case;
      set => ApiLoad.Case = value;
    }
    public string Name {
      get => ApiLoad.Name;
      set => ApiLoad.Name = value;
    }

    public GsaGridLineLoad() {
      ApiLoad.PolyLineReference = 0;
      ApiLoad.Direction = Direction.Z;
    }

    public IGsaLoad Duplicate() {
      var dup = new GsaGridLineLoad {
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
          ValueAtStart = ApiLoad.ValueAtStart,
          ValueAtEnd = ApiLoad.ValueAtEnd,
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
