using System;
using System.Drawing;
using System.Linq;
using System.Runtime.Remoting.Messaging;
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

using Rhino.Geometry;

using LengthUnit = OasysUnits.Units.LengthUnit;

namespace GsaGH.Components {
  /// <summary>
  ///   Component to edit a Prop2d and ouput the information
  /// </summary>
  public class Edit2dProperty : GH_OasysComponent, IGH_VariableParameterComponent {
    public override Guid ComponentGuid => new Guid("ff7d1bb6-1d74-4393-a090-6f0fc083b853");
    public override GH_Exposure Exposure => GH_Exposure.quinary;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    protected override Bitmap Icon => Resources.Edit2dProperty;
    private LengthUnit _lengthUnit = DefaultUnits.LengthUnitSection;
    private int _referenceEdge;
    private int _supportTypeIndex;

    public Edit2dProperty() : base("Edit 2D Property", "EditPA", "Modify GSA 2D Property",
      CategoryName.Name(), SubCategoryName.Cat1()) {
      Hidden = true;
    }

    public override void AppendAdditionalMenuItems(ToolStripDropDown menu) {
      if (!(menu is ContextMenuStrip)) {
        return; // this method is also called when clicking EWR balloon
      }

      Menu_AppendSeparator(menu);

      var unitsMenu = new ToolStripMenuItem("Select unit", Resources.ModelUnits) {
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
      Params.Input[7].Name = "Thickness [" + Length.GetAbbreviation(_lengthUnit) + "]";
      Params.Output[7].Name = "Thickness [" + Length.GetAbbreviation(_lengthUnit) + "]";
      Params.Input[9].Name = "Offset [" + Length.GetAbbreviation(_lengthUnit) + "]";
      Params.Output[9].Name = "Offset [" + Length.GetAbbreviation(_lengthUnit) + "]";
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
      pManager.AddParameter(new GsaProperty2dParameter(), GsaProperty2dGoo.Name, GsaProperty2dGoo.NickName,
        GsaProperty2dGoo.Description + " to get or set information for. Leave blank to create a new "
        + GsaProperty2dGoo.Name, GH_ParamAccess.item);
      pManager.AddIntegerParameter("Prop2d Number", "ID",
        "Set 2D Property Number. If ID is set it will replace any existing 2D Property in the model",
        GH_ParamAccess.item);
      pManager.AddTextParameter("Prop2d Name", "Na", "Set Name of 2D Proerty", GH_ParamAccess.item);
      pManager.AddColourParameter("Prop2d Colour", "Co", "Set 2D Property Colour", GH_ParamAccess.item);
      pManager.AddGenericParameter("Axis", "Ax",
        "Input a Plane to set a custom Axis or input an integer (Global (0) or Topological (-1)) to reference a predefined Axis in the model",
        GH_ParamAccess.item);
      pManager.AddTextParameter("Type", "Ty",
        "Set 2D Property Type." + Environment.NewLine + "Input either text string or integer:"
        + Environment.NewLine + "Plane Stress : 1" + Environment.NewLine + "Plane Strain : 2"
        + Environment.NewLine + "Axis Symmetric : 3" + Environment.NewLine + "Fabric : 4"
        + Environment.NewLine + "Plate : 5" + Environment.NewLine + "Shell : 6"
        + Environment.NewLine + "Curved Shell : 7" + Environment.NewLine + "Torsion : 8"
        + Environment.NewLine + "Wall : 9" + Environment.NewLine + "Load : 10",
        GH_ParamAccess.item);
      pManager.AddParameter(new GsaMaterialParameter());
      pManager.AddGenericParameter("Thickness [" + Length.GetAbbreviation(_lengthUnit) + "]", "Th",
        "Set Property Thickness", GH_ParamAccess.item);
      pManager.AddGenericParameter("Reference Surface", "RS",
        "Reference Surface Middle (default) = 0, Top = 1, Bottom = 2", GH_ParamAccess.item);
      pManager.AddGenericParameter($"Offset [{Length.GetAbbreviation(_lengthUnit)}]", "Off", "Additional Offset",
        GH_ParamAccess.item);
      pManager.AddParameter(new GsaProperty2dModifierParameter());
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

      for (int i = 0; i < pManager.ParamCount; i++) {
        pManager[i].Optional = true;
      }
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager) {
      pManager.AddParameter(new GsaProperty2dParameter(), GsaProperty2dGoo.Name, GsaProperty2dGoo.NickName,
        GsaProperty2dGoo.Description + " with applied changes.", GH_ParamAccess.item);
      pManager.AddIntegerParameter("Prop2d ID", "ID", "2D Property ID", GH_ParamAccess.item);
      pManager.AddTextParameter("Name", "Na", "Name of 2D Proerty", GH_ParamAccess.item);
      pManager.AddColourParameter("Colour", "Co", "2D Property Colour", GH_ParamAccess.item);
      pManager.AddGenericParameter("Axis", "Ax",
        "Get Local Axis either as `Plane` for custom local axis or an `Integer` (Global: 0 or " +
        "Topological: 1) for a referenced Axis.",
        GH_ParamAccess.item);
      pManager.AddTextParameter("Type", "Ty", "2D Property Type", GH_ParamAccess.item);
      pManager.AddParameter(new GsaMaterialParameter());
      pManager.AddGenericParameter("Thickness [" + Length.GetAbbreviation(_lengthUnit) + "]", "Th",
        "Get Property Thickness", GH_ParamAccess.item);
      pManager.AddGenericParameter("Reference Surface", "RS",
        "Reference Surface Middle (default) = 0, Top = 1, Bottom = 2", GH_ParamAccess.item);
      pManager.AddGenericParameter($"Offset [{Length.GetAbbreviation(_lengthUnit)}]", "Off",
        "Additional Offset", GH_ParamAccess.item);
      pManager.AddParameter(new GsaProperty2dModifierParameter());
      pManager.AddGenericParameter("Support Type", "ST", "Support Type", GH_ParamAccess.item);
      pManager.AddIntegerParameter("Reference Edge", "RE",
        "Reference Edge for Load Panels with support type other than Auto and All Edges",
        GH_ParamAccess.item);
    }

    protected override void SolveInstance(IGH_DataAccess da) {
      var prop = new GsaProperty2d();

      GsaProperty2dGoo prop2dGoo = null;
      if (da.GetData(0, ref prop2dGoo)) {
        prop = new GsaProperty2d(prop2dGoo.Value);
      }

      int id = 0;
      if (da.GetData(1, ref id)) {
        prop.Id = id;
      }

      string name = string.Empty;
      if (da.GetData(2, ref name)) {
        prop.ApiProp2d.Name = name;
      }

      Color colour = Color.Empty;
      if (da.GetData(3, ref colour)) {
        prop.ApiProp2d.Colour = colour;
      }

      var ghPlaneOrInt = new GH_ObjectWrapper();
      if (da.GetData(4, ref ghPlaneOrInt)) {
        var pln = new Plane();
        if (ghPlaneOrInt.Value.GetType() == typeof(GH_Plane)) {
          if (GH_Convert.ToPlane(ghPlaneOrInt.Value, ref pln, GH_Conversion.Both)) {
            prop.LocalAxis = pln;
          }
        } else if (GH_Convert.ToInt32(ghPlaneOrInt.Value, out int axis, GH_Conversion.Both)) {

          if (prop.ApiProp2d.Type == Property2D_Type.LOAD && axis != 0) {
            AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, $"Load panel property should be in global axis(Ax = 0)");
            return;
          }
          prop.ApiProp2d.AxisProperty = axis;
        }
      }

      // first we need to set type then if load
      // we can set support Type and then if not load support type
      // we can set reference egde
      GH_ObjectWrapper ghType = null;
      if (da.GetData(5, ref ghType)) {
        if (GH_Convert.ToInt32(ghType, out int number, GH_Conversion.Both)) {
          prop.ApiProp2d.Type = (Property2D_Type)number;
        } else if (GH_Convert.ToString(ghType, out string type, GH_Conversion.Both)) {
          prop.ApiProp2d.Type = GsaProperty2d.PropTypeFromString(type);
        }
      }

      GsaMaterialGoo materialGoo = null;
      if (da.GetData(6, ref materialGoo)) {
        prop.Material = materialGoo.Value;
      }

      if (Params.Input[7].SourceCount > 0) {
        prop.Thickness = (Length)Input.UnitNumber(this, da, 7, _lengthUnit, true);
      }

      var ghReferenceSurface = new GH_ObjectWrapper();
      if (da.GetData("Reference Surface", ref ghReferenceSurface)) {
        try {
          if (GH_Convert.ToInt32(ghReferenceSurface.Value, out int reference, GH_Conversion.Both)) {
            prop.ApiProp2d.ReferenceSurface = (ReferenceSurface)reference;
          } else if (GH_Convert.ToString(ghReferenceSurface, out string value, GH_Conversion.Both)) {
            prop.ApiProp2d.ReferenceSurface = (ReferenceSurface)Enum.Parse(typeof(ReferenceSurface), value, ignoreCase: true);
          }
        } catch {
          this.AddRuntimeError("Unable to convert input " + ghReferenceSurface.Value +
            " to a Reference Surface (Middle = 0, Top = 1, Bottom = 2)");
          return;
        }
      }

      if (Params.Input[9].SourceCount > 0) {
        prop.AdditionalOffsetZ = (Length)Input.UnitNumber(this, da, 9, _lengthUnit, true);
      }

      GsaProperty2dModifierGoo modifierGoo = null;
      if (da.GetData(10, ref modifierGoo)) {
        if (modifierGoo.Value.InPlane is Length inPlaneQuantity) {
          prop.ApiProp2d.PropertyModifier.InPlane = new Prop2DModifierAttribute(Prop2DModifierOptionType.TO, inPlaneQuantity.As(LengthUnit.Meter));
        } else if (modifierGoo.Value.InPlane is Ratio ratio) {
          prop.ApiProp2d.PropertyModifier.InPlane = new Prop2DModifierAttribute(Prop2DModifierOptionType.BY, ratio.As(RatioUnit.DecimalFraction));
        }

        if (modifierGoo.Value.Bending is Volume bendingQuantity) {
          prop.ApiProp2d.PropertyModifier.Bending = new Prop2DModifierAttribute(Prop2DModifierOptionType.TO, bendingQuantity.As(VolumeUnit.CubicMeter));
        } else if (modifierGoo.Value.Bending is Ratio ratio) {
          prop.ApiProp2d.PropertyModifier.Bending = new Prop2DModifierAttribute(Prop2DModifierOptionType.BY, ratio.As(RatioUnit.DecimalFraction));
        }

        if (modifierGoo.Value.Shear is Length shearQuantity) {
          prop.ApiProp2d.PropertyModifier.Shear = new Prop2DModifierAttribute(Prop2DModifierOptionType.TO, shearQuantity.As(LengthUnit.Meter));
        } else if (modifierGoo.Value.Shear is Ratio ratio) {
          prop.ApiProp2d.PropertyModifier.Shear = new Prop2DModifierAttribute(Prop2DModifierOptionType.BY, ratio.As(RatioUnit.DecimalFraction));
        }

        if (modifierGoo.Value.Volume is Length volumeQuantity) {
          prop.ApiProp2d.PropertyModifier.Volume = new Prop2DModifierAttribute(Prop2DModifierOptionType.TO, volumeQuantity.As(LengthUnit.Meter));
        } else if (modifierGoo.Value.Volume is Ratio ratio) {
          prop.ApiProp2d.PropertyModifier.Volume = new Prop2DModifierAttribute(Prop2DModifierOptionType.BY, ratio.As(RatioUnit.DecimalFraction));
        }

        prop.ApiProp2d.PropertyModifier.AdditionalMass = modifierGoo.Value.AdditionalMass.As(AreaDensityUnit.KilogramPerSquareMeter);
      } else {
        prop.ApiProp2d.PropertyModifier.InPlane = new Prop2DModifierAttribute(Prop2DModifierOptionType.BY, 1);
        prop.ApiProp2d.PropertyModifier.Bending = new Prop2DModifierAttribute(Prop2DModifierOptionType.BY, 1);
        prop.ApiProp2d.PropertyModifier.Shear = new Prop2DModifierAttribute(Prop2DModifierOptionType.BY, 1);
        prop.ApiProp2d.PropertyModifier.Volume = new Prop2DModifierAttribute(Prop2DModifierOptionType.BY, 1);
        prop.ApiProp2d.PropertyModifier.AdditionalMass = 0;
      }

      GH_ObjectWrapper ghSupportType = null;
      if (da.GetData(11, ref ghSupportType)) {
        if (ghSupportType.Value is GH_Integer supportTypeIndex) {
          prop.ApiProp2d.SupportType = (SupportType)supportTypeIndex.Value;
        } else if (GH_Convert.ToString(ghSupportType.Value, out string supportTypeName,
          GH_Conversion.Both)) {
          supportTypeName = supportTypeName.Replace(" ", string.Empty).Replace("1", "One")
           .Replace("2", "Two").Replace("3", "Three");
          supportTypeName = supportTypeName.Replace("all", "All").Replace("adj", "Adj")
           .Replace("auto", "Auto").Replace("edge", "Edge").Replace("cant", "Cant");
          prop.ApiProp2d.SupportType = (SupportType)Enum.Parse(typeof(SupportType), supportTypeName);
        } else {
          this.AddRuntimeError("Cannot convert support type to 'int' or 'string'");
        }
      }

      int refEdge = 0;
      if (da.GetData(12, ref refEdge)) {
        prop.ApiProp2d.ReferenceEdge = refEdge;
      }

      int ax = (prop.ApiProp2d == null) ? 0 : prop.ApiProp2d.AxisProperty;
      string nm = (prop.ApiProp2d == null) ? "--" : prop.ApiProp2d.Name;

      da.SetData(0, new GsaProperty2dGoo(prop));
      da.SetData(1, prop.Id);
      da.SetData(2, nm);
      da.SetData(3, prop.ApiProp2d?.Colour);
      if (prop.LocalAxis != null && prop.LocalAxis.IsValid) {
        da.SetData(4, new GH_Plane(prop.LocalAxis));
      } else {
        da.SetData(4, ax);
      }
      da.SetData(5, Mappings._prop2dTypeMapping.FirstOrDefault(x => x.Value == prop.ApiProp2d.Type).Key);
      da.SetData(6, new GsaMaterialGoo(prop.Material));
      da.SetData(7,
        prop.ApiProp2d.Description == string.Empty ? new GH_UnitNumber(Length.Zero) :
          new GH_UnitNumber(prop.Thickness.ToUnit(_lengthUnit)));
      da.SetData(8, prop.ApiProp2d.ReferenceSurface);
      da.SetData(9, prop.AdditionalOffsetZ.ToUnit(_lengthUnit));
      da.SetData(10, new GsaProperty2dModifierGoo(
        new GsaProperty2dModifier(prop.ApiProp2d.PropertyModifier)));
      da.SetData(11, prop.ApiProp2d.SupportType);
      da.SetData(12, prop.ApiProp2d.SupportType != SupportType.Auto
        ? prop.ApiProp2d.ReferenceEdge : -1);
    }

    internal void Update(string unit) {
      _lengthUnit = (LengthUnit)UnitsHelper.Parse(typeof(LengthUnit), unit);
      Message = unit;
      (this as IGH_VariableParameterComponent).VariableParameterMaintenance();
      ExpireSolution(true);
    }
  }
}
