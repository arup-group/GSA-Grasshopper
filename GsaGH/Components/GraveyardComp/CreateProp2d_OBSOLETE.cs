using System;
using System.Collections.Generic;
using System.Linq;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Parameters;
using Grasshopper.Kernel.Types;
using GsaAPI;
using GsaGH.Helpers.GH;
using GsaGH.Parameters;
using OasysGH;
using OasysGH.Components;
using OasysGH.Helpers;
using OasysUnits;
using OasysUnits.Units;

namespace GsaGH.Components {
  /// <summary>
  /// Component to create a new Prop2d
  /// </summary>
  // ReSharper disable once InconsistentNaming
  public class CreateProp2d_OBSOLETE : GH_OasysComponent, IGH_VariableParameterComponent {
    #region Name and Ribbon Layout
    public override Guid ComponentGuid => new Guid("3fd61492-b5ff-47ea-8c7c-89cf639b32dc");

    public CreateProp2d_OBSOLETE()
      : base("Create 2D Property", "Prop2d", "Create GSA 2D Property",
        CategoryName.Name(),
        SubCategoryName.Cat1()) {
      Hidden = true;
    }
    public override GH_Exposure Exposure => GH_Exposure.hidden;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    protected override System.Drawing.Bitmap Icon => Properties.Resources.CreateProp2d;
    #endregion

    #region Custom UI
    public override void CreateAttributes() {
      if (_first) {
        _dropDownItems = new List<List<string>>();
        _selectedItems = new List<string>();

        _dropDownItems.Add(_dropdownTopList);
        _dropDownItems.Add(OasysGH.Units.Helpers.FilteredUnits.FilteredLengthUnits);

        _selectedItems.Add(_dropdownTopList[3]);
        _selectedItems.Add(_lengthUnit.ToString());

        IQuantity quantity = new Length(0, _lengthUnit);
        _unitAbbreviation = string.Concat(quantity.ToString().Where(char.IsLetter));

        _first = false;
      }

      m_attributes = new OasysGH.UI.DropDownComponentAttributes(this, SetSelected, _dropDownItems, _selectedItems, _spacerDescriptions);
    }

    public void SetSelected(int i, int j) {
      _selectedItems[i] = _dropDownItems[i][j];

      if (i == 0) {
        switch (_selectedItems[i]) {
          case "Plane Stress":
            if (_dropDownItems.Count < 2)
              _dropDownItems.Add(OasysGH.Units.Helpers.FilteredUnits.FilteredLengthUnits);
            Mode1Clicked();
            break;
          case "Fabric":
            if (_dropDownItems.Count > 1)
              _dropDownItems.RemoveAt(1);
            Mode2Clicked();
            break;
          case "Flat Plate":
            if (_dropDownItems.Count < 2)
              _dropDownItems.Add(OasysGH.Units.Helpers.FilteredUnits.FilteredLengthUnits);
            Mode3Clicked();
            break;
          case "Shell":
            if (_dropDownItems.Count < 2)
              _dropDownItems.Add(OasysGH.Units.Helpers.FilteredUnits.FilteredLengthUnits);
            Mode4Clicked();
            break;
          case "Curved Shell":
            if (_dropDownItems.Count < 2)
              _dropDownItems.Add(OasysGH.Units.Helpers.FilteredUnits.FilteredLengthUnits);
            Mode5Clicked();
            break;
          case "Load Panel":
            if (_dropDownItems.Count > 1)
              _dropDownItems.RemoveAt(1);
            Mode6Clicked();
            break;
        }
      }
      else {
        _lengthUnit = (LengthUnit)Enum.Parse(typeof(LengthUnit), _selectedItems[i]);
      }

      (this as IGH_VariableParameterComponent).VariableParameterMaintenance();
      ExpireSolution(true);
      Params.OnParametersChanged();
      OnDisplayExpired(true);
    }
    private void UpdateUiFromSelectedItems() {
      CreateAttributes();
      (this as IGH_VariableParameterComponent).VariableParameterMaintenance();
      ExpireSolution(true);
      Params.OnParametersChanged();
      OnDisplayExpired(true);
    }
    #endregion

    #region Input and output
    private readonly List<string> _dropdownTopList = new List<string>(new[]
    {
            "Plane Stress",
            "Fabric",
            "Flat Plate",
            "Shell",
            "Curved Shell",
            "Load Panel",
    });

    private List<List<string>> _dropDownItems;
    private List<string> _selectedItems;
    private List<string> _spacerDescriptions = new List<string>(new[] {
      "Element Type",
      "Unit",
    });
    private bool _first = true;
    private LengthUnit _lengthUnit = OasysGH.Units.DefaultUnits.LengthUnitGeometry;
    private string _unitAbbreviation;
    #endregion

    protected override void RegisterInputParams(GH_InputParamManager pManager) {
      Params.RegisterInputParam(new Param_GenericObject());
      Params.RegisterInputParam(new Param_Number());

      (this as IGH_VariableParameterComponent).VariableParameterMaintenance();
      Params.OnParametersChanged();
      ExpireSolution(true);
    }
    protected override void RegisterOutputParams(GH_OutputParamManager pManager) {
      pManager.AddGenericParameter("2D Property", "PA", "GSA 2D Property", GH_ParamAccess.item);
    }

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
          var ghTyp = new GH_ObjectWrapper();
          if (da.GetData(0, ref ghTyp)) {
            var material = new GsaMaterial();
            if (ghTyp.Value is GsaMaterialGoo) {
              ghTyp.CastTo(ref material);
              prop.Material = material;
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

          prop.Thickness = (Length)Input.UnitNumber(this, da, 1, _lengthUnit);
        }
        else
          prop.Material = new GsaMaterial(8);
      }

      da.SetData(0, new GsaProp2dGoo(prop));

    }
    #region menu override
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
        while (Params.Input.Count > 0)
          Params.UnregisterInputParameter(Params.Input[0], true);

        Params.RegisterInputParam(new Param_GenericObject());
        Params.RegisterInputParam(new Param_GenericObject());
      }
      _mode = FoldMode.PlaneStress;

      (this as IGH_VariableParameterComponent).VariableParameterMaintenance();
      Params.OnParametersChanged();
      ExpireSolution(true);
    }
    private void Mode2Clicked() {
      if (_mode == FoldMode.Fabric)
        return;

      RecordUndoEvent("Fabric Parameters");
      _mode = FoldMode.Fabric;

      while (Params.Input.Count > 0)
        Params.UnregisterInputParameter(Params.Input[0], true);

      Params.RegisterInputParam(new Param_GenericObject());

      (this as IGH_VariableParameterComponent).VariableParameterMaintenance();
      Params.OnParametersChanged();
      ExpireSolution(true);
    }
    private void Mode3Clicked() {
      if (_mode == FoldMode.FlatPlate)
        return;

      RecordUndoEvent("Flat Plate Parameters");
      if (_mode == FoldMode.LoadPanel || _mode == FoldMode.Fabric) {
        while (Params.Input.Count > 0)
          Params.UnregisterInputParameter(Params.Input[0], true);

        Params.RegisterInputParam(new Param_GenericObject());
        Params.RegisterInputParam(new Param_GenericObject());
      }
      _mode = FoldMode.FlatPlate;

      (this as IGH_VariableParameterComponent).VariableParameterMaintenance();
      Params.OnParametersChanged();
      ExpireSolution(true);
    }

    private void Mode4Clicked() {
      if (_mode == FoldMode.Shell)
        return;

      RecordUndoEvent("Shell Parameters");
      if (_mode == FoldMode.LoadPanel || _mode == FoldMode.Fabric) {
        while (Params.Input.Count > 0)
          Params.UnregisterInputParameter(Params.Input[0], true);

        Params.RegisterInputParam(new Param_GenericObject());
        Params.RegisterInputParam(new Param_GenericObject());
      }
      _mode = FoldMode.Shell;

      (this as IGH_VariableParameterComponent).VariableParameterMaintenance();
      Params.OnParametersChanged();
      ExpireSolution(true);
    }

    private void Mode5Clicked() {
      if (_mode == FoldMode.CurvedShell)
        return;

      RecordUndoEvent("Curved Shell Parameters");
      if (_mode == FoldMode.LoadPanel || _mode == FoldMode.Fabric) {
        while (Params.Input.Count > 0)
          Params.UnregisterInputParameter(Params.Input[0], true);

        Params.RegisterInputParam(new Param_GenericObject());
        Params.RegisterInputParam(new Param_GenericObject());
      }
      _mode = FoldMode.CurvedShell;

      (this as IGH_VariableParameterComponent).VariableParameterMaintenance();
      Params.OnParametersChanged();
      ExpireSolution(true);
    }

    private void Mode6Clicked() {
      if (_mode == FoldMode.LoadPanel)
        return;

      RecordUndoEvent("Load Panel Parameters");
      _mode = FoldMode.LoadPanel;

      while (Params.Input.Count > 0)
        Params.UnregisterInputParameter(Params.Input[0], true);

      (this as IGH_VariableParameterComponent).VariableParameterMaintenance();
      Params.OnParametersChanged();
      ExpireSolution(true);
    }
    #endregion
    #region (de)serialization
    public override bool Write(GH_IO.Serialization.GH_IWriter writer) {
      WriteDropDownComponents(ref writer, _dropDownItems, _selectedItems, _spacerDescriptions);
      return base.Write(writer);
    }
    public override bool Read(GH_IO.Serialization.GH_IReader reader) {
      try// if users has an old version of this component then dropdown menu wont read
      {
        ReadDropDownComponents(ref reader, ref _dropDownItems, ref _selectedItems, ref _spacerDescriptions);
        _mode = (FoldMode)Enum.Parse(typeof(FoldMode), _selectedItems[0].Replace(" ", string.Empty));
        _lengthUnit = (LengthUnit)Enum.Parse(typeof(LengthUnit), _selectedItems[1]);
      }
      catch (Exception) {
        _dropDownItems = new List<List<string>>();
        _selectedItems = new List<string>();
        _dropDownItems.Add(_dropdownTopList);
        _dropDownItems.Add(OasysGH.Units.Helpers.FilteredUnits.FilteredLengthUnits);

        _mode = (FoldMode)reader.GetInt32("Mode"); //old version would have this set
        _selectedItems.Add(reader.GetString("select")); // same
        _selectedItems.Add(_lengthUnit.ToString());

        _lengthUnit = LengthUnit.Meter;

        IQuantity quantity = new Length(0, _lengthUnit);
        _unitAbbreviation = string.Concat(quantity.ToString().Where(char.IsLetter));
      }
      UpdateUiFromSelectedItems();
      _first = false;
      return base.Read(reader);
    }

    bool IGH_VariableParameterComponent.CanInsertParameter(GH_ParameterSide side, int index) {
      return false;
    }
    bool IGH_VariableParameterComponent.CanRemoveParameter(GH_ParameterSide side, int index) {
      return false;
    }
    IGH_Param IGH_VariableParameterComponent.CreateParameter(GH_ParameterSide side, int index) {
      return null;
    }
    bool IGH_VariableParameterComponent.DestroyParameter(GH_ParameterSide side, int index) {
      return false;
    }
    #endregion
    #region IGH_VariableParameterComponent null implementation
    void IGH_VariableParameterComponent.VariableParameterMaintenance() {
      if (_mode != FoldMode.LoadPanel && _mode != FoldMode.Fabric) {
        IQuantity length = new Length(0, _lengthUnit);
        _unitAbbreviation = string.Concat(length.ToString().Where(char.IsLetter));

        int i = 0;
        Params.Input[i].NickName = "Mat";
        Params.Input[i].Name = "Material";
        Params.Input[i].Description = "GsaMaterial or Number referring to a Material already in Existing GSA Model." + Environment.NewLine
            + "Accepted inputs are: " + Environment.NewLine
            + "0 : Generic" + Environment.NewLine
            + "1 : Steel" + Environment.NewLine
            + "2 : Concrete (default)" + Environment.NewLine
            + "3 : Aluminium" + Environment.NewLine
            + "4 : Glass" + Environment.NewLine
            + "5 : FRP" + Environment.NewLine
            + "7 : Timber" + Environment.NewLine
            + "8 : Fabric";

        Params.Input[i].Access = GH_ParamAccess.item;
        Params.Input[i].Optional = true;

        i++;
        Params.Input[i].NickName = "Thk";
        Params.Input[i].Name = "Thickness [" + _unitAbbreviation + "]";
        Params.Input[i].Description = "Section thickness";
        Params.Input[i].Access = GH_ParamAccess.item;
        Params.Input[i].Optional = true;
      }

      if (_mode != FoldMode.Fabric) {
        return;
      }

      {
        const int i = 0;
        Params.Input[i].NickName = "Mat";
        Params.Input[i].Name = "Material";
        Params.Input[i].Description = "GsaMaterial or Reference ID for Material Property in Existing GSA Model";
        Params.Input[i].Access = GH_ParamAccess.item;
        Params.Input[i].Optional = true;
      }
    }
    #endregion

    internal static GH_IO.Serialization.GH_IWriter WriteDropDownComponents(ref GH_IO.Serialization.GH_IWriter writer, List<List<string>> dropDownItems, List<string> selectedItems, List<string> spacerDescriptions) {
      bool dropdown = false;
      if (dropDownItems != null) {
        writer.SetInt32("dropdownCount", dropDownItems.Count);
        for (int i = 0; i < dropDownItems.Count; i++) {
          writer.SetInt32("dropdowncontentsCount" + i, dropDownItems[i].Count);
          for (int j = 0; j < dropDownItems[i].Count; j++)
            writer.SetString("dropdowncontents" + i + j, dropDownItems[i][j]);
        }
        dropdown = true;
      }
      writer.SetBoolean("dropdown", dropdown);

      bool spacer = false;
      if (spacerDescriptions != null) {
        writer.SetInt32("spacerCount", spacerDescriptions.Count);
        for (int i = 0; i < spacerDescriptions.Count; i++)
          writer.SetString("spacercontents" + i, spacerDescriptions[i]);
        spacer = true;
      }
      writer.SetBoolean("spacer", spacer);

      bool select = false;
      if (selectedItems != null) {
        writer.SetInt32("selectionCount", selectedItems.Count);
        for (int i = 0; i < selectedItems.Count; i++)
          writer.SetString("selectioncontents" + i, selectedItems[i]);
        select = true;
      }
      writer.SetBoolean("select", select);

      return writer;
    }

    internal static void ReadDropDownComponents(ref GH_IO.Serialization.GH_IReader reader, ref List<List<string>> dropDownItems, ref List<string> selectedItems, ref List<string> spacerDescriptions) {
      if (reader.ItemExists("dropdown")) {
        int dropdownCount = reader.GetInt32("dropdownCount");
        dropDownItems = new List<List<string>>();
        for (int i = 0; i < dropdownCount; i++) {
          int dropdowncontentsCount = reader.GetInt32("dropdowncontentsCount" + i);
          var tempcontent = new List<string>();
          for (int j = 0; j < dropdowncontentsCount; j++)
            tempcontent.Add(reader.GetString("dropdowncontents" + i + j));
          dropDownItems.Add(tempcontent);
        }
      }
      else
        throw new Exception("Component doesnt have 'dropdown' content stored");

      if (reader.ItemExists("spacer")) {
        int dropdownspacerCount = reader.GetInt32("spacerCount");
        spacerDescriptions = new List<string>();
        for (int i = 0; i < dropdownspacerCount; i++)
          spacerDescriptions.Add(reader.GetString("spacercontents" + i));
      }

      if (!reader.ItemExists("select")) {
        return;
      }

      {
        int selectionsCount = reader.GetInt32("selectionCount");
        selectedItems = new List<string>();
        for (int i = 0; i < selectionsCount; i++)
          selectedItems.Add(reader.GetString("selectioncontents" + i));
      }
    }
  }
}
