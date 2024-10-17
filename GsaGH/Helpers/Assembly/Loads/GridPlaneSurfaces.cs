using System;
using System.Collections.Generic;

using Grasshopper.Kernel;

using GsaAPI;

using GsaGH.Helpers.GH;
using GsaGH.Parameters;
using GsaGH.Parameters.Enums;

using OasysUnits;

using EntityType = GsaGH.Parameters.EntityType;

namespace GsaGH.Helpers.Assembly {
  internal partial class ModelAssembly {
    private GsaGuidDictionary<GridPlane> _gridPlanes;
    private GsaGuidDictionary<GridSurface> _gridSurfaces;

    private int AddAxis(GsaGridPlaneSurface gridplanesurface) {
      int axisId = gridplanesurface.AxisId;

      Axis axis = gridplanesurface.GetAxis(_unit);

      if (axisId > 0) {
        gridplanesurface.GridPlane.AxisProperty = axisId;
        if (_axes.ReadOnlyDictionary.ContainsKey(axisId)) {
          _axes.SetValue(axisId, axis);
        } else {
          axisId = _axes.AddValue(axis);
        }
      } else {
        if (string.IsNullOrEmpty(axis.Name)) {
          if (gridplanesurface.Plane.Equals(Rhino.Geometry.Plane.WorldXY)) {
            return 0;
          } else if (gridplanesurface.Plane.Equals(Rhino.Geometry.Plane.WorldYZ)) {
            return -11;
          } else if (gridplanesurface.Plane.Equals(Rhino.Geometry.Plane.WorldZX)) {
            return -12;
          }
        }
        axisId = TryGetExistingAxisId(axis);
      }

      if (_axes.ReadOnlyDictionary[axisId].Name == string.Empty) {
        _axes.ReadOnlyDictionary[axisId].Name = "Axis " + axisId;
      }

      return axisId;
    }

    private int AddGridPlane(GsaGridPlaneSurface grdPlnSrf) {
      if (grdPlnSrf.Elevation != "0") {
        var elevation = new Length();
        try {
          elevation = Length.Parse(grdPlnSrf.Elevation);
        } catch (Exception) {
          if (double.TryParse(grdPlnSrf.Elevation, out double elev)) {
            elevation = new Length(elev, _unit);
          }
        }

        grdPlnSrf.GridPlane.Elevation = elevation.Meters;
      }

      if (grdPlnSrf.StoreyToleranceAbove != "auto") {
        var tolerance = new Length();
        try {
          tolerance = Length.Parse(grdPlnSrf.StoreyToleranceAbove);
        } catch (Exception) {
          if (double.TryParse(grdPlnSrf.StoreyToleranceAbove, out double tol)) {
            tolerance = new Length(tol, _unit);
          }
        }

        grdPlnSrf.GridPlane.ToleranceAbove = tolerance.Meters;
      }

      if (grdPlnSrf.StoreyToleranceBelow != "auto") {
        var tolerance = new Length();
        try {
          tolerance = Length.Parse(grdPlnSrf.StoreyToleranceBelow);
        } catch (Exception) {
          if (double.TryParse(grdPlnSrf.StoreyToleranceBelow, out double tol)) {
            tolerance = new Length(tol, _unit);
          }
        }

        grdPlnSrf.GridPlane.ToleranceBelow = tolerance.Meters;
      }

      int gridPlaneId = grdPlnSrf.GridPlaneId;
      if (gridPlaneId > 0) {
        _gridPlanes.SetValue(gridPlaneId, grdPlnSrf.GridPlaneGuid, grdPlnSrf.GridPlane);
      } else {
        if (grdPlnSrf.GridPlaneGuid == new Guid()) {
          gridPlaneId = TryUseExistingGridPlane(grdPlnSrf);
        } else {
          gridPlaneId = _gridPlanes.AddValue(grdPlnSrf.GridPlaneGuid, grdPlnSrf.GridPlane);
        }
      }

      if (_gridPlanes.ReadOnlyDictionary[gridPlaneId].
        Name == string.Empty) {
        _gridPlanes.ReadOnlyDictionary[gridPlaneId].Name = "Grid plane " + gridPlaneId;
      }

      return gridPlaneId;
    }

    private int AddGridSurface(GsaGridPlaneSurface grdPlnSrf, GH_Component owner) {
      var tolerance = new Length();
      try {
        tolerance = Length.Parse(grdPlnSrf.Tolerance);
      } catch (Exception) {
        if (double.TryParse(grdPlnSrf.Tolerance, out double tol)) {
          tolerance = new Length(tol, _unit);
        }
      }

      grdPlnSrf.GridSurface.Tolerance = tolerance.Meters;

      if (grdPlnSrf._referenceType != ReferenceType.None) {
        if (grdPlnSrf._referenceType == ReferenceType.List) {
          if (grdPlnSrf._refList == null
            || grdPlnSrf._refList.EntityType != EntityType.Element
            || grdPlnSrf._refList.EntityType != EntityType.Member) {
            owner.AddRuntimeWarning("Invalid List type for GridSurface " + grdPlnSrf.ToString()
              + Environment.NewLine + "Element list has not been set");
          }
          grdPlnSrf.GridSurface.Elements += GetElementList(grdPlnSrf._refList, owner);
        } else {
          grdPlnSrf.GridSurface.Elements += GetReferenceElementIdsDefinition(grdPlnSrf);
        }
      }

      int gridSurfaceId = grdPlnSrf.GridSurfaceId;
      if (gridSurfaceId > 0) {
        _gridSurfaces.SetValue(grdPlnSrf.GridSurfaceId, grdPlnSrf.GridSurfaceGuid, grdPlnSrf.GridSurface);
      } else {
        if (grdPlnSrf.GridSurfaceGuid == new Guid()) {
          gridSurfaceId = TryUseExistingGridSurface(grdPlnSrf);
        } else {
          gridSurfaceId = _gridSurfaces.AddValue(grdPlnSrf.GridSurfaceGuid, grdPlnSrf.GridSurface);
        }
      }

      return gridSurfaceId;
    }

    private int ConvertGridPlaneSurface(GsaGridPlaneSurface gridPlaneSurface, GH_Component owner) {
      gridPlaneSurface.GridPlane.AxisProperty = AddAxis(gridPlaneSurface);
      gridPlaneSurface.GridSurface.GridPlane = AddGridPlane(gridPlaneSurface);
      return AddGridSurface(gridPlaneSurface, owner);
    }

    private void ConvertGridPlaneSurface(List<GsaGridPlaneSurface> gridPlaneSurfaces, GH_Component owner) {
      if (gridPlaneSurfaces == null) {
        return;
      }

      foreach (GsaGridPlaneSurface gridPlaneSurface in gridPlaneSurfaces) {
        if (gridPlaneSurface == null) {
          continue;
        }

        GsaGridPlaneSurface gps = gridPlaneSurface.Duplicate();

        if (gps.GridPlane != null) {
          gps.GridPlane.AxisProperty = AddAxis(gps);
          gps.GridSurface.GridPlane = AddGridPlane(gps);
        }

        AddGridSurface(gps, owner);
      }
    }

    private int TryUseExistingGridPlane(GsaGridPlaneSurface grdPlnSrf) {
      GridPlane newPlane = grdPlnSrf.GridPlane;
      foreach (KeyValuePair<int, GridPlane> kvp in
      _gridPlanes.ReadOnlyDictionary) {
        if (kvp.Key < 1) {
          continue;
        }

        if (kvp.Value.AxisProperty == newPlane.AxisProperty
          && kvp.Value.Elevation == newPlane.Elevation
          && kvp.Value.IsStoreyType == newPlane.IsStoreyType
          && kvp.Value.ToleranceAbove == newPlane.ToleranceAbove
          && kvp.Value.ToleranceBelow == newPlane.ToleranceBelow
          && (kvp.Value.Name == newPlane.Name || newPlane.Name == string.Empty)) {
          return kvp.Key;
        }
      }

      return _gridPlanes.AddValue(Guid.NewGuid(), grdPlnSrf.GridPlane);
    }

    private int TryUseExistingGridSurface(GsaGridPlaneSurface grdPlnSrf) {
      GridSurface newSrf = grdPlnSrf.GridSurface;
      foreach (KeyValuePair<int, GridSurface> kvp in
        _gridSurfaces.ReadOnlyDictionary) {
        if (kvp.Value.Direction == newSrf.Direction
          && kvp.Value.Elements == newSrf.Elements
          && kvp.Value.ElementType == newSrf.ElementType
          && kvp.Value.ExpansionType == newSrf.ExpansionType
          && kvp.Value.GridPlane == newSrf.GridPlane
          && kvp.Value.SpanType == newSrf.SpanType
          && kvp.Value.Tolerance == newSrf.Tolerance
          && newSrf.Name == string.Empty) {
          return kvp.Key;
        }
      }
      return _gridSurfaces.AddValue(Guid.NewGuid(), grdPlnSrf.GridSurface);
    }
  }
}
