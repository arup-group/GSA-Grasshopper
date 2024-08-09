using System;

using GsaAPI;

using GsaGH.Parameters.Enums;

using OasysUnits;

using Rhino.Geometry;

using LengthUnit = OasysUnits.Units.LengthUnit;

namespace GsaGH.Parameters {
  public class GsaGridPointLoad : IGsaGridLoad {
    public GsaGridPlaneSurface GridPlaneSurface { get; set; } = new GsaGridPlaneSurface();
    public GridPointLoad ApiLoad { get; set; } = new GridPointLoad();
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

    public GsaGridPointLoad() {
      ApiLoad.Direction = Direction.Z;
    }

    public IGsaLoad Duplicate() {
      var dup = new GsaGridPointLoad {
        ApiLoad = {
          AxisProperty = ApiLoad.AxisProperty,
          Case = ApiLoad.Case,
          Direction = ApiLoad.Direction,
          GridSurface = ApiLoad.GridSurface,
          Name = ApiLoad.Name.ToString(),
          X = ApiLoad.X,
          Y = ApiLoad.Y,
          Value = ApiLoad.Value,
        },
        GridPlaneSurface = GridPlaneSurface.Duplicate(),
      };

      if (LoadCase != null) {
        dup.LoadCase = LoadCase;
      }

      return dup;
    }

    internal Point3d GetPoint(LengthUnit unit) {
      LengthUnit m = LengthUnit.Meter;
      double z = 0;
      if (GridPlaneSurface != null) {
        z = GridPlaneSurface.Plane.OriginZ;
      }

      return new Point3d(
              new Length(ApiLoad.X, m).As(unit),
              new Length(ApiLoad.Y, m).As(unit),
              new Length(z, m).As(unit));
    }
  }
}
