using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using GsaAPI;
using Rhino.Geometry;
using GhSA.Parameters;
using System.Threading;

namespace GhSA.Util.Gsa.ToGSA
{
    class Loads
    {
        public static void ConvertLoad(List<GsaLoad> loads,
            ref List<GravityLoad> gravityLoads,
            ref List<NodeLoad> nodeLoads_node, ref List<NodeLoad> nodeLoads_displ, ref List<NodeLoad> nodeLoads_settle,
            ref List<BeamLoad> beamLoads, ref List<FaceLoad> faceLoads,
            ref List<GridPointLoad> gridPointLoads, ref List<GridLineLoad> gridLineLoads, ref List<GridAreaLoad> gridAreaLoads,
            ref Dictionary<int, Axis> existingAxes, 
            ref Dictionary<int, GridPlane> existingGridPlanes,
            ref Dictionary<int, GridSurface> existingGridSurfaces,
            ref Dictionary<Guid, int> gp_guid, ref Dictionary<Guid, int> gs_guid,
            GrasshopperAsyncComponent.WorkerInstance workerInstance = null,
            Action<string, double> ReportProgress = null)
        {
            // create a counter for creating new axes, gridplanes and gridsurfaces
            int axisidcounter = (existingAxes.Count > 0) ? existingAxes.Keys.Max() + 1 : 1;
            int gridplaneidcounter = (existingGridPlanes.Count > 0) ? existingGridPlanes.Keys.Max() + 1 : 1;
            int gridsurfaceidcounter = (existingGridSurfaces.Count > 0) ? existingGridSurfaces.Keys.Max() + 1 : 1;

            if (loads != null)
            {
                for (int i = 0; i < loads.Count; i++)
                {
                    if (workerInstance != null)
                    {
                        if (workerInstance.CancellationToken.IsCancellationRequested) return;
                        ReportProgress("Loads ", (double)i / (loads.Count - 1));
                    }

                    if (loads[i] != null)
                    {
                        GsaLoad load = loads[i];
                        ConvertLoad(load, ref gravityLoads, ref nodeLoads_node, ref nodeLoads_displ, ref nodeLoads_settle,
                            ref beamLoads, ref faceLoads, ref gridPointLoads, ref gridLineLoads, ref gridAreaLoads,
                                ref existingAxes, ref axisidcounter, ref existingGridPlanes, ref gridplaneidcounter,
                                    ref existingGridSurfaces, ref gridsurfaceidcounter, ref gp_guid, ref gs_guid);
                    }
                }
            }
            if (workerInstance != null)
            {
                ReportProgress("Loads assembled", -2);
            }
        }
        public static void ConvertLoad(GsaLoad load,
            ref List<GravityLoad> gravityLoads,
        ref List<NodeLoad> nodeLoads_node, ref List<NodeLoad> nodeLoads_displ, ref List<NodeLoad> nodeLoads_settle,
        ref List<BeamLoad> beamLoads, ref List<FaceLoad> faceLoads,
        ref List<GridPointLoad> gridPointLoads, ref List<GridLineLoad> gridLineLoads, ref List<GridAreaLoad> gridAreaLoads,
        ref Dictionary<int, Axis> existingAxes, ref int axisidcounter,
        ref Dictionary<int, GridPlane> existingGridPlanes, ref int gridplaneidcounter,
        ref Dictionary<int, GridSurface> existingGridSurfaces, ref int gridsurfaceidcounter,
        ref Dictionary<Guid, int> gp_guid, ref Dictionary<Guid, int> gs_guid)
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
                        gridplnsrf.GridPlane.AxisProperty = SetAxis(ref gridplnsrf, ref existingAxes, ref axisidcounter);

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
                        gridplnsrf.GridPlane.AxisProperty = SetAxis(ref gridplnsrf, ref existingAxes, ref axisidcounter);

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
                        gridplnsrf.GridPlane.AxisProperty = SetAxis(ref gridplnsrf, ref existingAxes, ref axisidcounter);

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

        public static int SetAxis(ref GsaGridPlaneSurface gridplanesurface,
            ref Dictionary<int, Axis> existingAxes, ref int axisidcounter)
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

        public static int SetGridPlane(ref GsaGridPlaneSurface gridplanesurface,
            ref Dictionary<int, GridPlane> existingGridPlanes, ref int gridplaneidcounter, ref Dictionary<Guid, int> gp_guid, Dictionary<int, Axis> existingAxes)
        {
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
                                return key;
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
                    gp_guid.Add(gridplanesurface.GridPlaneGUID, gridplaneidcounter);
                gridplaneidcounter++;
            }
            return gp_ID;
        }

        public static int SetGridSurface(ref GsaGridPlaneSurface gridplanesurface,
            ref Dictionary<int, GridSurface> existingGridSurfaces, ref int gridsurfaceidcounter, ref Dictionary<Guid, int> gs_guid, 
            Dictionary<int, GridPlane> existingGridPlanes, Dictionary<int, Axis> existingAxes)
        {
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
                    gs_ID = gsID;
                }

                // if it does not exist we add the grid surface to the model
                existingGridSurfaces.Add(gridsurfaceidcounter, gridplanesurface.GridSurface);
                gs_ID = gridsurfaceidcounter;
                // and add it to the our list of grid surfaces
                if (gridplanesurface.GridSurfaceGUID != new Guid())
                    gs_guid.Add(gridplanesurface.GridSurfaceGUID, gs_ID);
                gridsurfaceidcounter++;
            }
            return gs_ID;
        }

        public static void ConvertGridPlaneSurface(List<GsaGridPlaneSurface> gridPlaneSurfaces,
            ref Dictionary<int, Axis> existingAxes, ref Dictionary<int, GridPlane> existingGridPlanes,
            ref Dictionary<int, GridSurface> existingGridSurfaces, 
            ref Dictionary<Guid, int> gp_guid, ref Dictionary<Guid, int> gs_guid, 
            GrasshopperAsyncComponent.WorkerInstance workerInstance = null,
            Action<string, double> ReportProgress = null)
        {
            // create a counter for creating new axes, gridplanes and gridsurfaces
            int axisidcounter = (existingAxes.Count > 0) ? existingAxes.Keys.Max() + 1 : 1;
            int gridplaneidcounter = (existingGridPlanes.Count > 0) ? existingGridPlanes.Keys.Max() + 1 : 1;
            int gridsurfaceidcounter = (existingGridSurfaces.Count > 0) ? existingGridSurfaces.Keys.Max() + 1 : 1;

            if (gridPlaneSurfaces != null)
            {
                for (int i = 0; i < gridPlaneSurfaces.Count; i++)
                {
                    if (gridPlaneSurfaces[i] != null)
                    {
                        GsaGridPlaneSurface gps = gridPlaneSurfaces[i];

                        if (gps.GridPlane != null)
                        {
                            // add / set Axis and set the id in the grid plane
                            gps.GridPlane.AxisProperty = SetAxis(ref gps, ref existingAxes, ref axisidcounter);

                            // add / set Grid Plane and set the id in the grid surface
                            gps.GridSurface.GridPlane = SetGridPlane(ref gps, ref existingGridPlanes, ref gridplaneidcounter, ref gp_guid, existingAxes);
                        }
                        // add / set Grid Surface
                        SetGridSurface(ref gps, ref existingGridSurfaces, ref gridsurfaceidcounter, ref gs_guid, existingGridPlanes, existingAxes);
                    }
                }
            }
            if (workerInstance != null)
            {
                ReportProgress("GridPlaneSurfaces assembled", -2);
            }
        }
    }
}
