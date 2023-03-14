using System;
using System.Linq;
using System.Windows.Forms;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using GsaAPI;
using GsaGH.Helpers.GH;
using GsaGH.Helpers.GsaAPI;
using GsaGH.Parameters;
using OasysGH;
using OasysGH.Components;
using OasysGH.Helpers;
using OasysGH.Parameters;
using OasysGH.Units;
using OasysGH.Units.Helpers;
using OasysUnits;
using OasysUnits.Units;
using Rhino.Geometry;

namespace GsaGH.Components {
  /// <summary>
  /// Component to edit a Prop2d and ouput the information
  /// </summary>
  public class EditProp2d : GH_OasysComponent, IGH_VariableParameterComponent {
    #region Name and Ribbon Layout
    // This region handles how the component in displayed on the ribbon including name, exposure level and icon
    public override Guid ComponentGuid => new Guid("dfb17a0f-a856-4a54-ae5c-d794961f3c52");
    public override GH_Exposure Exposure => GH_Exposure.tertiary;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    protected override System.Drawing.Bitmap Icon => Properties.Resources.EditProp2d;

    public EditProp2d() : base("Edit 2D Property",
      "Prop2dEdit",
      "Modify GSA 2D Property",
      CategoryName.Name(),
      SubCategoryName.Cat1()) {
        Hidden = true;
    } // sets the initial state of the component to hidden
    #endregion

    #region Input and output
    protected override void RegisterInputParams(GH_InputParamManager pManager) {
      pManager.AddParameter(new GsaProp2dParameter(), GsaProp2dGoo.Name, GsaProp2dGoo.NickName, GsaProp2dGoo.Description + " to get or set information for. Leave blank to create a new " + GsaProp2dGoo.Name, GH_ParamAccess.item);
      pManager.AddIntegerParameter("Prop2d Number", "ID", "Set 2D Property Number. If ID is set it will replace any existing 2D Property in the model", GH_ParamAccess.item);
      pManager.AddParameter(new GsaMaterialParameter());
      pManager.AddGenericParameter("Thickness [" + Length.GetAbbreviation(_lengthUnit) + "]", "Th", "Set Property Thickness", GH_ParamAccess.item);
      pManager.AddGenericParameter("Axis", "Ax", "Input a Plane to set a custom Axis or input an integer (Global (0) or Topological (-1)) to reference a predefined Axis in the model", GH_ParamAccess.item);
      pManager.AddTextParameter("Prop2d Name", "Na", "Set Name of 2D Proerty", GH_ParamAccess.item);
      pManager.AddColourParameter("Prop2d Colour", "Co", "Set 2D Property Colour", GH_ParamAccess.item);
      pManager.AddTextParameter("Type", "Ty", "Set 2D Property Type." + Environment.NewLine +
          "Input either text string or integer:"
          + Environment.NewLine + "Plane Stress : 1"
          + Environment.NewLine + "Plane Strain : 2"
          + Environment.NewLine + "Axis Symmetric : 3"
          + Environment.NewLine + "Fabric : 4"
          + Environment.NewLine + "Plate : 5"
          + Environment.NewLine + "Shell : 6"
          + Environment.NewLine + "Curved Shell : 7"
          + Environment.NewLine + "Torsion : 8"
          + Environment.NewLine + "Wall : 9"
          + Environment.NewLine + "Load : 10",
          GH_ParamAccess.item);
      for (int i = 0; i < pManager.ParamCount; i++)
        pManager[i].Optional = true;
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager) {
      pManager.AddParameter(new GsaProp2dParameter(), GsaProp2dGoo.Name, GsaProp2dGoo.NickName, GsaProp2dGoo.Description + " with applied changes.", GH_ParamAccess.item);
      pManager.AddIntegerParameter("Prop2d Number", "ID", "2D Property Number", GH_ParamAccess.item);
      pManager.AddParameter(new GsaMaterialParameter());
      pManager.AddGenericParameter("Thickness [" + Length.GetAbbreviation(_lengthUnit) + "]", "Th", "Get Property Thickness", GH_ParamAccess.item);
      pManager.AddGenericParameter("Axis", "Ax", "Get Local Axis either as Plane for custom or an integer (Global (0) or Topological (1)) for referenced Axis.", GH_ParamAccess.item);
      pManager.AddTextParameter("Prop2d Name", "Na", "Name of 2D Proerty", GH_ParamAccess.item);
      pManager.AddColourParameter("Prop2d Colour", "Co", "2D Property Colour", GH_ParamAccess.item);
      pManager.AddTextParameter("Type", "Ty", "2D Property Type", GH_ParamAccess.item);
    }
    #endregion

    protected override void SolveInstance(IGH_DataAccess da) {
      var gsaProp2d = new GsaProp2d();
      var prop = new GsaProp2d();
      if (da.GetData(0, ref gsaProp2d)) {
        prop = gsaProp2d.Duplicate();
      }

      if (prop != null) {
        // #### inputs ####
        // 1 ID
        var ghId = new GH_Integer();
        if (da.GetData(1, ref ghId)) {
          if (GH_Convert.ToInt32(ghId, out int id, GH_Conversion.Both))
            prop.Id = id;
        }

        // 2 Material
        var ghTyp = new GH_ObjectWrapper();
        if (da.GetData(2, ref ghTyp)) {
          if (ghTyp.Value is GsaMaterialGoo) {
            ghTyp.CastTo(out GsaMaterial material);
            prop.Material = material ?? new GsaMaterial();
          }
          else {
            if (GH_Convert.ToInt32(ghTyp.Value, out int idd, GH_Conversion.Both))
              prop.MaterialID = idd;
            else {
              this.AddRuntimeError("Unable to convert PB input to a Section Property of reference integer");
              return;
            }
          }
        }

        // 3 Thickness
        if (Params.Input[3].SourceCount > 0)
          prop.Thickness = (Length)Input.UnitNumber(this, da, 3, _lengthUnit, true);

        // 4 Axis
        var ghax = new GH_ObjectWrapper();
        if (da.GetData(4, ref ghax)) {
          var pln = new Plane();
          if (ghax.Value.GetType() == typeof(GH_Plane)) {
            if (GH_Convert.ToPlane(ghax.Value, ref pln, GH_Conversion.Both))
              prop.LocalAxis = pln;
          }
          else if (GH_Convert.ToInt32(ghax.Value, out int axis, GH_Conversion.Both))
            prop.AxisProperty = axis;
        }

        // 5 name
        var ghnm = new GH_String();
        if (da.GetData(5, ref ghnm)) {
          if (GH_Convert.ToString(ghnm, out string name, GH_Conversion.Both))
            prop.Name = name;
        }

        // 6 Colour
        var ghcol = new GH_Colour();
        if (da.GetData(6, ref ghcol)) {
          if (GH_Convert.ToColor(ghcol, out System.Drawing.Color col, GH_Conversion.Both))
            prop.Colour = col;
        }

        // 7 type
        var ghType = new GH_ObjectWrapper();
        if (da.GetData(7, ref ghType)) {
          if (GH_Convert.ToInt32(ghType, out int number, GH_Conversion.Both))
            prop.Type = (Property2D_Type)number;
          else if (GH_Convert.ToString(ghType, out string type, GH_Conversion.Both))
            prop.Type = GsaProp2d.PropTypeFromString(type);
        }

        //#### outputs ####
        int ax = (prop.API_Prop2d == null) ? 0 : prop.AxisProperty;
        string nm = (prop.API_Prop2d == null) ? "--" : prop.Name;
        ValueType colour = prop.API_Prop2d?.Colour;

        da.SetData(0, new GsaProp2dGoo(prop));
        da.SetData(1, prop.Id);
        da.SetData(2, new GsaMaterialGoo(prop.Material));
        da.SetData(3,
          prop.API_Prop2d.Description == ""
            ? new GH_UnitNumber(Length.Zero)
            : new GH_UnitNumber(prop.Thickness.ToUnit(_lengthUnit)));
        if (prop.AxisProperty == -2)
          da.SetData(4, new GH_Plane(prop.LocalAxis));
        else
          da.SetData(4, ax);
        da.SetData(5, nm);
        da.SetData(6, colour);

        da.SetData(7, Mappings.Prop2dTypeMapping.FirstOrDefault(x => x.Value == prop.Type).Key);
      }
      else
        this.AddRuntimeError("Prop2d is Null");
    }

    #region Custom UI
    protected override void BeforeSolveInstance() {
      Message = Length.GetAbbreviation(_lengthUnit);
    }

    private LengthUnit _lengthUnit = DefaultUnits.LengthUnitSection;
    public override void AppendAdditionalMenuItems(ToolStripDropDown menu) {
      Menu_AppendSeparator(menu);

      var unitsMenu = new ToolStripMenuItem("Select unit", Properties.Resources.Units) {
        Enabled = true,
        ImageScaling = ToolStripItemImageScaling.SizeToFit,
      };
      foreach (string unit in UnitsHelper.GetFilteredAbbreviations(EngineeringUnits.Length)) {
        var toolStripMenuItem = new ToolStripMenuItem(unit, null, (s, e) => { Update(unit); })
        {
          Checked = unit == Length.GetAbbreviation(_lengthUnit),
          Enabled = true,
        };
        unitsMenu.DropDownItems.Add(toolStripMenuItem);
      }

      menu.Items.Add(unitsMenu);

      Menu_AppendSeparator(menu);
    }
    private void Update(string unit) {
      _lengthUnit = (LengthUnit)UnitsHelper.Parse(typeof(LengthUnit), unit);
      Message = unit;
      (this as IGH_VariableParameterComponent).VariableParameterMaintenance();
      ExpireSolution(true);
    }
    public override bool Write(GH_IO.Serialization.GH_IWriter writer) {
      writer.SetString("LengthUnit", _lengthUnit.ToString());
      return base.Write(writer);
    }
    public override bool Read(GH_IO.Serialization.GH_IReader reader) {
      _lengthUnit = (LengthUnit)UnitsHelper.Parse(typeof(LengthUnit), reader.GetString("LengthUnit"));
      return base.Read(reader);
    }

    #region IGH_VariableParameterComponent null implementation
    public virtual void VariableParameterMaintenance() {
      Params.Input[3].Name = "Thickness [" + Length.GetAbbreviation(_lengthUnit) + "]";
      Params.Output[3].Name = "Thickness [" + Length.GetAbbreviation(_lengthUnit) + "]";
    }

    bool IGH_VariableParameterComponent.CanInsertParameter(GH_ParameterSide side, int index) => false;
    bool IGH_VariableParameterComponent.CanRemoveParameter(GH_ParameterSide side, int index) => false;
    IGH_Param IGH_VariableParameterComponent.CreateParameter(GH_ParameterSide side, int index) => null;
    bool IGH_VariableParameterComponent.DestroyParameter(GH_ParameterSide side, int index) => false;
    #endregion
    #endregion
  }
}
