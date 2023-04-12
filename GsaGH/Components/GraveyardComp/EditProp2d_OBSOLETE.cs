using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using GH_IO.Serialization;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using GsaAPI;
using GsaGH.Helpers.GH;
using GsaGH.Helpers.GsaApi;
using GsaGH.Parameters;
using GsaGH.Properties;
using OasysGH;
using OasysGH.Components;
using OasysGH.Helpers;
using OasysGH.Parameters;
using OasysGH.Units;
using OasysGH.Units.Helpers;
using OasysUnits;
using OasysUnits.Units;

namespace GsaGH.Components {
  /// <summary>
  ///   Component to edit a Prop2d and ouput the information
  /// </summary>
  // ReSharper disable once InconsistentNaming
  public class EditProp2d_OBSOLETE : GH_OasysComponent {
    public override Guid ComponentGuid => new Guid("ab8af109-7ebc-4e49-9f5d-d4cb8ee45557");
    public override GH_Exposure Exposure => GH_Exposure.hidden;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    protected override Bitmap Icon => Resources.EditProp2d;
    private LengthUnit _lengthUnit = DefaultUnits.LengthUnitSection;

    public EditProp2d_OBSOLETE() : base("Edit 2D Property",
              "Prop2dEdit",
      "Modify GSA 2D Property",
      CategoryName.Name(),
      SubCategoryName.Cat1())
      => Hidden = true;

    public override void AppendAdditionalMenuItems(ToolStripDropDown menu) {
      Menu_AppendSeparator(menu);

      var unitsMenu = new ToolStripMenuItem("Select unit", Resources.Units) {
        Enabled = true,
        ImageScaling = ToolStripItemImageScaling.SizeToFit,
      };
      foreach (string unit in UnitsHelper.GetFilteredAbbreviations(EngineeringUnits.Length)) {
        var toolStripMenuItem = new ToolStripMenuItem(unit, null, (s, e) => { Update(unit); }) {
          Checked = unit == Length.GetAbbreviation(_lengthUnit),
          Enabled = true,
        };
        unitsMenu.DropDownItems.Add(toolStripMenuItem);
      }

      menu.Items.Add(unitsMenu);

      Menu_AppendSeparator(menu);
    }

    public override bool Read(GH_IReader reader) {
      if (reader.ItemExists("LengthUnit"))
        _lengthUnit
          = (LengthUnit)UnitsHelper.Parse(typeof(LengthUnit), reader.GetString("LengthUnit"));
      else
        _lengthUnit = DefaultUnits.LengthUnitSection;
      return base.Read(reader);
    }

    public override bool Write(GH_IWriter writer) {
      writer.SetString("LengthUnit", _lengthUnit.ToString());
      return base.Write(writer);
    }

    protected override void BeforeSolveInstance() => Message = Length.GetAbbreviation(_lengthUnit);

    protected override void RegisterInputParams(GH_InputParamManager pManager) {
      pManager.AddParameter(new GsaProp2dParameter(),
        GsaProp2dGoo.Name,
        GsaProp2dGoo.NickName,
        GsaProp2dGoo.Description
        + " to get or set information for. Leave blank to create a new "
        + GsaProp2dGoo.Name,
        GH_ParamAccess.item);
      pManager.AddIntegerParameter("Prop2d Number",
        "ID",
        "Set 2D Property Number. If ID is set it will replace any existing 2D Property in the model",
        GH_ParamAccess.item);
      pManager.AddParameter(new GsaMaterialParameter());
      pManager.AddGenericParameter("Thickness [" + Length.GetAbbreviation(_lengthUnit) + "]",
        "Th",
        "Set Property Thickness",
        GH_ParamAccess.item);
      pManager.AddIntegerParameter("Axis",
        "Ax",
        "Set Axis as integer: Global (0) or Topological (1)",
        GH_ParamAccess.item);
      pManager.AddTextParameter("Prop2d Name", "Na", "Set Name of 2D Proerty", GH_ParamAccess.item);
      pManager.AddColourParameter("Prop2d Colour",
        "Co",
        "Set 2D Property Colour",
        GH_ParamAccess.item);
      pManager.AddTextParameter("Type",
        "Ty",
        "Set 2D Property Type."
        + Environment.NewLine
        + "Input either text string or integer:"
        + Environment.NewLine
        + "Plane Stress : 1"
        + Environment.NewLine
        + "Plane Strain : 2"
        + Environment.NewLine
        + "Axis Symmetric : 3"
        + Environment.NewLine
        + "Fabric : 4"
        + Environment.NewLine
        + "Plate : 5"
        + Environment.NewLine
        + "Shell : 6"
        + Environment.NewLine
        + "Curved Shell : 7"
        + Environment.NewLine
        + "Torsion : 8"
        + Environment.NewLine
        + "Wall : 9"
        + Environment.NewLine
        + "Load : 10",
        GH_ParamAccess.item);
      for (int i = 0; i < pManager.ParamCount; i++)
        pManager[i]
          .Optional = true;
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager) {
      pManager.AddParameter(new GsaProp2dParameter(),
        GsaProp2dGoo.Name,
        GsaProp2dGoo.NickName,
        GsaProp2dGoo.Description + " with applied changes.",
        GH_ParamAccess.item);
      pManager.AddIntegerParameter("Prop2d Number",
        "ID",
        "2D Property Number",
        GH_ParamAccess.item);
      pManager.AddParameter(new GsaMaterialParameter());
      pManager.AddGenericParameter("Thickness [" + Length.GetAbbreviation(_lengthUnit) + "]",
        "Th",
        "Get Property Thickness",
        GH_ParamAccess.item);
      pManager.AddIntegerParameter("Axis",
        "Ax",
        "Get Axis: Global (0) or Topological (1)",
        GH_ParamAccess.item);
      pManager.AddTextParameter("Prop2d Name", "Na", "Name of 2D Proerty", GH_ParamAccess.item);
      pManager.AddColourParameter("Prop2d Colour", "Co", "2D Property Colour", GH_ParamAccess.item);
      pManager.AddTextParameter("Type", "Ty", "2D Property Type", GH_ParamAccess.item);
    }

    protected override void SolveInstance(IGH_DataAccess da) {
      var gsaProp2d = new GsaProp2d();
      var prop = new GsaProp2d();
      if (da.GetData(0, ref gsaProp2d))
        prop = gsaProp2d.Duplicate();

      if (prop != null) {
        var ghId = new GH_Integer();
        if (da.GetData(1, ref ghId))
          if (GH_Convert.ToInt32(ghId, out int id, GH_Conversion.Both))
            prop.Id = id;

        var ghTyp = new GH_ObjectWrapper();
        if (da.GetData(2, ref ghTyp)) {
          var material = new GsaMaterial();
          if (ghTyp.Value is GsaMaterialGoo) {
            ghTyp.CastTo(ref material);
            prop.Material = material;
          }
          else {
            if (GH_Convert.ToInt32(ghTyp.Value, out int idd, GH_Conversion.Both))
              prop.MaterialId = idd;
            else {
              this.AddRuntimeError(
                "Unable to convert PB input to a Section Property of reference integer");
              return;
            }
          }
        }

        if (Params.Input[3]
            .SourceCount
          > 0)
          prop.Thickness = (Length)Input.UnitNumber(this, da, 3, _lengthUnit, true);

        var ghAxis = new GH_Integer();
        if (da.GetData(4, ref ghAxis))
          if (GH_Convert.ToInt32(ghAxis, out int axis, GH_Conversion.Both))
            prop.AxisProperty = axis;

        var ghName = new GH_String();
        if (da.GetData(5, ref ghName))
          if (GH_Convert.ToString(ghName, out string name, GH_Conversion.Both))
            prop.Name = name;

        var ghColour = new GH_Colour();
        if (da.GetData(6, ref ghColour))
          if (GH_Convert.ToColor(ghColour, out Color col, GH_Conversion.Both))
            prop.Colour = col;

        var ghType = new GH_ObjectWrapper();
        if (da.GetData(7, ref ghType)) {
          if (GH_Convert.ToInt32(ghType, out int number, GH_Conversion.Both))
            prop.Type = (Property2D_Type)number;
          else if (GH_Convert.ToString(ghType, out string type, GH_Conversion.Both))
            prop.Type = GsaProp2d.PropTypeFromString(type);
        }

        int ax = (prop.ApiProp2d == null)
          ? 0
          : prop.AxisProperty;
        string nm = (prop.ApiProp2d == null)
          ? "--"
          : prop.Name;
        ValueType colour = prop.ApiProp2d?.Colour;

        da.SetData(0, new GsaProp2dGoo(prop));
        da.SetData(1, prop.Id);
        da.SetData(2, new GsaMaterialGoo(new GsaMaterial(prop)));
        da.SetData(3,
          prop.ApiProp2d?.Description == ""
            ? new GH_UnitNumber(Length.Zero)
            : new GH_UnitNumber(prop.Thickness.ToUnit(_lengthUnit)));
        da.SetData(4, ax);
        da.SetData(5, nm);
        da.SetData(6, colour);

        da.SetData(7,
          Mappings.s_prop2dTypeMapping.FirstOrDefault(x => x.Value == prop.Type)
            .Key);
      }
      else
        this.AddRuntimeError("Prop2d is Null");
    }

    private void Update(string unit) {
      _lengthUnit = (LengthUnit)UnitsHelper.Parse(typeof(LengthUnit), unit);
      Message = unit;
      ExpireSolution(true);
    }
  }
}
