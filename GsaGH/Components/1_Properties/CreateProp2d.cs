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
using OasysGH.Units;
using OasysGH.Units.Helpers;
using OasysUnits;
using OasysUnits.Units;

namespace GsaGH.Components
{
    /// <summary>
    /// Component to create a new Prop2d
    /// </summary>
    public class CreateProp2d : GH_OasysDropDownComponent
  {
    #region Name and Ribbon Layout
    public override Guid ComponentGuid => new Guid("d693b4ad-7aaf-450e-a436-afbb9d2061fc");
    public override GH_Exposure Exposure => GH_Exposure.primary;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    protected override System.Drawing.Bitmap Icon => GsaGH.Properties.Resources.CreateProp2d;

    public CreateProp2d() : base("Create 2D Property",
      "Prop2d",
      "Create GSA 2D Property",
      CategoryName.Name(),
      SubCategoryName.Cat1())
    { this.Hidden = true; } // sets the initial state of the component to hidden
    #endregion

    #region Input and output
    protected override void RegisterInputParams(GH_InputParamManager pManager)
    {
      pManager.AddGenericParameter("Thickness [" + Length.GetAbbreviation(this.LengthUnit) + "]", "Thk", "Section thickness", GH_ParamAccess.item);
      pManager.AddParameter(new GsaMaterialParameter());
    }
    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
    {
      pManager.AddParameter(new GsaProp2dParameter());
    }
    #endregion

    protected override void SolveInstance(IGH_DataAccess DA)
    {
      GsaProp2d prop = new GsaProp2d();

      // element type (picked in dropdown)
      switch (_mode)
      {
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

      if (_mode != FoldMode.LoadPanel)
      {
        prop.AxisProperty = 0;

        if (_mode != FoldMode.Fabric)
        {
          // 0 Thickness
          prop.Thickness = (Length)Input.UnitNumber(this, DA, 0, this.LengthUnit);

          // 1 Material
          GH_ObjectWrapper gh_typ = new GH_ObjectWrapper();
          if (DA.GetData(1, ref gh_typ))
          {
            GsaMaterial material = new GsaMaterial();
            if (gh_typ.Value is GsaMaterialGoo)
            {
              gh_typ.CastTo(ref material);
              prop.Material = material;
            }
            else
            {
              if (GH_Convert.ToInt32(gh_typ.Value, out int idd, GH_Conversion.Both))
              {
                prop.Material = new GsaMaterial(idd);
              }
              else
              {
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

      DA.SetData(0, new GsaProp2dGoo(prop));
    }

    #region Custom UI
    private readonly List<string> _dropdownTopLevel = new List<string>(new string[]
    {
      "Plane Stress",
      "Fabric",
      "Flat Plate",
      "Shell",
      "Curved Shell",
      "Load Panel"
    });

    private LengthUnit LengthUnit = DefaultUnits.LengthUnitSection;

    public override void InitialiseDropdowns()
    {
      this.SpacerDescriptions = new List<string>(new string[]
        {
          "Type", "Unit"
        });

      this.DropDownItems = new List<List<string>>();
      this.SelectedItems = new List<string>();

      // Type
      this.DropDownItems.Add(this._dropdownTopLevel);
      this.SelectedItems.Add(this._dropdownTopLevel[3]);

      // Length
      this.DropDownItems.Add(UnitsHelper.GetFilteredAbbreviations(EngineeringUnits.Length));
      this.SelectedItems.Add(Length.GetAbbreviation(this.LengthUnit));

      this.IsInitialised = true;
    }

    public override void SetSelected(int i, int j)
    {
      this.SelectedItems[i] = this.DropDownItems[i][j];

      if (i == 0) // if change is made to the first list
      {
        switch (this.SelectedItems[i])
        {
          case "Plane Stress":
            if (this.DropDownItems.Count < 2)
              this.DropDownItems.Add(UnitsHelper.GetFilteredAbbreviations(EngineeringUnits.Length));
            Mode1Clicked();
            break;
          case "Fabric":
            if (this.DropDownItems.Count > 1)
              this.DropDownItems.RemoveAt(1); // remove length unit dropdown
            Mode2Clicked();
            break;
          case "Flat Plate":
            if (this.DropDownItems.Count < 2)
              this.DropDownItems.Add(UnitsHelper.GetFilteredAbbreviations(EngineeringUnits.Length));
            Mode3Clicked();
            break;
          case "Shell":
            if (this.DropDownItems.Count < 2)
              this.DropDownItems.Add(UnitsHelper.GetFilteredAbbreviations(EngineeringUnits.Length));
            Mode4Clicked();
            break;
          case "Curved Shell":
            if (this.DropDownItems.Count < 2)
              this.DropDownItems.Add(UnitsHelper.GetFilteredAbbreviations(EngineeringUnits.Length));
            Mode5Clicked();
            break;
          case "Load Panel":
            if (this.DropDownItems.Count > 1)
              this.DropDownItems.RemoveAt(1); // remove length unit dropdown
            Mode6Clicked();
            break;
        }
      }
      else
        this.LengthUnit = (LengthUnit)UnitsHelper.Parse(typeof(LengthUnit), this.SelectedItems[i]);

      base.UpdateUI();
    }

    public override void UpdateUIFromSelectedItems()
    {
      switch (this.SelectedItems[0])
      {
        case "Plane Stress":
          if (this.DropDownItems.Count < 2)
            this.DropDownItems.Add(UnitsHelper.GetFilteredAbbreviations(EngineeringUnits.Length));
          Mode1Clicked();
          break;
        case "Fabric":
          if (this.DropDownItems.Count > 1)
            this.DropDownItems.RemoveAt(1); // remove length unit dropdown
          Mode2Clicked();
          break;
        case "Flat Plate":
          if (this.DropDownItems.Count < 2)
            this.DropDownItems.Add(UnitsHelper.GetFilteredAbbreviations(EngineeringUnits.Length));
          Mode3Clicked();
          break;
        case "Shell":
          if (this.DropDownItems.Count < 2)
            this.DropDownItems.Add(UnitsHelper.GetFilteredAbbreviations(EngineeringUnits.Length));
          Mode4Clicked();
          break;
        case "Curved Shell":
          if (this.DropDownItems.Count < 2)
            this.DropDownItems.Add(UnitsHelper.GetFilteredAbbreviations(EngineeringUnits.Length));
          Mode5Clicked();
          break;
        case "Load Panel":
          if (this.DropDownItems.Count > 1)
            this.DropDownItems.RemoveAt(1); // remove length unit dropdown
          Mode6Clicked();
          break;
      }
      this.LengthUnit = (LengthUnit)UnitsHelper.Parse(typeof(LengthUnit), this.SelectedItems[1]);
      base.UpdateUIFromSelectedItems();
    }

    #region update inputs
    private enum FoldMode
    {
      PlaneStress,
      Fabric,
      FlatPlate,
      Shell,
      CurvedShell,
      LoadPanel
    }
    private FoldMode _mode = FoldMode.Shell;

    private void Mode1Clicked()
    {
      if (_mode == FoldMode.PlaneStress)
        return;

      RecordUndoEvent("Plane Stress Parameters");
      if (_mode == FoldMode.LoadPanel || _mode == FoldMode.Fabric)
      {
        //remove input parameters
        while (Params.Input.Count > 0)
          Params.UnregisterInputParameter(Params.Input[0], true);

        //register input parameter
        Params.RegisterInputParam(new Param_GenericObject());
        Params.RegisterInputParam(new GsaMaterialParameter());
      }
      _mode = FoldMode.PlaneStress;
    }
    private void Mode2Clicked()
    {
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
    private void Mode3Clicked()
    {
      if (_mode == FoldMode.FlatPlate)
        return;

      RecordUndoEvent("Flat Plate Parameters");
      if (_mode == FoldMode.LoadPanel || _mode == FoldMode.Fabric)
      {
        //remove input parameters
        while (Params.Input.Count > 0)
          Params.UnregisterInputParameter(Params.Input[0], true);

        //register input parameter
        Params.RegisterInputParam(new Param_GenericObject());
        Params.RegisterInputParam(new GsaMaterialParameter());
      }
      _mode = FoldMode.FlatPlate;
    }

    private void Mode4Clicked()
    {
      if (_mode == FoldMode.Shell)
        return;

      RecordUndoEvent("Shell Parameters");
      if (_mode == FoldMode.LoadPanel || _mode == FoldMode.Fabric)
      {
        //remove input parameters
        while (Params.Input.Count > 0)
          Params.UnregisterInputParameter(Params.Input[0], true);

        //register input parameter
        Params.RegisterInputParam(new Param_GenericObject());
        Params.RegisterInputParam(new GsaMaterialParameter());
      }
      _mode = FoldMode.Shell;
    }

    private void Mode5Clicked()
    {
      if (_mode == FoldMode.CurvedShell)
        return;

      RecordUndoEvent("Curved Shell Parameters");
      if (_mode == FoldMode.LoadPanel || _mode == FoldMode.Fabric)
      {
        //remove input parameters
        while (Params.Input.Count > 0)
          Params.UnregisterInputParameter(Params.Input[0], true);

        //register input parameter
        Params.RegisterInputParam(new Param_GenericObject());
        Params.RegisterInputParam(new GsaMaterialParameter());
      }
      _mode = FoldMode.CurvedShell;
    }

    private void Mode6Clicked()
    {
      if (_mode == FoldMode.LoadPanel)
        return;

      RecordUndoEvent("Load Panel Parameters");
      _mode = FoldMode.LoadPanel;

      //remove input parameters
      while (Params.Input.Count > 0)
        Params.UnregisterInputParameter(Params.Input[0], true);
    }
    #endregion
    
    public override void VariableParameterMaintenance()
    {
      if (_mode != FoldMode.LoadPanel && _mode != FoldMode.Fabric)
      {
        int i = 0;
        Params.Input[i].NickName = "Thk";
        Params.Input[i].Name = "Thickness [" + Length.GetAbbreviation(this.LengthUnit) + "]"; // "Thickness [m]";
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

      if (_mode == FoldMode.Fabric)
      {
        int i = 0;
        Params.Input[i].NickName = "Mat";
        Params.Input[i].Name = "Material";
        Params.Input[i].Description = "GSA Material";
        Params.Input[i].Access = GH_ParamAccess.item;
        Params.Input[i].Optional = true;
      }
    }
    #endregion
  }
}
