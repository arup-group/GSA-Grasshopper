using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Grasshopper.Kernel;
using GsaAPI;
using GsaGH.Helpers.GH;
using GsaGH.Parameters;
using OasysUnits;
using OasysUnits.Units;
using Rhino.Geometry;

namespace GsaGH.Helpers.Export {
  internal partial class Loads {
    internal static void ConvertLoad(List<GsaLoad> loads,
        ref List<GravityLoad> gravityLoads, ref List<BeamLoad> beamLoads, ref List<FaceLoad> faceLoads,
        ref List<GridPointLoad> gridPointLoads, ref List<GridLineLoad> gridLineLoads, ref List<GridAreaLoad> gridAreaLoads, ref Dictionary<int, Axis> existingAxes, ref Dictionary<int, GridPlane> existingGridPlanes,
        ref Dictionary<int, GridSurface> existingGridSurfaces, ref Dictionary<Guid, int> gpGuid, ref Dictionary<Guid, int> gsGuid, LengthUnit unit, ref ConcurrentDictionary<int, ConcurrentBag<int>> memberElementRelationship, Model model, GsaGuidDictionary<Section> apiSections, GsaGuidDictionary<Prop2D> apiProp2ds, GsaGuidDictionary<Prop3D> apiProp3ds, GsaGuidIntListDictionary<Element> apiElements, GsaGuidDictionary<Member> apiMembers, GH_Component owner) {
      if (loads == null) {
        return;
      }

      int axisidcounter = existingAxes.Count > 0 ? existingAxes.Keys.Max() + 1 : 1;
      int gridplaneidcounter = existingGridPlanes.Count > 0 ? existingGridPlanes.Keys.Max() + 1 : 1;
      int gridsurfaceidcounter = existingGridSurfaces.Count > 0 ? existingGridSurfaces.Keys.Max() + 1 : 1;

      GetGridPlaneSurfaceCounters(loads, ref gridplaneidcounter, ref gridsurfaceidcounter);

      foreach (GsaLoad load in loads.Where(gsaLoad => gsaLoad != null))
      {
        ConvertLoad(load, ref gravityLoads, ref beamLoads, ref faceLoads, ref gridPointLoads, ref gridLineLoads, ref gridAreaLoads, ref existingAxes, ref axisidcounter, ref existingGridPlanes, ref gridplaneidcounter, ref existingGridSurfaces, ref gridsurfaceidcounter, ref gpGuid, ref gsGuid, unit, ref memberElementRelationship, model, apiSections, apiProp2ds, apiProp3ds, apiElements, apiMembers, owner);
      }
    }

    internal static void ConvertLoad(GsaLoad load,
        ref List<GravityLoad> gravityLoads, ref List<BeamLoad> beamLoads, ref List<FaceLoad> faceLoads, ref List<GridPointLoad> gridPointLoads, ref List<GridLineLoad> gridLineLoads, ref List<GridAreaLoad> gridAreaLoads,
    ref Dictionary<int, Axis> existingAxes, ref int axisidcounter, ref Dictionary<int, GridPlane> existingGridPlanes, ref int gridplaneidcounter, ref Dictionary<int, GridSurface> existingGridSurfaces, ref int gridsurfaceidcounter,
    ref Dictionary<Guid, int> gpGuid, ref Dictionary<Guid, int> gsGuid, LengthUnit unit, ref ConcurrentDictionary<int, ConcurrentBag<int>> memberElementRelationship, Model model, GsaGuidDictionary<Section> apiSections, GsaGuidDictionary<Prop2D> apiProp2ds, GsaGuidDictionary<Prop3D> apiProp3ds, GsaGuidIntListDictionary<Element> apiElements, GsaGuidDictionary<Member> apiMembers, GH_Component owner) {
      switch (load.LoadType) {
        case GsaLoad.LoadTypes.Gravity:
          PostHog.Load(load.LoadType, load.GravityLoad.ReferenceType);
          if (load.GravityLoad.ReferenceType != ReferenceType.None) {
            if (memberElementRelationship == null)
              memberElementRelationship = ElementListFromReference.GetMemberElementRelationship(model);
            string objectElemList = ElementListFromReference.GetRefElementIds(load.GravityLoad, apiSections, apiProp2ds, apiProp3ds, apiElements, apiMembers, memberElementRelationship);
            if (objectElemList.Trim() != "")
              load.GravityLoad.GravityLoad.Elements = objectElemList;
            else {
              string warning = "One or more GravityLoads with reference to a " + load.GravityLoad.ReferenceType + " could not be added to the model. Ensure the reference " + load.GravityLoad.ReferenceType + " has been added to the model.";
              if (!owner.RuntimeMessages(GH_RuntimeMessageLevel.Warning).Contains(warning))
                owner.AddRuntimeWarning(warning);
              break;
            }
          }
          gravityLoads.Add(load.GravityLoad.GravityLoad);
          break;

        case GsaLoad.LoadTypes.Beam:
          PostHog.Load(load.LoadType, load.BeamLoad.ReferenceType);
          if (load.BeamLoad.ReferenceType != ReferenceType.None) {
            if (memberElementRelationship == null)
              memberElementRelationship = ElementListFromReference.GetMemberElementRelationship(model);
            string objectElemList = ElementListFromReference.GetRefElementIds(load.BeamLoad, apiSections, apiElements, apiMembers, memberElementRelationship);
            if (objectElemList.Trim() != "")
              load.BeamLoad.BeamLoad.Elements = objectElemList;
            else {
              string warning = "One or more BeamLoads with reference to a " + load.BeamLoad.ReferenceType + " could not be added to the model. Ensure the reference " + load.BeamLoad.ReferenceType + " has been added to the model.";
              if (!owner.RuntimeMessages(GH_RuntimeMessageLevel.Warning).Contains(warning))
                owner.AddRuntimeWarning(warning);
              break;
            }
          }
          beamLoads.Add(load.BeamLoad.BeamLoad);
          break;

        case GsaLoad.LoadTypes.Face:
          PostHog.Load(load.LoadType, load.FaceLoad.ReferenceType);
          if (load.FaceLoad.ReferenceType != ReferenceType.None) {
            if (memberElementRelationship == null)
              memberElementRelationship = ElementListFromReference.GetMemberElementRelationship(model);
            string objectElemList = ElementListFromReference.GetRefElementIds(load.FaceLoad, apiProp2ds, apiElements, apiMembers, memberElementRelationship);
            if (objectElemList.Trim() != "")
              load.FaceLoad.FaceLoad.Elements = objectElemList;
            else {
              string warning = "One or more FaceLoads with reference to a " + load.FaceLoad.ReferenceType + " could not be added to the model. Ensure the reference " + load.FaceLoad.ReferenceType + " has been added to the model.";
              if (!owner.RuntimeMessages(GH_RuntimeMessageLevel.Warning).Contains(warning))
                owner.AddRuntimeWarning(warning);
              break;
            }
          }
          faceLoads.Add(load.FaceLoad.FaceLoad);
          break;

        case GsaLoad.LoadTypes.GridPoint:
          PostHog.Load(load.LoadType, ReferenceType.None);
          if (load.PointLoad.GridPlaneSurface == null) // if gridsurface id has been set
          {
            gridPointLoads.Add(load.PointLoad.GridPointLoad);
            break;
          }
          GsaGridPointLoad gridptref = load.PointLoad;
          GsaGridPlaneSurface gridplnsrf = gridptref.GridPlaneSurface;

          if (gridplnsrf.GridPlane != null) {
            // - grid load references a grid surface number
            // -- grid surface references a grid plane number
            // --- grid plane references an Axis number
            // toggle through the members in reverse order, set/add to model in each step

            gridplnsrf.GridPlane.AxisProperty = SetAxis(ref gridplnsrf, ref existingAxes, ref axisidcounter, unit);
            gridplnsrf.GridSurface.GridPlane = SetGridPlane(ref gridplnsrf, ref existingGridPlanes, ref gridplaneidcounter, ref gpGuid, existingAxes, unit);
            gridptref.GridPointLoad.GridSurface = SetGridSurface(ref gridplnsrf, ref existingGridSurfaces, ref gridsurfaceidcounter, ref gsGuid, existingGridPlanes, existingAxes, unit, ref memberElementRelationship, model, apiSections, apiProp2ds, apiProp3ds, apiElements, apiMembers);
          }
          gridPointLoads.Add(gridptref.GridPointLoad);
          break;

        case GsaLoad.LoadTypes.GridLine:
          PostHog.Load(load.LoadType, ReferenceType.None);
          if (load.LineLoad.GridPlaneSurface == null) // if gridsurface id has been set
          {
            gridLineLoads.Add(load.LineLoad.GridLineLoad);
            break;
          }
          GsaGridLineLoad gridlnref = load.LineLoad;
          gridplnsrf = gridlnref.GridPlaneSurface;

          if (gridplnsrf.GridPlane != null) {
            // - grid load references a grid surface number
            // -- grid surface references a grid plane number
            // --- grid plane references an Axis number
            // toggle through the members in reverse order, set/add to model in each step

            gridplnsrf.GridPlane.AxisProperty = SetAxis(ref gridplnsrf, ref existingAxes, ref axisidcounter, unit);
            gridplnsrf.GridSurface.GridPlane = SetGridPlane(ref gridplnsrf, ref existingGridPlanes, ref gridplaneidcounter, ref gpGuid, existingAxes, unit);
            gridlnref.GridLineLoad.GridSurface = SetGridSurface(ref gridplnsrf, ref existingGridSurfaces, ref gridsurfaceidcounter, ref gsGuid, existingGridPlanes, existingAxes, unit, ref memberElementRelationship, model, apiSections, apiProp2ds, apiProp3ds, apiElements, apiMembers);
          }
          gridLineLoads.Add(gridlnref.GridLineLoad);
          break;

        case GsaLoad.LoadTypes.GridArea:
          PostHog.Load(load.LoadType, ReferenceType.None, load.AreaLoad.GridAreaLoad.Type.ToString());
          if (load.AreaLoad.GridAreaLoad.Type == GridAreaPolyLineType.POLYGON)
            load.AreaLoad.GridAreaLoad.PolyLineDefinition += "(" + Length.GetAbbreviation(unit) + ")";

          if (load.AreaLoad.GridPlaneSurface == null) // if gridsurface id has been set
          {
            gridAreaLoads.Add(load.AreaLoad.GridAreaLoad);
            break;
          }

          GsaGridAreaLoad gridarref = load.AreaLoad;
          gridplnsrf = gridarref.GridPlaneSurface;

          if (gridplnsrf.GridPlane != null) {
            // - grid load references a grid surface number
            // -- grid surface references a grid plane number
            // --- grid plane references an Axis number
            // toggle through the members in reverse order, set/add to model in each step

            gridplnsrf.GridPlane.AxisProperty = SetAxis(ref gridplnsrf, ref existingAxes, ref axisidcounter, unit);
            gridplnsrf.GridSurface.GridPlane = SetGridPlane(ref gridplnsrf, ref existingGridPlanes, ref gridplaneidcounter, ref gpGuid, existingAxes, unit);
            gridarref.GridAreaLoad.GridSurface = SetGridSurface(ref gridplnsrf, ref existingGridSurfaces, ref gridsurfaceidcounter, ref gsGuid, existingGridPlanes, existingAxes, unit, ref memberElementRelationship, model, apiSections, apiProp2ds, apiProp3ds, apiElements, apiMembers);
          }
          gridAreaLoads.Add(gridarref.GridAreaLoad);
          break;
      }
    }

    internal static void ConvertNodeLoad(List<GsaLoad> loads,
        ref List<NodeLoad> nodeLoadsNode, ref List<NodeLoad> nodeLoadsDispl, ref List<NodeLoad> nodeLoadsSettle, ref GsaIntDictionary<Node> apiNodes, LengthUnit unit) {
      if (loads == null) {
        return;
      }

      foreach (GsaLoad load in loads.Where(load => load != null).Where(load => load.LoadType == GsaLoad.LoadTypes.Node))
        ConvertNodeLoad(load, ref nodeLoadsNode, ref nodeLoadsDispl, ref nodeLoadsSettle, ref apiNodes, unit);
    }

    internal static void ConvertNodeLoad(GsaLoad load, ref List<NodeLoad> nodeLoadsNode, ref List<NodeLoad> nodeLoadsDispl, ref List<NodeLoad> nodeLoadsSettle, ref GsaIntDictionary<Node> apiNodes, LengthUnit unit) {
      if (load.NodeLoad.RefPoint != Point3d.Unset)
        load.NodeLoad.NodeLoad.Nodes = Nodes.AddNode(ref apiNodes, load.NodeLoad.RefPoint, unit).ToString();
      switch (load.NodeLoad.Type)
      {
        case GsaNodeLoad.NodeLoadTypes.APPLIED_DISP:
          nodeLoadsDispl.Add(load.NodeLoad.NodeLoad);
          break;
        case GsaNodeLoad.NodeLoadTypes.NODE_LOAD:
          nodeLoadsNode.Add(load.NodeLoad.NodeLoad);
          break;
        case GsaNodeLoad.NodeLoadTypes.SETTLEMENT:
          nodeLoadsSettle.Add(load.NodeLoad.NodeLoad);
          break;
      }

      PostHog.Load(load.NodeLoad.RefPoint != Point3d.Unset, load.NodeLoad.Type.ToString());
    }
  }
}
