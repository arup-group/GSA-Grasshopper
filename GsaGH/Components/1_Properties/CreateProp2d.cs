using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Parameters;
using Grasshopper.Kernel.Types;
using GsaAPI;
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

namespace GsaGH.Components {
  /// <summary>
  ///   Component to create a new Prop2d
  /// </summary>
  public class CreateProp2d : GH_OasysDropDownComponent {
    protected override void SolveInstance(IGH_DataAccess da) {
      var prop = new GsaProp2d();
      switch (_mode) {
        case FoldMode.PlaneStress:
          prop.Type = Property2D_Type.PL_STRESS;
          break;

        case FoldMode.Fabric:
          prop.Type = Property2D_Type.FABRIC;
          break;

        case FoldMode.FlatPlate:
          prop.Type = Property2D_Type.PLATE;
          break;

        case FoldMode.Shell:
          prop.Type = Property2D_Type.SHELL;
          break;

        case FoldMode.CurvedShell:
          prop.Type = Property2D_Type.CURVED_SHELL;
          break;

        case FoldMode.LoadPanel:
          prop.Type = Property2D_Type.LOAD;
          break;

        default:
          prop.Type = Property2D_Type.UNDEF;
          break;
      }

      if (_mode != FoldMode.LoadPanel) {
        prop.AxisProperty = 0;

        if (_mode != FoldMode.Fabric) {
          prop.Thickness = (Length)Input.UnitNumber(this, da, 0, _lengthUnit);
          var ghTyp = new GH_ObjectWrapper();
          if (da.GetData(1, ref ghTyp)) {
            var material = new GsaMaterial();
            if (ghTyp.Value is GsaMaterialGoo) {
              ghTyp.CastTo(ref material);
              prop.Material = material ?? new GsaMaterial();
            }
            else {
              if (GH_Convert.ToInt32(ghTyp.Value, out int idd, GH_Conversion.Both))
                prop.Material = new GsaMaterial(idd);
              else {
                this.AddRuntimeError(
                  "Unable to convert PB input to a Section Property of reference integer");
                return;
              }
            }
          }
          else
            prop.Material = new GsaMaterial(2);
        }
        else
          prop.Material = new GsaMaterial(8);
      }

      da.SetData(0, new GsaProp2dGoo(prop));
    }

    #region Name and Ribbon Layout

    public override Guid ComponentGuid => new Guid("d693b4ad-7aaf-450e-a436-afbb9d2061fc");
    public override GH_Exposure Exposure => GH_Exposure.primary;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    protected override Bitmap Icon => Resources.CreateProp2d;

    public CreateProp2d() : base("Create 2D Property",
      "Prop2d",
      "Create GSA 2D Property",
      CategoryName.Name(),
      SubCategoryName.Cat1())
      => Hidden = true;

    #endregion

    #region Input and output

    protected override void RegisterInputParams(GH_InputParamManager pManager) {
      pManager.AddGenericParameter("Thickness [" + Length.GetAbbreviation(_lengthUnit) + "]",
        "Thk",
        "Section thickness",
        GH_ParamAccess.item);
      pManager.AddParameter(new GsaMaterialParameter());
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
      => pManager.AddParameter(new GsaProp2dParameter());

    #endregion

    #region Custom UI

    private readonly IReadOnlyDictionary<FoldMode, string> _dropdownTopLevel = new Dictionary<FoldMode, string>(){
      { FoldMode.PlaneStress, "Plane Stress"},
      { FoldMode.Fabric, "Fabric"},
      { FoldMode.FlatPlate, "Flat Plate"},
      { FoldMode.Shell, "Shell"},
      { FoldMode.CurvedShell, "Curved Shell"},
      { FoldMode.LoadPanel, "Load Panel"},
    };

    private LengthUnit _lengthUnit = DefaultUnits.LengthUnitSection;

    public override void InitialiseDropdowns() {
      SpacerDescriptions = new List<string>(new[] {
        "Type",
        "Unit",
      });

      DropDownItems = new List<List<string>>();
      SelectedItems = new List<string>();

      DropDownItems.Add(_dropdownTopLevel.Values.ToList());
      SelectedItems.Add(_dropdownTopLevel.Values.ElementAt(3));

      DropDownItems.Add(UnitsHelper.GetFilteredAbbreviations(EngineeringUnits.Length));
      SelectedItems.Add(Length.GetAbbreviation(_lengthUnit));

      IsInitialised = true;
    }

    public override void SetSelected(int i, int j) {
      SelectedItems[i] = DropDownItems[i][j];

      if (i == 0) {
        FoldMode mode = GetModeBy(SelectedItems[0]);
        UpdateParameters(mode);
        UpdateDropDownItems(mode);
      }
      else
        _lengthUnit = (LengthUnit)UnitsHelper.Parse(typeof(LengthUnit), SelectedItems[i]);

      base.UpdateUI();
    }

    public override void UpdateUIFromSelectedItems() {
      UpdateDropDownItems(GetModeBy(SelectedItems[0]));

      UpdateParameters(GetModeBy(SelectedItems[0]));
      _lengthUnit = (LengthUnit)UnitsHelper.Parse(typeof(LengthUnit), SelectedItems[1]);

      base.UpdateUIFromSelectedItems();
    }
    #region update inputs

    private enum FoldMode {
      PlaneStress,
      Fabric,
      FlatPlate,
      Shell,
      CurvedShell,
      LoadPanel,
    }

    private FoldMode _mode = FoldMode.Shell;

    private void UpdateParameters(FoldMode mode) {
      if (_mode == mode)
        return;

      _dropdownTopLevel.TryGetValue(mode, out string eventName);
      RecordUndoEvent($"{eventName} Parameters");

      switch (mode) {
        case FoldMode.Shell:
        case FoldMode.PlaneStress:
        case FoldMode.FlatPlate:
        case FoldMode.CurvedShell:
          switch (Params.Input.Count) {
            case 0:
              Params.RegisterInputParam(new Param_GenericObject());
              Params.RegisterInputParam(new GsaMaterialParameter());
              break;
            case 1:
              Params.RegisterInputParam(new GsaMaterialParameter());
              break;
          }

          break;
        case FoldMode.Fabric:
          switch (Params.Input.Count) {
            case 0:
              Params.RegisterInputParam(new Param_GenericObject());
              break;
            case 2:
              Params.UnregisterInputParameter(Params.Input[1], true);
              break;
          }

          break;
        case FoldMode.LoadPanel:
          while (Params.Input.Count > 0)
            Params.UnregisterInputParameter(Params.Input[0], true);
          break;
      }

      _mode = mode;
    }

    private void UpdateDropDownItems(FoldMode mode) {
      switch (mode) {
        case FoldMode.PlaneStress:
        case FoldMode.FlatPlate:
        case FoldMode.Shell:
        case FoldMode.CurvedShell:
          if (DropDownItems.Count < 2)
            DropDownItems.Add(UnitsHelper.GetFilteredAbbreviations(EngineeringUnits.Length));

          break;
        case FoldMode.Fabric:
        case FoldMode.LoadPanel:
          if (DropDownItems.Count > 1)
            DropDownItems.RemoveAt(1);
          break;
      }
    }

    private FoldMode GetModeBy(string name) {
      FoldMode mode = FoldMode.Shell;

      foreach (KeyValuePair<FoldMode, string> item in _dropdownTopLevel)
        if (item.Value.Contains(name))
          mode = item.Key;
      return mode;
    }

    #endregion

    public override void VariableParameterMaintenance() {
      switch (_mode) {
        case FoldMode.LoadPanel:
          return;
        case FoldMode.Fabric:
          SetMaterialInputAt(0);
          break;
        case FoldMode.Shell:
        case FoldMode.PlaneStress:
        case FoldMode.FlatPlate:
        case FoldMode.CurvedShell:
          SetInputProperties(index: 0, nickname: "Thk", name: $"Thickness [{Length.GetAbbreviation(_lengthUnit)}]", description: "Section thickness", optional: false);
          SetMaterialInputAt(1);
          break;
      }
    }

    private void SetInputProperties(
      int index,
      string nickname,
      string name,
      string description,
      GH_ParamAccess access = GH_ParamAccess.item,
      bool optional = true) {
      Params.Input[index]
        .NickName = nickname;
      Params.Input[index]
        .Name = name;
      Params.Input[index]
        .Description = description;
      Params.Input[index]
        .Access = access;
      Params.Input[index]
        .Optional = optional;
    }

    private void SetMaterialInputAt(int index) => SetInputProperties(index, "Mat", "Material", "GSA Material");
    #endregion
  }
}
