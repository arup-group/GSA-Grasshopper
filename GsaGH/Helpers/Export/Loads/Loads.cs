using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Grasshopper.Kernel;
using GsaAPI;
using GsaGH.Parameters;
using OasysUnits;
using OasysUnits.Units;
using Rhino.Geometry;

namespace GsaGH.Helpers.Export
{
  internal partial class Loads
  {
    internal static void ConvertLoad(List<GsaLoad> loads,
        ref List<GravityLoad> gravityLoads, ref List<BeamLoad> beamLoads, ref List<FaceLoad> faceLoads,
        ref List<GridPointLoad> gridPointLoads, ref List<GridLineLoad> gridLineLoads, ref List<GridAreaLoad> gridAreaLoads, ref Dictionary<int, Axis> existingAxes, ref Dictionary<int, GridPlane> existingGridPlanes,
        ref Dictionary<int, GridSurface> existingGridSurfaces, ref Dictionary<Guid, int> gp_guid, ref Dictionary<Guid, int> gs_guid, LengthUnit unit, ref ConcurrentDictionary<int, ConcurrentBag<int>> memberElementRelationship, Model model, GsaGuidDictionary<Section> apiSections, GsaGuidDictionary<Prop2D> apiProp2ds, GsaGuidDictionary<Prop3D> apiProp3ds, GsaGuidIntListDictionary<Element> apiElements, GsaGuidDictionary<Member> apiMembers, GH_Component owner)
    {
      if (loads != null)
      {
        // create a counter for creating new axes, gridplanes and gridsurfaces
        int axisidcounter = existingAxes.Count > 0 ? existingAxes.Keys.Max() + 1 : 1;
        int gridplaneidcounter = existingGridPlanes.Count > 0 ? existingGridPlanes.Keys.Max() + 1 : 1;
        int gridsurfaceidcounter = existingGridSurfaces.Count > 0 ? existingGridSurfaces.Keys.Max() + 1 : 1;

        // get the highest gridplaneID+1 and gridsurfaceID+1
        GetGridPlaneSurfaceCounters(loads, ref gridplaneidcounter, ref gridsurfaceidcounter);

        for (int i = 0; i < loads.Count; i++)
        {
          if (loads[i] != null)
          {
            GsaLoad load = loads[i];
            ConvertLoad(load, ref gravityLoads, ref beamLoads, ref faceLoads, ref gridPointLoads, ref gridLineLoads, ref gridAreaLoads, ref existingAxes, ref axisidcounter, ref existingGridPlanes, ref gridplaneidcounter, ref existingGridSurfaces, ref gridsurfaceidcounter, ref gp_guid, ref gs_guid, unit, ref memberElementRelationship, model, apiSections, apiProp2ds, apiProp3ds, apiElements, apiMembers, owner);
          }
        }
      }
    }

    internal static void ConvertLoad(GsaLoad load,
        ref List<GravityLoad> gravityLoads, ref List<BeamLoad> beamLoads, ref List<FaceLoad> faceLoads, ref List<GridPointLoad> gridPointLoads, ref List<GridLineLoad> gridLineLoads, ref List<GridAreaLoad> gridAreaLoads,
    ref Dictionary<int, Axis> existingAxes, ref int axisidcounter, ref Dictionary<int, GridPlane> existingGridPlanes, ref int gridplaneidcounter, ref Dictionary<int, GridSurface> existingGridSurfaces, ref int gridsurfaceidcounter,
    ref Dictionary<Guid, int> gp_guid, ref Dictionary<Guid, int> gs_guid, LengthUnit unit, ref ConcurrentDictionary<int, ConcurrentBag<int>> memberElementRelationship, Model model, GsaGuidDictionary<Section> apiSections, GsaGuidDictionary<Prop2D> apiProp2ds, GsaGuidDictionary<Prop3D> apiProp3ds, GsaGuidIntListDictionary<Element> apiElements, GsaGuidDictionary<Member> apiMembers, GH_Component owner)
    {
      switch (load.LoadType)
      {
        case GsaLoad.LoadTypes.Gravity:
          PostHog.Load(load.LoadType, load.GravityLoad.ReferenceType);
          if (load.GravityLoad.ReferenceType != ReferenceType.None)
          {
            if (memberElementRelationship == null)
              memberElementRelationship = ElementListFromReference.GetMemberElementRelationship(model);
            string objectElemList = ElementListFromReference.GetRefElementIds(load.GravityLoad, apiSections, apiProp2ds, apiProp3ds, apiElements, apiMembers, memberElementRelationship);
            if (objectElemList.Trim() != "")
              load.GravityLoad.GravityLoad.Elements = objectElemList;
            else
            {
              string warning = "One or more GravityLoads with reference to a " + load.GravityLoad.ReferenceType + " could not be added to the model. Ensure the reference " + load.GravityLoad.ReferenceType + " has been added to the model.";
              if (!owner.RuntimeMessages(GH_RuntimeMessageLevel.Warning).Contains(warning))
                owner.AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, warning);
              break;
            }
          }
          gravityLoads.Add(load.GravityLoad.GravityLoad);
          break;

        case GsaLoad.LoadTypes.Beam:
          PostHog.Load(load.LoadType, load.BeamLoad.ReferenceType);
          if (load.BeamLoad.ReferenceType != ReferenceType.None)
          {
            if (memberElementRelationship == null)
              memberElementRelationship = ElementListFromReference.GetMemberElementRelationship(model);
            string objectElemList = ElementListFromReference.GetRefElementIds(load.BeamLoad, apiSections, apiElements, apiMembers, memberElementRelationship);
            if (objectElemList.Trim() != "")
              load.BeamLoad.BeamLoad.Elements = objectElemList;
            else
            {
              string warning = "One or more BeamLoads with reference to a " + load.BeamLoad.ReferenceType + " could not be added to the model. Ensure the reference " + load.BeamLoad.ReferenceType + " has been added to the model.";
              if (!owner.RuntimeMessages(GH_RuntimeMessageLevel.Warning).Contains(warning))
                owner.AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, warning);
              break;
            }
          }
          beamLoads.Add(load.BeamLoad.BeamLoad);
          break;

        case GsaLoad.LoadTypes.Face:
          PostHog.Load(load.LoadType, load.FaceLoad.ReferenceType);
          if (load.FaceLoad.ReferenceType != ReferenceType.None)
          {
            if (memberElementRelationship == null)
              memberElementRelationship = ElementListFromReference.GetMemberElementRelationship(model);
            string objectElemList = ElementListFromReference.GetRefElementIds(load.FaceLoad, apiProp2ds, apiElements, apiMembers, memberElementRelationship);
            if (objectElemList.Trim() != "")
              load.FaceLoad.FaceLoad.Elements = objectElemList;
            else
            {
              string warning = "One or more FaceLoads with reference to a " + load.FaceLoad.ReferenceType + " could not be added to the model. Ensure the reference " + load.FaceLoad.ReferenceType + " has been added to the model.";
              if (!owner.RuntimeMessages(GH_RuntimeMessageLevel.Warning).Contains(warning))
                owner.AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, warning);
              break;
            }
          }
          faceLoads.Add(load.FaceLoad.FaceLoad);
          break;

        case GsaLoad.LoadTypes.GridPoint:
          PostHog.Load(load.LoadType, ReferenceType.None);
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
            gridplnsrf.GridSurface.GridPlane = SetGridPlane(ref gridplnsrf, ref existingGridPlanes, ref gridplaneidcounter, ref gp_guid, existingAxes, unit);

            // ### GRID SURFACE ###
            // set the surface number in the load, add/set the surface in the model
            gridptref.GridPointLoad.GridSurface = SetGridSurface(ref gridplnsrf, ref existingGridSurfaces, ref gridsurfaceidcounter, ref gs_guid, existingGridPlanes, existingAxes, unit, ref memberElementRelationship, model, apiSections, apiProp2ds, apiProp3ds, apiElements, apiMembers);
          }

          // add the load to our list of loads to be set later
          gridPointLoads.Add(gridptref.GridPointLoad);
          break;

        case GsaLoad.LoadTypes.GridLine:
          PostHog.Load(load.LoadType, ReferenceType.None);
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
            gridplnsrf.GridSurface.GridPlane = SetGridPlane(ref gridplnsrf, ref existingGridPlanes, ref gridplaneidcounter, ref gp_guid, existingAxes, unit);

            // ### GRID SURFACE ###
            // set the surface number in the load, add/set the surface in the model
            gridlnref.GridLineLoad.GridSurface = SetGridSurface(ref gridplnsrf, ref existingGridSurfaces, ref gridsurfaceidcounter, ref gs_guid, existingGridPlanes, existingAxes, unit, ref memberElementRelationship, model, apiSections, apiProp2ds, apiProp3ds, apiElements, apiMembers);
          }
          // add the load to our list of loads to be set later
          gridLineLoads.Add(gridlnref.GridLineLoad);
          break;

        case GsaLoad.LoadTypes.GridArea:
          PostHog.Load(load.LoadType, ReferenceType.None, load.AreaLoad.GridAreaLoad.Type.ToString());
          // update polygon definition with unit
          if (load.AreaLoad.GridAreaLoad.Type == GridAreaPolyLineType.POLYGON)
            load.AreaLoad.GridAreaLoad.PolyLineDefinition += "(" + Length.GetAbbreviation(unit) + ")";

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
            gridplnsrf.GridSurface.GridPlane = SetGridPlane(ref gridplnsrf, ref existingGridPlanes, ref gridplaneidcounter, ref gp_guid, existingAxes, unit);

            // ### GRID SURFACE ###
            // set the surface number in the load, add/set the surface in the model
            gridarref.GridAreaLoad.GridSurface = SetGridSurface(ref gridplnsrf, ref existingGridSurfaces, ref gridsurfaceidcounter, ref gs_guid, existingGridPlanes, existingAxes, unit, ref memberElementRelationship, model, apiSections, apiProp2ds, apiProp3ds, apiElements, apiMembers);
          }
          // add the load to our list of loads to be set later
          gridAreaLoads.Add(gridarref.GridAreaLoad);
          break;
      }
    }

    internal static void ConvertNodeLoad(List<GsaLoad> loads,
        ref List<NodeLoad> nodeLoads_node, ref List<NodeLoad> nodeLoads_displ, ref List<NodeLoad> nodeLoads_settle, ref GsaIntDictionary<Node> apiNodes, LengthUnit unit)
    {
      if (loads != null)
        for (int i = 0; i < loads.Count; i++)
          if (loads[i] != null)
            if (loads[i].LoadType == GsaLoad.LoadTypes.Node)
              ConvertNodeLoad(loads[i], ref nodeLoads_node, ref nodeLoads_displ, ref nodeLoads_settle, ref apiNodes, unit);
    }

    internal static void ConvertNodeLoad(GsaLoad load, ref List<NodeLoad> nodeLoads_node, ref List<NodeLoad> nodeLoads_displ, ref List<NodeLoad> nodeLoads_settle, ref GsaIntDictionary<Node> apiNodes, LengthUnit unit)
    {
      if (load.NodeLoad.RefPoint != Point3d.Unset)
        load.NodeLoad.NodeLoad.Nodes = Nodes.AddNode(ref apiNodes, load.NodeLoad.RefPoint, unit).ToString();
      if (load.NodeLoad.Type == GsaNodeLoad.NodeLoadTypes.APPLIED_DISP)
        nodeLoads_displ.Add(load.NodeLoad.NodeLoad);
      if (load.NodeLoad.Type == GsaNodeLoad.NodeLoadTypes.NODE_LOAD)
        nodeLoads_node.Add(load.NodeLoad.NodeLoad);
      if (load.NodeLoad.Type == GsaNodeLoad.NodeLoadTypes.SETTLEMENT)
        nodeLoads_settle.Add(load.NodeLoad.NodeLoad);
      PostHog.Load(load.NodeLoad.RefPoint != Point3d.Unset, load.NodeLoad.Type.ToString());
    }
  }
}
