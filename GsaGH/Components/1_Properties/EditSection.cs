using System;
using System.Drawing;
using System.Windows.Forms;

using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;

using GsaAPI;

using GsaGH.Helpers.GH;
using GsaGH.Parameters;
using GsaGH.Properties;

using OasysGH;
using OasysGH.Components;
using OasysGH.Helpers;
using OasysGH.Units;
using OasysGH.Units.Helpers;

using OasysUnits;

using LengthUnit = OasysUnits.Units.LengthUnit;

namespace GsaGH.Components {
  /// <summary>
  ///   Component to edit a Section and ouput the information
  /// </summary>
  public class EditSection : GH_OasysComponent, IGH_VariableParameterComponent {
    public override Guid ComponentGuid => new Guid("cb1f5d76-3b12-4c2a-8d41-c9d4699faaf9");
    public override GH_Exposure Exposure => GH_Exposure.quarternary;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    protected override Bitmap Icon => Resources.EditSection;
    private LengthUnit _lengthUnit = DefaultUnits.LengthUnitSection;

    public EditSection() : base("Edit Section", "EditPB", "Modify GSA Section",
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
          Enabled = true,
          Checked = unit == Length.GetAbbreviation(_lengthUnit),
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

    public virtual void VariableParameterMaintenance() {
      string unitAbbreviation = Length.GetAbbreviation(_lengthUnit);
      Params.Input[5].Name = $"Add. Offset Y [{unitAbbreviation}]";
      Params.Input[6].Name = $"Add. Offset Z [{unitAbbreviation}]";
      Params.Output[5].Name = $"Add. Offset Y [{unitAbbreviation}]";
      Params.Output[6].Name = $"Add. Offset Z [{unitAbbreviation}]";
    }

    protected override void RegisterInputParams(GH_InputParamManager pManager) {
      pManager.AddParameter(new GsaSectionParameter(), GsaSectionGoo.Name, GsaSectionGoo.NickName,
        GsaSectionGoo.Description + " to get or set information for. Leave blank to create a new "
        + GsaSectionGoo.Name, GH_ParamAccess.item);
      pManager.AddIntegerParameter("Section Number", "ID",
        "Set Section Number. If ID is set it will replace any existing 2D Property in the model",
        GH_ParamAccess.item);
      pManager.AddTextParameter("Section Profile", "Pf",
        "Profile name following GSA naming convention (eg 'STD I 1000 500 15 25')",
        GH_ParamAccess.item);
      pManager.AddParameter(new GsaMaterialParameter(), GsaMaterialGoo.Name,
        GsaMaterialGoo.NickName, "Set " + GsaMaterialGoo.Name, GH_ParamAccess.item);
      pManager.AddGenericParameter("Basic Offset", "BO",
        "Set Basic Offset Centroid = 0 (default), Top = 1, TopLeft = 2, TopRight = 3, Left = 4, Right = 5, Bottom = 6, BottomLeft = 7, BottomRight = 8", GH_ParamAccess.item);
      pManager.AddGenericParameter($"Add. Offset Y [{Length.GetAbbreviation(_lengthUnit)}]", "AOY",
        "Set Additional Offset Y", GH_ParamAccess.item);
      pManager.AddGenericParameter($"Add. Offset Z [{Length.GetAbbreviation(_lengthUnit)}]", "AOZ",
        "Set Additional Offset Z", GH_ParamAccess.item);
      pManager.AddParameter(new GsaSectionModifierParameter(), GsaSectionModifierGoo.Name,
        GsaSectionModifierGoo.NickName, "Set " + GsaSectionModifierGoo.Name, GH_ParamAccess.item);
      pManager.AddIntegerParameter("Section Pool", "Po", "Set Section pool", GH_ParamAccess.item);
      pManager.AddTextParameter("Section Name", "Na", "Set Section name", GH_ParamAccess.item);
      pManager.AddColourParameter("Section Colour", "Co", "Set Section colour",
        GH_ParamAccess.item);

      for (int i = 0; i < pManager.ParamCount; i++) {
        pManager[i].Optional = true;
      }
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager) {
      pManager.AddParameter(new GsaSectionParameter(), GsaSectionGoo.Name, GsaSectionGoo.NickName,
        GsaSectionGoo.Description + " with applied changes.", GH_ParamAccess.item);
      pManager.AddIntegerParameter("Section Number", "ID",
        "Original Section number (ID) if the Section ever belonged to a GSA Model",
        GH_ParamAccess.item);
      pManager.AddTextParameter("Section Profile", "Pf", "Profile description",
        GH_ParamAccess.item);
      pManager.AddParameter(new GsaMaterialParameter(), GsaMaterialGoo.Name,
        GsaMaterialGoo.NickName, "Get " + GsaMaterialGoo.Name, GH_ParamAccess.item);
      pManager.AddGenericParameter("Basic Offset", "BO",
        "Get Basic Offset", GH_ParamAccess.item);
      pManager.AddGenericParameter($"Add. Offset Y [{Length.GetAbbreviation(_lengthUnit)}]", "AOY",
        "Get Additional Offset Y", GH_ParamAccess.item);
      pManager.AddGenericParameter($"Add. Offset Z [{Length.GetAbbreviation(_lengthUnit)}]", "AOZ",
        "Get Additional Offset Z", GH_ParamAccess.item);
      pManager.AddParameter(new GsaSectionModifierParameter(), GsaSectionModifierGoo.Name,
        GsaSectionModifierGoo.NickName, "Get " + GsaSectionModifierGoo.Name, GH_ParamAccess.item);
      pManager.AddIntegerParameter("Section Pool", "Po", "Get Section pool", GH_ParamAccess.item);
      pManager.AddTextParameter("Section Name", "Na", "Get Section name", GH_ParamAccess.item);
      pManager.AddColourParameter("Section Colour", "Co", "Get Section colour",
        GH_ParamAccess.item);
    }

    protected override void SolveInstance(IGH_DataAccess da) {
      var section = new GsaSection();

      GsaSectionGoo sectionGoo = null;
      if (da.GetData(0, ref sectionGoo)) {
        section = new GsaSection(sectionGoo.Value);
      }

      int id = 0;
      if (da.GetData(1, ref id)) {
        section.Id = id;
      }

      string profile = string.Empty;
      if (da.GetData(2, ref profile)) {
        if (GsaSection.IsValidProfile(profile)) {
          section.ApiSection.Profile = profile;
        } else {
          this.AddRuntimeError("Invalid profile syntax: " + profile);
          return;
        }
      }

      GsaMaterialGoo materialGoo = null;
      if (da.GetData(3, ref materialGoo)) {
        section.Material = materialGoo.Value;
      }

      var ghBasicOffset = new GH_ObjectWrapper();
      if (da.GetData("Basic Offset", ref ghBasicOffset)) {
        try {
          if (GH_Convert.ToInt32(ghBasicOffset.Value, out int offset, GH_Conversion.Both)) {
            section.ApiSection.BasicOffset = (BasicOffset)offset;
          } else if (GH_Convert.ToString(ghBasicOffset, out string value, GH_Conversion.Both)) {
            section.ApiSection.BasicOffset = (BasicOffset)Enum.Parse(typeof(BasicOffset), value, ignoreCase: true);
          }
        } catch {
          this.AddRuntimeError("Unable to convert input " + ghBasicOffset.Value + " to a Basic Offset (Centroid = 0, Top = 1, TopLeft = 2, TopRight = 3, Left = 4, Right = 5, Bottom = 6, BottomLeft = 7, BottomRight = 8)");
          return;
        }
      }

      section.AdditionalOffsetY = (Length)Input.UnitNumber(this, da, 5, _lengthUnit, true);
      section.AdditionalOffsetZ = (Length)Input.UnitNumber(this, da, 6, _lengthUnit, true);

      GsaSectionModifierGoo modifierGoo = null;
      if (da.GetData(7, ref modifierGoo)) {
        section.Modifier = modifierGoo.Value;
      }

      int pool = 0;
      if (da.GetData(8, ref pool)) {
        section.ApiSection.Pool = pool;
      }

      string name = string.Empty;
      if (da.GetData(9, ref name)) {
        section.ApiSection.Name = name;
      }

      Color colour = Color.Empty;
      if (da.GetData(10, ref colour)) {
        section.ApiSection.Colour = colour;
      }

      string prof = (section.ApiSection == null) ? "--" : section.ApiSection.Profile;
      int poo = (section.ApiSection == null) ? 0 : section.ApiSection.Pool;
      string nm = (section.ApiSection == null) ? "--" : section.ApiSection.Name;

      da.SetData(0, new GsaSectionGoo(section));
      da.SetData(1, section.Id);
      da.SetData(2, prof);
      da.SetData(3, new GsaMaterialGoo(section.Material));
      da.SetData(4, section.ApiSection.BasicOffset);
      da.SetData(5, section.AdditionalOffsetY.ToUnit(_lengthUnit));
      da.SetData(6, section.AdditionalOffsetZ.ToUnit(_lengthUnit));
      da.SetData(7, new GsaSectionModifierGoo(section.Modifier));
      da.SetData(8, poo);
      da.SetData(9, nm);
      da.SetData(10, section.ApiSection.Colour);
    }

    internal void Update(string unit) {
      _lengthUnit = (LengthUnit)UnitsHelper.Parse(typeof(LengthUnit), unit);
      Message = unit;
      (this as IGH_VariableParameterComponent).VariableParameterMaintenance();
      ExpireSolution(true);
    }
  }
}
