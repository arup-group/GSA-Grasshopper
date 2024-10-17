using System;
using System.Drawing;
using System.Windows.Forms;

using GH_IO.Serialization;

using Grasshopper.Kernel;

using GsaGH.Helpers.GH;
using GsaGH.Parameters;
using GsaGH.Properties;

using OasysGH;
using OasysGH.Components;
using OasysGH.Parameters;
using OasysGH.Units;
using OasysGH.Units.Helpers;

using OasysUnits;
using OasysUnits.Units;

using LengthUnit = OasysUnits.Units.LengthUnit;

namespace GsaGH.Components {
  public class Get2dPropertyModifier : GH_OasysComponent {
    public override Guid ComponentGuid => new Guid("945d2e3b-4e5d-4a6e-a64b-c2447c0c0723");
    public override GH_Exposure Exposure => GH_Exposure.quinary | GH_Exposure.obscure;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    protected override Bitmap Icon => Resources.Get2dPropertyModifier;
    private LengthUnit _lengthUnit = DefaultUnits.LengthUnitSection;

    public Get2dPropertyModifier() : base("Get 2D Property Modifier", "GetP2M",
      "Get GSA 2D Property Modifier", CategoryName.Name(), SubCategoryName.Cat1()) {
      Hidden = true;
    }

    public override void AppendAdditionalMenuItems(ToolStripDropDown menu) {
      if (!(menu is ContextMenuStrip)) {
        return; // this method is also called when clicking EWR balloon
      }

      Menu_AppendSeparator(menu);

      var lengthUnitsMenu = new ToolStripMenuItem("Length") {
        Enabled = true,
      };
      foreach (string unit in UnitsHelper.GetFilteredAbbreviations(EngineeringUnits.Length)) {
        var toolStripMenuItem = new ToolStripMenuItem(unit, null, (s, e) => UpdateLength(unit)) {
          Checked = unit == Length.GetAbbreviation(_lengthUnit),
          Enabled = true,
        };
        lengthUnitsMenu.DropDownItems.Add(toolStripMenuItem);
      }

      var densityUnitsMenu = new ToolStripMenuItem("Density") {
        Enabled = true,
      };

      var unitsMenu = new ToolStripMenuItem("Select Units", Resources.ModelUnits);
      unitsMenu.DropDownItems.AddRange(new ToolStripItem[] {
        lengthUnitsMenu,
        densityUnitsMenu,
      });
      unitsMenu.ImageScaling = ToolStripItemImageScaling.SizeToFit;

      menu.Items.Add(unitsMenu);

      Menu_AppendSeparator(menu);
    }

    public override bool Read(GH_IReader reader) {
      _lengthUnit
        = (LengthUnit)UnitsHelper.Parse(typeof(LengthUnit), reader.GetString("LengthUnit"));
      return base.Read(reader);
    }

    public override bool Write(GH_IWriter writer) {
      writer.SetString("LengthUnit", _lengthUnit.ToString());
      return base.Write(writer);
    }

    protected override void BeforeSolveInstance() {
      UpdateMessage();
    }

    protected override void RegisterInputParams(GH_InputParamManager pManager) {
      pManager.AddParameter(new GsaProperty2dModifierParameter(), GsaProperty2dModifierGoo.Name,
        GsaProperty2dModifierGoo.NickName, GsaProperty2dModifierGoo.Description + " to get" +
        "information for." + GsaProperty2dModifierGoo.Name, GH_ParamAccess.item);
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager) {
      pManager.AddGenericParameter("In-plane Modifier", "Ip", "Effective in-plane stiffness",
        GH_ParamAccess.item);

      pManager.AddGenericParameter("Bending Modifier", "B", "Effective bending stiffness",
        GH_ParamAccess.item);

      pManager.AddGenericParameter("Shear Modifier", "S", "Effective shear stiffness",
        GH_ParamAccess.item);

      pManager.AddGenericParameter("Volume Modifier", "V", "Effective volume",
        GH_ParamAccess.item);

      pManager.AddGenericParameter("Additional Mass", "+kg", "Additional mass per unit area",
        GH_ParamAccess.item);
    }

    protected override void SolveInstance(IGH_DataAccess da) {
      var modifier = new GsaProperty2dModifier();

      GsaProperty2dModifierGoo modifierGoo = null;
      if (da.GetData(0, ref modifierGoo)) {
        modifier = modifierGoo.Value.Duplicate();
      }

      da.SetData(0, new GH_UnitNumber(modifier.InPlane));
      da.SetData(1, new GH_UnitNumber(modifier.Bending));
      da.SetData(2, new GH_UnitNumber(modifier.Shear));
      da.SetData(3, new GH_UnitNumber(modifier.Volume));
      da.SetData(4, new GH_UnitNumber(modifier.AdditionalMass.ToUnit(AreaDensityUnit.KilogramPerSquareMeter)));
    }

    private void Update() {
      UpdateMessage();
      (this as IGH_VariableParameterComponent).VariableParameterMaintenance();
      ExpireSolution(true);
    }

    private void UpdateLength(string unit) {
      _lengthUnit = (LengthUnit)UnitsHelper.Parse(typeof(LengthUnit), unit);
      Update();
    }

    private void UpdateMessage() {
      Message = Length.GetAbbreviation(_lengthUnit);
    }
  }
}
