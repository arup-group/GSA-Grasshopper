using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Grasshopper.Kernel;
using GsaAPI;
using GsaGH.Helpers.GH;
using GsaGH.Helpers.Import;
using GsaGH.Parameters;
using GsaGH.Properties;
using OasysGH;
using OasysGH.Components;
using OasysGH.Units;
using OasysGH.Units.Helpers;
using OasysUnits;
using OasysUnits.Units;

namespace GsaGH.Components {
  /// <summary>
  ///   Component to retrieve non-geometric objects from a GSA model
  /// </summary>
  public class GetLoads : GH_OasysDropDownComponent {
    public override Guid ComponentGuid => new Guid("87ff28e5-a1a6-4d78-ba71-e930e01dca13");
    public override GH_Exposure Exposure => GH_Exposure.secondary | GH_Exposure.obscure;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    public GetLoads() : base("Get Model Loads",
      "GetLoads",
      "Get Loads and Grid Planes/Surfaces from GSA model",
      CategoryName.Name(),
      SubCategoryName.Cat0())
      => Hidden = true;

    public override void SetSelected(int i, int j) {
      _selectedItems[i] = _dropDownItems[i][j];
      _lengthUnit = (LengthUnit)UnitsHelper.Parse(typeof(LengthUnit), _selectedItems[i]);
      base.UpdateUI();
    }

    public override void VariableParameterMaintenance() {
      string unitAbbreviation = Length.GetAbbreviation(_lengthUnit);
      int i = 4;
      Params.Output[i++]
        .Name = "Grid Point Loads [" + unitAbbreviation + "]";
      Params.Output[i++]
        .Name = "Grid Line Loads [" + unitAbbreviation + "]";
      Params.Output[i++]
        .Name = "Grid Area Loads [" + unitAbbreviation + "]";
      Params.Output[i]
        .Name = "Grid Plane Surfaces [" + unitAbbreviation + "]";
    }

    protected override Bitmap Icon => Resources.GetLoads;
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

    protected override void RegisterInputParams(GH_InputParamManager pManager)
      => pManager.AddParameter(new GsaModelParameter(),
        "GSA Model",
        "GSA",
        "GSA model containing some loads",
        GH_ParamAccess.item);

    protected override void RegisterOutputParams(GH_OutputParamManager pManager) {
      string unitAbbreviation = Length.GetAbbreviation(_lengthUnit);

      pManager.AddParameter(new GsaLoadParameter(),
        "Gravity Loads",
        "Gr",
        "Gravity Loads from GSA Model",
        GH_ParamAccess.list);
      pManager.AddParameter(new GsaLoadParameter(),
        "Node Loads",
        "No",
        "Node Loads from GSA Model",
        GH_ParamAccess.list);
      pManager.AddParameter(new GsaLoadParameter(),
        "Beam Loads",
        "Be",
        "Beam Loads from GSA Model",
        GH_ParamAccess.list);
      pManager.AddParameter(new GsaLoadParameter(),
        "Face Loads",
        "Fa",
        "Face Loads from GSA Model",
        GH_ParamAccess.list);
      pManager.AddParameter(new GsaLoadParameter(),
        "Grid Point Loads [" + unitAbbreviation + "]",
        "Pt",
        "Grid Point Loads from GSA Model",
        GH_ParamAccess.list);
      pManager.AddParameter(new GsaLoadParameter(),
        "Grid Line Loads [" + unitAbbreviation + "]",
        "Ln",
        "Grid Line Loads from GSA Model",
        GH_ParamAccess.list);
      pManager.AddParameter(new GsaLoadParameter(),
        "Grid Area Loads [" + unitAbbreviation + "]",
        "Ar",
        "Grid Area Loads from GSA Model",
        GH_ParamAccess.list);
      pManager.AddParameter(new GsaGridPlaneParameter(),
        "Grid Plane Surfaces [" + unitAbbreviation + "]",
        "GPS",
        "Grid Plane Surfaces from GSA Model",
        GH_ParamAccess.list);
      pManager.HideParameter(7);
    }

    protected override void SolveInstance(IGH_DataAccess da) {
      var gsaModel = new GsaModel();
      if (!da.GetData(0, ref gsaModel))
        return;

      Model model = gsaModel.Model;

      List<GsaLoadGoo> gravity = Loads.GetGravityLoads(model.GravityLoads());
      List<GsaLoadGoo> node = Loads.GetNodeLoads(model);
      List<GsaLoadGoo> beam = Loads.GetBeamLoads(model.BeamLoads());
      List<GsaLoadGoo> face = Loads.GetFaceLoads(model.FaceLoads());

      IReadOnlyDictionary<int, GridSurface> srfDict = model.GridSurfaces();
      IReadOnlyDictionary<int, GridPlane> plnDict = model.GridPlanes();
      IReadOnlyDictionary<int, Axis> axDict = model.Axes();
      List<GsaLoadGoo> point
        = Loads.GetGridPointLoads(model.GridPointLoads(), srfDict, plnDict, axDict, _lengthUnit);
      List<GsaLoadGoo> line
        = Loads.GetGridLineLoads(model.GridLineLoads(), srfDict, plnDict, axDict, _lengthUnit);
      List<GsaLoadGoo> area
        = Loads.GetGridAreaLoads(model.GridAreaLoads(), srfDict, plnDict, axDict, _lengthUnit);

      var gps = srfDict.Keys.Select(key
          => new GsaGridPlaneSurfaceGoo(Loads.GetGridPlaneSurface(srfDict,
            plnDict,
            axDict,
            key,
            _lengthUnit)))
        .ToList();

      da.SetDataList(0, gravity);
      da.SetDataList(1, node);
      da.SetDataList(2, beam);
      da.SetDataList(3, face);
      da.SetDataList(4, point);
      da.SetDataList(5, line);
      da.SetDataList(6, area);
      da.SetDataList(7, gps);
    }

    protected override void UpdateUIFromSelectedItems() {
      _lengthUnit = (LengthUnit)UnitsHelper.Parse(typeof(LengthUnit), _selectedItems[0]);
      base.UpdateUIFromSelectedItems();
    }

    private LengthUnit _lengthUnit = DefaultUnits.LengthUnitGeometry;
  }
}
