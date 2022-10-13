﻿using System;
using System.Collections.Generic;
using System.Linq;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Parameters;
using Grasshopper.Kernel.Types;
using GsaAPI;
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
  public class CreateProp2d : GH_OasysComponent, IGH_VariableParameterComponent
  {
    #region Name and Ribbon Layout
    public override Guid ComponentGuid => new Guid("d693b4ad-7aaf-450e-a436-afbb9d2061fc");
    public override GH_Exposure Exposure => GH_Exposure.primary;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    protected override System.Drawing.Bitmap Icon => GsaGH.Properties.Resources.CreateProp2d;

    public CreateProp2d() : base("Create 2D Property",
      "Prop2d",
      "Create GSA 2D Property",
      Ribbon.CategoryName.Name(),
      Ribbon.SubCategoryName.Cat1())
    { this.Hidden = true; } // sets the initial state of the component to hidden
    #endregion

    #region Custom UI
    //This region overrides the typical component layout
    public override void CreateAttributes()
    {
      if (first)
      {
        dropdownitems = new List<List<string>>();
        selecteditems = new List<string>();

        // length
        dropdownitems.Add(dropdownTopList);
        dropdownitems.Add(FilteredUnits.FilteredLengthUnits);

        selecteditems.Add(dropdownTopList[3]);
        selecteditems.Add(this.LengthUnit.ToString());

        first = false;
      }

      m_attributes = new UI.MultiDropDownComponentUI(this, SetSelected, dropdownitems, selecteditems, spacerDescriptions);
    }

    public void SetSelected(int i, int j)
    {
      // change selected item
      selecteditems[i] = dropdownitems[i][j];

      if (i == 0) // if change is made to the first list
      {
        switch (selecteditems[i])
        {
          case "Plane Stress":
            if (dropdownitems.Count < 2)
              dropdownitems.Add(FilteredUnits.FilteredLengthUnits); // add length unit dropdown
            Mode1Clicked();
            break;
          case "Fabric":
            if (dropdownitems.Count > 1)
              dropdownitems.RemoveAt(1); // remove length unit dropdown
            Mode2Clicked();
            break;
          case "Flat Plate":
            if (dropdownitems.Count < 2)
              dropdownitems.Add(FilteredUnits.FilteredLengthUnits); // add length unit dropdown
            Mode3Clicked();
            break;
          case "Shell":
            if (dropdownitems.Count < 2)
              dropdownitems.Add(FilteredUnits.FilteredLengthUnits); // add length unit dropdown
            Mode4Clicked();
            break;
          case "Curved Shell":
            if (dropdownitems.Count < 2)
              dropdownitems.Add(FilteredUnits.FilteredLengthUnits); // add length unit dropdown
            Mode5Clicked();
            break;
          case "Load Panel":
            if (dropdownitems.Count > 1)
              dropdownitems.RemoveAt(1); // remove length unit dropdown
            Mode6Clicked();
            break;
        }
      }
      else
      {
        this.LengthUnit = (LengthUnit)Enum.Parse(typeof(LengthUnit), selecteditems[i]);
      }

      // update name of inputs (to display unit on sliders)
      (this as IGH_VariableParameterComponent).VariableParameterMaintenance();
      ExpireSolution(true);
      Params.OnParametersChanged();
      this.OnDisplayExpired(true);
    }
    private void UpdateUIFromSelectedItems()
    {
      CreateAttributes();
      (this as IGH_VariableParameterComponent).VariableParameterMaintenance();
      ExpireSolution(true);
      Params.OnParametersChanged();
      this.OnDisplayExpired(true);
    }
    #endregion

    #region Input and output
    readonly List<string> dropdownTopList = new List<string>(new string[]
    {
      "Plane Stress",
      "Fabric",
      "Flat Plate",
      "Shell",
      "Curved Shell",
      "Load Panel"
    });

    // list of lists with all dropdown lists conctent
    List<List<string>> dropdownitems;
    // list of selected items
    List<string> selecteditems;
    // list of descriptions 
    List<string> spacerDescriptions = new List<string>(new string[]
    {
      "Element Type",
      "Unit"
    });
    private bool first = true;
    private LengthUnit LengthUnit = DefaultUnits.LengthUnitSection;
    #endregion

    protected override void RegisterInputParams(GH_InputParamManager pManager)
    {
      pManager.AddGenericParameter("Thickness [" + Length.GetAbbreviation(this.LengthUnit) + "]", "Thk", "Section thickness", GH_ParamAccess.item);
      pManager.AddParameter(new GsaMaterialParameter());
    }
    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
    {
      pManager.AddParameter(new GsaProp2dParameter());
    }

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
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Unable to convert PB input to a Section Property of reference integer");
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
    #region menu override
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

      (this as IGH_VariableParameterComponent).VariableParameterMaintenance();
      Params.OnParametersChanged();
      ExpireSolution(true);
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

      (this as IGH_VariableParameterComponent).VariableParameterMaintenance();
      Params.OnParametersChanged();
      ExpireSolution(true);
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

      (this as IGH_VariableParameterComponent).VariableParameterMaintenance();
      Params.OnParametersChanged();
      ExpireSolution(true);
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

      (this as IGH_VariableParameterComponent).VariableParameterMaintenance();
      Params.OnParametersChanged();
      ExpireSolution(true);
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

      (this as IGH_VariableParameterComponent).VariableParameterMaintenance();
      Params.OnParametersChanged();
      ExpireSolution(true);
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

      (this as IGH_VariableParameterComponent).VariableParameterMaintenance();
      Params.OnParametersChanged();
      ExpireSolution(true);
    }
    #endregion
    #region (de)serialization
    public override bool Write(GH_IO.Serialization.GH_IWriter writer)
    {
      Util.GH.DeSerialization.writeDropDownComponents(ref writer, dropdownitems, selecteditems, spacerDescriptions);
      return base.Write(writer);
    }
    public override bool Read(GH_IO.Serialization.GH_IReader reader)
    {
      Util.GH.DeSerialization.readDropDownComponents(ref reader, ref dropdownitems, ref selecteditems, ref spacerDescriptions);
      _mode = (FoldMode)Enum.Parse(typeof(FoldMode), selecteditems[0].Replace(" ", string.Empty));
      this.LengthUnit = (LengthUnit)Enum.Parse(typeof(LengthUnit), selecteditems[1]);

      UpdateUIFromSelectedItems();
      first = false;
      return base.Read(reader);
    }

    bool IGH_VariableParameterComponent.CanInsertParameter(GH_ParameterSide side, int index)
    {
      return false;
    }
    bool IGH_VariableParameterComponent.CanRemoveParameter(GH_ParameterSide side, int index)
    {
      return false;
    }
    IGH_Param IGH_VariableParameterComponent.CreateParameter(GH_ParameterSide side, int index)
    {
      return null;
    }
    bool IGH_VariableParameterComponent.DestroyParameter(GH_ParameterSide side, int index)
    {
      return false;
    }
    #endregion
    #region IGH_VariableParameterComponent null implementation
    void IGH_VariableParameterComponent.VariableParameterMaintenance()
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
