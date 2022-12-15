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

namespace GsaGH.Components
{
  /// <summary>
  /// Component to create a new Prop2d
  /// </summary>
  public class CreateProp2d_OBSOLETE : GH_OasysComponent, IGH_VariableParameterComponent
  {
    #region Name and Ribbon Layout
    // This region handles how the component in displayed on the ribbon
    // including name, exposure level and icon
    public override Guid ComponentGuid => new Guid("3fd61492-b5ff-47ea-8c7c-89cf639b32dc");
    public CreateProp2d_OBSOLETE()
      : base("Create 2D Property", "Prop2d", "Create GSA 2D Property",
            CategoryName.Name(),
            SubCategoryName.Cat1())
    { this.Hidden = true; } // sets the initial state of the component to hidden
    public override GH_Exposure Exposure => GH_Exposure.hidden;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    protected override System.Drawing.Bitmap Icon => GsaGH.Properties.Resources.CreateProp2d;
    #endregion

    #region Custom UI
    //This region overrides the typical component layout
    public override void CreateAttributes()
    {
      if (first)
      {
        DropDownItems = new List<List<string>>();
        SelectedItems = new List<string>();

        // length
        DropDownItems.Add(dropdownTopList);
        DropDownItems.Add(OasysGH.Units.Helpers.FilteredUnits.FilteredLengthUnits);

        SelectedItems.Add(dropdownTopList[3]);
        SelectedItems.Add(lengthUnit.ToString());

        IQuantity quantity = new Length(0, lengthUnit);
        unitAbbreviation = string.Concat(quantity.ToString().Where(char.IsLetter));

        first = false;
      }

      m_attributes = new OasysGH.UI.DropDownComponentAttributes(this, SetSelected, DropDownItems, SelectedItems, SpacerDescriptions);
    }

    public void SetSelected(int i, int j)
    {
      // change selected item
      SelectedItems[i] = DropDownItems[i][j];

      if (i == 0) // if change is made to the first list
      {
        switch (SelectedItems[i])
        {
          case "Plane Stress":
            if (DropDownItems.Count < 2)
              DropDownItems.Add(OasysGH.Units.Helpers.FilteredUnits.FilteredLengthUnits); // add length unit dropdown
            Mode1Clicked();
            break;
          case "Fabric":
            if (DropDownItems.Count > 1)
              DropDownItems.RemoveAt(1); // remove length unit dropdown
            Mode2Clicked();
            break;
          case "Flat Plate":
            if (DropDownItems.Count < 2)
              DropDownItems.Add(OasysGH.Units.Helpers.FilteredUnits.FilteredLengthUnits); // add length unit dropdown
            Mode3Clicked();
            break;
          case "Shell":
            if (DropDownItems.Count < 2)
              DropDownItems.Add(OasysGH.Units.Helpers.FilteredUnits.FilteredLengthUnits); // add length unit dropdown
            Mode4Clicked();
            break;
          case "Curved Shell":
            if (DropDownItems.Count < 2)
              DropDownItems.Add(OasysGH.Units.Helpers.FilteredUnits.FilteredLengthUnits); // add length unit dropdown
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
      {
        lengthUnit = (LengthUnit)Enum.Parse(typeof(LengthUnit), SelectedItems[i]);
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
    List<List<string>> DropDownItems;
    // list of selected items
    List<string> SelectedItems;
    // list of descriptions 
    List<string> SpacerDescriptions = new List<string>(new string[]
    {
            "Element Type",
            "Unit"
    });
    private bool first = true;
    private LengthUnit lengthUnit = OasysGH.Units.DefaultUnits.LengthUnitGeometry;
    string unitAbbreviation;
    #endregion

    protected override void RegisterInputParams(GH_InputParamManager pManager)
    {
      //register input parameter
      Params.RegisterInputParam(new Param_GenericObject());
      Params.RegisterInputParam(new Param_Number());

      (this as IGH_VariableParameterComponent).VariableParameterMaintenance();
      Params.OnParametersChanged();
      ExpireSolution(true);
    }
    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
    {
      pManager.AddGenericParameter("2D Property", "PA", "GSA 2D Property", GH_ParamAccess.item);
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
          // 0 Material
          GH_ObjectWrapper gh_typ = new GH_ObjectWrapper();
          if (DA.GetData(0, ref gh_typ))
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

          prop.Thickness = (Length)Input.UnitNumber(this, DA, 1, lengthUnit);
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
        //Params.RegisterInputParam(new Param_Integer());
        Params.RegisterInputParam(new Param_GenericObject());
        Params.RegisterInputParam(new Param_GenericObject());
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
      //Params.RegisterInputParam(new Param_Integer());
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
        //Params.RegisterInputParam(new Param_Integer());
        Params.RegisterInputParam(new Param_GenericObject());
        Params.RegisterInputParam(new Param_GenericObject());
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
        //Params.RegisterInputParam(new Param_Integer());
        Params.RegisterInputParam(new Param_GenericObject());
        Params.RegisterInputParam(new Param_GenericObject());
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
        //Params.RegisterInputParam(new Param_Integer());
        Params.RegisterInputParam(new Param_GenericObject());
        Params.RegisterInputParam(new Param_GenericObject());
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
      writeDropDownComponents(ref writer, DropDownItems, SelectedItems, SpacerDescriptions);
      return base.Write(writer);
    }
    public override bool Read(GH_IO.Serialization.GH_IReader reader)
    {
      try// if users has an old version of this component then dropdown menu wont read
      {
        readDropDownComponents(ref reader, ref DropDownItems, ref SelectedItems, ref SpacerDescriptions);
        _mode = (FoldMode)Enum.Parse(typeof(FoldMode), SelectedItems[0].Replace(" ", string.Empty));
        lengthUnit = (LengthUnit)Enum.Parse(typeof(LengthUnit), SelectedItems[1]);
      }
      catch (Exception)
      {
        DropDownItems = new List<List<string>>();
        SelectedItems = new List<string>();
        // length
        DropDownItems.Add(dropdownTopList);
        DropDownItems.Add(OasysGH.Units.Helpers.FilteredUnits.FilteredLengthUnits);

        _mode = (FoldMode)reader.GetInt32("Mode"); //old version would have this set
        SelectedItems.Add(reader.GetString("select")); // same
        SelectedItems.Add(lengthUnit.ToString());

        // set length to meters as this was the only option for old components
        lengthUnit = LengthUnit.Meter;

        IQuantity quantity = new Length(0, lengthUnit);
        unitAbbreviation = string.Concat(quantity.ToString().Where(char.IsLetter));
      }
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
        IQuantity length = new Length(0, lengthUnit);
        unitAbbreviation = string.Concat(length.ToString().Where(char.IsLetter));

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
        Params.Input[i].Name = "Thickness [" + unitAbbreviation + "]"; // "Thickness [m]";
        Params.Input[i].Description = "Section thickness";
        Params.Input[i].Access = GH_ParamAccess.item;
        Params.Input[i].Optional = true;
      }

      if (_mode == FoldMode.Fabric)
      {
        int i = 0;
        Params.Input[i].NickName = "Mat";
        Params.Input[i].Name = "Material";
        Params.Input[i].Description = "GsaMaterial or Reference ID for Material Property in Existing GSA Model";
        Params.Input[i].Access = GH_ParamAccess.item;
        Params.Input[i].Optional = true;
      }
    }
    #endregion

    internal static GH_IO.Serialization.GH_IWriter writeDropDownComponents(ref GH_IO.Serialization.GH_IWriter writer, List<List<string>> DropDownItems, List<string> SelectedItems, List<string> SpacerDescriptions)
    {
      // to save the dropdownlist content, spacer list and selection list 
      // loop through the lists and save number of lists as well
      bool dropdown = false;
      if (DropDownItems != null)
      {
        writer.SetInt32("dropdownCount", DropDownItems.Count);
        for (int i = 0; i < DropDownItems.Count; i++)
        {
          writer.SetInt32("dropdowncontentsCount" + i, DropDownItems[i].Count);
          for (int j = 0; j < DropDownItems[i].Count; j++)
            writer.SetString("dropdowncontents" + i + j, DropDownItems[i][j]);
        }
        dropdown = true;
      }
      writer.SetBoolean("dropdown", dropdown);

      // spacer list
      bool spacer = false;
      if (SpacerDescriptions != null)
      {
        writer.SetInt32("spacerCount", SpacerDescriptions.Count);
        for (int i = 0; i < SpacerDescriptions.Count; i++)
          writer.SetString("spacercontents" + i, SpacerDescriptions[i]);
        spacer = true;
      }
      writer.SetBoolean("spacer", spacer);

      // selection list
      bool select = false;
      if (SelectedItems != null)
      {
        writer.SetInt32("selectionCount", SelectedItems.Count);
        for (int i = 0; i < SelectedItems.Count; i++)
          writer.SetString("selectioncontents" + i, SelectedItems[i]);
        select = true;
      }
      writer.SetBoolean("select", select);

      return writer;
    }

    internal static void readDropDownComponents(ref GH_IO.Serialization.GH_IReader reader, ref List<List<string>> DropDownItems, ref List<string> SelectedItems, ref List<string> SpacerDescriptions)
    {
      // dropdown content list
      if (reader.ItemExists("dropdown"))
      {
        int dropdownCount = reader.GetInt32("dropdownCount");
        DropDownItems = new List<List<string>>();
        for (int i = 0; i < dropdownCount; i++)
        {
          int dropdowncontentsCount = reader.GetInt32("dropdowncontentsCount" + i);
          List<string> tempcontent = new List<string>();
          for (int j = 0; j < dropdowncontentsCount; j++)
            tempcontent.Add(reader.GetString("dropdowncontents" + i + j));
          DropDownItems.Add(tempcontent);
        }
      }
      else
        throw new Exception("Component doesnt have 'dropdown' content stored");

      // spacer list
      if (reader.ItemExists("spacer"))
      {
        int dropdownspacerCount = reader.GetInt32("spacerCount");
        SpacerDescriptions = new List<string>();
        for (int i = 0; i < dropdownspacerCount; i++)
          SpacerDescriptions.Add(reader.GetString("spacercontents" + i));
      }

      // selection list
      if (reader.ItemExists("select"))
      {
        int selectionsCount = reader.GetInt32("selectionCount");
        SelectedItems = new List<string>();
        for (int i = 0; i < selectionsCount; i++)
          SelectedItems.Add(reader.GetString("selectioncontents" + i));
      }
    }
  }
}