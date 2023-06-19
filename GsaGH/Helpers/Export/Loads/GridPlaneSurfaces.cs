using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Grasshopper.Kernel;
using GsaAPI;
using GsaAPI.Materials;
using GsaGH.Helpers.GH;
using GsaGH.Parameters;
using OasysUnits;
using LengthUnit = OasysUnits.Units.LengthUnit;

namespace GsaGH.Helpers.Export {
  internal class GridPlaneSurfaces {
    internal GsaGuidDictionary<GridPlane> GridPlanes;
    internal GsaGuidDictionary<GridSurface> GridSurfaces;
    internal ModelAssembly Model;

    internal GridPlaneSurfaces(ref ModelAssembly model) {
      GridPlanes = new GsaGuidDictionary<GridPlane>(model.Model.GridPlanes());
      GridSurfaces = new GsaGuidDictionary<GridSurface>(model.Model.GridSurfaces());
      Model = model;
    }

    internal void ConvertGridPlaneSurface(
      List<GsaGridPlaneSurface> gridPlaneSurfaces, GH_Component owner) {
      if (gridPlaneSurfaces == null) {
        return;
      }

      foreach (GsaGridPlaneSurface gridPlaneSurface in gridPlaneSurfaces) {
        if (gridPlaneSurface == null) {
          continue;
        }

        GsaGridPlaneSurface gps = gridPlaneSurface;

        if (gps.GridPlane != null) {
          gps.GridPlane.AxisProperty = AddAxis(gps);
          gps.GridSurface.GridPlane = AddGridPlane(gps);
        }

        AddGridSurface(gps, owner);
      }
    }

    internal int AddAxis(GsaGridPlaneSurface gridplanesurface) {
      int axisId = gridplanesurface.AxisId;

      Axis axis = gridplanesurface.GetAxis(Model.Unit);

      if (axisId > 0) {
        gridplanesurface.GridPlane.AxisProperty = axisId;
        if (Model.Axes.ReadOnlyDictionary.ContainsKey(axisId)) {
          Model.Axes.SetValue(axisId, axis);
        } else {
          axisId = Model.Axes.AddValue(axis);
        }
      } else {
        axisId = Export.Axes.TryGetExistingAxisId(ref Model.Axes, axis);
      }

      if (Model.Axes.ReadOnlyDictionary[axisId].Name == string.Empty) {
        Model.Axes.ReadOnlyDictionary[axisId].Name = "Axis " + axisId;
      }

      return axisId;
    }

    internal int AddGridPlane(GsaGridPlaneSurface grdPlnSrf) {
      if (grdPlnSrf.Elevation != "0") {
        var elevation = new Length();
        try {
          elevation = Length.Parse(grdPlnSrf.Elevation);
        } catch (Exception) {
          if (double.TryParse(grdPlnSrf.Elevation, out double elev)) {
            elevation = new Length(elev, Model.Unit);
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
            tolerance = new Length(tol, Model.Unit);
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
            tolerance = new Length(tol, Model.Unit);
          }
        }

        grdPlnSrf.GridPlane.ToleranceBelow = tolerance.Meters;
      }

      int gridPlaneId = grdPlnSrf.GridPlaneId;
      if (gridPlaneId > 0) {
        GridPlanes.SetValue(gridPlaneId, grdPlnSrf.GridPlaneGuid, grdPlnSrf.GridPlane);
      } else {
        if (grdPlnSrf.GridPlaneGuid == new Guid()) {
          gridPlaneId = TryUseExistingGridPlane(grdPlnSrf);
        } else {
          gridPlaneId = GridPlanes.AddValue(grdPlnSrf.GridPlaneGuid, grdPlnSrf.GridPlane);
        }
      }

      if (GridPlanes.ReadOnlyDictionary[gridPlaneId].Name == string.Empty) {
        GridPlanes.ReadOnlyDictionary[gridPlaneId].Name = "Grid plane " + gridPlaneId;
      }

      return gridPlaneId;
    }

    internal int AddGridSurface(GsaGridPlaneSurface grdPlnSrf, GH_Component owner) {
      var tolerance = new Length();
      try {
        tolerance = Length.Parse(grdPlnSrf.Tolerance);
      } catch (Exception) {
        if (double.TryParse(grdPlnSrf.Tolerance, out double tol)) {
          tolerance = new Length(tol, Model.Unit);
        }
      }

      grdPlnSrf.GridSurface.Tolerance = tolerance.Meters;

      if (grdPlnSrf._referenceType != ReferenceType.None) {
        if (grdPlnSrf._referenceType == ReferenceType.List) {
          if (grdPlnSrf._refList == null
            || grdPlnSrf._refList.EntityType != Parameters.EntityType.Element
            || grdPlnSrf._refList.EntityType != Parameters.EntityType.Member) {
            owner.AddRuntimeWarning("Invalid List type for GridSurface " + grdPlnSrf.ToString()
              + Environment.NewLine + "Element list has not been set");
          }
          grdPlnSrf.GridSurface.Elements +=
            Lists.GetElementList(
              grdPlnSrf._refList,
              ref Model.Lists,
              Model.Properties,
              Model.Elements,
              Model.Members,
              Model.MemberElementRelationship,
              owner);
        } else {
          grdPlnSrf.GridSurface.Elements +=
            ElementListFromReference.GetReferenceElementIdsDefinition(
              grdPlnSrf,
              Model.Properties,
              Model.Elements,
              Model.Members,
              Model.MemberElementRelationship);
        }
      }

      int gridSurfaceId = grdPlnSrf.GridSurfaceId;
      if (gridSurfaceId > 0) {
        GridSurfaces.SetValue(
          grdPlnSrf.GridSurfaceId, grdPlnSrf.GridSurfaceGuid, grdPlnSrf.GridSurface);
      } else {
        if (grdPlnSrf.GridSurfaceGuid == new Guid()) {
          gridSurfaceId = TryUseExistingGridSurface(grdPlnSrf);
        } else {
          gridSurfaceId = GridSurfaces.AddValue(grdPlnSrf.GridSurfaceGuid, grdPlnSrf.GridSurface);
        }
      }

      if (GridSurfaces.ReadOnlyDictionary[gridSurfaceId].Name == string.Empty) {
        GridSurfaces.ReadOnlyDictionary[gridSurfaceId].Name = "Grid surface " + gridSurfaceId;
      }

      return gridSurfaceId;
    }

    private int TryUseExistingGridPlane(GsaGridPlaneSurface grdPlnSrf) {
      GridPlane newPlane = grdPlnSrf.GridPlane;
      foreach (KeyValuePair<int, GridPlane> kvp in GridPlanes.ReadOnlyDictionary) {
        if (kvp.Value.AxisProperty == newPlane.AxisProperty
          && kvp.Value.Elevation == newPlane.Elevation
          && kvp.Value.IsStoreyType == newPlane.IsStoreyType
          && kvp.Value.ToleranceAbove == newPlane.ToleranceAbove
          && kvp.Value.ToleranceBelow == newPlane.ToleranceBelow
          && newPlane.Name == string.Empty) {
          return kvp.Key;
        }
      }
      return GridPlanes.AddValue(Guid.NewGuid(), grdPlnSrf.GridPlane);
    }

    private int TryUseExistingGridSurface(GsaGridPlaneSurface grdPlnSrf) {
      GridSurface newSrf = grdPlnSrf.GridSurface;
      foreach (KeyValuePair<int, GridSurface> kvp in GridSurfaces.ReadOnlyDictionary) {
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
      return GridSurfaces.AddValue(Guid.NewGuid(), grdPlnSrf.GridSurface);
    }
  }
}
