using System;
using System.Collections.Generic;
using System.Linq;
using Grasshopper.Kernel;
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
  public class CreateNodeLoad : GH_OasysDropDownComponent
  {
    #region Name and Ribbon Layout
    public override Guid ComponentGuid => new Guid("dd16896d-111d-4436-b0da-9c05ff6efd81");
    public override GH_Exposure Exposure => GH_Exposure.primary;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    protected override System.Drawing.Bitmap Icon => GsaGH.Properties.Resources.NodeLoad;

    public CreateNodeLoad() : base("Create Node Load",
      "NodeLoad",
      "Create GSA Node Load",
      Ribbon.CategoryName.Name(),
      Ribbon.SubCategoryName.Cat3())
    { this.Hidden = true; } // sets the initial state of the component to hidden
    #endregion

    #region Input and output
    protected override void RegisterInputParams(GH_InputParamManager pManager)
    {
      string funit = " ";
      switch (this._mode)
      {
        case FoldMode.Node_Force:
          funit = "Value [" + Force.GetAbbreviation(this.ForceUnit) + "]";
          break;
        case FoldMode.Node_Moment:
          funit = "Value [" + Moment.GetAbbreviation(this.MomentUnit) + "]";
          break;
        case FoldMode.Applied_Displ:
        case FoldMode.Settlements:
          funit = "Value [" + Length.GetAbbreviation(this.LengthUnit) + "]";
          break;
      }

      pManager.AddIntegerParameter("Load case", "LC", "Load case number (default 1)", GH_ParamAccess.item, 1);
      pManager.AddTextParameter("Node list", "No", "List of Nodes to apply load to." + System.Environment.NewLine +
           "Node list should take the form:" + System.Environment.NewLine +
           " 1 11 to 72 step 2 not (XY3 31 to 45)" + System.Environment.NewLine +
           "Refer to GSA help file for definition of lists and full vocabulary.", GH_ParamAccess.item);
      pManager.AddTextParameter("Name", "Na", "Load Name", GH_ParamAccess.item);
      pManager.AddTextParameter("Direction", "Di", "Load direction (default z)." +
              System.Environment.NewLine + "Accepted inputs are:" +
              System.Environment.NewLine + "x" +
              System.Environment.NewLine + "y" +
              System.Environment.NewLine + "z" +
              System.Environment.NewLine + "xx" +
              System.Environment.NewLine + "yy" +
              System.Environment.NewLine + "zz", GH_ParamAccess.item, "z");
      pManager.AddGenericParameter("Value [" + funit + "]", "V", "Load Value", GH_ParamAccess.item);
      pManager[0].Optional = true;
      pManager[2].Optional = true;
      pManager[3].Optional = true;

    }
    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
    {
      pManager.AddParameter(new GsaLoadParameter(), "Node Load", "Ld", "GSA Node Load", GH_ParamAccess.item);
    }
    #endregion

    protected override void SolveInstance(IGH_DataAccess DA)
    {
      GsaNodeLoad nodeLoad = new GsaNodeLoad();

      // Node load type
      switch (_mode)
      {
        case FoldMode.Node_Force:
        case FoldMode.Node_Moment:
          nodeLoad.NodeLoadType = GsaNodeLoad.NodeLoadTypes.NODE_LOAD;
          break;
        case FoldMode.Applied_Displ:
          nodeLoad.Type = GsaNodeLoad.NodeLoadTypes.APPLIED_DISP;
          break;
        case FoldMode.Settlements:
          nodeLoad.Type = GsaNodeLoad.NodeLoadTypes.SETTLEMENT;
          break;
      }

      // 0 Load case
      int lc = 1;
      GH_Integer gh_lc = new GH_Integer();
      if (DA.GetData(0, ref gh_lc))
        GH_Convert.ToInt32(gh_lc, out lc, GH_Conversion.Both);
      nodeLoad.NodeLoad.Case = lc;

      // 1 element/beam list
      // check that user has not inputted Gsa geometry elements here
      GH_ObjectWrapper gh_typ = new GH_ObjectWrapper();
      if (DA.GetData(1, ref gh_typ))
      {
        string type = gh_typ.Value.ToString().ToUpper();
        if (type.StartsWith("GSA "))
        {
          Params.Owner.AddRuntimeMessage(GH_RuntimeMessageLevel.Error,
              "You cannot input a Node/Element/Member in NodeList input!" + System.Environment.NewLine +
              "Element list should take the form:" + System.Environment.NewLine +
              "'1 11 to 20 step 2 P1 not (G1 to G6 step 3) P11 not (PA PB1 PS2 PM3 PA4 M1)'" + System.Environment.NewLine +
              "Refer to GSA help file for definition of lists and full vocabulary.");
          return;
        }
      }
      string nodeList = "all";
      GH_String gh_nl = new GH_String();
      if (DA.GetData(1, ref gh_nl))
        GH_Convert.ToString(gh_nl, out nodeList, GH_Conversion.Both);
      nodeLoad.NodeLoad.Nodes = nodeList;

      // 3 Name
      string name = "";
      GH_String gh_name = new GH_String();
      if (DA.GetData(3, ref gh_name))
      {
        if (GH_Convert.ToString(gh_name, out name, GH_Conversion.Both))
          nodeLoad.NodeLoad.Name = name;
      }

      // 3 direction
      string dir = "Z";
      Direction direc = Direction.Z;

      GH_String gh_dir = new GH_String();
      if (DA.GetData(3, ref gh_dir))
        GH_Convert.ToString(gh_dir, out dir, GH_Conversion.Both);
      dir = dir.ToUpper().Trim();
      if (dir == "X")
        direc = Direction.X;
      if (dir == "Y")
        direc = Direction.Y;
      if (dir == "XX")
        direc = Direction.XX;
      if (dir == "YY")
        direc = Direction.YY;
      if (dir == "ZZ")
        direc = Direction.ZZ;

      nodeLoad.NodeLoad.Direction = direc;

      double load = 0;
      if (_mode == FoldMode.Node_Force || _mode == FoldMode.Node_Moment)
      {
        switch (dir)
        {
          case "X":
          case "Y":
          case "Z":
            load = ((Force)Input.UnitNumber(this, DA, 4, ForceUnit)).Newtons;
            if (_mode != FoldMode.Node_Force)
              AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Direction input set to imply a 'Force' but type is set to 'Moment'. The output Node Load has been created to be of type 'Force'.");
            break;
          case "XX":
          case "YY":
          case "ZZ":
            load = ((Moment)Input.UnitNumber(this, DA, 4, MomentUnit)).NewtonMeters;
            if (_mode != FoldMode.Node_Moment)
              AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Direction input set to imply a 'Moment' force but type is set to 'Force'. The output Node Load has been created to be of type 'Moment'.");
            break;
        }
      }
      else
      {
        switch (dir)
        {
          case "X":
          case "Y":
          case "Z":
            load = ((Length)Input.UnitNumber(this, DA, 4, LengthUnit)).Meters;
            break;
          case "XX":
          case "YY":
          case "ZZ":
            load = ((Angle)Input.UnitNumber(this, DA, 4, AngleUnit.Radian)).Radians;
            AddRuntimeMessage(GH_RuntimeMessageLevel.Remark, "Direction input is set to be rotational type, the output load has been set to as a rotation in Radian unit.");
            break;
        }
      }

      nodeLoad.NodeLoad.Value = load;

      GsaLoad gsaLoad = new GsaLoad(nodeLoad);
      DA.SetData(0, new GsaLoadGoo(gsaLoad));
    }


    #region Custom UI
    private enum FoldMode
    {
      Node_Force,
      Node_Moment,
      Applied_Displ,
      Settlements
    }
    readonly List<string> _type = new List<string>(new string[]
    {
      "Node Force",
      "Node Moment",
      "Applied Displ",
      "Settlement"
    });
    private FoldMode _mode = FoldMode.Node_Force;
    private ForceUnit ForceUnit = DefaultUnits.ForceUnit;
    private MomentUnit MomentUnit = DefaultUnits.MomentUnit;
    private LengthUnit LengthUnit = DefaultUnits.LengthUnitResult;
    public override void InitialiseDropdowns()
    {
      this.SpacerDescriptions = new List<string>(new string[]
        {
          "Type", "Unit"
        });

      this.DropDownItems = new List<List<string>>();
      this.SelectedItems = new List<string>();

      // Type
      this.DropDownItems.Add(this._type);
      this.SelectedItems.Add(this._mode.ToString().Replace('_', ' '));

      // Force
      this.DropDownItems.Add(FilteredUnits.FilteredForceUnits);
      this.SelectedItems.Add(this.ForceUnit.ToString());

      this.IsInitialised = true;
    }

    public override void SetSelected(int i, int j)
    {
      this.SelectedItems[i] = this.DropDownItems[i][j];

      if (i == 0) // change is made to the first dropdown list
      {
        switch (SelectedItems[0])
        {
          case "Node Force":
            this._mode = FoldMode.Node_Force;
            this.DropDownItems[1] = FilteredUnits.FilteredForceUnits;
            this.SelectedItems[1] = this.ForceUnit.ToString();
            break;
          case "Node Moment":
            this._mode = FoldMode.Node_Moment;
            this.DropDownItems[1] = FilteredUnits.FilteredMomentUnits;
            this.SelectedItems[1] = this.MomentUnit.ToString();
            break;
          case "Applied Displ":
            this._mode = FoldMode.Applied_Displ;
            this.DropDownItems[1] = FilteredUnits.FilteredLengthUnits;
            this.SelectedItems[1] = this.LengthUnit.ToString();
            break;
          case "Settlement":
            this._mode = FoldMode.Settlements;
            this.DropDownItems[1] = FilteredUnits.FilteredLengthUnits;
            this.SelectedItems[1] = this.LengthUnit.ToString();
            break;
        }
      }
      else
      {
        switch (this._mode)
        {
          case FoldMode.Node_Force:
            this.ForceUnit = (ForceUnit)Enum.Parse(typeof(ForceUnit), this.SelectedItems[1]);
            break;
          case FoldMode.Node_Moment:
            this.MomentUnit = (MomentUnit)Enum.Parse(typeof(MomentUnit), this.SelectedItems[1]);
            break;
          case FoldMode.Applied_Displ:
          case FoldMode.Settlements:
            this.LengthUnit = (LengthUnit)Enum.Parse(typeof(LengthUnit), this.SelectedItems[1]);
            break;
        }
      }

      base.UpdateUI();
    }
    public override void UpdateUIFromSelectedItems()
    {
      this._mode = (FoldMode)Enum.Parse(typeof(FoldMode), SelectedItems[0].Replace(' ', '_'));
      switch (this._mode)
      {
        case FoldMode.Node_Force:
          this.ForceUnit = (ForceUnit)Enum.Parse(typeof(ForceUnit), this.SelectedItems[1]);
          break;
        case FoldMode.Node_Moment:
          this.MomentUnit = (MomentUnit)Enum.Parse(typeof(MomentUnit), this.SelectedItems[1]);
          break;
        case FoldMode.Applied_Displ:
        case FoldMode.Settlements:
          this.LengthUnit = (LengthUnit)Enum.Parse(typeof(LengthUnit), this.SelectedItems[1]);
          break;
      }
      base.UpdateUIFromSelectedItems();
    }

    public override void VariableParameterMaintenance()
    {
      switch (this._mode)
      {
        case FoldMode.Node_Force:
          Params.Input[4].Name = "Value [" + Force.GetAbbreviation(this.ForceUnit) + "]";
          break;
        case FoldMode.Node_Moment:
          Params.Input[4].Name = "Value [" + Moment.GetAbbreviation(this.MomentUnit) + "]";
          break;
        case FoldMode.Applied_Displ:
        case FoldMode.Settlements:
          Params.Input[4].Name = "Value [" + Length.GetAbbreviation(this.LengthUnit) + "]";
          break;
      }
    }
    #endregion
  }
}
