using System;
using GsaAPI;
using OasysUnits;
using Rhino.Geometry;
using LengthUnit = OasysUnits.Units.LengthUnit;

namespace GsaGH.Parameters {
  public class GsaGridPointLoad : IGsaLoad {
    public GsaGridPlaneSurface GridPlaneSurface { get; set; } = new GsaGridPlaneSurface();
    public GridPointLoad GridPointLoad { get; set; } = new GridPointLoad();
    public GsaLoadCase LoadCase { get; set; }
    public LoadType LoadType => LoadType.GridPoint;
    public ReferenceType ReferenceType => GridPlaneSurface._referenceType;
    public GsaList ReferenceList => GridPlaneSurface._refList;
    public Guid RefObjectGuid => GridPlaneSurface._refObjectGuid;

    public GsaGridPointLoad() { }

    public int CaseId() {
      return GridPointLoad.Case;
    }

    public IGsaLoad Duplicate() {
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
      
      if (LoadCase != null) {
        dup.LoadCase = LoadCase;
      }

      return dup;
    }

    internal Point3d GetPoint(LengthUnit unit) {
      LengthUnit m = LengthUnit.Meter;
      return new Point3d(
              new Length(GridPointLoad.X, m).As(unit),
              new Length(GridPointLoad.Y, m).As(unit),
              new Length(GridPlaneSurface.Plane.OriginZ, m).As(unit));
    }

    public override string ToString() {
      return string.Join(" ", LoadType.ToString().Trim(), GridPointLoad.Name.Trim()).Trim().Replace("  ", " ");
    }
  }
}
