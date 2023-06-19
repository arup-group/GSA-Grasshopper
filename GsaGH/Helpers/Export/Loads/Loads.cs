using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Grasshopper.Kernel;
using GsaAPI;
using GsaAPI.Materials;
using GsaGH.Helpers.Export.Loads;
using GsaGH.Helpers.GH;
using GsaGH.Parameters;
using OasysUnits;
using Rhino.Geometry;
using LengthUnit = OasysUnits.Units.LengthUnit;

namespace GsaGH.Helpers.Export {
  internal partial class Loads {
    internal Load.NodeLoads Nodes;
    internal List<GravityLoad> Gravities;
    internal List<BeamLoad> Beams;
    internal List<FaceLoad> Faces;
    internal List<GridPointLoad> GridPoints;
    internal List<GridLineLoad> GridLines;
    internal List<GridAreaLoad> GridAreas;
    internal GridPlaneSurfaces GridPlaneSurfaces;

    internal Loads() {
      Nodes = new Load.NodeLoads();

      Gravities = new List<GravityLoad>();
      Beams = new List<BeamLoad>();
      Faces = new List<FaceLoad>();

      GridPoints = new List<GridPointLoad>();
      GridLines = new List<GridLineLoad>();
      GridAreas = new List<GridAreaLoad>();
    }

    internal void ConvertLoad(List<GsaLoad> loads, ref ModelAssembly model, GH_Component owner) {
      if (loads == null) {
        return;
      }

      foreach (GsaLoad load in loads.Where(gsaLoad => gsaLoad != null)) {
        ConvertLoad(load, ref model, owner);
      }
    }

    internal void ConvertLoad(GsaLoad load, ref ModelAssembly model, GH_Component owner) {
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
                Lists.GetElementList(load.GravityLoad._refList, ref model, owner);
            } else {
              objectElemList += ElementListFromReference.GetReferenceElementIdsDefinition(load.GravityLoad, model);
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

          Gravities.Add(load.GravityLoad.GravityLoad);
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
                Lists.GetElementList(load.BeamLoad._refList, ref model, owner);
            } else {
              objectElemList += ElementListFromReference.GetReferenceElementIdsDefinition(
                load.BeamLoad, model);
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

          Beams.Add(load.BeamLoad.BeamLoad);
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
                Lists.GetElementList(load.FaceLoad._refList, ref model, owner);
            } else {
              objectElemList += ElementListFromReference.GetReferenceElementIdsDefinition(
                load.FaceLoad, model);
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

          Faces.Add(load.FaceLoad.FaceLoad);
          break;

        case GsaLoad.LoadTypes.GridPoint:
          PostHog.Load(load.LoadType, ReferenceType.None);
          if (load.PointLoad.GridPlaneSurface == null) {
            GridPoints.Add(load.PointLoad.GridPointLoad);
            break;
          }

          GsaGridPointLoad gridptref = load.PointLoad.Duplicate();
          if (model.Unit != LengthUnit.Meter) {
            gridptref.GridPointLoad.X = new Length(
              gridptref.GridPointLoad.X, model.Unit).As(LengthUnit.Meter);
            gridptref.GridPointLoad.Y = new Length(
              gridptref.GridPointLoad.Y, model.Unit).As(LengthUnit.Meter);
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
          if (unit != LengthUnit.Meter
            && gridlnref.GridLineLoad.Type == GridLineLoad.PolyLineType.EXPLICIT_POLYLINE) {
            gridlnref.GridLineLoad.PolyLineDefinition =
              GridLoadHelper.ClearDefinitionForUnit(gridlnref.GridLineLoad.PolyLineDefinition) +
              $"({Length.GetAbbreviation(unit)})";
          }

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
            load.AreaLoad.GridAreaLoad.PolyLineDefinition =
            GridLoadHelper.ClearDefinitionForUnit(load.AreaLoad.GridAreaLoad.PolyLineDefinition) +
            $"({Length.GetAbbreviation(unit)})";
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

    
  }
}
