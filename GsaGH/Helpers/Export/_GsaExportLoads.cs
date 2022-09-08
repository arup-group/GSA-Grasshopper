using System;
using System.Collections.Generic;
using System.Linq;
using GsaAPI;
using GsaGH.Parameters;
using UnitsNet.Units;

namespace GsaGH.Util.Gsa.ToGSA
{
  class Loads
  {
    /// <summary>
    /// Method to loop through a list of Loads with GridPlaneSurfaces and return the highest set ID + 1 for gridplan and gridsurface
    /// </summary>
    /// <param name="loads"></param>
    /// <param name="gridplaneidcounter"></param>
    /// <param name="gridsurfaceidcounter"></param>
    public static void GetGridPlaneSurfaceCounters(List<GsaLoad> loads, ref int gridplaneidcounter, ref int gridsurfaceidcounter)
    {
      for (int i = 0; i < loads.Count; i++)
      {
        if (loads[i] != null)
        {
          GsaLoad load = loads[i];

          switch (load.LoadType)
          {
            case GsaLoad.LoadTypes.GridArea:
              if (load.AreaLoad.GridPlaneSurface.GridPlaneID > 0)
                gridplaneidcounter = Math.Max(gridplaneidcounter, load.AreaLoad.GridPlaneSurface.GridPlaneID + 1);
              if (load.AreaLoad.GridPlaneSurface.GridSurfaceID > 0)
                gridsurfaceidcounter = Math.Max(gridsurfaceidcounter, load.AreaLoad.GridPlaneSurface.GridSurfaceID + 1);
              break;

            case GsaLoad.LoadTypes.GridLine:
              if (load.LineLoad.GridPlaneSurface.GridPlaneID > 0)
                gridplaneidcounter = Math.Max(gridplaneidcounter, load.LineLoad.GridPlaneSurface.GridPlaneID + 1);
              if (load.LineLoad.GridPlaneSurface.GridSurfaceID > 0)
                gridsurfaceidcounter = Math.Max(gridsurfaceidcounter, load.LineLoad.GridPlaneSurface.GridSurfaceID + 1);
              break;

            case GsaLoad.LoadTypes.GridPoint:
              if (load.PointLoad.GridPlaneSurface.GridPlaneID > 0)
                gridplaneidcounter = Math.Max(gridplaneidcounter, load.PointLoad.GridPlaneSurface.GridPlaneID + 1);
              if (load.PointLoad.GridPlaneSurface.GridSurfaceID > 0)
                gridsurfaceidcounter = Math.Max(gridsurfaceidcounter, load.PointLoad.GridPlaneSurface.GridSurfaceID + 1);
              break;
          }
        }
      }
    }
    /// <summary>
    /// Method to convert a list of loads into dictionaries containing GsaAPI objects
    /// </summary>
    /// <param name="loads"></param>
    /// <param name="gravityLoads"></param>
    /// <param name="nodeLoads_node"></param>
    /// <param name="nodeLoads_displ"></param>
    /// <param name="nodeLoads_settle"></param>
    /// <param name="beamLoads"></param>
    /// <param name="faceLoads"></param>
    /// <param name="gridPointLoads"></param>
    /// <param name="gridLineLoads"></param>
    /// <param name="gridAreaLoads"></param>
    /// <param name="existingAxes"></param>
    /// <param name="existingGridPlanes"></param>
    /// <param name="existingGridSurfaces"></param>
    /// <param name="gp_guid"></param>
    /// <param name="gs_guid"></param>
    /// <param name="workerInstance"></param>
    /// <param name="ReportProgress"></param>
    public static void ConvertLoad(List<GsaLoad> loads,
        ref List<GravityLoad> gravityLoads,
        ref List<NodeLoad> nodeLoads_node, ref List<NodeLoad> nodeLoads_displ, ref List<NodeLoad> nodeLoads_settle,
        ref List<BeamLoad> beamLoads, ref List<FaceLoad> faceLoads,
        ref List<GridPointLoad> gridPointLoads, ref List<GridLineLoad> gridLineLoads, ref List<GridAreaLoad> gridAreaLoads,
        ref Dictionary<int, Axis> existingAxes,
        ref Dictionary<int, GridPlane> existingGridPlanes,
        ref Dictionary<int, GridSurface> existingGridSurfaces,
        ref Dictionary<Guid, int> gp_guid, ref Dictionary<Guid, int> gs_guid, LengthUnit unit)
    {
      if (loads != null)
      {
        // create a counter for creating new axes, gridplanes and gridsurfaces
        int axisidcounter = (existingAxes.Count > 0) ? existingAxes.Keys.Max() + 1 : 1;
        int gridplaneidcounter = (existingGridPlanes.Count > 0) ? existingGridPlanes.Keys.Max() + 1 : 1;
        int gridsurfaceidcounter = (existingGridSurfaces.Count > 0) ? existingGridSurfaces.Keys.Max() + 1 : 1;

        // get the highest gridplaneID+1 and gridsurfaceID+1
        GetGridPlaneSurfaceCounters(loads, ref gridplaneidcounter, ref gridsurfaceidcounter);

        for (int i = 0; i < loads.Count; i++)
        {
          if (loads[i] != null)
          {
            GsaLoad load = loads[i];
            ConvertLoad(load, ref gravityLoads, ref nodeLoads_node, ref nodeLoads_displ, ref nodeLoads_settle,
                ref beamLoads, ref faceLoads, ref gridPointLoads, ref gridLineLoads, ref gridAreaLoads,
                    ref existingAxes, ref axisidcounter, ref existingGridPlanes, ref gridplaneidcounter,
                        ref existingGridSurfaces, ref gridsurfaceidcounter, ref gp_guid, ref gs_guid, unit);
          }
        }
      }
    }
    /// <summary>
    /// Method to convert a single of loads into dictionaries containing GsaAPI objects. Maintains the referenced dictionaries and counters.
    /// </summary>
    /// <param name="load"></param>
    /// <param name="gravityLoads"></param>
    /// <param name="nodeLoads_node"></param>
    /// <param name="nodeLoads_displ"></param>
    /// <param name="nodeLoads_settle"></param>
    /// <param name="beamLoads"></param>
    /// <param name="faceLoads"></param>
    /// <param name="gridPointLoads"></param>
    /// <param name="gridLineLoads"></param>
    /// <param name="gridAreaLoads"></param>
    /// <param name="existingAxes"></param>
    /// <param name="axisidcounter"></param>
    /// <param name="existingGridPlanes"></param>
    /// <param name="gridplaneidcounter"></param>
    /// <param name="existingGridSurfaces"></param>
    /// <param name="gridsurfaceidcounter"></param>
    /// <param name="gp_guid"></param>
    /// <param name="gs_guid"></param>
    public static void ConvertLoad(GsaLoad load,
        ref List<GravityLoad> gravityLoads,
    ref List<NodeLoad> nodeLoads_node, ref List<NodeLoad> nodeLoads_displ, ref List<NodeLoad> nodeLoads_settle,
    ref List<BeamLoad> beamLoads, ref List<FaceLoad> faceLoads,
    ref List<GridPointLoad> gridPointLoads, ref List<GridLineLoad> gridLineLoads, ref List<GridAreaLoad> gridAreaLoads,
    ref Dictionary<int, Axis> existingAxes, ref int axisidcounter,
    ref Dictionary<int, GridPlane> existingGridPlanes, ref int gridplaneidcounter,
    ref Dictionary<int, GridSurface> existingGridSurfaces, ref int gridsurfaceidcounter,
    ref Dictionary<Guid, int> gp_guid, ref Dictionary<Guid, int> gs_guid, LengthUnit unit)
    {

      switch (load.LoadType)
      {
        case GsaLoad.LoadTypes.Gravity:
          gravityLoads.Add(load.GravityLoad.GravityLoad);
          break;

        case GsaLoad.LoadTypes.Node:
          if (load.NodeLoad.NodeLoadType == GsaNodeLoad.NodeLoadTypes.APPLIED_DISP)
            nodeLoads_displ.Add(load.NodeLoad.NodeLoad);
          if (load.NodeLoad.NodeLoadType == GsaNodeLoad.NodeLoadTypes.NODE_LOAD)
            nodeLoads_node.Add(load.NodeLoad.NodeLoad);
          if (load.NodeLoad.NodeLoadType == GsaNodeLoad.NodeLoadTypes.SETTLEMENT)
            nodeLoads_settle.Add(load.NodeLoad.NodeLoad);
          break;

        case GsaLoad.LoadTypes.Beam:
          beamLoads.Add(load.BeamLoad.BeamLoad);
          break;

        case GsaLoad.LoadTypes.Face:
          faceLoads.Add(load.FaceLoad.FaceLoad);
          break;

        case GsaLoad.LoadTypes.GridPoint:
          if (load.PointLoad.GridPlaneSurface == null) // if gridsurface id has been set
          {
            // add the load to our list of loads to be set later
            gridPointLoads.Add(load.PointLoad.GridPointLoad);
            break;
          }

          // set grid point load and grid plane surface
          GsaGridPointLoad gridptref = load.PointLoad;
          GsaGridPlaneSurface gridplnsrf = gridptref.GridPlaneSurface;

          if (gridplnsrf.GridPlane != null)
          {
            // - grid load references a grid surface number
            // -- grid surface references a grid plane number
            // --- grid plane references an Axis number
            // toggle through the members in reverse order, set/add to model in each step

            // ### AXIS ###
            // set axis property in grid plane, add/set axis in model
            gridplnsrf.GridPlane.AxisProperty = SetAxis(ref gridplnsrf, ref existingAxes, ref axisidcounter, unit);

            // ### GRID PLANE ###
            // set grid plane number in grid surface, add/set grid plane in model
            gridplnsrf.GridSurface.GridPlane = SetGridPlane(ref gridplnsrf, ref existingGridPlanes, ref gridplaneidcounter, ref gp_guid, existingAxes);

            // ### GRID SURFACE ###
            // set the surface number in the load, add/set the surface in the model
            gridptref.GridPointLoad.GridSurface = SetGridSurface(ref gridplnsrf, ref existingGridSurfaces, ref gridsurfaceidcounter, ref gs_guid, existingGridPlanes, existingAxes);
          }

          // add the load to our list of loads to be set later
          gridPointLoads.Add(gridptref.GridPointLoad);
          break;

        case GsaLoad.LoadTypes.GridLine:
          if (load.LineLoad.GridPlaneSurface == null) // if gridsurface id has been set
          {
            // add the load to our list of loads to be set later
            gridLineLoads.Add(load.LineLoad.GridLineLoad);
            break;
          }

          // set grid line load and grid plane surface
          GsaGridLineLoad gridlnref = load.LineLoad;
          gridplnsrf = gridlnref.GridPlaneSurface;

          if (gridplnsrf.GridPlane != null)
          {
            // - grid load references a grid surface number
            // -- grid surface references a grid plane number
            // --- grid plane references an Axis number
            // toggle through the members in reverse order, set/add to model in each step

            // ### AXIS ###
            // set axis property in grid plane, add/set axis in model
            gridplnsrf.GridPlane.AxisProperty = SetAxis(ref gridplnsrf, ref existingAxes, ref axisidcounter, unit);

            // ### GRID PLANE ###
            // set grid plane number in grid surface, add/set grid plane in model
            gridplnsrf.GridSurface.GridPlane = SetGridPlane(ref gridplnsrf, ref existingGridPlanes, ref gridplaneidcounter, ref gp_guid, existingAxes);

            // ### GRID SURFACE ###
            // set the surface number in the load, add/set the surface in the model
            gridlnref.GridLineLoad.GridSurface = SetGridSurface(ref gridplnsrf, ref existingGridSurfaces, ref gridsurfaceidcounter, ref gs_guid, existingGridPlanes, existingAxes);
          }
          // add the load to our list of loads to be set later
          gridLineLoads.Add(gridlnref.GridLineLoad);
          break;

        case GsaLoad.LoadTypes.GridArea:
          if (load.AreaLoad.GridPlaneSurface == null) // if gridsurface id has been set
          {
            // add the load to our list of loads to be set later
            gridAreaLoads.Add(load.AreaLoad.GridAreaLoad);
            break;
          }

          // set grid line load and grid plane surface
          GsaGridAreaLoad gridarref = load.AreaLoad;
          gridplnsrf = gridarref.GridPlaneSurface;

          if (gridplnsrf.GridPlane != null)
          {
            // - grid load references a grid surface number
            // -- grid surface references a grid plane number
            // --- grid plane references an Axis number
            // toggle through the members in reverse order, set/add to model in each step

            // ### AXIS ###
            // set axis property in grid plane, add/set axis in model
            gridplnsrf.GridPlane.AxisProperty = SetAxis(ref gridplnsrf, ref existingAxes, ref axisidcounter, unit);

            // ### GRID PLANE ###
            // set grid plane number in grid surface, add/set grid plane in model
            gridplnsrf.GridSurface.GridPlane = SetGridPlane(ref gridplnsrf, ref existingGridPlanes, ref gridplaneidcounter, ref gp_guid, existingAxes);

            // ### GRID SURFACE ###
            // set the surface number in the load, add/set the surface in the model
            gridarref.GridAreaLoad.GridSurface = SetGridSurface(ref gridplnsrf, ref existingGridSurfaces, ref gridsurfaceidcounter, ref gs_guid, existingGridPlanes, existingAxes);
          }
          // add the load to our list of loads to be set later
          gridAreaLoads.Add(gridarref.GridAreaLoad);
          break;
      }

    }
    /// <summary>
    /// Method to set a single axis in ref dictionary, looking up existing axes to reference. Returns axis ID set in dictionary or existing axis ID if similar already exist.
    /// </summary>
    /// <param name="gridplanesurface"></param>
    /// <param name="existingAxes"></param>
    /// <param name="axisidcounter"></param>
    /// <returns></returns>
    public static int SetAxis(ref GsaGridPlaneSurface gridplanesurface,
        ref Dictionary<int, Axis> existingAxes, ref int axisidcounter, LengthUnit unit)
    {
      int axis_id = gridplanesurface.AxisID;
      Axis axis = gridplanesurface.Axis;

      if (axis.Name == "")
        axis.Name = "Axis " + axisidcounter;

      // see if AXIS has been set
      if (gridplanesurface.AxisID > 0)
      {
        // assign the axis property to the grid plane (in the load)
        gridplanesurface.GridPlane.AxisProperty = axis_id;
        // set the axis in existing dictionary
        if (existingAxes.ContainsKey(axis_id))
          existingAxes[axis_id] = axis;
        else
          existingAxes.Add(axis_id, axis);
      }
      else
      {
        // check if there's already an axis with same properties in the model:
        axis_id = Axes.GetExistingAxisID(existingAxes, axis);
        if (axis_id > 0)
          gridplanesurface.GridPlane.AxisProperty = axis_id; // set the id if axis exist
        else
        {
          // else add the axis to the model and assign the new axis number to the grid plane
          axis_id = axisidcounter;
          gridplanesurface.GridPlane.AxisProperty = axisidcounter;

          existingAxes.Add(gridplanesurface.GridPlane.AxisProperty, axis);
          axisidcounter++;
        }
      }
      return axis_id;
    }
    /// <summary>
    /// Method to set a single gridplane in ref dictionary, looking up existing axes to reference. Returns the GridPlane ID set in dictionary or existing ID if similar already exist. Maintains the axis dictionary
    /// </summary>
    /// <param name="gridplanesurface"></param>
    /// <param name="existingGridPlanes"></param>
    /// <param name="gridplaneidcounter"></param>
    /// <param name="gp_guid"></param>
    /// <param name="existingAxes"></param>
    /// <returns></returns>
    public static int SetGridPlane(ref GsaGridPlaneSurface gridplanesurface,
        ref Dictionary<int, GridPlane> existingGridPlanes, ref int gridplaneidcounter, ref Dictionary<Guid, int> gp_guid, Dictionary<int, Axis> existingAxes)
    {
      if (existingGridPlanes.Count > 0)
        gridplaneidcounter = Math.Max(existingGridPlanes.Keys.Max() + 1, gridplaneidcounter);

      if (gridplanesurface.GridPlane.Name == "")
        gridplanesurface.GridPlane.Name = "Grid plane " + gridplaneidcounter;

      int gp_ID = gridplanesurface.GridPlaneID;
      // see if grid plane ID has been set by user
      if (gridplanesurface.GridPlaneID > 0)
      {
        // assign the grid plane number set by user in the load's grid surface
        gp_ID = gridplanesurface.GridPlaneID;
        // set grid plane in model
        existingGridPlanes[gp_ID] = gridplanesurface.GridPlane;
      }
      else
      {
        // check first if guid is 0000-0000 indicating that we want to try use existing axis
        if (gridplanesurface.GridPlaneGUID == new Guid())
        {
          // check if there's already an axis with same properties in the model:
          int axis_id = Axes.GetExistingAxisID(existingAxes, gridplanesurface.Axis);

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
    public static int SetGridSurface(ref GsaGridPlaneSurface gridplanesurface,
        ref Dictionary<int, GridSurface> existingGridSurfaces, ref int gridsurfaceidcounter, ref Dictionary<Guid, int> gs_guid,
        Dictionary<int, GridPlane> existingGridPlanes, Dictionary<int, Axis> existingAxes)
    {
      if (existingGridSurfaces.Count > 0)
        gridsurfaceidcounter = Math.Max(existingGridSurfaces.Keys.Max() + 1, gridsurfaceidcounter);

      int gs_ID = gridplanesurface.GridSurfaceID;

      if (gridplanesurface.GridSurface.Name == "")
        gridplanesurface.GridSurface.Name = "Grid surface " + gridsurfaceidcounter;

      // see if grid surface ID has been set by user
      if (gridplanesurface.GridSurfaceID > 0)
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
          int axis_id = Axes.GetExistingAxisID(existingAxes, gridplanesurface.Axis);

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
    /// Method to loop through a list of GridPlaneSurfaces and return the highest set ID + 1 for gridplan and gridsurface
    /// </summary>
    /// <param name="gridPlaneSurfaces"></param>
    /// <param name="gridplaneidcounter"></param>
    /// <param name="gridsurfaceidcounter"></param>
    public static void GetGridPlaneSurfaceCounters(List<GsaGridPlaneSurface> gridPlaneSurfaces, ref int gridplaneidcounter, ref int gridsurfaceidcounter)
    {
      for (int i = 0; i < gridPlaneSurfaces.Count; i++)
      {
        if (gridPlaneSurfaces[i] != null)
        {
          GsaGridPlaneSurface gps = gridPlaneSurfaces[i];

          if (gps.GridPlaneID > 0)
            gridplaneidcounter = Math.Max(gridplaneidcounter, gps.GridPlaneID + 1);
          if (gps.GridSurfaceID > 0)
            gridsurfaceidcounter = Math.Max(gridsurfaceidcounter, gps.GridSurfaceID + 1);
        }
      }
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
    /// <param name="workerInstance"></param>
    /// <param name="ReportProgress"></param>
    public static void ConvertGridPlaneSurface(List<GsaGridPlaneSurface> gridPlaneSurfaces,
        ref Dictionary<int, Axis> existingAxes, ref Dictionary<int, GridPlane> existingGridPlanes,
        ref Dictionary<int, GridSurface> existingGridSurfaces,
        ref Dictionary<Guid, int> gp_guid, ref Dictionary<Guid, int> gs_guid, LengthUnit unit)
    {
      if (gridPlaneSurfaces != null)
      {
        // create a counter for creating new axes, gridplanes and gridsurfaces
        int axisidcounter = (existingAxes.Count > 0) ? existingAxes.Keys.Max() + 1 : 1;
        int gridplaneidcounter = (existingGridPlanes.Count > 0) ? existingGridPlanes.Keys.Max() + 1 : 1;
        int gridsurfaceidcounter = (existingGridSurfaces.Count > 0) ? existingGridSurfaces.Keys.Max() + 1 : 1;

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
              gps.GridSurface.GridPlane = SetGridPlane(ref gps, ref existingGridPlanes, ref gridplaneidcounter, ref gp_guid, existingAxes);
            }
            // add / set Grid Surface
            SetGridSurface(ref gps, ref existingGridSurfaces, ref gridsurfaceidcounter, ref gs_guid, existingGridPlanes, existingAxes);
          }
        }
      }
    }
  }
}
