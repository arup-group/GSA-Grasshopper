using System;
using System.Collections.Generic;
using Grasshopper.Kernel;
using GsaAPI;
using GsaGH.Helpers.GH;
using GsaGH.Parameters;
using OasysUnits;
using EntityType = GsaGH.Parameters.EntityType;

namespace GsaGH.Helpers.Export {
  internal class GridPlaneSurfaces {
    internal GsaGuidDictionary<GridPlane> GridPlanes;
    internal GsaGuidDictionary<GridSurface> GridSurfaces;

    internal GridPlaneSurfaces(Model model) {
      GridPlanes = new GsaGuidDictionary<GridPlane>(model.GridPlanes());
      GridSurfaces = new GsaGuidDictionary<GridSurface>(model.GridSurfaces());
    }

    internal static int ConvertGridPlaneSurface(GsaGridPlaneSurface gridPlaneSurface, ref ModelAssembly model, GH_Component owner) {
      gridPlaneSurface.GridPlane.AxisProperty = AddAxis(gridPlaneSurface, ref model);
      gridPlaneSurface.GridSurface.GridPlane = AddGridPlane(gridPlaneSurface, ref model);
      return AddGridSurface(gridPlaneSurface, ref model, owner);
    }

    internal static void ConvertGridPlaneSurface(
      List<GsaGridPlaneSurface> gridPlaneSurfaces, ref ModelAssembly model, GH_Component owner) {
      if (gridPlaneSurfaces == null) {
        return;
      }

      foreach (GsaGridPlaneSurface gridPlaneSurface in gridPlaneSurfaces) {
        if (gridPlaneSurface == null) {
          continue;
        }

        GsaGridPlaneSurface gps = gridPlaneSurface;

        if (gps.GridPlane != null) {
          gps.GridPlane.AxisProperty = AddAxis(gps, ref model);
          gps.GridSurface.GridPlane = AddGridPlane(gps, ref model);
        }

        AddGridSurface(gps, ref model, owner);
      }
    }

    internal static int AddAxis(GsaGridPlaneSurface gridplanesurface, ref ModelAssembly model) {
      int axisId = gridplanesurface.AxisId;

      Axis axis = gridplanesurface.GetAxis(model.Unit);

      if (axisId > 0) {
        gridplanesurface.GridPlane.AxisProperty = axisId;
        if (model.Axes.ReadOnlyDictionary.ContainsKey(axisId)) {
          model.Axes.SetValue(axisId, axis);
        } else {
          axisId = model.Axes.AddValue(axis);
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
        axisId = Axes.TryGetExistingAxisId(ref model.Axes, axis);
      }

      if (model.Axes.ReadOnlyDictionary[axisId].Name == string.Empty) {
        model.Axes.ReadOnlyDictionary[axisId].Name = "Axis " + axisId;
      }

      return axisId;
    }

    internal static int AddGridPlane(GsaGridPlaneSurface grdPlnSrf, ref ModelAssembly model) {
      if (grdPlnSrf.Elevation != "0") {
        var elevation = new Length();
        try {
          elevation = Length.Parse(grdPlnSrf.Elevation);
        } catch (Exception) {
          if (double.TryParse(grdPlnSrf.Elevation, out double elev)) {
            elevation = new Length(elev, model.Unit);
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
            tolerance = new Length(tol, model.Unit);
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
            tolerance = new Length(tol, model.Unit);
          }
        }

        grdPlnSrf.GridPlane.ToleranceBelow = tolerance.Meters;
      }

      int gridPlaneId = grdPlnSrf.GridPlaneId;
      if (gridPlaneId > 0) {
        model.Loads.GridPlaneSurfaces.GridPlanes.SetValue(
          gridPlaneId, grdPlnSrf.GridPlaneGuid, grdPlnSrf.GridPlane);
      } else {
        if (grdPlnSrf.GridPlaneGuid == new Guid()) {
          gridPlaneId = TryUseExistingGridPlane(grdPlnSrf, ref model);
        } else {
          gridPlaneId = model.Loads.GridPlaneSurfaces.GridPlanes.AddValue(
            grdPlnSrf.GridPlaneGuid, grdPlnSrf.GridPlane);
        }
      }

      if (model.Loads.GridPlaneSurfaces.GridPlanes.ReadOnlyDictionary[gridPlaneId].
        Name == string.Empty) {
        model.Loads.GridPlaneSurfaces.GridPlanes.ReadOnlyDictionary[gridPlaneId].
          Name = "Grid plane " + gridPlaneId;
      }

      return gridPlaneId;
    }

    internal static int AddGridSurface(
      GsaGridPlaneSurface grdPlnSrf, ref ModelAssembly model, GH_Component owner) {
      var tolerance = new Length();
      try {
        tolerance = Length.Parse(grdPlnSrf.Tolerance);
      } catch (Exception) {
        if (double.TryParse(grdPlnSrf.Tolerance, out double tol)) {
          tolerance = new Length(tol, model.Unit);
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
          grdPlnSrf.GridSurface.Elements +=
            Lists.GetElementList(grdPlnSrf._refList, ref model, owner);
        } else {
          grdPlnSrf.GridSurface.Elements +=
            ElementListFromReference.GetReferenceElementIdsDefinition(grdPlnSrf, model);
        }
      }

      int gridSurfaceId = grdPlnSrf.GridSurfaceId;
      if (gridSurfaceId > 0) {
        model.Loads.GridPlaneSurfaces.GridSurfaces.SetValue(
          grdPlnSrf.GridSurfaceId, grdPlnSrf.GridSurfaceGuid, grdPlnSrf.GridSurface);
      } else {
        if (grdPlnSrf.GridSurfaceGuid == new Guid()) {
          gridSurfaceId = TryUseExistingGridSurface(grdPlnSrf, ref model);
        } else {
          gridSurfaceId = model.Loads.GridPlaneSurfaces.GridSurfaces.AddValue(
            grdPlnSrf.GridSurfaceGuid, grdPlnSrf.GridSurface);
        }
      }

      return gridSurfaceId;
    }

    private static int TryUseExistingGridPlane(
      GsaGridPlaneSurface grdPlnSrf, ref ModelAssembly model) {
      GridPlane newPlane = grdPlnSrf.GridPlane;
      foreach (KeyValuePair<int, GridPlane> kvp in
        model.Loads.GridPlaneSurfaces.GridPlanes.ReadOnlyDictionary) {
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

      return model.Loads.GridPlaneSurfaces.GridPlanes.AddValue(
        Guid.NewGuid(), grdPlnSrf.GridPlane);
    }
    private static int TryUseExistingGridSurface(
      GsaGridPlaneSurface grdPlnSrf, ref ModelAssembly model) {
      GridSurface newSrf = grdPlnSrf.GridSurface;
      foreach (KeyValuePair<int, GridSurface> kvp in
        model.Loads.GridPlaneSurfaces.GridSurfaces.ReadOnlyDictionary) {
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
      return model.Loads.GridPlaneSurfaces.GridSurfaces.AddValue(
        Guid.NewGuid(), grdPlnSrf.GridSurface);
    }
  }
}
