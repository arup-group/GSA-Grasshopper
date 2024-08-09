using System;
using System.Collections.Generic;
using System.Drawing;

using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;

using GsaGH.Helpers.GH;
using GsaGH.Parameters;
using GsaGH.Parameters.Enums;
using GsaGH.Properties;

using OasysGH;
using OasysGH.Components;
using OasysGH.Helpers;
using OasysGH.Units;
using OasysGH.Units.Helpers;

using OasysUnits;

using EntityType = GsaGH.Parameters.EntityType;
using TemperatureUnit = OasysUnits.Units.TemperatureUnit;

namespace GsaGH.Components {
  public class CreateBeamThermalLoad : GH_OasysDropDownComponent {
    private enum FoldMode {
      Uniform,
      //GradientInY,
      //GradientInZ
    }

    public override Guid ComponentGuid => new Guid("efd3c9a5-3bd6-47c1-aedd-510e02c01cf9");
    public override GH_Exposure Exposure => GH_Exposure.secondary | GH_Exposure.obscure;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    protected override Bitmap Icon => Resources.CreateBeamThermalLoad;
    private readonly List<string> _loadTypeOptions = new List<string>(new[] {
      "Uniform",
      //"GradientInY",
      //"GradientInZ"
    });
    private TemperatureUnit _temperatureUnit = DefaultUnits.TemperatureUnit;
    private FoldMode _mode = FoldMode.Uniform;

    public CreateBeamThermalLoad() : base("Create Beam Thermal Load", "BeamThermalLoad", "Create GSA Beam Thermal Load",
      CategoryName.Name(), SubCategoryName.Cat3()) {
      Hidden = true;
    }

    public override void SetSelected(int i, int j) {
      _selectedItems[i] = _dropDownItems[i][j];

      if (i == 0) {
        switch (_selectedItems[0]) {
          case "Uniform":
            //Mode1Clicked();
            break;
        }
      } else {
        _temperatureUnit
          = (TemperatureUnit)UnitsHelper.Parse(typeof(TemperatureUnit), _selectedItems[1]);
      }

      base.UpdateUI();
    }

    public override void VariableParameterMaintenance() {
      string unitAbbreviation = Temperature.GetAbbreviation(_temperatureUnit);

      switch (_mode) {
        case FoldMode.Uniform:
          Params.Input[3].NickName = "V";
          Params.Input[3].Name = "Value [" + unitAbbreviation + "]";
          Params.Input[3].Description = "Load Value";
          Params.Input[3].Access = GH_ParamAccess.item;
          Params.Input[3].Optional = false;
          break;
      }
    }

    protected override void InitialiseDropdowns() {
      _spacerDescriptions = new List<string>(new[] {
        "Type",
        "Unit",
      });

      _dropDownItems = new List<List<string>>();
      _selectedItems = new List<string>();

      _dropDownItems.Add(_loadTypeOptions);
      _selectedItems.Add(_mode.ToString());

      _dropDownItems.Add(UnitsHelper.GetFilteredAbbreviations(EngineeringUnits.Temperature));
      _selectedItems.Add(Temperature.GetAbbreviation(_temperatureUnit));

      _isInitialised = true;
    }

    protected override void RegisterInputParams(GH_InputParamManager pManager) {
      string unitAbbreviation = Temperature.GetAbbreviation(_temperatureUnit);

      pManager.AddParameter(new GsaLoadCaseParameter());
      pManager.AddGenericParameter("Loadable 1D Objects", "G1D",
        "List, Custom Material, Section, 1D Elements or 1D Members to apply load to; either input Section, Element1d, or Member1d, or a text string."
        + Environment.NewLine + "Text string with Element list should take the form:"
        + Environment.NewLine
        + " 1 11 to 20 step 2 P1 not (G1 to G6 step 3) P11 not (PA PB1 PS2 PM3 PA4 M1)"
        + Environment.NewLine
        + "Refer to GSA help file for definition of lists and full vocabulary.",
        GH_ParamAccess.item);
      pManager.AddTextParameter("Name", "Na", "Load Name", GH_ParamAccess.item);
      pManager.AddNumberParameter("Value [" + unitAbbreviation + "]", "V", "Load Value",
        GH_ParamAccess.item);

      pManager[0].Optional = true;
      pManager[2].Optional = true;
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager) {
      pManager.AddParameter(new GsaLoadParameter(), "Beam Thermal Load", "Ld",
        "GSA Beam Thermal Load", GH_ParamAccess.item);
    }

    protected override void SolveInternal(IGH_DataAccess da) {
      var beamThermalLoad = new GsaBeamThermalLoad();

      var loadcase = new GsaLoadCase(1);
      GsaLoadCaseGoo loadCaseGoo = null;
      if (da.GetData(0, ref loadCaseGoo)) {
        if (loadCaseGoo.Value != null) {
          loadcase = loadCaseGoo.Value;
        }
      }

      beamThermalLoad.LoadCase = loadcase;

      var ghTyp = new GH_ObjectWrapper();
      if (da.GetData(1, ref ghTyp)) {
        switch (ghTyp.Value) {
          case GsaListGoo listGoo:
            if (listGoo.Value.EntityType == EntityType.Element
              || listGoo.Value.EntityType == EntityType.Member) {
              beamThermalLoad.ReferenceList = listGoo.Value;
              beamThermalLoad.ReferenceType = ReferenceType.List;
            } else {
              this.AddRuntimeError(
                "List must be of type Element or Member to apply to beam loading");
              return;
            }

            break;

          case GsaElement1dGoo element1dGoo:
            beamThermalLoad.RefObjectGuid = element1dGoo.Value.Guid;
            beamThermalLoad.ApiLoad.EntityType = GsaAPI.EntityType.Element;
            beamThermalLoad.ReferenceType = ReferenceType.Element;
            break;

          case GsaMember1dGoo member1dGoo:
            beamThermalLoad.RefObjectGuid = member1dGoo.Value.Guid;
            beamThermalLoad.ApiLoad.EntityType = GsaAPI.EntityType.Member;
            beamThermalLoad.ReferenceType = ReferenceType.Member;
            break;

          case GsaMaterialGoo materialGoo:
            if (materialGoo.Value.Id != 0) {
              this.AddRuntimeWarning(
              "Reference Material must be a Custom Material");
              return;
            }
            beamThermalLoad.RefObjectGuid = materialGoo.Value.Guid;
            beamThermalLoad.ApiLoad.EntityType = GsaAPI.EntityType.Element;
            beamThermalLoad.ReferenceType = ReferenceType.Property;
            this.AddRuntimeRemark(
                "Load from Material reference created as Element load");
            break;

          case GsaSectionGoo sectionGoo:
            beamThermalLoad.RefObjectGuid = sectionGoo.Value.Guid;
            beamThermalLoad.ApiLoad.EntityType = GsaAPI.EntityType.Element;
            beamThermalLoad.ReferenceType = ReferenceType.Property;
            this.AddRuntimeRemark(
                "Load from Section reference created as Element load");
            break;

          default:
            if (GH_Convert.ToString(ghTyp.Value, out string beamList, GH_Conversion.Both)) {
              beamThermalLoad.ApiLoad.EntityType = GsaAPI.EntityType.Element;
              beamThermalLoad.ApiLoad.EntityList = beamList;
              if (beamThermalLoad.ApiLoad.EntityList != beamList && beamList.ToLower() != "all") {
                beamThermalLoad.ApiLoad.EntityList = $"\"{beamList}\"";
              }
            }
            break;
        }
      }

      var ghName = new GH_String();
      if (da.GetData(2, ref ghName)) {
        if (GH_Convert.ToString(ghName, out string name, GH_Conversion.Both)) {
          beamThermalLoad.ApiLoad.Name = name;
        }
      }

      var temperature = (Temperature)Input.UnitNumber(this, da, 3, _temperatureUnit);
      switch (_mode) {
        case FoldMode.Uniform:
          if (_mode == FoldMode.Uniform) {
            beamThermalLoad.ApiLoad.UniformTemperature = temperature.DegreesCelsius;
          }
          break;
      }

      da.SetData(0, new GsaLoadGoo(beamThermalLoad));
    }

    protected override void UpdateUIFromSelectedItems() {
      _mode = (FoldMode)Enum.Parse(typeof(FoldMode), _selectedItems[0]);

      _temperatureUnit
        = (TemperatureUnit)UnitsHelper.Parse(typeof(TemperatureUnit), _selectedItems[1]);
      base.UpdateUIFromSelectedItems();
    }
  }
}
