using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Grasshopper.Kernel;
using GsaAPI;
using GsaGH.Parameters;
using OasysUnits;
using OasysUnits.Units;

namespace GsaGH.Helpers.Export
{
  internal partial class Loads
  {
    /// <summary>
    /// Method to set a single axis in ref dictionary, looking up existing axes to reference. Returns axis ID set in dictionary or existing axis ID if similar already exist.
    /// </summary>
    /// <param name="gridplanesurface"></param>
    /// <param name="existingAxes"></param>
    /// <param name="axisidcounter"></param>
    /// <returns></returns>
    internal static int SetAxis(ref GsaGridPlaneSurface gridplanesurface, ref Dictionary<int, Axis> existingAxes, ref int axisidcounter, LengthUnit modelUnit)
    {
      int axisId = gridplanesurface.AxisId;

      Axis axis = gridplanesurface.GetAxis(modelUnit);

      if (axis.Name == "")
        axis.Name = "Axis " + axisidcounter;

      // see if AXIS has been set
      if (gridplanesurface.AxisId > 0)
      {
        // assign the axis property to the grid plane (in the load)
        gridplanesurface.GridPlane.AxisProperty = axisId;
        // set the axis in existing dictionary
        if (existingAxes.ContainsKey(axisId))

          // why???
          existingAxes[axisId] = axis;
        else
          existingAxes.Add(axisId, axis);
      }
      else
      {
        // check if there's already an axis with same properties in the model:
        axisId = Axes.GetExistingAxisID(existingAxes, axis);
        if (axisId > 0)
          gridplanesurface.GridPlane.AxisProperty = axisId; // set the id if axis exist
        else
        {
          // else add the axis to the model and assign the new axis number to the grid plane
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
    internal static void GetGridPlaneSurfaceCounters(List<GsaGridPlaneSurface> gridPlaneSurfaces, ref int gridplaneidcounter, ref int gridsurfaceidcounter)
    {
      for (int i = 0; i < gridPlaneSurfaces.Count; i++)
      {
        GsaGridPlaneSurface gps = gridPlaneSurfaces[i];

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
    internal static void GetGridPlaneSurfaceCounters(List<GsaLoad> loads, ref int gridplaneidcounter, ref int gridsurfaceidcounter)
    {
      for (int i = 0; i < loads.Count; i++)
      {
        if (loads[i] != null)
        {
          GsaLoad load = loads[i];

          switch (load.LoadType)
          {
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
    /// <param name="gp_guid"></param>
    /// <param name="existingAxes"></param>
    /// <returns></returns>
    internal static int SetGridPlane(ref GsaGridPlaneSurface gridplanesurface,
        ref Dictionary<int, GridPlane> existingGridPlanes, ref int gridplaneidcounter, ref Dictionary<Guid, int> gp_guid, Dictionary<int, Axis> existingAxes, LengthUnit modelUnit)
    {
      if (existingGridPlanes.Count > 0)
        gridplaneidcounter = Math.Max(existingGridPlanes.Keys.Max() + 1, gridplaneidcounter);

      if (gridplanesurface.GridPlane.Name == "")
        gridplanesurface.GridPlane.Name = "Grid plane " + gridplaneidcounter;

      // set API elevation converted to [m]
      if (gridplanesurface.Elevation != "0")
      {
        Length elevation = new Length();
        try
        {
          elevation = Length.Parse(gridplanesurface.Elevation);
        }
        catch (Exception)
        {
          if (double.TryParse(gridplanesurface.Elevation, out double elev))
            elevation = new Length(elev, modelUnit);
        }
        gridplanesurface.GridPlane.Elevation = elevation.Meters;
      }
      if (gridplanesurface.StoreyToleranceAbove != "auto")
      {
        Length tolerance = new Length();
        try
        {
          tolerance = Length.Parse(gridplanesurface.StoreyToleranceAbove);
        }
        catch (Exception)
        {
          if (double.TryParse(gridplanesurface.StoreyToleranceAbove, out double tol))
            tolerance = new Length(tol, modelUnit);
        }
        gridplanesurface.GridPlane.ToleranceAbove = tolerance.Meters;
      }
      if (gridplanesurface.StoreyToleranceBelow != "auto")
      {
        Length tolerance = new Length();
        try
        {
          tolerance = Length.Parse(gridplanesurface.StoreyToleranceBelow);
        }
        catch (Exception)
        {
          if (double.TryParse(gridplanesurface.StoreyToleranceBelow, out double tol))
            tolerance = new Length(tol, modelUnit);
        }
        gridplanesurface.GridPlane.ToleranceBelow = tolerance.Meters;
      }

      int gp_ID = gridplanesurface.GridPlaneId;
      // see if grid plane Id has been set by user
      if (gridplanesurface.GridPlaneId > 0)
      {
        // assign the grid plane number set by user in the load's grid surface
        gp_ID = gridplanesurface.GridPlaneId;
        // set grid plane in model
        existingGridPlanes[gp_ID] = gridplanesurface.GridPlane;
      }
      else
      {
        // check first if guid is 0000-0000 indicating that we want to try use existing axis
        if (gridplanesurface.GridPlaneGUID == new Guid())
        {
          // check if there's already an axis with same properties in the model:
          int axis_id = Axes.GetExistingAxisID(existingAxes, gridplanesurface.GetAxis(modelUnit));

          if (axis_id > 0)
          {
            // if axis is found, the loop through existing gridplanes to find the first that is using this axis
            foreach (int key in existingGridPlanes.Keys)
            {
              if (existingGridPlanes[key].AxisProperty == axis_id)
              {
                if (existingGridPlanes[key].Elevation == gridplanesurface.GridPlane.Elevation &
                    existingGridPlanes[key].ToleranceAbove == gridplanesurface.GridPlane.ToleranceAbove &
                    existingGridPlanes[key].ToleranceBelow == gridplanesurface.GridPlane.ToleranceBelow)
                  return key;
              }
            }
          }
        }
        else if (gp_guid.ContainsKey(gridplanesurface.GridPlaneGUID)) // check if grid plane has already been added to model by other loads
        {
          gp_guid.TryGetValue(gridplanesurface.GridPlaneGUID, out int gpID);
          // if guid exist in our dictionary it has been added to the model 
          // and we just assign the value to the grid surface
          return gpID;
        }

        // if it does not exist we add the grid plane to the model
        existingGridPlanes.Add(gridplaneidcounter, gridplanesurface.GridPlane);
        // then set the id to grid surface
        gp_ID = gridplaneidcounter;
        // and add it to the our list of grid planes
        if (gridplanesurface.GridPlaneGUID != new Guid())
        {
          gp_guid.Add(gridplanesurface.GridPlaneGUID, gridplaneidcounter);
        }
        gridplaneidcounter++;
      }
      return gp_ID;
    }

    /// <summary>
    /// Method to set a single gridsurface in ref dictionary, looking up existing axes and gridplanes to reference. Returns the GridSurface ID set in dictionary or existing ID if similar already exist. Maintains the gridplane and axis dictionary
    /// </summary>
    /// <param name="gridplanesurface"></param>
    /// <param name="existingGridSurfaces"></param>
    /// <param name="gridsurfaceidcounter"></param>
    /// <param name="gs_guid"></param>
    /// <param name="existingGridPlanes"></param>
    /// <param name="existingAxes"></param>
    /// <returns></returns>
    internal static int SetGridSurface(ref GsaGridPlaneSurface gridplanesurface,
        ref Dictionary<int, GridSurface> existingGridSurfaces, ref int gridsurfaceidcounter, ref Dictionary<Guid, int> gs_guid,
        Dictionary<int, GridPlane> existingGridPlanes, Dictionary<int, Axis> existingAxes, LengthUnit modelUnit, ref ConcurrentDictionary<int, ConcurrentBag<int>> memberElementRelationship, Model model, GsaGuidDictionary<Section> apiSections, GsaGuidDictionary<Prop2D> apiProp2ds, GsaGuidDictionary<Prop3D> apiProp3ds, GsaGuidIntListDictionary<Element> apiElements, GsaGuidDictionary<Member> apiMembers)
    {
      if (existingGridSurfaces.Count > 0)
        gridsurfaceidcounter = Math.Max(existingGridSurfaces.Keys.Max() + 1, gridsurfaceidcounter);

      int gs_ID = gridplanesurface.GridSurfaceId;

      if (gridplanesurface.GridSurface.Name == "")
        gridplanesurface.GridSurface.Name = "Grid surface " + gridsurfaceidcounter;

      // set API tolerance converted to [m]
      Length tolerance = new Length();
      try
      {
        tolerance = Length.Parse(gridplanesurface.Tolerance);
      }
      catch (Exception)
      {
        if (double.TryParse(gridplanesurface.Tolerance, out double tol))
          tolerance = new Length(tol, modelUnit);
      }
      gridplanesurface.GridSurface.Tolerance = tolerance.Meters;

      // add referenced loads
      if (model != null && gridplanesurface.ReferenceType != ReferenceType.None)
      {
        if (memberElementRelationship == null)
          memberElementRelationship = ElementListFromReference.GetMemberElementRelationship(model);
        gridplanesurface.GridSurface.Elements += ElementListFromReference.GetRefElementIds(gridplanesurface, apiSections, apiProp2ds, apiProp3ds, apiElements, apiMembers, memberElementRelationship);
      }

      // see if grid surface ID has been set by user
      if (gridplanesurface.GridSurfaceId > 0)
      {
        // set the grid surface in model
        existingGridSurfaces[gs_ID] = gridplanesurface.GridSurface;
      }
      else
      {
        // check first if guid is 0000-0000 indicating that we want to try use existing axis
        if (gridplanesurface.GridSurfaceGUID == new Guid())
        {
          // check if there's already an axis with same properties in the model:
          int axis_id = Axes.GetExistingAxisID(existingAxes, gridplanesurface.GetAxis(modelUnit));

          if (axis_id > 0)
          {
            // if axis is found, the loop through existing gridplanes to find the first that is using this axis
            foreach (int keyPln in existingGridPlanes.Keys)
            {
              if (existingGridPlanes[keyPln].AxisProperty == axis_id)
              {
                // if grid plane is found loop through existing grid surfaces to 
                // find the first that is referencing this grid plane
                foreach (int keySrf in existingGridSurfaces.Keys)
                {
                  if (existingGridSurfaces[keySrf].GridPlane == keyPln)
                  {
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
          }
        }
        else if (gs_guid.ContainsKey(gridplanesurface.GridSurfaceGUID)) // check if grid surface has already been added to model by other loads
        {
          gs_guid.TryGetValue(gridplanesurface.GridSurfaceGUID, out int gsID);
          // if guid exist in our dictionary it has been added to the model 
          // and we just assign the value to the load
          //gs_ID = gsID;
          return gsID;
        }

        // if it does not exist we add the grid surface to the model
        existingGridSurfaces.Add(gridsurfaceidcounter, gridplanesurface.GridSurface);
        gs_ID = gridsurfaceidcounter;
        // and add it to the our list of grid surfaces
        if (gridplanesurface.GridSurfaceGUID != new Guid())
        {
          gs_guid.Add(gridplanesurface.GridSurfaceGUID, gs_ID);
        }
        gridsurfaceidcounter++;
      }
      return gs_ID;
    }



    /// <summary>
    /// Method to convert a list of GridPlaneSurfaces and set Axes, GridPlanes and GridSurfaces in ref Dictionaries
    /// </summary>
    /// <param name="gridPlaneSurfaces"></param>
    /// <param name="existingAxes"></param>
    /// <param name="existingGridPlanes"></param>
    /// <param name="existingGridSurfaces"></param>
    /// <param name="gp_guid"></param>
    /// <param name="gs_guid"></param>
    internal static void ConvertGridPlaneSurface(List<GsaGridPlaneSurface> gridPlaneSurfaces,
        ref Dictionary<int, Axis> existingAxes, ref Dictionary<int, GridPlane> existingGridPlanes,
        ref Dictionary<int, GridSurface> existingGridSurfaces,
        ref Dictionary<Guid, int> gp_guid, ref Dictionary<Guid, int> gs_guid, LengthUnit unit, ref ConcurrentDictionary<int, ConcurrentBag<int>> memberElementRelationship, Model model, GsaGuidDictionary<Section> apiSections, GsaGuidDictionary<Prop2D> apiProp2ds, GsaGuidDictionary<Prop3D> apiProp3ds, GsaGuidIntListDictionary<Element> apiElements, GsaGuidDictionary<Member> apiMembers)
    {
      if (gridPlaneSurfaces != null)
      {
        // create a counter for creating new axes, gridplanes and gridsurfaces
        int axisidcounter = existingAxes.Count > 0 ? existingAxes.Keys.Max() + 1 : 1;
        int gridplaneidcounter = existingGridPlanes.Count > 0 ? existingGridPlanes.Keys.Max() + 1 : 1;
        int gridsurfaceidcounter = existingGridSurfaces.Count > 0 ? existingGridSurfaces.Keys.Max() + 1 : 1;

        // get the highest gridplaneID+1 and gridsurfaceID+1
        GetGridPlaneSurfaceCounters(gridPlaneSurfaces, ref gridplaneidcounter, ref gridsurfaceidcounter);

        for (int i = 0; i < gridPlaneSurfaces.Count; i++)
        {
          if (gridPlaneSurfaces[i] != null)
          {
            GsaGridPlaneSurface gps = gridPlaneSurfaces[i];

            if (gps.GridPlane != null)
            {
              // add / set Axis and set the id in the grid plane
              gps.GridPlane.AxisProperty = SetAxis(ref gps, ref existingAxes, ref axisidcounter, unit);

              // add / set Grid Plane and set the id in the grid surface
              gps.GridSurface.GridPlane = SetGridPlane(ref gps, ref existingGridPlanes, ref gridplaneidcounter, ref gp_guid, existingAxes, unit);
            }
            // add / set Grid Surface
            SetGridSurface(ref gps, ref existingGridSurfaces, ref gridsurfaceidcounter, ref gs_guid, existingGridPlanes, existingAxes, unit, ref memberElementRelationship, model, apiSections, apiProp2ds, apiProp3ds, apiElements, apiMembers);
          }
        }
      }
    }
  }
}
