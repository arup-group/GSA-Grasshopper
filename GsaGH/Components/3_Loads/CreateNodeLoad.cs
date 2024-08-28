using System;
using System.Collections.Generic;
using System.Drawing;

using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;

using GsaGH.Helpers;
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

using Rhino.Geometry;

using EntityType = GsaGH.Parameters.EntityType;

namespace GsaGH.Components {
  public class CreateNodeLoad : GH_OasysDropDownComponent {
    private enum FoldMode {
      NodeForce,
      NodeMoment,
      AppliedDispl,
      Settlement,
    }

    public override Guid ComponentGuid => new Guid("dd16896d-111d-4436-b0da-9c05ff6efd81");
    public override GH_Exposure Exposure => GH_Exposure.secondary;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    protected override Bitmap Icon => Resources.CreateNodeLoad;
    private readonly List<string> _type = new List<string>(new[] {
      "Node Force",
      "Node Moment",
      "Applied Displ",
      "Settlement",
    });
    private ForceUnit _forceUnit = DefaultUnits.ForceUnit;
    private LengthUnit _lengthUnit = DefaultUnits.LengthUnitResult;
    private FoldMode _mode = FoldMode.NodeForce;
    private MomentUnit _momentUnit = DefaultUnits.MomentUnit;

    public CreateNodeLoad() : base("Create Node Load", "NodeLoad", "Create GSA Node Load",
      CategoryName.Name(), SubCategoryName.Cat3()) {
      Hidden = true;
    }

    public override void SetSelected(int i, int j) {
      _selectedItems[i] = _dropDownItems[i][j];

      if (i == 0) {
        switch (_selectedItems[0]) {
          case "Node Force":
            _mode = FoldMode.NodeForce;
            _dropDownItems[1] = UnitsHelper.GetFilteredAbbreviations(EngineeringUnits.Force);
            _selectedItems[1] = Force.GetAbbreviation(_forceUnit);
            break;

          case "Node Moment":
            _mode = FoldMode.NodeMoment;
            _dropDownItems[1] = UnitsHelper.GetFilteredAbbreviations(EngineeringUnits.Moment);
            _selectedItems[1] = Moment.GetAbbreviation(_momentUnit);
            break;

          case "Applied Displ":
            _mode = FoldMode.AppliedDispl;
            _dropDownItems[1] = UnitsHelper.GetFilteredAbbreviations(EngineeringUnits.Length);
            _selectedItems[1] = Length.GetAbbreviation(_lengthUnit);
            break;

          case "Settlement":
            _mode = FoldMode.Settlement;
            _dropDownItems[1] = UnitsHelper.GetFilteredAbbreviations(EngineeringUnits.Length);
            _selectedItems[1] = Length.GetAbbreviation(_lengthUnit);
            break;
        }
      } else {
        switch (_mode) {
          case FoldMode.NodeForce:
            _forceUnit = (ForceUnit)UnitsHelper.Parse(typeof(ForceUnit), _selectedItems[1]);
            break;

          case FoldMode.NodeMoment:
            _momentUnit = (MomentUnit)UnitsHelper.Parse(typeof(MomentUnit), _selectedItems[1]);
            break;

          case FoldMode.AppliedDispl:
          case FoldMode.Settlement:
            _lengthUnit = (LengthUnit)UnitsHelper.Parse(typeof(LengthUnit), _selectedItems[1]);
            break;
        }
      }

      base.UpdateUI();
    }

    public override void VariableParameterMaintenance() {
      switch (_mode) {
        case FoldMode.NodeForce:
          Params.Input[4].Name = "Value [" + Force.GetAbbreviation(_forceUnit) + "]";
          break;

        case FoldMode.NodeMoment:
          Params.Input[4].Name = "Value [" + Moment.GetAbbreviation(_momentUnit) + "]";
          break;

        case FoldMode.AppliedDispl:
        case FoldMode.Settlement:
          Params.Input[4].Name = "Value [" + Length.GetAbbreviation(_lengthUnit) + "]";
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

      _dropDownItems.Add(_type);
      _selectedItems.Add(_mode.ToString().Replace('_', ' '));

      _dropDownItems.Add(UnitsHelper.GetFilteredAbbreviations(EngineeringUnits.Force));
      _selectedItems.Add(Force.GetAbbreviation(_forceUnit));

      _isInitialised = true;
    }

    protected override void RegisterInputParams(GH_InputParamManager pManager) {
      string funit = " ";
      switch (_mode) {
        case FoldMode.NodeForce:
          funit = "Value [" + Force.GetAbbreviation(_forceUnit) + "]";
          break;

        case FoldMode.NodeMoment:
          funit = "Value [" + Moment.GetAbbreviation(_momentUnit) + "]";
          break;

        case FoldMode.AppliedDispl:
        case FoldMode.Settlement:
          funit = "Value [" + Length.GetAbbreviation(_lengthUnit) + "]";
          break;
      }

      pManager.AddParameter(new GsaLoadCaseParameter());
      pManager.AddGenericParameter("Node list", "Pt",
        "Node or Point to apply load to; either input Node, Point, or a text string."
        + Environment.NewLine + "Text string with Node list should take the form:"
        + Environment.NewLine + " 1 11 to 72 step 2 not (XY3 31 to 45)" + Environment.NewLine
        + "Refer to GSA help file for definition of lists and full vocabulary.",
        GH_ParamAccess.item);
      pManager.AddTextParameter("Name", "Na", "Load Name", GH_ParamAccess.item);
      pManager.AddTextParameter("Direction", "Di",
        "Load direction (default z)." + Environment.NewLine + "Accepted inputs are:"
        + Environment.NewLine + "x" + Environment.NewLine + "y" + Environment.NewLine + "z"
        + Environment.NewLine + "xx" + Environment.NewLine + "yy" + Environment.NewLine + "zz",
        GH_ParamAccess.item, "z");
      pManager.AddGenericParameter("Value [" + funit + "]", "V", "Load Value", GH_ParamAccess.item);
      pManager[0].Optional = true;
      pManager[2].Optional = true;
      pManager[3].Optional = true;
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager) {
      pManager.AddParameter(new GsaLoadParameter(), "Node Load", "Ld", "GSA Node Load",
        GH_ParamAccess.item);
    }

    protected override void SolveInternal(IGH_DataAccess da) {
      var nodeLoad = new GsaNodeLoad();

      switch (_mode) {
        case FoldMode.NodeForce:
        case FoldMode.NodeMoment:
          nodeLoad.Type = NodeLoadType.NodeLoad;
          break;

        case FoldMode.AppliedDispl:
          nodeLoad.Type = NodeLoadType.AppliedDisp;
          break;

        case FoldMode.Settlement:
          nodeLoad.Type = NodeLoadType.Settlement;
          break;
      }

      var loadcase = new GsaLoadCase(1);
      GsaLoadCaseGoo loadCaseGoo = null;
      if (da.GetData(0, ref loadCaseGoo)) {
        if (loadCaseGoo.Value != null) {
          loadcase = loadCaseGoo.Value;
        }
      }

      nodeLoad.LoadCase = loadcase;

      var ghTyp = new GH_ObjectWrapper();
      if (da.GetData(1, ref ghTyp)) {
        var refPt = new Point3d();
        if (ghTyp.Value is GsaNodeGoo goo) {
          nodeLoad._refPoint = goo.Value.Point;
        } else if (ghTyp.Value is GsaListGoo value) {
          if (value.Value.EntityType == EntityType.Node) {
            nodeLoad.ReferenceList = value.Value;
          } else {
            this.AddRuntimeWarning("List must be of type Node to apply to Node loading");
          }
        } else if (GH_Convert.ToPoint3d(ghTyp.Value, ref refPt, GH_Conversion.Both)) {
          nodeLoad._refPoint = refPt;
          this.AddRuntimeRemark(
            "Point loading in GsaGH will automatically find the corrosponding node and apply the load to that node by ID. If you save the file and continue working in GSA please note that the point-load relationship will be lost.");
        } else if (GH_Convert.ToString(ghTyp.Value, out string nodeList, GH_Conversion.Both)) {
          nodeLoad.ApiLoad.Nodes = nodeList;
        }
      }

      var ghName = new GH_String();
      if (da.GetData(2, ref ghName)) {
        if (GH_Convert.ToString(ghName, out string name, GH_Conversion.Both)) {
          nodeLoad.ApiLoad.Name = name;
        }
      }

      string dir = "Z";
      GsaAPI.Direction direc = GsaAPI.Direction.Z;

      var ghDir = new GH_String();
      if (da.GetData(3, ref ghDir)) {
        GH_Convert.ToString(ghDir, out dir, GH_Conversion.Both);
      }

      dir = dir.ToUpper().Trim();
      switch (dir) {
        case "X":
          direc = GsaAPI.Direction.X;
          break;

        case "Y":
          direc = GsaAPI.Direction.Y;
          break;

        case "XX":
          direc = GsaAPI.Direction.XX;
          break;

        case "YY":
          direc = GsaAPI.Direction.YY;
          break;

        case "ZZ":
          direc = GsaAPI.Direction.ZZ;
          break;
      }

      nodeLoad.ApiLoad.Direction = direc;

      double load = 0;
      if (_mode == FoldMode.NodeForce || _mode == FoldMode.NodeMoment) {
        switch (dir) {
          case "X":
          case "Y":
          case "Z":
            load = ((Force)Input.UnitNumber(this, da, 4, _forceUnit)).Newtons;
            if (_mode != FoldMode.NodeForce) {
              this.AddRuntimeWarning(
                "Direction input set to imply a 'Force' but type is set to 'Moment'. The output Node Load has been created to be of type 'Force'.");
            }

            break;

          case "XX":
          case "YY":
          case "ZZ":
            load = ((Moment)Input.UnitNumber(this, da, 4, _momentUnit)).NewtonMeters;
            if (_mode != FoldMode.NodeMoment) {
              this.AddRuntimeWarning(
                "Direction input set to imply a 'Moment' force but type is set to 'Force'. The output Node Load has been created to be of type 'Moment'.");
            }

            break;
        }
      } else {
        switch (dir) {
          case "X":
          case "Y":
          case "Z":
            load = ((Length)Input.UnitNumber(this, da, 4, _lengthUnit)).Meters;
            break;

          case "XX":
          case "YY":
          case "ZZ":
            load = ((Angle)Input.UnitNumber(this, da, 4, AngleUnit.Radian)).Radians;
            this.AddRuntimeRemark(
              "Direction input is set to be rotational type, the output load has been set to as a rotation in Radian unit.");
            break;
        }
      }

      nodeLoad.ApiLoad.Value = load;

      da.SetData(0, new GsaLoadGoo(nodeLoad));
    }

    protected override void UpdateUIFromSelectedItems() {
      string md = _selectedItems[0].Replace(" ", "_").ToPascalCase();
      _mode = md.ToLower() == "node" ? FoldMode.NodeForce :
        (FoldMode)Enum.Parse(typeof(FoldMode), md, true);

      switch (_mode) {
        case FoldMode.NodeForce:
          _forceUnit = (ForceUnit)UnitsHelper.Parse(typeof(ForceUnit), _selectedItems[1]);
          break;

        case FoldMode.NodeMoment:
          _momentUnit = (MomentUnit)UnitsHelper.Parse(typeof(MomentUnit), _selectedItems[1]);
          break;

        case FoldMode.AppliedDispl:
        case FoldMode.Settlement:
          _lengthUnit = (LengthUnit)UnitsHelper.Parse(typeof(LengthUnit), _selectedItems[1]);
          break;
      }

      base.UpdateUIFromSelectedItems();
    }
  }
}
