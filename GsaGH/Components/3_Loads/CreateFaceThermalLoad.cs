using System;
using System.Collections.Generic;
using System.Drawing;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using GsaGH.Helpers.GH;
using GsaGH.Parameters;
using GsaGH.Properties;
using OasysGH;
using OasysGH.Components;
using OasysGH.Helpers;
using OasysGH.Units;
using OasysGH.Units.Helpers;
using OasysUnits;
using OasysUnits.Units;
using EntityType = GsaGH.Parameters.EntityType;

namespace GsaGH.Components {
  public class CreateFaceThermalLoad : GH_OasysDropDownComponent {
    private enum FoldMode {
      Uniform,
      //Gradient,
      //General
    }

    public override Guid ComponentGuid => new Guid("0e3fc316-c1cc-4f35-801a-695ad905dc59");
    public override GH_Exposure Exposure => GH_Exposure.secondary;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    protected override Bitmap Icon => Resources.CreateFaceThermalLoad;
    private readonly List<string> _loadTypeOptions = new List<string>(new[] {
      "Uniform",
      //"Gradient",
      //"General"
    });
    private TemperatureUnit _temperatureUnit = DefaultUnits.TemperatureUnit;
    private FoldMode _mode = FoldMode.Uniform;

    public CreateFaceThermalLoad() : base("Create Face Thermal Load", "FaceLoad", "Create GSA Face Thermal Load",
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
        _temperatureUnit = (TemperatureUnit)UnitsHelper.Parse(typeof(TemperatureUnit), _selectedItems[1]);
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
      pManager.AddGenericParameter("Element list", "G2D",
        "List, Custom Material, 2D Property, 2D Elements or 2D Members to apply load to; either input Prop2d, Element2d, or Member2d, or a text string."
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
      pManager.AddParameter(new GsaLoadParameter(), "Face Thermal Load", "Ld", "GSA Face Thermal Load", GH_ParamAccess.item);
    }

    protected override void SolveInstance(IGH_DataAccess da) {
      var faceThermalLoad = new GsaFaceThermalLoad();

      var loadcase = new GsaLoadCase(1);
      GsaLoadCaseGoo loadCaseGoo = null;
      if (da.GetData(0, ref loadCaseGoo)) {
        loadcase = loadCaseGoo.Value;
      }

      faceThermalLoad.LoadCase = loadcase;

      var ghTyp = new GH_ObjectWrapper();
      if (da.GetData(1, ref ghTyp)) {
        switch (ghTyp.Value) {
          case GsaListGoo value: {
              if (value.Value.EntityType == EntityType.Element
                || value.Value.EntityType == EntityType.Member) {
                faceThermalLoad.ReferenceList = value.Value;
                faceThermalLoad.ReferenceType = ReferenceType.List;
              } else {
                this.AddRuntimeWarning(
                  "List must be of type Element or Member to apply to face loading");
              }

              if (value.Value.EntityType == EntityType.Member) {
                this.AddRuntimeRemark(
                  "Member list applied to loading in GsaGH will automatically find child elements created from parent member with the load still being applied to elements." + Environment.NewLine + "If you save the file and continue working in GSA please note that the member-loading relationship will be lost.");
              }

              break;
            }
          case GsaElement2dGoo value: {
              faceThermalLoad.RefObjectGuid = value.Value.Guid;
              faceThermalLoad.ReferenceType = ReferenceType.Element;
              break;
            }
          case GsaMember2dGoo value: {
              faceThermalLoad.RefObjectGuid = value.Value.Guid;
              faceThermalLoad.ReferenceType = ReferenceType.MemberChildElements;
              this.AddRuntimeRemark(
                "Member loading in GsaGH will automatically find child elements created from parent member with the load still being applied to elements." + Environment.NewLine + "If you save the file and continue working in GSA please note that the member-loading relationship will be lost.");

              break;
            }
          case GsaMaterialGoo value: {
              if (value.Value.Id != 0) {
                this.AddRuntimeWarning(
                "Reference Material must be a Custom Material");
                return;
              }
              faceThermalLoad.RefObjectGuid = value.Value.Guid;
              faceThermalLoad.ReferenceType = ReferenceType.Property;
              break;
            }
          case GsaProperty2dGoo value: {
              faceThermalLoad.RefObjectGuid = value.Value.Guid;
              faceThermalLoad.ReferenceType = ReferenceType.Property;
              break;
            }
          default: {
              if (GH_Convert.ToString(ghTyp.Value, out string elemList, GH_Conversion.Both)) {
                faceThermalLoad.FaceThermalLoad.EntityList = elemList;
              }

              break;
            }
        }
      }

      var ghName = new GH_String();
      if (da.GetData(2, ref ghName)) {
        if (GH_Convert.ToString(ghName, out string name, GH_Conversion.Both)) {
          faceThermalLoad.FaceThermalLoad.Name = name;
        }
      }

      var temperature = (Temperature)Input.UnitNumber(this, da, 3, _temperatureUnit);
      switch (_mode) {
        case FoldMode.Uniform:
          if (_mode == FoldMode.Uniform) {
            faceThermalLoad.FaceThermalLoad.UniformTemperature = temperature.DegreesCelsius;
          }
          break;
      }

      da.SetData(0, new GsaLoadGoo(faceThermalLoad));
    }

    protected override void UpdateUIFromSelectedItems() {
      _mode = (FoldMode)Enum.Parse(typeof(FoldMode), _selectedItems[0]);

      _temperatureUnit = (TemperatureUnit)UnitsHelper.Parse(typeof(TemperatureUnit), _selectedItems[1]);
      base.UpdateUIFromSelectedItems();
    }
  }
}
