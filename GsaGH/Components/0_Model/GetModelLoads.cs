﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using Grasshopper.Kernel;
using GsaGH.Helpers.GH;
using GsaGH.Helpers.Import;
using GsaGH.Parameters;
using GsaGH.Properties;
using OasysGH;
using OasysGH.Components;
using OasysGH.Units;
using OasysGH.Units.Helpers;
using OasysUnits;
using LengthUnit = OasysUnits.Units.LengthUnit;

namespace GsaGH.Components {
  /// <summary>
  ///   Component to retrieve non-geometric objects from a GSA model
  /// </summary>
  public class GetModelLoads : GH_OasysDropDownComponent {
    public override Guid ComponentGuid => new Guid("97f5f7ac-4e3f-410e-9d47-c4b85caa307e");
    public override GH_Exposure Exposure => GH_Exposure.secondary | GH_Exposure.obscure;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    protected override Bitmap Icon => Resources.GetModelLoads;
    private LengthUnit _lengthUnit = DefaultUnits.LengthUnitGeometry;

    public GetModelLoads() : base("Get Model Loads", "GetLoads",
      "Get Loads and Grid Planes/Surfaces from GSA model", CategoryName.Name(),
      SubCategoryName.Cat0()) {
      Hidden = true;
    }

    public override void SetSelected(int i, int j) {
      _selectedItems[i] = _dropDownItems[i][j];
      _lengthUnit = (LengthUnit)UnitsHelper.Parse(typeof(LengthUnit), _selectedItems[i]);
      base.UpdateUI();
    }

    public override void VariableParameterMaintenance() {
      string unitAbbreviation = Length.GetAbbreviation(_lengthUnit);
      int i = 4;
      Params.Output[i++].Name = "Grid Point Loads [" + unitAbbreviation + "]";
      Params.Output[i++].Name = "Grid Line Loads [" + unitAbbreviation + "]";
      Params.Output[i++].Name = "Grid Area Loads [" + unitAbbreviation + "]";
      Params.Output[i].Name = "Grid Plane Surfaces [" + unitAbbreviation + "]";
    }

    protected override void InitialiseDropdowns() {
      _spacerDescriptions = new List<string>(new[] {
        "Unit",
      });

      _dropDownItems = new List<List<string>>();
      _selectedItems = new List<string>();

      _dropDownItems.Add(UnitsHelper.GetFilteredAbbreviations(EngineeringUnits.Length));
      _selectedItems.Add(Length.GetAbbreviation(_lengthUnit));

      _isInitialised = true;
    }

    protected override void RegisterInputParams(GH_InputParamManager pManager) {
      pManager.AddParameter(new GsaModelParameter(), "GSA Model", "GSA",
        "GSA model containing some loads", GH_ParamAccess.item);
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager) {
      string unitAbbreviation = Length.GetAbbreviation(_lengthUnit);
      pManager.AddParameter(new GsaLoadCaseParameter(), "Load Cases", "LC",
        "Load Cases from GSA Model", GH_ParamAccess.list);
      pManager.AddParameter(new GsaLoadParameter(), "Gravity Loads", "Gr",
        "Gravity Loads from GSA Model", GH_ParamAccess.list);
      pManager.AddParameter(new GsaLoadParameter(), "Node Loads", "No", "Node Loads from GSA Model",
        GH_ParamAccess.list);
      pManager.AddParameter(new GsaLoadParameter(), "Beam Loads", "Be", "Beam Loads from GSA Model",
        GH_ParamAccess.list);
      pManager.AddParameter(new GsaLoadParameter(), "Face Loads", "Fa", "Face Loads from GSA Model",
        GH_ParamAccess.list);
      pManager.AddParameter(new GsaLoadParameter(), "Grid Point Loads [" + unitAbbreviation + "]",
        "Pt", "Grid Point Loads from GSA Model", GH_ParamAccess.list);
      pManager.AddParameter(new GsaLoadParameter(), "Grid Line Loads [" + unitAbbreviation + "]",
        "Ln", "Grid Line Loads from GSA Model", GH_ParamAccess.list);
      pManager.AddParameter(new GsaLoadParameter(), "Grid Area Loads [" + unitAbbreviation + "]",
        "Ar", "Grid Area Loads from GSA Model", GH_ParamAccess.list);
      pManager.AddParameter(new GsaGridPlaneSurfaceParameter(),
        "Grid Plane Surfaces [" + unitAbbreviation + "]", "GPS",
        "Grid Plane Surfaces from GSA Model", GH_ParamAccess.list);
      pManager.HideParameter(7);
    }

    protected override void SolveInstance(IGH_DataAccess da) {
      GsaModelGoo modelGoo = null;
      da.GetData(0, ref modelGoo);

      GsaAPI.Model model = modelGoo.Value.Model;
      ReadOnlyDictionary<int, GsaAPI.LoadCase> loadCases = model.LoadCases();
      List<GsaLoadCaseGoo> cases = Loads.GetLoadCases(loadCases);
      List<GsaLoadGoo> gravity = Loads.GetGravityLoads(model.GravityLoads(), loadCases);
      List<GsaLoadGoo> node = Loads.GetNodeLoads(model, loadCases);
      List<GsaLoadGoo> beam = Loads.GetBeamLoads(model.BeamLoads(), loadCases);
      List<GsaLoadGoo> beamThermal = Loads.GetBeamThermalLoads(model.BeamThermalLoads(), loadCases);
      List<GsaLoadGoo> face = Loads.GetFaceLoads(model.FaceLoads(), loadCases);
      List<GsaLoadGoo> faceThermal = Loads.GetFaceThermalLoads(model.FaceThermalLoads(), loadCases);

      IReadOnlyDictionary<int, GsaAPI.GridSurface> srfDict = model.GridSurfaces();
      IReadOnlyDictionary<int, GsaAPI.GridPlane> plnDict = model.GridPlanes();
      IReadOnlyDictionary<int, GsaAPI.Axis> axDict = model.Axes();
      List<GsaLoadGoo> point = Loads.GetGridPointLoads(
        model.GridPointLoads(), srfDict, plnDict, axDict, loadCases, _lengthUnit);
      List<GsaLoadGoo> line = Loads.GetGridLineLoads(
        model.GridLineLoads(), srfDict, plnDict, axDict, loadCases, _lengthUnit);
      List<GsaLoadGoo> area = Loads.GetGridAreaLoads(
        model.GridAreaLoads(), srfDict, plnDict, axDict, loadCases, _lengthUnit);

      var gps = srfDict.Keys.Select(key
        => new GsaGridPlaneSurfaceGoo(Loads.GetGridPlaneSurface(srfDict, plnDict, axDict, key,
          _lengthUnit))).ToList();

      da.SetDataList(0, cases);
      da.SetDataList(1, gravity);
      da.SetDataList(2, node);
      da.SetDataList(3, beam);
      da.SetDataList(4, face);
      da.SetDataList(5, point);
      da.SetDataList(6, line);
      da.SetDataList(7, area);
      da.SetDataList(8, gps);
    }

    protected override void UpdateUIFromSelectedItems() {
      _lengthUnit = (LengthUnit)UnitsHelper.Parse(typeof(LengthUnit), _selectedItems[0]);
      base.UpdateUIFromSelectedItems();
    }
  }
}