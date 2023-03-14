using System;
using System.Collections.Generic;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Parameters;
using Grasshopper.Kernel.Types;
using GsaAPI;
using GsaGH.Helpers.GH;
using GsaGH.Parameters;
using OasysGH;
using OasysGH.Components;
using OasysGH.Helpers;
using OasysGH.Units;
using OasysGH.Units.Helpers;
using OasysUnits;
using OasysUnits.Units;

namespace GsaGH.Components {
  /// <summary>
  /// Component to create a new Prop2d
  /// </summary>
  public class CreateProp2d : GH_OasysDropDownComponent {
    #region Name and Ribbon Layout
    public override Guid ComponentGuid => new Guid("d693b4ad-7aaf-450e-a436-afbb9d2061fc");
    public override GH_Exposure Exposure => GH_Exposure.primary;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    protected override System.Drawing.Bitmap Icon => Properties.Resources.CreateProp2d;

    public CreateProp2d() : base("Create 2D Property",
      "Prop2d",
      "Create GSA 2D Property",
      CategoryName.Name(),
      SubCategoryName.Cat1()) {
        Hidden = true;
    } // sets the initial state of the component to hidden
    #endregion

    #region Input and output
    protected override void RegisterInputParams(GH_InputParamManager pManager) {
      pManager.AddGenericParameter("Thickness [" + Length.GetAbbreviation(_lengthUnit) + "]", "Thk", "Section thickness", GH_ParamAccess.item);
      pManager.AddParameter(new GsaMaterialParameter());
    }
    protected override void RegisterOutputParams(GH_OutputParamManager pManager) {
      pManager.AddParameter(new GsaProp2dParameter());
    }
    #endregion

    protected override void SolveInstance(IGH_DataAccess da) {
      var prop = new GsaProp2d();

      // element type (picked in dropdown)
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
          // 0 Thickness
          prop.Thickness = (Length)Input.UnitNumber(this, da, 0, _lengthUnit);

          // 1 Material
          var ghTyp = new GH_ObjectWrapper();
          if (da.GetData(1, ref ghTyp)) {
            if (ghTyp.Value is GsaMaterialGoo) {
              ghTyp.CastTo(out GsaMaterial material);
              prop.Material = material ?? new GsaMaterial();
            }
            else {
              if (GH_Convert.ToInt32(ghTyp.Value, out int idd, GH_Conversion.Both)) {
                prop.Material = new GsaMaterial(idd);
              }
              else {
                this.AddRuntimeError("Unable to convert PB input to a Section Property of reference integer");
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

    #region Custom UI
    private readonly List<string> _dropdownTopLevel = new List<string>(new []
    {
      "Plane Stress",
      "Fabric",
      "Flat Plate",
      "Shell",
      "Curved Shell",
      "Load Panel",
    });

    private LengthUnit _lengthUnit = DefaultUnits.LengthUnitSection;

    public override void InitialiseDropdowns() {
      SpacerDescriptions = new List<string>(new []
        {
          "Type", "Unit",
        });

      DropDownItems = new List<List<string>>();
      SelectedItems = new List<string>();

      // Type
      DropDownItems.Add(_dropdownTopLevel);
      SelectedItems.Add(_dropdownTopLevel[3]);

      // Length
      DropDownItems.Add(UnitsHelper.GetFilteredAbbreviations(EngineeringUnits.Length));
      SelectedItems.Add(Length.GetAbbreviation(_lengthUnit));

      IsInitialised = true;
    }

    public override void SetSelected(int i, int j) {
      SelectedItems[i] = DropDownItems[i][j];

      if (i == 0) // if change is made to the first list
      {
        switch (SelectedItems[i]) {
          case "Plane Stress":
            if (DropDownItems.Count < 2)
              DropDownItems.Add(UnitsHelper.GetFilteredAbbreviations(EngineeringUnits.Length));
            Mode1Clicked();
            break;
          case "Fabric":
            if (DropDownItems.Count > 1)
              DropDownItems.RemoveAt(1); // remove length unit dropdown
            Mode2Clicked();
            break;
          case "Flat Plate":
            if (DropDownItems.Count < 2)
              DropDownItems.Add(UnitsHelper.GetFilteredAbbreviations(EngineeringUnits.Length));
            Mode3Clicked();
            break;
          case "Shell":
            if (DropDownItems.Count < 2)
              DropDownItems.Add(UnitsHelper.GetFilteredAbbreviations(EngineeringUnits.Length));
            Mode4Clicked();
            break;
          case "Curved Shell":
            if (DropDownItems.Count < 2)
              DropDownItems.Add(UnitsHelper.GetFilteredAbbreviations(EngineeringUnits.Length));
            Mode5Clicked();
            break;
          case "Load Panel":
            if (DropDownItems.Count > 1)
              DropDownItems.RemoveAt(1); // remove length unit dropdown
            Mode6Clicked();
            break;
        }
      }
      else
        _lengthUnit = (LengthUnit)UnitsHelper.Parse(typeof(LengthUnit), SelectedItems[i]);

      base.UpdateUI();
    }

    public override void UpdateUIFromSelectedItems() {
      switch (SelectedItems[0]) {
        case "Plane Stress":
          if (DropDownItems.Count < 2)
            DropDownItems.Add(UnitsHelper.GetFilteredAbbreviations(EngineeringUnits.Length));
          Mode1Clicked();
          break;
        case "Fabric":
          if (DropDownItems.Count > 1)
            DropDownItems.RemoveAt(1); // remove length unit dropdown
          Mode2Clicked();
          break;
        case "Flat Plate":
          if (DropDownItems.Count < 2)
            DropDownItems.Add(UnitsHelper.GetFilteredAbbreviations(EngineeringUnits.Length));
          Mode3Clicked();
          break;
        case "Shell":
          if (DropDownItems.Count < 2)
            DropDownItems.Add(UnitsHelper.GetFilteredAbbreviations(EngineeringUnits.Length));
          Mode4Clicked();
          break;
        case "Curved Shell":
          if (DropDownItems.Count < 2)
            DropDownItems.Add(UnitsHelper.GetFilteredAbbreviations(EngineeringUnits.Length));
          Mode5Clicked();
          break;
        case "Load Panel":
          if (DropDownItems.Count > 1)
            DropDownItems.RemoveAt(1); // remove length unit dropdown
          Mode6Clicked();
          break;
      }

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

    private void Mode1Clicked() {
      if (_mode == FoldMode.PlaneStress)
        return;

      RecordUndoEvent("Plane Stress Parameters");
      if (_mode == FoldMode.LoadPanel || _mode == FoldMode.Fabric) {
        //remove input parameters
        while (Params.Input.Count > 0)
          Params.UnregisterInputParameter(Params.Input[0], true);

        //register input parameter
        Params.RegisterInputParam(new Param_GenericObject());
        Params.RegisterInputParam(new GsaMaterialParameter());
      }

      _mode = FoldMode.PlaneStress;
    }
    private void Mode2Clicked() {
      if (_mode == FoldMode.Fabric)
        return;

      RecordUndoEvent("Fabric Parameters");
      _mode = FoldMode.Fabric;

      //remove input parameters
      while (Params.Input.Count > 0)
        Params.UnregisterInputParameter(Params.Input[0], true);

      //register input parameter
      Params.RegisterInputParam(new Param_GenericObject());
    }
    private void Mode3Clicked() {
      if (_mode == FoldMode.FlatPlate)
        return;

      RecordUndoEvent("Flat Plate Parameters");
      if (_mode == FoldMode.LoadPanel || _mode == FoldMode.Fabric) {
        //remove input parameters
        while (Params.Input.Count > 0)
          Params.UnregisterInputParameter(Params.Input[0], true);

        //register input parameter
        Params.RegisterInputParam(new Param_GenericObject());
        Params.RegisterInputParam(new GsaMaterialParameter());
      }

      _mode = FoldMode.FlatPlate;
    }

    private void Mode4Clicked() {
      if (_mode == FoldMode.Shell)
        return;

      RecordUndoEvent("Shell Parameters");
      if (_mode == FoldMode.LoadPanel || _mode == FoldMode.Fabric) {
        //remove input parameters
        while (Params.Input.Count > 0)
          Params.UnregisterInputParameter(Params.Input[0], true);

        //register input parameter
        Params.RegisterInputParam(new Param_GenericObject());
        Params.RegisterInputParam(new GsaMaterialParameter());
      }

      _mode = FoldMode.Shell;
    }

    private void Mode5Clicked() {
      if (_mode == FoldMode.CurvedShell)
        return;

      RecordUndoEvent("Curved Shell Parameters");
      if (_mode == FoldMode.LoadPanel || _mode == FoldMode.Fabric) {
        //remove input parameters
        while (Params.Input.Count > 0)
          Params.UnregisterInputParameter(Params.Input[0], true);

        //register input parameter
        Params.RegisterInputParam(new Param_GenericObject());
        Params.RegisterInputParam(new GsaMaterialParameter());
      }

      _mode = FoldMode.CurvedShell;
    }

    private void Mode6Clicked() {
      if (_mode == FoldMode.LoadPanel)
        return;

      RecordUndoEvent("Load Panel Parameters");
      _mode = FoldMode.LoadPanel;

      //remove input parameters
      while (Params.Input.Count > 0)
        Params.UnregisterInputParameter(Params.Input[0], true);
    }
    #endregion

    public override void VariableParameterMaintenance() {
      if (_mode != FoldMode.LoadPanel && _mode != FoldMode.Fabric) {
        int i = 0;
        Params.Input[i].NickName = "Thk";
        Params.Input[i].Name = "Thickness [" + Length.GetAbbreviation(_lengthUnit) + "]"; // "Thickness [m]";
        Params.Input[i].Description = "Section thickness";
        Params.Input[i].Access = GH_ParamAccess.item;
        Params.Input[i].Optional = false;
        i++;
        Params.Input[i].NickName = "Mat";
        Params.Input[i].Name = "Material";
        Params.Input[i].Description = "GSA Material";
        Params.Input[i].Access = GH_ParamAccess.item;
        Params.Input[i].Optional = true;
      }

      if (_mode != FoldMode.Fabric) {
        return;
      }

      Params.Input[0].NickName = "Mat";
      Params.Input[0].Name = "Material";
      Params.Input[0].Description = "GSA Material";
      Params.Input[0].Access = GH_ParamAccess.item;
      Params.Input[0].Optional = true;
    }
    #endregion
  }
}
