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

    internal static void ConvertLoad(
      List<GsaLoad> loads, ref List<GravityLoad> gravityLoads, ref List<BeamLoad> beamLoads,
      ref List<FaceLoad> faceLoads, ref List<GridPointLoad> gridPointLoads,
      ref List<GridLineLoad> gridLineLoads, ref List<GridAreaLoad> gridAreaLoads,
      ref Dictionary<int, Axis> existingAxes, ref Dictionary<int, GridPlane> existingGridPlanes,
      ref Dictionary<int, GridSurface> existingGridSurfaces, ref Dictionary<Guid, int> gpGuid,
      ref Dictionary<Guid, int> gsGuid, ref GsaGuidDictionary<EntityList> apiLists, LengthUnit unit,
      ConcurrentDictionary<int, ConcurrentBag<int>> memberElementRelationship, Model model,
      GsaGuidDictionary<AnalysisMaterial> apiMaterials, GsaGuidDictionary<Section> apiSections, 
      GsaGuidDictionary<Prop2D> apiProp2ds, GsaGuidDictionary<Prop3D> apiProp3ds, 
      GsaGuidIntListDictionary<Element> apiElements, GsaGuidDictionary<Member> apiMembers, 
      GH_Component owner) {
      if (loads == null) {
        return;
      }

      int axisidcounter = existingAxes.Count > 0 ? existingAxes.Keys.Max() + 1 : 1;
      int gridplaneidcounter = existingGridPlanes.Count > 0 ? existingGridPlanes.Keys.Max() + 1 : 1;
      int gridsurfaceidcounter
        = existingGridSurfaces.Count > 0 ? existingGridSurfaces.Keys.Max() + 1 : 1;

      GetGridPlaneSurfaceCounters(loads, ref gridplaneidcounter, ref gridsurfaceidcounter);

      foreach (GsaLoad load in loads.Where(gsaLoad => gsaLoad != null)) {
        ConvertLoad(load, ref gravityLoads, ref beamLoads, ref faceLoads, ref gridPointLoads,
          ref gridLineLoads, ref gridAreaLoads, ref existingAxes, ref axisidcounter,
          ref existingGridPlanes, ref gridplaneidcounter, ref existingGridSurfaces,
          ref gridsurfaceidcounter, ref gpGuid, ref gsGuid, ref apiLists, unit, memberElementRelationship,
          model, apiMaterials, apiSections, apiProp2ds, apiProp3ds, apiElements, apiMembers, owner);
      }
    }

    internal static void ConvertLoad(
      GsaLoad load, ref List<GravityLoad> gravityLoads, ref List<BeamLoad> beamLoads,
      ref List<FaceLoad> faceLoads, ref List<GridPointLoad> gridPointLoads,
      ref List<GridLineLoad> gridLineLoads, ref List<GridAreaLoad> gridAreaLoads,
      ref Dictionary<int, Axis> existingAxes, ref int axisidcounter,
      ref Dictionary<int, GridPlane> existingGridPlanes, ref int gridplaneidcounter,
      ref Dictionary<int, GridSurface> existingGridSurfaces, ref int gridsurfaceidcounter,
      ref Dictionary<Guid, int> gpGuid, ref Dictionary<Guid, int> gsGuid,
      ref GsaGuidDictionary<EntityList> apiLists, LengthUnit unit,
      ConcurrentDictionary<int, ConcurrentBag<int>> memberElementRelationship, Model model,
      GsaGuidDictionary<AnalysisMaterial> apiMaterials, GsaGuidDictionary<Section> apiSections, 
      GsaGuidDictionary<Prop2D> apiProp2ds, GsaGuidDictionary<Prop3D> apiProp3ds, 
      GsaGuidIntListDictionary<Element> apiElements, GsaGuidDictionary<Member> apiMembers, 
      GH_Component owner) {
      switch (load.LoadType) {
        case GsaLoad.LoadTypes.Gravity:
          PostHog.Load(load.LoadType, load.GravityLoad._referenceType);
          if (load.GravityLoad._referenceType != ReferenceType.None) {
            string objectElemList = load.GravityLoad.GravityLoad.Elements;
            
            if (load.GravityLoad._referenceType == ReferenceType.List) {
              if (load.GravityLoad._refList == null
                && (load.GravityLoad._refList.EntityType != Parameters.EntityType.Element
                || load.GravityLoad._refList.EntityType != Parameters.EntityType.Member)) {
                owner.AddRuntimeWarning("Invalid List type for GravityLoad " + load.ToString()
                  + Environment.NewLine + "Element list has not been set");
              }
              objectElemList +=
                Lists.GetElementList(load.GravityLoad._refList, ref apiLists, apiMaterials, apiSections,
                apiProp2ds, apiProp3ds, apiElements, apiMembers, memberElementRelationship, owner);
            } else {
              objectElemList += ElementListFromReference.GetRefElementIds(load.GravityLoad,
              apiMaterials, apiSections, apiProp2ds, apiProp3ds, apiElements, apiMembers,
              memberElementRelationship);
            }
            
            if (objectElemList.Trim() != string.Empty) {
              load.GravityLoad.GravityLoad.Elements = objectElemList;
            } else {
              string warning = "One or more GravityLoads with reference to a "
                + load.GravityLoad._referenceType
                + " could not be added to the model. Ensure the reference "
                + load.GravityLoad._referenceType + " has been added to the model.";
              if (!owner.RuntimeMessages(GH_RuntimeMessageLevel.Warning).Contains(warning)) {
                owner.AddRuntimeWarning(warning);
              }

              break;
            }
          }

          gravityLoads.Add(load.GravityLoad.GravityLoad);
          break;

        case GsaLoad.LoadTypes.Beam:
          PostHog.Load(load.LoadType, load.BeamLoad._referenceType);
          if (load.BeamLoad._referenceType != ReferenceType.None) {
            string objectElemList = load.BeamLoad.BeamLoad.Elements;

            if (load.BeamLoad._referenceType == ReferenceType.List) {
              if (load.BeamLoad._refList == null 
                && (load.BeamLoad._refList.EntityType != Parameters.EntityType.Element 
                || load.BeamLoad._refList.EntityType != Parameters.EntityType.Member)) {
                owner.AddRuntimeWarning("Invalid List type for BeamLoad " + load.ToString()
                  + Environment.NewLine + "Element list has not been set");
              }
              objectElemList +=
                Lists.GetElementList(load.BeamLoad._refList, ref apiLists, apiMaterials, apiSections,
                apiProp2ds, apiProp3ds, apiElements, apiMembers, memberElementRelationship, owner);
            } else {
              objectElemList += ElementListFromReference.GetRefElementIds(load.BeamLoad,
              apiMaterials, apiSections, apiElements, apiMembers, memberElementRelationship);
            }

            if (objectElemList.Trim() != string.Empty) {
              load.BeamLoad.BeamLoad.Elements = objectElemList;
            } else {
              string warning = "One or more BeamLoads with reference to a "
                + load.BeamLoad._referenceType
                + " could not be added to the model. Ensure the reference "
                + load.BeamLoad._referenceType + " has been added to the model.";
              if (!owner.RuntimeMessages(GH_RuntimeMessageLevel.Warning).Contains(warning)) {
                owner.AddRuntimeWarning(warning);
              }

              break;
            }
          }

          beamLoads.Add(load.BeamLoad.BeamLoad);
          break;

        case GsaLoad.LoadTypes.Face:
          PostHog.Load(load.LoadType, load.FaceLoad._referenceType);
          if (load.FaceLoad._referenceType != ReferenceType.None) {
            string objectElemList = load.FaceLoad.FaceLoad.Elements;

            if (load.FaceLoad._referenceType == ReferenceType.List) {
              if (load.FaceLoad._refList == null
                && (load.FaceLoad._refList.EntityType != Parameters.EntityType.Element
                || load.FaceLoad._refList.EntityType != Parameters.EntityType.Member)) {
                owner.AddRuntimeWarning("Invalid List type for BeamLoad " + load.ToString()
                  + Environment.NewLine + "Element list has not been set");
              }
              objectElemList +=
                Lists.GetElementList(load.FaceLoad._refList, ref apiLists, apiMaterials, apiSections,
                apiProp2ds, apiProp3ds, apiElements, apiMembers, memberElementRelationship, owner);
            } else {
              objectElemList += ElementListFromReference.GetRefElementIds(load.FaceLoad,
              apiMaterials, apiProp2ds, apiElements, apiMembers, memberElementRelationship);
            }
            
            if (objectElemList.Trim() != string.Empty) {
              load.FaceLoad.FaceLoad.Elements = objectElemList;
            } else {
              string warning = "One or more FaceLoads with reference to a "
                + load.FaceLoad._referenceType
                + " could not be added to the model. Ensure the reference "
                + load.FaceLoad._referenceType + " has been added to the model.";
              if (!owner.RuntimeMessages(GH_RuntimeMessageLevel.Warning).Contains(warning)) {
                owner.AddRuntimeWarning(warning);
              }

              break;
            }
          }

          faceLoads.Add(load.FaceLoad.FaceLoad);
          break;

        case GsaLoad.LoadTypes.GridPoint:
          PostHog.Load(load.LoadType, ReferenceType.None);
          if (load.PointLoad.GridPlaneSurface == null) {
            gridPointLoads.Add(load.PointLoad.GridPointLoad);
            break;
          }

          GsaGridPointLoad gridptref = load.PointLoad.Duplicate();
          if (unit != LengthUnit.Meter) {
            gridptref.GridPointLoad.X = new Length(gridptref.GridPointLoad.X, unit).As(LengthUnit.Meter);
            gridptref.GridPointLoad.Y = new Length(gridptref.GridPointLoad.Y, unit).As(LengthUnit.Meter);
          }

          GsaGridPlaneSurface gridplnsrf = gridptref.GridPlaneSurface;

          if (gridplnsrf.GridPlane != null) {
            // - grid load references a grid surface number
            // -- grid surface references a grid plane number
            // --- grid plane references an Axis number
            // toggle through the members in reverse order, set/add to model in each step

            gridplnsrf.GridPlane.AxisProperty
              = SetAxis(ref gridplnsrf, ref existingAxes, ref axisidcounter, unit);
            gridplnsrf.GridSurface.GridPlane = SetGridPlane(ref gridplnsrf, ref existingGridPlanes,
              ref gridplaneidcounter, ref gpGuid, existingAxes, unit);
            gridptref.GridPointLoad.GridSurface = SetGridSurface(ref gridplnsrf,
              ref existingGridSurfaces, ref gridsurfaceidcounter, ref gsGuid, ref apiLists, existingGridPlanes,
              existingAxes, unit, memberElementRelationship, model, apiMaterials, apiSections, 
              apiProp2ds, apiProp3ds, apiElements, apiMembers, owner);
          }

          gridPointLoads.Add(gridptref.GridPointLoad);
          break;

        case GsaLoad.LoadTypes.GridLine:
          PostHog.Load(load.LoadType, ReferenceType.None);
          if (load.LineLoad.GridPlaneSurface == null) {
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

            gridplnsrf.GridPlane.AxisProperty
              = SetAxis(ref gridplnsrf, ref existingAxes, ref axisidcounter, unit);
            gridplnsrf.GridSurface.GridPlane = SetGridPlane(ref gridplnsrf, ref existingGridPlanes,
              ref gridplaneidcounter, ref gpGuid, existingAxes, unit);
            gridlnref.GridLineLoad.GridSurface = SetGridSurface(ref gridplnsrf,
              ref existingGridSurfaces, ref gridsurfaceidcounter, ref gsGuid, ref apiLists, 
              existingGridPlanes, existingAxes, unit, memberElementRelationship, model, apiMaterials, 
              apiSections, apiProp2ds, apiProp3ds, apiElements, apiMembers, owner);
          }

          gridLineLoads.Add(gridlnref.GridLineLoad);
          break;

        case GsaLoad.LoadTypes.GridArea:
          PostHog.Load(load.LoadType, ReferenceType.None,
            load.AreaLoad.GridAreaLoad.Type.ToString());
          if (load.AreaLoad.GridAreaLoad.Type == GridAreaPolyLineType.POLYGON) {
            load.AreaLoad.GridAreaLoad.PolyLineDefinition
              += "(" + Length.GetAbbreviation(unit) + ")";
          }

          if (load.AreaLoad.GridPlaneSurface == null) {
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

            gridplnsrf.GridPlane.AxisProperty
              = SetAxis(ref gridplnsrf, ref existingAxes, ref axisidcounter, unit);
            gridplnsrf.GridSurface.GridPlane = SetGridPlane(ref gridplnsrf, ref existingGridPlanes,
              ref gridplaneidcounter, ref gpGuid, existingAxes, unit);
            gridarref.GridAreaLoad.GridSurface = SetGridSurface(ref gridplnsrf,
              ref existingGridSurfaces, ref gridsurfaceidcounter, ref gsGuid, ref apiLists,
              existingGridPlanes, existingAxes, unit, memberElementRelationship, model, apiMaterials, 
              apiSections, apiProp2ds, apiProp3ds, apiElements, apiMembers, owner);
          }

          gridAreaLoads.Add(gridarref.GridAreaLoad);
          break;
      }
    }

    internal static void ConvertNodeLoad(
      List<GsaLoad> loads, ref List<NodeLoad> nodeLoadsNode, ref List<NodeLoad> nodeLoadsDispl,
      ref List<NodeLoad> nodeLoadsSettle, ref GsaIntDictionary<Node> apiNodes,
      ref GsaGuidDictionary<EntityList> apiLists, LengthUnit unit) {
      if (loads == null) {
        return;
      }

      foreach (GsaLoad load in loads.Where(load => load != null)
       .Where(load => load.LoadType == GsaLoad.LoadTypes.Node)) {
        ConvertNodeLoad(load, ref nodeLoadsNode, ref nodeLoadsDispl, ref nodeLoadsSettle,
          ref apiNodes, ref apiLists, unit);
      }
    }

    internal static void ConvertNodeLoad(
      GsaLoad load, ref List<NodeLoad> nodeLoadsNode, ref List<NodeLoad> nodeLoadsDispl,
      ref List<NodeLoad> nodeLoadsSettle, ref GsaIntDictionary<Node> apiNodes,
      ref GsaGuidDictionary<EntityList> apiLists, LengthUnit unit) {
      if (load.NodeLoad._refPoint != Point3d.Unset) {
        load.NodeLoad.NodeLoad.Nodes
          = Nodes.AddNode(ref apiNodes, load.NodeLoad._refPoint, unit).ToString();
      }
      if (load.NodeLoad._refList != null) {
        load.NodeLoad.NodeLoad.Nodes = Lists.GetNodeList(
          load.NodeLoad._refList, ref apiLists, ref apiNodes, unit);
      }

      switch (load.NodeLoad.Type) {
        case GsaNodeLoad.NodeLoadTypes.AppliedDisp:
          nodeLoadsDispl.Add(load.NodeLoad.NodeLoad);
          break;

        case GsaNodeLoad.NodeLoadTypes.NodeLoad:
          nodeLoadsNode.Add(load.NodeLoad.NodeLoad);
          break;

        case GsaNodeLoad.NodeLoadTypes.Settlement:
          nodeLoadsSettle.Add(load.NodeLoad.NodeLoad);
          break;
      }

      PostHog.Load(load.NodeLoad._refPoint != Point3d.Unset, load.NodeLoad.Type.ToString());
    }
  }
}
