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
  public class CreateBeamThermalLoad : GH_OasysDropDownComponent {
    private enum FoldMode {
      Uniform,
      //GradientInY,
      //GradientInZ
    }

    public override Guid ComponentGuid => new Guid("efd3c9a5-3bd6-47c1-aedd-510e02c01cf9");
    public override GH_Exposure Exposure => GH_Exposure.secondary;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    protected override Bitmap Icon => Resources.BeamLoad;
    private readonly List<string> _loadTypeOptions = new List<string>(new[] {
      "Uniform",
      //"GradientInY",
      //"GradientInZ"
    });
    private bool _duringLoad;
    private TemperatureUnit _temperatureUnit = DefaultUnits.TemperatureUnit;
    private FoldMode _mode = FoldMode.Uniform;
    private EntityType _entityType = EntityType.Member;

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
      } else if (i == 1) {
        switch (_selectedItems[1]) {
          case "Element":
            _entityType = EntityType.Element;
            break;

          case "Member":
            _entityType = EntityType.Member;
            break;
        }
      } else {
        _temperatureUnit
          = (TemperatureUnit)UnitsHelper.Parse(typeof(TemperatureUnit), _selectedItems[2]);
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
        "EntityType",
        "Unit",
      });

      _dropDownItems = new List<List<string>>();
      _selectedItems = new List<string>();

      _dropDownItems.Add(_loadTypeOptions);
      _selectedItems.Add(_mode.ToString());

      _dropDownItems.Add(new List<string>(new[] {
        "Element",
        "Member"
      }));
      _selectedItems.Add(_entityType.ToString());

      _dropDownItems.Add(UnitsHelper.GetFilteredAbbreviations(EngineeringUnits.Temperature));
      _selectedItems.Add(Temperature.GetAbbreviation(_temperatureUnit));

      _isInitialised = true;
    }

    protected override void RegisterInputParams(GH_InputParamManager pManager) {
      string unitAbbreviation = Temperature.GetAbbreviation(_temperatureUnit);

      pManager.AddParameter(new GsaLoadCaseParameter());
      pManager.AddGenericParameter("Element list", "G1D",
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
      pManager[3].Optional = true;
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager) {
      pManager.AddParameter(new GsaLoadParameter(), "Beam Thermal Load", "Ld",
        "GSA Beam Thermal Load", GH_ParamAccess.item);
    }

    protected override void SolveInstance(IGH_DataAccess da) {
      var beamThermalLoad = new GsaBeamThermalLoad();

      GsaLoadCaseGoo loadCaseGoo = null;
      da.GetData(0, ref loadCaseGoo);
      beamThermalLoad.LoadCase = loadCaseGoo.IsValid ? loadCaseGoo.Value : new GsaLoadCase(1);

      if (_entityType == EntityType.Element) {
        beamThermalLoad.ReferenceType = ReferenceType.Element;
        beamThermalLoad.BeamThermalLoad.EntityType = GsaAPI.EntityType.Element;
      } else if (_entityType == EntityType.Member) {
        beamThermalLoad.ReferenceType = ReferenceType.Member;
        beamThermalLoad.BeamThermalLoad.EntityType = GsaAPI.EntityType.Member;
      } else {
        throw new ArgumentException("Entity type " + _entityType.ToString() + " not supported.");
      }

      var ghTyp = new GH_ObjectWrapper();
      if (da.GetData(1, ref ghTyp)) {
        switch (ghTyp.Value) {
          case GsaListGoo listGoo:
            if (listGoo.Value.EntityType == EntityType.Element
              || listGoo.Value.EntityType == EntityType.Member) {
              beamThermalLoad.ReferenceList = listGoo.Value;
              beamThermalLoad.ReferenceType = ReferenceType.List;
            } else {
              this.AddRuntimeWarning(
                "List must be of type Element or Member to apply to beam loading");
            }

            if (listGoo.Value.EntityType == EntityType.Member) {
              this.AddRuntimeRemark(
                "Member list applied to loading in GsaGH will automatically find child elements created from parent member with the load still being applied to elements." + Environment.NewLine + "If you save the file and continue working in GSA please note that the member-loading relationship will be lost.");
            }
            break;


          case GsaElement1dGoo element1dGoo:
            if (_entityType != EntityType.Element) {
              this.AddRuntimeWarning("Beam loads can only be applied to elements matching the selected enttiy type.");
              break;
            }
            beamThermalLoad.RefObjectGuid = element1dGoo.Value.Guid;
            beamThermalLoad.BeamThermalLoad.EntityType = GsaAPI.EntityType.Element;
            break;

          case GsaMember1dGoo member1dGoo:
            if (_entityType != EntityType.Member) {
              this.AddRuntimeError("Beam loads can only be applied to members matching the selected enttiy type.");
              return;
            }
            beamThermalLoad.RefObjectGuid = member1dGoo.Value.Guid;
            beamThermalLoad.BeamThermalLoad.EntityType = GsaAPI.EntityType.Member;
            break;

          case GsaMaterialGoo materialGoo:
            if (materialGoo.Value.Id != 0) {
              this.AddRuntimeWarning(
              "Reference Material must be a Custom Material");
              return;
            }
            beamThermalLoad.RefObjectGuid = materialGoo.Value.Guid;
            beamThermalLoad.ReferenceType = ReferenceType.Property;
            break;

          case GsaSectionGoo sectionGoo:
            beamThermalLoad.RefObjectGuid = sectionGoo.Value.Guid;
            beamThermalLoad.ReferenceType = ReferenceType.Property;
            break;

          default:
            if (GH_Convert.ToString(ghTyp.Value, out string beamList, GH_Conversion.Both)) {
              beamThermalLoad.BeamThermalLoad.EntityList = beamList;
            }
            break;
        }
      }

      var ghName = new GH_String();
      if (da.GetData(2, ref ghName)) {
        if (GH_Convert.ToString(ghName, out string name, GH_Conversion.Both)) {
          beamThermalLoad.BeamThermalLoad.Name = name;
        }
      }

      var temperature = (Temperature)Input.UnitNumber(this, da, 3, _temperatureUnit);
      switch (_mode) {
        case FoldMode.Uniform:
          if (_mode == FoldMode.Uniform) {
            beamThermalLoad.BeamThermalLoad.UniformTemperature = temperature.DegreesCelsius;
          }
          break;
      }

      da.SetData(0, new GsaLoadGoo(beamThermalLoad));
    }

    protected override void UpdateUIFromSelectedItems() {
      _mode = (FoldMode)Enum.Parse(typeof(FoldMode), _selectedItems[0]);
      _duringLoad = true;
      switch (_selectedItems[0]) {
        case "Uniform":
          //Mode1Clicked();
          break;
      }
      _duringLoad = false;

      _entityType = (EntityType)Enum.Parse(typeof(EntityType), _selectedItems[1]);

      _temperatureUnit
        = (TemperatureUnit)UnitsHelper.Parse(typeof(TemperatureUnit), _selectedItems[2]);
      base.UpdateUIFromSelectedItems();
    }
  }
}
