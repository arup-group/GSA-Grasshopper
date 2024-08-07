using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

using Grasshopper.Kernel;

using GsaAPI;

using GsaGH.Helpers.GH;
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
      Params.Output[5].Name = "Grid Point Loads [" + unitAbbreviation + "]";
      Params.Output[6].Name = "Grid Line Loads [" + unitAbbreviation + "]";
      Params.Output[7].Name = "Grid Area Loads [" + unitAbbreviation + "]";
      Params.Output[8].Name = "Grid Plane Surfaces [" + unitAbbreviation + "]";
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

    protected override void SolveInternal(IGH_DataAccess da) {
      GsaModelGoo modelGoo = null;
      da.GetData(0, ref modelGoo);

      Model model = modelGoo.Value.ApiModel;
      List<GsaLoadCaseGoo> cases = GsaLoadFactory.CreateLoadCasesFromApi(model);
      List<GsaLoadGoo> gravity = GsaLoadFactory.CreateGravityLoadsFromApi(model);
      List<GsaLoadGoo> node = GsaLoadFactory.CreateNodeLoadsFromApi(model);
      List<GsaLoadGoo> beam = GsaLoadFactory.CreateBeamLoadsFromApi(model);
      List<GsaLoadGoo> beamThermal = GsaLoadFactory.CreateBeamThermalLoadsFromApi(model);
      List<GsaLoadGoo> face = GsaLoadFactory.CreateFaceLoadsFromApi(model);
      List<GsaLoadGoo> faceThermal = GsaLoadFactory.CreateFaceThermalLoadsFromApi(model);
      List<GsaLoadGoo> point = GsaLoadFactory.CreateGridPointLoadsFromApi(model, _lengthUnit);
      List<GsaLoadGoo> line = GsaLoadFactory.CreateGridLineLoadsFromApi(model, _lengthUnit);
      List<GsaLoadGoo> area = GsaLoadFactory.CreateGridAreaLoadsFromApi(model, _lengthUnit);

      IReadOnlyDictionary<int, GridSurface> srfDict = model.GridSurfaces();
      var gps = srfDict.Keys.Select(key
        => new GsaGridPlaneSurfaceGoo(GsaLoadFactory.CreateGridPlaneSurfaceFromApi(model, key, _lengthUnit))).ToList();

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
