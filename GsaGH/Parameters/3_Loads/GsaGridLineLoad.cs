using System;
using System.Collections.Generic;
using System.Linq;
using GsaAPI;
using Rhino.Collections;
using Rhino.Geometry;

namespace GsaGH.Parameters {
  public class GsaGridLineLoad : IGsaLoad {
    public GridLineLoad GridLineLoad { get; set; } = new GridLineLoad();
    public GsaGridPlaneSurface GridPlaneSurface { get; set; } = new GsaGridPlaneSurface();
    public GsaLoadCase LoadCase { get; set; }
    public ReferenceType ReferenceType => GridPlaneSurface._referenceType;
    public GsaList ReferenceList => GridPlaneSurface._refList;
    public Guid RefObjectGuid => GridPlaneSurface._refObjectGuid;
    public LoadType LoadType => LoadType.GridLine;
    public int CaseId {
      get => GridLineLoad.Case;
      set => GridLineLoad.Case = value;
    }
    public string Name {
      get => GridLineLoad.Name;
      set => GridLineLoad.Name = value;
    }
    internal Point3dList Points { get; set; } = new Point3dList();

    public GsaGridLineLoad() {
      GridLineLoad.PolyLineReference = 0;
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
        Points = Points,
      };
      
      if (LoadCase != null) {
        dup.LoadCase = LoadCase;
      }

      return dup;
    }
  }
}
