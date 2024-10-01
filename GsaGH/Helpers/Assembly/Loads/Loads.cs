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

namespace GsaGH.Helpers.Assembly {
  internal partial class ModelAssembly {
    private List<GravityLoad> _gravityLoads = new List<GravityLoad>();
    private List<BeamLoad> _beamLoads = new List<BeamLoad>();
    private List<BeamThermalLoad> _beamThermalLoads = new List<BeamThermalLoad>();
    private List<FaceLoad> _faceLoads = new List<FaceLoad>();
    private List<FaceThermalLoad> _faceThermalLoads = new List<FaceThermalLoad>();
    private List<GridPointLoad> _gridPointLoads = new List<GridPointLoad>();
    private List<GridLineLoad> _gridLineLoads = new List<GridLineLoad>();
    private List<GridAreaLoad> _gridAreaLoads = new List<GridAreaLoad>();
    private Dictionary<int, LoadCase> _loadCases;

    private void ConvertBeamLoad(GsaBeamLoad load, GH_Component owner) {
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
          objectElemList += " " + GetElementOrMemberList(load.ReferenceList, owner);
          load.ApiLoad.EntityType = GsaList.GetApiEntityType(load.ReferenceList.EntityType);
        } else {
          objectElemList += " " + GetLoadReferenceDefinition(load);
        }

        if (objectElemList.Trim() != string.Empty) {
          load.ApiLoad.EntityList = objectElemList.Trim();
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

      foreach (EntityList list in _lists.ReadOnlyDictionary.Values) {
        if (load.ApiLoad.EntityList == $"\"{list.Name}\"") {
          load.ApiLoad.EntityType = list.Type;
        }
      }

      _beamLoads.Add(load.ApiLoad);
    }

    private void ConvertBeamThermalLoad(GsaBeamThermalLoad load, GH_Component owner) {
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
          objectElemList += " " + GetElementOrMemberList(load.ReferenceList, owner);
          load.ApiLoad.EntityType = GsaList.GetApiEntityType(load.ReferenceList.EntityType);
        } else {
          objectElemList += " " + GetLoadReferenceDefinition(load);
        }

        if (objectElemList.Trim() != string.Empty) {
          load.ApiLoad.EntityList = objectElemList.Trim();
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

      foreach (EntityList list in _lists.ReadOnlyDictionary.Values) {
        if (load.ApiLoad.EntityList == $"\"{list.Name}\"") {
          load.ApiLoad.EntityType = list.Type;
        }
      }

      _beamThermalLoads.Add(load.ApiLoad);
    }

    private void ConvertFaceLoad(GsaFaceLoad load, GH_Component owner) {
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
          objectElemList += " " + GetElementOrMemberList(load.ReferenceList, owner);
          load.ApiLoad.EntityType = GsaList.GetApiEntityType(load.ReferenceList.EntityType);
        } else {
          objectElemList += " " + GetLoadReferenceDefinition(load);
        }

        if (objectElemList.Trim() != string.Empty) {
          load.ApiLoad.EntityList = objectElemList.Trim();
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


      foreach (EntityList list in _lists.ReadOnlyDictionary.Values) {
        if (load.ApiLoad.EntityList == $"\"{list.Name}\"") {
          load.ApiLoad.EntityType = list.Type;
        }
      }

      _faceLoads.Add(load.ApiLoad);
    }

    private void ConvertFaceThermalLoad(GsaFaceThermalLoad load, GH_Component owner) {
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
          objectElemList += " " + GetElementOrMemberList(load.ReferenceList, owner);
          load.ApiLoad.EntityType = GsaList.GetApiEntityType(load.ReferenceList.EntityType);
        } else {
          objectElemList += " " + GetLoadReferenceDefinition(load);
        }

        if (objectElemList.Trim() != string.Empty) {
          load.ApiLoad.EntityList = objectElemList.Trim();
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

      foreach (EntityList list in _lists.ReadOnlyDictionary.Values) {
        if (load.ApiLoad.EntityList == $"\"{list.Name}\"") {
          load.ApiLoad.EntityType = list.Type;
        }
      }

      _faceThermalLoads.Add(load.ApiLoad);
    }

    private void ConvertGravityLoad(GsaGravityLoad load, GH_Component owner) {
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
          objectElemList += " " + GetElementOrMemberList(load.ReferenceList, owner);
          load.ApiLoad.EntityType = GsaList.GetApiEntityType(load.ReferenceList.EntityType);
        } else {
          objectElemList += " " + GetLoadReferenceDefinition(load);
          objectElemList = objectElemList.Trim();
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

      foreach (EntityList list in _lists.ReadOnlyDictionary.Values) {
        if (load.ApiLoad.EntityList == $"\"{list.Name}\"") {
          load.ApiLoad.EntityType = list.Type;
        }
      }

      _gravityLoads.Add(load.ApiLoad);
    }

    private void ConvertGridAreaLoad(GsaGridAreaLoad load, GH_Component owner) {
      PostHog.Load(load, ReferenceType.None,
        load.ApiLoad.Type.ToString());
      if (load.ApiLoad.Type == GridAreaPolyLineType.POLYGON) {
        load.ApiLoad.PolyLineDefinition =
        GridLoadHelper.ClearDefinitionForUnit(load.ApiLoad.PolyLineDefinition) +
        $"({Length.GetAbbreviation(_unit)})";
      }

      if (load.ApiLoad.PolyLineReference > 0) {
        _model.AddPolyline(load.ApiPolyline);
      }

      if (load.GridPlaneSurface == null) {
        _gridAreaLoads.Add(load.ApiLoad);
        return;
      }

      GsaGridPlaneSurface gridplnsrf = load.GridPlaneSurface;

      if (gridplnsrf.GridPlane != null) {
        load.ApiLoad.GridSurface = ConvertGridPlaneSurface(gridplnsrf, owner);
      }

      _gridAreaLoads.Add(load.ApiLoad);
    }

    private void ConvertGridLineLoad(GsaGridLineLoad load, GH_Component owner) {
      PostHog.Load(load, ReferenceType.None);
      if (load.GridPlaneSurface == null) {
        _gridLineLoads.Add(load.ApiLoad);
        return;
      }

      if (_unit != LengthUnit.Meter
        && load.ApiLoad.Type == GridLineLoad.PolyLineType.EXPLICIT_POLYLINE) {
        load.ApiLoad.PolyLineDefinition =
          GridLoadHelper.ClearDefinitionForUnit(load.ApiLoad.PolyLineDefinition) +
          $"({Length.GetAbbreviation(_unit)})";
      }

      if (load.ApiLoad.PolyLineReference > 0) {
        _model.AddPolyline(load.ApiPolyline);
      }

      GsaGridPlaneSurface gridplnsrf = load.GridPlaneSurface;

      if (gridplnsrf.GridPlane != null) {
        load.ApiLoad.GridSurface = ConvertGridPlaneSurface(gridplnsrf, owner);
      }

      _gridLineLoads.Add(load.ApiLoad);
    }

    private void ConvertGridPointLoad(GsaGridPointLoad load, GH_Component owner) {
      PostHog.Load(load, ReferenceType.None);
      if (load.GridPlaneSurface == null) {
        _gridPointLoads.Add(load.ApiLoad);
        return;
      }

      var gridptref = (GsaGridPointLoad)load.Duplicate();
      if (_unit != LengthUnit.Meter) {
        gridptref.ApiLoad.X = new Length(gridptref.ApiLoad.X, _unit).As(LengthUnit.Meter);
        gridptref.ApiLoad.Y = new Length(gridptref.ApiLoad.Y, _unit).As(LengthUnit.Meter);
      }

      GsaGridPlaneSurface gridplnsrf = gridptref.GridPlaneSurface;

      if (gridplnsrf.GridPlane != null) {
        gridptref.ApiLoad.GridSurface = ConvertGridPlaneSurface(gridplnsrf, owner);
      }

      _gridPointLoads.Add(gridptref.ApiLoad);
    }

    private void ConvertLoad(List<IGsaLoad> loads, GH_Component owner) {
      if (loads == null) {
        return;
      }

      foreach (IGsaLoad load in loads.Where(gsaLoad => gsaLoad != null)) {
        ConvertLoad(load.Duplicate(), owner);
      }
    }

    private void ConvertLoad(IGsaLoad load, GH_Component owner) {
      ConvertLoadCase(load.LoadCase, owner);
      if (load.LoadCase != null) {
        load.CaseId = load.LoadCase.Id;
      }

      switch (load) {
        case GsaGravityLoad gravity:
          ConvertGravityLoad(gravity, owner);
          break;

        case GsaBeamLoad beam:
          ConvertBeamLoad(beam, owner);
          break;

        case GsaBeamThermalLoad beamThermal:
          ConvertBeamThermalLoad(beamThermal, owner);
          break;

        case GsaFaceLoad face:
          ConvertFaceLoad(face, owner);
          break;

        case GsaFaceThermalLoad faceThermal:
          ConvertFaceThermalLoad(faceThermal, owner);
          break;

        case GsaGridPointLoad point:
          ConvertGridPointLoad(point, owner);
          break;

        case GsaGridLineLoad line:
          ConvertGridLineLoad(line, owner);
          break;

        case GsaGridAreaLoad area:
          ConvertGridAreaLoad(area, owner);
          break;
      }
    }

    private void ConvertLoadCase(GsaLoadCase loadCase, GH_Component owner) {
      if (loadCase == null || loadCase.LoadCase == null) {
        return;
      }

      if (_loadCases.ContainsKey(loadCase.Id)) {
        LoadCase existingCase = _loadCases[loadCase.Id];
        LoadCase newCase = loadCase.LoadCase;
        if (newCase.CaseType != existingCase.CaseType || newCase.Name != existingCase.Name) {
          _loadCases[loadCase.Id] = newCase;

          owner?.AddRuntimeRemark($"LoadCase {loadCase.Id} either already existed in the model " +
           $"or two load cases with ID:{loadCase.Id} were added.{Environment.NewLine}" +
           $"{newCase.Name} - {newCase.CaseType} replaced previous LoadCase");
        }

        return;
      }

      _loadCases.Add(loadCase.Id, loadCase.LoadCase);
    }

    private void GetLoadCasesFromModel(Model model) {
      _loadCases = model.LoadCases().ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
      foreach (int key in _loadCases.Keys) {
        // some old gwb files stores Dead as (int)0:
        if (_loadCases[key].CaseType == 0) {
          _loadCases[key].CaseType = GsaAPI.LoadCaseType.Dead;
        }
      }
    }
  }
}
