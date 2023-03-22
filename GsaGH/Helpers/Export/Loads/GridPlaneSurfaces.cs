using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using GsaAPI;
using GsaGH.Parameters;
using OasysUnits;
using OasysUnits.Units;

namespace GsaGH.Helpers.Export {
  internal partial class Loads {
    /// <summary>
    /// Method to set a single axis in ref dictionary, looking up existing axes to reference. Returns axis ID set in dictionary or existing axis ID if similar already exist.
    /// </summary>
    /// <param name="gridplanesurface"></param>
    /// <param name="existingAxes"></param>
    /// <param name="axisidcounter"></param>
    /// <param name="modelUnit"></param>
    /// <returns></returns>
    internal static int SetAxis(ref GsaGridPlaneSurface gridplanesurface, ref Dictionary<int, Axis> existingAxes, ref int axisidcounter, LengthUnit modelUnit) {
      int axisId = gridplanesurface.AxisId;

      Axis axis = gridplanesurface.GetAxis(modelUnit);

      if (axis.Name == "")
        axis.Name = "Axis " + axisidcounter;

      if (gridplanesurface.AxisId > 0) {
        gridplanesurface.GridPlane.AxisProperty = axisId;
        if (existingAxes.ContainsKey(axisId))
          existingAxes[axisId] = axis;
        else
          existingAxes.Add(axisId, axis);
      }
      else {
        axisId = Axes.GetExistingAxisId(existingAxes, axis);
        if (axisId > 0)
          gridplanesurface.GridPlane.AxisProperty = axisId; // set the id if axis exist
        else {
          axisId = axisidcounter;
          gridplanesurface.GridPlane.AxisProperty = axisidcounter;

          existingAxes.Add(gridplanesurface.GridPlane.AxisProperty, axis);
          axisidcounter++;
        }
      }
      return axisId;
    }

    /// <summary>
    /// Method to loop through a list of GridPlaneSurfaces and return the highest set Id + 1 for gridplane and gridsurface
    /// </summary>
    /// <param name="gridPlaneSurfaces"></param>
    /// <param name="gridplaneidcounter"></param>
    /// <param name="gridsurfaceidcounter"></param>
    internal static void GetGridPlaneSurfaceCounters(List<GsaGridPlaneSurface> gridPlaneSurfaces, ref int gridplaneidcounter, ref int gridsurfaceidcounter) {
      foreach (GsaGridPlaneSurface gps in gridPlaneSurfaces) {
        if (gps.GridPlaneId > 0)
          gridplaneidcounter = Math.Max(gridplaneidcounter, gps.GridPlaneId + 1);
        if (gps.GridSurfaceId > 0)
          gridsurfaceidcounter = Math.Max(gridsurfaceidcounter, gps.GridSurfaceId + 1);
      }
    }

    /// <summary>
    /// Method to loop through a list of Loads with GridPlaneSurfaces and return the highest set ID + 1 for gridplan and gridsurface
    /// </summary>
    /// <param name="loads"></param>
    /// <param name="gridplaneidcounter"></param>
    /// <param name="gridsurfaceidcounter"></param>
    internal static void GetGridPlaneSurfaceCounters(List<GsaLoad> loads, ref int gridplaneidcounter, ref int gridsurfaceidcounter) {
      foreach (GsaLoad gsaLoad in loads) {
        if (gsaLoad != null) {
          GsaLoad load = gsaLoad;

          switch (load.LoadType) {
            case GsaLoad.LoadTypes.GridArea:
              if (load.AreaLoad.GridPlaneSurface.GridPlaneId > 0)
                gridplaneidcounter = Math.Max(gridplaneidcounter, load.AreaLoad.GridPlaneSurface.GridPlaneId + 1);
              if (load.AreaLoad.GridPlaneSurface.GridSurfaceId > 0)
                gridsurfaceidcounter = Math.Max(gridsurfaceidcounter, load.AreaLoad.GridPlaneSurface.GridSurfaceId + 1);
              break;

            case GsaLoad.LoadTypes.GridLine:
              if (load.LineLoad.GridPlaneSurface.GridPlaneId > 0)
                gridplaneidcounter = Math.Max(gridplaneidcounter, load.LineLoad.GridPlaneSurface.GridPlaneId + 1);
              if (load.LineLoad.GridPlaneSurface.GridSurfaceId > 0)
                gridsurfaceidcounter = Math.Max(gridsurfaceidcounter, load.LineLoad.GridPlaneSurface.GridSurfaceId + 1);
              break;

            case GsaLoad.LoadTypes.GridPoint:
              if (load.PointLoad.GridPlaneSurface.GridPlaneId > 0)
                gridplaneidcounter = Math.Max(gridplaneidcounter, load.PointLoad.GridPlaneSurface.GridPlaneId + 1);
              if (load.PointLoad.GridPlaneSurface.GridSurfaceId > 0)
                gridsurfaceidcounter = Math.Max(gridsurfaceidcounter, load.PointLoad.GridPlaneSurface.GridSurfaceId + 1);
              break;
          }
        }
      }
    }

    /// <summary>
    /// Method to set a single gridplane in ref dictionary, looking up existing axes to reference. Returns the GridPlane Id set in dictionary or existing ID if similar already exist. Maintains the axis dictionary
    /// </summary>
    /// <param name="gridplanesurface"></param>
    /// <param name="existingGridPlanes"></param>
    /// <param name="gridplaneidcounter"></param>
    /// <param name="gpGuid"></param>
    /// <param name="existingAxes"></param>
    /// <param name="modelUnit"></param>
    /// <returns></returns>
    internal static int SetGridPlane(ref GsaGridPlaneSurface gridplanesurface,
        ref Dictionary<int, GridPlane> existingGridPlanes, ref int gridplaneidcounter, ref Dictionary<Guid, int> gpGuid, Dictionary<int, Axis> existingAxes, LengthUnit modelUnit) {
      if (existingGridPlanes.Count > 0)
        gridplaneidcounter = Math.Max(existingGridPlanes.Keys.Max() + 1, gridplaneidcounter);

      if (gridplanesurface.GridPlane.Name == "")
        gridplanesurface.GridPlane.Name = "Grid plane " + gridplaneidcounter;

      if (gridplanesurface.Elevation != "0") {
        var elevation = new Length();
        try {
          elevation = Length.Parse(gridplanesurface.Elevation);
        }
        catch (Exception) {
          if (double.TryParse(gridplanesurface.Elevation, out double elev))
            elevation = new Length(elev, modelUnit);
        }
        gridplanesurface.GridPlane.Elevation = elevation.Meters;
      }
      if (gridplanesurface.StoreyToleranceAbove != "auto") {
        var tolerance = new Length();
        try {
          tolerance = Length.Parse(gridplanesurface.StoreyToleranceAbove);
        }
        catch (Exception) {
          if (double.TryParse(gridplanesurface.StoreyToleranceAbove, out double tol))
            tolerance = new Length(tol, modelUnit);
        }
        gridplanesurface.GridPlane.ToleranceAbove = tolerance.Meters;
      }
      if (gridplanesurface.StoreyToleranceBelow != "auto") {
        var tolerance = new Length();
        try {
          tolerance = Length.Parse(gridplanesurface.StoreyToleranceBelow);
        }
        catch (Exception) {
          if (double.TryParse(gridplanesurface.StoreyToleranceBelow, out double tol))
            tolerance = new Length(tol, modelUnit);
        }
        gridplanesurface.GridPlane.ToleranceBelow = tolerance.Meters;
      }

      int gpId;
      if (gridplanesurface.GridPlaneId > 0) {
        gpId = gridplanesurface.GridPlaneId;
        existingGridPlanes[gpId] = gridplanesurface.GridPlane;
      }
      else {
        if (gridplanesurface.GridPlaneGUID == new Guid()) {
          int axisId = Axes.GetExistingAxisId(existingAxes, gridplanesurface.GetAxis(modelUnit));

          if (axisId > 0) {
            foreach (int key in existingGridPlanes.Keys) {
              if (existingGridPlanes[key].AxisProperty != axisId) {
                continue;
              }

              if (existingGridPlanes[key].Elevation == gridplanesurface.GridPlane.Elevation &
                  existingGridPlanes[key].ToleranceAbove == gridplanesurface.GridPlane.ToleranceAbove &
                  existingGridPlanes[key].ToleranceBelow == gridplanesurface.GridPlane.ToleranceBelow)
                return key;
            }
          }
        }
        else if (gpGuid.ContainsKey(gridplanesurface.GridPlaneGUID)) {
          gpGuid.TryGetValue(gridplanesurface.GridPlaneGUID, out int id);
          return id;
        }

        existingGridPlanes.Add(gridplaneidcounter, gridplanesurface.GridPlane);
        gpId = gridplaneidcounter;
        if (gridplanesurface.GridPlaneGUID != new Guid()) {
          gpGuid.Add(gridplanesurface.GridPlaneGUID, gridplaneidcounter);
        }
        gridplaneidcounter++;
      }
      return gpId;
    }

    /// <summary>
    /// Method to set a single gridsurface in ref dictionary, looking up existing axes and gridplanes to reference. Returns the GridSurface ID set in dictionary or existing ID if similar already exist. Maintains the gridplane and axis dictionary
    /// </summary>
    /// <param name="gridplanesurface"></param>
    /// <param name="existingGridSurfaces"></param>
    /// <param name="gridsurfaceidcounter"></param>
    /// <param name="gsGuid"></param>
    /// <param name="existingGridPlanes"></param>
    /// <param name="existingAxes"></param>
    /// <param name="modelUnit"></param>
    /// <param name="memberElementRelationship"></param>
    /// <param name="model"></param>
    /// <param name="apiSections"></param>
    /// <param name="apiProp2ds"></param>
    /// <param name="apiProp3ds"></param>
    /// <param name="apiElements"></param>
    /// <param name="apiMembers"></param>
    /// <returns></returns>
    internal static int SetGridSurface(ref GsaGridPlaneSurface gridplanesurface,
        ref Dictionary<int, GridSurface> existingGridSurfaces, ref int gridsurfaceidcounter, ref Dictionary<Guid, int> gsGuid,
        Dictionary<int, GridPlane> existingGridPlanes, Dictionary<int, Axis> existingAxes, LengthUnit modelUnit, ref ConcurrentDictionary<int, ConcurrentBag<int>> memberElementRelationship, Model model, GsaGuidDictionary<Section> apiSections, GsaGuidDictionary<Prop2D> apiProp2ds, GsaGuidDictionary<Prop3D> apiProp3ds, GsaGuidIntListDictionary<Element> apiElements, GsaGuidDictionary<Member> apiMembers) {
      if (existingGridSurfaces.Count > 0)
        gridsurfaceidcounter = Math.Max(existingGridSurfaces.Keys.Max() + 1, gridsurfaceidcounter);

      int gsId = gridplanesurface.GridSurfaceId;

      if (gridplanesurface.GridSurface.Name == "")
        gridplanesurface.GridSurface.Name = "Grid surface " + gridsurfaceidcounter;

      var tolerance = new Length();
      try {
        tolerance = Length.Parse(gridplanesurface.Tolerance);
      }
      catch (Exception) {
        if (double.TryParse(gridplanesurface.Tolerance, out double tol))
          tolerance = new Length(tol, modelUnit);
      }
      gridplanesurface.GridSurface.Tolerance = tolerance.Meters;

      if (model != null && gridplanesurface.ReferenceType != ReferenceType.None) {
        if (memberElementRelationship == null)
          memberElementRelationship = ElementListFromReference.GetMemberElementRelationship(model);
        gridplanesurface.GridSurface.Elements += ElementListFromReference.GetRefElementIds(gridplanesurface, apiSections, apiProp2ds, apiProp3ds, apiElements, apiMembers, memberElementRelationship);
      }

      if (gridplanesurface.GridSurfaceId > 0) {
        existingGridSurfaces[gsId] = gridplanesurface.GridSurface;
      }
      else {
        if (gridplanesurface.GridSurfaceGUID == new Guid()) {
          int axisId = Axes.GetExistingAxisId(existingAxes, gridplanesurface.GetAxis(modelUnit));

          if (axisId > 0) {
            foreach (int keyPln in existingGridPlanes.Keys) {
              if (existingGridPlanes[keyPln].AxisProperty != axisId) {
                continue;
              }

              foreach (int keySrf in existingGridSurfaces.Keys) {
                if (existingGridSurfaces[keySrf].GridPlane != keyPln) {
                  continue;
                }

                if (existingGridSurfaces[keySrf].Direction == gridplanesurface.GridSurface.Direction &
                    existingGridSurfaces[keySrf].Elements == gridplanesurface.GridSurface.Elements &
                    existingGridSurfaces[keySrf].ElementType == gridplanesurface.GridSurface.ElementType &
                    existingGridSurfaces[keySrf].ExpansionType == gridplanesurface.GridSurface.ExpansionType &
                    existingGridSurfaces[keySrf].SpanType == gridplanesurface.GridSurface.SpanType &
                    existingGridSurfaces[keySrf].Tolerance == gridplanesurface.GridSurface.Tolerance)
                  return keySrf;
              }
            }
          }
        }
        else if (gsGuid.ContainsKey(gridplanesurface.GridSurfaceGUID)) {
          gsGuid.TryGetValue(gridplanesurface.GridSurfaceGUID, out int id);
          return id;
        }

        existingGridSurfaces.Add(gridsurfaceidcounter, gridplanesurface.GridSurface);
        gsId = gridsurfaceidcounter;
        if (gridplanesurface.GridSurfaceGUID != new Guid()) {
          gsGuid.Add(gridplanesurface.GridSurfaceGUID, gsId);
        }
        gridsurfaceidcounter++;
      }
      return gsId;
    }

    /// <summary>
    /// Method to convert a list of GridPlaneSurfaces and set Axes, GridPlanes and GridSurfaces in ref Dictionaries
    /// </summary>
    /// <param name="gridPlaneSurfaces"></param>
    /// <param name="existingAxes"></param>
    /// <param name="existingGridPlanes"></param>
    /// <param name="existingGridSurfaces"></param>
    /// <param name="gpGuid"></param>
    /// <param name="gsGuid"></param>
    /// <param name="unit"></param>
    /// <param name="memberElementRelationship"></param>
    /// <param name="model"></param>
    /// <param name="apiSections"></param>
    /// <param name="apiProp2ds"></param>
    /// <param name="apiProp3ds"></param>
    /// <param name="apiElements"></param>
    /// <param name="apiMembers"></param>
    internal static void ConvertGridPlaneSurface(List<GsaGridPlaneSurface> gridPlaneSurfaces,
        ref Dictionary<int, Axis> existingAxes, ref Dictionary<int, GridPlane> existingGridPlanes,
        ref Dictionary<int, GridSurface> existingGridSurfaces,
        ref Dictionary<Guid, int> gpGuid, ref Dictionary<Guid, int> gsGuid, LengthUnit unit, ref ConcurrentDictionary<int, ConcurrentBag<int>> memberElementRelationship, Model model, GsaGuidDictionary<Section> apiSections, GsaGuidDictionary<Prop2D> apiProp2ds, GsaGuidDictionary<Prop3D> apiProp3ds, GsaGuidIntListDictionary<Element> apiElements, GsaGuidDictionary<Member> apiMembers) {
      if (gridPlaneSurfaces == null) {
        return;
      }

      int axisidcounter = existingAxes.Count > 0 ? existingAxes.Keys.Max() + 1 : 1;
      int gridplaneidcounter = existingGridPlanes.Count > 0 ? existingGridPlanes.Keys.Max() + 1 : 1;
      int gridsurfaceidcounter = existingGridSurfaces.Count > 0 ? existingGridSurfaces.Keys.Max() + 1 : 1;

      GetGridPlaneSurfaceCounters(gridPlaneSurfaces, ref gridplaneidcounter, ref gridsurfaceidcounter);

      foreach (GsaGridPlaneSurface gridPlaneSurface in gridPlaneSurfaces) {
        if (gridPlaneSurface == null) {
          continue;
        }
        GsaGridPlaneSurface gps = gridPlaneSurface;

        if (gps.GridPlane != null) {
          gps.GridPlane.AxisProperty = SetAxis(ref gps, ref existingAxes, ref axisidcounter, unit);
          gps.GridSurface.GridPlane = SetGridPlane(ref gps, ref existingGridPlanes, ref gridplaneidcounter, ref gpGuid, existingAxes, unit);
        }
        SetGridSurface(ref gps, ref existingGridSurfaces, ref gridsurfaceidcounter, ref gsGuid, existingGridPlanes, existingAxes, unit, ref memberElementRelationship, model, apiSections, apiProp2ds, apiProp3ds, apiElements, apiMembers);
      }
    }
  }
}
