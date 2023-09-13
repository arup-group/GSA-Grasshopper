using System;
using System.Collections.Generic;
using System.Linq;
using Grasshopper.Kernel;
using GsaAPI;
using GsaGH.Helpers.GH;
using GsaGH.Parameters;
using OasysUnits;
using EntityType = GsaGH.Parameters.EntityType;
using LengthUnit = OasysUnits.Units.LengthUnit;
using LoadCase = GsaAPI.LoadCase;

namespace GsaGH.Helpers.Export {
  internal partial class Loads {
    internal Load.NodeLoads Nodes = new Load.NodeLoads();
    internal List<GravityLoad> Gravities = new List<GravityLoad>();
    internal List<BeamLoad> Beams = new List<BeamLoad>();
    internal List<BeamThermalLoad> BeamThermals = new List<BeamThermalLoad>();
    internal List<FaceLoad> Faces = new List<FaceLoad>();
    internal List<FaceThermalLoad> FaceThermals = new List<FaceThermalLoad>();
    internal List<GridPointLoad> GridPoints = new List<GridPointLoad>();
    internal List<GridLineLoad> GridLines = new List<GridLineLoad>();
    internal List<GridAreaLoad> GridAreas = new List<GridAreaLoad>();
    internal GridPlaneSurfaces GridPlaneSurfaces;
    internal Dictionary<int, LoadCase> LoadCases;

    internal Loads(Model model) {
      GetLoadCasesFromModel(model);
      GridPlaneSurfaces = new GridPlaneSurfaces(model);
    }

    internal static void ConvertLoad(List<IGsaLoad> loads, ref ModelAssembly model, GH_Component owner) {
      if (loads == null) {
        return;
      }

      foreach (IGsaLoad load in loads.Where(gsaLoad => gsaLoad != null)) {
        ConvertLoad(load, ref model, owner);
      }
    }

    internal void GetLoadCasesFromModel(Model model) {
      LoadCases = model.LoadCases().ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
      foreach (int key in LoadCases.Keys) {
        // some old gwb files stores Dead as (int)0:
        if ((int)LoadCases[key].CaseType == 0) {
          LoadCases[key].CaseType = GsaAPI.LoadCaseType.Dead;
        }
      }
    }

    internal static void ConvertLoad(IGsaLoad load, ref ModelAssembly model, GH_Component owner) {
      ConvertLoadCase(load.LoadCase, ref model, owner);
      load.CaseId = load.LoadCase.Id;
      switch (load.LoadType) {
        case LoadType.Gravity:
          ConvertGravityLoad((GsaGravityLoad)load, ref model, owner);
          break;

        case LoadType.Beam:
          ConvertBeamLoad((GsaBeamLoad)load, ref model, owner);
          break;

        case LoadType.BeamThermal:
          ConvertBeamThermalLoad((GsaBeamThermalLoad)load, ref model, owner);
          break;

        case LoadType.Face:
          ConvertFaceLoad((GsaFaceLoad)load, ref model, owner);
          break;

        case LoadType.FaceThermal:
          ConvertFaceThermalLoad((GsaFaceThermalLoad)load, ref model, owner);
          break;

        case LoadType.GridPoint:
          ConvertGridPointLoad((GsaGridPointLoad)load, ref model, owner);
          break;

        case LoadType.GridLine:
          ConvertGridLineLoad((GsaGridLineLoad)load, ref model, owner);
          break;

        case LoadType.GridArea:
          ConvertGridAreaLoad((GsaGridAreaLoad)load, ref model, owner);
          break;
      }
    }

    internal static void ConvertLoadCase(
      GsaLoadCase loadCase, ref ModelAssembly model, GH_Component owner) {
      if (loadCase == null) {
        return;
      }

      if (model.Loads.LoadCases.ContainsKey(loadCase.Id)) {
        LoadCase existingCase = model.Loads.LoadCases[loadCase.Id];
        LoadCase newCase = loadCase.LoadCase;
        if (newCase.CaseType != existingCase.CaseType || newCase.Name != existingCase.Name) {
          model.Loads.LoadCases[loadCase.Id] = newCase;
          owner?.AddRuntimeRemark($"LoadCase {loadCase.Id} either already existed in the model " +
           $"or two load cases with ID:{loadCase.Id} was added.{Environment.NewLine}" +
           $"{newCase.Name} - {newCase.CaseType} replaced previous LoadCase");
        }
      } else {
        if (loadCase.LoadCase != null) {
          model.Loads.LoadCases.Add(loadCase.Id, loadCase.LoadCase);
        }
      }
    }

    private static void ConvertGridAreaLoad(GsaGridAreaLoad load, ref ModelAssembly model, GH_Component owner) {
      PostHog.Load(load.LoadType, ReferenceType.None,
        load.GridAreaLoad.Type.ToString());
      if (load.GridAreaLoad.Type == GridAreaPolyLineType.POLYGON) {
        load.GridAreaLoad.PolyLineDefinition =
        GridLoadHelper.ClearDefinitionForUnit(load.GridAreaLoad.PolyLineDefinition) +
        $"({Length.GetAbbreviation(model.Unit)})";
      }

      if (load.GridPlaneSurface == null) {
        model.Loads.GridAreas.Add(load.GridAreaLoad);
        return;
      }

      GsaGridPlaneSurface gridplnsrf = load.GridPlaneSurface;

      if (gridplnsrf.GridPlane != null) {
        load.GridAreaLoad.GridSurface = GridPlaneSurfaces.ConvertGridPlaneSurface(gridplnsrf, ref model, owner);
      }

      model.Loads.GridAreas.Add(load.GridAreaLoad);
    }

    private static void ConvertGridLineLoad(GsaGridLineLoad load, ref ModelAssembly model, GH_Component owner) {
      PostHog.Load(load.LoadType, ReferenceType.None);
      if (load.GridPlaneSurface == null) {
        model.Loads.GridLines.Add(load.GridLineLoad);
        return;
      }

      if (model.Unit != LengthUnit.Meter
        && load.GridLineLoad.Type == GridLineLoad.PolyLineType.EXPLICIT_POLYLINE) {
        load.GridLineLoad.PolyLineDefinition =
          GridLoadHelper.ClearDefinitionForUnit(load.GridLineLoad.PolyLineDefinition) +
          $"({Length.GetAbbreviation(model.Unit)})";
      }

      GsaGridPlaneSurface gridplnsrf = load.GridPlaneSurface;

      if (gridplnsrf.GridPlane != null) {
        load.GridLineLoad.GridSurface
          = GridPlaneSurfaces.ConvertGridPlaneSurface(gridplnsrf, ref model, owner);
      }

      model.Loads.GridLines.Add(load.GridLineLoad);
    }

    private static void ConvertGridPointLoad(GsaGridPointLoad load, ref ModelAssembly model, GH_Component owner) {
      PostHog.Load(load.LoadType, ReferenceType.None);
      if (load.GridPlaneSurface == null) {
        model.Loads.GridPoints.Add(load.GridPointLoad);
        return;
      }

      var gridptref = (GsaGridPointLoad)load.Duplicate();
      if (model.Unit != LengthUnit.Meter) {
        gridptref.GridPointLoad.X = new Length(gridptref.GridPointLoad.X, model.Unit).As(LengthUnit.Meter);
        gridptref.GridPointLoad.Y = new Length(gridptref.GridPointLoad.Y, model.Unit).As(LengthUnit.Meter);
      }

      GsaGridPlaneSurface gridplnsrf = gridptref.GridPlaneSurface;

      if (gridplnsrf.GridPlane != null) {
        gridptref.GridPointLoad.GridSurface
          = GridPlaneSurfaces.ConvertGridPlaneSurface(gridplnsrf, ref model, owner);
      }

      model.Loads.GridPoints.Add(gridptref.GridPointLoad);
    }

    private static void ConvertBeamLoad(
      GsaBeamLoad load, ref ModelAssembly model, GH_Component owner) {
      PostHog.Load(load.LoadType, load.ReferenceType);
      if (load.ReferenceType != ReferenceType.None) {
        string objectElemList = load.BeamLoad.EntityList;

        if (load.ReferenceType == ReferenceType.List) {
          if (load.ReferenceList == null
            && (load.ReferenceList.EntityType != EntityType.Element
            || load.ReferenceList.EntityType != EntityType.Member)) {
            owner.AddRuntimeWarning("Invalid List type for BeamLoad " + load.ToString()
              + Environment.NewLine + "Element list has not been set");
          }
          objectElemList += Lists.GetElementOrMemberList(load.ReferenceList, ref model, owner);
          load.BeamLoad.EntityType = GsaList.GetAPIEntityType(load.ReferenceList.EntityType);
        } else {
          objectElemList += ElementListFromReference.GetReferenceDefinition(load, model);
        }

        if (objectElemList.Trim() != string.Empty) {
          load.BeamLoad.EntityList = objectElemList;
        } else {
          string warning = "One or more BeamLoads with reference to a " + load.ReferenceType
            + " could not be added to the model. Ensure the reference " + load.ReferenceType
            + " has been added to the model.";
          if (!owner.RuntimeMessages(GH_RuntimeMessageLevel.Warning).Contains(warning)) {
            owner.AddRuntimeWarning(warning);
          }

          return;
        }
      }

      model.Loads.Beams.Add(load.BeamLoad);
    }

    private static void ConvertBeamThermalLoad(GsaBeamThermalLoad load, ref ModelAssembly model, GH_Component owner) {
      PostHog.Load(load.LoadType, load.ReferenceType);
      if (load.ReferenceType != ReferenceType.None) {
        string objectElemList = load.BeamThermalLoad.EntityList;

        if (load.ReferenceType == ReferenceType.List) {
          if (load.ReferenceList == null
            && (load.ReferenceList.EntityType != EntityType.Element
            || load.ReferenceList.EntityType != EntityType.Member)) {
            owner.AddRuntimeWarning("Invalid List type for BeamThermalLoad " + load.ToString()
              + Environment.NewLine + "Element list has not been set");
          }
          objectElemList += Lists.GetElementOrMemberList(load.ReferenceList, ref model, owner);
          load.BeamThermalLoad.EntityType = GsaList.GetAPIEntityType(load.ReferenceList.EntityType);
        } else {
          objectElemList += ElementListFromReference.GetReferenceDefinition(load, model);
        }

        if (objectElemList.Trim() != string.Empty) {
          load.BeamThermalLoad.EntityList = objectElemList;
        } else {
          string warning = "One or more BeamThermalLoads with reference to a " + load.ReferenceType
            + " could not be added to the model. Ensure the reference " + load.ReferenceType
            + " has been added to the model.";
          if (!owner.RuntimeMessages(GH_RuntimeMessageLevel.Warning).Contains(warning)) {
            owner.AddRuntimeWarning(warning);
          }

          return;
        }
      }

      model.Loads.BeamThermals.Add(load.BeamThermalLoad);
    }

    private static void ConvertGravityLoad(GsaGravityLoad load, ref ModelAssembly model, GH_Component owner) {
      PostHog.Load(load.LoadType, load.ReferenceType);
      if (load.ReferenceType != ReferenceType.None) {
        string objectElemList = load.GravityLoad.EntityList;

        if (load.ReferenceType == ReferenceType.List) {
          if (load.ReferenceList == null
            && (load.ReferenceList.EntityType != EntityType.Element
            || load.ReferenceList.EntityType != EntityType.Member)) {
            owner.AddRuntimeWarning("Invalid List type for GravityLoad " + load.ToString()
              + Environment.NewLine + "Element list has not been set");
          }
          objectElemList +=
            Lists.GetElementOrMemberList(load.ReferenceList, ref model, owner);
          load.GravityLoad.EntityType = GsaList.GetAPIEntityType(load.ReferenceList.EntityType);
        } else {
          objectElemList += ElementListFromReference.GetReferenceDefinition(load, model);
        }

        if (objectElemList.Trim() != string.Empty) {
          load.GravityLoad.EntityList = objectElemList;
        } else {
          string warning = "One or more GravityLoads with reference to a " + load.ReferenceType
            + " could not be added to the model. Ensure the reference " + load.ReferenceType
            + " has been added to the model.";
          if (!owner.RuntimeMessages(GH_RuntimeMessageLevel.Warning).Contains(warning)) {
            owner.AddRuntimeWarning(warning);
          }

          return;
        }
      }

      model.Loads.Gravities.Add(load.GravityLoad);
    }

    private static void ConvertFaceLoad(GsaFaceLoad load, ref ModelAssembly model, GH_Component owner) {
      PostHog.Load(load.LoadType, load.ReferenceType);
      if (load.ReferenceType != ReferenceType.None) {
        string objectElemList = load.FaceLoad.EntityList;

        if (load.ReferenceType == ReferenceType.List) {
          if (load.ReferenceList == null
            && (load.ReferenceList.EntityType != EntityType.Element
            || load.ReferenceList.EntityType != EntityType.Member)) {
            owner.AddRuntimeWarning("Invalid List type for FaceLoad " + load.ToString()
              + Environment.NewLine + "Element list has not been set");
          }
          objectElemList +=
            Lists.GetElementOrMemberList(load.ReferenceList, ref model, owner);
          load.FaceLoad.EntityType = GsaList.GetAPIEntityType(load.ReferenceList.EntityType);
        } else {
          objectElemList += ElementListFromReference.GetReferenceDefinition(load, model);
        }

        if (objectElemList.Trim() != string.Empty) {
          load.FaceLoad.EntityList = objectElemList;
        } else {
          string warning = "One or more FaceLoads with reference to a " + load.ReferenceType
            + " could not be added to the model. Ensure the reference " + load.ReferenceType
            + " has been added to the model.";
          if (!owner.RuntimeMessages(GH_RuntimeMessageLevel.Warning).Contains(warning)) {
            owner.AddRuntimeWarning(warning);
          }
          return;
        }
      }

      model.Loads.Faces.Add(load.FaceLoad);
    }

    private static void ConvertFaceThermalLoad(GsaFaceThermalLoad load, ref ModelAssembly model, GH_Component owner) {
      PostHog.Load(load.LoadType, load.ReferenceType);
      if (load.ReferenceType != ReferenceType.None) {
        string objectElemList = load.FaceThermalLoad.EntityList;

        if (load.ReferenceType == ReferenceType.List) {
          if (load.ReferenceList == null
            && (load.ReferenceList.EntityType != EntityType.Element
            || load.ReferenceList.EntityType != EntityType.Member)) {
            owner.AddRuntimeWarning("Invalid List type for FaceThermalLoads " + load.ToString()
              + Environment.NewLine + "Element list has not been set");
          }
          objectElemList +=
            Lists.GetElementOrMemberList(load.ReferenceList, ref model, owner);
          load.FaceThermalLoad.EntityType = GsaList.GetAPIEntityType(load.ReferenceList.EntityType);
        } else {
          objectElemList += ElementListFromReference.GetReferenceDefinition(load, model);
        }

        if (objectElemList.Trim() != string.Empty) {
          load.FaceThermalLoad.EntityList = objectElemList;
        } else {
          string warning = "One or more FaceThermalLoads with reference to a " + load.ReferenceType
            + " could not be added to the model. Ensure the reference " + load.ReferenceType
            + " has been added to the model.";
          if (!owner.RuntimeMessages(GH_RuntimeMessageLevel.Warning).Contains(warning)) {
            owner.AddRuntimeWarning(warning);
          }
          return;
        }
      }

      model.Loads.FaceThermals.Add(load.FaceThermalLoad);
    }
  }
}
