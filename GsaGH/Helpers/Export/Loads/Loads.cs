using System;
using System.Collections.Generic;
using System.Linq;
using Grasshopper.Kernel;
using GsaAPI;
using GsaGH.Helpers.GH;
using GsaGH.Parameters;
using GsaGH.Parameters.Enums;
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
        if (LoadCases[key].CaseType == 0) {
          LoadCases[key].CaseType = GsaAPI.LoadCaseType.Dead;
        }
      }
    }

    internal static void ConvertLoad(IGsaLoad load, ref ModelAssembly model, GH_Component owner) {
      ConvertLoadCase(load.LoadCase, ref model, owner);
      if (load.LoadCase != null) {
        load.CaseId = load.LoadCase.Id;
      }

      switch (load) {
        case GsaGravityLoad gravity:
          ConvertGravityLoad(gravity, ref model, owner);
          break;

        case GsaBeamLoad beam:
          ConvertBeamLoad(beam, ref model, owner);
          break;

        case GsaBeamThermalLoad beamThermal:
          ConvertBeamThermalLoad(beamThermal, ref model, owner);
          break;

        case GsaFaceLoad face:
          ConvertFaceLoad(face, ref model, owner);
          break;

        case GsaFaceThermalLoad faceThermal:
          ConvertFaceThermalLoad(faceThermal, ref model, owner);
          break;

        case GsaGridPointLoad point:
          ConvertGridPointLoad(point, ref model, owner);
          break;

        case GsaGridLineLoad line:
          ConvertGridLineLoad(line, ref model, owner);
          break;

        case GsaGridAreaLoad area:
          ConvertGridAreaLoad(area, ref model, owner);
          break;
      }
    }

    internal static void ConvertLoadCase(
      GsaLoadCase loadCase, ref ModelAssembly model, GH_Component owner) {
      if (loadCase == null || loadCase.LoadCase == null) {
        return;
      }

      if (model.Loads.LoadCases.ContainsKey(loadCase.Id)) {
        LoadCase existingCase = model.Loads.LoadCases[loadCase.Id];
        LoadCase newCase = loadCase.LoadCase;
        if (newCase.CaseType != existingCase.CaseType || newCase.Name != existingCase.Name) {
          model.Loads.LoadCases[loadCase.Id] = newCase;

          owner?.AddRuntimeRemark($"LoadCase {loadCase.Id} either already existed in the model " +
           $"or two load cases with ID:{loadCase.Id} were added.{Environment.NewLine}" +
           $"{newCase.Name} - {newCase.CaseType} replaced previous LoadCase");
        }

        return;
      }

      model.Loads.LoadCases.Add(loadCase.Id, loadCase.LoadCase);
    }

    private static void ConvertGridAreaLoad(GsaGridAreaLoad load, ref ModelAssembly model, GH_Component owner) {
      PostHog.Load(load, ReferenceType.None,
        load.ApiLoad.Type.ToString());
      if (load.ApiLoad.Type == GridAreaPolyLineType.POLYGON) {
        load.ApiLoad.PolyLineDefinition =
        GridLoadHelper.ClearDefinitionForUnit(load.ApiLoad.PolyLineDefinition) +
        $"({Length.GetAbbreviation(model.Unit)})";
      }

      if (load.GridPlaneSurface == null) {
        model.Loads.GridAreas.Add(load.ApiLoad);
        return;
      }

      GsaGridPlaneSurface gridplnsrf = load.GridPlaneSurface;

      if (gridplnsrf.GridPlane != null) {
        load.ApiLoad.GridSurface = GridPlaneSurfaces.ConvertGridPlaneSurface(gridplnsrf, ref model, owner);
      }

      model.Loads.GridAreas.Add(load.ApiLoad);
    }

    private static void ConvertGridLineLoad(GsaGridLineLoad load, ref ModelAssembly model, GH_Component owner) {
      PostHog.Load(load, ReferenceType.None);
      if (load.GridPlaneSurface == null) {
        model.Loads.GridLines.Add(load.ApiLoad);
        return;
      }

      if (model.Unit != LengthUnit.Meter
        && load.ApiLoad.Type == GridLineLoad.PolyLineType.EXPLICIT_POLYLINE) {
        load.ApiLoad.PolyLineDefinition =
          GridLoadHelper.ClearDefinitionForUnit(load.ApiLoad.PolyLineDefinition) +
          $"({Length.GetAbbreviation(model.Unit)})";
      }

      GsaGridPlaneSurface gridplnsrf = load.GridPlaneSurface;

      if (gridplnsrf.GridPlane != null) {
        load.ApiLoad.GridSurface
          = GridPlaneSurfaces.ConvertGridPlaneSurface(gridplnsrf, ref model, owner);
      }

      model.Loads.GridLines.Add(load.ApiLoad);
    }

    private static void ConvertGridPointLoad(GsaGridPointLoad load, ref ModelAssembly model, GH_Component owner) {
      PostHog.Load(load, ReferenceType.None);
      if (load.GridPlaneSurface == null) {
        model.Loads.GridPoints.Add(load.ApiLoad);
        return;
      }

      var gridptref = (GsaGridPointLoad)load.Duplicate();
      if (model.Unit != LengthUnit.Meter) {
        gridptref.ApiLoad.X = new Length(gridptref.ApiLoad.X, model.Unit).As(LengthUnit.Meter);
        gridptref.ApiLoad.Y = new Length(gridptref.ApiLoad.Y, model.Unit).As(LengthUnit.Meter);
      }

      GsaGridPlaneSurface gridplnsrf = gridptref.GridPlaneSurface;

      if (gridplnsrf.GridPlane != null) {
        gridptref.ApiLoad.GridSurface
          = GridPlaneSurfaces.ConvertGridPlaneSurface(gridplnsrf, ref model, owner);
      }

      model.Loads.GridPoints.Add(gridptref.ApiLoad);
    }

    private static void ConvertBeamLoad(
      GsaBeamLoad load, ref ModelAssembly model, GH_Component owner) {
      PostHog.Load(load, load.ReferenceType);
      if (load.ReferenceType != ReferenceType.None) {
        string objectElemList = load.ApiLoad.EntityList;

        if (load.ReferenceType == ReferenceType.List) {
          if (load.ReferenceList == null
            && (load.ReferenceList.EntityType != EntityType.Element
            || load.ReferenceList.EntityType != EntityType.Member)) {
            owner.AddRuntimeWarning("Invalid List type for BeamLoad " + load.ToString()
              + Environment.NewLine + "Element list has not been set");
          }
          objectElemList += Lists.GetElementOrMemberList(load.ReferenceList, ref model, owner);
          load.ApiLoad.EntityType = GsaList.GetAPIEntityType(load.ReferenceList.EntityType);
        } else {
          objectElemList += ElementListFromReference.GetReferenceDefinition(load, model);
        }

        if (objectElemList.Trim() != string.Empty) {
          load.ApiLoad.EntityList = objectElemList;
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

      model.Loads.Beams.Add(load.ApiLoad);
    }

    private static void ConvertBeamThermalLoad(GsaBeamThermalLoad load, ref ModelAssembly model, GH_Component owner) {
      PostHog.Load(load, load.ReferenceType);
      if (load.ReferenceType != ReferenceType.None) {
        string objectElemList = load.ApiLoad.EntityList;

        if (load.ReferenceType == ReferenceType.List) {
          if (load.ReferenceList == null
            && (load.ReferenceList.EntityType != EntityType.Element
            || load.ReferenceList.EntityType != EntityType.Member)) {
            owner.AddRuntimeWarning("Invalid List type for BeamThermalLoad " + load.ToString()
              + Environment.NewLine + "Element list has not been set");
          }
          objectElemList += Lists.GetElementOrMemberList(load.ReferenceList, ref model, owner);
          load.ApiLoad.EntityType = GsaList.GetAPIEntityType(load.ReferenceList.EntityType);
        } else {
          objectElemList += ElementListFromReference.GetReferenceDefinition(load, model);
        }

        if (objectElemList.Trim() != string.Empty) {
          load.ApiLoad.EntityList = objectElemList;
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

      model.Loads.BeamThermals.Add(load.ApiLoad);
    }

    private static void ConvertGravityLoad(GsaGravityLoad load, ref ModelAssembly model, GH_Component owner) {
      PostHog.Load(load, load.ReferenceType);
      if (load.ReferenceType != ReferenceType.None) {
        string objectElemList = load.ApiLoad.EntityList;

        if (load.ReferenceType == ReferenceType.List) {
          if (load.ReferenceList == null
            && (load.ReferenceList.EntityType != EntityType.Element
            || load.ReferenceList.EntityType != EntityType.Member)) {
            owner.AddRuntimeWarning("Invalid List type for GravityLoad " + load.ToString()
              + Environment.NewLine + "Element list has not been set");
          }
          objectElemList +=
            Lists.GetElementOrMemberList(load.ReferenceList, ref model, owner);
          load.ApiLoad.EntityType = GsaList.GetAPIEntityType(load.ReferenceList.EntityType);
        } else {
          objectElemList += ElementListFromReference.GetReferenceDefinition(load, model);
        }

        if (objectElemList.Trim() != string.Empty) {
          load.ApiLoad.EntityList = objectElemList;
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

      model.Loads.Gravities.Add(load.ApiLoad);
    }

    private static void ConvertFaceLoad(GsaFaceLoad load, ref ModelAssembly model, GH_Component owner) {
      PostHog.Load(load, load.ReferenceType);
      if (load.ReferenceType != ReferenceType.None) {
        string objectElemList = load.ApiLoad.EntityList;

        if (load.ReferenceType == ReferenceType.List) {
          if (load.ReferenceList == null
            && (load.ReferenceList.EntityType != EntityType.Element
            || load.ReferenceList.EntityType != EntityType.Member)) {
            owner.AddRuntimeWarning("Invalid List type for FaceLoad " + load.ToString()
              + Environment.NewLine + "Element list has not been set");
          }
          objectElemList +=
            Lists.GetElementOrMemberList(load.ReferenceList, ref model, owner);
          load.ApiLoad.EntityType = GsaList.GetAPIEntityType(load.ReferenceList.EntityType);
        } else {
          objectElemList += ElementListFromReference.GetReferenceDefinition(load, model);
        }

        if (objectElemList.Trim() != string.Empty) {
          load.ApiLoad.EntityList = objectElemList;
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

      model.Loads.Faces.Add(load.ApiLoad);
    }

    private static void ConvertFaceThermalLoad(GsaFaceThermalLoad load, ref ModelAssembly model, GH_Component owner) {
      PostHog.Load(load, load.ReferenceType);
      if (load.ReferenceType != ReferenceType.None) {
        string objectElemList = load.ApiLoad.EntityList;

        if (load.ReferenceType == ReferenceType.List) {
          if (load.ReferenceList == null
            && (load.ReferenceList.EntityType != EntityType.Element
            || load.ReferenceList.EntityType != EntityType.Member)) {
            owner.AddRuntimeWarning("Invalid List type for FaceThermalLoads " + load.ToString()
              + Environment.NewLine + "Element list has not been set");
          }
          objectElemList +=
            Lists.GetElementOrMemberList(load.ReferenceList, ref model, owner);
          load.ApiLoad.EntityType = GsaList.GetAPIEntityType(load.ReferenceList.EntityType);
        } else {
          objectElemList += ElementListFromReference.GetReferenceDefinition(load, model);
        }

        if (objectElemList.Trim() != string.Empty) {
          load.ApiLoad.EntityList = objectElemList;
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

      model.Loads.FaceThermals.Add(load.ApiLoad);
    }
  }
}
