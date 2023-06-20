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
using Rhino.Geometry;
using LengthUnit = OasysUnits.Units.LengthUnit;

namespace GsaGH.Components {
  /// <summary>
  ///   Component to edit a Prop2d and ouput the information
  /// </summary>
  public class EditProp2d : GH_OasysComponent, IGH_VariableParameterComponent {
    public override Guid ComponentGuid => new Guid("5e7627a2-9c3c-4ace-9a08-f9c9a7106755");
    public override GH_Exposure Exposure => GH_Exposure.tertiary;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    protected override Bitmap Icon => Resources.EditProp2d;
    private LengthUnit _lengthUnit = DefaultUnits.LengthUnitSection;
    private int _referenceEdge;
    private int _supportTypeIndex;

    public EditProp2d() : base("Edit 2D Property", "Prop2dEdit", "Modify GSA 2D Property",
      CategoryName.Name(), SubCategoryName.Cat1()) {
      Hidden = true;
    }

    public override void AppendAdditionalMenuItems(ToolStripDropDown menu) {
      if (!(menu is ContextMenuStrip)) {
        return; // this method is also called when clicking EWR balloon
      }

      Menu_AppendSeparator(menu);

      var unitsMenu = new ToolStripMenuItem("Select unit", Resources.Units) {
        Enabled = true,
        ImageScaling = ToolStripItemImageScaling.SizeToFit,
      };
      foreach (string unit in UnitsHelper.GetFilteredAbbreviations(EngineeringUnits.Length)) {
        var toolStripMenuItem = new ToolStripMenuItem(unit, null, (s, e) => Update(unit)) {
          Checked = unit == Length.GetAbbreviation(_lengthUnit),
          Enabled = true,
        };
        unitsMenu.DropDownItems.Add(toolStripMenuItem);
      }

      menu.Items.Add(unitsMenu);

      Menu_AppendSeparator(menu);
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

    public override bool Read(GH_IReader reader) {
      _lengthUnit
        = (LengthUnit)UnitsHelper.Parse(typeof(LengthUnit), reader.GetString("LengthUnit"));
      _supportTypeIndex = reader.GetInt32("SupportType");
      _referenceEdge = reader.GetInt32("ReferenceEdge");
      return base.Read(reader);
    }

    public virtual void VariableParameterMaintenance() {
      Params.Input[3].Name = "Thickness [" + Length.GetAbbreviation(_lengthUnit) + "]";
      Params.Output[3].Name = "Thickness [" + Length.GetAbbreviation(_lengthUnit) + "]";
    }

    public override bool Write(GH_IWriter writer) {
      writer.SetString("LengthUnit", _lengthUnit.ToString());
      writer.SetInt32("SupportType", _supportTypeIndex);
      writer.SetInt32("ReferenceEdge", _referenceEdge);
      return base.Write(writer);
    }

    protected override void BeforeSolveInstance() {
      Message = Length.GetAbbreviation(_lengthUnit);
    }

    protected override void RegisterInputParams(GH_InputParamManager pManager) {
      pManager.AddParameter(new GsaProp2dParameter(), GsaProp2dGoo.Name, GsaProp2dGoo.NickName,
        GsaProp2dGoo.Description + " to get or set information for. Leave blank to create a new "
        + GsaProp2dGoo.Name, GH_ParamAccess.item);
      pManager.AddIntegerParameter("Prop2d Number", "ID",
        "Set 2D Property Number. If ID is set it will replace any existing 2D Property in the model",
        GH_ParamAccess.item);
      pManager.AddParameter(new GsaMaterialParameter());
      pManager.AddGenericParameter("Thickness [" + Length.GetAbbreviation(_lengthUnit) + "]", "Th",
        "Set Property Thickness", GH_ParamAccess.item);
      pManager.AddGenericParameter("Axis", "Ax",
        "Input a Plane to set a custom Axis or input an integer (Global (0) or Topological (-1)) to reference a predefined Axis in the model",
        GH_ParamAccess.item);
      pManager.AddGenericParameter("Support Type", "ST",
        "Set Load Panel Support Type." + Environment.NewLine
        + "Input either text string or integer:" + Environment.NewLine + "Auto : 1"
        + Environment.NewLine + "All Edges : 2" + Environment.NewLine + "Three Edges : 3"
        + Environment.NewLine + "Two Edges : 4" + Environment.NewLine + "Two Adjacent Edges : 5"
        + Environment.NewLine + "One Edge : 6" + Environment.NewLine + "Cantilever : 7",
        GH_ParamAccess.item);
      pManager.AddIntegerParameter("Reference Edge", "RE",
        "Reference Edge for Load Panels with support type other than Auto and All Edges",
        GH_ParamAccess.item);

      pManager.AddTextParameter("Prop2d Name", "Na", "Set Name of 2D Proerty", GH_ParamAccess.item);
      pManager.AddColourParameter("Prop2d Colour", "Co", "Set 2D Property Colour",
        GH_ParamAccess.item);
      pManager.AddTextParameter("Type", "Ty",
        "Set 2D Property Type." + Environment.NewLine + "Input either text string or integer:"
        + Environment.NewLine + "Plane Stress : 1" + Environment.NewLine + "Plane Strain : 2"
        + Environment.NewLine + "Axis Symmetric : 3" + Environment.NewLine + "Fabric : 4"
        + Environment.NewLine + "Plate : 5" + Environment.NewLine + "Shell : 6"
        + Environment.NewLine + "Curved Shell : 7" + Environment.NewLine + "Torsion : 8"
        + Environment.NewLine + "Wall : 9" + Environment.NewLine + "Load : 10",
        GH_ParamAccess.item);
      pManager.AddGenericParameter("Reference Surface", "RS",
        "Reference Surface Middle = 0, Top = 1 (default), Bottom = 2", GH_ParamAccess.item);
      pManager.AddGenericParameter($"Offset [{Length.GetAbbreviation(_lengthUnit)}]", "Off", "Additional Offset",
        GH_ParamAccess.item);

      for (int i = 0; i < pManager.ParamCount; i++) {
        pManager[i].Optional = true;
      }
      // pManager.HideParameter(6);//hide reference edge
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager) {
      pManager.AddParameter(new GsaProp2dParameter(), GsaProp2dGoo.Name, GsaProp2dGoo.NickName,
        GsaProp2dGoo.Description + " with applied changes.", GH_ParamAccess.item);
      pManager.AddIntegerParameter("Prop2d Number", "ID", "2D Property Number",
        GH_ParamAccess.item);
      pManager.AddParameter(new GsaMaterialParameter());
      pManager.AddGenericParameter("Thickness [" + Length.GetAbbreviation(_lengthUnit) + "]", "Th",
        "Get Property Thickness", GH_ParamAccess.item);
      pManager.AddGenericParameter("Axis", "Ax",
        "Get Local Axis either as Plane for custom or an integer (Global (0) or Topological (1)) for referenced Axis.",
        GH_ParamAccess.item);
      pManager.AddGenericParameter("Support Type", "ST", "Support Type", GH_ParamAccess.item);
      pManager.AddIntegerParameter("Reference Edge", "RE",
        "Reference Edge for Load Panels with support type other than Auto and All Edges",
        GH_ParamAccess.item);
      pManager.AddTextParameter("Prop2d Name", "Na", "Name of 2D Proerty", GH_ParamAccess.item);
      pManager.AddColourParameter("Prop2d Colour", "Co", "2D Property Colour", GH_ParamAccess.item);
      pManager.AddTextParameter("Type", "Ty", "2D Property Type", GH_ParamAccess.item);
      pManager.AddGenericParameter("Reference Surface", "RS",
        "Reference Surface Middle = 0, Top = 1 (default), Bottom = 2", GH_ParamAccess.item);
      pManager.AddGenericParameter($"Offset [{Length.GetAbbreviation(_lengthUnit)}]", "Off", "Additional Offset",
        GH_ParamAccess.item);
    }

    protected override void SolveInstance(IGH_DataAccess da) {
      var prop = new GsaProp2d();

      GsaProp2dGoo prop2dGoo = null;
      if (da.GetData(0, ref prop2dGoo)) {
        prop = prop2dGoo.Value.Duplicate(true);
      }

      int id = 0;
      if (da.GetData(1, ref id)) {
        prop.Id = id;
      }

      GsaMaterialGoo materialGoo = null;
      if (da.GetData(2, ref materialGoo)) {
        prop.Material = materialGoo.Value;
      }

      if (Params.Input[3].SourceCount > 0) {
        prop.Thickness = (Length)Input.UnitNumber(this, da, 3, _lengthUnit, true);
      }

      var ghPlaneOrInt = new GH_ObjectWrapper();
      if (da.GetData(4, ref ghPlaneOrInt)) {
        var pln = new Plane();
        if (ghPlaneOrInt.Value.GetType() == typeof(GH_Plane)) {
          if (GH_Convert.ToPlane(ghPlaneOrInt.Value, ref pln, GH_Conversion.Both)) {
            prop.LocalAxis = pln;
          }
        } else if (GH_Convert.ToInt32(ghPlaneOrInt.Value, out int axis, GH_Conversion.Both)) {
          prop.AxisProperty = axis;
        }
      }

      // first we need to set type then if load
      // we can set support Type and then if not load support type
      // we can set reference egde
      GH_ObjectWrapper ghType = null;
      if (da.GetData(9, ref ghType)) {
        if (GH_Convert.ToInt32(ghType, out int number, GH_Conversion.Both)) {
          prop.Type = (Property2D_Type)number;
        } else if (GH_Convert.ToString(ghType, out string type, GH_Conversion.Both)) {
          prop.Type = GsaProp2d.PropTypeFromString(type);
        }
      }

      GH_ObjectWrapper ghSupportType = null;
      if (da.GetData(5, ref ghSupportType)) {
        if (ghSupportType.Value is GH_Integer supportTypeIndex) {
          prop.SupportType = (SupportType)supportTypeIndex.Value;
        } else if (GH_Convert.ToString(ghSupportType.Value, out string supportTypeName,
          GH_Conversion.Both)) {
          supportTypeName = supportTypeName.Replace(" ", string.Empty).Replace("1", "One")
           .Replace("2", "Two").Replace("3", "Three");
          supportTypeName = supportTypeName.Replace("all", "All").Replace("adj", "Adj")
           .Replace("auto", "Auto").Replace("edge", "Edge").Replace("cant", "Cant");
          prop.SupportType = (SupportType)Enum.Parse(typeof(SupportType), supportTypeName);
        } else {
          this.AddRuntimeError("Cannot convert support type to 'int' or 'string'");
        }
      }

      int refEdge = 0;
      if (da.GetData(6, ref refEdge)) {
        prop.ReferenceEdge = refEdge;
      }

      string name = string.Empty;
      if (da.GetData(7, ref name)) {
        prop.Name = name;
      }

      Color colour = Color.Empty;
      if (da.GetData(8, ref colour)) {
        prop.Colour = colour;
      }

      var ghReferenceSurface = new GH_ObjectWrapper();
      if (da.GetData("Reference Surface", ref ghReferenceSurface)) {
        try {
          if (GH_Convert.ToInt32(ghReferenceSurface.Value, out int reference, GH_Conversion.Both)) {
            prop.ReferenceSurface = (ReferenceSurface)reference;
          } else if (GH_Convert.ToString(ghReferenceSurface, out string value, GH_Conversion.Both)) {
            prop.ReferenceSurface = (ReferenceSurface)Enum.Parse(typeof(ReferenceSurface), value, ignoreCase: true);
          }
        } catch {
          this.AddRuntimeError("Unable to convert input " + ghReferenceSurface.Value + " to a Reference Surface (Middle = 0, Top = 1, Bottom = 2)");
          return;
        }
      }

      var ghOffset = new GH_Number();
      if (da.GetData(11, ref ghOffset)) {
        if (GH_Convert.ToDouble(ghOffset, out double offset, GH_Conversion.Both)) {
          prop.AdditionalOffsetZ = new Length(offset, _lengthUnit);
        }
      }

      int ax = (prop.ApiProp2d == null) ? 0 : prop.AxisProperty;
      string nm = (prop.ApiProp2d == null) ? "--" : prop.Name;

      da.SetData(0, new GsaProp2dGoo(prop));
      da.SetData(1, prop.Id);
      da.SetData(2, new GsaMaterialGoo(prop.Material));
      da.SetData(3,
        prop.ApiProp2d.Description == string.Empty ? new GH_UnitNumber(Length.Zero) :
          new GH_UnitNumber(prop.Thickness.ToUnit(_lengthUnit)));
      if (prop.AxisProperty == -2) {
        da.SetData(4, new GH_Plane(prop.LocalAxis));
      } else {
        da.SetData(4, ax);
      }

      da.SetData(5, prop.SupportType);
      da.SetData(6, prop.SupportType != SupportType.Auto ? prop.ReferenceEdge : -1);
      da.SetData(7, nm);
      da.SetData(8, prop.ApiProp2d?.Colour);
      da.SetData(9, Mappings.prop2dTypeMapping.FirstOrDefault(x => x.Value == prop.Type).Key);
      da.SetData(10, prop.ReferenceSurface);
      da.SetData(11, prop.AdditionalOffsetZ.ToUnit(_lengthUnit));
    }

    private void Update(string unit) {
      _lengthUnit = (LengthUnit)UnitsHelper.Parse(typeof(LengthUnit), unit);
      Message = unit;
      (this as IGH_VariableParameterComponent).VariableParameterMaintenance();
      ExpireSolution(true);
    }
  }
}
