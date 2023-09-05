using System;
using System.Drawing;
using System.Linq;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using GsaGH.Helpers.GH;
using GsaGH.Parameters;
using GsaGH.Properties;
using OasysGH;
using OasysGH.Components;
using OasysGH.Helpers;
using OasysGH.Parameters;
using OasysGH.Units;
using OasysUnits;

namespace GsaGH.Components {
  /// <summary>
  ///   Component to edit a Prop2d and ouput the information
  /// </summary>
  // ReSharper disable once InconsistentNaming
  public class EditProp2d2_OBSOLETE : GH_OasysComponent, IGH_PreviewObject {
    public override Guid ComponentGuid => new Guid("4cfdee19-451b-4ee3-878b-93a86767ffef");
    public override GH_Exposure Exposure => GH_Exposure.hidden;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    protected override Bitmap Icon => Resources.Edit2dProperty;

    public EditProp2d2_OBSOLETE() : base("Edit 2D Property", "Prop2dEdit", "Modify GSA 2D Property",
      CategoryName.Name(), SubCategoryName.Cat1()) {
      Hidden = true;
    }

    protected override void RegisterInputParams(GH_InputParamManager pManager) {
      IQuantity quantity = new Length(0, DefaultUnits.LengthUnitSection);
      string unitAbbreviation = string.Concat(quantity.ToString().Where(char.IsLetter));

      pManager.AddGenericParameter("2D Property", "PA",
        "GSA 2D Property to get or set information for", GH_ParamAccess.item);
      pManager.AddIntegerParameter("Prop2d Number", "ID",
        "Set 2D Property Number. If ID is set it will replace any existing 2D Property in the model",
        GH_ParamAccess.item);
      pManager.AddGenericParameter("Material", "Ma",
        "Set GSA Material or reference existing material by ID", GH_ParamAccess.item);
      pManager.AddGenericParameter("Thickness [" + unitAbbreviation + "]", "Th",
        "Set Property Thickness", GH_ParamAccess.item);
      pManager.AddIntegerParameter("Axis", "Ax",
        "Set Axis as integer: Global (0) or Topological (1)", GH_ParamAccess.item);
      pManager.AddTextParameter("Prop2d Name", "Na", "Set Name of 2D Proerty", GH_ParamAccess.item);
      pManager.AddColourParameter("Prop2d Colour", "Co", "Set 2D Property Colour",
        GH_ParamAccess.item);
      for (int i = 1; i < pManager.ParamCount; i++) {
        pManager[i].Optional = true;
      }
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager) {
      pManager.AddGenericParameter("2D Property", "PA", "GSA 2D Property with changes",
        GH_ParamAccess.item);
      pManager.AddIntegerParameter("Prop2d Number", "ID", "2D Property Number",
        GH_ParamAccess.item);
      pManager.AddGenericParameter("Material", "Ma", "Get GSA Material", GH_ParamAccess.item);
      pManager.AddGenericParameter("Thickness", "Th", "Get Property Thickness",
        GH_ParamAccess.item);
      pManager.AddIntegerParameter("Axis", "Ax", "Get Axis: Global (0) or Topological (1)",
        GH_ParamAccess.item);
      pManager.AddTextParameter("Prop2d Name", "Na", "Name of 2D Proerty", GH_ParamAccess.item);
      pManager.AddColourParameter("Prop2d Colour", "Co", "2D Property Colour", GH_ParamAccess.item);
      pManager.AddGenericParameter("Type", "Ty", "2D Property Type", GH_ParamAccess.item);
    }

    protected override void SolveInstance(IGH_DataAccess da) {
      var prop = new GsaProperty2d();

      GsaProperty2dGoo prop2dGoo = null;
      if (da.GetData(0, ref prop2dGoo)) {
        prop = new GsaProperty2d(prop2dGoo.Value);
      }

      var ghId = new GH_Integer();
      if (da.GetData(1, ref ghId)) {
        if (GH_Convert.ToInt32(ghId, out int id, GH_Conversion.Both)) {
          prop.Id = id;
        }
      }

      GsaMaterialGoo materialGoo = null;
      if (da.GetData(2, ref materialGoo)) {
        prop.Material = materialGoo.Value;
      }

      if (Params.Input[3].SourceCount > 0) {
        prop.Thickness
          = (Length)Input.UnitNumber(this, da, 3, DefaultUnits.LengthUnitSection, true);
      }

      var ghAxis = new GH_Integer();
      if (da.GetData(4, ref ghAxis)) {
        if (GH_Convert.ToInt32(ghAxis, out int axis, GH_Conversion.Both)) {
          prop.ApiProp2d.AxisProperty = axis;
        }
      }

      var ghString = new GH_String();
      if (da.GetData(5, ref ghString)) {
        if (GH_Convert.ToString(ghString, out string name, GH_Conversion.Both)) {
          prop.ApiProp2d.Name = name;
        }
      }

      var ghColour = new GH_Colour();
      if (da.GetData(6, ref ghColour)) {
        if (GH_Convert.ToColor(ghColour, out Color col, GH_Conversion.Both)) {
          prop.ApiProp2d.Colour = col;
        }
      }

      int ax = (prop.ApiProp2d == null) ? 0 : prop.ApiProp2d.AxisProperty;
      string nm = (prop.ApiProp2d == null) ? "--" : prop.ApiProp2d.Name;
      ValueType colour = prop.ApiProp2d?.Colour;

      da.SetData(0, new GsaProperty2dGoo(prop));
      da.SetData(1, prop.Id);
      da.SetData(2, new GsaMaterialGoo(prop.Material));
      da.SetData(3,
        prop.ApiProp2d?.Description == string.Empty ? new GH_UnitNumber(Length.Zero) :
          new GH_UnitNumber(prop.Thickness));
      da.SetData(4, ax);
      da.SetData(5, nm);
      da.SetData(6, colour);

      string str = (prop.ApiProp2d == null) ? "--" : prop.ApiProp2d.Type.ToString();
      if (prop.ApiProp2d == null) {
        str = char.ToUpper(str[0]) + str.Substring(1).ToLower().Replace("_", " ");
      }

      da.SetData(7, str);
    }
  }
}
